using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("RespondentList_Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_RespondentList")]
    public class SMRespondentList : LWCoreObjectBase
	{
		#region properties
        [PetaPoco.Column("RespondentList_Id", IsNullable = false)]
        public long ID { get; set; }
        [PetaPoco.Column(Length = 35, IsNullable = false)]
        public string BatchID { get; set; }
        [PetaPoco.Column(Length = 255, PersistEnumAsString = true, IsNullable = false)]
        public SMRespondentListStatusEnum Status { get; set; }
        [PetaPoco.Column]
        public string Progress { get; set; }
        [PetaPoco.Column]
        public string LastError { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long FieldListID { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long SurveyID { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long JobID { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long RunID { get; set; }
        [PetaPoco.Column]
        public string JobProperties { get; set; }
		#endregion

		#region constructors
		public SMRespondentList()
		{
			ID = -1;
			Status = SMRespondentListStatusEnum.NotInitialized;
			JobID = -1;
			RunID = -1;
		}

		public SMRespondentList(SMRespondentList existing)
		{
			ID = -1;
			BatchID = existing.BatchID;
			Status = existing.Status;
			Progress = existing.Progress;
			LastError = existing.LastError;
			FieldListID = existing.FieldListID;
			SurveyID = existing.SurveyID;
			JobID = existing.JobID;
			RunID = existing.RunID;
			JobProperties = existing.JobProperties;
		}
		#endregion

		#region public methods
		public Dictionary<string, object> GetJobProperties()
		{
			Dictionary<string, object> result = new Dictionary<string, object>();
			if (!string.IsNullOrWhiteSpace(JobProperties))
			{
				result = JsonConvert.DeserializeObject<Dictionary<string, object>>(JobProperties);
			}
			return result;
		}

		public void SetJobProperties(Dictionary<string, object> jobProperties)
		{
			if (jobProperties == null)
			{
				jobProperties = new Dictionary<string, object>();
			}
			JobProperties = JsonConvert.SerializeObject(jobProperties);
		}
		#endregion
	}
}
