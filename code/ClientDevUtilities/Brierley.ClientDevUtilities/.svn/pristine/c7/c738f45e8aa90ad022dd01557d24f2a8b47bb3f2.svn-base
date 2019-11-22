using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_MemberMobileEvent")]
    public class MemberMobileEvent : LWCoreObjectBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long MemberId { get; set; }
        [PetaPoco.Column(Length=25, IsNullable=false, PersistEnumAsString=true)]
        public MemberMobileEventActionType Action { get; set; }
        [PetaPoco.Column]
        public double? Latitude { get; set; }
        [PetaPoco.Column]
        public double? Longitude { get; set; }
        [PetaPoco.Column]
        public double? Radius { get; set; }
        [PetaPoco.Column]
        public long? StoreDefId { get; set; }
    }
}
