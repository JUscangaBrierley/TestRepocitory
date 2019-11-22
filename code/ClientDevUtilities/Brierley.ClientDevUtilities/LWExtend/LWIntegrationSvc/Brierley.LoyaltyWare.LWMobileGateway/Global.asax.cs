using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Brierley.WebFrameWork.Application;

namespace Brierley.LoyaltyWare.LWMobileGateway
{
	public class Global : LWHttpApplication
	{
		private static List<string> _allowedOrigins = null;

		private static List<string> AllowedOrigins
		{
			get
			{
				if (_allowedOrigins == null)
				{
					string allowedOrigins = System.Configuration.ConfigurationManager.AppSettings["AllowedOrigins"];
					if (!string.IsNullOrEmpty(allowedOrigins))
					{
						_allowedOrigins = allowedOrigins.ToLower().Split(';').ToList();
					}
					if (_allowedOrigins == null)
					{
						_allowedOrigins = new List<string>();
					}
				}
				return _allowedOrigins;
			}
		}

		protected void Application_Start(object sender, EventArgs e)
		{

		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			if (AllowedOrigins != null && AllowedOrigins.Count > 0)
			{
				//we're allowing a CORS origin, so we need to respond with the origin named as allowed in the header and also allow options pre-flight calls
				var context = HttpContext.Current;
				string origin = context.Request.Headers["Origin"];
				if (string.IsNullOrEmpty(origin) && context.Request.UrlReferrer != null)
				{
					origin = context.Request.UrlReferrer.AbsoluteUri;
					string absolutePath = context.Request.UrlReferrer.AbsolutePath;
					if (!string.IsNullOrEmpty(absolutePath))
					{
						origin = origin.Replace(absolutePath, "");
					}
				}
				if (!string.IsNullOrEmpty(origin) && AllowedOrigins.Contains(origin))
				{
					context.Response.AppendHeader("Access-Control-Allow-Origin", origin);
					context.Response.AppendHeader("Access-Control-Allow-Methods", "GET,POST,PUT");
					context.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
					context.Response.AppendHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");

					if (context.Request.HttpMethod == "OPTIONS")
					{
						HttpContext.Current.Response.End();
					}
				}
			}

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}

		protected void Application_Error(object sender, EventArgs e)
		{
		}

		protected void Session_End(object sender, EventArgs e)
		{
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}
	}
}