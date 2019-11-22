using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Bonuses
{
	public class GetBonusDefinitionCount : OperationProviderBase
	{
		private const string _className = "GetBonusDefinitionCount";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public GetBonusDefinitionCount()
			: base("GetBonusDefinitionCount")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parameters)
		{
			const string methodName = "Invoke";

			if (parameters == null || parameters.Length != 2)
			{
				string errMsg = "Invalid parameters provided for GetBonusDefinitionCount.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			var parameterList = new List<Dictionary<string, object>>();

			bool activeOnly = (bool)parameters[0];
			if (activeOnly)
			{
				var entry = new Dictionary<string, object>()
				{
					{ "Property", "Active" }, 
					{ "Predicate", LWCriterion.Predicate.Eq }, 
					{ "Value", true }
				};
				parameterList.Add(entry);
			}


			string contentAttributesStr = (string)parameters[1];
			if (!string.IsNullOrEmpty(contentAttributesStr))
			{
				MGContentAttribute[] contentAttributes = MGContentAttribute.ConvertFromJson(contentAttributesStr);
				foreach (MGContentAttribute ca in contentAttributes)
				{
					Dictionary<string, object> e = new Dictionary<string, object>();
					e.Add("Property", ca.AttributeName);
					e.Add("Predicate", LWCriterion.Predicate.Eq);
					e.Add("Value", ca.AttributeValue);
					if (ca.AttributeName != "Name")
					{
						e.Add("IsAttribute", true);
					}
					parameterList.Add(e);
				}
			}
			int count = ContentService.HowManyBonusDefs(parameterList);
			return count;
		}
	}
}