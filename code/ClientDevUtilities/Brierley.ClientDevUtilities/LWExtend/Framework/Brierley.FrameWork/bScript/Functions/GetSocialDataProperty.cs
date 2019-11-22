using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
    /// The GetSocialDataProperty function returns the property value of the social data
	/// </summary>
	/// <example>
    ///     Usage : GetSocialDataProperty('PropertyName')
	/// </example>
	/// <remarks>
	///</remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns the property value of the social data",
        DisplayName = "GetSocialDataProperty",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Objects,
		WizardDescription = "Social Data Property",
		AdvancedWizard = true,		
		EvalRequiresMember = false
	)]

	[ExpressionParameter(Name = "Property", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which property?", Helpers = ParameterHelpers.SocialProperty)]
	public class GetSocialDataProperty : UnaryOperation
	{
		/// <summary>
		/// Public Constructor
		/// </summary>
		public GetSocialDataProperty()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetSocialDataProperty(Expression rhs)
            : base("GetSocialDataProperty", rhs)
		{
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
                return "GetSocialDataProperty('PropertyName')";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
            if (!contextObject.Environment.ContainsKey("SocialCampaignData"))
            {
                throw new LWBScriptException("GetSocialDataProperty must be invoked with a social campaign data object.");
            }

            SocialCampaignData data = (SocialCampaignData)contextObject.Environment["SocialCampaignData"];

			string propertyName = GetRight().evaluate(contextObject).ToString();
			object propertyValue = null;
            PropertyInfo pi = data.GetType().GetProperty(propertyName);
            if (pi != null)
            {
                propertyValue = pi.GetValue(data, null);
            }
            			
			return propertyValue;
		}
	}
}
