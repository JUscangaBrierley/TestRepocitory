using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class CSRoleFunctionDao : DaoBase<CSRoleFunction>
    {
        public CSRoleFunctionDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

        public CSRoleFunction Retrieve(long id)
        {
            return GetEntity(id);
        }

        public List<CSRoleFunction> RetrieveByRole(long roleId)
        {
            return Database.Fetch<CSRoleFunction>("select * from LW_CSRoleFunction where RoleId = @0", roleId);
        }

        public List<CSRoleFunction> RetrieveByFunction(long funcId)
        {
            return Database.Fetch<CSRoleFunction>("select * from LW_CSRoleFunction where FunctionId = @0", funcId);
        }

        public List<CSRoleFunction> RetrieveByRoleAndFunction(long roleId, long funcId)
        {
            return Database.Fetch<CSRoleFunction>("select * from LW_CSRoleFunction where RoleId = @0 and FunctionId = @1", roleId, funcId);
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
        }

        public void DeleteByRole(long roleId)
        {
            Database.Execute("delete from LW_CSRoleFunction where RoleId = @0", roleId);
        }
    }
}
