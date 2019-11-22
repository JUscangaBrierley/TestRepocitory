using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
	public class GetCouponDefinitions : OperationProviderBase
	{
		public GetCouponDefinitions() : base("GetCouponDefinitions") { }

		public override string Invoke(string source, string parms)
		{
			try
			{
				string response = string.Empty;

				string language = string.Empty;
				string channel = string.Empty;
				ActiveCouponOptions options = ActiveCouponOptions.Default;

				List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
				int pageNumber = 0;
				int resultsPerPage = 0;
				bool returnAttributes = false;

				if (!string.IsNullOrEmpty(parms))
				{
					APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
					if (args.ContainsKey("Language"))
					{
						language = (string)args["Language"];
					}
					if (args.ContainsKey("Channel"))
					{
						channel = (string)args["Channel"];
					}

					if (args.ContainsKey("ActiveCouponOptions"))
					{
						APIStruct optionList = (APIStruct)args["ActiveCouponOptions"];

						if (optionList.Parms != null)
						{
							if (optionList.Parms.ContainsKey("RestrictDate"))
							{
								var restrictDate = optionList.Parms["RestrictDate"];
								if (restrictDate != null)
								{
									options.RestrictDate = (DateTime)restrictDate;
								}
							}

							if (optionList.Parms.ContainsKey("RestrictGlobalCoupons"))
							{
								var restrictGlobal = (string)optionList.Parms["RestrictGlobalCoupons"];
								if (restrictGlobal != null)
								{
									options.RestrictGlobalCoupons = (GlobalCouponRestriction)Enum.Parse(typeof(GlobalCouponRestriction), restrictGlobal);
								}
							}
						}
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
							if (att.Parms["AttributeName"].ToString() != "Name")
							{
								entry.Add("IsAttribute", true);
							}
							parmsList.Add(entry);
						}
					}
					if (args.ContainsKey("ReturnAttributes"))
					{
						returnAttributes = (bool)args["ReturnAttributes"];
					}

					pageNumber = args.ContainsKey("PageNumber") ? (int)args["PageNumber"] : -1;
					resultsPerPage = args.ContainsKey("ResultsPerPage") ? (int)args["ResultsPerPage"] : -1;
				}

				// validate language and channel
				if (string.IsNullOrEmpty(language))
				{
					language = LanguageChannelUtil.GetDefaultCulture();
				}
				if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
				{
					throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
				}
				if (string.IsNullOrEmpty(channel))
				{
					channel = LanguageChannelUtil.GetDefaultChannel();
				}
				if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
				{
					throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
				}

				//LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);
				if (pageNumber < 1)
				{
					throw new LWOperationInvocationException("Invalid PageNumber provided. Must be greater than zero.") { ErrorCode = 3304 };
				}
				if (resultsPerPage < 1)
				{
					throw new LWOperationInvocationException("Invalid ResultsPerPage provided. Must be greater than zero.") { ErrorCode = 3305 };
				}

				PetaPoco.Page<CouponDef> coupons = ContentService.GetCouponDefs(parmsList, options, returnAttributes, pageNumber, resultsPerPage);
				if (coupons.Items.Count == 0)
				{
					throw new LWOperationInvocationException("No content available that matches the specified criteria.") { ErrorCode = 3362 };
				}

				APIArguments responseArgs = new APIArguments();
				APIStruct[] couponList = new APIStruct[coupons.Items.Count];
				int i = 0;
				foreach (CouponDef coupon in coupons.Items)
				{
					couponList[i++] = CouponHelper.SerializeCouponDef(language, channel, returnAttributes, coupon);
				}

				responseArgs.Add("CouponDefinition", couponList);
				responseArgs.Add("TotalPages", coupons.TotalPages);
				responseArgs.Add("TotalItems", coupons.TotalItems);

				response = SerializationUtils.SerializeResult(Name, Config, responseArgs);

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
