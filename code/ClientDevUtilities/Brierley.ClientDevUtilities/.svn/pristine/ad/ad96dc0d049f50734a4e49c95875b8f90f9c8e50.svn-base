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

using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Rewards
{
    public class GetRewardCategories : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetRewardCategories";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetRewardCategories() : base("GetRewardCategories") { }

        #region Private Helpers        
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 3)
            {
                string errMsg = "Invalid parameters provided for GetRewardCategories.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            
            string catStr = (string)parms[0];
            long categoryId = !string.IsNullOrEmpty(catStr) ? (long)int.Parse(catStr) : 0;
            string startIndexStr = (string)parms[1];
            string batchSizeStr = (string)parms[2];
            int? startIndex = !string.IsNullOrEmpty(startIndexStr) ? (int?)int.Parse(startIndexStr) : null;
            int? batchSize = !string.IsNullOrEmpty(batchSizeStr) ? (int?)int.Parse(batchSizeStr) : null;

            LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);
            IList<Category> catList = ContentService.GetChildCategories(categoryId, true, batchInfo);

            if (catList == null || catList.Count == 0)
            {
                throw new LWOperationInvocationException("No reward categories found.") { ErrorCode = 3362 };
            }

            List<MGRewardCategory> categories = new List<MGRewardCategory>();
            foreach (Category cat in catList)
            {
                MGRewardCategory mgCat = MGRewardCategory.Hydrate(cat);
                categories.Add(mgCat);                
            }

            return categories;
        }
        #endregion
    }
}