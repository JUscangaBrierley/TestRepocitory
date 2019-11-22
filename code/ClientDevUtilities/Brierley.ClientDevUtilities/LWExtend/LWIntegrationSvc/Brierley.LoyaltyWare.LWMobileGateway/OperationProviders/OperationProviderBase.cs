using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.ServiceModel.Web;
using System.Threading;
using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders
{
	public abstract class OperationProviderBase : IAPIOperation, IDisposable
	{
		private const string _className = "OperationProviderBase";

		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
		private static Dictionary<string, IMemberResponseHelper> _intMap = new Dictionary<string, IMemberResponseHelper>();
		private static LWTriggerUserEventLogger _eventLogger = null;
		private static object _eventLoggerLock = new object();

		private bool _disposed = false;
		private NameValueCollection _functionProviderParms = null;
		private MobileGatewayDirectives _config = null;
		private DataService _dataService = null;
		private LoyaltyDataService _loyaltyService = null;
		private MobileDataService _mobileService = null;
		private ContentService _contentService = null;
		private SocialDataService _socialService = null;

		private class PostProcessState
		{
			public string Organization { get; set; }
			public string Environment { get; set; }
			public string EventName { get; set; }
			public MobileGatewayDirectives.InterceptorDirective Interceptor { get; set; }
			public Dictionary<string, object> Context { get; set; }
		}

		public MobileGatewayDirectives Config
		{
			get { return _config; }
		}

		public NameValueCollection FunctionProviderParms
		{
			get { return _functionProviderParms; }
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

		protected LoyaltyDataService LoyaltyService
		{
			get
			{
				if (_loyaltyService == null)
				{
					_loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance();
				}
				return _loyaltyService;
			}
		}

		protected MobileDataService MobileService
		{
			get
			{
				if (_mobileService == null)
				{
					_mobileService = LWDataServiceUtil.MobileServiceInstance();
				}
				return _mobileService;
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

		protected SocialDataService SocialService
		{
			get
			{
				if (_socialService == null)
				{
					_socialService = LWDataServiceUtil.SocialServiceInstance();
				}
				return _socialService;
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

		public static IAPIOperation GetOperationProvider(MobileGatewayDirectives config, string opName)
		{
			string methodName = "GetCustomOperationProvider";

			System.Reflection.Assembly assembly = null;
			string providerType = string.Empty;
			string providerAssembly = string.Empty;

			IAPIOperation op = null;
			MobileGatewayDirectives.APIOperationDirective opDirective = config.GetOperationDirectiveByName(opName) as
					MobileGatewayDirectives.APIOperationDirective;
			if (string.IsNullOrEmpty(opDirective.FunctionProvider))
			{
				int index = providerType.LastIndexOf(".");
				if (index == -1)
				{
					providerType = "Brierley.LoyaltyWare.LWMobileGateway.OperationProviders." + opName;
				}
			}
			else
			{
				providerType = opDirective.FunctionProvider;
			}

			if (string.IsNullOrEmpty(opDirective.ProviderAssemblyPath))
			{
				providerAssembly = "Brierley.LoyaltyWare.LWMobileGateway.dll";
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
					//object temp = ClassLoaderUtil.CreateInstance(assembly, providerType);
					op = (IAPIOperation)ClassLoaderUtil.CreateInstance(assembly, providerType);
					op.Initialize(opDirective.Name, config, opDirective.FunctionProviderParms);
				}
				catch (Exception e)
				{
					string errMsg = string.Format("Error loading \"{0}\" from assembly {1}.", providerType, providerAssembly);
					LWException ex = new LWException(errMsg, e) { ErrorCode = 3200 };
					_logger.Error(_className, methodName, "Error loading operation provider.", ex);
					throw ex;
				}
			}
			else
			{
				string errMsg = string.Format("Unable to find assembly {0} containing {1}", providerAssembly, providerType);
				LWException ex = new LWException(errMsg) { ErrorCode = 3201 };
				_logger.Error(_className, methodName, "Error loading operation provider.", ex);
				throw ex;
			}
			return op;
		}

		public abstract object Invoke(string source, WcfAuthenticationToken token, object[] parms);

		public virtual void Initialize(string opName, MobileGatewayDirectives config, NameValueCollection functionProviderParms)
		{
			_config = config;
			Name = opName;
			_functionProviderParms = functionProviderParms;
		}

		public string GetFunctionParameter(string name)
		{
			if (_functionProviderParms != null)
			{
				return _functionProviderParms[name];
			}
			else
			{
				return string.Empty;
			}
		}

		public void Dispose()
		{
			if (_dataService != null)
			{
				_dataService.Dispose();
				_dataService = null;
			}
			if (_loyaltyService != null)
			{
				_loyaltyService.Dispose();
				_loyaltyService = null;
			}
			if (_mobileService != null)
			{
				_mobileService.Dispose();
				_mobileService = null;
			}
			if (_contentService != null)
			{
				_contentService.Dispose();
				_contentService = null;
			}
			if (_socialService != null)
			{
				_socialService.Dispose();
				_socialService = null;
			}
			string methodName = "Dispose";
			_logger.Debug(_className, methodName, string.Format("Function {0} is being disposed.", Name));
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void PostProcessSuccessfullInvocation(Dictionary<string, object> context)
		{
			string methodName = "PostProcessSuccessfullInvocation";

			LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
			MobileGatewayDirectives.OperationDirective opDirective = _config.GetOperationDirectiveByName(Name);
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

		protected virtual void Cleanup()
		{
		}

		protected Member LoadMemberAttributeSets(MobileGatewayDirectives.OperationDirective opDirective, Member member)
		{
			string methodName = "LoadMemberAttributeSets";

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				if (opDirective.ResponseDirective != null)
				{
					if (opDirective.ResponseDirective.ReloadMember && member.IpCode > 0)
					{
						_logger.Trace(_className, methodName, "Reloading member with ipcode " + member.IpCode);
						long ipcode = member.IpCode;
						// we do not want to loose any transient properties that the member has
						ICollection propNames = member.GetTransientPropertyNames();
						Hashtable props = new Hashtable();
						foreach (string propName in propNames)
						{
							props.Add(propName, member.GetTransientProperty(propName));
						}
						member = null;
						member = loyalty.LoadMemberFromIPCode(ipcode);
						if (props.Count > 0)
						{
							foreach (DictionaryEntry entry in props)
							{
								member.UpdateTransientProperty((string)entry.Key, props[entry.Key]);
							}
						}
					}
					IList<string> attributeSetsToLoad = opDirective.ResponseDirective.AttributeSetsToLoad;
					if (attributeSetsToLoad != null && attributeSetsToLoad.Count > 0)
					{
						// load the requested attribute sets                        
						member = (Member)LoadMemberAttributeSets(member, attributeSetsToLoad, 0);
						// If the member response helper is defined then process it.
						IMemberResponseHelper helper = null;
						if (opDirective.ResponseDirective.ResponseHelperDirective != null)
						{
							helper = GetMemberResponseHelper(opDirective.Name, opDirective.ResponseDirective.ResponseHelperDirective);
						}
						if (helper != null)
						{
							try
							{
								_logger.Debug(_className, methodName, "Invoking ProcessMemberAfterAttributeSetLoad method on response helper.");
								member = helper.ProcessMemberAfterAttributeSetLoad(opDirective.Name, opDirective.ResponseDirective, member);
								_logger.Debug(_className, methodName, "Done invoking ProcessMemberAfterAttributeSetLoad method on response helper.");
							}
							catch (LWException ex)
							{
								_logger.Error(_className, methodName, "Error invoking ProcessMemberBeforeAttributeSetLoad method of response helper.", ex);
								throw;
							}
							catch (Exception ex)
							{
								_logger.Error(_className, methodName, "Error invoking ProcessMemberBeforeAttributeSetLoad method of response helper.", ex);
								throw new LWException("Error invoking ProcessMemberBeforeAttributeSetLoad method of response helper.", ex) { ErrorCode = 3111 };
							}
						}
						else
						{
							_logger.Debug(_className, methodName, "No member response helper available to invoke for " + opDirective.Name);
						}
					}
				}
				return member;
			}
		}

		protected ISAPIOnSuccessfullInvocation GetOnSuccessInterceptor(MobileGatewayDirectives.InterceptorDirective dir)
		{
			return InterceptorUtil.GetInterceptor(dir) as ISAPIOnSuccessfullInvocation;
		}

		protected void ProcessEventRules(object state)
		{
			string methodName = "ProcessEventRules";

			PostProcessState eventState = (PostProcessState)state;

			LWConfigurationUtil.SetCurrentEnvironmentContext(eventState.Organization, eventState.Environment);

			_logger.Trace(_className, methodName, "Executing rules in event  " + eventState.EventName);
			LWEvent lwevent = LoyaltyService.GetLWEventByName(eventState.EventName);
			if (lwevent == null)
			{
				string errMsg = string.Format("User define event {0} does not exist.", eventState.EventName);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg) { ErrorCode = 3211 };
			}
			// Find the first member object and use it for the rule.
			Member member = null;
			foreach (object dataobj in eventState.Context.Values)
			{
				if (dataobj is Member)
				{
					member = dataobj as Member;
					break;
				}
			}

			if (member == null && eventState.Context.ContainsKey("memberId"))
			{
				long memberId = (long)eventState.Context["memberId"];
				member = LoyaltyService.LoadMemberFromIPCode(memberId);
			}

			if (member != null)
			{
				ContextObject ctx = new ContextObject() { Owner = member, Environment = eventState.Context };
				LoyaltyService.ExecuteEventRules(ctx, lwevent.Name, RuleInvocationType.Manual);
			}
			else
			{
				string errMsg = string.Format("No member provided to execute user define event {0}.", eventState.EventName);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg) { ErrorCode = 3211 };
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

		protected LWTriggerUserEventLogger EventLogger
		{
			get
			{
				InitializeUserEventLogger();
				return _eventLogger;
			}
		}

		protected bool GetBooleanFunctionalParameter(string parmName, bool defaultValue)
		{
			string boolParmStr = GetFunctionParameter(parmName);
			bool boolParm = defaultValue;
			if (!string.IsNullOrEmpty(boolParmStr))
			{
				boolParm = bool.Parse(boolParmStr);
			}
			return boolParm;
		}

		protected int GetIntegerFunctionalParameter(string parmName, int defaultValue)
		{
			string intParmStr = GetFunctionParameter(parmName);
			int intParm = defaultValue;
			if (!string.IsNullOrEmpty(intParmStr))
			{
				intParm = int.Parse(intParmStr);
			}
			return intParm;
		}

		protected double GetDoubleFunctionalParameter(string parmName, double defaultValue)
		{
			string doubleParmStr = GetFunctionParameter(parmName);
			double doubleParm = defaultValue;
			if (!string.IsNullOrEmpty(doubleParmStr))
			{
				doubleParm = double.Parse(doubleParmStr);
			}
			return doubleParm;
		}

		protected decimal GetDecimalFunctionalParameter(string parmName, decimal defaultValue)
		{
			string doubleParmStr = GetFunctionParameter(parmName);
			decimal doubleParm = defaultValue;
			if (!string.IsNullOrEmpty(doubleParmStr))
			{
				doubleParm = decimal.Parse(doubleParmStr);
			}
			return doubleParm;
		}

		protected void SetResponseCode(HttpStatusCode code)
		{
			var ctx = WebOperationContext.Current;
			ctx.OutgoingResponse.StatusCode = code;
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

		private IMemberResponseHelper LookupInterceptor(string key)
		{
			lock (_intMap)
			{
				return _intMap.ContainsKey(key) ? _intMap[key] : null;
			}
		}

		private void AddInterceptor(string key, IMemberResponseHelper interceptor)
		{
			lock (_intMap)
			{
				_intMap.Add(key, interceptor);
			}
		}

		private IMemberResponseHelper GetMemberResponseHelper(string opName, LWIntegrationConfig.InterceptorDirective directive)
		{
			string methodName = "GetMemberResponseHelper";

			IMemberResponseHelper interceptor = null;
			if (directive == null)
			{
				_logger.Debug(_className, methodName, "No member response helper directive provided.");
				return null;
			}

			if (string.IsNullOrEmpty(directive.InterceptorAssemlyName))
			{
				_logger.Debug(_className, methodName, "No member response helper assembly name provided.");
				return null;
			}
			if (string.IsNullOrEmpty(directive.InterceptorTypeName))
			{
				_logger.Debug(_className, methodName, "No member response helper type provided.");
				return null;
			}
			try
			{
				if (directive.ReuseForFile)
				{
					interceptor = LookupInterceptor(opName);
					if (interceptor != null)
					{
						_logger.Debug(_className, methodName, "Reusing cached member response helper.");
						return interceptor;
					}
				}
				_logger.Debug(_className, methodName, "Creating instance of member response helper " + directive.InterceptorTypeName);
				interceptor = (IMemberResponseHelper)ClassLoaderUtil.CreateInstance(directive.InterceptorAssemlyName, directive.InterceptorTypeName);
				if (interceptor != null)
				{
					_logger.Debug(_className, methodName, "Initializing member response helper " + directive.InterceptorTypeName);
					interceptor.Initialize(directive.InterceptorParms);
				}
				else
				{
					_logger.Error(_className, methodName, "Unable to load member response helper.");
					throw new LWIntegrationException("Unable to load member response helper") { ErrorCode = 9995 };
				}
				if (directive.ReuseForFile)
				{
					AddInterceptor(opName, interceptor);
				}
			}
			catch (LWIntegrationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error loading member response helper,", ex);
				Console.Out.WriteLine("Error loading member response helper: " + ex.Message);
				throw new LWIntegrationException("Error loading member response helper", ex) { ErrorCode = 9995 };
			}
			return interceptor;
		}

		private IAttributeSetContainer LoadAttributeSet(IAttributeSetContainer thisContainer, string attributeSetToLoad)
		{
			using (var ds = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				string[] attributeSets = attributeSetToLoad.Split('/');
				string attributeSetName = attributeSets[0];
				if (attributeSetName == "VirtualCard")
				{
					throw new LWIntegrationException(
						string.Format("Invalid LoadAttributeSet directive {0} found.", attributeSetToLoad))
						{
							ErrorCode = 3000
						};
				}
				AttributeSetMetaData asDef = ds.GetAttributeSetMetaData(attributeSetName);
				if (!thisContainer.IsLoaded(attributeSetName))
				{
					ds.LoadAttributeSetList(thisContainer, attributeSetName, false);
				}
				IList<IClientDataObject> aSet = thisContainer.GetChildAttributeSets(attributeSetName);
				foreach (IClientDataObject row in aSet)
				{
					// now call recursively to process the next
					if (attributeSets.Length > 1)
					{
						string attLoadStr = attributeSetToLoad.Substring(attributeSetToLoad.IndexOf("/") + 1);
						LoadAttributeSet(row, attLoadStr);
					}
				}
				return thisContainer;
			}
		}

		private Member LoadMemberAttributeSets(Member member, IList<string> attributeSetsToLoad, int index)
		{
			bool virtualCardPresent = false;
			foreach (string attributeSetToLoad in attributeSetsToLoad)
			{
				if (!attributeSetToLoad.StartsWith("VirtualCard"))
				{
					member = (Member)LoadAttributeSet(member, attributeSetToLoad);
				}
				else
				{
					virtualCardPresent = true;
				}
			}
			// now process virtual cards
			if (virtualCardPresent)
			{
				foreach (string attributeSetToLoad in attributeSetsToLoad)
				{
					if (attributeSetToLoad.StartsWith("VirtualCard"))
					{
						foreach (VirtualCard vc in member.LoyaltyCards)
						{
							// strip off the VirtualCard part.
							int idx = attributeSetToLoad.IndexOf("VirtualCard/");
							if (idx == 0)
							{
								string astl = attributeSetToLoad.Substring("VirtualCard/".Length);
								LoadAttributeSet(vc, astl);
							}
						}
					}
				}
			}
			return member;
		}

		private void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					Cleanup();
				}
				_disposed = true;
			}
		}
	}
}
