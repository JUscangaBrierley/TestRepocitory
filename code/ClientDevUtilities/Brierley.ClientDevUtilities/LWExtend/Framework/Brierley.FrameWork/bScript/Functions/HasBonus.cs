using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{

	/// <summary>
	/// The HasBonus function will return true if the member is in the named bonus.
	/// </summary>
	/// <example>
	///     Usage : HasBonus('bonus name')
	/// </example>
	/// <remarks>
	/// </remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns true if the member has the named bonus, else false.",
		DisplayName = "HasBonus",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Booleans,

		WizardDescription = "Has Bonus?",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Profile,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Bonus Name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which bonus?", Helpers = ParameterHelpers.Bonus)]
	public class HasBonus : UnaryOperation
	{
		private Expression _rhs = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public HasBonus()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal HasBonus(Expression rhs)
			: base("HasBonus", rhs)
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
				return "HasBonus('BonusName')";
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
				using (var content = LWDataServiceUtil.ContentServiceInstance())
				{
					string bonusName = _rhs.evaluate(contextObject).ToString();
					BonusDef def = content.GetBonusDef(bonusName);
					if (def != null)
					{
						return service.HowManyMemberBonusesByType(member.IpCode, def.Id) > 0;
					}
					return false;
				}
			}
			else
			{
				throw new CRMException("HasBonus must be evaluated in the context of a loyalty member.");
			}
		}
	}
}
