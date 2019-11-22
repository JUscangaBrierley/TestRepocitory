using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.CustomerService
{
    public class GetCSNotes : OperationProviderBase
    {
        private const string _className = "GetCSNotes";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        public GetCSNotes() : base("GetCSNotes") { }

        public override string Invoke(string source, string parms)
        {
            const string methodName = "Invoke";
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No card id provided for add loyalty event.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                DateTime startDate = args.ContainsKey("StartDate") ? (DateTime)args["StartDate"] : DateTimeUtil.MinValue;
                DateTime endDate = args.ContainsKey("EndDate") ? (DateTime)args["EndDate"] : DateTimeUtil.MaxValue;

                if (endDate < startDate)
                {
                    _logger.Error(_className, methodName, "End date cannot be earlier than the start date");
                    throw new LWOperationInvocationException("End date cannot be earlier than the start date") { ErrorCode = 3204 };
                }

                Member member = LoadMember(args);

                using (CSService service = LWDataServiceUtil.CSServiceInstance())
                {
                    IList<CSNote> notes = service.GetNotesByMember(member.IpCode, startDate, endDate);
                    if (notes.Count > 0)
                    {
                        APIArguments responseArgs = new APIArguments();
                        APIStruct[] noteList = new APIStruct[notes.Count];
                        int i = 0;
                        foreach (CSNote note in notes)
                        {
                            APIArguments rparms = new APIArguments();
                            rparms.Add("Note", note.Note);
                            rparms.Add("CreatedBy", note.CreatedBy);
                            rparms.Add("CreateDate", note.CreateDate);
                            APIStruct snote = new APIStruct() { Name = "CSNote", IsRequired = false, Parms = rparms };
                            noteList[i++] = snote;
                        }

                        responseArgs.Add("CSNote", noteList);
                        response = SerializationUtils.SerializeResult(Name, Config, responseArgs);
                    }
                    return response;
                }
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
