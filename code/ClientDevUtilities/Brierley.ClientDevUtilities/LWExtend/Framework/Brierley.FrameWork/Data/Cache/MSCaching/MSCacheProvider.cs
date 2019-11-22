//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Data.Cache.MSCaching
{
	/// <summary>
	/// This class implements IDataCacheProvider interface using Microsoft Enterprise Library's
	/// Caching Application Block.
	/// </summary>
	public class MSCacheProvider : IDataCacheProvider
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private IDictionary<string, MSCacheRegion> _regionMap = new Dictionary<string, MSCacheRegion>();
		private List<string> _invalidRegionMap = new List<string>();
		private string _cachePrefix = string.Empty;
		private int _expirationInterval = 3600;		

		public IDictionary<string, MSCacheRegion> Regions
		{
			get { return _regionMap; }
			set { _regionMap = value; }
		}

		public MSCacheProvider()
		{
		}

		public MSCacheProvider(int expirationInterval)
		{
			this._expirationInterval = expirationInterval;
		}

		public int ExpirationInterval
		{
			get { return _expirationInterval; }
			set { _expirationInterval = value; }
		}

		public string CachePrefix
		{
			get { return _cachePrefix; }
			set { _cachePrefix = value; }
		}

		public bool RegionExists(string region)
		{
			MSCacheRegion r = GetRegion(region);
			return r != null;
		}

		public void Add(string region, object key, object dataObject)
		{
			MSCacheRegion r = GetRegion(region);
			if (r != null)
			{
				string regKey = !string.IsNullOrEmpty(_cachePrefix) ? _cachePrefix + "_" + key.ToString() : key.ToString();
				r.Add(regKey, dataObject);
			}
		}

		public void Update(string region, object key, object dataObject)
		{
			MSCacheRegion r = GetRegion(region);
			if (r != null)
			{
				string regKey = !string.IsNullOrEmpty(_cachePrefix) ? _cachePrefix + "_" + key.ToString() : key.ToString();
				r.Update(regKey, dataObject);
			}
		}

		public void Remove(string region, object key)
		{
			MSCacheRegion r = GetRegion(region);
			if (r != null)
			{
				string regKey = !string.IsNullOrEmpty(_cachePrefix) ? _cachePrefix + "_" + key.ToString() : key.ToString();
				r.Remove(regKey);
			}
		}

		public object Get(string region, object key)
		{
			object data = null;
			MSCacheRegion r = GetRegion(region);
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
				if (_regionMap != null)
				{
					return _regionMap.Keys.ToList();
				}
				return new List<string>();
			}
		}

		public int Count(string region)
		{
			MSCacheRegion r = GetRegion(region);
			return r != null ? r.Count() : 0;
		}

		public void RemoveRegion(string region)
		{
			MSCacheRegion r = GetRegion(region);
			if (r != null)
			{
				r.Flush();
			}
		}

		public void Dispose()
		{
			lock (_regionMap)
			{
				_regionMap.Clear();
			}
		}

		private MSCacheRegion GetRegion(string region)
		{
			MSCacheRegion r = null;
			lock (_regionMap)
			{
				if (!_invalidRegionMap.Contains(region))
				{
					if (_regionMap.ContainsKey(region))
					{
						r = _regionMap[region];
					}
					else
					{
						// try to create the region
						try
						{
							r = new MSCacheRegion(region, _expirationInterval);
							_regionMap.Add(region, r);
						}
						catch (Exception ex)
						{
							// region is not specified in the application configuration file.
							_invalidRegionMap.Add(region);
                            if (ex is Microsoft.Practices.ServiceLocation.ActivationException)
                                _logger.Warning(
                                    string.Format(
                                        "Cache region {0} does not appear to be configured, so it has been disabled for this session. To avoid exceptions, this region should be configured. If caching is not needed, set the maximumElementsInCacheBeforeScavenging to zero (0).",
                                        region));
                            else
                                _logger.Error(string.Format("Exception caught while initializing cache region {0}", region), ex);
						}
					}
				}
			}
			return r;
		}
	}
}
