using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling
{
    public class SerializationUtils
    {
        #region Fields
        private const string _className = "SerializationUtils";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		private static readonly XNamespace NS_SAPI = "http://www.brierley.com/StandardApiPayload";
		private static readonly XNamespace NS_ANON = ""; 
        #endregion

        #region Private Methods
        //private static IAttributeSetContainer LoadAttributeSet(IAttributeSetContainer thisContainer, string attributeSetToLoad)
        //{
        //    ILWDataService ds = LWDataServiceUtil.DataServiceInstance(true);

        //    string[] attributeSets = attributeSetToLoad.Split('/');
        //    //int nTokens = attributeSets.Length;
        //    string attributeSetName = attributeSets[0];
        //    if (attributeSetName == "VirtualCard")
        //    {
        //        throw new LWIntegrationException(
        //            string.Format("Invalid LoadAttributeSet directive {0} found.", attributeSetToLoad)) { ErrorCode = 3000 };
        //    }
        //    AttributeSetMetaData asDef = ds.GetAttributeSetMetaData(attributeSetName);
        //    if (!thisContainer.IsLoaded(attributeSetName))
        //    {
        //        ds.LoadAttributeSetList(thisContainer, attributeSetName, false);
        //    }
        //    IList<IClientDataObject> aSet = thisContainer.GetChildAttributeSets(attributeSetName);
        //    foreach (IClientDataObject row in aSet)
        //    {
        //        // now call recursively to process the next
        //        if (attributeSets.Length > 1)
        //        {
        //            string attLoadStr = attributeSetToLoad.Substring(attributeSetToLoad.IndexOf("/") + 1);
        //            LoadAttributeSet(row, attLoadStr);
        //        }
        //    }
        //    return thisContainer;
        //}

        //private static Member LoadMemberAttributeSets(Member member, IList<string> attributeSetsToLoad, int index)
        //{
        //    bool virtualCardPresent = false;
        //    foreach (string attributeSetToLoad in attributeSetsToLoad)
        //    {
        //        if (!attributeSetToLoad.StartsWith("VirtualCard"))
        //        {
        //            member = (Member)LoadAttributeSet(member, attributeSetToLoad);
        //        }
        //        else
        //        {
        //            virtualCardPresent = true;
        //        }
        //    }
        //    // now process virtual cards
        //    if (virtualCardPresent)
        //    {
        //        foreach (string attributeSetToLoad in attributeSetsToLoad)
        //        {
        //            if (attributeSetToLoad.StartsWith("VirtualCard"))
        //            {
        //                foreach (VirtualCard vc in member.LoyaltyCards)
        //                {
        //                    // strip off the VirtualCard part.
        //                    int idx = attributeSetToLoad.IndexOf("VirtualCard/");
        //                    if (idx == 0)
        //                    {
        //                        string astl = attributeSetToLoad.Substring(("VirtualCard/").Length);
        //                        LoadAttributeSet(vc, astl);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return member;
        //}

        private static XElement SerializeMemberToXml(LWIntegrationDirectives config, XElement rootNode, Member member)
        {
            XElement memberNode = new XElement(NS_ANON + "Member");
            rootNode.Add(memberNode);

            memberNode = LWIntegrationUtilities.ProcessMemberAttributesToXml(config, member, memberNode, config.GetDateConversionFormat());

            Dictionary<string, List<IClientDataObject>> children = member.GetChildAttributeSets();
            if (children != null && children.Count > 0)
            {
                foreach (string chName in children.Keys)
                {
                    IList<IClientDataObject> chSet = member.GetChildAttributeSets(chName);
                    foreach (IClientDataObject ch in chSet)
                    {
                        memberNode = LWIntegrationUtilities.SerializeAttributeSetToXml(config, memberNode, ch, config.GetDateConversionFormat());
                    }
                }
            }
            foreach (VirtualCard vc in member.LoyaltyCards)
            {
                XElement vcNode = new XElement(NS_ANON + "VirtualCard");
                memberNode.Add(vcNode);
                // process this virtual card attributes
                vcNode = LWIntegrationUtilities.ProcessLoyaltyCardAttributesToXml(config, vc, vcNode, config.GetDateConversionFormat());
                // now process the attribute sets of the virtual card
                Dictionary<string, List<IClientDataObject>> vcChildren = vc.GetChildAttributeSets();
                if (vcChildren != null && vcChildren.Count > 0)
                {
                    foreach (string vcChName in vcChildren.Keys)
                    {
                        IList<IClientDataObject> vcChSet = vc.GetChildAttributeSets(vcChName);
                        foreach (IClientDataObject ch in vcChSet)
                        {
                            // LW-743
                            //vcNode = LWIntegrationUtilities.SerializeAttributeSetToXml(config, vcNode, ch, config.GetDateConversionFormat());
                            XElement cNode = LWIntegrationUtilities.SerializeAttributeSetToXml(config, vcNode, ch, config.GetDateConversionFormat());
                        }
                    }
                }
            }
            return rootNode;
        }
        #endregion

        #region serialization
                
        public static string SerializeResult(string opName, LWIntegrationDirectives config, Member member)
        {
            APIArguments responseArgs = new APIArguments();
            responseArgs.Add("member", member);
            return SerializationUtils.SerializeResult(opName, config, responseArgs);
        }
        
        public static String SerializeResult(string opName, LWIntegrationDirectives config, APIArguments resultParams)
        {
            try
            {
                // Encoding.Default.WebName will normally be "windows-1252" unless the server has a non-english windows installed.
                //XDocument doc = new XDocument(new XDeclaration("1.0", Encoding.Default.WebName, "yes"));
                XDocument doc = new XDocument();
				XElement envelope = new XElement(NS_SAPI + opName + "OutParms");
                doc.Add(envelope);
                LWIntegrationDirectives.OperationDirective opDirective = config.GetOperationDirective(opName);
				if (opDirective == null)
					throw new LWIntegrationException("Invalid operation directive: " + opName);

                if (opDirective.OperationMetadata.OperationOutput != null && opDirective.OperationMetadata.OperationOutput.OutputParms != null)
                {
                    if (opDirective.OperationMetadata.OperationOutput.OutputParms.Count > 1)
                    {
                        XElement outParm = new XElement(NS_SAPI + "Parm");
                        outParm.Add(new XAttribute("Name", opName + "Out"));
                        outParm.Add(new XAttribute("Type", opName + "Out"));
                        outParm.Add(new XAttribute("IsArray", "false"));
                        envelope.Add(outParm);
                        foreach (LWIntegrationDirectives.OperationParmType parm in config.GetOperationDirective(opName).OperationMetadata.OperationOutput.OutputParms)
                        {
                            XElement node = AddParm(config, opDirective, parm, resultParams);
                            if (node != null)
                            {
                                outParm.Add(node);
                            }
                        }
                    }
                    else
                    {
                        LWIntegrationDirectives.OperationParmType parm = opDirective.OperationMetadata.OperationOutput.OutputParms[0];
                        XElement node = AddParm(config, opDirective, parm, resultParams);
                        if (node != null)
                        {
                            envelope.Add(node);
                        }
                    }
                }
                StringBuilder sb = new StringBuilder(string.Format("<?xml version=\"1.0\" encoding=\"{0}\"?>", Encoding.UTF8.WebName));
                sb.Append(doc.ToString());
                return sb.ToString();                
            }
            catch (LWIntegrationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWResponseMarshallingException("Error marshalling response.", ex) { ErrorCode = 3110 };
            }
        }

        private static XElement AddParm(LWIntegrationDirectives config, LWIntegrationDirectives.OperationDirective opDirective, LWIntegrationDirectives.OperationParmType parm, APIArguments resultParams)
        {
            XElement node = new XElement(NS_SAPI + "Parm");
            node.Add(new XAttribute("Name", parm.Name));
            node.Add(new XAttribute("Type", parm.Type));
            node.Add(new XAttribute("IsRequired", parm.IsRequired));
            if (parm.IsArray)
            {
                node.Add(new XAttribute("IsArray", true));
            }

            try
            {
                switch (parm.Type)
                {
                    case LWIntegrationDirectives.ParmDataTypeEnums.Struct:
                        if (parm.IsArray)
                        {
                            APIStruct[] parmStructs = resultParams[parm.Name] as APIStruct[];
                            if (parmStructs == null && parm.IsRequired)
                            {
                                string errMsg = string.Format("Required parameter {0} is missing from response of {1}.",
                                    parm.Name, opDirective.Name);
                                throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3109 };
                            }
							if (parmStructs != null)
							{
								foreach (APIStruct parmStruct in parmStructs)
								{
									XElement valNode = new XElement(NS_SAPI + "Parm");
									valNode.Add(new XAttribute("Name", parm.Name));
									valNode.Add(new XAttribute("Type", parm.Type));
									valNode.Add(new XAttribute("IsRequired", parmStruct.IsRequired));
									valNode.Add(new XAttribute("IsArray", false));

									node.Attribute("IsRequired").SetValue(parmStruct.IsRequired);
									APIArguments subparms = parmStruct.Parms;
									foreach (LWIntegrationDirectives.OperationParmType subparm in parm.Parms)
									{
										XElement subnode = AddParm(config, opDirective, subparm, subparms);
										if (subnode != null)
										{
											valNode.Add(subnode);
										}
									}
									node.Add(valNode);
								}
							}
                        }
                        else
                        {
                            APIStruct parmStruct = resultParams[parm.Name] as APIStruct;
                            if (parmStruct == null && parm.IsRequired)
                            {
                                string errMsg = string.Format("Required parameter {0} is missing from response of {1}.",
                                    parm.Name, opDirective.Name);
                                throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3109 };
                            }
                            node.Attribute("IsRequired").SetValue(parmStruct.IsRequired);
                            APIArguments subparms = parmStruct.Parms;
                            foreach (LWIntegrationDirectives.OperationParmType subparm in parm.Parms)
                            {
                                XElement subnode = AddParm(config, opDirective, subparm, subparms);
                                if (subnode != null)
                                {
                                    node.Add(subnode);
                                }
                            }
                        }
                        break;
                    case LWIntegrationDirectives.ParmDataTypeEnums.Member:
                        Brierley.FrameWork.Common.Config.LWConfigurationContext ctx = Brierley.FrameWork.Common.Config.LWConfigurationUtil.GetCurrentEnvironmentContext();
                        XNamespace clientNamespace = string.Format("http://{0}.MemberProcessing.Schemas.{0}", ctx.Organization);
                        XElement root = new XElement(clientNamespace + "AttributeSets");
                        node.Add(root);
                        if (!parm.IsArray)
                        {
                            Member member = (Member)resultParams[parm.Name];                            
                            member = MemberResponseHelperUtil.LoadMemberAttributeSets(opDirective, member);
                            root = SerializeMemberToXml(config, root, member);                            
                        }
                        else
                        {
                            foreach (Member member in (IList<Member>)resultParams[parm.Name])
                            {
                                if (opDirective.ResponseDirective != null)
                                {
                                    Member m = null;
                                    IList<string> attributeSetsToLoad = opDirective.ResponseDirective.AttributeSetsToLoad;
                                    if (attributeSetsToLoad != null && attributeSetsToLoad.Count > 0)
                                    {
                                        // load the requested attribute sets                        
                                        m = MemberResponseHelperUtil.LoadMemberAttributeSets(opDirective, member);
                                    }
                                    root = SerializeMemberToXml(config, root, m);
                                }
                            }
                        }
                        break;
                    case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                        if (parm.IsArray)
                        {
                            object[] vals = resultParams[parm.Name] as object[];
							if (vals != null)
							{
								foreach (object val in vals)
								{
									string tmp = config.HasISO8601CompliantDateStringFormat
										? DateTimeUtil.ConvertDateToISO8601String((DateTime)val, config.IgnoreTimeZoneOffset)
										: DateTimeUtil.ConvertDateToString(config.GetDateConversionFormat(), (DateTime)val);
									node.Add(new XElement(NS_SAPI + "Value", tmp));
								}
							}
                        }
                        else
                        {
							string tmp = config.HasISO8601CompliantDateStringFormat
									? DateTimeUtil.ConvertDateToISO8601String((DateTime)resultParams[parm.Name], config.IgnoreTimeZoneOffset)
                                    : DateTimeUtil.ConvertDateToString(config.GetDateConversionFormat(), (DateTime)resultParams[parm.Name]);
							node.Add(new XElement(NS_SAPI + "Value", tmp));
                        }
                        break;
                    case LWIntegrationDirectives.ParmDataTypeEnums.Global:
                        if (parm.IsArray)
                        {
                            string clientAssembly = DataServiceUtil.ClientsDataBaseName;
                            Assembly assembly = Assembly.Load(clientAssembly);
                            //Type refType = assembly.GetType(clientAssembly + "." + searchValue);

                            Type parmStructType = resultParams[parm.Name].GetType();
                            PropertyInfo[] refProperties = parmStructType.GetProperties();
                            Array parms = Array.CreateInstance(parmStructType, resultParams.Count);

                            if (parms == null && parm.IsRequired)
                            {
                                string errMsg = string.Format("Required parameter {0} is missing from response of {1}.",
                                    parm.Name, opDirective.Name);
                                throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3109 };
                            }

                            node.Attribute("Name").SetValue(parmStructType.Name.Replace("[","").Replace("]",""));
                            node.Attribute("IsRequired").SetValue(parm.IsRequired);


                            if (resultParams.FirstOrDefault().Value is object[])
                            {
                                object[] values = resultParams.FirstOrDefault().Value as object[];
                                if (values != null && values.Length > 0)
                                {
                                    foreach (object value in values)
                                    {
                                        //ReferenceDataRecords
                                        XElement valNode = new XElement(NS_SAPI + "Parm");
                                        valNode.Add(new XAttribute("Name", parmStructType.Name.Replace("[", "").Replace("]", "")));
                                        valNode.Add(new XAttribute("Type", parmStructType.FullName.Replace("[", "").Replace("]", "")));
                                        valNode.Add(new XAttribute("IsRequired", parm.IsRequired));
                                        valNode.Add(new XAttribute("IsArray", false));

                                        Type type = value.GetType();
                                        PropertyInfo[] valueProperties = type.GetProperties();

                                        foreach (PropertyInfo pInfo in valueProperties)
                                        {
                                            PropertyInfo propertyInfo = type.GetProperty(pInfo.Name);

                                            XElement dataNode = new XElement(pInfo.Name) { Name = pInfo.Name, Value = (((IClientDataObject)value).GetAttributeValue(pInfo.Name) ?? string.Empty).ToString() };
                                            if (dataNode != null)
                                            {
                                                valNode.Add(dataNode);
                                            }
                                        }
                                        node.Add(valNode);
                                    }
                                }
                            }

                            //APIStruct[] parmStructs = resultParams[parm.Name] as APIStruct[];
                            //if (parmStructs == null && parm.IsRequired)
                            //{
                            //    string errMsg = string.Format("Required parameter {0} is missing from response of {1}.",
                            //        parm.Name, opDirective.Name);
                            //    throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3109 };
                            //}
                            //foreach (APIStruct parmStruct in parmStructs)
                            //{
                            //    //ReferenceDataRecords
                            //    XElement valNode = new XElement(NS_SAPI + "Parm");
                            //    valNode.Add(new XAttribute("Name", parm.Name));
                            //    valNode.Add(new XAttribute("Type", parm.Type));
                            //    valNode.Add(new XAttribute("IsRequired", parmStruct.IsRequired));
                            //    valNode.Add(new XAttribute("IsArray", false));

                            //    node.Attribute("IsRequired").SetValue(parmStruct.IsRequired);

                            //    XElement subNode = new XElement(NS_SAPI + "Parm");
                            //    subNode.Add(new XAttribute("Name", parmStruct.Name));
                            //    subNode.Add(new XAttribute("Type", parmStruct.GetType()));
                            //    subNode.Add(new XAttribute("IsRequired", parmStruct.IsRequired));
                            //    subNode.Add(new XAttribute("IsArray", false));

                            //    APIArguments subparms = parmStruct.Parms;
                            //    foreach (var subparm in subparms)
                            //    {
                            //        XElement dataNode = new XElement(subparm.Key) { Name = subparm.Key, Value = (subparm.Value ?? string.Empty).ToString() };
                            //        if (dataNode != null)
                            //        {
                            //            subNode.Add(dataNode);
                            //        }
                            //    }
                            //    node.Add(subNode);
                            //}
                        }
                        else
                        {
                            APIStruct parmStruct = resultParams[parm.Name] as APIStruct;
                            if (parmStruct == null && parm.IsRequired)
                            {
                                string errMsg = string.Format("Required parameter {0} is missing from response of {1}.",
                                    parm.Name, opDirective.Name);
                                throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3109 };
                            }
                            node.Attribute("IsRequired").SetValue(parmStruct.IsRequired);
                            APIArguments subparms = parmStruct.Parms;
                            foreach (var subparm in subparms)
                            {
                                XElement subnode = new XElement(subparm.Key) { Name = subparm.Key, Value = (subparm.Value ?? string.Empty).ToString() };
                                if (subnode != null)
                                {
                                    node.Add(subnode);
                                }
                            }
                        }
                        break;
                    case LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet:
                        Brierley.FrameWork.Common.Config.LWConfigurationContext ctxas = Brierley.FrameWork.Common.Config.LWConfigurationUtil.GetCurrentEnvironmentContext();
                        XNamespace clientNamespaceas = string.Format("http://{0}.MemberProcessing.Schemas.{0}", ctxas.Organization);
                        XElement rootas = new XElement(clientNamespaceas + "AttributeSets");
                        node.Add(rootas);
                        if (!parm.IsArray)
                        {
                            IClientDataObject dObj = ((IList<IClientDataObject>)resultParams[parm.Name]).FirstOrDefault();

                            XElement objectNode = new XElement(NS_ANON + dObj.GetAttributeSetName());
                            objectNode = LWIntegrationUtilities.SerializeAttributeSetToXml(config, rootas, dObj, config.GetDateConversionFormat());
                       
                        }
                        else
                        {

                            foreach (IClientDataObject obj in ((IList<IClientDataObject>)resultParams[parm.Name]))
                            {
                                XElement objectNode = new XElement(NS_ANON + obj.GetAttributeSetName());

                                objectNode = LWIntegrationUtilities.SerializeAttributeSetToXml(config, rootas, obj, config.GetDateConversionFormat());
                            }
                        }
                        break;
                    default:
                        if (parm.IsArray)
                        {
                            object[] vals = resultParams[parm.Name] as object[];
                            if (vals == null)
                            {
                                // most likely an array of primitive types like long[], int[]. double[], boolean[]
                                if (parm.Type == LWIntegrationDirectives.ParmDataTypeEnums.Integer)
                                {
                                    int[] ivals = resultParams[parm.Name] as int[];
									if (ivals != null)
									{
										foreach (object val in ivals)
										{
											node.Add(new XElement(NS_SAPI + "Value", val));
										}
									}
                                }
                                else if (parm.Type == LWIntegrationDirectives.ParmDataTypeEnums.Long)
                                {
                                    long[] ivals = resultParams[parm.Name] as long[];
                                    foreach (object val in ivals)
                                    {
                                        node.Add(new XElement(NS_SAPI + "Value", val));
                                    }
                                }
                                else if (parm.Type == LWIntegrationDirectives.ParmDataTypeEnums.Decimal)
                                {
                                    decimal[] ivals = resultParams[parm.Name] as decimal[];
                                    foreach (object val in ivals)
                                    {
                                        node.Add(new XElement(NS_SAPI + "Value", val));
                                    }
                                }
                                else if (parm.Type == LWIntegrationDirectives.ParmDataTypeEnums.Boolean)
                                {
                                    bool[] ivals = resultParams[parm.Name] as bool[];
                                    foreach (object val in ivals)
                                    {
                                        node.Add(new XElement(NS_SAPI + "Value", val));
                                    }
                                }                                
                            }
                            else
                            {
                                foreach (object val in vals)
                                {
                                    node.Add(new XElement(NS_SAPI + "Value", val));
                                }
                            }                            
                        }
                        else
                        {
							node.Add(new XElement(NS_SAPI + "Value", resultParams[parm.Name]));
                        }
                        break;
                }
            }
            catch (KeyNotFoundException ex)
            {
                if (parm.IsRequired)
                {
                    string errMsg = string.Format("Required parameter {0} is missing from response of {1}.",
                        parm.Name, opDirective.Name);
                    throw new LWResponseMarshallingException(errMsg, ex) { ErrorCode = 3109 };
                }
                else
                {
                    node = null;
                }
            }
            catch (Exception ex)
            {                
                throw new LWResponseMarshallingException("Error processing parameter " + parm.Name, ex) { ErrorCode = 3110 };
            }
            return node;
        }
        #endregion

        #region deserialization

        private static APIArguments DeserializeParm(LWIntegrationDirectives config, XElement parm, APIArguments result)
        {
            string parmName = parm.Attribute("Name").Value;
            bool isArray = (parm.Attribute("IsArray") != null ? bool.Parse(parm.Attribute("IsArray").Value) : false);
            bool isRequired = (parm.Attribute("IsRequired") != null ? bool.Parse(parm.Attribute("IsRequired").Value) : false);
            XAttribute typeAttr = parm.Attribute("Type");
            switch (typeAttr.Value)
            {
                case "Integer":
                    if (isArray)
                    {
                        int[] vals = new int[parm.NSElements("Value").Count()];
                        int index = 0;
                        foreach (XElement arrayVal in parm.NSElements("Value"))
                        {
                            vals[index++] = int.Parse(arrayVal.Value);
                        }
                        result.Add(parmName, vals);
                    }
                    else
                    {
                        int val = int.Parse(parm.NSElement("Value").Value);
                        result.Add(parmName, val);
                    }
                    break;
                case "Long":
                    if (isArray)
                    {
                        long[] vals = new long[parm.NSElements("Value").Count()];
                        int index = 0;
                        foreach (XElement arrayVal in parm.NSElements("Value"))
                        {
                            vals[index++] = long.Parse(arrayVal.Value);
                        }
                        result.Add(parmName, vals);
                    }
                    else
                    {
                        long val = long.Parse(parm.NSElement("Value").Value);
                        result.Add(parmName, val);
                    }
                    break;
                case "Decimal":
                    if (isArray)
                    {
                        Decimal[] vals = new Decimal[parm.NSElements("Value").Count()];
                        int index = 0;
                        foreach (XElement arrayVal in parm.NSElements("Value"))
                        {
                            vals[index++] = Decimal.Parse(arrayVal.Value);
                        }
                        result.Add(parmName, vals);
                    }
                    else
                    {
                        Decimal val = Decimal.Parse(parm.NSElement("Value").Value);
                        result.Add(parmName, val);
                    }
                    break;
                case "Boolean":
                    if (isArray)
                    {
                        Boolean[] vals = new Boolean[parm.NSElements("Value").Count()];
                        int index = 0;
                        foreach (XElement arrayVal in parm.NSElements("Value"))
                        {
                            vals[index++] = Boolean.Parse(arrayVal.Value);
                        }
                        result.Add(parmName, vals);
                    }
                    else
                    {
                        Boolean val = Boolean.Parse(parm.NSElement("Value").Value);
                        result.Add(parmName, val);
                    }
                    break;
                case "Date":
                    if (isArray)
                    {
                        DateTime[] vals = new DateTime[parm.NSElements("Value").Count()];
                        int index = 0;
                        foreach (XElement arrayVal in parm.NSElements("Value"))
                        {
                            vals[index++] = config.HasISO8601CompliantDateStringFormat 
								? DateTimeUtil.ConvertISO8601StringToDate(arrayVal.Value, config.IgnoreTimeZoneOffset)
                                : DateTimeUtil.ConvertStringToDate(config.GetDateConversionFormat(), arrayVal.Value);
                        }
                        result.Add(parmName, vals);
                    }
                    else
                    {
						DateTime val = config.HasISO8601CompliantDateStringFormat
                            ? DateTimeUtil.ConvertISO8601StringToDate(parm.NSElement("Value").Value, config.IgnoreTimeZoneOffset)
                            : DateTimeUtil.ConvertStringToDate(config.GetDateConversionFormat(), parm.NSElement("Value").Value);
                        result.Add(parmName, val);
                    }
                    break;
                case "String":
                    if (isArray)
                    {
                        String[] vals = new String[parm.NSElements("Value").Count()];
                        int index = 0;
                        foreach (XElement arrayVal in parm.NSElements("Value"))
                        {
                            vals[index++] = arrayVal.Value;
                        }
                        result.Add(parmName, vals);
                    }
                    else
                    {
                        String val = parm.NSElement("Value").Value;
                        if (!string.IsNullOrEmpty(val))
                        {
                            result.Add(parmName, val);
                        }
                    }
                    break;
                case "Struct":
                    if (isArray)
                    {
                        APIStruct[] stList = null;
                        int count = parm.NSElements("Parm").Count();
                        if (isRequired && count == 0)
                        {
                            string errMsg = string.Format("Required parameter {0} is missing from struct {1}.",
                                parm.Name, parmName);
                            throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3108 };
                        }
                        stList = new APIStruct[count];
                        int i = 0;
                        foreach (XElement argNode in parm.NSElements("Parm"))
                        {
                            APIStruct st = new APIStruct() { Name = parmName, IsRequired = isRequired };
                            APIArguments args = new APIArguments();
                            foreach (XElement p in argNode.NSElements("Parm"))
                            {                                
                                if (isRequired && p == null || p.Elements().Count() == 0)
                                {
                                    string errMsg = string.Format("Required parameter is missing from struct {0}.", parmName);
                                    throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3108 };
                                }                                
                                st.Parms = DeserializeParm(config, p, args);
                            }
                            stList[i++] = st;
                        }
                        result.Add(parmName, stList);
                    }
                    else
                    { // non-array struct
                        APIStruct st = new APIStruct() { Name = parmName, IsRequired = isRequired };
                        //XElement v = parm.Element("Parm");
                        if (isRequired && (parm == null || parm.HasElements == false || parm.Elements().Count() == 0))
                        {
                            string errMsg = string.Format("Required parameter {0} is missing from struct {1}.",
                                parm.Name, parmName);
                            throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3108 };
                        }
                        APIArguments args = new APIArguments();
                        foreach (XElement p in parm.NSElements("Parm"))
                        {
                            if (isRequired && (p == null || p.HasElements == false || p.Elements().Count() == 0))
                            {
                                string errMsg = string.Format("Required parameter is missing from struct {0}.", parmName);
                                throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3108 };
                            }
                            st.Parms = DeserializeParm(config, p, args);
                        }
                        result.Add(parmName, st);
                    }
                    break;
                case "AttributeSet":
                    if (isArray)
                    {
                        string clientAssembly = DataServiceUtil.ClientsDataBaseName;
                        Assembly assembly = Assembly.Load(clientAssembly);

                        foreach (XElement elm in parm.Elements())
                        {
                            if (assembly.DefinedTypes.Select(u => u.Name).Contains(elm.Name.ToString()))
                            {
                                string currentKey = elm.Name.ToString();
                                if(result.ContainsKey(currentKey))
                                {
                                    IList<XElement> existingElements = new List<XElement>();
                                    existingElements = (IList<XElement>)result[currentKey];
                                    existingElements.Add(elm);
                                    result[currentKey] = existingElements;
                                }
                                else
                                {
                                    IList<XElement> elmList = new List<XElement>();
                                    elmList.Add(elm);
                                    result.Add(currentKey, elmList);
                                }
                            }
                            else
                            {
                                string errMsg = string.Format("{0} is not in the client datamodel.", elm.Name.ToString());
                                throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3108 };
                            }
                        }
                    }
                    else
                    {
                        string clientAssembly = DataServiceUtil.ClientsDataBaseName;
                        Assembly assembly = Assembly.Load(clientAssembly);

                        foreach (XElement elm in parm.Elements())
                        {
                            //Verify this element is in the allowed attributes to add?
                            if (assembly.DefinedTypes.Select(u => u.Name).Contains(elm.Name.ToString()))
                            {
                                result.Add(elm.Name.ToString(), elm);
                            }
                            else
                            {
                                string errMsg = string.Format("{0} is not in the client datamodel.", elm.Name.ToString());
                                throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3108 };
                            }
                        }
                    }
                    break;
            }
            return result;
        }

        public static APIArguments DeserializeRequest(string opName, LWIntegrationDirectives config, string payload)
        {
            string methodName = "DeserializeRequest";

            string errMsg;
            XDocument doc = XDocument.Parse(payload);
            XElement envelop = doc.Root;
            if (envelop.Name.LocalName != (opName + "InParms"))
            {
                errMsg = string.Format("Expected {0}InParms as the envelope.  Found {1}", opName, envelop.Name.LocalName);
                _logger.Error(_className, methodName, errMsg);
                throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3101 };
            }
            XElement inParm = (XElement)envelop.FirstNode;
            if (inParm.Name.LocalName != "Parm")
            {
                errMsg = string.Format("Expected 'Parm' as the enclosing type.  Found {0}", inParm.Name.LocalName);
                _logger.Error(_className, methodName, errMsg);
                throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3101 };
            }
            LWIntegrationDirectives.OperationDirective opDir = config.GetOperationDirective(opName);
            APIArguments result = new APIArguments();
            if (opDir.OperationMetadata.OperationInput != null && opDir.OperationMetadata.OperationInput.InputParms != null)
            {
                if (opDir.OperationMetadata.OperationInput.InputParms.Count > 1)
                {
                    XAttribute typeAttr = inParm.Attribute("Type");
                    if (typeAttr.Value != (opName + "In"))
                    {
                        errMsg = string.Format("Expected {0}In as the enclosing 'Type' attribute.  Found {1}", opName, typeAttr.Value);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3101 };
                    }
                    //IEnumerable<XElement> parms = inParm.NSElements(NS_SAPI + "Parm");
                    IEnumerable<XElement> parms = inParm.NSElements("Parm");
                    if (parms != null)
                    {
                        foreach (XElement parm in parms)
                        {
                            string parmName = parm.Attribute("Name").Value;
                            bool isArray = (parm.Attribute("IsArray") != null ? bool.Parse(parm.Attribute("IsArray").Value) : false);
                            bool isRequired = (parm.Attribute("IsRequired") != null ? bool.Parse(parm.Attribute("IsRequired").Value) : false);
                            if (!isArray && parm.Attribute("Type").Value != "Struct")
                            {
                                if (isRequired && (parm.NSElement("Value") == null || string.IsNullOrEmpty(parm.NSElement("Value").Value)))
                                {
                                    errMsg = string.Format("Required parameter {0} missing for operation {1}", parmName, opName);
                                    throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3108 };
                                }
                            }
                            result = DeserializeParm(config, parm, result);                            
                        }
                    }
                }
                else
                {
                    // Only one parameter
                    string parmName = inParm.Attribute("Name").Value;
                    bool isArray = (inParm.Attribute("IsArray") != null ? bool.Parse(inParm.Attribute("IsArray").Value) : false);
                    bool isRequired = (inParm.Attribute("IsRequired") != null ? bool.Parse(inParm.Attribute("IsRequired").Value) : false);
                    XAttribute typeAttr = inParm.Attribute("Type");
                    result = DeserializeParm(config, inParm, result);                    
                }                        
            }                        
            return result;
        }
        #endregion
    }
}
