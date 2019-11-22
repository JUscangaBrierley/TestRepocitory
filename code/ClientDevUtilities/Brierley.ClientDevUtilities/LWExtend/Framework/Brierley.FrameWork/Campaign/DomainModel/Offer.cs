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
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLOffer")]
	public class Offer : LWCoreObjectBase
	{
		[PetaPoco.Column]
		public long Id { get; set; }

		[PetaPoco.Column]
        [ForeignKey(typeof(Campaign), "Id")]
		public long CampaignId { get; set; }

		[PetaPoco.Column(Length = 255, IsNullable = false)]
		public string OfferCode { get; set; }

		public List<CampaignAttribute> Attributes { get; set; }
		public List<long> Segments { get; set; }

		public Offer()
		{
			Segments = new List<long>();
			Attributes = new List<CampaignAttribute>();
		}
	}
}
