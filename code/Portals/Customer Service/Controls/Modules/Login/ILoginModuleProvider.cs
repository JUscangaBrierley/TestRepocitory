using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.Login
{
	public interface ILoginModuleProvider
	{
		string GetSuccessURL(LoginModuleConfig config);
		string GetRegistrationURL(LoginModuleConfig config);
		string GetSsoSuccessURL(LoginModuleConfig config);
		string GetErrorURL(LoginModuleConfig config, string[] errParams);
		string GetForgotPasswordURL(LoginModuleConfig config);
		string SetRegistrationDatahookCookie(string cookie, string socialName, object socialUserObject);
	}
}
