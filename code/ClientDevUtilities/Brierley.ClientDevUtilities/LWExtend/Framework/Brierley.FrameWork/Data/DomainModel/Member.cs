using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Rules;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for LWMember.
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("IPCode", sequenceName = "IPCODE_SEQ")]
    [PetaPoco.TableName("LW_LoyaltyMember")]
    [AuditLog(false)]
    public class Member : AttributeSetContainer
    {
        public const string IPCODE_COOKIE_ENCRYPTION_KEY = "bHdtZW1iZXJpcGNvZGVrZXk=";

        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private const string _className = "Member";

        [PetaPoco.Column("MemberStatus")]
        private MemberStatusEnum _memberStatus { get; set; }

        [PetaPoco.Column("NewStatus")]
        private MemberStatusEnum? _newStatus { get; set; }

        [PetaPoco.Column("NewStatusEffectiveDate")]
        private DateTime? _newStatusEffectiveDate { get; set; }

        private DateTime _memberCreateDate = DateTime.Now;
        private DateTime? _memberCloseDate = null;
        private DateTime? _birthDate = null;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _middleName = string.Empty;
        private string _namePrefix = string.Empty;
        private string _nameSuffix = string.Empty;
        private string _alternateId;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _salt = string.Empty;
        private string _primaryEmailAddress = string.Empty;
        private string _primaryPhoneNumber = string.Empty;
        private string _primaryPostalCode = string.Empty;
        private string _preferredLanguage;
        private DateTime? _lastActivityDate = null;
        private bool? _isEmployee;
        private string _changedBy = string.Empty;
        private string _resetCode = null;
        private DateTime? _resetCodeDate = null;
        private int _failedPasswordAttemptCount = 0;
        private bool _passwordChangeRequired = false;
        private DateTime? _passwordExpireDate;
        private IList<VirtualCard> _loyaltyCards;

        #region Properties

        public virtual bool IsCardDirty
        {
            get
            {
                bool dirtyFlag = false;
                lock (_loyaltyCards)
                {
                    foreach (VirtualCard card in _loyaltyCards)
                    {
                        dirtyFlag = card.IsDirty;
                        if (dirtyFlag)
                        {
                            break;
                        }
                    }
                }
                return dirtyFlag;
            }
        }

        /// <summary>
        /// Gets or sets the IpCode for the current LWMember
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public virtual Int64 IpCode
        {
            get { return MyKey; }
            set { MyKey = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the MemberCreateDate for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(IsNullable = false)]
        public virtual DateTime MemberCreateDate
        {
            get { return _memberCreateDate; }
            set { _memberCreateDate = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the MemberCloseDate for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column]
        public virtual DateTime? MemberCloseDate
        {
            get { return _memberCloseDate; }
            set { _memberCloseDate = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the MemberStatus for the current LWMember
        /// </summary>
        public virtual MemberStatusEnum MemberStatus
        {
            get
            {
                if (_newStatus.HasValue)
                {
                    var effectiveDate = _newStatusEffectiveDate.GetValueOrDefault(DateTime.Now);
                    if (DateTimeUtil.LessEqual(effectiveDate, DateTime.Now))
                    {
                        _memberStatus = _newStatus.Value;
                        StatusChangeDate = DateTime.Now;
                        if (effectiveDate < DateTime.Now)
                        {
                            StatusChangeDate = effectiveDate;
                        }
                        if (_memberStatus == MemberStatusEnum.Disabled || _memberStatus == MemberStatusEnum.Terminated)
                        {
                            _memberCloseDate = effectiveDate;
                        }
                        _newStatus = null;
                        _newStatusEffectiveDate = null;
                        IsDirty = true;
                    }
                }
                return _memberStatus;
            }
        }

        [PetaPoco.Column]
        public virtual DateTime? StatusChangeDate { get; set; }

        [PetaPoco.Column]
        public virtual long? MergedToMember { get; set; }

        /// <summary>
        /// Gets or sets the BirthDate for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column]
        public virtual DateTime? BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the FirstName for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 50)]
        public virtual string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the LastName for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 50)]
        public virtual string LastName
        {
            get { return _lastName; }
            set { _lastName = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the MiddleName for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 50)]
        public virtual string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the NamePrefix for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 10)]
        public virtual string NamePrefix
        {
            get { return _namePrefix; }
            set { _namePrefix = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the NameSuffix for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 10)]
        public virtual string NameSuffix
        {
            get { return _nameSuffix; }
            set { _nameSuffix = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the AlternateId for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 255)]
        [ColumnIndex]
        public virtual string AlternateId
        {
            get { return _alternateId; }
            set { _alternateId = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the Username for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 254)]
        [ColumnIndex]
        public virtual string Username
        {
            get { return _username; }
            set { _username = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the Password for the current LWMember
        /// </summary>
        [PetaPoco.Column(Length = 50)]
        public virtual string Password
        {
            get { return _password; }
            set { _password = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the Salt for the current LWMember
        /// </summary>
        [PetaPoco.Column(Length = 50)]
        public virtual string Salt
        {
            get { return _salt; }
            set { _salt = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the PrimaryEmailAddress for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 254)]
        [UniqueIndex(RequiresLowerFunction = true)]
        public virtual string PrimaryEmailAddress
        {
            get { return _primaryEmailAddress; }
            set
            {
                _primaryEmailAddress = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the PrimaryPhoneNumber for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 25)]
        public virtual string PrimaryPhoneNumber
        {
            get { return _primaryPhoneNumber; }
            set { _primaryPhoneNumber = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the PrimaryPostalCode for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 15)]
        public virtual string PrimaryPostalCode
        {
            get { return _primaryPostalCode; }
            set { _primaryPostalCode = value; IsDirty = true; }
        }


        /// <summary>
		/// Gets or sets the PrimaryPostalCode for the current LWMember
		/// </summary>
		[Browsable(true)]
        [PetaPoco.Column(Length = 8)]
        [ForeignKey(typeof(LanguageDef), "Culture")]
        public virtual string PreferredLanguage
        {
            get { return _preferredLanguage; }
            set { _preferredLanguage = value; IsDirty = true; }
        }


        /// <summary>
        /// Gets or sets the LastActivityDate for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column]
        public virtual DateTime? LastActivityDate
        {
            get { return _lastActivityDate; }
            set { _lastActivityDate = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the IsEmployee for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column]
        public virtual bool? IsEmployee
        {
            get { return _isEmployee; }
            set { _isEmployee = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the ChangedBy for the current LWMember
        /// </summary>
        [Browsable(true)]
        [PetaPoco.Column(Length = 25)]
        public virtual string ChangedBy
        {
            get { return _changedBy; }
            set { _changedBy = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the NewStatus for the current LWMember
        /// </summary>
        [Browsable(true)]
        public virtual MemberStatusEnum? NewStatus
        {
            set
            {
                _newStatus = value; IsDirty = true;
                NewStatusEffectiveDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Gets or sets the NewStatus for the current LWMember
        /// </summary>
        [Browsable(true)]
        public virtual DateTime? NewStatusEffectiveDate
        {
            set { _newStatusEffectiveDate = value; IsDirty = true; }
        }

        [PetaPoco.Column(Length = 255)]
        public virtual string StatusChangeReason { get; set; }


        /// <summary>
        /// Gets or sets the LoyaltyCards for the current LWMember
        /// </summary>
        public virtual IList<VirtualCard> LoyaltyCards
        {
            get { return _loyaltyCards; }
            set { _loyaltyCards = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the ResetCode for the current LWMember
        /// </summary>
        [PetaPoco.Column(Length = 6)]
        public virtual string ResetCode
        {
            get { return _resetCode; }
            set { _resetCode = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the ResetCodeDate for the current LWMember
        /// </summary>
        [PetaPoco.Column]
        public virtual DateTime? ResetCodeDate
        {
            get { return _resetCodeDate; }
            set { _resetCodeDate = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the FailedPasswordAttemptCount for the current LWMember
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public virtual int FailedPasswordAttemptCount
        {
            get { return _failedPasswordAttemptCount; }
            set { _failedPasswordAttemptCount = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the PasswordChangeRequired for the current LWMember
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public virtual bool PasswordChangeRequired
        {
            get { return _passwordChangeRequired; }
            set { _passwordChangeRequired = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the PasswordExpireDate for the current LWMember
        /// </summary>
        [PetaPoco.Column]
        public virtual DateTime? PasswordExpireDate
        {
            get { return _passwordExpireDate; }
            set { _passwordExpireDate = value; IsDirty = true; }
        }

        /// <summary>
        /// Initializes a new instance of the LWMember class
        /// </summary>
        public Member()
        {
            _loyaltyCards = new List<VirtualCard>();
            _memberStatus = MemberStatusEnum.Active;
        }


        public override LWObjectAuditLogBase GetArchiveObject()
        {
            Member_AL ar = new Member_AL()
            {
                ObjectId = this.IpCode,
                MemberCreateDate = this.MemberCreateDate,
                MemberCloseDate = this.MemberCloseDate,
                MemberStatus = this.MemberStatus,
                StatusChangeDate = this.StatusChangeDate,
                MergedToMember = this.MergedToMember,
                BirthDate = this.BirthDate,
                FirstName = this.FirstName,
                LastName = this.LastName,
                MiddleName = this.MiddleName,
                NamePrefix = this.NamePrefix,
                NameSuffix = this.NameSuffix,
                AlternateId = this.AlternateId,
                Username = this.Username,
                Password = this.Password,
                Salt = this.Salt,
                PrimaryEmailAddress = this.PrimaryEmailAddress,
                PrimaryPhoneNumber = this.PrimaryPhoneNumber,
                PrimaryPostalCode = this.PrimaryPostalCode,
                LastActivityDate = this.LastActivityDate,
                IsEmployee = this.IsEmployee,
                ChangedBy = this.ChangedBy,
                NewStatus = _newStatus,
                NewStatusEffectiveDate = _newStatusEffectiveDate,
                StatusChangeReason = this.StatusChangeReason,
                ResetCode = this.ResetCode,
                ResetCodeDate = this.ResetCodeDate,
                FailedPasswordAttemptCount = this.FailedPasswordAttemptCount,
                PasswordChangeRequired = this.PasswordChangeRequired,
                PasswordExpireDate = this.PasswordExpireDate,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate
            };
            return ar;
        }

        public virtual bool IsPasswordChangeRequired()
        {
            return _passwordChangeRequired || (_passwordExpireDate != null && DateTime.Today > _passwordExpireDate);
        }

        #region Virtual Cards

        public virtual VirtualCard CreateNewVirtualCard(DateTime? expDate = null)
        {
            string methodName = "CreateNewVirtualCard";
            string msg = string.Format("Creating new virtual card for member with ipcode {0}.", MyKey.ToString());
            _logger.Debug(_className, methodName, msg);

            VirtualCard vc = expDate != null ? new VirtualCard(expDate.Value) : new VirtualCard();
            vc.Parent = this;
            //vc.Status = VirtualCardStatusType.Active;
            vc.IsPrimary = true;
            vc.Member = this;
            vc.DateIssued = DateTime.Now;
            vc.DateRegistered = DateTime.Now;
            lock (_loyaltyCards)
            {
                _loyaltyCards.Add(vc);
            }
            return vc;
        }

        public virtual VirtualCard GetLoyaltyCard(long vcKey)
        {
            lock (_loyaltyCards)
            {
                foreach (VirtualCard vc in _loyaltyCards)
                {
                    if (vc.VcKey == vcKey)
                    {
                        return vc;
                    }
                }
            }
            return null;
        }

        public virtual VirtualCard GetLoyaltyCard(string loyaltyCardId)
        {
            lock (_loyaltyCards)
            {
                foreach (VirtualCard vc in _loyaltyCards)
                {
                    if (vc.LoyaltyIdNumber == loyaltyCardId)
                    {
                        return vc;
                    }
                }
            }
            return null;
        }

        public virtual VirtualCard GetFirstCard()
        {
            lock (_loyaltyCards)
            {
                return _loyaltyCards[0];
            }
        }

        public virtual VirtualCard GetLoyaltyCardByType(VirtualCardSearchType type)
        {
            lock (_loyaltyCards)
            {
                if (_loyaltyCards.Count == 0)
                {
                    return null;
                }
                VirtualCard theCard = _loyaltyCards[0];
                switch (type)
                {
                    case VirtualCardSearchType.PrimaryCard:
                        foreach (VirtualCard vc in _loyaltyCards)
                        {
                            if (vc.IsPrimary == true)
                            {
                                return vc;
                            }
                        }
                        return null;
                    case VirtualCardSearchType.EarliestIssued:
                        foreach (VirtualCard vc in _loyaltyCards)
                        {
                            if (vc.DateIssued < theCard.DateIssued)
                            {
                                theCard = vc;
                            }
                        }
                        return theCard;
                    case VirtualCardSearchType.MostRecentIssued:
                        foreach (VirtualCard vc in _loyaltyCards)
                        {
                            if (vc.DateIssued > theCard.DateIssued)
                            {
                                theCard = vc;
                            }
                        }
                        return theCard;
                    case VirtualCardSearchType.EarliestRegistered:
                        foreach (VirtualCard vc in _loyaltyCards)
                        {
                            if (vc.DateRegistered < theCard.DateRegistered)
                            {
                                theCard = vc;
                            }
                        }
                        return theCard;
                    case VirtualCardSearchType.MostRecentRegistered:
                        foreach (VirtualCard vc in _loyaltyCards)
                        {
                            if (vc.DateRegistered > theCard.DateRegistered)
                            {
                                theCard = vc;
                            }
                        }
                        return theCard;
                    default:
                        return null;
                }
            }
        }

        public virtual void MarkVirtualCardAsPrimary(VirtualCard vc)
        {
            vc.IsPrimary = true;
            lock (_loyaltyCards)
            {
                foreach (VirtualCard ovc in _loyaltyCards)
                {
                    if (ovc != vc)
                    {
                        ovc.IsPrimary = false;
                    }
                }
            }
        }

        public virtual long[] GetLoyaltyCardIds()
        {
            IList<VirtualCard> validCards = new List<VirtualCard>();
            foreach (VirtualCard v in LoyaltyCards)
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
            return vcs;
        }

        #endregion

        #region Tier Stuff
        public virtual long AddTier(string tierName, DateTime enrollmentDate, DateTime? expDate, string description)
        {
            string methodName = "AddTier";
            _logger.Debug(_className, methodName,
                string.Format("Adding tier {0} to member {1}", tierName, IpCode));

            using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                TierDef rtier = loyalty.GetTierDef(tierName);
                if (rtier == null)
                {
                    string errmsg = string.Format("Unable to find a reference tier with name {0}", tierName);
                    throw new Brierley.FrameWork.Common.Exceptions.LWMetaDataException(errmsg);
                }
                MemberTier tier = new MemberTier();
                tier.MemberId = IpCode;
                tier.TierDefId = rtier.Id;
                tier.FromDate = enrollmentDate;
                tier.Description = description;

                if (expDate != null)
                {
                    _logger.Debug(_className, methodName,
                        string.Format("Provided expression date is {0} will override expression date in tier definition.", expDate.Value.ToString()));
                    tier.ToDate = (DateTime)expDate;
                }
                else
                {
                    // get the expiration date
                    _logger.Debug(_className, methodName,
                        string.Format("Evaluating expression {0} for expiration date.", rtier.ExpirationDateExpression));
                    ExpressionFactory exprF = new ExpressionFactory();
                    ContextObject context = new ContextObject() { Owner = this };
                    if (rtier.AddToEnrollmentDate)
                    {
                        string daysString = exprF.Create(rtier.ExpirationDateExpression).evaluate(context).ToString();
                        try
                        {
                            double days = double.Parse(daysString);
                            tier.ToDate = enrollmentDate.AddDays(days);
                            _logger.Debug(_className, methodName,
                                string.Format("Adding {0} days to the enrollment date {1}.  Expiration date is {2}",
                                days, enrollmentDate, tier.ToDate));
                        }
                        catch (FormatException)
                        {
                            _logger.Error(_className, methodName,
                                string.Format("Was expecting number of days to add to enrollment date.  Unable to convert {0} to days", daysString));
                            throw;
                        }
                    }
                    else
                    {
                        try
                        {
                            tier.ToDate = (DateTime)exprF.Create(rtier.ExpirationDateExpression).evaluate(context);
                            _logger.Debug(_className, methodName,
                                string.Format("Expiration date is {0}", tier.ToDate));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(_className, methodName,
                                string.Format("Unable to evaluate expression for expiry date for tier {0}", tierName), ex);
                            throw;
                        }
                    }
                }

                MemberTier currentTier = GetTier(DateTime.Now);
                if (currentTier != null)
                {
                    currentTier.ToDate = tier.FromDate.AddMinutes(-1);
                }
                loyalty.CreateMemberTier(tier);
                if (currentTier != null)
                {
                    loyalty.UpdateMemberTier(currentTier);
                }
                _logger.Trace(_className, methodName,
                    string.Format("Member {0} is now in tier {1} with id {2}", IpCode, rtier.Name, tier.Id));
                return tier.Id;
            }
        }

        /// <summary>
        /// This method is used by the merge member method to move a tier from a victim to survivor.
        /// It expires the provided tier and expires the member's current tier and then adds a tier 
        /// equivalent to the victim tier to the surviving member.
        /// </summary>
        /// <param name="victimTier"></param>
        /// <returns></returns>
        public virtual MemberTier MoveTierToMember(MemberTier victimTier)
        {
            MemberTier toTier = new MemberTier()
            {
                MemberId = IpCode,
                TierDefId = victimTier.TierDefId,
                FromDate = victimTier.FromDate,
                ToDate = victimTier.ToDate
            };
            // See PI 22674
            victimTier.Description = EvaluateTier.TIER_MEMBERMERGE;
            //toTier.Description = string.Format("Member tier with id {0} being moved to member with ipcode {1}",
            //    victimTier.Id, victimTier.MemberId);
            victimTier.ToDate = DateTime.Now.AddMinutes(-1);
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                // update the old tier
                service.UpdateMemberTier(victimTier);
                // expire the member's current tier
                MemberTier currentTier = GetTier(DateTime.Now);
                if (currentTier != null)
                {
                    currentTier.ToDate = DateTime.Now.AddMinutes(-1);
                    service.UpdateMemberTier(currentTier);
                }
                // add the new tier
                service.CreateMemberTier(toTier);
                return toTier;
            }
        }

        public virtual MemberTier GetTier(DateTime date)
        {
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                return service.GetMemberTier(this, date);
            }
        }

        public virtual IList<MemberTier> GetTiers()
        {
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                return service.GetMemberTiers(this);
            }
        }

        public virtual bool IsInTier(string tierName)
        {
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                return service.IsMemberInTier(this, tierName);
            }
        }

        #endregion

        #region Reward Stuff
        /// <summary>
        /// This method gets all rewards for this member.
        /// </summary>
        /// <returns></returns>
        public virtual IList<MemberReward> GetRewards()
        {
            using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                return loyalty.GetMemberRewards(this, null);
            }
        }

        /// <summary>
        /// This method retrieves the member reward given the id of a reward definition.
        /// </summary>
        /// <param name="rewardDefId"></param>
        /// <returns></returns>
        public virtual IList<MemberReward> GetRewardsByDefId(long rewardDefId)
        {
            using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                return loyalty.GetMemberRewardsByDefId(this, rewardDefId);
            }
        }
        #endregion

        #region Point Balance Stuff

        #region Balances
        public virtual decimal GetPoints(DateTime? StartDate, DateTime? EndDate)
        {
            return GetPoints(null, null, StartDate, EndDate);
        }

        public virtual decimal GetPoints(PointType type, DateTime? StartDate, DateTime? EndDate)
        {
            long[] ptIds = null;
            if (type != null)
            {
                ptIds = new long[] { type.ID };
            }
            return GetPoints(ptIds, null, StartDate, EndDate);
        }

        public virtual decimal GetPoints(long[] pointTypeIds, DateTime? StartDate, DateTime? EndDate)
        {
            return GetPoints(pointTypeIds, null, StartDate, EndDate);
        }

        public virtual decimal GetPoints(long[] pointTypeIds, long[] pointEventIds, DateTime? StartDate, DateTime? EndDate)
        {
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                decimal pointBalance = 0;
                IList<long> vcKeys = new List<long>();
                lock (_loyaltyCards)
                {
                    foreach (VirtualCard vc in _loyaltyCards)
                    {
                        if (!vc.IsValid()) continue;
                        vcKeys.Add(vc.VcKey);
                    }
                }
                if (vcKeys.Count > 0)
                {
                    long[] keys = vcKeys.ToArray<long>();
                    pointBalance = service.GetPointBalance(keys, pointTypeIds, pointEventIds, null, StartDate, EndDate, null, null, null, null, null, null, null);
                }
                return pointBalance;
            }
        }

        public virtual decimal GetPointsToNextTier(bool includeExpiredPoints = false)
        {
            string methodName = "GetPointsToNextTier";

            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                decimal pointsToNextTier = 0;

                MemberTier currentTier = service.GetMemberTier(this, DateTime.Now);
                List<TierDef> allTiers = service.GetAllTierDefs();
                allTiers.Sort((t1, t2) => t1.EntryPoints.CompareTo(t2.EntryPoints));
                // get a list of all members active cards
                var cardList = (from v in LoyaltyCards where v.Status == VirtualCardStatusType.Active select v);
                if (cardList.Count() == 0)
                {
                    _logger.Trace(_className, methodName,
                        string.Format("There is no active card for the member ipcode {0} for calculating points to next tier", IpCode));
                    return pointsToNextTier;
                }

                ContextObject context = new ContextObject() { Owner = this };
                string[] pointTypes;
                string[] pointEvents;

                // points calculation would be driven by the point types and event configured in the tiers
                if (currentTier == null)
                {
                    foreach (TierDef tier in allTiers)
                    {
                        DateTime from = DateTime.Now;
                        DateTime to = DateTime.Now;
                        TierUtil.CalculateTierActivityDates(null, context, ref from, ref to, tier);
                        pointTypes = this.GetPointTypes(tier, service);
                        pointEvents = this.GetPointEvents(tier, service);
                        decimal currentBalance = TierUtil.GetCumulativePoints(this, LoyaltyCards, pointTypes, pointEvents, from, to, includeExpiredPoints);
                        _logger.Debug(_className, methodName,
                            string.Format("Cumulative points for member Ipcode {0} for Tier {1} are {2}.",
                            IpCode, tier.Name, currentBalance));
                        if (currentBalance < tier.EntryPoints)
                        {
                            pointsToNextTier = tier.EntryPoints - currentBalance;
                            break;
                        }
                    }
                }
                else
                {
                    TierDef rtier = service.GetTierDef(currentTier.TierDefId);
                    foreach (TierDef tier in allTiers)
                    {
                        DateTime from = DateTime.Now;
                        DateTime to = DateTime.Now;
                        TierUtil.CalculateTierActivityDates(null, context, ref from, ref to, tier);
                        pointTypes = this.GetPointTypes(tier, service);
                        pointEvents = this.GetPointEvents(tier, service);
                        decimal currentBalance = TierUtil.GetCumulativePoints(this, LoyaltyCards, pointTypes, pointEvents, from, to, includeExpiredPoints);
                        if (tier.EntryPoints > rtier.ExitPoints && currentBalance < tier.EntryPoints)
                        {
                            pointsToNextTier = tier.EntryPoints - currentBalance;
                            break;
                        }
                    }
                }

                return pointsToNextTier;
            }
        }

        public virtual decimal GetPointsToNextReward(string defaultTier = default(string), string overrideTier = default(string))
        {
            string methodName = "GetPointsToNextReward";

            using (var content = LWDataServiceUtil.ContentServiceInstance())
            using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                decimal pointsToNextReward = 0;
                long? tierId = null;

                IList<RewardDef> rewardDefs;
                IList<RewardDef> rewardDefsAll = content.GetAllRewardDefs();

                MemberTier currentTier = loyalty.GetMemberTier(this, DateTime.Now);
                if (currentTier != null || !string.IsNullOrEmpty(defaultTier) || !string.IsNullOrEmpty(overrideTier))
                {
                    TierDef defaultTierDef = string.IsNullOrEmpty(defaultTier) ? null : loyalty.GetTierDef(defaultTier);
                    TierDef overrideTierDef = string.IsNullOrEmpty(overrideTier) ? null : loyalty.GetTierDef(overrideTier);

                    tierId = !string.IsNullOrEmpty(overrideTier)
                                  ? overrideTierDef != null
                                    ? overrideTierDef.Id // If the tier is being overridden get the tier id from the tier name passed in.
                                    : -1
                                  : currentTier == null && !string.IsNullOrEmpty(defaultTier)
                                    ? defaultTierDef != null
                                      ? defaultTierDef.Id // If the member has no tier or it is expired and there is a default tier passed in get the tier id from the default tier name passed in
                                      : -1
                                    : currentTier != null
                                      ? currentTier.TierDefId // If there is no default tier or override tier passed in and the member has a current tier use that tier id
                                      : -1;

                    tierId = tierId == -1 ? null : tierId;
                }

                rewardDefs = (from x in rewardDefsAll
                              where x.TierId == tierId
                              && x.Active == true
                              && x.CatalogStartDate <= DateTime.Now
                              && x.CatalogEndDate >= DateTime.Now
                              select x).ToList<RewardDef>();

                if (rewardDefs.Count > 0)
                {
                    // sort the rewards based on PointsToEarn
                    var sortedRewards = rewardDefs.OrderBy(x => x.HowManyPointsToEarn).ToList<RewardDef>();

                    var vcKeys = (from x in LoyaltyCards where x.Status == VirtualCardStatusType.Active select x.VcKey);
                    if (vcKeys.Count() == 0)
                    {
                        _logger.Trace(_className, methodName,
                            string.Format("There is no active card for the member ipcode {0} for calculating points to next reward", IpCode));
                        return pointsToNextReward;
                    }
                    decimal currentBalance = loyalty.GetPointBalance(vcKeys.ToArray<long>(), null, null, null, null, null, null, null, string.Empty, string.Empty, null, null, null);
                    foreach (var reward in sortedRewards)
                    {
                        if (reward.HowManyPointsToEarn > currentBalance)
                        {
                            pointsToNextReward = reward.HowManyPointsToEarn - currentBalance;
                            break;
                        }
                    }
                }
                return pointsToNextReward;
            }
        }

        public virtual decimal GetPointsExcludingPointTypes(DateTime? StartDate, DateTime? EndDate, long[] excludePointTypes)
        {
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                decimal PointBalance = 0;
                lock (_loyaltyCards)
                {
                    foreach (VirtualCard vc in _loyaltyCards)
                    {
                        if (!vc.IsValid()) continue;
                        PointBalance += service.GetPointBalanceExcludingPointTypes(vc, StartDate, EndDate, excludePointTypes);
                    }
                }
                return PointBalance;
            }
        }

        public virtual decimal GetPointsToRewardChoice()
        {
            decimal ret = 0;
            
            using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            using (var content = LWDataServiceUtil.ContentServiceInstance())
            {
                RewardDef choice = loyalty.GetCurrentRewardChoiceOrDefault(this);
                if (choice!= null)
                {
                    ret = Math.Max(0, choice.HowManyPointsToEarn - GetPoints(null, null));
                }
            }
            return ret;
        }

        #endregion

        #region Points Management
        protected internal virtual decimal ConsumePoints(
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
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                decimal totalConsumed = 0;
                lock (_loyaltyCards)
                {
                    totalConsumed = service.ConsumePoints(_loyaltyCards, new List<PointType>() { pt }, new List<PointEvent> { pe }, consumptionDate, points, ownerType, string.Empty, ownerId, rowKey);
                }
                return totalConsumed;
            }
        }

        protected internal virtual decimal ConsumePoints(PointType pt, PointEvent pe,
            DateTime consumptionDate, decimal points, PointTransactionOwnerType ownerType, long ownerId, long rowKey, string locationId, string changedBy)
        {
            return ConsumePoints(pt, pe, consumptionDate, DateTimeUtil.MaxValue, points, ownerType, ownerId, rowKey, locationId, changedBy);
        }

        protected internal virtual void HoldAllPoints(DateTime transactionDate, string notes)
        {
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                lock (_loyaltyCards)
                {
                    foreach (VirtualCard vc in _loyaltyCards)
                    {
                        service.HoldAllPoints(vc, transactionDate, notes);
                    }
                }
            }
        }

        protected internal virtual void HoldPoints(PointType pt, PointEvent pe, decimal points, DateTime transactionDate,
            PointTransactionOwnerType ownerType, long ownerId, long rowKey, string notes, string locationId, string changedBy)
        {
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                lock (_loyaltyCards)
                {
                    foreach (VirtualCard vc in _loyaltyCards)
                    {
                        service.HoldPoints(vc, pt, pe, points, transactionDate, ownerType, ownerId, rowKey, notes, locationId, changedBy);
                    }
                }
            }
        }

        protected internal virtual decimal ExpirePoints(string notes, PointExpirationReason reason)
        {
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                List<long> vcKeys = new List<long>();
                decimal pointsExpired = 0;
                lock (_loyaltyCards)
                {
                    foreach (VirtualCard vc in _loyaltyCards)
                    {
                        if (!vc.IsValid()) continue;
                        vcKeys.Add(vc.VcKey);
                    }
                }
                if (vcKeys.Count > 0)
                {
                    pointsExpired = service.ExpirePoints(vcKeys.ToArray<long>(), null, DateTime.Now, reason, notes);
                }
                return pointsExpired;
            }
        }

        protected internal virtual void ExtendPointsExpirationDate(DateTime from, DateTime to, DateTime newExpiryDate)
        {
            lock (_loyaltyCards)
            {
                using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
                {
                    foreach (VirtualCard vc in _loyaltyCards)
                    {
                        loyalty.ExtendPointsExpirationDate(vc, from, to, newExpiryDate);
                    }
                }
            }
        }
        #endregion

        #endregion

        private string[] GetPointTypes(TierDef tier, LoyaltyDataService svc)
        {
            string[] pointTypes = tier.GetPointTypes();
            if (pointTypes == null || pointTypes.Length < 1)
            {
                IList<PointType> listPointTypes = svc.GetAllPointTypes();
                pointTypes = listPointTypes.Select(s => s.Name).ToArray();
            }
            return pointTypes;
        }

        private string[] GetPointEvents(TierDef tier, LoyaltyDataService svc)
        {
            string[] pointEvents = tier.GetPointEvents();
            if (pointEvents == null || pointEvents.Length < 1)
            {
                IList<PointEvent> listPointEvents = svc.GetAllPointEvents();
                pointEvents = listPointEvents.Select(s => s.Name).ToArray();
            }
            return pointEvents;
        }

        #endregion
    }
}
