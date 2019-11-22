using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.LWIntegration;

namespace Brierley.FrameWork.LWIntegration.Util
{
	public class DynamicCodeUtil
	{
		#region Fields
		private const string _className = "DynamicCodeUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		#endregion

		#region Dynamic Code Invocation

		public static object CompileDynamicCode(IList<LWIntegrationConfig.DynamicScript> scriptsList)
		{
			string methodName = "CompileDynamicCode";

			_logger.Trace(_className, methodName, "Compiling dynamic scripts.");

			int lineCount = 0; //number of lines before user code begins
			DateTime start = DateTime.Now;
			object compiledCode = null;

			CompilerParameters Parameters = new CompilerParameters();
			Parameters.ReferencedAssemblies.Add("System.dll");
			Parameters.ReferencedAssemblies.Add("System.Core.dll");
			Parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
			Parameters.ReferencedAssemblies.Add("System.XML.dll");
			Parameters.ReferencedAssemblies.Add("System.XML.Linq.dll");
            Parameters.ReferencedAssemblies.Add((typeof(DynamicCodeUtil)).Assembly.Location);

			StringBuilder sb = new StringBuilder();
			sb.Append("using System;\n");
			sb.Append("using System.IO;\n");
			sb.Append("using System.Text;\n");
			sb.Append("using System.Collections.Generic;\n");
			sb.Append("using System.Linq;\n");
			sb.Append("namespace codetest\n{\npublic class test{");
			foreach (LWIntegrationConfig.DynamicScript spec in scriptsList)
			{
				_logger.Trace(_className, methodName, "generating code for dynamic script " + spec.Name);

				sb.Append(string.Format("\npublic string {0}(", spec.Name));
				if (!string.IsNullOrEmpty(spec.InParmName))
				{
					sb.Append("string " + spec.InParmName);
				}
				sb.Append(")\n{\n" + spec.Code + "\n}");
			}
			sb.Append("}}");
			string userCode = sb.ToString();

			Parameters.GenerateInMemory = false;
			CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });

			CompilerResults Compiled = provider.CompileAssemblyFromSource(Parameters, userCode);

			if (Compiled.Errors.HasErrors)
			{
				string errorMessage = string.Empty;
				errorMessage = Compiled.Errors.Count.ToString() + " Errors:";
				for (int x = 0; x < Compiled.Errors.Count; x++)
				{
					errorMessage += "\r\nLine: " + (Compiled.Errors[x].Line - lineCount).ToString() + " - " + Compiled.Errors[x].ErrorText;
				}
				_logger.Error(_className, methodName, errorMessage);
				throw new Brierley.FrameWork.Common.Exceptions.LWException(errorMessage);
			}
			else
			{
				compiledCode = Compiled.CompiledAssembly.CreateInstance("codetest.test");
			}


			return compiledCode;
		}

		public static string ExecuteDynamicCode(object compiledCode, LWIntegrationConfig.DynamicScript script, string inParm)
		{
			string result = string.Empty;
			try
			{
				if (!string.IsNullOrEmpty(script.InParmName))
				{
					string[] parms = new string[1];
					parms[0] = inParm;
					result = (string)ClassLoaderUtil.InvokeMethod(compiledCode, script.Name, parms);
				}
				else
				{
					result = (string)ClassLoaderUtil.InvokeMethod(compiledCode, script.Name, null);
				}
			}
			catch (Exception ex)
			{
				string errMsg = string.Format("Error executing script {0}", script.Name);
				_logger.Error(_className, "ExecuteDynamicCode", errMsg, ex);
				throw;
			}
			return result;
		}

		#endregion
	}
}
