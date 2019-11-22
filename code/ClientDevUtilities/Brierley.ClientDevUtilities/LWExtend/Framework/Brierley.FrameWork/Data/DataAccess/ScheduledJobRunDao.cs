using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ScheduledJobRunDao : DaoBase<ScheduledJobRun>
	{
		public ScheduledJobRunDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public bool HasJobBeenRun(long jobId)
		{
			int count = Database.ExecuteScalar<int>("select count(*) from LW_ScheduledJobRun where JobId = @0", jobId);
			return count > 0;
		}

		public DateTime? GetLastJobRun(long jobId)
		{
			DateTime? lastRun = Database.FirstOrDefault<DateTime?>("select max(StartTime) from LW_ScheduledJobRun where jobid = @0", jobId);
			if (lastRun.HasValue)
			{
				return lastRun.Value;
			}
			return null;
		}

		public ScheduledJobRun Retrieve(long id)
		{
			return Database.FirstOrDefault<ScheduledJobRun>("select * from LW_ScheduledJobRun where id = @0", id);
		}

		public List<ScheduledJobRun> RetrieveByJobId(long jobId)
		{
			return Database.Fetch<ScheduledJobRun>("select * from LW_ScheduledJobRun where jobid = @0 order by StartTime desc", jobId);
		}

		public IEnumerable<ScheduledJobRun> RetrieveIncompleteJobRuns(DateTime afterStartDate)
		{
			return Database.Fetch<ScheduledJobRun>(
				"select * from LW_ScheduledJobRun where EndTime is null and status = @0 and StartTime >= @1",
				ScheduleJobStatus.InProcess,
				afterStartDate);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
