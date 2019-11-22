using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Rules.UIDesign;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.CampaignManagement.DomainModel;

namespace Brierley.FrameWork.Rules
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RealtimeCampaign : RuleBase
    {
        private string _className = "RealtimeCampaign";

        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private string _campaignId = "0";

        private CampaignRuleUtil _campaignUtil = null;


        /// <summary>
        /// 
        /// </summary>
        public RealtimeCampaign()
            : base("RealtimeCampaign")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Validate()
        {
        }

        public override string DisplayText
        {
            get { return "Execute Realtime Campaign"; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Realtime Campaign")]
        [Description("The realtime campaign to run")]
        [RuleProperty(false, true, false, "RealtimeCampaigns")]
        [RulePropertyOrder(1)]
        public string CampaignToRun
        {
            get { return _campaignId; }
            set { _campaignId = value; }
        }


        /// <summary>
        /// Returns a dictionary of all realtime campaigns
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, string> RealtimeCampaigns
        {
            get
            {
                InitializeCampaignUtility();
                return GetCampaigns();
            }
        }

        private void InitializeCampaignUtility()
        {
            if (_campaignUtil == null)
            {
                _campaignUtil = new CampaignRuleUtil();
            }
        }

        private Dictionary<string, string> GetCampaigns()
        {
            InitializeCampaignUtility();
            return _campaignUtil.GetRealtimeCampaigns();
        }

        private ContextObject ExecuteCampaign(long id, ContextObject context)
        {
            InitializeCampaignUtility();
            List<CampaignResult> result = _campaignUtil.ExecuteRealTimeCampaign(id, context);
            return context;
        }

        public List<long> ExtractOutput(string type, object result)
        {
            return _campaignUtil.ExtractResultContent(type, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="PreviousResultCode"></param>
        /// <returns></returns>
        public override void Invoke(ContextObject Context)
        {
            string methodName = "Invoke";

            Member lwmember = null;
            VirtualCard lwvirtualCard = null;

            ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualCard);

            if (lwmember == null)
            {
                string errMsg = string.Format("Realtime Campaign rule must be invoked in the context of a member.");
                _logger.Error(_className, methodName, errMsg);
                throw new LWRulesException(errMsg) { ErrorCode = 3214 };
            }

            _logger.Trace(_className, methodName, "Invoking realtime campaign rule for member with Ipcode = " + lwmember.IpCode);

            if (lwmember.MemberStatus == MemberStatusEnum.Disabled ||
                lwmember.MemberStatus == MemberStatusEnum.Merged ||
                lwmember.MemberStatus == MemberStatusEnum.Terminated)
            {
                string errMsg = string.Format("Member with Ipcode {0} is in {1} status.  Content cannot be awarded to it.", lwmember.IpCode, lwmember.MemberStatus.ToString());
                _logger.Error(_className, methodName, errMsg);
                throw new LWRulesException(errMsg) { ErrorCode = 3215 };
            }

            long id = long.Parse(_campaignId);

            ExecuteCampaign(id, Context);
        }

        public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
        {
            string methodName = "MigrateRuleInstance";

            _logger.Trace(_className, methodName, "Migrating RealtimeCampaign rule.");

            RealtimeCampaign src = (RealtimeCampaign)source;

            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

            // LW-921
            using (CampaignManager sourceCampaignManager = LWDataServiceUtil.CampaignManagerInstance(sourceConfig.Organization, sourceConfig.Environment))
            {
                long campaignID = long.Parse(src.CampaignToRun);
                //if (campaignID != 0)
                //{
                Campaign sourceCampaign = sourceCampaignManager.GetCampaign(long.Parse(src.CampaignToRun));
                if (sourceCampaign != null)
                {
                    using (CampaignManager targetCampaignManager = LWDataServiceUtil.CampaignManagerInstance(targetConfig.Organization, targetConfig.Environment))
                    {
                        Campaign targetCampaign = targetCampaignManager.GetCampaign(sourceCampaign.Name);

                        if (targetCampaign == null)
                        {
                            string errMsg = string.Format("RealtimeCampaign rule cannot be migrated when campaign {0} does not exist in the target environment.", sourceCampaign.Name);
                            _logger.Error(_className, methodName, errMsg);
                            throw new LWRulesException(errMsg) { ErrorCode = 3216 };
                        }

                        this._campaignId = targetCampaign.Id.ToString();
                    }
                }
                else
                {
                    string errMsg = string.Format("Campaign id {0} cannot be found in the source environment.", src.CampaignToRun);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWRulesException(errMsg) { ErrorCode = 3217 };
                }
            }
            return this;
        }
    }
}
