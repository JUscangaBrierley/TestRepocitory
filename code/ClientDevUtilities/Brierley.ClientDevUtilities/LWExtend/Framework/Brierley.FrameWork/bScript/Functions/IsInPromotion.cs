using System;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The IsInPromotion function will return true if the member is in the named promotion.
	/// </summary>
	/// <example>
	///     Usage : IsInPromotion('promotion code', CheckEnrollment)
	/// </example>
	/// <remarks>
	/// </remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns true if the member is in the named promotion.",
		DisplayName = "IsInPromotion",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Booleans,

		WizardDescription = "Has Promotion?",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Profile,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Promotion Code", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which promotion?", Helpers = ParameterHelpers.PromotionCode)]
	[ExpressionParameter(Order = 1, Name = "Check Enrollment", Type = ExpressionApplications.Booleans, Optional = true, WizardDescription = "Check Enrollment?", Helpers = ParameterHelpers.Boolean)]
	public class IsInPromotion : UnaryOperation
	{
		private Expression _promoCodeExpression = null;
		private Expression _checkEnrollment = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public IsInPromotion()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal IsInPromotion(Expression rhs)
			: base("IsInPromotion", rhs)
		{
			// single argument is expression for linkText
			if (!(rhs is ParameterList))
			{
				_promoCodeExpression = rhs;
				return;
			}

			// multiple arguments are multiple parameters, so make sure 1-2 arguments are present
			int numArgs = ((ParameterList)rhs).Expressions.Length;
			if (numArgs < 1 || numArgs > 2)
			{
				string msg = "Invalid Function Call: Wrong number of arguments passed to IsInPromotion.";
				throw new CRMException(msg);
			}

			// linkText
			_promoCodeExpression = ((ParameterList)rhs).Expressions[0];
			_checkEnrollment = ((ParameterList)rhs).Expressions[1];
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "IsInPromotion('PromotionCode')";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			Member member = ResolveMember(contextObject.Owner);
			if (member != null)
			{
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					string promoCode = _promoCodeExpression.evaluate(contextObject).ToString();
					bool checkEnrollment = false;
					if (_checkEnrollment != null)
					{
						checkEnrollment = Convert.ToBoolean(_checkEnrollment.evaluate(contextObject));
						if (checkEnrollment)
						{
							var promo = service.GetPromotionByCode(promoCode);
							if (promo != null && promo.EnrollmentSupportType == Common.PromotionEnrollmentSupportType.None)
							{
								//enrollment not supported. Cannot enforce enrollment, so no need to check it
								checkEnrollment = false;
							}
						}
					}
					if (checkEnrollment)
					{
						return service.IsMemberEnrolledInPromotion(promoCode, member.IpCode);
					}
					else
					{
						return service.IsMemberInPromotionList(promoCode, member.IpCode);
					}
				}
			}
			else
			{
				throw new CRMException("Is In Promotion must be evaluated in the context of a loyalty member.");
			}
		}
	}
}
