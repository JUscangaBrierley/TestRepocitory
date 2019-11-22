using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// A decision state within a survey's state diagram.
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Decision_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_Decision")]
    public class SMDecision : LWCoreObjectBase
	{
		#region fields
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private const string _className = "SMDecision";
		#endregion

		#region properties
		/// <summary>
		/// The unique identifier for this decision.
		/// </summary>
        [PetaPoco.Column("Decision_ID", IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The unique identifier for the associated state in the state diagram.
		/// </summary>
        [PetaPoco.Column("State_ID", IsNullable = false)]
        [ForeignKey(typeof(SMState), "ID")]
        public long StateID { get; set; }

		/// <summary>
		/// An XML string containing a set of conditions.  Each condition is a valid BScript expression.  
		/// Each BScript expression must return a boolean value, and that value
		/// is used to determine flow in the state diagram at runtime.
		/// </summary>
        [PetaPoco.Column]
        public string Expression { get; set; }
		#endregion

		#region constructors
		public SMDecision()
		{
			ID = -1;
			StateID = -1;
			Expression = string.Empty;
		}
		#endregion

		#region public methods
		public long NumConditions()
		{
			const string methodName = "NumConditions";

			long result = 0;
			if (!string.IsNullOrEmpty(Expression))
			{
				try
				{
					// <expressions>
					//    <expression description="abc">bscript</expression>
					//    <expression description="def">bscript</expression>
					//</expressions>
					XElement root = XElement.Parse(Expression);
					if (root != null)
					{
						if (root.Name == "expressions")
						{
							var conditions = root.Elements("expression");
							if (conditions != null)
							{
								foreach (var condition in conditions)
								{
									result++;
								}
							}
							else
							{
								_logger.Error(_className, methodName, "Expression has no conditions: " + Expression);
							}
						}
						else
						{
							_logger.Error(_className, methodName, "Expression missing 'expressions' tag: " + Expression);
						}
					}
					else
					{
						_logger.Error(_className, methodName, "Expression is not parseable: " + Expression);
					}
				}
				catch (Exception ex)
				{
					// invalid expression, so no conditions
					_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				}
			}
			return result;
		}

		public string GetCondition(int index)
		{
			const string methodName = "GetCondition";

			string result = string.Empty;
			if (!string.IsNullOrEmpty(Expression))
			{
				try
				{
					// <expressions>
					//    <expression description="abc">bscript</expression>
					//    <expression description="def">bscript</expression>
					//</expressions>
					XElement root = XElement.Parse(Expression);
					if (root != null)
					{
						if (root.Name == "expressions")
						{
							var conditions = root.Elements("expression");
							if (conditions != null)
							{
								int count = 0;
								foreach (var condition in conditions)
								{
									if (count == index)
									{
										result = condition.Value;
										break;
									}
									count++;
								}
							}
							else
							{
								_logger.Error(_className, methodName, "Expression has no conditions: " + Expression);
							}
						}
						else
						{
							_logger.Error(_className, methodName, "Expression missing 'expressions' tag: " + Expression);
						}
					}
					else
					{
						_logger.Error(_className, methodName, "Expression is not parseable: " + Expression);
					}
				}
				catch (Exception ex)
				{
					// invalid expression, so no conditions
					_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				}
			}
			return result;
		}
		#endregion
	}
}
