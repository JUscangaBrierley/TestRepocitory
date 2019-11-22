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
    public class GetLoyaltyCurrencyNames : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetLoyaltyCurrencyNames";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetLoyaltyCurrencyNames() : base("GetLoyaltyCurrencyNames") { }
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
                IList<string> names = new List<string>();                
                foreach (PointType pt in ptList)
                {
                    names.Add(pt.Name);                    
                }
                
                APIArguments resultParams = new APIArguments();
                resultParams.Add("CurrencyName", names.ToArray());
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
