using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{

	/// <summary>
	/// The HasCompletedBonus function will return true if the member has completed the named bonus.
	/// </summary>
	/// <example>
	///     Usage : HasCompletedBonus('bonus name')
	/// </example>
	/// <remarks>
	/// </remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns true if the member has completed the named bonus, else false.",
		DisplayName = "HasCompletedBonus",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Booleans,

		WizardDescription = "Has Completed Bonus?",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Profile,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Bonus Name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which bonus?", Helpers = ParameterHelpers.Bonus)]
	public class HasCompletedBonus : UnaryOperation
	{
		private Expression _rhs = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public HasCompletedBonus()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal HasCompletedBonus(Expression rhs)
			: base("HasCompletedBonus", rhs)
		{
			_rhs = rhs;
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "HasCompletedBonus('BonusName')";
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
				string bonusName = _rhs.evaluate(contextObject).ToString();

				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				using (var content = LWDataServiceUtil.ContentServiceInstance())
				{
					BonusDef def = content.GetBonusDef(bonusName);
					if (def != null)
					{
						return service.HowManyCompletedBonusesByType(member.IpCode, def.Id) > 0;
					}
					return false;
				}
			}
			else
			{
				throw new CRMException("HasCompletedBonus must be evaluated in the context of a loyalty member.");
			}
		}
	}
}
