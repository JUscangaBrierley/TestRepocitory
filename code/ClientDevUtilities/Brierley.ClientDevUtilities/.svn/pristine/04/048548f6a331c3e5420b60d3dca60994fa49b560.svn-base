using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Rewards
{
    public class GetRewardCatalog : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetRewardCatalog";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetRewardCatalog() : base("GetRewardCatalog") { }

        #region Private Helpers
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 10)
            {
                string errMsg = "Invalid parameters provided for GetRewardCatalog.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string catStr = (string)parms[2];
            long? categoryId = !string.IsNullOrEmpty(catStr) ? (long?)int.Parse(catStr) : null;

            string language = (string)parms[3];
            if (string.IsNullOrEmpty(language))
            {
                language = LanguageChannelUtil.GetDefaultCulture();
            }
            
            string channel = (string)parms[4];
            if (string.IsNullOrEmpty(channel))
            {
                channel = LanguageChannelUtil.GetDefaultChannel();
            }
            if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
            {
                throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
            }
            if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
            {
                throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
            }

            List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
            if (!string.IsNullOrEmpty((string)parms[0]))
            {
                bool activeOnly = bool.Parse((string)parms[0]);
                Dictionary<string, object> entry = new Dictionary<string, object>();
                entry.Add("Property", "Active");
                entry.Add("Predicate", LWCriterion.Predicate.Eq);
                entry.Add("Value", activeOnly);
                parmsList.Add(entry);
            }

            if (!string.IsNullOrEmpty((string)parms[1]))
            {
                TierDef tier = LoyaltyService.GetTierDef((string)parms[1]);
                if (tier != null)
                {
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    entry.Add("Property", "TierId");
                    entry.Add("Predicate", LWCriterion.Predicate.Eq);
                    entry.Add("Value", tier.Id);
                    parmsList.Add(entry);
                }
            }

            string contentAttributesStr = (string)parms[5];            
            if (!string.IsNullOrEmpty(contentAttributesStr))
            {
                MGContentAttribute[] contentAttributes = MGContentAttribute.ConvertFromJson(contentAttributesStr);
                foreach (MGContentAttribute ca in contentAttributes)
                {
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    entry.Add("Property", ca.AttributeName);
                    entry.Add("Predicate", LWCriterion.Predicate.Eq);
                    entry.Add("Value", ca.AttributeValue);
                    entry.Add("IsAttribute", true);
                    parmsList.Add(entry);
                }                
            }

            long? currencyToEarnLow = null;
            long? currencyToEarnHigh = null;
            string currencyRange = (string)parms[6];
            if ( !string.IsNullOrEmpty(currencyRange) )
            {
                string[] tokens = currencyRange.Split('-');
                currencyToEarnLow = (long)long.Parse(tokens[0]);
                if (tokens.Length > 1 && tokens[1] != "*")
                {
                    currencyToEarnHigh = (long)long.Parse(tokens[1]);
                }
            }
            if (currencyToEarnLow != null)
            {
                Dictionary<string, object> entry = new Dictionary<string, object>();
                entry.Add("Property", "HowManyPointsToEarn");
                entry.Add("Predicate", LWCriterion.Predicate.Ge);
                entry.Add("Value", currencyToEarnLow);
                parmsList.Add(entry);
            }
            if (currencyToEarnHigh != null)
            {
                Dictionary<string, object> entry = new Dictionary<string, object>();
                entry.Add("Property", "HowManyPointsToEarn");
                entry.Add("Predicate", LWCriterion.Predicate.Le);
                entry.Add("Value", currencyToEarnHigh);
                parmsList.Add(entry);
            }
            
            string returnAttributesStr = (string)parms[7];
            bool returnAttributes = !string.IsNullOrEmpty(returnAttributesStr) ? bool.Parse(returnAttributesStr) : false;

            string startIndexStr = (string)parms[8];
            string batchSizeStr = (string)parms[9];
            int? startIndex = !string.IsNullOrEmpty(startIndexStr) ? (int?)int.Parse(startIndexStr) : null;
            int? batchSize = !string.IsNullOrEmpty(batchSizeStr) ? (int?)int.Parse(batchSizeStr) : null;
            LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

            IList<RewardDef> rewards = ContentService.GetRewardDefsByProperty(parmsList, null, false, batchInfo, categoryId);
            if (rewards == null || rewards.Count == 0)
            {
                throw new LWOperationInvocationException("No reward definitions found.") { ErrorCode = 3362 };
            }

            List<MGRewardDefinition> mgRewardsList = new List<MGRewardDefinition>();
            foreach (RewardDef reward in rewards)
            {
                mgRewardsList.Add(MGRewardDefinition.Hydrate(reward, language, channel, returnAttributes));
            }

            return mgRewardsList;
        }
        #endregion
    }
}