using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.LWModules.LoyaltyCardsGrid.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Interceptors;
using Brierley.WebFrameWork.Controls.List;
//using Brierley.LWModules.LoyaltyCards.Components.List;

namespace Brierley.LWModules.LoyaltyCardsGrid
{
	public partial class ViewLoyaltyCardsGrid : ModuleControlBase, IIpcEventHandler
	{
		private const string _className = "ViewLoyaltyCardsGrid";
		private const string _modulePath = "~/Controls/Modules/LoyaltyCardsGrid/ViewLoyaltyCardsGrid.ascx";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private LoyaltyCardsGridConfig _config = null;
		private IDynamicGridProvider _gridProvider = null;
		private AspDynamicGrid _grid = null;
		private string _validationGroup = string.Empty;

		protected long SelectedCardId
		{
			get
			{
				if (ViewState["SelectedCardId"] != null)
				{
					return (long)ViewState["SelectedCardId"];
				}
				return 0;
			}
			set
			{
				ViewState["SelectedCardId"] = value;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			const string methodName = "OnInit";
			try
			{
				if (PortalState.CurrentMember == null)
				{
					this.Visible = false;
					return;
				}

				_config = ConfigurationUtil.GetConfiguration<LoyaltyCardsGridConfig>(ConfigurationKey);

				if (_config == null)
				{
					_logger.Error(_className, methodName, string.Format("Missing configuration for module {0}.", ConfigurationKey));
					this.Visible = false;
					return;
				}

				_validationGroup = PortalModules.LoyaltyCardsGrid.ToString() + base.ConfigurationKey.ConfigName;
				vldLoyaltyIdExists.ValidationGroup = _validationGroup;
				reqLoyaltyId.ValidationGroup = _validationGroup;
				reqMemberId.ValidationGroup = _validationGroup;
				lnkNewOk.ValidationGroup = _validationGroup;
				lnkPrimaryOk.ValidationGroup = _validationGroup;
				lnkCancelOk.ValidationGroup = _validationGroup;
				lnkReplOk.ValidationGroup = _validationGroup;
				lnkRenewOk.ValidationGroup = _validationGroup;
				lnkTransferOk.ValidationGroup = _validationGroup;

				if (_config.AllowUpdates && _config.EnableAddCard)
				{
					lnkAddNew.Visible = true;
				}
				else
				{
					lnkAddNew.Visible = false;
				}


				//Grid Details
				if (string.IsNullOrEmpty(_config.ProviderClassName) || string.IsNullOrEmpty(_config.ProviderAssemblyName))
				{
					_logger.Trace(_className, methodName, "No provider has been set. Using default");
					_gridProvider = new DefaultGridProvider(_config) { ParentControl = _modulePath };
				}
				else
				{
					_gridProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
					_gridProvider.ParentControl = _modulePath;
				}
				if (_gridProvider is AspGridProviderBase)
				{
					((AspGridProviderBase)_gridProvider).ValidationGroup = _validationGroup;
				}
				_gridProvider.FilteringEnabled = _config.EnableGridFiltering;

				_grid = new AspDynamicGrid() { AutoRebind = false, CreateTopPanel = false };
				_grid.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
				_grid.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
				_grid.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
				_grid.Provider = _gridProvider;
				//_grid.SetSearchParm("Configuration", _config);

				_grid.GridActionClicked += new AspDynamicGrid.GridActionClickedHandler(GridActionClickedHandler);

				phLoyaltyCards.Controls.Add(_grid);

				_grid.Rebind();


				lblNoResults.Text = StringUtils.FriendlyString(ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey), "No records to display.");
				h2Title.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.TitleResourceKey);

				_logger.Debug(_className, methodName, "_config = " + ConfigurationKey);

				if (!IsPostBack)
				{
					ddlTargetMemberId.Items.Add(new ListItem() { Text = "LoyaltyId", Value = "1" });
					ddlTargetMemberId.Items.Add(new ListItem() { Text = "Email Address", Value = "2" });
					ddlTargetMemberId.Items.Add(new ListItem() { Text = "Alternate Id", Value = "3" });
					ddlTargetMemberId.Items.Add(new ListItem() { Text = "Username", Value = "4" });
					ddlTargetMemberId.Items.Add(new ListItem() { Text = "IpCode", Value = "5" });

					dtEffective.MinDate = dtReplEffectiveDate.MinDate = DateTimeUtil.MinValue;
					dtEffective.MaxDate = dtReplEffectiveDate.MaxDate = DateTimeUtil.MaxValue;

					string validationGroup = "ViewLoyaltyCards" + ConfigurationKey.ConfigName;
					lnkCancelOk.ValidationGroup = validationGroup;
					lnkPrimaryOk.ValidationGroup = validationGroup;
					lnkReplOk.ValidationGroup = validationGroup;
					reqMemberId.ValidationGroup = validationGroup;

					if (!string.IsNullOrEmpty(_config.CardTypeAttributeSetName))
					{
						pchCardType.Visible = true;

						var set = LoyaltyService.GetAttributeSetObjects(null, _config.CardTypeAttributeSetName, null, null, false);
						List<ListItem> vals = new List<ListItem>();
						foreach (var row in set)
						{
							var li = new ListItem();
							li.Text = row.GetAttributeValue(_config.CardLabelAttributeName).ToString();
							li.Value = row.GetAttributeValue(_config.CardTypeAttributeName).ToString();
							ddlCardType.Items.Add(li);
						}
					}
					else
					{
						pchCardType.Visible = false;
					}


				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
		}



		/// <summary>
		/// Page load event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			lnkPrimaryCancel.Click += delegate { CardOperationViews.Visible = false; };
			lnkCancelCancel.Click += delegate { CardOperationViews.Visible = false; };
			lnkReplCancel.Click += delegate { CardOperationViews.Visible = false; };
			lnkTransferCancel.Click += delegate { CardOperationViews.Visible = false; };
			lnkNewCancel.Click += delegate { CardOperationViews.Visible = false; };
			lnkRenewCancel.Click += delegate { CardOperationViews.Visible = false; };
			lnkPassbookCancel.Click += delegate { CardOperationViews.Visible = false; };

			lnkPrimaryOk.Click += new EventHandler(lnkPrimaryOk_Click);
			lnkCancelOk.Click += new EventHandler(lnkCancelOk_Click);
			lnkReplOk.Click += new EventHandler(lnkReplOk_Click);
			lnkTransferOk.Click += new EventHandler(lnkTransferOk_Click);
			lnkNewOk.Click += new EventHandler(lnkNewOk_Click);
			lnkRenewOk.Click += new EventHandler(lnkRenewOk_Click);
			lnkPassbookOk.Click += new EventHandler(lnkPassbookOk_Click);

			lnkAddNew.Click += new EventHandler(lnkAddNew_Click);

			vldLoyaltyIdExists.ServerValidate += new ServerValidateEventHandler(vldLoyaltyIdExists_ServerValidate);

			//if (!IsPostBack && _list != null)
			//{
			//    _list.Rebind();
			//}
		}

