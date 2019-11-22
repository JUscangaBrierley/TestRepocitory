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

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// This function advances the current index int the context object.  This function is designed to be used in 
    /// conjunction with the InitializeDataIterator function and GetCurrentDataRowIndex.
    /// 
    /// </summary>
	[ExpressionContext(Description = "Advances the current index int the context object.  This function is designed to be used in conjunction with the InitializeDataIterator function and GetCurrentDataRowIndex.", 
		DisplayName = "AdvanceDataRowIndex", 
		ExcludeContext = ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Content, 
		ExpressionReturns = 0)]
	public class AdvanceDataRowIndex : UnaryOperation
    {
        private const string _className = "AdvanceDataRowIndex";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        
        /// <summary>
        /// Internal Constructor
        /// </summary>
        public AdvanceDataRowIndex()
            : base("AdvanceDataRowIndex", null)
        {            
        }

        /// <summary>
        /// 
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "AdvanceDataRowIndex()";
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

            int currentIndex = contextObject.CurrentIndex + 1;
            if (currentIndex < contextObject.StartIndex)
            {
                msg = string.Format("Invalid index {0} calculated.", currentIndex);
                CRMException exception = new CRMException(msg);
                _logger.Error(_className, methodName, "Error get next loop index.", exception);
                throw exception;
            }

            if (currentIndex >= contextObject.EndIndex)
            {
                return false;
            }

            contextObject.CurrentIndex = currentIndex;

            return true;                        
        }        
    }
}
