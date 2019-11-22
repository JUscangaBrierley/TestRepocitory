using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XigniteTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.TestAPI();

        }
        public void TestAPI()
        {

            RemoteGlobalCurrencies.GetOfficialRateRequest rateRequest = new RemoteGlobalCurrencies.GetOfficialRateRequest();



            RemoteGlobalCurrencies.XigniteGlobalCurrenciesSoapClient client = new RemoteGlobalCurrencies.XigniteGlobalCurrenciesSoapClient("XigniteGlobalCurrenciesSoap");

            // add authentication info
            RemoteGlobalCurrencies.Header objHeader = new RemoteGlobalCurrencies.Header();
            RemoteGlobalCurrencies.GetRealTimeRateRequest request = new RemoteGlobalCurrencies.GetRealTimeRateRequest();
            objHeader.Username = "01788D18914548A382360C69B2F4B878";

            request.Header = objHeader;
            request.Symbol = "CANUSD";
            //RemoteGlobalCurrencies.Rate rate = client.GetRealTimeRate(objHeader, "CANUSD");
            RemoteGlobalCurrencies.Rate rate = client.GetRealTimeRate(objHeader, "CADUSD");

            if (rate == null)
            {
                // add error processing here 
                // this condition could be caused by an HTTP error (404,500...)
                Console.Write("Service is unavailable at this time.");
            }
            else
            {
                //RemoteGlobalCurrencies.Rate rate = response.GetRealTimeRateResult;
                switch (rate.Outcome)
                {
                    case RemoteGlobalCurrencies.OutcomeTypes.Success:
                        // add processing for displaying the results, e.g. 
                        // display the value for objRate.Source
                        // other values could be consumed in the same manner
                        Console.Write(rate.Source);
                        double currentRate = rate.Mid;
                        break;
                    default:
                        // add processing for handling request problems, e.g. 
                        // you could pass back the info message received from the service 
                        Console.Write(rate.Message);
                        break;
                }
            }
        }
    }
}
