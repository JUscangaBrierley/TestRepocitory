using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Surveys
{
    public class GetProfileSurveys : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetProfileSurveys";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetProfileSurveys() : base("GetProfileSurveys") { }

        #region Private Helpers        
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 2)
            {
                string errMsg = "Invalid parameters provided for GetProfileSurveys.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string language = (string)parms[0];
            if (string.IsNullOrEmpty(language))
            {
                language = LanguageChannelUtil.GetDefaultCulture();
            }
            string channel = (string)parms[1];
            if (string.IsNullOrEmpty(channel))
            {
                channel = LanguageChannelUtil.GetDefaultChannel();
            }

            Member member = token.CachedMember;

            List<MGSurvey>  results = MGSurveyManager.GetSurveys(language, SurveyType.Profile, member);
            if (results == null || results.Count == 0)
            {
                throw new LWOperationInvocationException("No profile surveys found.") { ErrorCode = 3362 };
            }

            return results;
        }
        #endregion
    }
}