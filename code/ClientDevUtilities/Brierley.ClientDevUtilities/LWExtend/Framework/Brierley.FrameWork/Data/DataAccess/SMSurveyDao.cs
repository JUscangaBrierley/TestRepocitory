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
    public class SMSurveyDao : DaoBase<SMSurvey>
    {
        public SMSurveyDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }
        
        public SMSurvey Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        public SMSurvey Retrieve(string surveyName)
        {
            return Database.FirstOrDefault<SMSurvey>("select * from LW_SM_Survey where lower(Survey_Name) = lower(@0)", surveyName);
        }

        public List<long> Retrieve(string search, DateTime? startDate, DateTime? endDate, SurveyType? type, SurveyAudience? audience, SurveyStatus? status)
        {
            string[] searchList = search.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<object> searchParams = new List<object>();
            string queryStr = "select distinct s.survey_ID "
                            + "from lw_sm_survey s "
                            + "left join LW_SM_State state on state.Survey_ID = s.survey_ID "
                            + "left join LW_SM_Question q on q.State_ID = state.state_ID "
                            + "left join LW_SM_QuestionContent qc on qc.Question_ID = q.question_ID "
                            + "WHERE 1=1";


            List<string> clauses = new List<string>();

            searchParams.Add(startDate);
            if (startDate.HasValue)
            {
                clauses.Add("s.Effective_Date >= @0");
            }
            searchParams.Add(endDate);
            if (endDate.HasValue)
            {
                clauses.Add("s.Expiration_Date < @1");
            }
            searchParams.Add((int?)type);
            if (type.HasValue)
            {
                clauses.Add("SurveyType = @2");
            }
            searchParams.Add((int?)audience);
            if (audience.HasValue)
            {
                clauses.Add("SurveyAudience = @3");
            }
            searchParams.Add((int?)status);
            if (status.HasValue)
            {
                clauses.Add("SurveyStatus = @4");
            }

            List<string> searchClauses = new List<string>();
            if (searchList.Length > 0)
            {
                for (int i = 0; i < searchList.Length; i++)
                {
                    searchClauses.Add(string.Format("(lower(s.survey_name) like @{0} OR lower(s.Survey_Description) like @{0} or lower(state.UIName) like @{0} or lower(state.UIDescription) like @{0})", (i + 5).ToString()));
                    searchParams.Add("%" + searchList[i].Replace("%", string.Empty) + "%");
                }
            }

            if (clauses.Count > 0 || searchClauses.Count > 0)
            {
                bool first = true;
                if (clauses.Count > 0)
                {
                    if(first)
                    {
                        queryStr += " AND ";
                        first = false;
                    }

                    queryStr += string.Join(" AND ", clauses);
                }
                if (searchClauses.Count > 0)
                {
                    queryStr += string.Format(" AND ({0})", string.Join(" OR ", searchClauses));
                }
            }

            return Database.Fetch<long>(queryStr, searchParams.ToArray());
        }

        public List<SMSurvey> RetrieveAll()
        {
            return Database.Fetch<SMSurvey>("select * from LW_SM_Survey order by Survey_Name");
        }

        public List<SMSurvey> RetrieveAll(int statusFilter, int typeFilter, long? folderId = null)
        {
            string sql = "select * from LW_SM_Survey where 1=1";
            
            if (statusFilter > -1)
            {
                if (statusFilter == (int)SurveyStatus.Design)
                {
                    sql += " and (SurveyStatus is null or SurveyStatus=@0)";
                }
                else
                {
                    sql += " and SurveyStatus=@0";
                }
            }
            
            if (typeFilter > -1) sql += " and SurveyType=@1";

            if (folderId.HasValue)
            {
                sql += " and FolderId = @2";
            }

            sql += " order by Survey_Name asc";

            object statusFilterEnum = statusFilter > -1 ? Enum.Parse(typeof(SurveyStatus), statusFilter.ToString()) : null;
            object typeFilterEnum = typeFilter > -1 ? Enum.Parse(typeof(SurveyType), typeFilter.ToString()) : null;

            return Database.Fetch<SMSurvey>(sql, statusFilterEnum, typeFilterEnum, folderId);
        }

        public List<SMSurvey> RetrieveAllProfileSurveys()
        {
            return Database.Fetch<SMSurvey>("select * from LW_SM_Survey where SurveyType = @0 order by DisplayOrder", SurveyType.Profile);
        }

        public List<SMSurvey> RetrieveAllNonProfileSurveys()
        {
            return Database.Fetch<SMSurvey>("select * from LW_SM_Survey where SurveyType != @0 order by Survey_Name", SurveyType.Profile);
        }

        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }
    }
}
