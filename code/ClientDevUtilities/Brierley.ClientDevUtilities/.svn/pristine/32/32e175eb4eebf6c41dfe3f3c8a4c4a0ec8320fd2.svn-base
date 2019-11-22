using System;
using System.Collections.Generic;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders
{
    public class GetServiceInfo : OperationProviderBase
    {
        #region Fields        
        #endregion

        #region Construction
        public GetServiceInfo() : base("GetServiceInfo") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();

                APIArguments resultParams = new APIArguments();
                resultParams.Add("Organization", ctx.Organization);
                resultParams.Add("Environment", ctx.Environment);
                using (DataService svc = LWDataServiceUtil.DataServiceInstance())
                {
                    resultParams.Add("LoyaltyWareAssemblyVersion", svc.Version);
                    IList<LWSchemaVersion> versions = svc.GetSchemaVersion("FrameworkObjects");
                    if (versions.Count > 0)
                    {
                        resultParams.Add("LoyaltyWareSchemaVersion", versions[0].VersionNumber);
                    }
                }
                Assembly assembly = Assembly.GetExecutingAssembly();
                resultParams.Add("CDISAssemblyVersion", System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion);

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
