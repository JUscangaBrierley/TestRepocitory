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
    /// The AttrValue function returns the value for the named attribute with the provided rowkey
    /// from the named attribute set.
    /// </summary>
    /// <example>
    ///     Usage : AttrValue('AttributeSetName',RowKey, 'attrName')
    /// </example>
    /// <remarks>
    /// Attribute Set Name must be the name of a valid attribute set.
	/// RowKey is the primay key of the attribute set.
    /// attrName must be the name of a valid attribute in the named attribute set.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the value for the named attribute with the provided rowkey from the named attribute set.", 
		DisplayName = "AttrValueByRowKey", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Objects,
		
		WizardDescription = "Member attribute value (by row key)",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function)]

	[ExpressionParameter(Order = 0, Name = "Attribute set name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute set?", Helpers = ParameterHelpers.AttributeSet)]
	[ExpressionParameter(Order = 1, Name = "Row key", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "Which row key?")]
	[ExpressionParameter(Order = 2, Name = "Attribute name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute?", Helpers = ParameterHelpers.Attribute)]

	public class AttrValueByRowKey : UnaryOperation
    {
        private const string _className = "AttrValueByRowKey";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private Expression _attributeSetName = null;
        private Expression _attributeName = null;
        private Expression _rowKey = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public AttrValueByRowKey()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal AttrValueByRowKey(Expression rhs)
			: base("AttrValueByRowKey", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 3)
            {
                _attributeSetName = ((ParameterList)rhs).Expressions[0];
                _rowKey = ((ParameterList)rhs).Expressions[1];
                _attributeName = ((ParameterList)rhs).Expressions[2];
                return;
            }
			else
			{
				throw new CRMException("Invalid Function Call: Wrong number of arguments passed to AttrValueByRowKey.");
			}
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
				return "AttrValueByRowKey('AttributeSetName',RowKey,'attrName')";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
			//string methodName = "evaluate";
			//string msg = "";

			//Member member = ResolveMember(contextObject.Owner);            
			object attrValue = string.Empty;
			using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData metadata = svc.GetAttributeSetMetaData(_attributeSetName.evaluate(contextObject).ToString());
				if (metadata != null)
				{
					long theIndex = System.Int64.Parse(this._rowKey.evaluate(contextObject).ToString());
					IClientDataObject cobj = svc.GetAttributeSetObject(_attributeSetName.evaluate(contextObject).ToString(), theIndex, false);
					if (cobj != null)
					{
						attrValue = cobj.GetAttributeValue(_attributeName.evaluate(contextObject).ToString());
					}
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
