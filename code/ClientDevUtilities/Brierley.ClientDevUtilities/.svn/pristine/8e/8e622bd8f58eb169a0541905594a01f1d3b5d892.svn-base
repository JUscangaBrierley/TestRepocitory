using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Contains the information related to a survey.
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Survey_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_Survey")]
    public class SMSurvey : LWCoreObjectBase
	{
		#region fields
		private const string _className = "SMSurvey";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private long _templateID;
		private long _documentID;
		#endregion

		#region properties
		/// <summary>
		/// A unique identifier for the survey.
		/// </summary>
        [PetaPoco.Column("Survey_ID", IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The name of the survey.
		/// </summary>
        [PetaPoco.Column("Survey_Name", Length = 64, IsNullable = false)]
        public String Name { get; set; }

		/// <summary>
		/// Gets or sets the folder id for the current survey.
		/// </summary>
        [PetaPoco.Column]
        public long? FolderId { get; set; }

		/// <summary>
		/// A description of the survey.
		/// </summary>
        [PetaPoco.Column("Survey_Description", Length = 500)]
        public String Description { get; set; }

		/// <summary>
		/// The date/time upon which the survey becomes effective.
		/// </summary>
        [PetaPoco.Column("Effective_Date", IsNullable = false)]
        public DateTime EffectiveDate { get; set; }

		/// <summary>
		/// The date/time upon which the survey expires.
		/// </summary>
        [PetaPoco.Column("Expiration_Date", IsNullable = false)]
        public DateTime ExpirationDate { get; set; }

        [PetaPoco.Column]
        public string ConstraintsXML { get; set; }

		/// <summary>
		/// The associated Email to be used for sending a "Thank You" email message.
		/// </summary>
        [PetaPoco.Column("Email_ID", IsNullable = false)]
        public long EmailID { get; set; }

		/// <summary>
		/// Type of survey.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public SurveyType SurveyType { get; set; }

		/// <summary>
		/// The type of survey audience.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public SurveyAudience SurveyAudience { get; set; }

		/// <summary>
		/// A number indicating the relative display order to use when showing this survey.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long DisplayOrder { get; set; }

		/// <summary>
		/// The ID for the rule to use when the survey is completed successfully.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long SurveyCompleteRuleId { get; set; }

		/// <summary>
		/// The ID for the rule to use when the survey is completed as a "terminate and tally".
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long SurveyTerminateAndTallyRuleId { get; set; }

		/// <summary>
		/// The ID for a template associated with this survey.
		/// </summary>
        [PetaPoco.Column("Template_ID", IsNullable = false)]
        public long TemplateID
		{
			get { return _templateID; }
			set
			{
				_templateID = value;
				// nullable value will default to 0, so make it -1
				if (_templateID == 0) _templateID = -1;
			}
		}

		/// <summary>
		/// The ID for a document associated with this survey.
		/// </summary>
        [PetaPoco.Column("Document_ID", IsNullable = false)]
        public long DocumentID
		{
			get { return _documentID; }
			set
			{
				_documentID = value;
				// nullable value will default to 0, so make it -1
				if (_documentID == 0) _documentID = -1;
			}
		}

		/// <summary>
		/// Status of survey.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public SurveyStatus SurveyStatus { get; set; }
		#endregion

		#region constructors
		/// <summary>
		/// Initializes a new instance of the Survey class
		/// </summary>
		public SMSurvey()
		{
			ID = -1;
			Name = string.Empty;
			Description = string.Empty;
			EffectiveDate = DateTime.Now;
			ExpirationDate = DateTimeUtil.MaxValue;
			ConstraintsXML = "<constraints><absolute-quota enabled=\"false\" quota=\"0\" /></constraints>";
			EmailID = -1;
			SurveyType = SurveyType.General;
			SurveyAudience = SurveyAudience.PreSelected;
			DisplayOrder = -1;
			SurveyCompleteRuleId = -1;
			SurveyTerminateAndTallyRuleId = -1;
			TemplateID = -1;
			DocumentID = -1;
			SurveyStatus = SurveyStatus.Design;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="existing"></param>
		public SMSurvey(SMSurvey existing)
			: base()
		{
			ID = -1;
			Name = existing.Name;
			Description = existing.Description;
			EffectiveDate = existing.EffectiveDate;
			ExpirationDate = existing.ExpirationDate;
			ConstraintsXML = StringUtils.FriendlyString(existing.ConstraintsXML, "<constraints><absolute-quota enabled=\"false\" quota=\"0\" /></constraints>");
			EmailID = existing.EmailID;
			SurveyType = existing.SurveyType;
			SurveyAudience = existing.SurveyAudience;
			DisplayOrder = existing.DisplayOrder;
			SurveyCompleteRuleId = existing.SurveyCompleteRuleId;
			SurveyTerminateAndTallyRuleId = existing.SurveyTerminateAndTallyRuleId;
			TemplateID = existing.TemplateID;
			DocumentID = existing.DocumentID;
			SurveyStatus = SurveyStatus.Design;
		}
		#endregion

		#region public methods
		/// <summary>
		/// The states associated with this survey.
		/// </summary>
		public SMStateCollection GetStates(ServiceConfig config)
		{
			SMStateCollection states = new SMStateCollection(config, ID);
			return states;
		}

		public bool AutoCreateRespondents()
		{
			switch (SurveyAudience)
			{
				case SurveyAudience.OpenToAnyMember:
				case SurveyAudience.OpenToEveryone:
					return true;
			}
			return false;
		}

		public bool CanReviewQuestions()
		{
			switch (SurveyType)
			{
				case SurveyType.Profile:
					return true;
			}
			return false;
		}

		public bool CanPreviewSurvey()
		{
			switch (SurveyStatus)
			{
				case SurveyStatus.Design:
				case SurveyStatus.Active:
					return true;
			}
			return false;
		}

		public bool CanEditSurvey()
		{
			switch (SurveyStatus)
			{
				case SurveyStatus.Design:
					return true;
			}
			return false;
		}

		public bool CanPublishSurvey()
		{
			switch (SurveyStatus)
			{
				case SurveyStatus.Design:
					return true;
			}
			return false;
		}

		public bool CanUnpublishSurvey()
		{
			switch (SurveyStatus)
			{
				case SurveyStatus.Active:
					return true;
			}
			return false;
		}

		public bool CanCloseSurvey()
		{
			switch (SurveyStatus)
			{
				case SurveyStatus.Active:
					return true;
			}
			return false;
		}

		public bool CanDeleteSurvey()
		{
			switch (SurveyStatus)
			{
				case SurveyStatus.Design:
				case SurveyStatus.Closed:
					return true;
			}
			return false;
		}

		public bool CanTakeSurvey()
		{
			switch (SurveyStatus)
			{
				case SurveyStatus.Active:
					return true;
			}
			return false;
		}

		public string GetSurveyTypeName()
		{
			return Enum.GetName(typeof(SurveyType), SurveyType);
		}

		public string GetSurveyAudienceName()
		{
			return Enum.GetName(typeof(SurveyAudience), SurveyAudience);
		}

		public string GetSurveyStatusName()
		{
			return Enum.GetName(typeof(SurveyStatus), SurveyStatus);
		}

		public long GetAbsoluteQuota()
		{
			long result = -1;
			Constraint constraint = GetConstraint("absolute-quota");
			if (constraint != null && constraint.Count > 0)
			{
				if (StringUtils.FriendlyBool(constraint[0]["enabled"], false))
				{
					result = StringUtils.FriendlyInt64(constraint[0]["quota"]);
				}
			}
			return result;
		}

		public void SetAbsoluteQuota(long quota)
		{
			bool enabled = quota > -1;

			Constraint constraint = new Constraint();
			ConstraintAttributes attrs = new ConstraintAttributes();
			attrs.Add("enabled", enabled.ToString());
			attrs.Add("quota", quota.ToString());
			constraint.Add(attrs);

			SetConstraint("absolute-quota", constraint);
		}

		public List<SMSegmentQuota> GetSegmentQuotas()
		{
			List<SMSegmentQuota> result = new List<SMSegmentQuota>();
			Constraint constraint = GetConstraint("segment-max-views");
			if (constraint != null && constraint.Count > 0)
			{
				foreach (ConstraintAttributes attrs in constraint)
				{
					result.Add(new SMSegmentQuota()
					{
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

		public bool CanViewSurvey(ServiceConfig config, SMRespondent respondent)
		{
			string dummy1 = null;
			string dummy2 = null;

			return
				IsSurveyEffective()
				&& IsAbsoluteQuotaNotExceeded(config)
				&& IsSegmentQuotaNotExceeded(config, respondent, ref dummy1, ref dummy2);
		}

		public bool IsSurveyEffective()
		{
			const string methodName = "IsSurveyEffective";
			if (EffectiveDate > DateTime.Now)
			{
				_logger.Debug(_className, methodName, string.Format("Effective date not yet reached for survey '{0}' ({1})", Name, ID));
				return false;
			}
			if (ExpirationDate <= DateTime.Now)
			{
				_logger.Debug(_className, methodName, string.Format("Expiration date passed for survey '{0}' ({1})", Name, ID));
				return false;
			}
			return true;
		}

		public bool IsAbsoluteQuotaNotExceeded(ServiceConfig config)
		{
			const string methodName = "IsAbsoluteQuotaNotExceeded";
			using (var svc = new SurveyManager(config))
			{
				bool result = true;
				long quota = GetAbsoluteQuota();
				if (quota == 0)
				{
					_logger.Debug(_className, methodName, string.Format("Absolute quota == 0 for survey '{0}' ({1})", Name, ID));
					result = false;
				}
				else if (quota > 0 && svc.IsQuotaMet(ID, quota))
				{
					_logger.Debug(_className, methodName, string.Format("Absolute quota has been met for survey '{0}' ({1})", Name, ID));
					result = false;
				}
				return result;
			}
		}

		public bool IsSegmentQuotaNotExceeded(ServiceConfig config, SMRespondent respondent, ref string propName, ref string propValue)
		{
			const string methodName = "IsSegmentQuotaNotExceeded";
			bool result = true;
			Constraint constraint = GetConstraint("segment-max-views");
			if (constraint != null && constraint.Count > 0)
			{
				using (var svc = new SurveyManager(config))
				{
					foreach (ConstraintAttributes attrs in constraint)
					{
						string respondentPropName = attrs["propname"];
						string respondentPropValue = StringUtils.FriendlyString(respondent.GetProperty(respondentPropName));
						if (respondentPropValue == attrs["propvalue"])
						{
							long quota = StringUtils.FriendlyInt64(attrs["quota"]);
							if (quota < 1)
							{
								propName = respondentPropName;
								propValue = respondentPropValue;
								_logger.Debug(_className, methodName, string.Format("Segment quota ({0} == {1}) < 1 for survey '{2}' ({3})", respondentPropName, respondentPropValue, Name, ID));
								result = false;
								break;
							}
							long count = svc.NumCompletesForSegment(ID, respondentPropName, respondentPropValue);
							if (count >= quota)
							{
								propName = respondentPropName;
								propValue = respondentPropValue;
								_logger.Debug(_className, methodName, string.Format("Segment quota ({0} == {1}) has been met ({2} > {3}) for survey '{4}' ({5})", respondentPropName, respondentPropValue, count, quota, Name, ID));
								result = false;
								break;
							}
						}
					}
				}
			}
			return result;
		}
		#endregion

		#region private methods
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