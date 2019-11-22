using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class GetRewardCatalogItem : OperationProviderBase
    {
        public GetRewardCatalogItem() : base("GetRewardCatalogItem") { }

        public override string Invoke(string source, string parms)
        {
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for reward catalog item.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string language = args.ContainsKey("Language") ? (string)args["Language"] : LanguageChannelUtil.GetDefaultCulture();
                string channel = args.ContainsKey("Channel") ? (string)args["Channel"] : LanguageChannelUtil.GetDefaultChannel();
                long rewardId = (long)args["RewardID"];
                bool returnAttributes = args.ContainsKey("ReturnAttributes") ? (bool)args["ReturnAttributes"] : false;


                RewardDef reward = ContentService.GetRewardDef(rewardId);
                if (reward == null)
                {
                    throw new LWOperationInvocationException("Unable to find reward definition with id " + rewardId) { ErrorCode = 3342 };
                }
                Product product = ContentService.GetProduct(reward.ProductId);
                if (product == null)
                {
                    throw new LWOperationInvocationException("Unable to find product definition with id " + reward.ProductId) { ErrorCode = 3365 };
                }
                IList<ProductVariant> variants = product.GetVariants();

                APIStruct[] productVariants = null;
                if (variants.Count > 0)
                {
                    productVariants = new APIStruct[variants.Count];
                    int i = 0;
                    foreach (ProductVariant variant in variants)
                    {
                        APIArguments productVariantParams = new APIArguments();
                        productVariantParams.Add("ProductVariantID", variant.ID);
                        productVariantParams.Add("Description", variant.VariantDescription);
                        if (!string.IsNullOrEmpty(variant.PartNumber))
                        {
                            productVariantParams.Add("PartNumber", variant.PartNumber);
                        }
                        if (variant.Quantity != null)
                        {
                            productVariantParams.Add("Quantity", variant.Quantity);
                        }
                        productVariantParams.Add("VariantOrder", variant.VariantOrder);
                        APIStruct v = new APIStruct() { Name = "ProductVariant", IsRequired = false, Parms = productVariantParams };
                        productVariants[i++] = v;
                    }
                }


                APIArguments productParams = new APIArguments();
                productParams.Add("ProductID", product.Id);
                productParams.Add("CategoryName", ContentService.GetCategory(product.CategoryId).Name);
                productParams.Add("ProductName", product.Name);
                if (!string.IsNullOrEmpty(product.BrandName))
                {
                    productParams.Add("BrandName", product.BrandName);
                }
                if (!string.IsNullOrEmpty(product.GetShortDescription(language, channel)))
                {
                    productParams.Add("ShortDescription", product.GetShortDescription(language, channel));
                }
                if (!string.IsNullOrEmpty(product.GetLongDescription(language, channel)))
                {
                    productParams.Add("LongDescription", product.GetLongDescription(language, channel));
                }
                if (product.Quantity != null)
                {
                    productParams.Add("Quantity", product.Quantity);
                }
                if (!string.IsNullOrEmpty(product.PartNumber))
                {
                    productParams.Add("PartNumber", product.PartNumber);
                }
                if (productVariants != null && productVariants.Length > 0)
                {
                    productParams.Add("ProductVariant", productVariants);
                }
                APIStruct p = new APIStruct() { Name = "Product", IsRequired = false, Parms = productParams };

                APIArguments rewardCatalogItemParams = new APIArguments();
                rewardCatalogItemParams.Add("RewardID", rewardId);
                rewardCatalogItemParams.Add("RewardName", reward.Name);
                rewardCatalogItemParams.Add("DisplayName", reward.GetDisplayName(language, channel));
                if (!string.IsNullOrEmpty(reward.CertificateTypeCode))
                {
                    rewardCatalogItemParams.Add("TypeCode", reward.CertificateTypeCode);
                }
                if (!string.IsNullOrEmpty(reward.GetShortDescription(language, channel)))
                {
                    rewardCatalogItemParams.Add("ShortDescription", reward.GetShortDescription(language, channel));
                }
                if (!string.IsNullOrEmpty(reward.GetLongDescription(language, channel)))
                {
                    rewardCatalogItemParams.Add("LongDescription", reward.GetLongDescription(language, channel));
                }
                if (!string.IsNullOrEmpty(reward.GetLegalText(language, channel)))
                {
                    rewardCatalogItemParams.Add("LegalText", reward.GetLegalText(language, channel));
                }
                rewardCatalogItemParams.Add("CurrencyToEarn", reward.HowManyPointsToEarn);
                rewardCatalogItemParams.Add("CurrencyType", reward.PointType);
                if (!string.IsNullOrEmpty(reward.SmallImageFile))
                {
                    rewardCatalogItemParams.Add("SmallImageFile", reward.SmallImageFile);
                }
                if (!string.IsNullOrEmpty(reward.MediumImageFile))
                {
                    rewardCatalogItemParams.Add("MediumImageFile", reward.MediumImageFile);
                }
                if (!string.IsNullOrEmpty(reward.LargeImageFile))
                {
                    rewardCatalogItemParams.Add("LargeImageFile", reward.LargeImageFile);
                }
                if (reward.CatalogStartDate != null)
                {
                    rewardCatalogItemParams.Add("CatalogStartDate", reward.CatalogStartDate);
                }
                if (reward.CatalogEndDate != null)
                {
                    rewardCatalogItemParams.Add("CatalogEndDate", reward.CatalogEndDate);
                }
                rewardCatalogItemParams.Add("Active", reward.Active);
                if (reward.RedeemTimeLimit != null)
                {
                    rewardCatalogItemParams.Add("RedeemTimeLimit", reward.RedeemTimeLimit);
                }
                rewardCatalogItemParams.Add("Product", p);

                if (returnAttributes && reward.Attributes.Count > 0)
                {
                    APIStruct[] atts = new APIStruct[reward.Attributes.Count];
                    int idx = 0;
                    foreach (ContentAttribute ra in reward.Attributes)
                    {
                        ContentAttributeDef def = ContentService.GetContentAttributeDef(ra.ContentAttributeDefId);
                        APIArguments attparms = new APIArguments();
                        attparms.Add("AttributeName", def.Name);
                        attparms.Add("AttributeValue", ra.Value);
                        APIStruct v = new APIStruct() { Name = "ContentAttributes", IsRequired = false, Parms = attparms };
                        atts[idx++] = v;
                    }
                    rewardCatalogItemParams.Add("ContentAttributes", atts);
                }

                APIStruct rewardCatalogItem = new APIStruct() { Name = "RewardCatalogItem", IsRequired = true, Parms = rewardCatalogItemParams };

                APIArguments resultParams = new APIArguments();
                resultParams.Add("RewardCatalogItem", rewardCatalogItem);

                response = SerializationUtils.SerializeResult(Name, Config, resultParams);

                return response;
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }
    }
}
