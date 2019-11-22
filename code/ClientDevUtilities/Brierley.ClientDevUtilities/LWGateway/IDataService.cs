using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.LWGateway.DataAccess;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public interface IDataService : IServiceBase
    {
        IClientConfigurationDao ClientConfigurationDao { get; }
        IContactHistoryDao ContactHistoryDao { get; }
        bool DebitPayOffMethodOn { get; }
        bool DebitPayOffPointTypeRestrictionOn { get; }
        bool DebitPayOffPointEventRestrictionOn { get; }
        Dictionary<string, AuditLogConfig> AuditableObjects { get; }

        void CreateArchiveObject(ArchiveObject archive);
        void CreateAsyncJob(AsyncJob job);
        void CreateAsyncJobProcessedObjects(List<AsyncJobProcessedObjects> objects);
        void CreateAuditLogConfig(AuditLogConfig cfg);
        void CreateAuditObject(LWObjectAuditLogBase arc);
        void CreateBscriptExpression(Bscript bscript);
        void CreateCacheRefresh(string username);
        void CreateClientConfiguration(ClientConfiguration config);
        void CreateIDGenerator(string objectName);
        void CreateScheduledJob(ScheduledJob job);
        void CreateScheduledJobRun(ScheduledJobRun run);
        void CreateSkin(Skin entity);
        void CreateSkinItem(SkinItem entity);
        void CreateSyncJob(SyncJob job);
        void CreateX509Cert(X509Cert entity);
        void DeleteArchiveObject(long id);
        void DeleteAuditLogConfig(long id);
        void DeleteBscriptExpression(string bsName);
        void DeleteBscriptExpression(long bsId);
        void DeleteClientConfiguration(string key);
        void DeleteRemoteAssembly(RemoteAssembly assembly);
        void DeleteScheduledJob(long jobId);
        void DeleteScheduledJobRun(long runId);
        void DeleteSkin(long ID);
        void DeleteSkinItem(long id);
        void DeleteX509Cert(long id);
        IList<AsyncJob> GetAllAsyncJobs();
        IList<Bscript> GetAllBscriptExpressions();
        IList<Bscript> GetAllBscriptExpressions(long[] ids);
        IList<Bscript> GetAllBscriptExpressions(string[] names);
        IList<Bscript> GetAllChangedBscriptExpressions(DateTime since);
        IList<ClientConfiguration> GetAllChangedClientConfigurations(DateTime since, bool onlyExternal);
        IList<string> GetAllClientConfigurationKeys();
        IList<ClientConfiguration> GetAllClientConfigurations();
        IList<ClientConfiguration> GetAllClientConfigurationsLike(string keyPattern);
        IList<ClientConfiguration> GetAllExternalClientConfigurations();
        IList<ClientConfiguration> GetAllExternalClientConfigurationsByFolder(long folderId);
        List<RemoteAssembly> GetAllRemoteAssemblies();
        IList<ScheduledJobRun> GetAllScheduledJobRuns(long jobId);
        IList<ScheduledJob> GetAllScheduledJobs();
        IList<SkinItem> GetAllSkinItems(long skinId, SkinItemTypeEnum skinItemType);
        IList<SkinItem> GetAllSkinItems(long skinId, SkinItemTypeEnum skinItemType, DateTime changedSince);
        IList<SkinItem> GetAllSkinItems(long skinId, DateTime changedSince);
        IList<SkinItem> GetAllSkinItems(long skinId);
        IList<Skin> GetAllSkins();
        IList<Skin> GetAllSkins(DateTime changedSince);
        List<X509Cert> GetAllX509CertByCertType(X509CertType certType);
        List<X509Cert> GetAllX509CertByPassType(string passType);
        List<X509Cert> GetAllX509Certs();
        List<X509Cert> GetAllX509Certs(DateTime changedSince);
        List<X509Cert> GetAllX509Certs(string sortExpression, bool ascending);
        IList<ArchiveObject> GetArchiveObjectByGroup(long groupId, long runNumber);
        AsyncJob GetAsyncJobById(long jobId);
        AsyncJob GetAsyncJobByJobNumber(long jobNumber);
        List<long> GetAsyncJobProcessedObjectIdsByJobName(string jobName, string objectName);
        List<long> GetAsyncJobProcessedObjectIdsByJobNumber(long jobNumber, string objectName);
        IList<AsyncJob> GetAsyncJobs(List<Dictionary<string, object>> parms);
        AuditLogConfig GetAuditLogConfig(string typeName);
        List<AuditLogConfig> GetAuditLogConfigs();
        Bscript GetBscriptExpression(long bsId);
        Bscript GetBscriptExpression(string bsName);
        string GetClientConfigProp(string key);
        ClientConfiguration GetClientConfiguration(string key);
        RemoteAssembly.ComponentReference GetComponentReference(CustomComponentTypeEnum componentType, string name);
        IEnumerable<ScheduledJobRun> GetIncompleteJobRuns(DateTime afterStartDate);
        CacheRefresh GetLatestCacheRefresh();
        long GetNextID(string objectName);
        long GetNextID(string objectName, int howMany);
        long GetNextSequence();
        RemoteAssembly GetRemoteAssembly(long id);
        RemoteAssembly GetRemoteAssembly(string filename);
        ScheduledJob GetScheduledJob(string jobName);
        IList<ScheduledJob> GetScheduledJob(string assemblyName, string factoryName);
        ScheduledJob GetScheduledJob(long jobId);
        DateTime? GetScheduledJobLastRunTime(long jobId);
        ScheduledJobRun GetScheduledJobRun(long runId);
        IList<ScheduledJob> GetScheduledJobs(List<Dictionary<string, object>> parms);
        Skin GetSkin(string skinName);
        Skin GetSkin(long ID);
        SkinItem GetSkinItem(long id);
        SkinItem GetSkinItem(long skinId, SkinItemTypeEnum skinItemType, string fileName);
        X509Cert GetX509Cert(long id);
        X509Cert GetX509Cert(string certName);
        bool HasScheduledJobBeenRun(long jobId);
        int HowManyLIBMessages(long jobNumber);
        void LogLIBMessage(LIBMessageLog msg);
        LIBMessageLog RetrieveLIBMessage(long messageId);
        IList<LIBMessageLog> RetrieveLIBMessagesByJobNumber(long jobNumber);
        void SaveRemoteAssembly(RemoteAssembly assembly);
        IEnumerable<Bscript> SearchBScriptExpressions(string search, ExpressionContexts context, string currentConditionAttributeSet, int maxResults);
        void SetClientConfigProp(string key, string value);
        void TestConnection();
        void Update(List<AsyncJobProcessedObjects> objects);
        void UpdateAsyncJob(AsyncJob job);
        AuditLogConfig UpdateAuditLogConfig(AuditLogConfig cfg);
        void UpdateBscriptExpression(Bscript bscript);
        void UpdateClientConfiguration(ClientConfiguration config);
        void UpdateMessageDef(ArchiveObject archive);
        void UpdateScheduledJob(ScheduledJob job);
        void UpdateScheduledJobRun(ScheduledJobRun run);
        void UpdateSkin(Skin entity);
        void UpdateSkinItem(SkinItem entity);
        void UpdateX509Cert(X509Cert entity);
    }
}
