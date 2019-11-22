using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Brierley.FrameWork.Common.Security;

namespace Brierley.FrameWork.Common.Exceptions
{
	public class InvalidSFTPKeyException : LWException
	{
		public string ErrorHtml { get; set; }
		public string ErrorText { get; set; }
		public string ErrorXml { get; set; }

		public InvalidSFTPKeyException(string message, string errorHtml, string errorText, string errorXml)
			: base(message)
		{
			ErrorHtml = errorHtml;
			ErrorText = errorText;
			ErrorXml = errorXml;
		}
	}
}
