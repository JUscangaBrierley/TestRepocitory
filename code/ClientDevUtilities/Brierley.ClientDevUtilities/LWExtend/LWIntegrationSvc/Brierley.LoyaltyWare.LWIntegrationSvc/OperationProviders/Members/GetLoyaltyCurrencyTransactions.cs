using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class GetLoyaltyCurrencyTransactions : OperationProviderBase
	{
		private const string _className = "GetLoyaltyCurrencyTransactions";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetLoyaltyCurrencyTransactions() : base("GetLoyaltyCurrencyTransactions") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No member id provided to retrieve account history.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string loyaltyIdNumber = args.ContainsKey("CardID") ? (string)args["CardID"] : string.Empty;
				DateTime? startDate = args.ContainsKey("StartDate") ? (DateTime?)args["StartDate"] : null;
				DateTime? endDate = args.ContainsKey("EndDate") ? (DateTime?)args["EndDate"] : null;
				string[] pointTypeNames = args.ContainsKey("LoyaltyCurrencyNames") ? (string[])args["LoyaltyCurrencyNames"] : null;
				string[] pointEventNames = args.ContainsKey("LoyaltyEventNames") ? (string[])args["LoyaltyEventNames"] : null;
				int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
				int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;

				LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

				Member member = LoadMember(args);

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

				long[] pointTypeIds = null;
				if (pointTypeNames != null && pointTypeNames.Length > 0)
				{
					IList<PointType> ptList = LoyaltyDataService.GetPointTypes(pointTypeNames);
					if (ptList.Count < pointTypeNames.Length)
					{
						throw new LWOperationInvocationException("Unable to find loyalty currencies.") { ErrorCode = 3311 };
					}
					if (ptList.Count > 0)
					{
						pointTypeIds = new long[ptList.Count];
						int idx = 0;
						foreach (PointType pt in ptList)
						{
							pointTypeIds[idx++] = pt.ID;
						}
					}
				}

				long[] pointEventIds = null;
				if (pointEventNames != null && pointEventNames.Length > 0)
				{
					IList<PointEvent> peList = LoyaltyDataService.GetPointEvents(pointEventNames);
					if (peList.Count < pointEventNames.Length)
					{
						throw new LWOperationInvocationException("Unable to find loyalty events.") { ErrorCode = 3310 };
					}
					if (peList.Count > 0)
					{
						pointEventIds = new long[peList.Count];
						int idx = 0;
						foreach (PointEvent pe in peList)
						{
							pointEventIds[idx++] = pe.ID;
						}
					}
				}

				bool retrieveExpiredTransactions = false;
				if (!string.IsNullOrEmpty(GetFunctionParameter("RetrieveExpiredTransactions")))
				{
					retrieveExpiredTransactions = bool.Parse(GetFunctionParameter("RetrieveExpiredTransactions"));
				}

				IList<PointTransaction> txnList = null;
				if (string.IsNullOrEmpty(loyaltyIdNumber))
				{
					txnList = LoyaltyDataService.GetPointTransactionsByPointTypePointEvent(member, startDate, endDate, null, pointTypeIds, pointEventIds, null, null, null, retrieveExpiredTransactions, batchInfo);
				}
				else
				{
					VirtualCard vc = member.GetLoyaltyCard(loyaltyIdNumber);
					if (vc == null)
					{
						throw new LWOperationInvocationException("Unable to find loyalty card from CardID " + loyaltyIdNumber) { ErrorCode = 3358 };
					}
					long[] vckeys = new long[] { vc.VcKey };
					txnList = LoyaltyDataService.GetPointTransactionsByPointTypePointEvent(vckeys, startDate, endDate, null, pointTypeIds, pointEventIds, null, null, null, retrieveExpiredTransactions, batchInfo);
				}

				if (txnList == null || txnList.Count == 0)
				{
					throw new LWOperationInvocationException("No point transactions found.") { ErrorCode = 3362 };
				}

				APIArguments responseArgs = new APIArguments();
				APIStruct[] txnPoints = new APIStruct[txnList.Count];
				int idx1 = 0;
				foreach (PointTransaction txn in txnList)
				{
					txnPoints[idx1++] = PointTransactionHelper.SerializePointTransaction(txn, "PointsHistory");
				}
				responseArgs.Add("PointsHistory", txnPoints);
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
