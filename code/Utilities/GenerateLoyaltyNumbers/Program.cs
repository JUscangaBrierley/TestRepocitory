//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="B+P">
//     Copyright (c) B+P. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace AmericanEagle.Utilities.GenerateLoyaltyNumbers
{
    #region Using Statement
    using System;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Brierley.FrameWork.Common.Logging;
    #endregion

    /// <summary>
    /// Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Starting point of the application
        /// </summary>
        /// <param name="args">String array of arguments</param>
        public static void Main(string[] arrArguments)
        {
            Processor processor = new Processor();

            // Object for logging
            LWLogger logger = LWLoggerManager.GetLogger("GenerateLoyaltyNumbers");
            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                string errorMessage = string.Empty;
                if (arrArguments.Length > 0)
                {
                    if (arrArguments.Length == 1)
                    {
                        if (Program.IsNumeric(arrArguments[0]))
                        {
                            errorMessage = "Number of Loyalty Numbers to Generate: " + arrArguments[0] + Environment.NewLine + "Brand not passed in. Defaulting to AEO brand prefix = 80";
                            Console.WriteLine(errorMessage);
                            logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, errorMessage);
                            processor.Process(Convert.ToInt64(arrArguments[0]), Convert.ToString(80));
                            Console.WriteLine("[" + arrArguments[0] + "] Loyalty numbers has been generated successfully.");
                        }
                        else
                        {
                            errorMessage = "Loyalty Number Qty is not numeric";
                            Console.WriteLine(errorMessage);
                            logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, errorMessage);
                        }
                    }
                    else if (arrArguments.Length >= 2)
                    {
                        string numberToGenerate = arrArguments[0].Trim();
                        string prefixLoyaltyNumber = arrArguments[1].Trim();

                        if (!Program.IsNumeric(numberToGenerate))
                        {
                            errorMessage = "Loyalty Number Qty is not numeric";
                            Console.WriteLine(errorMessage);
                            logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, errorMessage);
                        }
                        else if (!Program.IsNumeric(prefixLoyaltyNumber))
                        {
                            errorMessage = "Prefix To Loyalty Number is not numeric";
                            Console.WriteLine(errorMessage);
                            logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, errorMessage);
                        }
                        else
                        {
                            processor.Process(Convert.ToInt64(numberToGenerate), prefixLoyaltyNumber);
                            Console.WriteLine("[" + numberToGenerate + "] Loyalty numbers has been generated successfully.");
                        }
                    }
                }
                else
                {
                    errorMessage = "This job takes 2 parameters. " + Environment.NewLine + "The number of loyalty numbers to generate" + Environment.NewLine + "LoyaltyNumberPrefix" + Environment.NewLine + "Example: GenerateNewNumbers.exe 2000000 80" + Environment.NewLine + "This would generate 2 million new numbers of AEO brand";
                    Console.WriteLine(errorMessage);
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, errorMessage);
                }

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                Console.WriteLine(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        /// <summary>
        /// Method to determine that the object value is numeric or not
        /// </summary>
        /// <param name="str">String value</param>
        /// <returns>True: if value is numeric, False: if value is not numeric</returns>
        public static bool IsNumeric(string str)
        {
            Regex regEx = new Regex(@"^\d+$");
            Match match = regEx.Match(str);
            return match.Success;
        }
    }
}
