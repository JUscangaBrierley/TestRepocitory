using System;
using System.Web.UI;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.MTouchProcessing
{
	public partial class ViewMTouchProcessing : ModuleControlBase, IIpcEventHandler
	{
		private const string _className = "ViewMTouchProcessing";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private MTouchProcessingConfig _config = null;

		protected string MTouch
		{
			get
			{
				if (ViewState["MTouch"] != null)
				{
					return (string)ViewState["MTouch"];
				}
				return null;
			}
			set
			{
				ViewState["MTouch"] = value;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			lblError.Text = string.Empty;
			lblSuccess.Text = string.Empty;
			_config = ConfigurationUtil.GetConfiguration<MTouchProcessingConfig>(ConfigurationKey);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (_config == null)
			{
				return;
			}

			if (!IsPostBack)
			{
				CheckMTouch();
			}
			else
			{
				if (!string.IsNullOrEmpty(MTouch))
				{
					IpcManager.RegisterEventHandler("MemberUpdated", this, false);
					IpcManager.RegisterEventHandler("MemberCreated", this, false);
				}
			}
		}

		private void CheckMTouch()
		{
			const string methodName = "CheckMTouch";

			var member = PortalState.CurrentMember;
			if (member == null)
			{
				//no member. nothing to do.
				_logger.Debug(_className, methodName, "No loyalty member selected. Exiting.");
				return;
			}

			string mTouch = Request.QueryString["MTouch"];
			if (string.IsNullOrEmpty(mTouch))
			{
				_logger.Debug(_className, methodName, "No MTouch. Exiting.");
				return;
			}
			if (!string.IsNullOrEmpty(_config.SpecificMTouch) && mTouch.ToLower() != _config.SpecificMTouch.ToLower())
			{
				_logger.Debug(_className, methodName, string.Format("MTouch {0} does not match configured mtouch {1}. Exiting.", mTouch, _config.SpecificMTouch));
				return;
			}

			MTouch mt = LoyaltyService.GetMTouch(mTouch);
			if (mt == null)
			{
				_logger.Debug(_className, methodName, string.Format("MTouch {0} not found in database. Exiting.", mTouch));
				return;
			}

			long refId = -1;
			if (!long.TryParse(mt.SecondaryId, out refId))
			{
				_logger.Debug(_className, methodName, string.Format("Could not parse MTouch SecondaryId {0}. Exiting.", mt.SecondaryId));
				return;
			}

			int usesAllowed = mt.UsesAllowed.GetValueOrDefault(0);
			if (usesAllowed > 0)
			{
				int useCount = LoyaltyService.GetMTouchUsageCount(mt);
				if (useCount >= usesAllowed)
				{
					//this mtouch has exhausted its usage limit
					_logger.Debug(_className, methodName, string.Format("MTouch {0} has reached or exceeded its usage limit {1}. Exiting.", mTouch, useCount.ToString()));
					lblError.Text = Convert.ToString(GetLocalResourceObject("MTouchQuotaMet"));
					mvMain.SetActiveView(ErrorView);
					return;
				}
			}

			if (!string.IsNullOrEmpty(mt.EntityId))
			{
				bool wrongMember = false;
				long ipCode = -1;
				if (long.TryParse(mt.EntityId, out ipCode))
				{
					if (member.IpCode != ipCode)
					{
						wrongMember = true;
					}
				}
				else
				{
					var m = LoyaltyService.LoadMemberFromEmailAddress(mt.EntityId);
					if (m == null || m.IpCode != member.IpCode)
					{
						wrongMember = true;
					}
				}

				if (wrongMember)
				{
					//wrong/missing member for this mtouch
					lblError.Text = Convert.ToString(GetLocalResourceObject("WrongMember"));
					mvMain.SetActiveView(ErrorView);
					return;
				}
			}

			if (HasArtifactBeenApplied(member, mt.MTouchType, mt.ID))
			{
				_logger.Debug(_className, methodName, string.Format("MTouch {0} has already been applied to member {1}. Exiting.", mTouch, member.IpCode.ToString()));
				lblError.Text = Convert.ToString(GetLocalResourceObject("ArtifactAlreadyApplied"));
				mvMain.SetActiveView(ErrorView);
				return;
			}

			if (_config.ArtifactApplication == MTouchProcessingConfig.ArtifactApplications.ApplyImmediately && !Page.IsPostBack)
			{
				ApplyArtifact(PortalState.CurrentMember, mt);
			}
			else if (_config.ArtifactApplication == MTouchProcessingConfig.ArtifactApplications.WaitForProfileUpdate)
			{
				MTouch = mt.MTouchValue;
			}
		}

		private bool HasArtifactBeenApplied(Member m, MTouchType type, long refId)
		{
			switch (type)
			{
				case MTouchType.Bonus:
					foreach (var mb in LoyaltyService.GetMemberBonusesByMember(m.IpCode, null))
					{
						if (mb.MTouchId.HasValue && mb.MTouchId.Value == refId)
						{
							//a bonus has already been applied to this account with this mtouch
							return true;
						}
					}
					break;
				case MTouchType.Coupon:
					foreach (var mc in LoyaltyService.GetMemberCouponsByMember(m.IpCode))
					{
						if (mc.MTouchId.HasValue && mc.MTouchId.Value == refId)
						{
							return true;
						}
					}
					break;
				case MTouchType.Promotion:
					if (LoyaltyService.IsMemberInPromotionList(m.IpCode))
					{
						//promotion has already been applied to this account. no need to apply it again
						return true;
					}
					break;
				case MTouchType.Survey:
					var respondent = SurveyManager.RetrieveRespondent(m.IpCode);
					if (respondent != null && respondent.SurveyID == refId)
					{
						return true;
					}
					break;
			}
			return false;
		}

		private void ApplyArtifact(Member m, MTouch mt)
		{
			const string methodName = "ApplyArtifact";
			_logger.Debug(_className, methodName, string.Format("Applying {0} mtouch {1} to member {2}.", mt.MTouchType.ToString(), mt.MTouchValue, m.IpCode.ToString()));

			long refId = -1;
			if(!long.TryParse(mt.SecondaryId, out refId))
			{
				return;
			}

			switch (mt.MTouchType)
			{
				case MTouchType.Bonus:
					var bonusDef = ContentService.GetBonusDef(refId);
					if (bonusDef == null)
					{
						return;
					}
                    // Check quote
                    if (bonusDef.Quota != null && bonusDef.Quota.Value > 0)
                    {
						long completed = bonusDef.ApplyQuotaToReferral.GetValueOrDefault() 
                            ? LoyaltyService.HowManyCompletedBonusReferrals(bonusDef.Id) 
                            : LoyaltyService.HowManyCompletedBonusesByType(bonusDef.Id);
                        if (completed >= bonusDef.Quota.Value)
                        {
                            // quota has already been met
							litQuotaMet.Text = bonusDef.QuotaMetHtml;
							mvMain.SetActiveView(QuotaMetView);
							return;
                        }
                    }
					var bonus = new MemberBonus() { 
						BonusDefId = refId, 
						MemberId = m.IpCode, 
						MTouchId = mt.ID
					};
					LoyaltyService.CreateMemberOffer(bonus);
					if (bonusDef.SurveyId.HasValue)
					{
						CreateSurveyRespondent(m.IpCode, bonusDef.SurveyId.Value, mt);
					}
					break;
				case MTouchType.Coupon:
					var couponDef = ContentService.GetCouponDef(refId);
					if (couponDef == null)
					{
						return;
					}
					var coupon = new MemberCoupon()
					{
						CouponDefId = couponDef.Id,
						CreateDate = DateTime.Now,
						DateIssued = DateTime.Now,
						ExpiryDate = couponDef.ExpiryDate,
						MemberId = m.IpCode, 
						MTouchId = mt.ID
					};
					LoyaltyService.CreateMemberCoupon(coupon);
					break;
				case MTouchType.Promotion:
					var promoDef = ContentService.GetPromotion(refId);
					if (promoDef == null)
					{
						return;
					}
					var promo = new MemberPromotion()
					{
						Code = promoDef.Code, 
						MemberId = m.IpCode, 
						MTouchId = mt.ID
					};
					LoyaltyService.CreateMemberPromotion(promo);
					break;
				case MTouchType.Survey:
					var surveyDef = SurveyManager.RetrieveSurvey(refId);
					if (surveyDef == null)
					{
						return;
					}
					CreateSurveyRespondent(m.IpCode, surveyDef.ID, mt);
					break;
			}
			lblSuccess.Text = Convert.ToString(GetLocalResourceObject("ArtifactSuccessfullyApplied"));
			mvMain.SetActiveView(SuccessView);
		}

		private long CreateSurveyRespondent(long ipCode, long surveyId, MTouch mt)
		{
			long respondentId = 0;

			var survey = SurveyManager.RetrieveSurvey(surveyId);
			if (survey == null)
			{
                throw new Exception(string.Format(GetLocalResourceObject("SurveyNotFound.Text").ToString(), surveyId.ToString()));
			}

			SMLanguage english = SurveyManager.RetrieveLanguage("English");

			if (english != null)
			{
				SMRespondent respondent = new SMRespondent();
				respondent.CreateDate = DateTime.Today;
				respondent.IPCode = ipCode;
				respondent.LanguageID = english.ID;
				respondent.SurveyID = survey.ID;
				respondent.MTouch = mt.MTouchValue;
				SurveyManager.CreateRespondent(respondent);
				respondentId = respondent.ID;
			}

			return respondentId;
		}

		public void HandleEvent(IpcEventInfo info)
		{
			const string methodName = "HandleEvent";
			try
			{
				if (
					info.PublishingModule != base.ConfigurationKey &&
					(info.EventName == "MemberUpdated" || info.EventName == "MemberCreated") &&
					_config != null && !string.IsNullOrEmpty(MTouch)
					)
				{
					Member m = info.EventData as Member;
					if (m != null && MTouch != null)
					{
						MTouch mt = LoyaltyService.GetMTouch(MTouch);
						MTouch = null;
						ApplyArtifact(m, mt);
						IpcManager.PublishEvent("MemberUpdated", this.ConfigurationKey, m);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, string.Empty, ex);
			}
		}

		public ModuleConfigurationKey GetConfigurationKey()
		{
			return base.ConfigurationKey;
		}

	}
}