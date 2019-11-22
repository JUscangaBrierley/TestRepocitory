//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Brierley + Partners">
//     Copyright Brierley + Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Brierley.Clients.AmericanEagle.Processors.ProcessRequestCredit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Brierley.FrameWork.Common.Logging;

    /// <summary>
    /// Class Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Logger for ProcessRequestCredit
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("ProcessRequestCredit");

        /// <summary>
        /// Holds Class Name
        /// </summary>
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Starting point of the application
        /// </summary>
        /// <param name="arrArguments">String array of arguments</param>
        public static void Main(string[] arrArguments)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            
            try
            {
                Console.WriteLine("Processing Records.......");
                logger.Trace(className, methodName, "Begin Method " + methodName);
                Processor processor = new Processor();
                processor.Process();
                logger.Trace(className, methodName, "End Method " + methodName);
                Console.WriteLine("Record Processing Completed");
            }
            catch (Exception ex)
            {
                logger.Error(className, methodName, ex.Message, ex);
                throw;
            }
        }
    }
}
