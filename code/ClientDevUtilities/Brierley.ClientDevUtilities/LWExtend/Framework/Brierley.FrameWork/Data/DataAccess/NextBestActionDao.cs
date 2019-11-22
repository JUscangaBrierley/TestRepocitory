using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class NextBestActionDao : DaoBase<NextBestAction>
	{
		public NextBestActionDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

        public override void Update(NextBestAction t)
        {
            t.UpdateDate = DateTime.Now;
            Database.Execute("update LW_NextBestAction set ActionId = @2, ActionType = @3, CreateDate = @4, UpdateDate = @5 where MemberId = @0 and Priority = @1"
                , t.MemberId, t.Priority, t.ActionId, (int)t.ActionType, t.CreateDate, t.UpdateDate);
        }

        public override void Update(IEnumerable<NextBestAction> ts)
        {
            ITransaction txn = Database.GetTransaction();
            try
            {
                int i = 0;
                int bulkLoadingBatchSize = Config.BulkLoadingBatchSize.GetValueOrDefault(100);
                foreach (NextBestAction t in ts)
                {
                    Update(t);
                    if (i != 0 && bulkLoadingBatchSize > 0 && i % bulkLoadingBatchSize == 0)
                    {
                        txn.Complete();
                        txn.Dispose();
                        txn = Database.GetTransaction();
                    }
                    i++;
                }
                if (txn != null)
                {
                    txn.Complete();
                    txn.Dispose();
                    txn = null;
                }
            }
            catch
            {
                if (txn != null)
                {
                    txn.Dispose();
                }
                throw;
            }
        }

		public void Delete(long memberId, int priority)
		{
			Database.Execute("delete from LW_NextBestAction where MemberId = @0 and Priority = @1", memberId, priority);
		}

		public IEnumerable<NextBestAction> Retrieve(long memberId, LWQueryBatchInfo batchInfo = null)
		{
			var args = new object[] {memberId};
			string sql = "select n.* from LW_NextBestAction n where MemberId = @0 order by Priority";
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<NextBestAction>(sql, args);
		}

		public NextBestAction Retrieve(long memberId, int priority)
		{
			return Database.FirstOrDefault<NextBestAction>("select * from LW_NextBestAction where MemberId = @0 and Priority = @1", memberId, priority);
		}
	}
}
