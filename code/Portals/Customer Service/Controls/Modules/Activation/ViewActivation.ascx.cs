using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Security;

namespace Brierley.LWModules.Activation
{
	public partial class ViewActivation : ModuleControlBase
	{
		private const string _className = "ViewActivation";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private ActivationConfiguration _config;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			_config = ConfigurationUtil.GetConfiguration<ActivationConfiguration>(ConfigurationKey);
			if (_config == null)
			{
				throw new Exception(string.Format("Missing configuration for module {0}.", ConfigurationKey.ToString()));
			}
			if (_config.RequiredFields == 0)
			{
				throw new Exception(string.Format("Module {0} does not have any required fields set.", ConfigurationKey.ToString()));
			}
			if (string.IsNullOrEmpty(_config.ProfileRouteUrl))
			{
				throw new Exception(string.Format("Module {0} does not have a redirect url defined.", ConfigurationKey.ToString()));
			}

			pchAlternateId.Visible = (_config.RequiredFields & ActivationFields.AlternateId) == ActivationFields.AlternateId;
			pchHomePhone.Visible = (_config.RequiredFields & ActivationFields.HomePhone) == ActivationFields.HomePhone;
			pchLoyaltyId.Visible = (_config.RequiredFields & ActivationFields.LoyaltyId) == ActivationFields.LoyaltyId;
			pchMobilePhone.Visible = (_config.RequiredFields & ActivationFields.MobilePhone) == ActivationFields.MobilePhone;
			pchPrimaryEmail.Visible = (_config.RequiredFields & ActivationFields.PrimaryEmail) == ActivationFields.PrimaryEmail;
			pchPrimaryPhone.Visible = (_config.RequiredFields & ActivationFields.PrimaryPhone) == ActivationFields.PrimaryPhone;
			pchUsername.Visible = (_config.RequiredFields & ActivationFields.Username) == ActivationFields.Username;
			pchWorkPhone.Visible = (_config.RequiredFields & ActivationFields.WorkPhone) == ActivationFields.WorkPhone;

			pchError.Visible = false;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			lnkSubmit.Click += lnkSubmit_Click;
		}

		protected void lnkSubmit_Click(object sender, EventArgs e)
		{
			Member member = null;

			//lookup member starting with the field most likely to get a unique result, then verify other collected data
			if (pchAlternateId.Visible)
			{
				member = LoyaltyService.LoadMemberFromAlternateID(txtAlternateId.Text.Trim());
				if (member != null && !VerifyFields(member))
				{
					member = null;
				}
			}
			else if (pchLoyaltyId.Visible)
			{
				member = LoyaltyService.LoadMemberFromLoyaltyID(txtLoyaltyId.Text.Trim());
				if (member != null && !VerifyFields(member))
				{
					member = null;
				}
			}
			else if (pchPrimaryEmail.Visible)
			{
				member = LoyaltyService.LoadMemberFromEmailAddress(txtPrimaryEmail.Text.Trim());
				if (member != null && !VerifyFields(member))
				{
					member = null;
				}
			}
			else if (pchUsername.Visible)
			{
				member = LoyaltyService.LoadMemberFromUserName(txtUsername.Text.Trim());
				if (member != null && !VerifyFields(member))
				{
					member = null;
				}
			}
			else
			{
				if (pchPrimaryPhone.Visible)
				{
					List<Member> members = null;
					LWCriterion criteria = new LWCriterion("Member");
					criteria.Add(LWCriterion.OperatorType.AND, "PrimaryPhoneNumber", FormatPhone(txtPrimaryPhone.Text), LWCriterion.Predicate.Eq);
					LWQueryBatchInfo batch = new LWQueryBatchInfo(0, 100);
					members = LoyaltyService.GetAllMembers(criteria, batch);
					if (members != null)
					{
						foreach (var m in members)
						{
							if (VerifyFields(m))
							{
								if (member != null)
								{
									//we found more than one that match. this is not allowed
									ShowMultipleMatchesError();
									return;
								}
								member = m;
							}
						}
					}
				}
				else
				{
					//the rest are member detail fields
					LWCriterion criteria = new LWCriterion("MemberDetails");
					if (pchHomePhone.Visible)
					{
						criteria.Add(LWCriterion.OperatorType.AND, "HomePhone", FormatPhone(txtHomePhone.Text), LWCriterion.Predicate.Eq);
					}
					if (pchWorkPhone.Visible)
					{
						criteria.Add(LWCriterion.OperatorType.AND, "WorkPhone", FormatPhone(txtWorkPhone.Text), LWCriterion.Predicate.Eq);
					}
					if (pchMobilePhone.Visible)
					{
						criteria.Add(LWCriterion.OperatorType.AND, "MobilePhone", FormatPhone(txtMobilePhone.Text), LWCriterion.Predicate.Eq);
					}
					LWQueryBatchInfo batch = new LWQueryBatchInfo(0, 100);

					var empty = DataServiceUtil.GetNewClientDataObject("MemberDetails");
					var details = LoyaltyService.GetAttributeSetObjects(null, "MemberDetails", criteria, batch, false, false);
					if (details != null)
					{
						bool found = false;
						foreach (dynamic detail in details)
						{
							var ipcode = Convert.ToInt64(detail.IpCode);
							if (member != null && member.IpCode == ipcode)
							{
								//record for same member we've already found. skip.
								continue;
							}
							var m = LoyaltyService.LoadMemberFromIPCode(ipcode);
							if (m != null)
							{
								if (VerifyFields(m))
								{
									if (member != null)
									{
										//we found more than one that match. this is not allowed
										ShowMultipleMatchesError();
										return;
									}
									member = m;
								}
							}
						}
					}
				}
			}

			if (member == null)
			{
				ShowNoMatchingMemberError();
			}
			else
			{
				if (member.MemberStatus != MemberStatusEnum.PreEnrolled)
				{
					ShowNotPreEnrolledError();
					return;
				}
				int ageLimit = _config.AccountAgeLimitInDays.GetValueOrDefault();
				if (ageLimit > 0 && (DateTime.Now - member.CreateDate).TotalDays > ageLimit)
				{
					ShowExpiredError();
					return;
				}

				//we have a matching member in the preenrolled status and satisfying any account age restriction. 
				//login and redirect to the configured profile page:
				LoginAndRedirectMember(member);
			}
		}

