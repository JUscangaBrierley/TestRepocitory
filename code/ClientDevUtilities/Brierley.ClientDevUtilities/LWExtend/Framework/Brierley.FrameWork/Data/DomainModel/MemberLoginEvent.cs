using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberLoginEvent")]
    public class MemberLoginEvent : LWCoreObjectBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public Int64 Id { get; set; }

        [PetaPoco.Column(Length = 255)]
		public String ProvidedUsername { get; set; }

        [PetaPoco.Column(Length = 255)]
		public String RemoteIPAddress { get; set; }

        [PetaPoco.Column(Length = 255)]
		public String RemoteUserName { get; set; }

        [PetaPoco.Column(Length = 500)]
		public String RemoteUserAgent { get; set; }

        [PetaPoco.Column(Length = 255)]
		public String EventSource { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public MemberLoginEventType EventType { get; set; }		
    }
}
