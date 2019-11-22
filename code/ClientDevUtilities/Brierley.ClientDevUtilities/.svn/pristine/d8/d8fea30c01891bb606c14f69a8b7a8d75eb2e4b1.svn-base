using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.LWGateway.DataAccess
{
    public interface IPointTransactionDao : IDaoBase<PointTransaction>
    {
        void Delete(long pointTxnID);
        int DeleteByRowKey(long rowKey);
        int DeleteByVcKey(long vcKey);
        int DeleteByVcKeys(long[] vcKeys);
        int ExpireAllTransactions(long[] vcKeys, DateTime? cutOffDate, DateTime newExpDate, PointExpirationReason reason, string notes);
        int ExpireTransactions(long[] txnIds, DateTime newExpDate, PointExpirationReason? reason, string notes);
        long HowMany(long[] vcKeys, long[] pointTypeIds, long[] pointEventids, DateTime? startDate, DateTime? endDate, bool includeExpired);
        PointTransaction Retrieve(long txnId);
        List<PointTransaction> RetrieveAll();
        decimal RetrieveExpiredPointBalance(long[] vcKeys, long[] pointTypeIds, long[] pointEventIds, PointBankTransactionType[] transactionTypes, DateTime? from, DateTime? to, DateTime? awardDateFrom, DateTime? awardDateTo, string changedBy, string locationId, PointTransactionOwnerType? ownerType, long? ownerId, long[] rowkeys);
        List<PointTransaction> RetrieveExpiredPointTransactionsWithNoExpiredRecords(long[] vcKeys, LWQueryBatchInfo batchInfo);
        List<long> RetrieveMembersBasedOnPointBalance(PointBalanceType type, long balance, LWCriterion.Predicate predicate, long[] pointTypeIds, long[] pointEventIds, DateTime? startDate, DateTime? endDate, LWQueryBatchInfo batchInfo);
        List<long> RetrieveMembersWithExpiredPoints(LWQueryBatchInfo batchInfo);
        List<PointTransaction> RetrieveOnHoldPointTransactions(long[] vcKeys, long[] pointTypeIds, long[] pointEventIds, DateTime? from, DateTime? to, long[] parentIds, PointTransactionOwnerType? ownerType, long? ownerId, long? rowkey);
        decimal RetrievePointBalance(long[] vcKeys, long[] pointTypeIds, long[] pointEventIds, PointBankTransactionType[] transactionTypes, DateTime? from, DateTime? to, DateTime? awardDateFrom, DateTime? awardDateTo, string changedBy, string locationId, PointTransactionOwnerType? ownerType, long? ownerId, long[] rowkeys, bool includeExpiredPoints);
        decimal RetrievePointBalanceByDateExcludingPointTypes(long vcKey, DateTime? from, DateTime? to, long[] excludePointTypes);
        decimal RetrievePointsExpired(long[] vcKeys, long[] pointTypeIds, long[] pointEventIds, DateTime from, DateTime to);
        decimal RetrievePointsOnHold(long[] vcKeys, long[] pointTypeIds, long[] pointEventIds, DateTime? from, DateTime? to, long[] parentIds, PointTransactionOwnerType? ownerType, long? ownerId, long? rowkey);
        List<PointTransaction> RetrievePointTransactions(long[] vcKeys, long[] pointTypeIds, long[] pointEventIds, DateTime? from, DateTime? to, string promoCode, long[] txnIds, PointBankTransactionType[] txnTypes, PointTransactionOwnerType? ownerType, long? ownerId, long[] rowkeys, bool orderDateDescending, bool includeExpired, LWQueryBatchInfo batchInfo);
        decimal RetrieveTotalPointsAwarded(long[] pointTypeIds, long[] pointEventIds, PointBankTransactionType[] transactionTypes, DateTime? from, DateTime? to, bool includeExpiredPoints);
        List<PointTransaction> RetrieveTransactionsNotConsumed(long[] vcKeys, long[] pointTypes, long[] pointEvents, PointBankTransactionType txnType);
        List<PointTransaction> RetrieveUnexpiredTransactions(long[] vcKeys, long[] pointTypeIds, long[] pointEventIds, PointBankTransactionType[] txnTypes, DateTime? from, DateTime? to, DateTime? expDateFrom, DateTime? expToDate);
    }
}
