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
    public class LWSerializationException : LWException
    {
		public LWSerializationException() : base() { }
        public LWSerializationException(string msg) : base(msg) { }
        public LWSerializationException(string msg, Exception inExcp) : base(msg, inExcp) { }
    }
}
