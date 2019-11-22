//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// This function gets the current index from the context object.  This function is designed to be used in 
    /// conjunction with the InitializeDataIterator function and AdvanceDataRowIndex.
    /// 
    /// </summary>
	[ExpressionContext(Description = "Gets the current index from the context object. This function is designed to be used in conjunction with the InitializeDataIterator function and AdvanceDataRowIndex.", 
		DisplayName = "GetCurrentDataRowIndex", 
		ExcludeContext = ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Content, 
		ExpressionReturns = ExpressionApplications.Numbers)]
	public class GetCurrentDataRowIndex : UnaryOperation
    {
        //private static string className = "GetCurrentDataRowIndex";
        
        /// <summary>
        /// Internal Constructor
        /// </summary>
        public GetCurrentDataRowIndex()
            : base("GetNextDataRowIndex", null)
        {            
        }

        /// <summary>
        /// 
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetNextDataRowIndex()";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            return contextObject.CurrentIndex;                        
        }        
    }
}