		void vldLoyaltyIdExists_ServerValidate(object source, ServerValidateEventArgs args)
		{
			try
			{
				var member = LoyaltyService.LoadMemberFromLoyaltyID(txtLoyaltyId.Text.Trim());
				if (member != null)
				{
					args.IsValid = false;
				}
			}
			catch { }
		}

		private Member CreateVirtualCard(Member member, string loyaltyIdNumber, int cardType, DateTime? expirationDate)
		{
            VirtualCard vc = member.CreateNewVirtualCard(expirationDate);
			vc.IsPrimary = false;

			vc.LoyaltyIdNumber = loyaltyIdNumber;
			vc.CardType = cardType;

			LoyaltyService.SaveMember(member);
			return member;
		}

		void lnkNewOk_Click(object sender, EventArgs e)
		{
			if (!Page.IsValid)
			{
				return;
			}
			const string methodName = "lnkNewOk_Click";
			try
			{
				var member = PortalState.CurrentMember;
				string loyaltyIdNumber = txtLoyaltyId.Text.Trim();
				int cardType = 0;
				DateTime? expirationDate = null;

				if (_config.DefaultCardType.HasValue)
				{
					cardType = _config.DefaultCardType.Value;
				}
				else
				{
					cardType = StringUtils.FriendlyInt32(ddlCardType.SelectedValue, 0);
				}
				expirationDate = dtExpirationDate.SelectedDate;

				ILoyaltyCardsInterceptor interceptor = null;

				// Check to make sure that the two loyalty cards match
				if (_config.ConfirmLoyaltyId)
				{
					string loyaltyIdNumber2 = txtLoyaltyIdConfirm.Text.Trim();
					if (loyaltyIdNumber != loyaltyIdNumber2)
					{
						string errMsg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "LoyaltyCardsDoNotMatch.Text", "Loyalty cards \"{0}\" and \"{1}\" do not match."), loyaltyIdNumber, loyaltyIdNumber2);
						_logger.Error(_className, methodName, errMsg);
						AddInvalidField(errMsg, txtLoyaltyId);
						return;
					}
				}

