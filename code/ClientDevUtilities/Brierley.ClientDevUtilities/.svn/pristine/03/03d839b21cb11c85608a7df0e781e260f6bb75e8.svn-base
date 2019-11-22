//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using Commons.Collections;

using NVelocity.App;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Process;
using Brierley.FrameWork.Common.IO;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.CodeGen.ClientDataModel
{
    public class ClientDataModelSolutionGenerator : IDisposable
    {
        private const string _className = "ClientDataModelSolutionGenerator";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private bool _disposed = false;
        string ns = string.Empty;
        string solutionPath = string.Empty;
        GeneratedSolutionInfo info = null;
        string organizationName = string.Empty;
		private bool zipIt = true;
        private bool buildIt = true;

        public ClientDataModelSolutionGenerator(bool buildIt, bool zipIt)
        {
            string methodName = "ClientDataModelSolutionGenerator";

            this.buildIt = buildIt;
            this.zipIt = zipIt;
            ns = DataServiceUtil.ClientDataModelNamespace;
            _logger.Debug(_className, methodName, "Name space = " + ns);

            this.solutionPath = IOUtils.GetTempDirectory() + ns + Path.DirectorySeparatorChar;

            _logger.Trace(_className, methodName, "Temporary path for solution files = " + this.solutionPath);

            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            organizationName = ctx.Organization;
            // make sure the outdir exists.
            try
            {
                if (!Directory.Exists(this.solutionPath))
                {
                    _logger.Debug(_className, methodName, "Creating directory " + this.solutionPath);
                    Directory.CreateDirectory(this.solutionPath);
                }
                string mapperDirBase = this.solutionPath + "Mappers" + Path.DirectorySeparatorChar;
                if (!Directory.Exists(mapperDirBase))
                {
                    _logger.Debug(_className, methodName, "Creating directory " + mapperDirBase);
                    Directory.CreateDirectory(mapperDirBase);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error creating directory.", ex);
                throw;
            }
        }

        public GeneratedSolutionInfo Generate()
        {
            string methodName = "Generate";

            info = new GeneratedSolutionInfo();
            string fwkAssembly = string.Empty;
            try
            {
				using (var loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					// Generate necessary source files
					List<AttributeSetMetaData> attSets = loyaltyService.GetAllTopLevelAttributeSets();
					string publicKeyToken = GenerateStrongNameKey();
					_logger.Trace(_className, methodName, "Public token = " + publicKeyToken);
					GenerateProjectCode(publicKeyToken, loyaltyService);
					GenerateSolutionCode();
					info.SolutionPath = this.solutionPath;
					if (attSets != null && attSets.Count > 0)
					{
						foreach (AttributeSetMetaData attSet in attSets)
						{
							GenerateAttributeSetCode(publicKeyToken, attSet);
						}
					}
					else
					{
						// there are no attribute sets in this Org.  Create an empty class so that
						// the solution can be compiled atleast.
						AddEmptyClassToProject();
					}

					// Build the solution
					if (buildIt)
					{
						_logger.Debug(_className, methodName, "Building solution.");
						fwkAssembly = CopyFrameworkAssembly();
						info.AssemblyFileName = CodeGenUtil.BuildSolution(solutionPath, DataServiceUtil.ClientsDataBaseName);
					}
					// Create a zip file
					if (zipIt)
					{
						_logger.Debug(_className, methodName, "Zipping up the solution.");
						string path = Path.GetTempFileName();
						string baseName = Path.GetFileNameWithoutExtension(path);
						string tempDir = Path.GetDirectoryName(path);
						string zipfile = IOUtils.GetTempDirectory() + baseName + ".zip";
						File.Delete(path);
						if (ZipUtils.ZipFolder(solutionPath, zipfile))
						{
							info.ZipFileName = zipfile;
						}
						else
						{
							throw new LWCodeGenException("Unable to create zip file " + zipfile);
						}
					}
				}
            }
            catch (LWCodeGenException ex)
            {
                _logger.Error(_className, methodName, "Error generating code.", ex);
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fwkAssembly) && File.Exists(fwkAssembly))
                {
                    File.Delete(fwkAssembly);
                }
            }            
            return info;
        }

        #region Private Methods

        private string CopyFrameworkAssembly()
        {
            System.Reflection.Assembly fwkAssembly = System.Reflection.Assembly.GetExecutingAssembly();            
            string srcAssembly = fwkAssembly.CodeBase;
            if (srcAssembly.StartsWith("file:///"))
            {
                // It is in URI format.  Convert it to absolute path.
                srcAssembly = srcAssembly.Substring(8);                
            }
            if (!Directory.Exists(this.solutionPath + "bin"))
            {
                Directory.CreateDirectory(this.solutionPath + "bin");
            }
            if (!Directory.Exists(this.solutionPath + "bin\\debug"))
            {
                Directory.CreateDirectory(this.solutionPath + "bin\\debug");
            }
            string dstAssembly = string.Format("{0}bin\\debug\\Brierley.FrameWork.dll", this.solutionPath);
            Brierley.FrameWork.Common.IO.IOUtils.CopyFile(srcAssembly, dstAssembly, true);
            return dstAssembly;
        }

        //private string BuildSolution()
        //{
        //    string methodName = "BuildSolution";

        //    _logger.Trace(_className, methodName, "Building solution.");

        //    try
        //    {
        //        string windir = System.Environment.GetEnvironmentVariable("windir");
        //        if (string.IsNullOrEmpty(windir))
        //        {
        //            throw new LWCodeGenException("Unable to find windows directory from the environment.");
        //        }
        //        _logger.Debug(_className, methodName, "Windows directory = " + windir);

        //        //string msbuildPath = windir + @"\Microsoft.NET\Framework\v2.0.50727\msbuild.exe";
        //        //string msbuildPath = windir + @"\Microsoft.NET\Framework\v3.5\msbuild.exe";
        //        string msbuildPath = windir + @"\Microsoft.NET\Framework\v4.0.30319\msbuild.exe";
        //        if (!File.Exists(msbuildPath))
        //        {
        //            throw new LWCodeGenException("Unable to find msbuild command at path " + msbuildPath);
        //        }
        //        _logger.Debug(_className, methodName, "MSBuild path = " + msbuildPath);

        //        string solutionFile = string.Format("\"{0}{1}.sln\"", this.solutionPath, DataServiceUtil.GetClientsDataBaseName());

        //        string cmd = msbuildPath;

        //        if (ProcessUtil.ExecuteWithoutFeedback(cmd, solutionFile) != 0)
        //        {
        //            string msg = string.Format("Failed to successfully build solution {0}", solutionFile);
        //            throw new LWCodeGenException(msg);
        //        }

        //        string assemblyPath = string.Format("{0}{1}bin{1}Debug{1}{2}.dll", this.solutionPath, Path.DirectorySeparatorChar, DataServiceUtil.GetClientsDataBaseName());
        //        if (!File.Exists(assemblyPath))
        //        {
        //            throw new LWCodeGenException("Compiled assembly " + assemblyPath + " not found.");
        //        }
        //        _logger.Trace(_className, methodName, "Successfully built assembly " + assemblyPath);
        //        return assemblyPath;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(_className, methodName, "Error building solution.", ex);
        //        throw;
        //    }
        //}

        private string SearchExecutable(string exeName)
        {
            string methodName = "SearchExecutable";

            string exePath = string.Empty;

            string searchPath = System.Configuration.ConfigurationManager.AppSettings["LWAssemblyPath"];
            if (string.IsNullOrEmpty(searchPath))
            {
                // look for search path in the environment.
                searchPath = System.Environment.GetEnvironmentVariable("LWAssemblyPath");
            }
			_logger.Debug(_className, methodName, string.Format("Search for {0} in path {1}", exeName, searchPath));
            if (!string.IsNullOrEmpty(searchPath))
            {
                string[] pathList = searchPath.Split(';');
                foreach (string path in pathList)
                {
                    if (Directory.Exists(path))
                    {
                        string[] files = Directory.GetFiles(path);
                        string fileToLoad = string.Empty;
                        foreach (string file in files)
                        {
                            if (Path.GetFileName(file).ToLower() == exeName.ToLower())
                            {
                                exePath = file;
                            }
                        }
                    }
                }
            }
            return exePath;
        }

        private string GenerateStrongNameKey()
        {
            string methodName = "GenerateStrongNameKey";

            string publicKeyToken = string.Empty;
            //string cmd = lnTemplatePath + "sn.exe";
            string cmd = SearchExecutable("sn.exe");
            if (string.IsNullOrEmpty(cmd))
            {
                string msg = "Unable to locate strong name generator: sn.exe";
                _logger.Critical(_className, methodName, msg);
                throw new LWDataModelGenException(msg);
            }
            string args = string.Format("-k \"{0}{1}ClientDataModel.snk\"", solutionPath, organizationName);
            //int rcode = ProcessUtil.Execute(cmd, args);
			ManagedProcess managedProcess = new ManagedProcess();
			int rcode = managedProcess.Execute(cmd, args);
            if (rcode == 0)
            {
                // strong name file has been successfully created.
                args = string.Format("-p \"{0}{1}ClientDataModel.snk\" \"{0}{1}ClientDataModelPublic.snk\"", solutionPath, organizationName);
                _logger.Debug(_className, methodName,
                    string.Format("Executing the following command: {0} {1}", cmd, args));
                //if (ProcessUtil.Execute(cmd, args) == 0)
				if (managedProcess.Execute(cmd, args) == 0)
                {
                    // public key has been successfully extracted.
                    args = string.Format("-t \"{0}{1}ClientDataModelPublic.snk\"", solutionPath, organizationName);
                    _logger.Debug(_className, methodName,
                    string.Format("Executing the following command: {0} {1}", cmd, args));
                    //string error = null;
                    string output = null;
                    //if (ProcessUtil.ExecuteWithReturn(cmd, args, ref error, ref output) == 0)
					if (managedProcess.Execute(cmd, args) == 0)
                    {
						output = managedProcess.StdOut;
						if (!string.IsNullOrEmpty(output) && output.Contains("Public key token"))
                        {
                            int index = output.LastIndexOf(" ");
                            int length = output.Length - index - 2;
                            publicKeyToken = output.Substring(index, length).Trim();
                        }
                    }
                    // delete the temporary file.
                    string pktfile = string.Format("{0}{1}ClientDataModelPublic.snk", solutionPath, organizationName);
                    File.Delete(pktfile);
                }
                else
                {
                    // unable to extract the public key
					string msg = string.Format("Unable to generate strong name key file.  Return code is {0}.  Please see Loyalty Navigator logs for more details.", rcode);
                    _logger.Critical(_className, methodName, msg);
                    throw new LWDataModelGenException(msg);
                }
            }
            else
            {
                // unable to extract the public key
                    string msg = string.Format("Unable to generate strong name key file.  Please see Loyalty Navigator logs for more details.");
                    _logger.Critical(_className,methodName,msg);
                    throw new LWDataModelGenException(msg);
            }
            return publicKeyToken;
        }
        
        private void GenerateAttributeSetCode(string publicKeyToken,AttributeSetMetaData attSet)
        {
            if (attSet.ChildAttributeSets != null && attSet.ChildAttributeSets.Count > 0)
            {
                foreach (AttributeSetMetaData child in attSet.ChildAttributeSets)
                {
                    GenerateAttributeSetCode(publicKeyToken, child);
                }
            }
            // now generate this attribute code for this attribute set.            
            GenerateTypeClass(publicKeyToken, attSet);
			GenerateTypeMapping(publicKeyToken, attSet);
        }
        
        private void GenerateProjectCode(string publicKeyToken, LoyaltyDataService service)
        {
            string assName = DataServiceUtil.ClientsDataBaseName;            
            string fname = string.Format("{0}\\{1}.csproj", this.solutionPath, assName);
            List<string> dbTypes = new List<string>();
            List<AttributeSetMetaData> attSets = service.GetAllAttributeSets();
            ProjectMapping projectMapping = new ProjectMapping(ns, assName, organizationName, attSets);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.FrameWork.CodeGen.ClientDataModel.Templates", "vsproject.vm", projectMapping, writer, null);
        }

		private void AddEmptyClassToProject()
		{
			string methodName = "AddEmptyClassToProject";

			FileWriter writer = null;
			try
			{
				string fname = string.Format("{0}\\{1}.csproj", this.solutionPath, ns);
				XmlDocument doc = new XmlDocument();
				doc.Load(fname);
				XmlNode firstItemGroup = Brierley.FrameWork.Common.XML.XmlUtils.SelectSingleNode(doc, "Project/ItemGroup");
				XmlNode emptyClassItemNode = doc.CreateElement("ItemGroup", doc.DocumentElement.NamespaceURI);
				XmlNode emptyClassNode = doc.CreateElement("Compile", doc.DocumentElement.NamespaceURI);
				XmlAttribute incAtt = doc.CreateAttribute("Include");
				incAtt.Value = "EmptyClass.cs";
				emptyClassNode.Attributes.Append(incAtt);
				emptyClassItemNode.AppendChild(emptyClassNode);
				doc.DocumentElement.InsertAfter(emptyClassItemNode, firstItemGroup);
				writer = new FileWriter(fname, null, true);
				writer.write(doc.OuterXml);				
				// now copy the empty class to project dir
                StringBuilder emptyClass = new StringBuilder("using System;\n");
                emptyClass.Append(string.Format("namespace Brierley.Clients.{0}DataModel\n", organizationName));
                emptyClass.Append("{");
                emptyClass.Append("class EmptyClass{}}");
                //string src = IOUtils.AppendSeparatorToFolderPath(this.lnTemplatePath) + "EmptyClass.cs";
				string dst = IOUtils.AppendSeparatorToFolderPath(this.solutionPath) + "EmptyClass.cs";
                //string srcStr = IOUtils.GetFromFile(src);
                //srcStr = srcStr.Replace("##Organization##", organizationName);
                //IOUtils.SaveToFile(dst, srcStr);
                IOUtils.SaveToFile(dst, emptyClass.ToString());				
			}
			catch (Exception ex)
			{
				string msg = "Error adding empty class to the project.";
				_logger.Error(_className, methodName, "Error adding empty class to the project.", ex);
				throw new LWCodeGenException(msg, ex);
			}
			finally
			{
				if (writer != null)
				{
					writer.dispose();
				}
			}
		}

        private void GenerateSolutionCode()
        {
            SolutionMapping solutionMapping = new SolutionMapping(DataServiceUtil.ClientsDataBaseName);
            string fname = string.Format("{0}\\{1}.sln", this.solutionPath, solutionMapping.BaseName);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.FrameWork.CodeGen.ClientDataModel.Templates", "vssolution.vm", solutionMapping, writer, null);            
        }
                
        private void GenerateFromTemplate(string publicKeyToken, string templatename, string dtType, string name, AttributeSetMetaData attSet, TextWriter writer)
        {
            ClassMapping classMapping = new ClassMapping(ns, name, DataServiceUtil.ClientsDataBaseName,dtType, publicKeyToken, attSet);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.FrameWork.CodeGen.ClientDataModel.Templates", templatename, classMapping, writer, null);            
        }
        
        private void GenerateTypeClass(string publicKeyToken, AttributeSetMetaData attSet)
        {
            string fname = string.Format("{0}\\{1}.cs", this.solutionPath, attSet.Name);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            GenerateFromTemplate(publicKeyToken, "netclass.vm", null, attSet.Name, attSet, writer);
            fname = string.Format("{0}\\{1}_AL.cs", this.solutionPath, attSet.Name);
            writer = CodeGenUtil.GetWriter(fname, null);
            GenerateFromTemplate(publicKeyToken, "alnetclass.vm", null, attSet.Name, attSet, writer);
        }

		private void GenerateTypeMapping(string publicKeyToken, AttributeSetMetaData attSet)
		{
			string fname = string.Format("{0}\\Mappers\\{1}Mapper.cs", this.solutionPath, attSet.Name);
			TextWriter writer = CodeGenUtil.GetWriter(fname, null);
			GenerateFromTemplate(publicKeyToken, "netmapper.vm", null, attSet.Name, attSet, writer);
			//audit log table can use the same mapper, so no nede to generate another one
			//fname = string.Format("{0}\\Mappings\\{1}\\{2}_AL.hbm.xml", this.solutionPath, dtType, attSet.Name);
			//writer = CodeGenUtil.GetWriter(fname, null);
			//GenerateFromTemplate(publicKeyToken, "alhbmmapping.vm", dtType, attSet.Name, attSet, writer);
		}

        #endregion

        protected virtual void Dispose(bool disposing)
        {
			string methodName = "Dispose";
			if (!_disposed)
			{
				if (disposing)
				{
					if (info != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(info.SolutionPath) && Directory.Exists(info.SolutionPath))
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
				_disposed = true;
			}
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
