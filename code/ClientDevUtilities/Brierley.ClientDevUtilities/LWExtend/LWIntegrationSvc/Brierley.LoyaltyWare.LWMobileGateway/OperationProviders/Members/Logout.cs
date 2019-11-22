using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.Authorization;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
    public class Logout : OperationProviderBase
    {
        #region Fields
        private const string _className = "Logout";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        #region Initialization
        public Logout() : base("Logout") { }

        public override void Initialize(string opName, MobileGatewayDirectives config, System.Collections.Specialized.NameValueCollection functionProviderParms)
        {
            base.Initialize(opName, config, functionProviderParms);
        }
        #endregion

        #region Private Helpers        
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            _logger.Trace(_className, methodName,
                string.Format("Logging out member {0}", token.CachedMember.Username));

            WcfAuthenticationToken.RemoveAuthTokenFromCache(token);

            return null;
        }
        #endregion
    }
}