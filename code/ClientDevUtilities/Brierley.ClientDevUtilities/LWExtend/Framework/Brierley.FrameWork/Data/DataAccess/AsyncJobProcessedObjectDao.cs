using System.Collections.Generic;
using System.Diagnostics;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class AsyncJobProcessedObjectDao : DaoBase<AsyncJobProcessedObjects>
	{
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private string _className = "AsyncJobProcessedObjectDao";

		public AsyncJobProcessedObjectDao(Database database, ServiceConfig config) : base(database, config)
		{
		}

		public List<long> RetrieveObjectIds(string jobName, string objectName)
		{
			string methodName = "RetrieveObjectIds";

			string sql = string.Format("select a.RowKey from LW_AsyncJobProcessedObjects a where a.JobNumber = ((select max(JobNumber) from LW_AsyncJobProcessedObjects)) and");
			if (!string.IsNullOrEmpty(jobName))
			{
				sql = string.Format("{0} a.JobName = @0 and", sql);
			}
			sql = string.Format("{0} a.ObjectName = @1", sql);

			_logger.Debug(_className, methodName, "Executing query: " + sql);
			
			
			Stopwatch timer = new Stopwatch();
			timer.Start();
			List<long> result = Database.Fetch<long>(sql, jobName, objectName);
			timer.Stop();

			long count = result != null ? result.Count : 0;

			_logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", count, timer.ElapsedMilliseconds));

			return result;
		}

		public List<long> RetrieveObjectIds(long jobNumber, string objectName)
		{
			string methodName = "RetrieveObjectIds";

			string sql = string.Format("select a.RowKey from LW_AsyncJobProcessedObjects a where a.JobNumber = @0 and a.ObjectName = @1");

			_logger.Debug(_className, methodName, "Executing query: " + sql);

			Stopwatch timer = new Stopwatch();
			timer.Start();
			List<long> result = Database.Fetch<long>(sql, jobNumber, objectName);
			timer.Stop();

			long count = result != null ? result.Count : 0;

			_logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", count, timer.ElapsedMilliseconds));

			return result;
		}              
	}
}
