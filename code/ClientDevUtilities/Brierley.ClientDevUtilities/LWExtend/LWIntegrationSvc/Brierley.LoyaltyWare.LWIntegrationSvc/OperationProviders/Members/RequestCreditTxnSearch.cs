using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;

using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class RequestCreditTxnSearch : OperationProviderBase
	{
		private const string _className = "RequestCreditTxnSearch";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public RequestCreditTxnSearch() : base("RequestCreditTxnSearch") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";

			try
			{
				string response = string.Empty;

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

				string txnTypeStr = args.ContainsKey("TxnType") ? (string)args["TxnType"] : string.Empty;
				if (string.IsNullOrEmpty(txnTypeStr))
				{
					throw new LWOperationInvocationException(string.Format("Please provide a valid transaction type.")) { ErrorCode = 3376 };
				}

				TransactionType txnType = (TransactionType)Enum.Parse(typeof(TransactionType), txnTypeStr);

				if (!args.ContainsKey("RequestCreditTxnSearchParameters"))
				{
					throw new LWOperationInvocationException(string.Format("No parameters provided for txn search.")) { ErrorCode = 3377 };
				}

				Dictionary<string, string> searchParms = new Dictionary<string, string>();
				APIStruct[] parmsList = (APIStruct[])args["RequestCreditTxnSearchParameters"];

				string searchFields = txnType == TransactionType.Store ? GetFunctionParameter("StoreTxnSearchFields") : GetFunctionParameter("OnlineTxnSearchFields");
				if (string.IsNullOrEmpty(searchFields))
				{
					throw new LWOperationInvocationException(string.Format("No search fields configured.")) { ErrorCode = 3378 };
				}
				string[] searchTokens = searchFields.Split(',');

				foreach (APIStruct parm in parmsList)
				{
					string fieldName = parm.Parms["ParmName"].ToString();
					bool isAllowed = (from x in searchTokens where x == fieldName select 1).Count() > 0;
					if (!isAllowed)
					{
						throw new LWOperationInvocationException(string.Format("Search parameter {0} is not an allowed search field.", fieldName)) { ErrorCode = 3379 };
					}
					searchParms.Add(parm.Parms["ParmName"].ToString(), parm.Parms["ParmValue"].ToString());
				}

				string autoApplyStr = GetFunctionParameter("AutoApplyCreditIfSingleResult");
				bool autoApply = false;
				if (!string.IsNullOrEmpty(autoApplyStr))
				{
					autoApply = bool.Parse(autoApplyStr);
				}

				Member member = LoadMember(args);
				string loyaltyId = string.Empty;
				string note = string.Empty;
				if (autoApply)
				{
					loyaltyId = args.ContainsKey("CardID") ? (string)args["CardID"] : string.Empty;
					note = args.ContainsKey("Note") ? (string)args["Note"] : string.Empty;
				}

				int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
				int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;

				LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

				string processIdSuppressionList = GetFunctionParameter("ProcessIdExclusionList");

				string helperAssemblyName = GetFunctionParameter("HelperAssemblyName");
				string helperTypeName = GetFunctionParameter("HelperTypeName");
				IRequestCreditInterceptor helper = RequestCreditHelper.CreateRequestCreditInterceptor(helperTypeName, helperAssemblyName);

				IList<IClientDataObject> txnHeaders = helper.SearchTransaction(txnType, searchParms, processIdSuppressionList, batchInfo);

				if (txnHeaders == null || txnHeaders.Count == 0)
				{
					throw new LWOperationInvocationException(string.Format("No transactions found that match the search criteria.")) { ErrorCode = 3362 };
				}

				decimal pointsEarned = 0;
				bool creditApplied = false;
				string txnHeaderId = string.Empty;
				if (autoApply && txnHeaders.Count == 1)
				{
					if (member.GetLoyaltyCard(loyaltyId) == null)
					{
						string errMsg = string.Format(
							"Member with identity {0} does not have a loyalty card with id {1}.",
							member.IpCode, 
							loyaltyId);
						_logger.Error(_className, methodName, errMsg);
						throw new LWOperationInvocationException(errMsg) { ErrorCode = 3380 };
					}
					IClientDataObject txnHeader = txnHeaders[0];
					txnHeaderId = (string)txnHeader.GetAttributeValue("TxnHeaderId");
					object processId = txnHeader.GetAttributeValue("ProcessId");
					if (processId != null && processId is long && (long)processId == (long)ProcessCode.RequestCreditApplied)
					{
						string errMsg = string.Format(
							"Transaction header id {0} has already been credited to Loyalty Id {1}.",
							txnHeaderId, 
							txnHeader.GetAttributeValue("TxnLoyaltyId").ToString());
						_logger.Error(_className, methodName, errMsg);
						throw new LWOperationInvocationException(errMsg) { ErrorCode = 3380 };
					}
					pointsEarned = helper.AddLoyaltyTransaction(member, loyaltyId, txnHeaderId);
					creditApplied = true;

					if (!string.IsNullOrEmpty(note))
					{
						var csnote = new CSNote() { MemberId = member.IpCode, CreateDate = DateTime.Now, Note = note, CreatedBy = 0 };
						using (CSService inst = LWDataServiceUtil.CSServiceInstance())
						{
							inst.CreateNote(csnote);
						}
					}
				}


				APIArguments responseArgs = new APIArguments();
				APIStruct[] headers = new APIStruct[txnHeaders.Count];
				int i = 0;
				foreach (IClientDataObject txnHdr in txnHeaders)
				{
					APIArguments rparms = new APIArguments();
					rparms.Add("TxnHeaderId", txnHdr.GetAttributeValue("TxnHeaderId"));
					rparms.Add("TxnNumber", txnHdr.GetAttributeValue("TxnNumber"));
					rparms.Add("TxnDate", txnHdr.GetAttributeValue("TxnDate"));
					rparms.Add("TxnRegisterNumber", txnHdr.GetAttributeValue("TxnRegisterNumber"));
					rparms.Add("TxnAmount", txnHdr.GetAttributeValue("TxnAmount"));
					rparms.Add("TxnStoreNumber", txnHdr.GetAttributeValue("TxnStoreNumber"));
					APIStruct txnHistory = new APIStruct() { Name = "TxnHistory", IsRequired = true, Parms = rparms };
					headers[i++] = txnHistory;
				}
				responseArgs.Add("TxnHistory", headers);
				responseArgs.Add("PointsEarned", pointsEarned);

				response = SerializationUtils.SerializeResult(Name, Config, responseArgs);

				if (creditApplied)
				{
					Dictionary<string, object> context = new Dictionary<string, object>();
					context.Add("member", member);
					context.Add("headerId", txnHeaderId);
					context.Add("pointsearned", (long)pointsEarned);
					PostProcessSuccessfullInvocation(context);
				}

				return response;
			}
			catch (LWException ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
			catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
	}
}
