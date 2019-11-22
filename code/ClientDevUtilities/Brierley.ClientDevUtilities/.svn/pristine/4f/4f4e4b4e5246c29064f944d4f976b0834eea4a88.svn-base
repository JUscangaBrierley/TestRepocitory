//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Process;

namespace Brierley.FrameWork.CodeGen
{
    public class CodeGenUtil
    {
        #region Fields
        private const string _className = "CodeGenUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        #endregion

        public static string GetTemplatePath(string mappedDir)
        {
            string templatesPath = System.Configuration.ConfigurationManager.AppSettings["LWTemplatePath"];
            if (string.IsNullOrEmpty(templatesPath))
            {
                templatesPath = mappedDir;
            }
            if (string.IsNullOrEmpty(templatesPath) || !System.IO.Directory.Exists(templatesPath))
            {
                throw new System.IO.DirectoryNotFoundException("Cannot locate template path.  Please make sure that either LWTemplatePath is correctly defined in web.config or Templates subdirectory exists.");
            }
            return templatesPath;
        }

        public static string BuildSolution(string solutionPath, string baseName)
        {
            string methodName = "BuildSolution";

            _logger.Trace(_className, methodName, "Building solution.");

            string tempPath = System.Environment.GetEnvironmentVariable("TEMP");
            string tmpPath = System.Environment.GetEnvironmentVariable("TMP");

            try
            {
                System.Environment.SetEnvironmentVariable("TEMP", solutionPath);
                System.Environment.SetEnvironmentVariable("TMP", solutionPath);

                string windir = System.Environment.GetEnvironmentVariable("windir");
                if (string.IsNullOrEmpty(windir))
                {
                    throw new LWCodeGenException("Unable to find windows directory from the environment.");
                }
                _logger.Debug(_className, methodName, "Windows directory = " + windir);

                string msbuildPath = windir + @"\Microsoft.NET\Framework\v4.0.30319\msbuild.exe";
                if (!File.Exists(msbuildPath))
                {
                    throw new LWCodeGenException("Unable to find msbuild command at path " + msbuildPath);
                }
                _logger.Debug(_className, methodName, "MSBuild path = " + msbuildPath);

                //string solutionFile = string.Format("\"{0}{1}.sln\"", solutionPath, DataServiceUtil.GetClientsDataBaseName());
                string solutionFile = string.Format("\"{0}{1}.sln\"", solutionPath, baseName);

                string cmd = msbuildPath;

                // Spawn a process to build the solution file
                ManagedProcess managedProcess = new ManagedProcess();
                int rc = managedProcess.Execute(cmd, solutionFile);
                if (rc != 0)
                {
                    string msg = string.Format("Failed to successfully build solution {0}.  Please see Loyalty Navigator logs for more details.", solutionFile);
                    throw new LWCodeGenException(msg);
                }

                string assemblyPath = string.Format("{0}{1}bin{1}Debug{1}{2}.dll", solutionPath, Path.DirectorySeparatorChar, baseName);
                if (!File.Exists(assemblyPath))
                {
                    throw new LWCodeGenException("Compiled assembly " + assemblyPath + " not found.");
                }
                _logger.Trace(_className, methodName, "Successfully built assembly " + assemblyPath);
                return assemblyPath;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error building solution.", ex);
                throw;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(tempPath))
                {
                    System.Environment.SetEnvironmentVariable("TEMP", tempPath);
                }
                if (!string.IsNullOrWhiteSpace(tmpPath))
                {
                    System.Environment.SetEnvironmentVariable("TMP", tempPath);
                }
            }
        }

        public static TextWriter GetWriter(string filename, Encoding encoding)
        {
            TextWriter writer = null;
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            FileStream fstream = File.Create(filename);
            if (encoding != null)
            {
                writer = new StreamWriter(fstream, encoding);
            }
            else
            {
                writer = new StreamWriter(fstream);
            }
            return writer;
        }
    }
}
