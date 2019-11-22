using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.CampaignManagement;
using CW = Brierley.FrameWork.CampaignManagement.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Sets an in-memory override of the specified attribute for the currently executing campaign.
	/// </summary>
	/// <example>
	///     Usage : SetCampaignAttributeValue('Attribute Name', 'Attribute Value')
	/// </example>
	/// <remarks>
	/// This function only works against an executing campaign. The new attribute value is only set in memory as an override and is not persisted.
	///</remarks>
	[Serializable]
	[ExpressionContext(Description = "Sets an in-memory override of the specified attribute for the currently executing campaign.",
		DisplayName = "SetCampaignAttributeValue",
		ExcludeContext = ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings | ExpressionApplications.Campaign,
		ExpressionReturns = ExpressionApplications.Strings,

		WizardDescription = "Set a Campaign attribute value",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function,
		EvalRequiresMember = false
		)]

	[ExpressionParameter(Order = 0, Name = "Attribute Name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which campaign attribute?", Helpers = ParameterHelpers.CampaignAttribute)]
	[ExpressionParameter(Order = 1, Name = "Attribute Value", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Set to what value?")]
	public class SetCampaignAttributeValue : UnaryOperation
	{
		private const string _className = "SetCampaignAttributeValue";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private Expression _attributeName = null;
		private Expression _attributeValue = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public SetCampaignAttributeValue()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal SetCampaignAttributeValue(Expression rhs)
			: base("SetCampaignAttributeValue", rhs)
		{
			if (rhs == null)
			{
				throw new Exception("Invalid function call. Wrong number of arguments passed to SetCampaignAttributeValue.");
			}

			ParameterList plist = rhs as ParameterList;

			if (plist.Expressions.Length != 2)
			{
				throw new Exception("Invalid function call. Wrong number of arguments passed to SetCampaignAttributeValue.");
			}
			else
			{
				_attributeName = plist.Expressions[0];
				_attributeValue = plist.Expressions[1];
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "SetCampaignAttributeValue('Attribute Name', 'Attribute Value')";
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			try
			{
				string attributeName = null;
				string attributeValue = null;

				if (contextObject == null || contextObject.Environment == null || !contextObject.Environment.ContainsKey("CurrentCampaign"))
				{
					throw new Exception("Invalid function call. SetCampaignAttributeValue must be invoked in the context of an executing campaign.");
				}

				if (!contextObject.Environment.ContainsKey("OverrideParameters"))
				{
					contextObject.Environment.Add(EnvironmentKeys.OverrideParameters, new Dictionary<string, string>());
				}
				//overkill?
				//else if (!(contextObject.Environment["OverrideParameters"] is Dictionary<string, string>))
				//{
				//	contextObject.Environment["OverrideParameters"] = new Dictionary<string, string>();
				//}

				var overrideParameters = contextObject.Environment["OverrideParameters"] as Dictionary<string, string>;
				if (overrideParameters == null)
				{
					overrideParameters = new Dictionary<string, string>();
					contextObject.Environment["OverrideParameters"] = overrideParameters;
				}
				
				attributeName = (_attributeName.evaluate(contextObject) ?? string.Empty).ToString();
				if (string.IsNullOrEmpty(attributeName))
				{
					throw new Exception("Attribute name evaluates to null or empty");
				}

				attributeValue = (_attributeValue.evaluate(contextObject) ?? string.Empty).ToString();

				
				if (overrideParameters.ContainsKey(attributeName))
				{
					overrideParameters[attributeName] = attributeValue;
				}
				else
				{
					overrideParameters.Add(attributeName, attributeValue);
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "Evaluate", string.Empty, ex);
				throw;
			}
		}

	}
}
