using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

using Commons.Collections;
using NVelocity.App;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.CodeGen
{
    public class VelocityTemplateUtil
    {
        #region Fields
        private const string _className = "VelocityTemplateUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        #endregion

        #region Private Helpers
        private static string GetTemplate(string prefixNs, string templateName, Assembly assembly)
        {            
            string methodName = "GetTemplate";

            StreamReader reader = null;
            string templateSrc = string.Empty;
            try
            {
                if (assembly == null)
                {
                    assembly = System.Reflection.Assembly.Load(DataServiceUtil.FrameworkAssembly);
                }
                if (assembly == null)
                {
                    throw new FileNotFoundException("Unable to load assembly [" + "Brierley.FrameWork" + "]");
                }
                //string configResourceName = String.Format("Brierley.FrameWork.CodeGen.ClientDataModel.Templates.{0}", templateName);
                string configResourceName = String.Format("{0}.{1}", prefixNs, templateName);
                Stream stream = assembly.GetManifestResourceStream(configResourceName);
                reader = new StreamReader(stream);
                templateSrc = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error getting template " + templateName, ex);
                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return templateSrc;
        }
        #endregion

        #region Public Methods
        public static void GenerateFromTemplate(string prefixNs, string templatename, object mapping, TextWriter writer,Assembly assembly)
        {
            string methodName = "GenerateFromTemplate";

            string path = string.Empty;
            try
            {
                string templateSrc = GetTemplate(prefixNs, templatename, assembly);
                path = Path.GetTempFileName();
                _logger.Debug(_className, methodName, "Temporary path for template is " + path);
                TextWriter tempFile = null;
                try
                {
                    tempFile = new StreamWriter(path);
                    tempFile.Write(templateSrc);
                }
                finally
                {
                    tempFile.Close();
                }
                // initialize the engine
                ExtendedProperties p = new ExtendedProperties();
                VelocityEngine ve = new VelocityEngine();
                p.SetProperty("resource.loader", "file");
                p.AddProperty("file.resource.loader.class", "NVelocity.Runtime.Resource.Loader.FileResourceLoader, NVelocity");
                p.AddProperty("file.resource.loader.path", Path.GetDirectoryName(path));                
                ve = new VelocityEngine();
                ve.Init(p);
                NVelocity.Template template = ve.GetTemplate(Path.GetFileName(path));                

                NVelocity.VelocityContext vc = new NVelocity.VelocityContext();
                vc.Put("clazz", mapping);
                StringWriter sw = new StringWriter();
                template.Merge(vc, sw);
                //Console.Out.WriteLine(sw.ToString());
                ve.Evaluate(vc, writer, "xxx", sw.ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error generating code from template " + templatename, ex);
                throw;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (!string.IsNullOrEmpty(path))
                {
                    File.Delete(path);
                }
            }
        }
        #endregion
    }
}
