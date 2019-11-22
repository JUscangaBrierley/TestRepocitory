//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.Email;

namespace Brierley.FrameWork.LWIntegration.Util
{
	public static class LWIntegrationUtilities
	{
		#region Fields
		private static string _className = "LWIntegrationUtilities";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		#endregion

		#region General Helper Methods

		public static Member LoadExistingMember(
			LWIntegrationConfig config,
			XElement memberNode,
			LWIntegrationConfig.MemberLoadMethod loadMethod,
			string valuePath,
			IInboundInterceptor interceptor,
			ref string errorStr)
		{
			string methodName = "GetMember";

			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{

				_logger.Debug(_className, methodName, "Processing directives to obtain a member.");
				Member member = null;

				if (loadMethod == LWIntegrationConfig.MemberLoadMethod.UseInterceptor)
				{
					if (interceptor != null)
					{
						_logger.Debug(_className, methodName, "Loading a member using the interceptor.");
						try
						{
							member = interceptor.LoadMember(config, memberNode);
							if (member == null)
							{
								_logger.Debug(_className, methodName, "No member found by interceptor.");
							}
						}
						catch (Exception ex)
						{
							_logger.Error(_className, methodName, "Error thrown by LoadMember method of interceptor.", ex);
							throw;
						}
					}
					else
					{
						_logger.Debug(_className, methodName, "Load directive UseInterceptor specified but no interceptor found.");
					}
				}
				else
				{
					string searchValue = GetValueByPath(memberNode, valuePath);
					if (!string.IsNullOrEmpty(searchValue))
					{
						if (loadMethod == LWIntegrationConfig.MemberLoadMethod.IpCode)
						{
							_logger.Debug(_className, methodName, "Loading a member by Ipcode: " + searchValue);
							member = service.LoadMemberFromIPCode(long.Parse(searchValue));
						}
						else if (loadMethod == LWIntegrationConfig.MemberLoadMethod.CertificateNumber)
						{
							_logger.Debug(_className, methodName, "Loading a member by certificate number: " + searchValue);
							member = service.LoadFromCertificateNumber(searchValue);
						}
						else if (loadMethod == LWIntegrationConfig.MemberLoadMethod.AlternateId)
						{
							_logger.Debug(_className, methodName, "Loading a member by AlternateId: " + searchValue);
							member = service.LoadMemberFromAlternateID(searchValue);
						}
						else if (loadMethod == LWIntegrationConfig.MemberLoadMethod.EmailAddress)
						{
							_logger.Debug(_className, methodName, "Loading a member by EmailAddress: " + searchValue);
							member = service.LoadMemberFromEmailAddress(searchValue);
						}
						else if (loadMethod == LWIntegrationConfig.MemberLoadMethod.UserName)
						{
							_logger.Debug(_className, methodName, "Loading a member by Username: " + searchValue);
							member = service.LoadMemberFromUserName(searchValue);
						}
						else if (loadMethod == LWIntegrationConfig.MemberLoadMethod.LoyaltyIdNumber)
						{
							_logger.Debug(_className, methodName, "Loading a member by LoyaltyIdNumber: " + searchValue);
							member = service.LoadMemberFromLoyaltyID(searchValue);
						}
						if (member == null)
						{
							_logger.Debug(_className, methodName, "No member found for " + searchValue);
							if (!string.IsNullOrEmpty(errorStr))
							{
								errorStr += ",";
							}
							errorStr += valuePath + " = " + searchValue;
						}
					}
					else  // empty search value
					{
						string err = "Empty " + valuePath + " in input Xml for Member lookup.";
						_logger.Error(_className, methodName, err);
						throw new LWIntegrationException(err) { ErrorCode = 9994 };
					}
				}
				return member;
			}
		}

        public static Member GetMember(
            LWIntegrationConfig config,
            XElement memberNode, LWIntegrationConfig.MemberLoadDirectives memberLoadDirectives,
            IInboundInterceptor interceptor
            )
        {
            string methodName = "GetMember";

            _logger.Debug(_className, methodName, "Processing directives to obtain a member.");
            Member member = null;
            string errorStr = "";

            if (memberLoadDirectives.LoadDirectives != null && memberLoadDirectives.LoadDirectives.Count > 0)
            {
                foreach (LWIntegrationConfig.MemberLoadDirective directive in memberLoadDirectives.LoadDirectives)
                {
                    member = LoadExistingMember(config, memberNode, directive.Method, directive.ValuePath, interceptor, ref errorStr);
                    if (member != null)
                    {
                        break;
                    }
                }
            }
            if (member == null)
            {
                if (memberLoadDirectives.HandleMemberNotFoundInInterceptor)
                {
                    if (interceptor == null)
                    {
                        string err = "No interceptor found to handle MemberNotFound.";
                        _logger.Error(_className, methodName, err);
                        throw new LWIntegrationException(err) { ErrorCode = 3202 };
                    }
                    else
                    {
                        interceptor.HandleMemberNotFound(config, memberNode);
                    }
                }
                else if (memberLoadDirectives.ExceptionIfNotFound)
                {
                    // raise an exception here.
                    string err = "Unable to find existing member by " + errorStr;
                    _logger.Error(_className, methodName, err);
                    throw new LWIntegrationException(err) { ErrorCode = 9992 };
                }
                else
                {
                    _logger.Debug(_className, methodName, "Creating a new instance of member.");
                    member = new Member();
                    member.MemberCreateDate = DateTime.Now;
                }
            }
            else if (memberLoadDirectives.ExceptionIfFound)
            {
                // raise an exception here.
                string err = "A member already exists.";
                _logger.Error(_className, methodName, err);
                throw new LWIntegrationException(err) { ErrorCode = 9991 };
            }
            return member;
        }

        public static Member HandleInitialMemberSatusTransientProperty(Member member)
        {
            string methodName = "HandleInitialMemberSatusTransientProperty";
            
            if (member.IpCode <= 0)
            {
                // first look for new member statis property
                if ( member.HasTransientProperty("initialstatus" ))
                {
                    member.NewStatusEffectiveDate = DateTime.Now;
                    string value = ((string)member.GetTransientProperty("initialstatus")).ToLower();
                    if (value == "disabled")
                    {
                        member.NewStatus = MemberStatusEnum.Disabled;                        
                    }
                    else if (value == "terminated")
                    {
                        member.NewStatus = MemberStatusEnum.Terminated; 
                    }
                    else if (value == "locked")
                    {
                        member.NewStatus = MemberStatusEnum.Locked;
                    }
                    else if (value == "nonmember")
                    {
                        member.NewStatus = MemberStatusEnum.NonMember;
                    }
                    else if (value == "merged")
                    {
                        member.NewStatus = MemberStatusEnum.Merged;
                    }
                    else if (value == "preenrolled")
                    {
                        member.NewStatus = MemberStatusEnum.PreEnrolled;
                    }
                    _logger.Trace(_className, methodName, "A  new member is being created with the initial status set to " + value);
                }
                else if (member.HasTransientProperty("nonmember" ))
                {
                    bool isNonMember = true;
                    object value = member.GetTransientProperty("nonmember");
                    if (value is string)
                    {
                        isNonMember = bool.Parse((string)value);
                    }
                    else if (value is bool)
                    {
                        isNonMember = (bool)value;
                    }
                    if (isNonMember)
                    {
                        member.NewStatus = MemberStatusEnum.NonMember;
                        member.NewStatusEffectiveDate = DateTime.Now;
                        _logger.Trace(_className, methodName, "A non-member is being created with the transient property set to true.");
                    }
                }
            }
            
            return member;
        }

        //public static Member HandleNonMemberTransientProperty(Member member)
        //{
        //    string methodName = "HandleNonMemberTransientProperty";

        //    // Handle NonMember transient property
        //    if (member.IpCode <= 0 && member.HasTransientProperty("nonmember"))
        //    {
        //        bool isNonMember = true;
        //        object value = member.GetTransientProperty("nonmember");
        //        if (value is string)
        //        {
        //            isNonMember = bool.Parse((string)value);
        //        }
        //        else if (value is bool)
        //        {
        //            isNonMember = (bool)value;
        //        }
        //        if (isNonMember)
        //        {
        //            member.NewStatus = MemberStatusEnum.NonMember;
        //            member.NewStatusEffectiveDate = DateTime.Now;
        //            _logger.Trace(_className, methodName, "A non-member is being created with the transient property set to true.");
        //        }
        //    }

        //    return member;
        //}

		private static bool IsNullableEnum(Type t)
		{
			return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && t.GetGenericArguments()[0].IsEnum;
		}

