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
    public class SMStateDao : DaoBase<SMState>
    {
        public SMStateDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        /// <summary>
        /// Get a specific state.
        /// </summary>
        /// <param name="ID">the unique ID for the state</param>
        /// <returns>specified state</returns>
        public SMState Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a specific state.
        /// </summary>
        /// <param name="surveyID">unique identifier for the survey</param>
        /// <param name="name">the UI name for the state</param>
        /// <returns>specified state</returns>
        public SMState Retrieve(long surveyID, string name)
        {
            return Database.FirstOrDefault<SMState>("select * from LW_SM_State where Survey_Id = @0 and UIName = @1", surveyID, name);
        }

        /// <summary>
        /// Get a list of all states.
        /// </summary>
        /// <returns>list of states, or null if none</returns>
        public List<SMState> RetrieveAll()
        {
            return Database.Fetch<SMState>("select * from LW_SM_State");
        }

        /// <summary>
        /// Get a list of the states for a specific survey.
        /// </summary>
        /// <param name="surveyID">unique ID for the survey</param>
        /// <returns>list of states, or null if none</returns>
        public List<SMState> RetrieveAllBySurveyID(long surveyID)
        {
            return Database.Fetch<SMState>("select * from LW_SM_State where Survey_Id = @0", surveyID);
        }

        /// <summary>
        /// Get a list of states on a page.
        /// </summary>
        /// <param name="pageID">unique ID of the page</param>
        /// <returns>list of states</returns>
        public List<SMState> RetrieveAllByPageID(long pageID)
        {
            return Database.Fetch<SMState>("select * from LW_SM_State where Page = @0", pageID);
        }

        /// <summary>
        /// Delete a specific state.
        /// </summary>
        /// <param name="ID">unique ID for the state</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        /// <summary>
        /// Delete all states for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            return Database.Execute("delete from LW_SM_State where Survey_ID=@0", surveyID);
        }
    }
}
