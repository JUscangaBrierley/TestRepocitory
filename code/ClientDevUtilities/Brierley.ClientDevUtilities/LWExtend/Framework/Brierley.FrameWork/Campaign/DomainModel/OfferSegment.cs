using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("OfferId, SegmentId", autoIncrement = false)]
	[PetaPoco.TableName("LW_CLOfferSegmentXref")]
	public class OfferSegment : LWCoreObjectBase
	{
		[PetaPoco.Column]
        [ForeignKey(typeof(Offer), "Id")]
		public long OfferId { get; set; }

		[PetaPoco.Column]
        [ForeignKey(typeof(Segment), "Id")]
		public long SegmentId { get; set; }

		public OfferSegment()
		{
		}

		public OfferSegment(long offerId, long segmentId)
		{
			OfferId = offerId;
			SegmentId = segmentId;
		}

		public override bool Equals(object obj)
		{
			OfferSegment otherInstance = obj as OfferSegment;
			if (otherInstance != null)
			{
				return otherInstance.OfferId == OfferId && otherInstance.SegmentId == SegmentId;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
