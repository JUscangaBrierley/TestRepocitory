using System;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The IsEmailSuppressed function will return true if the specified email address is currently being suppressed.
	/// </summary>
	/// <example>
	///     Usage : IsEmailSuppressed(emailAddress)
	/// </example>
	/// <remarks>
	/// </remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns true if the specified email address is currently being suppressed.",
		DisplayName = "IsEmailSuppressed",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Member,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Booleans,

		WizardDescription = "Is an email address currently being suppressed?",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function,
		EvalRequiresMember = false)]

	[ExpressionParameter(Order = 0, Name = "Email Address", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which email address?")]
	public class IsEmailSuppressed : UnaryOperation
	{
		public new string Syntax
		{
			get
			{
				return "IsEmailSuppressed(emailAddress)";
			}
		}

		public IsEmailSuppressed()
		{
		}

		internal IsEmailSuppressed(Expression rhs)
			: base("IsEmailSuppressed", rhs)
		{
		}

		public override object evaluate(ContextObject contextObject)
		{
			try
			{
				string email = GetRight().evaluate(contextObject).ToString();
				using (var svc = LWDataServiceUtil.EmailServiceInstance())
				{
					return svc.IsEmailSuppressed(email);
				}
			}
			catch (Exception)
			{
				throw new CRMException("Illegal Expression: The operand of the GetDay function must be a DateTime");
			}
		}
	}
}
