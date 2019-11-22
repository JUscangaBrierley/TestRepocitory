//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// This function intializes an iterator for content data element so that loops can be performed over 
    /// multiple content rows.
    /// </summary>
	[ExpressionContext(Description = "Intializes an iterator for content data element so that loops can be performed over multiple content rows.", 
		DisplayName = "InitializeDataIterator", 
		ExcludeContext = ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Content, 
		ExpressionReturns = 0)]
	public class InitializeDataIterator : UnaryOperation
    {
        private const string _className = "InitializeDataIterator";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private Expression _startIndexExpr = null;
        private Expression _endIndexExpr = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public InitializeDataIterator()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        public InitializeDataIterator(Expression rhs)
            : base("InitializeDataIterator", rhs)
        {
			ParameterList plist = rhs as ParameterList;
			//ContextObject cObj = new ContextObject();
            if (rhs != null)
            {
                if (rhs.GetType() != typeof(ParameterList))
                {
                    _startIndexExpr = rhs;
                }
                else if (plist.Expressions.Length == 2)
                {
                    _startIndexExpr = plist.Expressions[0];
                    _endIndexExpr = plist.Expressions[1];
                }
                else
                {
                    throw new CRMException("Invalid Function Call: Wrong number of arguments passed to InitializeDataIterator.");
                }
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "InitializeDataIterator([StartIndex[,EndIndex]])";
            }
        }
        
        /// <summary>
        /// Performs the operation defined by this function.  If the EndIndex is omitted, then the 
        /// ContextObject argument should contain a StructuredDataRows object in it's Environment 
        /// attribute so that EndIndex can be determined.
        /// </summary>
        /// <param name="contextObject">context object used for evaluating argument expressions</param>
        /// <returns>integer indicating number of rows remaining to process</returns>
        public override object evaluate(ContextObject contextObject)
        {
            string methodName = "evaluate";
			//string msg = "";

            // Determine start index, default is 0
            int startIndex = 0;
            if (_startIndexExpr != null)
            {
                object obj = _startIndexExpr.evaluate(contextObject);
                startIndex = StringUtils.FriendlyInt32(obj, -1);
                if (startIndex == -1)
                {
                    CRMException exception = new CRMException(string.Format("Invalid StartIndex provided: {0}", obj));
                    _logger.Error(_className, methodName, "Error initializing data iterator.", exception);
                    throw exception;
                }
            }

            // Determine end index, default is last row in contextObject.Environment as StructuredDataRows
            int endIndex = -1;
            if (_endIndexExpr == null)
            {
                if (contextObject.Environment != null)
                {
                    if (contextObject.Environment.ContainsKey("StructuredDataRows"))
                    {
						endIndex = ((StructuredDataRows)contextObject.Environment["StructuredDataRows"]).Count;
                    }
                    else
                    {
                        CRMException exception = new CRMException(string.Format("Can't determine EndIndex: context.Environment is not a StructuredDataRows."));
                        _logger.Error(_className, methodName, "Error initializing data iterator.", exception);
                        throw exception;
                    }
                }
                else
                {
                    CRMException exception = new CRMException(string.Format("Can't determine EndIndex: contextObject.Environment is null."));
                    _logger.Error(_className, methodName, "Error initializing data iterator.", exception);
                    throw exception;
                }
            }
            else
            {
                object obj = _endIndexExpr.evaluate(contextObject);
                endIndex = StringUtils.FriendlyInt32(obj, -1);
                if (endIndex == -1)
                {
                    CRMException exception = new CRMException(string.Format("Invalid EndIndex provided: {0}", obj));
                    _logger.Error(_className, methodName, "Error initializing data iterator.", exception);
                    throw exception;
                }
            }

            contextObject.StartIndex = startIndex;
            contextObject.EndIndex = endIndex;
            contextObject.CurrentIndex = startIndex - 1;
            return endIndex - startIndex;
        }
    }
}
