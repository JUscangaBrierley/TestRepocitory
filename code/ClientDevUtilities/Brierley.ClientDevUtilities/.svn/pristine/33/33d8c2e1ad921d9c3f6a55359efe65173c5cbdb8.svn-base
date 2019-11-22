using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class AuditLogConfigDao : DaoBase<AuditLogConfig>
	{
		public AuditLogConfigDao(Database database, ServiceConfig config) : base(database, config)
		{
		}

		public AuditLogConfig Retrieve(long id)
		{
			return Database.FirstOrDefault<AuditLogConfig>("select * from LW_AuditLogConfig where id = @0", id);
		}

		public AuditLogConfig Retrieve(string typeName)
		{
			return Database.FirstOrDefault<AuditLogConfig>("select * from LW_AuditLogConfig where TypeName = @0", typeName);
		}

		public List<AuditLogConfig> RetrieveAll()
		{
			return Database.Fetch<AuditLogConfig>("select * from LW_AuditLogConfig");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
