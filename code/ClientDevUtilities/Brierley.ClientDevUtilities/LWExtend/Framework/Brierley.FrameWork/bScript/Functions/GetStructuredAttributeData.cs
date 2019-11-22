using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The GetStructuredAttributeData function returns an attribute from the the provided row. 
    /// </summary>
    /// <example>
    ///     Usage : GetStructuredAttributeData('StructuredElementName','AttributeName',RowIndex)
    /// </example>
    /// <remarks>
    /// StructuredElementName is the name of the structured element to which the rows belong.
    /// AttributeName must be the name of a valid attribute in the specified structured element.
    /// RowIndex - must be either a number or a bScript expression that evaluates to a number.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns an attribute from the the provided row.", 
		DisplayName = "GetStructuredAttributeData", 
		ExcludeContext = ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Content, 
		ExpressionReturns = ExpressionApplications.Objects //, 

		//AdvancedWizard = true, 
		//WizardDescription = "Get Structured Attribute Data", 
		//WizardCategory = WizardCategories.Content
		
		)]
	public class GetStructuredAttributeData : UnaryOperation
    {
        private const string _className = "GetStructuredAttributeData";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private Expression _elementName = null;
        private Expression _attributeName = null;
        private Expression _rowIndexExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetStructuredAttributeData()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public GetStructuredAttributeData(Expression rhs)
            : base("GetStructuredAttributeData", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 3)
            {
                _elementName = ((ParameterList)rhs).Expressions[0];
                _attributeName = ((ParameterList)rhs).Expressions[1];
                _rowIndexExpression = ((ParameterList)rhs).Expressions[2];
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetStructuredAttributeData.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetStructuredAttributeData('StructuredElementName','AttributeName',RowIndex)";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">An instance of ContextObject</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
			if (contextObject.Environment == null || !contextObject.Environment.ContainsKey("StructuredDataRows")) return null;

			StructuredDataRows rows = contextObject.Environment["StructuredDataRows"] as StructuredDataRows;
            if (rows == null || rows.Count < 1) return null;

            object obj = _rowIndexExpression.evaluate(contextObject);
            if (obj == null)
                throw new CRMException("GetStructuredAttributeData: error evaluating RowIndex");

            int rowIndex = StringUtils.FriendlyInt32(obj, -1);
            if (rowIndex == -1)
                throw new CRMException(string.Format("GetStructuredAttributeData: RowIndex evaluates to invalid value: '{0}'", obj.ToString()));

			using (var cms = LWDataServiceUtil.ContentServiceInstance())
			{
				return GetAttributeValue(rows, cms, rowIndex, contextObject);
			}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string parseMetaData()
        {
            string meta = _attributeName.ToString();
            return meta;
        }

        #region Private Methods
        private static StructuredContentAttribute GetStructuredAttribute(ContentService cms, string elementName, string attributeName)
        {
            StructuredContentAttribute attr = cms.GetGlobalAttribute(attributeName);
            if (attr == null)
            {
                StructuredContentElement element = cms.GetContentElement(elementName);
                if (element == null)
                    throw new LWException("Global attribute " + attributeName + " and element " + elementName + " do not exist"); 

                attr = cms.GetElementAttribute(attributeName, element.ID);
                if (attr == null)
                    throw new LWException("Attribute " + attributeName + " does not exist globally nor under element " + elementName);
            }
            return attr;
        }

        private static DateTime GetDateTime(string date)
        {
			//System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
            DateTime dt = DateTime.ParseExact(date, "G", null);
            return dt;
        }

        private object GetAttributeValue(StructuredDataRows rows, ContentService cms, int rowIndex, ContextObject contextObject)
        {
            string methodName = "GetAttributeValue";
            string msg = "";

            if (rows == null || rows.Count < 1)
                throw new CRMException("No structured element data was found.");
            if (rowIndex < 0 || rowIndex >= rows.Count)
                throw new CRMException(string.Format("RowIndex out of range: {0}.", rowIndex));

            msg = string.Format("Evaluating attribute of {0}.", _attributeName.evaluate(contextObject).ToString());
            _logger.Debug(_className, methodName, msg);

            StructuredContentAttribute attMeta = GetStructuredAttribute(cms, _elementName.evaluate(contextObject).ToString(), _attributeName.evaluate(contextObject).ToString());
            DataRow row = rows[rowIndex];
            string value = (row != null ? StringUtils.FriendlyString(row[_attributeName.evaluate(contextObject).ToString()]) : string.Empty);
            object val = null;
            if (attMeta.DataType == StructuredDataType.STRING.ToString())
            {
                val = value;
            }
            else if (attMeta.DataType == StructuredDataType.BOOL.ToString())
            {
                val = bool.Parse(value);
            }
            else if (attMeta.DataType == StructuredDataType.DATETIME.ToString())
            {
                val = GetDateTime(value);
            }
            else if (attMeta.DataType == StructuredDataType.INT.ToString())
            {
                val = int.Parse(value);
            }
            else if (attMeta.DataType == StructuredDataType.REAL.ToString())
            {
                val = decimal.Parse(value);
            }
            return val;
        }

        #endregion
    }
}
