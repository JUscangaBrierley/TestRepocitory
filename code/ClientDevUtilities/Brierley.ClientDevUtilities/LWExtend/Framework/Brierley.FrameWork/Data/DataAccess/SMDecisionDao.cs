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
    public class SMDecisionDao : DaoBase<SMDecision>
    {
        public SMDecisionDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        /// <summary>
        /// Get a specific decision.
        /// </summary>
        /// <param name="ID">the unique ID for the decision</param>
        /// <returns>specified decision, or null if it doesn't exist</returns>
        public SMDecision Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a specific decision.
        /// </summary>
        /// <param name="stateID">the ID of the associated state</param>
        /// <returns>the specified decision, or null if it doesn't exist</returns>
        public SMDecision RetrieveByStateID(long stateID)
        {
            return Database.FirstOrDefault<SMDecision>("select * from LW_SM_Decision where State_Id = @0", stateID);
        }

        /// <summary>
        /// Get a list of all decisions.
        /// </summary>
        /// <returns>list of decisions, or null if none</returns>
        public List<SMDecision> RetrieveAll()
        {
            return Database.Fetch<SMDecision>("select * from LW_SM_Decision");
        }

        /// <summary>
        /// Delete a specific decision.
        /// </summary>
        /// <param name="ID">unique ID for the decision</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        /// <summary>
        /// Delete all decisions for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            string queryStr = @"
               delete from LW_SM_Decision where State_ID in (
                  select distinct State_ID from LW_SM_State 
                  where Survey_ID = @0)";

            return Database.Execute(queryStr, surveyID);
        }
    }
}
