using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
using Brierley.FrameWork.Data.DataAccess;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
	public class GetCouponDefinitionCount : OperationProviderBase
	{
		public GetCouponDefinitionCount() : base("GetCouponDefinitionCount") { }

		public override string Invoke(string source, string parms)
		{
			try
			{
				string response = string.Empty;

				List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
				ActiveCouponOptions options = ActiveCouponOptions.Default;

				if (!string.IsNullOrEmpty(parms))
				{
					APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

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
				}

				int count = ContentService.HowManyCouponDefs(parmsList, options);

				APIArguments responseArgs = new APIArguments();
				responseArgs.Add("Count", count);
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
