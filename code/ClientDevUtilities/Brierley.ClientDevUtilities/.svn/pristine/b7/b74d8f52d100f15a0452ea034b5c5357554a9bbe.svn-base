using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

using Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Surveys;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.CheckIn
{
    public class PreCheckIn : CheckInBase
    {
        #region Fields
        private const string _className = "PreCheckIn";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public PreCheckIn() : base("PreCheckIn") { }
        
        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 2)
            {
                string errMsg = "Invalid parameters provided for PreCheckin.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            MGLocation location = (MGLocation)parms[0];
            double mapRadiusInKM = (double)parms[1];
            Member member = token.CachedMember;

            MGPreCheckInResponse response = new MGPreCheckInResponse();

            long nTimesChecked = 0;
            StoreDef store = null;
            response.CheckInStatus = VerifyCheckInAllowed(member, location, mapRadiusInKM, out nTimesChecked, ref store);
			if (store != null)
			{
				response.StoreLocation = MGStoreDef.Hydrate(store);
			}
            
            if (response.CheckInStatus == MemberMobileCheckInStatus.Ok)
            {
                string preCheckInSurveyName = GetFunctionParameter("PreCheckInSurveyName");
                if (!string.IsNullOrEmpty(preCheckInSurveyName))
                {
                    string language = LanguageChannelUtil.GetDefaultCulture();
                    response.PreCheckInSurvey = MGSurveyManager.GetSurvey(language, preCheckInSurveyName, member);
                }
            }

            #region Post Processing
            if (response.CheckInStatus == MemberMobileCheckInStatus.Ok)
            {
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("member", member);
                context.Add("store", response.StoreLocation);
                PostProcessSuccessfullInvocation(context);
            }
            #endregion
            
            return response;
        }
        #endregion
    }
}