using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net;
using System.Reflection;

using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;

using System.Web.Script.Serialization;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.FrameWork.LWIntegration.IPFiltering;

using Brierley.LoyaltyWare.LWMobileGateway.OperationProviders;
using Brierley.LoyaltyWare.LWMobileGateway.Authorization;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway
{
	public class MobileGatewayImpl
	{
		#region Fields
		private const string _className = "LWIntegrationService";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		private static LWSyncJobLogger _syncJobLogger;
		private static object _syncLoggerLock = new object();

		private static MobileGatewayDirectives _directives;
		private static object _directivesLock = new object();

		private object _requestIdLock = new object();
		private static long _requestId = -1;
		private static long _maxRequestId = -1;
		private static int _requestIdBlockSize = 1000;

		private object _log4netLock = new object();
		private static bool _log4NetInitialized = false;

		#endregion

		#region Private Helpers

		#region API Stats & Message Logging

		private void InitializeLog4Net()
		{
			const string methodName = "InitializeLog4net";
			lock (_log4netLock)
			{
				if (!_log4NetInitialized)
				{
					try
					{
						string log4netConfigFile = string.Empty;
						HttpContext httpContext = HttpContext.Current;
						if (httpContext != null)
						{
							log4netConfigFile = httpContext.Server.MapPath("~/log4net.xml");
						}
						else
						{
							log4netConfigFile = System.Web.Hosting.HostingEnvironment.MapPath("~/log4net.xml");
						}
						if (string.IsNullOrWhiteSpace(log4netConfigFile))
						{
							// maybe this is configured as a windows service
							log4netConfigFile = Brierley.FrameWork.Common.IO.IOUtils.AppendSeparatorToFolderPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + "log4net.xml";
						}
						if (File.Exists(log4netConfigFile))
						{
							log4net.Config.XmlConfigurator.Configure(new FileInfo(log4netConfigFile));
							_logger.Trace(_className, methodName, string.Format("Initialized log4net using config file '{0}'", log4netConfigFile));
						}
						else
						{
							_logger.Error(_className, methodName, string.Format("Failed to initialize log4net: missing config file '{0}'", log4netConfigFile));
						}
						_log4NetInitialized = true;
					}
					catch (Exception)
					{
					}
				}
			}
		}

		private void InitializeSyncLogger()
		{
			lock (_syncLoggerLock)
			{
				if (_syncJobLogger == null)
				{
					LWConfiguration lwcfg = LWConfigurationUtil.GetCurrentConfiguration();
					_syncJobLogger = new LWSyncJobLogger(lwcfg);
					_syncJobLogger.Start();
				}
			}
		}

		private long GetNextRequestId()
		{
			lock (_requestIdLock)
			{
				if (_requestId == -1 || _requestId == _maxRequestId - 1)
				{
					// initialize request id
					using (var svc = LWDataServiceUtil.DataServiceInstance())
					{
						_requestId = svc.GetNextID("LIBJob", _requestIdBlockSize);
						_maxRequestId = _requestId + _requestIdBlockSize;
					}
				}
				else
				{
					_requestId++;
				}
			}
			return _requestId;
		}

		private SyncJob StartRequest(string opName, string source, string payload, bool logRequest)
		{
			SyncJob job = null;
			job = new SyncJob();
			job.Source = source;
			job.Start();
			job.OperationName = opName;
			if (logRequest)
			{
				job.OperationParm = payload;
			}
			job.MessageId = GetNextRequestId();
			_logger.AssignJobId(job.MessageId.ToString());
			return job;
		}

		private SyncJob StartRequest(MobileGatewayDirectives.OperationDirective directive, string source, string payload, bool logRequest)
		{
			return StartRequest(directive.Name, source, payload, logRequest);
		}

		private SyncJob StartRequest(string opName, string source, string payload)
		{
			return StartRequest(opName, source, payload, true);
		}

		private void EndRequest(MobileGatewayDirectives.OperationDirective directive, SyncJob job, int responseCode, object result)
		{
			const string methodName = "EndRequest";
			try
			{
				if (job != null)
				{
					InitializeSyncLogger();
					job.End();
					job.Status = responseCode;
					if (directive != null && (directive.LogResponse))
					{
						job.Response = SerializeResponse(result);
					}
					_syncJobLogger.RequestQueue.Add(job);
					_logger.ClearJobId();
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
			}
		}
		#endregion

		#region General
		protected void SetEnvironment()
		{
			string orgName = System.Configuration.ConfigurationManager.AppSettings["LWOrganization"];
			if (string.IsNullOrEmpty(orgName))
			{
				string error = "No organization has been defined.  Please define LWOrganization property";
				throw new LWIntegrationCfgException(error) { ErrorCode = 9996 };
			}
			string envName = System.Configuration.ConfigurationManager.AppSettings["LWEnvironment"];
			if (string.IsNullOrEmpty(orgName))
			{
				string error = "No environment has been defined.  Please define LWEnvironment property";
				throw new LWIntegrationCfgException(error) { ErrorCode = 9997 };
			}
			LWConfigurationUtil.SetCurrentEnvironmentContext(orgName, envName);
		}
		#endregion

		#region Method Invocation

		private string SerializeObjectToJson(object value)
		{
			string strvalue = string.Empty;
			if (value != null)
			{
				var json = new JavaScriptSerializer().Serialize(value);
				strvalue = json.ToString();
			}
			return strvalue;
		}

		private string SerializeRequestParms(object[] parms)
		{
			string strRequest = string.Empty;
			if (parms != null && parms.Length > 0)
			{
				foreach (object parm in parms)
				{
					if (string.IsNullOrEmpty(strRequest))
					{
						strRequest += ":::";
					}
					string strValue = SerializeObjectToJson(parm);
					strRequest += strValue;
				}
			}
			return strRequest;
		}

		private string SerializeResponse(object response)
		{
			return SerializeObjectToJson(response);
		}

		private object ProcessRequest(string source, WcfAuthenticationToken token, MobileGatewayDirectives config, MobileGatewayDirectives.OperationDirective opDirective, long messageId, object[] parms)
		{
			string methodName = "ProcessRequest";

			object response = null;
			try
			{
				using (IAPIOperation provider = OperationProviderBase.GetOperationProvider(config, opDirective.Name))
				{
					if (provider != null)
					{
						response = provider.Invoke(source, token, parms);
					}
					else
					{
						string msg = string.Format("Unable to create an instance of the provider for {0}", opDirective.Name);
						throw new LWOperationInvocationException(msg) { ErrorCode = 3200 };
					}
				}
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error processing member.", ex);
				//LWOperationInvocationException leads to BadRequest (400) being returned by the service. This should be a InternalServerError (500) response, 
				//so we'll not wrap the exception in LWOperationInvocationException. Otherwise, we can't tell an unhandled exception from a simple validation issue.
				//BadRequest (400) should be returned for (but not limited to): "Invalid parameters provided for <method>."
				//InternalServerError (500) should be returned for any exception that we were unable to prevent/trap and wrap a reasonable error message around.
				//throw new LWOperationInvocationException(ex.Message, ex);
				throw;
			}
			finally
			{
			}
			return response;
		}
		#endregion

		#region Directives
		private void InitializeDirectives()
		{
			string methodName = "InitializeDirectives";

			lock (_directivesLock)
			{
				if (_directives == null)
				{
					_logger.Trace(_className, methodName, "Initializing directives.");
					string configFile = System.Configuration.ConfigurationManager.AppSettings["LWIntgrConfig"];
					if (string.IsNullOrEmpty(configFile))
					{
						string error = "Unable to find the configuration file path.  Please define property LWIntgrConfig.";
						_logger.Critical(_className, methodName, error);
						throw new LWIntegrationCfgException(error);
					}
					if (!File.Exists(configFile))
					{
						string error = string.Format("Configuration file {0} does not exist.", configFile);
						_logger.Critical(_className, methodName, error);
						throw new LWIntegrationCfgException(error);
					}
					_directives = new MobileGatewayDirectives();
					_directives.Load(configFile);
				}
			}
		}

		private MobileGatewayDirectives.OperationDirective GetOperationDirective(string opName)
		{
			string methodName = "GetOperationDirective";
			InitializeDirectives();
			MobileGatewayDirectives.OperationDirective opDirective = _directives.GetOperationDirectiveByName(opName);
			if (opDirective == null)
			{
				string error = string.Format("No configuration exists for operation {0}.  Not yet Implemented.", opName);
				_logger.Critical(_className, methodName, error);
				throw new LWIntegrationCfgException(error) { ErrorCode = 9999 };
			}
			return opDirective;
		}

		#endregion

		#region IP Restrictions

		private void CheckForEndpointRestrictions(MobileGatewayDirectives.OperationDirective opDirective)
		{
			string methodName = "CheckForEndpointRestrictions";

			if (opDirective.EndPointRestrictions == null || !opDirective.EndPointRestrictions.HasRestrictions)
			{
				// no restrictions have been defined for this operartion.
				return;
			}

			var props = OperationContext.Current.IncomingMessageProperties;
			var endpointProperty = props[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
			/*
			 * This property is only available when the configured endpoint is either http ot tcp.
			 * */
			if (endpointProperty != null)
			{
				_logger.Debug(_className, methodName, "Ip address: " + endpointProperty.Address);
				_logger.Debug(_className, methodName, endpointProperty.Port.ToString());
				IPAddress addr = IPAddress.Parse(endpointProperty.Address);
				IPHostEntry entry = Dns.GetHostEntry(addr);
				_logger.Debug(_className, methodName, "Host name: " + entry.HostName);
				if (opDirective.EndPointRestrictions.AllowList != null && opDirective.EndPointRestrictions.AllowList.Count > 0)
				{
					// check to make sure that the client end point is in the list.
					if (!IPRange.IsAddressInRange(opDirective.EndPointRestrictions.AllowList, addr))
					{
						// this address is not in the allow list
						string error = string.Format("Unauthorized access to operation {0}.  Ip address {1} is not in the allow list.", opDirective.Name, INetUtil.GetStringIpAddress(INetUtil.GetIPv4Address(addr)));
						_logger.Critical(_className, methodName, error);
						throw new LWIntegrationCfgException(error) { ErrorCode = 9998 };
					}
				}
				if (opDirective.EndPointRestrictions.DenyList != null && opDirective.EndPointRestrictions.DenyList.Count > 0)
				{
					// check to make sure that the client end point is not in the disallow list.
					if (IPRange.IsAddressInRange(opDirective.EndPointRestrictions.DenyList, addr))
					{
						// this address is not in the allow list
						string error = string.Format("Unauthorized access to operation {0}.  Ip address {1} is in the deny list.", opDirective.Name, INetUtil.GetStringIpAddress(INetUtil.GetIPv4Address(addr)));
						_logger.Critical(_className, methodName, error);
						throw new LWIntegrationCfgException(error) { ErrorCode = 9998 };
					}
				}
			}
		}

		#endregion

		#endregion

		#region Authorization
		protected IAuthorizationInterceptor GetAuthInterceptor()
		{
			string methodName = "GetAuthInterceptor";

			IAuthorizationInterceptor authIntctp = null;

			if (_directives.AuthorizationInterceptor != null)
			{
				authIntctp = InterceptorUtil.GetInterceptor(_directives.AuthorizationInterceptor) as IAuthorizationInterceptor;
				if (authIntctp == null)
				{
					_logger.Error(_className, methodName, "Unable to instantiate authorization interceptor.");
					throw new LWException("Unable to instantiate authorization interceptor.") { ErrorCode = 1 };
				}
			}
			return authIntctp;
		}

		protected WcfAuthenticationToken CheckAuthorization(string clientId, MobileGatewayDirectives.OperationDirective opDirective)
		{
			string methodName = "CheckAuthentication";

			if (opDirective.RequiresAuthorization)
			{
				IAuthorizationInterceptor authInterceptor = GetAuthInterceptor();
				return authInterceptor.CheckAuthorization(clientId);
			}
			else
			{
				_logger.Trace(_className, methodName,
					string.Format("Method {0} does not require authentication.", opDirective.Name));
				return null;
			}
		}

		#endregion

		#region Protected Operations
		protected object InvokeOperation(string operationName, object[] parms)
		{
			string methodName = "InvokeOperation";

			SyncJob job = null;
			object result = null;
			string requestPayLoad = string.Empty;
			MobileGatewayDirectives.OperationDirective opDirective = null;
			WcfAuthenticationToken token = null;
			System.Net.HttpStatusCode resultCode = HttpStatusCode.OK;
			InitializeLog4Net();
			_logger.Trace(_className, methodName, string.Format("Invoking operation {0}.", operationName));
			try
			{
				SetEnvironment();
				try
				{
					opDirective = GetOperationDirective(operationName);
					if (opDirective.LogRequest)
					{
						requestPayLoad = SerializeRequestParms(parms);
					}
					job = StartRequest(operationName, string.Empty, requestPayLoad, opDirective.LogRequest);
					token = CheckAuthorization(_directives.ClientId, opDirective);
                    if (token != null && token.PasswordResetRequired && operationName != "ChangePassword")
                        throw new AuthorizationException("Password change is required");
				}
				catch (Exception)
				{
					job = StartRequest(operationName, string.Empty, string.Empty);
					throw;
				}
				if (!opDirective.IsAuthorized)
				{
					string error = string.Format("Unauthorized access to operation {0}.", operationName);
					_logger.Critical(_className, methodName, error);
					throw new LWIntegrationCfgException(error) { ErrorCode = 9998 };
				}
				CheckForEndpointRestrictions(opDirective);
				if (_directives.GlobalOperationValidator != null)
				{
					try
					{
						IInboundInterceptor validator = InterceptorUtil.GetInterceptor(_directives.GlobalOperationValidator) as IInboundInterceptor;
						if (validator != null)
						{
							validator.ValidateOperationParameter(operationName, string.Empty, requestPayLoad);
						}
						else
						{
							_logger.Error(_className, methodName, "Unable to instantiate global parameter validator.");
						}
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Error thrown by global parameter validator.", ex);
						throw;
					}
				}
				result = ProcessRequest(string.Empty, token, _directives, opDirective, job.MessageId, parms);
			}
			catch (Exception ex)
			{
				LWFailedMessageLogManager.Initialize(LWConfigurationUtil.GetCurrentConfiguration());
				LIBMessageLog errorMsg = new LIBMessageLog();
				errorMsg.Message = string.IsNullOrEmpty(requestPayLoad) ? "none" : requestPayLoad;
				errorMsg.Exception = ex;
				if (job != null)
				{
					errorMsg.JobNumber = job.MessageId;
				}
				LWFailedMessageLogManager.LogFailedMessage(LWConfigurationUtil.GetCurrentConfiguration(), errorMsg);
				string error = string.Format("Error invoking operation {0}.", operationName);
				_logger.Error(_className, methodName, error, ex);
				if (ex is AuthorizationException)
				{
					resultCode = HttpStatusCode.Forbidden;
				}
				else if (ex is LWException)
				{
					resultCode = HttpStatusCode.BadRequest;
				}
				else
				{
					resultCode = HttpStatusCode.InternalServerError;
				}
				throw new WebFaultException<string>(ex.Message, resultCode);
			}
			finally
			{
				if (job != null)
				{
					EndRequest(opDirective, job, (int)resultCode, result);
				}
				_logger.Trace(_className, methodName, string.Format("Operation {0} finished.", operationName));
			}
			return result;
		}
		#endregion
	}
}