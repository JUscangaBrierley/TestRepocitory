using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
	public class RedeemMemberCouponByCertNmbr : OperationProviderBase
    {
        public RedeemMemberCouponByCertNmbr() : base("RedeemCouponByCertNmbr") { }

		public override string Invoke(string source, string parms)
        {            
            //string methodName = "Invoke";

            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided to redeem coupon.") { ErrorCode = 3300 };
                }

                string language = LanguageChannelUtil.GetDefaultCulture();
                string channel = LanguageChannelUtil.GetDefaultChannel();                
                bool returnAttributes = false;
				bool ignoreViolations = false;

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string certNmbr = (string)args["CertNmbr"];
                if (args.ContainsKey("Language"))
                {
                    language = (string)args["Language"];
                }
                if (args.ContainsKey("Channel"))
                {
                    channel = (string)args["Channel"];
                }                
                if (args.ContainsKey("ReturnAttributes"))
                {
                    returnAttributes = (bool)args["ReturnAttributes"];
                }
				if (args.ContainsKey("IgnoreViolations"))
				{
					ignoreViolations = (bool)args["IgnoreViolations"];
				}

				DateTime? redeemDate = args.ContainsKey("RedemptionDate") ? (DateTime?)args["RedemptionDate"] : null;
                int timesUsed = args.ContainsKey("TimesUsed") ? (int)args["TimesUsed"] : 1;

                MemberCoupon mc = CouponUtil.RedeemCoupon(certNmbr, timesUsed, redeemDate, ignoreViolations);
                
                CouponDef coupon = ContentService.GetCouponDef(mc.CouponDefId);
                
                APIArguments responseArgs = new APIArguments();

                Member member = LoyaltyDataService.LoadMemberFromIPCode(mc.MemberId);
                string memberIdentity = ReturnMemberIdentity(member);

                responseArgs.Add("MemberIdentity", memberIdentity);
                responseArgs.Add("NumberOfUsesLeft", coupon.UsesAllowed - mc.TimesUsed);

                // validate language and channel
                if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
                {
                    throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
                }
                if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
                {
                    throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
                }

                APIStruct rv = CouponHelper.SerializeCouponDef(language, channel, returnAttributes, coupon);
                responseArgs.Add("CouponDefinition", rv); 
                                              
                response = SerializationUtils.SerializeResult(Name, Config, responseArgs);

                #region Post Processing
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("member", member);
                context.Add("coupon", mc);
                PostProcessSuccessfullInvocation(context);
                #endregion

                return response;
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }
    }
}
