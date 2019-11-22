using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data.DomainModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Core;
using PushSharp.Apple;
using PushSharp.Google;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using System.Security.Cryptography.X509Certificates;

namespace Brierley.FrameWork.Push
{
	public class PushService : ServiceBase
	{
        private const string _className = "PushService";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PUSH);

        string _gcmSenderID;
        string _gcmAuthToken;
        string _apnsPushCertName;

        X509Cert APNSCert = null;

        public PushService(ServiceConfig config)
            : base(config)
        {
			_gcmSenderID = LWConfigurationUtil.GetConfigurationValue(Brierley.FrameWork.Push.Constants.GCMSenderID);
			_gcmAuthToken = LWConfigurationUtil.GetConfigurationValue(Brierley.FrameWork.Push.Constants.GCMAuthToken);
            _apnsPushCertName = LWConfigurationUtil.GetConfigurationValue(Brierley.FrameWork.Push.Constants.APNSPushCertName);

            if (!string.IsNullOrEmpty(_apnsPushCertName))
            {
                using (var dataService = new DataService(config))
                    APNSCert = dataService.GetX509Cert(_apnsPushCertName);
            }

            bool AndroidSetup = !string.IsNullOrEmpty(_gcmSenderID) && !string.IsNullOrEmpty(_gcmAuthToken);
            bool AppleSetup = APNSCert != null;

            if (!AndroidSetup && !AppleSetup)
			{
				throw new Exception(
					string.Format(
					"Push notification setup has not been completed. Please have an administrator complete setup by providing framework settings for either GCM: {0}, {1} or APNS {2}",
					Brierley.FrameWork.Push.Constants.GCMSenderID,
					Brierley.FrameWork.Push.Constants.GCMAuthToken,
					Brierley.FrameWork.Push.Constants.APNSPushCertName));
			}
		}

