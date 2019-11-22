using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberMessageDao : DaoBase<MemberMessage>
	{
		public MemberMessageDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public long HowManyActiveMessages(long memberId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberMessage m where MemberId = @0 m.ExpiryDate > @1", memberId, DateTime.Now);
		}

		public long HowManyActiveUnreadMessages(long memberId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberMessage m where MemberId = @0 and m.ExpiryDate > @1 and m.Status = 0", memberId, DateTime.Now);
		}

		public MemberMessage RetrieveByMemberId(long id)
		{
			return GetEntity(id);
		}

		public List<MemberMessage> Retrieve(LWQueryBatchInfo batchInfo)
		{
			if (batchInfo == null)
			{
				throw new ArgumentNullException("batchInfo");
			}
			var args = new object[0];
			string sql = "select m.* from LW_MemberMessage m";
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<MemberMessage>(sql, args);
		}

		public List<MemberMessage> Retrieve(long ipCode, LWQueryBatchInfo batchInfo)
		{
			var args = new object[] { ipCode, DateTime.Now };
			string sql = "select m.* from LW_MemberMessage m where MemberId = @0 and (ExpiryDate is null or ExpiryDate > @1) order by Status, DisplayOrder, DateIssued";
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<MemberMessage>(sql, args);
		}

		public List<MemberMessage> Retrieve(long memberId, long defId, bool unexpiredOnly)
		{
			string sql = "select * from LW_MemberMessage where MemberId = @0 and MessageDefId = @1";
			if (unexpiredOnly)
			{
				sql += " and (ExpiryDate is null or ExpiryDate > @2)";
			}
			return Database.Fetch<MemberMessage>(sql, memberId, defId, DateTime.Now);
		}

		public Page<MemberMessage> Retrieve(long memberId, List<MemberMessageStatus> status, bool activeOnly, long pageNumber, long resultsPerPage, DateTime? startDate = null, DateTime? endDate = null, MemberMessageOrder order = MemberMessageOrder.Newest)
		{
			if (pageNumber == 0)
			{
				throw new ArgumentOutOfRangeException("pageNumber is 1 based, not an index. 0 is an invalid page number.");
			}

			if (status == null || status.Count == 0)
			{
				status = new List<MemberMessageStatus>() { MemberMessageStatus.Unread };
			}

			var args = new object[] { memberId, new { statuses = status.ToArray() }, DateTime.Now, startDate, endDate };
			string sql = "select m.* from LW_MemberMessage m where MemberId = @0 and Status in (@statuses)";
			if (activeOnly)
			{
				sql += " and StartDate <= @2 and (ExpiryDate is null or ExpiryDate > @2)";
			}
			if (startDate.HasValue)
			{
				sql += " and DateIssued >= @3";
			}
			if (endDate.HasValue)
			{
				sql += " and DateIssued <= @4";
			}
			sql += string.Format(" order by DateIssued {0}", order == MemberMessageOrder.Oldest ? "asc" : "desc");
			return Database.Page<MemberMessage>(pageNumber, resultsPerPage, sql, args);
		}

		/*
		 * CDIS - and many other systems - use LWQueryBatchInfo to retrieve batches of data. Messages are "paged", which seems to work better than batching in cases where a user
		 * is shown pages of data. Calling PetaPoco's Page<T> method returns a PetaPoco.Page that includes the total number of rows and pages. Without it, you either have to count the 
		 * results in a separate query or just take wild guess at how many items there are.
		 * 
		 * This method uses LWQueryBatchInfo in order to keep CDIS backward compatible. Any caller that isn't CDIS should not use this method.
		 */
		[Obsolete("This method uses LWQueryBatchInfo in order to maintain backward compatibility with CDIS. It may be removed in a future LoyaltyWare version. Use the method that returns Page<MemberMessage> instead.")]
		public List<MemberMessage> Retrieve(long memberId, List<MemberMessageStatus> status, bool activeOnly, LWQueryBatchInfo batchInfo, DateTime? startDate = null, DateTime? endDate = null, MemberMessageOrder order = MemberMessageOrder.Newest)
		{
			if (status == null || status.Count == 0)
			{
				status = new List<MemberMessageStatus>() { MemberMessageStatus.Unread };
			}

			var args = new object[] { memberId, new { statuses = status.ToArray() }, DateTime.Now, startDate, endDate };
			string sql = "select m.* from LW_MemberMessage m where MemberId = @0 and Status in (@statuses)";
			if (activeOnly)
			{
				sql += " and StartDate <= @2 and (ExpiryDate is null or ExpiryDate > @2)";
			}
			if (startDate.HasValue)
			{
				sql += " and DateIssued >= @3";
			}
			if (endDate.HasValue)
			{
				sql += " and DateIssued <= @4";
			}
			sql += string.Format(" order by DateIssued {0}", order == MemberMessageOrder.Oldest ? "asc" : "desc");

			ApplyBatchInfo(batchInfo, ref sql, ref args);

			return Database.Fetch<MemberMessage>(sql, args);			
		}

		public List<MemberMessage> Retrieve(long[] ids)
		{
			if (ids == null || ids.Length == 0)
			{
				return new List<MemberMessage>();
			}
			return RetrieveByArray<long>("select * from LW_MemberMessage where Id in (@0) order by Status, DisplayOrder, DateIssued", ids);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MemberMessage where MemberId = @0", memberId);
		}

		public int DeleteByMembers(long[] memberIds)
		{
			int keysRemaining = memberIds.Length;
			int startIdx = 0;
			int nRows = 0;
			while (keysRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(memberIds, ref startIdx, ref keysRemaining);
				nRows += Database.Execute("delete from LW_MemberMessage where MemberId in (@ids)", new { ids = ids });
			}
			return nRows;
		}
	}
}
