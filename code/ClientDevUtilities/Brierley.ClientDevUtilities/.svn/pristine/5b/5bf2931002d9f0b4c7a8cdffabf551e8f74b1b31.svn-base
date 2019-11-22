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
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
    public class GetCouponDefinition : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetCouponDefinition";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetCouponDefinition() : base("GetCouponDefinition") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided to retrieve coupon definition.") { ErrorCode = 3300 };
                }

                string language = string.Empty;
                string channel = string.Empty;                
                bool returnAttributes = false;

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                long id = (long)args["CouponDefId"];
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

                // validate language and channel
                if (string.IsNullOrEmpty(language))
                {
                    language = LanguageChannelUtil.GetDefaultCulture();
                }
                if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
                {
                    throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
                }
                if (string.IsNullOrEmpty(channel))
                {
                    channel = LanguageChannelUtil.GetDefaultChannel();
                }
                if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
                {
                    throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
                }

                CouponDef coupon = ContentService.GetCouponDef(id);
                if (coupon == null)
                {
                    throw new LWOperationInvocationException("No coupon definition found with " + id + ".") { ErrorCode = 3369 };
                }

                APIArguments responseArgs = new APIArguments();                
                APIStruct rv = CouponHelper.SerializeCouponDef(language, channel, returnAttributes, coupon);
                responseArgs.Add("CouponDefinition", rv);                
                response = SerializationUtils.SerializeResult(Name, Config, responseArgs);

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

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
