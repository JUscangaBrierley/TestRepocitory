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
    public class GetNearbyStores : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetNearbyStores";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetNearbyStores() : base("GetNearbyStores") { }

        #region Private Helpers
        private bool IsInsideStore(MGLocation mdiLocation, StoreDef store, double storeRadiusInKM, ref double distanceInKM, ref bool invalidLocation)
        {
            if (store.Latitude == null || store.Longitude == null)
            {
                invalidLocation = true;
                return false;
            }

            distanceInKM = GeoLocationUtils.GeoDistanceKM(
                mdiLocation.Latitude, mdiLocation.Longitude,
                (double)store.Latitude, (double)store.Longitude);

            // TODO: take into account the accuracy of the location (e.g., +/- 500m)

            if (distanceInKM <= storeRadiusInKM) return true;
            return false;
        }
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 2)
            {
                string errMsg = "Invalid parameters provided for GetNearbyStores.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string locationStr = (string)parms[0];
            MGLocation location = MGLocation.ConvertFromJson(locationStr);

            string mapRadiusInKMStr = (string)parms[1];
            double mapRadiusInKM = 1.0;
            if (!string.IsNullOrEmpty(mapRadiusInKMStr))
            {
                mapRadiusInKM = double.Parse(mapRadiusInKMStr);
            }
                                    
            int maxStores = Int32.MaxValue;
            if (string.IsNullOrEmpty(GetFunctionParameter("StoreListLimit")))
            {
                maxStores = int.Parse(GetFunctionParameter("StoreListLimit"));
            }

            IList<StoreDef> storeList = ContentService.GetAllStoreDefs();
            List<MGStoreDef> nearbyStores = new List<MGStoreDef>();
            if (storeList != null && storeList.Count > 0)
            {
                int nStores = 0;
                foreach (StoreDef store in storeList)
                {
                    if ( ++nStores > maxStores )
                    {
                        break;
                    }

                    double storeRadiusInKM = 1.0;  // TODO: StoreDef should have this
                    if (store.Status != StoreStatus.Open)
                    {
                        _logger.Debug(_className, methodName, string.Format("store {0} is not open.", store.StoreName));
                        continue;
                    }

                    double distanceInKM = 0;
                    bool invalidLocation = false;
                    IsInsideStore(location, store, storeRadiusInKM, ref distanceInKM, ref invalidLocation);
                    if (!invalidLocation && distanceInKM <= mapRadiusInKM)
                    {
                        MGStoreDef mgStore = MGStoreDef.Hydrate(store);
                        mgStore.DistanceInKM = distanceInKM;
                        nearbyStores.Add(mgStore);
                    }
                }
            }
            else
            {
                _logger.Error(_className, methodName, "No stores are defined.");
            }
            return nearbyStores.OrderBy(o => o.DistanceInKM).ToList();
        }
        #endregion
    }
}