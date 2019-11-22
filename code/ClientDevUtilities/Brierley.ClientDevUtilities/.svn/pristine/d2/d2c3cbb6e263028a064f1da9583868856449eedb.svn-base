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
    public class GetTop10Stores : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetTop10Stores";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetTop10Stores() : base("GetTop10Stores") { }

        #region Private Helpers
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            Member m = token.CachedMember;

            List<MGStoreDef> stores = token.Top10Stores;

            if (stores == null)
            {
                _logger.Debug(_className, methodName, string.Format("Retrieving top 10 stores for member with ipcode = {0}", m.IpCode));
                stores = new List<MGStoreDef>();
                List<MemberStore> top10Stores = LoyaltyService.GetMemberStoresByMember(m.IpCode);                
                foreach (MemberStore store in top10Stores)
                {
                    StoreDef storeDef = ContentService.GetStoreDef(store.StoreDefId);
                    stores.Add(MGStoreDef.Hydrate(storeDef));
                }
                token.Top10Stores = stores;
            }

            return stores;
        }
        #endregion
    }
}