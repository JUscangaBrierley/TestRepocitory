using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    [DataContract]
    public class MGLoyaltyCard
    {
        #region Data Members

        [DataMember]
        public virtual Int64 VcKey { get; set; }

        [DataMember]
        public virtual String LoyaltyIdNumber { get; set; }

        [DataMember]
        public virtual Int64 IpCode { get; set; }

        [DataMember]
        public virtual DateTime DateIssued { get; set; }

        [DataMember]
        public virtual DateTime DateRegistered { get; set; }

        [DataMember]
        public virtual DateTime? ExpirationDate { get; set; }

        [DataMember]
        public virtual VirtualCardStatusType Status { get; set; }

        [DataMember]
        public virtual Boolean IsPrimary { get; set; }

        [DataMember]
        public virtual Int64 CardType { get; set; }

		[DataMember]
		public List<MGClientEntity> ChildEntities { get; set; }

        #endregion

        public MGLoyaltyCard()
        {
        }

        #region Data Transfer Methods
        public static MGLoyaltyCard Hydrate(Brierley.FrameWork.Data.DomainModel.VirtualCard vc)
        {
            MGLoyaltyCard v = new MGLoyaltyCard()
            {
                VcKey = vc.VcKey,
                LoyaltyIdNumber = vc.LoyaltyIdNumber,
                IpCode = vc.IpCode,
                DateIssued = vc.DateIssued,
                DateRegistered = vc.DateRegistered,
                ExpirationDate = vc.ExpirationDate,
                Status = vc.Status,
                IsPrimary = vc.IsPrimary,
                CardType = vc.CardType
            };

            Dictionary<string, List<IClientDataObject>> children = vc.GetChildAttributeSets();
            if (children != null && children.Count > 0)
            {
				v.ChildEntities = new List<MGClientEntity>();
                foreach (KeyValuePair<string, List<IClientDataObject>> kv in children)
                {
                    foreach (IClientDataObject entity in kv.Value)
                    {
                        MGClientEntity mgEntity = MGClientEntity.Hydrate(entity);
						//mgEntity.Name = kv.Key;
                        v.ChildEntities.Add(mgEntity);
                    }
                }
            }

            return v;
        }
        #endregion

        #region Serialization/Deserialization
        public static MGLoyaltyCard DeSerialize(string jsonString)
        {
            MGLoyaltyCard member = JsonConvert.DeserializeObject<MGLoyaltyCard>(jsonString);
            return member;
        }
        #endregion

        #region Populate Methods
        public VirtualCard PopulateVirtualCard(LWIntegrationConfig config, MobileGatewayDirectives.APIOperationDirective opDirective)
        {
            VirtualCard vc = new VirtualCard()
            {
                LoyaltyIdNumber = LoyaltyIdNumber,
                CardType = CardType
            };
            vc.DateIssued = DateIssued;
            vc.DateRegistered = DateRegistered;
            vc.ExpirationDate = ExpirationDate;            
            vc.IsPrimary = false;
            		
            try
            {
                MGMemberUtils.PopulateAttributeSets(ChildEntities, config, opDirective.CreateDreatives, vc, null, config.GetDateConversionFormat(), true, true);
            }
            catch (Exception)
            {
                throw;
            }
            return vc;
        }
        #endregion
    }
}