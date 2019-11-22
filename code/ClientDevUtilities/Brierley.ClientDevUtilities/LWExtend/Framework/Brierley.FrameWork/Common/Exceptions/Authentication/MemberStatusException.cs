using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions.Authentication
{
	public class MemberStatusException : AuthenticationException
	{
		public MemberStatusEnum MemberStatus { get; set; }

		public MemberStatusException(MemberStatusEnum memberStatus) 
			: base(string.Format("Account {0}.", memberStatus)) 
		{ 
			MemberStatus = memberStatus;
		}
	}
}
