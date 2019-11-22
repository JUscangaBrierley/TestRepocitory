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

namespace Brierley.FrameWork.LWIntegration
{
	public abstract class LWIntegrationConfig
	{
		#region Enumerations
        public enum DataTypeEnums : byte { MemberAttributeSets, GlobalAttributeSets, Coupons, Products, Stores, MessageDefs, Rewards, MemberBonus, MemberCoupon, CSAgents, Certificates, ContactHistory, SocialListening, NextBestAction, SmsOptIn, ExchangeRate };
		public enum MemberLoadMethod : byte { IpCode, CertificateNumber, AlternateId, EmailAddress, UserName, LoyaltyIdNumber, UseInterceptor };
		public enum AttributeSetFindMethodEnum : byte { First, Last, Newest, Index, Attribute };
		public enum CreateType : byte { Create, Update };
		#endregion

		#region Fields
		private const string _className = "LWIntegrationConfig";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		#endregion

		#region Properties
		public bool HasISO8601CompliantDateStringFormat { get; set; }
		public bool IgnoreTimeZoneOffset { get; set; }
		#endregion

		#region Constructors
		public LWIntegrationConfig()
		{
			HasISO8601CompliantDateStringFormat = false;
			IgnoreTimeZoneOffset = false;
		}
		#endregion

		#region Classes

		public class EmailInformation
		{
            //private string _smtpServer = "cypwebmail.brierleyweb.com";
            private string _smtpServer;
            public string SmtpServer
			{
				get 
                {
                    if (string.IsNullOrWhiteSpace(_smtpServer))
                    {
                        _smtpServer = Brierley.FrameWork.Common.Config.LWConfigurationUtil.GetConfigurationValue("SmtpServer");
                    }
                    return _smtpServer; 
                }
				set { _smtpServer = value; }
			}

			private string _senderEmail;
			public string SenderEmail
			{
				get { return _senderEmail; }
				set { _senderEmail = value; }
			}

			private string _senderDisplayName = "Brierley DAP Administrator";
			public string SenderDisplayName
			{
				get { return _senderDisplayName; }
				set { _senderDisplayName = value; }
			}

			private string _recepientEmail;
			public string RecepientEmail
			{
				get { return _recepientEmail; }
				set { _recepientEmail = value; }
			}

			public string Subject { get; set; }

            public InterceptorDirective EmailInterceptor { get; set; }
		}

		public class InterceptorDirective
		{
			public bool ReuseForFile { get; set; }
			public NameValueCollection InterceptorParms { get; set; }
			public string InterceptorAssemlyName { get; set; }
			public string InterceptorTypeName { get; set; }
		}

		public class OperationResponseDirective
		{
            public bool ReloadMember { get; set; }

			private List<string> attributeSetsToLoad = new List<string>();
			public List<string> AttributeSetsToLoad
			{
				get { return attributeSetsToLoad; }
			}
			public LWIntegrationConfig.InterceptorDirective ResponseHelperDirective { get; set; }
		}

		public class Directive
		{
		}

		public class MemberLoadDirectives
		{
			public bool ExceptionIfFound { get; set; }
			private bool exceptionIfNotFound = false;
			public bool ExceptionIfNotFound
			{
				get { return exceptionIfNotFound; }
				set { exceptionIfNotFound = value; }
			}

			public bool HandleMemberNotFoundInInterceptor { get; set; }

			private List<MemberLoadDirective> memberLoadDirectives = new List<MemberLoadDirective>();
			public List<MemberLoadDirective> LoadDirectives
			{
				get { return memberLoadDirectives; }
				set { memberLoadDirectives = value; }
			}
		}

		public class MemberLoadDirective
		{
			public MemberLoadMethod Method { get; set; }
			public string ValuePath { get; set; }
		}

		public class IfAttribute
		{
			public string Name { get; set; }
			public string Value { get; set; }
			public string FindBy { get; set; }
			public CreateType Action { get; set; }
		}

		public class AttributeSetDirective : Directive
		{
			public string Name { get; set; }
			public bool AddMissingPropertyAsTransient { get; set; }
			public bool UpdateLastActivityDate { get; set; }
		}

		public class AttributeSetCreateDirective : AttributeSetDirective
		{
			public string IfPresent { get; set; }

