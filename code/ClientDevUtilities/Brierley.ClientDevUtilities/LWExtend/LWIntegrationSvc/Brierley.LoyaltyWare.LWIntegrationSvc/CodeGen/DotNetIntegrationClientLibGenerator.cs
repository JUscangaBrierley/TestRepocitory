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

//using NVelocity;
using NVelocity.App;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Process;
using Brierley.FrameWork.Common.IO;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.CodeGen;

using Brierley.LoyaltyWare.LWIntegrationSvc;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen
{
    public class DotNetIntegrationClientLibGenerator : IDisposable
    {
        private const string _className = "DotNetIntegrationClientLibGenerator";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
       
        string solutionPath = string.Empty;
        GeneratedSolutionInfo info = null;
        string ns = string.Empty;
		private bool zipIt = true;
        private bool buildIt = true;
        private string templateFolder = string.Empty;
        private bool buildStatus = false;

        public DotNetIntegrationClientLibGenerator(string templateFolder, bool buildIt, bool zipIt)
        {
            string methodName = "IntegrationClientLibGenerator";
                      
            this.buildIt = buildIt;
            this.zipIt = zipIt;
            this.templateFolder = string.Format("{0}ClientLib{1}", IOUtils.AppendSeparatorToFolderPath(templateFolder), Path.DirectorySeparatorChar);

            ns = "Brierley.LoyaltyWare.ClientLib";
            //this.solutionPath = string.Format("{0}{2}{1}", Path.GetTempPath(),Path.DirectorySeparatorChar, ns);
            this.solutionPath = string.Format("{0}{2}{1}", IOUtils.GetTempDirectory(), Path.DirectorySeparatorChar, ns);

            _logger.Trace(_className, methodName, "Temporary path for solution files = " + this.solutionPath);

            //LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
           
            // make sure the outdir exists.
            try
            {
                if (!Directory.Exists(this.solutionPath))
                {
                    _logger.Debug(_className, methodName, "Creating directory " + this.solutionPath);
                    Directory.CreateDirectory(this.solutionPath);
                }

                string propertiesFolder = this.solutionPath + "Properties" + Path.DirectorySeparatorChar;
                if (!Directory.Exists(propertiesFolder))
                {
                    _logger.Debug(_className, methodName, "Creating directory " + propertiesFolder);
                    Directory.CreateDirectory(propertiesFolder);
                }

                string domainModelBase = this.solutionPath + "DomainModel" + Path.DirectorySeparatorChar;
                if (!Directory.Exists(domainModelBase))
                {
                    _logger.Debug(_className, methodName, "Creating directory " + domainModelBase);
                    Directory.CreateDirectory(domainModelBase);
                }

                string clientDataModel = domainModelBase + "Client" + Path.DirectorySeparatorChar;
                if (!Directory.Exists(clientDataModel))
                {
                    _logger.Debug(_className, methodName, "Creating directory " + clientDataModel);
                    Directory.CreateDirectory(clientDataModel);
                }
                string fwkDataModel = domainModelBase + "Framework" + Path.DirectorySeparatorChar;
                if (!Directory.Exists(fwkDataModel))
                {
                    _logger.Debug(_className, methodName, "Creating directory " + fwkDataModel);
                    Directory.CreateDirectory(fwkDataModel);
                }
                                              
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
                string key = p.Name + "Struct";
                if (!map.ContainsKey(key))
                {
                    map.Add(key, p);
                }
                else
                {
                    // Determine if there are differences
                    foreach (var parm in map[key].Parms)
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
                        var parm2 = (from t in map[key].Parms where parm.Name == t.Name select t).FirstOrDefault();
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
                //if (directive.OperationMetadata.OperationOutput.OutputParms.Count > 1)
                //{
                //    foreach (LWIntegrationDirectives.OperationParmType p in directive.OperationMetadata.OperationOutput.OutputParms)
                //    {
                //        map = GetStructsToGenerate(map, p);
                //    }
                //}                
            }
            return map;
        }

        private Dictionary<string, LWIntegrationDirectives.OperationParmType> GetAllStructsToGenerate(LWIntegrationDirectives directives)
        {
            Dictionary<string, LWIntegrationDirectives.OperationParmType> map = null;
            foreach (LWIntegrationDirectives.OperationDirective op in directives.OperationDirectives.Values)
            {                                
                map = GetStructsToGenerate(map, op);                
            }
            return map;
        }

        public GeneratedSolutionInfo Generate(string configFile)
        {
            string methodName = "Generate";

            info = new GeneratedSolutionInfo();            
            try
            {
                // initialize the configuration file
                LWIntegrationDirectives config = new LWIntegrationDirectives();
                config.Load(configFile);

                List<AttributeSetMetaData> attSets;
                Dictionary<string, LWIntegrationDirectives.OperationParmType> map = GetAllStructsToGenerate(config);
                // Generate necessary source files
                using (LoyaltyDataService dataService = LWDataServiceUtil.LoyaltyDataServiceInstance())
                {
                    attSets = dataService.GetAllTopLevelAttributeSets();
                }
                
                // generate the project code
                GenerateProjectCode(config, map);
                GenerateSolutionCode();
                // Generate the data model classes
                info.SolutionPath = this.solutionPath;
				if (attSets != null && attSets.Count > 0)
				{
					foreach (AttributeSetMetaData attSet in attSets)
					{
						GenerateAttributeSetCode(attSet);
					}
				}

                foreach (LWIntegrationDirectives.OperationDirective op in config.OperationDirectives.Values)
                {                    
                    if (op.OperationMetadata.OperationInput != null && op.OperationMetadata.OperationInput.InputParms != null)
                    {
                        if (op.OperationMetadata.OperationInput.InputParms.Count > 1)
                        {
                            // generate the in class
                            string className = op.Name + "In";
                            GenerateOperationParmTypeWrapperClass(className, op.OperationMetadata.OperationInput.InputParms);
                        }                        
                    }                    
                    if (op.OperationMetadata.OperationOutput != null && op.OperationMetadata.OperationOutput.OutputParms != null)
                    {
                        if (op.OperationMetadata.OperationOutput.OutputParms.Count > 1)
                        {
                            // generate the in class
                            string className = op.Name + "Out";
                            GenerateOperationParmTypeWrapperClass(className, op.OperationMetadata.OperationOutput.OutputParms);
                        }
                        if (op.OperationMetadata.OperationOutput.OutputParms.Count == 1)
                        {
                            if (op.OperationMetadata.OperationOutput.OutputParms[0].Type == LWIntegrationDirectives.ParmDataTypeEnums.Struct)
                            {
                                // generate the in class
                                string className = op.Name + "Struct";
                                GenerateOperationParmTypeWrapperClass(className, op.OperationMetadata.OperationOutput.OutputParms[0].Parms);
                            }
                        }
                    }                    
                }

                foreach (KeyValuePair<string, LWIntegrationDirectives.OperationParmType> kp in map)
                {
                    GenerateOperationParmTypeClass(kp.Value);
                }

                GenerateSvcManagerClass(config);

                CopyGenericCode();                

                // Build the solution
                if (buildIt)
                {
                    _logger.Debug(_className, methodName, "Building solution.");                    
                    info.AssemblyFileName = CodeGenUtil.BuildSolution(this.solutionPath,ns);
                    buildStatus = true;
                }
                // Create a zip file
                if (zipIt)
                {
                    _logger.Debug(_className, methodName, "Zipping up the solution.");
                    string path = Path.GetTempFileName();
					String baseName = Path.GetFileNameWithoutExtension(path);
                    String tempDir = Path.GetDirectoryName(path);
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

        private static void CopyTemplateToTarget(string srcFolder, string srcName, string destFolder, string destName)
        {
            string srcFile = srcFolder + srcName;
            string destFile = destFolder + destName;
            IOUtils.CopyFile(srcFile, destFile, true);            
        }

        private static void CopyReplaceTemplateToTarget(string srcFolder, string srcName, string destFolder, string destName)
        {
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            string srcFile = srcFolder + srcName;
            string srcStr = IOUtils.GetFromFile(srcFile);
            srcStr = srcStr.Replace("##OrganizationName##", ctx.Organization);
            string destFile = destFolder + destName;
            IOUtils.SaveToFile(destFile, srcStr);
            //IOUtils.CopyFile(srcFile, destFile, true);
        }

        private void CopyGenericCode()
        {
            CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_AssemblyInfo.cs", this.solutionPath + "Properties\\", "AssemblyInfo.cs");
            CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_Member.cs", this.solutionPath + "DomainModel\\Framework\\", "Member.cs");
            CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_VirtualCard.cs", this.solutionPath + "DomainModel\\Framework\\", "VirtualCard.cs");
            CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWAttributeSetContainer.cs", this.solutionPath + "DomainModel\\", "LWAttributeSetContainer.cs");
            CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWClientDataObject.cs", this.solutionPath + "DomainModel\\", "LWClientDataObject.cs");
            CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWClientException.cs", this.solutionPath , "LWClientException.cs");
            CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWLogger.cs", this.solutionPath, "LWLogger.cs");
            CopyReplaceTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWXmlSerializer.cs", this.solutionPath, "LWXmlSerializer.cs");
            CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWMetaAttribute.cs", this.solutionPath, "LWMetaAttribute.cs");
            CopyTemplateToTarget(this.templateFolder, "LWIntgrClientLib_LWIntegrationSvcProxy.cs", this.solutionPath, "LWIntegrationSvcProxy.cs");
        }

        private void GenerateAttributeSetCode(AttributeSetMetaData attSet)
        {
            if (attSet.ChildAttributeSets != null && attSet.ChildAttributeSets.Count > 0)
            {
                foreach (AttributeSetMetaData child in attSet.ChildAttributeSets)
                {
                    GenerateAttributeSetCode(child);
                }
            }
            // now generate this attribute code for this attribute set.
            GenerateTypeClass(attSet);                                   
        }

        private void GenerateProjectCode(LWIntegrationDirectives config, Dictionary<string, LWIntegrationDirectives.OperationParmType> map)
        {            
            string assName = "Brierley.LoyaltyWare.ClientLib";            
            string fname = string.Format("{0}\\Brierley.LoyaltyWare.ClientLib.csproj", this.solutionPath);           
            IList<AttributeSetMetaData> attSets;
            using (LoyaltyDataService dataService = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                attSets = dataService.GetAllAttributeSets();
            }
            
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);

            IList<string> ioClasses = new List<string>();

            foreach (LWIntegrationDirectives.OperationDirective op in config.OperationDirectives.Values)
            {
                if (op.OperationMetadata.OperationInput != null && op.OperationMetadata.OperationInput.InputParms != null)
                {
                    if (op.OperationMetadata.OperationInput.InputParms.Count > 0)
                    {
                        if (op.OperationMetadata.OperationInput.InputParms.Count == 1)
                        {
                            //if (op.OperationMetadata.OperationInput.InputParms[0].Type == LWIntegrationDirectives.ParmDataTypeEnums.Struct)
                            //{
                            //    ioClasses.Add(op.Name + "Struct.cs");
                            //}
                        }
                        else
                        {
                            ioClasses.Add(op.Name + "In.cs");
                        }
                    }
                }
                if (op.OperationMetadata.OperationOutput != null && op.OperationMetadata.OperationOutput.OutputParms != null)
                {
                    if (op.OperationMetadata.OperationOutput.OutputParms.Count > 0)
                    {
                        if (op.OperationMetadata.OperationOutput.OutputParms.Count == 1)
                        {
                            //if (op.OperationMetadata.OperationOutput.OutputParms[0].Type == LWIntegrationDirectives.ParmDataTypeEnums.Struct)
                            //{
                            //    ioClasses.Add(op.Name + "Struct.cs");
                            //}
                        }
                        else
                        {
                            ioClasses.Add(op.Name + "Out.cs");
                        }
                    }
                }                                
            }

            foreach (KeyValuePair<string, LWIntegrationDirectives.OperationParmType> kp in map)
            {
                ioClasses.Add(kp.Key + ".cs");
            } 

            DotNetProjectMapping projectMapping = new DotNetProjectMapping(ns, assName, attSets, ioClasses);

            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "vsproject.vm", projectMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());
        }
        
        private void GenerateSolutionCode()
        {
            SolutionMapping solutionMapping = new SolutionMapping(ns);
            string fname = string.Format("{0}\\{1}.sln", this.solutionPath, ns);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "vssolution.vm", solutionMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());            
        }
                                
        private void GenerateTypeClass(AttributeSetMetaData attSet)
        {
            string cns = ns + ".DomainModel.Client";
            string fname = string.Format("{0}DomainModel\\Client\\{1}.cs", this.solutionPath, attSet.Name);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            DotNetClassMapping classMapping = new DotNetClassMapping(cns, attSet.Name, attSet);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "netclass.vm", classMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());            
        }

        private void GenerateOperationParmTypeWrapperClass(string name,IList<LWIntegrationDirectives.OperationParmType> parms)
        {
            string cns = ns + ".DomainModel.Client";
            string fname = string.Format("{0}DomainModel\\Client\\{1}.cs", this.solutionPath, name);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            DotNetOperationParmClassMapping classMapping = new DotNetOperationParmClassMapping(cns, name, parms);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "netoperationparmclass.vm", classMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());
        }

        private void GenerateOperationParmTypeClass(LWIntegrationDirectives.OperationParmType parm)
        {
            string cns = ns + ".DomainModel.Client";
            string parmName = parm.Type == LWIntegrationDirectives.ParmDataTypeEnums.Struct ? parm.Name + "Struct" : parm.Name;
            string fname = string.Format("{0}DomainModel\\Client\\{1}.cs", this.solutionPath, parmName);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);            
            DotNetOperationParmClassMapping classMapping = new DotNetOperationParmClassMapping(cns, parmName, parm.Parms);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "netoperationparmclass.vm", classMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());
        }
        
        private void GenerateSvcManagerClass(LWIntegrationDirectives config)
        {
            string fname = string.Format("{0}LWIntegrationSvcClientManager.cs", this.solutionPath);
            TextWriter writer = CodeGenUtil.GetWriter(fname, null);
            DotNetSvcManagerClassMapping classMapping = new DotNetSvcManagerClassMapping(config);
            VelocityTemplateUtil.GenerateFromTemplate("Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen.Templates", "netsvcmgrclass.vm", classMapping, writer, System.Reflection.Assembly.GetExecutingAssembly());
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
                catch(Exception ex)
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
