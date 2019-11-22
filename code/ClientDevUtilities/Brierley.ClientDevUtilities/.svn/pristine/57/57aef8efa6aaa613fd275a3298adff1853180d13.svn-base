using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
    public class GetMemberCouponById : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetMemberCouponById";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetMemberCouponById() : base("GetMemberCouponById") { }
        #endregion

        #region Helpers
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {
            string methodName = "Invoke";
            try
            {
                string response = string.Empty;
                string language = LanguageChannelUtil.GetDefaultCulture();
                string channel = LanguageChannelUtil.GetDefaultChannel();
                
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided to retrieve member coupons.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                long id = args.ContainsKey("Id") ? (long)args["Id"] : -1;
                if (args.ContainsKey("Language"))
                {
                    language = (string)args["Language"];
                }
                if (args.ContainsKey("Channel"))
                {
                    channel = (string)args["Channel"];
                }

                bool returnAttributes = false;
                if (args.ContainsKey("ReturnAttributes"))
                {
                    returnAttributes = (bool)args["ReturnAttributes"];
                }

                if (id == -1)
                {
                    throw new LWOperationInvocationException("No coupon id provided.") { ErrorCode = 3383 };
                }

                // validate language and channel
                if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
                {
                    throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
                }
                if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
                {
                    throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
                }

                MemberCoupon coupon = LoyaltyDataService.GetMemberCoupon(id);
                if (coupon != null)
                {
                    Member member = LoyaltyDataService.LoadMemberFromIPCode(coupon.MemberId);
                    APIStruct memberCoupon = CouponHelper.SerializeMemberCoupon(member, language, channel, coupon, returnAttributes);                    
                    APIArguments resultParams = new APIArguments();
                    resultParams.Add("MemberCoupon", memberCoupon);
                    response = SerializationUtils.SerializeResult(Name, Config, resultParams);
                    return response;
                }
                else
                {
                    throw new LWOperationInvocationException("No coupon found with the provided cert number.") { ErrorCode = 3370 };
                }                                                
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, ex.Message, ex);
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }            
        }
        protected override void Cleanup()
        {
        }
        #endregion
    }
}
