using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The Last Index Of function returns the index number of the last row in an attribute set that matches
    /// the expression specified by bScriptExpression.
    /// </summary>
    /// <example>
    ///     Usage : LastIndexOf('AttributeSetName',bScriptExpression,UseChildren)
    /// </example>
    /// <remarks>
    /// AttributeSetName must be the name of an attribute set.
    /// UseChildren is a boolean value indicating whether only valid child rows of the row invoking the rule will be included.
    /// bScriptExpression used to match or filter rows.
    /// The function will return 0 if no rows are found matching the expression.
    /// To find the highest row index in a set use RowCount().
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the index number of the last row in an attribute set that matches the expression specified by bScriptExpression.", 
		DisplayName = "LastIndexOf", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Numbers,
		EvalRequiresMember = true)]
	public class LastIndexOf : UnaryOperation
    {
        private Expression AttributeSetName = null;
        Expression MatchingExpression = null;
        bool UseChildren = false;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public LastIndexOf()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal LastIndexOf(Expression rhs)
			: base("LastIndexOf", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 3)
            {
                AttributeSetName = ((ParameterList)rhs).Expressions[0];
                MatchingExpression = ((ParameterList)rhs).Expressions[1];
                UseChildren = System.Convert.ToBoolean(((ParameterList)rhs).Expressions[2].ToString());
                return;
            }
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to LastIndexOf.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "LastIndexOf('AttributeSetName',bScriptExpression,UseChildren)";
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
                decimal currentRowIndex = -1;
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					AttributeSetMetaData metadata = service.GetAttributeSetMetaData(AttributeSetName.evaluate(contextObject).ToString());
					if (metadata != null)
					{
						if (!contextObject.Owner.IsLoaded(AttributeSetName.evaluate(contextObject).ToString()))
						{
							service.LoadAttributeSetList(contextObject.Owner, AttributeSetName.evaluate(contextObject).ToString(), true);
						}
						IList<IClientDataObject> atsList = contextObject.Owner.GetChildAttributeSets(AttributeSetName.evaluate(contextObject).ToString());
						if (UseChildren)
						{
							try
							{
								IList<AttributeSetMetaData> childMetalist = metadata.ChildAttributeSets;
								foreach (IClientDataObject row in atsList)
								{
									foreach (AttributeSetMetaData childAttSetList in childMetalist)
									{
										if (!row.IsLoaded(childAttSetList.Name))
										{
											service.LoadAttributeSetList(row, childAttSetList.Name, true);
										}
										IList<IClientDataObject> childList = contextObject.Owner.GetChildAttributeSets(AttributeSetName.evaluate(contextObject).ToString());
										foreach (IClientDataObject childRow in childList)
										{
											ContextObject cObj = new ContextObject();
											cObj.Owner = member;
											cObj.InvokingRow = childRow;
											if ((bool)this.MatchingExpression.evaluate(cObj))
											{
												currentRowIndex = atsList.IndexOf(childRow);
											}
										}
									}
								}
							}
							catch
							{
								throw new CRMException("LastIndexOf function unable to obtain child rows in " + AttributeSetName.evaluate(contextObject).ToString() + " for parent attribute set " + contextObject.InvokingRow.Parent.GetMetaData().Name);
							}
						}
						else
						{
							foreach (IClientDataObject row in atsList)
							{
								ContextObject cObj = new ContextObject();
								cObj.Owner = member;
								cObj.InvokingRow = row;
								if ((bool)this.MatchingExpression.evaluate(cObj))
								{
									currentRowIndex = atsList.IndexOf(row);
								}
							}
						}
						return currentRowIndex;
					}
					else
					{
						throw new CRMException("LastIndexOf must be evaluated in the context of a loyalty member.");
					}
				}
            }
            else
            {
                throw new CRMException("LastIndexOf must be evaluated in the context of a loyalty member.");
            }
        }
    }
}
