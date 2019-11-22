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
    public class SMConceptViewDao : DaoBase<SMConceptView>
    {
        public SMConceptViewDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public SMConceptView Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        public List<SMConceptView> RetrieveAll()
        {
            return Database.Fetch<SMConceptView>("select * from LW_SM_ConceptView");
        }

        public List<SMConceptView> RetrieveAllByConcept(long conceptID)
        {
            return Database.Fetch<SMConceptView>("select * from LW_SM_ConceptView where Concept_Id = @0", conceptID);
        }

        public List<SMConceptView> RetrieveAllByRespondent(long respondentID)
        {
            return Database.Fetch<SMConceptView>("select * from LW_SM_ConceptView where Respondent_Id = @0", respondentID);
        }

        public List<SMConceptView> RetrieveAllByRespondentAndState(long respondentID, long stateID)
        {
            return Database.Fetch<SMConceptView>("select * from LW_SM_ConceptView where Respondent_Id = @0 and State_Id = @1", respondentID, stateID);
        }

        public Dictionary<long, long> RetrieveSummaryReport(long surveyID, string filter)
        {
            Dictionary<long, long> result = new Dictionary<long, long>();
            
            string filterWhere = string.Empty;
            switch (filter)
            {
                case "completed":
                    filterWhere = "and rt.Start_Date is not null and rt.Complete_Date is not null and rt.Skipped='F' ";
                    break;

                case "terminated":
                    filterWhere = "and rt.Start_Date is not null and rt.Complete_Date is not null and rt.Skipped='T' ";
                    break;

                case "incompleted":
                    filterWhere = "and rt.Start_Date is not null and rt.Complete_Date is null ";
                    break;
            }

            string hql = @"select cv.Concept_ID, count(*) as count from LW_SM_ConceptView cv, LW_SM_Respondent rt 
                           where cv.Respondent_ID = rt.Respondent_ID and cv.Concept_ID in (select Concept_ID from LW_SM_Concept where Survey_ID=@0) " 
                + filterWhere + "group by cv.Concept_ID";

            var rawRows = Database.Fetch<dynamic>(hql.ToUpper(), surveyID);
            if (rawRows != null)
            {
                foreach (dynamic rawRow in rawRows)
                {
                    long conceptID = long.Parse(rawRow.CONCEPT_ID.ToString());
                    long numRespondents = long.Parse(rawRow.COUNT.ToString());
                    result.Add(conceptID, numRespondents);
                }
            }
            return result;
        }

        public long RetrieveCountBySegment(long conceptID, string respondentPropName, string respondentPropValue)
        {
            string likeprop = string.Format("%property name=\"{0}\" value=\"{1}\" %", respondentPropName, respondentPropValue);
            
            string hql = @"select count(*) from LW_SM_ConceptView cv where cv.Concept_ID = @0 and cv.Respondent_ID in (
	                        select r.Respondent_ID from SMRespondent r where r.PropertiesXML like @1)";
            
            return Database.ExecuteScalar<long>(hql, conceptID, likeprop);
        }

        public long RetrieveCountByGroup(long respondentID, string groupName)
        {
            string hql = @"select count(*) from LW_SM_ConceptView cv where cv.Respondent_ID = @0 and cv.Concept_ID in (
                            select c.Concept_ID from LW_SM_Concept c where c.GroupName=@1)";
            return Database.ExecuteScalar<long>(hql, respondentID, groupName);
        }

        public long RetrieveCountBySurvey(long surveyID)
        {
            string hql = "select count(*) from LW_SM_ConceptView cv where cv.Concept_ID in (select c.Concept_ID from LW_SM_Concept c where c.Survey_ID = @0)";
            return Database.ExecuteScalar<long>(hql, surveyID);
        }

        public List<long> RetrieveIDs(long surveyID, long languageID = -1)
        {
            if (surveyID < 0)
            {
                throw new Exception("Survey ID is required");
            }

            string hql = "select ID from LW_SM_ConceptView cv where cv.Concept_ID in (select c.Concept_ID from LW_SM_Concept c where c.Survey_ID=@0";
            if (languageID > -1)
            {
                hql += " and c.LanguageID=@1";
            }
            hql += ")";

            return Database.Fetch<long>(hql, surveyID, languageID);
        }

        public void Delete(long conceptViewID)
        {
            DeleteEntity(conceptViewID);
        }

        public int Delete(List<long> conceptViewIDs)
        {
            if (conceptViewIDs.Count < 1)
                throw new ArgumentNullException("conceptViewIDs");

            return Database.Execute("delete from LW_SM_ConceptView where ConceptView_ID in (@Ids)", new { Ids = conceptViewIDs });
        }

        public int DeleteAllForSurvey(long surveyID, long languageID = -1)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            string queryStr = @"
               delete from LW_SM_ConceptView where Concept_ID in (
                  select distinct Concept_ID from LW_SM_Concept 
                  where Survey_ID=@0";
            if (languageID != -1) queryStr += " and Language_ID=@1";
            queryStr += ")";

            return Database.Execute(queryStr, surveyID, languageID);
        }

        public int DeleteAllForRespondentIDs(List<long> respondentIDs)
        {
            if (respondentIDs.Count < 1)
                throw new ArgumentNullException("respondentIDs");

            return Database.Execute("delete from LW_SM_ConceptView where Respondent_ID in (@Ids)", new { Ids = respondentIDs });
        }
    }
}
