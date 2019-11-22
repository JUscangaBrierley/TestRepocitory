using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class SMQuestionContentDao : DaoBase<SMQuestionContent>
    {
        public SMQuestionContentDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public void SetForceQueryCacheReset(bool value)
        {
            //ForceQueryCacheReset = value;
        }

        /// <summary>
        /// Get a specific questionContent.
        /// </summary>
        /// <param name="ID">the unique ID for the questionContent</param>
        /// <returns>specified questionContent</returns>
        public SMQuestionContent Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a list of all questionContents.
        /// </summary>
        /// <returns>list of questionContents, or null if none</returns>
        public List<SMQuestionContent> RetrieveAll()
        {
            return Database.Fetch<SMQuestionContent>("select * from LW_SM_QuestionContent");
        }

        /// <summary>
        /// Get a list of questionContent
        /// </summary>
        /// <param name="questionID">associated question</param>
        /// <param name="languageID">associated language</param>
        /// <param name="contentType">content type</param>
        /// <returns>list of matching QuestionContent</returns>
        public List<SMQuestionContent> RetrieveAllByType(long questionID, long languageID, QuestionContentType contentType)
        {
            return Database.Fetch<SMQuestionContent>("select * from LW_SM_QuestionContent where Question_ID = @0 and Language_Id = @1 and Content_Type = @2 order by Matrix_Index",
                questionID, languageID, contentType);
        }

        /// <summary>
        /// Delete a specific questionContent.
        /// </summary>
        /// <param name="ID">unique ID for the questionContent</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        /// <summary>
        /// Delete all question contents for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            string queryStr = @"
               delete from LW_SM_QuestionContent where Question_ID in (
                  select distinct Question_ID from LW_SM_Question where State_ID in (
                     select distinct State_ID from LW_SM_State 
                     where Survey_ID=@0))";

            return Database.Execute(queryStr, surveyID);
        }
    }
}
