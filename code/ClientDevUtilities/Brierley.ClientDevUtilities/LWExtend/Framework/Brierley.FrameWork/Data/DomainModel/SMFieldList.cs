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
    [PetaPoco.PrimaryKey("FieldList_Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_FieldList")]
    public class SMFieldList : LWCoreObjectBase
	{
		#region properties
        [PetaPoco.Column("FieldList_Id", IsNullable = false)]
        public long ID { get; set; }
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string FieldListName { get; set; }
        [PetaPoco.Column]
        public string FieldNames { get; set; }
		#endregion

		#region constructors
		public SMFieldList()
		{
			ID = -1;
		}

		public SMFieldList(SMFieldList existing)
		{
			ID = -1;
			FieldListName = existing.FieldListName;
			FieldNames = existing.FieldNames;
		}
		#endregion

		#region public methods
		public List<string> GetFieldNames()
		{
			List<string> result = new List<string>();
			if (!string.IsNullOrWhiteSpace(FieldNames))
			{
				result = JsonConvert.DeserializeObject<List<string>>(FieldNames);
			}
			return result;
		}

		public void SetFieldNames(List<string> fieldNames)
		{
			if (fieldNames == null)
			{
				fieldNames = new List<string>();
			}
			FieldNames = JsonConvert.SerializeObject(fieldNames);
		}
		#endregion
	}
}
