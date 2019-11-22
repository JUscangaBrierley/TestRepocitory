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
    public class PostSurveyResponse : OperationProviderBase
    {
        #region Fields
        private const string _className = "PostSurveyResponse";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public PostSurveyResponse() : base("PostSurveyResponse") { }

        #region Private Helpers        
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 4)
            {
                string errMsg = "Invalid parameters provided for PostSurveyResponse.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string surveyIdStr = (string)parms[0];
            long surveyId = -1;
            if (!string.IsNullOrEmpty(surveyIdStr))
            {
                surveyId = long.Parse(surveyIdStr);
            }
            string stateIdStr = (string)parms[1];
            long stateId = -1;
            if (!string.IsNullOrEmpty(stateIdStr))
            {
                stateId = long.Parse(stateIdStr);
            }

			string surveyResponseStr = (string)parms[2];
			MGSurveyResponse surveyResponse = null;
			if (!string.IsNullOrEmpty(surveyResponseStr))
			{
				surveyResponse = MGSurveyResponse.DeSerialize(surveyResponseStr);
			}

            string culture = (string)parms[3];
            if (string.IsNullOrEmpty(culture))
            {
                culture = LanguageChannelUtil.GetDefaultCulture();
            }

            Member member = token.CachedMember;

            bool result = MGSurveyManager.PostSurveyResponse(member, culture, surveyId, stateId, surveyResponse);

            return result;
        }
        #endregion
    }
}