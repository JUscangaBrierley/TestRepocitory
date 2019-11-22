using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.bScript.Functions
{
    [Serializable]
    [ExpressionContext(Description = "Returns a content attribute value for a given component",
        DisplayName = "GetContentAttributeValue",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Content,
        ExpressionReturns = ExpressionApplications.Strings,

        WizardDescription = "Content Attribute Value",
        AdvancedWizard = true,
        WizardCategory = WizardCategories.Content

        )]

    [ExpressionParameter(Name = "ContentObjType", WizardDescription = "Which content type?", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 1)]
    [ExpressionParameter(Name = "SearchType", WizardDescription = "How to find the content?", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentTypeSearch, Order = 2)]
    [ExpressionParameter(Name = "SearchValue", WizardDescription = "What is the search value?", Type = ExpressionApplications.Strings, Optional = false, Order = 3)]
    [ExpressionParameter(Name = "ContentAttributeName", WizardDescription = "What is the name of the content attribute?", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentAttributeName, Order = 4)]
    public class GetContentAttributeValue : UnaryOperation
    {
        public GetContentAttributeValue()
        {
        }

        internal GetContentAttributeValue(Expression rhs)
            : base("GetContentAttributeValue", rhs)
        {
            if (!(rhs is ParameterList && ((ParameterList)rhs).Expressions.Length == 4))
                throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetContentAttributeValue");
        }

        public new string Syntax
        {
            get
            {
                return "GetContentAttributeValue('ContentObjType', 'SearchType', 'SearchValue', 'ContentAttributeName')";
            }
        }

        public override object evaluate(ContextObject contextObject)
        {
            ParameterList plist = GetRight() as ParameterList;
            string sCompType = plist.Expressions[0].evaluate(contextObject).ToString();
            string searchType = plist.Expressions[1].evaluate(contextObject).ToString().ToLower(); // name, id, partnumber, variantpartnumber, code
            string searchValue = plist.Expressions[2].evaluate(contextObject).ToString();
            string contentAttributeName = plist.Expressions[3].evaluate(contextObject).ToString();
            
            ContentObjType componentType;
            if (!Enum.TryParse(sCompType, true, out componentType))
                throw new CRMException(string.Format("Invalid content type: {0}", sCompType));

            long lSearchValue = 0;
            if (searchType.Equals("id") && !long.TryParse(searchValue, out lSearchValue))
                throw new CRMException(string.Format("Invalid number in the search value for search type 'Id': {0}", searchValue));

            ContentAttribute value = null;
            ContentDefBase contentObject = null;

            using (LoyaltyDataService loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            using (ContentService content = LWDataServiceUtil.ContentServiceInstance())
            {
                switch (componentType)
                {
                    case ContentObjType.Product:
                        if (searchType.Equals("id"))
                            contentObject = content.GetProduct(lSearchValue);
                        else if (searchType.Equals("name"))
                            contentObject = content.GetProduct(searchValue);
                        else if (searchType.Equals("partnumber"))
                            contentObject = content.GetProductByPartNumber(searchValue);
                        else if (searchType.Equals("variantpartnumber"))
                            contentObject = content.GetProductByVariantPartNumber(searchValue);
                        else
                            throw new CRMException(string.Format("Invalid Product search type '{0}'. Valid search types are Id, Name, PartNumber, and VariantPartNumber.", searchType));
                        break;

                    case ContentObjType.Reward:
                        if (searchType.Equals("id"))
                            contentObject = content.GetRewardDef(lSearchValue);
                        else if (searchType.Equals("name"))
                            contentObject = content.GetRewardDef(searchValue);
                        else
                            throw new CRMException(string.Format("Invalid Reward search type '{0}'. Valid search types are Id and Name.", searchType));
                        break;

                    case ContentObjType.Message:
                        if (searchType.Equals("id"))
                            contentObject = content.GetMessageDef(lSearchValue);
                        else if (searchType.Equals("name"))
                            contentObject = content.GetMessageDef(searchValue);
                        else
                            throw new CRMException(string.Format("Invalid Message search type '{0}'. Valid search types are Id and Name.", searchType));
                        break;

                    case ContentObjType.Coupon:
                        if (searchType.Equals("id"))
                            contentObject = content.GetCouponDef(lSearchValue);
                        else if (searchType.Equals("name"))
                            contentObject = content.GetCouponDef(searchValue);
                        else if (searchType.Equals("code"))
                            contentObject = content.GetCouponDefByCode(searchValue);
                        else
                            throw new CRMException(string.Format("Invalid Coupon search type '{0}'. Valid search types are Id, Name, and Code.", searchType));
                        break;

                    case ContentObjType.Bonus:
                        if (searchType.Equals("id"))
                            contentObject = content.GetBonusDef(lSearchValue);
                        else if (searchType.Equals("name"))
                            contentObject = content.GetBonusDef(searchValue);
                        else
                            throw new CRMException(string.Format("Invalid Bonus search type '{0}'. Valid search types are Id and Name.", searchType));
                        break;

                    case ContentObjType.Promotion:
                        if (searchType.Equals("id"))
                            contentObject = content.GetPromotion(lSearchValue);
                        else if (searchType.Equals("name"))
                            contentObject = content.GetPromotionByName(searchValue);
                        else if (searchType.Equals("code"))
                            contentObject = content.GetPromotionByCode(searchValue);
                        else
                            throw new CRMException(string.Format("Invalid Promotion search type '{0}'. Valid search types are Id, Name, and Code.", searchType));
                        break;

                    case ContentObjType.Tier:
                        if (searchType.Equals("id"))
                            contentObject = loyalty.GetTierDef(lSearchValue);
                        else if (searchType.Equals("name"))
                            contentObject = loyalty.GetTierDef(searchValue);
                        else
                            throw new CRMException(string.Format("Invalid Tier search type '{0}'. Valid search types are Id and Name.", searchType));
                        break;

                    default:
                        throw new CRMException(string.Format("Unsupported content object type: {0}", componentType));
                }

                if (contentObject == null)
                    throw new CRMException(string.Format("Content of type '{0}' by search type '{1}' not found with value '{2}'", sCompType, searchType, searchValue));

                value = contentObject.Attributes.Where(c => content.GetContentAttributeDef(c.ContentAttributeDefId).Name.Equals(contentAttributeName)).FirstOrDefault();

                if (value != null)
                    return value.Value;

                // Try the default value
                var contentAttribute = content.GetContentAttributeDef(contentAttributeName);
                if (contentAttribute != null && contentAttribute.ContentTypes.Contains(componentType.ToString()))
                    return contentAttribute.DefaultValues;

                throw new CRMException(string.Format("Content attribute named '{0}' not found for type '{1}'", contentAttributeName, componentType.ToString()));
            }
        }
    }
}
