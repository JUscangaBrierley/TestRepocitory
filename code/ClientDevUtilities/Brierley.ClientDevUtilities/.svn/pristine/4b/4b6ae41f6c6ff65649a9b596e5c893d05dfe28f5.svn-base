using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class GetMemberSocialHandles : OperationProviderBase
    {
        public GetMemberSocialHandles() : base("GetMemberSocialHandles") { }

        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				Member member = LoadMember(args);

                IList<MemberSocNet> handles = LoyaltyDataService.GetMemberSocNets(member.IpCode);
                if (handles == null || handles.Count == 0)
                {
                    throw new LWOperationInvocationException("No social handles found.") { ErrorCode = 3362 };
                }                                
                APIArguments resultParams = new APIArguments();
                APIStruct[] pes = new APIStruct[handles.Count];
                int idx = 0;
                foreach (MemberSocNet handle in handles)
                {
                    APIArguments tparms = new APIArguments();
                    tparms.Add("ProviderType", handle.ProviderType.ToString());
                    tparms.Add("ProviderUID", handle.ProviderUID);
                    APIStruct pevent = new APIStruct() { Name = "MemberSocialHandle", IsRequired = false, Parms = tparms };
                    pes[idx++] = pevent;
                }
                resultParams.Add("MemberSocialHandle", pes);
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);

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
