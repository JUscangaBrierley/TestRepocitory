using System;
using System.Collections.Generic;

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
	public class ApplyTxnCredit : OperationProviderBase
	{
		private const string _className = "ApplyTxnCredit";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public ApplyTxnCredit() : base("ApplyTxnCredit") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";

			try
			{
				string response = string.Empty;

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

				Member member = LoadMember(args);

				string loyaltyId = args.ContainsKey("CardID") ? (string)args["CardID"] : string.Empty;

				string txnHeaderId = args.ContainsKey("TxnHeaderId") ? (string)args["TxnHeaderId"] : string.Empty;
				if (string.IsNullOrEmpty(txnHeaderId))
				{
					throw new LWOperationInvocationException(string.Format("Please provide a txn header id.")) { ErrorCode = 3373 };
				}

				string note = args.ContainsKey("Note") ? (string)args["Note"] : string.Empty;

				LWCriterion lwCriteria = new LWCriterion("HistoryTxnDetail");
				lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txnHeaderId, LWCriterion.Predicate.Eq);
				IList<IClientDataObject> oHistoryRecords = LoyaltyDataService.GetAttributeSetObjects(null, "HistoryTxnDetail", lwCriteria, null, false);
				IClientDataObject historyTxnDetail = oHistoryRecords[0];
				object processId = historyTxnDetail.GetAttributeValue("ProcessId");
				if (processId != null && processId is long && (long)processId == (long)ProcessCode.RequestCreditApplied)
				{
					string errMsg = string.Format(
						"Transaction header id {0} has already been credited to Loyalty Id {1}.",
						txnHeaderId, 
						historyTxnDetail.GetAttributeValue("TxnLoyaltyId").ToString());
					_logger.Error(_className, methodName, errMsg);
					throw new LWOperationInvocationException(errMsg) { ErrorCode = 3374 };
				}

				string helperAssemblyName = GetFunctionParameter("HelperAssemblyName");
				string helperTypeName = GetFunctionParameter("HelperTypeName");
				IRequestCreditInterceptor helper = RequestCreditHelper.CreateRequestCreditInterceptor(helperTypeName, helperAssemblyName);

				decimal pointsEarned = helper.AddLoyaltyTransaction(member, loyaltyId, txnHeaderId);

				if (!string.IsNullOrEmpty(note))
				{
					var csnote = new CSNote() { MemberId = member.IpCode, CreateDate = DateTime.Now, Note = note, CreatedBy = 0 };
					using (CSService inst = LWDataServiceUtil.CSServiceInstance())
					{
						inst.CreateNote(csnote);
					}
				}

				Dictionary<string, object> context = new Dictionary<string, object>();
				context.Add("member", member);
				context.Add("headerId", txnHeaderId);
				context.Add("pointsearned", (long)pointsEarned);
				PostProcessSuccessfullInvocation(context);

				APIArguments resultParams = new APIArguments();
				resultParams.Add("PointsEarned", pointsEarned);

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
