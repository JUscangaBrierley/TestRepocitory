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
    /// The GetRowIndexByAttributeValue function returns the index of the first row that has the 
    /// requested attribuute value. 
    /// </summary>
    /// <example>
    ///     Usage : GetRowIndexByAttributeValue('AttributeName',AttributeValue)
    /// </example>
    /// <remarks>
    /// AttributeName must be the name of a valid attribute in the specified structured element.
    /// AttributeValue - must be bScript expression that evaluates to a string.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the index of the first row that has the requested attribuute value.", 
		DisplayName = "GetRowIndexByAttributeValue", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Numbers)]
	public class GetRowIndexByAttributeValue : UnaryOperation
    {
        //private static string _className = "GetRowIndexByAttributeValue";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LOYALTYNAVIGATOR);

        private Expression _attributeName = null;
        private Expression _attributeValueExpr = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetRowIndexByAttributeValue()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public GetRowIndexByAttributeValue(Expression rhs)
            : base("GetRowIndexByAttributeValue", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 2)
            {
				_attributeName = ((ParameterList)rhs).Expressions[0];
                _attributeValueExpr = ((ParameterList)rhs).Expressions[1];
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetRowIndexByAttributeValue.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetRowIndexByAttributeValue('AttributeName',AttributeValue)";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">An instance of ContextObject</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            if (contextObject.Environment == null) return null;

            Dictionary<string, object> env = contextObject.Environment as Dictionary<string, object>;
            if (env == null) return null;

            if (env.ContainsKey("table"))
            {
                DataTable table = env["table"] as DataTable;
                if (table == null || table.Rows == null || table.Rows.Count < 1 || !table.Columns.Contains(_attributeName.evaluate(contextObject).ToString())) return null;

                string attributeValue = (string)_attributeValueExpr.evaluate(contextObject);
                string selectExpr = string.Format("[{0}] = '{1}'", _attributeName.evaluate(contextObject).ToString(), attributeValue);
                DataRow[] rows = table.Select(selectExpr);
                if (rows == null || rows.Length < 1) return -1;
                int index = table.Rows.IndexOf(rows[0]);
                return index;
            }
            else
            {
                return null;
            }
        }
    }
}
