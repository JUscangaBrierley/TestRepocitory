using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.IPFiltering;

namespace Brierley.LoyaltyWare.LWIntegrationSvc
{
    public class LWIntegrationDirectives : LWIntegrationConfig
    {
        #region Enumerations
        //public enum OperationIOTypeEnums : byte { None, Key, Member, Global, Xml, Primitive };
        public enum ParmDataTypeEnums : byte { Member, Global, Xml, String, Integer, Long, Decimal, Date, Boolean, Struct, AttributeSet };
        public enum OperationType : byte { FrameworkManaged, Custom };
        public enum MemberIdentityTypeEnums : byte { IpCode, AlternateId, EmailAddress, UserName, LoyaltyIdNumber };
        #endregion

        #region Fields
        private const string _className = "LWIntegrationDirectives";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        private Dictionary<string, OperationParmType> structMap = new Dictionary<string, OperationParmType>();
        #endregion

        #region Properties
        //public string DateConversionFormat { get; set; }
        public bool TrimStrings { get; set; }
        public bool CheckForChangedValues { get; set; }
        public bool EnforceValidBatch { get; set; }
		public string PasswordChangedEmailName { get; set; }
        public InterceptorDirective GlobalOperationValidator { get; set; }
        public MemberIdentityTypeEnums MemberIdentityType { get; set; }
        #endregion

		#region Constructors
		public LWIntegrationDirectives()
		{
			HasISO8601CompliantDateStringFormat = true;
		}
		#endregion

        #region Classes

        public class OperationParmType
        {
            public string Name { get; set; }            
            public ParmDataTypeEnums Type { get; set; }
            public bool IsRequired { get; set; }
            public bool IsArray { get; set; }
            public IList<OperationParmType> parms = new List<OperationParmType>();
            public IList<OperationParmType> Parms
            {
                get { return parms; }
                set { parms = value; }
            }
            public int StringLength { get; set; }
        }

        public class OperationInputType
        {            
            public string Description { get; set; }
            public bool LogParameters { get; set; }
            public IList<OperationParmType> inputParms = new List<OperationParmType>();
            public IList<OperationParmType> InputParms
            {
                get { return inputParms; }
                set { inputParms = value; }
            }
        }

        public class OperationOutputType
        {            
            public string Description { get; set; }
            public bool LogParameters { get; set; }
            public IList<OperationParmType> outputParms = new List<OperationParmType>();
            public IList<OperationParmType> OutputParms
            {
                get { return outputParms; }
                set { outputParms = value; }
            }
        }

        public class OperationMetadataType
        {
            //public bool StandardAPI { get; set; }
            public string Summary { get; set; }
            public OperationInputType OperationInput { get; set; }
            public OperationOutputType OperationOutput { get; set; }
        }
        
        public class EndPointRestriction
        {
            public IList<IPRange> AllowList { get; set; }
            public IList<IPRange> DenyList { get; set; }

