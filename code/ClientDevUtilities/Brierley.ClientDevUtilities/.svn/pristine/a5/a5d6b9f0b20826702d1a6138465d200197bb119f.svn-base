using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class CampaignTableDao : DaoBase<CampaignTable>
	{
		public CampaignTableDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public CampaignTable Retrieve(long id)
		{
			return GetEntity(id);
		}

		public CampaignTable Retrieve(string name)
		{
			return Database.FirstOrDefault<CampaignTable>("select * from LW_CLTable where lower(TableName) = lower(@0)", name);
		}

		public CampaignTable RetrieveByAlias(string alias)
		{
			return Database.FirstOrDefault<CampaignTable>("select * from LW_CLTable where lower(Alias) = lower(@0)", alias);
		}

		public List<CampaignTable> RetrieveAll(TableInclusionType inclusionType = TableInclusionType.IncludeAll)
		{
			string sql = "select * from LW_CLTable";
			if (inclusionType == TableInclusionType.IncludeOnlyFramework)
			{
				sql += " where IsFrameworkSchema = 1";
			}
			else if (inclusionType == TableInclusionType.IncludeOnlyWarehouse)
			{
				sql += " where IsFrameworkSchema = 0";
			}
			return Database.Fetch<CampaignTable>(sql);
		}

		public List<CampaignTable> RetrieveAllByType(TableType[] tableType, TableInclusionType inclusionType = TableInclusionType.IncludeAll)
		{
			string sql = "select * from LW_CLTable where TableTypeId in(@tableTypes)";

			if (inclusionType == TableInclusionType.IncludeOnlyFramework)
			{
				sql += " and IsFrameworkSchema = 1";
			}
			else if (inclusionType == TableInclusionType.IncludeOnlyWarehouse)
			{
				sql += " and IsFrameworkSchema = 0";
			}
			sql += " order by TableName";
			return Database.Fetch<CampaignTable>(sql, new { tableTypes = tableType });
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
