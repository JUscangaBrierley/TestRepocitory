//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_CSLoginEvent")]
    public class CSLoginEvent : LWCoreObjectBase
    {
        #region Properties
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string ProvidedUsername { get; set; }
        [PetaPoco.Column(Length = 255)]
		public string ProvidedPassword { get; set; }
        [PetaPoco.Column(Length = 255)]
		public string RemoteIPAddress { get; set; }
        [PetaPoco.Column(Length = 255)]
		public string RemoteUserName { get; set; }
        [PetaPoco.Column(Length = 500)]
		public string RemoteUserAgent { get; set; }
        [PetaPoco.Column(Length = 255)]
		public string EventSource { get; set; }
        [PetaPoco.Column(IsNullable = false)]
		public CSLoginEventType EventType { get; set; }		
        #endregion
    }
}
