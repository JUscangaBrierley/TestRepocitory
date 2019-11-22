using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Bonuses
{
	public class GetBonusDefinitions : OperationProviderBase
	{
		private const string _className = "GetBonusDefinitions";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public GetBonusDefinitions()
			: base("GetBonusDefinitions")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parameters)
		{
			const string methodName = "Invoke";

			if (parameters == null || parameters.Length != 7)
			{
				string errMsg = "Invalid parameters provided for GetBonusDefinitions.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			string language = (string)parameters[0];
			if (string.IsNullOrEmpty(language))
			{
				language = LanguageChannelUtil.GetDefaultCulture();
			}
			string channel = (string)parameters[1];
			if (string.IsNullOrEmpty(channel))
			{
				channel = LanguageChannelUtil.GetDefaultChannel();
			}

			var paramsList = new List<Dictionary<string, object>>();

			bool activeOnly = (bool)parameters[2];
			if (activeOnly)
			{
				var entry = new Dictionary<string, object>()
				{
					{ "Property", "Active" }, 
					{ "Predicate", LWCriterion.Predicate.Eq }, 
					{ "Value", true }
				};
				paramsList.Add(entry);
			}
			
			string contentAttributesStr = (string)parameters[3];
			if (!string.IsNullOrEmpty(contentAttributesStr))
			{
				MGContentAttribute[] contentAttributes = MGContentAttribute.ConvertFromJson(contentAttributesStr);
				foreach (MGContentAttribute ca in contentAttributes)
				{
					var e = new Dictionary<string, object>()
					{
						{ "Property", ca.AttributeName }, 
						{ "Predicate", LWCriterion.Predicate.Eq }, 
						{ "Value", ca.AttributeValue }
					};
					if (ca.AttributeName != "Name")
					{
						e.Add("IsAttribute", true);
					}
					paramsList.Add(e);
				}
			}

			int startIndex = (int)parameters[4];
			int? batchSize = (int)parameters[5];
			if (batchSize == 0)
			{
				batchSize = null; //otherwise GetValidBatch(...) below throws an exception
			}

			LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

			bool returnAttributes = (bool)parameters[6];

			IList<BonusDef> bonuses = ContentService.GetBonusDefs(paramsList, returnAttributes, batchInfo);

			//this really should not be throwing an exception just because nothing was found. That's not exceptional.
			//if (bonuses.Count == 0)
			//{
			//	throw new LWOperationInvocationException("No Bonus definitions found.") { ErrorCode = 3362 };
			//}

			var bonusList = new List<MGBonusDef>();
			foreach (BonusDef bonus in bonuses)
			{
				bonusList.Add(MGBonusDef.Hydrate(bonus, language, channel, returnAttributes));
			}

			return bonusList;
		}
	}
}