using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules;

namespace Brierley.FrameWork.Data
{
	public class MobileDataService : ServiceBase
	{
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private const string _className = "MobileDataService";

		private ContentAttributeDao _contentAttributeDao;
		private LangChanContentDao _langChanContentDao;
        private MemberMobileEventDao _memberMobileEventDao;
        private MemberAppleWalletLoyaltyCardDao _memberAppleWalletLoyaltyCardDao;
        private AppleWalletDeviceDao _appleWalletDeviceDao;
        private AppleWalletDeviceRegistrationDao _appleWalletDeviceRegistrationDao;
        private AppleWalletLoyaltyCardDao _appleWalletLoyaltyCardDao;
        private AppleWalletExtendedFieldDao _appleWalletExtendedFieldDao;
        private AndroidPayLoyaltyCardDao _androidPayLoyaltyCardDao;

        public LangChanContentDao LangChanContentDao
		{
			get
			{
				if (_langChanContentDao == null)
				{
					_langChanContentDao = new LangChanContentDao(Database, Config);
				}
				return _langChanContentDao;
			}
		}

		public ContentAttributeDao ContentAttributeDao
		{
			get
			{
				if (_contentAttributeDao == null)
				{
					_contentAttributeDao = new ContentAttributeDao(Database, Config);
				}
				return _contentAttributeDao;
			}
		}

        public MemberMobileEventDao MemberMobileEventDao
        {
            get
            {
                if (_memberMobileEventDao == null)
                {
                    _memberMobileEventDao = new MemberMobileEventDao(Database, Config);
                }
                return _memberMobileEventDao;
            }
        }

        public MemberAppleWalletLoyaltyCardDao MemberAppleWalletLoyaltyCardDao
        {
            get
            {
                if (_memberAppleWalletLoyaltyCardDao == null)
                {
                    _memberAppleWalletLoyaltyCardDao = new MemberAppleWalletLoyaltyCardDao(Database, Config);
                }
                return _memberAppleWalletLoyaltyCardDao;
            }
        }

        public AppleWalletDeviceDao AppleWalletDeviceDao
        {
            get
            {
                if (_appleWalletDeviceDao == null)
                {
                    _appleWalletDeviceDao = new AppleWalletDeviceDao(Database, Config);
                }
                return _appleWalletDeviceDao;
            }
        }

        public AppleWalletDeviceRegistrationDao AppleWalletDeviceRegistrationDao
        {
            get
            {
                if (_appleWalletDeviceRegistrationDao == null)
                {
                    _appleWalletDeviceRegistrationDao = new AppleWalletDeviceRegistrationDao(Database, Config);
                }
                return _appleWalletDeviceRegistrationDao;
            }
        }
		
        public AppleWalletLoyaltyCardDao AppleWalletLoyaltyCardDao
        {
            get
            {
                if (_appleWalletLoyaltyCardDao == null)
                {
                    _appleWalletLoyaltyCardDao = new AppleWalletLoyaltyCardDao(Database, Config, LangChanContentDao, ContentAttributeDao);
                }
                return _appleWalletLoyaltyCardDao;
            }
        }

        public AppleWalletExtendedFieldDao AppleWalletExtendedFieldDao
        {
            get
            {
                if(_appleWalletExtendedFieldDao == null)
                {
                    _appleWalletExtendedFieldDao = new AppleWalletExtendedFieldDao(Database, Config, LangChanContentDao, ContentAttributeDao);
                }
                return _appleWalletExtendedFieldDao;
            }
        }

        public AndroidPayLoyaltyCardDao AndroidPayLoyaltyCardDao
        {
            get
            {
                if(_androidPayLoyaltyCardDao == null)
                {
                    _androidPayLoyaltyCardDao = new AndroidPayLoyaltyCardDao(Database, Config, LangChanContentDao, ContentAttributeDao);
                }
                return _androidPayLoyaltyCardDao;
            }
        }

        public MobileDataService(ServiceConfig config)
			: base(config)
		{
		}

        #region Apple Wallet
        #region Loyalty Cards
        public void CreateAppleWalletLoyaltyCard(AppleWalletLoyaltyCard entity)
        {
            AppleWalletLoyaltyCardDao.Create(entity);
        }

