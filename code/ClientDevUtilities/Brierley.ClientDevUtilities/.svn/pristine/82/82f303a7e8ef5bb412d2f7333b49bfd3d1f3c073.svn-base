using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class AsyncJobDao : DaoBase<AsyncJob>
	{
		public AsyncJobDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public AsyncJob Retrieve(long id)
		{
			return Database.FirstOrDefault<AsyncJob>("select * from LW_LibJob where Id = @0", id);
		}

		public AsyncJob RetrieveByJobNumber(long jobNumber)
		{
			return Database.FirstOrDefault<AsyncJob>("select * from LW_LibJob where JobNumber = @0", jobNumber);
		}

		public List<AsyncJob> RetrieveAll()
		{
			return Database.Fetch<AsyncJob>("select * from LW_LibJob");
		}

		public List<AsyncJob> RetrieveAll(List<Dictionary<string, object>> parms)
		{
			string sql = "select * from LW_LibJob j";

			bool first = true;
			int i = 0;
			Dictionary<string, object> queryParms = new Dictionary<string, object>();
			foreach (Dictionary<string, object> entry in parms)
			{
				if (first)
				{
					sql += " where ";
					first = false;
				}
				else
				{
					// handle AND/OR
					if (entry.ContainsKey("Operator"))
					{
						sql += entry["Operator"].ToString();
					}
					else
					{
						sql += " and ";
					}
					i++;
				}

				string queryParm = string.Format("parm{0}", i);
				string parmName = entry["Property"].ToString();
				string actualParmName = parmName;
				switch (parmName)
				{
					case "Name":
						actualParmName = "JobName";
						queryParms.Add(queryParm, entry["Value"]);
						break;
					case "Type":
						actualParmName = "JobType";
						queryParms.Add(queryParm, entry["Value"]);
						break;
					case "Status":
						actualParmName = "JobStatus";
						LIBJobStatusEnum s = LIBJobStatusEnum.InProcess;
						try
						{
							s = (LIBJobStatusEnum)Enum.Parse(typeof(LIBJobStatusEnum), entry["Value"].ToString());
							queryParms.Add(queryParm, s);
						}
						catch (Exception)
						{
							// the status is not a valid status for this type of job.
							string errMsg = string.Format("{0} is an invalid status value for this type of searching AsynJobs", entry["Value"].ToString());
							throw new Brierley.FrameWork.Common.Exceptions.LWException(errMsg);
						}
						break;
				}
				sql = string.Format("{0} j.{1} {3} @{2}", sql, actualParmName, i, LWCriterion.GetSqlPredicate((LWCriterion.Predicate)entry["Predicate"]));
			}

			var parameters = new List<object>();

			foreach (string key in queryParms.Keys)
			{
				parameters.Add(queryParms[key]);
			}

			return Database.Fetch<AsyncJob>(sql, parameters.ToArray());
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
