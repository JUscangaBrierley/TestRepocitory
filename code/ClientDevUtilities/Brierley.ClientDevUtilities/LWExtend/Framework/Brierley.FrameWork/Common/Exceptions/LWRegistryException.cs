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
    public class LWRegistryException : LWException
    {
		public LWRegistryException() : base() { }
        public LWRegistryException(string errorMessage) : base(errorMessage) { }
        public LWRegistryException(string errorMessage,Exception ex) : base(errorMessage,ex) {}
    }
}
