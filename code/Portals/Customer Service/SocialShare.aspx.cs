using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Util;

public partial class SocialShare : System.Web.UI.Page
{
	private const string _className = "SocialShare.aspx";
	private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

	protected override void OnInit(EventArgs e)
	{
		const string methodName = "OnInit";
		base.OnInit(e);

		try
		{
			string encryptionKey = CryptoUtil.EncodeUTF8("sOc14l5hAr3");
			Response.ContentType = "text/html";

			var id = Request.QueryString["id"];
			if (!string.IsNullOrEmpty(id))
			{
				id = id.Replace(' ', '+');
				string query = CryptoUtil.Decrypt(encryptionKey, id);
				NameValueCollection args = HttpUtility.ParseQueryString(query);
				switch (args["command"])
				{
					case "reward":
						SendRewardMarkup(args, Request, Response);
						break;

					case "coupon":
						SendCouponMarkup(args, Request, Response);
						break;
				}
			}
		}
		catch (Exception ex)
		{
			_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
		}
		finally
		{
			Response.End();
		}
	}

	private static void SendRewardMarkup(NameValueCollection args, HttpRequest Request, HttpResponse Response) 
	{
		long rewardID = StringUtils.FriendlyInt64(args["rewardID"]);
		if (rewardID < 0)
			return;

		using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
		{
			MemberReward memberReward = loyalty.GetMemberReward(rewardID);
			string markup = GetMarkup(Request, memberReward.RewardDef.Name, memberReward.RewardDef.ShortDescription, memberReward.RewardDef.MediumImageFile);
			Response.Write(markup);
		}
	}

	private static void SendCouponMarkup(NameValueCollection args, HttpRequest Request, HttpResponse Response)
	{
		long couponID = StringUtils.FriendlyInt64(args["couponID"]);
		if (couponID < 0)
			return;

		using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
		using (var content = LWDataServiceUtil.ContentServiceInstance())
		{
			MemberCoupon memberCoupon = loyalty.GetMemberCoupon(couponID);
			CouponDef couponDef = content.GetCouponDef(memberCoupon.CouponDefId);
			string markup = GetMarkup(Request, couponDef.Name, couponDef.ShortDescription, couponDef.LogoFileName);
			Response.Write(markup);
		}
	}

	private static string GetMarkup(HttpRequest Request, string title, string description, string imageFileName)
	{
		bool forceSSL = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("ForceSSL"), false);
		string imageUrl = GetImageUrl(imageFileName);
		string imageSecureUrl = imageUrl.Replace("http:", "https:");
		string pageUrl = Request.Url.AbsoluteUri;

		string markup = string.Format(@"<!DOCTYPE html>
<html>
<head>
	<meta name=""og:image"" content=""{2}"" />
	<meta name=""og:image:secure_url"" content=""{3}"" />
	<meta name=""og:title"" content=""{0}"" />
	<meta name=""og:type"" content=""og:product"" />
	<meta name=""og:url"" content=""{4}"" />
	<link href=""{4}"" rel=""canonical"" />
	<title>{0}</title>
</head>
<body>
<table>
	<tr>
		<td>
			<img src=""{5}"" />
		</td>
		<td>
			<h1>{0}</h1>
			<p>{1}</p>
		</td>
	</tr>
</table>
</body>
</html>
", title, description, imageUrl, imageSecureUrl, pageUrl, forceSSL ? imageSecureUrl : imageUrl);
		return markup;
	}

	private static string GetImageUrl(string imageFileName)
	{
		string result = string.Empty;
		if (imageFileName.StartsWith("http"))
		{
			result = imageFileName;
		}
		else
		{
			string imageBaseURL = LWConfigurationUtil.GetConfigurationValue("LWContentRootURL");
			if (!imageBaseURL.EndsWith("/"))
			{
				imageBaseURL += "/";
			}
			imageBaseURL += PortalState.Config.Organization + "/";

			result = imageBaseURL + imageFileName;
		}
		return result;
	}
}