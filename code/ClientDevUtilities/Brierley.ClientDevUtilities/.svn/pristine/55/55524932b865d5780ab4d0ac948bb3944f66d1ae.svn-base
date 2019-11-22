//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Email
{
	public static class EmailUtil
	{
        private const string _className = "EmailUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_EMAIL);

		public static string EmailValidationRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

		public static bool ValidateEmailAddress(string EmailAddress)
		{
			return Regex.IsMatch(EmailAddress, EmailValidationRegex);
		}

		public static string CombineXsltForLinkDisplay(string EmailHtml, string EmailText)
		{
			if (!string.IsNullOrEmpty(EmailText))
			{
				return "HTML:\r\n\r\n" + EmailHtml + "\r\n\r\n\r\nText:\r\n\r\n" + EmailText;
			}
			else
			{
				return EmailHtml;
			}
		}
    }
}
