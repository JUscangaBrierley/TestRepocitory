using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("TestMemberId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_PromoTestMember")]
	public class PromoTestMember : LWCoreObjectBase
	{
        [PetaPoco.Column("TestMemberId", IsNullable = false)]
        public long Id { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public long SetId { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public long IpCode { get; set; }

        [PetaPoco.Column(Length = 100, IsNullable = true)]
		public string TemplateName { get; set; }

        public PromoTestMember()
        {
        }        
	}
}
