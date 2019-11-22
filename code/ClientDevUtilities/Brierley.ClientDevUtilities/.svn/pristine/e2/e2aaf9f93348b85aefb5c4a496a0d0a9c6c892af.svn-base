using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;
using System.Collections.Generic;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class RestRoleDao : RestDaoBase<RestRole>
    {
        public RestRoleDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public RestRole Retrieve(long id)
        {
            return GetEntity(id);
        }

        public RestRole RetrieveByName(string name, bool includeSoftDeleted=false)
        {
            var sql = "select * from LW_RestRole role " +
                      "where role.name = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "and role.isdeleted='N' ";
            }
            return Database.SingleOrDefault<RestRole>(sql, name);
        }

        public List<RestRole> RetrieveByRestConsumerId(long restConsumerId, bool includeSoftDeleted=false)
        {
            var sql = "select role.* from LW_RestRole role " +
                      "inner join LW_RestConsumerRole cnrole on role.id=cnrole.restroleid and cnrole.restconsumerid = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "where role.isdeleted='N' and cnrole.isdeleted='N' ";
            }

            return Database.Fetch<RestRole>(sql, restConsumerId);
        }

        public List<RestRole> RetrieveByRestGroupId(long restGroupId, bool includeSoftDeleted = false)
        {
            var sql = "select role.* from LW_RestRole role " +
                      "inner join LW_RestGroupRole gprole on role.id=gprole.restroleid and gprole.restgroupid = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "where role.isdeleted='N' and gprole.isdeleted='N' ";
            }

            return Database.Fetch<RestRole>(sql, restGroupId);
        }

        public List<RestRole> RetrieveAll(bool includeSoftDeleted = false)
        {
            var sql = "select * from LW_RestRole role ";
            if (!includeSoftDeleted)
            {
                sql += "where role.isdeleted='N' ";
            }

            return Database.Fetch<RestRole>(sql);
        }

    }
}