			private IList markTrueAttributes = new ArrayList();
			public void AddMarkTrueAttribute(string attName)
			{
				markTrueAttributes.Add(attName);
			}

			public IList MarkTrueAttributes
			{
				get { return markTrueAttributes; }
			}

			public bool LoadExisting { get; set; }
		}

		public class DynamicScript
		{
			private string _name;
			public string Name
			{
				get { return _name; }
				set { _name = value; }
			}

			private string _inParmName;
			public string InParmName
			{
				get { return _inParmName; }
				set { _inParmName = value; }
			}

			private string _code;
			public string Code
			{
				get { return _code; }
				set { _code = value; }
			}
		}

		//public class AttributeSetSkipDirective : AttributeSetDirective
		//{            
		//}

		public class AttributeSetUpdateDirective : AttributeSetDirective
		{
			public AttributeSetFindMethodEnum FindMethod { get; set; }
			public string FindValue { get; set; }

			//public string FindBy { get; set; }

			private bool createIfNotFound = true;
			public bool CreateIfNotFound
			{
				get { return createIfNotFound; }
				set { createIfNotFound = value; }
			}
		}

		class AttributeSetModifyDirective : AttributeSetDirective
		{
			private IList ifAttributes = new ArrayList();
			public void AddIfAttribute(IfAttribute att)
			{
				ifAttributes.Add(att);
			}
			private IList IfAttributes
			{
				get { return ifAttributes; }
			}
		}

        public class AttributeSetAddDirective : AttributeSetDirective
        {
            private string Name;
            private VirtualCardSearchType virtualCardType;
            public VirtualCardSearchType VirtualCardType
            {
                get { return virtualCardType; }
                set { virtualCardType = value; }
            }

        }
		#endregion

		#region Protected Methods

		protected InterceptorDirective ProcessInterceptorAssembly(XElement node)
		{
			InterceptorDirective directive = new InterceptorDirective();
			XAttribute att = node.Attribute("ReuseForFile");
			directive.ReuseForFile = true;
			if (att != null && !string.IsNullOrEmpty(att.Value))
			{
				directive.ReuseForFile = bool.Parse(att.Value);
			}

			foreach (XElement childNode in node.Nodes().OfType<XElement>())
			{
				if (childNode.Name.LocalName == "InterceptorAssemlyName")
				{
					directive.InterceptorAssemlyName = childNode.Value;
				}
				else if (childNode.Name.LocalName == "InterceptorTypeName")
				{
					directive.InterceptorTypeName = childNode.Value;
				}
				else if (childNode.Name.LocalName == "InterceptorParms")
				{
					NameValueCollection nv = directive.InterceptorParms;
					if (nv == null)
					{
						nv = new NameValueCollection();
					}
					foreach (XElement parmNode in childNode.Nodes().OfType<XElement>())
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
					directive.InterceptorParms = nv;
				}
			}
			return directive;
		}

		protected MemberLoadDirectives ProcesMemberLoadDirectives(XElement loadDirectiveNode)
		{
			MemberLoadDirectives loadDirectives = new MemberLoadDirectives();

			XAttribute att = loadDirectiveNode.Attribute("ExceptionIfNotFound");
			if (att != null)
			{
				loadDirectives.ExceptionIfNotFound = bool.Parse(att.Value);
			}
			else
			{
				loadDirectives.ExceptionIfNotFound = false;
			}

			loadDirectives.HandleMemberNotFoundInInterceptor = false;
			att = loadDirectiveNode.Attribute("HandleMemberNotFoundInInterceptor");
			if (att != null)
			{
				loadDirectives.HandleMemberNotFoundInInterceptor = bool.Parse(att.Value);
			}

			att = loadDirectiveNode.Attribute("ExceptionIfFound");
			if (att != null)
			{
				loadDirectives.ExceptionIfFound = bool.Parse(att.Value);
			}
			else
			{
				loadDirectives.ExceptionIfFound = true;
			}

			if (loadDirectives.ExceptionIfFound && loadDirectives.ExceptionIfNotFound)
			{
				string msg = "Invalid attributes for MemberLoadDirective.  Both attributes cannot be true";
				throw new LWIntegrationCfgException(msg);
			}

			foreach (XElement directiveNode in loadDirectiveNode.Elements(loadDirectiveNode.Name.Namespace + "LoadDirective"))
			{
				MemberLoadDirective directive = ProcessMemberLoadDirective(directiveNode);
				loadDirectives.LoadDirectives.Add(directive);
			}

			return loadDirectives;
		}

