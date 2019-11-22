using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    /// <summary>
    /// DAO class for RestRoleResource, for exposing inherited Create, Update, Delete.
    /// </summary>
    public class RestRoleResourceDao : RestDaoBase<RestRoleResource>
    {
        public RestRoleResourceDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public void Update(long restRoleId, long restResourceId, RestPermissionType permission)
        {
            var sql =
                "update LW_RestRoleResource t set t.restpermissiontype=@0 where t.restroleid=@1 and t.restresourceid=@2";
            Database.Execute(sql, (long) permission, restRoleId, restResourceId);
        }
    }
}
