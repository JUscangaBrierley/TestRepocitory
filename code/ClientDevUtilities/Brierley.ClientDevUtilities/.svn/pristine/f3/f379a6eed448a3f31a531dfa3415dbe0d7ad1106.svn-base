/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.CodeGen;
using Brierley.FrameWork.Data.Sql;
using Brierley.FrameWork.Dmc;
using Brierley.FrameWork.CodeGen.Schema;
using Brierley.FrameWork.Messaging.Messages;
using PetaPoco;
*/

using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.LWExtend.FrameWork.Data
{
	public class LWExtendLoyaltyDataService : ILWExtendLoyaltyDataService
	{
        private const string _className = "LWExtendLoyaltyDataService";
        private static readonly LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private LWGateway.ILWDataServiceUtil _lwDataServiceUtil;

        public LWExtendLoyaltyDataService(LWGateway.ILWDataServiceUtil lwDataServiceUtil)
        {
            _lwDataServiceUtil = lwDataServiceUtil;
        }

        public static LWExtendLoyaltyDataService Instance { get; private set; }

        static LWExtendLoyaltyDataService()
        {
            Instance = new LWExtendLoyaltyDataService(LWGateway.LWDataServiceUtil.Instance);
        }

        /*
		private const string _className = "LoyaltyDataService";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private AttributeSetMetaDataDao _attributeSetMetaDataDao;
		private AttributeMetaDataDao _attributeMetaDataDao;
		private ClientDataObjectDao _clientDataObjectDao;
		private AtsLockDao _atsLockDao;
		private MemberDao _memberDao;
		private RuleTriggerDao _ruleTriggerDao;
		private MemberLoginEventDao _memberLoginEventDao;
		private TierDao _tierDao;
		private MemberTierDao _memberTierDao;
		private PointTypeDao _pointTypeDao;
		private PointEventDao _pointEventDao;
		private PointTransactionDao _pointTransactionDao;
		private MemberRewardDao _memberRewardDao;
		private MemberOrderDao _memberOrderDao;
		private MemberCouponDao _memberCouponDao;
		private MemberCouponRedemptionDao _memberCouponRedemptionDao;
		private MemberPromotionDao _memberPromotionDao;
		private MemberBonusDao _memberBonusDao;
		private MemberMessageDao _memberMessageDao;
		private MemberSocNetDao _memberSocNetDao;
		private MemberStoreDao _memberStoreDao;
		private LWEventDao _lwEventDao;
		private MTouchDao _mTouchDao;
		private PointsSummaryDao _pointsSummaryDao;
		private TriggerUserEventLogDao _triggerUserEventLogDao;
		private NextBestActionDao _nextBestActionDao;
		private MemberNextBestActionDao _memberNextBestActionDao;
		private ValidatorDao _validatorDao;
		private ValidatorTriggerDao _validatorTriggerDao;
		private RuleExecutionLogDao _ruleExecutionLogDao;
		private FulfillmentProviderDao _fulfillmentProviderDao;
		private PushSessionDao _pushSessionDao;
		private MobileDeviceDao _mobileDeviceDao;
		private RewardChoiceDao _rewardChoiceDao;

		protected static bool QueueingEnabled { get; private set; }

		public AttributeSetMetaDataDao AttributeSetMetaDataDao
		{
			get
			{
				if (_attributeSetMetaDataDao == null)
				{
					_attributeSetMetaDataDao = new AttributeSetMetaDataDao(Database, Config) { AttributeMetaDataDao = AttributeMetaDataDao };
				}
				return _attributeSetMetaDataDao;
			}
		}

		public AttributeMetaDataDao AttributeMetaDataDao
		{
			get
			{
				if (_attributeMetaDataDao == null)
				{
					_attributeMetaDataDao = new AttributeMetaDataDao(Database, Config);
				}
				return _attributeMetaDataDao;
			}
		}

		public ClientDataObjectDao ClientDataObjectDao
		{
			get
			{
				if (_clientDataObjectDao == null)
				{
					_clientDataObjectDao = new ClientDataObjectDao(Database, Config) { AtsLockDao = AtsLockDao };
				}
				return _clientDataObjectDao;
			}
		}

		public AtsLockDao AtsLockDao
		{
			get
			{
				if (_atsLockDao == null)
				{
					_atsLockDao = new AtsLockDao(Database, Config);
				}
				return _atsLockDao;
			}
		}

		public MemberDao MemberDao
		{
			get
			{
				if (_memberDao == null)
				{
					_memberDao = new MemberDao(Database, Config);
				}
				return _memberDao;
			}
		}

		public MemberLoginEventDao MemberLoginEventDao
		{
			get
			{
				if (_memberLoginEventDao == null)
				{
					_memberLoginEventDao = new MemberLoginEventDao(Database, Config);
				}
				return _memberLoginEventDao;
			}
		}

		public RuleTriggerDao RuleTriggerDao
		{
			get
			{
				if (_ruleTriggerDao == null)
				{
					_ruleTriggerDao = new RuleTriggerDao(Database, Config);
				}
				return _ruleTriggerDao;
			}
		}

		public TierDao TierDao
		{
			get
			{
				if (_tierDao == null)
				{
                    _tierDao = new TierDao(Database,
                                           Config,
                                           new LangChanContentDao(Database, Config),
                                           new ContentAttributeDao(Database, Config));
				}
				return _tierDao;
			}
		}

		public MemberTierDao MemberTierDao
		{
			get
			{
				if (_memberTierDao == null)
				{
					_memberTierDao = new MemberTierDao(Database, Config) { TierDao = TierDao };
				}
				return _memberTierDao;
			}
		}

		public PointTypeDao PointTypeDao
		{
			get
			{
				if (_pointTypeDao == null)
				{
					_pointTypeDao = new PointTypeDao(Database, Config);
				}
				return _pointTypeDao;
			}
		}

		public PointEventDao PointEventDao
		{
			get
			{
				if (_pointEventDao == null)
				{
					_pointEventDao = new PointEventDao(Database, Config);
				}
				return _pointEventDao;
			}
		}

		public PointTransactionDao PointTransactionDao
		{
			get
			{
				if (_pointTransactionDao == null)
				{
					_pointTransactionDao = new PointTransactionDao(Database, Config);
				}
				return _pointTransactionDao;
			}
		}

		public MemberRewardDao MemberRewardDao
		{
			get
			{
				if (_memberRewardDao == null)
				{
					_memberRewardDao = new MemberRewardDao(
						Database,
						Config,
						new RewardDao(
							Database,
							Config,
							new LangChanContentDao(Database, Config),
							new ContentAttributeDao(Database, Config)));
				}
				return _memberRewardDao;
			}
		}

		public MemberOrderDao MemberOrderDao
		{
			get
			{
				if (_memberOrderDao == null)
				{
					_memberOrderDao = new MemberOrderDao(Database, Config);
				}
				return _memberOrderDao;
			}
		}

		public MemberCouponDao MemberCouponDao
		{
			get
			{
				if (_memberCouponDao == null)
				{
					_memberCouponDao = new MemberCouponDao(Database, Config);
				}
				return _memberCouponDao;
			}
		}

		public MemberCouponRedemptionDao MemberCouponRedemptionDao
		{
			get
			{
				if (_memberCouponRedemptionDao == null)
				{
					_memberCouponRedemptionDao = new MemberCouponRedemptionDao(Database, Config);
				}
				return _memberCouponRedemptionDao;
			}
		}

		public MemberPromotionDao MemberPromotionDao
		{
			get
			{
				if (_memberPromotionDao == null)
				{
					_memberPromotionDao = new MemberPromotionDao(Database, Config);
				}
				return _memberPromotionDao;
			}
		}

		public MemberBonusDao MemberBonusDao
		{
			get
			{
				if (_memberBonusDao == null)
				{
					_memberBonusDao = new MemberBonusDao(Database, Config);
				}
				return _memberBonusDao;
			}
		}

		public MemberMessageDao MemberMessageDao
		{
			get
			{
				if (_memberMessageDao == null)
				{
					_memberMessageDao = new MemberMessageDao(Database, Config);
				}
				return _memberMessageDao;
			}
		}

		public MemberSocNetDao MemberSocNetDao
		{
			get
			{
				if (_memberSocNetDao == null)
				{
					_memberSocNetDao = new MemberSocNetDao(Database, Config);
				}
				return _memberSocNetDao;
			}
		}

		public MemberStoreDao MemberStoreDao
		{
			get
			{
				if (_memberStoreDao == null)
				{
					_memberStoreDao = new MemberStoreDao(Database, Config);
				}
				return _memberStoreDao;
			}
		}

		public LWEventDao LWEventDao
		{
			get
			{
				if (_lwEventDao == null)
				{
					_lwEventDao = new LWEventDao(Database, Config);
				}
				return _lwEventDao;
			}
		}

		public MTouchDao MTouchDao
		{
			get
			{
				if (_mTouchDao == null)
				{
					_mTouchDao = new MTouchDao(Database, Config);
				}
				return _mTouchDao;
			}
		}

		public PointsSummaryDao PointsSummaryDao
		{
			get
			{
				if (_pointsSummaryDao == null)
				{
					_pointsSummaryDao = new PointsSummaryDao(Database, Config);
				}
				return _pointsSummaryDao;
			}
		}

		public TriggerUserEventLogDao TriggerUserEventLogDao
		{
			get
			{
				if (_triggerUserEventLogDao == null)
				{
					_triggerUserEventLogDao = new TriggerUserEventLogDao(Database, Config);
				}
				return _triggerUserEventLogDao;
			}
		}

		public NextBestActionDao NextBestActionDao
		{
			get
			{
				if (_nextBestActionDao == null)
				{
					_nextBestActionDao = new NextBestActionDao(Database, Config);
				}
				return _nextBestActionDao;
			}
		}

		public MemberNextBestActionDao MemberNextBestActionDao
		{
			get
			{
				if (_memberNextBestActionDao == null)
				{
					_memberNextBestActionDao = new MemberNextBestActionDao(Database, Config);
				}
				return _memberNextBestActionDao;
			}
		}

		public ValidatorDao ValidatorDao
		{
			get
			{
				if (_validatorDao == null)
				{
					_validatorDao = new ValidatorDao(Database, Config);
				}
				return _validatorDao;
			}
		}

		public ValidatorTriggerDao ValidatorTriggerDao
		{
			get
			{
				if (_validatorTriggerDao == null)
				{
					_validatorTriggerDao = new ValidatorTriggerDao(Database, Config);
				}
				return _validatorTriggerDao;
			}
		}

		public RuleExecutionLogDao RuleExecutionLogDao
		{
			get
			{
				if (_ruleExecutionLogDao == null)
				{
					_ruleExecutionLogDao = new RuleExecutionLogDao(Database, Config);
				}
				return _ruleExecutionLogDao;
			}
		}

		public FulfillmentProviderDao FulfillmentProviderDao
		{
			get
			{
				if (_fulfillmentProviderDao == null)
				{
					_fulfillmentProviderDao = new FulfillmentProviderDao(Database, Config);
				}
				return _fulfillmentProviderDao;
			}
		}

		public MobileDeviceDao MobileDeviceDao
		{
			get
			{
				if (_mobileDeviceDao == null)
				{
					_mobileDeviceDao = new MobileDeviceDao(Database, Config);
				}
				return _mobileDeviceDao;
			}
		}

		public PushSessionDao PushSessionDao
		{
			get
			{
				if (_pushSessionDao == null)
				{
					_pushSessionDao = new PushSessionDao(Database, Config);
				}
				return _pushSessionDao;
			}
		}

		public RewardChoiceDao RewardChoiceDao
		{
			get
			{
				if (_rewardChoiceDao == null)
				{
					_rewardChoiceDao = new RewardChoiceDao(Database, Config);
				}
				return _rewardChoiceDao;
			}
		}

		static LoyaltyDataService()
		{
			QueueingEnabled = !LWConfigurationUtil.IsQueueProcessor && Messaging.MessagingBus.CanSend(typeof(RuleMessage));
		}

		public LoyaltyDataService(ServiceConfig config)
			: base(config)
		{
		}

		#region Member

		/// <summary>
		/// Loads a member by a specified identity field.
		/// </summary>
		/// <param name="identityType"></param>
		/// <param name="identity"></param>
		/// <returns></returns>
		/// <exception cref="InvalidMemberIdentityException">
		/// Though not an "exceptional" condition, this is thrown if no member is found.
		/// </exception>
		public Member LoadMemberFromIdentity(AuthenticationFields identityType, string identity)
		{
			Member member = null;
			switch (identityType)
			{
				case AuthenticationFields.Username:
					member = LoadMemberFromUserName(identity);
					break;

				case AuthenticationFields.PrimaryEmailAddress:
					member = LoadMemberFromEmailAddress(identity);
					break;

				case AuthenticationFields.LoyaltyIdNumber:
					member = LoadMemberFromLoyaltyID(identity);
					break;

				case AuthenticationFields.AlternateId:
					member = LoadMemberFromAlternateID(identity);
					break;
			}
			if (member == null)
			{
				throw new InvalidMemberIdentityException(identityType, identity);
			}
			return member;
		}

		/// <summary>
		/// This method loads a member by its ipcode.
		/// </summary>
		/// <param name="ipcode">ipcode.</param>
		/// <returns>member if found.  Null if not found.</returns>
		public Member LoadMemberFromIPCode(long ipcode)
		{
			Member member = (Member)CacheManager.Get(CacheRegions.MemberByIPCode, ipcode);
			if (member == null)
			{
				member = MemberDao.Retrieve(ipcode, true);
			}
			if (member != null)
			{
				PutMemberInCache(member);
				ContextObject context = new ContextObject() { Owner = member };
				ExecuteEventRules(context, "MemberLoad", RuleInvocationType.AfterInsert);
				member.IsDirty = false;
			}
			return member;
		}

		/// <summary>
		/// This method loads a member by the email address.
		/// </summary>
		/// <param name="emailAddress">email address.</param>
		/// <returns>member if found.  Null if not found.</returns>
		public Member LoadMemberFromEmailAddress(string emailAddress)
		{
			string email = emailAddress; // ConvertEmailToUpper ? emailAddress.ToUpper() : emailAddress; // LW-1320
			Member member = MemberDao.RetrieveByEmailAddress(email, true);
			if (member != null)
			{
				PutMemberInCache(member);
				ContextObject context = new ContextObject() { Owner = member };
				ExecuteEventRules(context, "MemberLoad", RuleInvocationType.AfterInsert);
				member.IsDirty = false;
			}
			return member;
		}

		/// <summary>
		/// This method loads a member by the username.
		/// </summary>
		/// <param name="userName">user name</param>
		/// <returns>member if found.  Null if not found.</returns>
		public Member LoadMemberFromUserName(string userName)
		{
			Member member = MemberDao.RetrieveByUserName(userName, true);
			if (member != null)
			{
				PutMemberInCache(member);
				ContextObject context = new ContextObject() { Owner = member };
				ExecuteEventRules(context, "MemberLoad", RuleInvocationType.AfterInsert);
				member.IsDirty = false;
			}
			return member;
		}

		/// <summary>
		/// This method loads a member by the alternate id.
		/// </summary>
		/// <param name="alternateID">alternate id.</param>
		/// <returns>member if found.  Null if not found.</returns>
		public Member LoadMemberFromAlternateID(string alternateID)
		{
			Member member = MemberDao.RetrieveByAlternateID(alternateID, true);
			if (member != null)
			{
				PutMemberInCache(member);
				ContextObject context = new ContextObject() { Owner = member };
				ExecuteEventRules(context, "MemberLoad", RuleInvocationType.AfterInsert);
				member.IsDirty = false;
			}
			return member;
		}

		/// <summary>
		/// This method loads a member by the loyalty id.
		/// </summary>
		/// <param name="loyaltyId">loyalty id.</param>
		/// <returns>member if found.  Null if not found.</returns>
		public Member LoadMemberFromLoyaltyID(string loyaltyId)
		{
			Member member = (Member)CacheManager.Get(CacheRegions.MemberByLoyaltyId, loyaltyId);
			if (member == null)
			{
				member = MemberDao.RetrieveByLoyaltyIDNumber(loyaltyId, true);
			}
			if (member != null)
			{
				PutMemberInCache(member);
				ContextObject context = new ContextObject() { Owner = member };
				ExecuteEventRules(context, "MemberLoad", RuleInvocationType.AfterInsert);
				member.IsDirty = false;
			}
			return member;
		}

		/// <summary>
		/// This method loads a member by the reset code
		/// </summary>
		/// <param name="resetCode">the reset code</param>
		/// <returns>member or null</returns>
		public Member LoadMemberFromResetCode(string resetCode)
		{
			Member member = MemberDao.RetrieveByResetCode(resetCode);
			if (member != null)
			{
				PutMemberInCache(member);
				ContextObject context = new ContextObject() { Owner = member };
				ExecuteEventRules(context, "MemberLoad", RuleInvocationType.AfterInsert);
				member.IsDirty = false;
			}
			return member;
		}

		/// <summary>
		/// This method loads a member given a certificate number.
		/// </summary>
		/// <param name="certificateNumber"></param>
		/// <returns></returns>
		public Member LoadFromCertificateNumber(string certificateNumber)
		{
			Member member = null;
			MemberReward reward = GetMemberRewardByCert(certificateNumber);
			if (reward != null)
			{
				member = LoadMemberFromIPCode(reward.MemberId);
			}
			if (member != null)
			{
				PutMemberInCache(member);
				ContextObject context = new ContextObject() { Owner = member };
				ExecuteEventRules(context, "MemberLoad", RuleInvocationType.AfterInsert);
				member.IsDirty = false;
			}
			return member;
		}

		public List<Member> GetAllMembers(long[] ipCodeList, bool loadVirtualCards)
		{
			List<Member> members = MemberDao.RetrieveAll(ipCodeList, loadVirtualCards) ?? new List<Member>();
			foreach (Member member in members)
			{
				PutMemberInCache(member);
			}
			return members;
		}

		public List<Member> GetAllMembers(LWCriterion criteria, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null)
			{
				batchInfo.Validate();
			}

			EvaluatedCriterion whereClause = null;
			string orderBy = string.Empty;
			if (criteria != null)
			{
				whereClause = criteria.Evaluate();
				orderBy = criteria.EvaluateOrderBy();
			}
			string alias = criteria != null && criteria.UseAlias ? criteria.Alias : string.Empty;
			//string alias = criteria.UseAlias ? criteria.Alias : string.Empty;
			List<Member> members = MemberDao.Retrieve(alias, whereClause, orderBy, batchInfo) ?? new List<Member>();
			return members;
		}

		public List<Member> GetAllMembersByVcKeys(long[] vcKeys)
		{
			List<Member> members = MemberDao.RetrieveAllByVcKeys(vcKeys) ?? new List<Member>();
			foreach (Member member in members)
			{
				PutMemberInCache(member);
			}
			return members;
		}

		public List<long> GetAllMemberIds(LWCriterion criteria, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null)
			{
				batchInfo.Validate();
			}
			EvaluatedCriterion whereClause = null;
			string orderBy = string.Empty;
			if (criteria != null)
			{
				whereClause = criteria.Evaluate();
				orderBy = criteria.EvaluateOrderBy();
			}
			string alias = criteria != null && criteria.UseAlias ? criteria.Alias : string.Empty;
			List<long> ipcodes = MemberDao.RetrieveIds(alias, whereClause, orderBy, batchInfo) ?? new List<long>();
			return ipcodes;
		}

		public List<Member> GetMembersByName(string firstName, string lastName, string middleName, LWQueryBatchInfo batchInfo)
		{
			const string methodName = "GetMembersByName";
			if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(middleName))
			{
				_logger.Error(_className, methodName, "Empty parameters provided for member search by name.");
				throw new LWDataServiceException("Empty parameters provided for member search by name.") { ErrorCode = 3359 };
			}

			if (batchInfo != null)
			{
				batchInfo.Validate();
			}

			List<Member> members = MemberDao.RetrieveByName(firstName, lastName, middleName, batchInfo) ?? new List<Member>();
			foreach (Member member in members)
			{
				PutMemberInCache(member);
			}
			return members;
		}

		public List<Member> GetMembersByPhoneNumber(string phoneNumber, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null)
			{
				batchInfo.Validate();
			}
			List<Member> members = MemberDao.RetrieveByPhoneNumber(phoneNumber, batchInfo) ?? new List<Member>();
			foreach (Member member in members)
			{
				PutMemberInCache(member);
			}
			return members;
		}

		public List<Member> GetMembersByPostalCode(string postalCode, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null)
			{
				batchInfo.Validate();
			}
			List<Member> members = MemberDao.RetrieveByPostalCode(postalCode, batchInfo) ?? new List<Member>();
			foreach (Member member in members)
			{
				PutMemberInCache(member);
			}
			return members;
		}

		public void SaveMember(Member member, List<ContextObject.RuleResult> results = null, RuleExecutionMode mode = RuleExecutionMode.Real, bool skipRules = false)
		{
			const string methodName = "SaveMember";

			if (results == null)
			{
				results = new List<ContextObject.RuleResult>();
				ClearRuleResult(member);
			}

			using (var txn = Database.GetTransaction())
			{

				if (member.IsDirty || member.IsCardDirty)
				{
                    VerifyMember(member);
                    
                    ContextObject context = new ContextObject() { Owner = member, Mode = mode, Results = results };

					if (member.IpCode <= 0)
					{
						// save this member.
						if (!skipRules)
						{
							ExecuteEventRules(context, "MemberSave", RuleInvocationType.BeforeInsert);
						}
						if (mode == RuleExecutionMode.Real)
						{
							MemberDao.Create(member);
						}
						if (!skipRules)
						{
							ExecuteEventRules(context, "MemberSave", RuleInvocationType.AfterInsert);
						}
					}
					else if (member.IsDirty || member.IsCardDirty)
					{
						bool memberDirty = member.IsDirty;
						// update this member.
						if (!skipRules && member.IsDirty)
						{
							ExecuteEventRules(context, "MemberSave", RuleInvocationType.BeforeUpdate);
						}
						if (mode == RuleExecutionMode.Real)
						{
							MemberDao.Update(member);
						}
						if (!skipRules && memberDirty)
						{
							ExecuteEventRules(context, "MemberSave", RuleInvocationType.AfterUpdate);
						}
					}
				}
				// Save the member's child attribute sets
				Dictionary<string, List<IClientDataObject>> childAttSets = member.GetChildAttributeSets();
				if (childAttSets != null && childAttSets.Count > 0)
				{
					foreach (List<IClientDataObject> list in childAttSets.Values)
					{
						foreach (IClientDataObject cobj in list)
						{
							SaveClientDataObject(cobj, results, mode, skipRules);
						}
					}
				}
				// save the virtual card's child attribute sets.
				if (member.LoyaltyCards != null && member.LoyaltyCards.Count > 0)
				{
					foreach (VirtualCard card in member.LoyaltyCards)
					{
						Dictionary<string, List<IClientDataObject>> vcChildAttSets = card.GetChildAttributeSets();
						if (vcChildAttSets != null && vcChildAttSets.Count > 0)
						{
							foreach (List<IClientDataObject> list in vcChildAttSets.Values)
							{
								foreach (IClientDataObject cobj in list)
								{
									SaveClientDataObject(cobj, results, mode, skipRules);
								}
							}
						}
					}
				}

				if ((member.IsDirty || member.IsCardDirty) && mode == RuleExecutionMode.Real)
				{
					MemberDao.Update(member);
				}

				member.IsDirty = false;
				foreach (VirtualCard card in member.LoyaltyCards)
				{
					card.IsDirty = false;
				}
				ClearMemberFromCache(member);
				txn.Complete();
			}
		}


		/// <summary>
		/// This method is only used to save members and attribut sets in bulk mode.
		/// </summary>
		/// <param name="member">The members to be saved.</param>        
		/// <returns>the saved members</returns>
		public void SaveMembers(IEnumerable<Member> members)
		{
			var createList = new List<Member>();
			var updateList = new List<Member>();
			foreach (Member member in members)
			{
				if (member.IpCode <= 0)
				{
					createList.Add(member);
				}
				else
				{
					updateList.Add(member);
				}
			}

			if (createList.Count > 0)
			{
				MemberDao.Create(createList);
			}
			if (updateList.Count > 0)
			{
				MemberDao.Update(updateList);
			}

			Dictionary<string, List<IClientDataObject>> memberChildAttSets = new Dictionary<string, List<IClientDataObject>>();
			foreach (Member member in members)
			{
				// Save the member's child attribute sets
				Dictionary<string, List<IClientDataObject>> childAttSets = member.GetChildAttributeSets();
				if (childAttSets != null && childAttSets.Count > 0)
				{
					foreach (string attSetName in childAttSets.Keys)
					{
						List<IClientDataObject> childAttSetList = null;
						if (memberChildAttSets.ContainsKey(attSetName))
						{
							childAttSetList = memberChildAttSets[attSetName];
						}
						else
						{
							childAttSetList = new List<IClientDataObject>();
							memberChildAttSets.Add(attSetName, childAttSetList);
						}
						List<IClientDataObject> list = childAttSets[attSetName];
						foreach (IClientDataObject cobj in list)
						{
							if (cobj.MyKey == -1)
							{
								if (cobj.Parent != null)
								{
									// The parent object for global attribute sets is null.
									cobj.ParentRowKey = cobj.Parent.MyKey;
								}
								cobj.SetLinkKey(cobj.Parent, cobj.Parent);
							}
							childAttSetList.Add(cobj);
						}
					}
				}
			}

			foreach (string attSetName in memberChildAttSets.Keys)
			{
				List<IClientDataObject> attSets = memberChildAttSets[attSetName];
				SaveClientDataObjects(attSets);
			}

			Dictionary<string, List<IClientDataObject>> vcChildAttSets = new Dictionary<string, List<IClientDataObject>>();
			foreach (Member member in members)
			{
				foreach (VirtualCard vcCard in member.LoyaltyCards)
				{
					// Save the member's child attribute sets
					Dictionary<string, List<IClientDataObject>> childAttSets = vcCard.GetChildAttributeSets();
					if (childAttSets != null && childAttSets.Count > 0)
					{
						foreach (string attSetName in childAttSets.Keys)
						{
							List<IClientDataObject> childAttSetList = null;
							if (vcChildAttSets.ContainsKey(attSetName))
							{
								childAttSetList = vcChildAttSets[attSetName];
							}
							else
							{
								childAttSetList = new List<IClientDataObject>();
								vcChildAttSets.Add(attSetName, childAttSetList);
							}
							List<IClientDataObject> list = childAttSets[attSetName];
							foreach (IClientDataObject cobj in list)
							{
								if (cobj.MyKey == -1)
								{
									if (cobj.Parent != null)
									{
										// The parent object for global attribute sets is null.
										cobj.ParentRowKey = cobj.Parent.MyKey;
									}
									cobj.SetLinkKey(cobj.Parent, cobj.Parent);
								}
								childAttSetList.Add(cobj);
							}
						}
					}
				}
			}

			foreach (string attSetName in vcChildAttSets.Keys)
			{
				List<IClientDataObject> attSets = vcChildAttSets[attSetName];
				SaveClientDataObjects(attSets);
			}
		}

		public void CreateMemberLoginEvent(MemberLoginEvent memberLoginEvent)
		{
			if (memberLoginEvent == null)
			{
				throw new ArgumentNullException("memberLoginEvent");
			}
			MemberLoginEventDao.Create(memberLoginEvent);
		}

		public decimal CancelOrTerminateMember(Member m, DateTime effectiveDate, string reason, bool terminateFlag, MemberCancelOptions options)
		{
			const string methodName = "CancelOrTerminateMember";
			if (!terminateFlag)
			{
				_logger.Trace(_className, methodName, "Cancelling member with IpCode = " + m.IpCode);
			}
			else
			{
				_logger.Trace(_className, methodName, "Terminating member with IpCode = " + m.IpCode);
			}
			decimal pointsExpired = 0;
			PointExpirationReason expReason = !terminateFlag ? PointExpirationReason.Cancellation : PointExpirationReason.Termination;
			foreach (VirtualCard vc in m.LoyaltyCards)
			{
				pointsExpired += CancelVirtualCard(vc, "Membership being cancelled", expReason, effectiveDate, false, false, options.DeactivateCard, options.ExpirePoints);
			}
			if (!terminateFlag)
			{
				m.NewStatus = MemberStatusEnum.Disabled;
			}
			else
			{
				m.NewStatus = MemberStatusEnum.Terminated;
			}
			m.NewStatusEffectiveDate = effectiveDate;
			m.MemberCloseDate = effectiveDate;
			m.StatusChangeReason = reason;

			if (options.CancelSms || terminateFlag)
			{
				List<IClientDataObject> details = m.GetChildAttributeSets("MemberDetails");
				if (details != null && details.Count > 0)
				{
					foreach (IClientDataObject detail in details)
					{
						detail.SetAttributeValue("SmsOptIn", false);
						detail.SetAttributeValue("SmsConsentChangeDate", null);
					}
				}
			}

			ClearMemberFromCache(m);
			SaveMember(m, null, RuleExecutionMode.Real);

			// Cancel member's tiers
			if (options.CancelTiers)
			{
				List<MemberTier> tiers = GetMemberTiers(m);
				foreach (MemberTier tier in tiers)
				{
					tier.ToDate = effectiveDate;
					UpdateMemberTier(tier);
				}
			}

			if (options.CancelRewards)
			{
				// Cancel member's rewards
				List<MemberReward> rewards = GetMemberRewards(m, null);
				foreach (MemberReward reward in rewards)
				{
					reward.Expiration = effectiveDate;
					UpdateMemberReward(reward);
				}
			}

			if (options.CancelCoupons)
			{
				List<MemberCoupon> couponList = MemberCouponDao.RetrieveByMember(m.IpCode, null, false, false, null, null);
				if (couponList != null && couponList.Count > 0)
				{
					foreach (MemberCoupon coupon in couponList)
					{
						if (!coupon.ExpiryDate.HasValue || coupon.ExpiryDate.Value > DateTime.Now)
						{
							coupon.ExpiryDate = effectiveDate;
							UpdateMemberCoupon(coupon);
						}

					}
				}
			}

			if (options.CancelPromotions)
			{
				MemberPromotionDao.DeleteByMember(m.IpCode);
			}

			return pointsExpired;
		}

		public void ChangeMemberPassword(Member member, string newPassword, bool updateMember = true)
		{
			const string methodName = "ChangeMemberPassword";
			if (member == null) throw new ArgumentNullException("member");
			if (string.IsNullOrEmpty(member.Username)) throw new ArgumentNullException("member.Username");

			// ValidatePassword() throws AuthenticationException when password is invalid
			LWPasswordUtil.ValidatePassword(member.Username, newPassword);

			_logger.Debug(_className, methodName, "Change password for member: " + member.Username);

			if (LWPasswordUtil.IsHashingEnabled())
			{
				if (string.IsNullOrEmpty(member.Salt))
				{
					member.Salt = CryptoUtil.GenerateSalt();
				}
				member.Password = LWPasswordUtil.HashPassword(member.Salt, newPassword);
			}
			else
			{
				member.Password = newPassword;
			}

			member.PasswordChangeRequired = false;
			member.FailedPasswordAttemptCount = 0;

			bool enablePasswordExpiry = !StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWPasswordExpiryDisabled"), false);
			if (enablePasswordExpiry)
			{
				// PCI 8.5.9: Change user passwords at least every 90 days. 
				double daysToAdd = StringUtils.FriendlyDouble(LWConfigurationUtil.GetConfigurationValue("LWPasswordExpiryInterval"), 90);
				if (daysToAdd > 90) daysToAdd = 90;
				member.PasswordExpireDate = DateTime.Today.AddDays(daysToAdd);
			}
			else
			{
				member.PasswordExpireDate = DateTimeUtil.MaxValue;
			}
			member.ResetCode = null;
			member.ResetCodeDate = null;
			if (updateMember)
			{
				MemberDao.Update(member);
			}
		}

		public void ChangeMemberPassword(AuthenticationFields identityType, string identity, string oldPassword, string newPassword, bool passwordResetRequired)
		{
			// load member
			Member member = LoadMemberFromIdentity(identityType, identity);

			if (!passwordResetRequired) // Member is changing passwords via reset code, so they don't have the old password
			{
				// provided old password must match existing member password
				bool oldMatches = true;
				if (string.IsNullOrEmpty(member.Password) ^ string.IsNullOrEmpty(oldPassword))
				{
					oldMatches = false;
				}
				else if (!string.IsNullOrEmpty(member.Password) && !string.IsNullOrEmpty(oldPassword))
				{
					if (LWPasswordUtil.IsHashingEnabled())
					{
						try
						{
							oldPassword = LWPasswordUtil.HashPassword(member.Salt, oldPassword);
						}
						catch (LWException ex)
						{
							throw new AuthenticationException("Unable to hash 'Old Password': " + ex.Message, ex);
						}
					}
					else
					{
						oldPassword = CryptoUtil.EncodeUTF8(oldPassword);
					}
					if (member.Password != oldPassword)
					{
						oldMatches = false;
					}
				}
				if (!oldMatches)
				{
					throw new BadPasswordIncorrectOldPasswordException();
				}
			}

			// new password must be different than old password
			if (!string.IsNullOrEmpty(member.Password))
			{
				string tmp;
				if (LWPasswordUtil.IsHashingEnabled())
				{
					tmp = LWPasswordUtil.HashPassword(member.Salt, newPassword);
				}
				else
				{
					tmp = CryptoUtil.EncodeUTF8(newPassword);
				}
				if (member.Password == tmp)
				{
					throw new BadPasswordMatchesOldPasswordException();
				}
			}

			// try to change password, exception thrown if password is invalid
			ChangeMemberPassword(member, newPassword);
		}

		public string GenerateMemberResetCode(Member member, int expiryMinutes)
		{
			const string methodName = "GenerateMemberResetCode";
			// generate member's single-use code
			string singleUseCode = Guid.NewGuid().ToString("N").Substring(26, 6); // Get the last 6 digits of the 32-character Guid
			_logger.Trace(_className, methodName, "singleUseCode: " + singleUseCode);
			member.ResetCode = singleUseCode;
			member.ResetCodeDate = DateTime.Now.AddMinutes(expiryMinutes);
			SaveMember(member);
			return singleUseCode;
		}

		public Dictionary<string, string> GetPasswordResetOptions(Member member, string emailName, string smsName)
		{
			string email = string.Empty;
			string phone = string.Empty;

			IClientDataObject memberDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault();
			if (memberDetails != null)
			{
				AttributeSetMetaData memberDetailsMeta = memberDetails.GetMetaData();

				// Email info
				bool emailOptIn = false;
				if (memberDetailsMeta.GetAttribute("EmailOptIn") != null)
					emailOptIn = memberDetails.GetAttributeValue("EmailOptIn") != null && (bool)memberDetails.GetAttributeValue("EmailOptIn");

				if (emailOptIn && !string.IsNullOrEmpty(emailName))
					email = MaskEmail(member.PrimaryEmailAddress);

				// SMS info
				bool smsOptIn = false;
				if (memberDetailsMeta.GetAttribute("SmsOptIn") != null)
					smsOptIn = memberDetails.GetAttributeValue("SmsOptIn") != null && (bool)memberDetails.GetAttributeValue("SmsOptIn");

				if (smsOptIn && !string.IsNullOrEmpty(smsName) && memberDetailsMeta.GetAttribute("MobilePhone") != null)
					phone = MaskPhoneNumber(memberDetails.GetAttributeValue("MobilePhone") as string);
			}
			Dictionary<string, string> options = new Dictionary<string, string>();
			options.Add("email", email);
			options.Add("sms", phone);
			return options;
		}

		public Member ConvertToMember(Member member, DateTime effectiveDate)
		{
			const string methodName = "ConvertToMember";

			if (member.MemberStatus != MemberStatusEnum.NonMember)
			{
				_logger.Debug(_className, methodName, string.Format("Member with ipcode {0} is already a member.", member.IpCode));
				return member;
			}

			_logger.Trace(_className, methodName, string.Format("Converting non-member with ipcode {0} to member.", member.IpCode));
			member.NewStatus = MemberStatusEnum.Active;
			member.NewStatusEffectiveDate = effectiveDate;

			int gracePeriod = 0;
			string gracePeriodStr = LWConfigurationUtil.GetConfigurationValue("LW_NonMemberPointGracePeriod");
			if (!string.IsNullOrEmpty(gracePeriodStr))
			{
				gracePeriod = int.Parse(gracePeriodStr);
				_logger.Debug(_className, methodName, string.Format("Grace period {0} days will be used for accruing points for converting member with ipcode {1}", gracePeriod, member.IpCode));
			}

			DateTime? startDate = null;
			if (gracePeriod > 0)
			{
				startDate = DateTime.Now.AddDays(-1 * gracePeriod);
			}

			long[] vckeys = null;
			if (member.LoyaltyCards.Count > 0)
			{
				vckeys = new long[member.LoyaltyCards.Count];
			}
			int vidx = 0;
			foreach (VirtualCard vc in member.LoyaltyCards)
			{
				vckeys[vidx++] = vc.VcKey;
			}
			using (var txn = Database.GetTransaction())
			{
				if (gracePeriod > 0 && vckeys != null)
				{
					// expire points accrued on the card prior to the startDate.
					ExpirePoints(vckeys, startDate, effectiveDate, PointExpirationReason.MemberMerge, "Expiring points on non-member's card prior to grace period.");
				}

				//TODO: Handle any business rules related to the conversion

				ClearMemberFromCache(member);

				SaveMember(member, null, RuleExecutionMode.Real);

                txn.Complete();
            }
			return LoadMemberFromIPCode(member.IpCode);
		}

		public Member LoginMember(AuthenticationFields identityType, string identity, string password, string resetCode, ref LoginStatusEnum loginStatus, bool unlockAccount = false)
		{
			const string methodName = "LoginMember";

			// validate arguments
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(resetCode))
			{
				throw new ArgumentNullException("password");
			}
            if (!(string.IsNullOrEmpty(password) ^ string.IsNullOrEmpty(resetCode)))
            {
                throw new ArgumentException("resetCode");
            }

			// load member
			_logger.Debug(_className, methodName, string.Format("Authenticating member: {0} ({1})", identity, identityType));
			Member member = null;
			try
			{
				member = LoadMemberFromIdentity(identityType, identity);
			}
			catch (InvalidMemberIdentityException)
			{
				// no member found with that identity
				CreateMemberLoginEvent(identity, null, MemberLoginEventType.UsernameInvalid);
				throw;
			}

			// handle member resolved but member status doesn't allow login
			switch (member.MemberStatus)
			{
				case MemberStatusEnum.Disabled:
				case MemberStatusEnum.Merged:
					CreateMemberLoginEvent(identity, null, MemberLoginEventType.AccountDisabled);
					loginStatus = LoginStatusEnum.Disabled;
					return null;

				case MemberStatusEnum.Locked:
                    if (!unlockAccount || string.IsNullOrEmpty(resetCode))
                    {
                        CreateMemberLoginEvent(identity, null, MemberLoginEventType.AccountLocked);
                        loginStatus = LoginStatusEnum.LockedOut;
                        return null;
                    }
                    break;

				case MemberStatusEnum.Terminated:
					CreateMemberLoginEvent(identity, null, MemberLoginEventType.AccountTerminated);
					loginStatus = LoginStatusEnum.Terminated;
					return null;
			}

            if (!string.IsNullOrEmpty(password)) // Password authentication
            {
                // compare provided password with actual password
                string hashedPassword = password;
                if (LWPasswordUtil.IsHashingEnabled())
                {
                    if (string.IsNullOrEmpty(member.Salt))
                    {
                        member.Salt = CryptoUtil.GenerateSalt();
                    }
                    hashedPassword = LWPasswordUtil.HashPassword(member.Salt, password);
                }

                if (member.Password != hashedPassword)
                {
                    member.FailedPasswordAttemptCount = member.FailedPasswordAttemptCount + 1;
                    bool enableAccountLock = !StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWMemberAccountLockDisabled"), false);
                    string cfgMaxPasswordAttempts = LWConfigurationUtil.GetConfigurationValue("LWMemberMaxFailedPasswordAttempts");
                    int maxFailedPasswordAttempts = StringUtils.FriendlyInt32(cfgMaxPasswordAttempts, Config.MaxFailedPasswordAttempts);
                    if (enableAccountLock && member.FailedPasswordAttemptCount >= maxFailedPasswordAttempts)
                    {
                        // lock the account
                        loginStatus = LoginStatusEnum.LockedOut;
                        member.NewStatus = MemberStatusEnum.Locked;
                        member.NewStatusEffectiveDate = DateTime.Now;
                        MemberDao.Update(member);
                        CreateMemberLoginEvent(identity, null, MemberLoginEventType.AccountLocked);
                        loginStatus = LoginStatusEnum.LockedOut;
                        return null;
                    }
                    else
                    {
                        // not locked yet, so just an invalid password this time
                        MemberDao.Update(member);
                        CreateMemberLoginEvent(identity, null, MemberLoginEventType.PasswordInvalid);
                        loginStatus = LoginStatusEnum.Failure;
                        return null;
                    }
                }

                // passwords match
                if (member.IsPasswordChangeRequired())
                {
                    // login success, but password is expired
                    CreateMemberLoginEvent(identity, null, MemberLoginEventType.PasswordExpired);
                    loginStatus = LoginStatusEnum.PasswordResetRequired;
                }
                else
                {
                    CreateMemberLoginEvent(identity, null, MemberLoginEventType.LoginSuccess);
                    loginStatus = LoginStatusEnum.Success;
                }
            }
            else // Reset code
            {
                if (member.ResetCode != resetCode) // Invalid reset code
                {
                    CreateMemberLoginEvent(identity, null, MemberLoginEventType.ResetCodeInvalid);
                    loginStatus = LoginStatusEnum.Failure;
                    return null;
                }
                if (member.ResetCodeDate < DateTime.Now) // Reset code expired
                {
                    CreateMemberLoginEvent(identity, null, MemberLoginEventType.ResetCodeExpired);
                    loginStatus = LoginStatusEnum.Failure;
                    return null;
                }

                CreateMemberLoginEvent(identity, null, MemberLoginEventType.PasswordExpired);
                member.PasswordChangeRequired = true;
                if (unlockAccount && member.MemberStatus == MemberStatusEnum.Locked)
                    member.NewStatus = MemberStatusEnum.Active;
                //we can't wipe the reset code here - we still need it to change the member's password:
                //member.ResetCode = null;
                //member.ResetCodeDate = null;
                MemberDao.Update(member);
                loginStatus = LoginStatusEnum.PasswordResetRequired;
            }

			// login success, reset failed count if needed
			if (member.FailedPasswordAttemptCount > 0)
			{
				member.FailedPasswordAttemptCount = 0;
				MemberDao.Update(member);
			}

			// invoke AuthenticateMember event
			try
			{
				ContextObject contextObject = new ContextObject() { Owner = member };
				ExecuteEventRules(contextObject, "MemberAuthenticate", RuleInvocationType.Manual);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error executing event rules for MemberAuthenticate: " + ex.Message, ex);
			}
			return member;
		}

		/// <summary>
		/// logs in the member specified in the provided MemberSocNet object.
		/// </summary>
		/// <remarks>
		/// This method is intended for use by social network authentication, where a UID and auth token are supplied and matched
		/// to a member in the framework database's LW_MemberSocNet table. At this point, the member has been identified. This
		/// method will validate the account (status and password reset required), create a login event and execute any rules 
		/// tied to the MemberAuthenticate event.
		/// </remarks>
		/// <param name="socNet"></param>
		/// <param name="loginStatus"></param>
		/// <returns></returns>
		public Member LoginMember(MemberSocNet socNet, ref LoginStatusEnum loginStatus)
		{
			const string methodName = "LoginMember(MemberSocNet, LoginStatusEnum)";

			// validate arguments
			if (socNet == null)
			{
				throw new ArgumentNullException("socNet");
			}
			if (socNet.MemberId < 1)
			{
				throw new ArgumentException("Invalid MemberId in socNet");
			}

			// load member
			Member member = LoadMemberFromIPCode(socNet.MemberId);
			if (member == null)
			{
				throw new Exception(string.Format("Member with IPCode {0} not found.", socNet.MemberId));
			}

			switch (member.MemberStatus)
			{
				case MemberStatusEnum.Disabled:
				case MemberStatusEnum.Merged:
					CreateMemberLoginEvent(string.Format("{0}:{1}", socNet.ProviderType.ToString(), socNet.ProviderUID), string.Empty, MemberLoginEventType.AccountDisabled);
					loginStatus = LoginStatusEnum.Failure;
					return null;

				case MemberStatusEnum.Locked:
					CreateMemberLoginEvent(socNet.ProviderType.ToString(), socNet.ProviderUID, MemberLoginEventType.AccountLocked);
					loginStatus = LoginStatusEnum.LockedOut;
					return null;

				case MemberStatusEnum.Terminated:
					CreateMemberLoginEvent(socNet.ProviderType.ToString(), socNet.ProviderUID, MemberLoginEventType.AccountTerminated);
					loginStatus = LoginStatusEnum.Terminated;
					return null;
			}

			//should be uncommon, but possible that a new password is required
			if (member.IsPasswordChangeRequired())
			{
				// login success, but password is expired
				CreateMemberLoginEvent(socNet.ProviderType.ToString(), socNet.ProviderUID, MemberLoginEventType.PasswordExpired);
				loginStatus = LoginStatusEnum.PasswordResetRequired;
			}
			else
			{
				CreateMemberLoginEvent(socNet.ProviderType.ToString(), socNet.ProviderUID, MemberLoginEventType.LoginSuccess);
				loginStatus = LoginStatusEnum.Success;
			}

			// login success, reset failed count if needed
			if (member.FailedPasswordAttemptCount > 0)
			{
				member.FailedPasswordAttemptCount = 0;
				MemberDao.Update(member);
			}

			// invoke AuthenticateMember event
			try
			{
				ContextObject contextObject = new ContextObject() { Owner = member };
				ExecuteEventRules(contextObject, "MemberAuthenticate", RuleInvocationType.Manual);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error executing event rules for MemberAuthenticate: " + ex.Message, ex);
			}
			return member;
		}

		public Member MergeMember(Member from, Member to, PointEvent pe, PointType pt, DateTime expDate, MemberMergeOptions options)
		{
			const string methodName = "MergeMember";

			if (from.IpCode == to.IpCode)
			{
				string errMsg = "A member cannot be merged with its ownself.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg) { ErrorCode = 9976 };
			}

			if (from.MemberStatus == MemberStatusEnum.NonMember)
			{
				// "to" has to be either be an active or a non member
				if (to.MemberStatus != MemberStatusEnum.Active && to.MemberStatus != MemberStatusEnum.NonMember)
				{
					string errMsg = string.Format("Cannot merge member non-member with ipcode {0} into member with ipcode {1} with status {2}.", from.IpCode, to.IpCode, to.MemberStatus.ToString());
					_logger.Error(_className, methodName, errMsg);
					throw new LWDataServiceException(errMsg) { ErrorCode = 9968 };
				}
			}
			else if (from.MemberStatus == MemberStatusEnum.PreEnrolled)
			{
				// "to" has to be an active member
				if (to.MemberStatus != MemberStatusEnum.Active)
				{
					string errMsg = string.Format("Cannot merge pre-enrolled member with ipcode {0} into member with ipcode {1} with status {2}.", from.IpCode, to.IpCode, to.MemberStatus.ToString());
					_logger.Error(_className, methodName, errMsg);
					throw new LWDataServiceException(errMsg) { ErrorCode = 9968 };
				}
			}
			else if (from.MemberStatus == MemberStatusEnum.Active)
			{
				// "to" has to be an active meember
				if (to.MemberStatus != MemberStatusEnum.Active)
				{
					string errMsg = "Both members need to be in active status to be merged.";
					_logger.Error(_className, methodName, errMsg);
					throw new LWDataServiceException(errMsg) { ErrorCode = 9977 };
				}
			}
			else
			{
				// invalid status
				string errMsg = string.Format("Invalid status {0} of member with ipcode {1}.", from.MemberStatus.ToString(), from.IpCode);
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg) { ErrorCode = 9977 };
			}

			int gracePeriod = -1;
			if (from.MemberStatus == MemberStatusEnum.NonMember &&
				(options.PointBalance || options.VirtualCards))
			{
				string gracePeriodStr = LWConfigurationUtil.GetConfigurationValue("LW_NonMemberPointGracePeriod");
				if (!string.IsNullOrEmpty(gracePeriodStr))
				{
					gracePeriod = int.Parse(gracePeriodStr);
				}
			}

			_logger.Trace(_className, methodName, string.Format("Merging member {0} to {1}", from.IpCode, to.IpCode));
			if (options.PointBalance && options.VirtualCards)
			{
				string errMsg = "Merge point balance and virtual cards are mutually exclusive options.";
				_logger.Error(_className, methodName, errMsg);
				// throw an exception.  Both cannot be true.
				throw new LWDataServiceException(errMsg) { ErrorCode = 9985 };
			}

			if (options.MemberProfile_Name)
			{
				_logger.Debug(_className, methodName, "Merging member profile names.");
				to.FirstName = from.FirstName;
				to.LastName = from.LastName;
				to.MiddleName = from.MiddleName;
				to.NameSuffix = from.NameSuffix;
				to.NamePrefix = from.NamePrefix;
			}

			if (options.MemberProfile_MailingAddress)
			{
				try
				{
					_logger.Debug(_className, methodName, "Merging member profile mailing address.");
					List<IClientDataObject> fromMdList = from.GetChildAttributeSets("MemberDetails");
					if (fromMdList != null && fromMdList.Count > 0)
					{
						// transfer address details
						IClientDataObject fromDetail = fromMdList[0];
						List<IClientDataObject> toMdList = to.GetChildAttributeSets("MemberDetails");
						if (toMdList != null && toMdList.Count > 0)
						{
							IClientDataObject toDetail = toMdList[0];
							toDetail.SetAttributeValue("AddressLineOne", fromDetail.GetAttributeValue("AddressLineOne"));
							toDetail.SetAttributeValue("AddressLineTwo", fromDetail.GetAttributeValue("AddressLineTwo"));
							toDetail.SetAttributeValue("AddressLineThree", fromDetail.GetAttributeValue("AddressLineThree"));
							toDetail.SetAttributeValue("AddressLineFour", fromDetail.GetAttributeValue("AddressLineFour"));
							toDetail.SetAttributeValue("City", fromDetail.GetAttributeValue("City"));
							toDetail.SetAttributeValue("StateOrProvince", fromDetail.GetAttributeValue("StateOrProvince"));
							toDetail.SetAttributeValue("ZipOrPostalCode", fromDetail.GetAttributeValue("ZipOrPostalCode"));
							toDetail.SetAttributeValue("County", fromDetail.GetAttributeValue("County"));
							toDetail.SetAttributeValue("Country", fromDetail.GetAttributeValue("Country"));
						}
						else
						{
							_logger.Trace(_className, methodName, "From member does not have an address.  New being created.");
						}
					}
					else
					{
						_logger.Debug(_className, methodName, "To member does not have an address to merge.");
					}
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, "Error transfering the mailing address.", ex);
					throw;
				}
			}
			if (options.MemberProfile_PrimaryPhoneNumber)
			{
				to.PrimaryPhoneNumber = from.PrimaryPhoneNumber;
			}
			DateTime? startDate = null;
			if (gracePeriod >= 0)
			{
				startDate = DateTime.Now.AddDays(-1 * gracePeriod);
			}
			using (var transaction = Database.GetTransaction())
			{
				if (options.PointBalance)
				{
					_logger.Debug(_className, methodName, "Transferring point balance from one member to another.");
					decimal totalPoints = 0;
					foreach (VirtualCard vc in from.LoyaltyCards)
					{
						if (vc.Status == VirtualCardStatusType.Active)
						{
							// release any points on hold in the card.
							long[] vcKeys = { vc.VcKey };
							ReleaseAllPoints(vcKeys);
							decimal pb = GetPointBalance(vcKeys, null, null, null, startDate, null, null, null, null, null, null, null, null);
							totalPoints += pb;
							// zero out the card balance
							ExpirePoints(vc, DateTime.Now, PointExpirationReason.MemberMerge, "Card being merged.");
							// cancel all cards of the from member
							vc.NewStatus = VirtualCardStatusType.Cancelled;
							vc.NewStatusEffectiveDate = DateTime.Now;
						}
					}
					if (totalPoints > 0)
					{
						// put a lump sum credit transaction in the to member's primary card
						VirtualCard pv = to.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
						if (pv != null)
						{
							if (pe == null)
							{
								_logger.Error(_className, methodName, "A loyalty event is required to transfer points.");
								throw new LWDataServiceException("A loyalty event is required to transfer points.") { ErrorCode = 9987 };
							}
							if (pt == null)
							{
								_logger.Error(_className, methodName, "A loyalty currency is required to transfer points.");
								throw new LWDataServiceException("A loyalty currency is required to transfer points.") { ErrorCode = 9988 };
							}
							_logger.Debug(_className, methodName,
								string.Format("Transferring {0} points to card with key = {0}", pv.VcKey));
							/*IList<PointTransaction> txns = 
							Credit(pv, pt, pe, totalPoints, string.Empty, DateTime.Now, expDate, null, null);
						}
						else
						{
							_logger.Debug(_className, methodName,
								string.Format("No primary card found to transfer the points to."));
						}
					}
					else
					{
						_logger.Debug(_className, methodName, "No points found to transfer.");
					}
				}
				else if (options.VirtualCards)
				{
					_logger.Debug(_className, methodName, "Merging virtual cards.");
					if (options.FromPrimaryIsNewPrimaryVirtualCard)
					{
						_logger.Debug(_className, methodName, "Making all cards of the to member non-primary.");
						foreach (VirtualCard vc in to.LoyaltyCards)
						{
							vc.IsPrimary = false;
						}
					}
					else
					{
						_logger.Debug(_className, methodName, "Making all cards of the from member non-primary.");
						foreach (VirtualCard vc in from.LoyaltyCards)
						{
							vc.IsPrimary = false;
						}
					}
					// change the ipcode of the from's virtual cards to the to's ipcode.
					long[] vckeys = null;
					if (from.LoyaltyCards.Count > 0)
					{
						vckeys = new long[from.LoyaltyCards.Count];
					}
					int vidx = 0;
					foreach (VirtualCard vc in from.LoyaltyCards)
					{
						vc.IpCode = to.IpCode;
                        to.LoyaltyCards.Add(vc);
						vckeys[vidx++] = vc.VcKey;
					}
                    from.LoyaltyCards.Clear();
					if (vckeys != null && gracePeriod > 0)
					{
						// expire points accrued on the card prior to the startDate.
						ExpirePoints(vckeys, startDate, expDate, PointExpirationReason.MemberMerge, "Expiring points on non-member's card prior to grace period.");
					}
				}

				if (options.MemberTiers)
				{
					_logger.Debug(_className, methodName, "Merging tiers.");
					TierDef fromDef = null;
					TierDef toDef = null;
					MemberTier fromTier = GetMemberTier(from, DateTime.Now);
					if (fromTier != null)
					{
						fromDef = GetTierDef(fromTier.TierDefId);
					}
					MemberTier toTier = GetMemberTier(to, DateTime.Now);
					if (toTier != null)
					{
						toDef = GetTierDef(toTier.TierDefId);
					}
					if (fromDef != null)
					{
						if (toDef == null)
						{
							// surviving member is not in any tier.
							to.MoveTierToMember(fromTier);
						}
						else
						{
							if (toDef.Id == fromDef.Id)
							{
								// they were both in the same tier.  Use the later expiration date.
								toTier.ToDate = DateTimeUtil.LessThan(toTier.ToDate, fromTier.ToDate) ? fromTier.ToDate : toTier.ToDate;
								UpdateMemberTier(toTier);
								// we need to expire the fromTier.
								fromTier.ToDate = DateTime.Now.AddMinutes(-1);
								UpdateMemberTier(fromTier);
							}
							else
							{
								// find which is the higher tier and use that one.
								if (fromDef.EntryPoints > toDef.EntryPoints)
								{
									// from tier is higher.  Move it to the surviving member.
									to.MoveTierToMember(fromTier);
								}
								else
								{
									// to tier is higher.  Just simply expire the from tier.
									fromTier.ToDate = DateTime.Now.AddMinutes(-1);
								}
							}
						}
					}
					else
					{
						// no tier to merge
						_logger.Debug(_className, methodName, string.Format("Member with ipcode = {0} is not in a tier.  Nothing to merge.", from.IpCode));
					}
				}

				// Merge rewards
				if (options.MemberRewards)
				{
					List<MemberReward> rewards = MemberRewardDao.RetrieveUnexpiredAndUnredeemedByMember(from.IpCode);
					if (rewards != null)
					{
						foreach (MemberReward reward in rewards)
						{
							reward.MemberId = to.IpCode;
							UpdateMemberReward(reward);
						}
					}
				}

				if (options.MemberPromotions)
				{
					_logger.Debug(_className, methodName, "Merging promotions.");
					List<MemberPromotion> promoList = MemberPromotionDao.RetrieveByMember(from.IpCode, null, false);
					if (promoList != null && promoList.Count > 0)
					{
						foreach (MemberPromotion promo in promoList)
						{
							promo.MemberId = to.IpCode;
							MemberPromotionDao.Update(promo);
						}
					}
				}

				if (options.MemberBonuses)
				{
					_logger.Debug(_className, methodName, "Merging bonuses.");
					List<MemberBonus> bonusList = MemberBonusDao.RetrieveByMember(from.IpCode, null);
					if (bonusList != null && bonusList.Count > 0)
					{
						foreach (MemberBonus bonus in bonusList)
						{
							bonus.MemberId = to.IpCode;
							MemberBonusDao.Update(bonus);
						}
					}
				}

				if (options.MemberCoupons)
				{
					_logger.Debug(_className, methodName, "Merging coupons.");
					List<MemberCoupon> couponList = MemberCouponDao.RetrieveByMember(from.IpCode, null, false, false, null, null);
					if (couponList != null && couponList.Count > 0)
					{
						foreach (MemberCoupon coupon in couponList)
						{
							coupon.MemberId = to.IpCode;
							MemberCouponDao.Update(coupon);
						}
					}
				}

				#region Manage Last Activity Date
				if (from.LastActivityDate != null && to.LastActivityDate == null)
				{
					to.LastActivityDate = from.LastActivityDate;
				}
				else if (from.LastActivityDate != null && to.LastActivityDate != null)
				{
					if (DateTimeUtil.LessThan(to.LastActivityDate.Value, from.LastActivityDate.Value))
					{
						to.LastActivityDate = from.LastActivityDate;
					}
				}
				#endregion

				//from.NewStatus = MemberStatusEnum.Disabled;
				from.NewStatus = MemberStatusEnum.Merged;
				from.NewStatusEffectiveDate = DateTime.Now;
				from.MergedToMember = to.IpCode;

				ClearMemberFromCache(from);
				ClearMemberFromCache(to);

				SaveMember(from, null, RuleExecutionMode.Real);
				SaveMember(to, null, RuleExecutionMode.Real);

				transaction.Complete();

				return LoadMemberFromIPCode(to.IpCode);
			}
		}

		public void DeleteMember(long ipCode, bool deep)
		{
			const string methodName = "DeleteMember";

			Member member = LoadMemberFromIPCode(ipCode);
			if (member == null)
			{
				_logger.Debug(_className, methodName, string.Format("No member with ipcode {0} could be loaded for deletion.", ipCode));
				return;
			}

			using (var txn = Database.GetTransaction())
			using (var contentService = new ContentService(Config))
			{
				// first delete data from all attribute sets
				List<AttributeSetMetaData> attSets = GetAttributeSetsByType(AttributeSetType.Member);
				if (attSets != null && attSets.Count > 0)
				{
					foreach (AttributeSetMetaData attSet in attSets)
					{
						DeleteAttributeSetData(member, attSet);
					}
				}
				// now remove all attribute sets under virtual card
				attSets = GetAttributeSetsByType(AttributeSetType.VirtualCard);
				if (attSets != null && attSets.Count > 0)
				{
					foreach (AttributeSetMetaData attSet in attSets)
					{
						foreach (VirtualCard vc in member.LoyaltyCards)
						{
							DeleteAttributeSetData(vc, attSet);
						}
					}
				}
				// delete the member itself.
				ClearMemberFromCache(member);
				foreach (VirtualCard vc in member.LoyaltyCards)
				{
					PointTransactionDao.DeleteByVcKey(vc.VcKey);
				}
				List<MemberReward> rewardsList = MemberRewardDao.RetrieveByMember(ipCode, null);
				if (rewardsList != null)
				{
					var certNmbrs = (from x in rewardsList where !string.IsNullOrWhiteSpace(x.CertificateNmbr) select x.CertificateNmbr);
					if (certNmbrs != null && certNmbrs.Count() > 0)
					{
						contentService.ReclaimCertificates(certNmbrs.ToArray<string>());
					}
				}
				MemberRewardDao.DeleteByMember(ipCode);
				MemberOrderDao.DeleteByMember(ipCode);
				MemberTierDao.DeleteByMember(ipCode);
				MemberPromotionDao.DeleteByMember(ipCode);
				MemberCouponDao.DeleteByMember(ipCode);
				MemberBonusDao.DeleteByMember(ipCode);
				MemberMessageDao.DeleteByMember(ipCode);
				MemberSocNetDao.DeleteByMember(ipCode);
				MemberStoreDao.DeleteByMember(ipCode);
				MemberDao.Delete(ipCode);

				txn.Complete();
			}

			_logger.Trace(_className, methodName, "Deleted member with ipcode " + ipCode);
		}

		/// <summary>
		/// This method will delete a member.
		/// </summary>
		/// <param name="ipCode"></param>
		public void DeleteMembers(long[] ipCodes, bool deep)
		{
			using (var transaction = Database.GetTransaction())
			{
				List<Member> members = MemberDao.RetrieveAll(ipCodes, true);
				List<VirtualCard> vcCards = new List<VirtualCard>();
				foreach (Member member in members)
				{
					foreach (VirtualCard vc in member.LoyaltyCards)
					{
						vcCards.Add(vc);
					}
				}

				// first delete data from all attribute sets
				List<AttributeSetMetaData> attSets = GetAttributeSetsByType(AttributeSetType.Member);
				if (attSets != null && attSets.Count > 0)
				{
					foreach (AttributeSetMetaData attSet in attSets)
					{
						DeleteAttributeSetsData(members.ToArray<IAttributeSetContainer>(), attSet);
					}
				}
				// now remove all attribute sets under virtual card
				attSets = GetAttributeSetsByType(AttributeSetType.VirtualCard);
				if (attSets != null && attSets.Count > 0 && vcCards.Count > 0)
				{
					foreach (AttributeSetMetaData attSet in attSets)
					{
						DeleteAttributeSetsData(vcCards.ToArray<IAttributeSetContainer>(), attSet);
					}
				}
				// delete the member itself.
				foreach (Member member in members)
				{
					ClearMemberFromCache(member);
				}

				var vcKeys = (from x in vcCards select x.VcKey);

				PointTransactionDao.DeleteByVcKeys(vcKeys.ToArray<long>());

				MemberRewardDao.DeleteByMembers(ipCodes);
				MemberOrderDao.DeleteByMembers(ipCodes);
				MemberTierDao.DeleteByMembers(ipCodes);
				MemberPromotionDao.DeleteByMembers(ipCodes);
				MemberCouponDao.DeleteByMembers(ipCodes);
				MemberBonusDao.DeleteByMembers(ipCodes);
				MemberMessageDao.DeleteByMembers(ipCodes);
				MemberSocNetDao.DeleteByMembers(ipCodes);
				MemberStoreDao.DeleteByMembers(ipCodes);
				MemberDao.Delete(ipCodes);
                transaction.Complete();
			}
		}


		#endregion

		#region Virtual Cards
		/// <summary>
		/// This method will cancel the specified card.  It expires all the points for this member and if required
		/// then disables the member if this was the last active card. 
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="reason"></param>
		/// <param name="expReason"></param>
		/// <param name="effectiveDate"></param>
		/// <param name="cancelMembership"></param>
		/// <param name="saveMember"></param>
		/// <param name="expirePoints"></param>
		/// <returns></returns>
		private decimal CancelVirtualCard(VirtualCard vc, string reason, PointExpirationReason expReason, DateTime effectiveDate, bool cancelMembership, bool saveMember, bool deactivateCard, bool expirePoints = true)
		{
			const string methodName = "CancelVirtualCard";
			_logger.Trace(_className, methodName, "Cancelling card with loyalty id = " + vc.LoyaltyIdNumber);
			decimal pointsExpired = 0;
			if (vc.Status == VirtualCardStatusType.Active)
			{
				// expire all its points
				if (expirePoints)
				{
					pointsExpired = ExpirePoints(vc, effectiveDate, expReason, reason);
				}
				// set the card status to cancelled
				if (deactivateCard)
				{
					vc.NewStatus = VirtualCardStatusType.InActive;
				}
				else
				{
					vc.NewStatus = VirtualCardStatusType.Cancelled;
				}
				vc.NewStatusEffectiveDate = effectiveDate;
				vc.StatusChangeReason = reason;

				if (cancelMembership)
				{
					bool lastCard = true;
					foreach (VirtualCard c in vc.Member.LoyaltyCards)
					{
						if (c.Status == VirtualCardStatusType.Active)
						{
							lastCard = false;
						}
					}
					if (lastCard)
					{
						vc.Member.NewStatus = MemberStatusEnum.Disabled;
						vc.Member.NewStatusEffectiveDate = effectiveDate;
						// TODO: Expire Tiers, rewards and promotinons
					}
				}
				if (saveMember)
				{
					SaveMember(vc.Member, null, RuleExecutionMode.Real);
				}
			}
			else
			{
				_logger.Error(_className, methodName, "Only active cards can be cancelled.");
			}
			return pointsExpired;
		}

		public decimal CancelVirtualCard(Member m, long vcKey, string reason, DateTime effectiveDate, bool cancelMembership, bool saveMember, bool expirePoints)
		{
			return CancelVirtualCard(m.GetLoyaltyCard(vcKey), reason, PointExpirationReason.Cancellation, effectiveDate, cancelMembership, saveMember, false, expirePoints);
		}

		public decimal CancelVirtualCard(Member m, string loyaltyId, string reason, DateTime effectiveDate, bool cancelMembership, bool saveMember, bool expirePoints)
		{
			return CancelVirtualCard(m.GetLoyaltyCard(loyaltyId), reason, PointExpirationReason.Cancellation, effectiveDate, cancelMembership, saveMember, false, expirePoints);
		}

		/// <summary>
		/// This method will replace the specified card.
		/// </summary>
		/// <param name="oldCard"></param>
		/// <param name="newLoyaltyId"></param>
		/// <param name="effectiveDate"></param>
		/// <param name="transferPoints"></param>
		/// <returns></returns>
		private long ReplaceVirtualCard(
			VirtualCard oldCard,
			string newLoyaltyId,
			DateTime effectiveDate,
			bool transferPoints,
			string replacementReason)
		{
			string methodName = "ReplaceVirtualCard";
			_logger.Trace(_className, methodName, "Replacing card with loyalty id = " + oldCard.LoyaltyIdNumber);

			long pointTypeId = -1;
			long pointEventId = -1;

			if (oldCard.Status != VirtualCardStatusType.Active)
			{
				string errMsg = string.Format("The status of Loyalty Card with id {0} is {1}.  Only Active cards can be replaced.", oldCard.LoyaltyIdNumber, oldCard.Status);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg) { ErrorCode = 3224 };
			}

			using (var transaction = Database.GetTransaction())
			{

				// create the new card if it does not already exist
				VirtualCard newCard = oldCard.Member.GetLoyaltyCard(newLoyaltyId);
				if (newCard == null)
				{
					// make sure that another member does not have this same card
					Member m = LoadMemberFromLoyaltyID(newLoyaltyId);
					if (m != null)
					{
						string errMsg = string.Format("Another member with Ipcode {0} already has a loyalty card with id {1}.",
								m.IpCode, newLoyaltyId);
						_logger.Error(_className, methodName, errMsg);
						throw new LWException(errMsg) { ErrorCode = 9967 };
					}
					newCard = oldCard.Member.CreateNewVirtualCard();
					newCard.CardType = oldCard.CardType;
					newCard.LoyaltyIdNumber = newLoyaltyId;
				}
				if (newCard.VcKey == oldCard.VcKey ||
					newCard.LoyaltyIdNumber == oldCard.LoyaltyIdNumber)
				{
					// throw an exception
					string errMsg = string.Format("You cannot replace a card with itself.");
					_logger.Error(_className, methodName, errMsg);
					throw new LWException(errMsg) { ErrorCode = 3226 };
				}
				oldCard.NewStatus = VirtualCardStatusType.Replaced;
				oldCard.NewStatusEffectiveDate = effectiveDate;
				oldCard.StatusChangeReason = replacementReason;
				if (newCard.VcKey > 0)
				{
					oldCard.LinkKey = newCard.VcKey;
				}
				// Make the new card the primary card if the card being replaced was primary.
				if (oldCard.IsPrimary)
				{
					newCard.IsPrimary = true;
					oldCard.IsPrimary = false;
				}
				SaveMember(oldCard.Member, null, RuleExecutionMode.Real);
				if (oldCard.LinkKey == null || oldCard.LinkKey <= 0)
				{
					oldCard.LinkKey = newCard.VcKey;
					SaveMember(oldCard.Member, null, RuleExecutionMode.Real);
				}
				long[] vcKeys = new long[1] { oldCard.VcKey };
				List<VirtualCard> vcList = new List<VirtualCard>();
				vcList.Add(oldCard);
				if (transferPoints)
				{
					// transfer the points to the new card
					List<PointTransaction> holdTxnList = null;
					List<PointTransaction> transferredTxns = new List<PointTransaction>();
					Dictionary<long, PointTransaction> unconsumedTxnMap = new Dictionary<long, PointTransaction>();
					Dictionary<long, PointTransaction> debitTxnMap = new Dictionary<long, PointTransaction>();
					decimal transferDebitAmount = 0;

					long[] vckeys = new long[] { oldCard.VcKey };
					// First release any points that might be on hold.                               
					decimal onHold = GetPointsOnHold(vcList, null, null, DateTimeUtil.MinValue, DateTimeUtil.MaxValue);
					if (onHold > 0)
					{
						_logger.Trace(_className, methodName, "Releasing all points on hold for VC card " + oldCard.LoyaltyIdNumber);
						holdTxnList = PointTransactionDao.RetrieveOnHoldPointTransactions(vcKeys, null, null, DateTimeUtil.MinValue, DateTimeUtil.MaxValue, null, null, null, null);
						ReleasePointsOnHold(vckeys, null, -1, null);
					}

					decimal balance = GetPointBalance(vckeys, null, null, null, null, null, null, null, null, null, null, null, null);
					if (balance != 0)
					{
						decimal pointsToTransfer = 0;
						// get all unconsumed and unexpired transactions
						List<PointTransaction> txnList = GetTransactionsNotConsumed(oldCard, PointBankTransactionType.Credit);
						if (txnList.Count > 0)
						{
							//Dictionary<long, PointTransaction> txnMap = new Dictionary<long, PointTransaction>();
							long[] pidList = new long[txnList.Count];
							int idx = 0;
							// build pid list for all unconsumed transactions
							foreach (PointTransaction txn in txnList)
							{
								pidList[idx++] = txn.Id;
							}
							// get child debit transactions for all the unconsumed transactions in the pid list
							List<PointTransaction> childTxns = GetPointTransactionsByParents(vcKeys, pidList, PointBankTransactionType.Debit);
							foreach (PointTransaction txn in txnList)
							{
								// for each unconsumed transaction, build a list of its child debits.
								List<PointTransaction> childDebits = new List<PointTransaction>();
								foreach (PointTransaction cTxn in childTxns)
								{
									if (cTxn.ParentTransactionId == txn.Id)
									{
										childDebits.Add(cTxn);
									}
								}
								// calculate the total debit amount for the unconsumed transaction
								decimal debitAmount = 0;
								foreach (PointTransaction childDebit in childDebits)
								{
									debitAmount += childDebit.Points;
									if (!debitTxnMap.ContainsKey(childDebit.Id))
									{
										debitTxnMap.Add(childDebit.Id, childDebit);
									}
								}
								// now calculate the actual amout to transfer for this transaction
								decimal transferAmount = txn.Points - txn.PointsConsumed + debitAmount;
								if (transferAmount > 0)
								{
									// keep track of the unconsumed transaction 
									unconsumedTxnMap.Add(txn.Id, txn.Clone());
									pointsToTransfer += transferAmount;
									txn.ExpirationDate = DateTime.Now;
									txn.ExpirationReason = PointExpirationReason.Replacement;
									UpdatePointTransaction(txn);
									PointTransaction rtxn = CreatePtTxn(oldCard.VcKey, txn.PointTypeId, txn.PointEventId, -transferAmount, string.Empty, PointBankTransactionType.Transferred, DateTime.Now,
										DateTime.Now, DateTime.Now,
										0, string.Empty, PointTransactionOwnerType.Unknown, -1, -1, txn.Id, txn.LocationId, txn.ChangedBy);
									rtxn.ExpirationReason = PointExpirationReason.Replacement;
									UpdatePointTransaction(rtxn);
									// keep track of the transferred transaction created.                                
									transferredTxns.Add(rtxn);
								}
							}
						}
						/*
						 * Returns or reversal can be processed in multiple ways.  The most common way is to enter a 
						 * debit which is linked to the original attribute set.  This case was processed above while 
						 * processing unconsumed transactions.  
						 * Returns or reversal may also be processed by a entering a different attribute set in which
						 * case they need to be processed here.
						 * Finally returns or reversal can be processed as orphan debits.  
						 * 
						 * For all debits, that have not been so far processed, we will enter a lumpsum amount.
						 * 
						PointBankTransactionType[] txnTypes = new PointBankTransactionType[1] { PointBankTransactionType.Debit };
						List<PointTransaction> debits =
							PointTransactionDao.RetrievePointTransactions(vcKeys, null, null, null, null, string.Empty, null, txnTypes, null, null, null, true, false, null);
						if (debits != null && debits.Count > 0)
						{
							foreach (PointTransaction dtxn in debits)
							{
								if (debitTxnMap.ContainsKey(dtxn.Id))
								{
									// this was probably a child debit and has already been processed
									continue;
								}
								debitTxnMap.Add(dtxn.Id, dtxn);
								// this is a non-child debit - either an orphan debit or a debit through reveral
								dtxn.ExpirationDate = DateTime.Now;
								dtxn.ExpirationReason = PointExpirationReason.Replacement;
								UpdatePointTransaction(dtxn);
								decimal comp = Math.Abs(dtxn.Points);
								PointTransaction rtxn = CreatePtTxn(oldCard.VcKey, dtxn.PointTypeId, dtxn.PointEventId, comp, string.Empty, PointBankTransactionType.Transferred, DateTime.Now,
									DateTime.Now, DateTime.Now,
									0, string.Empty, PointTransactionOwnerType.Unknown, -1, -1, dtxn.Id, dtxn.LocationId, dtxn.ChangedBy);
								rtxn.ExpirationReason = PointExpirationReason.Replacement;
								UpdatePointTransaction(rtxn);
								pointsToTransfer += dtxn.Points;
								transferDebitAmount += dtxn.Points;
								// later on we are going to need a point type and point event.  
								// It has to come from somewhere.
								// let us just remember the first one from here.
								if (pointTypeId == -1)
								{
									pointTypeId = dtxn.PointTypeId;
									pointEventId = dtxn.PointEventId;
								}
							}
						}
						// now let us expire all transactions in the old card
						PointTransactionDao.ExpireAllTransactions(vcKeys, null, DateTime.Now, PointExpirationReason.Replacement, string.Empty);
						if (pointsToTransfer != balance)
						{
							string errMsg = string.Format("The amount calculated to transfer {0} does not match up with balance {1}.",
								pointsToTransfer, balance);
							_logger.Error(_className, methodName, errMsg);
							throw new LWException(errMsg);
						}

						if (pointsToTransfer > 0)
						{
							// foreach transferred transaction on the old card, enter a credit transaction on the new card
							_logger.Trace(_className, methodName, "Entering a credit transaction of " + pointsToTransfer + " points.");
							foreach (PointTransaction ttxn in transferredTxns)
							{
								// get the parent transaction it was for
								PointTransaction originalTxn = unconsumedTxnMap[ttxn.ParentTransactionId];
								PointTransaction newCreditTxn =
									CreatePtTxn(newCard.VcKey,
									originalTxn.PointTypeId,
									originalTxn.PointEventId,
									Math.Abs(ttxn.Points),
									originalTxn.PromoCode,
									PointBankTransactionType.Credit,
									DateTime.Now,
									DateTime.Now,
									(DateTime)originalTxn.ExpirationDate, // set the expiration date to the original txn
									0,
									string.Format("Credit transferred from old card with key {0}", oldCard.VcKey),
									originalTxn.OwnerType,
									originalTxn.OwnerId,
									originalTxn.RowKey,
									ttxn.Id,  // set the parent transaction id to the transfer txn on the old card
									originalTxn.LocationId,
									originalTxn.ChangedBy);

								// see if there were any transactions on hold tied to the original transaction
								if (holdTxnList != null)
								{
									foreach (PointTransaction htxn in holdTxnList)
									{
										if (htxn.ParentTransactionId == originalTxn.Id)
										{
											// yes.  Enter an equivalent on hold transaction.
											/*PointTransaction onHoldTxnTxn =
											CreatePtTxn(newCard.VcKey,
											htxn.PointTypeId,
											htxn.PointEventId,
											htxn.Points,
											string.Empty,
											PointBankTransactionType.Hold,
											htxn.TransactionDate,
											htxn.PointAwardDate,
											(DateTime)htxn.ExpirationDate, // set the expiration date to the original txn
											0,
											htxn.Notes,
											htxn.OwnerType,
											htxn.OwnerId,
											htxn.RowKey,
											newCreditTxn.Id,  // set the new credit txn just entered
											htxn.LocationId,
											htxn.ChangedBy);
											// now update the new creaidt txn.
											newCreditTxn.PointsOnHold += htxn.Points;
											UpdatePointTransaction(newCreditTxn);
										}
									}
								}
							}
							// now enter a single orphan debit transaction for the orphan debit transaction found on the
							// old card.
							if (Math.Abs(transferDebitAmount) > 0)
							{
								CreatePtTxn(newCard.VcKey,
									pointTypeId,
									pointEventId,
									transferDebitAmount,
									string.Empty,
									PointBankTransactionType.Debit,
									DateTime.Now,
									DateTime.Now,
									DateTimeUtil.MaxValue,
									0,
									string.Empty,
									PointTransactionOwnerType.Unknown,
									-1,
									-1,
									-1,
									null,
									null);
							}
						}
						else
						{
							// the total amount of points to transfer was negative so a debit lumpsump is entered
							_logger.Trace(_className, methodName, "Entering a debit transaction of " + pointsToTransfer + " points.");
							CreatePtTxn(newCard.VcKey,
								pointTypeId,
								pointEventId,
								pointsToTransfer,
								string.Empty,
								PointBankTransactionType.Debit,
								DateTime.Now,
								DateTime.Now,
								DateTimeUtil.MaxValue,
								0,
								string.Empty,
								PointTransactionOwnerType.Unknown,
								-1,
								-1,
								-1,
								null,
								null);
						}
						ClearMemberFromCache(oldCard.Member);
					}
					else
					{
						_logger.Trace(_className, methodName, "No pints to transfer for VC card " + oldCard.LoyaltyIdNumber);
					}
				}
				else
				{
					// expire all unexpired points on the old card.
					ExpirePoints(oldCard, effectiveDate, PointExpirationReason.Replacement, "Points being expired because of card replacement");
				}
				transaction.Complete();
				return newCard.VcKey;
			}
		}

		public long ReplaceVirtualCard(Member m, long vcKey, string newLoyaltyId, DateTime effectiveDate, bool transferPoints, string replacementReason)
		{
			return ReplaceVirtualCard(m.GetLoyaltyCard(vcKey), newLoyaltyId, effectiveDate, transferPoints, replacementReason);
		}

		public long TransferVirtualCard(VirtualCard fromCard, Member toMember, bool makeCardPrimary, bool cancelMembership)
		{
			string methodName = "TransferVirtualCard";

			_logger.Trace(_className, methodName,
				string.Format("Transferring virtual card with VcKey {0} to member with IpCode = {1}.", fromCard.VcKey, toMember.IpCode));

			if (fromCard.Status != VirtualCardStatusType.Active)
			{
				string errMsg = string.Format("The status of Loyalty Card with id {0} is {1}.  Only Active cards can be transferred.", fromCard.LoyaltyIdNumber, fromCard.Status);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg) { ErrorCode = 3224 };
			}

			Member m = fromCard.Member;
			fromCard.IpCode = toMember.IpCode;
			if (cancelMembership)
			{
				bool lastCard = true;
				foreach (VirtualCard c in fromCard.Member.LoyaltyCards)
				{
					if (c.Status == VirtualCardStatusType.Active && c.VcKey != fromCard.VcKey)
					{
						lastCard = false;
					}
				}
				if (lastCard)
				{
					_logger.Trace(_className, methodName,
						string.Format("Member with ipcode {0} is being disabled because its last active card has been transferred.",
						fromCard.Member.IpCode));
					fromCard.Member.NewStatus = MemberStatusEnum.Disabled;
					fromCard.Member.NewStatusEffectiveDate = DateTime.Now;
					// TODO: Expire Tiers, rewards and promotions
				}
			}
			SaveMember(m, null, RuleExecutionMode.Real);
			ClearMemberFromCache(toMember);
			ClearMemberFromCache(m);
			if (makeCardPrimary)
			{
				Member newMember = LoadMemberFromIPCode(toMember.IpCode);
				foreach (VirtualCard vc in newMember.LoyaltyCards)
				{
					if (vc.VcKey == fromCard.VcKey)
					{
						vc.IsPrimary = true;
					}
					else
					{
						vc.IsPrimary = false;
					}
				}
				SaveMember(newMember, null, RuleExecutionMode.Real);
			}
			return fromCard.VcKey;
		}

		#endregion

		#region Points Summary

		public void CreatePointsSummary(PointsSummary summary)
		{
			PointsSummaryDao.Create(summary);
		}

		public void CreatePointsSummaries(List<PointsSummary> summaries)
		{
			PointsSummaryDao.Create(summaries);
		}

		public void UpdatePointsSummary(PointsSummary summary)
		{
			PointsSummaryDao.Update(summary);
		}

		public PointsSummary RetrievePointsSummary(long id)
		{
			return PointsSummaryDao.Retrieve(id);
		}

		public List<PointsSummary> RetrievePointsSummariesByMember(long memberId)
		{
			return PointsSummaryDao.RetrieveByMember(memberId) ?? new List<PointsSummary>();
		}

		public PointsSummary RetrievePointsSummariesByMember(long memberId, string pointEvent, string pointType)
		{
			return PointsSummaryDao.RetrieveByMember(memberId, pointEvent, pointType);
		}

		public List<PointsSummary> RetrievePointsSummariesByMember(long memberId, string[] pointEvents, string[] pointTypes)
		{
			return PointsSummaryDao.RetrieveByMember(memberId, pointEvents, pointTypes) ?? new List<PointsSummary>();
		}

		public int DeletePointsSummaryByMember(long memberId)
		{
			return PointsSummaryDao.DeleteByMember(memberId);
		}

		public int DeletePointsSummariesByMembers(long[] memberIds)
		{
			return PointsSummaryDao.DeleteByMembers(memberIds);
		}

		#endregion

		#region Point Type

		public void CreatePointType(PointType pt)
		{
			PointTypeDao.Create(pt);
			CacheManager.Update(CacheRegions.PointTypeByName, pt.Name, pt);
		}

		public void UpdatePointType(PointType pt)
		{
			PointTypeDao.Update(pt);
			CacheManager.Update(CacheRegions.PointTypeByName, pt.Name, pt);
		}

		public PointType GetPointType(long pointTypeId)
		{
			PointType ret = PointTypeDao.Retrieve(pointTypeId);
			if (ret != null)
			{
				CacheManager.Update(CacheRegions.PointTypeByName, ret.Name, ret);
			}
			return ret;
		}

		public PointType GetPointType(string pointTypeName)
		{
			var pt = (PointType)CacheManager.Get(CacheRegions.PointTypeByName, pointTypeName);
			if (pt == null)
			{
				pt = PointTypeDao.Retrieve(pointTypeName);
				if (pt != null)
				{
					CacheManager.Update(CacheRegions.PointTypeByName, pt.Name, pt);
				}
			}
			return pt;
		}

		public List<PointType> GetPointTypes(string[] names)
		{
			List<PointType> list = PointTypeDao.Retrieve(names) ?? new List<PointType>();
			foreach (PointType pt in list)
			{
				CacheManager.Update(CacheRegions.PointTypeByName, pt.Name, pt);
			}
			return list;
		}

		public List<PointType> GetAllPointTypes()
		{
            List<PointType> list = (List<PointType>)CacheManager.Get(CacheRegions.PointTypes, "all");

            if (list == null)
            {
                list = PointTypeDao.RetrieveAll() ?? new List<PointType>();
                CacheManager.Update(CacheRegions.PointTypes, "all", list);

                foreach (PointType pt in list)
                {
                    CacheManager.Update(CacheRegions.PointTypeByName, pt.Name, pt);
                }
            }
			return list;
		}

		public virtual decimal GetMemberPointBalance(Member member, DateTime StartDate, DateTime EndDate)
		{
			const string methodName = "GetMemberPointBalance";
			string msg = string.Format("Retrieve points for member with ipcode {0}.", member.IpCode);
			_logger.Debug(_className, methodName, msg);

			decimal PointBalance = 0;
			lock (member.LoyaltyCards)
			{
				foreach (VirtualCard c in member.LoyaltyCards)
				{
					long[] vcKeys = { c.VcKey };
					PointBalance += GetPointBalance(vcKeys, null, null, null, StartDate, EndDate, null, null, null, null, null, null, null);
				}
			}

			msg = string.Format("Member with ipcode {0} has {1} points.", member.IpCode, PointBalance.ToString());
			_logger.Debug(_className, methodName, msg);

			return PointBalance;
		}

		public List<PointType> GetAllChangedPointTypes(DateTime since)
		{
			return PointTypeDao.RetrieveChangedObjects(since) ?? new List<PointType>();
		}

		public void DeletePointType(long ptId)
		{
			PointType pt = GetPointType(ptId);
			if (pt != null)
			{
				/*
				 * RewardDef can have dependency on pointtype name.  However a foreign key cannot be
				 * created because RewardDef can have point type "All" and there is no point type of
				 * all.  This has to be manually checked.
				 * 
				List<RewardDef> rewards = MemberRewardDao.RewardDao.RetrieveRewardDefsByPointType(pt.Name);
				if (rewards != null && rewards.Count > 0)
				{
					throw new LWException("Point type cannot be deleted because it is in use.");
				}
				PointTypeDao.Delete(ptId);
				CacheManager.Remove(CacheRegions.PointTypeByName, pt.Name);

			}
		}

		#endregion

		#region LW Events

		public void CreateLWEvent(LWEvent lwevent)
		{
			LWEventDao.Create(lwevent);
			CacheManager.Update(CacheRegions.EventByName, lwevent.Name, lwevent);
		}

		public void UpdateLWEvent(LWEvent lwevent)
		{
			LWEventDao.Update(lwevent);
			CacheManager.Update(CacheRegions.EventByName, lwevent.Name, lwevent);
		}

		public LWEvent GetLWEvent(long lweventId)
		{
			LWEvent ret = LWEventDao.Retrieve(lweventId);
			if (ret != null)
			{
				CacheManager.Update(CacheRegions.EventByName, ret.Name, ret);
			}
			return ret;
		}

		public LWEvent GetLWEventByName(string lweventName)
		{
			var ret = (LWEvent)CacheManager.Get(CacheRegions.EventByName, lweventName);
			if (ret == null)
			{
				ret = LWEventDao.Retrieve(lweventName);
				if (ret != null)
				{
					CacheManager.Update(CacheRegions.EventByName, ret.Name, ret);
				}
			}
			return ret;
		}

		public List<LWEvent> GetAllChangedLWEvents(DateTime since, bool userDefinedOnly)
		{
			return LWEventDao.RetrieveChangedObjects(since, userDefinedOnly) ?? new List<LWEvent>();
		}

		public List<LWEvent> GetAllLWEvents(bool userDefinedOnly)
		{
			return LWEventDao.RetrieveAll(userDefinedOnly) ?? new List<LWEvent>();
		}

		public void DeleteLWEvent(long lweventId)
		{
			LWEvent lwevent = GetLWEvent(lweventId);
			if (lwevent != null)
			{
				List<RuleTrigger> rules = GetRuleByObjectName(lwevent.Name);
				if (rules != null && rules.Count > 0)
				{
					throw new LWDataServiceException("This event has rules defined under it.  Please delete those rules first.");
				}
				LWEventDao.Delete(lweventId);
			}
		}

		#endregion

		#region Point Event

		public void CreatePointEvent(PointEvent pe)
		{
			PointEventDao.Create(pe);
			CacheManager.Update(CacheRegions.PointEventByName, pe.Name, pe);
		}

		public void UpdatePointEvent(PointEvent pe)
		{
			PointEventDao.Update(pe);
			CacheManager.Update(CacheRegions.PointEventByName, pe.Name, pe);
		}

		public PointEvent GetPointEvent(long pointEventId)
		{
			PointEvent pe = PointEventDao.Retrieve(pointEventId);
			if (pe != null)
			{
				CacheManager.Update(CacheRegions.PointEventByName, pe.Name, pe);
			}
			return pe;
		}

		public PointEvent GetPointEvent(string pointEventName)
		{
			PointEvent pe = (PointEvent)CacheManager.Get(CacheRegions.PointEventByName, pointEventName);
			if (pe == null)
			{
				pe = PointEventDao.Retrieve(pointEventName);
				if (pe != null)
				{
					CacheManager.Update(CacheRegions.PointEventByName, pe.Name, pe);
				}
			}
			return pe;
		}

		public List<PointEvent> GetPointEvents(string[] names)
		{
			List<PointEvent> list = PointEventDao.Retrieve(names);
			if (list != null)
			{
				foreach (PointEvent pe in list)
				{
					CacheManager.Update(CacheRegions.PointEventByName, pe.Name, pe);
				}
			}
			return list ?? new List<PointEvent>();
		}

		public List<PointEvent> GetAllChangedPointEvents(DateTime since)
		{
			return PointEventDao.RetrieveChangedObjects(since) ?? new List<PointEvent>();
		}

		public List<PointEvent> GetAllPointEvents()
		{
            List<PointEvent> list = (List<PointEvent>)CacheManager.Get(CacheRegions.PointEvents, "all");

            if (list == null)
            {
                list = PointEventDao.RetrieveAll();
                CacheManager.Update(CacheRegions.PointEvents, "all", list);
                if (list != null)
                {
                    foreach (PointEvent pe in list)
                    {
                        CacheManager.Update(CacheRegions.PointEventByName, pe.Name, pe);
                    }
                }
            }
			return list ?? new List<PointEvent>();
		}

		public void DeletePointEvent(long peId)
		{
			PointEvent pe = GetPointEvent(peId);
			if (pe != null)
			{
				/*
				 * RewardDef can have dependency on pointevent name.  However a fereign key cannot be
				 * created because RewardDef can have point type "All" and there is no point type of
				 * all.  This has to be manually checked.
				 * 
				using (var contentService = LWDataServiceUtil.ContentServiceInstance(Organization, Environment))
				{
					List<RewardDef> rewards = contentService.RewardDao.RetrieveRewardDefsByPointEvent(pe.Name);
					if (rewards != null && rewards.Count > 0)
					{
						throw new LWException("Point event cannot be deleted because it is in use.");
					}
				}
				PointEventDao.Delete(peId);
				CacheManager.Remove(CacheRegions.PointEventByName, pe.Name);
			}
		}

		#endregion

		#region Point Bank Transactions

		#region General

		public void CreatePointTransaction(PointTransaction txn)
		{
			PointTransactionDao.Create(txn);
		}

		public void CreatePointTransactions(List<PointTransaction> txnList)
		{
			PointTransactionDao.Create(txnList);
		}

		public void UpdatePointTransaction(PointTransaction txn)
		{
			PointTransactionDao.Update(txn);
		}

        public void UpdatePointTransactions(List<PointTransaction> txnList)
        {
            PointTransactionDao.Update(txnList);
        }

		public long HowManyPointTransactions(Member member, long[] pointTypeIds, long[] pointEventIds, DateTime? startDate, DateTime? endDate, bool includeExpired)
		{
			const string methodName = "HowManyPointTransactions";
			if (member.LoyaltyCards == null || member.LoyaltyCards.Count == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}
			long[] vcKeys = new long[member.LoyaltyCards.Count];
			int idx = 0;
			foreach (VirtualCard vc in member.LoyaltyCards)
			{
				vcKeys[idx++] = vc.VcKey;
			}
			return PointTransactionDao.HowMany(vcKeys, pointTypeIds, pointEventIds, startDate, endDate, includeExpired);
		}

		public long HowManyPointTransactions(long[] vcKeys, long[] pointTypeIds, long[] pointEventIds, DateTime? startDate, DateTime? endDate, bool includeExpired)
		{
			const string methodName = "HowManyPointTransactions";
			if (vcKeys == null || vcKeys.Length == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}
			return PointTransactionDao.HowMany(vcKeys, pointTypeIds, pointEventIds, startDate, endDate, includeExpired);
		}

		public List<PointTransaction> GetPointTransactionsByPointTypePointEvent(
			Member member,
			DateTime? from,
			DateTime? to,
			PointBankTransactionType[] txnTypes,
			long[] pointTypeIds,
			long[] pointEventIds,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long[] rowkeys,
			bool includeExpired,
			LWQueryBatchInfo batchInfo)
		{
			const string methodName = "GetPointTransactionsByPointTypePointEvent";

			if (member.LoyaltyCards == null || member.LoyaltyCards.Count == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg) { ErrorCode = 3230 };
			}

			if (batchInfo != null) batchInfo.Validate();

			long[] vcKeys = new long[member.LoyaltyCards.Count];
			int idx = 0;
			foreach (VirtualCard vc in member.LoyaltyCards)
			{
				vcKeys[idx++] = vc.VcKey;
			}
			try
			{
				return GetPointTransactionsByPointTypePointEvent(vcKeys, from, to, txnTypes, pointTypeIds, pointEventIds, ownerType, ownerId, rowkeys, includeExpired, batchInfo);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error getting point transactions.", ex);
				throw;
			}
		}

		public List<PointTransaction> GetPointTransactionsByPointTypePointEvent(
			long[] vcKeys,
			DateTime? from,
			DateTime? to,
			PointBankTransactionType[] txnTypes,
			long[] pointTypeIds,
			long[] pointEventIds,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long[] rowkeys,
			bool includeExpired,
			LWQueryBatchInfo batchInfo)
		{
			string methodName = "GetPointTransactionsByPointTypePointEvent";

			if (vcKeys == null || vcKeys.Length == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}

			if (batchInfo != null) batchInfo.Validate();

			if (txnTypes == null)
			{
				txnTypes = new PointBankTransactionType[] { PointBankTransactionType.Credit, PointBankTransactionType.Debit, PointBankTransactionType.Hold, PointBankTransactionType.Consumed, PointBankTransactionType.Transferred };
			}

			return
                //LW3856 Chaing this order to descending so that it matches the direction of TxnHeader
				PointTransactionDao.RetrievePointTransactions(vcKeys, pointTypeIds, pointEventIds, from, to, string.Empty, null, txnTypes, ownerType, ownerId, rowkeys, true, includeExpired, batchInfo)
				??
				new List<PointTransaction>();
		}

		public List<PointTransaction> GetExpiredPointTransactionsWithNoExpiredRecords(Member member, LWQueryBatchInfo batchInfo)
		{
			const string methodName = "GetExpiredPointTransactionsWithNoExpiredRecords";
			if (member.LoyaltyCards == null || member.LoyaltyCards.Count == 0)
			{
				string errMsg = "The provided member has no virtual cards.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}
			var vcKeys = (from v in member.LoyaltyCards select v.VcKey);
			if (batchInfo != null) batchInfo.Validate();
			return PointTransactionDao.RetrieveExpiredPointTransactionsWithNoExpiredRecords(vcKeys.ToArray<long>(), batchInfo) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetPointTransactions(Member member, DateTime? from, DateTime? to, string promoCode, string exclusionList, int? startIndex, int? batchSize, bool includeExpired)
		{
			const string methodName = "GetPointTransactions";
			if (member.LoyaltyCards == null || member.LoyaltyCards.Count == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}
			long[] vcKeys = new long[member.LoyaltyCards.Count];
			int idx = 0;
			foreach (VirtualCard vc in member.LoyaltyCards)
			{
				vcKeys[idx++] = vc.VcKey;
			}
			List<long> pointTypeIds = new List<long>();
			if (!string.IsNullOrEmpty(exclusionList))
			{
				List<PointType> ptList = GetAllPointTypes();
				string[] tokens = exclusionList.Split(';');
				foreach (PointType pt in ptList)
				{
					bool toExclude = false;
					for (int i = 0; i < tokens.Length; i++)
					{
						if (pt.Name == tokens[i])
						{
							toExclude = true;
							break;
						}
						if (!toExclude)
						{
							pointTypeIds.Add(pt.ID);
						}
					}
				}
			}
			long[] ptIds = pointTypeIds.ToArray<long>();
			LWQueryBatchInfo batchInfo = null;
			if (startIndex != null && batchSize != null)
			{
				batchInfo = new LWQueryBatchInfo() { StartIndex = startIndex.Value, BatchSize = batchSize.Value };
				batchInfo.Validate();
			}

			PointBankTransactionType[] txnTypes = new PointBankTransactionType[] { PointBankTransactionType.Credit, PointBankTransactionType.Debit, PointBankTransactionType.Hold, PointBankTransactionType.Consumed, PointBankTransactionType.Transferred };

			return PointTransactionDao.RetrievePointTransactions(vcKeys, ptIds, null, from, to, promoCode, null, txnTypes, null, null, null, true, includeExpired, batchInfo) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetPointTransactions(VirtualCard vc, DateTime? from, DateTime? to, string promoCode, string exclusionList, bool includeExpired)
		{
			return GetPointTransactions(vc, from, to, promoCode, exclusionList, true, includeExpired);
		}

		public List<PointTransaction> GetPointTransactions(VirtualCard vc, DateTime? from, DateTime? to, string promoCode, string exclusionList, bool orderDateDescending, bool includeExpired)
		{
			List<long> pointTypeIds = new List<long>();
			if (!string.IsNullOrEmpty(exclusionList))
			{
				List<PointType> ptList = GetAllPointTypes();
				string[] tokens = exclusionList.Split(';');
				foreach (PointType pt in ptList)
				{
					bool toExclude = false;
					for (int i = 0; i < tokens.Length; i++)
					{
						if (pt.Name == tokens[i])
						{
							toExclude = true;
							break;
						}
						if (!toExclude)
						{
							pointTypeIds.Add(pt.ID);
						}
					}
				}
			}
			long[] ptIds = pointTypeIds.ToArray<long>();
			long[] vcKeys = new long[1] { vc.VcKey };

			PointBankTransactionType[] txnTypes = new PointBankTransactionType[] { PointBankTransactionType.Credit, PointBankTransactionType.Debit, PointBankTransactionType.Hold, PointBankTransactionType.Consumed, PointBankTransactionType.Transferred };

			return PointTransactionDao.RetrievePointTransactions(vcKeys, ptIds, null, from, to, promoCode, null, txnTypes, null, null, null, true, includeExpired, null) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetPointTransactionsByParents(long[] vcKeys, long[] txnIds, PointBankTransactionType? txnType)
		{
			const string methodName = "GetPointTransactionsByParents";

			if (txnIds == null || txnIds.Length == 0)
			{
				string errMsg = "No parent transactions provided for provided for point transactions.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}

			PointBankTransactionType[] txnTypes = null;
			if (txnType != null)
			{
				txnTypes = new PointBankTransactionType[1] { (PointBankTransactionType)txnType };
			}
			return PointTransactionDao.RetrievePointTransactions(vcKeys, null, null, null, null, string.Empty, txnIds, txnTypes, null, null, null, true, false, null) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetPointTransactionsByOwner(long[] vckeys, PointBankTransactionType[] txnTypes, PointTransactionOwnerType? ownerType, long? ownerId, long[] rowkeys, bool includeExpired)
		{
			return PointTransactionDao.RetrievePointTransactions(vckeys, null, null, null, null, string.Empty, null, txnTypes, ownerType, ownerId, rowkeys, true, includeExpired, null) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetTransactionsNotConsumed(VirtualCard vc, PointType pt, PointEvent pe, PointBankTransactionType txnType)
		{
			long[] vcKeys = new long[] { vc.VcKey };
			long[] ptIds = new long[] { pt.ID };
			long[] peIds = new long[] { pe.ID };
			return PointTransactionDao.RetrieveTransactionsNotConsumed(vcKeys, ptIds, peIds, txnType) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetTransactionsNotConsumed(VirtualCard vc, PointType pt, PointBankTransactionType txnType) // LW-1301
		{
			long[] vcKeys = new long[] { vc.VcKey };
			long[] ptIds = new long[] { pt.ID };
			return PointTransactionDao.RetrieveTransactionsNotConsumed(vcKeys, ptIds, null, txnType) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetTransactionsNotConsumed(VirtualCard vc, PointEvent pe, PointBankTransactionType txnType) // LW-1301
		{
			long[] vcKeys = new long[] { vc.VcKey };
			long[] peIds = new long[] { pe.ID };
			return PointTransactionDao.RetrieveTransactionsNotConsumed(vcKeys, null, peIds, txnType) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetTransactionsNotConsumed(VirtualCard vc, PointBankTransactionType txnType)
		{
			long[] vcKeys = new long[] { vc.VcKey };
			return PointTransactionDao.RetrieveTransactionsNotConsumed(vcKeys, null, null, txnType) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetTransactionsNotConsumed(long[] vcs, long[] pts, long[] pes, PointBankTransactionType txnType)
		{
			const string methodName = "GetTransactionsNotConsumed";
			if (vcs == null || vcs.Length == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}
			return PointTransactionDao.RetrieveTransactionsNotConsumed(vcs, pts, pes, txnType) ?? new List<PointTransaction>();
		}

		public PointTransaction GetPointTransaction(long txnId)
		{
			return PointTransactionDao.Retrieve(txnId);
		}

		public List<PointTransaction> GetOnHoldPointTransactions(
			IList<VirtualCard> vcList,
			IList<PointType> pointTypes,
			IList<PointEvent> pointEvents,
			DateTime? from,
			DateTime? to,
			long[] parentIds,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long? rowkey)
		{
			const string methodName = "GetPointsOnHold";

			if (vcList == null || vcList.Count == 0)
			{
				_logger.Error(_className, methodName, "No virtual cards specified for get points on hold");
				throw new LWException("No virtual cards specified for points on hold");
			}

			Dictionary<long, VirtualCard> vcMap = new Dictionary<long, VirtualCard>();
			List<VirtualCard> validCards = new List<VirtualCard>();
			foreach (VirtualCard v in vcList)
			{
				if (v.IsValid())
				{
					validCards.Add(v);
					vcMap.Add(v.VcKey, v);
				}
			}
			long[] vcs = new long[validCards.Count];
			int idx = 0;
			foreach (VirtualCard v in validCards)
			{
				vcs[idx++] = v.VcKey;
			}
			long[] pts = null;
			if (pointTypes != null && pointTypes.Count > 0)
			{
				pts = new long[pointTypes.Count];
				idx = 0;
				foreach (PointType p in pointTypes)
				{
					pts[idx++] = p.ID;
				}
			}
			long[] pes = null;
			if (pointEvents != null && pointEvents.Count > 0)
			{
				pes = new long[pointEvents.Count];
				idx = 0;
				foreach (PointEvent p in pointEvents)
				{
					pes[idx++] = p.ID;
				}
			}
			return PointTransactionDao.RetrieveOnHoldPointTransactions(vcs, pts, pes, from, to, parentIds, ownerType, ownerId, rowkey) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetAllUnexpiredPointTransactions(VirtualCard vc)
		{
			long[] vcKeys = new long[] { vc.VcKey };
			PointBankTransactionType[] ttypes = new PointBankTransactionType[] { PointBankTransactionType.Credit, PointBankTransactionType.Debit, PointBankTransactionType.Hold, PointBankTransactionType.Consumed, PointBankTransactionType.Transferred };
			return PointTransactionDao.RetrieveUnexpiredTransactions(vcKeys, null, null, ttypes, null, null, null, null) ?? new List<PointTransaction>();
		}

		public List<PointTransaction> GetAllPointTransactions()
		{
			return PointTransactionDao.RetrieveAll() ?? new List<PointTransaction>();
		}

		public void DeletePointTransaction(long txnId)
		{
			PointTransactionDao.Delete(txnId);
		}

		public int DeletePointTransactionRowKey(long rowKey)
		{
			return PointTransactionDao.DeleteByRowKey(rowKey);
		}

        public decimal ConsumePoints(
			VirtualCard vc,
			PointType pt,
			PointEvent pe,
			DateTime consumptionDate,
			decimal points,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			string notes,
			string locationId,
			string changedBy)
		{
			string methodName = "ConsumePoints";
			string msg = string.Format("Consume points for virtual card {0}.", vc.VcKey);
			_logger.Trace(_className, methodName, msg);

			if (vc.Member.MemberStatus == MemberStatusEnum.NonMember)
			{
				msg = "Cannot consume points for a non-member.";
				_logger.Error(_className, methodName, msg);
				throw new LWDataServiceException(msg) { ErrorCode = 9970 };
			}

			if (!vc.IsValid())
			{
				_logger.Error(_className, methodName, string.Format("Status {0} for card with loyalty id {1} is not valid for point consumption.", vc.Status, vc.LoyaltyIdNumber));
				return 0;
			}

			//Get points that haven't been consumed...these are returned with the                    
			List<PointTransaction> trxs = GetTransactionsNotConsumed(vc, pt, pe, PointBankTransactionType.Credit);

			decimal totalAvailableBalance = trxs.Sum(x => x.Points - x.PointsConsumed - x.PointsOnHold);

            if (totalAvailableBalance < points)
			{
				string err = string.Format("Insufficient point balance.  Available balance = {0} while required points = {1}.",
					totalAvailableBalance, points);
				_logger.Error(_className, methodName, err);
				return 0;
			}

            decimal totalConsumed = 0;
            List<PointTransaction> txnsToUpdate = new List<PointTransaction>();
            List<PointTransaction> txnsToCreate = new List<PointTransaction>();
            foreach (PointTransaction trx in trxs)
            {
                if (trx.TransactionType != PointBankTransactionType.Credit)
                    continue;

                //Break as soon as all the necessary points have been consumed
                if (totalConsumed >= points)
                    break;

                decimal pointsLeftToConsume = points - totalConsumed;
                decimal pointsForDebit = trx.Points - trx.PointsConsumed - trx.PointsOnHold;
                decimal debitAmount = pointsLeftToConsume < pointsForDebit ? pointsLeftToConsume : pointsForDebit;
                if (debitAmount > 0)
                {
                    DateTime expDate = trx.ExpirationDate != null ? (DateTime)trx.ExpirationDate : DateTimeUtil.MaxValue;
                    PointTransaction txn = new PointTransaction()
                    {
                        VcKey = trx.VcKey,
                        PointTypeId = trx.PointTypeId,
                        PointEventId = trx.PointEventId,
                        TransactionType = PointBankTransactionType.Consumed,
                        TransactionDate = consumptionDate,
                        PointAwardDate = DateTime.Now,
                        Points = -debitAmount,
                        ExpirationDate = expDate,
                        Notes = notes,
                        PromoCode = string.Empty,
                        OwnerType = ownerType,
                        OwnerId = ownerId,
                        PointsConsumed = 0,
                        RowKey = rowKey,
                        ParentTransactionId = trx.Id,
                        LocationId = locationId,
                        ChangedBy = changedBy
                    };
                    txnsToCreate.Add(txn);
                    trx.PointsConsumed += debitAmount;
                    txnsToUpdate.Add(trx);
                    totalConsumed = totalConsumed + debitAmount;
                }
            }
            CreatePointTransactions(txnsToCreate);
            UpdatePointTransactions(txnsToUpdate);
			return totalConsumed;
		}

		/// <summary>
		/// This method is called to consume points on the list of cirtual cards that are
		/// provided and for the list of point types and point events that are provided.
		/// </summary>
		/// <param name="vcList"></param>
		/// <param name="pt"></param>
		/// <param name="pe"></param>
		/// <param name="consumptionDate"></param>
		/// <param name="expirationDate"></param>
		/// <param name="points"></param>
		/// <param name="ownerType"></param>
		/// <param name="ownerId"></param>
		/// <param name="rowKey"></param>
		/// <returns></returns>
		public decimal ConsumePoints(
			IList<VirtualCard> vcList,
			IList<PointType> pt,
			IList<PointEvent> pe,
			DateTime consumptionDate,
			//DateTime expirationDate, 
			decimal points,
			PointTransactionOwnerType ownerType,
			string notes,
			long ownerId,
			long rowKey)
		{
			string methodName = "ConsumePoints";

			decimal totalConsumed = 0;

			if (vcList == null || vcList.Count == 0)
			{
				_logger.Error(_className, methodName, "No virtual cards specified for point consumption");
				throw new LWException("No virtual cards specified for point consumption");
			}

			if (((Member)vcList[0].Member).MemberStatus == MemberStatusEnum.NonMember)
			{
				string msg = "Cannot consume points for a non-member.";
				_logger.Error(_className, methodName, msg);
				throw new LWDataServiceException(msg) { ErrorCode = 9970 };
			}

			if (pt == null || pt.Count == 0)
			{
				_logger.Error(_className, methodName, "No point types specified for point consumption");
				throw new LWException("No point types specified for point consumption");
			}

			if (pe == null || pe.Count == 0)
			{
				_logger.Error(_className, methodName, "No point types specified for point consumption");
				throw new LWException("No point types specified for point consumption");
			}

			Dictionary<long, VirtualCard> vcMap = new Dictionary<long, VirtualCard>();
			List<VirtualCard> validCards = new List<VirtualCard>();
			foreach (VirtualCard v in vcList)
			{
				if (!v.IsValid())
				{
					_logger.Error(_className, methodName, string.Format("Card status for card with loyalty id {0} is not valid for point consumption.", v.Status));
					//return 0;
				}
				else
				{
					validCards.Add(v);
					vcMap.Add(v.VcKey, v);
				}
			}
			long[] vcs = new long[validCards.Count];
			int idx = 0;
			foreach (VirtualCard v in validCards)
			{
				vcs[idx++] = v.VcKey;
			}
			long[] pts = new long[pt.Count];
			idx = 0;
			foreach (PointType p in pt)
			{
				pts[idx++] = p.ID;
			}
			long[] pes = new long[pe.Count];
			idx = 0;
			foreach (PointEvent p in pe)
			{
				pes[idx++] = p.ID;
			}
			//Get points that haven't been consumed...these are returned with the                    
			List<PointTransaction> trxs = GetTransactionsNotConsumed(vcs, pts, pes, PointBankTransactionType.Credit);

			decimal totalAvailableBalance = trxs.Sum(x => x.Points - x.PointsConsumed - x.PointsOnHold);

			if (totalAvailableBalance < points)
			{
				string err = string.Format("Insufficient point balance.  Available balance = {0} while required points = {1}.",
					totalAvailableBalance, points);
				_logger.Error(_className, methodName, err);
				return 0;
			}

            List<PointTransaction> txnsToUpdate = new List<PointTransaction>();
            List<PointTransaction> txnsToCreate = new List<PointTransaction>();
			foreach (PointTransaction trx in trxs)
			{
				if (trx.TransactionType != PointBankTransactionType.Credit)
					continue;

				//Break as soon as all the necessary points have been consumed
				if (totalConsumed >= points)
					break;

				decimal pointsLeftToConsume = points - totalConsumed;
                decimal pointsForDebit = trx.Points - trx.PointsConsumed - trx.PointsOnHold;
                decimal debitAmount = pointsLeftToConsume < pointsForDebit ? pointsLeftToConsume : pointsForDebit;
                if (debitAmount > 0)
                {
                    DateTime expDate = trx.ExpirationDate != null ? (DateTime)trx.ExpirationDate : DateTimeUtil.MaxValue;
                    PointTransaction txn = new PointTransaction()
                    {
                        VcKey = trx.VcKey,
                        PointTypeId = trx.PointTypeId,
                        PointEventId = trx.PointEventId,
                        TransactionType = PointBankTransactionType.Consumed,
                        TransactionDate = consumptionDate,
                        PointAwardDate = DateTime.Now,
                        Points = -debitAmount,
                        ExpirationDate = expDate,
                        Notes = notes,
                        PromoCode = string.Empty,
                        OwnerType = ownerType,
                        OwnerId = ownerId,
                        PointsConsumed = 0,
                        RowKey = rowKey,
                        ParentTransactionId = trx.Id,
                        LocationId = trx.LocationId,
                        ChangedBy = trx.ChangedBy
                    };
                    txnsToCreate.Add(txn);
                    trx.PointsConsumed += debitAmount;
                    txnsToUpdate.Add(trx);
                    totalConsumed = totalConsumed + debitAmount;
                }
			}
            CreatePointTransactions(txnsToCreate);
            UpdatePointTransactions(txnsToUpdate);

            return totalConsumed;
		}

		#endregion

		#region Point Bank

		private PointTransaction CreatePtTxn(
			long vcKey,
			long ptId,
			long peId,
			decimal points,
			string promoCode,
			PointBankTransactionType type,
			DateTime pointsTransactionDate,
			DateTime awardDate,
			DateTime pointsExpirationDate,
			decimal pointsConsumed,
			string notes,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			long parentTxnKey,
			string locationId,
			string changedBy)
		{
			PointTransaction txn = new PointTransaction();

			txn.VcKey = vcKey;
			txn.PointTypeId = ptId;
			txn.PointEventId = peId;
			txn.TransactionType = type;
			txn.TransactionDate = pointsTransactionDate;
			txn.PointAwardDate = awardDate;
			txn.Points = points;
			txn.ExpirationDate = pointsExpirationDate;
			txn.Notes = notes;
			txn.PromoCode = promoCode;
			txn.OwnerType = ownerType;
			txn.OwnerId = ownerId;
			txn.PointsConsumed = pointsConsumed;
			txn.RowKey = rowKey;
			txn.ParentTransactionId = parentTxnKey;
			txn.LocationId = locationId;
			txn.ChangedBy = changedBy;

			CreatePointTransaction(txn);
			return txn;
		}

		public decimal ConsumePoints(
			long vcKey,
			PointTransaction trx,
			decimal pointsToConsume,
			PointType pt,
			PointEvent pe,
			DateTime consumptionDate,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			string notes,
			string locationId,
			string changedBy)
		{
			decimal pointsForDebit = trx.Points - trx.PointsConsumed - trx.PointsOnHold;
			if (pointsForDebit <= 0)
			{
				return 0;
			}
			//Need to determine how much to debit per transaction entry
			decimal debitAmount = 0;
			if (pointsToConsume < pointsForDebit)
				debitAmount = pointsToConsume;
			else
				debitAmount = pointsForDebit;

			if (debitAmount > 0)
			{
				DateTime expDate = trx.ExpirationDate != null ? (DateTime)trx.ExpirationDate : DateTimeUtil.MaxValue;
				CreatePtTxn(vcKey, pt.ID, pe.ID, -debitAmount, string.Empty, PointBankTransactionType.Consumed, consumptionDate,
					DateTime.Now, expDate,
					0, notes, ownerType, ownerId, rowKey, trx.Id, locationId, changedBy);
				// update the transaction that we just consumed.
				trx.PointsConsumed += debitAmount;
				//Update this point transaction                
				UpdatePointTransaction(trx);
			}
			return debitAmount;
		}

		private List<PointTransaction> CreditWithDebtPayoff(
			VirtualCard vc,
			PointType pt,
			PointEvent pe,
			decimal points,
			string promoCode,
			DateTime pointsTransactionDate,
			DateTime pointsExpirationDate,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			string notes,
			string locationId,
			string changedBy)
		{
			List<PointTransaction> txns = new List<PointTransaction>();

			//Get debit point transactions that have not been paid yet                    
			List<PointTransaction> trxs = GetTransactionsNotConsumed(vc, pt, pe, PointBankTransactionType.Debit);
			decimal totalCreditLeft = points;
			foreach (PointTransaction trx in trxs)
			{
				if (totalCreditLeft <= 0)
				{
					break;
				}
				// pay this debit off
				decimal unpaid = Math.Abs(trx.Points) - trx.PointsConsumed;
				if (unpaid > 0)
				{
					// need to pay off this debt.
					DateTime expDate = trx.ExpirationDate != null ? (DateTime)trx.ExpirationDate : pointsExpirationDate;
					decimal amount = totalCreditLeft <= unpaid ? totalCreditLeft : unpaid;
					PointTransaction txn = CreatePtTxn(vc.VcKey, pt.ID, pe.ID, amount, promoCode, PointBankTransactionType.Credit,
						pointsTransactionDate, DateTime.Now, expDate, amount, notes, ownerType, ownerId, rowKey, trx.Id, locationId, changedBy);
					trx.PointsConsumed += amount;
					UpdatePointTransaction(trx);
					totalCreditLeft -= amount;
					txns.Add(txn);
				}
			}
			if (totalCreditLeft > 0)
			{
				PointTransaction txn = CreatePtTxn(vc.VcKey, pt.ID, pe.ID, totalCreditLeft, promoCode, PointBankTransactionType.Credit,
					pointsTransactionDate, DateTime.Now, pointsExpirationDate, 0, notes, ownerType, ownerId, rowKey, -1, locationId, changedBy);
				txns.Add(txn);
			}
			return txns;
		}

		private List<PointTransaction> DebitWithCreditConsumption(
			VirtualCard vc,
			PointType pt,
			PointEvent pe,
			decimal points,
			DateTime transactionDate,
			DateTime expDate,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			string notes,
			string locationId,
			string changedBy,
            string promoCode)
		{
			List<PointTransaction> txns = new List<PointTransaction>();

			//Get debit point transactions that have not been paid yet
			List<PointTransaction> trxs = new List<PointTransaction>();

			if (Config.DebitPayOffPointTypeRestrictionOn && Config.DebitPayOffPointEventRestrictionOn)
			{
				trxs = GetTransactionsNotConsumed(vc, pt, pe, PointBankTransactionType.Credit);
			}
			else if (Config.DebitPayOffPointTypeRestrictionOn && !Config.DebitPayOffPointEventRestrictionOn)
			{
				trxs = GetTransactionsNotConsumed(vc, pt, PointBankTransactionType.Credit);
			}
			else if (!Config.DebitPayOffPointTypeRestrictionOn && Config.DebitPayOffPointEventRestrictionOn)
			{
				trxs = GetTransactionsNotConsumed(vc, pe, PointBankTransactionType.Credit);
			}
			else
			{
				trxs = GetTransactionsNotConsumed(vc, PointBankTransactionType.Credit);
			}
			decimal totalDebitLeft = points;
			foreach (PointTransaction trx in trxs)
			{
				if (totalDebitLeft <= 0)
				{
					break;
				}
				// pay this debit off
				decimal unpaid = Math.Abs(trx.Points) - trx.PointsConsumed;
				if (unpaid > 0)
				{
					// need to consume this credit to payoff debt.
					DateTime dexpDate = trx.ExpirationDate != null ? (DateTime)trx.ExpirationDate : expDate; // LW-971
					decimal amount = totalDebitLeft <= unpaid ? totalDebitLeft : unpaid;
					PointTransaction txn = CreatePtTxn(vc.VcKey, pt.ID, pe.ID, -amount, promoCode, PointBankTransactionType.Debit,
						transactionDate, DateTime.Now, dexpDate, amount, notes, ownerType, ownerId, rowKey, trx.Id, locationId, changedBy);
					trx.PointsConsumed += amount;
					UpdatePointTransaction(trx);
					totalDebitLeft -= amount;
					txns.Add(txn);
				}
			}
			if (totalDebitLeft > 0)
			{
				PointTransaction txn = CreatePtTxn(vc.VcKey, pt.ID, pe.ID, -totalDebitLeft, promoCode, PointBankTransactionType.Debit,
					transactionDate, DateTime.Now, expDate, 0, notes, ownerType, ownerId, rowKey, -1, locationId, changedBy);
				txns.Add(txn);
			}
			return txns;
		}

		public decimal ConsumePoints(
			Member member,
			PointType pt,
			PointEvent pe,
			DateTime consumptionDate,
			DateTime expirationDate,
			decimal points,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			string locationId,
			string changedBy)
		{
			return member.ConsumePoints(pt, pe, consumptionDate, expirationDate, points, ownerType, ownerId, rowKey, locationId, changedBy);
		}

		public decimal ConsumePoints(
			Member member,
			PointType pt,
			PointEvent pe,
			DateTime consumptionDate,
			decimal points,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			string locationId,
			string changedBy)
		{
			return member.ConsumePoints(pt, pe, consumptionDate, points, ownerType, ownerId, rowKey, locationId, changedBy);
		}

		public void HoldAllPoints(Member member, DateTime transactionDate, string notes)
		{
			member.HoldAllPoints(transactionDate, notes);
		}

		public void HoldPoints(Member member, PointType pt, PointEvent pe, decimal points, DateTime transactionDate,
			PointTransactionOwnerType ownerType, long ownerId, long rowKey, string notes, string locationId, string changedBy)
		{
			member.HoldPoints(pt, pe, points, transactionDate, ownerType, ownerId, rowKey, notes, locationId, changedBy);
		}

		public void ExpirePoints(Member member, string notes, PointExpirationReason reason)
		{
			member.ExpirePoints(notes, reason);
		}

		public void ExtendPointsExpirationDate(Member member, DateTime from, DateTime to, DateTime newExpiryDate)
		{
			member.ExtendPointsExpirationDate(from, to, newExpiryDate);
		}

		public List<PointTransaction> Credit(
			VirtualCard vc,
			PointType pt,
			PointEvent pe,
			decimal points,
			string promoCode,
			DateTime pointsTransactionDate,
			DateTime pointsExpirationDate,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			string notes,
			string locationId,
			string changedBy)
		{
			if (!vc.IsValid())
			{
				throw new Brierley.FrameWork.Common.Exceptions.LWException(string.Format("Cannot credit points to card {0} in invalid state.", vc.LoyaltyIdNumber));
			}
			List<PointTransaction> txns = new List<PointTransaction>();
			if (!Config.DebitPayOffMethodOn)
			{
				// the old regular method which is also default
				_logger.Trace(_className, "Credit", string.Format("Crediting {0} points to virtual card {1}.", points, vc.VcKey));
				PointTransaction txn = CreatePtTxn(vc.VcKey, pt.ID, pe.ID, points, promoCode, PointBankTransactionType.Credit,
					pointsTransactionDate, DateTime.Now, pointsExpirationDate, 0, notes, ownerType, ownerId, rowKey, -1, locationId, changedBy);
				txns.Add(txn);
			}
			else
			{
				// look for unpaid debit transactions.
				txns = CreditWithDebtPayoff(vc, pt, pe, points, promoCode, pointsTransactionDate, pointsExpirationDate, ownerType, ownerId, rowKey, notes, locationId, changedBy);
			}
			return txns;
		}

		public List<PointTransaction> Credit(
			VirtualCard vc,
			PointType pt,
			PointEvent pe,
			decimal points,
			string promoCode,
			DateTime pointsTransactionDate,
			DateTime pointsExpirationDate,
			string locationId,
			string changedBy)
		{
			return Credit(vc, pt, pe, points, promoCode, pointsTransactionDate, pointsExpirationDate, PointTransactionOwnerType.Unknown, -1, -1, string.Empty, locationId, changedBy);
		}

		public List<PointTransaction> Debit(
			VirtualCard vc,
			PointType pt,
			PointEvent pe,
			decimal points,
			DateTime transactionDate,
			DateTime expDate,
			string locationId,
			string changedBy)
		{
			return Debit(vc, pt, pe, points, transactionDate, expDate, PointTransactionOwnerType.Unknown, -1, -1, string.Empty, locationId, changedBy);
		}

		public List<PointTransaction> Debit(
			VirtualCard vc,
			PointType pt,
			PointEvent pe,
			decimal points,
			DateTime transactionDate,
			DateTime expDate,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			string notes,
			string locationId,
			string changedBy,
            string promoCode = "")
		{
			string methodName = "Debit";
			string msg = string.Format("Debiting {0} points to virtual card {1}.", points.ToString(), vc.VcKey.ToString());
			_logger.Trace(_className, methodName, msg);

			if (!vc.IsValid())
			{
				throw new Brierley.FrameWork.Common.Exceptions.LWException(string.Format("Cannot debit points to card {0} in invalid state.", vc.LoyaltyIdNumber));
			}

			List<PointTransaction> txns = new List<PointTransaction>();
			if (!Config.DebitPayOffMethodOn)
			{
				// the old regular method which is also default.
				PointTransaction txn = CreatePtTxn(vc.VcKey, pt.ID, pe.ID, -points, promoCode, PointBankTransactionType.Debit,
					transactionDate, DateTime.Now, expDate, 0, notes, ownerType, ownerId, rowKey, -1,
					locationId, changedBy);
				txns.Add(txn);
			}
			else
			{
				txns = DebitWithCreditConsumption(vc, pt, pe, points, transactionDate, expDate, ownerType, ownerId, rowKey, notes, locationId, changedBy, promoCode);
			}
			return txns;
		}

		/// <summary>
		/// This method puts all points for a virtual card on hold.
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="transactionDate"></param>
		/// <param name="notes"></param>
		public void HoldAllPoints(VirtualCard vc, DateTime transactionDate, string notes)
		{
			string methodName = "Hold";
			string msg = string.Format("Holding all points to virtual card {0}.", vc.VcKey.ToString());
			_logger.Trace(_className, methodName, msg);

			if (!vc.IsValid())
			{
				throw new Brierley.FrameWork.Common.Exceptions.LWException(string.Format("Cannot hold points from card {0} in invalid state.", vc.LoyaltyIdNumber));
			}

			List<PointTransaction> trxs = GetTransactionsNotConsumed(vc, PointBankTransactionType.Credit);
			foreach (PointTransaction trx in trxs)
			{
				if (trx.TransactionType != PointBankTransactionType.Credit)
				{
					continue;
				}

				decimal available = trx.Points - trx.PointsConsumed - trx.PointsOnHold;
				if (available > 0)
				{
					DateTime expDate = trx.ExpirationDate != null ? (DateTime)trx.ExpirationDate : DateTimeUtil.MaxValue;

					CreatePtTxn(vc.VcKey, trx.PointTypeId, trx.PointEventId, available, string.Empty, PointBankTransactionType.Hold,
						transactionDate, DateTime.Now, expDate, 0, notes, PointTransactionOwnerType.Unknown, -1, -1, trx.Id, null, null);
					trx.PointsOnHold += available;
					UpdatePointTransaction(trx);
					_logger.Debug(_className, methodName, string.Format("{0} points were put on hold from transaction {1}.", available, trx.Id));
				}
			}
		}

		/// <summary>
		/// See issue 5304 for original requirements.  OwnerId is the id of the owner.  
		/// In general owner id is the idenfieir of the meta data and row key is the identifier of the instance data.
		/// OwnerType = AttributeSet, OwnerId = Attributeset code, RowKey is the rowkey of the actual row.
		/// OwnerType = Store, OwnerId = id of the store, RowKey = the rowkey from CheckInEvent
		/// OwnerType = Bid, OwnerId = null, rowkey = bid id
		/// OwnerType = Bonus, OwnerId = bonus defintion id, rowkey = row key of member bonus
		/// OwnerType = Survey, ownerid = null, rowkey = id of the survey
		/// OwnerType = Reward, owberid = id of reward defintino, rowkey = rowkey fo member rewrad
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="pt"></param>
		/// <param name="pe"></param>
		/// <param name="points"></param>
		/// <param name="transactionDate"></param>
		/// <param name="ownerType"></param>
		/// <param name="ownerId"></param>
		/// <param name="rowKey"></param>
		/// <param name="notes"></param>
		public void HoldPoints(
			VirtualCard vc,
			PointType pt,
			PointEvent pe,
			decimal points,
			DateTime transactionDate,
			PointTransactionOwnerType ownerType,
			long ownerId,
			long rowKey,
			string notes,
			string locationId,
			string changedBy)
		{
			string methodName = "Hold";
			string msg = string.Format("Holding {0} points to virtual card {1}.", points.ToString(), vc.VcKey.ToString());
			_logger.Trace(_className, methodName, msg);

			if (!vc.IsValid())
			{
				throw new Brierley.FrameWork.Common.Exceptions.LWException(string.Format("Cannot hold points from card {0} in invalid state.", vc.LoyaltyIdNumber));
			}

			decimal totalOnHold = 0;

			List<PointTransaction> trxs = GetTransactionsNotConsumed(vc, pt, pe, PointBankTransactionType.Credit);
			foreach (PointTransaction trx in trxs)
			{
				if (trx.TransactionType != PointBankTransactionType.Credit)
					continue;

				//Break as soon as all the necessary points have been consumed
				if (totalOnHold >= points)
					break;

				long[] parentIds = new long[1] { trx.Id };
				List<PointTransaction> onHold = GetOnHoldPointTransactions(vc.Member.LoyaltyCards, null, null, DateTimeUtil.MinValue, DateTimeUtil.MaxValue, parentIds, null, null, null);
				decimal available = trx.Points - trx.PointsConsumed;
				foreach (PointTransaction htrx in onHold)
				{
					available -= htrx.Points;
				}
				if (available <= 0)
					continue;
				decimal pointsLeftToHold = points - totalOnHold;
				decimal hold = (pointsLeftToHold >= available ? available : pointsLeftToHold);
				DateTime expDate = trx.ExpirationDate != null ? (DateTime)trx.ExpirationDate : DateTimeUtil.MaxValue;
				CreatePtTxn(vc.VcKey, pt.ID, pe.ID, hold, string.Empty, PointBankTransactionType.Hold,
					transactionDate, DateTime.Now, expDate, 0, notes, ownerType, ownerId, rowKey, trx.Id, locationId, changedBy);
				trx.PointsOnHold = hold;
				UpdatePointTransaction(trx);
				totalOnHold += hold;
			}
		}

		public void ReleaseAllPoints(long[] vcKeys)
		{
			ReleasePointsOnHold(vcKeys, PointTransactionOwnerType.Unknown, -1, -1);
		}

		public void ReleasePointsOnHold(long[] vcKeys, PointTransactionOwnerType? ownerType, long? ownerId, long? rowKey)
		{
			string methodName = "Debit";
			string msg = string.Format("Releasing points on hold.");
			_logger.Trace(_className, methodName, msg);

			List<PointTransaction> trxs = PointTransactionDao.RetrieveOnHoldPointTransactions(vcKeys, null, null, DateTimeUtil.MinValue, DateTimeUtil.MaxValue, null, ownerType, ownerId, rowKey);
			foreach (PointTransaction trx in trxs)
			{
				PointTransaction parent = GetPointTransaction(trx.ParentTransactionId);
				if (parent != null)
				{
					parent.PointsOnHold -= trx.Points;
					UpdatePointTransaction(parent);
				}
				DeletePointTransaction(trx.Id);
			}
		}

		/// <summary>
		/// This operation will deposit a debit transaction for each credit transaction
		/// that was awarded as a result of the row being passed in.  If some of the points originally
		/// awarded have already been consumed, then this may return in a negative point balance.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="vc"></param>
		/// <param name="pt"></param>
		/// <param name="pe"></param>
		/// <param name="transactionDate"></param>
		/// <param name="expDate"></param>
		/// <param name="note"></param>
		/// <param name="locationId"></param>
		/// <param name="changedBy"></param>
		/// <returns></returns>
		public decimal Debit(
			IClientDataObject row, // the row that 
			VirtualCard vc,
			PointType pt,
			PointEvent pe,
			DateTime transactionDate,
			DateTime expDate,
			string note,
			string locationId,
			string changedBy,
            string promoCode = "")
		{
			string methodName = "Debit";
			string errMsg = string.Empty;

			if (row == null)
			{
				errMsg = "No row provided to debit awarded points.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg) { ErrorCode = 3209 };
			}
			long[] rowkeys = { row.MyKey };
			long[] vcKeys = null;
			if (vc != null)
			{
				vcKeys = new long[1];
				vcKeys[0] = vc.VcKey;
			}
			decimal debitPoints = 0;
			//long vcKey = vc != null ? vc.VcKey : -1;
			//long ptId = pt != null ? pt.ID : -1;
			//long peId = pe != null ? pe.ID : -1;
			PointBankTransactionType[] txnTypes = new PointBankTransactionType[1] { PointBankTransactionType.Credit };
			List<PointTransaction> txnList = PointTransactionDao.RetrievePointTransactions(vcKeys, null, null, null, null, string.Empty, null, txnTypes, PointTransactionOwnerType.AttributeSet, row.GetMetaData().ID, rowkeys, true, false, null);
			if (txnList != null && txnList.Count > 0)
			{
				foreach (PointTransaction txn in txnList)
				{
					debitPoints += txn.Points;
					CreatePtTxn(
						txn.VcKey,
						txn.PointTypeId, txn.PointEventId, -txn.Points, promoCode,
						PointBankTransactionType.Debit, DateTime.Now, DateTime.Now, (DateTime)txn.ExpirationDate, 0, note,
						txn.OwnerType, txn.OwnerId, txn.RowKey, -1, locationId, changedBy);
				}
			}
			_logger.Trace(_className, methodName, debitPoints + " have been debited from the member's account.");
			return debitPoints;
		}

		public decimal ExpirePoints(VirtualCard vc, DateTime expiryDate, PointExpirationReason reason, string notes)
		{
			long[] vcKeys = { vc.VcKey };
			return ExpirePoints(vcKeys, null, expiryDate, reason, notes);
		}

		public decimal ExpirePoints(long[] vcKeys, DateTime? cutOffDate, DateTime expiryDate, PointExpirationReason reason, string notes)
		{
			decimal unexpired = 0;
			PointBankTransactionType[] ttypes = new PointBankTransactionType[] { PointBankTransactionType.Credit, PointBankTransactionType.Debit, PointBankTransactionType.Hold, PointBankTransactionType.Consumed, PointBankTransactionType.Transferred };
			List<PointTransaction> unexpiredTxns = PointTransactionDao.RetrieveUnexpiredTransactions(vcKeys, null, null, ttypes, cutOffDate, null, null, null);
			if (unexpiredTxns != null && unexpiredTxns.Count > 0)
			{
				foreach (PointTransaction utxn in unexpiredTxns)
				{
					unexpired += utxn.Points;
				}
				var txnIds = (from txn in unexpiredTxns select txn.Id);
				PointTransactionDao.ExpireTransactions(txnIds.ToArray<long>(), expiryDate, reason, notes);
			}
			return unexpired;
		}

		#endregion

		#region Balances

		public decimal GetTotalPointsAwarded(
			long[] pointTypes,
			long[] pointEvents,
			DateTime? from,
			DateTime? to,
			bool includeExpiredPoints)
		{
			PointBankTransactionType[] tt = { PointBankTransactionType.Credit, PointBankTransactionType.Debit };
			return PointTransactionDao.RetrieveTotalPointsAwarded(pointTypes, pointEvents, tt, from, to, includeExpiredPoints);
		}
		*/

		public decimal GetEarnedPointBalance(
			IList<VirtualCard> vcList,
			long[] pointTypes,
			long[] pointEvents,
			DateTime? from,
			DateTime? to,
			DateTime? awardDateFrom,
			DateTime? awardDateTo,
			bool includeExpiredPoints)
		{
			string methodName = "GetEarnedPointBalance";
			if (vcList == null || vcList.Count == 0)
			{
				_logger.Error(_className, methodName, "No virtual cards specified for getting earned point balance.");
				throw new LWException("No virtual cards specified for getting earned point balance.");
			}

			List<VirtualCard> validCards = new List<VirtualCard>();
			foreach (VirtualCard v in vcList)
			{
				if (v.IsValid())
				{
					validCards.Add(v);
				}
			}
			long[] vcs = new long[validCards.Count];
			int idx = 0;
			foreach (VirtualCard v in validCards)
			{
				vcs[idx++] = v.VcKey;
			}

			PointBankTransactionType[] tt = { PointBankTransactionType.Credit, PointBankTransactionType.Debit };

			using (var service = _lwDataServiceUtil.LoyaltyDataServiceInstance())
			{
				return service.PointTransactionDao.RetrievePointBalance(vcs, pointTypes, pointEvents, tt, from, to, awardDateFrom, awardDateTo, null, null, null, null, null, includeExpiredPoints);
			}
		}

		public decimal GetEarnedPointBalance(
			IList<VirtualCard> vcList,
			IList<PointType> pointTypes,
			IList<PointEvent> pointEvents,
			DateTime? from,
			DateTime? to,
			DateTime? awardDateFrom,
			DateTime? awardDateTo,
			bool includeExpiredPoints)
		{
			int idx = 0;

			long[] pts = null;
			if (pointTypes != null && pointTypes.Count > 0)
			{
				pts = new long[pointTypes.Count];
				idx = 0;
				foreach (PointType p in pointTypes)
				{
					pts[idx++] = p.ID;
				}
			}
			long[] pes = null;
			if (pointEvents != null && pointEvents.Count > 0)
			{
				pes = new long[pointEvents.Count];
				idx = 0;
				foreach (PointEvent p in pointEvents)
				{
					pes[idx++] = p.ID;
				}
			}
			return GetEarnedPointBalance(vcList, pts, pes, from, to, awardDateFrom, awardDateTo, includeExpiredPoints);
		}

		/*
		public decimal GetPointBalance(
			long[] vcKeys,
			long[] pointTypeIds,
			long[] pointEventIds,
			PointBankTransactionType[] txnTypes,
			DateTime? from,
			DateTime? to,
			DateTime? awardDateFrom,
			DateTime? awardDateTo,
			string changedBy,
			string locationId,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long[] rowkeys,
			bool includeExpiredPoints = false)
		{
			string methodName = "GetPointBalance";

			if (vcKeys == null || vcKeys.Length == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}
			PointBankTransactionType[] tt = null;
			if (txnTypes != null && txnTypes.Length > 0)
			{
				tt = new PointBankTransactionType[txnTypes.Length];
				int index = 0;
				foreach (PointBankTransactionType t in txnTypes)
				{
					tt[index++] = t;
				}
			}
			else
			{
				tt = new PointBankTransactionType[] { PointBankTransactionType.Consumed, PointBankTransactionType.Credit, PointBankTransactionType.Debit };
			}
			return PointTransactionDao.RetrievePointBalance(vcKeys, pointTypeIds, pointEventIds, tt, from, to, awardDateFrom, awardDateTo, changedBy, locationId, ownerType, ownerId, rowkeys, includeExpiredPoints);
		}

		public decimal GetPointsConsumed(
			long[] vcKeys,
			long[] pointTypeIds,
			long[] pointEventIds,
			DateTime? from,
			DateTime? to,
			string changedBy,
			string locationId,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long[] rowkeys)
		{
			string methodName = "GetPointsConsumed";

			if (vcKeys == null || vcKeys.Length == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg) { ErrorCode = 3230 };
			}
			PointBankTransactionType[] tt = { PointBankTransactionType.Consumed };
			return PointTransactionDao.RetrievePointBalance(vcKeys, pointTypeIds, pointEventIds, tt, from, to, null, null, changedBy, locationId, ownerType, ownerId, rowkeys, false);
		}

		public decimal GetPointBalanceExcludingPointTypes(VirtualCard vc, DateTime? FromDate, DateTime? ToDate, long[] excludePointTypes)
		{
			List<long> pointTypeIds = new List<long>();
			if (excludePointTypes != null && excludePointTypes.Length > 0)
			{
				List<PointType> ptList = GetAllPointTypes();
				foreach (PointType pt in ptList)
				{
					bool toExclude = false;
					for (int i = 0; i < excludePointTypes.Length; i++)
					{
						if (pt.ID == excludePointTypes[i])
						{
							toExclude = true;
							break;
						}
						if (!toExclude)
						{
							pointTypeIds.Add(pt.ID);
						}
					}
				}
			}
			return PointTransactionDao.RetrievePointBalanceByDateExcludingPointTypes(vc.VcKey, FromDate, ToDate, excludePointTypes);
		}

		public decimal GetPointsOnHold(IList<VirtualCard> vcList, IList<PointType> pointTypes, IList<PointEvent> pointEvents, DateTime? from, DateTime? to)
		{
			string methodName = "GetPointsOnHold";

			if (vcList == null || vcList.Count == 0)
			{
				_logger.Error(_className, methodName, "No virtual cards specified for get points on hold");
				throw new LWException("No virtual cards specified for points on hold");
			}

			Dictionary<long, VirtualCard> vcMap = new Dictionary<long, VirtualCard>();
			List<VirtualCard> validCards = new List<VirtualCard>();
			foreach (VirtualCard v in vcList)
			{
				if (v.IsValid())
				{
					validCards.Add(v);
					vcMap.Add(v.VcKey, v);
				}
			}
			long[] vcs = new long[validCards.Count];
			int idx = 0;
			foreach (VirtualCard v in validCards)
			{
				vcs[idx++] = v.VcKey;
			}
			long[] pts = null;
			if (pointTypes != null && pointTypes.Count > 0)
			{
				pts = new long[pointTypes.Count];
				idx = 0;
				foreach (PointType p in pointTypes)
				{
					pts[idx++] = p.ID;
				}
			}
			long[] pes = null;
			if (pointEvents != null && pointEvents.Count > 0)
			{
				pes = new long[pointEvents.Count];
				idx = 0;
				foreach (PointEvent p in pointEvents)
				{
					pes[idx++] = p.ID;
				}
			}
			return PointTransactionDao.RetrievePointsOnHold(vcs, pts, pes, from, to, null, null, null, null);
		}

		#endregion

		#region Points Expiration

		public decimal GetExpiredPointBalance(
			long[] vcKeys,
			long[] pointTypeIds,
			long[] pointEventIds,
			PointBankTransactionType[] txnTypes,
			DateTime? from,
			DateTime? to,
			DateTime? awardDateFrom,
			DateTime? awardDateTo,
			string changedBy,
			string locationId,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long[] rowkeys)
		{
			string methodName = "GetExpiredPointBalance";

			if (vcKeys == null || vcKeys.Length == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}
			PointBankTransactionType[] tt = null;
			if (txnTypes != null && txnTypes.Length > 0)
			{
				tt = new PointBankTransactionType[txnTypes.Length];
				int index = 0;
				foreach (PointBankTransactionType t in txnTypes)
				{
					tt[index++] = t;
				}
			}
			else
			{
				tt = new PointBankTransactionType[] { PointBankTransactionType.Consumed, PointBankTransactionType.Credit, PointBankTransactionType.Debit };
			}
			return PointTransactionDao.RetrieveExpiredPointBalance(vcKeys, pointTypeIds, pointEventIds, tt, from, to, awardDateFrom, awardDateTo, changedBy, locationId, ownerType, ownerId, rowkeys);
		}

		public decimal GetExpiredPointBalance(
			IList<VirtualCard> vcList,
			IList<PointType> pointTypes,
			IList<PointEvent> pointEvents,
			PointBankTransactionType[] txnTypes,
			DateTime? from,
			DateTime? to,
			DateTime? awardDateFrom,
			DateTime? awardDateTo,
			string changedBy,
			string locationId,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long[] rowkeys)
		{
			string methodName = "GetExpiredPointBalance";

			List<VirtualCard> validCards = new List<VirtualCard>();
			foreach (VirtualCard v in vcList)
			{
				if (v.IsValid())
				{
					validCards.Add(v);
				}
			}
			long[] vcKeys = new long[validCards.Count];
			if (vcKeys == null || vcKeys.Length == 0)
			{
				string errMsg = "No virtual cards provided for point calculations.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg);
			}
			long[] pts = null;
			int idx = 0;
			if (pointTypes != null && pointTypes.Count > 0)
			{
				pts = new long[pointTypes.Count];
				idx = 0;
				foreach (PointType p in pointTypes)
				{
					pts[idx++] = p.ID;
				}
			}
			long[] pes = null;
			if (pointEvents != null && pointEvents.Count > 0)
			{
				pes = new long[pointEvents.Count];
				idx = 0;
				foreach (PointEvent p in pointEvents)
				{
					pes[idx++] = p.ID;
				}
			}
			PointBankTransactionType[] tt = null;
			if (txnTypes != null && txnTypes.Length > 0)
			{
				tt = new PointBankTransactionType[txnTypes.Length];
				int index = 0;
				foreach (PointBankTransactionType t in txnTypes)
				{
					tt[index++] = t;
				}
			}
			else
			{
				tt = new PointBankTransactionType[] { PointBankTransactionType.Consumed, PointBankTransactionType.Credit, PointBankTransactionType.Debit };
			}
			return PointTransactionDao.RetrieveExpiredPointBalance(vcKeys, pts, pes, tt, from, to, awardDateFrom, awardDateTo, changedBy, locationId, ownerType, ownerId, rowkeys);
		}

		public void ExtendPointsExpirationDate(VirtualCard vc, DateTime expFrom, DateTime expTo, DateTime newExpiryDate)
		{
			string methodName = "ExtendPointsExpiryDate";

			string msg = string.Format("Resetting expiration date of all transactions that have expiration date between {0} - {1} for loyalty card {2} to {3}",
				expFrom.ToShortDateString(), expTo.ToShortDateString(), vc.LoyaltyIdNumber, newExpiryDate.ToShortDateString());
			_logger.Debug(_className, methodName, msg);

			long[] vcKeys = new long[] { vc.VcKey };
			PointBankTransactionType[] txnTypes = new PointBankTransactionType[] { PointBankTransactionType.Credit };
			List<PointTransaction> txnList = PointTransactionDao.RetrieveUnexpiredTransactions(vcKeys, null, null, txnTypes, null, null, expFrom, expTo);
			if (txnList != null && txnList.Count > 0)
			{
				_logger.Debug(_className, methodName, txnList.Count + " credit transactions found that were expiring in the provided date range.");
				var txnIds = (from txn in txnList
							  select txn.Id);
				PointTransactionDao.ExpireTransactions(txnIds.ToArray<long>(), newExpiryDate, null, string.Empty);
			}
		}

		#endregion

		#region DAP/Rules Processor Support

		public List<long> GetMembersBasedOnPointBalance(PointBalanceType type, long balance, LWCriterion.Predicate predicate, long[] pointTypes, long[] pointEvents, DateTime? startDate, DateTime? endDate, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();

			List<long> ipcodeList = PointTransactionDao.RetrieveMembersBasedOnPointBalance(type, balance, predicate, pointTypes, pointEvents, startDate, endDate, batchInfo);
			if (ipcodeList == null)
			{
				ipcodeList = new List<long>();
			}
			return ipcodeList;
		}

		public List<long> GetMembersWithExpiredPoints(LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();
			List<long> ipcodeList = PointTransactionDao.RetrieveMembersWithExpiredPoints(batchInfo);
			if (ipcodeList == null)
			{
				ipcodeList = new List<long>();
			}
			return ipcodeList;
		}

		#endregion

		#endregion

		#region Member Rewards

		public void CreateMemberReward(MemberReward reward)
		{
			MemberRewardDao.Create(reward);
		}

		public void UpdateMemberReward(MemberReward reward)
		{
			MemberRewardDao.Update(reward);
		}

		public long HowManyMemberRewards(long ipcode, DateTime startDate, DateTime endDate)
		{
			return MemberRewardDao.HowMany(ipcode, startDate, endDate);
		}

		public MemberReward GetMemberReward(long id)
		{
			MemberReward reward = MemberRewardDao.Retrieve(id);
			return reward;
		}

		public MemberReward GetMemberRewardByCert(string certNmbr)
		{
			MemberReward reward = MemberRewardDao.RetrieveByCert(certNmbr);
			return reward;
		}

		public List<MemberReward> GetMemberRewardsByOrderNumber(string orderNumber)
		{
			List<MemberReward> list = MemberRewardDao.RetrieveByOrderNumber(orderNumber);
			if (list == null)
			{
				list = new List<MemberReward>();
			}
			else
			{
				// sort the members on DateIssued
				var sortedRewards = list.OrderByDescending(d => d.DateIssued);
				list = sortedRewards.ToList<MemberReward>();
			}
			return list;
		}

		public List<MemberReward> GetMemberRewardsByFPOrderNumber(string orderNumber)
		{
			List<MemberReward> list = MemberRewardDao.RetrieveByFPOrderNumber(orderNumber);
			if (list == null)
			{
				list = new List<MemberReward>();
			}
			else
			{
				// sort the members on DateIssued
				var sortedRewards = list.OrderByDescending(d => d.DateIssued);
				list = sortedRewards.ToList<MemberReward>();
			}
			return list;
		}

		public List<MemberReward> GetMemberRewardsByDefId(Member member, long rewardDefId)
		{
			List<MemberReward> list = MemberRewardDao.RetrieveByDefId(member.IpCode, rewardDefId);
			if (list == null)
			{
				list = new List<MemberReward>();
			}
			else
			{
				// sort the members on DateIssued
				var sortedRewards = list.OrderByDescending(d => d.DateIssued);
				list = sortedRewards.ToList<MemberReward>();
			}
			return list;
		}

		public List<MemberReward> GetMemberRewards(Member member, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();

			List<MemberReward> list = MemberRewardDao.RetrieveByMember(member.IpCode, batchInfo);
			if (list == null)
			{
				list = new List<MemberReward>();
			}
			else
			{
				// sort the members on DateIssued
				var sortedRewards = list.OrderByDescending(d => d.DateIssued);
				list = sortedRewards.ToList<MemberReward>();
			}
			return list;
		}

		public List<long> GetMemberRewardIds(Member member, long? categoryId, DateTime? from, DateTime? to, DateTime? changedSince, bool unRedeemedOnly, bool unExpiredOnly)
		{
			long[] ipcode = null;
			if (member != null)
			{
				ipcode = new long[1];
				ipcode[0] = member.IpCode;
			}
			List<long> ids = MemberRewardDao.RetrieveIds(ipcode, categoryId, from, to, changedSince, unRedeemedOnly, unExpiredOnly);
			if (ids == null)
			{
				ids = new List<long>();
			}
			return ids;
		}

        public List<long> GetMemberRewardIds(Member member, long[] rewardDefIds, long[] excludeRewardDefIds, DateTime? from, DateTime? to,
            DateTime? redeemedFrom, DateTime? redeemedTo, bool? redeemed, bool? expired)
        {
            long[] ipcode = null;
            if (member != null)
            {
                ipcode = new long[1];
                ipcode[0] = member.IpCode;
            }
            List<long> ids = MemberRewardDao.RetrieveIds(ipcode,  rewardDefIds, excludeRewardDefIds, from, to, redeemedFrom, redeemedTo, redeemed, expired);
            if (ids == null)
            {
                ids = new List<long>();
            }
            return ids;
        }

        public List<MemberReward> GetMemberRewardByIds(long[] ids)
		{
			List<MemberReward> rewards = MemberRewardDao.Retrieve(ids);
			if (rewards == null)
			{
				rewards = new List<MemberReward>();
			}
			else
			{
                // sort the members on DateIssued
                var sortedRewards = rewards.OrderByDescending(d => d.DateIssued).ThenByDescending(d => d.CreateDate);
				rewards = sortedRewards.ToList<MemberReward>();
			}
			return rewards;
		}

		public List<MemberReward> GetAllMemberRewards(LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();

			List<MemberReward> rewards = MemberRewardDao.RetrieveAll(batchInfo);
			if (rewards == null)
			{
				rewards = new List<MemberReward>();
			}
			else
			{
				// sort the members on DateIssued
				var sortedRewards = rewards.OrderByDescending(d => d.DateIssued);
				rewards = sortedRewards.ToList<MemberReward>();
			}
			return rewards;
		}

		public void CancelOrReturnMemberReward(MemberReward reward, RewardOrderStatus newStatus, string notes)
		{
			string methodName = "CancelOrReturnMemberReward";

			/*
			 * Check to make sure that the reward can be cancelled or returned.
			 * 
			using (var transaction = Database.GetTransaction())
			{
				if (reward.OrderStatus != null)
				{
					if (reward.OrderStatus.Value != RewardOrderStatus.Created &&
						reward.OrderStatus.Value != RewardOrderStatus.Hold &&
						reward.OrderStatus.Value != RewardOrderStatus.Pending)
					{
						string emsg = string.Empty;
						if (reward.OrderStatus.Value == RewardOrderStatus.Cancelled)
						{
							emsg = string.Format("Reward with id {0} has already been cancelled.", reward.Id);
						}
						else if (reward.OrderStatus.Value == RewardOrderStatus.InProcess)
						{
							emsg = string.Format("Reward with id {0} is already being processed and cannot be cancelled anymore.", reward.Id);
						}
						else if (reward.OrderStatus.Value == RewardOrderStatus.Returned)
						{
							emsg = string.Format("Reward with id {0} is already been returned and cannot be cancelled anymore.", reward.Id);
						}
						else if (reward.OrderStatus.Value == RewardOrderStatus.Shipped)
						{
							emsg = string.Format("Reward with id {0} is already been shipped and cannot be cancelled anymore.", reward.Id);
						}
						throw new LWDataServiceException(emsg) { ErrorCode = 6013 };
					}
				}

				reward.OrderStatus = newStatus;

				Member member = LoadMemberFromIPCode(reward.MemberId);
				var vckeys = (from vc in member.LoyaltyCards select vc.VcKey);
				ReleasePointsOnHold(vckeys.ToArray<long>(), PointTransactionOwnerType.Reward, reward.RewardDefId, reward.Id);
				long[] rowkeys = new long[1] { reward.Id };
				PointBankTransactionType[] txnTypes = new PointBankTransactionType[1] { PointBankTransactionType.Consumed };
				List<PointTransaction> txnList = PointTransactionDao.RetrievePointTransactions(vckeys.ToArray<long>(), null, null, null, null, string.Empty, null, txnTypes, PointTransactionOwnerType.Reward, reward.RewardDefId, rowkeys, true, false, null);

				// credit a lumpsum amount
				RewardDef rdef = null;
				using (var contentService = LWDataServiceUtil.ContentServiceInstance(Organization, Environment))
				{
					rdef = contentService.GetRewardDef(reward.RewardDefId);

					if (txnList != null && txnList.Count > 0)
					{
						//var points = from txn in txnList select Math.Abs(txn.Points);
						//double pointsConsumed = points.Sum();                
						foreach (PointTransaction txn in txnList)
						{
							// we need to credit each consumed transaction.
							PointTransaction creditTxn = PointTransactionDao.Retrieve(txn.ParentTransactionId);
							//creditTxn.PointsConsumed -= Math.Abs(txn.Points);
							//PointTransactionDAO.Update(creditTxn);
							//PointTransactionDAO.Delete(txn.Id);
							VirtualCard vc = member.GetLoyaltyCard(creditTxn.VcKey);
							PointType pt = GetPointType(creditTxn.PointTypeId);
							PointEvent pe = GetPointEvent(creditTxn.PointEventId);
							Credit(
								vc, pt, pe, Math.Abs(txn.Points), string.Empty,
								DateTime.Now, (DateTime)creditTxn.ExpirationDate, PointTransactionOwnerType.Reward, rdef.Id, reward.Id, notes, string.Empty, string.Empty);
						}
					}

					// Reverse the product quantity
					if (reward.ProductVariantId > 0)
					{
						_logger.Trace(_className, methodName, "Updating quantity of product variant " + reward.ProductVariantId);
						ProductVariant pv = contentService.GetProductVariant(reward.ProductVariantId);
						if (pv != null)
						{
							if (pv.Quantity != null)
							{
                                long newQuantity = contentService.UpdateProductVariantQuantity(pv.ID, 1);
                                _logger.Debug(_className, methodName, "New product variant quantity = " + newQuantity);
							}
						}
						else
						{
							_logger.Error(_className, methodName, "Invalid product variant found for member reward.");
							throw new LWDataServiceException("Invalid product variant found for member reward.");
						}
					}
					else if (reward.ProductId > 0)
					{
						_logger.Trace(_className, methodName, "Updating quantity of product  " + reward.ProductVariantId);
						Product p = contentService.GetProduct(reward.ProductId);
						if (p != null)
						{
							if (p.Quantity != null)
							{
                                long newQuantity = contentService.UpdateProductQuantity(p.Id, 1);
                                _logger.Debug(_className, methodName, "New product quantity = " + newQuantity);
							}
						}
						else
						{
							_logger.Error(_className, methodName, "Invalid product found for member reward.");
							throw new LWDataServiceException("Invalid product found for member reward.");
						}
					}
				}
				MemberRewardDao.Update(reward);
				transaction.Complete();
			}
		}

		#endregion

		#region Reward Order Management
		public void CreateMemberOrder(Member member, MemberOrder order)
		{
			string methodName = "CreateMemberOrder";

			if (member.MemberStatus == MemberStatusEnum.NonMember)
			{
				string msg = string.Format("Cannot issue reward to non-member with ipcode {0}.", member.IpCode);
				_logger.Error(_className, methodName, msg);
				throw new LWDataServiceException(msg) { ErrorCode = 9969 };
			}

			_logger.Trace(_className, methodName, "Creating new member reward order for order number " + order.OrderNumber);

			MemberOrderDao.Create(order);

			if (order.OrderItems != null && order.OrderItems.Count > 0)
			{
				foreach (RewardOrderItem orderItem in order.OrderItems)
				{
					ContextObject context = new ContextObject();
					if (order.LoyaltyCard != null)
					{
						context.Owner = order.LoyaltyCard;
					}
					else
					{
						context.Owner = member;
					}
					var env = new Dictionary<string, object>();
					env.Add("RewardName", orderItem.RewardName);
					if (order != null)
					{
						env.Add("LWOrder", order);
					}
					else if (!string.IsNullOrEmpty(order.OrderNumber))
					{
						env.Add("LWOrderNumber", order.OrderNumber);
					}
					if (!string.IsNullOrEmpty(orderItem.FPOrderNumber))
					{
						env.Add("FPOrderNumber", orderItem.FPOrderNumber);
					}
					if (orderItem.ExpirationDate != null)
					{
						env.Add("ExpirationDate", orderItem.ExpirationDate);
					}
					if (orderItem.ProviderId != null)
					{
						env.Add("FulfillmentProviderId", orderItem.ProviderId.Value);
					}
					context.Environment = env;
					if (orderItem.PartNumber != orderItem.RewardDefinition.Product.PartNumber)
					{
						// The part number belongs to a variant
						List<ProductVariant> variants = orderItem.RewardDefinition.Product.GetVariants();
						foreach (ProductVariant variant in variants)
						{
							if (orderItem.PartNumber == variant.PartNumber)
							{
								env.Add("ProductVariantId", variant.ID);
								break;
							}
						}
					}
					if (!string.IsNullOrEmpty(orderItem.CertificateNumber))
					{
						env.Add("CertificateNumber", orderItem.CertificateNumber);
					}
					Execute(orderItem.Rule, context);
					Brierley.FrameWork.Rules.IssueRewardRuleResult result = context.Results.Last() as Brierley.FrameWork.Rules.IssueRewardRuleResult;
					if (result != null)
					{
						orderItem.MemberRewardId = result.RewardId;
					}
					if (orderItem.MemberRewardId == 0)
					{
						_logger.Error(_className, methodName, "Unable to create reward.");
						throw new LWDataServiceException("Unable to create member reward.") { ErrorCode = 3208 };
					}
				}
			}
		}

		public void UpdateMemberOrder(MemberOrder order)
		{
			MemberOrderDao.Update(order);
		}

		public MemberOrder GetMemberOrder(string orderNumber)
		{
			return MemberOrderDao.RetrieveByOrderNumber(orderNumber);
		}

		public List<MemberOrder> GetMemberOrders(string[] orderNumbers)
		{
			List<MemberOrder> orders = MemberOrderDao.RetrieveByOrderNumbers(orderNumbers);
			if (orders == null)
			{
				orders = new List<MemberOrder>();
			}
			return orders;
		}

		public List<MemberOrder> GetMemberOrdersByMember(long ipcode, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();

			List<MemberOrder> orders = MemberOrderDao.RetrieveByMember(ipcode, batchInfo);
			if (orders == null)
			{
				orders = new List<MemberOrder>();
			}
			return orders;
		}

		public void CancelOrReturnMemberOrder(string orderNumber, string cancellationNumber, string notes)
		{
			MemberOrder order = MemberOrderDao.RetrieveByOrderNumber(orderNumber);
			List<MemberReward> rewardsList = GetMemberRewardsByOrderNumber(orderNumber);
			foreach (MemberReward reward in rewardsList)
			{
				RewardOrderStatus newStatus = RewardOrderStatus.Cancelled;
				if (!string.IsNullOrEmpty(cancellationNumber))
				{
					//reward.OrderStatus = RewardOrderStatus.Cancelled;
					reward.LWCancellationNumber = cancellationNumber;
				}
				else
				{
					newStatus = RewardOrderStatus.Returned;
					//reward.OrderStatus = RewardOrderStatus.Returned;
				}
				CancelOrReturnMemberReward(reward, newStatus, notes);
			}
			order.OrderCancellationNumber = cancellationNumber;
			MemberOrderDao.Update(order);
			//MemberOrderDAO.Delete(order.Id);
		}

		#endregion

		#region MemberCoupons

		public void CreateMemberCoupon(MemberCoupon coupon)
		{
			MemberCouponDao.Create(coupon);
		}

		public void CreateMemberCoupons(List<MemberCoupon> coupons)
		{
			MemberCouponDao.Create(coupons);
		}

		public void UpdateMemberCoupon(MemberCoupon coupon)
		{
			MemberCouponDao.Update(coupon);
		}

		public long HowManyActiveMemberCoupons(long memberId)
		{
			return MemberCouponDao.HowManyActiveCoupons(memberId);
		}

		public long HowManyMemberCouponsByType(long memberId, long defId)
		{
			return MemberCouponDao.HowManyCouponsByType(memberId, defId);
		}

		public MemberCoupon GetMemberCoupon(long id)
		{
			return MemberCouponDao.Retrieve(id);
		}

		public IEnumerable<MemberCoupon> GetMemberCoupons(long memberId, long defId)
		{
			return MemberCouponDao.Retrieve(memberId, defId);
		}

		public List<MemberCoupon> GetMemberCoupons(long[] ids)
		{
			return MemberCouponDao.Retrieve(ids) ?? new List<MemberCoupon>();
		}

		public List<MemberCoupon> GetAllMemberCoupons()
		{
			return MemberCouponDao.RetrieveAll();
		}

		[Obsolete]
		public List<MemberCoupon> GetMemberCouponsByMember(long ipCode, LWQueryBatchInfo batchInfo = null, bool unRedeemedOnly = false, bool activeOnly = true, DateTime? from = null, DateTime? to = null)
		{
			return MemberCouponDao.RetrieveByMember(ipCode, batchInfo, unRedeemedOnly, activeOnly, from, to) ?? new List<MemberCoupon>();
		}

		public PetaPoco.Page<MemberCoupon> GetMemberCoupons(long ipCode, ActiveCouponOptions options, long page, long resultsPerPage)
		{
			return MemberCouponDao.Retrieve(ipCode, options, page, resultsPerPage);
		}

		public List<MemberCoupon> GetMemberCouponsByMemberByTypeCode(long ipCode, string typeCode, LWQueryBatchInfo batchInfo)
		{
			return MemberCouponDao.RetrieveByMemberAndTypeCode(ipCode, typeCode, batchInfo) ?? new List<MemberCoupon>();
		}

		public List<long> GetMemberCouponIds(Member member, DateTime? from, DateTime? to, bool unExpiredOnly)
		{
			long[] ipcode = null;
			if (member != null)
			{
				ipcode = new long[1] { member.IpCode };
			}
			return MemberCouponDao.RetrieveIds(ipcode, from, to, unExpiredOnly) ?? new List<long>();
		}

		public MemberCoupon GetMemberCouponByCertNmbr(string certNmbr)
		{
			return MemberCouponDao.RetrieveByCertNmbr(certNmbr);
		}

		public void DeleteMemberCoupon(long couponId)
		{
			MemberCouponDao.Delete(couponId);
		}

		public void CreateMemberCouponRedemption(MemberCouponRedemption redemption)
		{
			MemberCouponRedemptionDao.Create(redemption);
		}

		public List<MemberCouponRedemption> GetCouponRedemptions(long memberCouponId)
		{
			return MemberCouponRedemptionDao.Retrieve(memberCouponId);
		}

		public MemberCouponRedemption GetLastCouponRedemption(long memberCouponId)
		{
			return MemberCouponRedemptionDao.RetrieveLastRedemption(memberCouponId);
		}

		public void UpdateMemberCouponRedemption(MemberCouponRedemption redemption)
		{
			MemberCouponRedemptionDao.Update(redemption);
		}

		#endregion

		#region Member Bonuses

		public void CreateMemberOffer(MemberBonus ad)
		{
			MemberBonusDao.Create(ad);
		}

		public void UpdateMemberOffer(MemberBonus offer)
		{
			MemberBonusDao.Update(offer);
		}

		public int ExpireUnexpiredUncompletedBonus(long bonusId, DateTime newDate)
		{
			return MemberBonusDao.ExpireUnexpiredUncompletedBonus(bonusId, newDate);
		}

		public int ExtendExpiredUncompletedBonus(long bonusId, DateTime newDate)
		{
			return MemberBonusDao.ExtendExpiredUncompletedBonus(bonusId, newDate);
		}

		public long HowManyActiveMemberBonuses(long memberId)
		{
			return MemberBonusDao.HowManyActiveBonuses(memberId);
		}

		public long HowManyMemberBonusesByType(long memberId, long defId)
		{
			return MemberBonusDao.HowManyBonusesByType(memberId, defId);
		}

		public long HowManyCompletedBonusesByType(long defId)
		{
			return MemberBonusDao.HowManyCompletedByType(defId);
		}

		public long HowManyCompletedBonusesByType(long memberId, long defId)
		{
			return MemberBonusDao.HowManyCompletedByType(memberId, defId);
		}

		public long HowManyCompletedBonusReferrals(long defId)
		{
			return MemberBonusDao.HowManyReferralsCompleted(defId);
		}

		public long HowManyUnexpiredAndUncompletedBonuses(long defId)
		{
			return MemberBonusDao.HowManyUnexpiredAndUncompletedBonuses(defId);
		}

		public long HowManyExpiredAndUncompletedBonuses(long defId)
		{
			return MemberBonusDao.HowManyExpiredAndUncompletedBonuses(defId);
		}

		public MemberBonus GetMemberOffer(long id)
		{
			return MemberBonusDao.Retrieve(id);
		}

		public List<MemberBonus> GetMemberBonuses(long[] ids)
		{
			return MemberBonusDao.Retrieve(ids) ?? new List<MemberBonus>();
		}

		public List<MemberBonus> GetAllMemberOffers()
		{
			return MemberBonusDao.RetrieveAll();
		}

		public List<MemberBonus> GetMemberBonusesByMember(long ipCode, LWQueryBatchInfo batchInfo)
		{
			return MemberBonusDao.RetrieveByMember(ipCode, null, false, null, batchInfo);
		}

		public List<MemberBonus> GetMemberBonusesByMember(long ipCode, bool activeOnly, LWQueryBatchInfo batchInfo)
		{
			return MemberBonusDao.RetrieveByMember(ipCode, null, activeOnly, null, batchInfo);
		}

		public List<MemberBonus> GetMemberBonusesByMember(long ipCode, IEnumerable<MemberBonusStatus> statuses, bool activeOnly, LWQueryBatchInfo batchInfo)
		{
			return MemberBonusDao.RetrieveByMember(ipCode, statuses, activeOnly, null, batchInfo);
		}

		public List<MemberBonus> GetMemberBonusesByMember(long ipCode, IEnumerable<MemberBonusStatus> statuses, bool activeOnly, long? definitionId, LWQueryBatchInfo batchInfo)
		{
			return MemberBonusDao.RetrieveByMember(ipCode, statuses, activeOnly, definitionId, batchInfo);
		}

		public void DeleteMemberOffer(long offerId)
		{
			MemberBonusDao.Delete(offerId);
		}

		#endregion

		#region Member Message

		public void CreateMemberMessage(MemberMessage message)
		{
			MemberMessageDao.Create(message);
		}

		public void UpdateMemberMessage(MemberMessage message)
		{
			MemberMessageDao.Update(message);
		}

		public long HowManyActiveMemberMessages(long memberId)
		{
			return MemberMessageDao.HowManyActiveMessages(memberId);
		}

		public long HowManyActiveUnreadMessages(long memberId)
		{
			return MemberMessageDao.HowManyActiveUnreadMessages(memberId);
		}

		public MemberMessage GetMemberMessage(long id)
		{
			return MemberMessageDao.RetrieveByMemberId(id);
		}

		public List<MemberMessage> GetMemberMessages(long memberId, long defId, bool unexpiredOnly = false)
		{
			return MemberMessageDao.Retrieve(memberId, defId, unexpiredOnly);
		}

		public Page<MemberMessage> GetMemberMessages(long memberId, List<MemberMessageStatus> status, bool activeOnly, long pageNumber, long resultsPerPage, DateTime? startDate, DateTime? endDate, MemberMessageOrder order = MemberMessageOrder.Newest)
		{
			return MemberMessageDao.Retrieve(memberId, status, activeOnly, pageNumber, resultsPerPage, startDate, endDate, order);
		}

		/*
		 * CDIS - and many other systems - use LWQueryBatchInfo to retrieve batches of data. Messages are "paged", which seems to work better than batching in cases where a user
		 * is shown pages of data. Calling PetaPoco's Page<T> method returns a PetaPoco.Page that includes the total number of rows and pages. Without it, you either have to count the 
		 * results in a separate query or just take wild guess at how many items there are.
		 * 
		 * This method uses LWQueryBatchInfo in order to keep CDIS backward compatible. Any caller that isn't CDIS should not use this method.
		 
		[Obsolete("This method uses LWQueryBatchInfo in order to maintain backward compatibility with CDIS. It may be removed in a future LoyaltyWare version. Use the method that returns Page<MemberMessage> instead.")]
		public List<MemberMessage> GetMemberMessages(long memberId, List<MemberMessageStatus> status, bool activeOnly, LWQueryBatchInfo batchInfo, DateTime? startDate, DateTime? endDate, MemberMessageOrder order = MemberMessageOrder.Newest)
		{
			return MemberMessageDao.Retrieve(memberId, status, activeOnly, batchInfo, startDate, endDate, order);
		}

		public List<MemberMessage> GetMemberMessagesByMember(long ipCode, LWQueryBatchInfo batchInfo)
		{
			return MemberMessageDao.Retrieve(ipCode, batchInfo) ?? new List<MemberMessage>();
		}

		public List<MemberMessage> GetMemberMessagesByMemberAndType(long ipCode, long defId)
		{
			return MemberMessageDao.Retrieve(ipCode, defId, true) ?? new List<MemberMessage>();
		}

		public List<MemberMessage> GetMemberMessages(long[] ids)
		{
			return MemberMessageDao.Retrieve(ids) ?? new List<MemberMessage>();
		}

		public void DeleteMemberMessage(long messageId)
		{
			MemberMessageDao.Delete(messageId);
		}

		#endregion

		#region Member Promotion List

		public void CreateMemberPromotion(MemberPromotion promotion)
		{
			MemberPromotionDao.Create(promotion);
		}

		public MemberPromotion EnrollMemberPromotion(long promotionId, Member member, bool useCertificate = false, List<ContextObject.RuleResult> results = null, RuleExecutionMode mode = RuleExecutionMode.Real, bool skipRules = false)
		{
			using (var content = new ContentService(Config))
			{
				var promo = content.GetPromotion(promotionId);
				if (promo == null)
				{
					throw new Exception(string.Format("Failed to enroll member in promotion because the promotion id {0} does not exist.", promotionId));
				}
				return EnrollMemberPromotion(promo, member);
			}
		}

		public MemberPromotion EnrollMemberPromotion(string promoCode, Member member, bool useCertificate = false, List<ContextObject.RuleResult> results = null, RuleExecutionMode mode = RuleExecutionMode.Real, bool skipRules = false)
		{
			var promo = GetPromotionByCode(promoCode);
			if (promo == null)
			{
				throw new Exception(string.Format("Failed to enroll member in promotion because the promotion code {0} does not exist.", promoCode));
			}
			return EnrollMemberPromotion(promo, member);
		}

		private MemberPromotion EnrollMemberPromotion(Promotion promotion, Member member, bool useCertificate = false, List<ContextObject.RuleResult> results = null, RuleExecutionMode mode = RuleExecutionMode.Real, bool skipRules = false)
		{
			if (promotion == null)
			{
				throw new ArgumentNullException("promotion");
			}
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			if (member.IpCode < 1)
			{
				throw new ArgumentException("invalid member.", "member");
			}
			if (promotion.Id < 1)
			{
				throw new ArgumentException("invalid promotion.", "promotion");
			}
			if (promotion.EnrollmentSupportType == PromotionEnrollmentSupportType.None)
			{
				throw new ArgumentException(string.Format("Cannot enroll into promotion {0} because enrollment is not supported for the promotion.", promotion.Code));
			}

			bool enrolled = false;
			MemberPromotion ret = null;
			var memberPromos = GetMemberPromotion(member.IpCode, promotion.Code);
			if (memberPromos == null || memberPromos.Count == 0)
			{
				if (promotion.Targeted)
				{
					throw new Exception(string.Format("Cannot enroll member into promotion {0} because the promotion is targeted and the member is not in the promotion", promotion.Code));
				}

				ret = new MemberPromotion() { Code = promotion.Code, MemberId = member.IpCode, Enrolled = true };

				if (useCertificate)
				{
					using (var content = new ContentService(Config))
					{
						var cert = content.RetrieveFirstAvailablePromoCertificate(ContentObjType.Promotion, promotion.Code, null, null);
						if (cert == null)
						{
							throw new LWException(string.Format("Cannot retrieve promotion certificate for promotion code {0}. No more certificates available.", promotion.Code));
						}
						ret.CertificateNmbr = cert.CertNmbr;
					}
				}
				MemberPromotionDao.Create(ret);
				enrolled = true;
			}
			else
			{
				//Framework will allow a member to be targeted for the same promotion, which can result in duplicate records.
				//It doesn't matter which one the member is enrolled in, we'll take the first record.
				ret = memberPromos[0];
				if (!ret.Enrolled)
				{
					ret.Enrolled = true;
					MemberPromotionDao.Update(ret);
					enrolled = true;
				}
			}

			if (enrolled && !skipRules)
			{
				try
				{
					ContextObject co = new ContextObject() { Owner = member, Mode = mode };
					co.Environment.Add(EnvironmentKeys.PromotionCode, promotion.Code);
					co.Environment.Add(EnvironmentKeys.Targeted, promotion.Targeted);
					co.Environment.Add(EnvironmentKeys.Enrollment, (int)promotion.EnrollmentSupportType);
					co.Results = results ?? new List<ContextObject.RuleResult>();
					ExecuteEventRules(co, "PromotionEnrollment", RuleInvocationType.Manual);
				}
				catch (Exception ex)
				{
					_logger.Error(_className, "EnrollMemberPromotion", "Error executing event rules for PromotionEnrollment: " + ex.Message, ex);
				}
			}
			return ret;
		}


		public List<MemberPromotion> GetMemberPromotions(long[] ids)
		{
			return MemberPromotionDao.Retrieve(ids) ?? new List<MemberPromotion>();
		}

		public MemberPromotion GetMemberPromotion(long id)
		{
			return MemberPromotionDao.Retrieve(id);
		}

		public List<MemberPromotion> GetMemberPromotion(long memberId, string code)
		{
			return MemberPromotionDao.Retrieve(memberId, code) ?? new List<MemberPromotion>();
		}

		public long HowManyMemberPromotions(long ipCode)
		{
			return MemberPromotionDao.HowManyMemberPromotions(ipCode);
		}

		public List<MemberPromotion> GetMemberPromotionsByMember(long ipCode, LWQueryBatchInfo batchInfo, bool unExpiredOnly = true)
		{
			return MemberPromotionDao.RetrieveByMember(ipCode, batchInfo, unExpiredOnly) ?? new List<MemberPromotion>();
		}

		public List<MemberPromotion> GetAllMemberPromotion()
		{
			return MemberPromotionDao.RetrieveAll() ?? new List<MemberPromotion>();
		}

		public long HowManyMembersInPromotion(string code)
		{
			return MemberPromotionDao.HowManyMembers(code);
		}

		public bool IsMemberInPromotionList(string code, long memberId)
		{
			return MemberPromotionDao.MemberInPromotionList(code, memberId);
		}

		public bool IsMemberInPromotionList(long memberId)
		{
			return MemberPromotionDao.MemberInPromotionList(memberId);
		}

		public bool IsMemberEnrolledInPromotion(string code, long memberId)
		{
			return MemberPromotionDao.MemberEnrolledInPromotion(code, memberId);
		}

		public void DeleteMemberPromotion(long id)
		{
			MemberPromotionDao.Delete(id);
		}

		public void DeleteMemberPromotion(string code)
		{
			MemberPromotionDao.Delete(code);
		}

		#endregion

		#region Member Stores

		public void SaveMemberStore(MemberStore store)
		{
			if (store.Id > 0)
			{
				MemberStoreDao.Update(store);
			}
			else
			{
				MemberStoreDao.Create(store);
			}
		}

		public void SaveMemberStores(IEnumerable<MemberStore> stores)
		{
			if (stores != null)
			{
				using (var txn = Database.GetTransaction())
				{
					foreach (MemberStore store in stores)
					{
						SaveMemberStore(store);
					}
					txn.Complete();
				}
			}
		}

		public MemberStore GetMemberStore(long id)
		{
			return MemberStoreDao.Retrieve(id);
		}

		public List<MemberStore> GetMemberStoresByMember(long ipCode)
		{
			return MemberStoreDao.RetrieveByMember(ipCode) ?? new List<MemberStore>();
		}

		public void DeleteMemberStore(long storeId)
		{
			MemberStoreDao.Delete(storeId);
		}

		public void DeleteMemberStoreByMember(long memberId)
		{
			MemberStoreDao.DeleteByMember(memberId);
		}
		#endregion

		#region Member Soc(ial) Net(work)

		public void CreateMemberSocNet(MemberSocNet socNet)
		{
			MemberSocNetDao.Create(socNet);
		}

		public void CreateMemberSocNet(long memberId, SocialNetworkProviderType providerType, string providerUID, string properties)
		{
			if (memberId <= -1)
				throw new ArgumentOutOfRangeException("Invalid member for mapping.");
			if (providerType == SocialNetworkProviderType.None)
				throw new ArgumentOutOfRangeException("Can't map to 'none' provider.");
			if (string.IsNullOrEmpty(providerUID))
				throw new ArgumentOutOfRangeException("Invalid provider UID for mapping.");
			if (MemberSocNetDao.RetrieveByProviderUId(providerType, providerUID) != null)
				throw new ArgumentOutOfRangeException(string.Format("Provider UID '{0}' is already mapped.", providerUID));

			MemberSocNet entity = new MemberSocNet();
			entity.MemberId = memberId;
			entity.ProviderType = providerType;
			entity.ProviderUID = providerUID;
			entity.Properties = properties;
			MemberSocNetDao.Create(entity);
		}

		public void UpdateMemberSocNet(MemberSocNet socNet)
		{
			MemberSocNetDao.Update(socNet);
		}

		public Member LoadMemberFromSocNet(SocialNetworkProviderType providerType, string providerUID)
		{
			Member result = null;
			MemberSocNet entity = MemberSocNetDao.RetrieveByProviderUId(providerType, providerUID);
			if (entity != null)
			{
				result = LoadMemberFromIPCode(entity.MemberId);
			}
			return result;
		}

		public List<MemberSocNet> GetSocNetsForMember(long ipCode)
		{
			List<MemberSocNet> result = null;
			List<MemberSocNet> memberSocNets = MemberSocNetDao.RetrieveByMember(ipCode);
			if (memberSocNets != null && memberSocNets.Count > 0)
			{
				result = memberSocNets;
			}
			return result;
		}

		public MemberSocNet GetSocNetForMember(SocialNetworkProviderType providerType, long ipCode, string providerUID = null)
		{
			MemberSocNet result = null;
			List<MemberSocNet> memberSocNets = MemberSocNetDao.RetrieveByMember(ipCode);
			if (memberSocNets != null && memberSocNets.Count > 0)
			{
				foreach (MemberSocNet memberSocNet in memberSocNets)
				{
					if (!string.IsNullOrEmpty(providerUID))
					{
						if (memberSocNet.ProviderType == providerType && memberSocNet.ProviderUID == providerUID)
						{
							result = memberSocNet;
							break;
						}
					}
					else
					{
						if (memberSocNet.ProviderType == providerType)
						{
							result = memberSocNet;
							break;
						}
					}
				}
			}
			return result;
		}

		public string GetSocNetUIDForMember(SocialNetworkProviderType providerType, Member member)
		{
			string result = null;
			List<MemberSocNet> memberSocNets = MemberSocNetDao.RetrieveByMember(member.IpCode);
			if (memberSocNets != null && memberSocNets.Count > 0)
			{
				foreach (MemberSocNet memberSocNet in memberSocNets)
				{
					if (memberSocNet.ProviderType == providerType)
					{
						result = memberSocNet.ProviderUID;
						break;
					}
				}
			}
			return result;
		}

		public List<MemberSocNet> GetMemberSocNets(SocialNetworkProviderType providerType, string[] providerUIDs)
		{
			return MemberSocNetDao.RetrieveByProviderUId(providerType, providerUIDs);
		}

		public List<MemberSocNet> GetMemberSocNets(long ipCode)
		{
			return MemberSocNetDao.RetrieveByMember(ipCode) ?? new List<MemberSocNet>();
		}

		#endregion

		#region MTouch Management

		public void CreateMTouch(MTouchType mtouchType, int howMany, string secondaryId = null, int? usesAllowed = null)
		{
			const string methodName = "CreateMTouch";

			_logger.Trace(_className, methodName, string.Format("Creating {0} mtouch ids for type {1}", howMany, Enum.GetName(typeof(MTouchType), mtouchType)));

			for (int i = 0; i < howMany; i++)
			{
				MTouch mtouch = new MTouch()
				{
					MTouchType = mtouchType,
					UsesAllowed = usesAllowed,
					MTouchValue = Guid.NewGuid().ToString("N")
				};
				if (!string.IsNullOrEmpty(secondaryId))
				{
					mtouch.SecondaryId = secondaryId;
				}
				MTouchDao.Create(mtouch);
			}
		}

		public MTouch CreateMTouch(MTouchType mtouchType, string entityId, string secondaryId = null, bool available = true, int? usesAllowed = null)
		{
			MTouch mtouch = new MTouch()
			{
				MTouchType = mtouchType,
				MTouchValue = Guid.NewGuid().ToString("N"),
				EntityId = entityId,
				SecondaryId = secondaryId,
				Available = available,
				UsesAllowed = usesAllowed
			};
			MTouchDao.Create(mtouch);
			return mtouch;
		}

		public void CreateMTouches(IList<MTouch> mtouches)
		{
			foreach (var mtouch in mtouches)
			{
				MTouchDao.Create(mtouch);
			}
		}

		public MTouch GetMTouch(long mtouchId)
		{
			return MTouchDao.Retrieve(mtouchId);
		}

		public MTouch GetMTouch(string mtouch)
		{
			return MTouchDao.Retrieve(mtouch);
		}

		public long? GetNextMTouchId(MTouchType mtouchType, string entityId)
		{
			MTouch mtouch = MTouchDao.RetrieveNextMTouchId(mtouchType);
			if (mtouch != null)
			{
				mtouch.Available = false;
				mtouch.EntityId = entityId;
				MTouchDao.Update(mtouch);
				return mtouch.ID;
			}
			return null;
		}

		public List<MTouch> GetMTouchByObjectType(MTouchType type, bool onlyAvailable)
		{
			return MTouchDao.RetrieveByObjectType(type, onlyAvailable);
		}

		public int GetMTouchUsageCount(MTouch mTouch)
		{
			return MTouchDao.GetUsageCount(mTouch);
		}

		public int GetMTouchCount(MTouchType type, string secondaryId)
		{
			return MTouchDao.GetMTouchCount(type, secondaryId);
		}

		public List<long> GetMTouchIDs(MTouchType type, bool onlyAvailable, string secondaryID)
		{
			return MTouchDao.RetrieveIDs(type, onlyAvailable, secondaryID) ?? new List<long>();
		}

		public void DeleteMTouches(IList<long> mtouchIDs)
		{
			MTouchDao.DeleteByIDs(mtouchIDs);
		}

		#endregion

		#region TriggerUserEvent Log

		public void CreateTriggerUserEventLog(TriggerUserEventLog log)
		{
			if (!Config.TriggerUserEventLoggingDisabled)
			{
				TriggerUserEventLogDao.Create(log);
			}
		}

		public TriggerUserEventLog GetTriggerUserEventLog(long id)
		{
			return TriggerUserEventLogDao.Retrieve(id);
		}

		public List<long> GetTriggerUserEventLogIds(long memberId, string sortExpression, bool ascending)
		{
			return TriggerUserEventLogDao.Retrieve(memberId, sortExpression, ascending) ?? new List<long>();
		}

		#endregion

		#region Fulfillment Provider

		public FulfillmentProvider GetFulfillmentProvider(long id)
		{
			return FulfillmentProviderDao.Retrieve(id);
		}

		public FulfillmentProvider GetFulfillmentProvider(string name)
		{
			return FulfillmentProviderDao.Retrieve(name);
		}

		public List<FulfillmentProvider> GetAllFulfillmentProviders()
		{
			return FulfillmentProviderDao.RetrieveAll() ?? new List<FulfillmentProvider>();
		}

		#endregion

		#region Mobile Device

		public void CreateMobileDevice(MobileDevice device)
		{
			MobileDeviceDao.Create(device);
		}

		public void UpdateMobileDevice(MobileDevice device)
		{
			MobileDeviceDao.Update(device);
		}

		public long HowManyMobileDevices(long ipcode, DateTime startDate, DateTime endDate)
		{
			return MobileDeviceDao.HowMany(ipcode);
		}

		public MobileDevice GetMobileDevice(long id)
		{
			MobileDevice device = MobileDeviceDao.Retrieve(id);
			return device;
		}

		public MobileDevice GetMobileDeviceByDeviceId(string id)
		{
			MobileDevice device = MobileDeviceDao.RetrieveByDeviceId(id);
			return device;
		}

		public List<MobileDevice> GetMobileDevices(Member member, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();

			List<MobileDevice> list = MobileDeviceDao.RetrieveByMember(member.IpCode, batchInfo);
			if (list == null)
			{
				list = new List<MobileDevice>();
			}
			else
			{
				// sort the members on DateIssued
				var sortedRewards = list.OrderByDescending(d => d.UpdateDate);
				list = sortedRewards.ToList<MobileDevice>();
			}
			return list;
		}

		public MobileDevice GetLatestMobileDevice(Member member)
		{
			MobileDevice device = MobileDeviceDao.RetrieveActiveByMember(member.IpCode);
			//if (device == null)
			//    device = new MobileDevice();
			return device;
		}

		public List<MobileDevice> GetAllMobileDevices(LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();

			List<MobileDevice> devices = MobileDeviceDao.RetrieveAll(batchInfo);
			if (devices == null)
			{
				devices = new List<MobileDevice>();
			}
			else
			{
				// sort the members on DateIssued
				var sortedRewards = devices.OrderByDescending(d => d.UpdateDate);
				devices = sortedRewards.ToList<MobileDevice>();
			}
			return devices;
		}

		public void DeleteMobileDevice(long deviceId)
		{
			MobileDeviceDao.Delete(deviceId);
		}

		public void DeleteMobileDevicesByMember(long memberId)
		{
			MobileDeviceDao.DeleteByMember(memberId);
		}
		#endregion

		#region Push Session

		public void CreatePushSession(PushSession session)
		{
			PushSessionDao.Create(session);
		}

		public void UpdatePushSession(PushSession session)
		{
			PushSessionDao.Update(session);
		}

		public long HowManyPushSessions(long deviceId)
		{
			return PushSessionDao.HowMany(deviceId);
		}

		public long HowManyActivePushSessions(long deviceId)
		{
			return PushSessionDao.HowManyActive(deviceId);
		}

		public PushSession GetPushSession(long id)
		{
			PushSession session = PushSessionDao.Retrieve(id);
			return session;
		}


		public PushSession GetActivePushSessions(long deviceId)
		{
			PushSession session = PushSessionDao.RetrieveActiveSession(deviceId);
			//if (session == null)
			//{
			//    session = new PushSession();
			//}
			return session;
		}

		public List<PushSession> GetPushSessionsByDevice(long deviceId, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();

			List<PushSession> devices = PushSessionDao.RetrieveByDeviceId(deviceId, batchInfo);
			if (devices == null)
			{
				devices = new List<PushSession>();
			}
			else
			{
				// sort the members on DateIssued
				var sortedRewards = devices.OrderByDescending(d => d.EndDate);
				devices = sortedRewards.ToList<PushSession>();
			}
			return devices;
		}

		public void EndPushSession(PushSession session, DateTime endDate)
		{
			PushSessionDao.EndSession(session, endDate);
		}

		#endregion

		#region Reward Choice

		public PetaPoco.Page<RewardChoice> GetRewardChoiceHistory(long memberId, long page, long resultsPerPage)
		{
			return RewardChoiceDao.Retrieve(memberId, page, resultsPerPage);
		}

		public RewardChoice GetCurrentRewardChoice(long memberId)
		{
			return RewardChoiceDao.RetrieveCurrentChoice(memberId);
		}

		public RewardDef GetCurrentRewardChoiceOrDefault(Member member)
		{
			RewardDef ret = null;
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				var tier = member.GetTier(DateTime.Now);
				var choice = RewardChoiceDao.RetrieveCurrentChoice(member.IpCode);

				if (choice != null)
				{
					//make sure the member can still receive this reward (tier hasn't changed)
					ret = content.GetRewardDef(choice.RewardId);
					if (ret == null || tier == null || ret.TierId != tier.TierDefId)
					{
						// one of the following conditions are true: 
						//   member has changed tiers and has not chosen a reward for their new tier yet. 
						//   member is not in a tier
						//   chosen reward does not exist
						// in all cases: choice is invalid. set choice to null.
						choice = null;
						ret = null;
					}
				}

				if (choice == null && tier != null)
				{
					//use default reward choice of member's tier
					var t = GetTierDef(tier.TierDefId);
					if (t.DefaultRewardId.HasValue)
					{
						//make sure the reward still exists and belongs to the tier
						ret = content.GetRewardDef(t.DefaultRewardId.Value);
						if (ret != null && ret.TierId == t.Id)
						{
							choice = new Brierley.FrameWork.Data.DomainModel.RewardChoice(member.IpCode, t.DefaultRewardId.Value);
						}
						else
						{
							ret = null;
						}
					}
				}

				if (ret == null && choice != null)
				{
					ret = content.GetRewardDef(choice.RewardId);
				}
			}
			return ret;
		}

		public void SetMemberRewardChoice(long memberId, long rewardId, string changedBy = null, DateTime? changeDate = null)
		{
			Member member = LoadMemberFromIPCode(memberId);
			if (member == null)
			{
				throw new Exception(string.Format("Member with IPCode {0} not found", memberId));
			}
			SetMemberRewardChoice(member, rewardId, changedBy, changeDate);
		}

		public void SetMemberRewardChoice(Member member, long rewardId, string changedBy = null, DateTime? changeDate = null)
		{
			const string methodName = "SetMemberRewardChoice";
			const string eventName = "RewardChoiceSelection";

			_logger.Trace(_className, methodName, string.Format("Setting member {0} reward choice to {1}", member.IpCode, rewardId));

			var current = GetCurrentRewardChoice(member.IpCode);

			if (current != null && current.RewardId == rewardId)
			{
				_logger.Trace(_className, methodName, string.Format("Member {0} already has reward choice of {1}. Exiting.", member.IpCode, rewardId));
				return;
			}

			//todo: do we need this? The foreign key will prevent a choice that doesn't exist, saving us the time of looking up the reward.
			//you get a less descriptive exception - foreign key violation. 
			RewardDef reward = null;
			using (var content = LWDataServiceUtil.ContentServiceInstance(Organization, Environment))
			{
				reward = content.GetRewardDef(rewardId);
				if (reward == null)
				{
					string error = string.Format("Invalid reward choice {0} for member {1}. The reward definition does not exist.", rewardId, member.IpCode);
					_logger.Error(_className, methodName, error);
					throw new LWException(error) { ErrorCode = 10006 };
				}

				if (!reward.TierId.HasValue)
				{
					string error = string.Format("Invalid reward choice {0} for member {1}. The reward is not associated with a tier.", rewardId, member.IpCode);
					_logger.Error(_className, methodName, error);
					throw new LWException(error) { ErrorCode = 10007 };
				}

				if (!reward.Active || reward.CatalogStartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.Now || reward.CatalogEndDate.GetValueOrDefault(DateTime.MaxValue) <= DateTime.Now)
				{
					string error = string.Format("Invalid reward choice {0} for member {1}. The reward is not active.", rewardId, member.IpCode);
					_logger.Error(_className, methodName, error);
					throw new LWException(error) { ErrorCode = 10010 };
				}
			}

			var tier = member.GetTier(DateTime.Now);
			if (tier == null)
			{
				//this is a problem
				string error = string.Format("Invalid reward choice {0} for member {1}. The member does not belong to a tier and therefore cannot make a reward selection.", rewardId, member.IpCode);
				_logger.Error(_className, methodName, error);
				throw new LWException(error) { ErrorCode = 10008 };
			}

			if (tier.TierDefId != reward.TierId.Value)
			{
				string error = string.Format("Invalid reward choice {0} for member {1}. The reward's tier {2} does not match the member's current tier {3}.", rewardId, member.IpCode, reward.TierId.Value, tier.TierDefId);
				_logger.Error(_className, methodName, error);
				throw new LWException(error) { ErrorCode = 10009 };
			}

			var context = new ContextObject() { Owner = member };
			context.Environment.Add("RewardChoice", rewardId);

			ExecuteEventRules(context, eventName, RuleInvocationType.BeforeInsert);

			var choice = new RewardChoice(member.IpCode, rewardId, changedBy);

			RewardChoiceDao.Create(choice);

			ExecuteEventRules(context, eventName, RuleInvocationType.AfterInsert);
		}

        #endregion

        /// <summary>
        /// This method ensures that you do not create a new member or update a member to use
        /// an existing member's primary business identifiers.
        /// i.e. Loyalty id, username, primary email address or alternate id.
        /// Additionally, it ensures that you don't update an existing terminated member
        /// or set a member to non-member status.
        /// </summary>
        /// <param name="m">The member object being verified</param>
        private void VerifyMember(Member m)
		{
			string methodName = "VerifyMember";
			_logger.Trace(_className, methodName, "Verifying member before updating it.");

            // Get duplicate loyalty ids on the member
            var memberDuplicates = m.LoyaltyCards.GroupBy(v => v.LoyaltyIdNumber).Where(g => g.Count() > 1);
            if (memberDuplicates != null && memberDuplicates.Count() > 0)
            {
                string errMsg = string.Format("Found duplicate loyalty id = {0}.", memberDuplicates.First().Key);
                _logger.Error(_className, methodName, errMsg);
                throw new LWDataServiceException(errMsg) { ErrorCode = 9991 };
            }

            List<Member> members = MemberDao.RetrieveAllByUniqueIdentifiers(m);

            if (members == null || members.Count == 0)
                return;

            var otherMembers = members.Where(x => x.IpCode != m.IpCode);
            if (otherMembers != null && otherMembers.Count() > 0)
            {
                // Get cards owned by other members with the same loyalty id
                var duplicateLoyaltyIds = otherMembers.SelectMany(x => x.LoyaltyCards).Where(v => m.GetLoyaltyCard(v.LoyaltyIdNumber) != null && m.GetLoyaltyCard(v.LoyaltyIdNumber).VcKey != v.VcKey);
                if (duplicateLoyaltyIds != null && duplicateLoyaltyIds.Count() > 0)
                {
                    string errMsg = string.Format("Found existing member with same loyalty id = {0}.", duplicateLoyaltyIds.First().LoyaltyIdNumber);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWDataServiceException(errMsg) { ErrorCode = 9991 };
                }

                if (!string.IsNullOrEmpty(m.PrimaryEmailAddress))
                {
                    // Get members with a duplicate primary email address
                    var duplicateEmails = otherMembers.Where(x => x.PrimaryEmailAddress.Equals(m.PrimaryEmailAddress, StringComparison.InvariantCultureIgnoreCase));
                    if (duplicateEmails != null && duplicateEmails.Count() > 0)
                    {
                        string errMsg = string.Format("Found existing member with same primary email = {0}.", m.PrimaryEmailAddress);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWDataServiceException(errMsg) { ErrorCode = 9991 };
                    }
                }

                if (!string.IsNullOrEmpty(m.AlternateId))
                {
                    // Get members with a duplicate alternate id
                    var duplicateAlternateIds = otherMembers.Where(x => x.AlternateId == m.AlternateId);
                    if (duplicateAlternateIds != null && duplicateAlternateIds.Count() > 0)
                    {
                        string errMsg = string.Format("Found existing member with same alternate id = {0}.", m.AlternateId);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWDataServiceException(errMsg) { ErrorCode = 9991 };
                    }
                }

                if (!string.IsNullOrEmpty(m.Username))
                {
                    // Get members with a duplicate username
                    var duplicateUsernames = otherMembers.Where(x => x.Username == m.Username);
                    if (duplicateUsernames != null && duplicateUsernames.Count() > 0)
                    {
                        string errMsg = string.Format("Found existing member with same username = {0}.", m.Username);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWDataServiceException(errMsg) { ErrorCode = 9991 };
                    }
                }

                _logger.Warning(_className, methodName, 
                    string.Format("Other members found but no exception was thrown. Other ipcodes: {0}", 
                    string.Join(",", otherMembers.Select(o => o.IpCode.ToString()))));
            }

            // Check statuses for existing members
            var existingMember = members.Where(x => x.IpCode == m.IpCode);
            if (existingMember != null && existingMember.Count() > 0)
            {
                // Get duplicate cards owned by the member (i.e. adding a new card with the same loyalty id as an existing card)
                var memberCards = existingMember.SelectMany(x => x.LoyaltyCards)
                    .Where(v => m.GetLoyaltyCard(v.LoyaltyIdNumber) != null && m.GetLoyaltyCard(v.LoyaltyIdNumber).VcKey != v.VcKey);
                if (memberCards != null && memberCards.Count() > 0)
                {
                    string errMsg = string.Format("Found duplicate loyalty id = {0}.", memberCards.First().LoyaltyIdNumber);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWDataServiceException(errMsg) { ErrorCode = 9991 };
                }

                var existing = existingMember.First();
                if (existing.MemberStatus != m.MemberStatus)
                {
                    if (existing.MemberStatus == MemberStatusEnum.Terminated)
                    {
                        throw new LWException(string.Format("Member with ipcode {0} has previosuly been terminated.", existing.IpCode)) { ErrorCode = 9979 };
                    }
                }
                // Make sure that a member is not being changed to a non-member
                if (existing.MemberStatus != MemberStatusEnum.NonMember && m.MemberStatus == MemberStatusEnum.NonMember)
                {
                    string errMsg = string.Format("Existing member with ipcode {0} cannot be changed to non-member status.", existing.IpCode);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWDataServiceException(errMsg) { ErrorCode = 9971 };
                }
            }
        }

		public void ClearRuleResult(IClientDataObject container)
		{
			container.Results.Clear();
			Dictionary<string, List<IClientDataObject>> childAttSets = container.GetChildAttributeSets();
			if (childAttSets != null && childAttSets.Count > 0)
			{
				foreach (List<IClientDataObject> list in childAttSets.Values)
				{
					foreach (IClientDataObject cobj in list)
					{
						ClearRuleResult(cobj);
					}
				}
			}
		}

		public void ClearRuleResult(Member member)
		{
			member.Results.Clear();
			Dictionary<string, List<IClientDataObject>> childAttSets = member.GetChildAttributeSets();
			if (childAttSets != null && childAttSets.Count > 0)
			{
				foreach (List<IClientDataObject> list in childAttSets.Values)
				{
					foreach (IClientDataObject cobj in list)
					{
						ClearRuleResult(cobj);
					}
				}
			}
			foreach (VirtualCard card in member.LoyaltyCards)
			{
				card.Results.Clear();
				Dictionary<string, List<IClientDataObject>> vcChildAttSets = card.GetChildAttributeSets();
				if (vcChildAttSets != null && vcChildAttSets.Count > 0)
				{
					foreach (List<IClientDataObject> list in vcChildAttSets.Values)
					{
						foreach (IClientDataObject cobj in list)
						{
							ClearRuleResult(cobj);
						}
					}
				}
			}
		}

		#region Attribute sets (meta)

		public void CreateAttribute(
			string name,
			DataType dataType,
			long attSetCode,
			bool isRequired,
			bool isUnique,
			AttributeEncryptionType encryptionType,
			long minLength,
			long maxLength,
			string displayText,
			string description,
			string defaultValues,
			bool visibleInGrid = false,
			bool canBeUpdated = false,
			bool isSortable = false,
			bool isMigrationText = false)
		{
			string methodName = "CreateAttribute";
			_logger.Trace(_className, methodName, "creating new attribute " + name);
			AttributeSetMetaData attSet = GetAttributeSetMetaData(attSetCode);
			if (attSet == null)
			{
				string err = string.Format("AttributeSet with code {0} does not exist.", attSetCode);
				_logger.Error(_className, methodName, err);
				throw new LWDataServiceException(err);
			}
			name = name.Trim();
			if (attSet.GetAttribute(name) != null)
			{
				string err = string.Format("Attribute {0} already exists in attribute set {1}.", name, attSet.Name);
				_logger.Error(_className, methodName, err);
				throw new LWDataServiceException(err);
			}
			AttributeMetaData att = new AttributeMetaData();
			att.AttributeSetCode = attSetCode;
			att.DataType = Enum.GetName(typeof(DataType), dataType);
			att.DisplayText = displayText;
			att.Description = description;
			att.EncryptionType = encryptionType;
			att.IsRequired = isRequired;
			att.IsUnique = isUnique;
			att.MaxLength = maxLength;
			att.MinLength = minLength;
			att.Name = name;
			att.Status = 1;
			att.VisibleInGrid = visibleInGrid;
			att.CanBeUpdated = canBeUpdated;
			att.IsSortable = isSortable;
			att.IsMigrationText = isMigrationText;
			att.DefaultValues = defaultValues;
			AttributeMetaDataDao.Create(att);
			CacheManager.Remove(CacheRegions.AttributeSetMetadataById, attSetCode);
			CacheManager.Remove(CacheRegions.AttributeSetMetadataByName, attSet.Name);
		}

		public void CreateAttribute(AttributeMetaData att)
		{
			string methodName = "CreateAttribute";
			_logger.Trace(_className, methodName, "creating new attribute " + att.Name);
			AttributeSetMetaData attSet = GetAttributeSetMetaData(att.AttributeSetCode);
			if (attSet == null)
			{
				string err = string.Format("AttributeSet with code {0} does not exist.", att.AttributeSetCode);
				_logger.Error(_className, methodName, err);
				throw new LWDataServiceException(err);
			}
			att.Name = att.Name.Trim();
			if (attSet.GetAttribute(att.Name) != null)
			{
				string err = string.Format("Attribute {0} already exists in attribute set {1}.", att.Name, attSet.Name);
				_logger.Error(_className, methodName, err);
				throw new LWDataServiceException(err);
			}
			AttributeMetaDataDao.Create(att);
			CacheManager.Remove(CacheRegions.AttributeSetMetadataById, att.AttributeSetCode);
			CacheManager.Remove(CacheRegions.AttributeSetMetadataByName, attSet.Name);
		}

		public void UpdateAttribute(AttributeMetaData att)
		{
			AttributeMetaDataDao.Update(att);
		}

		public AttributeMetaData GetAttribute(long attCode)
		{
			return AttributeMetaDataDao.Retrieve(attCode);
		}

		public void DeleteAttribute(long attCode)
		{
			AttributeMetaDataDao.Delete(attCode);
		}

		public void DeleteAttribute(AttributeSetMetaData attSet, long attCode)
		{
			DeleteAttribute(attCode);
			CacheManager.Remove(CacheRegions.AttributeSetMetadataById, attSet.ID);
			CacheManager.Remove(CacheRegions.AttributeSetMetadataByName, attSet.Name);
		}

		private void CreateDeleteTables(long attSetCode, Generator.ScriptType scriptType)
		{
			string methodName = "CreateDeleteTables";
			AttributeSetMetaData meta = GetAttributeSetMetaData(attSetCode);

			Generator schemaGenerator = new Generator(DatabaseType);
			List<string> sqlScripts = schemaGenerator.GenerateModelScripts(meta, scriptType);
			StringBuilder errors = new StringBuilder();

			using (PetaPoco.Database database = LWDataServiceUtil.GetServiceConfiguration(Organization, Environment).CreateDatabase())
                    foreach (string script in sqlScripts)
                    {
                        try
                        {
                            DataServiceUtil.ExecuteRawSqlCommand(database, script.TrimEnd(';'));
                        }
                        catch (Exception e)
                        {
                            if (!e.Message.Contains("ORA-00942")
                                && !e.Message.Contains("ORA-00972")
                                && !e.Message.Contains("it does not exist or you do not have permission"))
                                errors.Append(string.Format("Error executing SQL: {0}.\n  Error Message: {1}\n\n", script, e.Message));
                            else
                                _logger.Debug(_className, methodName, string.Format("Suppressed error message: {0}.\n", e.Message));
                        }
                    }

			if (errors.Length > 0)
			{
				throw new LWException(errors.ToString());
			}
		}

		public void CreateAttributeSet(AttributeSetMetaData attSet)
		{
			AttributeSetMetaDataDao.Create(attSet);
			CacheManager.Update(CacheRegions.AttributeSetMetadataById, attSet.ID, attSet);
			CacheManager.Update(CacheRegions.AttributeSetMetadataByName, attSet.Name, attSet);
			if (attSet.ParentID != -1)
			{
				// refresh parent in cache.
				AttributeSetMetaData parent = AttributeSetMetaDataDao.Retrieve(attSet.ParentID);
				CacheManager.Update(CacheRegions.AttributeSetMetadataById, parent.ID, parent);
				CacheManager.Update(CacheRegions.AttributeSetMetadataByName, parent.Name, parent);
			}
		}

		public void UpdateAttributeSet(AttributeSetMetaData attSet)
		{
			AttributeSetMetaDataDao.Update(attSet);
			ClearAttributeSetCache(attSet);
		}

		public void DeleteAttributeSet(long attSetCode)
		{
			AttributeSetMetaData meta = GetAttributeSetMetaData(attSetCode);
			if (meta == null)
			{
				throw new LWDataServiceException("No attribute set with id " + attSetCode + " exists.");
			}

			if (!ClientDataObjectDao.IsEmpty(meta.Name))
			{
				throw new LWAttributeSetHasDataException("Attribute set with id " + attSetCode + " cannot be deleted because it has data.");
			}

			using (var txn = Database.GetTransaction())
			{
				AttributeMetaDataDao.DeleteByAttributeSetCode(meta.ID);
				AttributeSetMetaDataDao.Delete(meta.ID);
				txn.Complete();
			}
			ClearAttributeSetCache(meta);
		}

		public bool IsAttributeSetTableCreated(long attSetCode)
		{
			AttributeSetMetaData meta = GetAttributeSetMetaData(attSetCode);
			if (meta == null)
			{
				throw new LWDataServiceException("No attribute set with id " + attSetCode + " exists.");
			}
			return ClientDataObjectDao.TableExists(meta.Name);
		}

		public bool IsAttributeSetTableEmpty(long attSetCode)
		{
			AttributeSetMetaData meta = GetAttributeSetMetaData(attSetCode);
			if (meta == null)
			{
				throw new LWDataServiceException("No attribute set with id " + attSetCode + " exists.");
			}

			if (ClientDataObjectDao.TableExists(meta.Name))
			{
				return ClientDataObjectDao.IsEmpty(meta.Name);
			}
			return true;
		}

		public void CreateAttributeSetTable(long attSetCode)
		{
			AttributeSetMetaData meta = GetAttributeSetMetaData(attSetCode);
			if (meta == null)
			{
				throw new LWDataServiceException("No attribute set with id " + attSetCode + " exists.");
			}

			bool isEmpty = ClientDataObjectDao.IsEmpty(meta.Name);
			if (!isEmpty)
			{
				throw new LWAttributeSetHasDataException("Attribute set with id " + attSetCode + " cannot be deleted because it has data.");
			}

			CreateDeleteTables(attSetCode, Generator.ScriptType.Backout | Generator.ScriptType.Init);
			AttributeSetMetaData m = GetAttributeSetMetaData(attSetCode);
			m.TableCreationDate = DateTime.Now;
			UpdateAttributeSet(m);
		}

		public void DeleteAttributeSetTable(long attSetCode)
		{
			try
			{
				AttributeSetMetaData meta = GetAttributeSetMetaData(attSetCode);
				if (meta == null)
				{
					throw new LWDataServiceException("No attribute set with id " + attSetCode + " exists.");
				}

				if (!ClientDataObjectDao.IsEmpty(meta.Name))
				{
					throw new LWAttributeSetHasDataException("Attribute set with id " + attSetCode + " cannot be deleted because it has data.");
				}

				CreateDeleteTables(attSetCode, Generator.ScriptType.Backout);
				meta.TableCreationDate = null;
				UpdateAttributeSet(meta);
			}
			catch (System.IO.FileNotFoundException)
			{
				// the data assembly has not yet been generated.
			}
		}

		public List<AttributeSetMetaData> GetAllTopLevelAttributeSets()
		{
			List<AttributeSetMetaData> attSets = AttributeSetMetaDataDao.RetrieveTopLevel();
			if (attSets == null)
			{
				attSets = new List<AttributeSetMetaData>();
			}
			else
			{
				// update the attribute set cache
				foreach (AttributeSetMetaData asd in attSets)
				{
					CacheManager.Update(CacheRegions.AttributeSetMetadataById, asd.ID, asd);
					CacheManager.Update(CacheRegions.AttributeSetMetadataByName, asd.Name, asd);
				}
			}
			return attSets;
		}

		public List<AttributeSetMetaData> GetAttributeSetsByType(AttributeSetType type)
		{
			List<AttributeSetMetaData> attSets = AttributeSetMetaDataDao.RetrieveTopLevel(type);
			if (attSets == null)
			{
				attSets = new List<AttributeSetMetaData>();
			}
			else
			{
				// update the attribute set cache
				foreach (AttributeSetMetaData asd in attSets)
				{
					CacheManager.Update(CacheRegions.AttributeSetMetadataById, asd.ID, asd);
					CacheManager.Update(CacheRegions.AttributeSetMetadataByName, asd.Name, asd);
				}
			}
			return attSets;
		}

		public List<AttributeSetMetaData> GetAllChangedAttributeSets(DateTime since)
		{
			return AttributeSetMetaDataDao.RetrieveChangedObjects(since) ?? new List<AttributeSetMetaData>();
		}

		public List<AttributeSetMetaData> GetAllAttributeSets(long[] ids)
		{
			return AttributeSetMetaDataDao.RetrieveByIds(ids) ?? new List<AttributeSetMetaData>();
		}

		public List<AttributeSetMetaData> GetAllAttributeSets()
		{
			return AttributeSetMetaDataDao.RetrieveAll() ?? new List<AttributeSetMetaData>();
		}

		public List<string> GetAttributeSetNames()
		{
			return AttributeSetMetaDataDao.RetrieveAllNames();
		}

		public AttributeSetMetaData GetAttributeSetMetaData(long attributeSetCode)
		{
			AttributeSetMetaData attSet = (AttributeSetMetaData)CacheManager.Get(CacheRegions.AttributeSetMetadataById, attributeSetCode);
			if (attSet == null)
			{
				attSet = AttributeSetMetaDataDao.Retrieve(attributeSetCode);
				if (attSet != null)
				{
					CacheManager.Update(CacheRegions.AttributeSetMetadataById, attSet.ID, attSet);
					CacheManager.Update(CacheRegions.AttributeSetMetadataByName, attSet.Name, attSet);
				}
			}
			return attSet;
		}

		public AttributeSetMetaData GetAttributeSetMetaData(string attributeSetName)
		{
			AttributeSetMetaData attSet = (AttributeSetMetaData)CacheManager.Get(CacheRegions.AttributeSetMetadataByName, attributeSetName);
			if (attSet == null)
			{
				attSet = AttributeSetMetaDataDao.Retrieve(attributeSetName);
				if (attSet != null)
				{
					CacheManager.Update(CacheRegions.AttributeSetMetadataById, attSet.ID, attSet);
					CacheManager.Update(CacheRegions.AttributeSetMetadataByName, attSet.Name, attSet);
				}
			}
			return attSet;
		}

		public bool AttributeSetExists(string attSetName)
		{
			return AttributeSetMetaDataDao.Exists(attSetName);
		}

		private void DeleteAttributeSetData(IAttributeSetContainer link, AttributeSetMetaData attSet)
		{
			string methodName = "DeleteAttributeSetData";

			_logger.Trace(_className, methodName,
				string.Format("Deleting all data associated with attribute set {0} and Parent Code = {1}.", attSet.Name, link.MyKey));

			using (var txn = Database.GetTransaction())
			{
				List<AttributeSetMetaData> children = attSet.ChildAttributeSets;
				if (children != null && children.Count > 0)
				{
					foreach (AttributeSetMetaData childAttSet in children)
					{
						List<IClientDataObject> childAttSets = link.GetChildAttributeSets(attSet.Name);
						foreach (IClientDataObject child in childAttSets)
						{
							DeleteAttributeSetData(child, childAttSet);
						}
					}
				}
				ClientDataObjectDao.DeleteByLink(link, attSet.Name);
				txn.Complete();
			}
		}

		private void DeleteAttributeSetsData(IAttributeSetContainer[] links, AttributeSetMetaData attSet)
		{
			using (var txn = Database.GetTransaction())
			{
				List<AttributeSetMetaData> children = attSet.ChildAttributeSets;
				if (children != null && children.Count > 0)
				{
					foreach (AttributeSetMetaData childAttSet in children)
					{
						List<IClientDataObject> childAttSets = GetAttributeSetObjects((AttributeSetContainer[])links, attSet, null, null, false, false);
						foreach (IClientDataObject child in childAttSets)
						{
							DeleteAttributeSetData(child, childAttSet);
						}
					}
				}
				ClientDataObjectDao.DeleteByLinks(links, attSet.Name);
                txn.Complete();
			}
		}

		#endregion

		#region Data model assembly

		internal void SaveClientDataObjects(IList<IClientDataObject> objects, bool recursive = false)
		{
			ContextObject context = new ContextObject();

			// Save the objects first.
			using (var txn = Database.GetTransaction())
			{
				var createList = objects.Where(o => o.MyKey == -1).ToList();
				var updateList = objects.Where(o => o.MyKey != -1).ToList();

				if (createList.Count > 0)
				{
					foreach (IClientDataObject obj in objects)
					{
						if (obj.GetAttributeSetName().ToLower() == "memberdetails")
						{
							context.Owner = obj.Parent;
							context.InvokingRow = obj;
							context.Mode = RuleExecutionMode.Real;
							context.Results = new List<ContextObject.RuleResult>();
							CheckSMSOptIn(obj, context, RuleInvocationType.BeforeInsert);
						}
					}
					ClientDataObjectDao.Create(createList);
					foreach (IClientDataObject obj in objects)
					{
						if (obj.GetAttributeSetName().ToLower() == "memberdetails")
						{
							context.Owner = obj.Parent;
							context.InvokingRow = obj;
							context.Mode = RuleExecutionMode.Real;
							context.Results = new List<ContextObject.RuleResult>();
							CheckSMSOptIn(obj, context, RuleInvocationType.AfterInsert);
						}
					}
				}
				if (updateList.Count > 0)
				{
					foreach (IClientDataObject obj in objects)
					{
						if (obj.GetAttributeSetName().ToLower() == "memberdetails")
						{
							context.Owner = obj.Parent;
							context.InvokingRow = obj;
							context.Mode = RuleExecutionMode.Real;
							context.Results = new List<ContextObject.RuleResult>();
							CheckSMSOptIn(obj, context, RuleInvocationType.BeforeUpdate);
						}
					}
					ClientDataObjectDao.Update(updateList);
					foreach (IClientDataObject obj in objects)
					{
						if (obj.GetAttributeSetName().ToLower() == "memberdetails")
						{
							context.Owner = obj.Parent;
							context.InvokingRow = obj;
							context.Mode = RuleExecutionMode.Real;
							context.Results = new List<ContextObject.RuleResult>();
							CheckSMSOptIn(obj, context, RuleInvocationType.AfterUpdate);
						}
					}
				}
				if (recursive)
				{
					//save the children.
					var childrenToSave = new List<IClientDataObject>();
					foreach (IClientDataObject parent in objects)
					{
						Dictionary<string, List<IClientDataObject>> children = parent.GetChildAttributeSets();
						if (children != null)
						{
							foreach (IClientDataObject child in children.Values)
							{
								childrenToSave.Add(child);
								if (parent.MyKey == -1)
								{
									if (parent.Parent != null)
									{
										// The parent object for global attribute sets is null.
										parent.ParentRowKey = parent.Parent.MyKey;
									}
									parent.SetLinkKey(parent.Parent, parent.Parent);
								}
							}
						}
					}
					SaveClientDataObjects(childrenToSave);
				}
				txn.Complete();
			}
		}

		public IClientDataObject SaveClientDataObject(IClientDataObject cobj, List<ContextObject.RuleResult> results, RuleExecutionMode mode, bool skipRules = false)
		{
			if (results == null)
			{
				results = new List<ContextObject.RuleResult>();
			}

			ContextObject context = new ContextObject();
			context.Owner = cobj.Parent;
			context.InvokingRow = cobj;
			context.Mode = mode;
			context.Results = results;

			if (cobj.IsDirty)
			{
				skipRules = skipRules || (cobj.Parent == null && cobj.GetMetaData().Type == AttributeSetType.Global); // Skipping global attribute set rules since they typically don't have members
				List<RuleTrigger> ruleTriggers = skipRules ? new List<RuleTrigger>() : cobj.GetMetaData().RuleTriggers ?? new List<RuleTrigger>(); // LW-1188
				if (cobj.MyKey == -1)
				{
					// create this attribute set row

					#region Unique identifier for global objects
					if (cobj.GetMetaData().Type == AttributeSetType.Global)
					{
						PropertyInfo pi = cobj.GetType().GetProperty("LWIdentifier");
						if (pi != null)
						{
							long identifier = (long)pi.GetValue(cobj, null);
							if (identifier <= 0)
							{
								// assign a new identifier value... need data service
								using (var svc = LWDataServiceUtil.DataServiceInstance(Organization, Environment))
									identifier = svc.GetNextID("LWIdentifier");
								pi.SetValue(cobj, identifier, null);
							}
						}
					}
					#endregion

					if (cobj.Parent != null)
					{
						// The parent object for global attribute sets is null.
						cobj.ParentRowKey = cobj.Parent.MyKey;
					}
					cobj.SetLinkKey(cobj.Parent, cobj.Parent);
					if (cobj.GetAttributeSetName().ToLower() == "memberdetails")
					{
						CheckSMSOptIn(cobj, context, RuleInvocationType.BeforeInsert);
					}
					if (ruleTriggers.Count > 0 && !skipRules)
					{
						foreach (RuleTrigger rule in ruleTriggers)
						{
							if (rule.InvocationType == Enum.GetName(typeof(RuleInvocationType), RuleInvocationType.BeforeInsert))
							{
								Execute(rule, context);
							}
						}
					}
					if (mode == RuleExecutionMode.Real)
					{
						ClientDataObjectDao.Create(cobj);
					}
					if (cobj.GetAttributeSetName().ToLower() == "memberdetails")
					{
						CheckSMSOptIn(cobj, context, RuleInvocationType.AfterInsert);
					}
					if (ruleTriggers.Count > 0 && !skipRules)
					{
						Execute(ruleTriggers, context, RuleInvocationType.AfterInsert);
					}
				}
				else
				{
					// update this attribute set row
					if (cobj.GetAttributeSetName().ToLower() == "memberdetails")
					{
						CheckSMSOptIn(cobj, context, RuleInvocationType.BeforeUpdate);
					}
					if (ruleTriggers.Count > 0 && !skipRules) // LW-1188
					{
						Execute(ruleTriggers, context, RuleInvocationType.BeforeUpdate);
					}
					if (mode == RuleExecutionMode.Real)
					{
						ClientDataObjectDao.Update(cobj);
					}
					if (cobj.GetAttributeSetName().ToLower() == "memberdetails")
					{
						CheckSMSOptIn(cobj, context, RuleInvocationType.AfterUpdate);
					}
					if (ruleTriggers.Count > 0 && !skipRules) // LW-1188
					{
						Execute(ruleTriggers, context, RuleInvocationType.AfterUpdate);
					}
				}
			}
			// Save the child attribute sets
			Dictionary<string, List<IClientDataObject>> childAttSets = cobj.GetChildAttributeSets();
			if (childAttSets != null && childAttSets.Count > 0)
			{
				foreach (List<IClientDataObject> list in childAttSets.Values)
				{
					foreach (IClientDataObject chobj in list)
					{
						SaveClientDataObject(chobj, results, mode, skipRules);
					}
				}
			}
			cobj.IsDirty = false;
			return cobj;
		}

		public void LoadAttributeSetList(IAttributeSetContainer owner, AttributeSetMetaData meta, bool deep)
		{
			const string methodName = "LoadAttributeSetList";

			try
			{
                AttributeSetType expectedType = 0;

				if (owner is Member)
				{
					expectedType = AttributeSetType.Member;
				}
				else if (owner is VirtualCard)
				{
					expectedType = AttributeSetType.VirtualCard;
				}

				if (meta == null || (meta.Type != expectedType && meta.ParentID == -1 ))
				{
					if (meta.Type == AttributeSetType.VirtualCard && expectedType == AttributeSetType.Member)
					{
						foreach (VirtualCard vc in ((Member)owner).LoyaltyCards)
							LoadAttributeSetList(vc, meta, deep);
						return;
					}
					else if (meta != null)
					{
						throw new LWMetaDataException(
							string.Format(
								"This attribute set is not of the expected type while loading attribute set {0}.  Expected type was {1} while received {2}",
								meta.Name,
								Enum.GetName(typeof(AttributeSetType), expectedType),
								Enum.GetName(typeof(AttributeSetType), meta.Type)));
					}
					else
					{
						throw new LWMetaDataException("Unexpected type of attributeset to load - No meta provided.");
					}
				}

				IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
				List<IClientDataObject> attSets = ClientDataObjectDao.Retrieve(emptyObj.GetType(), owner, LockingMode.None);
				owner.SetChildAttributeSet(meta.Name, attSets);
				if (attSets != null)
				{
					foreach (IClientDataObject attSet in attSets)
					{
						attSet.IsDirty = false;
						attSet.Parent = owner;
						attSet.ParentRowKey = owner.MyKey;
						if (deep)
						{
							LoadAttributeSetList(attSet, deep);
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error loading attribute sets.", ex);
				throw;
			}
		}

		public void LoadAttributeSetList(IAttributeSetContainer owner, string attSetName, bool deep)
		{
			const string methodName = "LoadAttributeSetList";

			AttributeSetMetaData meta = GetAttributeSetMetaData(attSetName);
			if (meta == null)
			{
				string errMsg = string.Format("Attributue set {0} does not exist.", attSetName);
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg) { ErrorCode = 9987 };
			}
			try
			{
				LoadAttributeSetList(owner, meta, deep);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error loading attribute set " + attSetName + ".", ex);
				throw;
			}
		}

		public void LoadAttributeSetList(IAttributeSetContainer owner, bool deep)
		{
			List<AttributeSetMetaData> metaList = owner.GetMetaData().ChildAttributeSets;
			if (metaList != null)
			{
				foreach (AttributeSetMetaData meta in metaList)
				{
					LoadAttributeSetList(owner, meta, deep);
				}
			}
		}

		public IClientDataObject SaveAttributeSetObject(IClientDataObject cobj, List<ContextObject.RuleResult> results = null, RuleExecutionMode mode = RuleExecutionMode.Real, bool skipRules = false)
		{
			return SaveClientDataObject(cobj, results, mode, skipRules);
		}

		public List<IClientDataObject> GetAttributeSetObjects(string attSetName, long[] rowKeys, bool obtainLock = false)
		{
			const string methodName = "GetAttributeSetObjects";

			LockingMode lockMode = obtainLock ? LockingMode.Upgrade : LockingMode.None;
			AttributeSetMetaData meta = GetAttributeSetMetaData(attSetName);
			if (meta == null)
			{
				string errMsg = string.Format("Attributue set {0} does not exist.", attSetName);
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg) { ErrorCode = 9987 };
			}

			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
			List<IClientDataObject> attSetList = ClientDataObjectDao.Retrieve(emptyObj.GetType(), rowKeys, lockMode);
			if (attSetList == null)
			{
				attSetList = new List<IClientDataObject>();
			}
			return attSetList;
		}

		public List<IClientDataObject> GetAttributeSetObjects(IAttributeSetContainer owner, AttributeSetMetaData meta, LWCriterion criteria, LWQueryBatchInfo batchInfo, bool deep, bool obtainLock = false)
		{
			if (batchInfo != null) batchInfo.Validate();

			EvaluatedCriterion whereClause = null;
			List<string> distinct = null;

			LockingMode lockMode = obtainLock ? LockingMode.Upgrade : LockingMode.None;

			string orderBy = string.Empty;
			if (criteria != null)
			{
				whereClause = criteria.Evaluate();
				orderBy = criteria.EvaluateOrderBy();
				distinct = criteria.GetDisticntColumns();
			}

			string alias = criteria != null && criteria.UseAlias ? criteria.Alias : string.Empty;

			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
			if (distinct == null || distinct.Count == 0)
			{
				return ClientDataObjectDao.Retrieve(emptyObj.GetType(), owner, alias, whereClause, orderBy, batchInfo, lockMode);
			}
			else
			{
				return ClientDataObjectDao.Retrieve(emptyObj.GetType(), owner, alias, whereClause, distinct, orderBy, batchInfo, lockMode);
			}
		}

		public List<IClientDataObject> GetAttributeSetObjects(IAttributeSetContainer owner, string attSetName, LWCriterion criteria, int limitNumber, bool obtainLock = false)
		{
			EvaluatedCriterion whereClause = null;
			List<string> distinct = null;

			LockingMode lockMode = obtainLock ? LockingMode.Upgrade : LockingMode.None;

			string orderBy = string.Empty;
			if (criteria != null)
			{
				whereClause = criteria.Evaluate();
				orderBy = criteria.EvaluateOrderBy();
				distinct = criteria.GetDisticntColumns();
			}

			string alias = criteria != null && criteria.UseAlias ? criteria.Alias : string.Empty;
			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(attSetName);

			return ClientDataObjectDao.Retrieve(emptyObj.GetType(), owner, alias, whereClause, orderBy, limitNumber, lockMode);
		}

		public IClientDataObject GetAttributeSetObject(string attSetName, long key, bool deep, bool obtainLock = false)
		{
			string methodName = "GetAttributeSetObject";

			LockingMode lockMode = obtainLock ? LockingMode.Upgrade : LockingMode.None;
			AttributeSetMetaData meta = GetAttributeSetMetaData(attSetName);
			if (meta == null)
			{
				string errMsg = string.Format("Attributue set {0} does not exist.", attSetName);
				_logger.Error(_className, methodName, errMsg);
				throw new LWDataServiceException(errMsg) { ErrorCode = 9987 };
			}
			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
			IClientDataObject attSet = ClientDataObjectDao.Retrieve(emptyObj.GetType(), key, lockMode);
			return attSet;
		}

		public List<IClientDataObject> GetAttributeSetObjects(IAttributeSetContainer owner, string attSetName, LWCriterion criteria, LWQueryBatchInfo batchInfo, bool deep, bool obtainLock = false)
		{
			AttributeSetMetaData meta = GetAttributeSetMetaData(attSetName);
			return GetAttributeSetObjects(owner, meta, criteria, batchInfo, deep, obtainLock);
		}

		public List<IClientDataObject> GetAttributeSetObjects(AttributeSetContainer[] owners, AttributeSetMetaData meta, LWCriterion criteria, LWQueryBatchInfo batchInfo, bool deep, bool obtainLock = false)
		{
			if (batchInfo != null) batchInfo.Validate();

			EvaluatedCriterion whereClause = null;
			List<string> distinct = null;

			LockingMode lockMode = obtainLock ? LockingMode.Upgrade : LockingMode.None;

			string orderBy = string.Empty;
			if (criteria != null)
			{
				whereClause = criteria.Evaluate();
				orderBy = criteria.EvaluateOrderBy();
				distinct = criteria.GetDisticntColumns();
			}

			string alias = criteria != null && criteria.UseAlias ? criteria.Alias : string.Empty;

			List<IClientDataObject> result = null;
			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
			if (distinct == null || distinct.Count == 0)
			{
				result = ClientDataObjectDao.Retrieve(emptyObj.GetType(), owners, alias, whereClause, orderBy, batchInfo, lockMode);
			}
			else
			{
				result = ClientDataObjectDao.Retrieve(emptyObj.GetType(), owners, alias, whereClause, distinct, orderBy, batchInfo, lockMode);
			}
			if (result != null && owners != null)
			{
				foreach (IClientDataObject dataObj in result)
				{
					foreach (AttributeSetContainer owner in owners)
					{
						if (owner.MyKey == dataObj.ParentRowKey)
						{
							dataObj.Parent = owner;
							break;
						}
					}
				}
			}
			return result ?? new List<IClientDataObject>();
		}

		public List<long> GetAttributeSetObjectIds(AttributeSetContainer[] owners, AttributeSetMetaData meta, LWCriterion criteria)
		{
			EvaluatedCriterion whereClause = null;

			string orderBy = string.Empty;
			if (criteria != null)
			{
				whereClause = criteria.Evaluate();
				orderBy = criteria.EvaluateOrderBy();
			}

			string alias = criteria != null && criteria.UseAlias ? criteria.Alias : string.Empty;

			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
			return ClientDataObjectDao.RetrieveIds(emptyObj.GetType(), owners, alias, whereClause, orderBy) ?? new List<long>();
		}

		public List<long> GetChangedAttributeSetObjectIds(AttributeSetMetaData meta, DateTime since)
		{
			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
			return ClientDataObjectDao.RetrieveChangedObjects(emptyObj.GetType(), since) ?? new List<long>();
		}

		public List<IClientDataObject> GetAttributeSetObjects(IAttributeSetContainer owner, AttributeSetMetaData meta, string alias, string whereClause, string orderBy, LWQueryBatchInfo batchInfo, bool deep, bool obtainLock = false)
		{
			if (batchInfo != null) batchInfo.Validate();

			LockingMode lockMode = obtainLock ? LockingMode.Upgrade : LockingMode.None;
			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
			List<IClientDataObject> globalAttSetList = ClientDataObjectDao.Retrieve(emptyObj.GetType(), owner, alias, whereClause, orderBy, batchInfo, lockMode);
			if (globalAttSetList == null)
			{
				globalAttSetList = new List<IClientDataObject>();
			}
			else
			{
				foreach (IClientDataObject dataObj in globalAttSetList)
				{
					if (owner != null && owner.MyKey == dataObj.ParentRowKey)
					{
						dataObj.Parent = owner;
					}
				}
			}
			if (deep)
			{
				//foreach (IClientDataObject gObj in globalAttSetList)
				//{
				//    //LOad its child attribute sets.
				//}
			}
			return globalAttSetList;
		}

		public long CountAttributeSetObjects(IAttributeSetContainer owner, AttributeSetMetaData meta, string whereClause)
		{
			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
			long howMany = ClientDataObjectDao.HowMany(emptyObj.GetType(), owner, "a", whereClause);
			return howMany;
		}

		public long CountAttributeSetObjects(IAttributeSetContainer owner, AttributeSetMetaData meta, LWCriterion criteria)
		{
			EvaluatedCriterion whereClause = criteria != null ? criteria.Evaluate() : null;
			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(meta.Name);
			string alias = criteria != null && criteria.UseAlias ? criteria.Alias : string.Empty;
			//string alias = criteria.UseAlias ? criteria.Alias : string.Empty;
			//string alias = criteria != null ? criteria.Alias : "a";
			long howMany = ClientDataObjectDao.HowMany(emptyObj.GetType(), owner, alias, whereClause);
			return howMany;
		}

		public int DeleteClientDataObjects(string attributeSetName, long[] keys)
		{
			IClientDataObject emptyObj = DataServiceUtil.GetNewClientDataObject(attributeSetName);
			return ClientDataObjectDao.Delete(emptyObj.GetType(), keys);
		}

		#endregion

		#region Rule Execution

		public void ExecuteEventRules(ContextObject context, string eventName, RuleInvocationType invocation = RuleInvocationType.Manual)
		{
			const string methodName = "ExecuteEventRules";

			_logger.Debug(_className, methodName, string.Format("Executing rules for event: {0}. InvocationType: {1}", eventName, invocation));

			Func<RuleInvocationType, bool> isQueueableInvocation = delegate (RuleInvocationType t)
			{
				return t == RuleInvocationType.AfterInsert || t == RuleInvocationType.AfterUpdate || t == RuleInvocationType.Manual;
			};

			//.ToList() <-- this collection may be frequently modified. It was previously wrapped in a lock statement, which 
			//effectively turned event processing single threaded. 
			List<RuleTrigger> rules = GetRuleByObjectName(eventName).ToList();
			bool eventIsQueued = false;
			if (
				QueueingEnabled &&
				isQueueableInvocation(invocation) &&
				context != null &&
				context.Owner is Member &&
				rules.Count(o => o.CanQueue && o.ProperInvocationType == invocation) > 0
				)
			{
				//queue the event.
				var m = new RuleMessage(((Member)context.Owner).IpCode, invocation, eventName);
				Messaging.MessagingBus.Instance().Send<RuleMessage>(m);
				eventIsQueued = true;
			}

			foreach (RuleTrigger rule in rules.ToList())
			{
				if (rule.ProperInvocationType == invocation)
				{
					try
					{
						if (!eventIsQueued || !rule.CanQueue || !isQueueableInvocation(rule.ProperInvocationType))
						{
							Execute(rule, context, eventIsQueued);
						}
					}
					catch (Exception ex)
					{
						string msg = string.Format("Error executing rule {0}.  Error message: {1}", rule.RuleName, ex.Message);
						_logger.Error(_className, methodName, msg, ex);
						throw;
					}
				}
			}
		}

		public void Execute(RuleTrigger trigger, IAttributeSetContainer owner, IClientDataObject currentRow, List<ContextObject.RuleResult> results, RuleExecutionMode mode)
		{
			ContextObject context = new ContextObject();
			context.Owner = owner;
			context.InvokingRow = currentRow;
			context.Mode = mode;
			context.Results = results;
			Execute(trigger, context);
		}

		public void Execute(RuleTrigger trigger, ContextObject contextobj)
		{
			Execute(trigger, contextobj, false);
		}

		private void Execute(RuleTrigger trigger, ContextObject contextobj, bool eventWasQueued)
		{

            LWRuleExecutionLogger ruleLogger = new LWRuleExecutionLogger(LWConfigurationUtil.GetCurrentConfiguration());

			const string methodName = "Execute";
			if (trigger == null)
			{
				throw new ArgumentNullException("trigger");
			}
			if (contextobj == null)
			{
				throw new ArgumentNullException("contextobj");
			}

			Member m = null;
			VirtualCard v = null;
			trigger.Rule.ResolveOwners(contextobj.Owner, ref m, ref v);

			if (
				QueueingEnabled &&
				trigger.CanQueue &&
				!eventWasQueued &&
				(
				trigger.ProperInvocationType == RuleInvocationType.AfterInsert ||
				trigger.ProperInvocationType == RuleInvocationType.AfterUpdate ||
				trigger.ProperInvocationType == RuleInvocationType.Manual
				))
			{
				//queue the trigger.
				//todo: this is completely shaky: we need 
				var message = new RuleMessage(m.IpCode, trigger.ProperInvocationType, contextobj.InvokingRow.GetAttributeSetName(), contextobj.InvokingRow.MyKey, contextobj.Environment);
				if (v != null)
				{
					message.VCKey = v.VcKey;
				}
				Messaging.MessagingBus.Instance().Send<RuleMessage>(message);
				return;
			}

			Func<ContextObject.RuleResult.RuleSkippedReason, ContextObject.RuleResult> skipResult = (ContextObject.RuleResult.RuleSkippedReason reason) =>
			{
				return new ContextObject.RuleResult(
					m != null ? m.IpCode : -1,
					trigger.RuleName,
					trigger.Rule.GetType(),
					reason);
			};

			try
			{
				//check trigger's date range
				if (!trigger.IsActive)
				{
					_logger.Trace(_className, methodName, "Rule is not within date range.");
					contextobj.Results.Add(skipResult(ContextObject.RuleResult.RuleSkippedReason.RuleNotInDateRange));
					return;
				}

				if (trigger.Rule == null)
				{
					_logger.Error(_className, methodName, string.Format("Rule instance for  {0} is null.", trigger.RuleName));
					contextobj.Results.Add(skipResult(ContextObject.RuleResult.RuleSkippedReason.RuleInstanceNull));
					return;
				}

				if (!string.IsNullOrEmpty(trigger.PromotionCode))
				{
					//todo: trigger.Targeted may not always reflect the promotion's targeted flag.
					/*
					 * When configuring rules in the promotion application, the rule's Targeted flag is automatically set to match the promotion's 
					 * Targeted flag. This would mean that you can rely on the rule's flag to know if it belongs to a targeted promotion, but...
					 * In data modeling, under either the Rule Events or Rule Triggers tabs, you can check/uncheck the targeted flag and there
					 * is no validation to ensure that the flag matches the promotion the rule is tied to.
					 * 
					 * We should consider removing the rule's Targetd flag and just rely on the promotion's flag. It may have been done this way 
					 * to prevent the extra database call to get the promotion definition, but - as of 4.6.3 - we have to make the call to check
					 * the promotion's enrollment support type.
					 * 
					 * Since we have to make the call to get the promotion, we'll ignore the trigger's Targeted property and use the promotion's...
					 * 
					//if (trigger.Targeted)

					ContextObject.PromotionContext promoContext = null;

					if (contextobj.PromotionContexts.ContainsKey(trigger.PromotionCode))
					{
						//pull info from the context
						promoContext = contextobj.PromotionContexts[trigger.PromotionCode];
					}
					else
					{
						//lookup promotion

						Promotion promo = GetPromotionByCode(trigger.PromotionCode);
						if (promo == null)
						{
							throw new LWException(string.Format("Rule trigger is linked to promotion {0}, but the promotion does not exist.", trigger.PromotionCode));
						}

						if (promo.Targeted && m == null)
						{
							_logger.Trace(_className, methodName, string.Format("{0} rule is configured for a targeted promotion, but no member was passed.", trigger.RuleName));
							contextobj.Results.Add(skipResult(ContextObject.RuleResult.RuleSkippedReason.RuleTargetedAndNoMember));
							return;
						}

						if (!promo.IsValid())
						{
							_logger.Trace(_className, methodName, trigger.PromotionCode + " is not valid anymore.");
							contextobj.Results.Add(skipResult(ContextObject.RuleResult.RuleSkippedReason.PromotionNotValid));
							return;
						}

						bool enrolled = false;
						if (promo.EnrollmentSupportType != PromotionEnrollmentSupportType.None)
						{
							enrolled = IsMemberEnrolledInPromotion(trigger.PromotionCode, m.IpCode);
						}

						if (!enrolled)
						{
							//need to be enrolled in the promotion
							if (promo.EnrollmentSupportType == PromotionEnrollmentSupportType.Required)
							{
								_logger.Trace(_className, methodName, string.Format("{0} is not enrolled in promotion {1}", m.IpCode, trigger.PromotionCode));
								contextobj.Results.Add(skipResult(ContextObject.RuleResult.RuleSkippedReason.MemberNotEnrolledInPromotion));
								return;
							}
							if (promo.Targeted && !IsMemberInPromotionList(trigger.PromotionCode, m.IpCode))
							{
								//need to be in the promotion, but not necessarily enrolled (if enrolled, then we know the member is targeted)
								_logger.Trace(_className, methodName, string.Format("{0} is not in promotion {1}", m.IpCode, trigger.PromotionCode));
								contextobj.Results.Add(skipResult(ContextObject.RuleResult.RuleSkippedReason.MemberNotInPromotion));
								return;
							}
						}
						promoContext = new ContextObject.PromotionContext(trigger.Targeted, promo.EnrollmentSupportType, enrolled);
						contextobj.PromotionContexts.Add(trigger.PromotionCode, promoContext);
					}

					contextobj.Environment["PromotionCode"] = trigger.PromotionCode;
					contextobj.Environment["Targeted"] = promoContext.Targeted;
					contextobj.Environment["Enrollment"] = (int)promoContext.EnrollmentType;
					contextobj.Environment["Enrolled"] = promoContext.Enrolled;
				}

				_logger.Debug(_className, methodName, string.Format("Attempting to invoke rule {0}.", trigger.RuleName));

				Expression expression = null;
				try
				{
					if (!string.IsNullOrEmpty(trigger.ConditionalExpression))
					{
						_logger.Debug(_className, methodName,
							string.Format("Conditional expression for rule {0} is {1}", trigger.RuleName, trigger.ConditionalExpression));
						expression = new ExpressionFactory().Create(trigger.ConditionalExpression);
					}
					//weak. Why create something that eats resources when we can just ignore it?
					//else
					//{
					//	expression = new ExpressionFactory().Create("1==1");
					//}
				}
				catch (System.Exception ex)
				{
                    string msg = string.Format("The conditional expression associated with {0} failed to compile. {1}", trigger.RuleName, ex.Message);
					contextobj.Results.Add(new ContextObject.RuleResult()
					{
						MemberId = m != null ? m.IpCode : -1,
						Name = trigger.RuleName,
						RuleType = trigger.Rule.GetType(),
						SkipReason = ContextObject.RuleResult.RuleSkippedReason.RuleExpressionException,
						Detail = msg
					});
					_logger.Error(_className, methodName, string.Format("Error while executing rule {0}. The conditional expression failed to compile.", trigger.RuleName), ex);
					throw new CRMException(msg);
				}
				bool result = true;
				if (expression != null)
				{
					try
					{
						result = (bool)expression.evaluate(contextobj);
					}
					catch (Exception ex)
					{
						string msg = string.Format("Error while evaluating evaluation criteria {0}: {1}.", trigger.ConditionalExpression, ex.Message);
						contextobj.Results.Add(new ContextObject.RuleResult(
							m != null ? m.IpCode : -1,
							trigger.RuleName,
							trigger.Rule.GetType(),
							ContextObject.RuleResult.RuleSkippedReason.RuleExpressionException)
						{ Detail = msg });
						if (trigger.ContinueOnError)
						{
							msg = string.Format("{0} threw an error in its evaluate criteria - {1}.  However, its ContuneOnError flag is true.  Continuing...", trigger.RuleName, ex.Message);
							_logger.Error(_className, methodName, msg);
							return;
						}
						else
						{
							msg = string.Format("{0} threw an error in its evaluate criteria.  Specific error: {1}.", trigger.RuleName, ex.Message);
							_logger.Error(_className, methodName, msg);
							throw;
						}
					}
				}

				if (!result) // the condition evals to false.
				{
					_logger.Debug(_className, methodName, string.Format("Conditional expression {0} for rule {1} did not evaluate to true.", trigger.ConditionalExpression, trigger.RuleName));
					if (contextobj.Results.Count == 0)
					{
						contextobj.Results.Add(skipResult(ContextObject.RuleResult.RuleSkippedReason.RuleExpressionNotMet));
					}
					return;
				}

				if (result)
				{
					_logger.Debug(_className, methodName, "Invoking rule " + trigger.RuleName);
					try
					{
						contextobj.Name = trigger.RuleName;
						trigger.Rule.Invoke(contextobj);
						return;
					}
					catch (Exception ex)
					{
						string msg = string.Format("{0} threw an error: {1}.", trigger.RuleName, ex.Message);
						contextobj.Results.Add(new ContextObject.RuleResult()
						{
							MemberId = m != null ? m.IpCode : -1,
							Name = trigger.RuleName,
							RuleType = trigger.Rule.GetType(),
							SkipReason = ContextObject.RuleResult.RuleSkippedReason.RuleExecutionError,
							Detail = msg
						});
						if (trigger.ContinueOnError)
						{
							msg = string.Format("{0} threw an error.  However, its ContuneOnError flag is true.  Continuing...", trigger.RuleName);
							_logger.Error(_className, methodName, msg, ex);
							return;
						}
						else
						{
							throw;
						}
					}
				}
			}
			finally
			{
				contextobj.Environment.Remove("PromotionCode");
				contextobj.Environment.Remove("Targeted");
				contextobj.Environment.Remove("Enrollment");
				contextobj.Environment.Remove("Enrolled");

				if (trigger.LogExecution && !Config.RuleExecutionLoggingDisabled)
				{
					RuleExecutionLog log = new RuleExecutionLog()
					{
						RuleName = trigger.RuleName,
						ExecutionMode = contextobj.Mode,
						OwnerType = PointTransactionOwnerType.Unknown
					};

					List<ContextObject.RuleResult> results = contextobj.Results;

					ContextObject.RuleResult result = null;
					if (results == null || results.Count == 0)
					{
						log.ExecutionStatus = RuleExecutionStatus.Success;
					}
					else
					{
						result = results[results.Count - 1];

						log.MemberId = result.MemberId;
						Type rType = result.GetType();
						log.OwnerType = result.OwnerType ?? PointTransactionOwnerType.Unknown;
						log.OwnerId = result.OwnerId;
						log.RowKey = result.RowKey;

						if (result.SkipReason == null)
						{
							log.ExecutionStatus = RuleExecutionStatus.Success;
						}
						else
						{
							bool hasError = results.Where(x =>
									x.SkipReason != null &&
									(
									x.SkipReason.Value == ContextObject.RuleResult.RuleSkippedReason.RuleExecutionError ||
									x.SkipReason.Value == ContextObject.RuleResult.RuleSkippedReason.RuleExpressionException
									)
								).Count() > 0;
							if (hasError)
							{
								log.ExecutionStatus = RuleExecutionStatus.Error;
							}
							else
							{
								log.ExecutionStatus = RuleExecutionStatus.Skipped;
							}
							log.SkipReason = result.SkipReason.Value.ToString();
						}
						log.Detail = StringUtils.FriendlyString(result.Detail, string.Empty, 150);
					}
                    
                    //LW-3792 Use logger isntead of writing directly to table
                    ruleLogger.ProcessRequest(log);


                    if (result != null)
					{
						result.ExecutionLogId = log.Id;
					}
				}
			}
		}

		/// <summary>
		/// Executes a set of rule triggers, invoked by action on an attribute set row. 
		/// </summary>
		/// <remarks>
		/// This allows us to queue a single message that represents the entire list of rules to fire.
		/// </remarks>
		/// <param name="triggers"></param>
		/// <param name="owner"></param>
		/// <param name="currentRow"></param>
		/// <param name="results"></param>
		/// <param name="mode"></param>
		private void Execute(IEnumerable<RuleTrigger> triggers, ContextObject context, RuleInvocationType invocation)
		{
			if (triggers == null)
			{
				throw new ArgumentNullException("triggers");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (context.InvokingRow == null)
			{
				throw new ArgumentException("ContextObject must have an InvokingRow");
			}
			if (context.Owner == null)
			{
				throw new ArgumentException("ContextObject must have an Owner");
			}

			triggers = triggers.Where(o => o.ProperInvocationType == invocation);

			if (triggers.Count() < 1)
			{
				return;
			}

			Member m = null;
			VirtualCard v = null;
			triggers.First().Rule.ResolveOwners(context.Owner, ref m, ref v);

			Func<RuleInvocationType, bool> isQueueable = (RuleInvocationType t) => { return t == RuleInvocationType.AfterInsert || t == RuleInvocationType.AfterUpdate || t == RuleInvocationType.Manual; };
			bool invocationQueueable = isQueueable(invocation);

			var executeNow = QueueingEnabled && invocationQueueable ?
				triggers.Where(o => !o.CanQueue) :
				triggers;

			foreach (var rule in executeNow)
			{
				Execute(rule, context);
			}

			if (QueueingEnabled && invocationQueueable && triggers.Count(o => o.ProperInvocationType == invocation && o.CanQueue) > 0)
			{
				var message = new RuleMessage(m.IpCode, invocation, context.InvokingRow.GetAttributeSetName(), context.InvokingRow.MyKey, context.Environment);
				if (v != null)
				{
					message.VCKey = v.VcKey;
				}
				Messaging.MessagingBus.Instance().Send<RuleMessage>(message);
				return;
			}
		}

		protected void CreateRuleExecutionLog(RuleExecutionLog log)
		{
			RuleExecutionLogDao.Create(log);
		}

		public List<RuleExecutionLog> GetRuleExecutionLogs(long memberId, RuleExecutionStatus? status, PointTransactionOwnerType? ownerType, long? ownerId, long[] rowkeys, DateTime? fromDate, DateTime? toDate)
		{
			return RuleExecutionLogDao.Retrieve(memberId, status, ownerType, ownerId, rowkeys, fromDate, toDate) ?? new List<RuleExecutionLog>();
		}

		#endregion

		#region Tiers

		#region Member Tier

		public void CreateMemberTier(MemberTier tier)
		{
			MemberTierDao.Create(tier);
		}

		public void UpdateMemberTier(MemberTier tier)
		{
			MemberTierDao.Update(tier);
		}

		public MemberTier GetMemberTier(long id)
		{
			return MemberTierDao.Retrieve(id);
		}

		public List<MemberTier> GetMemberTiers(Member member)
		{
			return MemberTierDao.RetrieveByMember(member.IpCode) ?? new List<MemberTier>();
		}

		public List<MemberTier> GetAllMemberTiers()
		{
			return MemberTierDao.RetrieveAll() ?? new List<MemberTier>();
		}

		public MemberTier GetMemberTier(Member member, DateTime date)
		{
			return MemberTierDao.RetrieveByMember(member.IpCode, date);
		}

		public bool IsMemberInTier(Member member, string tierName)
		{
			return MemberTierDao.RetrieveByMember(member.IpCode, tierName, DateTime.Now) != null;
		}

		public bool IsMemberInTier(Member member, string tierName, DateTime date)
		{
			return MemberTierDao.RetrieveByMember(member.IpCode, tierName, date) != null;
		}

		public void DeleteMemberTier(long tierId)
		{
			MemberTierDao.Delete(tierId);
		}

		#endregion

		#region Tier Def

		public void CreateTierDef(TierDef tier)
		{
			TierDao.Create(tier);
			CacheManager.Update(CacheRegions.TierByName, tier.Name, tier);
            CacheManager.Update(CacheRegions.TierById, tier.Id, tier);
			CacheManager.Remove(CacheRegions.Tiers, "all");
		}

		public void UpdateTierDef(TierDef tier)
		{
			TierDao.Update(tier);
			CacheManager.Update(CacheRegions.TierByName, tier.Name, tier);
            CacheManager.Update(CacheRegions.TierById, tier.Id, tier);
            CacheManager.Remove(CacheRegions.Tiers, "all");
		}

		public List<TierDef> GetAllTierDefs()
		{
			List<TierDef> tiers = CacheManager.Get(CacheRegions.Tiers, "all") as List<TierDef>;
			if (tiers == null)
			{
				tiers = TierDao.RetrieveAll() ?? new List<TierDef>();
				foreach (TierDef tier in tiers)
				{
					CacheManager.Update(CacheRegions.TierByName, tier.Name, tier);
                    CacheManager.Update(CacheRegions.TierById, tier.Id, tier);
                }
				CacheManager.Update(CacheRegions.Tiers, "all", tiers);
			}
			return tiers;
		}

		public TierDef GetTierDef(long tierId)
		{
            TierDef tier = (TierDef)CacheManager.Get(CacheRegions.TierById, tierId);
            if (tier == null)
            {
                tier = TierDao.Retrieve(tierId);
                if (tier != null)
                {
                    CacheManager.Update(CacheRegions.TierByName, tier.Name, tier);
                    CacheManager.Update(CacheRegions.TierById, tier.Id, tier);
                }
            }
			return tier;
		}

		public TierDef GetTierDef(string tierName)
		{
			TierDef tier = (TierDef)CacheManager.Get(CacheRegions.TierByName, tierName);
			if (tier == null)
			{
				tier = TierDao.RetrieveByName(tierName);
				if (tier != null)
				{
					CacheManager.Update(CacheRegions.TierByName, tier.Name, tier);
                    CacheManager.Update(CacheRegions.TierById, tier.Id, tier);
                }
			}
			return tier;
		}

		public List<TierDef> GetAllChangedTierDefs(DateTime since)
		{
			return TierDao.RetrieveChangedObjects(since) ?? new List<TierDef>();
		}

		public void DeleteTierDef(long tierId)
		{
			TierDef tier = GetTierDef(tierId);
			if (tier != null)
			{
				TierDao.Delete(tierId);
				CacheManager.Remove(CacheRegions.TierByName, tier.Name);
                CacheManager.Remove(CacheRegions.TierById, tier.Id);
                CacheManager.Remove(CacheRegions.Tiers, "all");
			}
		}

		#endregion

		#endregion

		#region Next Best Action

		public void CreateNextBestAction(NextBestAction nba)
		{
			NextBestActionDao.Create(nba);
		}

		public void CreateNextBestActions(IEnumerable<NextBestAction> nbas)
		{
			NextBestActionDao.Create(nbas);
		}

		public void UpdateNextBestAction(NextBestAction nba)
		{
			NextBestActionDao.Update(nba);
		}

		public void UpdateNextBestActions(IEnumerable<NextBestAction> nbas)
		{
			NextBestActionDao.Update(nbas);
		}

		public void DeleteNextBestAction(long memberId, int priority)
		{
			NextBestActionDao.Delete(memberId, priority);
		}

		public IEnumerable<NextBestAction> GetNextBestActions(long memberId, int? startIndex = null, int? batchSize = null)
		{
			LWQueryBatchInfo batch = null;
			if (startIndex.HasValue && batchSize.HasValue)
			{
				batch = new LWQueryBatchInfo(startIndex.Value, batchSize.Value);
			}
			return NextBestActionDao.Retrieve(memberId, batch) ?? new List<NextBestAction>();
		}

		public NextBestAction GetNextBestAction(long memberId, int priority)
		{
			return NextBestActionDao.Retrieve(memberId, priority);
		}

		public void CreateMemberNextBestAction(MemberNextBestAction nba)
		{
			MemberNextBestActionDao.Create(nba);
		}

		public void UpdateMemberNextBestAction(MemberNextBestAction nba)
		{
			MemberNextBestActionDao.Update(nba);
		}

		public void DeleteMemberNextBestAction(long id)
		{
			MemberNextBestActionDao.Delete(id);
		}

		public MemberNextBestAction GetMemberNextBestAction(long id)
		{
			return MemberNextBestActionDao.Retrieve(id);
		}

		public IEnumerable<MemberNextBestAction> GetMemberNextBestAction(long memberId, NextBestActionType actionType)
		{
			return MemberNextBestActionDao.Retrieve(memberId, actionType);
		}

		public IEnumerable<MemberNextBestAction> GetMemberNextBestAction(long memberId, NextBestActionType actionType, long actionId)
		{
			return MemberNextBestActionDao.Retrieve(memberId, actionType, actionId);
		}

		/// <summary>
		/// Assigns next best actions to the member
		/// </summary>
		/// <param name="memberId">IPCode of the member to assign actions to</param>
		/// <param name="count">The number of actions to assign</param>
		/// <param name="persist">true to write each MemberNextBestAction to the database</param>
		/// <param name="actionTypes">Enumerable list of action types to include. A null or empty list will assume all action types are included.</param>
		/// <param name="assignCertificates">Indicates whether or not certificates will be assigned to each coupon (applies only to coupon action types)</param>
		/// <param name="displayOrder">Overrides the default display order of the action, if applicable</param>
		/// <param name="expirationDate">Overrides the default expiration date of the action, if applicable</param>
		/// <returns></returns>
		public IEnumerable<MemberNextBestAction> AssignNextBestActions(long memberId, int count, bool persist = true, IEnumerable<NextBestActionType> actionTypes = null, bool assignCertificates = false, int? displayOrder = null, DateTime? expirationDate = null)
		{
			const string methodName = "AssignNextBestActions";

			using (var contentService = LWDataServiceUtil.ContentServiceInstance(Organization, Environment))
			{

				Func<string, string> nextCert = delegate (string typeCode)
				{
					var cert = contentService.RetrieveFirstAvailablePromoCertificate(ContentObjType.Coupon, typeCode, null, null);
					if (cert == null)
					{
						throw new LWException(string.Format("Cannot retrieve coupon certificates for typecode {0}. No more certificates available.", typeCode));
					}
					return cert.CertNmbr;
				};

				try
				{
					_logger.Debug(_className, methodName, string.Format("begin, member {0}, count {1}", memberId, count));

					if (count < 1)
					{
						throw new ArgumentOutOfRangeException("count", "count must be greater than zero");
					}

					if (memberId < 1)
					{
						throw new ArgumentOutOfRangeException("memberId");
					}

					var ret = new List<MemberNextBestAction>();

					//there is some (unknown) probability that we'll get an action that has already 
					//been assigned. We'll start by batching at least X actions, just to keep us from
					//repeatedly returning to the database when we find that an action has already been
					//assigned to the member.
					int batchSize = Math.Max(count, 5);
					int index = 0;

					int inclusionCount = actionTypes == null ? 0 : actionTypes.Count();

					while (ret.Count < count)
					{
						var nbas = GetNextBestActions(memberId, index, batchSize);
						if (nbas == null || nbas.Count() == 0)
						{
							//we've exhausted the next best action list
							_logger.Debug(_className, methodName, string.Format("next best action list is exhausted, member {0}, count {1}", memberId, count));
							return ret;
						}

						foreach (NextBestAction nba in nbas)
						{
							if (inclusionCount > 0 && !actionTypes.Contains(nba.ActionType))
							{
								//this action type is not wanted. Skip.
								_logger.Debug(_className, methodName, string.Format("found next best action type of {0}, which is unwanted. Skipping", nba.ActionType));
								continue;
							}

							switch (nba.ActionType)
							{
								case NextBestActionType.Coupon:
									{
										var coupon = contentService.GetCouponDef(nba.ActionId);
										if (coupon == null || (coupon.ExpiryDate.HasValue && coupon.ExpiryDate.Value < DateTime.Now))
										{
											//definition doesn't exist or has expired, skip
											_logger.Debug(_className, methodName, string.Format("next best action coupon is null or expired , member {0}, coupon {1}", memberId, nba.ActionId));
											break;
										}

										//check to see if the member currently has this coupon in an active state
										bool hasActive = false;
										var memberCoupons = GetMemberCoupons(memberId, nba.ActionId);
										if (memberCoupons != null)
										{
											foreach (MemberCoupon mc in memberCoupons)
											{
												if (mc.Status.GetValueOrDefault(CouponStatus.Active) == CouponStatus.Active &&
													(coupon.UsesAllowed == 0 || mc.TimesUsed < coupon.UsesAllowed) &&
													(!mc.ExpiryDate.HasValue || mc.ExpiryDate.Value > DateTime.Now))
												{
													hasActive = true;
													_logger.Debug(_className, methodName, string.Format("next best action coupon is already or will be active, member {0}, coupon {1}", memberId, nba.ActionId));
													break;
												}
											}
										}

										if (!hasActive)
										{
											if (persist)
											{
												var mc = new MemberCoupon();
												mc.MemberId = memberId;
												mc.CouponDefId = nba.ActionId;
												if (assignCertificates)
												{
													mc.CertificateNmbr = nextCert(coupon.CouponTypeCode);
												}
												mc.DisplayOrder = displayOrder ?? coupon.DisplayOrder;
												mc.ExpiryDate = expirationDate ?? coupon.ExpiryDate;
												CreateMemberCoupon(mc);

												var mnba = new MemberNextBestAction(memberId, nba.Priority, nba.ActionType, nba.ActionId, mc.ID);
												CreateMemberNextBestAction(mnba);
												ret.Add(mnba);
											}
											else
											{
												ret.Add(new MemberNextBestAction(memberId, nba.Priority, nba.ActionType, nba.ActionId, 0));
											}
										}
									}
									break;
								case NextBestActionType.Message:
									{
										var message = contentService.GetMessageDef(nba.ActionId);
										if (message == null || (message.ExpiryDate.HasValue && message.ExpiryDate.Value < DateTime.Now))
										{
											//definition doesn't exist or has expired, skip
											_logger.Debug(_className, methodName, string.Format("next best action message is null or expired , member {0}, message {1}", memberId, nba.ActionId));
											break;
										}

										//check to see if the member currently has this message in an active state
										bool hasActive = false;
										var memberMessages = GetMemberMessages(memberId, nba.ActionId);
										if (memberMessages != null)
										{
											foreach (MemberMessage mm in memberMessages)
											{
												if (!mm.ExpiryDate.HasValue || mm.ExpiryDate.Value > DateTime.Now)
												{
													hasActive = true;
													_logger.Debug(_className, methodName, string.Format("next best action message is already active, member {0}, message {1}", memberId, nba.ActionId));
													break;
												}
											}
										}

										if (!hasActive)
										{
											if (persist)
											{
												var mm = new MemberMessage();
												mm.MemberId = memberId;
												mm.MessageDefId = nba.ActionId;
												mm.DisplayOrder = displayOrder ?? message.DisplayOrder;
												mm.ExpiryDate = expirationDate ?? message.ExpiryDate;
												CreateMemberMessage(mm);

												var mnba = new MemberNextBestAction(memberId, nba.Priority, nba.ActionType, nba.ActionId, mm.Id);
												if (persist)
												{
													CreateMemberNextBestAction(mnba);
												}
												ret.Add(mnba);
											}
											else
											{
												ret.Add(new MemberNextBestAction(memberId, nba.Priority, nba.ActionType, nba.ActionId, 0));
											}
										}
									}
									break;
								default:
									_logger.Debug(_className, methodName, string.Format("found next best action type of {0}, which has no implementation.", nba.ActionType));
									break;
							}

							if (ret.Count >= count)
							{
								break;
							}
						}

						index += batchSize;
					}
					return ret;
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, "Unexpected error while assigning next best actions", ex);
					throw;
				}
			}
		}

		#endregion

		#region Rule Triggers

		private RuleTrigger DeserializeRuleInstance(RuleTrigger rt)
		{
			if (rt != null && rt.Rule == null && !string.IsNullOrEmpty(rt.RuleInstance))
			{
				rt.Rule = Brierley.FrameWork.Rules.RuleBase.DeSerialize(rt.RuleInstance);
			}
			return rt;
		}

		public void CreateRuleTrigger(RuleTrigger rt)
		{
			if (rt.Rule != null)
			{
				rt.RuleInstance = rt.Rule.Serialize(Version);
			}
			RuleTriggerDao.Create(rt);
			CacheManager.Update(CacheRegions.RuleTriggerByName, rt.RuleName, rt);
			CacheManager.Remove(CacheRegions.RuleTriggerByAttributeSet, rt.AttributeSetCode);
			CacheManager.Remove(CacheRegions.RuleTriggerByObjectName, rt.OwningObject);
			if (!string.IsNullOrEmpty(rt.PromotionCode))
			{
				CacheManager.Remove(CacheRegions.RuleTriggerByPromotion, rt.PromotionCode);
			}
		}

		public void UpdateRuleTrigger(RuleTrigger rt)
		{
			if (rt.Rule != null)
			{
				rt.RuleInstance = rt.Rule.Serialize(Version);
				rt.Rule.Validate();
			}
			RuleTriggerDao.Update(rt);
			CacheManager.Update(CacheRegions.RuleTriggerByName, rt.RuleName, rt);
			CacheManager.Remove(CacheRegions.RuleTriggerByAttributeSet, rt.AttributeSetCode);
			CacheManager.Remove(CacheRegions.RuleTriggerByObjectName, rt.OwningObject);
			if (!string.IsNullOrEmpty(rt.PromotionCode))
			{
				CacheManager.Remove(CacheRegions.RuleTriggerByPromotion, rt.PromotionCode);
			}
		}

		public RuleTrigger GetRuleById(long id)
		{
			string methodName = "GetRuleById";
			RuleTrigger rt = null;
			rt = RuleTriggerDao.Retrieve(id);
			if (rt != null)
			{
				CacheManager.Update(CacheRegions.RuleTriggerByName, rt.RuleName, rt);
			}
			try
			{
				DeserializeRuleInstance(rt);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unable to desrialize rule " + rt.RuleName, ex);
			}
			return rt;
		}

		public RuleTrigger GetRuleByName(string rulename)
		{
			const string methodName = "GetRuleByName";
			RuleTrigger rt = (RuleTrigger)CacheManager.Get(CacheRegions.RuleTriggerByName, rulename);
			if (rt == null)
			{
				rt = RuleTriggerDao.Retrieve(rulename);
				if (rt != null)
				{
					CacheManager.Update(CacheRegions.RuleTriggerByName, rt.RuleName, rt);
				}
			}
			try
			{
				rt = DeserializeRuleInstance(rt);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unable to desrialize rule " + rt.RuleName, ex);
			}
			return rt;
		}

		public List<RuleTrigger> GetRuleByAttributeSetCode(long attSetCode)
		{
			const string methodName = "GetRuleByAttributeSetCode";

			List<RuleTrigger> rules = (List<RuleTrigger>)CacheManager.Get(CacheRegions.RuleTriggerByAttributeSet, attSetCode);
			if (rules == null)
			{
				rules = RuleTriggerDao.RetrieveByAttributeSet(attSetCode);
				if (rules != null)
				{
					foreach (RuleTrigger rt in rules)
					{
						try
						{
							DeserializeRuleInstance(rt);
							CacheManager.Update(CacheRegions.RuleTriggerByName, rt.RuleName, rt);
						}
						catch (Exception ex)
						{
							_logger.Error(_className, methodName, "Unable to desrialize rule " + rt.RuleName, ex);
						}
					}
					CacheManager.Update(CacheRegions.RuleTriggerByAttributeSet, attSetCode, rules);
				}
			}
			return rules;
		}

		public IList<RuleTrigger> GetRuleByPromotion(string promoCode)
		{
			const string methodName = "GetRuleByPromotion";

			List<RuleTrigger> rules = (List<RuleTrigger>)CacheManager.Get(CacheRegions.RuleTriggerByPromotion, promoCode);
			if (rules == null)
			{
				rules = RuleTriggerDao.RetrieveByPromotion(promoCode);
				if (rules != null)
				{
					foreach (RuleTrigger rt in rules)
					{
						try
						{
							DeserializeRuleInstance(rt);
							CacheManager.Update(CacheRegions.RuleTriggerByName, rt.RuleName, rt);
						}
						catch (Exception ex)
						{
							_logger.Error(_className, methodName, "Unable to desrialize rule " + rt.RuleName, ex);
						}
					}
					CacheManager.Update(CacheRegions.RuleTriggerByPromotion, promoCode, rules);
				}
			}
			return rules;
		}

		public List<RuleTrigger> GetRuleByObjectName(string objectName)
		{
			const string methodName = "GetRuleByObjectName";
			List<RuleTrigger> rules = (List<RuleTrigger>)CacheManager.Get(CacheRegions.RuleTriggerByObjectName, objectName);
			if (rules == null)
			{
				rules = RuleTriggerDao.RetrieveByOwningObject(objectName);
				if (rules != null)
				{
					foreach (RuleTrigger rt in rules)
					{
						try
						{
							DeserializeRuleInstance(rt);
							CacheManager.Update(CacheRegions.RuleTriggerByName, rt.RuleName, rt);
						}
						catch (Exception ex)
						{
							_logger.Error(_className, methodName, "Unable to desrialize rule " + rt.RuleName, ex);
						}
					}
					CacheManager.Update(CacheRegions.RuleTriggerByObjectName, objectName, rules);
				}
			}
			return rules;
		}

		public List<RuleTrigger> GetAllChangedRules(DateTime since)
		{
			return RuleTriggerDao.RetrieveChangedObjects(since) ?? new List<RuleTrigger>();
		}

		public List<RuleTrigger> GetAllRules()
		{
			const string methodName = "GetAllRules";

			List<RuleTrigger> rules = RuleTriggerDao.RetrieveAll();
			if (rules != null)
			{
				foreach (RuleTrigger rt in rules)
				{
					try
					{
						DeserializeRuleInstance(rt);
						CacheManager.Update(CacheRegions.RuleTriggerByName, rt.RuleName, rt);
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Unable to desrialize rule " + rt.RuleName, ex);
					}
				}
			}
			return rules;
		}

		public List<RuleTrigger> GetAllRules(long[] ids)
		{
			return RuleTriggerDao.RetrieveAll(ids) ?? new List<RuleTrigger>();
		}

		public void DeleteRuleTrigger(long rtId)
		{
			RuleTrigger rt = GetRuleById(rtId);
			if (rt != null)
			{
				RuleTriggerDao.Delete(rtId);
				CacheManager.Remove(CacheRegions.RuleTriggerByName, rt.RuleName);
				CacheManager.Remove(CacheRegions.RuleTriggerByAttributeSet, rt.AttributeSetCode);
				CacheManager.Remove(CacheRegions.RuleTriggerByObjectName, rt.OwningObject);
				if (!string.IsNullOrEmpty(rt.PromotionCode))
				{
					CacheManager.Remove(CacheRegions.RuleTriggerByPromotion, rt.PromotionCode);
				}
			}
		}

		#endregion

		#region Validators

		public void CreateValidator(Validator validator)
		{
			ValidatorDao.Create(validator);
		}

		public void UpdateValidator(Validator validator)
		{
			ValidatorDao.Update(validator);
		}

		public List<Validator> GetValidators(long[] ids)
		{
			return ValidatorDao.Retrieve(ids) ?? new List<Validator>();
		}

		public List<Validator> GetAllValidators()
		{
			return ValidatorDao.RetrieveAll() ?? new List<Validator>();
		}

		public Validator GetValidator(long validatorId)
		{
			return ValidatorDao.Retrieve(validatorId);
		}

		public Validator GetValidator(string name)
		{
			return ValidatorDao.Retrieve(name);
		}

		public void DeleteValidator(long validatorId)
		{
			ValidatorDao.Delete(validatorId);
		}

		public bool IsValidatorInUse(long validatorId)
		{
			return ValidatorDao.IsInUse(validatorId);
		}

		#endregion

		#region Validator Triggers

		public void CreateValidatorTrigger(ValidatorTrigger vt)
		{
			ValidatorTriggerDao.Create(vt);
			CacheManager.Remove(CacheRegions.ValidatorTriggerByAttribute, vt.AttributeCode);
		}

		/// <summary>
		/// This method creates a new validator trigger.
		/// </summary>
		/// <param name="attributeCode"></param>
		/// <param name="validatorId"></param>
		/// <param name="OnError"></param>
		/// <param name="sequence"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public ValidatorTrigger CreateValidatorTrigger(long attributeCode, long validatorId, bool OnError, long sequence, string errorMessage)
		{
			ValidatorTrigger vt = new ValidatorTrigger();
			vt.AttributeCode = attributeCode;
			vt.ValidatorId = validatorId;
			vt.Sequence = sequence;
			vt.OnErrorContinue = OnError;
			vt.ErrorMessage = errorMessage;
			CreateValidatorTrigger(vt);
			return vt;
		}

		public void UpdateValidatorTrigger(ValidatorTrigger vt)
		{
			ValidatorTriggerDao.Update(vt);
			CacheManager.Remove(CacheRegions.ValidatorTriggerByAttribute, vt.AttributeCode);
		}

		/// <summary>
		/// This method gets the list of validators configured for a particular attribute.
		/// </summary>
		/// <param name="attributeCode">attribute code</param>
		/// <returns>list of validators.</returns>
		public List<ValidatorTrigger> GetValidatorTriggers(long attributeCode)
		{
			var list = (List<ValidatorTrigger>)CacheManager.Get(CacheRegions.ValidatorTriggerByAttribute, attributeCode);
			if (list == null)
			{
				list = ValidatorTriggerDao.RetrieveByAttribute(attributeCode) ?? new List<ValidatorTrigger>();
				CacheManager.Update(CacheRegions.ValidatorTriggerByAttribute, attributeCode, list);
			}
			return list;
		}

		public List<ValidatorTrigger> GetValidatorTriggersByAttributes(long[] ids)
		{
			return ValidatorTriggerDao.RetrieveByAttribute(ids) ?? new List<ValidatorTrigger>();
		}

		public ValidatorTrigger GetValidatorTrigger(long validatorId, long attributeCode)
		{
			return ValidatorTriggerDao.Retrieve(validatorId, attributeCode);
		}

		public List<ValidatorTrigger> GetAllValidatorTriggers()
		{
			return ValidatorTriggerDao.RetrieveAll() ?? new List<ValidatorTrigger>();
		}

		public ValidatorTrigger GetValidatorTrigger(long id)
		{
			return ValidatorTriggerDao.Retrieve(id);
		}

		public void DeleteValidatorTrigger(long id)
		{
			ValidatorTrigger vt = GetValidatorTrigger(id);
			if (vt != null)
			{
				ValidatorTriggerDao.Delete(id);
				CacheManager.Remove(CacheRegions.ValidatorTriggerByAttribute, vt.AttributeCode);
			}
		}
		#endregion




		private void ClearAttributeSetCache(AttributeSetMetaData meta)
		{
			CacheManager.Remove(CacheRegions.AttributeSetMetadataById, meta.ID);
			CacheManager.Remove(CacheRegions.AttributeSetMetadataByName, meta.Name);
			if (meta.ParentID != -1)
			{
				AttributeSetMetaData parent = GetAttributeSetMetaData(meta.ParentID);
				if (parent != null)
				{
					ClearAttributeSetCache(parent);
				}
			}
		}

		private void PutMemberInCache(Member member)
		{
			CacheManager.Update(CacheRegions.MemberByIPCode, member.IpCode, member);
			if (CacheManager.RegionExists(CacheRegions.MemberByLoyaltyId))
			{
				foreach (VirtualCard vc in member.LoyaltyCards)
				{
					CacheManager.Update(CacheRegions.MemberByLoyaltyId, vc.LoyaltyIdNumber, member);
				}
			}
		}

		private void ClearMemberFromCache(Member member)
		{
			CacheManager.Remove(CacheRegions.MemberByIPCode, member.IpCode);
			// clear from Loyalty Id Cache
			if (CacheManager.RegionExists(CacheRegions.MemberByLoyaltyId))
			{
				foreach (VirtualCard vc in member.LoyaltyCards)
				{
					CacheManager.Remove(CacheRegions.MemberByLoyaltyId, vc.LoyaltyIdNumber);
				}
			}
		}

		public Promotion GetPromotionByCode(string code)
		{
			using (var content = new ContentService(Config))
			{
				return content.GetPromotionByCode(code);
			}
		}

		private void CreateMemberLoginEvent(String providedUsername, String providedPassword, MemberLoginEventType eventType)
		{
            if (!string.IsNullOrEmpty(providedUsername) && providedUsername.Length > 255) providedUsername = providedUsername.Substring(0, 255);
            //if (!string.IsNullOrEmpty(providedPassword) && providedPassword.Length > 255) providedPassword = providedPassword.Substring(0, 255);

			MemberLoginEvent memberLoginEvent = new MemberLoginEvent();
			memberLoginEvent.ProvidedUsername = providedUsername;
			//memberLoginEvent.ProvidedPassword = providedPassword; WTF IS THIS I DONT EVEN WHY WOULD YOU STORE A PLAIN TEXT PASSWORD
			memberLoginEvent.RemoteIPAddress = LWPasswordUtil.GetRemoteIPAddress();
			memberLoginEvent.RemoteUserName = LWPasswordUtil.GetRemoteUserName();
			memberLoginEvent.RemoteUserAgent = LWPasswordUtil.GetRemoteUserAgent();
			memberLoginEvent.EventSource = LWPasswordUtil.GetLocalHostName();
			memberLoginEvent.EventType = eventType;
			MemberLoginEventDao.Create(memberLoginEvent);
		}

		private void CheckSMSOptIn(IClientDataObject detail, ContextObject context, RuleInvocationType invocationType)
		{
			bool dmcSmsEnabled = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("DmcSmsEnabled"));
			bool smsDoubleOptIn = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("DmcSmsDoubleOptIn"));

			if (dmcSmsEnabled)
			{
				bool isOptingIn = (bool)detail.GetAttributeValue("IsOptingIn");
				bool isOptingOut = (bool)detail.GetAttributeValue("IsOptingOut");

				if (!isOptingIn && !isOptingOut)
				{
					return;
				}

				//todo: these should be checked before accessing - client may not be following the standard attribute set:
				bool optedIn = Convert.ToBoolean(detail.GetAttributeValue("SmsOptIn"));
				bool dblOptInComplete = detail.GetAttributeValue("SmsConsentChangeDate") != null;
				bool isCompletingDoubleOptIn = Convert.ToBoolean(detail.GetAttributeValue("IsCompletingDoubleOptIn"));

				string mobilePhoneCountryCode = null;
				string mobilePhone = null;
				string completePhoneNumber = null;

				var meta = detail.GetMetaData();
				if (meta.Attributes.FirstOrDefault(o => o.Name.Equals("MobilePhoneCountryCode", StringComparison.OrdinalIgnoreCase)) != null)
				{
					LWCriterion lwCriteria = new LWCriterion("RefCountry");
					List<IClientDataObject> clientDataObjects = GetAttributeSetObjects(null, "RefCountry", lwCriteria, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = int.MaxValue }, false);
					IClientDataObject item = clientDataObjects.Find(obj => obj.RowKey == Convert.ToInt64(detail.GetAttributeValue("MobilePhoneCountryCode")));

					if (item != null)
						mobilePhoneCountryCode = item.GetAttributeValue("PhoneCode").ToString();
				}

				if (meta.Attributes.FirstOrDefault(o => o.Name.Equals("MobilePhone", StringComparison.OrdinalIgnoreCase)) != null)
				{
					mobilePhone = detail.GetAttributeValue("MobilePhone").ToString();
				}

				if (isOptingOut)
				{
					detail.SetAttributeValue("SmsOptIn", false);
					detail.SetAttributeValue("SmsDblOptInComplete", false);
				}

				detail.SetAttributeValue("SmsConsentChangeDate", DateTime.Now);

				if (string.IsNullOrEmpty(mobilePhoneCountryCode) || string.IsNullOrEmpty(mobilePhone))
					return;

				completePhoneNumber = mobilePhoneCountryCode + mobilePhone;
				using (DmcService dmc = new Dmc.DmcService(Config.SmsLogger))
				{
					User user = dmc.GetUserByMobilePhone(completePhoneNumber);
					var attributes = new List<Dmc.Attribute>() { new Dmc.Attribute("sms_consent", isOptingIn.ToString().ToLower()) };
					if (user != null)
					{
						dmc.UpdateProfileByMobileNumber(completePhoneNumber, attributes);
					}
					else
					{
						bool _dmcUseAlternateMobile = bool.Parse(LWConfigurationUtil.GetConfigurationValue(Constants.DmcUseAlternateMobile));
						if (_dmcUseAlternateMobile)
						{
							attributes.Add(new Dmc.Attribute("SendSms", mobilePhone));
						}
						user = dmc.CreateUser(completePhoneNumber, attributes);
					}
				}

				if ((smsDoubleOptIn && optedIn && dblOptInComplete && (isOptingIn || isCompletingDoubleOptIn)) || (!smsDoubleOptIn && optedIn && (isOptingIn || isCompletingDoubleOptIn)))
				{
					ExecuteEventRules(context, "MemberSmsOptIn", invocationType);
				}
			}
		}

		public string MaskPhoneNumber(string phone)
		{
			if (string.IsNullOrEmpty(phone))
				return string.Empty;

			//(214) 123-4567
			//(214) ???-??67
			if (Regex.IsMatch(phone, @"\([0-9]{3}\) [0-9]{3}-[0-9]{4}"))
				return Regex.Replace(phone, "[0-9]{3}-[0-9]{2}", "???-??");

			//214.123.4567
			//214.???.??67
			if (Regex.IsMatch(phone, @"[0-9]{3}\.[0-9]{3}\.[0-9]{4}"))
				return Regex.Replace(phone, @"\.[0-9]{3}\.[0-9]{2}", ".???.??");

			//214-123-4567
			//214-???-??67
			if (Regex.IsMatch(phone, @"[0-9]{3}-[0-9]{3}-[0-9]{4}"))
				return Regex.Replace(phone, "-[0-9]{3}-[0-9]{2}", "-???-??");

			//214 123 4567
			//214 ??? ??67
			if (Regex.IsMatch(phone, @"[0-9]{3} [0-9]{3} [0-9]{4}"))
				return Regex.Replace(phone, " [0-9]{3} [0-9]{2}", " ??? ??");

			//2141234567
			//214?????67
			//same as below
			// No matches
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < phone.Length; i++)
			{
				if (i >= (phone.Length / 4d) && i < (phone.Length * 3d / 4d))
					sb.Append("?");
				else
					sb.Append(phone[i]);
			}
			return sb.ToString();
		}

		public string MaskEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
				return string.Empty;

			StringBuilder sb = new StringBuilder();
			// abcd@def.ghi
			if (Regex.IsMatch(email, @".+@.+\..+"))
			{
				int atSymbolIndex = email.IndexOf('@');
				for (int i = 0; i < atSymbolIndex; i++)
				{
					if (i >= (atSymbolIndex / 2))
						sb.Append("?");
					else
						sb.Append(email[i]);
				}
				sb.Append("@");
				int finalDotIndex = email.LastIndexOf('.');
				for (int i = atSymbolIndex + 1; i < email.Length; i++)
				{
					if (i >= (atSymbolIndex + finalDotIndex) / 2d && i < finalDotIndex)
						sb.Append("?");
					else
						sb.Append(email[i]);
				}
			}
			else
			{
				// Everything else
				for (int i = 0; i < email.Length; i++)
				{
					if (i >= (email.Length / 4d) && i < (email.Length * 3d / 4d))
						sb.Append("?");
					else
						sb.Append(email[i]);
				}
			}
			return sb.ToString();
		}
		*/
	}
}
