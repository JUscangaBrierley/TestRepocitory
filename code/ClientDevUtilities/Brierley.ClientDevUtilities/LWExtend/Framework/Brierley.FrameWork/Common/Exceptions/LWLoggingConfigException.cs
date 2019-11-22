//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions
{
    [Serializable]
    public class LWLoggingConfigException : LWException
    {
		public LWLoggingConfigException() : base() { }
        public LWLoggingConfigException(string errorMessage) : base(errorMessage) { }
        public LWLoggingConfigException(string errorMessage, Exception ex) : base(errorMessage, ex) { }
    }
}
