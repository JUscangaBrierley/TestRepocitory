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
	[PetaPoco.TableName("LW_CLGlobalAttribute")]
	public class GlobalAttribute : LWCoreObjectBase
	{
		[PetaPoco.Column]
		public long Id { get; set; }

		[PetaPoco.Column]
        [ForeignKey(typeof(Global), "Id")]
        [ColumnIndex]
		public long GlobalId { get; set; }

		[PetaPoco.Column(Length = 50)]
		public string AttributeName { get; set; }

		[PetaPoco.Column(Length = 500)]
		public string AttributeValue { get; set; }

		public GlobalAttribute Clone()
		{
			var cloned = new GlobalAttribute();
			cloned.AttributeName = AttributeName;
			cloned.AttributeValue = AttributeValue;
			return cloned;
		}
	}
}
