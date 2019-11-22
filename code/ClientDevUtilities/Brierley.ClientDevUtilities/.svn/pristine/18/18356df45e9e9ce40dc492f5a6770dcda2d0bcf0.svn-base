//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Microsoft.ApplicationServer.Caching;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.Cache.AppFabric.Exceptions;

namespace Brierley.FrameWork.Data.Cache.AppFabric
{
    /// <summary>
    /// This class implements IDataCacheProvider interface using Microsoft's AppFabric Caching Service.
    /// </summary>
    public class AppFabricCacheProvider : IDataCacheProvider
    {        
        private const string _className = "AppFabricCacheProvider";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private IDictionary<string, AppFabricCacheRegion> regionMap = new Dictionary<string, AppFabricCacheRegion>();
		private string _cacheName = "LwCache";
        private string _cachePrefix = string.Empty;
        private int _expirationInterval = 60;       // minutes

        public IDictionary<string, AppFabricCacheRegion> Regions
        {
            get { return regionMap; }
            set { regionMap = value; }
        }

        /// <summary>
        /// Name of the named cache. Normally in LW, we have a named cache for each Organization_Environment combination.
        /// This should be set in the configuration file.
        /// </summary>
        public string CacheName
        {
            get { return _cacheName; }
            set { _cacheName = value; }
        }

        public AppFabricCacheProvider() { }
        public AppFabricCacheProvider(int expirationInterval) 
        {
            this._expirationInterval = expirationInterval;
        }

        private AppFabricCacheRegion GetRegion(string regionName)
        {
            string methodName = "GetRegion";
            string mapKey = this.CacheName + regionName;
            AppFabricCacheRegion r = null;            
            lock (regionMap)
            {
                if (regionMap.ContainsKey(mapKey))
                {
                    r = regionMap[mapKey];
                }
                else
                {
                    // try to create the region
                    try
                    {
                        r = new AppFabricCacheRegion(this.CacheName, regionName, _expirationInterval);
                        regionMap.Add(mapKey, r);
                    }
                    catch (Exception e)
                    {
                        // occurs when region is not specified in application configuration file or named cache is not created

                        // log the exception if it's because AppFabric cache server is not started
                        if (e is DataCacheException && 
                            e.InnerException != null &&
                            e.InnerException.InnerException != null &&
                            e.InnerException.InnerException is SocketException)
                        {   
                            _logger.Error(_className, methodName, "Cannot connect to AppFabric cache server.", e);
                        }
                    }
                }
            }
            return r;
        }

        #region Interface methods

        public int ExpirationInterval
        {
            get { return _expirationInterval; }
            set { _expirationInterval = value;}
        }

		public string CachePrefix
		{
			get { return _cachePrefix; }
			set { _cachePrefix = value; }
		}

        public bool RegionExists(string regionName)
        {
            AppFabricCacheRegion r = GetRegion(regionName);
            return (r != null);
        }

        public void Add(string regionName, object key, object dataObject)
        {
            AppFabricCacheRegion r = GetRegion(regionName);
            if (r != null)
            {
                // final key to be used:  RegionName_CachePrefix_key   ('CachePrefix' may not be set)
				string regKey = !string.IsNullOrEmpty(_cachePrefix) ? _cachePrefix + "_" + key.ToString() : key.ToString();
				r.Add(regKey, dataObject);
            }
        }

        public void Update(string regionName, object key, object dataObject)
        {
            AppFabricCacheRegion r = GetRegion(regionName);
            if (r != null)
            {
				string regKey = !string.IsNullOrEmpty(_cachePrefix) ? _cachePrefix + "_" + key.ToString() : key.ToString();
				r.Update(regKey, dataObject);
            }
        }

        public void Remove(string regionName, object key)
        {
            AppFabricCacheRegion r = GetRegion(regionName);
            if (r != null)
            {
				string regKey = !string.IsNullOrEmpty(_cachePrefix) ? _cachePrefix + "_" + key.ToString() : key.ToString();
				r.Remove(regKey);
            }
        }

        public object Get(string regionName, object key)
        {
            object data = null;
            AppFabricCacheRegion r = GetRegion(regionName);
            if (r != null)
            {
				string regKey = !string.IsNullOrEmpty(_cachePrefix) ? _cachePrefix + "_" + key.ToString() : key.ToString();
				data = r.Get(regKey);
            }
            return data;
        }

		public List<string> RegionNames
		{
			get
			{
				if (regionMap != null)
				{
					return regionMap.Keys.ToList();
				}
				return new List<string>();
			}
		}

        public int Count(string regionName)
        {
            // pending: implement this later
            //AppFabricCacheRegion r = GetRegion(regionName);
            //return (r != null ? r.Count() : 0);
            throw new NotImplementedException("Count(...) method has not been implemented by AppFabricCacheProvider class.");
        }

        public void RemoveRegion(string regionName)
        {
            // pending: implement this later
            //AppFabricCacheRegion r = GetRegion(regionName);
            //if (r != null)
            //    r.Flush();
            throw new NotImplementedException("RemoveRegion(...) method has not been implemented by AppFabricCacheProvider class.");
        }

        public void Dispose()
        {
            lock (regionMap)
            {
                regionMap.Clear();
            }
        }
        #endregion
    }
}

