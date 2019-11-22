using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.Common.Exceptions
{
    [Serializable]    
    public class LWOperationInvocationException : LWIntegrationException
    {        
        public string Reason { get; set; }
        public LWOperationInvocationException() : base() { }
        public LWOperationInvocationException(string errorMessage) : base(errorMessage) { }
        public LWOperationInvocationException(string errorMessage, Exception ex) : base(errorMessage, ex) { }
		public LWOperationInvocationException(string errorMessage, int errorCode) : base(errorMessage) 
		{
			ErrorCode = errorCode;
		}
    }
}
