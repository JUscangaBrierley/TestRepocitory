using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class RemoteAssemblyDao : DaoBase<RemoteAssembly>
    {
        public RemoteAssemblyDao(Database database, ServiceConfig config)
			: base(database, config)
		{
        }

        public RemoteAssembly Retrieve(long id)
        {
            return GetEntity(id);
        }

        public RemoteAssembly Retrieve(string fileName)
        {
            return Database.FirstOrDefault<RemoteAssembly>("select * from LW_RemoteAssembly where AssemblyFileName = @0", fileName);
        }

        public List<RemoteAssembly> RetrieveAll()
        {
            return Database.Fetch<RemoteAssembly>("select * from LW_RemoteAssembly");
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
        }
    }
}
