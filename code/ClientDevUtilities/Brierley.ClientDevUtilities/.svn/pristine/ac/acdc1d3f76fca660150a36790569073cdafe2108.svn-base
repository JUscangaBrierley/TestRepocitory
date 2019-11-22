//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web;
using System.Xml;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.Common.Config
{
    public static class LWConfigurationUtil
    {
        private const string LW_ENV_SLOT = "LWEnvSlot";        
        private static Dictionary<string, LWConfiguration> _configMap = new Dictionary<string, LWConfiguration>();

		/// <summary>
		/// gets or sets whether the current application is a queue processor.
		/// </summary>
		/// <remarks>
		/// When true, queue eligible items will not be queued (e.g., a triggered email will be fully sent, rather than sent to a message queue).
		/// </remarks>
		public static bool IsQueueProcessor { get; set; }

        /// <summary>
        /// Determines whether the LWConfiguration for a particular entity (i.e., organization/environment tuple)
        /// exists in the cached set of configurations.
        /// </summary>
        /// <param name="orgName">the organization name</param>
        /// <param name="envName">the environment name</param>
        /// <returns>true if cached entity exists, false otherwise</returns>
        public static bool ConfigurationExistsInCache(string orgName, string envName)
        {
            string key = LWConfiguration.GetEntityKey(orgName, envName);
            bool result = false;
            lock (_configMap)
            {
                if (_configMap.ContainsKey(key))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Updates the cached config for the current environment.  Used by LN when 
        /// a framework property changes.
        /// </summary>
        public static void UpdateCacheForCurrentConfiguration(LWConfiguration config)
        {
            LWConfigurationContext ctx = GetCurrentEnvironmentContext();
            string key = LWConfiguration.GetEntityKey(ctx.Organization, ctx.Environment);
            lock (_configMap)
            {
                if (_configMap.ContainsKey(key))
                {
                    _configMap.Remove(key);
                }
                _configMap.Add(key, config);
            }
        }

        /// <summary>
        /// Removes the specified configuration from cache. Only used when deleting an organization or environment
        /// </summary>
        /// <param name="orgName">the organization name</param>
        /// <param name="envName">the environment name</param>
        public static void RemoveConfigurationFromCache(string orgName, string envName)
        {
            string key = LWConfiguration.GetEntityKey(orgName, envName);
            lock (_configMap)
            {
                if (_configMap.ContainsKey(key))
                {
                    _configMap.Remove(key);
                }
            }
        }

        /// <summary>
        /// Gets a LWConfiguration for a particular entity (i.e., organization/environment tuple).
        /// The LWConfiguration encapsulates properties for an entity.
        /// </summary>
        /// <param name="orgName">the organization name</param>
        /// <param name="envName">the environment name</param>
        /// <returns>LWConfiguration</returns>
        public static LWConfiguration GetConfiguration(string orgName, string envName)
        {
            string key = LWConfiguration.GetEntityKey(orgName, envName);
            LWConfiguration result = null;
            lock (_configMap)
            {
                if (_configMap.ContainsKey(key))
                {
                    result = _configMap[key];
                }
                else
                {
                    LWConfiguration newConfig = new LWConfiguration(orgName, envName);
                    _configMap.Add(key, newConfig);
                    result = newConfig;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a LWConfiguration for a particular LWConfigurationContext.
        /// </summary>
        /// <param name="ctx">the LWConfigurationContext</param>
        /// <returns>LWConfiguration</returns>
        public static LWConfiguration GetConfiguration(LWConfigurationContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException("ctx");

            return GetConfiguration(ctx.Organization, ctx.Environment);
        }

        /// <summary>
        /// Sets a configuration using the given LWConfiguration
        /// </summary>
        /// <param name="config">the LWConfiguration</param>
        public static void SetConfiguration(LWConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            string key = config.EntityKey;
            lock (_configMap)
            {
                if (_configMap.ContainsKey(key))
                {
                    _configMap[key] = config;
                }
                else
                {
                    _configMap.Add(key, config);
                }
            }
        }

        /// <summary>
        /// Gets the LWConfiguration for the current LWConfigurationContext.
        /// </summary>
        /// <returns>LWConfiguration or null if no current environment context is set</returns>
        public static LWConfiguration GetCurrentConfiguration()
        {
            LWConfiguration result = null;
            LWConfigurationContext ctx = GetCurrentEnvironmentContext();
            if (ctx != null)
            {
                result = GetConfiguration(ctx);
            }
            return result;
        }

		/// <summary>
        /// This method sets the organization and environment name for the thread of
        /// execution so that other functions can share that data without passing this through 
        /// function parameters.  If this is a web application, then this data is stored in
        /// the http context.  If this is not a web application, then this data is stored in 
        /// thread local storage.
        /// </summary>
		/// <param name="org">the organization name</param>
		/// <param name="env">the environment name</param>
        public static void SetCurrentEnvironmentContext(string org, string env)
        {
            LWConfigurationContext ctx = new LWConfigurationContext(org, env);
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
				HttpContext.Current.Session[LW_ENV_SLOT] = ctx;
            }
            else
            {
                LocalDataStoreSlot lwEnvSlot = Thread.GetNamedDataSlot(GetSlotName(LW_ENV_SLOT));
                Thread.SetData(lwEnvSlot, ctx);
            }
        }

        /// <summary>
        /// Gets the current configuration context (i.e., organization/environment tuple).
        /// </summary>
        /// <returns>LWConfigurationContext</returns>
        public static LWConfigurationContext GetCurrentEnvironmentContext()
        {
            // Get cached value
            LWConfigurationContext ctx = null;
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
				ctx = (LWConfigurationContext)HttpContext.Current.Session[LW_ENV_SLOT];
            }
            else
            {
                LocalDataStoreSlot lwEnvSlot = Thread.GetNamedDataSlot(GetSlotName(LW_ENV_SLOT));
                ctx = (LWConfigurationContext)Thread.GetData(lwEnvSlot);
            }

            // Get from app or web config file
            if (ctx == null)
            {
                string org = ConfigurationManager.AppSettings["LWOrganization"];
                string env = ConfigurationManager.AppSettings["LWEnvironment"];
                if (!string.IsNullOrEmpty(org) && !string.IsNullOrEmpty(env))
                {
                    SetCurrentEnvironmentContext(org, env);
                    ctx = new LWConfigurationContext(org, env);
                }
            }
            return ctx;
        }

        /// <summary>
        /// This method must be called by the non-web applications as part of the application cleanup.
        /// The best option is to call this method in the finally block of the main program.  There is 
        /// no harm in calling this method multiple times.
        /// </summary>
        public static void FreeCurrentEnvironmentContext()
        {
			if (HttpContext.Current == null)
			{
				if (Thread.GetNamedDataSlot(LW_ENV_SLOT) != null)
				{
					Thread.FreeNamedDataSlot(LW_ENV_SLOT);
				}
			}
        }

		/// <summary>
		/// This method first looks for the desired property name in the framework configuration.  If
		/// it finds the property, then that value is returned.  Otherwise, that property is looked
		/// up in the "appSettings" section of the application's configuration file and the value returned.
		/// </summary>
		/// <param name="propName">The property that needs to be looked up.</param>
		/// <returns>the value of the property is found.  Otherwise it is empty.</returns>
		public static string GetConfigurationValue(string propName)
		{
			string configValue = string.Empty;
            LWConfiguration cfg = null;
            try
            {
                cfg = LWConfigurationUtil.GetCurrentConfiguration();
            }
            catch (Exception)
            {
            }
			if (cfg != null && cfg.ContainsConfigName(propName))
			{
				configValue = cfg.GetConfigValue(propName);
			}
			else
			{
				configValue = ConfigurationManager.AppSettings[propName];
			}
			return configValue;
		}

		private static void SetCookie(LWConfigurationContext ctx)
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies[LW_ENV_SLOT];
			if (cookie == null)
			{
				cookie = new HttpCookie(LW_ENV_SLOT);
			}
			cookie["Organization"] = ctx.Organization;
			cookie["Environment"] = ctx.Environment;
			HttpContext.Current.Response.Cookies.Add(cookie);
		}

		private static LWConfigurationContext GetContextFromCookie()
		{
			LWConfigurationContext ctx = null;
			HttpCookie cookie = HttpContext.Current.Request.Cookies[LW_ENV_SLOT];
			if (cookie != null)
			{
				ctx = new LWConfigurationContext(cookie["Organization"], cookie["Environment"]);
			}
			return ctx;
		}

		private static void ClearContextCookie()
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies[LW_ENV_SLOT];
			if (cookie != null)
			{
				cookie.Expires = DateTime.Now;
			}			
		}

        private static string GetSlotName(string prefix)
        {
            return prefix + Thread.CurrentThread.ManagedThreadId;
        }        
    }
}
