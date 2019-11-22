using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Promotions
{
	public class GetMemberPromotions : OperationProviderBase
	{
		private const string _className = "GetMemberPromotions";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetMemberPromotions() : base("GetMemberPromotions") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
                List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
                IList<Promotion> globalPromotions = null;

                if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to retrieve member promotions.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                bool unexpiredOnly = args.ContainsKey("ActiveOnly") ? (bool)args["ActiveOnly"] : false;
				int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
				int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;


				LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

				bool returnDefinition = false;
				if (args.ContainsKey("ReturnDefinition"))
				{
					returnDefinition = (bool)args["ReturnDefinition"];
				}
				string language = LanguageChannelUtil.GetDefaultCulture();
				string channel = LanguageChannelUtil.GetDefaultChannel();
				if (args.ContainsKey("Language"))
				{
					language = (string)args["Language"];
				}
				if (args.ContainsKey("Channel"))
				{
					channel = (string)args["Channel"];
				}
				bool returnAttributes = false;
				if (args.ContainsKey("ReturnAttributes"))
				{
					returnAttributes = (bool)args["ReturnAttributes"];
				}
                bool returnGlobalAttributes = false;
                if(args.ContainsKey("ReturnGlobalPromotions"))
                {
                    returnGlobalAttributes = (bool)args["ReturnGlobalPromotions"];
                }
                if (args.ContainsKey("ContentSearchAttributes"))
                {
                    APIStruct[] attList = (APIStruct[])args["ContentSearchAttributes"];
                    foreach (APIStruct att in attList)
                    {
                        Dictionary<string, object> entry = new Dictionary<string, object>();
                        entry.Add("Property", att.Parms["AttributeName"]);
                        entry.Add("Predicate", LWCriterion.Predicate.Eq);
                        entry.Add("Value", att.Parms["AttributeValue"]);
                        if (att.Parms["AttributeName"].ToString() != "Name" && att.Parms["AttributeName"].ToString() != "Code")
                        {
                            entry.Add("IsAttribute", true);
                        }
                        parmsList.Add(entry);
                    }
                }

                //Global promotions will be define as only non-tagerget and no enrollment support
                if (returnGlobalAttributes)
                {

                    //Filter out target promotions
                    Dictionary<string, object> targetedEntry = new Dictionary<string, object>();
                    targetedEntry.Add("Property", "Targeted");
                    targetedEntry.Add("Predicate", LWCriterion.Predicate.Eq);
                    targetedEntry.Add("Value", "0");
                    targetedEntry.Add("IsAttribute", false);
                    parmsList.Add(targetedEntry);

                    //filter out promotions with enrollment support
                    Dictionary<string, object> enrolledEntry = new Dictionary<string, object>();
                    enrolledEntry.Add("Property", "EnrollmentSupportType");
                    enrolledEntry.Add("Predicate", LWCriterion.Predicate.Eq);
                    enrolledEntry.Add("Value", PromotionEnrollmentSupportType.None);
                    enrolledEntry.Add("IsAttribute", false);
                    parmsList.Add(enrolledEntry);
                }
                //if they have indicated active then we will add the filter to the global promotions
                if (args.ContainsKey("ActiveOnly") && (bool)args["ActiveOnly"])
                {
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    entry.Add("Property", "Active");
                    entry.Add("Predicate", LWCriterion.Predicate.Eq);
                    entry.Add("Value", true);
                    parmsList.Add(entry);
                }

                if (returnDefinition)
				{
					if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
					{
						throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
					}
					if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
					{
						throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
					}
				}

				Member member = LoadMember(args);
                APIArguments resultParams = new APIArguments();
                IList<MemberPromotion> promotions = LoyaltyDataService.GetMemberPromotionsByMember(member.IpCode, batchInfo, unexpiredOnly);
				if (promotions.Count > 0)
				{
					APIStruct[] memberPromotions = new APIStruct[promotions.Count];
					int msgIdx = 0;
					foreach (MemberPromotion promo in promotions)
					{
						memberPromotions[msgIdx++] = PromotionUtil.SerializeMemberPromotion(language, channel, promo, returnDefinition, returnAttributes);
					}
					
					resultParams.Add("MemberPromotion", memberPromotions);

                }

                //If the user indicates that they want global promotions we need to go fetch them
                if (returnGlobalAttributes)
                {
                    globalPromotions = ContentService.GetPromotions(parmsList, returnAttributes, batchInfo);
                    if (globalPromotions.Count > 0)
                    {
                        APIStruct[] globalMemberPromotions = new APIStruct[globalPromotions.Count];
                        int msgIdx = 0;
                        foreach (Promotion promo in globalPromotions)
                        {
                            globalMemberPromotions[msgIdx++] = PromotionUtil.SerializePromotionDefinition(language, channel, promo, returnAttributes);
                        }

                        resultParams.Add("GlobalPromotion", globalMemberPromotions);

                    }
                }

                //If both global promotions and member promotions are empty then throw the error. 
                if((globalPromotions == null || globalPromotions.Count == 0) && promotions.Count == 0)
				{
                    throw new LWOperationInvocationException("No member promotions found.") { ErrorCode = 3362 };
                }
                else
                {
                    response = SerializationUtils.SerializeResult(Name, Config, resultParams);
                    return response;
                }
            }
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
	}
}
