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
    public class LWCSException : LWException
    {
		public LWCSException() : base() { }
        public LWCSException(string errorMessage) : base(errorMessage) { }
        public LWCSException(string errorMessage, Exception ex) : base(errorMessage, ex) { }
    }
}
