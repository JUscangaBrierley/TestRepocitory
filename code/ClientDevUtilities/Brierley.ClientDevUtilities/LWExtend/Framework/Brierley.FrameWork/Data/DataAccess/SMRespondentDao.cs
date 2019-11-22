using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class SMRespondentDao : DaoBase<SMRespondent>
    {
        public SMRespondentDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public void SetForceQueryCacheReset(bool value)
        {
            //ForceQueryCacheReset = value;
        }

        public void CreateRespondentsOracle(long surveyID, long languageID, long respListID, List<string> mtouchValues, List<string> propertiesXMLs)
        {
            if (mtouchValues.Count < 1)
                throw new ArgumentException("mtouchValues.Count < 1");
            if (propertiesXMLs.Count < 1)
                throw new ArgumentException("propertiesXMLs.Count < 1");
            if (mtouchValues.Count != propertiesXMLs.Count)
                throw new ArgumentException("mtouchValues.Count != propertiesXMLs.Count");

            string sql = string.Format(@"INSERT INTO LW_SM_Respondent(Respondent_Id, MTouch, IPCode, Survey_Id, Language_Id, CreateDate, UpdateDate, Skipped, PropertiesXml, RespListID) 
                                         VALUES (hibernate_sequence.nextval,:mtouchvalue,-1,{0},{1},sysdate,sysdate,'F',:propertiesXml,{2})",
                surveyID, languageID, respListID);

            var cmd = Database.Connection.CreateCommand();
            cmd.CommandText = sql.ToString();

            var mtouchvalueParm = cmd.CreateParameter();
            mtouchvalueParm.ParameterName = "mtouchvalue";
            mtouchvalueParm.DbType = System.Data.DbType.String;
            mtouchvalueParm.Value = ((List<string>)mtouchValues).ToArray();
            cmd.Parameters.Add(mtouchvalueParm);

            var propertiesXmlParm = cmd.CreateParameter();
            propertiesXmlParm.ParameterName = "propertiesXml";
            propertiesXmlParm.DbType = System.Data.DbType.String;
            propertiesXmlParm.Value = ((List<string>)propertiesXMLs).ToArray();
            cmd.Parameters.Add(propertiesXmlParm);

            cmd.Prepare();
            cmd.GetType().InvokeMember("ArrayBindCount", System.Reflection.BindingFlags.SetProperty, null, cmd, new object[] { mtouchValues.Count });
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Reset the start and completion dates for a set of respondents.
        /// </summary>
        /// <param name="surveyID">ID of the survey, or -1 for all</param>
        /// <param name="languageID">ID of the language, or -1 for all</param>
        /// <param name="mtouch">MTouch code, or null/string.Empty for unspecified</param>
        /// <param name="ipcode">IPCode, or -1 for unspecified</param>
        /// <returns>number of rows updated</returns>
        public int Reset(long surveyID, long languageID, string mtouch, long ipcode)
        {
            string queryStr = "update LW_SM_Respondent set Start_Date=null, Complete_Date=null, Skipped='F' where (not Start_Date is null or not Complete_Date is null)";
            if (surveyID >= 0) queryStr += " and Survey_ID=@0";
            if (languageID >= 0) queryStr += " and Language_ID=@1";
            if (!string.IsNullOrEmpty(mtouch) && ipcode >= 0)
            {
                queryStr += " and (MTouch=@2 or IPCode=@3)";
            }
            else
            {
                if (!string.IsNullOrEmpty(mtouch)) queryStr += " and MTouch=@2";
                if (ipcode >= 0) queryStr += " and IPCode=@3";
            }

            return Database.Execute(queryStr, surveyID, languageID, mtouch, ipcode);
        }

        private string AddWhere(string whereClause, string newClause)
        {
            string result = whereClause;
            if (newClause != string.Empty)
            {
                if (whereClause == string.Empty)
                    result += " where " + newClause;
                else
                    result += " and " + newClause;
            }
            return result;
        }

        /// <summary>
        /// Get a specific respondent.
        /// </summary>
        /// <param name="ID">the unique ID for the respondent</param>
        /// <returns>specified respondent</returns>
        public SMRespondent Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        /// <summary>
        /// Get a specific respondent by MTouch code.
        /// </summary>
        /// <param name="surveyID">unique identifier for the survey</param>
        /// <param name="languageID">unique identifier for the language</param>
        /// <param name="mtouch">unique MTouch code</param>
        /// <returns>matching respondent or null if none</returns>
        public SMRespondent RetrieveByMTouch(long surveyID, long languageID, string mtouch)
        {
            return Database.FirstOrDefault<SMRespondent>("select * from LW_SM_Respondent where Survey_Id = @0 and Language_Id = @1 and MTouch = @2 order by Survey_Id desc, Language_Id desc"
                , surveyID, languageID, mtouch);
        }

        /// <summary>
        /// Get a specific respondent by IPCode.
        /// </summary>
        /// <param name="surveyID">unique identifier for the survey</param>
        /// <param name="languageID">unique identifier for the language</param>
        /// <param name="ipcode">unique IPCode for the member</param>
        /// <returns>matching respondent or null if none</returns>
        public SMRespondent RetrieveByIPCode(long surveyID, long languageID, long ipcode)
        {
            return Database.FirstOrDefault<SMRespondent>("select * from LW_SM_Respondent where Survey_Id = @0 and Language_Id = @1 and IPCode = @2 order by Survey_Id desc, Language_Id desc"
                , surveyID, languageID, ipcode);
        }

        /// <summary>
        /// Get a list of all respondents.
        /// </summary>
        /// <returns>list of respondents, or null if none</returns>
        public List<SMRespondent> RetrieveAll()
        {
            return Database.Fetch<SMRespondent>("select * from LW_SM_Respondent");
        }

        /// <summary>
        /// Get a list of provisioned MTouch codes.
        /// </summary>
        /// <returns>list of MTouch codes</returns>
        public List<string> RetrieveAllMTouches()
        {
            return Database.Fetch<string>("select distinct r.MTouch from LW_SM_Respondent r where r.IPCode=-1 order by r.MTouch");
        }

        /// <summary>
        /// Get a list of provisioned MTouch codes by survey ID
        /// </summary>
        /// <param name="surveyID">unique ID for the survey</param>
        /// <returns>list of MTouch codes</returns>
        public List<string> RetrieveAllMTouchesForSurveyID(long surveyID)
        {
            if (surveyID == -1) throw new Exception("Invalid survey ID: -1");

            return Database.Fetch<string>("select distinct r.MTouch from LW_SM_Respondent r where r.IPCode=-1 and r.Survey_ID=@0 order by r.MTouch", surveyID);
        }

        /// <summary>
        /// Get a list of provisioned MTouch codes by language ID
        /// </summary>
        /// <param name="languageID">unique ID for the language</param>
        /// <returns>list of MTouch codes</returns>
        public List<string> RetrieveAllMTouchesForLanguageID(long languageID)
        {
            if (languageID == -1) throw new Exception("Invalid language ID: -1");

            return Database.Fetch<string>("select distinct r.MTouch from LW_SM_Respondent r where r.IPCode=-1 and r.Language_ID=@0 order by r.MTouch", languageID);
        }

        /// <summary>
        /// Get a list of provisioned MTouch codes by survey ID and language ID
        /// </summary>
        /// <param name="surveyID">unique ID for the survey</param>
        /// <param name="languageID">unique ID for the language</param>
        /// <returns>list of MTouch codes</returns>
        public List<string> RetrieveAllMTouches(long surveyID, long languageID)
        {
            if (surveyID == -1) throw new Exception("Invalid survey ID: -1");
            if (languageID == -1) throw new Exception("Invalid language ID: -1");

            return Database.Fetch<string>("select distinct r.MTouch from LW_SM_Respondent r where r.IPCode=-1 and r.Survey_ID=@0 and r.Language_ID=@1 order by r.MTouch", surveyID, languageID);
        }

        /// <summary>
        /// Get a list of provisioned IPCodes
        /// </summary>
        /// <returns>list of IPCodes</returns>
        public List<long> RetrieveAllIPCodes()
        {
            return Database.Fetch<long>("select distinct r.IPCode from LW_SM_Respondent r where r.IPCode!=-1 order by r.IPCode");
        }

        /// <summary>
        /// Get a list of provisioned IPCodes by survey ID
        /// </summary>
        /// <param name="surveyID">unique ID for the survey</param>
        /// <returns>list of IPCodes</returns>
        public List<long> RetrieveAllIPCodesForSurveyID(long surveyID)
        {
            if (surveyID == -1) throw new Exception("Invalid survey ID: -1");

            return Database.Fetch<long>("select distinct r.IPCode from LW_SM_Respondent r where r.IPCode!=-1 and r.Survey_ID=@0 order by r.IPCode", surveyID);
        }

        /// <summary>
        /// Get a list of provisioned IPCodes by language ID
        /// </summary>
        /// <param name="languageID">unique ID for the language</param>
        /// <returns>list of IPCodes</returns>
        public List<long> RetrieveAllIPCodesForLanguageID(long languageID)
        {
            if (languageID == -1) throw new Exception("Invalid language ID: -1");

            return Database.Fetch<long>("select distinct r.IPCode from LW_SM_Respondent r where r.IPCode!=-1 and r.Language_ID=@0 order by r.IPCode", languageID);
        }

        /// <summary>
        /// Get a list of provisioned IPCodes by survey ID and language ID
        /// </summary>
        /// <param name="surveyID">unique ID for the survey</param>
        /// <param name="languageID">unique ID for the language</param>
        /// <returns>list of IPCodes</returns>
        public List<long> RetrieveAllIPCodes(long surveyID, long languageID)
        {
            if (surveyID == -1) throw new Exception("Invalid survey ID: -1");
            if (languageID == -1) throw new Exception("Invalid language ID: -1");

            return Database.Fetch<long>("select distinct r.IPCode from LW_SM_Respondent r where r.IPCode!=-1 and r.Survey_ID=@0 and r.Language_ID=@1 order by r.IPCode", surveyID, languageID);
        }

        /// <summary>
        /// Get a list of eligible MTouch respondents
        /// </summary>
        /// <param name="surveyID">unique survey ID</param>
        /// <param name="languageID">unique language ID</param>
        /// <param name="maxResults">max results to return</param>
        /// <returns>list of respondents</returns>
        public List<SMRespondent> RetrieveEligibleMTouches(long surveyID, long languageID, int maxResults)
        {
            if (maxResults < 1 || maxResults > 10000)
                throw new ArgumentException("maxResults " + maxResults + " not in [1..10000]");

            return Database.Fetch<SMRespondent>(1, maxResults
                , @"select r.* from LW_SM_Respondent r where r.Survey_ID=@0 and r.Language_ID=@1 and not r.MTouch is null and (r.Complete_Date is null or r.Complete_Date='')"
                , surveyID, languageID) ?? new List<SMRespondent>();
        }

        /// <summary>
        /// Get a list of all eligible respondents for a mtouch and/or ipcode.
        /// </summary>
        /// <param name="surveyID">unique identifier for the survey</param>
        /// <param name="languageID">unique identifier for the language</param>
        /// <param name="mtouch">mtouch</param>
        /// <param name="ipcode">ipcode</param>
        /// <returns>list of eligible respondents, or null if none</returns>
        public List<SMRespondent> RetrieveAllEligible(long surveyID, long languageID, string mtouch, long ipcode)
        {
            if (string.IsNullOrEmpty(mtouch) && ipcode == -1) return null;

            string sql = "select * from LW_SM_Respondent where Survey_Id = @0 and Language_Id = @1";
            if (ipcode == -1)
                sql += " and MTouch = @2";
            else if (string.IsNullOrEmpty(mtouch))
                sql += " and IPCode = @3";
            else
                sql += " and (MTouch = @2 or IPCode = @3)";

            return Database.Fetch<SMRespondent>(sql, surveyID, languageID, mtouch, ipcode);
        }

        /// <summary>
        /// Get a list of all eligible respondents for a mtouch and/or ipcode.
        /// </summary>
        /// <param name="languageID">unique identifier for the language</param>
        /// <param name="mtouch">mtouch</param>
        /// <param name="ipcode">ipcode</param>
        /// <returns>list of eligible respondents, or null if none</returns>
        public List<SMRespondent> RetrieveAllEligible(long languageID, string mtouch, long ipcode)
        {
            if (string.IsNullOrEmpty(mtouch) && ipcode == -1) return null;

            string sql = "select * from LW_SM_Respondent where Language_Id = @2";
            if (ipcode == -1)
                sql += " and MTouch = @0";
            else if (string.IsNullOrEmpty(mtouch))
                sql += " and IPCode = @1";
            else
                sql += " and (MTouch = @0 or IPCode = @1)";
            sql += " order by Survey_Id desc, Language_Id desc";

            return Database.Fetch<SMRespondent>(sql, mtouch, ipcode, languageID);
        }

        /// <summary>
        /// Evict cached respondent from cache
        /// </summary>
        /// <param name="respondent">respondent</param>
        public void EvictRespondentFromCache(SMRespondent respondent)
        {
            // Old NHibernate-related function
        }

        /// <summary>
        /// Get a list of all respondents for a specific survey and language.
        /// </summary>
        /// <param name="surveyID">associated survey</param>
        /// <param name="languageID">associated language</param>
        /// <returns>list of matching Respondent</returns>
        public List<SMRespondent> RetrieveAllForSurvey(long surveyID, long languageID)
        {
            return Database.Fetch<SMRespondent>("select * from LW_SM_Respondent r where r.Survey_ID=@0 and r.Language_ID=@1 order by r.ID asc", surveyID, languageID);
        }

        /// <summary>
        /// Is the provided mtouch valid?  That is, does it exist in the Respondent table?
        /// </summary>
        /// <param name="mtouch">mtouch code</param>
        /// <returns>true if valid, false otherwise</returns>
        public bool IsValidMTouch(string mtouch)
        {
            return Database.ExecuteScalar<long>("select count(r.MTouch) from LW_SM_Respondent r where r.MTouch=@0", mtouch) > 0;
        }

        public long NumRespondents(long surveyID, long languageID)
        {
            string sql = "select count(*) from LW_SM_Respondent where Survey_ID=@0";
            if (languageID > -1)
            {
                sql += " and Language_ID=@1";
            }

            return Database.ExecuteScalar<long>(sql, surveyID, languageID);
        }

        /// <summary>
        /// Number of completions by segment
        /// </summary>
        /// <param name="surveyID">unique ID of the survey</param>
        /// <param name="respondentPropName">segment property name</param>
        /// <param name="respondentPropValue">segment property value</param>
        /// <returns>count of completed surveys for specified segment</returns>
        public long NumCompletesForSegment(long surveyID, string respondentPropName, string respondentPropValue)
        {
            string likeprop = string.Format("%property name=\"{0}\" value=\"{1}\" %", respondentPropName, respondentPropValue);
            string sql = @"select count(*) from LW_SM_Respondent r where r.Survey_ID=@0 and r.Skipped='F' and (not r.Start_Date is null) and (not r.Complete_Date is null) and (PropertiesXML like @1)";

            return Database.ExecuteScalar<long>(sql, surveyID, likeprop);
        }

        public IDataReader RetrieveRespondentListExport(long respListID, SupportedDataSourceType dbtype)
        {
            string parmDelimiter = ":";
            if (dbtype == SupportedDataSourceType.MsSQL2005)
            {
                parmDelimiter = "@";
            }
            string sql = string.Format("select MTouch,{1} from LW_SM_Respondent where RespListID = {0}respListID order by Respondent_ID", parmDelimiter, GetClobColumn(dbtype, "PropertiesXML"));

            IDbCommand cmd = Database.Connection.CreateCommand();
            cmd.CommandText = sql;
            IDbDataParameter parm = cmd.CreateParameter();
            parm.ParameterName = "respListID";
            parm.DbType = System.Data.DbType.Int64;
            parm.Value = respListID;
            cmd.Parameters.Add(parm);

            IDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public List<long> RetrieveIDs(long surveyID, long languageID, string mtouch, long ipcode, long? respListID)
        {
            string whereClause = string.Empty;
            if (surveyID > -1)
            {
                whereClause += "r.Survey_ID=@0";
            }
            if (languageID > -1)
            {
                whereClause += "r.Language_ID=@1";
            }
            if (!string.IsNullOrWhiteSpace(mtouch))
            {
                whereClause += "r.MTouch=@2";
            }
            if (ipcode > -1)
            {
                whereClause += "r.IPCode=@3";
            }
            if (respListID.HasValue)
            {
                whereClause += "r.RespListID=@4";
            }

            if (string.IsNullOrWhiteSpace(whereClause))
            {
                throw new Exception("Must have at least one parameter for where clause");
            }

            string sql = "select Respondent_ID from LW_SM_Respondent r where 1=1 " + whereClause;

            return Database.Fetch<long>(sql, surveyID, languageID, mtouch, ipcode, respListID);
        }

        /// <summary>
        /// Delete a specific respondent.
        /// </summary>
        /// <param name="ID">unique ID for the respondent</param>
        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        /// <summary>
        /// Delete all respondents for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            return Database.Execute("delete from LW_SM_Respondent where Survey_ID=@0", surveyID);
        }

        public int DeleteAllForRespondentIDs(IList<long> respondentIDs)
        {
            if (respondentIDs.Count < 1)
                throw new ArgumentNullException("respondentIDs");

            return Database.Execute("delete from LW_SM_Respondent where Respondent_ID in (@respondents)", new { respondents = respondentIDs });
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
