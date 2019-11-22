using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions.Authentication
{
	public class CSAgentStatusException : AuthenticationException
	{
		public AgentAccountStatus CSAgentStatus { get; set; }

		public CSAgentStatusException(AgentAccountStatus csagentStatus)
			: base(string.Format("Account {0}.", csagentStatus)) 
		{
			CSAgentStatus = csagentStatus;
		}
	}
}
