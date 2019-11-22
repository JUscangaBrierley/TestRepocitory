using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.CheckIn
{
    public class CheckInUtils
    {
        #region Fields
        private static string _className = "CheckInUtils";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion
        
        #region Private Helpers        
        public static bool IsInsideStore(MGLocation mdiLocation, StoreDef store, double storeRadiusInKM, ref double distanceInKM, ref bool invalidLocation)
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

        public static bool CurrentlyCheckedIn(StoreDef store, IList<MemberMobileEvent> checkIns, int sameStoreCheckInTimeThreshold)
        {
            return false;
        }

        /*
         * For right now, I am not using the store in the calculation.  A configuration property should indicate
         * whether checkins are limited by date onyl or shoudl also include store.
         * */
        public static MemberMobileCheckInStatus VerifyMemberCheckInAllowed(
            Member member,
            StoreDef store, 
            int numberOfCheckInsAllowed,
            int sameStoreCheckInTimeThreshold,
            out long nTimesCheckedIn)
        {
            string methodname = "VerifyMemberCheckInAllowed";

			using (var mobileService = LWDataServiceUtil.MobileServiceInstance())
			{

				IList<MemberMobileEvent> checkIns = mobileService.GetmemberMobileEvents(member.IpCode, MemberMobileEventActionType.CheckIn, DateTime.Now, DateTime.Now);
				nTimesCheckedIn = checkIns.Count;

				_logger.Debug(_className, methodname,
					string.Format("Number of checkins allowed = {0}.  Member {1} has already checked in {2} times.",
					numberOfCheckInsAllowed, member.IpCode, nTimesCheckedIn));
				if (nTimesCheckedIn < numberOfCheckInsAllowed)
				{
					// the user has a checkin event for this store on the same date.  Check to see if the date threshold is within
					if (CurrentlyCheckedIn(store, checkIns, sameStoreCheckInTimeThreshold))
					{
						string msg = string.Format("Member ipcode {0} is already checked in the store {1} .",
							member.IpCode, store.StoreNumber);
						_logger.Error(_className, methodname, msg);
						return MemberMobileCheckInStatus.AlreadyCheckedIn;
					}
					_logger.Debug(_className, methodname, string.Format("Member {0} is allowed to checkin.", member.IpCode));
					return MemberMobileCheckInStatus.Ok;
				}
				else
				{
					string msg = string.Format("Member ipcode {0} already has {1} checkins for the day at store {2}",
						member.IpCode, nTimesCheckedIn, store.StoreNumber);
					_logger.Error(_className, methodname, msg);
					return MemberMobileCheckInStatus.ExceededMaxAllowed;
				}
			}
        }

        #endregion        
    }
}