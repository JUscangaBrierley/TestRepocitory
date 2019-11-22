using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;



namespace Brierley.FrameWork.JobScheduler.Jobs
{
    public class ExchangeRateJob : IJob
    {
        private const string _className = "CertificateJob";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_JOBSCHEDULER);
        private StringBuilder _report = null;

        public void FinalizeJob(ScheduleJobStatus jobStatus)
        {
        }

        public string GetReport()
        {
            return _report == null ? string.Empty : _report.ToString();
        }

        public void RequestAbort()
        {
        }

        public ScheduleJobStatus Resume(string parms)
        {
            throw new NotImplementedException();
        }

        public ScheduleJobStatus Run(string parms)
        {
            return GetExchangeRates();
        }

        /// <summary>
        /// This will try to create an instance of a class of type IRefreshExhangeRateJob and invoke the FetchExchangeRtes method,
        /// the method returns a list of exchange rate objects that will either be added or updated. 
        /// </summary>
        /// <returns></returns>
        public ScheduleJobStatus GetExchangeRates()
        {
            const string methodName = "GetExchangeRates";


            try
            {
                List<ExchangeRate> exchangeRates = new List<ExchangeRate>();
                string assemblyName = LWConfigurationUtil.GetConfigurationValue("LoyaltyCurrencyAsPayment AssemblyName");
                string className = LWConfigurationUtil.GetConfigurationValue("LoyaltyCurrencyAsPayment ClassName");

                //Load the assembly configured in the Framework config
                Assembly assembly = ClassLoaderUtil.LoadAssembly(assemblyName);
                IExchangeRateProvider provider = null;

                
                //If the assembly was loaded successfully then create an instance of the class
                if (assembly != null)
                {
                    provider = (IExchangeRateProvider)assembly.CreateInstance(className);
                }
                else
                {
                    string msg = string.Format("Unable to Load assembly with name: {0}.", assemblyName);
                    _logger.Error(_className, methodName, msg);
                    return FrameWork.Common.ScheduleJobStatus.Failure;
                }

                //If we were able to create an instance of the class then call the fetch method to get the data
                if (provider != null)
                {
                    exchangeRates = provider.FetchExchangeRates().ToList();
                }
                else
                {
                    string msg = string.Format("Unable to create an instance of the class {0}.", assemblyName);
                    _logger.Error(_className, methodName, msg);
                    return FrameWork.Common.ScheduleJobStatus.Failure;
                }

                //Update or add the new rates to the database
                foreach (var d in exchangeRates)
                {
                    using (var contentService = LWDataServiceUtil.ContentServiceInstance())
                    {
                        ExchangeRate rate = contentService.GetExchangeRate(d.FromCurrency, d.ToCurrency);
                        if (rate != null)
                        {
                            rate.Rate = d.Rate;
                            contentService.UpdateExchangeRate(rate);
                        }
                        else
                        {
                            ExchangeRate newRate = new ExchangeRate();
                            newRate.FromCurrency = d.FromCurrency;
                            newRate.ToCurrency = d.ToCurrency;
                            newRate.Rate = d.Rate;
                            contentService.CreateExchangeRate(newRate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Failed to update exchange rates.", ex);
                throw;
            }
            return ScheduleJobStatus.Success;
        }

        public void SetRunID(long runID)
        {
        }
    }

    public class ExchangeRateJobFactory : IJobFactory
    {
        public IJob GetJob()
        {
            return new ExchangeRateJob();
        }
    }
}