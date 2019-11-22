using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class SMLanguageDao : DaoBase<SMLanguage>
    {
        public SMLanguageDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public void SetForceQueryCacheReset(bool value)
        {
            //ForceQueryCacheReset = value;
        }

        /// <summary>
        /// Get a specific SMLanguage.
        /// </summary>
        /// <param name="ID">the unique ID for the SMLanguage</param>
        /// <returns>specified SMLanguage</returns>
        public SMLanguage Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a specific SMLanguage.
        /// </summary>
        /// <param name="description">description (e.g., "English")</param>
        /// <returns>specified SMLanguage, or null if not found</returns>
        public SMLanguage RetrieveByDescription(string description)
        {
            return Database.FirstOrDefault<SMLanguage>("select * from LW_SM_Language where Language_Description = @0", description);
        }

        /// <summary>
        /// Get a list of all Languages.
        /// </summary>
        /// <returns>list of Languages, or null if none</returns>
        public List<SMLanguage> RetrieveAll()
        {
            return Database.Fetch<SMLanguage>("select * from LW_SM_Language");
        }

        /// <summary>
        /// Delete a specific SMLanguage.
        /// </summary>
        /// <param name="ID">unique ID for the SMLanguage</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }
    }
}
