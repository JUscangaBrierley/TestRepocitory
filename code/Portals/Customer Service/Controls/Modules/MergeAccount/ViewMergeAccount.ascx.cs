using System;
using System.Collections.Generic;
using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
//using Brierley.LWModules.MergeAccount.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.FrameWork.Common.Exceptions;


namespace Brierley.LWModules.MergeAccount
{
	public partial class ViewMergeAccount : ModuleControlBase
	{
		private const string _className = "ViewMergeAccount";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private MergeAccountConfig _config = null;
		private Member _member;

		protected long FromMemberIPCode
		{
			get
			{
				if (ViewState["FromMemberIPCode"] != null)
				{
					return (long)ViewState["FromMemberIPCode"];
				}
				return 0;
			}
			set
			{
				ViewState["FromMemberIPCode"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";

			lnkValidate.Click += new EventHandler(lnkValidate_Click);
			lnkMerge.Click += new EventHandler(lnkMerge_Click);

			pchSuccessMessage.Visible = false;

			try
			{
				_config = ConfigurationUtil.GetConfiguration<MergeAccountConfig>(ConfigurationKey);
				if (_config == null)
				{
					_logger.Error(_className, methodName, string.Format("Missing configuration for module {0}.", ConfigurationKey));
					this.Visible = false;
					return;
				}

				_member = PortalState.CurrentMember;
				if (_member == null)
				{
					this.Visible = false;
					return;
				}

				lnkValidate.ValidationGroup = reqLoyaltyId.ValidationGroup = ValidationGroup;

				if (!_IsUserControlPostBack)
				{
					lnkMerge.Visible = false;					

					if (_config.DisplayConfirmationDialogBox)
					{
						var confirm = Convert.ToString(GetLocalResourceObject("MergeConfirmation.Text"));
						if (!string.IsNullOrEmpty(confirm))
						{
							lnkMerge.Attributes.Add("onclick", string.Format("return confirm('{0}');", StringUtils.JavascriptFriendly(confirm)));
						}
					}
                }

                litTitle.Text = (GetLocalResourceObject(_config.ModuleTitle) ?? _config.ModuleTitle).ToString(); //_config.ModuleTitle;
				var card = _member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
				if (card != null && !string.IsNullOrWhiteSpace(card.LoyaltyIdNumber))
				{
					lblToMemberLoyaltyIDText.Text = card.LoyaltyIdNumber;
				}
				else
				{
					lnkValidate.Enabled = false;
					ShowNegative(GetLocalResourceObject("MergeFailedNoPrimaryVC.Text").ToString());
				}

				pchAdditionalNotes.Visible = _config.ShowAdditionalNotes;				
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
		}


		void lnkValidate_Click(object sender, EventArgs e)
		{
			const string methodName = "BtnValidate_Click";
			try
			{
				lnkMerge.Visible = false;
                pchMergeConfirmation.Visible = false;

				if (!Page.IsValid)
				{
					return;
				}
				
				string fromMemberLoyaltyId = txtFromMemberLoyaltyId.Text.Trim();

				if (lblFromMemberLabel.Text == fromMemberLoyaltyId)
				{
					ShowNegative(Convert.ToString(GetLocalResourceObject("LoyaltyIdIsCurrentMember")));
					return;
				}

				var fromMember = LoyaltyService.LoadMemberFromLoyaltyID(fromMemberLoyaltyId);
				if (fromMember == null)
				{
					ShowNegative(Convert.ToString(GetLocalResourceObject("LoyaltyIdDoesNotExist")));
					return;
				}

				FromMemberIPCode = fromMember.IpCode;

				if (fromMember.LoyaltyCards == null || fromMember.LoyaltyCards.Count == 0)
				{
					ShowNegative(Convert.ToString(GetLocalResourceObject("FromMemberNoVirtualCards")));
					return;
				}

				if (fromMember.IpCode == _member.IpCode)
				{
					ShowNegative(Convert.ToString(GetLocalResourceObject("LoyaltyIdIsCurrentMember")));
					return;
				}

				pchMergeConfirmation.Visible = true;
				lnkMerge.Visible = true;

				DisplayMergeConfirmation(fromMember);

			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
		}


		void lnkMerge_Click(object sender, EventArgs e)
		{
			const string methodName = "lnkMerge_Click";
			try
			{
				Member fromMember = LoyaltyService.LoadMemberFromIPCode(FromMemberIPCode);

				// IMergeAccount interceptor before merge call
                IMergeMemberInterceptor iMerge = null;
				if (!string.IsNullOrEmpty(_config.MergeInterceptorAssemblyName) && !string.IsNullOrEmpty(_config.MergeInterceptorClassName))
				{
                    iMerge = ClassLoaderUtil.CreateInstance(_config.MergeInterceptorAssemblyName, _config.MergeInterceptorClassName) as IMergeMemberInterceptor;
					if (iMerge == null)
					{
                        throw new Exception(string.Format(GetLocalResourceObject("CreateInstanceBeforeFailed.Text").ToString(), _config.MergeInterceptorAssemblyName, _config.MergeInterceptorClassName));
					}
					try
					{
						iMerge.BeforeMerge(fromMember, _member);
					}
					catch (NotImplementedException)
					{
						// not implemented.
					}
					catch (LWValidationException ex)
					{
						ShowNegative(ex.Message);
						return;
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Exception thrown by merge member interceptor.", ex);
						throw;
					}
				}

                Member mergedMember = null;
                try { mergedMember = MergeAccount(); }
                catch (LWDataServiceException ex)
                {
                    ShowNegative(ex.Message);
                    return;
                }
                catch (Exception ex)
                {
                    throw;
                }

				// IMergeAccount interceptor after merge call
                if (iMerge != null)
                {
                    try
                    {
                        iMerge.AfterMerge(fromMember, _member);
                    }
                    catch (NotImplementedException)
                    {
                        // not implemented.
                    }
					catch (LWValidationException ex)
					{
						ShowNegative(ex.Message);
						return;
					}
                    catch (Exception ex)
                    {
                        _logger.Error(_className, methodName, "Exception thrown by merge member interceptor.", ex);
                        throw;
                    }
                }
				
                if (_config.OnSuccessEventId != 0)
                {
                    LWEvent lwevent = LoyaltyService.GetLWEvent(_config.OnSuccessEventId);
                    if (lwevent != null)
                    {
                        _logger.Trace(_className, methodName, "Executing rules for event " + lwevent.Name);
                        ContextObject ctx = new ContextObject() { Owner = mergedMember };
                        LoyaltyService.ExecuteEventRules(ctx, lwevent.Name, RuleInvocationType.Manual);
                    }
                }

                AddNotes(fromMember, _member);
				txtFromMemberLoyaltyId.Text = string.Empty;
				pchMergeConfirmation.Visible = false;
				lnkMerge.Visible = false;
				pchSuccessMessage.Visible = true;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
		}


		/// <summary>
		/// Add notes after merge
		/// </summary>
		/// <param name="notes">Note text to merge</param>
		private void AddNotes(Member from, Member to)
		{
			const string methodName = "AddNotes";
			try
			{
                // Add the note to "To" account.
                string txt = string.Format(GetLocalResourceObject("MergeToSuccess.Text").ToString(), txtFromMemberLoyaltyId.Text.Trim(), lblToMemberLoyaltyIDText.Text);
                CSNote note = new CSNote() { Note = txt, MemberId = to.IpCode, CreatedBy = PortalState.GetLoggedInCSAgent().Id};
                CSService.CreateNote(note);
                if (_config.ShowAdditionalNotes && !string.IsNullOrWhiteSpace(txtNotes.Text))
                {
                    note = new CSNote() { Note = txtNotes.Text, MemberId = to.IpCode, CreatedBy = PortalState.GetLoggedInCSAgent().Id };
                    CSService.CreateNote(note);
                }

                // Add the note to "from" account
                txt = string.Format(GetLocalResourceObject("MergeToSuccess.Text").ToString(), txtFromMemberLoyaltyId.Text.Trim(), lblToMemberLoyaltyIDText.Text);
                note = new CSNote() { Note = txt, MemberId = from.IpCode, CreatedBy = PortalState.GetLoggedInCSAgent().Id };
                CSService.CreateNote(note);				
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
		}


		private void DisplayMergeConfirmation(Member fromMember)
		{
			if (fromMember == null)
			{
				throw new ArgumentNullException("fromMember");
			}

			lblToLoyaltyId.Text = lblToMemberLoyaltyIDText.Text;
			lblToName.Text = string.Format("{0} {1}", _member.FirstName, _member.LastName);
			lblToEmail.Text = _member.PrimaryEmailAddress;
            lblToStatus.Text = _member.MemberStatus.ToString();

			lblFromLoyaltyId.Text = fromMember.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard).LoyaltyIdNumber;
			lblFromName.Text = string.Format("{0} {1}", fromMember.FirstName, fromMember.LastName);
			lblFromEmail.Text = fromMember.PrimaryEmailAddress;
            lblFromStatus.Text = fromMember.MemberStatus.ToString();

			AttributeSetMetaData meta = LoyaltyService.GetAttributeSetMetaData("MemberDetails");
			if (meta != null)
			{
				lblToAddress.Text = GetAddress(_member);
				lblFromAddress.Text = GetAddress(fromMember);
			}

			pchMergeConfirmation.Visible = true;
		}

		private string GetAddress(Member member)
		{
			string ret = string.Empty;
			IList<IClientDataObject> details = member.GetChildAttributeSets("MemberDetails");
			if (details != null && details.Count > 0)
			{
				IClientDataObject detail = details[0];
				ret = Convert.ToString(detail.GetAttributeValue("AddressLineOne"));

				string addressTwo = Convert.ToString(detail.GetAttributeValue("AddressLineTwo"));
				if (!string.IsNullOrEmpty(addressTwo))
				{
					ret += "<br />" + addressTwo;
				}

				string addressThree = Convert.ToString(detail.GetAttributeValue("AddressLineThree"));
				if (!string.IsNullOrEmpty(addressThree))
				{
					ret += "<br />" + addressThree;
				}

				string addressFour = Convert.ToString(detail.GetAttributeValue("AddressLineFour"));
				if (!string.IsNullOrEmpty(addressFour))
				{
					ret += "<br />" + addressFour;
				}

				if (!string.IsNullOrEmpty(ret))
				{
					ret += "<br/>";
				}

				string cityStateZip = Convert.ToString(detail.GetAttributeValue("City"));
				if (!string.IsNullOrEmpty(cityStateZip))
				{
					cityStateZip += ", ";
				}
				cityStateZip += Convert.ToString(detail.GetAttributeValue("StateOrProvince"));
				cityStateZip += " " + Convert.ToString(detail.GetAttributeValue("ZipOrPostalCode"));

				ret += cityStateZip;
			}
			return ret;
		}

		/// <summary>
		/// Get loyalty member by id
		/// </summary>
		/// <param name="loyaltyId">loyalty id to get member</param>
		/// <returns>Loyalty member pulled by loyalty id</returns>
		private Member GetLoyaltyMemberById(string loyaltyId)
		{
			const string methodName = "GetLoyaltyMemberById";
			Member member = null;
			try
			{
				member = LoyaltyService.LoadMemberFromLoyaltyID(loyaltyId);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}


			return member;
		}


		private bool ValidateMember()
		{
			/*
			if (string.IsNullOrEmpty(txtFromMemberLoyaltyId.Text.Trim()))
			{
				lblFalureMessage.Visible = true;
				lblFalureMessage.Text = _config.FromMemberLoyaltyIDRequiredErrorMessage;
				return false;
			}

			if (null == GetLoyaltyMemberById(txtFromMemberLoyaltyId.Text.Trim()))
			{
				lblFalureMessage.Visible = true;
				lblFalureMessage.Text = _config.FromMemberLoyaltyIDNotExistsErrorMessage;
				return false;
			}

			if (GetCachedMemberPrimaryLoyaltyID() == txtFromMemberLoyaltyId.Text.Trim())
			{
				lblFalureMessage.Visible = true;
				lblFalureMessage.Text = _config.FromMemberLoyaltyIDIsCurrentErrorMessage;
				return false;
			}

			Member member = GetCachedLoyaltyMember();
			if (member == null)
			{
				lblFalureMessage.Visible = true;
				lblFalureMessage.Text = "To Member not found";
				return false;
			}
			if (member.LoyaltyCards == null || member.LoyaltyCards.Count == 0)
			{
				lblFalureMessage.Visible = true;
				lblFalureMessage.Text = "To Member does not have virtual card";
				return false;
			}

			Member fromMember = _service.LoadMemberFromLoyaltyID(txtFromMemberLoyaltyId.Text.Trim());
			if (fromMember == null)
			{
				lblFalureMessage.Visible = true;
				lblFalureMessage.Text = "From Member not found";
				return false;
			}
			if (fromMember.LoyaltyCards == null || fromMember.LoyaltyCards.Count == 0)
			{
				lblFalureMessage.Visible = true;
				lblFalureMessage.Text = "From Member does not have virtual card";
				return false;
			}

			foreach (VirtualCard vc in member.LoyaltyCards)
			{
				lblFalureMessage.Visible = true;
				if (vc.LoyaltyIdNumber == txtFromMemberLoyaltyId.Text)
				{
					lblFalureMessage.Text = _config.FromMemberLoyaltyIDAlreadyExistsinToMemberErrorMessage;
					return false;
				}
			}


			lblFalureMessage.Visible = false;
			 * */
			return true;
		}

		/// <summary>
		/// Merge member account
		/// </summary>
		private Member MergeAccount()
		{
			const string methodName = "MergeAccount";
            try
            {
                PointType pointType = LoyaltyService.GetPointType(_config.PointType);
                PointEvent pointEvent = LoyaltyService.GetPointEvent(_config.PointEvent);

                Member mergedMember = LoyaltyService.MergeMember(GetLoyaltyMemberById(txtFromMemberLoyaltyId.Text.Trim()),
                    PortalState.CurrentMember,
                    pointEvent,
                    pointType,
                    DateTime.Now.AddYears(1),
                    _config.MergeOptions);
                return mergedMember;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, ex.Message, ex);
                throw;
            }
		}

	}
}
