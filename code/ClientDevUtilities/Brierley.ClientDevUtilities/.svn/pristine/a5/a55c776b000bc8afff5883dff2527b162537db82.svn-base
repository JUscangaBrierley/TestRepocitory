using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MobileDeviceDao : DaoBase<MobileDevice>
	{
		private const string _className = "MobileDeviceDao";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public MobileDeviceDao(Database database, ServiceConfig config)
			: base(database, config){}

		public long HowMany(long ipCode)
		{
			var args = new object[] { ipCode };
			string sql = "select count(*) from LW_MobileDevice m where m.MemberId = @0";
			return Database.ExecuteScalar<long>(sql, args);
		}

        public MobileDevice Retrieve(long Id)
        {
            return GetEntity(Id);
        }

        public MobileDevice RetrieveByDeviceId(string id)
        {
            if (id == null)
                return new MobileDevice();
            return Database.SingleOrDefault<MobileDevice>("select * from LW_MobileDevice where deviceid = @0", id);
        }

        public List<MobileDevice> Retrieve(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<MobileDevice>();
            return RetrieveByArray<long>("select * from LW_MobileDevice where id in (@0)", ids);
		}

		public List<MobileDevice> RetrieveByMember(long ipCode, LWQueryBatchInfo batchInfo)
		{
			var args = new object[]{ ipCode };
			string sql = "select r.* from LW_MobileDevice r where MemberId = @0 order by r.Id desc";
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<MobileDevice>(sql, args);
		}

        public MobileDevice RetrieveActiveByMember(long ipCode)
        {
            var args = new object[] { ipCode };
            string sql = "select r.* from LW_MobileDevice r where r.MemberId = @0 and r.pushtoken is not null order by r.UpdateDate desc";
            return Database.FirstOrDefault<MobileDevice>(sql, args);
        }

        public List<MobileDevice> RetrieveAll(LWQueryBatchInfo batchInfo)
		{
			var args = new object[0];
			string sql = "select r.* from LW_MobileDevice r";
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<MobileDevice>(sql, args);
		}

		public void Delete(long rewardId)
		{
			DeleteEntity(rewardId);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MobileDevice where MemberId = @0", memberId);
		}
	}
}
