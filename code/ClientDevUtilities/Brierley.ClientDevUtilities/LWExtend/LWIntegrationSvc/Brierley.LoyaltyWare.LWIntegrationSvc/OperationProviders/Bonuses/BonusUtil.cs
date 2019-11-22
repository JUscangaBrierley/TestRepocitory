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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Bonuses
{
	internal static class BonusUtil
	{
		public static APIStruct SerializeBonusDef(string language, string channel, bool returnAttributes, BonusDef bonus)
		{
			APIArguments cparms = new APIArguments();
			cparms.Add("Id", bonus.Id);
			cparms.Add("Name", bonus.Name);
			cparms.Add("CategoryId", bonus.CategoryId);
			if (!string.IsNullOrEmpty(bonus.LogoImageHero))
			{
				cparms.Add("LogoImageHero", bonus.LogoImageHero);
			}
			if (!string.IsNullOrEmpty(bonus.LogoImageWeb))
			{
				cparms.Add("LogoImageWeb", bonus.LogoImageWeb);
			}
			if (!string.IsNullOrEmpty(bonus.LogoImageMobile))
			{
				cparms.Add("LogoImageMobile", bonus.LogoImageMobile);
			}
			string descr = bonus.GetDescription(language, channel);
			if (!string.IsNullOrEmpty(descr))
			{
				cparms.Add("Description", descr);
			}
			descr = bonus.GetHeadline(language, channel);
			if (!string.IsNullOrEmpty(descr))
			{
				cparms.Add("Headline", descr);
			}
			descr = bonus.GetFinishedHtml(language, channel);
			if (!string.IsNullOrEmpty(descr))
			{
				cparms.Add("FinishedHtml", descr);
			}
			descr = bonus.GetQuotaMetHtml(language, channel);
			if (!string.IsNullOrEmpty(descr))
			{
				cparms.Add("QuotaMetHtml", descr);
			}
			//todo: adding this to satisfy LW-604. However, this is based on the OperationOutput of the config file. We should 
			//be passing the output parameters through here somehow. In doing so, we'd be able to read what exactly is expected
			//to be part of the response, which means we can avoid loading values into memory if they're going to be discarded.
			//Example: customers do not want HtmlContent in the bonus definition list (perfectly reasonable and almost expected).
			//         We shouldn't waste memory and resources pulling HtmlContent here (among other things) and adding it to the 
			//         response just to discard it once we determine that it's not going to be used.
			descr = bonus.GetHtmlContent(language, channel);
			if (!string.IsNullOrEmpty(descr))
			{
				cparms.Add("HtmlContent", descr);
			}
			if (bonus.Points != null && bonus.Points > 0)
			{
				//PointType pt = service.GetPointType(bonus.PointType);
				//cparms.Add("LoyaltyCurrency", pt.Name);
				cparms.Add("CurrencyAmount", bonus.Points);
			}
			if (!string.IsNullOrEmpty(bonus.SurveyText))
			{
				cparms.Add("SurveyText", bonus.SurveyText);
			}
			if (bonus.SurveyId != null)
			{
                using (SurveyManager surveySvc = LWDataServiceUtil.SurveyManagerInstance())
                {
                    SMSurvey survey = surveySvc.RetrieveSurvey((long)bonus.SurveyId);
                    if (survey != null)
                    {
                        cparms.Add("SurveyId", survey.ID);
                        cparms.Add("SurveyName", survey.Name);
                    }
                }
			}
			if (!string.IsNullOrEmpty(bonus.ReferralLabel))
			{
				cparms.Add("ReferralLabel", bonus.ReferralLabel);
			}
			if (!string.IsNullOrEmpty(bonus.MovieUrl))
			{
				cparms.Add("MovieUrl", bonus.MovieUrl);
			}
			if (!string.IsNullOrEmpty(bonus.ReferralUrl))
			{
				cparms.Add("ActionCompletePage", bonus.ReferralUrl);
			}
			if (bonus.DisplayOrder != null)
			{
				cparms.Add("DisplayOrder", bonus.DisplayOrder);
			}
			cparms.Add("StartDate", bonus.StartDate);
			if (bonus.ExpiryDate != null)
			{
				cparms.Add("ExpiryDate", bonus.ExpiryDate);
			}
			if (bonus.Quota != null && bonus.Quota > 0)
			{
				cparms.Add("Quota", bonus.Quota);
			}
			if (bonus.ApplyQuotaToReferral.GetValueOrDefault())
			{
				cparms.Add("ApplyQuotaToReferral", true);
			}
			if (returnAttributes && bonus.Attributes.Count > 0)
			{
				APIStruct[] atts = new APIStruct[bonus.Attributes.Count];
				int idx = 0;
                using (ContentService service = LWDataServiceUtil.ContentServiceInstance())
                {
                    foreach (ContentAttribute ra in bonus.Attributes)
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

			APIStruct rv = new APIStruct() { Name = "BonusDefinition", IsRequired = false, Parms = cparms };

			return rv;
		}

		public static APIStruct SerializeMemberBonus(string language, string channel, MemberBonus bonus)
		{
            using (ContentService service = LWDataServiceUtil.ContentServiceInstance())
            {
                BonusDef def = service.GetBonusDef(bonus.BonusDefId);

                APIArguments bParams = new APIArguments();
                bParams.Add("Id", bonus.ID);
                bParams.Add("BonusDefId", def.Id);
                bParams.Add("Name", def.Name);
                string description = def.GetDescription(language, channel);
                if (!string.IsNullOrEmpty(description))
                {
                    bParams.Add("Description", description);
                }
                description = def.GetHeadline(language, channel);
                if (!string.IsNullOrEmpty(description))
                {
                    bParams.Add("Headline", description);
                }
                description = def.GetFinishedHtml(language, channel);
                if (!string.IsNullOrEmpty(description))
                {
                    bParams.Add("FinishedHtml", description);
                }
                description = def.GetQuotaMetHtml(language, channel);
                if (!string.IsNullOrEmpty(description))
                {
                    bParams.Add("QuotaMetHtml", description);
                }
                description = def.GetHtmlContent(language, channel);
                if (!string.IsNullOrEmpty(description))
                {
                    bParams.Add("HtmlContent", description);
                }
                bParams.Add("TimesClicked", bonus.TimesClicked);
                bParams.Add("StartDate", bonus.StartDate);
                if (bonus.ExpiryDate != null)
                {
                    bParams.Add("ExpiryDate", (DateTime)bonus.ExpiryDate);
                }
                if (bonus.DisplayOrder != null)
                {
                    bParams.Add("DisplayOrder", bonus.DisplayOrder);
                }

                APIStruct rv = new APIStruct() { Name = "BonusDefinition", IsRequired = false, Parms = bParams };

                return rv;
            }
		}
	}
}
