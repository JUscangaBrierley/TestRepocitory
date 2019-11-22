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
	[PetaPoco.TableName("LW_CLGlobal")]
	public class Global : LWCoreObjectBase
	{
		[PetaPoco.Column]
		public long Id { get; set; }

		[PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = true)]
        public string Name { get; set; }

		[PetaPoco.Column(Length = 500)]
		public string Description { get; set; }

		public Global Clone()
		{
			var cloned = new Global();
			cloned.Name = Name;
			cloned.Description = Description;
			return cloned;
		}
	}
}
