using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Compilation;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Common.Globalization
{
	public class DBResourceProvider : IResourceProvider
	{
		private const string _className = "DBResourceProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private string _classKey;
		private Dictionary<string, Dictionary<string, string>> _resourceCache = new Dictionary<string, Dictionary<string, string>>();
		
		public DBResourceProvider(string classKey)
		{
			_classKey = classKey;
		}

		public object GetObject(string resourceKey, CultureInfo culture)
		{
			const string methodName = "GetObject";
			if (string.IsNullOrEmpty(resourceKey))
			{
				throw new ArgumentNullException("resourceKey");
			}
			_logger.Trace(_className, methodName, string.Format("{0}: get resource {1}", _classKey, resourceKey));

			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}

			string resourceValue = null;
			Dictionary<string, string> resCacheByCulture = null;
			if (_resourceCache.ContainsKey(culture.Name))
			{
				resCacheByCulture = _resourceCache[culture.Name];
				if (resCacheByCulture.ContainsKey(resourceKey))
				{
					resourceValue = resCacheByCulture[resourceKey];
				}
			}

			if (resourceValue == null)
			{
				resourceValue = string.Format("{0}/{1}: I got this from the DB", _classKey, resourceKey); //m_dalc.GetResourceByCultureAndKey(culture, resourceKey);

				lock (this)
				{
					if (resCacheByCulture == null)
					{
						resCacheByCulture = new Dictionary<string, string>();
						_resourceCache.Add(culture.Name, resCacheByCulture);
					}
					resCacheByCulture.Add(resourceKey, resourceValue);
				}
			}

			_logger.Trace(_className, methodName, string.Format("{0}: {1}: result = {2}", _classKey, resourceKey, resourceValue));
			return resourceValue;
		}

		public IResourceReader ResourceReader
		{
			get
			{
				ListDictionary resourceDictionary = new ListDictionary(); 
				//this.m_dalc.GetResourcesByCulture(CultureInfo.InvariantCulture);

				return new DBResourceReader(resourceDictionary);
			}
		}
	}
}