		protected EmailInformation ProcessEmailInformation(XElement emailNode)
		{
			EmailInformation emailInfo = null;
			if (emailNode.Name.LocalName == "EmailInformation")
			{
				emailInfo = new EmailInformation();
				foreach (XElement cnode in emailNode.Nodes().OfType<XElement>())
				{
					if (cnode.Name.LocalName == "SMTPServer")
					{
						emailInfo.SmtpServer = cnode.Value;
					}
					else if (cnode.Name.LocalName == "SenderEmail")
					{
						emailInfo.SenderEmail = cnode.Value;
					}
					else if (cnode.Name.LocalName == "SenderDisplayName")
					{
						emailInfo.SenderDisplayName = cnode.Value;
					}
					else if (cnode.Name.LocalName == "RecepientEmail")
					{
						emailInfo.RecepientEmail = cnode.Value;
					}
					else if (cnode.Name.LocalName == "Subject")
					{
						emailInfo.Subject = cnode.Value;
					}
                    else if (cnode.Name.LocalName == "EmailInterceptor")
                    {
                        emailInfo.EmailInterceptor = ProcessInterceptorAssembly(cnode);
                    }
				}
			}
			return emailInfo;
		}

		protected void ProcessDynamicScripts(Dictionary<string, DynamicScript> scripts, XElement scriptsNode)
		{
			foreach (XElement node in scriptsNode.Nodes().OfType<XElement>())
			{
				if (node.Name.LocalName == "Script")
				{
					DynamicScript script = new DynamicScript();
					XAttribute att = node.Attribute("Name");
					script.Name = att.Value;
					att = node.Attribute("InParmName");
					if (att != null)
					{
						script.InParmName = att.Value;
					}
					script.Code = node.Value;
					scripts.Add(script.Name, script);
				}
			}
		}

		protected IfAttribute ProcessifAttribute(XElement ifNode)
		{
			IfAttribute ifAtt = new IfAttribute();
			ifAtt.Name = ifNode.Attribute("Name").Value;
			ifAtt.Value = ifNode.Attribute("Value").Value;
			ifAtt.Action = (CreateType)Enum.Parse(typeof(CreateType), ifNode.Attribute("Action").Value);
			if (ifNode.Attribute("FindBy") != null)
			{
				ifAtt.FindBy = ifNode.Attribute("FindBy").Value;
			}
			return ifAtt;
		}


        protected AttributeSetDirective ProcessAttributeSetAddDirective(XElement directiveNode)
        {
            //Not sure what to do here for the directive Node.  What other configurations do we need?
            AttributeSetAddDirective directive = new AttributeSetAddDirective();

            directive.Name = directiveNode.Attribute("Name").Value;

            string vcSearchType = directiveNode.Attribute("VirtualCardSearchType") != null ? directiveNode.Attribute("VirtualCardSearchType").Value : null;

            VirtualCardSearchType searchCard = VirtualCardSearchType.PrimaryCard;

            switch (vcSearchType)
            {
                case "PrimaryCard":
                    searchCard = VirtualCardSearchType.PrimaryCard;
                    break;
                case "MostRecentRegistered":
                    searchCard = VirtualCardSearchType.MostRecentRegistered;
                    break;
                case "MostRecentIssued":
                    searchCard = VirtualCardSearchType.MostRecentIssued;
                    break;
                case "EarliestRegistered":
                    searchCard = VirtualCardSearchType.EarliestRegistered;
                    break;
                case "EarliestIssued":
                    searchCard = VirtualCardSearchType.EarliestIssued;
                    break;
                default:
                    searchCard = VirtualCardSearchType.PrimaryCard;
                    break;
            }

            directive.VirtualCardType = searchCard;
            
            return directive;
        }

