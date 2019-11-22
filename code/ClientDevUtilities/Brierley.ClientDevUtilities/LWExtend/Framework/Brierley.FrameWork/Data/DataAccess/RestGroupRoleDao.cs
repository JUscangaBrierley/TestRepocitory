using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    /// <summary>
    /// DAO class for RestGroupRoleDao, for exposing inherited Create, Update, Delete.
    /// </summary>
    public class RestGroupRoleDao : RestDaoBase<RestGroupRole>
    {
        public RestGroupRoleDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }
    }
}
