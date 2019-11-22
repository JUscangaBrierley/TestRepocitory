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
    public class SMAnswerContentDao : DaoBase<SMAnswerContent>
    {
        public SMAnswerContentDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public void SetForceQueryCacheReset(bool value)
        {
            //ForceQueryCacheReset = value;
        }

        /// <summary>
        /// Get a specific answerContent.
        /// </summary>
        /// <param name="ID">the unique ID for the answerContent</param>
        /// <returns>specified answerContent</returns>
        public SMAnswerContent Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a list of all answerContents.
        /// </summary>
        /// <returns>list of answerContents, or null if none</returns>
        public List<SMAnswerContent> RetrieveAll()
        {
            return Database.Fetch<SMAnswerContent>("select * from LW_SM_AnswerContent");
        }

        /// <summary>
        /// Get a list of all answerContents for a specific question and language.
        /// </summary>
        /// <param name="questionID">associated question</param>
        /// <param name="languageID">associated language</param>
        /// <returns>list of matching AnswerContent</returns>
        public List<SMAnswerContent> RetrieveAllForQuestion(long questionID, long languageID)
        {
            return Database.Fetch<SMAnswerContent>("select * from LW_SM_AnswerContent where Question_Id = @0 and Language_Id = @1 order by Display_Index", questionID, languageID);
        }

        /// <summary>
        /// Delete a specific answerContent.
        /// </summary>
        /// <param name="ID">unique ID for the answerContent</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        /// <summary>
        /// Delete all answer contents for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            string queryStr = @"
               delete from LW_SM_AnswerContent where Question_ID in (
                  select distinct Question_ID from LW_SM_Question where State_ID in (
                     select distinct State_ID from LW_SM_State 
                     where Survey_ID=@0))";

            return Database.Execute(queryStr, surveyID);
        }
    }
}
