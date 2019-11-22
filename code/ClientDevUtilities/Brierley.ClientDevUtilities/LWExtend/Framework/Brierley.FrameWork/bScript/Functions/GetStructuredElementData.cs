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
    /// The GetStructuredElementData function returns one or more rows of content element data that matches the 
    /// provided criteria.  This function supports comparison on one attribute field only.
    /// </summary>
    /// <example>
    ///     Usage: GetStructuredElementData('StructuredElementName','AttributeName',AttributeValue,UseDateConstraint,RowIndex)
    /// </example>
    /// <remarks>
    /// StructuredElementName must be the name of a valid structured content element.
    /// AttributeName must be the name of a valid attribute in the specified structured element.  It can also be a global attribute.
    /// AttributeValue must be a valid bScript expression that evaluates to a string that can be compared to the value of the specified attribute.
    /// UseDateConstraint is a boolean that when true only returns non-expired content.
    /// RowIndex must be either a number or a bScript expression that evaluates to a number.  The number indicates the zero-based index 
    /// of which row of data to return, or when -1 all rows will be returned.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns one or more rows of content element data that match the provided criteria. This function supports comparison on one attribute field only.", 
		DisplayName = "GetStructuredElementData", 
		ExcludeContext = ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Content, 
		ExpressionReturns = ExpressionApplications.Objects)]
	public class GetStructuredElementData : UnaryOperation
    {
        private const string _className = "GetStructuredElementData";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LOYALTYNAVIGATOR);

        private Expression _structuredElementName = null;
        private Expression _attributeName = null;
        private Expression _attributeValueExpression = null;
        bool _useDateConstraint = true;
        private Expression _rowIndexExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetStructuredElementData()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public GetStructuredElementData(Expression rhs)
            : base("GetStructuredElementData", rhs)
        {
            string methodName = "GetStructuredElementData";
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 5)
            {
                _structuredElementName = ((ParameterList)rhs).Expressions[0];
                _attributeName = ((ParameterList)rhs).Expressions[1];
                _attributeValueExpression = ((ParameterList)rhs).Expressions[2];
                _useDateConstraint = bool.Parse(((ParameterList)rhs).Expressions[3].ToString());
                _rowIndexExpression = ((ParameterList)rhs).Expressions[4];
                return;
            }
            string msg = "Invalid Function Call: Wrong number of arguments passed to StructuredElementData.";
            _logger.Error(_className, methodName, msg);
            throw new CRMException(msg);
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetStructuredElementData('StructuredElementName','AttributeName',AttributeValue,DateConstraint,RowIndex)";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">A context object used for evaluating argument expressions</param>
        /// <returns>A StructuredDataRows object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            string methodName = "evaluate";
            _logger.Debug(_className, methodName, string.Format("Evaluating attribute '{0}', element '{1}'.", _attributeName.evaluate(contextObject).ToString(), _structuredElementName.evaluate(contextObject).ToString()));

            // Evaluate attribute value expression
            object obj = _attributeValueExpression.evaluate(contextObject);
            if (obj == null)
                throw new CRMException("GetStructuredElementData: error evaluating AttributeValue expression");
            string attrValue = obj.ToString();

            // Evaluate row index expression (row index can be -1)
            obj = _rowIndexExpression.evaluate(contextObject);
            if (obj == null)
                throw new CRMException("GetStructuredElementData: error evaluating RowIndex expression");
            int rowIndex = StringUtils.FriendlyInt32(obj, -2);
            if (rowIndex == -2)
                throw new CRMException(string.Format("GetStructuredElementData: RowIndex evaluates to invalid value: '{0}'", obj.ToString()));

            // Get the data
			using (var cms = LWDataServiceUtil.ContentServiceInstance())
			{
				StructuredContentElement element = cms.GetContentElement(_structuredElementName.evaluate(contextObject).ToString());
				if (element == null)
					throw new CRMException(string.Format("Invalid element '{0}')", _structuredElementName.evaluate(contextObject).ToString()));
				StructuredContentAttribute attr = cms.GetAttribute(_structuredElementName.evaluate(contextObject).ToString(), _attributeName.evaluate(contextObject).ToString());
				if (attr == null)
					throw new CRMException(string.Format("Invalid attribute '{0}' for element '{1}'", _attributeName.evaluate(contextObject).ToString(), _structuredElementName.evaluate(contextObject).ToString()));
				DataTable allRows = cms.GetDataRows(-1, element.ID, attr, attrValue, _useDateConstraint);

				// Package and return the result
				StructuredDataRows result = new StructuredDataRows();
				result.Table = allRows;
				result.CurrentRowIndex = rowIndex;
				return result;
			}
        }

        /// <summary>
        /// Parse the expression for meta data.  Used in LoyaltyNavigator to determine which attributes need
        /// to be provided in order to render a page during preview.
        /// </summary>
        /// <returns>semicolon separated list of attributes or empty string if no metadata</returns>
        public override string parseMetaData()
        {
            string meta;
            if (_attributeValueExpression != null && (_attributeValueExpression is UnaryOperation || _attributeValueExpression is BinaryOperation))
            {
                meta = _attributeValueExpression.parseMetaData();
            }
            else
            {
                meta = "";
            }
            return meta;
        }
    }
}
