using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

using Brierley.FrameWork.Common.Config;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.PerformanceCounters
{
    public class SAPIPerformanceCounterUtil
    {
        #region Fields
        private const string SAPIPerfCategory = "Brierley SAPI";
        private const string NumberOfRequestsReceived = "# of requests received.";
        private const string NumberOfOutstandingRequests = "# of outstanding requests.";        

        private PerformanceCounter _numbersOfRequestsReceived = null;
        private PerformanceCounter _numberOfOutstandingrequests = null;        
        #endregion

        #region Construction & Initialization
        public SAPIPerformanceCounterUtil()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                if (!PerformanceCounterCategory.Exists(SAPIPerfCategory))
                {
                    CounterCreationDataCollection counters = new CounterCreationDataCollection();

                    // 1. Number of requests received.
                    CounterCreationData nRequestsReceived = new CounterCreationData();
                    nRequestsReceived.CounterName = NumberOfRequestsReceived;
                    nRequestsReceived.CounterHelp = "Number of requests received.";
                    nRequestsReceived.CounterType = PerformanceCounterType.NumberOfItems32;
                    counters.Add(nRequestsReceived);

                    // 2. Number of outstanding requests.
                    CounterCreationData nRequestsOutstanding = new CounterCreationData();
                    nRequestsOutstanding.CounterName = NumberOfOutstandingRequests;
                    nRequestsOutstanding.CounterHelp = "Number of outstanding requests.";
                    nRequestsOutstanding.CounterType = PerformanceCounterType.NumberOfItems32;
                    counters.Add(nRequestsOutstanding);

                    PerformanceCounterCategory.Create(SAPIPerfCategory, "SAPI Process performance counters.", PerformanceCounterCategoryType.SingleInstance, counters);
                }

                string strMonitor = LWConfigurationUtil.GetConfigurationValue("LWSAPIPerformanceCounters");
                bool perCounters = !string.IsNullOrEmpty(strMonitor) ? bool.Parse(strMonitor) : false;

                if (perCounters)
                {
                    _numbersOfRequestsReceived = new PerformanceCounter();
                    _numbersOfRequestsReceived.CategoryName = SAPIPerfCategory;
                    _numbersOfRequestsReceived.CounterName = NumberOfRequestsReceived;
                    _numbersOfRequestsReceived.MachineName = ".";
                    _numbersOfRequestsReceived.ReadOnly = false;

                    _numberOfOutstandingrequests = new PerformanceCounter();
                    _numberOfOutstandingrequests.CategoryName = SAPIPerfCategory;
                    _numberOfOutstandingrequests.CounterName = NumberOfOutstandingRequests;
                    _numberOfOutstandingrequests.MachineName = ".";
                    _numberOfOutstandingrequests.ReadOnly = false;

                }
            }
            catch (Exception ex)
            {
                throw new Brierley.FrameWork.Common.Exceptions.LWException("Unable to initialize performance counters.  Registry could not be accessed.", ex);
            }
        }
        #endregion

        #region Counter Management
        public void IncrementNumberOfRequestsReceived()
        {
            if (_numbersOfRequestsReceived != null)
            {
                _numbersOfRequestsReceived.Increment();
            }
        }

        public void IncrementNumberOfOutstandingRequests()
        {
            if (_numberOfOutstandingrequests != null)
            {
                _numberOfOutstandingrequests.Increment();
            }
        }


        public void DecrementNumberOfOutstandingRequests()
        {
            if (_numberOfOutstandingrequests != null)
            {
                _numberOfOutstandingrequests.Decrement();
            }
        }
                
        #endregion
    }
}
