//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.bScript
{
    public interface IExpression
    {
        /// <summary>
        /// Returns the syntax string implemented by this class.
        /// </summary>
        string Syntax { get; }        

        /// <summary>
        /// Performs an evaluation on the expression tree.
        /// </summary>
        /// <param name="contextObject">The current execution context of the expression</param>
        /// <returns>An object</returns>
        object evaluate(ContextObject contextObject);
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string parseMetaData();        
    }
}
