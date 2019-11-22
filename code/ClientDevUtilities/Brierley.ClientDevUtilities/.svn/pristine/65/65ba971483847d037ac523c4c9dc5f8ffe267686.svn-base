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
    public class LWAddressValidationException : LWException
    {
		private int _returnCode;
		private string _dpv;
		private string _errorCode;

		public LWAddressValidationException(int rc,string dpv,string ec) : base() 
		{
			_returnCode = rc;
			_dpv = dpv;
			_errorCode = ec;
		}
        public LWAddressValidationException(int rc,string dpv,string ec, string errorMessage) : base(errorMessage) 
		{
			_returnCode = rc;
			_dpv = dpv;
			_errorCode = ec;
		}
		public LWAddressValidationException(int rc,string dpv,string ec, string errorMessage, Exception ex) : base(errorMessage, ex) 
		{
			_returnCode = rc;
			_dpv = dpv;
			_errorCode = ec;
		}

		public int ReturnCode
		{
			get { return _returnCode; }
		}

		public string Dpv
		{
			get { return _dpv; }
		}

        //public string ErrorCode
        //{
        //    get { return _errorCode; }
        //}
    }
}
