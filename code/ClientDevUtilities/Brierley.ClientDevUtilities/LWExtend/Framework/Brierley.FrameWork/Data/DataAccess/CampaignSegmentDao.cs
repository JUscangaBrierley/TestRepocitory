using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class CampaignSegmentDao : DaoBase<Segment>
	{
		public CampaignSegmentDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public Segment Retrieve(long id)
		{
			return GetEntity(id);
		}

		public Segment Retrieve(long campaignId, string segmentCode)
		{
			return Database.FirstOrDefault<Segment>(
				"select * from LW_CLSegment where CampaignId = @0 and lower(SegmentCode) = lower(@1)", 
				campaignId, 
				segmentCode);
		}

		public IEnumerable<Segment> RetrieveByCampaign(long campaignId)
		{
			return Database.Fetch<Segment>("select * from LW_CLSegment where CampaignId = @0", campaignId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
