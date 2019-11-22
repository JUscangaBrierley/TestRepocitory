using System;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.WebFrameWork.SocialNetwork;
using Newtonsoft.Json;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
	public class AssociateSocialHandle : SocialOperationProvider
	{
		private const string _className = "AssociateSocialHandle";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public AssociateSocialHandle() : base("AssociateSocialHandle") { }

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			const string methodName = "Invoke";

			if (parms == null || parms.Length < 3)
			{
				string errMsg = "Invalid arguments.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			string provider = (string)parms[0];
			string uid = (string)parms[1];
			string authToken = (string)parms[2];
			string secret = null;
			if (parms.Length > 3)
			{
				secret = (string)parms[3];
			}

			SocialNetworkProviderType providerType = ProviderTypeFromString(provider);

			if (providerType == SocialNetworkProviderType.None)
			{
				SetResponseCode(System.Net.HttpStatusCode.NotFound);
				return null;
			}

			var socnet = LoyaltyService.MemberSocNetDao.RetrieveByProviderUId(providerType, uid);
			if (socnet != null && socnet.MemberId != token.CachedMember.IpCode)
			{
				//this is someone else's socnet entry - not allowed to hijack!
				throw new LWException("The specified Provider UID already exists in another account.");
			}

			if (socnet == null)
			{
				socnet = new MemberSocNet() { ProviderType = providerType, ProviderUID = uid, MemberId = token.CachedMember.IpCode };
			}

			//verify the provider's token before associating
			switch (providerType)
			{
				case SocialNetworkProviderType.Facebook:
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
							throw new LWException("Invalid google token");
						}
						if (user.id != uid)
						{
							throw new LWException("Invalid google uid");
						}
						socnet.Properties = JsonConvert.SerializeObject(user);
					}
					break;
			}

			if (socnet.Id < 1)
			{
				LoyaltyService.CreateMemberSocNet(socnet);
			}
			else
			{
				LoyaltyService.UpdateMemberSocNet(socnet);
			}
			return null;
		}
	}
}