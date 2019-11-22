using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Data
{
	public class CacheRegions
	{
		public const string AttributeSetMetadataById = "AttributeSetMetaDataById";
		public const string AttributeSetMetadataByName = "AttributeSetMetaDataByName";
		public const string PromotionByCode = "PromotionByCode";
		//private const string RULETRIGGER_BYEVENT = "RuleTriggerByEvent";
		public const string EventByName = "EventByName";
		public const string RuleTriggerByName = "RuleTriggerByName";
		public const string RuleTriggerByAttributeSet = "RuleTriggerByAttSet";
		public const string RuleTriggerByPromotion = "RuleTriggerByPromotion";
		public const string RuleTriggerByObjectName = "RuleTriggerByObjectName";
		public const string PointTypeByName = "PointTypeByName";
		public const string PointEventByName = "PointEventByName";
		public const string Tiers = "TierDefList";
		public const string TierByName = "TierDefByName";
        public const string TierById = "TierDefById";
        public const string Rewards = "RewardsList";
        public const string RewardByName = "RewardDefByName";
		public const string CouponByName = "CouponDefByName";
		public const string ContentAttributeDefByName = "ContentAttributeDefByName";
        public const string ContentAttributeDefById = "ContentAttributeDefById";
        public const string ValidatorTriggerByAttribute = "ValidatorTriggerByAttribute";
		public const string BScriptExpressionByName = "BScriptExpressionsByName";
        public const string RemoteAssemblies = "RemoteAssemblies";
        public const string Categories = "CategoriesList";
        public const string CategoryById = "CategoryById";
        public const string Products = "ProductsList";
		public const string ProductById = "ProductById";
		public const string Languages = "LanguageDefList";
		public const string Channels = "ChannelDefList";
        public const string FulfillmentProviderProductMapByProduct = "FulfillmentProviderProductMapByProduct";
        public const string FulfillmentProviderProductMapByVariant = "FulfillmentProviderProductMapByVariant";
        public const string PointTypes = "PointTypeList";
        public const string PointEvents = "PointEventList";

        //RTW 10/07/2016    LW-2759 Adding cache manager support for message definitions
        public const string MessageByName = "MessageByName";
        public const string MessageById = "MessageById";

		public const string PromoDataFileByName = "PromoDataFileByName";
		public const string PromoMappingFileByName = "PromoMappingFileByName";

		// Member Cache
		public const string MemberByIPCode = "MemberByIpCode";
		public const string MemberByLoyaltyId = "MemberByLoyaltyId";
		// Client configuration
		public const string ClientConfigurationByKey = "ClientConfigurationByKey";
		// Offeres
		public const string BonusByName = "BonusDefByName";
		public const string BonusById = "BonusDefByID"; //LW-1159


		//Content
		public const string DocumentById = "DocumentById";
		public const string TemplateById = "TemplateById";
		public const string TextBlockById = "TextBlockById";
		public const string ContentElementById = "ContentElementById";
		public const string ContentElementByName = "ContentElementByName";
		public const string ContentAttributeById = "ContentAttributeById";
		public const string GlobalAttributes = "GlobalAttributesList";
		public const string GlobalAttributeByName = "GlobalAttributeByName";
		public const string BatchByName = "BatchByName";
		public const string BatchElements = "BatchElementsList";
		public const string BatchGlobals = "BatchGlobalsList";

		public const string XsltByDocumentId = "XsltByDocumentId";

		//survey
		public const string SurveyLanguageById = "LanguageById";
		public const string SurveyLanguageByDescription = "LanguageByDescription";
		public const string SurveyById = "SurveyById";
		public const string SurveyByName = "SurveyByName";
		public const string QuestionByStateId = "QuestionByStateId";

        //REST ACL
	    public const string RestConsumerById = "RestConsumerById";
	    public const string RestConsumerByConsumerId = "RestConsumerByConsumerId";
	    public const string RestConsumerByUsername = "RestConsumerByUsername";
	    public const string RestGroupById = "RestGroupById";
	    public const string RestGroupByName = "RestGroupByName";
	    public const string RestGroupsByRestConsumerId = "RestGroupsByRestConsumerId";
	    public const string RestRoleById = "RestRoleById";
	    public const string RestRoleByName = "RestRoleByName";
	    public const string RestRolesByRestConsumerId = "RestRolesByRestConsumerId";
	    public const string RestRolesByRestGroupId = "RestRolesByRestGroupId";
	    public const string RestResourceById = "RestResourceById";
	    public const string RestResourcesByRestRoleId = "RestResourcesByRestRoleId";
	}
}
