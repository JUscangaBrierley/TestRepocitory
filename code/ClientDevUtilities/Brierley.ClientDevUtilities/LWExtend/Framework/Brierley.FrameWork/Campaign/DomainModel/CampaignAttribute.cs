using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLCampaignAttribute")]
    [ColumnIndex(ColumnName = "CampaignId,AttributeId,OfferId,SegmentId")]
    public class CampaignAttribute : LWCoreObjectBase
	{
		/// <summary>
		/// gets or sets the id of the attribute
		/// </summary>
		[PetaPoco.Column]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the campaign id of the attribute.
		/// </summary>
        [PetaPoco.Column]
        [ForeignKey(typeof(Campaign), "Id")]
        public long CampaignId { get; set; }

		/// <summary>
		/// Gets or sets the offer id of the attribute
		/// </summary>
		/// <remarks>
		/// Unless the attribute is an offer type, then this will be null. Otherwise, it points to 
		/// the offer that the attribute value belongs to.
		/// </remarks>
		[PetaPoco.Column]
        [ForeignKey(typeof(Offer), "Id")]
		public long? OfferId { get; set; }

		/// <summary>
		/// Gets or sets the segment id of the attribute
		/// </summary>
		/// <remarks>
		/// Unless the attribute is a segment type, then this will be null. Otherwise, it points to 
		/// the segment that the attribute value belongs to.
		/// </remarks>
		[PetaPoco.Column]
        [ForeignKey(typeof(Segment), "Id")]
		public long? SegmentId { get; set; }

		[PetaPoco.Column]
        [ForeignKey(typeof(Attribute), "Id")]
		public long AttributeId { get; set; }

		[PetaPoco.Column(Length = 500, IsNullable = false)]
		public string AttributeValue { get; set; }
		

		public override bool Equals(object obj)
		{
			CampaignAttribute otherInstance = obj as CampaignAttribute;
			if (otherInstance != null)
			{
				return otherInstance.AttributeId == AttributeId && otherInstance.CampaignId == CampaignId;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
