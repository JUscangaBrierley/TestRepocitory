using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CacheRefresh")]
	public class CacheRefresh
	{
		[PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		[PetaPoco.Column(Length = 50)]
		public string InitiatedBy { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		public CacheRefresh()
		{
			CreateDate = DateTime.Now;
		}
	}
}
