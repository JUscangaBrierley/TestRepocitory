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
	public class PushSessionDao : DaoBase<PushSession>
	{
		private const string _className = "PushSessionDao";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public PushSessionDao(Database database, ServiceConfig config)
			: base(database, config){}

        public override void Create(PushSession session)
        {
            PushSession activeSession = RetrieveActiveSession(session.DeviceId);
            DateTime currentDateTime = DateTime.Now;

            if (activeSession != null)
                EndSession(activeSession, currentDateTime);            
            
            session.StartDate = currentDateTime;
            session.EndDate = currentDateTime.AddSeconds(1814400);            

            SaveEntity(session);
        }

        public long HowMany(long deviceId)
		{
			var args = new object[] { deviceId };
			string sql = "select count(*) from LW_PushSession m where m.DeviceId";
			return Database.ExecuteScalar<long>(sql, args);
		}

        public long HowManyActive(long deviceId)
        {
            var args = new object[] { deviceId, DateTime.Now };
            string sql = "select count(*) from LW_PushSession m where m.DeviceId and @1 between r.StartDate and r.EndDate order by r.EndDate desc";
            return Database.ExecuteScalar<long>(sql, args);
        }

        public PushSession Retrieve(long Id)
        {
            return GetEntity(Id);
        }

        public PushSession RetrieveActiveSession(long deviceId)
        {
            var args = new object[] { deviceId, DateTime.Now };
            string sql = "select r.* from LW_PushSession r where r.DeviceId = @0 and @1 between r.StartDate and r.EndDate order by r.EndDate desc";
            return Database.Fetch<PushSession>(sql, args).FirstOrDefault();
        }

        public List<PushSession> RetrieveByDeviceId(long deviceId, LWQueryBatchInfo batchInfo)
		{
			var args = new object[]{ deviceId };
			string sql = "select r.* from LW_PushSession r where DeviceId = @0 order by DateIssued desc";
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<PushSession>(sql, args);
		}

        public void EndSession(PushSession session, DateTime endDate)
        {
            session.EndDate = endDate.AddSeconds(-1);
            UpdateEntity(session);
        }
	}
}
