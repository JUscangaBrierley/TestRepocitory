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
    public class SMMessageDao : DaoBase<SMMessage>
    {
        public SMMessageDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        /// <summary>
        /// Get a specific message.
        /// </summary>
        /// <param name="ID">the unique ID for the message</param>
        /// <returns>specified message, or null if it doesn't exist</returns>
        public SMMessage Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a specific message.
        /// </summary>
        /// <param name="stateID">the ID of the associated state</param>
        /// <returns>the specified message, or null if it doesn't exist</returns>
        public SMMessage RetrieveByStateID(long stateID)
        {
            return Database.FirstOrDefault<SMMessage>("select * from LW_SM_Message where State_Id = @0", stateID);
        }

        /// <summary>
        /// Get a list of all messages.
        /// </summary>
        /// <returns>list of messages, or null if none</returns>
        public List<SMMessage> RetrieveAll()
        {
            return Database.Fetch<SMMessage>("select * from LW_SM_Message");
        }

        /// <summary>
        /// Delete a specific message.
        /// </summary>
        /// <param name="ID">unique ID for the message</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        /// <summary>
        /// Delete all messages for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            string queryStr = @"
               delete from LW_SM_Message where State_ID in (
                  select distinct State_ID from LW_SM_State 
                  where Survey_ID = @0)";

            return Database.Execute(queryStr, surveyID);
        }
    }
}
