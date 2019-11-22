////-----------------------------------------------------------------------
////(C) 2008 Brierley & Partners.  All Rights Reserved
////THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
////-----------------------------------------------------------------------

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;

//using Brierley.FrameWork.Common;
//using Brierley.FrameWork.Common.Logging;

//namespace Brierley.FrameWork.Common.Process
//{
//    public static class ProcessUtil
//    {
//        private const string _className = "ProcessUtil";
//        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

//        public static int ExecuteWithoutFeedback(string cmd, string args)
//        {
//            const string methodName = "ExecuteWithoutFeedback";

//            System.Diagnostics.Process p = new System.Diagnostics.Process();
//            int exitCode = 1;
//            try
//            {
//                Console.Out.WriteLine("Command file: " + cmd);
//                Console.Out.WriteLine("Arguments: " + args);
//                p.StartInfo.FileName = cmd;
//                p.StartInfo.CreateNoWindow = true;
//                p.StartInfo.Arguments = args;                
//                p.StartInfo.UseShellExecute = false;
//                //p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
//                p.Start();                
//                p.WaitForExit();
//                exitCode = p.ExitCode;
//            }
//            catch (Exception ex)
//            {
//                string msg = string.Format("Error executing command: {0} {1}", cmd, args);
//                _logger.Error(_className, methodName, msg, ex);
//                Console.Error.WriteLine(ex.Message);
//                throw;
//            }
//            finally
//            {
//                p.Close();
//            }
//            return exitCode;
//        }

//        private static int Execute(string cmd, string args,ref StreamReader stderror,ref StreamReader stdout)
//        {
//            const string methodName = "Execute";

//            System.Diagnostics.Process p = new System.Diagnostics.Process();            
//            int exitCode = 1;
//            try
//            {
//                Console.Out.WriteLine("Command file: " + cmd);
//                Console.Out.WriteLine("Arguments: " + args);
//                p.StartInfo.FileName = cmd;
//                p.StartInfo.CreateNoWindow = true;
//                p.StartInfo.Arguments = args;
//                p.StartInfo.RedirectStandardError = true;
//                p.StartInfo.RedirectStandardOutput = true;
//                p.StartInfo.UseShellExecute = false;
//                p.Start();
//                stderror = p.StandardError;
//                stdout = p.StandardOutput;
//                p.WaitForExit();                
//                exitCode = p.ExitCode;
//            }
//            catch (Exception ex)
//            {
//                string msg = string.Format("Error executing command: {0} {1}", cmd, args);
//                _logger.Error(_className, methodName, msg, ex);
//                Console.Error.WriteLine(ex.Message);
//                throw;
//            }
//            finally
//            {
//                p.Close();                
//            }
//            return exitCode;
//        }

//        public static int Execute(string cmd,string args)
//        {
//            const string methodName = "Execute";

//            StreamReader stderror = null;
//            StreamReader stdout = null;
//            int exitCode = -1;
//            try
//            {
//                exitCode = Execute(cmd, args, ref stderror, ref stdout);
//                string error = stderror.ReadToEnd();
//                string output = stdout.ReadToEnd();
//                if (!string.IsNullOrEmpty(error))
//                {
//                    Console.Error.Write(error);
//                }
//                if (!string.IsNullOrEmpty(output))
//                {
//                    Console.Out.Write(output);
//                }
//            }
//            catch (Exception ex)
//            {
//                string msg = string.Format("Error executing command: {0} {1}", cmd, args);
//                _logger.Error(_className, methodName, msg, ex);
//                throw;
//            }
//            finally
//            {
//                if (stderror != null)
//                {
//                    stderror.Close();
//                }
//                if (stdout != null)
//                {
//                    stdout.Close();
//                }
//            }
//            return exitCode;            
//        }

//        public static int ExecuteWithReturn(string cmd, string args,ref string error,ref string output)
//        {
//            const string methodName = "ExecuteWithReturn";

//            StreamReader stderror = null;
//            StreamReader stdout = null;
//            int exitCode = -1;
//            try
//            {
//                exitCode = Execute(cmd, args, ref stderror, ref stdout);
//                error = stderror.ReadToEnd();
//                output = stdout.ReadToEnd();
//                if (!string.IsNullOrEmpty(error))
//                {
//                    Console.Error.Write(error);
//                }
//                if (!string.IsNullOrEmpty(output))
//                {
//                    Console.Out.Write(output);
//                }
//            }
//            catch (Exception ex)
//            {
//                string msg = string.Format("Error executing command: {0} {1}", cmd, args);
//                _logger.Error(_className, methodName, msg, ex);
//                throw;
//            }
//            finally
//            {
//                if (stderror != null)
//                {
//                    stderror.Close();
//                }
//                if (stdout != null)
//                {
//                    stdout.Close();
//                }
//            }
//            return exitCode;
//        }
//    }
//}
