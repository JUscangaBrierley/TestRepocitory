using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI;

using Brierley.FrameWork.Common.Logging;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Brierley.FrameWork.Common
{
	public class ResourceUtils
	{
		private const string _className = "ResourceUtils";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		/// <summary>
		/// Gets a string from a global web resource file.  For example, to get a resource string with 
		/// key MyResource from the global resource file ~/App_GlobalResources/LocalizedText.resx, call 
		/// ResourceUtils.GetGlobalWebResource("LocalizedText", "MyResource");
		/// </summary>
		/// <param name="classKey">the name of the resource file in App_GlobalResources</param>
		/// <param name="resourceKey">the name of the resource in the resource file</param>
		/// <returns>the value of the resource as a string</returns>
		public static string GetGlobalWebResource(string classKey, string resourceKey)
		{
			string result = string.Empty;
            if (!string.IsNullOrEmpty(resourceKey))
            {
                object obj = HttpContext.GetGlobalResourceObject(classKey, resourceKey);
                if (obj != null) result = obj.ToString();
            }
			return result;
		}
        
		/// <summary>
		/// Gets a string from a local web resource file.  For example, to get a resource for a page level
		/// resource named MyResource from the local resource file ~/App_LocalResources/MyPage.aspx.resx, call
		/// ResourceUtils.GetLocalWebResource("~/MyPage.aspx", "MyResource");  This only works for pages (*.aspx) 
		/// and user controls (*.ascx) on the website.  This method will not work in a DNN portal, so use
		/// Localization.GetString("mycontrol.Text", LocalResourceFile) instead.
		/// </summary>
		/// <param name="virtualPath">the virtual path on the web site to the page or control</param>
		/// <param name="resourceKey">the name of the resource in the resource file</param>
		/// <returns>the value of the resource as a string</returns>
		public static string GetLocalWebResource(string virtualPath, string resourceKey)
		{
			return GetLocalWebResource(virtualPath, resourceKey, string.Empty);
		}

        /// <summary>
        /// Gets a string from a local web resource file.  For example, to get a resource for a page level
        /// resource named MyResource from the local resource file ~/App_LocalResources/MyPage.aspx.resx, call
        /// ResourceUtils.GetLocalWebResource("~/MyPage.aspx", "MyResource");  This only works for pages (*.aspx) 
		/// and user controls (*.ascx) on the website.  This method will not work in a DNN portal, so use
		/// Localization.GetString("mycontrol.Text", LocalResourceFile) instead.
        /// </summary>
		/// <param name="virtualPath">the virtual path on the web site to the page or control</param>
        /// <param name="resourceKey">the name of the resource in the resource file</param>
        /// <param name="defaultValue">the default value that should be returned if no value is found</param>
        /// <returns>the value of the resource as a string</returns>
        public static string GetLocalWebResource(string virtualPath, string resourceKey, string defaultValue)
        {
			const string methodName = "GetLocalWebResource";
            string result = string.Empty;
            object obj = null;
			if (!string.IsNullOrEmpty(virtualPath) && !string.IsNullOrEmpty(resourceKey))
            {
                try
                {
                    if (!string.IsNullOrEmpty(resourceKey))
                    {
                        obj = HttpContext.GetLocalResourceObject(virtualPath, resourceKey);
                    }
                }
                catch (Exception ex)
				{
					if (ex is InvalidOperationException)
					{
						_logger.Error(_className, methodName, "Invalid virtualPath: " + virtualPath, ex);
					}
					else if (ex is ArgumentException)
					{
						_logger.Error(_className, methodName, "Unrooted resourceKey: " + resourceKey, ex);
					}
					else if (ex is MissingManifestResourceException)
					{
						_logger.Error(_className, methodName, "Invalid resourceKey: " + resourceKey, ex);
					}
					else
					{
						_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
					}
				}
                if (obj == null)
                {
                    if (!resourceKey.EndsWith(".Text"))
                    {
                        resourceKey += ".Text";
                        try
                        {
                            obj = HttpContext.GetLocalResourceObject(virtualPath, resourceKey);
                        }
						catch (Exception ex)
						{
							if (ex is InvalidOperationException)
							{
								_logger.Error(_className, methodName, "Invalid virtualPath: " + virtualPath, ex);
							}
							else if (ex is ArgumentException)
							{
								_logger.Error(_className, methodName, "Unrooted resourceKey: " + resourceKey, ex);
							}
							else if (ex is MissingManifestResourceException)
							{
								_logger.Error(_className, methodName, "Invalid resourceKey: " + resourceKey, ex);
							}
							else
							{
								_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
							}
						}
                    }
                }                
            }
            if (obj != null)
            {
                result = obj.ToString();
            }
            else
            {
                result = defaultValue;
            }
            return result;
        }

		/// <summary>
		/// Gets a resource manager for a resource embedded in a particular assembly.
		/// </summary>
		/// <param name="resourceName">Name of the resource in the assembly</param>
		/// <param name="assembly">an assembly</param>
		/// <returns>the resource manager</returns>
		public static ResourceManager GetResourceManager(string resourceName, Assembly assembly)
		{
			ResourceManager result = null;
			if (assembly != null)
			{
				result = new ResourceManager(resourceName, assembly);
			}
			return result;
		}

		/// <summary>
		/// Get a resource manager for a resource file.
		/// </summary>
		/// <param name="resourceFilePath">path to the resource file</param>
		/// <param name="resourceName">resource file name</param>
		/// <returns>the resource manager</returns>
		public static ResourceManager GetResourceManagerFromFile(string resourceFilePath, string resourceName)
		{
			ResourceManager result = ResourceManager.CreateFileBasedResourceManager(resourceName, resourceFilePath, null);
			return result;
		}

		/// <summary>
		/// Loads the specified manifest resource as a client script block if it is not already loaded.
		/// </summary>
		/// <param name="name">name of the manifest resource</param>
		/// <param name="clientScriptManager">the page's client script manager</param>
		/// <param name="type">the type to associate with the script</param>
		/// <param name="key">the key to identify the script</param>
		/// <param name="addScriptTags">whether the script block should be wrapped with "<script></script>" tags</param>
		public static void LoadResourceClientScript(string name, ClientScriptManager clientScriptManager, Type type, string key, bool addScriptTags)
		{
			if (!clientScriptManager.IsClientScriptBlockRegistered(key))
			{
				Assembly assembly = Assembly.GetCallingAssembly();
				string clientScript = GetManifestResourceString(name, assembly);
				clientScriptManager.RegisterClientScriptBlock(type, key, clientScript, addScriptTags);
			}
		}

		/// <summary>
		/// Loads the specified manifest resource from the calling assembly as a string.
		/// </summary>
		/// <param name="name">name of the manifest resource</param>
		/// <returns>value of the resource as a string</returns>
		public static string GetManifestResourceString(string name)
		{
			string result = string.Empty;
			Assembly assembly = Assembly.GetCallingAssembly();
			result = GetManifestResourceString(name, assembly);
			return result;
		}

		/// <summary>
		/// Loads the specified manifest resource from the specified assembly as a string.
		/// </summary>
		/// <param name="name">name of the manifest resource</param>
		/// <param name="assembly">assembly from which to load the resource</param>
		/// <returns>value of the resource as a string</returns>
		public static string GetManifestResourceString(string name, Assembly assembly)
		{
			string result = string.Empty;
			if (assembly != null)
			{
				Stream stream = assembly.GetManifestResourceStream(name);
				if (stream != null)
				{
					result = new StreamReader(stream).ReadToEnd();
				}
			}
			return result;
		}

		public static List<ListItem> FillWithISO4217CurrencySymbols(List<ListItem> vals)
		{
			SortedDictionary<string, string> currencies = new SortedDictionary<string, string>();
			foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
			{
				RegionInfo regionInfo = new RegionInfo(cultureInfo.LCID);
				string displayName = string.Format("{0} ({1})", regionInfo.CurrencyEnglishName, regionInfo.ISOCurrencySymbol);
				if (!currencies.ContainsKey(displayName))
				{
					currencies.Add(displayName, regionInfo.ISOCurrencySymbol);
				}
			}
			currencies.Add("No Currency", "XXX");

			foreach (var currency in currencies) {
				ListItem item = new ListItem(currency.Key, currency.Value);
				if (!vals.Contains(item))
				{
					vals.Add(item);
				}
			}

			return vals;
		}
	}
}
