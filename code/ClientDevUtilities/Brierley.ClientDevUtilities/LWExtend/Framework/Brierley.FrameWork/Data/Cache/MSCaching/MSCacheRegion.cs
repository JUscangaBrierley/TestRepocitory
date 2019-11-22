//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Text;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.BackingStoreImplementations;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Microsoft.Practices.EnterpriseLibrary.Caching.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation;

namespace Brierley.FrameWork.Data.Cache.MSCaching
{
	public class MSCacheRegion
	{
        public class NullCacheOperation : ICacheOperations
        {
            public int Count { get { return 0; } }
            public Hashtable CurrentCacheState { get { return new System.Collections.Hashtable(); } }
            public void RemoveItemFromCache(string key, CacheItemRemovedReason removalReason) { }
        }

		private ICacheManager cache = null;

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Expiration interval is in seconds.
		/// </summary>
		private int _expirationInterval;
		public int ExpirationInterval
		{
			get { return _expirationInterval; }
			set { _expirationInterval = value; }
		}

		public MSCacheRegion(string name, int expirationInterval)
		{
            try
            {
                cache = CacheFactory.GetCacheManager(name);
                Common.Logging.LWLoggerManager.GetLogger(Common.LWConstants.LW_FRAMEWORK).Trace("MSCacheRegion", "MSCacheRegion",
                    string.Format("Cache region found using configuration by name: {0}", name));
            }
            catch
            {
                if (!StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("DisableCacheRegionCreation"), false))
                {
                    IBackingStore backingStore = new NullBackingStore();
                    ICachingInstrumentationProvider instrProv = new CachingInstrumentationProvider(name, false, false, new NoPrefixNameFormatter());
                    Microsoft.Practices.EnterpriseLibrary.Caching.Cache c = new Microsoft.Practices.EnterpriseLibrary.Caching.Cache(backingStore, instrProv);
                    BackgroundScheduler bgScheduler = new BackgroundScheduler(new ExpirationTask(null, instrProv), new ScavengerTask(10, 1000, new NullCacheOperation(), instrProv), instrProv);
                    cache = new CacheManager(c, bgScheduler, new ExpirationPollTimer(expirationInterval));
                    Common.Logging.LWLoggerManager.GetLogger(Common.LWConstants.LW_FRAMEWORK).Trace("MSCacheRegion", "MSCacheRegion",
                        string.Format("Cache region named '{0}' created using defaults - Max items: {1}. Items to scavenge: {2}", name, 1000, 10));
                }
            }
            
			this._name = name;
			this._expirationInterval = expirationInterval;
		}

		public void Add(string key, object dobj)
		{
			TimeSpan refreshTime = new TimeSpan(0, 0, _expirationInterval);
			SlidingTime expireTime = new SlidingTime(refreshTime);
			cache.Add(key, dobj, CacheItemPriority.Normal, null, expireTime);
		}

		public void Update(string key, object dobj)
		{
			Add(key, dobj);
		}

		public void Remove(string key)
		{
			cache.Remove(key);
		}

		public void Flush()
		{
			cache.Flush();
		}

		public object Get(string key)
		{
			return cache.GetData(key);
		}

		public int Count()
		{
			return cache.Count;
		}

		public static void Dispose()
		{
			//cache.Dispose();
		}
	}
}
