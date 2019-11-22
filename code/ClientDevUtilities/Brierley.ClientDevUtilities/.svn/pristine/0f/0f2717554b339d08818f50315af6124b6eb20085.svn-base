using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The First Index of function will return the index number of the first row in an attribute set that
    /// matches the expression given by bScriptExpression
    /// </summary>
    /// <example>
    ///     Usage : FirstIndexOf('AttributeSetName',bScriptExpression,UseChildren)
    /// </example>
    /// <remarks>
    /// AttributeSetName must be the name of a valid attribute set.
    /// bScriptExpression must be a valid bScript Expression
    /// UseChildren is a boolean value indicating whether or not the function should consider only valid child rows
    /// of the row in the evaluation context.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Will return the index number of the first row in an attribute set that matches the expression given by bScriptExpression", 
		DisplayName = "FirstIndexOf", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Numbers,
		EvalRequiresMember = true)]
	public class FirstIndexOf : UnaryOperation
    {
        private Expression AttributeSetName = null;
        private Expression MatchingExpression = null;
        private bool UseChildren = false;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public FirstIndexOf()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal FirstIndexOf(Expression rhs)
			: base("FirstIndexOf", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 3)
            {
                AttributeSetName = ((ParameterList)rhs).Expressions[0];
                MatchingExpression = ((ParameterList)rhs).Expressions[1];
                UseChildren = System.Convert.ToBoolean(((ParameterList)rhs).Expressions[2].ToString());
                return;
            }
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to FirstIndexOf.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "FirstIndexOf('AttributeSetName',bScriptExpression,UseChildren)";
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
				using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					AttributeSetMetaData metadata = svc.GetAttributeSetMetaData(AttributeSetName.evaluate(contextObject).ToString());
					if (metadata != null)
					{
						if (!contextObject.Owner.IsLoaded(AttributeSetName.evaluate(contextObject).ToString()))
						{
							svc.LoadAttributeSetList(contextObject.Owner, AttributeSetName.evaluate(contextObject).ToString(), true);
						}
						IList<IClientDataObject> atsList = contextObject.Owner.GetChildAttributeSets(AttributeSetName.evaluate(contextObject).ToString());
						if (UseChildren)
						{
							try
							{
								foreach (IClientDataObject row in atsList)
								{
									IList<IClientDataObject> childAttSetList = row.GetChildAttributeSets(AttributeSetName.evaluate(contextObject).ToString());

									foreach (IClientDataObject childRow in childAttSetList)
									{
										ContextObject cObj = new ContextObject();
										cObj.Owner = member;
										cObj.InvokingRow = childRow;
										if ((bool)this.MatchingExpression.evaluate(cObj))
										{
											currentRowIndex = atsList.IndexOf(childRow);
											break;
										}
									}

								}
							}
							catch
							{
								throw new CRMException("LastIndexOf function unable to obtain child rows in " + AttributeSetName.evaluate(contextObject).ToString() + " for parent attribute set " + contextObject.InvokingRow.GetMetaData().Name);
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
									break;
								}
							}
						}
						return currentRowIndex;

					}
					else
					{
						throw new CRMException("LastIndexOf function unable to obtain rows in " + AttributeSetName.evaluate(contextObject).ToString() + " for parent attribute set " + contextObject.InvokingRow.GetMetaData().Name);
					}
				}
            }
            else
            {
                throw new CRMException("FirstIndexOf must be evaluated in the context of a loyalty member.");
            }                                            
        }
    }
}
