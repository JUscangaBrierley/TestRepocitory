using System;
using System.Collections.Generic;
using System.Configuration ;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using AmericanEagle.SDK.Global;

namespace Brierley.Clients.AE.TlogCounter
{


    /// <summary>
    /// Class Processor
    /// </summary>
    public class Processor
    {
        /// <summary>
        /// string fileName
        /// </summary>
        ///
        private string path = string.Empty;
        private static string fileName = "TLOGPOSTFLAG" ;
        private static string OutfileName = "TLOGPOSTREADY" + DateTime.Now.ToString("yyMMdd") + ".TXT";

        /// <summary>
        /// string jobName
        /// </summary>
        private static string jobName = "ProcessTlogwatcher";

        /// <summary>
        /// Logger for Processor
        /// </summary>
        private static LWLogger logger;

        /// <summary>
        /// Holds Class Name
        /// </summary>
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Initializes a new instance of the Processor class
        /// </summary>
        public Processor()
        {
            logger = LWLoggerManager.GetLogger("initializing");

        }

        /// <summary>
        /// Method Process
        /// </summary>
        public void Process( string arg)
        { 
        if (null == ConfigurationManager.AppSettings["FilePath"])
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No path defined in app.config");
                throw new Exception("No path defined in app.config");
            }
            else
            {
                path = ConfigurationManager.AppSettings["FilePath"];
            
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
                  Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
           
       //     if ( File.Exists(fileName1) && File.Exists(fileName2) && File.Exists(fileName3) && File.Exists(fileName4) && File.Exists(fileName5) && File.Exists(fileName6))
            string newpath = path+@"\" + fileName + arg + ".TXT" ;
            if (!(File.Exists(newpath)))
            {
                File.Create(newpath);        
            }
            
            {
                DirectoryInfo d = new DirectoryInfo(path);
                FileInfo[] Files = d.GetFiles("TLOGPOSTFLAG*.txt"); //Getting Text files
                if (Files.Length == 6)
                {
                  string outpath = path+@"\" + OutfileName  + ".TXT" ;
                  File.Create(outpath);  
                }
                else
                {
                    logger = LWLoggerManager.GetLogger("All Tlogs have not been processed");
                }
                
            }
            
            }
        }
 }
    

