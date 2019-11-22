using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using System.Collections.Generic;

namespace Brierley.FrameWork.Data
{
    /// <summary>
    /// All CRUD operations for REST ACL
    /// </summary>
    public class RestAclDataService : ServiceBase
    {
        private RestResourceDao _restResourceDao;
        private RestRoleDao _restRoleDao;
        private RestConsumerDao _restConsumerDao;
        private RestGroupDao _restGroupDao;

        private RestRoleResourceDao _restRoleResoureDao;
        private RestConsumerRoleDao _restConsumerRoleDao;
        private RestConsumerGroupDao _restConsumerGroupDao;
        private RestGroupRoleDao _restGroupRoleDao;

        /// <summary>
        /// Initializes a REST ACL Data Service
        /// </summary>
        /// <param name="config">LW Service config</param>
        /// <remarks>Note: Caching is not performed whenever soft deleted records are retrieved</remarks>
        public RestAclDataService(ServiceConfig config)
            : base(config)
        {
        }

        #region DAO properties

        /// <summary>
        /// Returns DAO for RestResource
        /// </summary>
        public RestResourceDao RestResourceDao
        {
            get
            {
                if (_restResourceDao == null)
                {
                    _restResourceDao = new RestResourceDao(Database, Config);
                }
                return _restResourceDao;
            }
        }

        /// <summary>
        /// Returns DAO for RestRole
        /// </summary>
        public RestRoleDao RestRoleDao
        {
            get
            {
                if (_restRoleDao == null)
                {
                    _restRoleDao = new RestRoleDao(Database, Config);
                }
                return _restRoleDao;
            }
        }

        /// <summary>
        /// Returns DAO for RestConsumer
        /// </summary>
        public RestConsumerDao RestConsumerDao
        {
            get
            {
                if (_restConsumerDao == null)
                {
                    _restConsumerDao = new RestConsumerDao(Database, Config);
                }
                return _restConsumerDao;
            }
        }

        /// <summary>
        /// Returns DAO for RestGroup
        /// </summary>
        public RestGroupDao RestGroupDao
        {
            get
            {
                if (_restGroupDao == null)
                {
                    _restGroupDao = new RestGroupDao(Database, Config);
                }
                return _restGroupDao;
            }
        }

        public RestRoleResourceDao RestRoleResourceDao
        {
            get
            {
                if (_restRoleResoureDao == null)
                {
                    _restRoleResoureDao = new RestRoleResourceDao(Database, Config);
                }
                return _restRoleResoureDao;
            }
        }

        public RestConsumerRoleDao RestConsumerRoleDao
        {
            get
            {
                if (_restConsumerRoleDao == null)
                {
                    _restConsumerRoleDao = new RestConsumerRoleDao(Database, Config);
                }
                return _restConsumerRoleDao;
            }
        }

        public RestConsumerGroupDao RestConsumerGroupDao
        {
            get
            {
                if (_restConsumerGroupDao == null)
                {
                    _restConsumerGroupDao = new RestConsumerGroupDao(Database, Config);
                }
                return _restConsumerGroupDao;
            }
        }

        public RestGroupRoleDao RestGroupRoleDao
        {
            get
            {
                if (_restGroupRoleDao == null)
                {
                    _restGroupRoleDao = new RestGroupRoleDao(Database, Config);
                }
                return _restGroupRoleDao;
            }
        }

        #endregion


        #region RestConsumer methods

