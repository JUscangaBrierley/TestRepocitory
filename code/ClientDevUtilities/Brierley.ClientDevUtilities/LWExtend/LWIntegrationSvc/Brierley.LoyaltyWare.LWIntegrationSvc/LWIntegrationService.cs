using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml.Linq;
using System.Web;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.FrameWork.LWIntegration.IPFiltering;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
using Brierley.LoyaltyWare.LWIntegrationSvc.PerformanceCounters;
using Brierley.FrameWork.Rules;
using System.ServiceModel.Activation;

namespace Brierley.LoyaltyWare.LWIntegrationSvc
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, Namespace = "urn:Brierley.LoyaltyWare.LWIntegrationSvc")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class LWIntegrationService : ILWIntegrationService
	{
		#region Fields
		private const string _className = "LWIntegrationService";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
		private static LWLogger _payloadLogger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTGRSRVC_PAYLOAD);

		private static LWSyncJobLogger _syncJobLogger;
		private static object _syncLoggerLock = new object();

		private static LWIntegrationDirectives _directives;
		private static object _directivesLock = new object();

		private object _requestIdLock = new object();
		private static long _requestId = -1;
		private static long _maxRequestId = -1;
		private static int _requestIdBlockSize = 1000;

		private object _perfLock = new object();
		private SAPIPerformanceCounterUtil _performUtil = null;

		private object _log4netLock = new object();
		private static bool _log4NetInitialized = false;

		private static List<string> _newrelicLogSoapHeaders = null;
		private static List<string> _newrelicLogHttpHeaders = null;

		#endregion

		#region Initialization & Shutdown

		static LWIntegrationService()
		{
			InitializeNewRelicLogging();
		}

		public static void Shutdown()
		{
			if (_syncJobLogger != null)
			{
				lock (_syncJobLogger)
				{
					if (_syncJobLogger.IsAlive())
					{
						_syncJobLogger.ShutDown();
						_syncJobLogger.WaitToFinish();
					}
				}
			}
			LWFailedMessageLogManager.Shutdown();
			OperationProviders.OperationProviderBase.Shutdown();
		}
		#endregion

		#region Private Helpers

		#region API Stats & Message Logging

		private static void InitializeNewRelicLogging()
		{
			char[] split = new char[] { ',' };

			string logHttp = System.Configuration.ConfigurationManager.AppSettings["newrelicLogHttpHeaders"];
			if (!string.IsNullOrEmpty(logHttp))
			{
				_newrelicLogHttpHeaders = logHttp.Split(split, StringSplitOptions.RemoveEmptyEntries).ToList();
			}

			string logSoap = System.Configuration.ConfigurationManager.AppSettings["newrelicLogSoapHeaders"];
			if (!string.IsNullOrEmpty(logSoap))
			{
				_newrelicLogSoapHeaders = logSoap.Split(split, StringSplitOptions.RemoveEmptyEntries).ToList();
			}
		}

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

		private static void InitializeSyncLogger()
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
					using (DataService service = LWDataServiceUtil.DataServiceInstance())
					{
						_requestId = service.GetNextID("LIBJob", _requestIdBlockSize);
					}
					_maxRequestId = _requestId + _requestIdBlockSize;
				}
				else
				{
					_requestId++;
				}
			}
			return _requestId;
		}

		private SyncJob StartRequest(string opName, string source, string sourceEnv, string payload, string externalId)
		{
			SyncJob job = null;
			job = new SyncJob();
			job.Source = source;
			job.SourceEnv = sourceEnv;
			job.ExternalId = externalId;
			job.ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			job.Start();
			job.OperationName = opName;
			if (_performUtil != null)
			{
				_performUtil.IncrementNumberOfRequestsReceived();
				_performUtil.IncrementNumberOfOutstandingRequests();
			}
			job.MessageId = GetNextRequestId();
			_logger.AssignJobId(job.MessageId.ToString());
			return job;
		}

		private SyncJob StartRequest(LWIntegrationDirectives.OperationDirective directive, string source, string sourceEnv, string externalId, string payload)
		{
			return StartRequest(directive.Name, source, sourceEnv, payload, externalId);
		}
		
		private void EndRequest(LWIntegrationDirectives.OperationDirective directive, SyncJob job, string payload, int responseCode, string responseDetail, List<NewRelicAttribute> newRelicAttributes)
		{
			const string methodName = "EndRequest";
			try
			{
				if (job != null)
				{
					InitializeSyncLogger();
					job.End();
					job.Status = responseCode;
					if (directive != null && (directive.OperationMetadata.OperationOutput == null || directive.OperationMetadata.OperationOutput.LogParameters))
					{
						string attributes = string.Empty;
						if (newRelicAttributes != null && newRelicAttributes.Count > 0)
						{
							attributes = String.Join(", ", newRelicAttributes);
						}

						log4net.ThreadContext.Properties["MessageID"] = job.MessageId;
						log4net.ThreadContext.Properties["OperationParm"] = payload;
						log4net.ThreadContext.Properties["Response"] = responseDetail;
						log4net.ThreadContext.Properties["Attributes"] = attributes;

						_payloadLogger.Trace("Message ID: " + job.MessageId + " OperationParm: " + payload + " Response: " + responseDetail + " Attributes: " + attributes);
					}
					_syncJobLogger.RequestQueue.Add(job);
					_logger.ClearJobId();
				}
				if (_performUtil != null)
				{
					_performUtil.DecrementNumberOfOutstandingRequests();
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
			}
		}
		#endregion

		#region General
		private static void SetEnvironment()
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

		private void InitializePerformaceCounters()
		{
			string strMonitor = LWConfigurationUtil.GetConfigurationValue("LWSAPIPerformanceCounters");
			bool perCounters = !string.IsNullOrEmpty(strMonitor) ? bool.Parse(strMonitor) : false;
			if (perCounters)
			{
				lock (_perfLock)
				{
					if (_performUtil == null)
					{
						_performUtil = new SAPIPerformanceCounterUtil();
					}
				}
			}
		}

		#endregion

		#region Directives
		private static void InitializeDirectives()
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
					_directives = new LWIntegrationDirectives();
					_directives.Load(configFile);
				}
			}
		}

		private static LWIntegrationDirectives.OperationDirective GetOperationDirective(string opName)
		{
			string methodName = "GetOperationDirective";
			InitializeDirectives();
			LWIntegrationDirectives.OperationDirective opDirective = _directives.GetOperationDirective(opName);
			if (opDirective == null)
			{
				string error = string.Format("No configuration exists for operation {0}.  Not yet Implemented.", opName);
				_logger.Critical(_className, methodName, error);
				throw new LWIntegrationCfgException(error) { ErrorCode = 9999 };
			}
			return opDirective;
		}

		#endregion

		#region Response
		private static LWAPIResponse CreateResponseFromException(LWIntegrationDirectives.OperationDirective op, Exception ex, long messageId)
		{
			LWAPIResponse response = new LWAPIResponse() { ResponseCode = 1, ResponseDescription = "Failed" };

            LWException lex = ex as LWException;
            if (lex != null)
                response.ResponseCode = lex.ErrorCode != 0 ? lex.ErrorCode : 1;

            string message = ex.Message;
            if (op != null &&
                op.Type == LWIntegrationDirectives.OperationType.FrameworkManaged &&
                !string.IsNullOrEmpty(((LWIntegrationDirectives.FrameworkManagedOperationDirective)op).GenericErrorMessage))
                message = ((LWIntegrationDirectives.FrameworkManagedOperationDirective)op).GenericErrorMessage;

            response.ResponseDetail = string.Format("{0}. Message id = {1}", message, messageId);
			return response;
		}
		#endregion

		#region IP Restrictions

		private static void CheckForEndpointRestrictions(LWIntegrationDirectives.OperationDirective opDirective)
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

        #region Request Processing

        #region Member Processing

        /// <summary>
        /// This method processes received member info (stored in XML) based on configuration
        /// and saves member and all its child attribute sets into database. 
        /// </summary>
        private static Member ProcessMember(LWIntegrationDirectives config, LWIntegrationDirectives.FrameworkManagedOperationDirective fwkOp, IInboundInterceptor interceptor, XElement memberNode, /*long messageId, */out decimal pointsReturned)
        {
            string methodName = "ProcessMember";

            memberNode = ProcessInterceptor(interceptor, "ProcessRawXml", memberNode, config, memberNode);

            Member member = LWIntegrationUtilities.GetMember(config, memberNode, fwkOp.MemberLoadDirectives, interceptor);

            member = ProcessInterceptor(interceptor, "ProcessMemberBeforePopulation", member, config, member, memberNode);

            // process member attributes
            LWIntegrationUtilities.LoadMemberAttributes(config, member, memberNode, config.TrimStrings, config.GetDateConversionFormat(), config.CheckForChangedValues);
            LWIntegrationUtilities.ProcessAttributeSet(config, fwkOp.CreateDreatives, member, memberNode, null, config.GetDateConversionFormat(), config.TrimStrings, config.CheckForChangedValues);

            RuleExecutionMode mode = RuleExecutionMode.Real;
            try
            {
                mode = member.HasTransientProperty("executionmode") ? (RuleExecutionMode)Enum.Parse(typeof(RuleExecutionMode), member.GetTransientProperty("executionmode").ToString(), true) : mode;
            }
            catch
            {
                string err = string.Format("Invalid execution mode {0} specified.  Valid values are real or simulation", member.GetTransientProperty("executionmode").ToString());
                throw new LWIntegrationException(err) { ErrorCode = 3231 };
            }

            // now let us process VirtualCard                
            LWIntegrationUtilities.ProcessVirtualCards(config, memberNode, member, fwkOp.CreateDreatives, config.TrimStrings, config.CheckForChangedValues);

            // The third interceptor point allows final modification of the member before it is saved.
            member = ProcessInterceptor(interceptor, "ProcessMemberBeforeSave", member, config, member, memberNode);

            // If the member has been updated and they didn't designate a ChangedBy value
            if (member.IsDirty && string.IsNullOrEmpty(LWIntegrationUtilities.GetAttributeValue(false, memberNode.Attribute("ChangedBy"))))
            {
                member.ChangedBy = "LWIntegrationService";
            }

            List<ContextObject.RuleResult> results = new List<ContextObject.RuleResult>();
            using (LoyaltyDataService svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                svc.ClearRuleResult(member);
                svc.SaveMember(member, results, mode);
            }

            // Calculate poins earned
            pointsReturned = results.Sum(x => x is AwardPointsRuleResult ? ((AwardPointsRuleResult)x).PointsAwarded : 0);

            member = ProcessInterceptor(interceptor, "ProcessMemberAfterSave", member, config, member, memberNode, results);

            _logger.Debug(_className, methodName, "Saved member with Ipcode " + member.MyKey.ToString());
            return member;
        }
		#endregion

		#region Global Processing
		private static void ProcessGlobal(LWIntegrationDirectives config, LWIntegrationDirectives.FrameworkManagedOperationDirective fwkOp, IInboundInterceptor interceptor, XElement globalNode/*, long messageId*/)
		{
			string methodName = "ProcessGlobal";

            globalNode = ProcessInterceptor(interceptor, "ProcessRawXml", globalNode, config, globalNode);

			using (LoyaltyDataService service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData asDef = service.GetAttributeSetMetaData(globalNode.Name.LocalName);
				if (asDef.Type != AttributeSetType.Global || asDef.ParentID != -1)
				{
					string msg = string.Format("A global attribute set was expected.  Found something else.");
					_logger.Error(_className, methodName, msg);
					throw new LWOperationInvocationException(msg) { ErrorCode = 9975 };
				}
				LWIntegrationConfig.AttributeSetDirective directive =
						fwkOp.CreateDreatives.ContainsKey(globalNode.Name.LocalName) ?
						fwkOp.CreateDreatives[globalNode.Name.LocalName] : null;
				if (directive == null)
				{
					_logger.Error(_className, methodName, "No directive found for " + globalNode.Name.LocalName);
					throw new LWIntegrationException("No directive found for " + globalNode.Name.LocalName);
				}
				IList<IClientDataObject> rows = LWIntegrationUtilities.GetGlobalAttributeSetsToModify(directive, asDef, globalNode);
				if (rows != null)
				{
					foreach (IClientDataObject aRow in rows)
					{
						LWIntegrationUtilities.ProcessAttributeSet(config, fwkOp.CreateDreatives, aRow, globalNode, asDef, config.GetDateConversionFormat(), config.TrimStrings, config.CheckForChangedValues);
						_logger.Debug(_className, methodName,
							"Row key = " + aRow.MyKey);
						service.SaveAttributeSetObject(aRow);
					}
				}
			}
		}

		#endregion

        private static T ProcessInterceptor<T>(IInboundInterceptor interceptor, string method, T retVal, params object[] parameters)
        {
            string methodName = "ProcessInterceptor";

            if (interceptor != null)
            {
                try
                {
                    var methodInfo = interceptor.GetType().GetMethod(method);
                    _logger.Debug(_className, methodName, string.Format("Invoking {0} on interceptor.", method));
                    retVal = (T)methodInfo.Invoke(interceptor, parameters);
                }
                catch (Exception iex)
                {
                    string msg = string.Format("Error generated by {0}.{1}", interceptor.GetType().ToString(), method);
                    _logger.Error(_className, methodName, msg, iex);
                    if (iex is TargetInvocationException)
                        throw iex.InnerException;
                    throw;
                }
            }
            return retVal;
        }

		private static string ProcessFrameworkRequest(LWIntegrationDirectives config, LWIntegrationDirectives.OperationDirective op,/* long messageId, */string payload)
		{
			string methodName = "ProcessFrameworkRequest";

			string response = string.Empty;
			try
			{
				LWIntegrationDirectives.FrameworkManagedOperationDirective fwkOp = op as LWIntegrationDirectives.FrameworkManagedOperationDirective;

				#region Extract the payload
				string errMsg = string.Empty;
				XDocument inDoc = XDocument.Parse(payload);
				XElement envelop = inDoc.Root;
				if (envelop.Name.LocalName != (op.Name + "InParms"))
				{
					errMsg = string.Format("Expected {0}InParms as the envelope.  Found {1}", op.Name, envelop.Name.LocalName);
					_logger.Error(_className, methodName, errMsg);
					throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3101 };
				}
				XElement memberParmNode = (XElement)envelop.FirstNode;
				XAttribute typeAtt = memberParmNode.Attribute("Type");
				if (typeAtt.Value != "Member" && typeAtt.Value != "Global")
				{
					// throw an exception here
					errMsg = string.Format("Expected parameter of type member or global.  Found {0}", typeAtt.Value);
					_logger.Error(_className, methodName, errMsg);
					throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3106 };
				}
				XElement msgRootNode = (XElement)memberParmNode.FirstNode;
				if (msgRootNode.Name.LocalName != "AttributeSets")
				{
					errMsg = string.Format("Expected {0}In as the enclosing type.  Found {1}", op.Name, msgRootNode.Name.LocalName);
					_logger.Error(_className, methodName, errMsg);
					throw new LWResponseMarshallingException(errMsg) { ErrorCode = 3101 };
				}
				#endregion

				if (msgRootNode != null)
				{
					IInboundInterceptor interceptor = InterceptorUtil.GetInterceptor(fwkOp.InterceptorDirective) as IInboundInterceptor;
					if (msgRootNode.Element("Member") != null)
					{
						decimal pointsReturned;
						Member member = ProcessMember(config, fwkOp, interceptor, msgRootNode.Element("Member"),
							/*messageId,*/ out pointsReturned);

						member.UpdateTransientProperty("PointsEarned", pointsReturned);

						response = SerializationUtils.SerializeResult(op.Name, config, member);
					}
					else
					{
						IEnumerable<System.Xml.Linq.XElement> globalRootList = msgRootNode.Elements("Global");
						if (globalRootList != null && globalRootList.Count() > 0)
						{
							foreach (XElement globalRoot in globalRootList)
							{
								foreach (XElement global in globalRoot.Elements())
								{
									/*
                                    * We should be able to process all the messages rather than stop the first
                                    * an exception is raised.
                                    **/
									ProcessGlobal(config, fwkOp, interceptor, global/*, messageId*/);
								}
								_logger.Debug(_className, methodName, "Finished processing of message.");
							}
						}
						else
						{
							errMsg = string.Format("Expected Global type data.  Found something else.");
							_logger.Error(_className, methodName, errMsg);
							throw new LWOperationInvocationException(errMsg) { ErrorCode = 9974 };
						}
					}
				}
			}
			catch (LWIntegrationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error processing member.", ex);
				throw;
			}
			finally
			{
			}
			return response;
		}

		private static string ProcessCustomRequest(string source, LWIntegrationDirectives config, LWIntegrationDirectives.OperationDirective opDirective, /*long messageId,*/ string payload)
		{
			string methodName = "ProcessCustomRequest";

			string response = string.Empty;
			try
			{
				using (IAPIOperation provider = OperationProviderBase.GetCustomOperationProvider(config, opDirective.Name))
				{
					if (provider != null)
					{
						response = provider.Invoke(source, payload);
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
				throw new LWOperationInvocationException(ex.Message, ex);
			}
			return response;
		}
		#endregion

		#region Service Contract
		public LWAPIResponse Execute(string operationName, string source, string sourceEnv, string externalId, string payload, List<NewRelicAttribute> newRelicAttributes)
		{
			string methodName = "Execute";

			SyncJob job = null;
			LWAPIResponse response = null;
			LWIntegrationDirectives.OperationDirective opDirective = null;
			InitializeLog4Net();

			try
			{

				if (newRelicAttributes == null)
				{
					newRelicAttributes = new List<NewRelicAttribute>();
				}

				if (_newrelicLogHttpHeaders != null && _newrelicLogHttpHeaders.Count > 0)
				{
					if (HttpContext.Current == null)
					{
						_logger.Warning(_className, methodName, "App setting, NewRelicLogHttpHeaders is set to true, but the service is unable to access the current HTTP Context. Please ensure that asp.net compatibility mode is enabled.");
					}

					foreach (string key in HttpContext.Current.Request.Headers)
					{
						if (_newrelicLogHttpHeaders.Contains(key))
						{
							newRelicAttributes.Add(new NewRelicAttribute(key, HttpContext.Current.Request.Headers[key]));
						}
					}
				}

				if (_newrelicLogSoapHeaders != null && _newrelicLogSoapHeaders.Count > 0)
				{
					var soapHeaders = OperationContext.Current.IncomingMessageHeaders;

					for (int i = 0; i < soapHeaders.Count; i++)
					{
						string name = soapHeaders[i].Name;

						if (_newrelicLogSoapHeaders.Contains(name))
						{
							string value = soapHeaders.GetHeader<string>(i);
							newRelicAttributes.Add(new NewRelicAttribute(name, value));
						}
					}
				}

				if (newRelicAttributes != null)
				{
					foreach (var attribute in newRelicAttributes)
					{
						NewRelic.Api.Agent.NewRelic.AddCustomParameter(attribute.Key, attribute.Value);
					}
				}

				SetEnvironment();
				InitializePerformaceCounters();
				try
				{
					opDirective = GetOperationDirective(operationName);
					job = StartRequest(opDirective, source, sourceEnv, externalId, payload);
				}
				catch (Exception)
				{
					job = StartRequest(operationName, source, sourceEnv, externalId, payload);
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
							validator.ValidateOperationParameter(operationName, source, payload);
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
				string responseDetail = string.Empty;
				switch (opDirective.Type)
				{
					case LWIntegrationDirectives.OperationType.FrameworkManaged:
						responseDetail = ProcessFrameworkRequest(_directives, opDirective, /*job.MessageId,*/ payload);
						break;
					case LWIntegrationDirectives.OperationType.Custom:
						responseDetail = ProcessCustomRequest(source, _directives, opDirective, /*job.MessageId,*/ payload);
						break;
				}

				response = new LWAPIResponse() { ResponseCode = 0, ResponseDescription = "Success", ResponseDetail = responseDetail };

			}
			catch (Exception ex)
			{
				LWFailedMessageLogManager.Initialize(LWConfigurationUtil.GetCurrentConfiguration());
				LIBMessageLog errorMsg = new LIBMessageLog();
				errorMsg.Message = string.IsNullOrEmpty(payload) ? "none" : payload;
				errorMsg.Exception = ex;
				if (job != null)
				{
					errorMsg.JobNumber = job.MessageId;
				}
				LWFailedMessageLogManager.LogFailedMessage(LWConfigurationUtil.GetCurrentConfiguration(), errorMsg);
                response = CreateResponseFromException(opDirective, ex, job != null ? job.MessageId : -1);
				_logger.Error(_className, methodName, ex.Message, ex);
			}
			finally
			{
				if (job != null)
				{
					EndRequest(opDirective, job, payload, response.ResponseCode, response.ResponseDetail, newRelicAttributes);
					response.ElapsedTime = job.ElapsedTime.GetValueOrDefault();
				}
			}
			return response;
		}
		#endregion
	}
}
