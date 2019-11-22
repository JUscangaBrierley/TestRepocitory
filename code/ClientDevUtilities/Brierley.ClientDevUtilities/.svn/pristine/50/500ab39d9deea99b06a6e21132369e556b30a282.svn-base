using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// First, care should be taken not to confuse this class (The Sum Function) with the Sum(+) Operator. The
	/// two are not the same thing. The sum function provides the ability to iterate over a set of rows in an
	/// attribute set and return the sum of a field or attribute in that set.
	/// </summary>
	/// <example>
	///     Usage : SUM('attrSetName','attrName',bScript Expression,UseChildren)
	/// </example>
	/// <remarks>The attrSetName paramater must be the name of an existing attribute set.
	/// The attrName parameter must be the name of an attribute that exists in the named attribute set.
	/// bScript expression is an expression that if supplied will be evaluated against every row being examined.
	/// If the bScript expression returns true that rows attribute value is included in the sum. If this expression
	/// returns false, that rows attribute value is not included in the sum.
	/// The UseChildren parameter value is a boolean value that determines wheather or not the function should only consider
	/// rows of the named attribute set that are formal children of the row that has invoked the rule.
	/// Function names are not case sensative.</remarks>
	[Serializable]
	[ExpressionContext(Description = "Provides the ability to iterate over a set of rows in an attribute set and return the sum of a field or attribute in that set.",
		DisplayName = "Sum",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers)]
	public class Sum : UnaryOperation
	{
		#region Private Variables

		private Expression AttributeSetName = null;
		private Expression AttributeName = null;
		private Expression MatchingExpression = null;
		private Expression UseChildren = null;
		private string attributeName = null;
        private string attributeSetName = null;
        private bool useChildren = false;

		#endregion

		#region Private Helpers
		private IList<string> GetTraversalPath(string rootName, IList<string> pathList, out bool found)
		{
			if (pathList == null)
			{
				pathList = new List<string>();
			}

			found = false;

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				IList<AttributeSetMetaData> metaList = null;
				if (rootName == "Member")
				{
					metaList = loyalty.GetAttributeSetsByType(Common.AttributeSetType.Member);
				}
				else if (rootName == "VirtualCard")
				{
					metaList = loyalty.GetAttributeSetsByType(Common.AttributeSetType.VirtualCard);
				}
				else
				{
					AttributeSetMetaData meta = loyalty.GetAttributeSetMetaData(rootName);
					if (meta.GetAttribute(attributeName) != null)
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
			if (owner is Member)
			{
				traversalPath = GetTraversalPath("Member", null, out found);
			}
			if (owner is VirtualCard)
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
				string errMsg = string.Format("No attribute set found that contains the attribute {1}", attributeName);
				throw new LWBScriptException(errMsg);
			}
			return attSets;
		}

		private decimal GetRowSum(ContextObject contextObject, IList<IClientDataObject> atsList)
		{
			decimal rowSum = 0;
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
						object value = row.GetAttributeValue(attributeName);
						if (System.Decimal.TryParse(value.ToString(), out theValue))
						{
							rowSum += theValue;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new LWBScriptException("Sum function unable to obtain child rows in " + attributeSetName + " for parent attribute set " + contextObject.InvokingRow.GetMetaData().Name, ex);
			}
			return rowSum;
		}
		#endregion

		/// <summary>
		/// public constructor used primarily by UI components. 
		/// </summary>
		public Sum()
		{
		}

		/// <summary>
		/// Internal constructor.
		/// </summary>
		/// <param name="rhs">An object of Type <see cref="Brierley.Framework.bScript.Expression"/></param>
		internal Sum(Expression rhs)
			: base("Sum", rhs)
		{
			int numArgs = ((ParameterList)rhs).Expressions.Length;
			if (numArgs == 4)
			{
				AttributeSetName = ((ParameterList)rhs).Expressions[0];
				AttributeName = ((ParameterList)rhs).Expressions[1];
				MatchingExpression = ((ParameterList)rhs).Expressions[2];
				UseChildren = ((ParameterList)rhs).Expressions[3];
				return;
			}
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetPoints.");
		}

		/// <summary>
		/// This method will return a string containing the functions syntax definition
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "SUM('attrSetName','attrName',bScript Expression,UseChildren)";
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
			decimal RowSum = 0;
            attributeSetName = AttributeSetName.evaluate(contextObject).ToString();
			attributeName = AttributeName.evaluate(contextObject).ToString();
            useChildren = Convert.ToBoolean(UseChildren.evaluate(contextObject).ToString());

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData metadata = loyalty.GetAttributeSetMetaData(attributeSetName);
				IAttributeSetContainer root = contextObject.InvokingRow != null ? contextObject.InvokingRow : contextObject.Owner;
				IList<IClientDataObject> atsList = null;
				if (metadata != null)
				{
					if (useChildren)
					{
						atsList = GetAttributeSets(root, atsList);
						RowSum = GetRowSum(contextObject, atsList);
					}
					else
					{
						if (metadata.GetAttribute(attributeName) == null)
						{
							errMsg = string.Format("{0} does not contain the attribute {1}", attributeSetName, attributeName);
							throw new LWBScriptException(errMsg);
						}
						atsList = root.GetChildAttributeSets(attributeSetName);
						RowSum = GetRowSum(contextObject, atsList);
					}
				}
				else
				{
					errMsg = string.Format("AttributeSet {0} does not exist.", attributeSetName);
					throw new LWBScriptException(errMsg);
				}
				return RowSum;
			}
		}
	}
}
