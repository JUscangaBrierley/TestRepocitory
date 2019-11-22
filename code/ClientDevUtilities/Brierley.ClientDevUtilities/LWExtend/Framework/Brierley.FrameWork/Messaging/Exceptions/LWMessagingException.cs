//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.Messaging.Exceptions
{
	[Serializable]
    public class LWMessagingException : LWException
    {
		public LWMessagingException() : base() { }
        public LWMessagingException(string msg) : base(msg) { }
        public LWMessagingException(string msg, Exception inExcp) : base(msg, inExcp) { }
    }
}
