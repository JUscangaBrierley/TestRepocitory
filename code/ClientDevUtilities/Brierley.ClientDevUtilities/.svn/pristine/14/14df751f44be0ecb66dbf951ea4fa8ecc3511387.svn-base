//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

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
	public class XsdGenerator
	{
		private const string _className = "DotNetIntegrationClientLibGenerator";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
		private string _outputPath = null;

		public XsdGenerator(string outputPath)
		{
			if (string.IsNullOrEmpty(outputPath))
			{
				throw new ArgumentNullException("outputPath");
			}
			_outputPath = outputPath;
		}

		public void Generate(string configFile)
		{
			try
			{
				string orgName = LWConfigurationUtil.GetCurrentEnvironmentContext().Organization;

				// initialize the configuration file
				LWIntegrationDirectives config = new LWIntegrationDirectives();
				config.Load(configFile);

				//generate client schema - member and attribute sets
				XmlSchemaGenerator xg = new XmlSchemaGenerator(LWConfigurationUtil.GetCurrentEnvironmentContext().Organization, _outputPath, true);
				xg.Generate();
				xg.Dispose();

				using (var fs = new System.IO.StreamWriter(string.Format("{0}StandardApiPayload.xsd", _outputPath), false))
				{
					fs.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
					fs.WriteLine("<xs:schema id=\"StandardApiPayload\"");
					fs.WriteLine("    targetNamespace=\"http://www.brierley.com/StandardApiPayload\"");
					fs.WriteLine("    elementFormDefault=\"qualified\"");
					fs.WriteLine("    xmlns=\"http://www.brierley.com/StandardApiPayload\"");
					fs.WriteLine("    xmlns:tns=\"http://www.brierley.com/StandardApiPayload\"");
					fs.WriteLine("    xmlns:dm=\"http://{0}.MemberProcessing.Schemas.{0}\"", orgName);
					fs.WriteLine("    xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">");
						
					fs.WriteLine();
					fs.WriteLine("<xs:import schemaLocation=\"{0}.xsd\" namespace=\"http://{0}.MemberProcessing.Schemas.{0}\" />", orgName);
						
					fs.WriteLine();
					fs.WriteLine("  <xs:simpleType name=\"SAPIDataTypeEnums\">");
					fs.WriteLine("    <xs:restriction base=\"xs:string\">");
					
					//SApiDataTypeEnums
					List<string> addedTypes = new List<string>();
					Action<string> addType = delegate(string type)
					{
						if (!addedTypes.Contains(type))
						{
							fs.WriteLine("      <xs:enumeration value=\"{0}\"/>", type);
							addedTypes.Add(type);
						}
					};
					
					foreach(var e in Enum.GetValues(typeof(LWIntegrationDirectives.ParmDataTypeEnums)))
					{
						addType(e.ToString());
					}
					foreach (LWIntegrationDirectives.OperationDirective op in config.OperationDirectives.Values)
					{
						//input
						var input = op.OperationMetadata.OperationInput;
						if (input != null && input.InputParms != null && input.InputParms.Count > 1)
						{
							addType(op.Name + "In");
						}
						//output
						var output = op.OperationMetadata.OperationOutput;
						if (output != null && output.OutputParms != null && output.outputParms.Count > 1)
						{
							addType(op.Name + "Out");
						}
					}
					fs.WriteLine("    </xs:restriction>");
					fs.WriteLine("  </xs:simpleType>");


					//Parm Definition
					fs.WriteLine("  <xs:element name=\"Parm\">");
					fs.WriteLine("    <xs:complexType>");
					fs.WriteLine("      <xs:sequence>");
					fs.WriteLine("        <xs:element name=\"Value\" type=\"xs:string\" minOccurs=\"0\" maxOccurs=\"unbounded\"/>");
                    fs.WriteLine("        <xs:element ref=\"Parm\" minOccurs=\"0\" maxOccurs=\"unbounded\"/>");
					fs.WriteLine("        <xs:element ref=\"dm:AttributeSets\" minOccurs=\"0\" maxOccurs=\"unbounded\"/>");
					fs.WriteLine("      </xs:sequence>");
					fs.WriteLine("      <xs:attribute name=\"Name\" type=\"xs:string\"/>");
					fs.WriteLine("      <xs:attribute name=\"Type\" type=\"SAPIDataTypeEnums\"/>");
					fs.WriteLine("      <xs:attribute name=\"IsRequired\" type=\"xs:boolean\"/>");
					fs.WriteLine("      <xs:attribute name=\"IsArray\" type=\"xs:boolean\"/>");
					fs.WriteLine("    </xs:complexType>");
					fs.WriteLine("  </xs:element>");

					//operation definitions
					foreach (LWIntegrationDirectives.OperationDirective op in config.OperationDirectives.Values)
					{
						fs.WriteLine();
						fs.WriteLine("<!-- {0} -->", op.Name);
						
						//input
						if (op.OperationMetadata.OperationInput != null && op.OperationMetadata.OperationInput.InputParms != null)
						{
							var inputParms = op.OperationMetadata.OperationInput.InputParms;
							fs.WriteLine("  <xs:element name=\"{0}InParms\">", op.Name);
							fs.WriteLine("    <xs:complexType>");
							fs.WriteLine("      <xs:sequence>");
							fs.WriteLine("        <xs:element ref=\"Parm\" minOccurs=\"1\" maxOccurs=\"{0}\" />", inputParms.Count > 1 ? "unbounded" : "1");
							fs.WriteLine("      </xs:sequence>");
							fs.WriteLine("    </xs:complexType>");
							fs.WriteLine("  </xs:element>");
						}

						//output
						if (op.OperationMetadata.OperationOutput != null && op.OperationMetadata.OperationOutput.OutputParms != null)
						{
                            //var outputParms = op.OperationMetadata.OperationOutput.outputParms;
							fs.WriteLine();
							fs.WriteLine("  <xs:element name=\"{0}OutParms\">", op.Name);
							fs.WriteLine("    <xs:complexType>");
							fs.WriteLine("      <xs:sequence>");
							fs.WriteLine("        <xs:element ref=\"Parm\"/>");
							fs.WriteLine("      </xs:sequence>");
							fs.WriteLine("    </xs:complexType>");
							fs.WriteLine("  </xs:element>");
						}
					}

					fs.WriteLine("</xs:schema>");
					fs.Close();
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "Generate", "Error generating XSD.", ex);
				throw;
			}
		}
	}
}
