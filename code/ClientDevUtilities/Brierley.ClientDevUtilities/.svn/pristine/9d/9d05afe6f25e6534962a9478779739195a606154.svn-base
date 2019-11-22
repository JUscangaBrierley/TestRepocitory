using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.FrameWork.Rules;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class AddAttributeSet : OperationProviderBase
    {
        private const string _className = "AddAttributeSet";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        public AddAttributeSet() : base("AddAttributeSet") { }

        public override string Invoke(string source, string parms)
        {
            string methodName = "Invoke";

            try
            {
                string response = string.Empty;
                List<ContextObject.RuleResult> results = new List<ContextObject.RuleResult>();
                IAddAttributeSetInterceptor interceptor = null;
                IList<IClientDataObject> clientObjectsAdded = new List<IClientDataObject>();
                RuleExecutionMode mode = RuleExecutionMode.Real;

                LWIntegrationDirectives.APIOperationDirective directive = Config.GetOperationDirective(Name) as LWIntegrationDirectives.APIOperationDirective;

                if (directive.Interceptor != null)
                {
                    interceptor = InterceptorUtil.GetInterceptor(directive.Interceptor) as IAddAttributeSetInterceptor;
                }

                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for Add Attribute Set.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

                Member member = LoadMember(args);

                if (member.HasTransientProperty("executionmode"))
                {
                    string modeStr = (string)member.GetTransientProperty("executionmode");
                    if (!string.IsNullOrEmpty(modeStr))
                    {
                        modeStr = modeStr.ToLower();
                        if (modeStr != "real" && modeStr != "simulation")
                        {
                            string err = string.Format("Invalid execution mode {0} specified.  Valid values are real or simulation", modeStr);
                            throw new LWIntegrationException(err) { ErrorCode = 3231 };
                        }
                        if (modeStr == "simulation")
                        {
                            mode = RuleExecutionMode.Simulation;
                        }
                    }
                }

                foreach (var val in args)
                {
                    if (val.Value is XContainer)
                    {
                        if (directive.AddDirectives.ContainsKey(val.Key))
                        {
                            Brierley.FrameWork.LWIntegration.LWIntegrationConfig.AttributeSetAddDirective currDirective = directive.AddDirectives[val.Key] as Brierley.FrameWork.LWIntegration.LWIntegrationConfig.AttributeSetAddDirective;

                            XElement node = (XElement)args[val.Key.ToString()];


                            if (interceptor != null)
                            {
                                try
                                {
                                    node = interceptor.ProcessRawXmlElement(Config, member, node);
                                }
                                catch (NotImplementedException)
                                {
                                    // not implemented.
                                }
                                catch (Exception ex)
                                {
                                    string errMsg = "Exception thrown by ProcessRawXmlElement interceptor.";
                                    _logger.Error(_className, methodName, errMsg, ex);
                                    throw;
                                }
                            }

                            IClientDataObject currentObject = ProcessNode(member, node, LoyaltyDataService, null, currDirective, interceptor);
                            clientObjectsAdded.Add(currentObject);
                        }
                        else
                        {
                            string errMsg = "This attribute set is not configured to be added.";
                            throw new LWOperationInvocationException(errMsg) { ErrorCode = 3402 };
                        }
                    }
                    if (val.Value is IList<XElement>)
                    {
                        if (directive.AddDirectives.ContainsKey(val.Key))
                        {
                            Brierley.FrameWork.LWIntegration.LWIntegrationConfig.AttributeSetAddDirective currDirective = directive.AddDirectives[val.Key] as Brierley.FrameWork.LWIntegration.LWIntegrationConfig.AttributeSetAddDirective;

                            IList<XElement> nodes = (IList<XElement>)args[val.Key.ToString()];

                            foreach (XElement node in nodes)
                            {
                                XElement currentNode = node;

                                if (interceptor != null)
                                {
                                    try
                                    {
                                        currentNode = interceptor.ProcessRawXmlElement(Config, member, currentNode);
                                    }
                                    catch (NotImplementedException)
                                    {
                                        // not implemented.
                                    }
                                    catch (Exception ex)
                                    {
                                        string errMsg = "Exception thrown by ProcessRawXmlElement interceptor.";
                                        _logger.Error(_className, methodName, errMsg, ex);
                                        throw;
                                    }
                                }

                                //Custom code created for processing the attribute sets.
                                IClientDataObject currentObject = ProcessNode(member, currentNode, LoyaltyDataService, null, currDirective, interceptor);
                                clientObjectsAdded.Add(currentObject);
                            }
                        }
                        else
                        {
                            string errMsg = "This attribute set is not configured to be added.";
                            throw new LWOperationInvocationException(errMsg) { ErrorCode = 3402 };
                        }
                    }
                }
                if (interceptor != null)
                {
                    try
                    {
                        member = interceptor.ProcessMemberBeforeSave(Config, member);
                    }
                    catch (NotImplementedException)
                    {
                        // not implemented.
                    }
                    catch (Exception ex)
                    {
                        string errMsg = "Exception thrown by ProcessMemberBeforeSave interceptor.";
                        _logger.Error(_className, methodName, errMsg, ex);
                        throw;
                    }
                }

                LoyaltyDataService.ClearRuleResult(member);
                LoyaltyDataService.SaveMember(member, results, mode, false);

                if (interceptor != null)
                {
                    try
                    {
                        member = interceptor.ProcessMemberAfterSave(Config, member, results);
                    }
                    catch (NotImplementedException)
                    {
                        // not implemented.
                    }
                    catch (Exception ex)
                    {
                        string errMsg = "Exception thrown by ProcessMemberBeforeSave interceptor.";
                        _logger.Error(_className, methodName, errMsg, ex);
                        throw;
                    }
                }

                // Do post processing
                Dictionary<string, object> context = new Dictionary<string, object>();

                context.Add("attributeset", clientObjectsAdded);
                PostProcessSuccessfullInvocation(context);

                APIArguments resultParams = new APIArguments();

                //Validate output Params to determine output.
                //If attribute set, return attribute set.
                //If Points, return points.
                //If neither, return success.

                IList<LWIntegrationDirectives.OperationParmType> outputs = directive.OperationMetadata.OperationOutput.OutputParms;

                if (outputs.Any(u => u.Name == "AttributeSet") && clientObjectsAdded.Count > 0)
                {
                    resultParams.Add("AttributeSet", clientObjectsAdded);
                }


                //This needs to be simplified.
                if (outputs.Any(u => u.Name == "EarnedPoints"))
                {
                    if (outputs.Where(u => u.Name == "EarnedPoints").FirstOrDefault().Parms.Any(u => u.Name == "PointType") && outputs.Where(u => u.Name == "EarnedPoints").FirstOrDefault().Parms.Any(x => x.Name == "PointValue"))
                    {
                        Dictionary<string, decimal> pointInfo = new Dictionary<string, decimal>();
                        // Calculate poins earned
                        foreach (ContextObject.RuleResult res in results)
                        {
                            AwardPointsRuleResult result = res as AwardPointsRuleResult;
                            if (result != null)
                            {
                                if (pointInfo.ContainsKey(result.PointType))
                                {
                                    pointInfo[result.PointType] += result.PointsAwarded;
                                }
                                else
                                {
                                    pointInfo.Add(result.PointType, result.PointsAwarded);
                                }
                            }
                        }

                        APIStruct[] pointReturnInfo = new APIStruct[pointInfo.Count()];
                        long pointIdx = 0;

                        if (pointInfo != null && pointInfo.Count > 0)
                        {
                            foreach (KeyValuePair<string, decimal> pInfo in pointInfo)
                            {
                                APIArguments pointParams = new APIArguments();
                                pointParams.Add("PointType", pInfo.Key);
                                pointParams.Add("PointValue", pInfo.Value);

                                APIStruct pSum = new APIStruct { Name = "EarnedPoints", IsRequired = true, Parms = pointParams };
                                pointReturnInfo[pointIdx++] = pSum;
                            }
                            resultParams.Add("EarnedPoints", pointReturnInfo);
                        }
                    }
                }

                if (resultParams.Count > 0)
                {
                    response = SerializationUtils.SerializeResult(Name, Config, resultParams);
                }

                return response;
            }
            catch (LWIntegrationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }

        protected override void Cleanup()
        {
        }

        private IClientDataObject ProcessNode(Member member, XElement node, LoyaltyDataService service, IClientDataObject parent, Brierley.FrameWork.LWIntegration.LWIntegrationConfig.AttributeSetAddDirective currDirective, IAddAttributeSetInterceptor interceptor = null)
        {
            string methodName = "ProcessNode";

            IClientDataObject newObj = DataServiceUtil.GetNewClientDataObject(node.Name.ToString());

            if (newObj != null)
            {
                AttributeSetMetaData metadata = service.GetAttributeSetMetaData(node.Name.ToString());

                if (metadata != null)
                {
                    IList<AttributeMetaData> attributeData = metadata.Attributes;

                    //Check required attributes
                    //This can probably be simplified somehow, need to check these qualifiers:
                    //  1. Required fields are in node.Attributes()
                    //  2. Fields can be parsed to correct datatype
                    //  3. Existing Uniqueness in the attribute set.
                    foreach (AttributeMetaData attr in attributeData)
                    {
                        foreach (XAttribute nodeAttr in node.Attributes())
                        {
                            if (attr.Name == nodeAttr.Name)
                            {
                                //Check uniqueness
                                if (attr.IsUnique)
                                {
                                    LWCriterion crit = new LWCriterion(metadata.Name);
                                    crit.Add(LWCriterion.OperatorType.AND, attr.Name, nodeAttr.Value, LWCriterion.Predicate.Eq);

                                    List<long> existingATS = service.GetAttributeSetObjectIds(null, metadata, crit);

                                    if (existingATS != null && existingATS.Count() > 0)
                                    {
                                        string errMsg = string.Format("Attribute {0} is unique and already exists with value of {1} on the {2} container.", nodeAttr.Name, nodeAttr.Value, node.Name.ToString());
                                        throw new LWOperationInvocationException(errMsg) { ErrorCode = 3399 };
                                    }
                                }

                                try
                                {
                                    bool addMissingPropertyAsTransient = currDirective != null ? currDirective.AddMissingPropertyAsTransient : false;

                                    LWIntegrationUtilities.ProcessAttributeFromXml(Config, newObj, nodeAttr, Config.GetDateConversionFormat(), Config.TrimStrings, Config.CheckForChangedValues, addMissingPropertyAsTransient, metadata, attr);
                                }
                                catch (LWException e)
                                {
                                    string errMsg = string.Format("Attribute {0} failed to parse or be added to the {1} container.", nodeAttr.Name, node.Name.ToString());
                                    throw new LWOperationInvocationException(errMsg, e) { ErrorCode = 3400 };
                                }
                            }
                        }

                        if (attr.IsRequired && !node.Attributes().Any(u => u.Name == attr.Name))
                        {
                            string errMsg = string.Format("Attribute {0} is a required attribute of {1} container.", attr.Name, node.Name.ToString());
                            throw new LWOperationInvocationException(errMsg) { ErrorCode = 3401 };
                        }
                    }

                    if (interceptor != null)
                    {
                        try
                        {
                            newObj = interceptor.ProcessObjectBeforeAdd(Config, member, newObj, metadata);
                        }
                        catch (NotImplementedException)
                        {
                            // not implemented.
                        }
                        catch (Exception ex)
                        {
                            string errMsg = "Exception thrown by ProcessObjectBeforeAdd interceptor.";
                            _logger.Error(_className, methodName, errMsg, ex);
                            throw;
                        }
                    }

                    //Add this attribute set to parent container
                    if (metadata.Type == AttributeSetType.Member)
                    {
                        member.AddChildAttributeSet(newObj);
                    }
                    else if (metadata.Type == AttributeSetType.VirtualCard && metadata.ParentID == -1)
                    {
                        //Check for VCKey on the node, if not found, use directive to get loyalty card.  Directive defaults to primary card if null.
                        if ((string)node.Attribute("VcKey") != null)
                        {
                            long vcKey = (long)node.Attribute("VcKey");
                            member.LoyaltyCards.FirstOrDefault(u => u.VcKey == vcKey).AddChildAttributeSet(newObj);
                        }
                        else
                        {
                            member.GetLoyaltyCardByType(currDirective.VirtualCardType).AddChildAttributeSet(newObj);
                        }
                    }
                    //Since we don't have the concept of child type we will determine that by looking at the parent id
                    else if (metadata.ParentID != -1)
                    {
                        if (parent != null)
                        {
                            parent.AddChildAttributeSet(newObj);
                        }
                        else
                        {
                            string errMsg = string.Format("Unable to add {0} container with no parent container.", node.Name.ToString());
                            throw new LWOperationInvocationException(errMsg) { ErrorCode = 3403 };
                        }
                    }
                    else
                    {
                        string errMsg = string.Format("Unable to determine {0} container meta data type.", node.Name.ToString());
                        throw new LWOperationInvocationException(errMsg) { ErrorCode = 3404 };
                    }
                }
                else
                {
                    string errMsg = string.Format("No metadata for {0} container can be found.", node.Name.ToString());
                    throw new LWOperationInvocationException(errMsg) { ErrorCode = 3405 };
                }
            }

            if (node.HasElements)
            {
                IEnumerable<XElement> children = node.Elements();
                foreach (XElement child in children)
                {
                    ProcessNode(member, child, service, newObj, currDirective, interceptor);
                }
            }

            return newObj;
        }
    }
}
