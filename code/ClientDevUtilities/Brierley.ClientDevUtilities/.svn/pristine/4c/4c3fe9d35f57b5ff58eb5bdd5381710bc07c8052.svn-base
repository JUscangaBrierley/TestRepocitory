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
    /// The IsSocialPublisher function returns true or false if the publisher matches the parameter
	/// </summary>
	/// <example>
    ///     Usage : IsSocialPublisher('PublisherName')
	/// </example>
	/// <remarks>
	///</remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns true/false if the publisher matches",
        DisplayName = "IsSocialPublisher",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Objects,
		WizardDescription = "Is Social Publisher",
		AdvancedWizard = true,		
		EvalRequiresMember = false
	)]

	[ExpressionParameter(Name = "Property", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which social publisher?", Helpers = ParameterHelpers.SocialPublisher)]
	public class IsSocialPublisher : UnaryOperation
	{
		/// <summary>
		/// Public Constructor
		/// </summary>
		public IsSocialPublisher()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal IsSocialPublisher(Expression rhs)
			: base("IsSocialPublisher", rhs)
		{
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
                return "IsSocialPublisher('PublisherName')";
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
                throw new LWBScriptException("IsSocialPublisher must be invoked with a social campaign data object.");
            }

            SocialCampaignData data = (SocialCampaignData)contextObject.Environment["SocialCampaignData"];

			string publisherName = GetRight().evaluate(contextObject).ToString();
            SocialNetworkProviderType publisher = (SocialNetworkProviderType)Enum.Parse(typeof(SocialNetworkProviderType), publisherName);

            return data.Publisher == publisher;			
		}
	}
}
