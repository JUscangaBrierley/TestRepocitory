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
    public class GetNextSurveyState : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetNextSurveyState";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetNextSurveyState() : base("GetNextSurveyState") { }

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 3)
            {
                string errMsg = "Invalid parameters provided for GetNextSurveyComponent.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string culture = (string)parms[0];
            if (string.IsNullOrEmpty(culture))
            {
                culture = LanguageChannelUtil.GetDefaultCulture();
            }

            string surveyIdStr = (string)parms[1];
            long surveyId = -1;
            if (!string.IsNullOrEmpty(surveyIdStr))
            {
                surveyId = long.Parse(surveyIdStr);
            }
            string stateIdStr = (string)parms[2];
            long stateId = -1;
            if (!string.IsNullOrEmpty(stateIdStr))
            {
                stateId = long.Parse(stateIdStr);
            }

            Member member = token.CachedMember;

            MGSurveyState state = MGSurveyManager.GetNextSurveyState(member, culture, surveyId, stateId);
            if (state == null )
            {
                throw new LWOperationInvocationException("No survey state found.") { ErrorCode = 3362 };
            }

            return state;
        }
        #endregion
    }
}