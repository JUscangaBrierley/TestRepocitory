//#define TraceInstantiation

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.Cache;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data
{
	public abstract class ServiceBase : IDisposable
	{
		private static ThreadLocal<Dictionary<string, ThreadLocalDatabase>> _database = new ThreadLocal<Dictionary<string, ThreadLocalDatabase>>(() => new Dictionary<string, ThreadLocalDatabase>());

		private bool _disposed = false;
        private ThreadLocalDatabase _serviceThreadLocalDatabase = null;

		public IDataCacheProvider CacheManager { get; internal set; }
		public string Organization { get; internal set; }
		public string Environment { get; internal set; }
		public SupportedDataSourceType DatabaseType { get; internal set; }

		public int? BulkLoadingBatchSize
		{
			get
			{
				return Config.BulkLoadingBatchSize;
			}
		}

		public string Version
		{
			get
			{
				var assembly = Assembly.GetExecutingAssembly();
				return System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
			}
		}

		protected ServiceConfig Config { get; set; }

		public PetaPoco.Database Database
		{
			get
			{
				if (_serviceThreadLocalDatabase.Database == null)
				{
                    _serviceThreadLocalDatabase.Database = Config.CreateDatabase();
				}
				return _serviceThreadLocalDatabase.Database;
			}
		}


#if TraceInstantiation
		private string _instanceStack = null;
#endif

		protected ServiceBase(ServiceConfig config)
		{
#if TraceInstantiation
			_instanceStack = System.Environment.StackTrace;
#endif
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}

			Organization = config.Organization;
			Environment = config.Environment;
			DatabaseType = config.DatabaseType;
			CacheManager = config.CacheManager;
			Config = config;

			if (CacheManager != null)
			{
				CacheManager.CachePrefix = Organization + "_" + Environment;
			}
			if (!_database.Value.ContainsKey(Config.ConnectString))
			{
				_database.Value[Config.ConnectString] = new ThreadLocalDatabase();
			}
            _serviceThreadLocalDatabase = _database.Value[Config.ConnectString];
            _serviceThreadLocalDatabase.ReferenceCount++;
		}

		/// <summary>
		/// Called when the cache is no longer valid and should be cleared.
		/// </summary>
		public void RefreshCache()
		{
			//these don't migrate through environments. Clearing the cache of them may even cause problems (e.g., until it is replaced with a
			//better solution for token storage, clearing out AuthenticationTokens will cause everyone to be logged out of the mobile gateway).
			string[] nonParticipatingRegions = { "MemberByIpCode", "MemberByLoyaltyId", "AuthenticationTokens", "AuthenticationTokenIds" };

			//remove cache regions
			var regions = CacheManager.RegionNames;
			foreach (string region in regions)
			{
				if (nonParticipatingRegions.Contains(region))
				{
					continue;
				}
				CacheManager.RemoveRegion(region);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual IList<LWSchemaVersion> GetSchemaVersions()
		{
			throw new NotImplementedException();
		}

		public virtual IList<LWSchemaVersion> GetSchemaVersion(string targetType)
		{
			throw new NotImplementedException();
		}

		public virtual LWSchemaVersion GetLatestSchemaVersion(string targetType)
		{
			throw new NotImplementedException();
		}

		public ITransaction StartTransaction()
		{
			return Database.GetTransaction();
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
                _serviceThreadLocalDatabase.ReferenceCount--;
				_disposed = true;
				if (_serviceThreadLocalDatabase.ReferenceCount < 1 && _serviceThreadLocalDatabase.Database != null)
				{
					try
					{
                        _serviceThreadLocalDatabase.Database.Dispose();
                        _serviceThreadLocalDatabase.Database = null;
					}
					catch (Exception ex)
					{
						var _logger = Brierley.FrameWork.Common.Logging.LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
						_logger.Critical("ServiceBase", "Dispose", "Unexpected error disposing.", ex);
					}
				}
			}
		}

		~ServiceBase()
		{
			//this should have been suppressed
			if (!_disposed)
			{
				var _logger = Brierley.FrameWork.Common.Logging.LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
#if TraceInstantiation
				_logger.Critical("ServiceBase", "Finalize", "An instance of Brierley.Framework.Data.ServiceBase has been finalized without first being disposed of. Stack trace where service was created is - " + _instanceStack);
#else
				_logger.Critical("ServiceBase", "Finalize", "An instance of Brierley.Framework.Data.ServiceBase has been finalized without first being disposed of.");
#endif
			}
		}
	}
}
