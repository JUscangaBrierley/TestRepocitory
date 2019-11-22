using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The Average function compute the average value for the named attribute from the named attribute set where
    /// the row being examined matches the expression given in bScriptExpression. 
    /// </summary>
    /// <example>
    ///     Useage : AVG('attrSetName','attrName',bScriptExpression,UseChildren)
    /// </example>
    /// <remarks>
    /// attrSetName must be the name of a valid attribute set
    /// attrName must be the name of a valid attribute on that set.
    /// bScriptExpression must be a valid bScript Expression
    /// UseChildren is a boolean value indicating whether or not the function should only consider children of the
    /// row invoking the rule.
    /// </remarks>
    [Serializable]
    [ExpressionContext(Description = "Computes the average value for the named attribute from the named attribute set where the row being examined matches the expression given in bScriptExpression.",
        DisplayName = "Avg",
        ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
        ExpressionReturns = ExpressionApplications.Numbers, 
		
		AdvancedWizard = true, 
		WizardCategory = WizardCategories.Function, 
		WizardDescription = "Average attribute value")]

	[ExpressionParameter(Order = 0, Name = "Attribute set name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute set?", Helpers = ParameterHelpers.AttributeSet)]
	[ExpressionParameter(Order = 1, Name = "Attribute name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute?", Helpers = ParameterHelpers.Attribute)]
	[ExpressionParameter(Order = 2, Name = "Expression", Type = ExpressionApplications.Objects, Optional = false, WizardDescription = "bScript Expression")]
	[ExpressionParameter(Order = 3, Name = "Use Children", Type = ExpressionApplications.Booleans, Optional = false, WizardDescription = "Use Children?")]
	

    public class Avg : UnaryOperation
    {
        #region Private Variables
        string AttributeSetName = string.Empty;
        string AttributeName = string.Empty;
        Expression MatchingExpression = null;
        bool UseChildren = false;
        #endregion

        #region Private Helpers
        private IList<string> GetTraversalPath(string rootName, IList<string> pathList, out bool found)
        {
            if (pathList == null)
            {
                pathList = new List<string>();
            }

            found = false;

			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				IList<AttributeSetMetaData> metaList = null;
				if (rootName == "Member")
				{
					metaList = service.GetAttributeSetsByType(Common.AttributeSetType.Member);
				}
				else if (rootName == "VirtualCard")
				{
					metaList = service.GetAttributeSetsByType(Common.AttributeSetType.VirtualCard);
				}
				else
				{
					AttributeSetMetaData meta = service.GetAttributeSetMetaData(rootName);
					if (meta.GetAttribute(AttributeName) != null)
					{
						pathList.Add(meta.Name);
						found = true;
					}
					else
					{
						metaList = meta.ChildAttributeSets;
					}
				}

				if (!found)
				{
					foreach (AttributeSetMetaData child in metaList)
					{
						IList<string> tempPath = GetTraversalPath(child.Name, null, out found);
						if (found == true)
						{
							if (rootName != "Member" && rootName != "VirtualCard")
							{
								pathList.Add(rootName);
							}
							foreach (string name in tempPath)
							{
								pathList.Add(name);
							}
							break;
						}
					}
				}
				return pathList;
			}
        }

        private IList<IClientDataObject> GetAttributeSets(IAttributeSetContainer owner, IList<string> traversalPath, int index, IList<IClientDataObject> atsList)
        {
            if (atsList == null)
            {
                atsList = new List<IClientDataObject>();
            }

            string attSetName = traversalPath[index];
            if (owner.GetMetaData().Name == attSetName)
            {
                atsList = GetAttributeSets(owner, traversalPath, index + 1, atsList);
            }
            else
            {
                IList<IClientDataObject> thisAtsList = owner.GetChildAttributeSets(attSetName);
                if (index == traversalPath.Count - 1)
                {
                    foreach (IClientDataObject obj in thisAtsList)
                    {
                        atsList.Add(obj);
                    }
                }
                else
                {
                    int newIndex = index + 1;
                    foreach (IClientDataObject obj in thisAtsList)
                    {
                        atsList = GetAttributeSets(obj, traversalPath, newIndex, atsList);
                    }
                }
            }
            return atsList;
        }

        private IList<IClientDataObject> GetAttributeSets(IAttributeSetContainer owner, IList<IClientDataObject> atsList)
        {
			bool found = false;
			IList<string> traversalPath = null;
			IList<IClientDataObject> attSets = null;
			if (owner.GetType() == typeof(Member))
			{
				traversalPath = GetTraversalPath("Member", null, out found);
			}
			if (owner.GetType() == typeof(VirtualCard))
			{
				traversalPath = GetTraversalPath("VirtualCard", null, out found);
			}
			else
			{
				traversalPath = GetTraversalPath(owner.GetMetaData().Name, null, out found);
			}

			if (found)
			{
				attSets = GetAttributeSets(owner, traversalPath, 0, attSets);
			}
			else
			{
				string errMsg = string.Format("No attribute set found that contains the attribute {0}", AttributeName);
				throw new LWBScriptException(errMsg);
			}
			return attSets;
        }

        private void GetRowSum(ContextObject contextObject, IList<IClientDataObject> atsList, out decimal rowSum, out decimal rowCount)
        {
            rowSum = 0;
            rowCount = 0;
            try
            {
                foreach (IClientDataObject row in atsList)
                {
                    ContextObject cObj = new ContextObject();
                    cObj.InvokingRow = row;
                    cObj.Owner = contextObject.Owner;
                    if ((bool)this.MatchingExpression.evaluate(cObj))
                    {
                        decimal theValue = 0;
                        object value = row.GetAttributeValue(AttributeName);
                        if (System.Decimal.TryParse(value.ToString(), out theValue))
                        {
                            rowSum += theValue;
                            rowCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new LWBScriptException("Sum function unable to obtain child rows in " + this.AttributeSetName + " for parent attribute set " + contextObject.InvokingRow.GetMetaData().Name, ex);
            }            
        }
        #endregion

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Avg()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal Avg(Expression rhs)
            : base("Avg", rhs)
        {
            ParameterList plist = rhs as ParameterList;
            ContextObject cObj = new ContextObject();
            if (plist.Expressions.Length == 4)
            {
                AttributeSetName = plist.Expressions[0].evaluate(cObj).ToString();
                AttributeName = plist.Expressions[1].evaluate(cObj).ToString();
                MatchingExpression = plist.Expressions[2];
                UseChildren = System.Convert.ToBoolean(plist.Expressions[3].evaluate(cObj));
                return;
            }
            throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to GetPoints.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "AVG('attrSetName','attrName',bScript Expression,UseChildren)";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            string errMsg = string.Empty;
            decimal Rowcount = 0;
            decimal RowSum = 0;
			using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData metadata = svc.GetAttributeSetMetaData(AttributeSetName);
				IAttributeSetContainer root = contextObject.InvokingRow != null ? contextObject.InvokingRow : contextObject.Owner;
				IList<IClientDataObject> atsList = null;
				if (metadata != null)
				{
					if (this.UseChildren)
					{
						//atsList = GetAttributeSets(contextObject.Owner, atsList);  
						atsList = GetAttributeSets(root, atsList);
						GetRowSum(contextObject, atsList, out RowSum, out Rowcount);
					}
					else
					{
						if (metadata.GetAttribute(AttributeName) == null)
						{
							errMsg = string.Format("{0} does not contain the attribute {1}", AttributeSetName, AttributeName);
							throw new LWBScriptException(errMsg);
						}
						//atsList = contextObject.Owner.GetChildAttributeSets(AttributeSetName);
						atsList = root.GetChildAttributeSets(AttributeSetName);
						GetRowSum(contextObject, atsList, out RowSum, out Rowcount);
					}
				}
				else
				{
					errMsg = string.Format("AttributeSet {0} does not exist.", AttributeSetName);
					throw new LWBScriptException(errMsg);
				}
				decimal retVal = 0;
				try
				{
					retVal = RowSum / Rowcount;
				}
				catch
				{
					retVal = 0;
				}
				return retVal;
			}
        }        
    }

}
