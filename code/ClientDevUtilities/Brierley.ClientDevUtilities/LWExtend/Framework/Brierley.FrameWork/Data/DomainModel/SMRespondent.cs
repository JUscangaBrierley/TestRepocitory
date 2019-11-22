using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// Someone who can take a particular survey.
    /// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Respondent_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_Respondent")]
    [ColumnIndex(ColumnName = "MTouch,LanguageID")]
    [ColumnIndex(ColumnName = "MTouch,SurveyID,LanguageID")]
    public class SMRespondent : LWCoreObjectBase
	{
		#region properties
		/// <summary>
		/// A unique identifier.
		/// </summary>
		[PetaPoco.Column("Respondent_ID", IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The mtouch code.
		/// </summary>
        [PetaPoco.Column(Length = 128)]
        public string MTouch { get; set; }

		/// <summary>
		/// The IPCode.
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
        public long IPCode { get; set; }

		/// <summary>
		/// The unique identifier for the related survey.
		/// </summary>
        [PetaPoco.Column("Survey_ID", IsNullable = false)]
        [ForeignKey(typeof(SMSurvey), "ID")]
        public long SurveyID { get; set; }

		/// <summary>
		/// The unique identifier for the related language.
		/// </summary>
        [PetaPoco.Column("Language_ID", IsNullable = false)]
        [ForeignKey(typeof(SMLanguage), "ID")]
        public long LanguageID { get; set; }

		/// <summary>
		/// The date/time the respondent started the survey.
		/// </summary>
        [PetaPoco.Column("Start_Date")]
        public DateTime? StartDate { get; set; }

		/// <summary>
		/// The date/time the respondent completed the survey.
		/// </summary>
        [PetaPoco.Column("Complete_Date")]
        public DateTime? CompleteDate { get; set; }

        /// <summary>
        /// Whether this respondent has been terminated from a survey due to a skip pattern.
        /// This field is the one stored in the database and should be 'T' or 'F'.
        /// </summary>
        [PetaPoco.Column("Skipped", Length=1)]
        public string Skipped_Char { get; set; }

		/// <summary>
		/// Whether this respondent has been terminated from a survey due to a skip pattern.
		/// </summary>
        public bool Skipped
        {
            get
            {
                return !string.IsNullOrEmpty(Skipped_Char) && Skipped_Char.ToUpper().Equals("T");
            }
            set
            {
                Skipped_Char = value ? "T" : "F";
            }
        }

		/// <summary>
		/// Any properties (as XML) associated with this respondent.
		/// </summary>
        [PetaPoco.Column]
        public string PropertiesXML { get; set; }

		/// <summary>
		/// Used to manage a group of respondents
		/// </summary>
        [PetaPoco.Column]
        [ColumnIndex]
        public long? RespListID { get; set; }
		#endregion

		#region constructor
		public SMRespondent()
		{
			ID = -1;
			MTouch = string.Empty;
			IPCode = -1;
			SurveyID = -1;
			LanguageID = -1;
			Skipped = false;
			PropertiesXML = "<properties/>";
		}
		#endregion

		#region public methods
		public bool HasProperty(string propertyName)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(PropertiesXML))
			{
				XElement root = XElement.Parse(PropertiesXML);
				if (root != null)
				{
					foreach (XElement prop in root.Elements("property"))
					{
						if (StringUtils.FriendlyXAttribute(prop.Attribute("name")) == propertyName)
						{
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}

		public string GetProperty(string propertyName)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(PropertiesXML))
			{
				XElement root = XElement.Parse(PropertiesXML);
				if (root != null)
				{
					foreach (XElement prop in root.Elements("property"))
					{
						if (StringUtils.FriendlyXAttribute(prop.Attribute("name")) == propertyName)
						{
							result = StringUtils.FriendlyXAttribute(prop.Attribute("value"));
							break;
						}
					}
				}
			}
			return result;
		}

		public void SetProperty(string propertyName, string propertyValue)
		{
			if (!string.IsNullOrEmpty(propertyName))
			{
				XElement root = XElement.Parse(PropertiesXML);
				if (root != null)
				{
					bool found = false;
					foreach (XElement prop in root.Elements("property"))
					{
						if (StringUtils.FriendlyXAttribute(prop.Attribute("name")) == propertyName)
						{
							prop.Attribute("value").SetValue(propertyValue);
							break;
						}
					}
					if (!found)
					{
						XElement newElement = new XElement("property");
						newElement.Add(new XAttribute("name", propertyName));
						newElement.Add(new XAttribute("value", propertyValue));
						root.Add(newElement);
					}
					PropertiesXML = root.ToString();
				}
			}
		}
		#endregion
	}
}
