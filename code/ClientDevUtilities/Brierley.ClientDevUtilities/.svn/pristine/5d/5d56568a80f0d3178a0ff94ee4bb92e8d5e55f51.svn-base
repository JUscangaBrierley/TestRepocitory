using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.Cache;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.ClientDevUtilities.LWGateway.PetaPocoGateway;
using PetaPoco;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public interface IServiceBase : IDisposable
    {
        IDataCacheProvider CacheManager { get; }
        string Organization { get; }
        string Environment { get; }
        SupportedDataSourceType DatabaseType { get; }
        int? BulkLoadingBatchSize { get; }
        string Version { get; }
        IDatabase Database { get; }

        LWSchemaVersion GetLatestSchemaVersion(string targetType);
        IList<LWSchemaVersion> GetSchemaVersion(string targetType);
        IList<LWSchemaVersion> GetSchemaVersions();
        void RefreshCache();
        ITransaction StartTransaction();
    }
}
