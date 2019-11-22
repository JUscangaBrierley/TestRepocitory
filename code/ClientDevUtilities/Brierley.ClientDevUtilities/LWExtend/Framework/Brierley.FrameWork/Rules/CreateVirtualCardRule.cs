using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.Sql;
using Brierley.FrameWork.Rules.UIDesign;
using Brierley.FrameWork.Sms;

namespace Brierley.FrameWork.Rules
{
	[Serializable]
	public class CreateVirtualCardRule : RuleBase
	{
        public class CreateVirtualCardRuleResult : ContextObject.RuleResult
        {
            public string LoyaltyIdNumber;
        }

		private const string _className = "CreateVirtualCardRule";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private enum LoyaltyIDSource { Existing, Generate, Bank };

        private bool _skipIfVCExists = false;
        private string _loyaltyIdGenerationSource = string.Empty;

        private static Dictionary<string, PetaPoco.Database> _databases = new Dictionary<string, PetaPoco.Database>();
        private static PetaPoco.Database Database
        {
            get
            {
                var config = LWDataServiceUtil.GetServiceConfiguration();
                string key = DataServiceUtil.GetKey(config.Organization, config.Environment);
                if (!_databases.ContainsKey(key))
                {
                    _databases.Add(key, config.CreateDatabase());
                }
                return _databases[key];
            }
        }

		public override string DisplayText
		{
			get { return "Create Virtual Card"; }
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Skip creation if member has an existing virtual card")]
		[Description("This is a bScript expression that provides the recipient mobile phone number.")]
		[RuleProperty(false, false, false, null, false)]
		[RulePropertyOrder(1)]
        public bool SkipIfVCExists
		{
			get
			{
                return _skipIfVCExists;
			}
			set
			{
                _skipIfVCExists = value;
			}
		}

        [XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
        [DisplayName("Loyalty Id Generation Source")]
		[Description("Used to select where the Loyalty Id for the new virtual card will be pulled from.")]
        [RuleProperty(false, true, false, "LoyaltyIdGenerationSources")]
		[RulePropertyOrder(2)]
		public string LoyaltyIdGenerationSource
		{
			get
			{
					return _loyaltyIdGenerationSource;
			}
			set
			{
				_loyaltyIdGenerationSource = value;
			}
		}

		/// <summary>
		/// Returns list of available Loyalty ID Sources
		/// </summary>
		[Browsable(false)]
		public Dictionary<string, string> LoyaltyIdGenerationSources
		{
			get
			{
                Dictionary<string, string> LoyaltyIdSources = new Dictionary<string, string>();
                LoyaltyIdSources.Add("Generate New ID", LoyaltyIDSource.Generate.ToString());
                LoyaltyIdSources.Add("Pull ID from Bank", LoyaltyIDSource.Bank.ToString());

                return LoyaltyIdSources;
			}
		}

        public CreateVirtualCardRule()
            : base("CreateVirtualCardRule")
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override void Invoke(ContextObject context)
		{
			string methodName = "Invoke";

			Member lwmember = null;
			VirtualCard lwvirtualcard = null;

            _logger.Debug(_className, methodName, "Invoking CreateVirtualCard rule");
			ResolveOwners(context.Owner, ref lwmember, ref lwvirtualcard);

            if (lwmember != null)
            {
                if (SkipIfVCExists && lwmember.LoyaltyCards.Count > 0)
                    return;
                else
                {


                    string loyaltyIdNumber = string.Empty;
                    switch ((LoyaltyIDSource)Enum.Parse(typeof(LoyaltyIDSource), _loyaltyIdGenerationSource))
                    {
                        case LoyaltyIDSource.Generate:
                            using (DataService service = LWDataServiceUtil.DataServiceInstance())
                                loyaltyIdNumber = service.GetNextID("LoyaltyId").ToString();
                            break;
                        case LoyaltyIDSource.Bank:
                            loyaltyIdNumber = GetLoyaltyIdFromBank();
                            break;
                    }

                    VirtualCard virtualCard = lwmember.CreateNewVirtualCard();
                    virtualCard.LoyaltyIdNumber = loyaltyIdNumber;
                    lwmember.MarkVirtualCardAsPrimary(virtualCard);

                    var result = new CreateVirtualCardRuleResult()
                    {
                        LoyaltyIdNumber = virtualCard.LoyaltyIdNumber,
                        Name = this.RuleName,
                        RuleType = this.GetType(),
                        MemberId = lwmember.IpCode
                    };
                    AddRuleResult(context, result);
                }
            }
            else
            {
                _logger.Error(_className, methodName, "Unable to resolve a member.");
                throw new LWException("Unable to resolve a member.") { ErrorCode = 3214 };
            }
			return;
		}

		public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
		{
			string methodName = "MigrateRuleInstance";

			_logger.Trace(_className, methodName, "Migrating CreateVirtualCardRule rule.");

			CreateVirtualCardRule src = (CreateVirtualCardRule)source;

            SkipIfVCExists = src.SkipIfVCExists;
            LoyaltyIdGenerationSource = src.LoyaltyIdGenerationSource;

			RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

			return this;
		}

        private string GetLoyaltyIdFromBank()
        {
            string MethodName = "GetLoyaltyIdFromBank";

            // Generate GUID
            string processId = Guid.NewGuid().ToString("N");

            ServiceConfig config = LWDataServiceUtil.GetServiceConfiguration();
            var database = Database;
            string LoyaltyIdNumber = string.Empty;

            // Update random record with GUID
            string sql = "update {0} ats_loyaltycardbank set a_cardused = 1, a_ownerguid = @0, updatedate = @1 where a_cardused = 0 {1}";
            string sTop = string.Empty;
            string sRow = string.Empty;
            switch(config.DatabaseType)
            {
                case SupportedDataSourceType.Oracle10g:
                    sRow = "and ROWNUM = 1";
                    break;
                case SupportedDataSourceType.MySQL55:
                    break;
                case SupportedDataSourceType.MsSQL2005:
                    sTop = "TOP(1)";
                    break;
            }

            string formattedSql = string.Format(sql, sTop, sRow);
            int results = database.Execute(formattedSql, processId, DateTime.Now);

            if(results == 0)
            {
                string msg = "No loyalty cards found";
                _logger.Error(_className, MethodName, msg);
                throw new LWException(msg) { ErrorCode = 3233 };
            }

            // Select the record
            using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                LWCriterion crit = new LWCriterion("LoyaltyCardBank");
                crit.Add(LWCriterion.OperatorType.AND, "CardUsed", 1, LWCriterion.Predicate.Eq);
                crit.Add(LWCriterion.OperatorType.AND, "OwnerGuid", processId, LWCriterion.Predicate.Eq);

                List<IClientDataObject> loyaltyIds = svc.GetAttributeSetObjects(null, "LoyaltyCardBank", crit, 1, false);
                if (loyaltyIds == null || loyaltyIds.Count == 0)
                {
                    string msg = string.Format("Loyalty card not found in the bank with OwnerGuid = {0}", processId);
                    _logger.Error(_className, MethodName, msg);
                    throw new LWException(msg) { ErrorCode = 3233 };
                }

                LoyaltyIdNumber = loyaltyIds[0].GetAttributeValue("LoyaltyIdNumber").ToString();
            }

            // Return the loyaltyID number
            _logger.Debug(_className, MethodName, string.Format("GetLoyaltyIdFromBank retrieved and used the following LoyaltyId from the card bank {0}", LoyaltyIdNumber));
            return LoyaltyIdNumber;
        }
	}
}
