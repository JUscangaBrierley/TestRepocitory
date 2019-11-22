using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;

using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.FrameWork.Interfaces;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
	public class MGMemberUtils
	{
		public enum MemberLoadDirective { IpCode, AlternateId, EmailAddress, UserName, UseInterceptor };

		private const string _className = "AuthenticateMember";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
		
		public static void Authenticate(IAuthenticateMemberInterceptor interceptor,
			string deviceType,
			string version,
			WcfAuthenticationToken.AuthenticationScheme authScheme,
			string username,
			string password,
			string resetCode,
			ref WcfAuthenticationToken token,
			out LoginStatusEnum loginStatus,
            bool unlockMember = false)
		{
			const string methodName = "Authenticate";

			Member m = null;
			loginStatus = LoginStatusEnum.Failure;
			token = null;

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				if (interceptor != null)
				{
					try
					{
						AuthenticationFields identityType = AuthenticationFields.Username;
						if (authScheme == WcfAuthenticationToken.AuthenticationScheme.EmailAndLoyaltyId)
						{
							identityType = AuthenticationFields.PrimaryEmailAddress;
						}
						_logger.Debug(_className, methodName, "Invoking BeforeAuthenticate method of the interceptor.");
						interceptor.BeforeAuthenticate(identityType, username, password, resetCode);
					}
					catch (NotImplementedException)
					{
						// not implemented.
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Exception thrown by BeforeAuthenticate interceptor.", ex);
						throw;
					}
				}

				switch (authScheme)
				{
					case WcfAuthenticationToken.AuthenticationScheme.UsernameAndPassword:
						_logger.Trace(_className, methodName, "Authenticating member: " + username);
						bool authenticated = false;
						try
						{
							if (interceptor != null)
							{
								try
								{
									m = interceptor.AuthenticateMember(AuthenticationFields.Username, username, password, resetCode, ref loginStatus);
								}
								catch (NotImplementedException)
								{
                                    m = loyalty.LoginMember(AuthenticationFields.Username, username, password, resetCode, ref loginStatus, unlockMember);
								}
							}
							else
							{
                                m = loyalty.LoginMember(AuthenticationFields.Username, username, password, resetCode, ref loginStatus, unlockMember);
							}

							authenticated = loginStatus == LoginStatusEnum.Success || loginStatus == LoginStatusEnum.PasswordResetRequired;
						}
						catch (AuthenticationException ex)
						{
							if (interceptor != null)
							{
								try
								{
									_logger.Debug(_className, methodName, "Invoking HandleAuthenticationException method of the interceptor.");
									authenticated = interceptor.HandleAuthenticationException(m, loginStatus, ex);
								}
								catch (NotImplementedException)
								{
									// not implemented.
								}
								catch (Exception ex2)
								{
									_logger.Error(_className, methodName, "Exception thrown by HandleAuthenticationException interceptor.", ex2);
									throw;
								}
							}
						}
						_logger.Trace(_className, methodName, "Member " + username + " has been authenticated.");
						break;
					case WcfAuthenticationToken.AuthenticationScheme.EmailAndLoyaltyId:
						_logger.Trace(_className, methodName, "Authenticating loyalty id: " + password);
						m = loyalty.LoadMemberFromLoyaltyID(password);
						if (m != null)
						{
							if (string.IsNullOrEmpty(m.PrimaryEmailAddress) && string.IsNullOrEmpty(username))
							{
								_logger.Trace(_className, methodName, string.Format("Member {0} has no email address.", m.IpCode));
							}
							else
							{
								if (string.IsNullOrEmpty(m.PrimaryEmailAddress))
								{
									string error = string.Format("Member {0} has no primary email address.", m.IpCode);
									_logger.Error(_className, methodName, error);
									throw new LWOperationInvocationException(error) { ErrorCode = 1 };
								}
								else if (string.IsNullOrEmpty(username))
								{
									string error = string.Format("No email provided for authenticating member {0}.", m.IpCode);
									_logger.Error(_className, methodName, error);
									throw new LWOperationInvocationException(error) { ErrorCode = 1 };
								}
								else if (m.PrimaryEmailAddress.ToLower() != username.ToLower())
								{
									string error = string.Format("Loyalty ID {0} could not be authenticated due to invalid email '{1}'.", password, username);
									_logger.Error(_className, methodName, error);
									throw new LWOperationInvocationException(error) { ErrorCode = 1 };
								}
								else if (m.MemberStatus == MemberStatusEnum.Locked)
								{
									_logger.Error(_className, methodName, "Loyalty ID " + password + " member is locked.");
									loginStatus = LoginStatusEnum.LockedOut;
									return;
								}
								else if (m.IsPasswordChangeRequired())
								{
									_logger.Trace(_className, methodName, "Loyalty ID " + password + " has been authenticated but requires a password change.");
									loginStatus = LoginStatusEnum.PasswordResetRequired;
								}
								else
								{
									_logger.Trace(_className, methodName, "Loyalty ID " + password + " has been authenticated.");
									loginStatus = LoginStatusEnum.Success;
								}
							}
						}
						else
						{
							string error = "Loyalty ID " + password + " not authenticated.";
							_logger.Error(_className, methodName, error);
							return;
						}
						break;
				}

				if (interceptor != null)
				{
					try
					{
						_logger.Debug(_className, methodName, "Invoking AfterAuthenticateOK method of the interceptor.");
						interceptor.AfterAuthenticateOK(m, loginStatus);
					}
					catch (NotImplementedException)
					{
						// not implemented.
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Exception thrown by AfterAuthenticateOK interceptor.", ex);
						throw;
					}
				}

                if (m != null)
                {
                    ContextObject context = new ContextObject() { Owner = m, Mode = RuleExecutionMode.Real };
                    loyalty.ExecuteEventRules(context, "MemberAuthenticate", RuleInvocationType.Manual);

                    _logger.Debug(_className, methodName, "User " + username + " has been successfully authenticated.");
                    token = new WcfAuthenticationToken(authScheme, username, string.IsNullOrEmpty(password) ? resetCode : password, m, loginStatus == LoginStatusEnum.PasswordResetRequired);
                    WcfAuthenticationToken.AddAuthTokenToCache(token);
                    _logger.Debug(_className, methodName, string.Format("Token {0} cached for user {1}", token.TokenId, username));
                }
			}
		}


		public static void Authenticate(MemberSocNet socnet, string cacheKey, string deviceType, string version, ref WcfAuthenticationToken token, out LoginStatusEnum loginStatus)
		{
			string methodName = "Authenticate";

			Member m = null;
			loginStatus = LoginStatusEnum.Failure;
			token = null;

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				//no interceptor, for now
				//WcfAuthenticationToken.AuthenticationScheme authScheme = (WcfAuthenticationToken.AuthenticationScheme)Enum.Parse(typeof(WcfAuthenticationToken.AuthenticationScheme), authSchemeStr);
				//if (interceptor != null)
				//{
				//	try
				//	{
				//		AuthenticationFields identityType = AuthenticationFields.Username;
				//		if (authScheme == WcfAuthenticationToken.AuthenticationScheme.EmailAndLoyaltyId)
				//		{
				//			identityType = AuthenticationFields.PrimaryEmailAddress;
				//		}
				//		_logger.Debug(_className, methodName, "Invoking BeforeAuthenticate method of the interceptor.");
				//		interceptor.BeforeAuthenticate(identityType, username, password);
				//	}
				//	catch (NotImplementedException)
				//	{
				//		// not implemented.
				//	}
				//	catch (Exception ex)
				//	{
				//		_logger.Error(_className, methodName, "Exception thrown by BeforeAuthenticate interceptor.", ex);
				//		throw;
				//	}
				//}

				try
				{
					m = loyalty.LoginMember(socnet, ref loginStatus);

					if (m != null)
					{
						_logger.Debug(_className, methodName, "User " + m.IpCode.ToString() + " has been successfully authenticated.");
						token = new WcfAuthenticationToken(WcfAuthenticationToken.AuthenticationScheme.SocialNetwork, socnet.ProviderType.ToString(), cacheKey, m, loginStatus == LoginStatusEnum.PasswordResetRequired);
						WcfAuthenticationToken.AddAuthTokenToCache(token);
						_logger.Debug(_className, methodName, string.Format("Token {0} cached for member {1}", token.TokenId, m.IpCode));
					}
				}
				catch (AuthenticationException ex)
				{
					//if (interceptor != null)
					//{
					//	try
					//	{
					//		_logger.Debug(_className, methodName, "Invoking HandleAuthenticationException method of the interceptor.");
					//		authenticated = interceptor.HandleAuthenticationException(m, loginStatus, ex);
					//	}
					//	catch (NotImplementedException)
					//	{
					//		// not implemented.
					//	}
					//	catch (Exception ex2)
					//	{
					//		_logger.Error(_className, methodName, "Exception thrown by HandleAuthenticationException interceptor.", ex);
					//		throw;
					//	}
					//}

					_logger.Error(_className, methodName, "Exception thrown by HandleAuthenticationException interceptor.", ex);
					throw;
				}

				//if (interceptor != null)
				//{
				//	try
				//	{
				//		_logger.Debug(_className, methodName, "Invoking AfterAuthenticateOK method of the interceptor.");
				//		interceptor.AfterAuthenticateOK(m, loginStatus);
				//	}
				//	catch (NotImplementedException)
				//	{
				//		// not implemented.
				//	}
				//	catch (Exception ex)
				//	{
				//		_logger.Error(_className, methodName, "Exception thrown by AfterAuthenticateOK interceptor.", ex);
				//		throw;
				//	}
				//}

				//this is done in loyalty.LoginMember - not sure why it's here, but it is duplicating the event rules
				//ContextObject context = new ContextObject() { Owner = m, Mode = RuleExecutionMode.Real };
				//loyalty.ExecuteEventRules(context, "MemberAuthenticate", RuleInvocationType.Manual);


			}
		}

        public static Member LoadExistingMember(WcfAuthenticationToken.AuthenticationScheme authScheme, Member incoming, IInboundMobileInterceptor interceptor)
        {
            Member existing = null;

            using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                switch (authScheme)
                {
                    case WcfAuthenticationToken.AuthenticationScheme.EmailAndLoyaltyId:
                        existing = !string.IsNullOrEmpty(incoming.PrimaryEmailAddress) ? loyalty.LoadMemberFromEmailAddress(incoming.PrimaryEmailAddress) : null;
                        break;
                    case WcfAuthenticationToken.AuthenticationScheme.UsernameAndPassword:
                        existing = loyalty.LoadMemberFromUserName(incoming.Username);
                        break;
                }

                return existing;
            }
        }

		public static Member LoadExistingMember(string loadDirectiveStr, MGMember incoming, IInboundMobileInterceptor interceptor)
		{
			string methodName = "LoadExistingMember";

			if (string.IsNullOrEmpty(loadDirectiveStr))
			{
				string errMsg = string.Format("No load directives specified to check.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg) { ErrorCode = 1 };
			}

			Member existing = null;

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				MemberLoadDirective loadDirective = (MemberLoadDirective)Enum.Parse(typeof(MemberLoadDirective), loadDirectiveStr);
				switch (loadDirective)
				{
					case MemberLoadDirective.IpCode:
						existing = loyalty.LoadMemberFromIPCode(incoming.IpCode);
						break;
					case MemberLoadDirective.AlternateId:
						existing = loyalty.LoadMemberFromAlternateID(incoming.AlternateId);
						break;
					case MemberLoadDirective.EmailAddress:
						existing = loyalty.LoadMemberFromEmailAddress(incoming.PrimaryEmailAddress);
						break;
					case MemberLoadDirective.UserName:
						existing = loyalty.LoadMemberFromUserName(incoming.Username);
						break;
					case MemberLoadDirective.UseInterceptor:
						break;
				}

				return existing;
			}
		}

		public static void PopulateAttributeSets(
			List<MGClientEntity> ChildEntities,
			LWIntegrationConfig config,
			IDictionary<string, LWIntegrationConfig.AttributeSetDirective> createDirectives,
			IAttributeSetContainer owner,
			AttributeSetMetaData asDef,
			string dateConversionFormat,
			bool trimStrings,
			bool checkForChangedValues)
		{
			string methodName = "PopulateAttributeSets";

			if (ChildEntities != null && ChildEntities.Count > 0)
			{
				using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					foreach (MGClientEntity entity in ChildEntities)
					{
						AttributeSetMetaData aSetMeta = loyalty.GetAttributeSetMetaData(entity.Name);
						if (aSetMeta != null)
						{
							// Process attributes of this attribute sets
							LWIntegrationConfig.AttributeSetDirective directive = createDirectives.ContainsKey(entity.Name) ? createDirectives[entity.Name] : null;
							IClientDataObject rowToModify = MGAttributeSetContainerUtility.GetMemberAttributeSetToModify(directive, owner, aSetMeta, entity);
							MGAttributeSetContainerUtility.ProcessAttributeSet(config, createDirectives, rowToModify, entity, asDef ?? aSetMeta, dateConversionFormat, trimStrings, checkForChangedValues);
						}
						else
						{
							string error = string.Format("Unable to find meta data for entity {0}.", entity.Name);
							_logger.Error(_className, methodName, error);
							throw new LWIntegrationException(error) { ErrorCode = 9987 };
						}
					}
				}
			}
		}

        public static void SaveMember(Member member)
        {
            using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                loyalty.SaveMember(member);
            }
        }
	}
}