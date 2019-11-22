using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;
using Newtonsoft.Json;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    [DataContract]
    public class MGMember
    {
        private const string _className = "MGMember";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
		
        [DataMember]
        public virtual long IpCode { get; set; }
        
        [DataMember]
        public virtual MemberStatusEnum Status { get; set; }

        [DataMember]
        public virtual DateTime? BirthDate { get; set; }

        [DataMember]
        public virtual string FirstName { get; set; }

        [DataMember]
        public virtual string LastName { get; set; }

        [DataMember]
        public virtual string MiddleName { get; set; }

        [DataMember]
        public virtual string NamePrefix { get; set; }

        [DataMember]
        public virtual string NameSuffix { get; set; }

        [DataMember]
        public virtual string AlternateId { get; set; }

        [DataMember]
        public virtual string Username { get; set; }

        [DataMember]
        public virtual string PrimaryEmailAddress { get; set; }

        [DataMember]
        public virtual string PrimaryPhoneNumber { get; set; }

        [DataMember]
        public virtual string PrimaryPostalCode { get; set; }

        [DataMember]
        public virtual string PreferredLanguage { get; set; }

        [DataMember]
		public List<MGClientEntity> ChildEntities { get; set; }
        

        public MGMember()
        {
            Status = MemberStatusEnum.Active;
        }

        public static MGMember Hydrate(Member m)
        {
            MGMember mm = new MGMember()
            {
                IpCode = m.IpCode,                
                Status = m.MemberStatus,
                BirthDate = m.BirthDate,
                FirstName = m.FirstName,
                LastName = m.LastName,
                MiddleName = m.MiddleName,
                NamePrefix = m.NamePrefix,
                NameSuffix = m.NameSuffix,
                AlternateId = m.AlternateId,
                Username = m.Username,
                PrimaryEmailAddress = m.PrimaryEmailAddress,
                PrimaryPhoneNumber = m.PrimaryPhoneNumber,
                PrimaryPostalCode = m.PrimaryPostalCode,
                PreferredLanguage = m.PreferredLanguage,
            };

			Dictionary<string, List<IClientDataObject>> children = m.GetChildAttributeSets();
			if (children != null && children.Count > 0)
			{
				mm.ChildEntities = new List<MGClientEntity>();
				foreach (KeyValuePair<string, List<IClientDataObject>> kv in children)
				{
					foreach (IClientDataObject entity in kv.Value)
					{
						MGClientEntity mgEntity = MGClientEntity.Hydrate(entity);
						//mgEntity.Name = kv.Key;
						mm.ChildEntities.Add(mgEntity);
					}
				}
			}

            return mm;
        }

		public static MGMember DeSerialize(string jsonString)
		{
			MGMember member = JsonConvert.DeserializeObject<MGMember>(jsonString);
			return member;
		}

        public Member PopulateMember(LWIntegrationConfig config, MobileGatewayDirectives.APIOperationDirective opDirective)
        {
            Member m = new Member();
			m.BirthDate = BirthDate.HasValue ? BirthDate.Value.ToLocalTime() : BirthDate;
            m.FirstName = FirstName;
            m.LastName = LastName;
            m.MiddleName = MiddleName;
            m.NamePrefix = NamePrefix;
            m.NameSuffix = NameSuffix;
            m.AlternateId = AlternateId;
            m.Username = Username;
            m.PrimaryEmailAddress = PrimaryEmailAddress;
            m.PrimaryPhoneNumber = PrimaryPhoneNumber;
            m.PrimaryPostalCode = PrimaryPostalCode;
            m.PreferredLanguage = PreferredLanguage;

            try
            {
                MGMemberUtils.PopulateAttributeSets(ChildEntities, config, opDirective.CreateDreatives, m, null, config.GetDateConversionFormat(), true, true);
            }
            catch (Exception)
            {
                throw;
            }
            return m;
        }

        public Member PopulateMember(LWIntegrationConfig config, MobileGatewayDirectives.APIOperationDirective opDirective, Member existing)
        {            
            existing.BirthDate = BirthDate.HasValue ? BirthDate.Value.ToLocalTime() : BirthDate;
            existing.FirstName = FirstName;
            existing.LastName = LastName;
            existing.MiddleName = MiddleName;
            existing.NamePrefix = NamePrefix;
            existing.NameSuffix = NameSuffix;
            existing.AlternateId = AlternateId;
            existing.Username = Username;
            existing.PrimaryEmailAddress = PrimaryEmailAddress;
            existing.PrimaryPhoneNumber = PrimaryPhoneNumber;
            existing.PrimaryPostalCode = PrimaryPostalCode;
            existing.PreferredLanguage = PreferredLanguage;

            MGMemberUtils.PopulateAttributeSets(ChildEntities, config, opDirective.CreateDreatives, existing, null, config.GetDateConversionFormat(), true, true);

            return existing;
        }        
    }
}