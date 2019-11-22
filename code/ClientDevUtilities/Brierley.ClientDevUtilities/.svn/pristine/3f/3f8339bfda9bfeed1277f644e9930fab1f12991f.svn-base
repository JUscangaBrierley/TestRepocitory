using System;
using System.Collections;
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
    public class SMResponseDao : DaoBase<SMResponse>
    {
        public SMResponseDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public SMResponse Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        public List<SMResponse> RetrieveAll()
        {
            return Database.Fetch<SMResponse>("select * from LW_SM_Response");
        }

        public List<SMResponse> RetrieveAllForState(long respondentID, long stateID)
        {
            return Database.Fetch<SMResponse>("select * from LW_SM_Response where Respondent_Id = @0 and State_Id = @1", respondentID, stateID);
        }

        public List<SMResponse> RetrieveAllForQuestionContent(long questionContentID)
        {
            return Database.Fetch<SMResponse>("select * from LW_SM_Response where QuestionContent_Id = @0", questionContentID);
        }

        public List<SMResponse> RetrieveAllForQuestionContent(long respondentID, long questionContentID)
        {
            return Database.Fetch<SMResponse>("select * from LW_SM_Response where Respondent_Id = @0 and QuestionContent_Id = @1", respondentID, questionContentID);
        }

        public List<dynamic> RetrieveRawResponses(SupportedDataSourceType dataSourceType, long surveyID)
        {
            string rs_content = GetClobColumn(dataSourceType, "rs.content");
            string rt_propertiesxml = GetClobColumn(dataSourceType, "rt.propertiesxml");
            string answerCompareSQL = string.Empty;

            switch (dataSourceType)
            {
                case SupportedDataSourceType.MsSQL2005:
                case SupportedDataSourceType.MySQL55:
                    answerCompareSQL = "(select isnull(ac.Display_Index, -1) from LW_SM_AnswerContent ac where CONVERT(NVARCHAR(MAX), ac.Content) = rs.PipedContent) as answer_index ";
                    break;
                case SupportedDataSourceType.Oracle10g:
                    answerCompareSQL = "(select nvl(ac.Display_Index, -1) from LW_SM_AnswerContent ac where TO_CHAR(ac.Content) = rs.PipedContent) as answer_index ";
                    break;
            }

            string sql = "select rt.respondent_id, rt.mtouch, rt.start_date, rt.complete_date, rt.skipped, "
                + "st.uiname, q.question_id, qc.matrix_index, rs.column_index, rs.answercontent_id, q.answer_control_type, "
                + rs_content + " as Answer, "
                + rt_propertiesxml + " as PropertiesXML, rs.MatrixAnswer_ID, "
                + answerCompareSQL
                + "from lw_sm_state st, lw_sm_question q, lw_sm_questioncontent qc, lw_sm_response rs, lw_sm_respondent rt "
                + "where st.state_id=q.state_id and qc.question_id=q.question_id and rs.questioncontent_id=qc.questioncontent_id "
                + "and rt.respondent_id=rs.respondent_id "
                + "and st.statetype_id in (" + (int)StateType.Question + ", " + (int)StateType.MatrixQuestion + ")"
                + " and st.survey_id=@0 "
                + "order by rt.Mtouch, st.uiname, q.question_id, qc.matrix_index, rs.column_index";

            return Database.Fetch<dynamic>(sql.ToUpper(), surveyID);
        }

        public List<dynamic> RetrieveConceptViews(long surveyID)
        {
            return Database.Fetch<dynamic>(@"select cv.Respondent_ID,c.name from LW_SM_Concept c,LW_SM_ConceptView cv where cv.Concept_ID=c.Concept_ID and c.survey_ID=@0".ToUpper(), surveyID);
        }

        public List<dynamic> RetrieveResponseReport(SupportedDataSourceType dataSourceType, long surveyID, string filter)
        {
            string content = GetClobColumn(dataSourceType, "r.content");
            string pipedcontent = GetClobColumn(dataSourceType, "r.pipedcontent");

            string filterWhere = string.Empty;
            switch (filter)
            {
                case "completed":
                    filterWhere = "and rt.start_date is not null and rt.complete_date is not null and rt.skipped='F' ";
                    break;

                case "terminated":
                    filterWhere = "and rt.start_date is not null and rt.complete_date is not null and rt.skipped='T' ";
                    break;

                case "incompleted":
                    filterWhere = "and rt.start_date is not null and rt.complete_date is null ";
                    break;
            }

            string sql = "select r.QuestionContent_ID, r.AnswerContent_ID, r.MatrixAnswer_ID, r.Column_Index, " + content + " as content, " + pipedcontent + " as pipedcontent, count(r.respondent_id) as count "
                + "from lw_sm_state st, lw_sm_question q, lw_sm_questioncontent qc, lw_sm_response r" + (string.IsNullOrEmpty(filterWhere) ? " " : ", lw_sm_respondent rt ")
                + "where r.QuestionContent_ID=qc.QuestionContent_ID and st.state_id=q.state_id and qc.question_id=q.question_id and st.survey_id=@0 " + filterWhere
                + "group by r.QuestionContent_ID, r.AnswerContent_ID, r.MatrixAnswer_ID, r.Column_Index, " + content + ", " + pipedcontent
                + "order by r.QuestionContent_ID, r.AnswerContent_ID, r.MatrixAnswer_ID, r.Column_Index, " + content + ", " + pipedcontent;

            return Database.Fetch<dynamic>(sql.ToUpper(), surveyID);
        }

        public bool IsQuotaMet(long surveyID, long quota)
        {
            if (quota < 0) return false;

            string queryStr = @"
               select count(distinct r1.respondent_id) from lw_sm_respondent r1 where not r1.complete_date is null and r1.respondent_id in (
                  select distinct r.respondent_id from lw_sm_state s, lw_sm_question q, lw_sm_questioncontent qc, lw_sm_response r  
                  where q.state_id=s.state_id and qc.question_id=q.question_id and r.questioncontent_id=qc.questioncontent_id and s.survey_id=@0
               ) and r1.Skipped='F'
            ";

            return Database.ExecuteScalar<long>(queryStr, surveyID) >= quota;
        }

        public long NumCompleted(long surveyID)
        {
            return Database.ExecuteScalar<long>("select count(distinct r.respondent_id) from lw_sm_respondent r where not r.complete_date is null and r.survey_id=@0 and r.Skipped='F'", surveyID);
        }

        public long NumSkipped(long surveyID)
        {
            return Database.ExecuteScalar<long>(@"select count(distinct r.respondent_id) from lw_sm_respondent r 
                where not r.start_date is null and not r.complete_date is null and r.survey_id=@0 and r.Skipped='T'", surveyID);
        }

        public long NumStarts(long surveyID)
        {
            return Database.ExecuteScalar<long>(@"select count(distinct r.respondent_id) from lw_sm_respondent r 
                where not r.start_date is null and r.complete_date is null and r.survey_id=@0 and r.Skipped='F'", surveyID);
        }

        public long NumRespondents(long surveyID, long questionID, long matrixIndex)
        {
            string queryStr = @"
               select count(distinct r1.respondent_id) from lw_sm_respondent r1 where r1.respondent_id in (
                  select distinct r.respondent_id from lw_sm_state s, lw_sm_question q, lw_sm_questioncontent qc, lw_sm_response r 
                  where 
                     q.state_id=s.state_id and qc.question_id=q.question_id and r.questioncontent_id=qc.questioncontent_id 
                     and s.survey_id=@0 and q.question_id=@1";
            if (matrixIndex >= 0)
                queryStr += " and qc.matrix_index=@2";
            queryStr += ")";

            return Database.ExecuteScalar<long>(queryStr, surveyID, questionID, matrixIndex);
        }

        public bool NeedResponse(long stateID, long languageID, long respondentID)
        {
            string hql = string.Format(@"select count(distinct qc.QuestionContent_ID) from LW_SM_Question q, LW_SM_QuestionContent qc, LW_SM_Response r 
                     where qc.Question_ID=q.Question_ID and qc.Content_Type in ({0},{1})                     
                        and q.State_ID=@0 and qc.Language_ID=@1 
                        and r.QuestionContent_ID=qc.QuestionContent_ID and r.Respondent_ID=@2"
                , (int)QuestionContentType.BODY_TEXT, (int)QuestionContentType.OTHER_SPECIFY_TEXT);

            return Database.ExecuteScalar<long>(hql, stateID, languageID, respondentID) < 1;
        }

        public long RetrieveCountBySurvey(long surveyID)
        {
            return Database.ExecuteScalar<long>(@"select count(*) from LW_SM_Response rs where rs.State_ID in (select s.State_ID from LW_SM_State s where s.Survey_ID=@0)", surveyID);
        }

        public List<long> RetrieveIDs(long surveyID, long languageID = -1)
        {
            if (surveyID < 0)
            {
                throw new Exception("Survey ID is required");
            }

            string hql = "select Response_ID from LW_SM_Response rs where rs.Respondent_ID in (select rt.Respondent_ID from LW_SM_Respondent rt where rt.Survey_ID=@0";
            if (languageID > -1)
            {
                hql += " and rt.Language_ID=@1";
            }
            hql += ")";

            return Database.Fetch<long>(hql, surveyID, languageID);
        }

        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        public int Delete(List<long> responseIDs)
        {
            if (responseIDs.Count < 1)
                throw new ArgumentNullException("responseIDs");

            return Database.Execute("delete from LW_SM_Response where Response_ID in(@responses)", new { responses = responseIDs });
        }

        public int DeleteResponses(long surveyID, long languageID, string mtouch, long ipcode)
        {
            string queryStr = @"
               delete from LW_SM_Response where Respondent_ID in (
                  select distinct Respondent_ID from LW_SM_Respondent 
                  where
                     '1'='1'";
            if (surveyID >= 0) queryStr += " and Survey_ID = @0";
            if (languageID >= 0) queryStr += " and Language_ID = @1";
            if (!string.IsNullOrEmpty(mtouch) && ipcode >= 0)
            {
                queryStr += " and (MTouch = @2 or IPCode = @3)";
            }
            else
            {
                if (!string.IsNullOrEmpty(mtouch)) queryStr += " and MTouch = @2";
                if (ipcode >= 0) queryStr += " and IPCode = @3";
            }
            queryStr += @"
               )
            ";

            return Database.Execute(queryStr, surveyID, languageID, mtouch, ipcode);
        }

        public int DeleteAllForSurvey(long surveyID, long languageID = -1)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            string queryStr = @"
               delete from LW_SM_Response where Respondent_ID in (
                  select distinct Respondent_ID from LW_SM_Respondent 
                  where Survey_ID = @0";
            if (languageID != -1) queryStr += " and Language_ID = @1";
            queryStr += ")";

            return Database.Execute(queryStr, surveyID, languageID);
        }

        public int DeleteAllForQuestionContent(long questionContentID)
        {
            if (questionContentID < 0)
                throw new ArgumentNullException("questionContentID");

            string queryStr = @"delete from LW_SM_Response where QuestionContent_ID = @0";

            return Database.Execute(queryStr, questionContentID);
        }

        public int DeleteAllForRespondent(long respondentID)
        {
            if (respondentID < 0)
                throw new ArgumentNullException("respondentID");

            string sql = @"delete from LW_SM_Response where Respondent_ID = @0";

            return Database.Execute(sql, respondentID);
        }

        public int DeleteAllForRespondentIDs(List<long> respondentIDs)
        {
            if (respondentIDs.Count < 1)
                throw new ArgumentNullException("respondentIDs");

            return Database.Execute("delete from LW_SM_Response where Respondent_ID in(@respondents)", new { respondents = respondentIDs });
        }

        #region private methods
        private string GetClobColumn(SupportedDataSourceType dataSourceType, string columnName)
        {
            string result = columnName;
            switch (dataSourceType)
            {
                case SupportedDataSourceType.Oracle10g:
                    result = string.Format("to_char(dbms_lob.substr({0},2000,1))", columnName);
                    break;
                case SupportedDataSourceType.MsSQL2005:
                    result = string.Format("substring({0},1,datalength({0}))", columnName);
                    break;
            }
            return result;
        }
        #endregion
    }
}
