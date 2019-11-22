using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ProductVariantDao : DaoBase<ProductVariant>
	{
		public ProductVariantDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public ProductVariant Retrieve(long id)
		{
			return GetEntity(id);
		}

		public ProductVariant RetrieveByProductAndPartNumber(long productId, string partNumber)
		{
			return Database.FirstOrDefault<ProductVariant>("select * from LW_ProductVariant where ProductId = @0 and PartNumber = @1 order by VariantOrder", productId, partNumber);
		}

        public ProductVariant RetrieveByProductAndDescription(long productId, string description)
        {
            return Database.FirstOrDefault<ProductVariant>("select * from LW_ProductVariant where ProductId = @0 and VariantDescription = @1", productId, description);
        }

		public List<ProductVariant> RetrieveByProduct(long productId)
		{
			return Database.Fetch<ProductVariant>("select * from LW_ProductVariant where ProductId = @0 order by VariantOrder", productId);
		}

		public List<ProductVariant> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<ProductVariant>("select * from LW_ProductVariant where UpdateDate >= @0", since);
		}

		public List<ProductVariant> RetrieveAll()
		{
			return Database.Fetch<ProductVariant>("select * from LW_ProductVariant");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

        public long UpdateQuantity(long id, long changeBy)
        {
            string sql;
            string output = string.Empty;
            switch (Config.DatabaseType)
            {
                case Common.SupportedDataSourceType.Oracle10g:
                    sql = "UPDATE LW_ProductVariant set Quantity = Quantity + @1 WHERE ID = @0 RETURNING Quantity INTO :newQuantity";
                    if (Database.Connection == null)
                        Database.OpenSharedConnection();
                    using (var cmd = Database.CreateCommand(Database.Connection, sql, id, changeBy))
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = ":newQuantity";
                        param.Value = DBNull.Value;
                        param.Direction = ParameterDirection.ReturnValue;
                        param.DbType = DbType.Int64;
                        cmd.Parameters.Add(param);
                        Database.ExecuteNonQueryHelper(cmd);
                        if (param.Value is DBNull)
                            throw new LWException("Null value returned for UpdateQuantity call");
                        return (long)param.Value;
                    }

                case Common.SupportedDataSourceType.MsSQL2005:
                    sql = "DECLARE ^ INT; UPDATE LW_ProductVariant set ^ = Quantity + @1, Quantity = Quantity + @1 WHERE ID = @0; SELECT ^;";
                    if (Database.Connection == null)
                        Database.OpenSharedConnection();
                    using (var cmd = Database.CreateCommand(Database.Connection, sql, id, changeBy))
                    {
                        cmd.CommandText = cmd.CommandText.Replace("^", "@qty");
                        var result = Database.ExecuteScalarHelper(cmd);
                        if (result is DBNull)
                            throw new LWException("Null value returned for UpdateQuantity call");
                        return long.Parse(result.ToString());
                    }

                default:
                    throw new LWException("Unsupported database type for ProductVariantDao.UpdateQuantity: " + Config.DatabaseType.ToString());
            }
        }
    }
}
