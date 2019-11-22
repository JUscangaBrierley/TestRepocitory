using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
    /// The IsSocialSentiment function returns true or false if the social sentiment matches the parameter
	/// </summary>
	/// <example>
    ///     Usage : IsSocialSentiment('Sentiment')
	/// </example>
	/// <remarks>
	///</remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns true/false if the sentiment matches",
        DisplayName = "IsSocialSentiment",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Objects,
		WizardDescription = "Is Social Sentiment",
		AdvancedWizard = true,		
		EvalRequiresMember = false
	)]

	[ExpressionParameter(Name = "Property", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which social sentiment?", Helpers = ParameterHelpers.SocialSentiment)]
	public class IsSocialSentiment : UnaryOperation
	{
		/// <summary>
		/// Public Constructor
		/// </summary>
		public IsSocialSentiment()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal IsSocialSentiment(Expression rhs)
            : base("IsSocialSentiment", rhs)
		{
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
                return "IsSocialSentiment('Sentiment')";
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
                throw new LWBScriptException("IsSocialSentiment must be invoked with a social campaign data object.");
            }

            SocialCampaignData data = (SocialCampaignData)contextObject.Environment["SocialCampaignData"];

			string sentimentStr = GetRight().evaluate(contextObject).ToString();
            SocialSentiment sentiment = (SocialSentiment)Enum.Parse(typeof(SocialSentiment), sentimentStr);

            return data.Sentiment == sentiment;			
		}
	}
}