				if (!string.IsNullOrEmpty(_config.VCCardInterceptorAssembly) && !string.IsNullOrEmpty(_config.VCCardInterceptorClass))
				{
					interceptor = (ILoyaltyCardsInterceptor)ClassLoaderUtil.CreateInstance(_config.VCCardInterceptorAssembly, _config.VCCardInterceptorClass);
				}

				if (interceptor != null)
				{
					// validate the card
					try
					{
						interceptor.Validate(member, loyaltyIdNumber, cardType, expirationDate);
					}
					catch (NotImplementedException)
					{
						// validation operation has not been implemented.                        
					}
					catch (LWValidationException ex)
					{
						ShowNegative(ex.Message);
					}

					// create the card
					try
					{
						member = interceptor.Create(member, loyaltyIdNumber, cardType, expirationDate);
					}
					catch (NotImplementedException)
					{
						member = CreateVirtualCard(member, loyaltyIdNumber, cardType, expirationDate);
					}
					catch (LWValidationException ex)
					{
						ShowNegative(ex.Message);
						return;
					}
				}
				else
				{
					member = CreateVirtualCard(member, loyaltyIdNumber, cardType, expirationDate);
				}

				PortalState.CurrentMember = member;
				IpcManager.PublishEvent("MemberUpdated", ConfigurationKey, member);
				_grid.Rebind();
				CardOperationViews.Visible = false;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}


		protected override void OnPreRender(EventArgs e)
		{
			if (_gridProvider != null)
			{
				lblNoResults.Visible = _gridProvider != null && _gridProvider.GetNumberOfRows() < 1;
			}
			base.OnPreRender(e);
		}


		#region Command Handlers

		void lnkAddNew_Click(object sender, EventArgs e)
		{
			SelectedCardId = -1;
			CardOperationViews.Visible = true;
			if (_config.ConfirmLoyaltyId)
			{
				lblLoyaltyIdConfirm.Visible = true;
				txtLoyaltyIdConfirm.Visible = true;
			}
			else
			{
				lblLoyaltyIdConfirm.Visible = false;
				txtLoyaltyIdConfirm.Visible = false;
			}
			CardOperationViews.SetActiveView(AddCardView);
			txtLoyaltyId.Text = string.Empty;
			if (ddlCardType.Items.Count > 0)
			{
				ddlCardType.SelectedIndex = 0;
			}
		}

		void _list_ListActionClicked(object sender, ListActionClickedArg e)
		{
			ActionClicked(e.Key, e.Command);
		}

		void GridActionClickedHandler(object sender, GridActionClickedArg e)
		{
			ActionClicked(e.Key, e.CommandName);
		}