        public void UpdateAppleWalletLoyaltyCard(AppleWalletLoyaltyCard entity)
        {
            AppleWalletLoyaltyCardDao.Update(entity);
        }

        public AppleWalletLoyaltyCard GetAppleWalletLoyaltyCard(long ID)
        {
            return AppleWalletLoyaltyCardDao.Retrieve(ID);
        }

        public AppleWalletLoyaltyCard GetAppleWalletLoyaltyCard(string name)
        {
            return AppleWalletLoyaltyCardDao.Retrieve(name);
        }

        public List<AppleWalletLoyaltyCard> GetAllAppleWalletLoyaltyCard()
        {
            return AppleWalletLoyaltyCardDao.RetrieveAll() ?? new List<AppleWalletLoyaltyCard>();
        }

        public List<AppleWalletLoyaltyCard> GetAllChangedAppleWalletLoyaltyCards(DateTime since)
        {
            return AppleWalletLoyaltyCardDao.RetrieveChangedObjects(since) ?? new List<AppleWalletLoyaltyCard>();
        }

        public void DeleteAppleWalletLoyaltyCard(long ID)
        {
            AppleWalletLoyaltyCardDao.Delete(ID);
        }


        public void CreateAppleWalletExtendedField(AppleWalletExtendedField entity)
        {
            AppleWalletExtendedFieldDao.Create(entity);
        }

        public void UpdateAppleWalletExtendedField(AppleWalletExtendedField entity)
        {
            AppleWalletExtendedFieldDao.Update(entity);
        }

        public AppleWalletExtendedField GetAppleWalletExtendedField(long ID)
        {
            return AppleWalletExtendedFieldDao.Retrieve(ID);
        }

        public AppleWalletExtendedField GetAppleWalletExtendedField(string name)
        {
            return AppleWalletExtendedFieldDao.Retrieve(name);
        }

        public void DeleteAppleWalletExtendedField(long ID)
        {
            AppleWalletExtendedFieldDao.Delete(ID);
        }

        public List<AppleWalletExtendedField> GetAllAppleWalletExtendedField()
        {
            return AppleWalletExtendedFieldDao.RetrieveAll() ?? new List<AppleWalletExtendedField>();
        }

        public List<AppleWalletExtendedField> GetAllChangedAppleWalletExtendedField(DateTime since)
        {
            return AppleWalletExtendedFieldDao.RetrieveAllChanged(since) ?? new List<AppleWalletExtendedField>();
        }

        public void DeleteAppleWalletExtendedFieldByParentId(long id)
        {
            AppleWalletExtendedFieldDao.DeleteByParentId(id);
        }
        #endregion

        #region Member Cards
        public void CreateMemberAppleWalletLoyaltyCard(MemberAppleWalletLoyaltyCard entity)
        {
            MemberAppleWalletLoyaltyCardDao.Create(entity);
        }

        public void UpdateMemberAppleWalletLoyaltyCard(MemberAppleWalletLoyaltyCard entity)
        {
            MemberAppleWalletLoyaltyCardDao.Update(entity);
        }

        public MemberAppleWalletLoyaltyCard GetMemberAppleWalletLoyaltyCard(long ID)
        {
            return MemberAppleWalletLoyaltyCardDao.Retrieve(ID);
        }

        public MemberAppleWalletLoyaltyCard GetMemberAppleWalletLoyaltyCardByMemberId(long memberId)
        {
            return MemberAppleWalletLoyaltyCardDao.RetrieveByMemberId(memberId);
        }

        public MemberAppleWalletLoyaltyCard GetMemberAppleWalletLoyaltyCard(string serialNumber, string authToken)
        {
            return MemberAppleWalletLoyaltyCardDao.Retrieve(serialNumber, authToken);
        }

        public List<MemberAppleWalletLoyaltyCard> GetAllMemberAppleWalletLoyaltyCards()
        {
            return MemberAppleWalletLoyaltyCardDao.RetrieveAll() ?? new List<MemberAppleWalletLoyaltyCard>();
        }

        public void DeleteMemberPBPass(long ID)
        {
            MemberAppleWalletLoyaltyCardDao.Delete(ID);
        }
        #endregion

        #region Devices
        public void CreateAppleWalletDevice(AppleWalletDevice entity)
        {
            AppleWalletDeviceDao.Create(entity);
        }

