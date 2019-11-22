using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
    internal static class CouponHelper
    {
        public static APIStruct SerializeCouponDef(string language, string channel, bool returnAttributes, CouponDef coupon)
        {
            APIArguments cparms = new APIArguments();
            cparms.Add("Id", coupon.Id);
            cparms.Add("Name", coupon.Name);
			cparms.Add("IsGlobal", coupon.IsGlobal);
			cparms.Add("CategoryId", coupon.CategoryId);
            if (!string.IsNullOrEmpty(coupon.CouponTypeCode))
            {
                cparms.Add("TypeCode", coupon.CouponTypeCode);
            }
            if (!string.IsNullOrEmpty(coupon.LogoFileName))
            {
                cparms.Add("LogoFileName", coupon.LogoFileName);
            }
            string descr = coupon.GetShortDescription(language, channel);
            if (!string.IsNullOrEmpty(descr))
            {
                cparms.Add("ShortDescription", descr);
            }
            descr = coupon.GetDescription(language, channel);
            if (!string.IsNullOrEmpty(descr))
            {
                cparms.Add("Description", descr);
            }
            cparms.Add("StartDate", coupon.StartDate);
            if (coupon.ExpiryDate != null)
            {
                cparms.Add("ExpiryDate", coupon.ExpiryDate);
            }
            cparms.Add("UsesAllowed", coupon.UsesAllowed);
            if (coupon.DisplayOrder != null)
            {
                cparms.Add("DisplayOrder", coupon.DisplayOrder.ToString());
            }

            if (returnAttributes && coupon.Attributes.Count > 0)
            {
                APIStruct[] atts = new APIStruct[coupon.Attributes.Count];
                int idx = 0;
                using (ContentService service = LWDataServiceUtil.ContentServiceInstance())
                {
                    foreach (ContentAttribute ra in coupon.Attributes)
                    {
                        ContentAttributeDef def = service.GetContentAttributeDef(ra.ContentAttributeDefId);
                        APIArguments attparms = new APIArguments();
                        attparms.Add("AttributeName", def.Name);
                        attparms.Add("AttributeValue", ra.Value);
                        APIStruct v = new APIStruct() { Name = "ContentAttributes", IsRequired = false, Parms = attparms };
                        atts[idx++] = v;
                    }
                }
                cparms.Add("ContentAttributes", atts);
            }
            APIStruct rv = new APIStruct() { Name = "CouponDefinition", IsRequired = false, Parms = cparms };
            return rv;
        }

        public static APIStruct SerializeMemberCoupon(Member member, string language, string channel, MemberCoupon coupon, bool returnAttributes)
        {
            using (ContentService service = LWDataServiceUtil.ContentServiceInstance())
            {
                CouponDef def = service.GetCouponDef(coupon.CouponDefId);

                APIArguments cParams = new APIArguments();
                cParams.Add("Id", coupon.ID);
                cParams.Add("CouponDefId", def.Id);
                cParams.Add("Name", def.Name);
                if (!string.IsNullOrEmpty(def.CouponTypeCode))
                {
                    cParams.Add("TypeCode", def.CouponTypeCode);
                }
                if (!string.IsNullOrEmpty(coupon.CertificateNmbr))
                {
                    cParams.Add("CertNmbr", coupon.CertificateNmbr);
                }

                string description = def.GetShortDescription(language, channel);
                if (!string.IsNullOrEmpty(description))
                {
                    cParams.Add("ShortDescription", def.EvaluateBScript(member, description));
                }
                description = def.GetDescription(language, channel);
                if (!string.IsNullOrEmpty(description))
                {
                    cParams.Add("Description", def.EvaluateBScript(member, description));
                }
                cParams.Add("TimesUsed", coupon.TimesUsed);
                cParams.Add("DateIssued", coupon.DateIssued);
                if (coupon.DateRedeemed != null)
                {
                    cParams.Add("DateRedeemed", (DateTime)coupon.DateRedeemed);
                }
                cParams.Add("StartDate", coupon.StartDate);
                if (coupon.ExpiryDate != null)
                {
                    cParams.Add("ExpiryDate", (DateTime)coupon.ExpiryDate);
                }
                string status = coupon.Status != null ? coupon.Status.Value.ToString() : CouponStatus.Active.ToString();
                cParams.Add("Status", status);
                if (coupon.DisplayOrder != null)
                {
                    cParams.Add("DisplayOrder", (int)coupon.DisplayOrder);
                }

                if (returnAttributes && def.Attributes.Count > 0)
                {
                    APIStruct[] atts = new APIStruct[def.Attributes.Count];
                    int idx = 0;

                    foreach (ContentAttribute ra in def.Attributes)
                    {
                        ContentAttributeDef cadef = service.GetContentAttributeDef(ra.ContentAttributeDefId);
                        APIArguments attparms = new APIArguments();
                        attparms.Add("AttributeName", cadef.Name);
                        attparms.Add("AttributeValue", ra.Value);
                        APIStruct v = new APIStruct() { Name = "ContentAttributes", IsRequired = false, Parms = attparms };
                        atts[idx++] = v;
                    }
                    cParams.Add("ContentAttributes", atts);
                }

                APIStruct rv = new APIStruct() { Name = "MemberCoupon", IsRequired = false, Parms = cParams };
                return rv;
            }
        }
    }
}
