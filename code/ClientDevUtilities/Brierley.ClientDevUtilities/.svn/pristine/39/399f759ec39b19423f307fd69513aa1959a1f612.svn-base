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
    public class SMMatrixAnswerDao : DaoBase<SMMatrixAnswer>
    {
        public SMMatrixAnswerDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        /// <summary>
        /// Get a specific matrixAnswer.
        /// </summary>
        /// <param name="ID">the unique ID for the matrixAnswer</param>
        /// <returns>specified matrixAnswer, or null if it doesn't exist</returns>
        public SMMatrixAnswer Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a list of matrixAnswer for a questionContentID.
        /// </summary>
        /// <param name="stateID">the ID of the associated questionContent</param>
        /// <returns>the list of matrixAnswer, or null if none</returns>
        public List<SMMatrixAnswer> RetrieveByQuestionContentID(long questionContentID)
        {
            return Database.Fetch<SMMatrixAnswer>("select * from LW_SM_MatrixAnswer ma where ma.QuestionContent_ID=@0 order by Column_Index asc", questionContentID);
        }

        /// <summary>
        /// Get a list of the matrixAnswers for a specific question/language.
        /// </summary>
        /// <param name="questionID">unique ID for the question</param>
        /// <param name="languageID">unique ID for the language</param>
        /// <returns>list of matrixAnswers, or null if none</returns>
        public List<SMMatrixAnswer> RetrieveByQuestionID(long questionID, long languageID)
        {
            return Database.Fetch<SMMatrixAnswer>(@"select * from LW_SM_MatrixAnswer ma where ma.QuestionContent_ID in 
                (select qc.QuestionContent_ID from LW_SM_QuestionContent qc where qc.Question_ID=@0 and qc.Language_ID=@1)", questionID, languageID);
        }

        /// <summary>
        /// Delete a specific matrixAnswer.
        /// </summary>
        /// <param name="ID">unique ID for the matrixAnswer</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        /// <summary>
        /// Delete all matrix answers for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            string queryStr = @"
				delete from LW_SM_MatrixAnswer where QuestionContent_ID in (
					select QuestionContent_ID from LW_SM_QuestionContent where Question_ID in (
						select distinct Question_ID from LW_SM_Question where State_ID in (
						select distinct State_ID from LW_SM_State where Survey_ID=@0
				)))
			";

            return Database.Execute(queryStr, surveyID);
        }
    }
}
