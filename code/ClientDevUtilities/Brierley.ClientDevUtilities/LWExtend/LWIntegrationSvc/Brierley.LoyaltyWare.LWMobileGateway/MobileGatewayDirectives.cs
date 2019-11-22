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

namespace Brierley.LoyaltyWare.LWMobileGateway
{
    public class MobileGatewayDirectives : LWIntegrationConfig
    {
        #region Enumerations
        public enum MemberIdentityTypeEnums : byte { IpCode, AlternateId, EmailAddress, UserName, LoyaltyIdNumber };
        #endregion

        #region Fields
        private const string _className = "MobileGatewayDirectives";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);        
        #endregion

        #region Properties
        public string ClientId { get; set; }
        public bool TrimStrings { get; set; }
        public bool CheckForChangedValues { get; set; }
        public bool EnforceValidBatch { get; set; }
        public bool CaptureAPIStats { get; set; }
		public string PasswordChangedEmailName { get; set; }
        public InterceptorDirective AuthorizationInterceptor { get; set; }
        public InterceptorDirective GlobalOperationValidator { get; set; }
        public MemberIdentityTypeEnums MemberIdentityType { get; set; }
        #endregion

		#region Constructors
        public MobileGatewayDirectives()
		{
			HasISO8601CompliantDateStringFormat = true;
		}
		#endregion

        #region Classes
                                
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
            //public string UriTemplate { get; set; }
            public bool LogRequest { get; set; }
            public bool LogResponse { get; set; }
            public bool RequiresAuthorization { get; set; }
            public EndPointRestriction EndPointRestrictions { get; set; }
            public OperationResponseDirective ResponseDirective { get; set; }
            private IDictionary<string, LWIntegrationConfig.AttributeSetDirective> createDreatives = new Dictionary<string, LWIntegrationConfig.AttributeSetDirective>();
            public IDictionary<string, LWIntegrationConfig.AttributeSetDirective> CreateDreatives
            {
                get { return createDreatives; }
                set { createDreatives = value; }
            }
            public OnSuccessDirective OnSuccess { get; set; }
        }
        
        public class APIOperationDirective : OperationDirective
        {            
            public string FunctionProvider { get; set; }            
            public string ProviderAssemblyPath { get; set; }
            public NameValueCollection FunctionProviderParms { get; set; }
            public InterceptorDirective Interceptor { get; set; }
        }

        private IDictionary<string, OperationDirective> _operations = new Dictionary<string, OperationDirective>();
        public IDictionary<string, OperationDirective> OperationDirectives
        {
            get { return _operations; }
        }

        #endregion

        #region Public Methods

        public MobileGatewayDirectives.OperationDirective GetOperationDirectiveByName(string opName)
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

            MobileGatewayDirectives directives = this;
            using (StreamReader reader = new StreamReader(configFile))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;
                if (root == null || root.Name.LocalName != "MobileGatewayOperationDirectives")
                {
                    string errMessage = string.Format("'MobileGatewayOperationDirectives' was expected in the configuration file.  Found {1}",
                        ((XElement)doc.FirstNode).Name.LocalName);
                    _logger.Error(_className, methodName, errMessage);
                    throw new LWIntegrationCfgException(errMessage);
                }

                XAttribute clientidAtt = root.Attribute("ClientId");
                if (clientidAtt != null)
                {
                    directives.ClientId = clientidAtt.Value;
                }

                directives.MemberIdentityType = MemberIdentityTypeEnums.IpCode;
                XAttribute idAtt = root.Attribute("MemberIdentityType");
                if (idAtt != null)
                {
                    directives.MemberIdentityType = (MemberIdentityTypeEnums)Enum.Parse(typeof(MemberIdentityTypeEnums), idAtt.Value);
                }
                
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
                att = root.Attribute("CaptureAPIStats");
                if (att != null)
                {
                    CaptureAPIStats = bool.Parse(att.Value);
                }
                else
                {
                    CaptureAPIStats = false;
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
                    XElement authIntNode = opNode.Element(ns + "AuthorizationInterceptor");
                    if (authIntNode != null)
                    {
                        AuthorizationInterceptor = ProcessInterceptorAssembly(authIntNode);
                    }
                    ProcessOperationDirectives(opNode);
                }
            }            
        }

        #endregion

        #region Private Helpers

        #region Operation Directives

        private void ProcessAttributeSetsToLoad(XElement node, List<string> attributesToLoad)
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
            string returnType = "None";
            if (att != null)
            {
                returnType = att.Value;
            }
            //directive.ReturnType = (OperationResponseType)Enum.Parse(typeof(OperationResponseType), returnType);

            XElement element = directiveNode.Element(directiveNode.Name.Namespace + "AttributeSetsToLoad");
            if (element != null)
            {
                att = element.Attribute("ReloadMember");
                directive.ReloadMember = false;
                if (att != null && !string.IsNullOrEmpty(att.Value))
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

        private void ProcessAttributeSetCreateDirectives(APIOperationDirective directive, XElement createDirectiveNode)
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
                if (drctv != null && !string.IsNullOrEmpty(drctv.Name))
                {
                    directive.CreateDreatives.Add(drctv.Name, drctv);
                }
            }
        }

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

        private EndPointRestriction ProcessEndPointRestriction(XElement node)
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
            string methodName = "ProcessOperationDirectives";
            // process operation directives
            foreach (XElement node in rootNode.Nodes().OfType<XElement>())
            {
                if (node.Name.LocalName == "APIOperationDirective")
                {
                    ProcessAPIOperationDirective(node);
                }
                else if (node.Name.LocalName == "GlobalOperationValidator" || node.Name.LocalName == "AuthorizationInterceptor")
                {
                    // already done with that - ignore it.
                }
                else
                {
                    _logger.Error(_className, methodName,
                        string.Format("Was expecting APIOperationDirective but instead got {0}", node.Name.LocalName));
                    throw new LWIntegrationCfgException("Invalid operation directive encountered.") { ErrorCode = 1 };
                }
            }
        }
        #endregion
        
        #region API Operations

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
            //directive.UriTemplate = directiveNode.Attribute("UriTemplate").Value;
            XAttribute logAtt = directiveNode.Attribute("LogRequest");
            directive.LogRequest = false;
            if (logAtt != null)
            {
                directive.LogRequest = bool.Parse(logAtt.Value);
            }
            
            logAtt = directiveNode.Attribute("LogResponse");
            directive.LogResponse = false;
            if (logAtt != null)
            {
                directive.LogResponse = bool.Parse(logAtt.Value);
            }

            directive.RequiresAuthorization = true;
            XAttribute authAtt = directiveNode.Attribute("RequiresAuthorization");
            if (authAtt != null)
            {
                directive.RequiresAuthorization = bool.Parse(authAtt.Value);
            }

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

            XElement operationRespNode = directiveNode.Element(directiveNode.Name.Namespace + "OperationResponse");
            if (operationRespNode != null)
            {
                directive.ResponseDirective = ProcessOperationResponseDirective(operationRespNode);
            }

            XElement createDirectiveNode = directiveNode.Element(directiveNode.Name.Namespace + "AttributeSetCreateDirectives");
            if (createDirectiveNode != null)
            {                
                ProcessAttributeSetCreateDirectives(directive, createDirectiveNode);                
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
