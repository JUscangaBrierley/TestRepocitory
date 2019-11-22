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
	public class CampaignDao : DaoBase<Campaign>
	{
		public CampaignDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public Campaign Retrieve(long id)
		{
			return GetEntity(id);
		}
		
		public Campaign Retrieve(string name)
		{
			return Database.FirstOrDefault<Campaign>("select * from LW_CLCampaign where lower(CampaignName) = lower(@0)", name);
		}

		public List<Campaign> RetrieveAll(bool includeTemplates, params CampaignType[] campaignType)
		{
			string sql = "select * from LW_CLCampaign";

			if (!includeTemplates)
			{
				sql += " where IsTemplate = 0";
			}

			if (campaignType.Length > 0)
			{
				sql += (includeTemplates ? " where " : " and ") + "CampaignType in (@campaignTypes)";
			}
			return Database.Fetch<Campaign>(sql, new { campaignTypes = campaignType.ToArray() });
		}

        public List<Campaign> RetrieveAll(bool includeTemplates, CampaignType? campaignType, long pageNumber, long pageSize)
        {
            List<object> searchParams = new List<object>();
            string sql = "select * from LW_CLCampaign";

            if (!includeTemplates)
            {
                sql += " where IsTemplate = 0 ";
            }
            else
            {
                sql += " where IsTemplate = 1 ";
            }

            if (campaignType != null)
            {
                sql += "AND CampaignType = @0";
                searchParams.Add(campaignType);
            }

            return Database.Fetch<Campaign>(pageNumber, pageSize, sql, searchParams.ToArray());
        }

        public List<Campaign> Retrieve(List<long> campaignIds, bool? isExecuting, CampaignType? campaignType )
        {
            List<object> searchParams = new List<object>();
            string sql = "select * from LW_CLCampaign where CampaignId in (@0) ";

            searchParams.Add(campaignIds);

            if(isExecuting != null)
            {
                sql += "AND IsExecuting = @1 ";
                searchParams.Add(isExecuting);
            }

            if(campaignType != null)
            {
                sql += "AND CampaignType = @2";
                searchParams.Add(campaignType);
            }
            


            return Database.Fetch<Campaign>(sql, searchParams.ToArray());
        }


        public List<Campaign> RetrieveByFolder(long folderId, bool includeTemplates)
		{
			string sql = "select * from LW_CLCampaign where FolderId = @0";
			if (!includeTemplates)
			{
				sql += " and IsTemplate = 0";
			}
			return Database.Fetch<Campaign>(sql, folderId);
		}

		public List<Campaign> RetrieveAllTemplates(CampaignType? campaignType)
		{
			string sql = "select * from LW_CLCampaign where IsTemplate = 1";
			if (campaignType.HasValue)
			{
				sql += " and CampaignType = @0";
			}
			return Database.Fetch<Campaign>(sql, campaignType);
		}

		public List<Campaign> RetrieveTemplatesByFolder(long folderId)
		{
			return Database.Fetch<Campaign>("select * from LW_CLCampaign where IsTemplate = 1 and FolderId = @0", folderId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