		void ActionClicked(object key, string commandName)
		{

			if (key == null || commandName == "AddNew")
			{
				return;
			}
			SelectedCardId = long.Parse(key.ToString());
			VirtualCard vc = PortalState.CurrentMember.GetLoyaltyCard(SelectedCardId);
			CardOperationViews.Visible = true;
			switch (commandName)
			{
				case "Primary":
					string msg = string.Format(GetLocalResourceObject("PrimaryCardConfirmation").ToString(), vc.LoyaltyIdNumber);
					CardOperationViews.SetActiveView(PrimaryConfirmationView);
					lblConfirmationMessage.Text = msg;
					break;

				case "Cancel Card":
					lblCancelCardId.Text = vc.LoyaltyIdNumber;
					dtEffective.SelectedDate = DateTime.Now;
					lblDeactivateMember.Visible = _config.ShowDeactivateOption;
					chkDeactivateMember.Visible = _config.ShowDeactivateOption;
					CardOperationViews.SetActiveView(CardCancellationView);
					break;

				case "Replace":
					lblReplaceCardId.Text = vc.LoyaltyIdNumber;
					dtReplEffectiveDate.SelectedDate = DateTime.Now;
					foreach (VirtualCard v in PortalState.CurrentMember.LoyaltyCards)
					{
						if (vc.VcKey != v.VcKey && v.CardType == vc.CardType)
						{
							ListItem i = new ListItem() { Text = v.LoyaltyIdNumber, Value = v.LoyaltyIdNumber };
							ddlReplacementCards.Items.Add(i);
						}
					}
					CardOperationViews.SetActiveView(ReplaceCardView);
					break;
				case "Renew":
					lblCardToRenewId.Text = vc.LoyaltyIdNumber;
					lblRenewCardStatusLabel.Text = vc.Status.ToString();
					lblRenewCardCurrentExpiration.Text = vc.ExpirationDate != null ? vc.ExpirationDate.Value.ToShortDateString() : string.Empty;
					dtNewExpirationDate.SelectedDate = DateTime.Now;
					CardOperationViews.SetActiveView(RenewCardView);
					break;
				case "Transfer":
					lblTransferCard.Text = vc.LoyaltyIdNumber;
					CardOperationViews.SetActiveView(TransferCardView);
					break;
				case "Passbook":
					lblCardToSendId.Text = vc.LoyaltyIdNumber;
					lblMemberEmail.Text = vc.Member.PrimaryEmailAddress;
					CardOperationViews.SetActiveView(PassbookCardView);
					break;
			}
		}

		protected void lnkPrimaryOk_Click(object sender, EventArgs e)
		{
			string methodName = "lnkPrimaryOk_Click";
			var member = PortalState.CurrentMember;
			VirtualCard vc = PortalState.CurrentMember.GetLoyaltyCard(SelectedCardId);
			member.MarkVirtualCardAsPrimary(vc);
			LoyaltyService.SaveMember(member);
			_logger.Trace(_className, methodName,
				string.Format("Loyalty card with id {0} has been made primary for member {1}", vc.LoyaltyIdNumber, PortalState.CurrentMember.IpCode));
			CardOperationViews.Visible = false;
			_grid.Rebind();
		}


		protected void lnkCancelOk_Click(object sender, EventArgs e)
		{
			string methodName = "lnkCancelOk_Click";
			VirtualCard vc = PortalState.CurrentMember.GetLoyaltyCard(SelectedCardId);
			DateTime effectiveDate = dtEffective.SelectedDate != null ? (DateTime)dtEffective.SelectedDate : DateTime.Now;
			if (vc.Status == VirtualCardStatusType.Active)
			{
				LoyaltyService.CancelVirtualCard(PortalState.CurrentMember, vc.LoyaltyIdNumber, txtCancelReason.Text, effectiveDate, chkDeactivateMember.Checked, true, true);
				_logger.Trace(_className, methodName,
					string.Format("Loyalty card with id {0} for member {1} has been cancelled.", vc.LoyaltyIdNumber, PortalState.CurrentMember.IpCode));
			}
			else
			{
				_logger.Error(_className, methodName,
					string.Format("Loyalty card with id {0} for member {1} is not in active status.", vc.LoyaltyIdNumber, PortalState.CurrentMember.IpCode));
			}
			CardOperationViews.Visible = false;
			_grid.Rebind();
		}


