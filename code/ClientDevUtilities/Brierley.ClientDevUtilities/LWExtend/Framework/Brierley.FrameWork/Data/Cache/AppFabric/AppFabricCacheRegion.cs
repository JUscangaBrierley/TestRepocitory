//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Microsoft.ApplicationServer.Caching;
using Brierley.FrameWork.Data.Cache.AppFabric.Config;
using Brierley.FrameWork.Data.Cache.AppFabric.Exceptions;

namespace Brierley.FrameWork.Data.Cache.AppFabric
{
    /// <summary>
    /// To avoid losing scalability benefits provided by AppFabric Caching Service, we do not use region the way as it's defined in AppFabric.
    /// The 'region' here is a term that defines those cached items that are stored under the same named cache and have the same key prefix. 
    /// The region name defined in application configuration file is used as the first part of the cache key that we use to access the cached item 
    /// from the same named cache. Client app uses the region name to configure which group of objects get cached and what is the TimeToLive 
    /// value for those cached items.
    /// </summary>
    public class AppFabricCacheRegion
    {                
        private DataCache _cache = null;
        private string _cacheName;
        private string _regionName;
        private int _expirationInterval;

        public string RegionName
        {
            get { return _regionName; }
            set { _regionName = value; }
        }

        public string CacheName
        {
            get { return _cacheName; }
            set { _cacheName = value; }
        }

        /// <summary>
        /// ExpirationInterval is in minutes.
        /// </summary>        
        public int ExpirationInterval
        {
            get { return _expirationInterval; }
            set { _expirationInterval = value; }
        }

        public AppFabricCacheRegion(string cacheName, string regionName, int expirationInterval)
        {   
            AppFabricCacheFactory cacheFactory = AppFabricCacheFactory.Instance();            
            RegionElement region = cacheFactory.Regions.FirstOrDefault(r => r.Name.ToLower() == regionName.ToLower());
            if (region == null || !region.Enabled)
                throw new LWAppFabricCacheException(string.Format("This region is not been configured for caching. Region name: {0}", regionName));
            
            this._cache = cacheFactory.GetCache(cacheName);
            if (this._cache == null)
                throw new LWAppFabricCacheException(string.Format("This named cache does not exist. Please use Windows PowerShell to create the named cache. Cache name: {0}", cacheName));

            this.CacheName = cacheName;
            this.RegionName = regionName;
            this.ExpirationInterval = expirationInterval;
            if (region.TimeToLive > 0)    // setting from region configuration overrides setting from cache provider
                this.ExpirationInterval = region.TimeToLive;
        }
        
        public void Add(string key, object dobj)
        {
            TimeSpan refreshTime = new TimeSpan(0, this.ExpirationInterval, 0);
            this._cache.Put(this.RegionName + "_" + key, dobj, refreshTime);    
            // depending on configuration, 'key' may already include cache prefix
        }

        /// <summary>
        /// At this point, Update() is basically the same as Add()
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dobj"></param>
        public void Update(string key, object dobj)
        {
            Add(key, dobj);
        }

        public void Remove(string key)
        {
            this._cache.Remove(this.RegionName + "_" + key);
        }

        //public void Flush()
        //{
        //    _cache.Remove(...);                       
        //}

        public object Get(string key)
        {
            return this._cache.Get(this.RegionName + "_" + key);
        }

        //public int Count()
        //{
        //    return _cache.Count;
        //}
        
        public static void Dispose()
        {            
            //cache.Dispose();
        }
    }

    /// <summary>
    /// A singleton class that provides access to AppFabric cache factory.
    /// </summary>
    internal class AppFabricCacheFactory
    {

        private static AppFabricCacheFactory _instance;
        private static object _syncLock = new object();
        private DataCacheFactory _cacheFactory;
        private List<RegionElement> _regions = new List<RegionElement>();


        internal List<RegionElement> Regions
        {
            get
            {
                return this._regions;
            }
        } 

        private AppFabricCacheFactory()
        {
            // create DataCacheFactory object using settings in application configuration file
            this._cacheFactory = new DataCacheFactory();

            // retrive region settings from application configuration file
            AppFabricCachingSection cfg = ConfigurationManager.GetSection("appFabricCaching") as AppFabricCachingSection;
            if (cfg != null)
            {
                foreach (RegionElement region in cfg.Regions)
                {
                    this._regions.Add(region);
                }
            }
        }

        internal static AppFabricCacheFactory Instance()
        {
            if (_instance == null)
            {
                lock (_syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppFabricCacheFactory();
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// Get the named cache.
        /// </summary>
        /// <param name="cacheName">Name of the named cache</param>
        /// <returns></returns>
        internal DataCache GetCache(string cacheName)
        {
            return this._cacheFactory.GetCache(cacheName);
        }
    }
}
