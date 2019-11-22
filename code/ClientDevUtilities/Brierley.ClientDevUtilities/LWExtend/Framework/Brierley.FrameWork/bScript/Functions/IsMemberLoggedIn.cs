using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Used to determine if the member is currently logged in.
	/// </summary>
	/// <example>
	///     Usage: IsMemberLoggedIn()
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns true if a member is logged in.",
		DisplayName = "IsMemberLoggedIn",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Booleans,
		WizardDescription = "Is Member Logged In?",
		AdvancedWizard = true)]
	public class IsMemberLoggedIn : UnaryOperation
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_WEBFRAMEWORK);
		private const string _className = "IsMemberLoggedIn";

		/// <summary>
		/// Default Constructor
		/// </summary>
		public IsMemberLoggedIn() : base ("IsMemberLoggedIn", null)
		{
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "IsMemberLoggedIn()";
			}
		}

		/// <summary>
		/// Performs the operation defined by this function
		/// </summary>
		/// <param name="contextObject">An instance of ContextObject</param>
		/// <returns>An object representing the result of the evaluation</returns>
		public override object evaluate(ContextObject contextObject)
		{
			bool result = false;
			if (contextObject.Owner != null 
				&& contextObject.Owner is Member 
				&& HttpContext.Current != null
				&& HttpContext.Current.Request != null
				&& HttpContext.Current.Request.IsAuthenticated)
			{
				result = true;
			}
			return result;
		}
	}
}
