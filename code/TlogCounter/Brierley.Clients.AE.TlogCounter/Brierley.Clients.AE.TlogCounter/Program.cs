using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Brierley.Clients.AE.TlogCounter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.Clients.AmericanEagle.DataModel;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Config;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    
   public class Program
    {
        /// <summary>
        /// Logger for Tlogwatcher
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("tlogwatcher begins");

        /// <summary>
        /// Holds Class Name
        /// </summary>
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Starting point of the application
        /// </summary>
        /// <param name="arrArgument">String array of arguments</param>
        static void Main(string[] args)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;

            try
            {
                Console.WriteLine("Processing Records.......");
                logger.Trace(className, methodName, "Begin Method " + methodName);
                Processor processor = new Processor();
                if (args.Length == 0)
                {
                    args = new string[] { "1" };
                }
                processor.Process(args[0]);
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
