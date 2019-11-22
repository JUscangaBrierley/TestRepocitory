//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;

using Commons.Collections;

using NVelocity.App;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.IO;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Process;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.CodeGen;
using Brierley.LoyaltyWare.LWIntegrationSvc;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen
{
    public class JavaIntegrationClientLibGenerator : IDisposable
    {
        private const string _className = "JavaIntegrationClientLibGenerator";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        string solutionPath = string.Empty;
        string srcBaseFolder = string.Empty;
        GeneratedSolutionInfo info = null;
        private StringBuilder classPath = new StringBuilder();
        //string ns = string.Empty;
        private bool zipIt = true;
        private bool buildIt = true;
        private string templateFolder = string.Empty;
        private bool buildStatus = false;

        public JavaIntegrationClientLibGenerator(string templateFolder, bool buildIt, bool zipIt)
        {
            string methodName = "JavaIntegrationClientLibGenerator";

            this.buildIt = buildIt;
            this.zipIt = zipIt;
            this.templateFolder = string.Format("{0}ClientLib{1}", IOUtils.AppendSeparatorToFolderPath(templateFolder), Path.DirectorySeparatorChar);

            //this.solutionPath = string.Format("{0}Brierley.LoyaltyWare.ClientLib{1}", Path.GetTempPath(), Path.DirectorySeparatorChar);
            this.solutionPath = string.Format("{0}Brierley.LoyaltyWare.ClientLib{1}", IOUtils.GetTempDirectory(), Path.DirectorySeparatorChar);

            _logger.Trace(_className, methodName, "Temporary path for solution files = " + this.solutionPath);
            
            // make sure the outdir exists.
            try
            {
                if (!Directory.Exists(this.solutionPath))
                {
                    _logger.Debug(_className, methodName, "Creating directory " + this.solutionPath);
                    Directory.CreateDirectory(this.solutionPath);
                }

                // create the bin folder
                string folder = string.Format("{0}bin{1}", this.solutionPath, Path.DirectorySeparatorChar);
                Directory.CreateDirectory(folder);
                classPath.Append(folder);

                // create the lib folder
                folder = string.Format("{0}lib{1}", this.solutionPath, Path.DirectorySeparatorChar);
                Directory.CreateDirectory(folder);
                // now copy the necessary libs
                CopyCommonLibs(templateFolder, folder);

                // create the src folder
                folder = string.Format("{0}src{1}", this.solutionPath, Path.DirectorySeparatorChar);
                Directory.CreateDirectory(folder);

                folder = string.Format("{0}com{1}", folder, Path.DirectorySeparatorChar);
                Directory.CreateDirectory(folder);

                folder = string.Format("{0}brierley{1}", folder, Path.DirectorySeparatorChar);
                Directory.CreateDirectory(folder);

                folder = string.Format("{0}loyaltyware{1}", folder, Path.DirectorySeparatorChar);
                Directory.CreateDirectory(folder);

                folder = string.Format("{0}clientlib{1}", folder, Path.DirectorySeparatorChar);
                Directory.CreateDirectory(folder);

                srcBaseFolder = folder;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error creating directory.", ex);
                throw;
            }
        }

        private Dictionary<string, LWIntegrationDirectives.OperationParmType> GetStructsToGenerate(
            Dictionary<string, LWIntegrationDirectives.OperationParmType> map,
            LWIntegrationDirectives.OperationParmType p)
        {
            if (p.Type == LWIntegrationDirectives.ParmDataTypeEnums.Struct)
            {
                if (!map.ContainsKey(p.Name))
                {
                    map.Add(p.Name, p);
                }
                else
                {
                    // Determine if there are differences
                    foreach (var parm in map[p.Name].Parms)
                    {
                        var parm2 = (from t in p.Parms where parm.Name == t.Name select t).FirstOrDefault();
                        if (parm2 == null)
                            throw new LWCodeGenException(string.Format("Multiple versions of {0} found but {1} is not found in one.", p.Name, parm.Name));
                        if (parm2.Type != parm.Type)
                            throw new LWCodeGenException(string.Format("Multiple versions of {0} found but {1} is both of type {2} and {3}.", p.Name, parm.Name, parm.Type, parm2.Type));
                        if (parm2.IsRequired != parm.IsRequired)
                            throw new LWCodeGenException(string.Format("Multiple versions of {0} found but {1} is both required and not required.", p.Name, parm.Name));
                        if (parm.Type == LWIntegrationDirectives.ParmDataTypeEnums.String && parm.StringLength != parm2.StringLength)
                            throw new LWCodeGenException(string.Format("Multiple versions of {0} found but {1} has varying string lengths.", p.Name, parm.Name));
                    }
                    // Make sure we don't have any new extra parms
                    foreach (var parm in p.Parms)
                    {
                        var parm2 = (from t in map[p.Name].Parms where parm.Name == t.Name select t).FirstOrDefault();
                        if (parm2 == null)
                            throw new LWCodeGenException(string.Format("Multiple versions of {0} found but {1} is not found in one.", p.Name, parm.Name));
                    }
                }
            }
            foreach (LWIntegrationDirectives.OperationParmType sp in p.Parms)
            {
                map = GetStructsToGenerate(map, sp);
            }
            return map;
        }

        private Dictionary<string, LWIntegrationDirectives.OperationParmType> GetStructsToGenerate(
            Dictionary<string, LWIntegrationDirectives.OperationParmType> map,
            LWIntegrationDirectives.OperationDirective directive)
        {
            if (map == null)
            {
                map = new Dictionary<string, LWIntegrationDirectives.OperationParmType>();
            }
            if (directive.OperationMetadata.OperationInput != null && directive.OperationMetadata.OperationInput.InputParms != null)
            {
                foreach (LWIntegrationDirectives.OperationParmType p in directive.OperationMetadata.OperationInput.InputParms)
                {
                    map = GetStructsToGenerate(map, p);
                }
            }
            if (directive.OperationMetadata.OperationOutput != null && directive.OperationMetadata.OperationOutput.OutputParms != null)
            {
                foreach (LWIntegrationDirectives.OperationParmType p in directive.OperationMetadata.OperationOutput.OutputParms)
                {
                    map = GetStructsToGenerate(map, p);
                }
            }
            return map;
        }

        public GeneratedSolutionInfo Generate(string configFile)
        {
            string methodName = "Generate";

            info = new GeneratedSolutionInfo();
            try
            {
                IList<string> filesToBuild = new List<string>();

                // initialize the configuration file
                LWIntegrationDirectives config = new LWIntegrationDirectives();
                config.Load(configFile);

                List<AttributeSetMetaData> attSets;
                using (LoyaltyDataService dataService = LWDataServiceUtil.LoyaltyDataServiceInstance())
                {
                    // Generate necessary source files
                    attSets = dataService.GetAllTopLevelAttributeSets();
                }
                // generate the project code                
                // Generate the data model classes
                info.SolutionPath = this.solutionPath;

                filesToBuild = CopyGenericCode(filesToBuild);

                //copy the domain model framework stuff
                string folder = string.Format("{0}domainmodel{1}client{1}", this.srcBaseFolder, Path.DirectorySeparatorChar);
                Directory.CreateDirectory(folder);

                if (attSets != null && attSets.Count > 0)
                {
                    foreach (AttributeSetMetaData attSet in attSets)
                    {
                        filesToBuild = GenerateAttributeSetCode(filesToBuild, attSet);
                    }
                }

                Dictionary<string, LWIntegrationDirectives.OperationParmType> map = null;
                foreach (LWIntegrationDirectives.OperationDirective op in config.OperationDirectives.Values)
                {
                    if (op.OperationMetadata.OperationInput != null && op.OperationMetadata.OperationInput.InputParms != null)
                    {
                        if (op.OperationMetadata.OperationInput.InputParms.Count > 1)
                        {
                            // generate the in class
                            string className = op.Name + "In";
                            GenerateOperationParmTypeWrapperClass(filesToBuild, className, op.OperationMetadata.OperationInput.InputParms);
                        }
                    }
                    if (op.OperationMetadata.OperationOutput != null && op.OperationMetadata.OperationOutput.OutputParms != null)
                    {
                        if (op.OperationMetadata.OperationOutput.OutputParms.Count > 1)
                        {
                            // generate the in class
                            string className = op.Name + "Out";
                            GenerateOperationParmTypeWrapperClass(filesToBuild, className, op.OperationMetadata.OperationOutput.OutputParms);
                        }
                    }
                    map = GetStructsToGenerate(map, op);
                }
                foreach (KeyValuePair<string, LWIntegrationDirectives.OperationParmType> kp in map)
                {
                    GenerateOperationParmTypeClass(filesToBuild, kp.Value);
                }

                filesToBuild = GenerateSvcManagerClass(filesToBuild, config);

                string classesToBuildFname = string.Format("{0}ClassesToBuild.txt", this.solutionPath);
                using (TextWriter writer = CodeGenUtil.GetWriter(classesToBuildFname, null))
                {
                    foreach (string file in filesToBuild)
                    {
                        writer.WriteLine(file);
                    }
                }
                string compilerOptionsFname = string.Format("{0}CompilerOptions.txt", this.solutionPath);
                using (TextWriter writer = CodeGenUtil.GetWriter(compilerOptionsFname, null))
                {
                    string cp = string.Format("-classpath {0}", classPath.ToString());
                    writer.WriteLine(cp);
                    string d = string.Format("-d {0}bin{1}", this.solutionPath, Path.DirectorySeparatorChar);
                    writer.WriteLine(d);
                }

                // Build the solution
                if (buildIt)
                {
                    _logger.Debug(_className, methodName, "Building solution.");                    
                    info.AssemblyFileName = BuildSolution(classesToBuildFname, compilerOptionsFname);
                    buildStatus = true;
                }
                // Create a zip file
                if (zipIt)
                {
                    _logger.Debug(_className, methodName, "Zipping up the solution.");
                    string path = Path.GetTempFileName();
                    String baseName = Path.GetFileNameWithoutExtension(path);
                    String tempDir = Path.GetDirectoryName(path);
                    //File.Delete(compilerOptionsFname);
                    //File.Delete(classesToBuildFname);
                    string zipfile = tempDir + Path.DirectorySeparatorChar + baseName + ".zip";
                    File.Delete(path);
                    ZipUtils.ZipFolder(solutionPath, zipfile);
                    info.ZipFileName = zipfile;
                }
            }
            catch (LWCodeGenException ex)
            {
                _logger.Error(_className, methodName, "Error generating code.", ex);
                throw;
            }
            finally
            {
                //if (!string.IsNullOrEmpty(fwkAssembly) && File.Exists(fwkAssembly))
                //{
                //    File.Delete(fwkAssembly);
                //}
            }
            return info;
        }

        #region Private Methods

        public string BuildSolution(string classesToBuildFname, string compilerOptionsFname)
        {
            string methodName = "BuildSolution";

            _logger.Trace(_className, methodName, "Building Java solution.");

            try
            {
                string javahome = System.Environment.GetEnvironmentVariable("JAVA_HOME");
                if (string.IsNullOrEmpty(javahome))
                {
                    throw new LWCodeGenException("Unable to find JAVA_HOME directory from the environment.");
                }

                javahome = Brierley.FrameWork.Common.IO.IOUtils.AppendSeparatorToFolderPath(javahome);

                _logger.Debug(_className, methodName, "JAVA_HOME directory = " + javahome);

                string javac = javahome + @"bin\javac.exe";
                if (!File.Exists(javac))
                {
                    throw new LWCodeGenException("Unable to find java compiler at path " + javac);
                }
                _logger.Debug(_className, methodName, "Using java compiler " + javac);

                string dir = Brierley.FrameWork.Common.IO.IOUtils.AppendSeparatorToFolderPath(Path.GetDirectoryName(classesToBuildFname));
                string compilerout = dir + "compiler.out";
                string args = string.Format("@{0} @{1}", classesToBuildFname, compilerOptionsFname, compilerout);
                string error = string.Empty;
                //string output = string.Empty;
                //int rc = ProcessUtil.ExecuteWithReturn(javac, args, ref error, ref output);
				ManagedProcess managedProcess = new ManagedProcess();
				int rc = managedProcess.Execute(javac, args);
                //output = managedProcess.StdOut;
				error = managedProcess.StdErr;
                if (rc != 0)
                {
                    string msg = string.Format("Failed to successfully build java solution.\n{0}", error);
                    throw new LWCodeGenException(msg);
                }

                string jar = javahome + @"bin\jar.exe";
                if (!File.Exists(jar))
                {
                    throw new LWCodeGenException("Unable to find jar at path " + javac);
                }

                string currentRuntimeDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(this.solutionPath + "bin");
                string jarfile = dir + "brierleyclient.jar";
                try
                {
                    args = string.Format("cf {0} .", jarfile, this.solutionPath);
                    error = string.Empty;
                    //output = string.Empty;
                    //rc = ProcessUtil.ExecuteWithReturn(jar, args, ref error, ref output);
					rc = managedProcess.Execute(jar, args);
                    //output = managedProcess.StdOut;
					error = managedProcess.StdErr;
                }
                finally
                {
                    Directory.SetCurrentDirectory(currentRuntimeDir);
                }
                if (rc != 0)
                {
                    string msg = string.Format("Failed to successfully build jar file.\n{0}", error);
                    throw new LWCodeGenException(msg);
                }

                string assemblyPath = jarfile;
                if (!File.Exists(assemblyPath))
                {
                    throw new LWCodeGenException("Compiled jar file " + assemblyPath + " not found.");
                }
                _logger.Trace(_className, methodName, "Successfully built jar file " + assemblyPath);
                return assemblyPath;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error building solution.", ex);
                throw;
            }
        }

        private void CopyCommonLibs(string templateFolder, string libPath)
        {
            string javalibs = LWConfigurationUtil.GetConfigurationValue("LWJavaLibs");
            if (!string.IsNullOrEmpty(javalibs))
            {
                if (!Directory.Exists(javalibs))
                {
                    throw new LWIntegrationException(javalibs + " does not exist.");
                }
                javalibs = IOUtils.AppendSeparatorToFolderPath(javalibs);
            }
            else
            {
                DirectoryInfo templateFolderInfo = new DirectoryInfo(templateFolder);
                javalibs = string.Format("{0}{1}Java{1}", templateFolderInfo.FullName, Path.DirectorySeparatorChar);
            }

            IOUtils.CopyFile(javalibs + "brierleyclientproxy.jar", libPath + "brierleyclientproxy.jar", true);
            classPath.Append(Path.PathSeparator);
            classPath.Append(libPath + "brierleyclientproxy.jar");
            
            IOUtils.CopyFile(javalibs + "joda-time-2.0.jar", libPath + "joda-time-2.0.jar", true);
            classPath.Append(Path.PathSeparator);
            classPath.Append(libPath + "joda-time-2.0.jar");            
        }

        private static string CopyTemplateToTarget(string srcFolder, string srcName, string destFolder, string destName)
        {
            string srcFile = srcFolder + srcName;
            string destFile = destFolder + destName;
            IOUtils.CopyFile(srcFile, destFile, true);
            return destFile;
        }

        private IList<string> CopyGenericCode(IList<string> filesToBuild)
        {
            // copy the basic code
            string destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWClientException.java", this.srcBaseFolder, "LWClientException.java");
            filesToBuild.Add(destFile);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWCDISExtraArgs.java", this.srcBaseFolder, "LWCDISExtraArgs.java");
            filesToBuild.Add(destFile);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWLogger.java", this.srcBaseFolder, "LWLogger.java");
            filesToBuild.Add(destFile);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWXmlSerializer.java", this.srcBaseFolder, "LWXmlSerializer.java");
            filesToBuild.Add(destFile);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWUtils.java", this.srcBaseFolder, "LWUtils.java");
            filesToBuild.Add(destFile);

            // copy the annotations
            string folder = string.Format("{0}annotations{1}", this.srcBaseFolder, Path.DirectorySeparatorChar);
            Directory.CreateDirectory(folder);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWIsRequired.java", folder, "LWIsRequired.java");
            filesToBuild.Add(destFile);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWStringLength.java", folder, "LWStringLength.java");
            filesToBuild.Add(destFile);
            
            //copy the domain model stuff
            folder = string.Format("{0}domainmodel{1}", this.srcBaseFolder, Path.DirectorySeparatorChar);
            Directory.CreateDirectory(folder);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWAttributeSetContainer.java", folder, "LWAttributeSetContainer.java");
            filesToBuild.Add(destFile);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWClientDataObject.java", folder, "LWClientDataObject.java");
            filesToBuild.Add(destFile);

            //copy the domain model framework stuff
            folder = string.Format("{0}domainmodel{1}framework{1}", this.srcBaseFolder, Path.DirectorySeparatorChar);
            Directory.CreateDirectory(folder);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_Member.java", folder, "Member.java");
            filesToBuild.Add(destFile);
            destFile = CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_VirtualCard.java", folder, "VirtualCard.java");
            filesToBuild.Add(destFile);
            return filesToBuild;
        }

        private IList<string> GenerateAttributeSetCode(IList<string> filesToBuild, AttributeSetMetaData attSet)
        {
            if (attSet.ChildAttributeSets != null && attSet.ChildAttributeSets.Count > 0)
            {
                foreach (AttributeSetMetaData child in attSet.ChildAttributeSets)
                {
                    GenerateAttributeSetCode(filesToBuild, child);
                }
            }
            // now generate this attribute code for this attribute set.
            return GenerateTypeClass(filesToBuild, attSet);
        }

        private IList<string> GenerateTypeClass(IList<string> filesToBuild, AttributeSetMetaData attSet)
        {
            string folder = string.Format("{0}domainmodel{1}client{1}", this.srcBaseFolder, Path.DirectorySeparatorChar);
            string cns = "com.brierley.loyaltyware.clientlib.domainmodel.client";
            string fname = string.Format("{0}{1}.java", folder, attSet.Name);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            JavaClassMapping classMapping = new JavaClassMapping(cns, attSet.Name, attSet);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "javaclass.vm", classMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());
            filesToBuild.Add(fname);
            return filesToBuild;
        }

        private IList<string> GenerateOperationParmTypeWrapperClass(IList<string> filesToBuild, string name, IList<LWIntegrationDirectives.OperationParmType> parms)
        {
            string folder = string.Format("{0}domainmodel{1}client{1}", this.srcBaseFolder, Path.DirectorySeparatorChar);
            string cns = "com.brierley.loyaltyware.clientlib.domainmodel.client";
            string fname = string.Format("{0}{1}.java", folder, name);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            JavaOperationParmClassMapping classMapping = new JavaOperationParmClassMapping(cns, name, parms);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "javaoperationparmclass.vm", classMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());
            filesToBuild.Add(fname);
            return filesToBuild;
        }

        private IList<string> GenerateOperationParmTypeClass(IList<string> filesToBuild, LWIntegrationDirectives.OperationParmType parm)
        {
            string folder = string.Format("{0}domainmodel{1}client{1}", this.srcBaseFolder, Path.DirectorySeparatorChar);
            string cns = "com.brierley.loyaltyware.clientlib.domainmodel.client";
            string parmName = parm.Type == LWIntegrationDirectives.ParmDataTypeEnums.Struct ? parm.Name + "Struct" : parm.Name;
            string fname = string.Format("{0}{1}.java", folder, parmName);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            JavaOperationParmClassMapping classMapping = new JavaOperationParmClassMapping(cns, parmName, parm.Parms);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "javaoperationparmclass.vm", classMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());
            filesToBuild.Add(fname);
            return filesToBuild;
        }

        private IList<string> GenerateSvcManagerClass(IList<string> filesToBuild, LWIntegrationDirectives config)
        {
            string fname = string.Format("{0}LWIntegrationSvcClientManager.java", this.srcBaseFolder);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            JavaSvcManagerClassMapping classMapping = new JavaSvcManagerClassMapping(config);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "javasvcmgrclass.vm", classMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());
            filesToBuild.Add(fname);
            return filesToBuild;
        }

        #endregion

        public void Dispose()
        {
            string methodName = "Dispose";

            if (info != null)
            {
                try
                {
                    if (buildStatus && !string.IsNullOrEmpty(info.SolutionPath) && Directory.Exists(info.SolutionPath))
                    {
                        IOUtils.RemoveDirectoryTree(solutionPath, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(_className, methodName, "Error deleting " + info.SolutionPath, ex);
                }
                try
                {
                    if (!string.IsNullOrEmpty(info.ZipFileName) && File.Exists(info.ZipFileName))
                    {
                        File.Delete(info.ZipFileName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(_className, methodName, "Error deleting " + info.ZipFileName, ex);
                }
            }
        }
    }
}
