using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;
using System.Collections.Generic;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class RestGroupDao : RestDaoBase<RestGroup>
    {
        public RestGroupDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public RestGroup Retrieve(long id)
        {
            return GetEntity(id);
        }

        public RestGroup RetrieveByName(string name, bool includeSoftDeleted = false)
        {
            var sql = "select * from LW_RestGroup gp " +
                      "where gp.name = @0 ";
            if (!includeSoftDeleted)
            {
                sql += " and gp.isdeleted='N' ";
            }

            return Database.SingleOrDefault<RestGroup>(sql, name);
        }

        public List<RestGroup> RetrieveByRestConsumerId(long restConsumerId, bool includeSoftDeleted = false)
        {
            var sql = "select gp.* from LW_RestGroup gp " +
                      "inner join LW_RestConsumerGroup cgp on gp.id = cgp.restgroupid and cgp.restconsumerid = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "where gp.isdeleted='N' and cgp.isdeleted='N' ";
            }

            return Database.Fetch<RestGroup>(sql, restConsumerId);
        }

        public List<RestGroup> RetrieveByRestRoleId(long restRoleId, bool includeSoftDeleted = false)
        {
            var sql = "select gp.* from LW_RestGroup gp " +
                      "inner join LW_RestGroupRole rgr on gp.id = rgr.restgroupid and rgr.restroleid = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "where gp.isdeleted='N' and rgr.isdeleted='N' ";
            }

            return Database.Fetch<RestGroup>(sql, restRoleId);

        }

    }
}
