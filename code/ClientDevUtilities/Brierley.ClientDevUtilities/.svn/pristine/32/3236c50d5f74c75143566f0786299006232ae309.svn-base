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
	/// This function determines if the provided virtual card is of a certain type.
	/// This method is only invoked on a loyalty card.
	/// </summary>
	/// <example>
	///     Usage : IsVirtualCardOfType()
	/// </example>   
	[Serializable]
	[ExpressionContext(Description = "Determines whether the virtual card is of certain type o rnot.",
	DisplayName = "IsVirtualCardOfType",
	ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
	ExpressionType = ExpressionTypes.Function,
	ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
	ExpressionReturns = ExpressionApplications.Booleans,

		WizardDescription = "Is virtual card of type?",
		AdvancedWizard = true)]

	[ExpressionParameter(Order = 0, Name = "Card Type", Type = ExpressionApplications.Numbers, Optional = true, WizardDescription = "Which card type?")]

	public class IsVirtualCardOfType : UnaryOperation
	{
		private Expression _arg = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public IsVirtualCardOfType()
			: base()
		{
		}

		public IsVirtualCardOfType(Expression arg)
			: base("IsVirtualCardOfType", arg)
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
				return "IsVirtualCardOfType(CardType)";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			VirtualCard vc = ResolveLoyaltyCard(contextObject.Owner);			
			if (vc != null)
			{
				string loyaltyId = string.Empty;
				long cardType = -1;
				if (_arg != null)
				{
					long.TryParse(_arg.evaluate(contextObject).ToString(), out cardType);
				}
				return vc.CardType == cardType;								
			}
			else
			{
				throw new CRMException("IsVirtualCardOfType must be evaluated in the context of a loyalty card.");
			}
		}
	}
}
