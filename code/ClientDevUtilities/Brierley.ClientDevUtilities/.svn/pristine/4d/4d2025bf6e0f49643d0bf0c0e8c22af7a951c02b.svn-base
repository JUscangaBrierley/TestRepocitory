using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules.UIDesign;

namespace Brierley.FrameWork.Rules
{
    [Serializable]
	public class AwardDefaultContent : RuleBase
	{
		public class AwardDefaultContentRuleResult : ContextObject.RuleResult
		{
			public IList<string> BonusAssigned = new List<string>();
			public IList<string> CouponAssigned = new List<string>();
		}
        
		private const string _className = "AwardDefaultContent";

		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private string _defaultBonusCategoryId = "0";
		private bool _issueIfNoOtherActiveBonuses = true;
		private bool _issueIfMemberNeverReceivedBonus = true;
		private string _defaultCouponCategoryId = "0";
		private bool _issueIfNoOtherActiveCoupons = true;
		private bool _issueIfMemberNeverReceivedCoupons = true;

        public AwardDefaultContent()
			: base("AwardDefaultContent")
		{
		}

		public override void Validate()
		{
		}

		public override string DisplayText
		{
			get { return "Award Default Content"; }
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Default Bonus Category")]
		[Description("The category that contains default bonuses")]
        [RuleProperty(false, true, false, "DefaultBonusCategories")]
		[RulePropertyOrder(1)]
		public string DefaultBonusCategory
		{
			get { return _defaultBonusCategoryId; }
			set { _defaultBonusCategoryId = value; }
		}


		/// <summary>
		/// Returns a dictionary of all defined top level categories
		/// </summary>
		[Browsable(false)]
		public Dictionary<string, string> DefaultBonusCategories
		{
			get
			{
				using (ContentService service = LWDataServiceUtil.ContentServiceInstance())
				{
					Dictionary<string, string> categoryMap = new Dictionary<string, string>();
					categoryMap.Add(" ", "0");
					IList<Category> catList = service.GetTopLevelCategoriesByType(CategoryType.Bonus, true);
					foreach (Category cat in catList)
					{
						categoryMap.Add(cat.Name, cat.ID.ToString());
					}
					return categoryMap;
				}
			}
		}

        /// <summary>
        /// Returns a dictionary of all defined top level categories
        /// </summary>
		[Browsable(false)]
		public Dictionary<string, string> DefaultCouponCategories
		{
			get
			{
				using (var svc = LWDataServiceUtil.ContentServiceInstance())
				{
					Dictionary<string, string> categoryMap = new Dictionary<string, string>();
					categoryMap.Add(" ", "0");
					IList<Category> catList = svc.GetTopLevelCategoriesByType(CategoryType.Coupon, true);
					foreach (Category cat in catList)
					{
						categoryMap.Add(cat.Name, cat.ID.ToString());
					}
					return categoryMap;
				}
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Issue if no other active bonuses")]
		[Description("Issue the offers if no other active bonuses are in the member's account.")]
		[RulePropertyOrder(2)]
		[RuleProperty(false, false, false, null, false, true)]
		public bool IssueIfNoOtherActiveBonuses
		{
			get
			{
				return _issueIfNoOtherActiveBonuses;
			}
			set
			{
				_issueIfNoOtherActiveBonuses = value;
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Issue member never received default bonuses")]
		[Description("Issue the offers if no other active bonuses are in the member's account.")]
		[RulePropertyOrder(3)]
        [RuleProperty(false, false, false, null, false, true)]
		public bool IssueIfMemberNeverReceivedBonus
		{
			get
			{
				return _issueIfMemberNeverReceivedBonus;
			}
			set
			{
				_issueIfMemberNeverReceivedBonus = value;
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Default Coupon Category")]
		[Description("The category that contains default coupons")]
        [RuleProperty(false, true, false, "DefaultCouponCategories")]
		[RulePropertyOrder(4)]
		public string DefaultCouponCategory
		{
			get { return _defaultCouponCategoryId; }
			set { _defaultCouponCategoryId = value; }
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Issue if no other active coupon")]
		[Description("Issue the offers if no other active coupons are in the member's account.")]
		[RulePropertyOrder(5)]
        [RuleProperty(false, false, false, null, false, true)]
		public bool IssueIfNoOtherActiveCoupons
		{
			get
			{
				return _issueIfNoOtherActiveCoupons;
			}
			set
			{
				_issueIfNoOtherActiveCoupons = value;
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Issue member never received default coupons")]
		[Description("Issue the offers if no other active coupons are in the member's account.")]
		[RulePropertyOrder(6)]
        [RuleProperty(false, false, false, null, false, true)]
		public bool IssueIfMemberNeverReceivedCoupon
		{
			get
			{
				return _issueIfMemberNeverReceivedCoupons;
			}
			set
			{
				_issueIfMemberNeverReceivedCoupons = value;
			}
		}

        private long CreateSurveyRespondent(MemberBonus mb, BonusDef def)
		{
			long respondentId = 0;
			if (def.SurveyId != null)
			{
				long surveyId = def.SurveyId.GetValueOrDefault();
				using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance())
				{
					var survey = surveyManager.RetrieveSurvey(surveyId);
					if (survey == null)
					{
						throw new Exception("Cannot create surveys because the survey (" + surveyId.ToString() + ") could not be found. Please ensure that the survey selected for the output step exists.");
					}

					SMLanguage english = surveyManager.RetrieveLanguage("English");

					if (english != null)
					{
						SMRespondent respondent = new SMRespondent();
						respondent.CreateDate = DateTime.Today;
						respondent.IPCode = mb.MemberId;
						respondent.LanguageID = english.ID;
						respondent.SurveyID = survey.ID;
						surveyManager.CreateRespondent(respondent);
						respondentId = respondent.ID;
					}
				}
			}
			return respondentId;
		}

		public override void Invoke(ContextObject Context)
		{
			string methodName = "Invoke";
			_logger.Trace(_className, methodName, "Invoking default content rule.");

			Member lwmember = null;
			VirtualCard lwvirtualCard = null;

			ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualCard);

			if (lwmember == null)
			{
				string errMsg = string.Format("AwardDefaultContent rule must be invoked in the context of a member.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWRulesException(errMsg) { ErrorCode = 3214 };
			}

			if (lwmember.MemberStatus == MemberStatusEnum.Disabled ||
				lwmember.MemberStatus == MemberStatusEnum.Merged ||
				lwmember.MemberStatus == MemberStatusEnum.Terminated)
			{
				string errMsg = string.Format("Member with Ipcode {0} is in {1} status.  Content cannot be awarded to it.", lwmember.IpCode, lwmember.MemberStatus.ToString());
				_logger.Error(_className, methodName, errMsg);
				throw new LWRulesException(errMsg) { ErrorCode = 3215 };
			}

			using(var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{

				AwardDefaultContentRuleResult result = new AwardDefaultContentRuleResult()
				{
					Name = !string.IsNullOrWhiteSpace(Context.Name) ? Context.Name : this.RuleName,
					Mode = Context.Mode,
					RuleType = this.GetType()
				};

				long bonusCategoryId = long.Parse(DefaultBonusCategory);
				if (bonusCategoryId > 0)
				{
					var bonusList = content.GetBonusDefsByCategory(bonusCategoryId, false);
					if (IssueIfMemberNeverReceivedBonus)
					{
						foreach (BonusDef bonus in bonusList)
						{
							if (loyalty.HowManyMemberBonusesByType(lwmember.IpCode, bonus.Id) == 0)
							{
								if (Context.Mode == RuleExecutionMode.Real)
								{
									MemberBonus mb = new MemberBonus()
									{
										BonusDefId = bonus.Id,
										MemberId = lwmember.IpCode,
										//ActionTaken = false,
										//SurveyTaken = false,
										Status = MemberBonusStatus.Issued,
										TimesClicked = 0
									};
									loyalty.CreateMemberOffer(mb);
									CreateSurveyRespondent(mb, bonus);
								}
								result.BonusAssigned.Add(bonus.Name);
							}
						}
					}
					if (IssueIfNoOtherActiveBonuses && loyalty.HowManyActiveMemberBonuses(lwmember.IpCode) == 0)
					{
						foreach (BonusDef bonus in bonusList)
						{
							if (Context.Mode == RuleExecutionMode.Real)
							{
								MemberBonus mb = new MemberBonus()
								{
									BonusDefId = bonus.Id,
									MemberId = lwmember.IpCode,
									//ActionTaken = false,
									//SurveyTaken = false,
									Status = MemberBonusStatus.Issued,
									TimesClicked = 0
								};
								loyalty.CreateMemberOffer(mb);
								CreateSurveyRespondent(mb, bonus);
							}
							result.BonusAssigned.Add(bonus.Name);
						}
					}
				}

				long couponCategoryId = long.Parse(DefaultCouponCategory);
				if (couponCategoryId > 0)
				{
					IList<CouponDef> couponList = content.GetCouponDefsByCategory(couponCategoryId, false);
					if (IssueIfMemberNeverReceivedCoupon)
					{
						foreach (CouponDef coupon in couponList)
						{
							if (loyalty.HowManyMemberCouponsByType(lwmember.IpCode, coupon.Id) == 0)
							{
								if (Context.Mode == RuleExecutionMode.Real && coupon.IsActive())
								{
									MemberCoupon mc = new MemberCoupon()
									{
										CouponDefId = coupon.Id,
										MemberId = lwmember.IpCode,
										TimesUsed = 0,
										DateIssued = DateTime.Now,
										ExpiryDate = coupon.ExpiryDate
									};
									loyalty.CreateMemberCoupon(mc);
								}
								result.CouponAssigned.Add(coupon.Name);
							}
						}
					}
					if (IssueIfNoOtherActiveCoupons && loyalty.HowManyActiveMemberCoupons(lwmember.IpCode) == 0)
					{
						foreach (CouponDef coupon in couponList)
						{
							if (Context.Mode == RuleExecutionMode.Real && coupon.IsActive())
							{
								MemberCoupon mc = new MemberCoupon()
								{
									CouponDefId = coupon.Id,
									MemberId = lwmember.IpCode,
									TimesUsed = 0,
									DateIssued = DateTime.Now,
									ExpiryDate = coupon.ExpiryDate
								};
								loyalty.CreateMemberCoupon(mc);
							}
							result.CouponAssigned.Add(coupon.Name);
						}
					}
				}

				AddRuleResult(Context, result);

				return;
			}
		}

		public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
		{
			string methodName = "MigrateRuleInstance";

			using (var content = new ContentService(sourceConfig))
			{
				_logger.Trace(_className, methodName, "Migrating AwardDefaultContent rule.");
				AwardDefaultContent src = (AwardDefaultContent)source;
				Category srcCat = content.GetCategory(long.Parse(src._defaultBonusCategoryId));
				if (srcCat != null)
				{
					// look for equivalent category in the destination system
					Category dstCat = RetrieveDestinationParentCategoryFromSource(srcCat, sourceConfig, targetConfig);
					if (dstCat == null)
					{
						string errMsg = "Unable to find default bonus category in destination.";
						_logger.Error(_className, methodName, errMsg);
						throw new LWRulesException(errMsg) { ErrorCode = 3216 };
					}
					_defaultBonusCategoryId = dstCat.ID.ToString();
				}
				IssueIfNoOtherActiveBonuses = src.IssueIfNoOtherActiveBonuses;
				IssueIfMemberNeverReceivedBonus = src.IssueIfMemberNeverReceivedBonus;
				srcCat = content.GetCategory(long.Parse(src._defaultCouponCategoryId));
				if (srcCat != null)
				{
					// look for equivalent category in the destination system
					Category dstCat = RetrieveDestinationParentCategoryFromSource(srcCat, sourceConfig, targetConfig);
					if (dstCat == null)
					{
						string errMsg = "Unable to find default coupon category in destination.";
						_logger.Error(_className, methodName, errMsg);
						throw new LWRulesException(errMsg) { ErrorCode = 3216 };
					}
					_defaultCouponCategoryId = dstCat.ID.ToString();
				}
				IssueIfNoOtherActiveCoupons = src.IssueIfNoOtherActiveCoupons;
				IssueIfMemberNeverReceivedCoupon = src.IssueIfMemberNeverReceivedCoupon;

				RuleVersion = src.RuleVersion;
                RuleDescription = src.RuleDescription;

				return this;
			}
		}
	}
}
