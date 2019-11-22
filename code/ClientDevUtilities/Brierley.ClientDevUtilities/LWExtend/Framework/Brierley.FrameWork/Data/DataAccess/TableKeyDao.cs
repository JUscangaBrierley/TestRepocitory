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
	public class TableKeyDao : DaoBase<TableKey>
	{
		public TableKeyDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public TableKey Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<TableKey> RetrieveAll()
		{
			return Database.Fetch<TableKey>("select * from LW_CLTableKey");
		}

		public List<TableKey> RetrieveAllAudienceLevels()
		{
			return Database.Fetch<TableKey>("select * from LW_CLTableKey where IsAudienceLevel = 1");
		}

		public List<TableKey> RetrieveByAudience(long audienceId)
		{
			return Database.Fetch<TableKey>("select * from LW_CLTableKey where AudienceId = @0", audienceId);
		}

		public List<TableKey> RetrieveByTable(long tableId)
		{
			return Database.Fetch<TableKey>("select * from LW_CLTableKey where TableId = @0", tableId);
		}

		public System.Data.DataTable GetTableKeyReferences(long keyId)
		{
			var references = new System.Data.DataTable();
			references.Columns.Add("CampaignName", typeof(string));
			references.Columns.Add("StepName", typeof(string));

			var steps = Database.Fetch<Step>("select * from LW_CLStep where KeyId = @0", keyId);

			foreach (Step step in steps)
			{
				string campaignName = string.Empty;
				if (step.CampaignId.HasValue)
				{
					campaignName = Database.SingleOrDefault<Campaign>("select * from LW_CLCampaign where CampaignId = @0", step.CampaignId.Value).Name;
				}
				references.Rows.Add(campaignName, step.UIName);
			}
			return references;
		}

		public Dictionary<long, string> GetMappableAudiences(TableInclusionType inclusionType)
		{
			Dictionary<long, string> mappables = new Dictionary<long, string>();
			string sql = @"
			select 
				tk.KeyId as ""TableKeyId"", a.AudienceName as ""AudienceName"", t.TableName as ""TableName"", t.Alias as ""TableAlias"", tk.FieldName as ""FieldName"", tf.Alias as ""FieldAlias""
			from 
				LW_CLAudience a, LW_CLTableKey tk, LW_CLTable t, LW_CLTableField tf 
			where 
				tk.TableId = t.TableId and tk.AudienceId = a.AudienceId and tf.TableId = tk.TableId and tf.FieldName = tk.FieldName and tk.IsAudienceLevel = 1";

			if (inclusionType == TableInclusionType.IncludeOnlyFramework)
			{
				sql += " and t.IsFrameworkSchema = 1";
			}
			else if (inclusionType == TableInclusionType.IncludeOnlyWarehouse)
			{
				sql += " and t.IsFrameworkSchema = 0";
			}

			DateTime start = DateTime.Now;

			var rows = Database.Fetch<dynamic>(sql);
			if (rows != null)
			{
				foreach (dynamic row in rows)
				{
					long keyId = Convert.ToInt64(row.TableKeyId);
					string audienceName = row.AudienceName != null ? row.AudienceName : string.Empty;
					string tableName = row.TableName != null ? row.TableName : string.Empty;
					string tableAlias = row.TableAlias != null ? row.TableAlias : string.Empty;
					string fieldName = row.FieldName != null ? row.FieldName : string.Empty;
					string fieldAlias = row.FieldAlias != null ? row.FieldAlias : string.Empty;

					string desc = string.Format(
						"{0} ({1} in {2})",
						audienceName,
						string.IsNullOrEmpty(fieldAlias) ? fieldName : fieldAlias,
						string.IsNullOrEmpty(tableAlias) ? tableName : tableAlias);

					mappables.Add(keyId, desc);
				}
			}
			return mappables;
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
