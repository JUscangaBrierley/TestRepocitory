using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    [DataContract]
    public class MGMemberCoupon
    {
        #region Properties
        [DataMember]
        public virtual Int64 Id { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual Int64 CategoryId { get; set; }
        [DataMember]
        public virtual string TypeCode { get; set; }
        [DataMember]
        public virtual string CertNmbr { get; set; }
        [DataMember]
        public virtual string ShortDescription { get; set; }
        [DataMember]
        public virtual string Description { get; set; }
        [DataMember]
        public virtual Int64 TimesUsed { get; set; }
        [DataMember]
        public virtual Int64 TimesRemaining { get; set; }
        [DataMember]
        public virtual DateTime DateIssued { get; set; }
        [DataMember]
        public virtual DateTime? DateRedeemed { get; set; }
        [DataMember]
        public virtual DateTime? ExpiryDate { get; set; }
        [DataMember]
        public virtual string Status { get; set; }
        [DataMember]
        public virtual Int32 DisplayOrder { get; set; }
        [DataMember]
        public virtual List<MGContentAttribute> ContentAttributes { get; set; }         
        #endregion

        #region Data Transfer Methods
		public static MGMemberCoupon Hydrate(Member member, MemberCoupon coupon, string lang, string channel, bool returnAttributes)
		{
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				CouponDef cdef = content.GetCouponDef(coupon.CouponDefId);
				MGMemberCoupon mc = new MGMemberCoupon()
				{
					Id = coupon.ID,
					Name = cdef.Name,
					CategoryId = cdef.CategoryId,
					TypeCode = cdef.CouponTypeCode,
					CertNmbr = coupon.CertificateNmbr,
					ShortDescription = cdef.EvaluateBScript(member, cdef.GetShortDescription(lang, channel)),
					Description = cdef.EvaluateBScript(member, cdef.GetDescription(lang, channel)),
					TimesUsed = coupon.TimesUsed,
					TimesRemaining = cdef.UsesAllowed - coupon.TimesUsed,
					DateIssued = coupon.DateIssued,
					DateRedeemed = coupon.DateRedeemed,
					ExpiryDate = coupon.ExpiryDate,
					Status = coupon.Status != null ? coupon.Status.Value.ToString() : CouponStatus.Active.ToString(),
					DisplayOrder = coupon.DisplayOrder != null ? coupon.DisplayOrder.Value : 1
				};
				if (returnAttributes)
				{
					mc.ContentAttributes = new List<MGContentAttribute>();
					foreach (ContentAttribute att in cdef.ContentAttributes)
					{
						mc.ContentAttributes.Add(MGContentAttribute.Hydrate(att));
					}
				}
				return mc;
			}
		}
        #endregion
    }
}