        public void UpdateAppleWalletDevice(AppleWalletDevice entity)
        {
            AppleWalletDeviceDao.Update(entity);
        }

        public AppleWalletDevice GetAppleWalletDevice(long ID)
        {
            return AppleWalletDeviceDao.Retrieve(ID);
        }

        public AppleWalletDevice GetAppleWalletDevice(string deviceID)
        {
            return AppleWalletDeviceDao.Retrieve(deviceID);
        }

        public List<AppleWalletDevice> GetAllAppleWalletDevice()
        {
            return AppleWalletDeviceDao.RetrieveAll() ?? new List<AppleWalletDevice>();
        }

        public List<AppleWalletDevice> GetAllAppleWalletDevice(string deviceID)
        {
            return AppleWalletDeviceDao.RetrieveAll(deviceID) ?? new List<AppleWalletDevice>();
        }

        public void DeleteAppleWalletDevice(long ID)
        {
            AppleWalletDeviceDao.Delete(ID);
        }
        #endregion

        #region Registrations
        public void CreateAppleWalletDeviceRegistration(AppleWalletDeviceRegistration entity)
        {
            AppleWalletDeviceRegistrationDao.Create(entity);
        }

        public void UpdateAppleWalletDeviceRegistrationn(AppleWalletDeviceRegistration entity)
        {
            AppleWalletDeviceRegistrationDao.Update(entity);
        }

        public AppleWalletDeviceRegistration GetAppleWalletDeviceRegistration(long ID)
        {
            return AppleWalletDeviceRegistrationDao.Retrieve(ID);
        }

        public AppleWalletDeviceRegistration GetAppleWalletDeviceRegistration(long deviceID, long memberCardID)
        {
            return AppleWalletDeviceRegistrationDao.Retrieve(deviceID, memberCardID);
        }

        public List<AppleWalletDeviceRegistration> GetAllAppleWalletDeviceRegistration()
        {
            return AppleWalletDeviceRegistrationDao.RetrieveAll() ?? new List<AppleWalletDeviceRegistration>();
        }

        public List<AppleWalletDeviceRegistration> GetAllAppleWalletDeviceRegistrationForDevice(long deviceID)
        {
            return AppleWalletDeviceRegistrationDao.RetrieveAllForDevice(deviceID) ?? new List<AppleWalletDeviceRegistration>();
        }

        public List<AppleWalletDeviceRegistration> GetAllAppleWalletDeviceRegistrationForMemberCard(long memberCardID)
        {
            return AppleWalletDeviceRegistrationDao.RetrieveAllForDevice(memberCardID) ?? new List<AppleWalletDeviceRegistration>();
        }

        public void DeleteAppleWalletDeviceRegistration(long ID)
        {
            AppleWalletDeviceRegistrationDao.Delete(ID);
        }
        #endregion

