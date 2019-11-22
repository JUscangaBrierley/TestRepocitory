using AmericanEagle.SDK.RemoteGlobalCurrencies;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace AmericanEagle.SDK.OutputProviders
{
    public class LoadConversionRate : IDAPOutputProvider, IDisposable
    {
        private LWLogger logger = LWLoggerManager.GetLogger("LWDataAcquisitionService");
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        public void Initialize(NameValueCollection globals, NameValueCollection args, long jobId, 
            DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
        }

        public void ProcessMessageBatch(List<string> messageBatch)
        {
            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    LWCriterion crit = new LWCriterion("ConversionRates");
                    AttributeSetMetaData meta = lwService.GetAttributeSetMetaData("ConversionRates");
                    crit.Add(LWCriterion.OperatorType.AND, "Currencycode", "CAD", LWCriterion.Predicate.Eq);
                    crit.Add(LWCriterion.OperatorType.AND, "Currencydate", DateTime.Now.ToString("M/d/yyyy"), LWCriterion.Predicate.Eq);
                    LWQueryBatchInfo lWQueryBatchInfo = new LWQueryBatchInfo();
                    lWQueryBatchInfo.BatchSize = 1;
                    lWQueryBatchInfo.StartIndex = 0;

                    IList<IClientDataObject> rows = lwService.GetAttributeSetObjects(null, "ConversionRates",
                        crit, lWQueryBatchInfo, false, false);

                    if (rows != null && rows.Count > 0)
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,
                            MethodBase.GetCurrentMethod().Name,
                            "Error: Exchange rate already exist for " + DateTime.Now.ToString("M/d/yyyy"));

                        throw new Exception("Exchange rate already exist for " + DateTime.Now.ToString("M/d/yyyy"));
                    }

                    XigniteGlobalCurrenciesSoapClient client =
                        new XigniteGlobalCurrenciesSoapClient("XigniteGlobalCurrenciesSoap");

                    Header objHeader = new Header();

                    ClientConfiguration config;
                    using (var dtService = _dataUtil.DataServiceInstance())
                    {
                        config = dtService.GetClientConfiguration("XigniteUserID");
                    }

                    if (config == null || config.Value == null || config.Value.Trim().Length <= 0)
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,
                            MethodBase.GetCurrentMethod().Name,
                            "Error: XigniteUserID key doesn't exist in database or it's empty. ");

                        throw new Exception("Error: XigniteUserID key doesn't exist in database or it's empty. ");
                    }

                    objHeader.Username = config.Value.Trim();
                    //Rate rate = client.GetRealTimeRate(objHeader, "CADUSD");
                    Rate rate = client.GetRealTimeRate(objHeader, "USDCAD");
                    if (rate == null)
                    {
                        throw new Exception("Service is unavailable at this time.");
                    }

                    OutcomeTypes outcome = rate.Outcome;
                    if (outcome != OutcomeTypes.Success)
                    {
                        throw new Exception(rate.Message);
                    }

                    ConversionRates rateRow = new ConversionRates();
                    rateRow.CreateDate = DateTime.Now;
                    rateRow.Currencydate = rateRow.CreateDate.Date;
                    rateRow.UpdateDate = new DateTime?(DateTime.Now);
                    rateRow.Currencycode = "CAD";
                    rateRow.Currencyrate = new decimal(rate.Mid);
                    rateRow.IpCode = -1L;

                    lwService.SaveAttributeSetObject(rateRow, null, RuleExecutionMode.Real, false);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, 
                    MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
                throw;
            }
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, 
                MethodBase.GetCurrentMethod().Name, "End");
        }

        public int Shutdown()
        {
            return 0;
        }

        public void Dispose()
        {
            return;
        }
    }
}
