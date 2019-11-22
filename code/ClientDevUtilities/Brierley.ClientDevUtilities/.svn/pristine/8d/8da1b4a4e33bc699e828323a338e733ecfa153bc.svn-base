using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class GetAccountActivitySummary : OperationProviderBase
	{
		private const string _className = "GetAccountActivitySummary";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetAccountActivitySummary() : base("GetAccountActivitySummary") { }
		
		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			string[] predefinedAttributes = GetAdditionalAttributesList();
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
				bool getPointsHistory = args.ContainsKey("GetPointsHistory") ? (bool)args["GetPointsHistory"] : false;
				int? startIndex = args.ContainsKey("SummaryStartIndex") ? (int?)args["SummaryStartIndex"] : null;
				int? batchSize = args.ContainsKey("SummaryBatchSize") ? (int?)args["SummaryBatchSize"] : null;

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

				LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

				bool getOtherPointsHistory = args.ContainsKey("GetOtherPointsHistory") ? (bool)args["GetOtherPointsHistory"] : false;
				int? otherStartIndex = args.ContainsKey("OtherStartIndex") ? (int?)args["OtherStartIndex"] : null;
				int? otherBatchSize = args.ContainsKey("OtherBatchSize") ? (int?)args["OtherBatchSize"] : null;

				bool retrieveExpiredTransactions = false;
				if (!string.IsNullOrEmpty(GetFunctionParameter("RetrieveExpiredTransactions")))
				{
					retrieveExpiredTransactions = bool.Parse(GetFunctionParameter("RetrieveExpiredTransactions"));
				}

				Member member = LoadMember(args);

				IList<IClientDataObject> txnHeaders = AccountActivityUtil.GetAccountActivitySummary(member, startDate, endDate, getPointsHistory, retrieveExpiredTransactions, batchInfo);


				APIArguments responseArgs = new APIArguments();

				if (txnHeaders != null && txnHeaders.Count > 0)
				{
					APIStruct[] summary = new APIStruct[txnHeaders.Count];
					int i = 0;
					foreach (IClientDataObject dobj in txnHeaders)
					{
						APIArguments rparms = new APIArguments();
						rparms.Add("TxnHeaderId", dobj.GetAttributeValue("TxnHeaderId"));
						object txnDate = dobj.GetAttributeValue("TxnDate");
						if (txnDate != null)
						{
							rparms.Add("TxnDate", txnDate);
						}
						rparms.Add("TxnNumber", dobj.GetAttributeValue("TxnNumber"));
						object txnRegNmbr = dobj.GetAttributeValue("TxnRegisterNumber");
						if (txnRegNmbr != null && !string.IsNullOrEmpty(txnRegNmbr.ToString()))
						{
							rparms.Add("TxnRegisterNumber", txnRegNmbr);
						}
						if (dobj.HasTransientProperty("Store"))
						{
							StoreDef store = (StoreDef)dobj.GetTransientProperty("Store");
							APIArguments tparms = new APIArguments();
							if (!string.IsNullOrEmpty(store.StoreName))
							{
								tparms.Add("StoreName", store.StoreName);
							}
							if (!string.IsNullOrEmpty(store.StoreNumber))
							{
								tparms.Add("StoreNumber", store.StoreNumber);
							}
							if (!string.IsNullOrEmpty(store.City))
							{
								tparms.Add("City", store.City);
							}
							if (!string.IsNullOrEmpty(store.StateOrProvince))
							{
								tparms.Add("State", store.StateOrProvince);
							}
							APIStruct storeInfo = new APIStruct() { Name = "TxnStoreInfo", IsRequired = false, Parms = tparms };
							rparms.Add("TxnStoreInfo", storeInfo);
						}
						rparms.Add("TxnAmount", dobj.GetAttributeValue("TxnAmount"));
						rparms.Add("TxnChannel", dobj.GetAttributeValue("TxnChannel"));
						if (getPointsHistory)
						{
							IList<PointTransaction> headerTxnList = (IList<PointTransaction>)dobj.GetTransientProperty("PointsHistory");
							if (headerTxnList != null && headerTxnList.Count > 0)
							{
								APIStruct[] txnPoints = new APIStruct[headerTxnList.Count];
								int idx1 = 0;
								foreach (PointTransaction txn in headerTxnList)
								{
									txnPoints[idx1++] = PointTransactionHelper.SerializePointTransaction(txn, "PointsHistory");
								}
								rparms.Add("PointsHistory", txnPoints);
							}
						}
						// handle additional attributes
						if (predefinedAttributes != null && predefinedAttributes.Length > 0)
						{
							Dictionary<string, object> attValues = new Dictionary<string, object>();
							foreach (string attName in predefinedAttributes)
							{
								object attValue = dobj.GetAttributeValue(attName);
								if (attValue != null && !string.IsNullOrEmpty(attValue.ToString()))
								{
									attValues.Add(attName, attValue);
								}
							}
							if (attValues.Count > 0)
							{
								APIStruct[] addAtts = new APIStruct[attValues.Count];
								int x = 0;
								foreach (string key in attValues.Keys)
								{
									APIArguments aparms = new APIArguments();
									aparms.Add("AttributeName", key);
									aparms.Add("AttributeValue", attValues[key]);
									aparms.Add(key, attValues[key]);
									APIStruct addAtt = new APIStruct() { Name = "AdditionalAttributes", IsRequired = false, Parms = aparms };
									addAtts[x++] = addAtt;
								}
								rparms.Add("AdditionalAttributes", addAtts);
							}
						}
						APIStruct activitySummary = new APIStruct() { Name = "AccountActivitySummary", IsRequired = false, Parms = rparms };
						summary[i++] = activitySummary;
					}

					responseArgs.Add("AccountActivitySummary", summary);
				}
				if (getOtherPointsHistory)
				{
					batchInfo = LWQueryBatchInfo.GetValidBatch(otherStartIndex, otherBatchSize, Config.EnforceValidBatch);

					IList<PointTransaction> filteredList = null;
					try
					{
						filteredList = AccountActivityUtil.GetOtherPointsHistory(
							 member,
							 startDate,
							 endDate,
							 GetFunctionParameter("OrphanTxnTypesFilter"),
							 GetFunctionParameter("OrphanPointTypesFilter"),
							 GetFunctionParameter("OrphanPointEventsFilter"),
							 retrieveExpiredTransactions,
							 false,
							 batchInfo);
					}
					catch (LWException ex)
					{
						if (ex.ErrorCode != 3230)
						{
							_logger.Error(_className, methodName, "Error loading grid data.", ex);
							throw;
						}
						else
						{
							_logger.Error(_className, methodName, ex.Message);
						}
					}
					if (filteredList != null && filteredList.Count > 0)
					{
						APIStruct[] txnPoints = new APIStruct[filteredList.Count];
						int idx1 = 0;
						foreach (PointTransaction txn in filteredList)
						{
							txnPoints[idx1++] = PointTransactionHelper.SerializePointTransaction(txn, "OtherPointsHistory");
						}
						responseArgs.Add("OtherPointsHistory", txnPoints);
					}
				}
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

		private string[] GetAdditionalAttributesList()
		{
			Dictionary<string, string> preDefined = new Dictionary<string, string>()
            {
                {"TxnHeaderId", "TxnHeaderId"},
                {"TxnDate", "TxnDate"},
                {"TxnNumber", "TxnNumber"},
                {"TxnRegisterNumber", "TxnRegisterNumber"},
                {"TxnStoreId", "TxnStoreId"},
                {"TxnAmount", "TxnAmount"},
                {"TxnChannel", "TxnChannel"}
            };
			string addAttParms = GetFunctionParameter("ExtendedHeaderFields");
			string[] addAttList = null;
			if (!string.IsNullOrEmpty(addAttParms))
			{
				List<string> validList = new List<string>();
				string[] tokens = addAttParms.Split(';');
				foreach (string token in tokens)
				{
					if (!preDefined.ContainsKey(token))
					{
						validList.Add(token);
					}
				}
				if (validList.Count > 0)
				{
					addAttList = validList.ToArray<string>();
				}
			}
			return addAttList;
		}
	}
}
