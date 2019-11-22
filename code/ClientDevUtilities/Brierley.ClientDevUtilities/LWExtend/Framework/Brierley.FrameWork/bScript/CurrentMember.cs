using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CurrentMember : Expression
    {
        private const string _className = "CurrentMember";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        
        /// <summary>
        /// Internal constructor
        /// </summary>        
        internal CurrentMember()
            : base()
        {            
        }
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            return ResolveMember(contextObject.Owner);
        }
    }
}
