using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.Authentication
{
    public class WcfAuthenticationToken
    {
        public enum AuthenticationScheme { UsernameAndPassword, EmailAndLoyaltyId, SocialNetwork };

        #region Fields
        private string _tokenId = Guid.NewGuid().ToString();        
        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _passwordResetRequired = false;
        private Member _member = null;
        private AuthenticationScheme _authScheme = AuthenticationScheme.UsernameAndPassword;                
        
        private static string Authentication_RG_NAME = "AuthenticationTokens";
        private static string AuthenticationIds_RG_NAME = "AuthenticationTokenIds";
        #endregion

        #region Properties
        public string TokenId
        {
            get { return _tokenId; }
        }
        
        public AuthenticationScheme AuthScheme
        {
            get { return _authScheme; }
        }

        public virtual string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }

        public bool PasswordResetRequired
        {
            get { return _passwordResetRequired; }
        }

        public Member CachedMember
        {
            get { return _member; }            
        }

        public List<MGStoreDef> Top10Stores { get; set; }

        public string MobileDeviceType { get; set; }

        public string MobileDeviceVersion { get; set; }
        #endregion

        #region Proxy Cache
		//todo: 
		/*
		 * the key generated here is a combination of username and password, which may 
		 * not be unique. Consider:
		 *      username:    test::test     password: test
		 *      username:    test           password: test::test
		 *
		 * both accounts result in the same key, test::test::test
		 * Probably not likely that we'd ever clash, but we might be better off hashing/encoding 
		 * the password so that we know it doesn't contain the delimiter in order to keep it 
		 * completely unique. 
		 * 
		 * Also, this code does the exact same thing, no matter the authentication scheme...
		*/
        private static string MakeCacheKey(AuthenticationScheme authScheme, string username, string password)
        {
            string key = string.Empty;
            switch (authScheme)
            {
                case AuthenticationScheme.UsernameAndPassword:
                    key = string.Format("U/P::{0}::{1}", username, password);
                    break;
                case AuthenticationScheme.EmailAndLoyaltyId:
                    key = string.Format("E/L::{0}::{1}", username, password);
                    break;                
				case AuthenticationScheme.SocialNetwork:
					key = string.Format("social::{0}::{1}", username, password);
					break;
            }
            return key;
        }

        public static WcfAuthenticationToken GetAuthTokenFromCache(AuthenticationScheme authScheme, string username, string password)
        {
			var config = LWDataServiceUtil.GetServiceConfiguration();
			return config.CacheManager.Get(Authentication_RG_NAME, MakeCacheKey(authScheme, username, password)) as WcfAuthenticationToken;
        }

		public static WcfAuthenticationToken GetAuthTokenFromCache(string tokenId)
		{
			var config = LWDataServiceUtil.GetServiceConfiguration();
			return config.CacheManager.Get(AuthenticationIds_RG_NAME, tokenId) as WcfAuthenticationToken;
		}

		public static int GetTokenCacheCount()
		{
			var config = LWDataServiceUtil.GetServiceConfiguration();
			return config.CacheManager.Count(AuthenticationIds_RG_NAME);
		}

		public static void AddAuthTokenToCache(WcfAuthenticationToken token)
		{
			var config = LWDataServiceUtil.GetServiceConfiguration();
			config.CacheManager.Update(Authentication_RG_NAME, MakeCacheKey(token._authScheme, token.Username, token.Password), token);
			config.CacheManager.Update(AuthenticationIds_RG_NAME, token.TokenId, token);
		}

        public static void RemoveAuthTokenFromCache(WcfAuthenticationToken token)
        {
			var config = LWDataServiceUtil.GetServiceConfiguration();
            config.CacheManager.Remove(Authentication_RG_NAME, MakeCacheKey(token._authScheme, token.Username, token.Password));
            config.CacheManager.Remove(AuthenticationIds_RG_NAME, token.TokenId);
        }
        #endregion

        public void ReplaceCachedMember(Member member)
        {
            _member = member;
        }

        public WcfAuthenticationToken(AuthenticationScheme authScheme, string username, string password, Member member, bool passwordResetRequired)
        {
            this._authScheme = authScheme;
            this._username = username;
            this._password = password;
            this._member = member;
            this._passwordResetRequired = passwordResetRequired;
        }
    }
}