		protected void lnkReplOk_Click(object sender, EventArgs e)
		{
			string methodName = "lnkReplOk_Click";
			var member = PortalState.CurrentMember;
			VirtualCard vc = member.GetLoyaltyCard(SelectedCardId);
			string replacementCardId = ddlReplacementCards.SelectedValue;
			DateTime effectiveDate = dtReplEffectiveDate != null ? (DateTime)dtReplEffectiveDate.SelectedDate : DateTime.Now;
			bool transferPoints = chkReplacementTransferPoints.Checked;
			string reason = txtReplReason.Text;

			if (member.MemberStatus == MemberStatusEnum.Active)
			{
				long id = 0;
				if (!string.IsNullOrEmpty(_config.VCCardInterceptorAssembly) && !string.IsNullOrEmpty(_config.VCCardInterceptorClass))
				{
					ILoyaltyCardsInterceptor interceptor = (ILoyaltyCardsInterceptor)ClassLoaderUtil.CreateInstance(_config.VCCardInterceptorAssembly, _config.VCCardInterceptorClass);
					if (interceptor == null)
					{
						throw new Exception("Unable to create interceptor assembly.");
					}
					/*
					 * If the replacement card exists then make sure that the two cards are of the same type.
					 * */
					VirtualCard toCard = member.GetLoyaltyCard(replacementCardId);
					if (toCard != null && toCard.CardType != vc.CardType)
					{
						AddInvalidField(ResourceUtils.GetLocalWebResource(_modulePath, "DifferentCardTypeError.Text", "You cannot replace a card with an existing card of a different type."), ddlReplacementCards);
						return;
					}
					try
					{
						id = interceptor.ReplaceCard(member, SelectedCardId, replacementCardId, effectiveDate, transferPoints, reason);
					}
					catch (NotFiniteNumberException)
					{
						id = LoyaltyService.ReplaceVirtualCard(member, SelectedCardId, replacementCardId,
						effectiveDate, transferPoints, reason);
					}
					catch (LWValidationException ex)
					{
						ShowNegative(ex.Message);
						return;
					}
				}
				else
				{
					id = LoyaltyService.ReplaceVirtualCard(member, SelectedCardId, replacementCardId,
						effectiveDate, transferPoints, reason);
				}
				_logger.Trace(_className, methodName,
						string.Format("Loyalty card with id {0} for member {1} has been transferred to {2}.", vc.LoyaltyIdNumber, member.IpCode, replacementCardId));
			}
			else
			{
				_logger.Error(_className, methodName,
					string.Format("Loyalty card with id {0} for member {1} is not in active status.", vc.LoyaltyIdNumber, member.IpCode));
			}
			CardOperationViews.Visible = false;
			_grid.Rebind();
		}



		protected void lnkTransferOk_Click(object sender, EventArgs e)
		{
			string methodName = "lnkTransferOk_Click";

			Member memberFrom = PortalState.CurrentMember;
			Member memberTo = null;

			bool valid = true;

			int option = int.Parse(ddlTargetMemberId.SelectedValue);
			string idValue = txtMemberId.Text;
			if (string.IsNullOrEmpty(idValue))
			{
				AddInvalidField(ResourceUtils.GetLocalWebResource(_modulePath, "reqTargetMemberId.ErrorMessage", "Please provide value for member identity."), txtMemberId);
				valid = false;
			}
			else
			{
				switch (option)
				{
					case 1: // loyalty id
						memberTo = LoyaltyService.LoadMemberFromLoyaltyID(idValue);
						break;
					case 2: // email address
						memberTo = LoyaltyService.LoadMemberFromEmailAddress(idValue);
						break;
					case 3: // alternate id
						memberTo = LoyaltyService.LoadMemberFromAlternateID(idValue);
						break;
					case 4: // username
						memberTo = LoyaltyService.LoadMemberFromUserName(idValue);
						break;
					case 5: // IpCode
						memberTo = LoyaltyService.LoadMemberFromIPCode(long.Parse(idValue));
						break;
				}
			}

			if (memberTo == null)
			{
				ShowNegative(string.Format(GetLocalResourceObject("MemberNotFound").ToString(), idValue));
				valid = false;
			}
			else if (memberFrom.MemberStatus != MemberStatusEnum.Active)
			{
				ShowNegative(GetLocalResourceObject("CurrentMemberNotActive").ToString());
				valid = false;
			}
			else if (memberTo.MemberStatus != MemberStatusEnum.Active)
			{
				ShowNegative(GetLocalResourceObject("TargetMemberNotActive").ToString());
				valid = false;
			}
			else if (memberTo.IpCode == memberFrom.IpCode)
			{
				ShowNegative(GetLocalResourceObject("MembersAreSame").ToString());
			}
			else
			{
				VirtualCard vc = PortalState.CurrentMember.GetLoyaltyCard(SelectedCardId);
				if (!string.IsNullOrEmpty(_config.VCCardInterceptorAssembly) && !string.IsNullOrEmpty(_config.VCCardInterceptorClass))
				{
					ILoyaltyCardsInterceptor interceptor = (ILoyaltyCardsInterceptor)ClassLoaderUtil.CreateInstance(_config.VCCardInterceptorAssembly, _config.VCCardInterceptorClass);
					if (interceptor == null)
					{
						throw new Exception("Unable to create interceptor assembly.");
					}
					try
					{
						interceptor.TransferCard(vc, memberTo, chkMakeCardPrimary.Checked, txtTsfrDeactivateMember.Checked);
					}
					catch (NotFiniteNumberException)
					{
						LoyaltyService.TransferVirtualCard(vc, memberTo, chkMakeCardPrimary.Checked, txtTsfrDeactivateMember.Checked);
					}
					catch (LWValidationException ex)
					{
						ShowNegative(ex.Message);
						return;
					}
				}
				else
				{
					LoyaltyService.TransferVirtualCard(vc, memberTo, chkMakeCardPrimary.Checked, txtTsfrDeactivateMember.Checked);
				}
				_logger.Error(_className, methodName,
						string.Format("Loyalty card with id {0} for member {1} has been transferred to IpCode {2}.", vc.LoyaltyIdNumber, PortalState.CurrentMember.IpCode, memberTo.IpCode));
			}

			if (valid)
			{
				CardOperationViews.Visible = false;
				_grid.Rebind();
			}
		}

