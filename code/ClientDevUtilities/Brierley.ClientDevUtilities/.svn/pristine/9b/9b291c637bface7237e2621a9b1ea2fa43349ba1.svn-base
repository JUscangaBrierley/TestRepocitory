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
    public class SMCultureMapDao : DaoBase<SMCultureMap>
    {
        public SMCultureMapDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public void SetForceQueryCacheReset(bool value)
        {
            //ForceQueryCacheReset = value;
        }

        /// <summary>
        /// Get a specific CultureMap.
        /// </summary>
        /// <param name="ID">the unique ID for the CultureMap</param>
        /// <returns>specified CultureMap</returns>
        public SMCultureMap Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a specific cultureMap by culture.
        /// </summary>
        /// <param name="culture">the culture (e.g., "en-US")</param>
        /// <returns>specified CultureMap, or none if not found</returns>
        public SMCultureMap RetrieveByCulture(string culture)
        {
            return Database.FirstOrDefault<SMCultureMap>("select * from LW_SM_CultureMap where Culture = @0", culture);
        }

        /// <summary>
        /// Get a list of all CultureMaps.
        /// </summary>
        /// <returns>list of CultureMaps, or null if none</returns>
        public List<SMCultureMap> RetrieveAll()
        {
            return Database.Fetch<SMCultureMap>("select * from LW_SM_CultureMap");
        }

        /// <summary>
        /// Delete a specific CultureMap.
        /// </summary>
        /// <param name="ID">unique ID for the CultureMap</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }
    }
}
