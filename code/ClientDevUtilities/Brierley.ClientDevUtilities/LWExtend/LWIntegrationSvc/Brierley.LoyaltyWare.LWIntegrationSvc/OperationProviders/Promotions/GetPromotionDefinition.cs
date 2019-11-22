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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Promotions
{
    public class GetPromotionDefinition : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetPromotionDefinition";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetPromotionDefinition() : base("GetPromotionDefinition") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided to retrieve promotion definition.") { ErrorCode = 3300 };
                }

                string language = LanguageChannelUtil.GetDefaultCulture();
                string channel = LanguageChannelUtil.GetDefaultChannel();                
                bool returnAttributes = false;

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                long? id = args.ContainsKey("PromotionDefId") ? (long?)args["PromotionDefId"] : null;
                string code = args.ContainsKey("PromotionCode") ? (string)args["PromotionCode"] : string.Empty;
                
                if ( id == null && string.IsNullOrEmpty(code) )
                {
                    throw new LWOperationInvocationException("No identifier specified to retrieve promotion definition.") { ErrorCode = 3391 };
                }

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

                Promotion promo = null;
                if (id != null)
                {
                    promo = ContentService.GetPromotion(id.Value);
                }
                else
                {
                    promo = ContentService.GetPromotionByCode(code);
                }

                if (promo == null)
                {
                    throw new LWOperationInvocationException("No content available that matches the specified criteria.") { ErrorCode = 3362 };
                }

                if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
                {
                    throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
                }
                if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
                {
                    throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
                }

                APIArguments responseArgs = new APIArguments();

                APIStruct rv = PromotionUtil.SerializePromotionDefinition(language, channel, promo, returnAttributes);
                responseArgs.Add("PromotionDefinition", rv);                
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
