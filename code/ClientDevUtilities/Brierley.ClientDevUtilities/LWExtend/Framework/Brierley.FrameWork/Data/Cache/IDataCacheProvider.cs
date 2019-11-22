//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Data.Cache
{    
    /// <summary>
    /// This interface describes a cache.  A cache is composed fo regions.  Each region
    /// contains similar data items, that are stored, indexed and retrieved based on the
    /// provided keys.  The data item can be anything.
    /// </summary>
    public interface IDataCacheProvider
    {
        /// <summary>
        /// This defines the amount of time for expiring this item from the cache if not
        /// accessed.
        /// </summary>
        int ExpirationInterval { get; set; }

		/// <summary>
		/// Some cache providers may create static regions fo rthe process.  This causes problems if multiple
		/// instances are created, one for each service factory.  This allows setting of the cache prefix that 
		/// is then prepended to all keys - hence separatign them for each service factory.
		/// </summary>
		string CachePrefix { get; set; }

        /// <summary>
        /// This method can be used to find out if the cache is initialized or not.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        bool RegionExists(string region);

        /// <summary>
        /// This method adds a data item in the cache.
        /// </summary>
        /// <param name="region">Specifies the region to be used for caching this data item.</param>
        /// <param name="key">The key of the data item being stored.</param>
        /// <param name="dataObject">The actual data item to be stored.</param>
        void Add(string region, object key, object dataObject);

        /// <summary>
        /// This method adds a data item in the cache.
        /// </summary>
        /// <param name="region">Specifies the region to be used for caching this data item.</param>
        /// <param name="key">The key of the data item being stored.</param>
        /// <param name="dataObject">The actual data item to be stored.</param>
        void Update(string region, object key, object dataObject);

        /// <summary>
        /// This method removes an item from the specified region.
        /// </summary>
        /// <param name="region">Specifies the region to be used for caching this data item.</param>
        /// <param name="key">The key of the data item being stored.</param>
        void Remove(string region, object key);

        /// <summary>
        /// This method retrieves a data item from the region.
        /// </summary>
        /// <param name="region">Specifies the region to be used for caching this data item.</param>
        /// <param name="key">The key of the data item being stored.</param>
        /// <returns>Data item if found.  Null if no data item exists with that key.</returns>
        object Get(string region, object key);

        /// <summary>
        /// This method returns the number of objects in the specified region.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        int Count(string region);

		/// <summary>
		/// Lists the active cache region names
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		List<string> RegionNames { get; }

        /// <summary>
        /// This method removes an entire region from the cache.  All the objects cached
        /// in that region are cleared.  The data objects themse,ves are not destroyed but rather
        /// their caching information is cleared.
        /// </summary>
        /// <param name="region">Name of the region to be cleared.</param>
        void RemoveRegion(string region);

        /// <summary>
        /// This method clears the entire cache.  It is equivalent to calling RemoveRegion for the
        /// entire cache.
        /// </summary>
        void Dispose();
    }
}
