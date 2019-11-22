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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Promotions
{
    internal static class PromotionUtil
    {
        public static APIStruct SerializePromotionDefinition(string language, string channel, Promotion promo, bool returnAttributes)
        {
            using (ContentService service = LWDataServiceUtil.ContentServiceInstance())
            {
                APIArguments rparms = new APIArguments();
                rparms.Add("Id", promo.Id);
                rparms.Add("Code", promo.Code);
                rparms.Add("Name", promo.Name);
                rparms.Add("InternalDescription", promo.PromotionDescription);
                rparms.Add("Targeted", promo.Targeted);
                rparms.Add("Enrollment", (int)promo.EnrollmentSupportType);
                if (promo.DisplayOrder != null)
                {
                    rparms.Add("DisplayOrder", promo.DisplayOrder.ToString());
                }
                rparms.Add("StartDate", promo.StartDate);
                if (promo.EndDate != null)
                {
                    rparms.Add("EndDate", promo.EndDate);
                }
                string displayName = promo.GetDisplayName(language, channel);
                if (!string.IsNullOrEmpty(displayName))
                {
                    rparms.Add("PromotionDisplayName", displayName);
                }
                string descr = promo.GetDescription(language, channel);
                if (!string.IsNullOrEmpty(descr))
                {
                    rparms.Add("Description", descr);
                }
                string content = promo.GetContent(language, channel);
                if (!string.IsNullOrEmpty(content))
                {
                    rparms.Add("Content", content);
                }
                if (returnAttributes && promo.Attributes.Count > 0)
                {
                    APIStruct[] atts = new APIStruct[promo.Attributes.Count];
                    int idx = 0;
                    foreach (ContentAttribute ra in promo.Attributes)
                    {
                        ContentAttributeDef def = service.GetContentAttributeDef(ra.ContentAttributeDefId);
                        APIArguments attparms = new APIArguments();
                        attparms.Add("AttributeName", def.Name);
                        attparms.Add("AttributeValue", ra.Value);
                        APIStruct v = new APIStruct() { Name = "ContentAttributes", IsRequired = false, Parms = attparms };
                        atts[idx++] = v;
                    }
                    rparms.Add("ContentAttributes", atts);
                }
                APIStruct rv = new APIStruct() { Name = "PromotionDefinition", IsRequired = false, Parms = rparms };
                return rv;
            }
        }

        public static APIStruct SerializeMemberPromotion(string language, string channel, MemberPromotion memberPromo, bool returnDefintion, bool returnAttributes)
        {
            using (LoyaltyDataService service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                Promotion def = service.GetPromotionByCode(memberPromo.Code);

                APIArguments msgParams = new APIArguments();
                msgParams.Add("Id", memberPromo.Id);
                if (!string.IsNullOrEmpty(memberPromo.CertificateNmbr))
                {
                    msgParams.Add("CertificateNmbr", memberPromo.CertificateNmbr);
                }
				msgParams.Add("Enrolled", memberPromo.Enrolled);
                if (returnDefintion)
                {
                    APIStruct promoDefStruct = SerializePromotionDefinition(language, channel, def, returnAttributes);
                    msgParams.Add("PromotionDefinition", promoDefStruct);
                }
                APIStruct rv = new APIStruct() { Name = "MemberPromotion", IsRequired = false, Parms = msgParams };

                return rv;
            }
        }        
    }
}
