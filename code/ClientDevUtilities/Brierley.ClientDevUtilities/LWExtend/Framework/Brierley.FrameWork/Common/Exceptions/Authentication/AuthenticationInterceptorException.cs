using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions.Authentication
{
	public class AuthenticationInterceptorException : LWException
	{
		public AuthenticationInterceptorException(string message) 
			: base(message) 
		{ 
		}

        public AuthenticationInterceptorException(string message, Exception exception)
            : base(message, exception)
        {
        }
	}
}
