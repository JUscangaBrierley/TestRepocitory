using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// A concept associated with a survey.
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Concept_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_Concept")]
    public class SMConcept : LWCoreObjectBase
	{
		#region properties
		/// <summary>
		/// The unique identifier for this concept.
		/// </summary>
        [PetaPoco.Column("Concept_ID", IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The unique identifier for the associated survey.
		/// </summary>
        [PetaPoco.Column("Survey_ID", IsNullable = false)]
        [ForeignKey(typeof(SMSurvey), "ID")]
        public long SurveyID { get; set; }

		/// <summary>
		/// The unique identifier for the associated language.
		/// </summary>
        [PetaPoco.Column("Language_ID", IsNullable = false)]
        [ForeignKey(typeof(SMLanguage), "ID")]
        public long LanguageID { get; set; }

		/// <summary>
		/// The name of the concept.
		/// </summary>
        [PetaPoco.Column(Length = 64, IsNullable = false)]
        public string Name { get; set; }

		/// <summary>
		/// The name of the group of concepts.
		/// </summary>
        [PetaPoco.Column(Length = 64)]
		public string GroupName { get; set; }

		/// <summary>
		/// The content to be displayed for the concept at run time.
		/// </summary>
        [PetaPoco.Column]
        public string Content { get; set; }

		/// <summary>
		/// The constraints for the concept as XML.
		/// </summary>
        [PetaPoco.Column]
        public string ConstraintsXML { get; set; }
		#endregion

		#region constructors
		/// <summary>
		/// Default constructor
		/// </summary>
		public SMConcept()
		{
			ID = -1;
			SurveyID = -1;
			LanguageID = -1;
			Name = string.Empty;
			GroupName = string.Empty;
			Content = string.Empty;
			ConstraintsXML = "<constraints />";
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="existing"></param>
		public SMConcept(SMConcept existing)
		{
			ID = -1;
			SurveyID = existing.SurveyID;
			LanguageID = existing.LanguageID;
			Name = existing.Name;
			GroupName = existing.GroupName;
			Content = existing.Content;
			ConstraintsXML = existing.ConstraintsXML;
		}
		#endregion

		#region public methods
		public long GetSegmentQuota(string segmentPropName, string segmentPropValue)
		{
			long result = -1;
			List<SMSegmentQuota> segmentQuotas = GetSegmentQuotas();
			if (segmentQuotas != null && segmentQuotas.Count > 0)
			{
				foreach (SMSegmentQuota segmentQuota in segmentQuotas)
				{
					if (segmentQuota.PropName == segmentPropName && segmentQuota.PropValue == segmentPropValue) 
					{
						result = segmentQuota.Quota;
						break;
					};
				}
			}
			return result;
		}

		public List<SMSegmentQuota> GetSegmentQuotas()
		{
			List<SMSegmentQuota> result = new List<SMSegmentQuota>();
			Constraint constraint = GetConstraint("segment-max-views");
			if (constraint != null && constraint.Count > 0)
			{
				foreach (ConstraintAttributes attrs in constraint)
				{
					result.Add(new SMSegmentQuota() { 
						PropName = attrs["propname"], 
						PropValue = attrs["propvalue"],
						Quota = StringUtils.FriendlyInt64(attrs["quota"]) 
					});
				}
			}
			return result;
		}

		public void SetSegmentQuotas(List<SMSegmentQuota> segmentQuotas)
		{
			Constraint constraint = new Constraint();
			if (segmentQuotas != null && segmentQuotas.Count > 0)
			{
				foreach (SMSegmentQuota segmentQuota in segmentQuotas)
				{
					ConstraintAttributes attrs = new ConstraintAttributes();
					attrs.Add("propname", segmentQuota.PropName);
					attrs.Add("propvalue", segmentQuota.PropValue);
					attrs.Add("quota", segmentQuota.Quota.ToString());
					constraint.Add(attrs);
				}
			}
			SetConstraint("segment-max-views", constraint);
		}

		public bool IsEqualSegmentQuotas(List<SMSegmentQuota> segmentQuotas)
		{
			bool result = true;
			Constraint constraint = GetConstraint("segment-max-views");
			if (constraint != null && constraint.Count > 0)
			{
				if (segmentQuotas != null && segmentQuotas.Count == constraint.Count)
				{
					for (int index = 0; index < constraint.Count; index++)
					{
						ConstraintAttributes attrs = constraint[index];
						if (segmentQuotas != null && segmentQuotas.Count > index)
						{
							SMSegmentQuota segmentQuota = segmentQuotas[index];
							if (segmentQuota == null
								|| attrs["propname"] != segmentQuota.PropName
								|| attrs["propvalue"] != segmentQuota.PropValue
								|| attrs["quota"] != segmentQuota.Quota.ToString())
							{
								result = false;
								break;
							}
						}
						else
						{
							result = false;
							break;
						}
					}
				}
				else
				{
					result = false;
				}
			}
			else if (segmentQuotas != null && segmentQuotas.Count > 0)
			{
				result = false;
			}
			return result;
		}

		public bool IsConceptExclusiveToGroup()
		{
			bool result = true;

			// no group name
			if (string.IsNullOrEmpty(GroupName))
			{
				result = false;
			}

			// no constraint set
			Constraint constraint = GetConstraint("concept-exclusive-to-group");
			if (constraint == null || constraint.Count < 1)
			{
				result = false;
			}

			return result;
		}

		public void SetConceptExclusiveToGroup(bool value)
		{
			Constraint constraint = GetConstraint("concept-exclusive-to-group");
			if (constraint == null || constraint.Count < 1)
			{
				if (value)
				{
					Constraint constraintValue = new Constraint();
					constraintValue.Add(new ConstraintAttributes());
					SetConstraint("concept-exclusive-to-group", constraintValue);
				}
			}
			else
			{
				if (!value)
				{
					SetConstraint("concept-exclusive-to-group", null);
				}
			}
		}

		public bool CanViewConcept(ServiceConfig config, long respondentID)
		{
			return SegmentQuotaNotExceeded(config, respondentID) && GroupConceptNotAlreadyViewed(config, respondentID);
		}
		#endregion

		#region private methods
		private bool SegmentQuotaNotExceeded(ServiceConfig config, long respondentID)
		{
			using (var surveyService = new SurveyManager(config))
			{
				bool result = true;
				SMRespondent respondent = surveyService.RetrieveRespondent(respondentID);
				if (respondent != null)
				{
					Constraint constraint = GetConstraint("segment-max-views");
					if (constraint != null && constraint.Count > 0)
					{

						foreach (ConstraintAttributes attrs in constraint)
						{
							// respondent must have this property name and value to be in the segment
							if (!respondent.HasProperty(attrs["propname"])) continue;
							string respondentPropValue = respondent.GetProperty(attrs["propname"]);
							if (respondentPropValue != attrs["propvalue"]) continue;

							// check if "null quota"
							long quota = StringUtils.FriendlyInt64(attrs["quota"]);
							if (quota < 1)
							{
								result = false;
								break;
							}

							// check segment quota
							long count = surveyService.RetrieveConceptViewsForSegment(ID, attrs["propname"], attrs["propvalue"]);
							if (count >= quota)
							{
								result = false;
								break;
							}

						}
					}
				}
				return result;
			}
		}

		private bool GroupConceptNotAlreadyViewed(ServiceConfig config, long respondentID)
		{
			bool result = true;
			if (!string.IsNullOrEmpty(GroupName))
			{
				Constraint constraint = GetConstraint("concept-exclusive-to-group");
				if (constraint != null && constraint.Count > 0)
				{
					using (var svc = new SurveyManager(config))
					{
						if (svc.RetrieveConceptViewsForGroup(respondentID, GroupName) > 0)
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		private class Constraint : List<ConstraintAttributes>
		{
		}

		private class ConstraintAttributes : Dictionary<string, string>
		{
		}

		private Constraint GetConstraint(string constraintName)
		{
			Constraint result = new Constraint();
			if (!string.IsNullOrEmpty(ConstraintsXML))
			{
				XElement root = XElement.Parse(ConstraintsXML);
				if (root != null)
				{					
					foreach (XElement constraint in root.Elements(constraintName))
					{
						ConstraintAttributes constraintAttributes = new ConstraintAttributes();
						foreach (XAttribute attribute in constraint.Attributes())
						{
							constraintAttributes.Add(attribute.Name.LocalName, attribute.Value);
						}
						result.Add(constraintAttributes);
					}
				}
			}
			return result;
		}

		private void SetConstraint(string constraintName, Constraint constraintValue)
		{
			if (!string.IsNullOrEmpty(constraintName))
			{
				if (string.IsNullOrEmpty(ConstraintsXML)) ConstraintsXML = "<constraints />";
				XElement root = XElement.Parse(ConstraintsXML);
				if (root != null)
				{
					root.Elements(constraintName).Remove<XElement>();
					if (constraintValue != null)
					{
						foreach (ConstraintAttributes constraintAttributes in constraintValue)
						{
							XElement newElement = new XElement(constraintName);
							foreach (var item in constraintAttributes)
							{
								newElement.Add(new XAttribute(item.Key, item.Value));
							}
							root.Add(newElement);
						}
					}											
					ConstraintsXML = root.ToString();
				}
			}
		}
		#endregion
	}
}
