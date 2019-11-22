using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;
using PetaPoco.Internal;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class RestResourceDao : RestDaoBase<RestResource>
    {
        public RestResourceDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public RestResource Retrieve(long id)
        {
            return GetEntity(id);
        }

        public List<RestResource> RetrieveByRestRoleId(long restRoleId, bool includeSoftDeleted=false)
        {
            var sql =  "select res.*, roleres.restpermissiontype from LW_RestResource res " +
                          "inner join LW_RestRoleResource roleres on res.id=roleres.restresourceid and roleres.restroleid = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "where res.isdeleted='N' and roleres.isdeleted='N' ";
            }

            return Database.Fetch<RestResource>(sql, restRoleId);
        }

        public List<RestResource> RetrieveByResourceType(LWQueryBatchInfo batchInfo, RestResourceType resourceType, bool includeSoftDeleted = false)
        {
            var sql = "select res.* from LW_RestResource res where res.RestResourceType = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "and res.isdeleted='N' ";
            }

            if (batchInfo != null)
            {
                object[] args = new object[] { (long)resourceType };

                PagingHelper.SQLParts parts;
                if (!PagingHelper.SplitSQL(sql, out parts))
                {
                    throw new LWDataServiceException("Unable to parse SQL statement for paged query");
                }
                sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);

                return Database.Fetch<RestResource>(sql, args);
            }

            return Database.Fetch<RestResource>(sql, (long)resourceType);
        }

        public List<RestResource> RetrieveAll(bool includeSoftDeleted = false)
        {
            var sql = "select * from LW_RestResource res ";
            if (!includeSoftDeleted)
            {
                sql += "where res.isdeleted='N' ";
            }

            return Database.Fetch<RestResource>(sql);
        }

    }
}