            public bool HasRestrictions
            {
                get
                {
                    if ((AllowList != null && AllowList.Count > 0) || (DenyList != null && DenyList.Count > 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public class OnSuccessDirective
        {
            public string OnSuccessEvent { get; set; }
            public InterceptorDirective OnSuccessInterceptor { get; set; }            
            public bool IsAsynchronous { get; set; }
        }

        public class OperationDirective : LWIntegrationConfig.Directive
        {
            public string Name { get; set; }
            public bool IsAuthorized { get; set; }
            public EndPointRestriction EndPointRestrictions { get; set; }
            public OperationType Type { get; set; }
            public OperationResponseDirective ResponseDirective { get; set; }
            public OperationMetadataType OperationMetadata { get; set; }
            public OnSuccessDirective OnSuccess { get; set; }
        }

        public class FrameworkManagedOperationDirective : OperationDirective
        {
            public LWIntegrationConfig.InterceptorDirective InterceptorDirective { get; set; }
            public LWIntegrationConfig.MemberLoadDirectives MemberLoadDirectives { get; set; }

            private IDictionary<string, LWIntegrationConfig.AttributeSetDirective> createDreatives = new Dictionary<string, LWIntegrationConfig.AttributeSetDirective>();
            public IDictionary<string, LWIntegrationConfig.AttributeSetDirective> CreateDreatives
            {
                get { return createDreatives; }
                set { createDreatives = value; }
            }
                        
            public string GenericErrorMessage { get; set; }            
        }

        public class APIOperationDirective : OperationDirective
        {            
            public string FunctionProvider { get; set; }            
            public string ProviderAssemblyPath { get; set; }
            public NameValueCollection FunctionProviderParms { get; set; }
            public InterceptorDirective Interceptor { get; set; }

            private IDictionary<string, LWIntegrationConfig.AttributeSetDirective> addDirectives = new Dictionary<string, LWIntegrationConfig.AttributeSetDirective>();
            public IDictionary<string, LWIntegrationConfig.AttributeSetDirective> AddDirectives
            {
                get { return addDirectives; }
                set { addDirectives = value;}
            }
        }

        private IDictionary<string, OperationDirective> _operations = new Dictionary<string, OperationDirective>();
        public IDictionary<string, OperationDirective> OperationDirectives
        {
            get { return _operations; }
        }

        #endregion

        #region Public Methods

        public LWIntegrationDirectives.OperationDirective GetOperationDirective(string opName)
        {
            string methodName = "GetOperationDirective";            
            if (OperationDirectives.ContainsKey(opName))
            {
                return OperationDirectives[opName];
            }
            else
            {
                string error = string.Format("No configuration exists for operation {0}.", opName);
                _logger.Critical(_className, methodName, error);
                return null;
            }
        }

        public void Load(string configFile)
        {
            string methodName = "Load";

            _logger.Trace(_className, methodName, "Parsing configuration file: " + configFile);

            LWIntegrationDirectives directives = this/*new LWIntegrationDirectives()*/;
            using (StreamReader reader = new StreamReader(configFile))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;               
                if (root == null || root.Name.LocalName != "IntegrationOperationDirectives")
                {
                    string errMessage = string.Format("'IntegrationOperationDirectives' was expected in the configuration file.  Found {1}",
                        ((XElement)doc.FirstNode).Name.LocalName);
                    _logger.Error(_className, methodName, errMessage);
                    throw new LWIntegrationCfgException(errMessage);
                }
                directives.MemberIdentityType = MemberIdentityTypeEnums.IpCode;
                XAttribute idAtt = root.Attribute("MemberIdentityType");
                if (idAtt != null)
                {
                    directives.MemberIdentityType = (MemberIdentityTypeEnums)Enum.Parse(typeof(MemberIdentityTypeEnums), idAtt.Value);
                }

                //XAttribute att = root.Attribute("DateConversionFormat");
                //if (att != null)
                //{
                //    DateConversionFormat = att.Value;
                //}
                XAttribute att = root.Attribute("TrimStringSpaces");
                if (att != null)
                {
                    TrimStrings = bool.Parse(att.Value);
                }
                else
                {
                    TrimStrings = false;
                }
                att = root.Attribute("CheckForChangedValues");
                if (att != null)
                {
                    CheckForChangedValues = bool.Parse(att.Value);
                }
                else
                {
                    CheckForChangedValues = false;
                }
                att = root.Attribute("EnforceValidBatch");
                if (att != null)
                {
                    EnforceValidBatch = bool.Parse(att.Value);
                }
                else
                {
                    EnforceValidBatch = false;
                }
				att = root.Attribute("IgnoreTimeZoneOffset");
				if (att != null)
				{
					IgnoreTimeZoneOffset = bool.Parse(att.Value);
				}
				else
				{
					IgnoreTimeZoneOffset = false;
				}
				att = root.Attribute("PasswordChangedEmailName");
				if (att != null)
				{
					PasswordChangedEmailName = att.Value;
				}
				else
				{
					PasswordChangedEmailName = string.Empty;
				}

                XNamespace ns = root.Name.Namespace;                
                XElement opNode = root.Element(ns + "OperationProcessingDirectives");
                if (opNode != null)
                {
                    XElement validatorNode = opNode.Element(ns + "GlobalOperationValidator");
                    if (validatorNode != null)
                    {
                        GlobalOperationValidator = ProcessInterceptorAssembly(validatorNode);
                    }
                    ProcessOperationDirectives(opNode);
                }
            }
            //return directives;
        }

        #endregion

        #region Private Helpers

        #region Operation Directives

        private OnSuccessDirective ProcessOnSuccessDirective(XElement node)
        {
            OnSuccessDirective dir = new OnSuccessDirective() { IsAsynchronous = false };
            XAttribute att = node.Attribute("Asynchronous");
            if (att != null && !string.IsNullOrEmpty(att.Value) )
            {
                dir.IsAsynchronous = bool.Parse(att.Value);
            }
            XElement osEvent = node.Element(node.Name.Namespace + "OnSuccessEvent");
            if (osEvent != null)
            {
                att = osEvent.Attribute("EventName");
                if (att == null || string.IsNullOrEmpty(att.Value))
                {
                    throw new LWIntegrationCfgException("No event name specified for OnSuccessEvent.") { ErrorCode = 8000 };
                }
                dir.OnSuccessEvent = att.Value;
            }
            else
            {
                XElement osIncpt = node.Element(node.Name.Namespace + "OnSuccessInterceptor");
                dir.OnSuccessInterceptor = ProcessInterceptorAssembly(osIncpt);
            }            
            return dir;
        }

        private static EndPointRestriction ProcessEndPointRestriction(XElement node)
        {
            EndPointRestriction r = new EndPointRestriction();
            XElement allow = node.Element(node.Name.Namespace + "Allow");
            if (allow != null && !string.IsNullOrEmpty(allow.Value) )
            {
                string[] tokens = allow.Value.Split(',');
                r.AllowList = tokens.Select((s) => IPRange.ParseRange(s)).ToArray();                
            }
            XElement disallow = node.Element(node.Name.Namespace + "Deny");
            if (disallow != null && !string.IsNullOrEmpty(disallow.Value))
            {
                string[] tokens = disallow.Value.Split(',');
                r.DenyList = tokens.Select((s) => IPRange.ParseRange(s)).ToArray();                
            }
            return r;
        }

        private void ProcessOperationDirectives(XElement rootNode)
        {
            // process operation directives
            foreach (XElement node in rootNode.Nodes().OfType<XElement>())
            {
                if (node.Name.LocalName == "FrameworkManagedOperationDirective")
                {
                    ProcessFrameworkManagedOperationDirective(node);
                }
                else if (node.Name.LocalName == "APIOperationDirective")
                {
                    ProcessAPIOperationDirective(node);
                }
            }
        }
        #endregion

        #region Framework Managed Operation

        private static void ProcessAttributeSetsToLoad(XElement node, List<string> attributesToLoad)
        {
            foreach (XElement curr in node.Elements(node.Name.Namespace + "LoadAttributeSet"))
            {
                attributesToLoad.Add(curr.Value);
            }
        }

        private OperationResponseDirective ProcessOperationResponseDirective(XElement directiveNode)
        {
            OperationResponseDirective directive = new OperationResponseDirective();            
            XAttribute att = directiveNode.Attribute("Return");
            //string returnType = "None";
            //if (att != null)
            //{
            //    returnType = att.Value;
            //}
            //directive.ReturnType = (OperationResponseType)Enum.Parse(typeof(OperationResponseType), returnType);

            XElement element = directiveNode.Element(directiveNode.Name.Namespace + "AttributeSetsToLoad");
            if (element != null)
            {
                att = element.Attribute("ReloadMember");
                directive.ReloadMember = false;
                if (att != null && !string.IsNullOrEmpty(att.Value) )
                {
                    directive.ReloadMember = bool.Parse(att.Value);
                }                
                ProcessAttributeSetsToLoad(element, directive.AttributeSetsToLoad);                
            }
            element = directiveNode.Element(directiveNode.Name.Namespace + "MemberResponseHelper");
            if (element != null)
            {
                directive.ResponseHelperDirective = ProcessInterceptorAssembly(element);
            }           
            return directive;
        }

        private APIOperationDirective ProcessAttributeSetAddDirectives(APIOperationDirective directive, XElement addDirectiveNode)
        {

            foreach (XElement directiveNode in addDirectiveNode.Nodes().OfType<XElement>())
            {
                LWIntegrationConfig.AttributeSetDirective drctv = null;
                if (directiveNode.Name.LocalName == "AttributeSetAdd")
                {
                    drctv = ProcessAttributeSetAddDirective(directiveNode);
                }
                if (drctv != null)
                {
                    directive.AddDirectives.Add(drctv.Name, drctv);
                }
            }

            return directive;
        }
           
        private void ProcessAttributeSetCreateDirectives(FrameworkManagedOperationDirective directive, XElement createDirectiveNode)
        {
            foreach (XElement directiveNode in createDirectiveNode.Nodes().OfType<XElement>())
            {
                LWIntegrationConfig.AttributeSetDirective drctv = null;
                if (directiveNode.Name.LocalName == "AttributeSetModify")
                {
                    drctv = ProcessAttributeSetModifyDirective(directiveNode);
                }
                else if (directiveNode.Name.LocalName == "AttributeSetCreate")
                {
                    drctv = ProcessAttributeSetCreateDirective(directiveNode);
                }
                else if (directiveNode.Name.LocalName == "AttributeSetUpdate")
                {
                    drctv = ProcessAttributeSetUpdateDirective(directiveNode);
                }
                if (drctv != null)
                {
                    directive.CreateDreatives.Add(drctv.Name, drctv);                    
                }
            }
        }

        private OperationParmType ProcessOperationParameterDirective(XElement node)
        {
            OperationParmType parm = new OperationParmType();
            parm.Name = node.Attribute("Name").Value;
            parm.Type = (ParmDataTypeEnums)Enum.Parse(typeof(ParmDataTypeEnums), node.Attribute("Type").Value);
            parm.IsRequired = false;
            if (node.Attribute("IsRequired") != null)
            {
                parm.IsRequired = bool.Parse(node.Attribute("IsRequired").Value);
            }
            parm.IsArray = false;
            if (node.Attribute("IsArray") != null)
            {
                parm.IsArray = bool.Parse(node.Attribute("IsArray").Value);
            }
            if (node.Attribute("StringLength") != null)
            {
                parm.StringLength = int.Parse(node.Attribute("StringLength").Value);
            }
            if (parm.Type == ParmDataTypeEnums.Struct)
            {
                if (structMap.ContainsKey(parm.Name))
                {
                    //throw new LWIntegrationCfgException("Struct " + parm.Name + " has already been declared previously.");
                }
                else
                {
                    structMap.Add(parm.Name, parm);
                }
                foreach (XElement sub in node.Elements())
                {
                    if (sub.NodeType == System.Xml.XmlNodeType.Element && sub.Name.LocalName == "Parm")
                    {
                        OperationParmType sp = ProcessOperationParameterDirective(sub);
                        parm.Parms.Add(sp);
                    }
                }
            }
            return parm;
        }

        private IList<OperationParmType> ProcessOperationParametersDirective(XElement directiveNode)
        {
            IList<OperationParmType> parms = new List<OperationParmType>();

            foreach (XElement cNode in directiveNode.Nodes().OfType<XElement>())
            {
                if (cNode.Name.LocalName == "Parm")
                {
                    OperationParmType parm = ProcessOperationParameterDirective(cNode);                    
                    parms.Add(parm);                    
                }
            }


            return parms;
        }

        private OperationMetadataType ProcessOperationMetaDirective(XElement directiveNode)
        {
            //OperationMetadataType directive = new OperationMetadataType() { StandardAPI = false };
            OperationMetadataType directive = new OperationMetadataType();


            //XAttribute att = directiveNode.Attribute("StandardAPI");
            //if ( att != null )
            //{
            //    directive.StandardAPI = bool.Parse(att.Value);
            //}
            XNamespace ns = directiveNode.Name.Namespace;

            XElement summaryNode = directiveNode.Element(ns + "Summary");
            if (summaryNode != null)
            {
                directive.Summary = summaryNode.Value;                
            }

            XElement inputNode = directiveNode.Element(ns + "OperationInput");
            if (inputNode != null)
            {
                OperationInputType input = new OperationInputType();                
                if (inputNode.Attribute("Description") != null)
                {
                    input.Description = inputNode.Attribute("Description").Value;
                }
                input.LogParameters = true;
                if (inputNode.Attribute("LogParameters") != null)
                {
                    input.LogParameters = bool.Parse(inputNode.Attribute("LogParameters").Value);
                }
                input.InputParms = ProcessOperationParametersDirective(inputNode);
                directive.OperationInput = input;
            }
            
            XElement outputNode = directiveNode.Element(ns + "OperationOutput");
            if (outputNode != null)
            {
                OperationOutputType output = new OperationOutputType();                
                if (outputNode.Attribute("Description") != null)
                {
                    output.Description = outputNode.Attribute("Description").Value;
                }
                output.LogParameters = true;
                if (outputNode.Attribute("LogParameters") != null)
                {
                    output.LogParameters = bool.Parse(outputNode.Attribute("LogParameters").Value);
                }
                output.OutputParms = ProcessOperationParametersDirective(outputNode);                
                directive.OperationOutput = output;
            }

            return directive;
        }

        private void ProcessFrameworkManagedOperationDirective(XElement directiveNode)
        {
            string methodName = "ProcessFrameworkManagedOperationDirective";

            FrameworkManagedOperationDirective directive = new FrameworkManagedOperationDirective();
            directive.Name = directiveNode.Attribute("Name").Value;
            directive.IsAuthorized = bool.Parse(directiveNode.Attribute("Authorized").Value);

            directive.Type = OperationType.FrameworkManaged;

            XNamespace ns = directiveNode.Name.Namespace;

            XElement endpointRestrictionNode = directiveNode.Element(ns + "EndPointRestriction");
            if (endpointRestrictionNode != null)
            {
                directive.EndPointRestrictions = ProcessEndPointRestriction(endpointRestrictionNode);
                if (directive.EndPointRestrictions != null && directive.EndPointRestrictions.AllowList != null)
                {
                    foreach (IPRange r in directive.EndPointRestrictions.AllowList)
                    {
                        _logger.Trace(_className, methodName,
                            string.Format("Allow list for operation {0} is ", directive.Name, r.ToString()));
                    }
                }
                if (directive.EndPointRestrictions != null && directive.EndPointRestrictions.DenyList!= null)
                {
                    foreach (IPRange r in directive.EndPointRestrictions.DenyList)
                    {
                        _logger.Trace(_className, methodName,
                            string.Format("Deny list for operation {0} is ", directive.Name, r.ToString()));
                    }
                }
            }

            XElement intercoptorNode = directiveNode.Element(ns + "InterceptorAssembly");            
            if (intercoptorNode != null)
            {
                directive.InterceptorDirective = ProcessInterceptorAssembly(intercoptorNode);
            }

            XElement memberLoadNode = directiveNode.Element(ns + "MemberLoadDirectives");
            if (memberLoadNode != null)
            {
                directive.MemberLoadDirectives = ProcesMemberLoadDirectives(memberLoadNode);
            }

            XElement createDirectiveNode = directiveNode.Element(ns + "AttributeSetCreateDirectives");
            if (createDirectiveNode != null)
            {
                ProcessAttributeSetCreateDirectives(directive, createDirectiveNode);
            }

            XElement responseDirectiveNode = directiveNode.Element(ns + "OperationResponse");
            if (responseDirectiveNode != null)
            {
                directive.ResponseDirective = ProcessOperationResponseDirective(responseDirectiveNode);
            }

            XElement errMsgNode = directiveNode.Element(ns + "GenericErrorMessage");
            if (errMsgNode != null)
            {
                directive.GenericErrorMessage = errMsgNode.Value;
            }

            XElement codeGenNode = directiveNode.Element(ns + "OperationMetadata");
            if (codeGenNode != null)
            {
                directive.OperationMetadata = ProcessOperationMetaDirective(codeGenNode);               
            }
            _operations.Add(directive.Name, directive);
        }
        #endregion

        #region Custom Operations

        private APIOperationDirective ProcessQueryProviderDirective(APIOperationDirective directive, XElement directiveNode)
        {
            foreach (XElement node in directiveNode.Nodes().OfType<XElement>())
            {
                if (node.Name.LocalName == "ProviderType")
                {
                    directive.FunctionProvider = node.Value;
                }
                if (node.Name.LocalName == "ProviderAssemlyPath")
                {
                    directive.ProviderAssemblyPath = node.Value;
                }
                else if (node.Name.LocalName == "Interceptor")
                {
                    directive.Interceptor = ProcessInterceptorAssembly(node);                    
                }
                else if (node.Name.LocalName == "FunctionProviderParms")
                {
                    NameValueCollection nv = directive.FunctionProviderParms;
                    if (nv == null)
                    {
                        nv = new NameValueCollection();
                    }
                    foreach (XElement parmNode in node.Nodes().OfType<XElement>())
                    {
                        if (parmNode.Name.LocalName == "Parm")
                        {
                            string pname = string.Empty;
                            string pvalue = string.Empty;
                            XAttribute patt = parmNode.Attribute("Name");
                            if (patt != null)
                            {
                                pname = patt.Value;
                            }
                            patt = parmNode.Attribute("Value");
                            if (patt != null)
                            {
                                pvalue = patt.Value;
                            }
                            nv.Add(pname, pvalue);
                        }
                    }
                    directive.FunctionProviderParms = nv;
                }
            }

            return directive;
        }

        private void ProcessAPIOperationDirective(XElement directiveNode)
        {
            string methodName = "ProcessAPIOperationDirective";

            APIOperationDirective directive = new APIOperationDirective();
            directive.Name = directiveNode.Attribute("Name").Value;
            directive.IsAuthorized = bool.Parse(directiveNode.Attribute("Authorized").Value);

            directive.Type = OperationType.Custom;

            XElement endpointRestrictionNode = directiveNode.Element(directiveNode.Name.Namespace + "EndPointRestriction");
            if (endpointRestrictionNode != null)
            {
                directive.EndPointRestrictions = ProcessEndPointRestriction(endpointRestrictionNode);
                if (endpointRestrictionNode != null)
                {
                    directive.EndPointRestrictions = ProcessEndPointRestriction(endpointRestrictionNode);
                    if (directive.EndPointRestrictions != null && directive.EndPointRestrictions.AllowList != null)
                    {
                        foreach (IPRange r in directive.EndPointRestrictions.AllowList)
                        {
                            _logger.Trace(_className, methodName,
                                string.Format("Allow list for operation {0} is ", directive.Name, r.ToString()));
                        }
                    }
                    if (directive.EndPointRestrictions != null && directive.EndPointRestrictions.DenyList != null)
                    {
                        foreach (IPRange r in directive.EndPointRestrictions.DenyList)
                        {
                            _logger.Trace(_className, methodName,
                                string.Format("Deny list for operation {0} is ", directive.Name, r.ToString()));
                        }
                    }
                }
            }

            XElement funcPrNode = directiveNode.Element(directiveNode.Name.Namespace + "FunctionProvider");
            if (funcPrNode != null)
            {
                directive = ProcessQueryProviderDirective(directive, funcPrNode);
            }

            XElement directNode = directiveNode.Element(directiveNode.Name.Namespace + "AttributeSetAddDirectives");
            if (directNode != null)
            {
                directive = ProcessAttributeSetAddDirectives(directive, directNode);
            }

            XElement respNode = directiveNode.Element(directiveNode.Name.Namespace + "OperationResponse");
            if (respNode != null)
            {
                directive.ResponseDirective = ProcessOperationResponseDirective(respNode);
            }

            XElement codeGenNode = directiveNode.Element(directiveNode.Name.Namespace + "OperationMetadata");
            if (codeGenNode != null)
            {
                directive.OperationMetadata = ProcessOperationMetaDirective(codeGenNode);
            }

            XElement postProcNode = directiveNode.Element(directiveNode.Name.Namespace + "OnSuccess");
            if (postProcNode != null)
            {
                directive.OnSuccess = ProcessOnSuccessDirective(postProcNode);
            }

            try
            {
                _operations.Add(directive.Name, directive);
            }
            catch (Exception ex)
            {
                throw new LWIntegrationException("Error adding directive for custom operation " + directive.Name, ex);
            }
        }

        
        #endregion

        #endregion

        #region Implementation of Abstract Methods from LWIntegrationConfig
        public override string GetDateConversionFormat()
        {
            //return DateConversionFormat;
            return string.Empty;
        }

		public override string GetPasswordChangedEmailName()
		{
			return PasswordChangedEmailName;
		}
        #endregion
	}
}
