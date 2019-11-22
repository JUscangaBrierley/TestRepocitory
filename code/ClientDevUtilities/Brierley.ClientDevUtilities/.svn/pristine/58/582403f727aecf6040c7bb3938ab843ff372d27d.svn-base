using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions.Authentication
{
	public class InvalidMemberIdentityException : AuthenticationException
	{
		public InvalidMemberIdentityException(AuthenticationFields identityType, string identity) 
			: base(string.Format("Unable to find '{0}' ({1})", identity, identityType)) 
		{ 
		}
	}
}