		protected AttributeSetDirective ProcessAttributeSetCreateDirective(XElement directiveNode)
		{
			AttributeSetCreateDirective directive = new AttributeSetCreateDirective();
			directive.Name = directiveNode.Attribute("Name").Value;
			directive.IfPresent = directiveNode.Attribute("IfPresent").Value;
			foreach (XElement markTrueNode in directiveNode.Elements(directiveNode.Name.Namespace + "MarkTrue"))
			{
				directive.AddMarkTrueAttribute(markTrueNode.Value);
			}
			XAttribute att = directiveNode.Attribute("LoadExisting");
			directive.LoadExisting = false;
			if (att != null)
			{
				directive.LoadExisting = bool.Parse(att.Value);
			}
			att = directiveNode.Attribute("AddMissingPropertyAsTransient");
			directive.AddMissingPropertyAsTransient = false;
			if (att != null)
			{
				directive.AddMissingPropertyAsTransient = bool.Parse(att.Value);
			}
			att = directiveNode.Attribute("UpdateLastActivityDate");
			directive.UpdateLastActivityDate = false;
			if (att != null)
			{
				directive.UpdateLastActivityDate = bool.Parse(att.Value);
			}
			return directive;
		}

		protected AttributeSetDirective ProcessAttributeSetUpdateDirective(XElement directiveNode)
		{
			AttributeSetUpdateDirective directive = new AttributeSetUpdateDirective();
			directive.Name = directiveNode.Attribute("Name").Value;
			directive.FindMethod = (AttributeSetFindMethodEnum)Enum.Parse(typeof(AttributeSetFindMethodEnum), directiveNode.Attribute("FindMethod").Value);
			if (directiveNode.Attribute("FindValue") != null)
			{
				directive.FindValue = directiveNode.Attribute("FindValue").Value;
			}
			//directive.FindBy = directiveNode.Attribute("FindBy").Value;
			if (directiveNode.Attribute("CreateIfNotFound") != null)
			{
				directive.CreateIfNotFound = bool.Parse(directiveNode.Attribute("CreateIfNotFound").Value);
			}
			XAttribute att = directiveNode.Attribute("AddMissingPropertyAsTransient");
			directive.AddMissingPropertyAsTransient = false;
			if (att != null)
			{
				directive.AddMissingPropertyAsTransient = bool.Parse(att.Value);
			}
			att = directiveNode.Attribute("UpdateLastActivityDate");
			directive.UpdateLastActivityDate = false;
			if (att != null)
			{
				directive.UpdateLastActivityDate = bool.Parse(att.Value);
			}
			return directive;
		}

		//protected AttributeSetDirective ProcessAttributeSetSkipDirective(XElement directiveNode)
		//{
		//    AttributeSetSkipDirective directive = new AttributeSetSkipDirective();
		//    directive.Name = directiveNode.Attribute("Name").Value;            
		//    return directive;
		//}

		protected AttributeSetDirective ProcessAttributeSetModifyDirective(XElement directiveNode)
		{
			AttributeSetModifyDirective directive = new AttributeSetModifyDirective();
			directive.Name = directiveNode.Attribute("Name").Value;
			foreach (XElement ifNode in directiveNode.Elements(directiveNode.Name.Namespace + "IfAttribute"))
			{
				IfAttribute ifAtt = new IfAttribute();
				ifAtt.Name = ifNode.Attribute("Name").Value;
				ifAtt.Value = ifNode.Attribute("Value").Value;
				ifAtt.Action = (CreateType)Enum.Parse(typeof(CreateType), ifNode.Attribute("Action").Value);
				if (ifNode.Attribute("FindBy") != null)
				{
					ifAtt.FindBy = ifNode.Attribute("FindBy").Value;
				}
				directive.AddIfAttribute(ifAtt);
			}
			return directive;
		}

		#endregion

		#region Private Helpers

		private MemberLoadDirective ProcessMemberLoadDirective(XElement loadDirectiveNode)
		{
			MemberLoadDirective directive = new MemberLoadDirective();
			XAttribute mAtt = loadDirectiveNode.Attribute("Method");
			directive.Method = (MemberLoadMethod)Enum.Parse(typeof(MemberLoadMethod), mAtt.Value);
			if (directive.Method != MemberLoadMethod.UseInterceptor)
			{
				XAttribute vAtt = loadDirectiveNode.Attribute("ValuePath");
				directive.ValuePath = vAtt.Value;
			}
			return directive;
		}

		#endregion

		#region Abstract Methods
		public abstract string GetDateConversionFormat();
		public abstract string GetPasswordChangedEmailName();
		#endregion
	}
}
