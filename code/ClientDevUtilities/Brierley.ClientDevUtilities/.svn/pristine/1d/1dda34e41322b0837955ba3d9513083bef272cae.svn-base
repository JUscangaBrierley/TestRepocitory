using System;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;
using System.Collections.Generic;
using Brierley.FrameWork.Common.Exceptions;
using PetaPoco.Internal;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class RestConsumerDao : RestDaoBase<RestConsumer>
    {
        public RestConsumerDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public RestConsumer Retrieve(long id)
        {
            return GetEntity(id);
        }

        public RestConsumer RetrieveByConsumerId(string consumerId, bool includeSoftDeleted = false)
        {
            var sql = "select * from LW_RestConsumer con " +
                      "where con.consumerid = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "and con.isdeleted='N' ";
            }

            return Database.SingleOrDefault<RestConsumer>(sql, consumerId);
        }

        public List<RestConsumer> RetrieveByCustomId(LWQueryBatchInfo batchInfo, string customId, bool includeSoftDeleted = false)
        {
            var sql = "select con.* from LW_RestConsumer con " +
                      "where con.customid = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "and con.isdeleted='N' ";
            }

            if (batchInfo != null)
            {
                object[] args = new object[] { customId };

                PagingHelper.SQLParts parts;
                if (!PagingHelper.SplitSQL(sql, out parts))
                {
                    throw new LWDataServiceException("Unable to parse SQL statement for paged query");
                }
                sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);

                return Database.Fetch<RestConsumer>(sql, args);
            }

            return Database.Fetch<RestConsumer>(sql, customId);
        }

        public RestConsumer RetrieveByUsername(string userName, bool includeSoftDeleted = false)
        {
            var sql = "select * from LW_RestConsumer con " +
                      "where con.username = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "and con.isdeleted='N' ";
            }

            return Database.SingleOrDefault<RestConsumer>(sql, userName);
        }

        public List<RestConsumer> RetrieveByRestRoleId(LWQueryBatchInfo batchInfo, long restRoleId, bool includeSoftDeleted = false)
        {
            var sql = "select con.* from LW_RestConsumer con " +
                      "inner join LW_RestConsumerRole rcr on con.id=rcr.restconsumerid and rcr.restroleid = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "where con.isdeleted='N' and rcr.isdeleted='N' ";
            }

            if (batchInfo != null)
            {
                object[] args = new object[] { restRoleId };

                PagingHelper.SQLParts parts;
                if (!PagingHelper.SplitSQL(sql, out parts))
                {
                    throw new LWDataServiceException("Unable to parse SQL statement for paged query");
                }
                sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);

                return Database.Fetch<RestConsumer>(sql, args);
            }

            return Database.Fetch<RestConsumer>(sql, restRoleId);
        }

        public List<RestConsumer> RetrieveByRestGroupId(LWQueryBatchInfo batchInfo, long restGroupId, bool includeSoftDeleted = false)
        {
            var sql = "select con.* from LW_RestConsumer con " +
                      "inner join LW_RestConsumerGroup rcg on con.id=rcg.restconsumerid and rcg.restgroupid = @0 ";
            if (!includeSoftDeleted)
            {
                sql += "where con.isdeleted='N' and rcg.isdeleted='N' ";
            }

            if (batchInfo != null)
            {
                object[] args = new object[] {restGroupId};

                PagingHelper.SQLParts parts;
                if (!PagingHelper.SplitSQL(sql, out parts))
                {
                    throw new LWDataServiceException("Unable to parse SQL statement for paged query");
                }
                sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);

                return Database.Fetch<RestConsumer>(sql, args);
            }

            return Database.Fetch<RestConsumer>(sql, restGroupId);
        }

        public List<RestConsumer> RetrieveAll(LWQueryBatchInfo batchInfo, bool includeSoftDeleted = false)
        {
            var sql = "select con.* from LW_RestConsumer con ";
            if (!includeSoftDeleted)
            {
                sql += "where con.isdeleted='N' ";
            }

            if (batchInfo != null)
            {
                object[] args = new object[] { };

                PagingHelper.SQLParts parts;
                if (!PagingHelper.SplitSQL(sql, out parts))
                {
                    throw new LWDataServiceException("Unable to parse SQL statement for paged query");
                }
                sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);

                return Database.Fetch<RestConsumer>(sql, args);
            }
            return Database.Fetch<RestConsumer>(sql);
        }
    }
}
