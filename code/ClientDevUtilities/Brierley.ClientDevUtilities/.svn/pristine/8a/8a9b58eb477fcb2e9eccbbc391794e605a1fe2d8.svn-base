using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.Authorization
{
    public abstract class AuthInterceptorBase : IAuthorizationInterceptor
    {
        #region Fields
        private const string _className = "OperationProviderBase";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        private bool _disposed = false;
        #endregion
        
        #region Dispose
        public void Dispose()
        {
            string methodName = "Dispose";
            _logger.Debug(_className, methodName, string.Format("Authentication interceptor is being disposed."));
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Cleanup();
                }
                _disposed = true;
            }
        }

        protected virtual void Cleanup()
        {
        }
        #endregion

        public virtual void Initialize(NameValueCollection parameters)
        {
        }

        abstract public WcfAuthenticationToken CheckAuthorization(string clientId);
    }
}