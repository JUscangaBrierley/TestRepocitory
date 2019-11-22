using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class CampaignAttributeDao : DaoBase<CampaignAttribute>
	{
		public CampaignAttributeDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public CampaignAttribute Retrieve(long id)
		{
			return GetEntity(id);
		}

		public CampaignAttribute Retrieve(long campaignId, long attributeId)
		{
			return Database.FirstOrDefault<CampaignAttribute>(
				"select * from LW_CLCampaignAttribute where CampaignId = @0 and AttributeId = @1", 
				campaignId, 
				attributeId);
		}

		public List<CampaignAttribute> RetrieveAll()
		{
			return Database.Fetch<CampaignAttribute>("select * from LW_CLCampaignAttribute");
		}

		public List<CampaignAttribute> RetrieveByCampaign(long campaignId)
		{
			return Database.Fetch<CampaignAttribute>("select * from LW_CLCampaignAttribute where CampaignId = @0", campaignId);
		}

		public List<CampaignAttribute> RetrieveByOffer(long offerId)
		{
			return Database.Fetch<CampaignAttribute>("select * from LW_CLCampaignAttribute where OfferId = @0", offerId);
		}

		public List<CampaignAttribute> RetrieveBySegment(long segmentId)
		{
			return Database.Fetch<CampaignAttribute>("select * from LW_CLCampaignAttribute where SegmentId = @0", segmentId);
		}

		public void Delete(long campaignId, long attributeId)
		{
			Database.Execute("delete from LW_CLCampaignAttribute where CampaignId = @0 and AttributeId = @1", campaignId, attributeId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
