using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class CampaignOfferDao : DaoBase<Offer>
	{
		public CampaignOfferDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public Offer Retrieve(long id)
		{
			return GetEntity(id);
		}

		public Offer Retrieve(long campaignId, string offerCode)
		{
			return Database.FirstOrDefault<Offer>("select * from LW_CLOffer where CampaignId = @0 and lower(OfferCode) = lower(@1)", campaignId, offerCode);
		}

		public IEnumerable<Offer> RetrieveByCampaign(long campaignId)
		{
			return Database.Fetch<Offer>("select * from LW_CLOffer where CampaignId = @0", campaignId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
