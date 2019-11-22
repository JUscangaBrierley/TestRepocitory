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
    public class SMQuestionDao : DaoBase<SMQuestion>
    {
        public SMQuestionDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        /// <summary>
        /// Get a specific question.
        /// </summary>
        /// <param name="ID">the unique ID for the question</param>
        /// <returns>specified question, or null if it doesn't exist</returns>
        public SMQuestion Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a specific question.
        /// </summary>
        /// <param name="stateID">the ID of the associated state</param>
        /// <returns>the specified question, or null if it doesn't exist</returns>
        public SMQuestion RetrieveByStateID(long stateID)
        {
            return Database.FirstOrDefault<SMQuestion>("select * from LW_SM_Question where State_Id = @0", stateID);
        }

        /// <summary>
        /// Get a specific question.
        /// </summary>
        /// <param name="surveyID">the associated survey</param>
        /// <param name="stateName">the name of the question's state</param>
        /// <returns>the specified question, or null if it doesn't exist</returns>
        public SMQuestion RetrieveByStateName(long surveyID, string stateName)
        {
            return Database.FirstOrDefault<SMQuestion>(@"select * from LW_SM_Question q where q.State_ID in 
                    (select s.State_ID from LW_SM_State s where s.Survey_ID=@0 and s.StateType_ID in (@2,@3) and s.UIName=@1)", 
                surveyID, stateName, (int)StateType.Question, (int)StateType.MatrixQuestion);
        }

        /// <summary>
        /// Get a list of all questions.
        /// </summary>
        /// <returns>list of questions, or null if none</returns>
        public List<SMQuestion> RetrieveAll()
        {
            return Database.Fetch<SMQuestion>("select * from LW_SM_Question");
        }

        /// <summary>
        /// Get a list of the questions for a specific state in a state diagram.
        /// </summary>
        /// <param name="stateID">unique ID for the state</param>
        /// <returns>list of questions, or null if none</returns>
        public List<SMQuestion> RetrieveAllByStateID(long stateID)
        {
            return Database.Fetch<SMQuestion>("select * from LW_SM_Question where State_Id = @0", stateID);
        }

        /// <summary>
        /// Get a list of the questions for a specific survey.
        /// </summary>
        /// <param name="surveyID">unique ID for the survey</param>
        /// <returns>list of questions, or null if none</returns>
        public List<SMQuestion> RetrieveAllBySurveyID(long surveyID)
        {
            return Database.Fetch<SMQuestion>(@"select * from LW_SM_Question q where q.State_ID in 
                    (select s.State_ID from LW_SM_State s where s.Survey_ID=@0 and s.StateType_ID in (@1,@2))", 
                surveyID, (int)StateType.Question, (int)StateType.MatrixQuestion);
        }

        /// <summary>
        /// Delete a specific question.
        /// </summary>
        /// <param name="ID">unique ID for the question</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        /// <summary>
        /// Delete all questions for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            string queryStr = @"
               delete from LW_SM_Question where State_ID in (
                  select distinct State_ID from LW_SM_State 
                  where Survey_ID=@0)";

            return Database.Execute(queryStr, surveyID);
        }
    }
}
