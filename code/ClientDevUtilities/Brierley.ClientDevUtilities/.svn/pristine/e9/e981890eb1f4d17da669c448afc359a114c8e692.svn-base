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
    public class SMConceptDao : DaoBase<SMConcept>
    {
        public SMConceptDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        /// <summary>
        /// Get a specific concept.
        /// </summary>
        /// <param name="ID">the unique ID for the concept</param>
        /// <returns>specified concept, or null if it doesn't exist</returns>
        public SMConcept Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a specific concept by name.
        /// </summary>
        /// <param name="surveyID">the ID of the associated survey</param>
        /// <param name="languageID">the ID of the associated language</param>
        /// <param name="name">the unique name for the concept</param>
        /// <returns>specified concept, or null if it doesn't exist</returns>
        public SMConcept Retrieve(long surveyID, long languageID, string name)
        {
            return Database.FirstOrDefault<SMConcept>("select * from LW_SM_Concept where Survey_Id = @0 and Language_Id = @1 and lower(Name) = lower(@2)", surveyID, languageID, name);
        }

        /// <summary>
        /// Get a list of all concepts.
        /// </summary>
        /// <returns>list of concepts</returns>
        public List<SMConcept> RetrieveAll()
        {
            return Database.Fetch<SMConcept>("select * from LW_SM_Concept");
        }

        /// <summary>
        /// Get a list of concepts for a specific survey in a particular language.
        /// </summary>
        /// <param name="surveyID">the ID of the associated survey</param>
        /// <param name="languageID">the ID of the associated language</param>
        /// <returns>the list of concepts for the specified survey</returns>
        public List<SMConcept> RetrieveAll(long surveyID, long languageID)
        {
            return Database.Fetch<SMConcept>("select * from LW_SM_Concept where Survey_Id = @0 and Language_Id = @1", surveyID, languageID);
        }

        public List<SMConcept> RetrieveAll(long surveyID, long languageID, List<string> names)
        {
            return Database.Fetch<SMConcept>("select * from LW_SM_Concept where Survey_Id = @0 and Language_Id = @1 and Name in (@namesList)", surveyID, languageID, new { namesList = names });
        }

        /// <summary>
        /// Delete a specific concept.
        /// </summary>
        /// <param name="ID">unique ID for the concept</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        /// <summary>
        /// Delete all concepts for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            return Database.Execute("delete from LW_SM_Concept where Survey_ID = @0", surveyID);
        }
    }
}
