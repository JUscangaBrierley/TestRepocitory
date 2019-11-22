using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders
{
	public abstract class OperationProviderBase : IAPIOperation, IDisposable
	{
		private class PostProcessState
		{
			public string Organization { get; set; }
			public string Environment { get; set; }
			public string EventName { get; set; }
			public LWIntegrationDirectives.InterceptorDirective Interceptor { get; set; }
			public Dictionary<string, object> Context { get; set; }
		}

		private const string _className = "OperationProviderBase";
		private static LWTriggerUserEventLogger _eventLogger = null;
		private static object _eventLoggerLock = new object();
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
		private bool _disposed = false;
		private NameValueCollection _functionProviderParms = null;
		private LWIntegrationDirectives _config = null;
		private DataService _dataService;
		private ContentService _contentService;
		private LoyaltyDataService _loyaltyDataService;

		public LWIntegrationDirectives Config
		{
			get { return _config; }
		}

		protected string Name { get; set; }

		protected DataService DataService
		{
			get
			{
				if (_dataService == null)
				{
					_dataService = LWDataServiceUtil.DataServiceInstance();
				}
				return _dataService;
			}
		}

		protected ContentService ContentService
		{
			get
			{
				if (_contentService == null)
				{
					_contentService = LWDataServiceUtil.ContentServiceInstance();
				}
				return _contentService;
			}
		}

		protected LoyaltyDataService LoyaltyDataService
		{
			get
			{
				if (_loyaltyDataService == null)
				{
					_loyaltyDataService = LWDataServiceUtil.LoyaltyDataServiceInstance();
				}
				return _loyaltyDataService;
			}
		}

		protected NameValueCollection FunctionProviderParms
		{
			get { return _functionProviderParms; }
		}

		protected LWTriggerUserEventLogger EventLogger
		{
			get
			{
				InitializeUserEventLogger();
				return _eventLogger;
			}
		}

		protected OperationProviderBase(string name)
		{
			Name = name;
		}

		public static void Shutdown()
		{
			if (_eventLoggerLock != null)
			{
				lock (_eventLoggerLock)
				{
					if (_eventLogger.IsAlive())
					{
						_eventLogger.ShutDown();
						_eventLogger.WaitToFinish();
					}
				}
			}
		}

		public static IAPIOperation GetCustomOperationProvider(LWIntegrationDirectives config, string opName)
		{
			string methodName = "GetCustomOperationProvider";

			System.Reflection.Assembly assembly = null;
			string providerType = string.Empty;
			string providerAssembly = string.Empty;

			IAPIOperation op = null;
			LWIntegrationDirectives.APIOperationDirective opDirective = config.GetOperationDirective(opName) as
					LWIntegrationDirectives.APIOperationDirective;
			if (string.IsNullOrEmpty(opDirective.FunctionProvider))
			{
				int index = providerType.LastIndexOf(".");
				if (index == -1)
				{
					providerType = "Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders." + opName;
				}
			}
			else
			{
				providerType = opDirective.FunctionProvider;
			}

			if (string.IsNullOrEmpty(opDirective.ProviderAssemblyPath))
			{
				providerAssembly = "Brierley.LoyaltyWare.LWIntegrationSvc.dll";
			}
			else
			{
				providerAssembly = opDirective.ProviderAssemblyPath;
			}

			assembly = ClassLoaderUtil.LoadAssembly(providerAssembly);
			if (assembly != null)
			{
				try
				{
					op = (IAPIOperation)ClassLoaderUtil.CreateInstance(assembly, providerType);
					op.Initialize(opDirective.Name, config, opDirective.FunctionProviderParms);
				}
				catch (Exception e)
				{
					string errMsg = string.Format("Error loading \"{0}\" from assembly {1}.", providerType, providerAssembly);
					LWOperationInvocationException ex = new LWOperationInvocationException(errMsg, e) { ErrorCode = 3200 };
					_logger.Error(_className, methodName, "Error loading operation provider.", ex);
					throw ex;
				}
			}
			else
			{
				string errMsg = string.Format("Unable to find assembly {0} containing {1}", providerAssembly, providerType);
				LWOperationInvocationException ex = new LWOperationInvocationException(errMsg) { ErrorCode = 3201 };
				_logger.Error(_className, methodName, "Error loading operation provider.", ex);
				throw ex;
			}
			return op;
		}

		public abstract string Invoke(string source, string parms);

		public virtual void Initialize(string opName, LWIntegrationDirectives config, NameValueCollection functionProviderParms)
		{
			_config = config;
			Name = opName;
			_functionProviderParms = functionProviderParms;
		}

		public string GetFunctionParameter(string name)
		{
			if (FunctionProviderParms != null)
			{
				return FunctionProviderParms[name];
			}
			else
			{
				return string.Empty;
			}
		}

		public void Dispose()
		{
			string methodName = "Dispose";
			_logger.Debug(_className, methodName, string.Format("Function {0} is being disposed.", Name));
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected static ISAPIOnSuccessfullInvocation GetOnSuccessInterceptor(LWIntegrationDirectives.InterceptorDirective dir)
		{
			return InterceptorUtil.GetInterceptor(dir) as ISAPIOnSuccessfullInvocation;
		}

		protected Member LoadMember(APIArguments args, string memberIdentityName = "MemberIdentity", string searchTypeName = "MemberSearchType", string searchValueName = "SearchValue")
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}

			if (args.ContainsKey(memberIdentityName))
			{
				string memberId = (string)args[memberIdentityName];
				if (string.IsNullOrEmpty(memberId))
				{
					throw new LWOperationInvocationException(string.Format("Please provide a member identity.")) { ErrorCode = 3301 };
				}
				Member member = LoadMember(memberId);
				if (member == null)
				{
					throw new LWOperationInvocationException(string.Format("Unable to find member with identity = {0}.", memberId)) { ErrorCode = 3302 };
				}
				return member;
			}

			//we didn't load from member identity, so there should be search values passed
			if (!args.ContainsKey(searchTypeName))
			{
				throw new LWOperationInvocationException("No search type provided.") { ErrorCode = 3317 };
			}
			string[] searchTypes = (string[])args[searchTypeName];

			if (!args.ContainsKey(searchValueName))
			{
				throw new LWOperationInvocationException("No search values provided.") { ErrorCode = 3317 };
			}
			string[] searchValues = (string[])args[searchValueName];

			if (searchTypes.Length != searchValues.Length)
			{
				throw new LWOperationInvocationException("The number of search values provided do not match the number of search options.") { ErrorCode = 3348 };
			}

			var members = new List<Member>();

			Func<IEnumerable<Member>, Func<Member, bool>, bool> checkMatches = delegate(IEnumerable<Member> matches, Func<Member, bool> check)
			{
				if (members.Count == 0 && matches != null && matches.Count() > 0)
				{
					members.AddRange(matches);
				}

				if (members.FirstOrDefault(check) == null)
				{
					//list already contains members, but no match on the current one, so the search fails.
					return false;
				}

				foreach (var m in members.Except(members.Where(check)).ToList())
				{
					members.Remove(m);
				}

				if (members.Count == 0)
				{
					//we've removed any possible results
					return false;
				}

				return true;
			};


			Func<Member, Func<Member, bool>, bool> checkMatch = delegate(Member match, Func<Member, bool> check)
			{
				if (members.Count == 0 && match != null)
				{
					members.Add(match);
					return true;
				}

				if (members.FirstOrDefault(check) == null)
				{
					//list already contains members, but no match on the current one, so the search fails.
					return false;
				}

				foreach (var m in members.Except(members.Where(check)).ToList())
				{
					members.Remove(m);
				}

				if (members.Count == 0)
				{
					//we've removed any possible results
					return false;
				}

				return true;
			};

			bool valid = true;
			for (int i = 0; i < searchTypes.Length; i++)
			{
				string searchType = searchTypes[i];
				Member member = null;
				IEnumerable<Member> matches = null;
				if (string.IsNullOrEmpty(searchType))
				{
					throw new LWOperationInvocationException(string.Format("No search type provided at index {0}.", i.ToString())) { ErrorCode = 3317 };
				}
				switch (searchType.ToLower())
				{
					case "memberid":
						if (string.IsNullOrEmpty(searchValues[i]))
						{
							throw new LWOperationInvocationException("No member id provided for member search.") { ErrorCode = 3301 };
						}
						long ipcode = long.Parse(searchValues[i]);
						if (members.Count == 0)
						{
							member = LoyaltyDataService.LoadMemberFromIPCode(ipcode);
						}
						valid = checkMatch(member, (o) => o.IpCode == ipcode);
						break;
					case "cardid":
						if (string.IsNullOrEmpty(searchValues[i]))
						{
							throw new LWOperationInvocationException("No card id provided for member search.") { ErrorCode = 3304 };
						}
						string loyaltyId = searchValues[i];
						if (members.Count == 0)
						{
							member = LoyaltyDataService.LoadMemberFromLoyaltyID(loyaltyId);
						}
						valid = checkMatch(member, (o) => o.GetLoyaltyCard(loyaltyId) != null);
						break;
					case "emailaddress":
						if (string.IsNullOrEmpty(searchValues[i]))
						{
							throw new LWOperationInvocationException("No email address provided for member search.") { ErrorCode = 3318 };
						}
						string email = searchValues[i];
						if (members.Count == 0)
						{
							member = LoyaltyDataService.LoadMemberFromEmailAddress(email);
						}
						valid = checkMatch(member, (o) => o.PrimaryEmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));
						break;
					case "phonenumber":
						if (string.IsNullOrEmpty(searchValues[i]))
						{
							throw new LWOperationInvocationException("No phone number provided for member search.") { ErrorCode = 3319 };
						}
						string phone = searchValues[i];
						if (members.Count == 0)
						{
							matches = LoyaltyDataService.GetMembersByPhoneNumber(phone, null);
						}
						valid = checkMatches(matches, (o) => o.PrimaryPhoneNumber.Equals(phone, StringComparison.OrdinalIgnoreCase));
						break;
					case "alternateid":
						if (string.IsNullOrEmpty(searchValues[i]))
						{
							throw new LWOperationInvocationException("No alternate id provided for member search.") { ErrorCode = 3320 };
						}
						string altId = searchValues[i];
						if (members.Count == 0)
						{
							member = LoyaltyDataService.LoadMemberFromAlternateID(altId);
						}
						valid = checkMatch(member, (o) => o.AlternateId.Equals(altId, StringComparison.OrdinalIgnoreCase));
						break;
					case "lastname":
						string name = searchValues[i];
						if (members.Count == 0)
						{
							matches = LoyaltyDataService.GetMembersByName(string.Empty, name, string.Empty, null);
						}
						valid = checkMatches(matches, (o) => o.LastName.Equals(name, StringComparison.OrdinalIgnoreCase));
						break;
					case "username":
						if (string.IsNullOrEmpty(searchValues[i]))
						{
							throw new LWOperationInvocationException("No username provided for member search.") { ErrorCode = 3321 };
						}
						string username = searchValues[i];
						if (members.Count == 0)
						{
							member = LoyaltyDataService.LoadMemberFromUserName(searchValues[i]);
						}
						valid = checkMatch(member, (o) => o.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
						break;
					case "postalcode":
						if (string.IsNullOrEmpty(searchValues[i]))
						{
							throw new LWOperationInvocationException("No postal code provided for member search.") { ErrorCode = 3322 };
						}
						string postalCode = searchValues[i];
						if (members.Count == 0)
						{
							matches = LoyaltyDataService.GetMembersByPostalCode(postalCode, null);
						}
						valid = checkMatches(matches, (o) => o.PrimaryPostalCode.Equals(postalCode, StringComparison.OrdinalIgnoreCase));
						break;
					default:
						throw new LWOperationInvocationException(string.Format("Invalid search type {0} provided.", searchType)) { ErrorCode = 3375 };
				}
				if (!valid)
				{
					break;
				}
			}

			if (!valid || members.Count == 0)
			{
				throw new LWOperationInvocationException(string.Format("Unable to find member with criteria {0}; {1}", string.Join(", ", searchTypes), string.Join(", ", searchValues))) { ErrorCode = 10000 };
			}
			else if (members.Count != 1)
			{
				throw new LWOperationInvocationException(string.Format("Multiple members found ({0}) using criteria {1}; {2}", members.Count, string.Join(", ", searchTypes), string.Join(", ", searchValues))) { ErrorCode = 10001 };
			}
			return members[0];
		}

		protected Member LoadMember(string memberId)
		{
			Member member = null;
			if (string.IsNullOrEmpty(memberId))
			{
				throw new LWOperationInvocationException("No member id provided for loading a member.") { ErrorCode = 3301 };
			}
			switch (_config.MemberIdentityType)
			{
				case LWIntegrationDirectives.MemberIdentityTypeEnums.IpCode:
					member = LoyaltyDataService.LoadMemberFromIPCode(long.Parse(memberId));
					break;
				case LWIntegrationDirectives.MemberIdentityTypeEnums.AlternateId:
					member = LoyaltyDataService.LoadMemberFromAlternateID(memberId);
					break;
				case LWIntegrationDirectives.MemberIdentityTypeEnums.EmailAddress:
					member = LoyaltyDataService.LoadMemberFromEmailAddress(memberId);
					break;
				case LWIntegrationDirectives.MemberIdentityTypeEnums.UserName:
					member = LoyaltyDataService.LoadMemberFromUserName(memberId);
					break;
				case LWIntegrationDirectives.MemberIdentityTypeEnums.LoyaltyIdNumber:
					member = LoyaltyDataService.LoadMemberFromLoyaltyID(memberId);
					break;
			}
			return member;
		}

		protected string ReturnMemberIdentity(Member member)
		{
			string memberIdentity = string.Empty;
			switch (_config.MemberIdentityType)
			{
				case LWIntegrationDirectives.MemberIdentityTypeEnums.IpCode:
					memberIdentity = member.IpCode.ToString();
					break;
				case LWIntegrationDirectives.MemberIdentityTypeEnums.AlternateId:
					memberIdentity = member.AlternateId;
					break;
				case LWIntegrationDirectives.MemberIdentityTypeEnums.EmailAddress:
					memberIdentity = member.PrimaryEmailAddress;
					break;
				case LWIntegrationDirectives.MemberIdentityTypeEnums.UserName:
					memberIdentity = member.Username;
					break;
				case LWIntegrationDirectives.MemberIdentityTypeEnums.LoyaltyIdNumber:
					VirtualCard vc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
					if (vc != null)
					{
						memberIdentity = vc.LoyaltyIdNumber;
					}
					break;
			}
			return memberIdentity;
		}

		protected void ProcessEventRules(object state)
		{
			string methodName = "ProcessEventRules";

			PostProcessState eventState = (PostProcessState)state;

			LWConfigurationUtil.SetCurrentEnvironmentContext(eventState.Organization, eventState.Environment);

			_logger.Trace(_className, methodName, "Executing rules in event  " + eventState.EventName);
			LWEvent lwevent = LoyaltyDataService.GetLWEventByName(eventState.EventName);
			if (lwevent == null)
			{
				string errMsg = string.Format("User define event {0} does not exist.", eventState.EventName);
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 3211 };
			}
			// Find the first member object and use it for the rule.
			Member member = null;
			foreach (object dataobj in eventState.Context.Values)
			{
				member = dataobj as Member;
				if (member != null)
				{
					break;
				}
			}

			if (member == null && eventState.Context.ContainsKey("memberId"))
			{
				long memberId = (long)eventState.Context["memberId"];
				member = LoyaltyDataService.LoadMemberFromIPCode(memberId);
			}

			if (member != null)
			{
				ContextObject ctx = new ContextObject() { Owner = member, Environment = eventState.Context };
				LoyaltyDataService.ExecuteEventRules(ctx, lwevent.Name, RuleInvocationType.Manual);
			}
			else
			{
				string errMsg = string.Format("No member provided to execute user define event {0}.", eventState.EventName);
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 3211 };
			}
		}

		protected void ProcessInterceptor(object state)
		{
			PostProcessState intcptState = (PostProcessState)state;

			LWConfigurationUtil.SetCurrentEnvironmentContext(intcptState.Organization, intcptState.Environment);
			if (!string.IsNullOrEmpty(intcptState.Interceptor.InterceptorAssemlyName) &&
				!string.IsNullOrEmpty(intcptState.Interceptor.InterceptorTypeName))
			{
				using (ISAPIOnSuccessfullInvocation interceptor = GetOnSuccessInterceptor(intcptState.Interceptor))
				{
					interceptor.Invoke(intcptState.Context);
				}
			}
		}

		protected virtual void PostProcessSuccessfullInvocation(Dictionary<string, object> context)
		{
			string methodName = "PostProcessSuccessfullInvocation";

			LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
			LWIntegrationDirectives.OperationDirective opDirective = _config.GetOperationDirective(Name);
			if (opDirective.OnSuccess != null)
			{
				if (!string.IsNullOrEmpty(opDirective.OnSuccess.OnSuccessEvent))
				{
					PostProcessState state = new PostProcessState()
					{
						Organization = ctx.Organization,
						Environment = ctx.Environment,
						Context = context,
						EventName = opDirective.OnSuccess.OnSuccessEvent
					};
					if (opDirective.OnSuccess.IsAsynchronous)
					{
						ThreadPool.QueueUserWorkItem(ProcessEventRules, state);
					}
					else
					{
						ProcessEventRules(state);
					}
				}
				else if (opDirective.OnSuccess.OnSuccessInterceptor != null)
				{
					PostProcessState state = new PostProcessState()
					{
						Organization = ctx.Organization,
						Environment = ctx.Environment,
						Context = context,
						Interceptor = opDirective.OnSuccess.OnSuccessInterceptor
					};
					if (opDirective.OnSuccess.IsAsynchronous)
					{
						ThreadPool.QueueUserWorkItem(ProcessInterceptor, state);
					}
					else
					{
						ProcessInterceptor(state);
					}
				}
				else
				{
					_logger.Debug(_className, methodName, "Empty post processing directive found for " + Name + ".");
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					Cleanup();
					if (DataService != null)
					{
						DataService.Dispose();
					}
					if (ContentService != null)
					{
						ContentService.Dispose();
					}
					if (LoyaltyDataService != null)
					{
						LoyaltyDataService.Dispose();
					}
				}
				_disposed = true;
			}
		}

		protected virtual void Cleanup()
		{
		}

		private static void InitializeUserEventLogger()
		{
			lock (_eventLoggerLock)
			{
				if (_eventLogger == null)
				{
					LWConfiguration lwcfg = LWConfigurationUtil.GetCurrentConfiguration();
					_eventLogger = new LWTriggerUserEventLogger(lwcfg);
					_eventLogger.Start();
				}
			}
		}
	}
}
