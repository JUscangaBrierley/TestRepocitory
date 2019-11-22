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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Messages
{
    public class GetMessageDefinition : OperationProviderBase
    {
        public GetMessageDefinition() : base("GetMessageDefinition") { }

        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided to retrieve message definition.") { ErrorCode = 3300 };
                }

                string language = LanguageChannelUtil.GetDefaultCulture();
                string channel = LanguageChannelUtil.GetDefaultChannel();                
                bool returnAttributes = false;

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                long id = (long)args["MessageDefId"];
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
                if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
                {
                    throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
                }
                if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
                {
                    throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
                }

                MessageDef message = ContentService.GetMessageDef(id);
                if (message == null)
                {
                    throw new LWOperationInvocationException("No message found with the provided id.") { ErrorCode = 3370 };
                }

                APIArguments responseArgs = new APIArguments();
                                
                //APIArguments rparms = MessageUtil.SerializeMessageDefinition(language, channel, message, returnAttributes);
                //APIStruct rv = new APIStruct() { Name = "MessageDefinition", IsRequired = false, Parms = rparms };

                APIStruct rv = MessageUtil.SerializeMessageDefinition(language, channel, message, returnAttributes);
                responseArgs.Add("MessageDefinition", rv);                
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
    }
}
