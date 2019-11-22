//-----------------------------------------------------------------------
// <copyright file="Processor.cs" company="B+P">
//     Copyright (c) B+P. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace AmericanEagle.Utilities.GenerateLoyaltyNumbers
{
    #region Using Statement
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork.Common.Config;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.ClientDevUtilities.LWGateway;
    #endregion

    /// <summary>
    /// Processor class
    /// </summary>
    public class Processor
    {
        /// <summary>
        /// Data service instance
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil();
        private IDataService dataService;

        /// <summary>
        /// Logger instance
        /// </summary>
        private LWLogger logger;

        /// <summary>
        /// Initializes static members of the Processor class
        /// </summary>
        public Processor()
        {
            // Checking Output path key
            if (null == ConfigurationManager.AppSettings["GenerateLoyaltyNumberOutputPath"])
            { 
                throw new Exception("GenerateLoyaltyNumberOutputPath key is not found in app.config.");
            }

            if (ConfigurationManager.AppSettings["GenerateLoyaltyNumberOutputPath"] == string.Empty)
            {
                throw new Exception("GenerateLoyaltyNumberOutputPath key has empty value in app.config.");
            }

            Console.WriteLine("Initializing Framework...");
            LWConfigurationUtil.SetCurrentEnvironmentContext(ConfigurationManager.AppSettings["OrganizationName"], ConfigurationManager.AppSettings["Environment"]);
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            dataService = _dataUtil.DataServiceInstance(ctx.Organization, ctx.Environment);
            logger = LWLoggerManager.GetLogger("GenerateLoyaltyNumbers");
        }

        /// <summary>
        /// Process method that is doing majority of the work
        /// </summary>
        /// <param name="numberOfLoyaltyNumbersToGenerate">long numberOfLoyaltyNumbersToGenerate</param>
        /// <param name="prefixOfLoyaltyNumber">string prefixOfLoyaltyNumber</param>
        public void Process(long numberOfLoyaltyNumbersToGenerate, string prefixOfLoyaltyNumber)
        {
            long lastLoyaltyNumber = 0;

            try
            {
                Console.WriteLine("Generating Numbers...");
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

                // Getting RefBrand from prefix
                RefBrand refBrand = Utilities.GetRefBrandFromBrandPrefix(prefixOfLoyaltyNumber);

                // Output file name
                string fileName = "LoyaltyNumber_" + refBrand.ShortBrandName + "_" + DateTime.Parse(DateTime.Now.ToLongDateString()).ToString("MMddyyyy") + "_" + Convert.ToString(numberOfLoyaltyNumbersToGenerate) + ".txt";

                ////Job name for process
                string jobName = "GenerateLoyaltyNumber";

                // Starting the job
                AsyncJob objASyncJob = Utilities.StartJob(dataService, fileName, jobName);
                long jobID = objASyncJob.JobId;

                // getting last loyalty number from configuration

                // Determine the output path for generated file
                string outputPath = ConfigurationManager.AppSettings["GenerateLoyaltyNumberOutputPath"] + @"\" + fileName;

                // Stream writer object
                StreamWriter sw = new StreamWriter(outputPath);

                LoyaltyCard.LoyaltyCardType cardType = 0;

                switch (prefixOfLoyaltyNumber)
                {
                    case "80":
                        cardType = LoyaltyCard.LoyaltyCardType.AE;
                        break;
                    case "81":
                        cardType = LoyaltyCard.LoyaltyCardType.AEOnline;
                        break;
                    case "82":
                        cardType = LoyaltyCard.LoyaltyCardType.AECredit;
                        break;
                    case "83":
                        cardType = LoyaltyCard.LoyaltyCardType.AerieOnline;
                        break;
                    case "84":
                        cardType = LoyaltyCard.LoyaltyCardType.aerie;
                        break;
                    case "85":
                        cardType = LoyaltyCard.LoyaltyCardType.AeriePearl;
                        break;
                    case "86":
                        cardType = LoyaltyCard.LoyaltyCardType.AerieCredit;
                        break;
                    default:
                        cardType = LoyaltyCard.LoyaltyCardType.AE;
                        break;
                }

                for (long i = numberOfLoyaltyNumbersToGenerate; i > 0; i--)
                {
                    lastLoyaltyNumber = LoyaltyCard.GetNextLoyaltyNumber(cardType);

                    sw.WriteLine(lastLoyaltyNumber.ToString());
                }

                sw.Close();

                // Stopping the job
                Utilities.StopJob(dataService, jobID, numberOfLoyaltyNumbersToGenerate);
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
