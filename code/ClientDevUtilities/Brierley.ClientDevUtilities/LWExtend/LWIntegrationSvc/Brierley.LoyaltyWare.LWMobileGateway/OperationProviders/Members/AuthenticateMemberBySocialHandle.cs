using System;
using System.Net;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.WebFrameWork.SocialNetwork;
using Newtonsoft.Json;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
	public class AuthenticateMemberBySocialHandle : SocialOperationProvider
	{
		private const string _className = "AuthenticateMember";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
		private static WcfAuthenticationToken.AuthenticationScheme? _scheme = null;

		protected WcfAuthenticationToken.AuthenticationScheme Scheme
		{
			get
			{
				if (_scheme == null)
				{
					_scheme = (WcfAuthenticationToken.AuthenticationScheme)Enum.Parse(typeof(WcfAuthenticationToken.AuthenticationScheme), GetFunctionParameter("AuthenticationScheme"));
				}
				return _scheme.Value;
			}
		}

		public AuthenticateMemberBySocialHandle()
			: base("AuthenticateMemberBySocialHandle")
		{
		}

		public override void Initialize(string opName, MobileGatewayDirectives config, System.Collections.Specialized.NameValueCollection functionProviderParms)
		{
			base.Initialize(opName, config, functionProviderParms);
		}

		public override object Invoke(string source, WcfAuthenticationToken notoken, object[] parms)
		{
			string methodName = "Invoke";

			if (parms == null || parms.Length < 6)
			{
				string errMsg = "Invalid parameters provided for authentication.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			LoginStatusEnum loginStatus = LoginStatusEnum.Failure;

			string provider = (string)parms[0];
			string uid = (string)parms[1];
			string authToken = (string)parms[2];
			string secret = (string)parms[3];
			string deviceType = (string)parms[4];
			string version = (string)parms[5];

			SocialNetworkProviderType providerType = ProviderTypeFromString(provider);
			if (providerType == SocialNetworkProviderType.None)
			{
				SetResponseCode(System.Net.HttpStatusCode.NotFound);
				return null;
			}

			//cacheKey is a hash of provider UID and the auth token. Without this, we'd have to verify the auth token with the provider on each call. Otherwise, 
			//one need only know a member's UID in order to hijack their session. With it, a successful authentication requires either a new auth token that is 
			//validated with the social provider or a re-authentication using the exact same auth token from a previous authentication (token retrieved from cache).
			string cacheKey = CryptoUtil.HashToString(uid + authToken);
			WcfAuthenticationToken token = WcfAuthenticationToken.GetAuthTokenFromCache(WcfAuthenticationToken.AuthenticationScheme.SocialNetwork, provider, cacheKey);
			if (token == null)
			{
				var socnet = LoyaltyService.MemberSocNetDao.RetrieveByProviderUId(providerType, uid);
				if (socnet == null)
				{
					SetResponseCode(System.Net.HttpStatusCode.NotFound);
					return null;
				}

				var member = LoyaltyService.LoadMemberFromIPCode(socnet.MemberId);
				if (member == null)
				{
					throw new Exception(string.Format("Found {0} MemberSocNet {1} with orphan member id {2}", providerType.ToString(), uid, socnet.MemberId));
				}

				//we have a member. Verify the provider's token before associating
				switch (providerType)
				{
					case SocialNetworkProviderType.Facebook:
						try
						{
							FacebookProvider fbProvider = new FacebookProvider(FacebookSettings);
							fbProvider.Token = authToken;
							var user = fbProvider.GetUser();
							if (user == null)
							{
								throw new LWException("Invalid facebook token");
							}
							if (user.id != uid)
							{
								throw new LWException("Invalid facebook uid");
							}
							socnet.Properties = JsonConvert.SerializeObject(user);
						}
						catch (WebException ex)
						{
							throw;
						}
						break;
					case SocialNetworkProviderType.Twitter:
						{
							TwitterProvider twProvider = new TwitterProvider(TwitterSettings);

							twProvider.AccessToken = new Brierley.WebFrameWork.SocialNetwork.TwitterProvider.TwitterAccessToken()
							{
								oauth_token = authToken,
								oauth_token_secret = secret,
								user_id = uid,
								//we don't know this yet
								screen_name = string.Empty
							};

							TWUser user = twProvider.GetUser();

							if (user == null)
							{
								throw new LWException("Invalid twitter token");
							}
							if (user.id_str != uid)
							{
								throw new LWException("Invalid twitter uid");
							}
							
							//we know the screen name now
							twProvider.AccessToken.screen_name = user.screen_name;
							
							socnet.Properties = JsonConvert.SerializeObject(twProvider.AccessToken);
						}
						break;
					case SocialNetworkProviderType.Google:
						{
							GooglePlusProvider gProvider = new GooglePlusProvider(GoogleSettings);
							gProvider.Token = JsonConvert.DeserializeObject<Brierley.WebFrameWork.SocialNetwork.GooglePlusProvider.GPAccessToken>(authToken);
							var user = gProvider.GetUser();
							if (user == null)
							{
								throw new LWException("Invalid google plus token");
							}
							if (user.id != uid)
							{
								throw new LWException("Invalid google plus uid");
							}
							socnet.Properties = JsonConvert.SerializeObject(user);
						}
						break;
				}

				string username = string.Empty;
				string password = string.Empty;

				//mark - old token spot
				//no interceptor, for now
				//IAuthenticateMemberInterceptor interceptor = null;
				//MobileGatewayDirectives.APIOperationDirective opDirective = Config.GetOperationDirectiveByName(Name) as MobileGatewayDirectives.APIOperationDirective;
				//if (opDirective.Interceptor != null)
				//{
				//	interceptor = InterceptorUtil.GetInterceptor(opDirective.Interceptor) as IAuthenticateMemberInterceptor;
				//}

				MGMemberUtils.Authenticate(socnet, cacheKey, deviceType, version, ref token, out loginStatus);
				if (token != null)
				{
					token.MobileDeviceType = deviceType;
					token.MobileDeviceVersion = version;

					_logger.Debug(
						_className,
						methodName,
						string.Format(
							"User with ipcode '{0}' has been authenticated and token {1} has been cached.",
							token.CachedMember != null ? token.CachedMember.IpCode.ToString() : "unknown member",
							token.TokenId));

					LoyaltyService.UpdateMemberSocNet(socnet);
				}
			}
			else
			{
				_logger.Debug(
					_className,
					methodName,
					string.Format(
						"Cached authentication token for '{0}' found",
						token.CachedMember != null ? token.CachedMember.IpCode.ToString() : "unknown member"));

				if (token.CachedMember.PasswordChangeRequired)
				{
					loginStatus = LoginStatusEnum.PasswordResetRequired;
				}
				else
				{
					loginStatus = LoginStatusEnum.Success;
				}
			}

			return new MGAuthenticateMember()
			{
				Token = token != null ? token.TokenId : null,
				LoginStatus = loginStatus,
				StatusText = Enum.GetName(typeof(LoginStatusEnum), loginStatus)
			};
		}
	}
}