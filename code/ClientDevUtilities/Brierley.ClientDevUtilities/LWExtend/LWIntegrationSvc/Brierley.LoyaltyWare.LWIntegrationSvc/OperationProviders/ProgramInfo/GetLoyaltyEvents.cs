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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.ProgramInfo
{
    public class GetLoyaltyEvents : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetLoyaltyEvents";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetLoyaltyEvents() : base("GetLoyaltyEvents") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;

                IList<PointEvent> peList = LoyaltyDataService.GetAllPointEvents();
                if (peList == null || peList.Count == 0)
                {
                    throw new LWOperationInvocationException("No loyalty events found.") { ErrorCode = 3362 };
                }                                
                APIArguments resultParams = new APIArguments();
                APIStruct[] pes = new APIStruct[peList.Count];
                int idx = 0;
                foreach (PointEvent pt in peList)
                {
                    APIArguments tparms = new APIArguments();
                    tparms.Add("Name", pt.Name);
                    if (!string.IsNullOrEmpty(pt.Description))
                    {
                        tparms.Add("Description", pt.Description);
                    }
                    if (pt.DefaultPoints != null)
                    {
                        tparms.Add("DefaultPoints", pt.DefaultPoints);
                    }
                    APIStruct pevent = new APIStruct() { Name = "LoyaltyEvent", IsRequired = false, Parms = tparms };
                    pes[idx++] = pevent;
                }
                resultParams.Add("LoyaltyEvent", pes);
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

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
