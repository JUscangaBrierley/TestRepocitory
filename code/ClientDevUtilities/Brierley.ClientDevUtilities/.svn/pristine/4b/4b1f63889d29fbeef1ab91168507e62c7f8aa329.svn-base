using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ProductDao : ContentDefDaoBase<Product>
	{
		public ProductDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.Product)
		{
		}

		public Product Retrieve(long id)
		{
			Product product = GetEntity(id);
			if (product != null)
			{
				PopulateContent(product);
			}
			return product;
		}

		public Product Retrieve(string name)
		{
			Product product = Database.FirstOrDefault<Product>("select * from LW_Product where name = @0", name);
			if (product != null)
			{
				PopulateContent(product);
			}
			return product;
		}

		public Product RetrieveByPartNumber(string partNumber)
		{
			Product product = Database.FirstOrDefault<Product>("select * from LW_Product where PartNumber = @0", partNumber);
			if (product != null)
			{
				PopulateContent(product);
			}
			return product;
		}

		public long RetrieveIdByPartNumber(string partNumber)
		{
			return Database.ExecuteScalar<long>("select Id from LW_Product where PartNumber = @0", partNumber);
		}

		public Product RetrieveByVariantPartNumber(string partNumber)
		{
            var ret = Database.FirstOrDefault<Product>("select p.* from LW_Product p, LW_ProductVariant v where p.Id = v.ProductId and v.PartNumber = @0", partNumber);
			if (ret != null)
			{
				PopulateContent(ret);
			}
			return ret;
		}

		public long HowMany()
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_Product");
		}

		public List<Product> RetrieveByCategory(long categoryId, bool visibleInLN)
		{
			string sql = "select * from LW_Product where CategoryId = @0";
			if (visibleInLN)
			{
				sql += " and (IsVisibleInLN is null or IsVisibleInLN = 1)";
			}
			else
			{
				sql += " and (IsVisibleInLN is not null and IsVisibleInLN = 0)";
			}
			return Database.Fetch<Product>(sql, categoryId);
		}

		public List<Product> RetrieveByCategorySortedByName(long categoryId, bool visibleInLN)
		{
			string sql = "select * from LW_Product where CategoryId = @0";
			if (visibleInLN)
			{
				sql += " and (IsVisibleInLN is null or IsVisibleInLN = 1)";
			}
			else
			{
				sql += " and (IsVisibleInLN is not null and IsVisibleInLN = 0)";
			}
			sql += " order by upper(name)";
			return Database.Fetch<Product>(sql, categoryId);
		}

		public List<Product> RetrieveAll(bool visibleInLN)
		{
			string sql = "select * from LW_Product";
			if (visibleInLN)
			{
				sql += " where (IsVisibleInLN is null or IsVisibleInLN = 1)";
			}
			else
			{
				sql += " where (IsVisibleInLN is not null and IsVisibleInLN = 0)";
			}
			return Database.Fetch<Product>(sql);
		}

		public List<Product> RetrieveAll(long[] ids, bool retrieveContent)
		{
			int idsRemaining = ids.Length;
			int startIdx = 0;
			List<Product> products = new List<Product>();
			while (idsRemaining > 0)
			{
				long[] idsBatch = LimitInClauseList<long>(ids, ref startIdx, ref idsRemaining);
                List<Product> set = Database.Fetch<Product>("select * from LW_Product where id in (@ids)", new { ids = idsBatch });
				if (set != null && set.Count > 0)
				{
					if (retrieveContent)
					{
						PopulateContent(set, false);
					}
					products.AddRange(set);
				}
			}
			return products;
		}

		public List<Product> RetrieveAllSortedByName(bool visibleInLN)
		{
			string sql = "select * from LW_Product";
			if (visibleInLN)
			{
				sql += " where (IsVisibleInLN is null or IsVisibleInLN = 1)";
			}
			else
			{
				sql += " where (IsVisibleInLN is not null and IsVisibleInLN = 0)";
			}
			sql += " order by upper(name)";
			return Database.Fetch<Product>(sql);
		}

		public List<Product> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<Product>("select * from LW_Product where UpdateDate >= @0", since);
		}

		public List<Product> RetrieveByUserField(string userField)
		{
			return Database.Fetch<Product>("select * from LW_Product where StrUserField >= @0", userField);
		}

		public List<Product> RetrieveByUserField(long userField)
		{
			return Database.Fetch<Product>("select * from LW_Product where StrUserField >= @0", userField);
		}

		public void Delete(long productId)
		{
			DeleteEntity(productId);
		}

		public List<long> RetrieveProductIdsByProperty(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, bool visibleInLN)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			flags.Add("visibleInLN", visibleInLN);
			List<long> ids = GetContentIds(parms, flags);
			if (!string.IsNullOrEmpty(sortExpression) && ids != null && ids.Count > 0)
			{
				ids = SortContentIds(ids, sortExpression, ascending);
			}
			return ids;
		}

		public List<long> RetrieveAllProductIds(string sortExpression, bool ascending, bool visibleInLN)
		{
			string sql = "select Id from LW_Product where ";
			if (visibleInLN)
			{
				sql += "(IsVisibleInLn is null or IsVisibleInLn = 1)";
			}
			else
			{
				sql += "(IsVisibleInLn is not null and IsVisibleInLn = 0)";
			}
			if (!string.IsNullOrEmpty(sortExpression))
			{
				sql += string.Format(" order by {0} {1}", sortExpression, ascending ? "asc" : "desc");
			}
			return Database.Fetch<long>(sql);
		}

        public long UpdateQuantity(long id, long changeBy)
        {
            string sql;
            string output = string.Empty;
            switch (Config.DatabaseType)
            {
                case Common.SupportedDataSourceType.Oracle10g:
                    sql = "UPDATE LW_Product set Quantity = Quantity + @1 WHERE ID = @0 RETURNING Quantity INTO :newQuantity";
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
                    sql = "DECLARE ^ INT; UPDATE LW_Product set ^ = Quantity + @1, Quantity = Quantity + @1 WHERE ID = @0; SELECT ^;";
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
                    throw new LWException("Unsupported database type for ProductDao.UpdateQuantity: " + Config.DatabaseType.ToString());
            }
        }
    }
}
