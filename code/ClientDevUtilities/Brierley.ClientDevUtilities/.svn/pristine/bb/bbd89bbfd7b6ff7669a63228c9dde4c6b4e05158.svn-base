using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class LWSchemaVersionDao : DaoBase<LWSchemaVersion>
	{
		public LWSchemaVersionDao(Database database, ServiceConfig config) : base(database, config)
		{
		}

		public List<LWSchemaVersion> RetrieveByTargetType(string targetType)
		{
			try
			{
				var versionSet = Database.Fetch<LWSchemaVersion>("select * from LW_Version where TargetType = @0 order by DateApplied desc", targetType);
				if (versionSet != null && versionSet.Count > 0)
				{
                    return versionSet.OrderByDescending(
                        x => x.VersionNumber.Split('.')[0]).ThenByDescending(
                        x => x.VersionNumber.Split('.').Length > 1 ? x.VersionNumber.Split('.')[1] : "0").ThenByDescending(
                        x => x.VersionNumber.Split('.').Length > 2 ? x.VersionNumber.Split('.')[2] : "0").ToList();
				}
				else
				{
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public LWSchemaVersion RetrieveLatestVersionByTargetType(string targetType)
		{
			try
			{
				var versionSet = Database.Fetch<LWSchemaVersion>("select * from LW_Version where TargetType = @0 order by DateApplied desc", targetType);
                if (versionSet != null && versionSet.Count > 0)
                {
                    return versionSet.OrderByDescending(
                        x => x.VersionNumber.Split('.')[0]).ThenByDescending(
                        x => x.VersionNumber.Split('.')[1]).ThenByDescending(
                        x => x.VersionNumber.Split('.')[2]).FirstOrDefault();
                }
                else
				{
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public List<LWSchemaVersion> RetrieveAll()
		{
			try
			{
				return Database.Fetch<LWSchemaVersion>("select * from LW_Version order by DateApplied desc");
			}
			catch
			{
				return null;
			}
		}
	}
}