		protected void lnkRenewOk_Click(object sender, EventArgs e)
		{
			string methodName = "lnkRenewOk_Click";

			Member member = PortalState.CurrentMember;
			VirtualCard vc = member.GetLoyaltyCard(SelectedCardId);

			try
			{
				if (dtNewExpirationDate.SelectedDate != null)
				{
					_logger.Trace(_className, methodName, "Changing expiration date on " + vc.LoyaltyIdNumber);
					vc.ExpirationDate = dtNewExpirationDate.SelectedDate.Value;
					LoyaltyService.SaveMember(member);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception", ex);
				return;
			}
			CardOperationViews.Visible = false;
			_grid.Rebind();
		}

		protected void lnkPassbookOk_Click(object sender, EventArgs e)
		{
			string methodName = "lnkPassbookOk_Click";

			var member = PortalState.CurrentMember;
			VirtualCard vc = member.GetLoyaltyCard(SelectedCardId);
			_logger.Debug(_className, methodName, string.Format("Sending Loyalty Card {0} to Passbook.", vc.LoyaltyIdNumber));

			MTouch mt = LoyaltyService.CreateMTouch(MTouchType.LoyaltyCard, member.IpCode.ToString(), vc.LoyaltyIdNumber);

			// now send triggered email with the mtouch in the link
			if (!string.IsNullOrWhiteSpace(_config.PassbookSendEmail))
			{
				EmailDocument emailDoc = EmailService.GetEmail(_config.PassbookSendEmail);
				using (ITriggeredEmail email =  TriggeredEmailFactory.Create(emailDoc.Id))
				{
					Dictionary<string, string> fields = new Dictionary<string, string>();
					fields.Add("memberid", member.IpCode.ToString());
					fields.Add("mtouch", mt.MTouchValue);
					fields.Add("iso639Code", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
					email.SendAsync(member, fields).Wait();
					_logger.Debug(_className, methodName, string.Format("Email {0} ({1}) sent", _config.PassbookSendEmail, emailDoc.Id));
				}
			}

			CardOperationViews.Visible = false;
		}
		#endregion


		protected override bool ControlRequiresTelerikSkins()
		{
			return false;
		}


		private DynamicGridColumnSpec GetKeyColumn()
		{
			if (_grid.Columns != null)
			{
				foreach (DynamicGridColumnSpec column in _grid.Columns)
				{
					if (column.IsKey)
					{
						return column;
					}
				}
			}
			return null;
		}


		public ModuleConfigurationKey GetConfigurationKey()
		{
			return ConfigurationKey;
		}

		public void HandleEvent(IpcEventInfo info)
		{
			const string methodName = "HandleEvent";
			_logger.Debug(_className, methodName, "Handling IPC event: " + info.EventName);
			if (
				_config != null &&
				(info.EventName == "MemberSelected" || info.EventName == "MemberUpdated") &&
				info.PublishingModule != base.ConfigurationKey
				)
			{
				_grid.Rebind();
			}
		}
	}
}
