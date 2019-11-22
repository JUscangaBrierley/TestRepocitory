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

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Stores
{
    public class GetAllStores : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetAllStores";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetAllStores() : base("GetAllStores") { }

        #region Private Helpers
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 2)
            {
                string errMsg = "Invalid parameters provided for GetAllStores.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string startIndexStr = (string)parms[0];
            string batchSizeStr = (string)parms[1];
            int? startIndex = !string.IsNullOrEmpty(startIndexStr) ? (int?)int.Parse(startIndexStr) : null;
            int? batchSize = !string.IsNullOrEmpty(batchSizeStr) ? (int?)int.Parse(batchSizeStr) : null;

            LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);
            
            IList<StoreDef> storeList = ContentService.GetAllStoreDefs(batchInfo);
            List<MGStoreDef> stores = new List<MGStoreDef>();
            if (storeList.Count > 0)
            {
                foreach(StoreDef store in storeList)
                {
                    stores.Add(MGStoreDef.Hydrate(store));
                }                                               
            }
            return stores;
        }
        #endregion
    }
}