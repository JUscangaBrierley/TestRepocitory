using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class CSRoleDao : DaoBase<CSRole>
    {
        public CSRoleDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public CSRole Retrieve(long roleID)
        {
            return GetEntity(roleID);
        }

        public CSRole Retrieve(string roleName)
        {
            return Database.FirstOrDefault<CSRole>("select * from LW_CSRole where Name = @0", roleName);
        }

        public List<CSFunction> RetrieveFunctions(long roleId)
        {
            return Database.Fetch<CSFunction>("select f.* from LW_CSFunction f, LW_CSRole r, LW_CSRoleFunction rf where rf.RoleId = r.Id and rf.FunctionId = f.Id and r.Id = @0", roleId);
        }

        public List<CSRole> RetrieveAll()
        {
            return Database.Fetch<CSRole>("select * from LW_CSRole");
        }

        public void Delete(long roleID)
        {
            DeleteEntity(roleID);
        }
    }
}
