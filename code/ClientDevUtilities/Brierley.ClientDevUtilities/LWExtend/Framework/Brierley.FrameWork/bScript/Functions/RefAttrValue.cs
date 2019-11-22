using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The RefAttrValue function returns the value for the named attribute at the index given by RowIndex
    /// from the named global attribute set.
    /// </summary>
    /// <example>
    ///     Usage : RefAttrValue('Global AttributeSetName','attrName','WhereClause')
    /// </example>
    /// <remarks>
    /// Attribute Set Name must be the name of a valid global attribute set.
    /// attrName must be the name of a valid attribute in the named attribute set.
    /// WhereClause must be a valid where clause of an SQL statement that would result in one row.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the value for the named attribute at the index given by RowIndex from the named global attribute set.", 
		DisplayName = "RefAttrValue", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Objects,
        WizardDescription="Reference Data",
        AdvancedWizard = true,
        WizardCategory = WizardCategories.Attributes)]
    [ExpressionParameter(Order = 0, Name = "Reference attribute Set Name", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which global attribute set name?", Helpers = ParameterHelpers.GlobalAttributeSet)]
    [ExpressionParameter(Order = 1, Name = "Attribute name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute name?", Helpers = ParameterHelpers.Attribute)]
    [ExpressionParameter(Order = 2, Name = "Alias", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Alias?")]
	[ExpressionParameter(Order = 3, Name = "WhereClause", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Criteria?")]    
	public class RefAttrValue : UnaryOperation
    {
        private const string _className = "RefAttrValue";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private Expression AttributeSetName = null;
        private Expression AttributeName = null;
        private Expression Alias = null;
        private Expression WhereClause = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public RefAttrValue()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal RefAttrValue(Expression rhs)
            : base("RefAttrValue", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 4)
            {
                AttributeSetName = ((ParameterList)rhs).Expressions[0];
                AttributeName = ((ParameterList)rhs).Expressions[1];
                Alias = ((ParameterList)rhs).Expressions[2];
                WhereClause = ((ParameterList)rhs).Expressions[3];
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to Attribute Value.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "AttrValue('AttributeSetName','attrName','Alias', 'WhereClause')";
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
            string msg = "";
            string attrValue = "";

            string attributeName = (string)AttributeName.evaluate(contextObject);
            string attributeSetName = (string)AttributeSetName.evaluate(contextObject);
            string alias = (string)Alias.evaluate(contextObject);

			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData refAsd = service.GetAttributeSetMetaData(AttributeSetName.evaluate(contextObject).ToString());
				if (refAsd != null)
				{
					if (refAsd.Type == AttributeSetType.Global)
					{
						msg = string.Format("Evaluating attribute of {0} of attribute set {1}.", attributeName, attributeSetName);
						_logger.Debug(_className, methodName, msg);

						string whereClause = (string)WhereClause.evaluate(contextObject);

						msg = string.Format("Executing global att set query for clause {0}.", whereClause);
						_logger.Debug(_className, methodName, msg);
						//LWQueryBatchInfo batchInfo = new LWQueryBatchInfo();
						IList<IClientDataObject> resultSet = service.GetAttributeSetObjects(null, refAsd, alias, whereClause, string.Empty, null, false, false);

						try
						{
							if (resultSet != null && resultSet.Count > 0)
							{
								IClientDataObject row = resultSet[0];
								attrValue = (string)row.GetAttributeValue(attributeName).ToString();
							}
						}
						catch (Exception ex)
						{
							throw new CRMException("RefAttrValue Error: " + ex.Message);
						}
					}
					else
					{
						throw new CRMException(string.Format("RefAttrValue Error: {0} is not a global attribute set.", attributeSetName));
					}
				}
				else
				{
					throw new CRMException("RefAttrValue Error: Invalid Global Attribute Set Name.");
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
            string meta = "Global." + AttributeSetName.ToString() + "." + AttributeName.ToString();
            return meta;
        }
    }
}
