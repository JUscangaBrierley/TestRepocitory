using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork.Common;
using Brierley.WebFrameWork.SocialNetwork;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders
{
	public abstract class SocialOperationProvider : OperationProviderBase
	{
		protected static FacebookProviderParams FacebookSettings = null;
		protected static TwitterProviderParams TwitterSettings = null;
		protected static GooglePlusProviderParams GoogleSettings = null;

		protected SocialOperationProvider(string name) : base(name)
		{
		}

		public override void Initialize(string opName, MobileGatewayDirectives config, System.Collections.Specialized.NameValueCollection functionProviderParms)
		{
			base.Initialize(opName, config, functionProviderParms);

			var apps = SocialService.GetSocialMediaApps();
			if (apps == null)
			{
				throw new Exception("AssociateSocialHandle cannot be invoked because no social media apps have been defined.");
			}

			string facebookAppName = GetFunctionParameter("FacebookAppName");
			if (!string.IsNullOrEmpty(facebookAppName))
			{
				var facebook = apps.FirstOrDefault(o => o.Name == facebookAppName && o.Type == SocialNetworkProviderType.Facebook);
				if (facebook == null)
				{
					throw new Exception(string.Format("Invalid app name for Facebook {0}", facebookAppName));
				}
				FacebookSettings = new FacebookProviderParams()
				{
					AuthCallbackUrl = facebook.CallbackUrl,
					ConsumerKey = facebook.ConsumerKey,
					ConsumerSecret = facebook.ConsumerSecret
				};
			}

			string twitterAppName = GetFunctionParameter("TwitterAppName");
			if (!string.IsNullOrEmpty(twitterAppName))
			{
				var twitter = apps.FirstOrDefault(o => o.Name == twitterAppName && o.Type == SocialNetworkProviderType.Twitter);
				if (twitter == null)
				{
					throw new Exception(string.Format("Invalid app name for Twitter {0}", twitterAppName));
				}
				TwitterSettings = new TwitterProviderParams(twitter.ConsumerKey, twitter.ConsumerSecret, twitter.CallbackUrl);
			}

			string googleAppName = GetFunctionParameter("GoogleAppName");
			if (!string.IsNullOrEmpty(googleAppName))
			{
				var google = apps.FirstOrDefault(o => o.Name == googleAppName && (o.Type == SocialNetworkProviderType.Google || o.Type == SocialNetworkProviderType.Google));
				if (google == null)
				{
					throw new Exception(string.Format("Invalid app name for Google {0}", googleAppName));
				}
				GoogleSettings = new GooglePlusProviderParams()
				{
					AuthCallbackUrl = google.CallbackUrl,
					ClientID = google.ConsumerKey,
					ClientSecret = google.ConsumerSecret
				};
			}			
		}

		protected SocialNetworkProviderType ProviderTypeFromString(string provider)
		{
			SocialNetworkProviderType ret = SocialNetworkProviderType.None;
			switch (provider.ToLower())
			{
				case "facebook":
					ret = SocialNetworkProviderType.Facebook;
					if (FacebookSettings == null)
					{
						throw new Exception("Invalid function call. FacebookAppName has not been defined.");
					}
					break;
				case "twitter":
					ret = SocialNetworkProviderType.Twitter;
					if (TwitterSettings == null)
					{
						throw new Exception("Invalid function call. TwitterAppName has not been defined.");
					}
					break;
				case "google":		//forgiving the use of googleplus instead of google for now
				case "googleplus":
					ret = SocialNetworkProviderType.Google;
					if (GoogleSettings == null)
					{
						throw new Exception("Invalid function call. GoogleAppName has not been defined.");
					}
					break;
			}
			return ret;
		}
	}
}