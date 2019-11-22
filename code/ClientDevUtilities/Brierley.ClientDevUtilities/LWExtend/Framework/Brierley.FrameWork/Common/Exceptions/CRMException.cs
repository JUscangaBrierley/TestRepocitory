using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Common.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
	[Serializable]
	public class CRMException : LWException
    {
        private const string _className = "CRMException";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		//private string stackTrace = string.Empty;
		//private string typeName = string.Empty;
        private string methodName = string.Empty;
		//private string exceptionName = string.Empty;
		//private string exceptionMessage = string.Empty;

		public CRMException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        public CRMException(string Message)
            : base(Message)
        {
			//string AssemblyName = System.Reflection.Assembly.GetCallingAssembly().FullName;
			//if (this.StackTrace != null) { stackTrace = this.StackTrace; }
			//if (this.Source != null) { typeName = this.Source; }
            if (this.TargetSite != null) { this.methodName = this.TargetSite.Name; }
            _logger.Error(_className, methodName, this.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="InnerException"></param>
        public CRMException(string Message, System.Exception InnerException)
            : base(Message, InnerException)
        {
			//string AssemblyName = System.Reflection.Assembly.GetCallingAssembly().FullName;
			//if (this.StackTrace != null) { stackTrace = this.StackTrace; }
			//if (this.Source != null) { typeName = this.Source; }
            if (this.TargetSite != null) { this.methodName = this.TargetSite.Name; }
            _logger.Error(_className, methodName, this.Message);
        }
    }
}
