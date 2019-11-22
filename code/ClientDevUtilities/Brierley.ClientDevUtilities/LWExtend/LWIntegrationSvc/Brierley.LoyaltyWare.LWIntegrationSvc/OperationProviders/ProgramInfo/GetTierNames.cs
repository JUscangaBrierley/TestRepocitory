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
    public class GetTierNames : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetTierNames";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetTierNames() : base("GetTierNames") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {
            try
            {
                string response = string.Empty;

                IList<TierDef> tiers = LoyaltyDataService.GetAllTierDefs();
                if (tiers == null || tiers.Count == 0)
                {
                    throw new LWOperationInvocationException("No tiers found.") { ErrorCode = 3362 };
                }
                IList<string> names = new List<string>();
                foreach (TierDef t in tiers)
                {
                    names.Add(t.Name);
                }                

                APIArguments resultParams = new APIArguments();
                resultParams.Add("TierName", names.ToArray());
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);
                
                return response;
            }
            catch (LWOperationInvocationException)
            {                
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message);
            }
        }

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
