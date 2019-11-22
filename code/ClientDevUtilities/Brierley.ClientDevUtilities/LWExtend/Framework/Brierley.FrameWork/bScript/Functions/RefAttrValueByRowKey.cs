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
    /// The RefAttrValueByRowKey function returns the value for the named attribute of the global 
    /// attribute set by its rowkey.
    /// </summary>
    /// <example>
    ///     Usage : RefAttrValueByRowKey('GlobalAttributeSetName',RowKey, 'attrName')
    /// </example>
    /// <remarks>
    /// Global Attribute Set Name must be the name of a valid attribute set.
    /// RowKey is the primay key of the attribute set.
    /// attrName must be the name of a valid attribute in the named attribute set.
    ///</remarks>
    [Serializable]
    [ExpressionContext(Description = "Returns the value for the named attribute with the provided rowkey from the named global attribute set.",
        DisplayName = "RefAttrValueByRowKey",
        ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
        ExpressionReturns = ExpressionApplications.Objects,

        WizardDescription = "Global attribute value (by row key)",
        AdvancedWizard = true,
        WizardCategory = WizardCategories.Function)]

    [ExpressionParameter(Order = 0, Name = "Global attribute set name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which global attribute set?", Helpers = ParameterHelpers.GlobalAttributeSet)]
    [ExpressionParameter(Order = 1, Name = "Row key", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "Which row key?")]
    [ExpressionParameter(Order = 2, Name = "Attribute name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute?", Helpers = ParameterHelpers.Attribute)]

    public class RefAttrValueByRowKey : UnaryOperation
    {
        private const string _className = "RefAttrValueByRowKey";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private Expression AttributeSetName = null;
        private Expression AttributeName = null;
        private Expression RowKey = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public RefAttrValueByRowKey()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal RefAttrValueByRowKey(Expression rhs)
            : base("RefAttrValueByRowKey", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 3)
            {
                AttributeSetName = ((ParameterList)rhs).Expressions[0];
                RowKey = ((ParameterList)rhs).Expressions[1];
                AttributeName = ((ParameterList)rhs).Expressions[2];
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
                return "RefAttrValueByRowKey('GlobalAttributeSetName',RowKey,'attrName')";
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

			string attributeSetName = (string)AttributeSetName.evaluate(contextObject);
			string attributeName = (string)AttributeName.evaluate(contextObject);

			//Member member = ResolveMember(contextObject.Owner);            
			object attrValue = string.Empty;
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData metadata = service.GetAttributeSetMetaData(attributeSetName);
				if (metadata != null)
				{
					object rowKey = this.RowKey.evaluate(contextObject);
					if (rowKey != null && !string.IsNullOrWhiteSpace(rowKey.ToString()))
					{
						long theIndex = System.Int64.Parse(rowKey.ToString());
						IClientDataObject cobj = service.GetAttributeSetObject(attributeSetName, theIndex, false);
						if (cobj != null)
						{
							attrValue = cobj.GetAttributeValue(attributeName);
						}
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
            string meta = AttributeSetName.ToString() + "." + AttributeName.ToString();
            return meta;
        }
    }
}
