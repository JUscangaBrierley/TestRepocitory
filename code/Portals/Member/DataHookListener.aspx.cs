using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

public partial class DataHookListener : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Response.Clear();
		if (!string.IsNullOrEmpty(Request["ClearHooks"]))
		{
			PortalState.SetCookie(CSDataHookConfig.DataHookCookieName, "expired", true, DateTime.Now);
		}
		else
		{
			string cookie = string.Empty;

			if (Page.Request.HttpMethod == "POST")
			{
				cookie = Request.Form.ToString();
			}
			else if (Page.Request.HttpMethod == "GET")
			{
				cookie = Request.QueryString.ToString();
			}
			PortalState.SetCookie(CSDataHookConfig.DataHookCookieName, cookie, true);
		}		
		if (Request.AcceptTypes.Contains("application/json"))
		{
			Response.Write("{\"Status\":\"Success\"}");
		}
		Response.End();
    }
}