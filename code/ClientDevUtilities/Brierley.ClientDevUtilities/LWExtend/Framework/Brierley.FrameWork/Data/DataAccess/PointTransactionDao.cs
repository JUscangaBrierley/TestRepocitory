using System;
using System.Collections.Generic;
using System.Diagnostics;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;
using PetaPoco.Internal;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PointTransactionDao : DaoBase<PointTransaction>
	{
		private const string _className = "PointTransactionDAO";

		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public PointTransactionDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public void Delete(long pointTxnID)
		{
			DeleteEntity(pointTxnID);
		}

		public int DeleteByRowKey(long rowKey)
		{
			return Database.Execute("delete from LW_PointTransaction where RowKey = @0", rowKey);
		}

		public int DeleteByVcKey(long vcKey)
		{
			return Database.Execute("delete from LW_PointTransaction where VcKey = @0", vcKey);
		}

		public int DeleteByVcKeys(long[] vcKeys)
		{
			int keysRemaining = vcKeys.Length;
			int startIdx = 0;
			int nRows = 0;
			while (keysRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(vcKeys, ref startIdx, ref keysRemaining);
				nRows += Database.Execute("delete from LW_PointTransaction where VcKey in (@vckeys)", new { vckeys = ids });
			}
			return nRows;
		}

		/// <summary>
		/// This could be an expensive method because it could potentially perform a full table scan.
		/// </summary>        
		/// <param name="pointTypeIds"></param>
		/// <param name="pointEventIds"></param>
		/// <param name="transactionTypes"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>        
		/// <param name="includeExpiredPoints"></param>
		/// <returns></returns>
		public decimal RetrieveTotalPointsAwarded(
			long[] pointTypeIds,
			long[] pointEventIds,
			PointBankTransactionType[] transactionTypes,
			DateTime? from,
			DateTime? to,
			bool includeExpiredPoints)
		{
			var parameters = new List<object>() { new { transactionTypes = transactionTypes } };
			string sql = "select sum(points) from LW_PointTransaction where transactiontype in (@transactionTypes)";
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventIds, from, to);
			if (!includeExpiredPoints)
			{
				sql += " and ExpirationDate >= @" + parameters.Count.ToString();
				parameters.Add(DateTime.Now);
			}
			return ExecuteScalar<decimal>(sql, parameters.ToArray());
		}

		public decimal RetrievePointBalance(
			long[] vcKeys,
			long[] pointTypeIds,
			long[] pointEventIds,
			PointBankTransactionType[] transactionTypes,
			DateTime? from,
			DateTime? to,
			DateTime? awardDateFrom,
			DateTime? awardDateTo,
			string changedBy,
			string locationId,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long[] rowkeys,
			bool includeExpiredPoints)
		{
			EnsureVcKeys(vcKeys);
			var parameters = new List<object>() { new { transactionTypes = transactionTypes }, new { vckeys = vcKeys } };
			string sql = "select sum(points) from LW_PointTransaction where transactiontype in (@transactionTypes) and VCKey in (@vckeys)";
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventIds, from, to, awardDateFrom, awardDateTo, changedBy, locationId, ownerType, ownerId, rowkeys);
			if (!includeExpiredPoints)
			{
				sql += " and ExpirationDate >= @" + parameters.Count.ToString();
				parameters.Add(DateTime.Now);
			}

			return ExecuteScalar<decimal>(sql, parameters.ToArray());
		}

		public decimal RetrieveExpiredPointBalance(
			long[] vcKeys,
			long[] pointTypeIds,
			long[] pointEventIds,
			PointBankTransactionType[] transactionTypes,
			DateTime? from,
			DateTime? to,
			DateTime? awardDateFrom,
			DateTime? awardDateTo,
			string changedBy,
			string locationId,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long[] rowkeys)
		{
			EnsureVcKeys(vcKeys);
			var parameters = new List<object>() { new { transactionTypes = transactionTypes }, new { vckeys = vcKeys } };
			string sql = "select sum(points) from LW_PointTransaction where transactiontype in (@transactionTypes) and VCKey in (@vckeys)";
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventIds, from, to, awardDateFrom, awardDateTo, changedBy, locationId, ownerType, ownerId, rowkeys);
			sql += " and ExpirationDate <= @" + parameters.Count.ToString();
			parameters.Add(DateTime.Now);
			return ExecuteScalar<decimal>(sql, parameters.ToArray());
		}

		public decimal RetrievePointBalanceByDateExcludingPointTypes(long vcKey, DateTime? from, DateTime? to, long[] excludePointTypes)
		{
			var parameters = new List<object>() 
			{
				vcKey, 
				DateTime.Now, 
				new { transactionTypes = new int[] { (int)PointBankTransactionType.Consumed, (int)PointBankTransactionType.Credit, (int)PointBankTransactionType.Debit } }
			};
			string sql = "select sum(points) from LW_PointTransaction where vckey = @0 and ExpirationDate > @1 and TransactionType in (@transactionTypes)";
            sql += " and not " + AddBatchedInList("PointTypeId", excludePointTypes, parameters);
			AddCommonParameters(ref sql, ref parameters, null, null, from, to);
			return ExecuteScalar<decimal>(sql, parameters.ToArray());
		}

		public decimal RetrievePointsOnHold(
			long[] vcKeys,
			long[] pointTypeIds,
			long[] pointEventIds,
			DateTime? from,
			DateTime? to,
			long[] parentIds,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long? rowkey)
		{
			EnsureVcKeys(vcKeys);
			var parameters = new List<object>() { PointBankTransactionType.Hold, DateTime.Now, new { vckeys = vcKeys } };
			string sql = "select sum(points) from LW_PointTransaction where transactiontype = @0 and ExpirationDate > @1 and VCKey in (@vckeys)";
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventIds, from, to, null, null, null, null, ownerType, ownerId);
			if (parentIds != null && parentIds.Length > 0)
			{
				sql += " and ParentTransactionId in (@parentIds)";
				parameters.Add(new { parentIds = parentIds });
			}
			if (rowkey != null)
			{
				sql += " and RowKey = @" + parameters.Count.ToString();
				parameters.Add(rowkey);
			}
			return ExecuteScalar<decimal>(sql, parameters.ToArray());
		}


		public List<PointTransaction> RetrieveOnHoldPointTransactions(
			long[] vcKeys,
			long[] pointTypeIds,
			long[] pointEventIds,
			DateTime? from,
			DateTime? to,
			long[] parentIds,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long? rowkey)
		{
			EnsureVcKeys(vcKeys);
			var parameters = new List<object>() { PointBankTransactionType.Hold, DateTime.Now, new { vckeys = vcKeys } };
			string sql = "select * from LW_PointTransaction where transactiontype = @0 and ExpirationDate > @1 and VCKey in (@vckeys)";
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventIds, from, to, null, null, null, null, ownerType, ownerId);
			if (parentIds != null && parentIds.Length > 0)
			{
				sql += " and ParentTransactionId in (@parentIds)";
				parameters.Add(new { parentIds = parentIds });
			}
			if (rowkey != null)
			{
				sql += " and RowKey = @" + parameters.Count.ToString();
				parameters.Add(rowkey);
			}
			return Database.Fetch<PointTransaction>(sql, parameters.ToArray());
		}

		/// <summary>
		/// This method gets the sume of points that have expired based on the provided date.  This method
		/// correctly accounts for the points that had been consumed using the FIFO method.  However,
		/// if any points were consumed using LumpSum method, they are not accounted for. 
		/// </summary>
		/// <param name="vcKeys"></param>
		/// <param name="pointTypeIds"></param>
		/// <param name="pointEventIds"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public decimal RetrievePointsExpired(long[] vcKeys, long[] pointTypeIds, long[] pointEventIds, DateTime from, DateTime to)
		{
			EnsureVcKeys(vcKeys);
			List<object> parameters = new List<object>() { new { vckeys = vcKeys }, PointBankTransactionType.Credit, DateTimeUtil.GetBeginningOfDay(from), DateTimeUtil.GetEndOfDay(to) };
			string sql = "select sum(Points) - sum(PointsConsumed) from LW_PointTransaction where vckey in (@vckeys) and TransactionType = @1 and ExpirationDate >= @2 and ExpirationDate <= @3";
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventIds);
			return ExecuteScalar<decimal>(sql, parameters.ToArray());
		}

		public List<PointTransaction> RetrieveUnexpiredTransactions(
			long[] vcKeys,
			long[] pointTypeIds,
			long[] pointEventIds,
			PointBankTransactionType[] txnTypes,
			DateTime? from,
			DateTime? to,
			DateTime? expDateFrom,
			DateTime? expToDate)
		{
			EnsureVcKeys(vcKeys);
			List<object> parameters = new List<object>() { new { vckeys = vcKeys } };
			string sql = "select * from LW_PointTransaction where vckey in (@vckeys)";
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventIds, from, to);
			if (txnTypes != null && txnTypes.Length > 0)
			{
				sql += " and TransactionType in (@transactionTypes)";
				parameters.Add(new { transactionTypes = txnTypes });
			}
			if (expDateFrom != null && expToDate != null)
			{
				sql += " and ExpirationDate >= @" + parameters.Count.ToString();
				parameters.Add(DateTimeUtil.GetBeginningOfDay((DateTime)expDateFrom));

				sql += " and ExpirationDate <= @" + parameters.Count.ToString();
				parameters.Add(DateTimeUtil.GetEndOfDay((DateTime)expToDate));
			}
			else
			{
				sql += " and ExpirationDate > @" + parameters.Count.ToString();
				parameters.Add(DateTime.Now);
			}
			return Database.Fetch<PointTransaction>(sql, parameters.ToArray());
		}

        public int ExpireAllTransactions(
            long[] vcKeys,
            DateTime? cutOffDate,
            DateTime newExpDate,
            PointExpirationReason reason,
            string notes)
        {
            EnsureVcKeys(vcKeys);
            List<object> parameters = new List<object>() { newExpDate, reason, notes };
            string sql = "update LW_PointTransaction set ExpirationDate = @0, ExpirationReason = @1";
            if (!string.IsNullOrEmpty(notes))
            {
                sql += ", notes = @2";
            }
            sql += " where VCKey in (@vckeys) and ExpirationDate > @0";
            if (cutOffDate != null)
            {
                parameters.Add(cutOffDate);
                sql += " and TransactionDate > @3";
            }
            parameters.Add(new { vckeys = vcKeys });
            return Database.Execute(sql, parameters.ToArray());
        }

		public int ExpireTransactions(
			long[] txnIds,
			DateTime newExpDate,
			PointExpirationReason? reason,
			string notes)
		{
			List<object> parameters = new List<object>() { newExpDate, reason, notes, new { ids = 0 } };
			string sql = "update LW_PointTransaction set ExpirationDate = @0";
			if (reason != null)
			{
				sql += ", ExpirationReason = @1";
				parameters.Add(reason);
			}
			if (!string.IsNullOrEmpty(notes))
			{
				sql += ", Notes = @2";
				parameters.Add(notes);
			}

			sql += " where pointtransactionid in (@ids)";

			int rowCount = 0;
			int idsRemaining = txnIds.Length;
			int startIdx = 0;
			while (idsRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(txnIds, ref startIdx, ref idsRemaining);
				parameters[3] = new { ids = ids };
				rowCount += Database.Execute(sql, parameters.ToArray());
			}
			return rowCount;
		}

		public PointTransaction Retrieve(long txnId)
		{
			return GetEntity(txnId);
		}


		public List<PointTransaction> RetrievePointTransactions(
			long[] vcKeys, // virtual cards ids
			long[] pointTypeIds,
			long[] pointEventIds,
			DateTime? from,
			DateTime? to,
			string promoCode,
			long[] txnIds, // parent transaction ids
			PointBankTransactionType[] txnTypes,
			PointTransactionOwnerType? ownerType,
			long? ownerId,
			long[] rowkeys,
			bool orderDateDescending,
			bool includeExpired,
			LWQueryBatchInfo batchInfo)
		{
			EnsureVcKeys(vcKeys);
			var parameters = new List<object>() { new { vckeys = vcKeys } };
			string sql = "select p.* from LW_PointTransaction p where VCKey in (@vckeys)";
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventIds, from, to, null, null, null, null, ownerType, ownerId);

			if (txnIds != null && txnIds.Length > 0)
			{
                sql += " and " + AddBatchedInList("ParentTransactionId", txnIds, parameters);
			}

			if (!string.IsNullOrEmpty(promoCode))
			{
				sql += " and PromoCode = @" + parameters.Count.ToString();
				parameters.Add(promoCode);
			}
			if (txnTypes != null && txnTypes.Length > 0)
			{
				sql += " and TransactionType in (@transactionTypes)";
				parameters.Add(new { transactionTypes = txnTypes });
			}

			if (!includeExpired)
			{
				sql += " and ExpirationDate >= @" + parameters.Count.ToString();
                //parameters.Add(DateTimeUtil.GetEndOfDay(DateTime.Now));
                parameters.Add(DateTime.Now);
            }

			if (rowkeys != null && rowkeys.Length > 0)
			{
                sql += " and " + AddBatchedInList("RowKey", rowkeys, parameters);
			}

			sql += " order by TransactionDate";
			if (orderDateDescending)
			{
				sql += " desc";
			}

			var args = parameters.ToArray();

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}

            return Database.Fetch<PointTransaction>(sql, args);
		}

		/// <summary>
		/// This method will return expired points (Credit & debit) point transactions
		/// that do not have corresponding "Expired" point transaction records.
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="batchInfo"></param>
		/// <returns></returns>
		public List<PointTransaction> RetrieveExpiredPointTransactionsWithNoExpiredRecords(long[] vcKeys, LWQueryBatchInfo batchInfo)
		{
			string methodName = "RetrieveMembersWithExpiredPoints";

			string op = string.Empty;
			string sql = @"
				select p.* 
				from LW_PointTransaction p
				where
					VcKey in (@vckeys) and
					(
						p.TransactionType = 2 or
						(p.TransactionType = 1 and p.Points != PointsConsumed)
					) and
					p.ExpirationDate <= @0 and
					not p.Id in (select ParentTransactionId from LW_PointTransaction where TransactionType = 6 and ParentTransactionId != null and VcKey in (@vckeys))";

			object[] parameters = new object[] { DateTime.Now, vcKeys };

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref parameters);
			}

			Stopwatch timer = new Stopwatch();
			timer.Start();
			var ret = Database.Fetch<PointTransaction>(sql, parameters);
			timer.Stop();
			long count = ret != null ? ret.Count : 0;
			_logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", count, timer.ElapsedMilliseconds));
			return ret;
		}

		public List<PointTransaction> RetrieveAll()
		{
			return Database.Fetch<PointTransaction>(string.Empty);
		}

		public List<long> RetrieveMembersBasedOnPointBalance(
			PointBalanceType type,
			long balance,
			LWCriterion.Predicate predicate,
			long[] pointTypeIds,
			long[] pointEventIds,
			DateTime? startDate,
			DateTime? endDate,
			LWQueryBatchInfo batchInfo)
		{
			const string methodName = "RetrieveMembersBasedOnPointBalance";
			var parameters = new List<object>();

			string op = string.Empty;
			switch (predicate)
			{
				case LWCriterion.Predicate.Eq:
					op = "=";
					break;
				case LWCriterion.Predicate.Ge:
					op = ">=";
					break;
				case LWCriterion.Predicate.Gt:
					op = ">";
					break;
				case LWCriterion.Predicate.Le:
					op = "<=";
					break;
				case LWCriterion.Predicate.Lt:
					op = "<";
					break;
				case LWCriterion.Predicate.Ne:
					op = "<>";
					break;
			}

			int[] transactionTypes = null;
			switch (type)
			{
				case PointBalanceType.PointBalance:
					transactionTypes = new int[] { (int)PointBankTransactionType.Consumed, (int)PointBankTransactionType.Credit, (int)PointBankTransactionType.Debit };
					break;
				case PointBalanceType.EarnedBalance:
					transactionTypes = new int[] { (int)PointBankTransactionType.Credit, (int)PointBankTransactionType.Debit };
					break;
				case PointBalanceType.PointsOnHold:
					transactionTypes = new int[] { (int)PointBankTransactionType.Hold };
					break;
			}
			parameters.Add(new { transactionTypes = transactionTypes });
			parameters.Add(DateTime.Now);
			parameters.Add(balance);

			string sql = "select m.IpCode from LW_LoyaltyMember m, LW_VirtualCard v, LW_PointTransaction p where p.VcKey = v.VcKey and m.IpCode = v.IpCode and TransactionType in (@transactionTypes) and p.ExpirationDate > @1";
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventIds, startDate, endDate);
			sql += "group by m.IpCode having sum(p.Points) " + op + " @2 order by m.IpCode asc";

			object[] args = parameters.ToArray();

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}

			Stopwatch timer = new Stopwatch();
			timer.Start();
			List<long> list = Database.Fetch<long>(sql, args);
			timer.Stop();
			long count = list != null ? list.Count : 0;
			_logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", count, timer.ElapsedMilliseconds));

			return list;
		}

		/// <summary>
		/// This method will return IpCode of all members that have expired points (Credit & debit)
		/// but do not have corresponding "Expired" point transaction records.
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="batchInfo"></param>
		/// <returns></returns>
		public List<long> RetrieveMembersWithExpiredPoints(LWQueryBatchInfo batchInfo)
		{
			const string methodName = "RetrieveMembersWithExpiredPoints";

			string op = string.Empty;
			string sql = @"select distinct m.IpCode from LW_LoyaltyMember m, LW_VirtualCard v, LW_PointTransaction p where
                                (
									p.TransactionType = 2 or
									(p.TransactionType = 1 and p.Points != PointsConsumed)
								) and
                                p.ExpirationDate <= @0 and
                                p.VcKey = v.VcKey and 
								m.IpCode = v.IpCode and
                                not p.Id in (select ParentTransactionId from LW_PointTransaction where TransactionType = 6 and ParentTransactionId != null)";

			var args = new object[] { DateTime.Now };

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}

			Stopwatch timer = new Stopwatch();
			timer.Start();
			List<long> list = Database.Fetch<long>(sql, args);
			timer.Stop();
			long count = list != null ? list.Count : 0;
			_logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", count, timer.ElapsedMilliseconds));

			return list;
		}

		public List<PointTransaction> RetrieveTransactionsNotConsumed(long[] vcKeys, long[] pointTypes, long[] pointEvents, PointBankTransactionType txnType)
		{
			var parameters = new List<object>() { new { vckeys = vcKeys }, txnType, DateTime.Now };
			string sql = "select * from LW_PointTransaction where VcKey in (@vckeys) and TransactionType = @1 and Points != PointsConsumed and ExpirationDate > @2 ";
			AddCommonParameters(ref sql, ref parameters, pointTypes, pointEvents);
			sql += " order by PointTransactionId asc";

			return Database.Fetch<PointTransaction>(sql, parameters.ToArray());
		}

		public long HowMany(
			long[] vcKeys,
			long[] pointTypeIds,
			long[] pointEventids,
			DateTime? startDate,
			DateTime? endDate,
			bool includeExpired)
		{
			var parameters = new List<object>() { new { vckeys = vcKeys } };
			string sql = "select count(*) from LW_PointTransaction where VcKey in(@vckeys)";
			if (!includeExpired)
			{
				sql += " and ExpirationDate > @1";
				parameters.Add(DateTime.Now);
			}
			AddCommonParameters(ref sql, ref parameters, pointTypeIds, pointEventids, startDate, endDate);
			return Database.ExecuteScalar<long>(sql, parameters.ToArray());
		}

		private void EnsureVcKeys(long[] vcKeys)
		{
			if (vcKeys == null || vcKeys.Length == 0)
			{
				string errMsg = "No virtual cards provided for point related operations.";
				_logger.Error(_className, "EnsureVcKeys", errMsg);
				throw new LWDataServiceException(errMsg) { ErrorCode = 3230 };
			}
		}

		private void AddCommonParameters(
			ref string sql,
			ref List<object> parameters,
			long[] pointTypeIds = null,
			long[] pointEventIds = null,
			DateTime? from = null,
			DateTime? to = null,
			DateTime? awardDateFrom = null,
			DateTime? awardDateTo = null,
			string changedBy = null,
			string locationId = null,
			PointTransactionOwnerType? ownerType = null,
			long? ownerId = null,
			long[] rowkeys = null)
		{
			if (from != null)
			{
				sql += " and TransactionDate >= @" + parameters.Count.ToString();
				parameters.Add(from);
			}

			if (to != null)
			{
				sql += " and TransactionDate <= @" + parameters.Count.ToString();
				parameters.Add(to);
			}

			if (awardDateFrom != null)
			{
				sql += " and PointAwardDate >= @" + parameters.Count.ToString();
				parameters.Add(awardDateFrom);
			}

			if (awardDateTo != null)
			{
				sql += " and PointAwardDate <= @" + parameters.Count.ToString();
				parameters.Add(awardDateTo);
			}

			if (pointTypeIds != null && pointTypeIds.Length > 0)
			{
                sql += " and " + AddBatchedInList("PointTypeId", pointTypeIds, parameters);
			}

			if (pointEventIds != null && pointEventIds.Length > 0)
			{
                sql += " and " + AddBatchedInList("PointEventId", pointEventIds, parameters);
			}

			if (!string.IsNullOrEmpty(changedBy))
			{
				sql += " and PTChangedBy = @" + parameters.Count.ToString();
				parameters.Add(changedBy);
			}
			if (!string.IsNullOrEmpty(locationId))
			{
				sql += " and PTLocationId = @" + parameters.Count.ToString();
				parameters.Add(locationId);
			}
			if (ownerType != null)
			{
				sql += " and OwnerType = @" + parameters.Count.ToString();
				parameters.Add(ownerType);

				if (ownerId != null)
				{
					sql += " and OwnerId = @" + parameters.Count.ToString();
					parameters.Add(ownerId);
				}
			}
			if (rowkeys != null && rowkeys.Length > 0)
			{
                sql += " and " + AddBatchedInList("RowKey", rowkeys, parameters);
			}
		}
	}
}
