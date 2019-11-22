using System;

using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.CustomerService
{
    public class CreateCSNote : OperationProviderBase
    {
        public CreateCSNote() : base("CreateCSNote") { }
        
		public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No card id provided for add loyalty event.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string note = (string)args["Note"];
                long createdBy = (long)args["CreatedBy"];

                Member member = LoadMember(args);

                CSNote csnote = new CSNote();
                csnote.MemberId = member.IpCode;
                csnote.Note = note;
                csnote.CreatedBy = createdBy;

				using (CSService service = LWDataServiceUtil.CSServiceInstance())
				{
					service.CreateNote(csnote);
				}

                APIArguments resultParams = new APIArguments();
                resultParams.Add("CSNoteID", csnote.Id);
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
