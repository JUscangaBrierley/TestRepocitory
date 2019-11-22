using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class GetAccountActivityCount : OperationProviderBase
	{
		private const string _className = "GetAccountActivityCount";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetAccountActivityCount() : base("GetAccountActivityCount") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided for reward catalog count.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				DateTime? startDate = args.ContainsKey("StartDate") ? (DateTime?)args["StartDate"] : null;
				DateTime? endDate = args.ContainsKey("EndDate") ? (DateTime?)args["EndDate"] : null;

				if (endDate < startDate)
				{
					_logger.Error(_className, methodName, "End date cannot be earlier than the start date");
					throw new LWOperationInvocationException("End date cannot be earlier than the start date") { ErrorCode = 3204 };
				}
				if (startDate != null)
				{
					startDate = DateTimeUtil.GetBeginningOfDay((DateTime)startDate);
				}
				if (endDate != null)
				{
					endDate = DateTimeUtil.GetEndOfDay((DateTime)endDate);
				}

				Member member = LoadMember(args);

				AttributeSetMetaData meta = LoyaltyDataService.GetAttributeSetMetaData("TxnHeader");
				if (meta == null)
				{
					throw new LWOperationInvocationException("Standard implementation requires TxnHeader attribute set to be defined.") { ErrorCode = 3357 };
				}

				LWCriterion crit = new LWCriterion(meta.Name);
				if (startDate != null)
				{
					crit.Add(LWCriterion.OperatorType.AND, "TxnDate", startDate, LWCriterion.Predicate.Gt);
				}
				if (endDate != null)
				{
					crit.Add(LWCriterion.OperatorType.AND, "TxnDate", endDate, LWCriterion.Predicate.Le);
				}

				long count = 0;
				foreach (VirtualCard vc in member.LoyaltyCards)
				{
					count += LoyaltyDataService.CountAttributeSetObjects(vc, meta, crit);
				}

				APIArguments responseArgs = new APIArguments();
				responseArgs.Add("Count", (int)count);
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