        /// <summary>
        /// Sends a notification with PushSharp via APNS or GCM.
        /// </summary>
        /// <param name="notificationId">The Id of the notification definition.</param>
        /// <param name="member">The member recieving the notification.</param>
        public async Task SendAsync(long notificationId, Member member)
        {
            const string methodName = "SendAsync";
            try
            {
                if (LWConfigurationUtil.GetCurrentEnvironmentContext() == null)
                    LWConfigurationUtil.SetCurrentEnvironmentContext(Config.Organization, Config.Environment);
                //Get the Device record for the member and check the device type
                using (var loyaltyService = new LoyaltyDataService(Config))
                {
                    MobileDevice device = loyaltyService.GetLatestMobileDevice(member);
                    if (device != null)
                    {
                        if (device.AcceptsPush)
                        {
                            PushSession activeSession = loyaltyService.GetActivePushSessions(device.Id);
                            if (activeSession != null)
                            {
                                Dictionary<string, object> payload = BuildNotificationPayload(member, notificationId, device.DeviceType, null);
                                switch (device.DeviceType)
                                {
                                    case DeviceType.Android:
                                        // Android payload { \"somekey\" : \"somevalue\" }
                                        await SendGCMNotification(member, notificationId, device.PushToken, payload);
                                        break;
                                    case DeviceType.Apple:
                                        // Apple Payload {\"aps\":{\"badge\":7}}
                                        await SendAPNSNotification(member, notificationId, device.PushToken, payload);
                                        break;
                                }
                            }
                            else
                            {
                                _logger.Error(_className, methodName, string.Format("Member with ipcode {0} does not have an active session.", member.IpCode));
                            }
                        }
                    }
                    else
                    {
                        _logger.Error(_className, methodName, string.Format("Member with ipcode {0} does not have any associated devices.", member.IpCode));
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Unexpected exception caught in PushService";
                _logger.Error(_className, methodName, msg, ex);
                throw ex;
            }
        }

        public void Send(Member member, long notificationId, long deviceId, string itemId)
        {
            const string methodName = "SendAsync";
            try
            {
                //Get the Device record for the member and check the device type
                using (var loyaltyService = new LoyaltyDataService(Config))
                {
                    MobileDevice device = loyaltyService.GetMobileDevice(deviceId);
                    if (device != null)
                    {
                        if (device.AcceptsPush)
                        {
                            Dictionary<string, object> payload = BuildNotificationPayload(member, notificationId, device.DeviceType, itemId);
                            switch (device.DeviceType)
                            {
                                case DeviceType.Android:
                                    // Android payload { \"somekey\" : \"somevalue\" }
                                    SendGCMNotification(member, notificationId, device.PushToken, payload);
                                    break;
                                case DeviceType.Apple:
                                    // Apple Payload {\"aps\":{\"badge\":7}}
                                    SendAPNSNotification(member, notificationId, device.PushToken, payload);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, ex.Message);
                throw new LWDataServiceException(ex.Message, ex) { ErrorCode = 3359 };
            }
        }

        private Dictionary<string, object> BuildNotificationPayload(Member member, long notificationId, DeviceType deviceType, string itemId)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            using (var contentService = new ContentService(Config))
            {
                string lang = LanguageChannelUtil.IsLanguageValid(contentService, member.PreferredLanguage)
                                ? member.PreferredLanguage
                                : LanguageChannelUtil.GetDefaultCulture();

                NotificationDef nDef = contentService.GetNotificationDef(notificationId);

                if (!string.IsNullOrEmpty(nDef.GetTitle(lang, "Push"))) payload.Add("title", nDef.GetTitle(lang, "Push"));
                if (!string.IsNullOrEmpty(nDef.GetBody(lang, "Push"))) payload.Add("body", nDef.GetBody(lang, "Push"));
                if (!string.IsNullOrEmpty(nDef.Sound)) payload.Add("sound", nDef.Sound);

                if (!string.IsNullOrEmpty(nDef.Actions))
                {
                    string[] actions = nDef.Actions.Split(';');
                    List<string> lstActions =    actions.Where(s => s.Split(':')[0].ToLower() != "category").Select(s => "\"" + s.Split(':')[0] + "\":\"" + s.Split(':')[1] + AddItemId(s.Split(':')[1], itemId) + "\"").ToList();
                    List<string> lstCategories = actions.Where(s => s.Split(':')[0].ToLower() == "category").Select(s => "\"" + s.Split(':')[0] + "\":\"" + s.Split(':')[1] + AddItemId(s.Split(':')[1], itemId) + "\"").ToList();

                    switch (deviceType)
                    {
                        case DeviceType.Android:
                            foreach (string action in lstActions)
                                payload.Add(action.Split(':')[0], action.Split(':')[1]);
                            break;
                        case DeviceType.Apple:
                            foreach (string category in lstCategories)
                                payload.Add(category.Split(':')[0], category.Split(':')[1]);
                            break;
                    }
                }
            }

            return payload;
        }

        private string AddItemId(string path, string itemId)
        {
            string val = string.Empty;

            if (!string.IsNullOrEmpty(itemId))
                val = path.EndsWith("/") ? itemId : "/" + itemId;

            return val;
        }

        private Task<int> SendGCMNotification(Member member, long notificaitonId, string pushToken, Dictionary<string, object> payload)
        {
            try
            {
                if (string.IsNullOrEmpty(_gcmSenderID) || string.IsNullOrEmpty(_gcmAuthToken))
                {
                    _logger.Error(string.Format("Push Token {0} for member ipcode {1} did not receive the notification due to incomplete setup for GCM.", pushToken,  member.IpCode));
                    _logger.Error(string.Format(
                    "Push notification setup has not been completed. Please have an administrator complete setup by providing framework settings for GCM: {0}, {1}",
                    Brierley.FrameWork.Push.Constants.GCMSenderID,
                    Brierley.FrameWork.Push.Constants.GCMAuthToken));
                    throw new LWException("GCM setup is incomplete");
                }
                // Configuration
                var config = new GcmConfiguration(_gcmSenderID, _gcmAuthToken, null);
                // Create a new broker
                var gcmBroker = new GcmServiceBroker(config);
                // Wire up events
                gcmBroker.OnNotificationFailed += (notification, aggregateEx) =>
                {
                    aggregateEx.Handle(ex =>
                    {
                        // See what kind of exception it was to further diagnose
                        if (ex is GcmNotificationException)
                        {
                            var notificationException = (GcmNotificationException)ex;
                            // Deal with the failed notification
                            var gcmNotification = notificationException.Notification;
                            var description = notificationException.Description;
                            _logger.Error(string.Format("GCM Notification Failed: ID= {0}, Desc={1}", gcmNotification.MessageId, description));
                        }
                        else if (ex is GcmMulticastResultException)
                        {
                            var multicastException = (GcmMulticastResultException)ex;

                            foreach (var succeededNotification in multicastException.Succeeded)
                            {
                                _logger.Error(string.Format("GCM Notification Failed: ID={0}", succeededNotification.MessageId));
                            }

                            foreach (var failedKvp in multicastException.Failed)
                            {
                                var n = failedKvp.Key;
                                var e = failedKvp.Value;

                                _logger.Error(string.Format("GCM Notification Failed: ID={0}, Desc={1}", n.MessageId, e.Message));
                            }
                        }
                        else if (ex is DeviceSubscriptionExpiredException)
                        {
                            var expiredException = (DeviceSubscriptionExpiredException)ex;
                            var oldId = expiredException.OldSubscriptionId;
                            var newId = expiredException.NewSubscriptionId;

                            _logger.Error(string.Format("Device RegistrationId Expired: {0}", oldId));

                            if (!string.IsNullOrWhiteSpace(newId))
                            {
                                // If this value isn't null, our subscription changed and we should update our database
                                _logger.Error(string.Format("Device RegistrationId Changed To: {0}", newId));
                            }
                        }
                        else if (ex is RetryAfterException)
                        {
                            var retryException = (RetryAfterException)ex;
                            // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                            _logger.Error(string.Format("GCM Rate Limited, don't send more until after {0}", retryException.RetryAfterUtc));
                        }
                        else
                        {
                            _logger.Error("GCM Notification Failed for some unknown reason");
                        }

                        // Mark it as handled
                        return true;
                    });
                };

                gcmBroker.OnNotificationSucceeded += (notification) =>
                {
                    _logger.Trace("GCM Notification Sent!");
                };

                // Start the broker
                gcmBroker.Start();

                // Queue a notification to send
                gcmBroker.QueueNotification(new GcmNotification
                {
                    RegistrationIds = new List<string> { pushToken },
                    Notification = JObject.Parse(JsonConvert.SerializeObject(payload))
                });

                // Stop the broker, wait for it to finish
                gcmBroker.Stop();
            }
            catch(Exception ex)
            {
                throw new LWDataServiceException(ex.Message);
            }

            return Task<int>.Factory.StartNew(() => 0);
        }

        private Task<int> SendAPNSNotification(Member member, long notificaitonId, string pushToken, Dictionary<string, object> payload)
        {
            try
            {
                if (APNSCert == null)
                {
                    _logger.Error(string.Format("Push Token {0} for member ipcode {1} did not receive the notification due to incomplete setup for APNS.", pushToken, member.IpCode));
                    _logger.Error(string.Format(
                    "Push notification setup has not been completed. Please have an administrator complete setup by providing framework settings for APNS: {0}",
                    Brierley.FrameWork.Push.Constants.APNSPushCertName));
                    throw new LWException("APNSCert is null");
                }

                // Configuration (NOTE: .pfx can also be used here)
                string certstr = APNSCert.Value;
                byte[] certbytes;
                if (certstr.StartsWith("-----BEGIN CERTIFICATE-----"))
                {
                    certbytes = new UTF8Encoding().GetBytes(certstr);
                }
                else
                {
                    certbytes = Convert.FromBase64String(certstr);
                }
                
                string certPassword = string.IsNullOrEmpty(APNSCert.CertPassword) ? string.Empty : CryptoUtil.DecodeUTF8(APNSCert.CertPassword);
                X509Certificate2 cert = new X509Certificate2(certbytes, certPassword, X509KeyStorageFlags.MachineKeySet);

                long memberActiveMessages = 0;
                using (var service = new LoyaltyDataService(Config))
                {
                    memberActiveMessages = service.HowManyActiveUnreadMessages(member.IpCode);
                }

                Dictionary<string, object> fullPayload = new Dictionary<string, object>();
                Dictionary<string, object> aps = new Dictionary<string, object>();
                aps.Add("alert", payload);
                fullPayload.Add("aps", aps);
                fullPayload.Add("badge", memberActiveMessages);

                var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, cert, true);
                // Create a new broker
                var apnsBroker = new ApnsServiceBroker(config);
                // Wire up events
                apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
                {
                    aggregateEx.Handle(ex =>
                    {
                        // See what kind of exception it was to further diagnose
                        if (ex is ApnsNotificationException)
                        {
                            var notificationException = (ApnsNotificationException)ex;
                            // Deal with the failed notification
                            var apnsNotification = notificationException.Notification;
                            var statusCode = notificationException.ErrorStatusCode;

                            _logger.Error(string.Format("Apple Notification Failed: ID={0}, Code={1}", apnsNotification.Identifier, statusCode));
                        }
                        else
                        {
                            // Inner exception might hold more useful information like an ApnsConnectionException           
                            _logger.Error(string.Format("Apple Notification Failed for some unknown reason : {0}", ex.InnerException));
                        }
                        _logger.Error(ex.Message, ex);

                        // Mark it as handled
                        return true;
                    });
                };

                apnsBroker.OnNotificationSucceeded += (notification) =>
                {
                    _logger.Trace("Apple Notification Sent!");
                };

                // Start the broker
                apnsBroker.Start();

                // Queue a notification to send
                apnsBroker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = pushToken,
                    Payload = JObject.Parse(JsonConvert.SerializeObject(fullPayload))
                });

                // Stop the broker, wait for it to finish   
                apnsBroker.Stop();
            }
            catch (Exception ex)
            {
                throw new LWDataServiceException(ex.Message, ex);
            }

            return Task<int>.Factory.StartNew(() => 0);
        }
    }
}