        /// <summary>
        /// Retrieves RestConsumer by primary key Id
        /// </summary>
        /// <param name="id">primary key</param>
        /// <param name="bypassCache">if true, will retrieve directly from db</param>
        /// <returns>Corresponding RestConsumer</returns>
        public RestConsumer RetrieveConsumer(long id, bool bypassCache = false)
        {
            if (bypassCache)
                return RestConsumerDao.Retrieve(id);

            RestConsumer result = (RestConsumer) CacheManager.Get(CacheRegions.RestConsumerById, id);
            if (result == null)
            {
                result = RestConsumerDao.Retrieve(id);
                if (result != null)
                {
                    UpdateConsumerCache(result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves RestConsumer by API Gateway Consumer Id
        /// </summary>
        /// <param name="id">API Gateway Consumer Id</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumer will also be returned.</param>
        /// <returns>Corresponding RestConsumer</returns>
        public RestConsumer RetrieveConsumerByConsumerId(string consumerId, bool includeSoftDeleted = false)
        {
            if (includeSoftDeleted)
            {
                return RestConsumerDao.RetrieveByConsumerId(consumerId, true);
            }

            RestConsumer result =
                (RestConsumer) CacheManager.Get(CacheRegions.RestConsumerByConsumerId, consumerId);
            if (result == null)
            {
                result = RestConsumerDao.RetrieveByConsumerId(consumerId);
                if (result != null)
                {
                    UpdateConsumerCache(result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves RestConsumer by API Gateway UserName
        /// </summary>
        /// <param name="userName">API Gateway UserName</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumer will also be returned.</param>
        /// <returns>Corresponding RestConsumer</returns>
        public RestConsumer RetrieveConsumerByUsername(string userName, bool includeSoftDeleted = false)
        {
            if (includeSoftDeleted)
            {
                return RestConsumerDao.RetrieveByUsername(userName, true);
            }

            RestConsumer result = (RestConsumer)CacheManager.Get(CacheRegions.RestConsumerByUsername, userName);
            if (result == null)
            {
                result = RestConsumerDao.RetrieveByUsername(userName);
                if (result != null)
                {
                    UpdateConsumerCache(result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves RestConsumers by API Gateway Custom Id
        /// </summary>
        /// <param name="customId">API Gateway Custom Id</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumer will also be returned.</param>
        /// <returns>Corresponding RestConsumer</returns>
        /// <remarks>This is not cached since it is an admin-only method that does not require high performance</remarks>
        public List<RestConsumer> RetrieveConsumersByCustomId(string customId, bool includeSoftDeleted = false)
        {
            return RestConsumerDao.RetrieveByCustomId(null, customId, includeSoftDeleted);
        }

        /// <summary>
        /// Retrieves RestConsumers by API Gateway Custom Id in batches
        /// </summary>
        /// <param name="customId">API Gateway Custom Id</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumer will also be returned.</param>
        /// <param name="batchInfo">Paged batch info</param>
        /// <returns>Corresponding RestConsumer</returns>
        /// <remarks>This is not cached since it is an admin-only method that does not require high performance</remarks>
        public List<RestConsumer> RetrieveConsumersByCustomId(LWQueryBatchInfo batchInfo, string customId, bool includeSoftDeleted = false)
        {
            return RestConsumerDao.RetrieveByCustomId(batchInfo, customId, includeSoftDeleted);
        }


        /// <summary>
        /// Retrieves RestConsumers by RestRoleId
        /// </summary>
        /// <param name="restRoleId">primary key id of RestRole</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumers will also be returned.</param>
        /// <returns>Corresponding List of RestConsumers</returns>
        /// <remarks>This is not cached since it is an admin-only method that does not require high performance</remarks>
        public List<RestConsumer> RetrieveConsumerByRestRoleId(long restRoleId, bool includeSoftDeleted = false)
        {
            return RestConsumerDao.RetrieveByRestRoleId(null, restRoleId, includeSoftDeleted);
        }

        /// <summary>
        /// Retrieves RestConsumers by RestRoleId in batches
        /// </summary>
        /// <param name="restRoleId">primary key id of RestRole</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumers will also be returned.</param>
        /// <param name="batchInfo">Paged batch info</param>
        /// <returns>Corresponding List of RestConsumers</returns>
        /// <remarks>This is not cached since it is an admin-only method that does not require high performance</remarks>
        public List<RestConsumer> RetrieveConsumerByRestRoleId(LWQueryBatchInfo batchInfo, long restRoleId, bool includeSoftDeleted = false)
        {
            return RestConsumerDao.RetrieveByRestRoleId(batchInfo, restRoleId, includeSoftDeleted);
        }

        /// <summary>
        /// Retrieves RestConsumers by RestGroupId
        /// </summary>
        /// <param name="restGroupId">primary key id of RestGroup</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumers will also be returned.</param>
        /// <returns>Corresponding List of RestConsumers</returns>
        /// <remarks>This is not cached since it is an admin-only method that does not require high performance</remarks>
        public List<RestConsumer> RetrieveConsumerByRestGroupId(long restGroupId, bool includeSoftDeleted = false)
        {
            return RestConsumerDao.RetrieveByRestGroupId(null, restGroupId, includeSoftDeleted);
        }

        /// <summary>
        /// Retrieves RestConsumers by RestGroupId in batches
        /// </summary>
        /// <param name="restGroupId">primary key id of RestGroup</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumers will also be returned.</param>
        /// <param name="batchInfo">Paged batch info</param>
        /// <returns>Corresponding List of RestConsumers</returns>
        /// <remarks>This is not cached since it is an admin-only method that does not require high performance</remarks>
        public List<RestConsumer> RetrieveConsumerByRestGroupId(LWQueryBatchInfo batchInfo, long restGroupId, bool includeSoftDeleted = false)
        {
            return RestConsumerDao.RetrieveByRestGroupId(batchInfo, restGroupId, includeSoftDeleted);
        }

        /// <summary>
        /// Retrieves all RestConsumers.
        /// </summary>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumers will also be returned.</param>
        /// <returns>List of all RestConsumers</returns>
        /// <remarks>This is not cached due to the potential volume of data</remarks>
        public List<RestConsumer> RetrieveAllConsumers(bool includeSoftDeleted = false)
        {
            return RestConsumerDao.RetrieveAll(null, includeSoftDeleted);
        }

        /// <summary>
        /// Retrieves all RestConsumers in batches
        /// </summary>
        /// <param name="batchInfo">Paged batch info</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestConsumers will also be returned.</param>
        /// <returns>List of all RestConsumers</returns>
        /// <remarks>This is not cached due to the potential volume of data</remarks>
        public List<RestConsumer> RetrieveAllConsumers(LWQueryBatchInfo batchInfo, bool includeSoftDeleted = false)
        {
            return RestConsumerDao.RetrieveAll(batchInfo, includeSoftDeleted);
        }

        /// <summary>
        /// Create a RestConsumer in DB
        /// </summary>
        /// <param name="consumer">source RestConsumer</param>
        public void CreateConsumer(RestConsumer consumer)
        {
            RestConsumerDao.Create(consumer);
            UpdateConsumerCache(consumer);
        }

        /// <summary>
        /// Update a RestConsumer in DB
        /// </summary>
        /// <param name="consumer">source RestConsumer</param>
        public void UpdateConsumer(RestConsumer consumer)
        {
            var existingConsumer = RetrieveConsumer(consumer.Id, true);
            //Clear existing cached data because unique identifier username may change, and we don't want this to be cached anymore
            if (existingConsumer != null)
            {
                ClearConsumerCache(existingConsumer);
            }
            RestConsumerDao.Update(consumer);
            UpdateConsumerCache(consumer);
        }

        /// <summary>
        /// Hard Delete a RestConsumer from DB
        /// </summary>
        /// <param name="consumer">source RestConsumer</param>
        /// <remarks>This is final and cannot be undeleted.</remarks>
        public void DeleteConsumer(RestConsumer consumer)
        {
            RestConsumerDao.Delete(consumer.Id);
            ClearConsumerCache(consumer);
        }

        /// <summary>
        /// Soft Delete a RestConsumer in DB
        /// </summary>
        /// <param name="consumer">source RestConsumer</param>
        /// <remarks>This can be undeleted.</remarks>
        public void SoftDeleteConsumer(RestConsumer consumer)
        {
            RestConsumerDao.SoftDelete(consumer.Id);
            ClearConsumerCache(consumer);
        }

        /// <summary>
        /// Undelete a soft deleted RestConsumer from DB
        /// </summary>
        /// <param name="consumer">source RestConsumer</param>
        public void UndeleteConsumer(RestConsumer consumer)
        {
            RestConsumerDao.Undelete(consumer.Id);
            UpdateConsumerCache(consumer);
        }


        /// <summary>
        /// Update cached RestConsumer
        /// </summary>
        /// <param name="consumer">source RestConsumer</param>
        protected void UpdateConsumerCache(RestConsumer consumer)
        {
            CacheManager.Update(CacheRegions.RestConsumerById, consumer.Id, consumer);
            if (!string.IsNullOrEmpty(consumer.ConsumerId))
            {
                CacheManager.Update(CacheRegions.RestConsumerByConsumerId, consumer.ConsumerId, consumer);
            }
            if (!string.IsNullOrEmpty(consumer.Username))
            {
                CacheManager.Update(CacheRegions.RestConsumerByUsername, consumer.Username, consumer);
            }
        }

        /// <summary>
        /// Clear cached RestConsumer
        /// </summary>
        /// <param name="consumer">source RestConsumer</param>
        protected void ClearConsumerCache(RestConsumer consumer)
        {
            CacheManager.Remove(CacheRegions.RestConsumerById, consumer.Id);
            if (!string.IsNullOrEmpty(consumer.ConsumerId))
            {
                CacheManager.Remove(CacheRegions.RestConsumerByConsumerId, consumer.ConsumerId);
            }
            if (!string.IsNullOrEmpty(consumer.Username))
            {
                CacheManager.Remove(CacheRegions.RestConsumerByUsername, consumer.Username);
            }
            CacheManager.Remove(CacheRegions.RestGroupsByRestConsumerId, consumer.Id);
            CacheManager.Remove(CacheRegions.RestRolesByRestConsumerId, consumer.Id);
        }

        #endregion

        #region RestGroup methods

        /// <summary>
        /// Retrieve RestGroup by primary key Id
        /// </summary>
        /// <param name="id">primary key</param>
        /// <returns>Corresponding RestGroup</returns>
        public RestGroup RetrieveGroupById(long id)
        {
            RestGroup result = (RestGroup) CacheManager.Get(CacheRegions.RestGroupById, id);
            if (result == null)
            {
                result = RestGroupDao.Retrieve(id);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.RestGroupById, id, result);
                    CacheManager.Update(CacheRegions.RestGroupByName, result.Name, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve RestGroup by name
        /// </summary>
        /// <param name="name">Name of RestGroup</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestGroup will also be returned.</param>
        /// <returns></returns>
        public RestGroup RetrieveGroupByName(string name, bool includeSoftDeleted=false)
        {
            if (includeSoftDeleted)
            {
                return RestGroupDao.RetrieveByName(name, true);
            }

            RestGroup result = (RestGroup) CacheManager.Get(CacheRegions.RestGroupByName, name);
            if (result == null)
            {
                result = RestGroupDao.RetrieveByName(name);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.RestGroupByName, name, result);
                    CacheManager.Update(CacheRegions.RestGroupById, result.Id, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve List of RestGroups by RestConsumer.Id (primary key value)
        /// </summary>
        /// <param name="restConsumerId">primary key id of RestConsumer</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestGroups will also be returned.</param>
        /// <returns>Corresponding List of RestGroups</returns>
        public List<RestGroup> RetrieveGroupsByRestConsumerId(long restConsumerId, bool includeSoftDeleted = false)
        {
            if (includeSoftDeleted)
            {
                return RestGroupDao.RetrieveByRestConsumerId(restConsumerId, true);
            }

            List<RestGroup> result =
                (List<RestGroup>) CacheManager.Get(CacheRegions.RestGroupsByRestConsumerId, restConsumerId);
            if (result == null)
            {
                result = RestGroupDao.RetrieveByRestConsumerId(restConsumerId);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.RestGroupsByRestConsumerId, restConsumerId, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve List of RestGroups by RestRole.Id (primary key value)
        /// </summary>
        /// <param name="restRoleId">primary key id of RestRole</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestGroups will also be returned.&lt;/param&gt;
        /// <returns>Corresponding List of RestGroups</returns>
        /// <remarks>This is not cached since it is an admin-only method that does not require high performance</remarks>
        public List<RestGroup> RetrieveGroupsByRestRoleId(long restRoleId, bool includeSoftDeleted = false)
        {
            return RestGroupDao.RetrieveByRestRoleId(restRoleId, includeSoftDeleted);
        }

        /// <summary>
        /// Create a RestGroup in DB
        /// </summary>
        /// <param name="group">source RestGroup</param>
        public void CreateGroup(RestGroup group)
        {
            RestGroupDao.Create(group);
            CacheManager.Update(CacheRegions.RestGroupByName, group.Name, group);
            CacheManager.Update(CacheRegions.RestGroupById, group.Id, group);
        }

        /// <summary>
        /// Update a RestGroup in DB
        /// </summary>
        /// <param name="group">source RestGroup</param>
        public void UpdateGroup(RestGroup group)
        {
            RestGroupDao.Update(group);
            CacheManager.Update(CacheRegions.RestGroupByName, group.Name, group);
            CacheManager.Update(CacheRegions.RestGroupById, group.Id, group);
        }

        /// <summary>
        /// Hard Delete a RestGroup from DB
        /// </summary>
        /// <param name="group">source RestGroup</param>
        /// <remarks>This is final and cannot be undeleted.</remarks>
        public void DeleteGroup(RestGroup group)
        {
            RestGroupDao.Delete(group.Id);
            CacheManager.Remove(CacheRegions.RestGroupByName, group.Name);
            CacheManager.Remove(CacheRegions.RestGroupById, group.Id);
            CacheManager.Remove(CacheRegions.RestRolesByRestGroupId, group.Id);
        }

        /// <summary>
        /// Soft Delete a RestGroup in DB
        /// </summary>
        /// <param name="group">source RestGroup</param>
        /// <remarks>This can be undeleted.</remarks>
        public void SoftDeleteGroup(RestGroup group)
        {
            RestGroupDao.SoftDelete(group.Id);
            CacheManager.Remove(CacheRegions.RestGroupByName, group.Name);
            CacheManager.Remove(CacheRegions.RestGroupById, group.Id);
            CacheManager.Remove(CacheRegions.RestRolesByRestGroupId, group.Id);
        }

        /// <summary>
        /// Undelete a soft deleted RestGroup from DB
        /// </summary>
        /// <param name="group">source RestGroup</param>
        public void UndeleteGroup(RestGroup group)
        {
            RestGroupDao.Undelete(group.Id);
            CacheManager.Update(CacheRegions.RestGroupByName, group.Name, group);
            CacheManager.Update(CacheRegions.RestGroupById, group.Id, group);
        }

        #endregion

        #region RestRole methods
        
        /// <summary>
        /// Retrieve RestRole by primary key id
        /// </summary>
        /// <param name="id">primary key</param>
        /// <returns>Corresponding RestRole</returns>
        public RestRole RetrieveRoleById(long id)
        {
            RestRole result = (RestRole) CacheManager.Get(CacheRegions.RestRoleById, id);
            if (result == null)
            {
                result = RestRoleDao.Retrieve(id);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.RestRoleById, id, result);
                    CacheManager.Update(CacheRegions.RestRoleByName, result.Name, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve RestRole by name
        /// </summary>
        /// <param name="name">Name of RestRole</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestRole will also be returned.</param>
        /// <returns>Corresponding RestRole</returns>
        public RestRole RetrieveRoleByName(string name, bool includeSoftDeleted = false)
        {
            if (includeSoftDeleted)
            {
                return RestRoleDao.RetrieveByName(name, true);
            }

            RestRole result = (RestRole) CacheManager.Get(CacheRegions.RestRoleByName, name);
            if (result == null)
            {
                result = RestRoleDao.RetrieveByName(name);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.RestRoleByName, name, result);
                    CacheManager.Update(CacheRegions.RestRoleById, result.Id, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve List of RestRoles by RestConsumer.Id (primary key)
        /// </summary>
        /// <param name="restConsumerId">primary key id of RestConsumer</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestRoles will also be returned.</param>
        /// <returns>Corresponding List of RestRoles</returns>
        public List<RestRole> RetrieveRolesByConsumerId(long restConsumerId, bool includeSoftDeleted = false)
        {
            if (includeSoftDeleted)
            {
                return RestRoleDao.RetrieveByRestConsumerId(restConsumerId, true);
            }

            List<RestRole> result =
                (List<RestRole>) CacheManager.Get(CacheRegions.RestRolesByRestConsumerId, restConsumerId);
            if (result == null)
            {
                result = RestRoleDao.RetrieveByRestConsumerId(restConsumerId);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.RestRolesByRestConsumerId, restConsumerId, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve List of RestRoles by RestGroup.Id (primary key)
        /// </summary>
        /// <param name="restGroupId">primary key id of RestGroup</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestRoles will also be returned.</param>
        /// <returns>Corresponding List of RestRoles</returns>
        public List<RestRole> RetrieveRolesByGroupId(long restGroupId, bool includeSoftDeleted = false)
        {
            if (includeSoftDeleted)
            {
                return RestRoleDao.RetrieveByRestGroupId(restGroupId, true);
            }

            List<RestRole> result = (List<RestRole>) CacheManager.Get(CacheRegions.RestRolesByRestGroupId, restGroupId);
            if (result == null)
            {
                result = RestRoleDao.RetrieveByRestGroupId(restGroupId);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.RestRolesByRestGroupId, restGroupId, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Create a RestRole in DB
        /// </summary>
        /// <param name="group">source RestRole</param>
        public void CreateRole(RestRole role)
        {
            RestRoleDao.Create(role);
            CacheManager.Update(CacheRegions.RestRoleById, role.Id, role);
            CacheManager.Update(CacheRegions.RestRoleByName, role.Name, role);
        }

        /// <summary>
        /// Update a RestRole in DB
        /// </summary>
        /// <param name="group">source RestRole</param>
        public void UpdateRole(RestRole role)
        {
            RestRoleDao.Update(role);
            CacheManager.Update(CacheRegions.RestRoleById, role.Id, role);
            CacheManager.Update(CacheRegions.RestRoleByName, role.Name, role);
        }

        /// <summary>
        /// Hard Delete a RestRole from DB
        /// </summary>
        /// <param name="group">source RestRole</param>
        /// <remarks>This is final and cannot be undeleted.</remarks>
        public void DeleteRole(RestRole role)
        {
            RestRoleDao.Delete(role.Id);
            CacheManager.Remove(CacheRegions.RestRoleById, role.Id);
            CacheManager.Remove(CacheRegions.RestRoleByName, role.Name);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, role.Id);
        }

        /// <summary>
        /// Soft Delete a RestRole in DB
        /// </summary>
        /// <param name="group">source RestRole</param>
        /// <remarks>This can be undeleted.</remarks>
        public void SoftDeleteRole(RestRole role)
        {
            RestRoleDao.SoftDelete(role.Id);
            CacheManager.Remove(CacheRegions.RestRoleById, role.Id);
            CacheManager.Remove(CacheRegions.RestRoleByName, role.Name);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, role.Id);
        }

        /// <summary>
        /// Undelete a soft deleted RestRole from DB
        /// </summary>
        /// <param name="group">source RestRole</param>
        public void UndeleteRole(RestRole role)
        {
            RestRoleDao.Undelete(role.Id);
            CacheManager.Update(CacheRegions.RestRoleById, role.Id, role);
            CacheManager.Update(CacheRegions.RestRoleByName, role.Name, role);
        }

        #endregion

        #region RestResource methods

        /// <summary>
        /// Retrieve RestResource by primary key id
        /// </summary>
        /// <param name="id">primary key</param>
        /// <returns>Corresponding RestResource</returns>
        public RestResource RetrieveResourceById(long id)
        {
            RestResource result = (RestResource) CacheManager.Get(CacheRegions.RestResourceById, id);
            if (result == null)
            {
                result = RestResourceDao.Retrieve(id);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.RestResourceById, id, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve List of RestResources corresponding to RestRole.Id (primary key)
        /// </summary>
        /// <param name="restRoleId">primary key Id of RestRole</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestResources will also be returned.</param>
        /// <returns>Corresponding List of RestResources</returns>
        public List<RestResource> RetrieveResourcesByRoleId(long restRoleId, bool includeSoftDeleted = false)
        {
            if (includeSoftDeleted)
            {
                return RestResourceDao.RetrieveByRestRoleId(restRoleId, true);
            }

            List<RestResource> result =
                (List<RestResource>) CacheManager.Get(CacheRegions.RestResourcesByRestRoleId, restRoleId);
            if (result == null)
            {
                result = RestResourceDao.RetrieveByRestRoleId(restRoleId);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.RestResourcesByRestRoleId, restRoleId, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve List of RestResources corresponding to RestResourceType
        /// </summary>
        /// <param name="resourceType">RestResourceType</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestResources will also be returned.</param>
        /// <returns>Corresponding List of RestResources</returns>
        public List<RestResource> RetrieveResourcesByType(RestResourceType resourceType,
            bool includeSoftDeleted = false)
        {
            //Note: Caching not enabled because this is unlikely to be used in realtime, as
            //rest resources are more useful by role rather than a list on their own by type.
            return RestResourceDao.RetrieveByResourceType(null, resourceType, includeSoftDeleted);            
        }

        /// <summary>
        /// Retrieve List of RestResources corresponding to RestResourceType in batches
        /// </summary>
        /// <param name="resourceType">RestResourceType</param>
        /// <param name="includeSoftDeleted">optional parameter that is false by default.  If true, soft deleted RestResources will also be returned.</param>
        /// <param name="batchInfo">Paged batch info</param>
        /// <returns>Corresponding List of RestResources</returns>
        public List<RestResource> RetrieveResourcesByType(LWQueryBatchInfo batchInfo, RestResourceType resourceType,
            bool includeSoftDeleted = false)
        {
            //Note: Caching not enabled because this is unlikely to be used in realtime, as
            //rest resources are more useful by role rather than a list on their own by type.
            return RestResourceDao.RetrieveByResourceType(batchInfo, resourceType, includeSoftDeleted);
        }

        /// <summary>
        /// Create a RestResource in DB
        /// </summary>
        /// <param name="resource">source RestResource</param>
        public void CreateResource(RestResource resource)
        {
            RestResourceDao.Create(resource);
            CacheManager.Update(CacheRegions.RestResourceById, resource.Id, resource);
        }

        /// <summary>
        /// Update a RestResource in DB
        /// </summary>
        /// <param name="resource">source RestResource</param>
        public void UpdateResource(RestResource resource)
        {
            RestResourceDao.Update(resource);
            CacheManager.Update(CacheRegions.RestResourceById, resource.Id, resource);
        }

        /// <summary>
        /// Hard Delete a RestResource from DB
        /// </summary>
        /// <param name="group">source RestResource</param>
        /// <remarks>This is final and cannot be undeleted.</remarks>
        public void DeleteResource(RestResource resource)
        {
            RestResourceDao.Delete(resource.Id);
            CacheManager.Remove(CacheRegions.RestResourceById, resource.Id);
        }

        /// <summary>
        /// Soft Delete a RestResource in DB
        /// </summary>
        /// <param name="group">source RestResource</param>
        /// <remarks>This can be undeleted.</remarks>
        public void SoftDeleteResource(RestResource resource)
        {
            RestResourceDao.SoftDelete(resource.Id);
            CacheManager.Remove(CacheRegions.RestResourceById, resource.Id);
        }

        /// <summary>
        /// Undelete a soft deleted RestResource from DB
        /// </summary>
        /// <param name="group">source RestResource</param>
        public void UndeleteResource(RestResource resource)
        {
            RestResourceDao.Undelete(resource.Id);
            CacheManager.Update(CacheRegions.RestResourceById, resource.Id, resource);
        }

        #endregion

        #region XREF Create, Update, Delete methods

        //Note: Updating/Deleting any XRef table row forces a cache refresh on some of the corresponding cache values

        //RestRoleResource
        public void CreateRestRoleResource(RestRoleResource restRoleResource)
        {
            RestRoleResourceDao.Create(restRoleResource);
        }

        public void UpdateRestRoleResource(RestRoleResource restRoleResource)
        {
            RestRoleResourceDao.Update(restRoleResource);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, restRoleResource.RestRoleId);
        }

        /// <summary>
        /// Update a RestRole's permission to a RestResource
        /// </summary>
        /// <param name="restRoleId">Id of RestRole</param>
        /// <param name="restResourceId">Id of RestResource</param>
        /// <param name="permission">Updated permission</param>
        public void UpdateRestRoleResource(long restRoleId, long restResourceId, RestPermissionType permission)
        {
            RestRoleResourceDao.Update(restRoleId, restResourceId, permission);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, restRoleId);
        }

        public void DeleteRestRoleResource(RestRoleResource restRoleResource)
        {
            RestRoleResourceDao.Delete(restRoleResource.Id);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, restRoleResource.RestRoleId);
        }

        public void DeleteRestRoleResource(long restRoleId, long restResourceId)
        {
            RestRoleResourceDao.Delete("RestRoleId", "RestResourceId", restRoleId, restResourceId);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, restRoleId);
        }

        public void SoftDeleteRestRoleResource(RestRoleResource restRoleResource)
        {
            RestRoleResourceDao.SoftDelete(restRoleResource.Id);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, restRoleResource.RestRoleId);
        }

        public void SoftDeleteRestRoleResource(long restRoleId, long restResourceId)
        {
            RestRoleResourceDao.SoftDelete("RestRoleId", "RestResourceId", restRoleId, restResourceId);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, restRoleId);
        }

        public void UndeleteRestRoleResource(RestRoleResource restRoleResource)
        {
            RestRoleResourceDao.Undelete(restRoleResource.Id);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, restRoleResource.RestRoleId);
        }

        public void UndeleteRestRoleResource(long restRoleId, long restResourceId)
        {
            RestRoleResourceDao.Undelete("RestRoleId", "RestResourceId", restRoleId, restResourceId);
            CacheManager.Remove(CacheRegions.RestResourcesByRestRoleId, restRoleId);
        }

        //RestConsumerRole
        public void CreateRestConsumerRole(RestConsumerRole restConsumerRole)
        {
            RestConsumerRoleDao.Create(restConsumerRole);
        }

        //Note: No update method because nothing to update

        public void DeleteRestConsumerRole(RestConsumerRole restConsumerRole)
        {
            RestConsumerRoleDao.Delete(restConsumerRole.Id);
            CacheManager.Remove(CacheRegions.RestRolesByRestConsumerId, restConsumerRole.RestConsumerId);
        }

        public void DeleteRestConsumerRole(long restConsumerId, long restRoleId)
        {
            RestConsumerRoleDao.Delete("RestConsumerId", "RestRoleId", restConsumerId, restRoleId);
            CacheManager.Remove(CacheRegions.RestRolesByRestConsumerId, restConsumerId);
        }

        public void SoftDeleteRestConsumerRole(RestConsumerRole restConsumerRole)
        {
            RestConsumerRoleDao.SoftDelete(restConsumerRole.Id);
            CacheManager.Remove(CacheRegions.RestRolesByRestConsumerId, restConsumerRole.RestConsumerId);
        }

        public void SoftDeleteRestConsumerRole(long restConsumerId, long restRoleId)
        {
            RestConsumerRoleDao.SoftDelete("RestConsumerId", "RestRoleId", restConsumerId, restRoleId);
            CacheManager.Remove(CacheRegions.RestRolesByRestConsumerId, restConsumerId);
        }

        public void UndeleteRestConsumerRole(RestConsumerRole restConsumerRole)
        {
            RestConsumerRoleDao.Undelete(restConsumerRole.Id);
            CacheManager.Remove(CacheRegions.RestRolesByRestConsumerId, restConsumerRole.RestConsumerId);
        }

        public void UndeleteRestConsumerRole(long restConsumerId, long restRoleId)
        {
            RestConsumerRoleDao.Undelete("RestConsumerId", "RestRoleId", restConsumerId, restRoleId);
            CacheManager.Remove(CacheRegions.RestRolesByRestConsumerId, restConsumerId);
        }


        //RestConsumerGroup
        public void CreateRestConsumerGroup(RestConsumerGroup restConsumerGroup)
        {
            RestConsumerGroupDao.Create(restConsumerGroup);
        }

        //Note: No update method because nothing to update

        public void DeleteRestConsumerGroup(RestConsumerGroup restConsumerGroup)
        {
            RestConsumerGroupDao.Delete(restConsumerGroup.Id);
            CacheManager.Remove(CacheRegions.RestGroupsByRestConsumerId, restConsumerGroup.RestConsumerId);
        }

        public void DeleteRestConsumerGroup(long restConsumerId, long restGroupId)
        {
            RestConsumerGroupDao.Delete("RestConsumerId", "RestGroupId", restConsumerId, restGroupId);
            CacheManager.Remove(CacheRegions.RestGroupsByRestConsumerId, restConsumerId);
        }

        public void SoftDeleteRestConsumerGroup(RestConsumerGroup restConsumerGroup)
        {
            RestConsumerGroupDao.SoftDelete(restConsumerGroup.Id);
            CacheManager.Remove(CacheRegions.RestGroupsByRestConsumerId, restConsumerGroup.RestConsumerId);
        }

        public void SoftDeleteRestConsumerGroup(long restConsumerId, long restGroupId)
        {
            RestConsumerGroupDao.SoftDelete("RestConsumerId", "RestGroupId", restConsumerId, restGroupId);
            CacheManager.Remove(CacheRegions.RestGroupsByRestConsumerId, restConsumerId);
        }

        public void UndeleteRestConsumerGroup(RestConsumerGroup restConsumerGroup)
        {
            RestConsumerGroupDao.Undelete(restConsumerGroup.Id);
            CacheManager.Remove(CacheRegions.RestGroupsByRestConsumerId, restConsumerGroup.RestConsumerId);
        }

        public void UndeleteRestConsumerGroup(long restConsumerId, long restGroupId)
        {
            RestConsumerGroupDao.Undelete("RestConsumerId", "RestGroupId", restConsumerId, restGroupId);
            CacheManager.Remove(CacheRegions.RestGroupsByRestConsumerId, restConsumerId);
        }

        //RestGroupRole
        public void CreateRestGroupRole(RestGroupRole restGroupRole)
        {
            RestGroupRoleDao.Create(restGroupRole);
        }

        //Note: No update method because nothing to update

        public void DeleteRestGroupRole(RestGroupRole restGroupRole)
        {
            RestGroupRoleDao.Delete(restGroupRole.Id);
            CacheManager.Remove(CacheRegions.RestRolesByRestGroupId, restGroupRole.RestGroupId);
        }

        public void DeleteRestGroupRole(long restGroupId, long restRoleId)
        {
            RestGroupRoleDao.Delete("RestGroupId", "RestRoleId", restGroupId, restRoleId);
            CacheManager.Remove(CacheRegions.RestRolesByRestGroupId, restGroupId);
        }

        public void SoftDeleteRestGroupRole(RestGroupRole restGroupRole)
        {
            RestGroupRoleDao.SoftDelete(restGroupRole.Id);
            CacheManager.Remove(CacheRegions.RestRolesByRestGroupId, restGroupRole.RestGroupId);
        }

        public void SoftDeleteRestGroupRole(long restGroupId, long restRoleId)
        {
            RestGroupRoleDao.SoftDelete("RestGroupId", "RestRoleId", restGroupId, restRoleId);
            CacheManager.Remove(CacheRegions.RestRolesByRestGroupId, restGroupId);
        }

        public void UndeleteRestGroupRole(RestGroupRole restGroupRole)
        {
            RestGroupRoleDao.Undelete(restGroupRole.Id);
            CacheManager.Remove(CacheRegions.RestRolesByRestGroupId, restGroupRole.RestGroupId);
        }

        public void UndeleteRestGroupRole(long restGroupId, long restRoleId)
        {
            RestGroupRoleDao.Undelete("RestGroupId", "RestRoleId", restGroupId, restRoleId);
            CacheManager.Remove(CacheRegions.RestRolesByRestGroupId, restGroupId);
        }

        #endregion
    }
}
