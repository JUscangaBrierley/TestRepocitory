using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class CampaignOfferSegmentDao : DaoBase<OfferSegment>
	{
		public CampaignOfferSegmentDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public OfferSegment Create(long offerId, long segmentId)
		{
			var xref = new OfferSegment(offerId, segmentId);
			SaveEntity(xref);
			return xref;
		}

		public OfferSegment Retrieve(long offerId, long segmentId)
		{
			return Database.SingleOrDefault<OfferSegment>("select * from LW_CLOfferSegmentXref where OfferId = @0 and SegmentId = @1", offerId, segmentId);
		}

		public IEnumerable<long> RetrieveOffers(long segmentId)
		{
			return Database.Fetch<long>("select OfferId from LW_CLOfferSegmentXref where SegmentId = @0", segmentId);
		}

		public IEnumerable<long> RetrieveSegments(long offerId)
		{
			return Database.Fetch<long>("select SegmentId from LW_CLOfferSegmentXref where OfferId = @0", offerId);
		}

		public void Delete(long offerId, long segmentId)
		{
			Database.Execute("delete from LW_CLOfferSegmentXref where OfferId = @0 and SegmentId = @1", offerId, segmentId);
		}

		public void DeleteByOffer(long id)
		{
			var segments = RetrieveSegments(id);
			if (segments != null)
			{
				foreach (long segmentId in segments)
				{
					Delete(id, segmentId);
				}
			}
		}

		public void DeleteBySegment(long id)
		{
			var offers = RetrieveOffers(id);
			if (offers != null)
			{
				foreach (long offerId in offers)
				{
					Delete(offerId, id);
				}
			}
		}
	}
}
