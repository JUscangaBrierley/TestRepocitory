//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.Cache
{
    public class DataCacheProvider : IDataCacheProvider
    {
        #region Fields
        private Dictionary<string, CacheRegion> regionMap = new Dictionary<string, CacheRegion>();
		private string _cachePrefix = string.Empty;        
        #endregion

        #region Cache Region Definition
        internal class CacheRegion : IDisposable
        {
            private Hashtable cache = new Hashtable();

            public void Add(string key,object dobj)
            {
                lock(cache)
                {
                    cache.Add(key,dobj);
                }
            }

            public void Update(string key,object dobj)
            {
                lock(cache)
                {
                    if ( cache.ContainsKey(key) )
                    {
                        cache.Remove(key);
                    }
                    cache.Add(key,dobj);
                }                                
            }

            public void Remove(string key)
            {
                lock (cache)
                {
                    cache.Remove(key);
                }
            }

            public object Get(string key)
            {
                lock (cache)
                {
                    return cache.ContainsKey(key) ? cache[key] : null;
                }
            }

            public int Count()
            {
                lock (cache)
                {
                    return cache.Count;
                }
            }

            public void Dispose()
            {
                cache.Clear();
            }
        }
        #endregion

        #region Private Helpers
        private CacheRegion GetRegion(string region)
        {
            CacheRegion r = null;
            lock (regionMap)
            {
				string regionKey = !string.IsNullOrEmpty(_cachePrefix) ? _cachePrefix + "_" + region : region;
				if (regionMap.ContainsKey(regionKey))
				{
					r = regionMap[regionKey];
				}
				else
				{
					r = new CacheRegion();
					regionMap.Add(regionKey, r);
				}
            }
            return r;
        }        
        #endregion

        #region Public Interface Methods

        public int ExpirationInterval 
        {
            get { return -1; }
            set { }
        }

		public string CachePrefix
		{
			get { return _cachePrefix; }
			set { _cachePrefix = value; }
		}

        public bool RegionExists(string region)
        {
            lock (regionMap)
            {
				string regionKey = !string.IsNullOrEmpty(_cachePrefix) ? _cachePrefix + "_" + region : region;
				return regionMap.ContainsKey(regionKey);
            }
        }
        public void Add(string region, object key, object dataObject)
        {
            GetRegion(region).Add(key.ToString(), dataObject);
        }
        public void Update(string region, object key, object dataObject)
        {
            GetRegion(region).Update(key.ToString(), dataObject);
        }

        public void Remove(string region, object key)
        {
            GetRegion(region).Remove(key.ToString());
        }

        public object Get(string region, object key)
        {
            return GetRegion(region).Get(key.ToString());
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

        public int Count(string region)
        {
            CacheRegion r = GetRegion(region);
            return (r != null ? r.Count() : 0);
        }

        public void RemoveRegion(string region)
        {
            lock (regionMap)
            {
                if (regionMap.ContainsKey(region))
                {
                    CacheRegion r = regionMap[region];
                    regionMap.Remove(region);
                    r.Dispose();
                }
            }            
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
