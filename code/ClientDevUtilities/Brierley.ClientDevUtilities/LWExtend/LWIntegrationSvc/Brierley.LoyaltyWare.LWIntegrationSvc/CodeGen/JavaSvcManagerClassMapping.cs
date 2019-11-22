//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWIntegrationSvc;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen
{
    public class JavaSvcManagerClassMapping
    {
        private LWIntegrationDirectives _config = null;
        private IList<LWIntegrationDirectives.OperationDirective> _opDirectives = new List<LWIntegrationDirectives.OperationDirective>();

        public JavaSvcManagerClassMapping(LWIntegrationDirectives config)
        {
            _config = config;
            foreach (KeyValuePair<string, LWIntegrationDirectives.OperationDirective> pair in _config.OperationDirectives)
            {
                _opDirectives.Add(pair.Value);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            }
        }

        public virtual IList<LWIntegrationDirectives.OperationDirective> Operations
        {
            get
            {
                IList<LWIntegrationDirectives.OperationDirective> opList = new List<LWIntegrationDirectives.OperationDirective>();
                foreach (LWIntegrationDirectives.OperationDirective op in _opDirectives)
                {
                    //if (!op.OperationMetadata.StandardAPI)
                    //{
                    opList.Add(op);
                    //}
                }
                return opList;
            }
        }

        public virtual string GetOperationName(LWIntegrationDirectives.OperationDirective op)
        {
            return op.Name.Substring(0, 1).ToLower() + op.Name.Substring(1);
        }

        public virtual string GetOperationReturnType(LWIntegrationDirectives.OperationDirective op)
        {
            string rtType = string.Empty;
            if (op.OperationMetadata.OperationOutput != null && op.OperationMetadata.OperationOutput.OutputParms != null && op.OperationMetadata.OperationOutput.OutputParms.Count > 0)
            {
                if (op.OperationMetadata.OperationOutput.OutputParms.Count > 1)
                {
                    rtType = op.Name + "Out";
                }
                else
                {
                    // there is only one return parameter
                    LWIntegrationDirectives.OperationParmType p = op.OperationMetadata.OperationOutput.OutputParms[0];
                    switch (p.Type)
                    {
                        case LWIntegrationDirectives.ParmDataTypeEnums.Boolean:
                            rtType = !p.IsArray ? "Boolean" : "Boolean[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                            rtType = !p.IsArray ? "java.math.BigDecimal" : "java.math.BigDecimal[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                            rtType = !p.IsArray ? "Date" : "Date[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                            rtType = !p.IsArray ? "Integer" : "Integer[]";
                            break;
                        //case LWIntegrationDirectives.ParmDataTypeEnums.Key:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                            rtType = !p.IsArray ? "Long" : "Long[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Member:
                            rtType = !p.IsArray ? "Member" : "Member[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Global:
                            rtType = !p.IsArray ? "LWClientDataObject" : "LWClientDataObject[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet:
                            rtType = !p.IsArray ? "LWAttributeSetContainer" : "LWAttributeSetContainer[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.String:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Xml:
                            rtType = !p.IsArray ? "String" : "String[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Struct:
                            rtType = !p.IsArray ? p.Name + "Struct" : string.Format("{0}Struct[]", p.Name);
                            break;
                    }
                }
            }
            else
            {
                rtType = "void";
            }

            return rtType;
        }

        public virtual string GetOperationSummary(LWIntegrationDirectives.OperationDirective op)
        {
            return (op != null ? op.OperationMetadata.Summary : string.Empty);
        }

        public virtual string GetOperationParamDoc(LWIntegrationDirectives.OperationDirective op)
        {
            string doc = string.Empty;
            return doc;
        }

        public virtual string GetOperationReturnDoc(LWIntegrationDirectives.OperationDirective op)
        {
            return string.Empty;
        }

        public virtual string GetOperationParms(LWIntegrationDirectives.OperationDirective op)
        {
            StringBuilder sb = new StringBuilder();
            if (op.OperationMetadata.OperationInput != null && op.OperationMetadata.OperationInput.InputParms != null)
            {
                foreach (LWIntegrationDirectives.OperationParmType p in op.OperationMetadata.OperationInput.InputParms)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    string rtType = string.Empty;
                    switch (p.Type)
                    {
                        case LWIntegrationDirectives.ParmDataTypeEnums.Boolean:
                            rtType = !p.IsArray ? "Boolean" : "Boolean[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                            rtType = !p.IsArray ? "java.math.BigDecimal" : "java.math.BigDecimal[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                            rtType = !p.IsArray ? "Date" : "Date[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                            rtType = !p.IsArray ? "Integer" : "Integer[]";
                            break;
                        //case LWIntegrationDirectives.ParmDataTypeEnums.Key:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                            rtType = !p.IsArray ? "Long" : "Long[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Member:
                            rtType = !p.IsArray ? "Member" : "Member[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Global:
                        case LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet:
                            rtType = !p.IsArray ? "LWClientDataObject" : "LWClientDataObject[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.String:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Xml:
                            rtType = !p.IsArray ? "String" : "String[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Struct:
                            rtType = !p.IsArray ? p.Name + "Struct" : string.Format("{0}Struct[]", p.Name);
                            break;
                    }
                    sb.Append(rtType + " " + p.Name.ToLower());
                }
            }
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }
            sb.Append("LWCDISExtraArgs extraArgs");
            return sb.ToString();
        }

        public virtual string GetSerializationStatement(LWIntegrationDirectives.OperationDirective op)
        {
            string statement = string.Empty;
            if (op.OperationMetadata.OperationInput != null && op.OperationMetadata.OperationInput.InputParms != null)
            {
                if (op.OperationMetadata.OperationInput.InputParms.Count == 0)
                {
                    statement = string.Format("String payload = \"\";");
                }
                if (op.OperationMetadata.OperationInput.InputParms.Count == 1)
                {
                    LWIntegrationDirectives.OperationParmType p = op.OperationMetadata.OperationInput.InputParms[0];
                    switch (p.Type)
                    {
                        case LWIntegrationDirectives.ParmDataTypeEnums.Boolean:
                            statement = string.Format("String payload = _serializer.serializeMethodPrimitiveParm(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                            statement = string.Format("String payload = _serializer.serializeMethodPrimitiveParm(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                            statement = string.Format("String payload = _serializer.serializeMethodPrimitiveParm(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                            statement = string.Format("String payload = _serializer.serializeMethodPrimitiveParm(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        //case LWIntegrationDirectives.ParmDataTypeEnums.Key:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                            statement = string.Format("String payload = _serializer.serializeMethodPrimitiveParm(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Member:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Global:
                        case LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet:
                            string env = "payload = String.format(\"<%sInParms>%s</%sInParms>\", method, payload, method);";
                            statement = string.Format("String payload = _serializer.serializeMethodParm(method, \"{0}\", {0});", p.Name.ToLower());
                            StringBuilder buf = new StringBuilder(statement);
                            buf.AppendLine();
                            buf.Append("\t\t\t");
                            buf.Append(env);
                            statement = buf.ToString();
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.String:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Xml:
                            if (p.IsArray)
                            {
                            }
                            else
                            {
                                statement = string.Format("String payload = _serializer.serializeMethodPrimitiveParm(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Struct:
                            statement = string.Format("String payload = string.Empty;");
                            break;
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder(string.Format("{0}In input = new {0}In();", op.Name));
                    sb.AppendLine();
                    sb.Append("\t\t\t");
                    foreach (LWIntegrationDirectives.OperationParmType p in op.OperationMetadata.OperationInput.InputParms)
                    {
                        sb.Append(string.Format("input.set{0}({1});", p.Name, p.Name.ToLower()));
                        sb.AppendLine();
                        sb.Append("\t\t\t");
                    }
                    sb.Append(string.Format("String payload = _serializer.serializeMethodParm(method, \"input\", input);"));
                    statement = sb.ToString();
                }
            }
            else
            {
                statement = string.Format("String payload = \"\";");
            }
            return statement;
        }

        public virtual string GetEnvelopeStatement(LWIntegrationDirectives.OperationDirective op)
        {
            if (op.GetType() == typeof(LWIntegrationDirectives.APIOperationDirective))
            {
                return "payload = String.format(\"<%sInParms>%s</%sInParms>\", method, payload, method);";
            }
            else
            {
                return string.Empty;
            }
        }

        public virtual string GetReturnStatement(LWIntegrationDirectives.OperationDirective op)
        {
            string statement = string.Empty;
            if (op.OperationMetadata.OperationOutput == null ||
                op.OperationMetadata.OperationOutput.OutputParms == null || op.OperationMetadata.OperationOutput.OutputParms.Count == 0)
            {
                return "return;";
            }
            else
            {
                if (op.OperationMetadata.OperationOutput.OutputParms.Count > 1)
                {
                    statement = string.Format("return ({0}Out)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());", op.Name);
                }
                else
                {
                    LWIntegrationDirectives.OperationParmType p = op.OperationMetadata.OperationOutput.OutputParms[0];
                    switch (p.Type)
                    {
                        case LWIntegrationDirectives.ParmDataTypeEnums.Boolean:
                            if (p.IsArray)
                            {
                                statement = string.Format("return (Boolean[])_serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            else
                            {
                                statement = string.Format("return (Boolean)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                            if (p.IsArray)
                            {
                                statement = string.Format("return (java.math.BigDecimal[])_serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            else
                            {
                                statement = string.Format("return (java.math.BigDecimal)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                            if (p.IsArray)
                            {
                                statement = string.Format("return (Date[])_serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            else
                            {
                                statement = string.Format("return (Date)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                            if (p.IsArray)
                            {
                                statement = string.Format("return(Integer[]) _serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            else
                            {
                                statement = string.Format("return (Integer)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            break;
                        //case LWIntegrationDirectives.ParmDataTypeEnums.Key:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                            if (p.IsArray)
                            {
                                statement = string.Format("return (Long[])_serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            else
                            {
                                statement = string.Format("return (Long)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Member:
                            if (p.IsArray)
                            {
                                statement = string.Format("return (Member[])_serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            else
                            {
                                statement = string.Format("return (Member)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Global:
                            if (p.IsArray)
                            {
                                statement = string.Format("return (LWClientDataObject[])_serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            else
                            {
                                statement = string.Format("return (LWClientDataObject)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet:
                            if (p.IsArray)
                            {
                                statement = string.Format("return (LWAttributeSetContainer[])_serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            else
                            {
                                statement = string.Format("return (LWAttributeSetContainer)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.String:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Xml:
                            if (p.IsArray)
                            {
                                statement = string.Format("return (String[])_serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            else
                            {
                                statement = string.Format("return (String)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Struct:
                            if (p.IsArray)
                            {
                                statement = string.Format("return ({0}Struct[])_serializer.deserializeResponseObject(method, response.getResponseDetail().getValue());", p.Name);
                            }
                            else
                            {
                                statement = string.Format("return ({0}Struct)_serializer.deserializeSingleResponseObject(method, response.getResponseDetail().getValue());", p.Name);
                            }
                            break;
                    }
                }
            }
            return statement;
        }
    }
}