        public List<string> GetSerialNumbersByDeviceID(long deviceID, DateTime? sinceUTCDate = null)
        {
            List<string> result = new List<string>();

            List<AppleWalletDeviceRegistration> registrations = GetAllAppleWalletDeviceRegistrationForDevice(deviceID);
            if (registrations != null && registrations.Count > 0)
            {
                foreach (AppleWalletDeviceRegistration registration in registrations)
                {
                    MemberAppleWalletLoyaltyCard memberLoyaltyCard = MemberAppleWalletLoyaltyCardDao.Retrieve(registration.MemberAppleWalletLoyaltyCardID);
                    if (memberLoyaltyCard != null)
                    {
                        if (sinceUTCDate == null || DateTimeUtil.GreaterEqual(memberLoyaltyCard.UpdateDate.Value, (DateTime)sinceUTCDate))
                        {
                            if (!result.Contains(memberLoyaltyCard.SerialNumber))
                            {
                                result.Add(memberLoyaltyCard.SerialNumber);
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region Android Pay
        public void CreateAndroidPayLoyaltyCard(AndroidPayLoyaltyCard entity)
        {
            AndroidPayLoyaltyCardDao.Create(entity);
        }

        public void UpdateAndroidPayLoyaltyCard(AndroidPayLoyaltyCard entity)
        {
            AndroidPayLoyaltyCardDao.Update(entity);
        }

        public AndroidPayLoyaltyCard GetAndroidPayLoyaltyCard(long ID)
        {
            return AndroidPayLoyaltyCardDao.Retrieve(ID);
        }

        public AndroidPayLoyaltyCard GetAndroidPayLoyaltyCard(string name)
        {
            return AndroidPayLoyaltyCardDao.Retrieve(name);
        }

        public List<AndroidPayLoyaltyCard> GetAllAndroidPayLoyaltyCard()
        {
            return AndroidPayLoyaltyCardDao.RetrieveAll() ?? new List<AndroidPayLoyaltyCard>();
        }

        public List<AndroidPayLoyaltyCard> GetAllChangedAndroidPayLoyaltyCards(DateTime since)
        {
            return AndroidPayLoyaltyCardDao.RetrieveChangedObjects(since) ?? new List<AndroidPayLoyaltyCard>();
        }

        public void DeleteAndroidPayLoyaltyCard(long ID)
        {
            AndroidPayLoyaltyCardDao.Delete(ID);
        }
        #endregion

        #region Mobile Events

        public void CreateMemberMobileEvent(MemberMobileEvent e)
		{
            MemberMobileEventDao.Create(e);
		}

		public List<MemberMobileEvent> GetmemberMobileEvents(long memberId, MemberMobileEventActionType action, DateTime? startDate, DateTime? endDate)
		{
            return MemberMobileEventDao.Retrieve(memberId, action, startDate, endDate) ?? new List<MemberMobileEvent>();
		}

		public long HowManyMemberMobileEvents(long memberId, MemberMobileEventActionType action, DateTime? startDate, DateTime? endDate)
		{
			return MemberMobileEventDao.HowManyEvents(memberId, action, startDate, endDate);
		}

		public ContextObject MemberCheckIn(
			Member member,
			double longitude,
			double latitude,
			double radius,
			StoreDef store,
			string awardPointRule,
			bool awardPoints,
			string checkInRulesEvent)
		{
            string methodName = "MemberCheckIn";

            #region Create Mobile Event
            MemberMobileEvent e = new MemberMobileEvent()
            {
                MemberId = member.IpCode,
                Action = MemberMobileEventActionType.CheckIn,
                Latitude = latitude,
                Longitude = longitude,
                Radius = radius,
                StoreDefId = store.StoreId,
            };
            MemberMobileEventDao.Create(e);
            #endregion

            ContextObject context = new ContextObject()
            {
                Owner = member,
                Mode = RuleExecutionMode.Real,
                Results = new List<ContextObject.RuleResult>()
            };

            context.Environment.Add(EnvironmentKeys.StoreNumber, store.StoreNumber);
            context.Environment.Add(EnvironmentKeys.StoreName, store.StoreName);
            context.Environment.Add(EnvironmentKeys.OwnerType, PointTransactionOwnerType.Store);
            context.Environment.Add(EnvironmentKeys.OwnerId, store.StoreId);
            context.Environment.Add(EnvironmentKeys.RowKey, e.Id);

            decimal pointsEarnedForCheckIn = 0;
            using (LoyaltyDataService loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance(Organization, Environment))
            {
                #region Award Points
                if (awardPoints && !string.IsNullOrEmpty(awardPointRule))
                {
                    RuleTrigger rt = loyaltyService.GetRuleByName(awardPointRule);
                    if (rt != null)
                    {
                        loyaltyService.Execute(rt, context);
                        foreach (ContextObject.RuleResult res in context.Results)
                        {
                            AwardPointsRuleResult result = res as AwardPointsRuleResult;
                            if (result != null)
                            {
                                pointsEarnedForCheckIn += result.PointsAwarded;
                            }
                        }
                    }
                    else
                    {
                        string msg = string.Format("Unable to retrieve rule {0} for awarding points for member checkin.", awardPointRule);
                        _logger.Error(_className, methodName, msg);
                        throw new LWRulesException(msg);
                    }
                }

                #endregion

                if (!string.IsNullOrEmpty(checkInRulesEvent))
                {
                    loyaltyService.ExecuteEventRules(context, checkInRulesEvent, RuleInvocationType.Manual);
                }
            }

            return context;
		}

		#endregion
	}
}
