using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules.UIDesign;
using Brierley.FrameWork.WalletPay;

namespace Brierley.FrameWork.Rules
{
    [Serializable]
    public class UpdateAndroidPayLoyaltyCard : RuleBase
    {
        public class UpdateAndroidPayLoyaltyCardRuleResult : ContextObject.RuleResult
        {
            public bool CardUpdated;
        }

        [NonSerialized]
        private const string _className = "UpdateAndroidPayLoyaltyCard";

        [NonSerialized]
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        /// <summary>
        /// 
        /// </summary>
        public UpdateAndroidPayLoyaltyCard()
            : base("UpdateAndroidPayLoyaltyCard")
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
            get { return "Update Android Pay Loyalty Card"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public override void Invoke(ContextObject Context)
        {
            string methodName = "Invoke";

            Member lwmember = null;
            VirtualCard lwvirtualCard = null;

            ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualCard);

            if (lwmember == null)
            {
                _logger.Error(_className, methodName, "Unable to resolve a member.");
                throw new System.Exception("Unable to resolve a member.");
            }

            bool wasUpdated = AndroidPayUtil.UpdateLoyaltyObject(lwmember);

            var result = new UpdateAndroidPayLoyaltyCardRuleResult()
            {
                CardUpdated = wasUpdated,
                Name = this.RuleName,
                RuleType = this.GetType(),
                MemberId = lwmember.IpCode
            };
            AddRuleResult(Context, result);

            _logger.Debug(_className, methodName, "Finished invoking update Android Pay loyalty card rule for member " + lwmember.IpCode);
            return;
        }

        public override List<string> GetBscriptsToMove()
        {
            return new List<string>();
        }

        public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
        {
            string methodName = "MigrateRuleInstance";

            _logger.Trace(_className, methodName, "Migrating UpdateAndroidPayLoyaltyCard rule.");

            UpdateAndroidPayLoyaltyCard src = (UpdateAndroidPayLoyaltyCard)source;

            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

            return this;
        }
    }
}
