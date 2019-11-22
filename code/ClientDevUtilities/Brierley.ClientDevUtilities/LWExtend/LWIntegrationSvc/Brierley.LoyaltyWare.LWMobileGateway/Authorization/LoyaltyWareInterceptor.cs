using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.ServiceModel.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.Authorization
{
    public class LoyaltyWareInterceptor : AuthInterceptorBase
    {
        #region Fields
        private const string _className = "LoyaltyWareInterceptor";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        private string GetTokenValue(string token)
        {
            string[] parts = token.Split('=');
            string value = parts[1];
            if (!string.IsNullOrEmpty(value))
            {
                if (value.StartsWith("\""))
                {
                    value = value.Substring(1);
                }
                if (value.EndsWith("\""))
                {
                    value = value.Substring(0,value.Length-1);
                }
            }
            return value;
        }

        public override WcfAuthenticationToken CheckAuthorization(string clientId)
        {
            string methodName = "CheckAuthorization";

            try
            {
                string header = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Authorization];
                if (string.IsNullOrEmpty(header))
                {
                    throw new AuthorizationException("No authorization header received.");
                }
                _logger.Debug(_className, methodName, "HTTP Header: " + header);
                //string authToken = Encoding.UTF8.GetString(Convert.FromBase64String(header.Substring("LoyaltyWare".Length)));            
                if (!header.StartsWith("LoyaltyWare"))
                {
                    throw new AuthorizationException("Unexpected authorization header received.");
                }
                string authToken = header.Substring("LoyaltyWare".Length);                
                string[] parts = authToken.Split(',');
                if (parts == null || parts.Length != 3)
                {
                    throw new AuthorizationException("Bad header format for authorization header received.");
                }
                string _clientid = GetTokenValue(parts[1]);
                if (string.IsNullOrEmpty(_clientid))
                {
                    throw new AuthorizationException("Missing information in the authorization header.");
                }
                if (_clientid != clientId)
                {
                    throw new AuthorizationException("Incorrect target information in authorization header.");
                }
                string wcftoken = GetTokenValue(parts[2]);
                if (string.IsNullOrEmpty(wcftoken))
                {
                    throw new AuthorizationException("Missing authorization token in autorization header.");
                }

                WcfAuthenticationToken cachedToken = WcfAuthenticationToken.GetAuthTokenFromCache(wcftoken);
                if (cachedToken == null)
                {
                    throw new AuthorizationException("Unauthenticated or expired authentication token.");
                }
                return cachedToken;
            }
            catch (LWException ex)
            {
                _logger.Error(_className, methodName, ex.Message, ex);
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}