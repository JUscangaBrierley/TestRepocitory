using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Gets the member's current primary Loyalty ID Number from VirtualCard.
	/// </summary>
	/// <example>
	///     Usage : GetActiveLoyaltyId()
	/// </example>   
	[Serializable]
	[ExpressionContext(Description = "Gets the member's current active Loyalty ID Number.",
	DisplayName = "GetActiveLoyaltyId",
	ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
	ExpressionType = ExpressionTypes.Function,
	ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
	ExpressionReturns = ExpressionApplications.Strings, 
	
	WizardDescription = "Loyalty Id", 
	WizardCategory= WizardCategories.Profile,
	AdvancedWizard = false,
	EvalRequiresMember = true
	)]

	[ExpressionParameter(Name = "Card Type", Type = ExpressionApplications.Numbers, Optional = true, WizardDescription = "Which card type?")]
	public class GetActiveLoyaltyId : UnaryOperation
	{
		private Expression _arg = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public GetActiveLoyaltyId()
			: base()
		{
		}

		public GetActiveLoyaltyId(Expression arg)
			: base("GetActiveLoyaltyId", null)
		{
			_arg = arg;
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				//return "GetActiveLoyaltyId()";
				return "GetActiveLoyaltyId([CardType])";
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
				string loyaltyId = string.Empty;
				long cardType = -1;
				if (_arg != null)
				{
					long.TryParse(_arg.evaluate(contextObject).ToString(), out cardType);
				}				
				if (member.LoyaltyCards != null)
				{
					foreach (VirtualCard card in member.LoyaltyCards)
					{
						if (card.IsPrimary && (cardType < 0 || card.CardType == cardType))
						{
							loyaltyId = card.LoyaltyIdNumber;
							break;
						}
					}
				}
				return loyaltyId;
			}
			else
			{
				throw new CRMException("GetActiveLoyaltyId must be evaluated in the context of a loyalty member.");
			}
		}
	}
}