		private static object GetFieldValue(PropertyInfo info, Member member)
		{
			object value = null;
			try
			{
				value = info.GetValue(member, null);
			}
			catch (System.ArgumentException)
			{
				FieldInfo finfo = member.GetType().GetField(info.Name.ToLower(), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (finfo != null)
				{
					value = finfo.GetValue(member);
				}				
			}
			return value;
		}
		
		public static void LoadMemberAttributes(LWIntegrationConfig config, Member member, XElement memberNode, bool trimStrings, string dateConversionFormat, bool checkForChangedValues)
		{
			string methodName = "LoadMemberAttributes";

			_logger.Debug(_className, methodName, "Loading member attributes.");

            string password = string.Empty;
			foreach (XAttribute attribute in memberNode.Attributes())
			{
				if (attribute.Name.LocalName == "IpCode" ||
					attribute.Name.LocalName == "MemberStatus" ||
					attribute.Name.LocalName == "MemberCreateDate" ||
					attribute.Name.LocalName == "ResetCode" ||
					attribute.Name.LocalName == "ResetCodeDate" ||
                    attribute.Name.LocalName == "Salt" )
				{
					// we do not want to change these properties.
					continue;
				}
                else if (attribute.Name.LocalName == "Password")
                {
                    password = GetAttributeValue(trimStrings, attribute);
                    continue;
                }
				string attValue = GetAttributeValue(trimStrings, attribute);
				PropertyInfo info = member.GetType().GetProperty(attribute.Name.LocalName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				if (info != null)
				{
                    object oldValue = GetFieldValue(info, member);
                    if (Type.GetType(info.PropertyType.FullName) == typeof(DateTime) ||
                                Type.GetType(info.PropertyType.FullName) == typeof(DateTime?))
                    {
                        DateTime? newValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            newValue = config.HasISO8601CompliantDateStringFormat
                                ? DateTimeUtil.ConvertISO8601StringToDate(attValue, config.IgnoreTimeZoneOffset)
                                : DateTimeUtil.ConvertStringToDate(dateConversionFormat, attValue);
                        }
                        if (!checkForChangedValues || HasPropertyValueChanged(Type.GetType(info.PropertyType.FullName), oldValue, newValue))
                        {
                            info.SetValue(member, newValue, null);
                        }
                    }
                    else if (Type.GetType(info.PropertyType.FullName) == typeof(Int64) ||
                        Type.GetType(info.PropertyType.FullName) == typeof(Int64?))
                    {
                        Int64? newValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            newValue = Convert.ToInt64(attValue);
                        }
                        if (!checkForChangedValues || HasPropertyValueChanged(Type.GetType(info.PropertyType.FullName), oldValue, newValue))
                        {
                            info.SetValue(member, newValue, null);
                        }
                    }
                    else if (Type.GetType(info.PropertyType.FullName) == typeof(Boolean) ||
                    Type.GetType(info.PropertyType.FullName) == typeof(Boolean?))
                    {
                        Boolean? newValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            newValue = Convert.ToBoolean(attValue);
                        }
                        if (!checkForChangedValues || HasPropertyValueChanged(Type.GetType(info.PropertyType.FullName), oldValue, newValue))
                        {
                            info.SetValue(member, newValue, null);
                        }
                    }
                    else if (Type.GetType(info.PropertyType.FullName) == typeof(String))
                    {
                        if (!checkForChangedValues || HasPropertyValueChanged(Type.GetType(info.PropertyType.FullName), oldValue, attValue))
                        {
                            info.SetValue(member, attValue, null);
                        }
                    }
                    else if (info.PropertyType.IsEnum)
                    {
                        Type t = Type.GetType(info.PropertyType.AssemblyQualifiedName);
                        object newValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            newValue = Enum.Parse(t, attValue);
                        }
                        if (!checkForChangedValues || HasPropertyValueChanged(t, oldValue, newValue))
                        {
                            info.SetValue(member, newValue, null);
                        }
                    }
                    else if (IsNullableEnum(info.PropertyType))
                    {
                        Type t = info.PropertyType.GetGenericArguments()[0];
                        object newValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            newValue = Enum.Parse(t, attValue);
                        }
                        if (!checkForChangedValues || HasPropertyValueChanged(t, oldValue, newValue))
                        {
                            info.SetValue(member, newValue, null);
                        }
                    }
                    else
                    {
                        if (!checkForChangedValues || HasPropertyValueChanged(Type.GetType(info.PropertyType.FullName), oldValue, attValue))
                        {
                            info.SetValue(member, attValue, null);
                        }
                    }
				}
				else
				{
					// This attribute does not have a corresponding property in the member.
					// Add it as a transient property.
					_logger.Debug(_className, methodName, "Setting transient property " + attribute.Name + " on the member.");
					member.UpdateTransientProperty(attribute.Name.LocalName, attValue);
				}
			}

            // handle username and password
			if (!string.IsNullOrEmpty(password))
			{
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					service.ChangeMemberPassword(member, password, false);

					if (member.IpCode > 0)
					{
						string passwordChangedEmailName = config.GetPasswordChangedEmailName();
						if (!string.IsNullOrEmpty(passwordChangedEmailName))
						{
							try
							{
								// send password changed email
								using (var emailService = LWDataServiceUtil.EmailServiceInstance())
								{
									EmailDocument emailDoc = emailService.GetEmail(passwordChangedEmailName);
									if (emailDoc != null)
									{
										ITriggeredEmail email = TriggeredEmailFactory.Create(emailDoc.Id);
										if (email != null)
										{
											email.SendAsync(member).Wait();
											_logger.Debug(_className, methodName, "Password change email was sent to: " + member.PrimaryEmailAddress);
										}
										else
										{
											_logger.Error(_className, methodName, "Unable to resolve email for ID: " + emailDoc.Id);
										}
									}
									else
									{
										_logger.Error(_className, methodName, "Unable to resolve emailDoc for name: " + passwordChangedEmailName);
									}
								}
							}
							catch (Exception ex)
							{
								_logger.Error(_className, methodName, "Error sending password changed email:" + ex.Message, ex);
							}
						}
						else
						{
							_logger.Debug(_className, methodName, "No password change email is configured.");
						}
					}
				}
			}
            
			// Perform some basic validation on the member			
			if (member.MemberCloseDate != null && member.MemberCreateDate > member.MemberCloseDate)
			{
				throw new LWIntegrationException("Member close date is prior to member create date.") { ErrorCode = 9978 };
			}

            member = LWIntegrationUtilities.HandleInitialMemberSatusTransientProperty(member);			
		}

        public static MemberSocNet ProcessSocnetNode(XElement asNode, bool trimStrings)
        {
            MemberSocNet socNet = null;
            if (asNode.Name.LocalName == "SocialNetworkHandle")
            {
                socNet = new MemberSocNet();
                XAttribute att = asNode.Attribute("ProviderType");
                if (att == null || string.IsNullOrWhiteSpace(att.Value))
                {
                    throw new LWIntegrationException(string.Format("Required ProviderType attribute is missing on SocialNetworkHandle.", asNode.Name.LocalName)) { ErrorCode = 10004 };
                }
                else
                {
                    socNet.ProviderType = (SocialNetworkProviderType)Enum.Parse(typeof(SocialNetworkProviderType), GetAttributeValue(trimStrings, att));
                }
                att = asNode.Attribute("ProviderUID");
                if (att == null || string.IsNullOrWhiteSpace(att.Value))
                {
                    throw new LWIntegrationException(string.Format("Required ProviderUID attribute is missing on SocialNetworkHandle.", asNode.Name.LocalName)) { ErrorCode = 10005 };
                }
                else
                {
                    socNet.ProviderUID = GetAttributeValue(trimStrings, att);
                }
            }
            else
            {
                throw new LWIntegrationException(string.Format("Expected SocialNetworkHandle.  Got {0}", asNode.Name.LocalName)) { ErrorCode = 9978 };
            }
            return socNet;
        }

		public static void ProcessAttributeSet(
			LWIntegrationConfig config,
			IDictionary<string, LWIntegrationConfig.AttributeSetDirective> createDirectives,
			IAttributeSetContainer owner,
			XElement asNode,
			AttributeSetMetaData asDef,
			string dateConversionFormat, bool trimStrings, bool checkForChangedValues)
		{
			string methodName = "ProcessAttributeSet";

			_logger.Debug(_className, methodName, string.Format("Processing attribute set {0}.", asNode.Name));

			LWIntegrationConfig.AttributeSetDirective directive = null;
			IClientDataObject rowToModify = null;
			// first see if the member has any attributes
			if (!asNode.Name.LocalName.Equals("Member") 
                && !asNode.Name.LocalName.Equals("VirtualCard")
                && !asNode.Name.LocalName.Equals("SocialNetworkHandle")
                )
			{
				_logger.Debug(_className, methodName, string.Format("Processing attributes of {0}.", asNode.Name));
				directive = createDirectives.ContainsKey(asNode.Name.LocalName) ? createDirectives[asNode.Name.LocalName] : null;
				if (asNode.Attributes() != null)
				{
					foreach (XAttribute attribute in asNode.Attributes())
					{
						// make sure that the attribute matches the metadata
						var attMeta = asDef.GetAttribute(attribute.Name.LocalName);
						if (attMeta != null)
						{
							// set the attribute name
							bool addMissingPropertyAsTransient = directive != null ? directive.AddMissingPropertyAsTransient : false;
							ProcessAttributeFromXml(config, (IClientDataObject)owner, attribute, dateConversionFormat, trimStrings, checkForChangedValues, addMissingPropertyAsTransient, asDef, attMeta);
						}
						else if (attribute.Name.LocalName == "StatusCode")
						{
							((IClientDataObject)owner).StatusCode = long.Parse(attribute.Value);
							owner.IsDirty = true;
						}
						else
						{
							string attValue = GetAttributeValue(true, attribute);
							if (!string.IsNullOrEmpty(attValue))
							{
								_logger.Debug(_className, methodName,
									string.Format("Adding transient property {0} to {1}", attribute.Name.LocalName, asNode.Name));
								((IClientDataObject)owner).AddTransientProperty(attribute.Name.LocalName, attValue);
							}
						}
					}
                    if (owner != null && owner.IsDirty)
                    {
                        SetLastActivityDate(directive, owner);
                    }
				}
				else
				{
					_logger.Debug(_className, methodName, string.Format("Attribute set {0} has no attributes.", asNode.Name));
				}
			}

			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				_logger.Debug(_className, methodName, string.Format("Processing child attribute sets {0}.", asNode.Name));
				foreach (XElement node in asNode.Elements())
				{
					if (node.NodeType == System.Xml.XmlNodeType.Element)
					{
						if (node.Name.LocalName == "VirtualCard" || node.Name.LocalName == "SocialNetworkHandle")
						{
							continue;
						}
						// found an attribute set                     
						AttributeSetMetaData sasDef = service.GetAttributeSetMetaData(node.Name.LocalName);
						if (sasDef != null)
						{
							directive = createDirectives.ContainsKey(node.Name.LocalName) ? createDirectives[node.Name.LocalName] : null;
							rowToModify = GetMemberAttributeSetToModify(directive, owner, sasDef, node);
							if (rowToModify != null)
							{
								ProcessAttributeSet(config, createDirectives, rowToModify, node, sasDef, dateConversionFormat, trimStrings, checkForChangedValues);
							}
							else
							{
								_logger.Debug(_className, methodName, "No row could be obtained to modify for " + sasDef.Name);
							}
						}
						else
						{
							if (!node.Name.LocalName.Equals("VirtualCard"))
							{
								// this is probably a severe error
								string msg = "Could not find metadata definition for " + node.Name.LocalName;
								_logger.Error(_className, methodName, msg);
								throw new LWIntegrationException(msg) { ErrorCode = 9987 };
							}
						}
					}
				}
			}
		}

        public static void ProcessVirtualCards(LWIntegrationConfig config, XElement memberNode, Member member, IDictionary<string, LWIntegrationConfig.AttributeSetDirective> CreateDirectives, bool trimStrings, bool checkForChangedValues)
        {
            string methodName = "ProcessVirtualCards";

            IEnumerable<XElement> vcList = memberNode.Elements("VirtualCard");
            foreach (XElement vcNode in vcList)
            {
                bool isPrimary = false;
                // will probably always be one
                VirtualCard vc = null;
                LWIntegrationConfig.AttributeSetDirective directive =
                    CreateDirectives.ContainsKey(vcNode.Name.LocalName) ?
                    CreateDirectives[vcNode.Name.LocalName] : null;
                if (directive == null)
                {
                    vc = LWIntegrationUtilities.CreateNewVirtualCard(member);
                }
                else if (directive.GetType().Name.Equals("AttributeSetCreateDirective"))
                {
                    LWIntegrationConfig.AttributeSetCreateDirective crtDirective =
                        (LWIntegrationConfig.AttributeSetCreateDirective)directive;
                    XAttribute attNode = vcNode.Attribute(crtDirective.IfPresent);
                    if (attNode != null && !string.IsNullOrEmpty(attNode.Value))
                    {
                        vc = LWIntegrationUtilities.CreateNewVirtualCard(member);
                    }
                    System.Collections.IList list = crtDirective.MarkTrueAttributes;
                    foreach (string mt in list)
                    {
                        if (mt != "IsPrimary")
                        {
                            string err = "IsPrimary is the only VirtualCard attribute that can be marked true";
                            throw new LWIntegrationException(err) { ErrorCode = 9988 };
                        }
                        isPrimary = true;
                    }
                }
                else if (directive.GetType().Name.Equals("AttributeSetUpdateDirective"))
                {
                    LWIntegrationConfig.AttributeSetUpdateDirective updDirective = (LWIntegrationConfig.AttributeSetUpdateDirective)directive;
                    // find the existing virtual card                 
                    if (member.LoyaltyCards.Count == 0)
                    {
                        if (updDirective.CreateIfNotFound)
                        {
                            vc = LWIntegrationUtilities.CreateNewVirtualCard(member);
                        }
                        else
                        {
                            string err = "Could not find virtual card to update in input Xml for VirtualCard update.";
                            throw new LWIntegrationException(err) { ErrorCode = 9986 };
                        }
                    }
                    else
                    {
                        vc = LWIntegrationUtilities.FindVirtualCardInList(member, updDirective, vcNode);
                        if (vc == null)
                        {
                            if (updDirective.CreateIfNotFound)
                            {
                                _logger.Trace(_className, methodName,
                                        string.Format("Could not find Virtual card required in the existing virtual cards for member with ipcode {0}.  Provided directive requires its creation.",
                                        member.MyKey));
                                vc = LWIntegrationUtilities.CreateNewVirtualCard(member);
                            }
                            else
                            {
                                string err = "Could not find a virtual card to update."/* + updDirective.FindBy + " = " + findAttNode.Value*/;
                                throw new LWIntegrationException(err) { ErrorCode = 9986 };
                            }
                        }
                    }
                }
                if (vc != null)
                {
                    switch (vc.Status)
                    {
                        case VirtualCardStatusType.Cancelled:
                            LWIntegrationUtilities.LoadLoyaltyCardAttributes(config, vc, vcNode, config.GetDateConversionFormat(), trimStrings, checkForChangedValues);
                            break;
                        case VirtualCardStatusType.Replaced:
                            VirtualCard rvc = member.GetLoyaltyCard((long)vc.LinkKey);
                            if (rvc == null)
                            {
                                string err = string.Format("Loyalty card with loyalty id # {0} has been replaced but failed to retrieve the new card.", vc.LoyaltyIdNumber);
                                throw new LWIntegrationException(err) { ErrorCode = 9981 };
                            }
                            else
                            {
                                vc = rvc;
                            }
                            LWIntegrationUtilities.ProcessAttributeSet(config, CreateDirectives, vc, vcNode, null, config.GetDateConversionFormat(), trimStrings, checkForChangedValues);
                            break;
                        case VirtualCardStatusType.Active:
                            LWIntegrationUtilities.LoadLoyaltyCardAttributes(config, vc, vcNode, config.GetDateConversionFormat(), trimStrings, checkForChangedValues);
                            LWIntegrationUtilities.ProcessAttributeSet(config, CreateDirectives, vc, vcNode, null, config.GetDateConversionFormat(), trimStrings, checkForChangedValues);
                            if (isPrimary && !vc.IsPrimary)
                            {
                                // WS - 02/03/2012
                                // the configuration says that the card being added should be made primary
                                // but the incoming message did not have IsPrimary marked to true. 
                                member.MarkVirtualCardAsPrimary(vc);
                            }
                            break;
                    }
                }
            }
        }

        public static void LoadLoyaltyCardAttributes(LWIntegrationConfig config, VirtualCard vc, XElement vcNode, string dateConversionFormat, bool trimStrings, bool checkForChangedValues)
		{
			string methodName = "LoadLoyaltyCardAttributes";

			_logger.Debug(_className, methodName, "Loading loyalty card attributes.");

			foreach (XAttribute attribute in vcNode.Attributes())
			{
                string attValue = GetAttributeValue(trimStrings, attribute);

				if (attribute.Name.LocalName.Equals("VcKey") ||
					attribute.Name.LocalName.Equals("IpCode") ||
					attribute.Name.LocalName.Equals("LinkKey") ||
					attribute.Name.LocalName.Equals("Status") ||
					attribute.Name.LocalName.Equals("CreateDate") ||
                    attribute.Name.LocalName.Equals("UpdateDate")
					)
				{
					continue;
				}
				else if (attribute.Name.LocalName.Equals("LoyaltyIdNumber"))
				{
                    if (!checkForChangedValues || HasPropertyValueChanged(typeof(string), vc.LoyaltyIdNumber, attValue))
                    {
                        vc.LoyaltyIdNumber = GetAttributeValue(trimStrings, attribute);
                    }					
				}
				else if (attribute.Name.LocalName.Equals("CardType"))
				{
                    long? newValue = null;
                    if (!string.IsNullOrEmpty(attValue))
                    {
                        newValue = long.Parse(attValue);
                    }
                    if (!checkForChangedValues || HasPropertyValueChanged(typeof(long), vc.CardType, newValue))
                    {
                        vc.CardType = (long)newValue;
                    }                    
				}
                else if (attribute.Name.LocalName.Equals("DateIssued"))
                {
                    if (!string.IsNullOrEmpty(attribute.Value))
                    {
                        DateTime? newValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            newValue = config.HasISO8601CompliantDateStringFormat
                            ? DateTimeUtil.ConvertISO8601StringToDate(GetAttributeValue(trimStrings, attribute), config.IgnoreTimeZoneOffset)
                            : DateTimeUtil.ConvertStringToDate(dateConversionFormat, GetAttributeValue(trimStrings, attribute));
                        }
                        if (!checkForChangedValues || HasPropertyValueChanged(typeof(DateTime), vc.DateIssued, newValue))
                        {
                            vc.DateIssued = (DateTime)newValue;
                        }
                    }
                }
                else if (attribute.Name.LocalName.Equals("DateRegistered"))
                {
                    if (!string.IsNullOrEmpty(attribute.Value))
                    {
                        DateTime? newValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            newValue = config.HasISO8601CompliantDateStringFormat
                            ? DateTimeUtil.ConvertISO8601StringToDate(GetAttributeValue(trimStrings, attribute), config.IgnoreTimeZoneOffset)
                            : DateTimeUtil.ConvertStringToDate(dateConversionFormat, GetAttributeValue(trimStrings, attribute));
                        }
                        if (!checkForChangedValues || HasPropertyValueChanged(typeof(DateTime), vc.DateRegistered, newValue))
                        {
                            vc.DateRegistered = (DateTime)newValue;
                        }
                    }
                }
				else if (attribute.Name.LocalName.Equals("IsPrimary"))
				{
                    bool? newValue = null;
                    if (!string.IsNullOrEmpty(attValue))
                    {
                        newValue = Convert.ToBoolean(attValue);
                    }
                    if (!checkForChangedValues || HasPropertyValueChanged(typeof(bool), vc.IsPrimary, newValue))
                    {
                        vc.IsPrimary = (bool)newValue;
                    }
				}
				else if (attribute.Name.LocalName.Equals("NewStatus"))
				{
					vc.NewStatus = (VirtualCardStatusType)Enum.Parse(typeof(VirtualCardStatusType), GetAttributeValue(trimStrings, attribute));
					vc.NewStatusEffectiveDate = DateTime.Now;
				}
				else if (attribute.Name.LocalName.Equals("NewStatusEffectiveDate"))
				{
					System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
					vc.NewStatusEffectiveDate = config.HasISO8601CompliantDateStringFormat
						? DateTimeUtil.ConvertISO8601StringToDate(GetAttributeValue(trimStrings, attribute), config.IgnoreTimeZoneOffset)
						: DateTimeUtil.ConvertStringToDate(dateConversionFormat, GetAttributeValue(trimStrings, attribute));
				}
				else if (attribute.Name.LocalName.Equals("StatusChangeReason"))
				{
                    if (!checkForChangedValues || HasPropertyValueChanged(typeof(string), vc.StatusChangeReason, attValue))
                    {
                        vc.StatusChangeReason = attValue;
                    }                    
				}
				else
				{
					// Add as a transient property
					vc.AddTransientProperty(attribute.Name.LocalName, GetAttributeValue(trimStrings, attribute));
				}
			}
		}

		public static VirtualCard CreateNewVirtualCard(Member member)
		{
			string methodName = "createNewVirtualCard";

			_logger.Debug(_className, methodName, "Creating new virtual card.");

			VirtualCard vc = member.CreateNewVirtualCard();
			vc.DateIssued = DateTime.Now;
			vc.DateRegistered = DateTime.Now;			           
			return vc;
		}
		
		public static string GetAttributeValue(bool trimStrings, XAttribute attribute)
		{
			string value = "";
			if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
			{
				if (trimStrings)
				{
					value = attribute.Value.Trim();
				}
				else
				{
					value = attribute.Value;
				}
			}
			return value;			
		}

		public static bool AreEqual(object attValue, string xmlvalue)
		{
			bool equal = false;
			if (attValue.GetType() == typeof(decimal))
			{
				decimal val = decimal.Parse(xmlvalue);
				equal = (val == (decimal)attValue);
			}
            //else if (attValue.GetType() == typeof(double))
            //{
            //    double val = double.Parse(xmlvalue);
            //    equal = (val == (double)attValue);
            //}
            else if (attValue.GetType() == typeof(decimal))
            {
                decimal val = decimal.Parse(xmlvalue);
                equal = (val == (decimal)attValue);
            }
			else if (attValue.GetType() == typeof(long))
			{
				long val = long.Parse(xmlvalue);
				equal = (val == (long)attValue);
			}
			else if (attValue.GetType() == typeof(int))
			{
				int val = int.Parse(xmlvalue);
				equal = (val == (int)attValue);
			}
			else if (attValue.GetType() == typeof(string))
			{
				equal = (xmlvalue == (string)attValue);
			}
			else if (attValue.GetType() == typeof(System.DBNull))
			{
				equal = string.IsNullOrEmpty(xmlvalue);
			}
			else if (attValue.GetType() == typeof(System.DateTime))
			{
				DateTime dt1 = DateTime.Parse(xmlvalue);
				equal = (DateTime.Compare(dt1, (DateTime)attValue) == 0);
			}
			return equal;
		}

		public static void HandleNonExistingMemberException(LWIntegrationConfig config, LWIntegrationException ex, XElement memberNode)
		{
			if (ex.ErrorCode == 9992)
			{
			}
			else
			{
				throw ex;
			}
		}

		#endregion

		#region Message Construction

		public static XElement CreateNewElement(XDocument doc, string elementName, string prefix, string namespaceUri)
		{
			XElement node = new XElement(elementName);
			return node;
		}

		public static void SetAttribute(XElement node, string name, string val)
		{
			XAttribute att = new XAttribute(name, val);
			node.Add(att);
		}

		public static XElement SerializeAttributeSetToXml(LWIntegrationConfig config, XElement root, IClientDataObject aSet, string dateConversionFormat)
		{
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData asDef = aSet.GetMetaData();
				XElement node = CreateNewElement(null, asDef.Name, "", "");
				root.Add(node);
				foreach (AttributeMetaData atm in asDef.Attributes)
				{
					string strVal = GetAttributeValue(config, aSet, atm.Name, dateConversionFormat);
					if (!string.IsNullOrEmpty(strVal))
					{
						SetAttribute(node, atm.Name, strVal);
					}
				}
				// Now process any transient properties
				ICollection properties = aSet.GetTransientPropertyNames();
				if (properties != null && properties.Count > 0)
				{
					string[] propNames = new string[properties.Count];
					properties.CopyTo(propNames, 0);
					foreach (string name in propNames)
					{
						string value = string.Empty;
						object propValue = aSet.GetTransientProperty(name);
						if (propValue.GetType() == typeof(DateTime))
						{
							System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
							value = config.HasISO8601CompliantDateStringFormat
								? DateTimeUtil.ConvertDateToISO8601String((DateTime)propValue, config.IgnoreTimeZoneOffset)
								: DateTimeUtil.ConvertDateToString(dateConversionFormat, (DateTime)propValue);
						}
						else
						{
							value = propValue.ToString();
						}
						SetAttribute(node, name, value);
					}
				}
				Dictionary<string, List<IClientDataObject>> children = aSet.GetChildAttributeSets();
				if (children != null && children.Count > 0)
				{
					foreach (string chName in children.Keys)
					{
						IList<IClientDataObject> chSet = aSet.GetChildAttributeSets(chName);
						foreach (IClientDataObject ch in chSet)
						{
							root = SerializeAttributeSetToXml(config, node, ch, dateConversionFormat);
						}
					}
				}
				return root;
			}
		}

		public static XElement ProcessMemberAttributesToXml(LWIntegrationConfig config, Member member, XElement memberNode, string dateConversionFormat)
		{
            //List<String> memberAttributes = new List<string>() { "IpCode", "MemberCloseDate", "MemberCreateDate", 
            //    "MemberStatus", "BirthDate", "FirstName", "LastName", "MiddleName", "NamePrefix", "NameSuffix",
            //    "PrimaryEmailAddress", "AlternateId", "Username", "Password", "PrimaryPhoneNumber",
            //    "PrimaryPostalCode", "LastActivityDate", "IsEmployee", "ChangedBy" };
            List<String> memberAttributes = new List<string>() { "IpCode", "MemberCloseDate", "MemberCreateDate", 
				"MemberStatus", "BirthDate", "FirstName", "LastName", "MiddleName", "NamePrefix", "NameSuffix",
				"PrimaryEmailAddress", "AlternateId", "Username", "PrimaryPhoneNumber",
				"PrimaryPostalCode", "PreferredLanguage", "LastActivityDate", "IsEmployee", "ChangedBy" };

			PropertyInfo[] pi = member.GetType().GetProperties();
			foreach (PropertyInfo info in pi)
			{
				string attrName = info.Name;
				if (!memberAttributes.Contains(attrName)) continue;
				try
				{
					string attValue = "";
					object val = info.GetValue(member, null);
					if (val != null)
					{
						if (val is DateTime || val is DateTime?)
						{
							System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
							attValue = config.HasISO8601CompliantDateStringFormat
								? DateTimeUtil.ConvertDateToISO8601String((DateTime)val, config.IgnoreTimeZoneOffset)
								: DateTimeUtil.ConvertDateToString(dateConversionFormat, (DateTime)val);
						}
						else if (val is long || val is Int64 || val is Int64?)
						{
							attValue = ((Int64)val).ToString();
						}
						else if (val is bool || val is bool? || val is Boolean || val is Boolean?)
						{
							attValue = ((Boolean)val).ToString();
						}
						else if (info.PropertyType.IsEnum)
						{
							Type t = Type.GetType(info.PropertyType.AssemblyQualifiedName);
							string name = Enum.GetName(t, val);
							if (!string.IsNullOrEmpty(name))
							{
								attValue = name;
							}
							else
							{
								// no value was set for the enum.
								continue;
							}
						}
						else if (IsNullableEnum(info.PropertyType))
						{
							Type t = info.PropertyType.GetGenericArguments()[0];
							string name = Enum.GetName(t, val);
							if (!string.IsNullOrEmpty(name))
							{
								attValue = name;
							}
							else
							{
								// no value was set for the enum.
								continue;
							}
						}
						else
						{
							attValue = (string)val;
						}
					}
					else if (Nullable.GetUnderlyingType(info.PropertyType) != null)
					{
						// attribute is nullable and it's value is null, so omit it from the XML
						continue;
					}
					SetAttribute(memberNode, attrName, attValue);
				}
				catch (Exception ex)
				{
					throw new LWIntegrationException("Error processing member attribute " + attrName, ex) { ErrorCode = 3109 };
				}
			}

			// Now process any transient properties
			ICollection properties = member.GetTransientPropertyNames();
			if (properties != null && properties.Count > 0)
			{
				string[] propNames = new string[properties.Count];
				properties.CopyTo(propNames, 0);
				foreach (string name in propNames)
				{
					string value = string.Empty;
					object propValue = member.GetTransientProperty(name);
					if (propValue.GetType() == typeof(DateTime))
					{
						System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
						value = config.HasISO8601CompliantDateStringFormat
							? DateTimeUtil.ConvertDateToISO8601String((DateTime)propValue, config.IgnoreTimeZoneOffset)
							: DateTimeUtil.ConvertDateToString(dateConversionFormat, (DateTime)propValue);
					}
					else
					{
						value = propValue.ToString();
					}
					SetAttribute(memberNode, name, value);
				}
			}
			return memberNode;
		}

		public static XElement ProcessLoyaltyCardAttributesToXml(LWIntegrationConfig config, VirtualCard vc, XElement vcNode, string dateConversionFormat)
		{
			SetAttribute(vcNode, "VcKey", vc.VcKey.ToString());
			SetAttribute(vcNode, "IpCode", vc.IpCode.ToString());
			SetAttribute(vcNode, "LoyaltyIdNumber", vc.LoyaltyIdNumber);
			if (vc.DateIssued != null)
			{
				string tmp = config.HasISO8601CompliantDateStringFormat
					? DateTimeUtil.ConvertDateToISO8601String(vc.DateIssued, config.IgnoreTimeZoneOffset)
					: DateTimeUtil.ConvertDateToString(dateConversionFormat, vc.DateIssued);
				SetAttribute(vcNode, "DateIssued", tmp);
			}
			if (vc.DateRegistered != null)
			{
				string tmp = config.HasISO8601CompliantDateStringFormat
					? DateTimeUtil.ConvertDateToISO8601String(vc.DateRegistered, config.IgnoreTimeZoneOffset)
					: DateTimeUtil.ConvertDateToString(dateConversionFormat, vc.DateRegistered);
				SetAttribute(vcNode, "DateRegistered", tmp);
			}
			SetAttribute(vcNode, "IsPrimary", vc.IsPrimary.ToString().ToLower());
			SetAttribute(vcNode, "Status", vc.Status.ToString());
			SetAttribute(vcNode, "CardType", vc.CardType.ToString());
			return vcNode;
		}

		private static string GetAttributeValue(LWIntegrationConfig config, IClientDataObject row, string attributeName, string dateConversionFormat)
		{
			string attValue = "";

			object val = row.GetAttributeValue(attributeName);
			if (val != null && val.GetType() != typeof(System.DBNull))
			{
				if (val.GetType() == typeof(DateTime))
				{
					attValue = config.HasISO8601CompliantDateStringFormat
						? DateTimeUtil.ConvertDateToISO8601String((DateTime)val, config.IgnoreTimeZoneOffset)
						: DateTimeUtil.ConvertDateToString(dateConversionFormat, (DateTime)val);
				}
				else
				{
					attValue = val.ToString();
				}
			}
			return attValue;
		}

		/// <summary>
		/// This method is used by the ourbound adapter to serialize members
		/// </summary>
		/// <param name="config"></param>
		/// <param name="rootNode"></param>
		/// <param name="member"></param>
		/// <returns></returns>
		public static XDocument SerializeMemberToXml(LWIntegrationConfig config, XElement rootNode, Member member, string dateConversionFormat)
		{
			//XDocument doc = rootNode.OwnerDocument;
			XElement memberNode = CreateNewElement(null, "Member", null, null);
			rootNode.Add(memberNode);

			memberNode = ProcessMemberAttributesToXml(config, member, memberNode, dateConversionFormat);

			Dictionary<string, List<IClientDataObject>> children = member.GetChildAttributeSets();
			if (children != null && children.Count > 0)
			{
				foreach (string chName in children.Keys)
				{
					IList<IClientDataObject> chSet = member.GetChildAttributeSets(chName);
					foreach (IClientDataObject ch in chSet)
					{
						memberNode = SerializeAttributeSetToXml(config, memberNode, ch, dateConversionFormat);
					}
				}
			}
			foreach (VirtualCard vc in member.LoyaltyCards)
			{
				XElement vcNode = CreateNewElement(null, "VirtualCard", null, null);
				memberNode.Add(vcNode);
				// process this virtual card attributes
				vcNode = ProcessLoyaltyCardAttributesToXml(config, vc, vcNode, dateConversionFormat);
				// now process the attribute sets of the virtual card
				Dictionary<string, List<IClientDataObject>> vcChildren = vc.GetChildAttributeSets();
				if (vcChildren != null && vcChildren.Count > 0)
				{
					foreach (string vcChName in vcChildren.Keys)
					{
						IList<IClientDataObject> vcChSet = vc.GetChildAttributeSets(vcChName);
						foreach (IClientDataObject ch in vcChSet)
						{
							vcNode = SerializeAttributeSetToXml(config, vcNode, ch, dateConversionFormat);
						}
					}
				}
			}
			return rootNode.Document;
		}

		#endregion

		#region Global Attribute Sets

		/// <summary>
		/// 
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="attSetName"></param>
		/// <param name="criteria"></param>
		/// <returns></returns>
		public static List<IClientDataObject> GetGlobalAttributeSet(string attSetName, LWCriterion criteria)
		{
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData meta = service.GetAttributeSetMetaData(attSetName);
				return service.GetAttributeSetObjects((IAttributeSetContainer)null, meta, criteria, null, false);
			}
		}

		#endregion

		#region Xml Related Helpers

		public static string GetValueByPath(XElement memberNode, string path)
		{

			//string svalue = "";
			if (string.IsNullOrEmpty(path))
			{
				throw new LWIntegrationException("Null search path specified.") { ErrorCode = 9993 };
			}
			string svalue = GetNodeValueByPath(memberNode, path);
			//if (!string.IsNullOrEmpty(svalue))
			//{
			//    svalue = FixNullString(svalue);
			//}
			return svalue;
		}

		public static XElement SelectSingleNode(XElement root, string name)
		{
			XElement selection = null;
			if (string.IsNullOrEmpty(root.Name.Namespace.ToString()))
			{
				selection = root.Element(name);
			}
			else
			{
				XName n = XName.Get(name, root.Name.Namespace.ToString());
				selection = root.Element(n);
			}
			return selection;
		}

		public static string GetNodeValue(XElement node, bool trimIt, string defaultValue)
		{
			string val = defaultValue;
			if (node != null)
			{
				val = node.Value;
				if (trimIt && !string.IsNullOrEmpty(val))
				{
					val = val.Trim();
				}
			}
			return val;
		}

		public static string GetNodeValueByPath(XElement memberNode, string path)
		{
			return XmlExtensions.GetNodeValueByPath(memberNode, path);
			//string value = string.Empty;
			//string lpath = path;
			//if (path.StartsWith("Member"))
			//{
			//    lpath = "./" + path.Substring("Member/".Length);
			//}
			//else if (path.StartsWith("/Member"))
			//{
			//    lpath = "./" + path.Substring("/Member/".Length);
			//}
			//// first search using the provided path.
			//XElement sNode = memberNode.XPathSelectElement(lpath);
			//if (sNode == null)
			//{
			//    XElement cNode = null;
			//    string satt = null;
			//    int idx = lpath.LastIndexOf('/');
			//    if (idx == -1)
			//    {
			//        satt = lpath;
			//        cNode = memberNode;
			//    }
			//    else
			//    {
			//        string cNodePath = string.Empty;
			//        if (lpath.StartsWith("./"))
			//        {
			//            cNodePath = lpath.Substring(2, idx - 2);
			//        }
			//        else
			//        {
			//            cNodePath = lpath.Substring(0, idx);
			//        }
			//        satt = lpath.Substring(idx + 1);
			//        cNode = memberNode.Element(cNodePath);
			//    }
			//    if (cNode != null && satt != null)
			//    {
			//        var atts = cNode.Attributes();
			//        if (atts != null)
			//        {
			//            foreach (XAttribute attribute in cNode.Attributes())
			//            {
			//                if (attribute.Name.LocalName.Equals(satt))
			//                {
			//                    value = attribute.Value;
			//                    break;
			//                }
			//            }
			//        }
			//    }
			//}
			//return value;
		}

		public static void WriteMessage(string msg, string debugOutputPath)
		{
			if (!string.IsNullOrEmpty(debugOutputPath) && System.IO.Directory.Exists(debugOutputPath))
			{
				string fname = debugOutputPath + Guid.NewGuid().ToString() + ".xml";
				StreamWriter writer = new StreamWriter(File.Create(fname), Encoding.UTF8);
				writer.Write(msg);
				writer.Close();
			}
		}
		#endregion

		#region Attribute Set Helpers
		public static bool HasValueChanged(string attName, DataType dt, IClientDataObject rowToModify, object newValue)
		{
			object existingValue = rowToModify.GetAttributeValue(attName);
            if (dt == DataType.String)
            {
                // one string can be null and other be empty.  this needs to be checked first.
                string oldStr = (string)existingValue;
                string newStr = (string)newValue;
                if (string.IsNullOrEmpty(oldStr) && string.IsNullOrEmpty(newStr))
                {
                    return false;
                }
            }
			if (existingValue != null && newValue == null ||
				existingValue == null && newValue != null)
			{
				return true;
			}
			else if (existingValue == null && newValue == null)
			{
				return false;
			}
			bool result = true;
			switch (dt)
			{
				case DataType.Date:
					result = !DateTimeUtil.Equal((DateTime)existingValue, (DateTime)newValue);
					break;
				case DataType.Decimal:
					result = existingValue.ToString() != newValue.ToString();
					break;
				case DataType.Integer:
					result = existingValue.ToString() != newValue.ToString();
					break;
				case DataType.Money:
					result = existingValue.ToString() != newValue.ToString();
					break;
				case DataType.Number:
					result = existingValue.ToString() != newValue.ToString();
					break;
				case DataType.Boolean:
					result = existingValue.ToString().ToLower() != newValue.ToString().ToLower();
					break;
				//case DataType.Clob:
				case DataType.String:
					result = existingValue.ToString() != newValue.ToString();
					break;
			}
			return result;
		}

		public static bool HasPropertyValueChanged(Type propType, object oldValue, object newValue)
		{
            if (propType == typeof(string))
            {
                // one string can be null and other be empty.  this needs to be checked first.
                string oldStr = (string)oldValue;
                string newStr = (string)newValue;
                if (string.IsNullOrEmpty(oldStr) && string.IsNullOrEmpty(newStr))
                {
                    return false;
                }
            }
            if (oldValue != null && newValue == null ||
                oldValue == null && newValue != null)
            {
                return true;
            }
            else if (oldValue == null && newValue == null)
            {
                return false;
            }
            bool result = true;
			if (propType == typeof(DateTime))
			{
				result = !DateTimeUtil.Equal((DateTime)oldValue, (DateTime)newValue);
			}
			else if (propType == typeof(DateTime?))
			{
				result = !DateTimeUtil.Equal((DateTime)oldValue, (DateTime)newValue);
			}
			else if (propType == typeof(Int64))
			{
				result = oldValue.ToString() != newValue.ToString();
			}
			else if (propType == typeof(Int64?))
			{
				result = oldValue.ToString() != newValue.ToString();
			}
            else if (propType == typeof(bool?) || propType == typeof(bool))
            {
                result = oldValue.ToString().ToLower() != newValue.ToString().ToLower();
            }
            else
            {
                // string type
                string oldStr = (string)oldValue;
                string newStr = (string)newValue;
                result = oldStr != newStr;
            }
			return result;
		}

        public static void UpdateAttributeValue(IClientDataObject rowToModify, DataType dt, object newValue, AttributeMetaData attMeta, bool checkForChangedValues)
        {
            string methodname = "UpdateAttributeValue";

            if (checkForChangedValues)
            {
                if (HasValueChanged(attMeta.Name, dt, rowToModify, newValue))
                {
                    if ((dt == DataType.String && string.IsNullOrEmpty((string)newValue) && attMeta.IsRequired) ||
                        (newValue == null && attMeta.IsRequired))
                    {
                        string errMsg = string.Format("Null value provided for required attribute {0}.", attMeta.Name);
                        throw new LWValidationException(errMsg) { ErrorCode = 6000 };
                    }
                    else
                    {
                        // update the value
                        if (newValue == null || string.IsNullOrEmpty(newValue.ToString()))
                        {
                            _logger.Debug(_className, methodname, "Setting value for " + attMeta.Name + " to null.");                            
                        }
                        rowToModify.SetAttributeValue(attMeta.Name, newValue, attMeta);
                    }
                }
            }
            else
            {
                rowToModify.SetAttributeValue(attMeta.Name, newValue, attMeta);
            }                        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowToModify"></param>
        /// <param name="attrNode"></param>
        public static void ProcessAttribute(
			LWIntegrationConfig config, 
			IClientDataObject rowToModify, 
			string attName, 
			DataType attType, 
			string attValue, 
			string dateConversionFormat, 
			bool trimStrings, 
			bool checkForChangedValues, 
			bool addMissingPropertyAsTransient,
			AttributeSetMetaData meta,
			AttributeMetaData attMeta)
        {
            string methodname = "ProcessAttributeFromXml";

            _logger.Debug(_className, methodname, string.Format("Processing attribute {0}.", attName));

			meta = meta ?? rowToModify.GetMetaData();
			attMeta = attMeta ?? meta.GetAttribute(attName);            

            try
            {
                DataType dt = (DataType)Enum.Parse(typeof(DataType), attMeta.DataType);
                switch (dt)
                {
                    case DataType.Date:
                        DateTime? newValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
							if (attValue.StartsWith("/Date(")) //Microsoft's WCF Date Format
							{
								newValue = DateTimeUtil.ConvertMicrosoftWcfStringToDate(attValue);
							}
                            else if (config != null)
                            {
                                newValue = config.HasISO8601CompliantDateStringFormat
                                    ? DateTimeUtil.ConvertISO8601StringToDate(attValue, config.IgnoreTimeZoneOffset)
                                    : DateTimeUtil.ConvertStringToDate(dateConversionFormat, attValue);
                            }
                            else
                            {
                                newValue = DateTimeUtil.ConvertStringToDate(dateConversionFormat, attValue);
                            }
                        }
                        UpdateAttributeValue(rowToModify, dt, newValue, attMeta, checkForChangedValues);
                        break;
                    case DataType.Decimal:
                        decimal? dValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            dValue = System.Decimal.Parse(attValue);
                        }
                        UpdateAttributeValue(rowToModify, dt, dValue, attMeta, checkForChangedValues);
                        break;
                    case DataType.Integer:
                        int? iValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            iValue = System.Int32.Parse(attValue);
                        }
                        UpdateAttributeValue(rowToModify, dt, iValue, attMeta, checkForChangedValues);
                        break;
                    case DataType.Money:
                        decimal? dbValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            dbValue = System.Decimal.Parse(attValue);
                        }
                        UpdateAttributeValue(rowToModify, dt, dbValue, attMeta, checkForChangedValues);
                        break;
                    case DataType.Number:
                        long? lValue = null;
                        if (!string.IsNullOrEmpty(attValue))
                        {
                            lValue = System.Int64.Parse(attValue);
                        }
                        UpdateAttributeValue(rowToModify, dt, lValue, attMeta, checkForChangedValues);
                        break;
                    case DataType.Boolean:
                        bool? bValue = null;
                        if (!String.IsNullOrEmpty(attValue) && (attValue == "0" || attValue == "1"))
                        {
                            if (attValue == "1")
                            {
                                bValue = true;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(attValue))
                            {
                                bValue = System.Boolean.Parse(attValue);
                            }
                        }
                        UpdateAttributeValue(rowToModify, dt, bValue, attMeta, checkForChangedValues);
                        break;
                    case DataType.String:
                        UpdateAttributeValue(rowToModify, dt, attValue, attMeta, checkForChangedValues);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (attMeta == null && addMissingPropertyAsTransient && ex is LWMetaDataException)
                {
                    rowToModify.UpdateTransientProperty(attName, attValue);
                }
                else
                {
                    string errmsg = string.Format("Error processing attribute {0} of {1} : {2}.", attName, meta.Name, ex.Message);
                    _logger.Error(_className, methodname, errmsg, ex);
                    throw new LWIntegrationException(errmsg, ex) { ErrorCode = 9989 };
                }
            }
            //}
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rowToModify"></param>
		/// <param name="attrNode"></param>
        public static void ProcessAttributeFromXml(
			LWIntegrationConfig config, 
			IClientDataObject rowToModify, 
			XAttribute attrNode, 
			string dateConversionFormat, 
			bool trimStrings, 
			bool checkForChangedValues, 
			bool addMissingPropertyAsTransient, 
			AttributeSetMetaData meta, 
			AttributeMetaData attMeta)
        {
            string methodname = "ProcessAttributeFromXml";

            _logger.Debug(_className, methodname, string.Format("Processing attribute {0}.", attrNode.Name));

			meta = meta ?? rowToModify.GetMetaData();
			attMeta = attMeta ?? meta.GetAttribute(attrNode.Name.LocalName);

            string attValue = LWIntegrationUtilities.GetAttributeValue(trimStrings, attrNode);
            DataType dt = (DataType)Enum.Parse(typeof(DataType), attMeta.DataType);

            ProcessAttribute(config, rowToModify, attrNode.Name.LocalName, dt, attValue, dateConversionFormat, trimStrings, checkForChangedValues, addMissingPropertyAsTransient, meta, attMeta);                        
        }

		public static IList<IClientDataObject> GetChildAttributeSets(IAttributeSetContainer owner, string attSetname)
		{
			if (owner != null)
			{
				return owner.GetChildAttributeSets(attSetname);
			}
			else
			{
				return LWIntegrationUtilities.GetGlobalAttributeSet(attSetname, null);
			}
		}

		private static IClientDataObject FindMemberAttributeSetInList(IList<IClientDataObject> attList, LWIntegrationConfig.AttributeSetUpdateDirective updDirective, XElement aNode)
		{
			IClientDataObject obj = null;
			switch (updDirective.FindMethod)
			{
				case LWIntegrationConfig.AttributeSetFindMethodEnum.First:
					if (attList != null && attList.Count > 0)
					{
						obj = attList[0];
					}
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Last:
					if (attList != null && attList.Count > 0)
					{
						obj = attList[attList.Count - 1];
					}
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Index:
					int index = int.Parse(updDirective.FindValue);
					if (attList != null && attList.Count > 0 && index <= attList.Count)
					{
						obj = attList[index];
					}
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Newest:
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Attribute:
					XAttribute findAttNode = aNode.Attribute(updDirective.FindValue);
					if (findAttNode != null)
					{
						foreach (IClientDataObject row in attList)
						{
							// it is important to perform the correct type of comparison
							object attValue = row.GetAttributeValue(updDirective.FindValue);
							if (attValue != null)
							{
								if (AreEqual(attValue, findAttNode.Value))
								{
									obj = row;
									break;
								}
							}
							else
							{
								//TODO: raise an exception.
							}
						}
					}
					break;
				default:
					break;
			}
			return obj;
		}

		private static List<IClientDataObject> FindGlobalAttributeSetInList(AttributeSetMetaData meta, LWIntegrationConfig.AttributeSetUpdateDirective updDirective, XElement aNode)
		{
			string methodName = "FindGlobalAttributeSetInList";
			string errMsg = string.Empty;


			List<IClientDataObject> objList = null;
			switch (updDirective.FindMethod)
			{
				case LWIntegrationConfig.AttributeSetFindMethodEnum.First:
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Last:
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Index:
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Newest:
					errMsg = "FindMethod not supported for global attribute sets.";
					_logger.Error(_className, methodName, errMsg);
					throw new LWIntegrationException(errMsg) { ErrorCode = 6001 };
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Attribute:
					XAttribute findAttNode = aNode.Attribute(updDirective.FindValue);
					if (findAttNode != null)
					{
						using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
						{
							LWCriterion crit = new LWCriterion(meta.Name) { UseAlias = true };
							//TODO: May have to convrt the string to object of correct type.
							crit.Add(LWCriterion.OperatorType.AND, updDirective.FindValue, findAttNode.Value, LWCriterion.Predicate.Eq);
							objList = service.GetAttributeSetObjects(null, meta.Name, crit, null, false, true);
						}
					}
					break;
				default:
					break;
			}
			return objList;
		}

		public static VirtualCard FindVirtualCardInList(Member member, LWIntegrationConfig.AttributeSetUpdateDirective updDirective, XElement aNode)
		{
			VirtualCard vc = null;
			switch (updDirective.FindMethod)
			{
				case LWIntegrationConfig.AttributeSetFindMethodEnum.First:
					if (member != null && member.LoyaltyCards.Count > 0)
					{
						vc = member.GetFirstCard();
					}
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Last:
					if (member != null && member.LoyaltyCards.Count > 0)
					{
						vc = member.LoyaltyCards[member.LoyaltyCards.Count - 1];
					}
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Index:
					int index = int.Parse(updDirective.FindValue);
					if (member != null && member.LoyaltyCards.Count > 0 && index <= member.LoyaltyCards.Count)
					{
						vc = member.LoyaltyCards[index];
					}
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Newest:
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Attribute:
					XAttribute findAttNode = aNode.Attribute(updDirective.FindValue);
					if (findAttNode != null)
					{
						vc = member.GetLoyaltyCard(findAttNode.Value);
						if (vc != null)
						{
							// Check its status
							if (!vc.IsValid())
							{
								string err = string.Format("Loyalty card with loyalty id # {0} is not valid anymore.", vc.LoyaltyIdNumber);
								throw new CRMException(err);
							}							
						}
					}
					break;
				default:
					break;
			}
			return vc;
		}

		private static void SetLastActivityDate(LWIntegrationConfig.AttributeSetDirective directive, object owner)
		{
            if (directive != null && directive.UpdateLastActivityDate)
			{
				Member member = null;
				if (owner != null)
				{
					if (owner.GetType() == typeof(Member))
					{
						member = (Member)owner;
					}
					else if (owner.GetType() == typeof(VirtualCard))
					{
						member = ((VirtualCard)owner).Member;
					}
					else
					{
						IClientDataObject obj = owner as ClientDataObject;
						if (obj != null)
						{
							SetLastActivityDate(directive, obj.Parent);
						}
					}
				}
				if (member != null)
				{
					member.LastActivityDate = DateTime.Now;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="directive"></param>
		/// <param name="owner"></param>
		/// <param name="def"></param>
		/// <param name="containingRow"></param>
		/// <param name="aNode"></param>
		/// <returns></returns>
		public static IClientDataObject GetMemberAttributeSetToModify(LWIntegrationConfig.AttributeSetDirective directive, IAttributeSetContainer owner, AttributeSetMetaData def, XElement aNode)
		{
			string methodname = "GetMemberAttributeSetToModify";

			IList<IClientDataObject> attList = null;
			IClientDataObject thisRow = null;
			if (directive == null)
			{
				_logger.Debug(_className, methodname, string.Format("No directives provided for {0}.", def.Name));
				thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
				if (owner != null)
				{
					owner.AddChildAttributeSet(thisRow);
				}
			}
			else
			{
				if (directive.GetType().Name.Equals("AttributeSetCreateDirective"))
				{
					_logger.Debug(_className, methodname, string.Format("Processing create directives for {0}.", def.Name));
					LWIntegrationConfig.AttributeSetCreateDirective crtDirective = (LWIntegrationConfig.AttributeSetCreateDirective)directive;
					if (crtDirective.MarkTrueAttributes.Count > 0)
					{
						_logger.Debug(_className, methodname, string.Format("Loading {0}.", def.Name));
						attList = GetChildAttributeSets(owner, def.Name);
					}
					else if (crtDirective.LoadExisting)
					{
						attList = GetChildAttributeSets(owner, def.Name);
					}
					XAttribute attNode = aNode.Attribute(crtDirective.IfPresent);
					if (attNode != null && !string.IsNullOrEmpty(attNode.Value))
					{
						thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
						if (owner != null)
						{
							owner.AddChildAttributeSet(thisRow);
						}
					}
					else
					{
						_logger.Trace(_className, methodname,
							string.Format("Skipping the creation of {0} because attribute {1} is not present.",
							def.Name, crtDirective.IfPresent));
					}
					//Process mark active attributes.
					System.Collections.IList list = crtDirective.MarkTrueAttributes;
					if (thisRow != null)
					{
						foreach (string mt in list)
						{
							AttributeMetaData att = def.GetAttribute(mt);
							if (att.DataType != Enum.GetName(typeof(DataType), DataType.Boolean))
							{
								string err = "The type of " + def.Name + "." + mt + " has to be a boolean for it to be marked true.";
								_logger.Error(_className, methodname, err);
								throw new LWIntegrationException(err);
							}
							thisRow.SetAttributeValue(mt, true, att);
							if (attList != null)
							{
								foreach (IClientDataObject row in attList)
								{
									if (row != thisRow)
									{
										row.SetAttributeValue(mt, false, att);
									}
								}
							}
						}
					}
                    //SetLastActivityDate(directive, owner);
				}
				else if (directive.GetType().Name.Equals("AttributeSetUpdateDirective"))
				{
					_logger.Debug(_className, methodname, string.Format("Processing update directives for {0}.", def.Name));
					LWIntegrationConfig.AttributeSetUpdateDirective updDirective = (LWIntegrationConfig.AttributeSetUpdateDirective)directive;
					// find the existing row in the attribute set
					attList = GetChildAttributeSets(owner, def.Name);
					if (attList == null || attList.Count == 0)
					{
						if (owner != null)
						{
							_logger.Debug(_className, methodname, string.Format("No existing rows exist for {0} for owner {1}.", def.Name, owner.MyKey));
						}
						else
						{
							_logger.Debug(_className, methodname, string.Format("No existing rows exist for {0}.", def.Name));
						}
						if (updDirective.CreateIfNotFound)
						{
							thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
							if (owner != null)
							{
								owner.AddChildAttributeSet(thisRow);
							}
						}
						else
						{
							//string err = "Could not find " + updDirective.FindBy + " in input Xml for update.";
							//string err = "Could not find required attribute to update in input Xml for update.";
							string err = string.Format("Could not find required attribute '{0}' in input Xml for update.", def.Name);
							_logger.Error(_className, methodname, err);
							throw new LWIntegrationException(err);
						}
					}
					else
					{
						if (owner != null)
						{
							_logger.Debug(_className, methodname, string.Format("{0} rows exist for {1} for owner {2}.", attList.Count, def.Name, owner.MyKey));
						}
						else
						{
							_logger.Debug(_className, methodname, string.Format("{0} rows exist for {1}.", attList.Count, def.Name));
						}
						thisRow = FindMemberAttributeSetInList(attList, updDirective, aNode);
						if (thisRow == null)
						{
							if (updDirective.CreateIfNotFound)
							{
								// create a new row
								thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
								if (owner != null)
								{
									owner.AddChildAttributeSet(thisRow);
								}
							}
							else
							{
								/*
								* This attribute set was supposed to be updated based on certain criteria
								* value.  However, that attribute value is not present in the input record.                            
								* */
								string err = "Could not find attribute to update in input Xml for update.";
								_logger.Error(_className, methodname, err);
								throw new LWIntegrationException(err);
							}
						}
					}                    
				}
				else if (directive.GetType().Name.Equals("AttributeSetModifyDirective"))
				{
					_logger.Debug(_className, methodname, string.Format("Processing modify directives for {0}.  NOT IMPLEMENTED YET.", def.Name));
					// not implemented yet.
				}
			}
			return thisRow;
		}

		public static List<IClientDataObject> GetGlobalAttributeSetsToModify(LWIntegrationConfig.AttributeSetDirective directive, AttributeSetMetaData def, XElement aNode)
		{
			string methodname = "GetGlobalAttributeSetToModify";

			List<IClientDataObject> attList = null;
			IClientDataObject thisRow = null;
			if (directive == null)
			{
				_logger.Debug(_className, methodname, string.Format("No directives provided for {0}.", def.Name));
				thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
			}
			else
			{
				if (directive.GetType().Name.Equals("AttributeSetCreateDirective"))
				{
					_logger.Debug(_className, methodname, string.Format("Processing create directives for {0}.", def.Name));
					LWIntegrationConfig.AttributeSetCreateDirective crtDirective = (LWIntegrationConfig.AttributeSetCreateDirective)directive;
					XAttribute attNode = aNode.Attribute(crtDirective.IfPresent);
					if (attNode != null && !string.IsNullOrEmpty(attNode.Value))
					{
						attList = new List<IClientDataObject>();
						thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
						attList.Add(thisRow);
					}
					else
					{
						_logger.Trace(_className, methodname,
							string.Format("Skipping the creation of {0} because attribute {1} is not present.",
							def.Name, crtDirective.IfPresent));
					}
				}
				else if (directive.GetType().Name.Equals("AttributeSetUpdateDirective"))
				{
					_logger.Debug(_className, methodname, string.Format("Processing update directives for {0}.", def.Name));
					LWIntegrationConfig.AttributeSetUpdateDirective updDirective = (LWIntegrationConfig.AttributeSetUpdateDirective)directive;
					// find the existing row in the attribute set
					attList = FindGlobalAttributeSetInList(def, updDirective, aNode);
					if (attList == null || attList.Count == 0)
					{
						if (updDirective.CreateIfNotFound)
						{
							attList = new List<IClientDataObject>();
							thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
							attList.Add(thisRow);
						}
						else
						{
							string err = string.Format("Could not find required attribute '{0}' in input Xml for update.", def.Name);
							_logger.Error(_className, methodname, err);
							throw new LWIntegrationException(err);
						}
					}
				}
				else if (directive.GetType().Name.Equals("AttributeSetModifyDirective"))
				{
					_logger.Debug(_className, methodname, string.Format("Processing modify directives for {0}.  NOT IMPLEMENTED YET.", def.Name));
					// not implemented yet.
				}
			}
			return attList;
		}
		#endregion

		#region Reference Data Helpers
		public static string GetDataAttributeFromXml(string mappingAttribute, string attributeName, XElement node, bool required)
		{
			string methodName = "GetRefDataAttributeFromXml";
			string attValue = string.Empty;
			if (string.IsNullOrEmpty(mappingAttribute))
			{
				mappingAttribute = attributeName;
			}
			XElement attNode = node.Element(mappingAttribute);
			if (attNode != null)
			{
				attValue = attNode.Value;
			}
			else
			{
				string msg = string.Format("Could not find node {0} in the message.", mappingAttribute);
				_logger.Error(_className, methodName, msg);
			}
			if (string.IsNullOrEmpty(attValue) && required)
			{
				string msg = string.Format("No value found for required attribute {0} in the message.", attributeName);
				_logger.Error(_className, methodName, msg);
				throw new LWException(msg);
			}
			else
			{
				return attValue;
			}
		}
		#endregion
	}
}