		protected override bool ControlRequiresJQueryUI()
		{
			return false;
		}

		protected override bool ControlRequiresTelerikSkins()
		{
			return false;
		}

		private bool VerifyFields(Member member)
		{
			if (pchAlternateId.Visible && member.AlternateId != txtAlternateId.Text.Trim())
			{
				return false;
			}
			if (pchLoyaltyId.Visible && member.LoyaltyCards.Where(o => o.LoyaltyIdNumber == txtLoyaltyId.Text.Trim()).Count() == 0)
			{
				return false;
			}
			if (pchPrimaryEmail.Visible && !member.PrimaryEmailAddress.Equals(txtPrimaryEmail.Text.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (pchUsername.Visible && !member.Username.Equals(txtUsername.Text.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (pchPrimaryPhone.Visible && member.PrimaryPhoneNumber != FormatPhone(txtPrimaryPhone.Text))
			{
				return false;
			}
			if (pchWorkPhone.Visible || pchHomePhone.Visible || pchMobilePhone.Visible)
			{
				var details = member.GetChildAttributeSets("MemberDetails");
				if (details == null || details.Count == 0)
				{
					return false;
				}
				bool found = false;
				foreach (var detail in details)
				{
					if (pchHomePhone.Visible && (detail.GetAttributeValue("HomePhone") ?? string.Empty).ToString() != FormatPhone(txtHomePhone.Text))
					{
						continue;
					}
					if (pchWorkPhone.Visible && (detail.GetAttributeValue("WorkPhone") ?? string.Empty).ToString() != FormatPhone(txtWorkPhone.Text))
					{
						continue;
					}
					if (pchMobilePhone.Visible && (detail.GetAttributeValue("MobilePhone") ?? string.Empty).ToString() != FormatPhone(txtMobilePhone.Text))
					{
						continue;
					}
					found = true;
				}
				if (!found)
				{
					return false;
				}
			}
			return true;
		}

		private string FormatPhone(string phone)
		{
			if (!string.IsNullOrEmpty(_config.PhoneNumberMask))
			{
				return PhoneUtil.MaskPhoneNumber(phone, _config.PhoneNumberMask);
			}
			else
			{
				return PhoneUtil.ToRawPhoneNumber(phone);
			}
		}

		private void ShowMultipleMatchesError()
		{
			_logger.Warning(
				_className,
				"ShowMultipleMatchesError",
				string.Format(
					"Multiple account matches were found during member activation. Required fields: {0}. Module should be configured with a combination of fields that will always yield a unique result.",
					_config.RequiredFields.ToString()));

			pchError.Visible = true;
			litError.Text = GetLocalResourceObject("ErrorMultipleResults").ToString();
		}

		private void ShowNoMatchingMemberError()
		{
			pchError.Visible = true;
			litError.Text = GetLocalResourceObject("ErrorNoMatchingMemberFound").ToString();
		}

		private void ShowNotPreEnrolledError()
		{
			pchError.Visible = true;
			litError.Text = GetLocalResourceObject("ErrorNotPreEnrolled").ToString();
		}

		private void ShowExpiredError()
		{
			pchError.Visible = true;
			litError.Text = GetLocalResourceObject("ErrorAccountTooOld").ToString();
		}

		private void LoginAndRedirectMember(Member member)
		{
			SecurityManager.LoginMember(member, false);
			Response.Redirect(_config.ProfileRouteUrl, false);
		}
	}
}