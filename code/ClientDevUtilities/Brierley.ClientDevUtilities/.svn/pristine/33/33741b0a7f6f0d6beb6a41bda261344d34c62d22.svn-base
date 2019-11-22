using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The AttrValue function returns the value for the named attribute at the index given by RowIndex
	/// from the named attribute set.
	/// </summary>
	/// <example>
	///     Usage : AttrValue('AttributeSetName','attrName','Parent', RowIndex)
	/// </example>
	/// <remarks>
	/// Attribute Set Name must be the name of a valid attribute set.
	/// attrName must be the name of a valid attribute in the named attribute set.
	/// Parent must be an expression that resolves to a parent object.
	/// RowIndex must be either a number or a bScript expression that evaluates to a number.
	///</remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns the value for the named attribute at the index given by RowIndex from the named attribute set.",
		DisplayName = "AttrValue",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Objects,

		WizardDescription = "Member attribute value (by index)",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function,
		EvalRequiresMember = true
		)]

	[ExpressionParameter(Order = 0, Name = "Attribute set name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute set?", Helpers = ParameterHelpers.AttributeSet)]
	[ExpressionParameter(Order = 1, Name = "Attribute name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute?", Helpers = ParameterHelpers.Attribute)]
	[ExpressionParameter(Order = 2, Name = "Row index", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "Which row in the set?")]
	public class AttrValue : UnaryOperation
	{
		private const string _className = "AttrValue";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private Expression _attributeSetName = null;
		private Expression _attributeName = null;
		private Expression _parent = null;
		private Expression _rowIndex = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public AttrValue()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal AttrValue(Expression rhs)
			: base("AttrValue", rhs)
		{
			// no arguments, so use defaults
			if (rhs == null)
			{
				return;
			}

			// multiple arguments are multiple parameters, so make sure at least 2 and no more than 4 arguments are present
			int numArgs = ((ParameterList)rhs).Expressions.Length;
			if (numArgs < 2 || numArgs > 4)
			{
				string msg = "Invalid Function Call: Wrong number of arguments passed to AttrValue.";
				_logger.Error(_className, "AttrValue", msg);
				throw new CRMException(msg);
			}

			// AttributeSetName and AttributeName
			_attributeSetName = ((ParameterList)rhs).Expressions[0];
			_attributeName = ((ParameterList)rhs).Expressions[1];

			// RowIndex
			if (numArgs > 2)
			{
				_rowIndex = ((ParameterList)rhs).Expressions[2];
			}

			// Parent and RowIndex
			if (numArgs > 3)
			{
				_parent = ((ParameterList)rhs).Expressions[2];
				_rowIndex = ((ParameterList)rhs).Expressions[3];
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "AttrValue('AttributeSetName','attrName',RowIndex)";
			}
		}

		private object GetAttrValueFromOwner(ContextObject contextObject, IAttributeSetContainer thisowner)
		{
			string methodName = "GetAttrValueFromOwner";

			object attrValue = string.Empty;
			using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				int theIndex = System.Int32.Parse(this._rowIndex.evaluate(contextObject).ToString());
				string attributeSetName = this._attributeSetName.evaluate(contextObject).ToString();
				string attributeName = this._attributeName.evaluate(contextObject).ToString();
				if (!thisowner.IsLoaded(attributeSetName))
				{
					svc.LoadAttributeSetList(thisowner, attributeSetName, false);
				}
				IList<IClientDataObject> atsList = thisowner.GetChildAttributeSets(attributeSetName);
				if (atsList != null && atsList.Count > 0 && theIndex >= 0 && theIndex < atsList.Count)
				{
					attrValue = atsList[theIndex].GetAttributeValue(attributeName);
				}
				else
				{
					_logger.Debug(_className, methodName, string.Format("No value at index {0}.", theIndex));
				}
				return attrValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			string methodName = "evaluate";
			string msg = string.Empty;

			Member member = ResolveMember(contextObject.Owner);
			if (member == null)
			{
				msg = string.Format("AttrValue must be evaluated in the context of a loyalty member.");
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			msg = string.Format("Evaluating attribute {0} of attribute set {1}.", _attributeName, _attributeSetName);
			_logger.Debug(_className, methodName, msg);

			object attrValue = string.Empty;
			using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData metadata = svc.GetAttributeSetMetaData((_attributeSetName.evaluate(contextObject) ?? string.Empty).ToString());
				if (metadata != null)
				{
					AttributeMetaData attMeta = metadata.GetAttribute((_attributeName.evaluate(contextObject) ?? string.Empty).ToString());
					if (attMeta == null)
					{
						// unable to find metadata for the attribute set.
						msg = string.Format("Unable to retrieve metadata for attribute {0} from attribute set {1}.", _attributeName, _attributeSetName);
						_logger.Error(_className, methodName, msg);
						throw new CRMException(msg);
					}
					else if (_parent != null)
					{
						//A parent expression is specified to get the attribute set.
						_logger.Debug(_className, methodName, "Using parent expression to lookup the attribute set.");
						IAttributeSetContainer parent = (IAttributeSetContainer)_parent.evaluate(contextObject);
						if (parent != null)
						{
							attrValue = GetAttrValueFromOwner(contextObject, parent);
						}
						else
						{
							throw new CRMException("Unable to resolve parent object.");
						}
					}
					else if (contextObject.InvokingRow != null && contextObject.InvokingRow.GetAttributeSetName().ToLower() == (_attributeSetName.evaluate(contextObject) ?? string.Empty).ToString().ToLower())
					{
						//If there is an invoking row then we should just get the attribute from the invoking row.
						_logger.Debug(_className, methodName, "Using invoking row to lookup the attribute set.");
						attrValue = contextObject.InvokingRow.GetAttributeValue((_attributeName.evaluate(contextObject) ?? string.Empty).ToString());
					}
					else
					{
						if (metadata.Type == AttributeSetType.Member)
						{
							attrValue = GetAttrValueFromOwner(contextObject, member);
						}
						else if (metadata.Type == AttributeSetType.Global)
						{
							int theIndex = System.Int32.Parse(this._rowIndex.evaluate(contextObject).ToString());
							IList<IClientDataObject> resultSet = svc.GetAttributeSetObjects(null, metadata, string.Empty, string.Empty, string.Empty, null, false, false);
							if (resultSet != null && resultSet.Count > 0)
							{
								IClientDataObject row = resultSet[theIndex];
								attrValue = (string)row.GetAttributeValue((_attributeName.evaluate(contextObject) ?? string.Empty).ToString());
							}
						}
						else
						{
							attrValue = GetAttrValueFromOwner(contextObject, contextObject.Owner);
						}
					}

				}
				else
				{
					// unable to find metadata for the attribute set.
					msg = string.Format("Unable to retrieve metadata for attribute set {0}.", (_attributeName.evaluate(contextObject) ?? string.Empty).ToString());
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}
				return attrValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string parseMetaData()
		{
			string meta = _attributeSetName + "." + _attributeName;
			return meta;
		}
	}
}
