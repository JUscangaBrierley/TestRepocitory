//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;

namespace Brierley.FrameWork.LWIntegration.Util
{
	public class InterceptorUtil
	{
		#region Fields
		private const string _className = "InterceptorUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private static Dictionary<string, ILWInterceptor> _intMap = new Dictionary<string, ILWInterceptor>();
		#endregion

		private static string GetInterceptorKey(string typeName, string assemblyName)
		{
			return typeName + "|" + assemblyName;
		}

        private static ILWInterceptor LookupInterceptor(string key)
		{
			lock (_intMap)
			{
				return _intMap.ContainsKey(key) ? _intMap[key] : null;
			}
		}

        private static void AddInterceptor(string key, ILWInterceptor interceptor)
		{
			lock (_intMap)
			{
                if (!_intMap.ContainsKey(key))
                {
                    _intMap.Add(key, interceptor);
                }
			}
		}

        public static ILWInterceptor GetInterceptor(LWIntegrationConfig.InterceptorDirective directive)
		{
            string methodName = "GetInterceptor";

            ILWInterceptor interceptor = null;
			if (directive == null)
			{
				_logger.Debug(_className, methodName, "No interceptor directive provided.");
				return null;
			}

			if (string.IsNullOrEmpty(directive.InterceptorAssemlyName))
			{
				_logger.Debug(_className, methodName, "No interceptor assembly name provided.");
				return null;
			}
			if (string.IsNullOrEmpty(directive.InterceptorTypeName))
			{
				_logger.Debug(_className, methodName, "No interceptor type provided.");
				return null;
			}
			try
			{
				string key = string.Empty;
				if (directive.ReuseForFile)
				{
					key = GetInterceptorKey(directive.InterceptorTypeName, directive.InterceptorAssemlyName);
					interceptor = LookupInterceptor(key);
					if (interceptor != null)
					{
						_logger.Debug(_className, methodName, "Reusing cached interceptor.");
						return interceptor;
					}
				}
				_logger.Debug(_className, methodName, "Creating instance of interceptor " + directive.InterceptorTypeName);
                interceptor = (ILWInterceptor)ClassLoaderUtil.CreateInstance(directive.InterceptorAssemlyName, directive.InterceptorTypeName);
				if (interceptor != null)
				{
					_logger.Debug(_className, methodName, "Initializing interceptor " + directive.InterceptorTypeName);
					interceptor.Initialize(directive.InterceptorParms);
				}
				else
				{
					_logger.Error(_className, methodName, "Unable to load interceptor.");
					throw new LWIntegrationException("Unable to load interceptor") { ErrorCode = 9995 };
				}
				if (directive.ReuseForFile)
				{
					AddInterceptor(key, interceptor);
				}
			}
			catch (LWIntegrationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error loading interceptor,", ex);
				Console.Out.WriteLine("Error loading interceptor: " + ex.Message);
				throw new LWIntegrationException("Error inbound interceptor", ex) { ErrorCode = 9995 };
			}
			return interceptor;
		}        
	}
}
