using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Interfaces
{
    public interface IRequestCreditInterceptor
    {
        /// <summary>
        /// Provides an opportunity for the client provider implementation to append additional criteria before searching. 
        /// </summary>
        /// <remarks>
        /// Default implementation is to exclude ProcessId 1 and 7. This method may be overridden to eliminate the default 
        /// filtering, or to add additional filtering, or both.
        /// </remarks>
        /// <example>
        /// //exclude ProcessId 1 and 7:
        /// criteria.Add(LWCriterion.OperatorType.AND, "ProcessId", 1, LWCriterion.Predicate.Ne);
        /// criteria.Add(LWCriterion.OperatorType.AND, "ProcessId", 7, LWCriterion.Predicate.Ne);
        /// </example>
        /// <param name="criteria"></param>
        void AddClientSpecificSearchCriteria(LWCriterion criteria, string processIdSuppressionList);

        /// <summary>
        /// Provides an opportunity for the client provider to filter out unwanted or invalid transaction headers before they are displayed.
        /// </summary>
        /// <remarks>
        /// Victoria's Secret requires that valid transaction headers have a TxnHeaderID that ends with "VSS" for in store and 
        /// "VSD" for online. That filtering - or any other client specific filtering - may be implemented in a custom class by 
        /// overriding this method.
        /// </remarks>
        /// <example>
        ///	IList<IClientDataObject> outputList = new List<IClientDataObject>();
        ///	foreach (IClientDataObject obj in transactionList)
        ///	{
        ///		string txnHeader = (string)obj.GetAttributeValue("TxnHeaderID");
        ///		if (transactionType == TransactionType.Store && txnHeader.EndsWith("VSS"))
        ///		{
        ///			outputList.Add(obj);
        ///		}
        ///		else if (transactionType == TransactionType.Online && txnHeader.EndsWith("VSD"))
        ///		{
        ///			outputList.Add(obj);
        ///		}
        ///	}
        ///	return outputList;
        /// </example>
        /// <param name="transactionList"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        IList<IClientDataObject> ApplyFilterOnTransactionHeader(IList<IClientDataObject> transactionList, TransactionType transactionType);

        /// <summary>
        /// This operation adds the the transaction identified by txn header from the history transaction detail table and
        /// properly adds the TxnHeader and TxnDetailItem attribute sets.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="cardId"></param>
        /// <param name="txnHeaderId"></param>
        /// <returns></returns>
        decimal AddLoyaltyTransaction(Member member, string cardId, string txnHeaderId);

        /// <summary>
        /// The operation searches for transactions based on the provided criteria.
        /// </summary>
        /// <param name="transactionType"></param>
        /// <param name="searchParms"></param>
        /// <param name="processIdSuppressionList"></param>
        /// <param name="batchInfo"></param>
        /// <returns></returns>
        IList<IClientDataObject> SearchTransaction(
            TransactionType transactionType,
            Dictionary<String, String> searchParms,
            string processIdSuppressionList,
            LWQueryBatchInfo batchInfo);
    }
}
