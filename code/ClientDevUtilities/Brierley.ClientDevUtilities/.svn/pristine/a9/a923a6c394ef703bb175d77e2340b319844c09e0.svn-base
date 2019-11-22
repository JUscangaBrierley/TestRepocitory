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
    public class DotNetSvcManagerClassMapping
    {
        private LWIntegrationDirectives _config = null;
        private IList<LWIntegrationDirectives.OperationDirective> _opDirectives = new List<LWIntegrationDirectives.OperationDirective>();

        public DotNetSvcManagerClassMapping(LWIntegrationDirectives config)
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
                            rtType = !p.IsArray ? "bool" : "bool[]";                            
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                            rtType = !p.IsArray ? "decimal" : "decimal[]";                            
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                            rtType = !p.IsArray ? "DateTime" : "DateTime[]";
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                            rtType = !p.IsArray ? "int" : "int[]";
                            break;
                        //case LWIntegrationDirectives.ParmDataTypeEnums.Key:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                            rtType = !p.IsArray ? "long" : "long[]";
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
                            rtType = !p.IsArray ? "string" : "string[]";
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
            //string parmName = string.Empty;
            //switch (op.OperationMetadata.OperationInput.InputType)
            //{
            //    case LWIntegrationDirectives.OperationIOTypeEnums.None:
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Key:
            //        parmName = "key";
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Xml:
            //        parmName = "xml";
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Global:
            //        parmName = "global";                                        
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Member:
            //        parmName = "member";
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Primitive:
            //        if (op.OperationMetadata.OperationInput.InputParms.Count > 1)
            //        {
            //            parmName = "input";
            //        }
            //        else
            //        {
            //            parmName = op.OperationMetadata.OperationInput.InputParms[0].Name;                        
            //        }
            //        break;
            //}
            //doc = string.Format("/// <param name=\"{1}\">{0}</param>", op.OperationMetadata.OperationInput.Description, parmName);                    
            return doc;
        }

        public virtual string GetOperationReturnDoc(LWIntegrationDirectives.OperationDirective op)
        {
            //if (op.OperationMetadata.OperationOutput.ReturnType == LWIntegrationDirectives.OperationIOTypeEnums.None)
            //{
            //    return string.Empty;
            //}
            //else
            //{
            //    return string.Format("/// <returns>{0}</returns>", op.OperationMetadata.OperationOutput.Description);
            //}
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
                            rtType = !p.IsArray ? "bool" : "bool[]";
                            if (!p.IsRequired && !p.IsRequired && !p.IsArray)
                            {
                                rtType += "?";
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                            rtType = !p.IsArray ? "decimal" : "decimal[]";
                            if (!p.IsRequired && !p.IsRequired && !p.IsArray)
                            {
                                rtType += "?";
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                            rtType = !p.IsArray ? "DateTime" : "DateTime[]";
                            if (!p.IsRequired && !p.IsRequired && !p.IsArray)
                            {
                                rtType += "?";
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                            rtType = !p.IsArray ? "int" : "int[]";
                            if (!p.IsRequired && !p.IsRequired && !p.IsArray)
                            {
                                rtType += "?";
                            }
                            break;
                        //case LWIntegrationDirectives.ParmDataTypeEnums.Key:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                            rtType = !p.IsArray ? "long" : "long[]";
                            if (!p.IsRequired && !p.IsRequired && !p.IsArray)
                            {
                                rtType += "?";
                            }
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
                            rtType = !p.IsArray ? "string" : "string[]";
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
            sb.Append("string externalId, out double elapsedTime");
            return sb.ToString();
        }

        public virtual string GetSerializationStatement(LWIntegrationDirectives.OperationDirective op)
        {
            string statement = string.Empty;
            if (op.OperationMetadata.OperationInput != null && op.OperationMetadata.OperationInput.InputParms != null)
            {
                if (op.OperationMetadata.OperationInput.InputParms.Count == 0)
                {
                    statement = string.Format("string payload = string.Empty;");
                }
                if (op.OperationMetadata.OperationInput.InputParms.Count == 1)
                {
                    LWIntegrationDirectives.OperationParmType p = op.OperationMetadata.OperationInput.InputParms[0];
                    switch (p.Type)
                    {
                        case LWIntegrationDirectives.ParmDataTypeEnums.Boolean:
                            statement = string.Format("string payload = _serializer.SerializeMethodPrimitiveParm<bool>(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                            statement = string.Format("string payload = _serializer.SerializeMethodPrimitiveParm<decimal>(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                            statement = string.Format("string payload = _serializer.SerializeMethodPrimitiveParm<DateTime>(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                            statement = string.Format("string payload = _serializer.SerializeMethodPrimitiveParm<int>(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        //case LWIntegrationDirectives.ParmDataTypeEnums.Key:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                            statement = string.Format("string payload = _serializer.SerializeMethodPrimitiveParm<long>(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Member:
                        case LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Global:
                            string env = "payload = string.Format(\"<{0}InParms>{1}</{0}InParms>\", method, payload);";
                            statement = string.Format("string payload = _serializer.SerializeMethodParm(method, \"{0}\", {0});", p.Name.ToLower());
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
                                statement = string.Format("string payload = _serializer.SerializeMethodPrimitiveParm<string>(method, \"{0}\", {1});", p.Name, p.Name.ToLower());
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Struct:
                            statement = string.Format("string payload = string.Empty;");
                            break;
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder(string.Format("{0}In input = new {0}In()", op.Name));
                    sb.Append("{");
                    bool first = true;
                    foreach (LWIntegrationDirectives.OperationParmType p in op.OperationMetadata.OperationInput.InputParms)
                    {
                        if (!first)
                        {
                            sb.Append(",");
                        }
                        first = false;
                        sb.Append(string.Format("{0} = {1}", p.Name, p.Name.ToLower()));
                    }
                    sb.Append("};");
                    sb.AppendLine();
                    sb.Append("\t\t\t");
                    sb.Append(string.Format("string payload = _serializer.SerializeMethodParm(method, \"input\", input);"));
                    statement = sb.ToString();

                }
            }
            else
            {
                statement = string.Format("string payload = string.Empty;");
            }
            return statement;
            //switch (op.OperationMetadata.OperationInput.InputType)
            //{
            //    case LWIntegrationDirectives.OperationIOTypeEnums.None:
            //        statement = "string payload = string.Empty;";
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Key:
            //        statement = "string payload = key;";
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Xml:
            //        statement = "string payload = xml;";
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Global:
            //        statement = "string payload = _serializer.SerializeGlobalAttributeSet(global);";
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Member:
            //        statement = "string payload = _serializer.SerializeMember(member);";
            //        break;
            //    case LWIntegrationDirectives.OperationIOTypeEnums.Primitive:
            //        if (op.OperationMetadata.OperationInput.InputParms.Count > 1)
            //        {
            //            statement = string.Format("string payload = _serializer.SerializeDataObject(input);");                        
            //        }
            //        else
            //        {
            //            statement = string.Format("string payload = _serializer.SerializeDataObject(method, \"{0}\", {0});", op.OperationMetadata.OperationInput.InputParms[0].Name);
            //            //statement = string.Format("string payload = {0}.ToString();", op.OperationMetadata.OperationInput.InputParms[0].Name);                        
            //        }
            //        break;
            //}
            //return statement;
        }

        public virtual string GetEnvelopeStatement(LWIntegrationDirectives.OperationDirective op)
        {
            if (op.GetType() == typeof(LWIntegrationDirectives.APIOperationDirective))
            {
                return "payload = string.Format(\"<{0}InParms>{1}</{0}InParms>\", method, payload);";
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
                    statement = string.Format("return ({0}Out)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);", op.Name);
                }
                else
                {
                    LWIntegrationDirectives.OperationParmType p = op.OperationMetadata.OperationOutput.OutputParms[0];
                    switch (p.Type)
                    {
                        case LWIntegrationDirectives.ParmDataTypeEnums.Boolean:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<bool>(method, response.ResponseDetail);");
                            }
                            else
                            {
                                statement = string.Format("return (bool)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<decimal>(method, response.ResponseDetail);");
                            }
                            else
                            {
                                statement = string.Format("return (decimal)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<DateTime>(method, response.ResponseDetail);");
                            }
                            else
                            {
                                statement = string.Format("return (DateTime)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<int>(method, response.ResponseDetail);");
                            }
                            else
                            {
                                statement = string.Format("return (int)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);");
                            }
                            break;
                        //case LWIntegrationDirectives.ParmDataTypeEnums.Key:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<long>(method, response.ResponseDetail);");
                            }
                            else
                            {
                                statement = string.Format("return (long)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Member:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<Member>(method, response.ResponseDetail);");
                            }
                            else
                            {
                                statement = string.Format("return (Member)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Global:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<LWClientDataObject>(method, response.ResponseDetail);");
                            }
                            else
                            {
                                statement = string.Format("return (LWClientDataObject)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<LWAttributeSetContainer>(method, response.ResponseDetail);");
                            }
                            else
                            {
                                statement = string.Format("return (LWAttributeSetContainer)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);");
                            }
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.String:
                        case LWIntegrationDirectives.ParmDataTypeEnums.Xml:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<string>(method, response.ResponseDetail);");
                            }
                            else
                            {
                                statement = string.Format("return (string)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);");
                            }                            
                            break;
                        case LWIntegrationDirectives.ParmDataTypeEnums.Struct:
                            if (p.IsArray)
                            {
                                statement = string.Format("return _serializer.DeserializeResponseObjectArray<{0}Struct>(method, response.ResponseDetail);", p.Name);                                
                            }
                            else
                            {
                                statement = string.Format("return ({0}Struct)_serializer.DeserializeSingleResponseObject(method, response.ResponseDetail);", p.Name);
                            }
                            break;
                    }
                }
            }            
            return statement;
        }        
    }
}
