using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ScheduledJobDao : DaoBase<ScheduledJob>
	{
		public ScheduledJobDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public ScheduledJob Retrieve(long jobId)
		{
			return Database.FirstOrDefault<ScheduledJob>("select * from LW_ScheduledJob where id = @0", jobId);
		}

		public List<ScheduledJob> Retrieve(string assemblyName, string factoryName)
		{
			return Database.Fetch<ScheduledJob>("select * from LW_ScheduledJob where AssemblyName = @0 and FactoryName = @1", assemblyName, factoryName);
		}

		public ScheduledJob Retrieve(string name)
		{
			return Database.FirstOrDefault<ScheduledJob>("select * from LW_ScheduledJob where name = @0", name);
		}

		public List<ScheduledJob> RetrieveAll()
		{
			return Database.Fetch<ScheduledJob>("select * from LW_ScheduledJob");
		}

		public List<ScheduledJob> RetrieveAll(List<Dictionary<string, object>> parms)
		{
			string sql = "select * from LW_ScheduledJob j";

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
						actualParmName = "Name";
						queryParms.Add(queryParm, entry["Value"]);
						break;
					case "Type":
						actualParmName = "JobType";
						queryParms.Add(queryParm, entry["Value"]);
						break;
                    case "StartDate":
                        actualParmName = "StartDate";
                        queryParms.Add(queryParm, entry["Value"]);
                        break;
                    case "EndDate":
                        actualParmName = "EndDate";
                        queryParms.Add(queryParm, entry["Value"]);
                        break;
				}
				sql = string.Format("{0} j.{1} {3} @{2}", sql, actualParmName, i, LWCriterion.GetSqlPredicate((LWCriterion.Predicate)entry["Predicate"]));
			}

			object[] parameters = queryParms.Values.ToArray();
			return Database.Fetch<ScheduledJob>(sql, parameters);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
