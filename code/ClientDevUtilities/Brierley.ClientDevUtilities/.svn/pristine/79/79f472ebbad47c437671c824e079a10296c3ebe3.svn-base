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
    public class MGCouponDef
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
        public virtual string ShortDescription { get; set; }
        [DataMember]
        public virtual string Description { get; set; }
        [DataMember]
        public virtual DateTime StartDate { get; set; }
        [DataMember]
        public virtual DateTime? ExpiryDate { get; set; }
        [DataMember]
        public virtual Int64 UsesAllowed { get; set; }
        [DataMember]
        public virtual Int32 DisplayOrder { get; set; }
        [DataMember]
        public virtual List<MGContentAttribute> ContentAttributes { get; set; }         
        #endregion

        #region Data Transfer Methods
        public static MGCouponDef Hydrate(CouponDef coupon, string lang, string channel, bool returnAttributes)
        {
            MGCouponDef mc = new MGCouponDef()
            {
                Id = coupon.Id,
                Name = coupon.Name,
                CategoryId = coupon.CategoryId,
                TypeCode = coupon.CouponTypeCode,
                ShortDescription = coupon.GetShortDescription(lang,channel),
                Description = coupon.GetDescription(lang, channel),
                StartDate = coupon.StartDate,
                ExpiryDate = coupon.ExpiryDate,
                UsesAllowed = coupon.UsesAllowed,
                DisplayOrder = coupon.DisplayOrder != null ? coupon.DisplayOrder.Value : 1
            };
            if (returnAttributes)
            {
                mc.ContentAttributes = new List<MGContentAttribute>();
                foreach (ContentAttribute att in coupon.ContentAttributes)
                {
                    mc.ContentAttributes.Add(MGContentAttribute.Hydrate(att));
                }
            }
            return mc;
        }
        #endregion
    }
}