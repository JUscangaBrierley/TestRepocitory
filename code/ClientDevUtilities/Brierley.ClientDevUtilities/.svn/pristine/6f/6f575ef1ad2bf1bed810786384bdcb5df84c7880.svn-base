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
    public class GetLoyaltyCurrencies : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetLoyaltyCurrencies";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetLoyaltyCurrencies() : base("GetLoyaltyCurrencies") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;

                IList<PointType> ptList = LoyaltyDataService.GetAllPointTypes();

                if (ptList == null || ptList.Count == 0)
                {
                    throw new LWOperationInvocationException("No loyalty currencies found.") { ErrorCode = 3362 };
                }

                APIArguments resultParams = new APIArguments();
                APIStruct[] pts = new APIStruct[ptList.Count];
                int idx = 0;
                foreach (PointType pt in ptList)
                {
                    APIArguments tparms = new APIArguments();
                    tparms.Add("Name", pt.Name);
                    if (!string.IsNullOrEmpty(pt.Description))
                    {
                        tparms.Add("Description", pt.Description);
                    }
                    tparms.Add("ConsumptionPriority", pt.ConsumptionPriority);
                    APIStruct point = new APIStruct() { Name = "LoyaltyCurrency", IsRequired = false, Parms = tparms };
                    pts[idx++] = point;
                }
                resultParams.Add("LoyaltyCurrency", pts);
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
