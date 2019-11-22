using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;


namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The RowCount functions returns the count of rows in the named attribute set that match the given bScript Expression.
	/// </summary>
	/// <example>
	///     Usage : RowCount('AttributeSetName',bScriptExpression,UseChildren)
	/// </example>
	/// <remarks>
	/// AttributeSetName must be the name of a valid attribute set
	/// bScript expression must evaluate to true or false. If true the row is counted.
	/// </remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns the count of rows in the named attribute set that match the given bScript Expression.",
		DisplayName = "RowCount",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Item Count",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function,
		EvalRequiresMember = true
		)]

	[ExpressionParameter(Order = 0, Name = "AttributeSetName", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute set?", Helpers = ParameterHelpers.AttributeSet)]
	[ExpressionParameter(Order = 1, Name = "Expression", Type = ExpressionApplications.Objects, Optional = false, WizardDescription = "Match Expression")]
	[ExpressionParameter(Order = 2, Name = "UseChildren?", Type = ExpressionApplications.Booleans, Optional = true, WizardDescription = "Use Children?", Helpers = ParameterHelpers.Boolean)]
	public class RowCount : UnaryOperation
	{
		private Expression AttributeSetName = null;
		private Expression MatchingExpression = null;
		bool UseChildren = false;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public RowCount()
		{
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "RowCount('AttributeSetName',bScriptExpression,UseChildren)";
			}
		}

		/// <summary>
		/// Internal constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal RowCount(Expression rhs)
			: base("RowCount", rhs)
		{
			int numArgs = ((ParameterList)rhs).Expressions.Length;
			if (numArgs == 2)
			{
				AttributeSetName = ((ParameterList)rhs).Expressions[0];
				MatchingExpression = ((ParameterList)rhs).Expressions[1];
				return;
			}
			else if (numArgs == 3)
			{
				AttributeSetName = ((ParameterList)rhs).Expressions[0];
				MatchingExpression = ((ParameterList)rhs).Expressions[1];
				UseChildren = System.Convert.ToBoolean(((ParameterList)rhs).Expressions[2].ToString());
				return;
			}
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to RowCount.");
		}

		protected long GetCount(IAttributeSetContainer container, ContextObject contextObject)
		{
			long count = 0;
			if (!container.IsLoaded(AttributeSetName.evaluate(contextObject).ToString()))
			{
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					service.LoadAttributeSetList(container, AttributeSetName.evaluate(contextObject).ToString(), false);
				}
			}
			IList<IClientDataObject> atsList = container.GetChildAttributeSets(AttributeSetName.evaluate(contextObject).ToString());
			foreach (IClientDataObject row in atsList)
			{
				ContextObject cObj = new ContextObject();
				cObj.Owner = container;
				cObj.InvokingRow = row;
				if ((bool)this.MatchingExpression.evaluate(cObj))
				{
					count++;
				}
			}
			return count;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			Member member = ResolveMember(contextObject.Owner);
			if (member == null)
			{
				throw new CRMException("RowCount must be evaluated in the context of a loyalty member.");
			}

			long count = 0;
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData metadata = service.GetAttributeSetMetaData(AttributeSetName.evaluate(contextObject).ToString());
				if (metadata == null)
				{
					throw new CRMException("Unable to retrieve metadata for " + AttributeSetName.evaluate(contextObject).ToString());
				}

				if (UseChildren)
				{
					IAttributeSetContainer parentContainer = null;
					if (contextObject.Owner.GetType() == typeof(Member))
					{
						parentContainer = member;
					}
					else if (contextObject.Owner.GetType() == typeof(Member))
					{
						parentContainer = ResolveLoyaltyCard(contextObject.Owner);
					}
					else
					{
						parentContainer = contextObject.InvokingRow;
					}

					count = GetCount(parentContainer, contextObject);
				}
				else
				{
					count = GetCount(contextObject.Owner, contextObject);
				}
				return count;
			}
		}
	}
}
