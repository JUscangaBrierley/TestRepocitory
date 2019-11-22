using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using System.ServiceModel;
using System.Xml.Linq;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Notification
{
	public class RegisterForPushNotification : OperationProviderBase
	{
		private const string _className = "RegisterForPushNotification";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public RegisterForPushNotification() : base("RegisterForPushNotification") { }

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			string methodName = "Invoke";
            if (parms == null || parms.Length != 6)
            {
                string errMsg = "Invalid parameters provided for RegisterForPushNotification.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string devicePushToken = (string)parms[0];
            bool forLoyaltyCards = (bool)parms[1];
            bool forCoupons = (bool)parms[2];
            bool forRewards = (bool)parms[3];
            bool forOffers = (bool)parms[4];
            bool forPromotions = (bool)parms[5];
            bool allFlagsFalse = !(forLoyaltyCards || forCoupons || forRewards || forOffers || forPromotions);

            string deviceID = token.CachedMember.IpCode.ToString();
            List<AppleWalletDevice> devices = MobileService.GetAllAppleWalletDevice(deviceID);
            if (devices != null && devices.Count > 0)
            {
                foreach (var existingDevice in devices)
                {
                    if (existingDevice.PushToken == devicePushToken)
                    {
                        _logger.Trace(_className, methodName, string.Format("Device ({0},{1}) already exists", deviceID, devicePushToken));

                        if (allFlagsFalse)
                        {
                            // all flags are false, so delete the device
                            MobileService.DeleteAppleWalletDevice(existingDevice.ID);
                            _logger.Trace(_className, methodName, string.Format("Unregistered device ({0},{1}) since all flags are false", deviceID, devicePushToken));
                            return null;
                        }

                        bool needsUpdate = false;
                        if (existingDevice.NotifyForLoyaltyCards != forLoyaltyCards)
                        {
                            existingDevice.NotifyForLoyaltyCards = forLoyaltyCards;
                            needsUpdate = true;
                        }
                        if (existingDevice.NotifyForCoupons != forCoupons)
                        {
                            existingDevice.NotifyForCoupons = forCoupons;
                            needsUpdate = true;
                        }
                        if (existingDevice.NotifyForRewards != forRewards)
                        {
                            existingDevice.NotifyForRewards = forRewards;
                            needsUpdate = true;
                        }
                        if (existingDevice.NotifyForOffers != forOffers)
                        {
                            existingDevice.NotifyForOffers = forOffers;
                            needsUpdate = true;
                        }
                        if (existingDevice.NotifyForPromotions != forPromotions)
                        {
                            existingDevice.NotifyForPromotions = forPromotions;
                            needsUpdate = true;
                        }
                        if (needsUpdate)
                        {
                            // flags changed for existing device, so update it
                            MobileService.UpdateAppleWalletDevice(existingDevice);
                            _logger.Trace(_className, methodName, string.Format("Updated device ({0},{1}), flags: cards={2}, coupons={3}, rewards={4}, offers={5}, promos={6}",
                                deviceID, devicePushToken, forLoyaltyCards, forCoupons, forRewards, forOffers, forPromotions));
                        }
                        else
                        {
                            _logger.Trace(_className, methodName, string.Format("No flag changes for device ({0},{1})", deviceID, devicePushToken));
                        }
                        return null;
                    }
                }
            }

            if (allFlagsFalse)
            {
                // all flags are false, but device is not currently registered
                _logger.Warning(_className, methodName, string.Format("Ignoring register new device ({0},{1}) since all flags are false", deviceID, devicePushToken));
                return null;
            }

            // new device registration with one or more true flags
            AppleWalletDevice newDevice = new AppleWalletDevice()
            {
                DeviceID = deviceID,
                PushToken = devicePushToken,
                NotifyForLoyaltyCards = forLoyaltyCards,
                NotifyForCoupons = forCoupons,
                NotifyForRewards = forRewards,
                NotifyForOffers = forOffers,
                NotifyForPromotions = forPromotions
            };
            MobileService.CreateAppleWalletDevice(newDevice);
            _logger.Trace(_className, methodName, string.Format("Registered device ({0},{1}), flags: cards={2}, coupons={3}, rewards={4}, offers={5}, promos={6}",
                deviceID, devicePushToken, forLoyaltyCards, forCoupons, forRewards, forOffers, forPromotions));
            return null;
		}
	}
}