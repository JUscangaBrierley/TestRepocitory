using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

using Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Surveys;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.CheckIn
{
    public abstract class CheckInBase : OperationProviderBase
    {
        #region Fields
        private const string _className = "CheckInBase";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        #region Properties
        //protected double StoreRadiusInKM { get; set; }
        //protected int NumberOfCheckInsAllowed { get; set; }
        //protected int SameStoreCheckInTimeThreshold { get; set; }
        #endregion
        
        public CheckInBase(string opName) : base(opName) 
        {
            //string methodName = "CheckInBase";
			
			//data used here is not available to the constructor (moved to VerifyCheckinAllowed):
			//StoreRadiusInKM = GetDoubleFunctionalParameter("StoreRadiusInKM", 10);         
			//_logger.Debug(_className, methodName, "Using store radius (km) " + StoreRadiusInKM);
			//NumberOfCheckInsAllowed = GetIntegerFunctionalParameter("NumberOfCheckInsAllowed", 1);                        
			//SameStoreCheckInTimeThreshold = GetIntegerFunctionalParameter("SameStoreCheckInTimeThreshold", 30);
        }

        public MemberMobileCheckInStatus VerifyCheckInAllowed
            (                        
            Member member, 
            MGLocation location,
            double mapRadiusInKM,
            out long nTimesCheckIn,
            ref StoreDef checkInStore)
        {
            string methodName = "CheckInAllowed";

			double storeRadiusInKM = GetDoubleFunctionalParameter("StoreRadiusInKM", mapRadiusInKM);            
			int numberOfCheckInsAllowed = GetIntegerFunctionalParameter("NumberOfCheckInsAllowed", 1);
			int sameStoreCheckInTimeThreshold = GetIntegerFunctionalParameter("SameStoreCheckInTimeThreshold", 30);

            MemberMobileCheckInStatus checkInStatus = MemberMobileCheckInStatus.Ok;
            List<StoreDef> stores = ContentService.GetAllStoreDefs();
            nTimesCheckIn = 0;
            checkInStore = null;
            bool isInStore = false;

            foreach (StoreDef store in stores)
            {
                if (store.Status != StoreStatus.Open)
                {
                    _logger.Debug(_className, methodName, string.Format("store {0} is not open.", store.StoreName));
                    continue;
                }

                double distanceInKM = 0;
                bool invalidLocation = false;
                if (CheckInUtils.IsInsideStore(location, store, storeRadiusInKM, ref distanceInKM, ref invalidLocation) && !invalidLocation)
                {
                    _logger.Debug(_className, methodName, string.Format("{0} is inside store {1}: {2} km <= {3} km", member.Username, store.StoreName, distanceInKM, storeRadiusInKM));
                    isInStore = true;

                    checkInStatus = CheckInUtils.VerifyMemberCheckInAllowed(member, store, numberOfCheckInsAllowed, sameStoreCheckInTimeThreshold, out nTimesCheckIn);
                    checkInStore = store;
                    string storeName = store.StoreName;
                    if (store.StoreId > 0) storeName += " " + store.StoreId;
                    break;
                }
                else
                {
                    _logger.Debug(_className, methodName, string.Format("{0} is outside store {1}: {2} km > {3} km", member.Username, store.StoreName, distanceInKM, storeRadiusInKM));
                }
            }

            if (!isInStore)
            {
                checkInStatus = MemberMobileCheckInStatus.NotInStore;
                string errMsg = string.Format("{0} is not inside any known store", member.Username);
                _logger.Debug(_className, methodName, errMsg);
            }

            return checkInStatus;
        }
    }
}