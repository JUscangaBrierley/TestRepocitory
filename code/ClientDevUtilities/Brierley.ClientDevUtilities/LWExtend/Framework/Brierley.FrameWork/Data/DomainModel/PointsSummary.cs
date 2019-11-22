using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_PointsSummary")]
    [UniqueIndex(ColumnName = "MemberId,PointEvent,PointType")]
    public class PointsSummary : LWCoreObjectBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public long MemberId { set; get; }

        [PetaPoco.Column(Length = 150, IsNullable = false)]
		public string PointEvent { set; get; }

        [PetaPoco.Column(Length = 150, IsNullable = false)]
		public string PointType { set; get; }

        [PetaPoco.Column(IsNullable = false)]
		public decimal Earned { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public decimal Balance { get; set; }              
    }
}
