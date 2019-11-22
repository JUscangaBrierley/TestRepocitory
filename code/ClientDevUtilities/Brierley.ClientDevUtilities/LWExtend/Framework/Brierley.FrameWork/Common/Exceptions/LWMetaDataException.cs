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
    public class LWMetaDataException : LWException
    {
		public LWMetaDataException() : base() { }
        public LWMetaDataException(string errorMessage) : base(errorMessage) { }
        public LWMetaDataException(string errorMessage, Exception ex) : base(errorMessage, ex) { }
    }
}
