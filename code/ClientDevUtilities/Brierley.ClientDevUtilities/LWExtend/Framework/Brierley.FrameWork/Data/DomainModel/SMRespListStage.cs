using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Newtonsoft.Json;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("RespListStage_Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_RespListStage")]
	public class SMRespListStage : LWCoreObjectBase
	{
		#region properties
        [PetaPoco.Column("RespListStage_Id", IsNullable = false)]
        public long ID { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        [ColumnIndex]
        public long RespListID { get; set; }
        [PetaPoco.Column]
        public byte[] RespondentList { get; set; }
		#endregion

		#region constructors
		public SMRespListStage()
		{
			ID = -1;
		}

		public SMRespListStage(SMRespListStage existing)
		{
			ID = -1;
			RespListID = existing.RespListID;
			RespondentList = existing.RespondentList;
		}
		#endregion
	}
}
