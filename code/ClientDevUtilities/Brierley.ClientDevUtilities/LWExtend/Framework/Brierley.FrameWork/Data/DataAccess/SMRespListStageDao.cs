using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using PetaPoco;
//using Oracle.ManagedDataAccess.Client;



namespace Brierley.FrameWork.Data.DataAccess
{
    public class SMRespListStageDao : DaoBase<SMRespListStage>
    {
        private const string _className = "SMRespListStageDAO";
        private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);

        public SMRespListStageDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

        public bool IsRespondentListStaged(long respListID)
        {
            string sql = "SELECT RespListID FROM LW_SM_RespListStage WHERE RespListID = @0";
            List<long> respListIDs = Database.Fetch<long>(sql, respListID);
            return respListIDs != null && respListIDs.Count > 0;
        }

        public int DeleteStagedRespondentList(long respListID)
        {
            string sql = "DELETE FROM LW_SM_RespListStage WHERE RespListID = @0";
            return Database.Execute(sql, respListID);
        }

        public void StagedRespondentList2File(long respListID, SupportedDataSourceType dbtype, string outputFileName, string connectionString)
        {
            switch (dbtype)
            {
                case SupportedDataSourceType.Oracle10g:
                    {
                        using (OracleConnection conn = new OracleConnection(connectionString))
                        {
                            conn.Open();
                            string sql = "select RespondentList from LW_SM_RespListStage where RespListID = :respListID";
                            using (OracleCommand cmd = new OracleCommand(sql, conn))
                            {
                                OracleParameter parm = new OracleParameter("respListID", DbType.Int64);
                                parm.Value = respListID;
                                cmd.Parameters.Add(parm);
                                using (OracleDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                                {
                                    if (reader != null)
                                    {
                                        if (reader.Read())
                                        {
                                            OracleLob blob = reader.GetOracleLob(0);
                                            using(Stream outputStream = File.OpenWrite(@outputFileName))
                                            using (BufferedStream bufferedOutput = new BufferedStream(outputStream))
                                            {
                                                byte[] buffer = new byte[blob.ChunkSize * 10];
                                                int actual = 0;
                                                while ((actual = blob.Read(buffer, 0, buffer.Length)) > 0)
                                                {
                                                    bufferedOutput.Write(buffer, 0, actual);
                                                    bufferedOutput.Flush();
                                                }
                                                bufferedOutput.Close();
                                            }
                                        }
                                    }
                                }
                            }
                            conn.Close();
                        }
                    }
                    break;

                case SupportedDataSourceType.MsSQL2005:
                    {
                        using (IDbCommand cmd = Database.Connection.CreateCommand())
                        {
                            cmd.CommandText = "select RespondentList from LW_SM_RespListStage where RespListID = @respListID";
                            IDbDataParameter parm = cmd.CreateParameter();
                            parm.ParameterName = "respListID";
                            parm.DbType = DbType.Int64;
                            parm.Value = respListID;
                            cmd.Parameters.Add(parm);
                            using (IDataReader genericReader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                            {
                                SqlDataReader myReader = genericReader as SqlDataReader;
                                if (myReader != null)
                                {
                                    int bufferSize = 10000;
                                    byte[] outbyte = new byte[bufferSize];
                                    long retval;
                                    long startIndex = 0;
                                    while (myReader.Read())
                                    {
                                        using(FileStream fs = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write))
                                        using (BinaryWriter bw = new BinaryWriter(fs))
                                        {
                                            startIndex = 0;
                                            retval = myReader.GetBytes(0, startIndex, outbyte, 0, bufferSize);
                                            while (retval == bufferSize)
                                            {
                                                bw.Write(outbyte);
                                                bw.Flush();
                                                startIndex += bufferSize;
                                                retval = myReader.GetBytes(0, startIndex, outbyte, 0, bufferSize);
                                            }
                                            bw.Write(outbyte, 0, (int)retval);
                                            bw.Flush();
                                            bw.Close();
                                            fs.Close();
                                        }
                                    }
                                    myReader.Close();
                                }
                            }
                        }
                    }
                    break;

                default:
                    throw new Exception(string.Format("DBType '{0}' not supported", dbtype.ToString()));
            }
        }

        public void File2StagedRespondentList(long respListID, SupportedDataSourceType dbtype, string inputFileName, string connectionString)
        {
            const string methodName = "File2StagedRespondentList";

            switch (dbtype)
            {
                case SupportedDataSourceType.Oracle10g:
                    {
                        // Unfortunately, this connection string is incomplete. It is missing the password.
                        //string connectionString = CurrentSession.Connection.ConnectionString;
                        using (OracleConnection conn = new OracleConnection(connectionString))
                        {
                            conn.Open();

                            string insertSQL = "insert into LW_SM_RespListStage (RespListStage_ID,RespListID,RespondentList,CreateDate,UpdateDate) values(hibernate_sequence.nextval,:respListID,empty_blob(),sysdate,sysdate)";
                            using (OracleCommand cmd = new OracleCommand(insertSQL, conn))
                            {
                                OracleParameter parm = new OracleParameter("respListID", DbType.Int64);
                                parm.Value = respListID;
                                cmd.Parameters.Add(parm);
                                cmd.ExecuteNonQuery();
                            }

                            string selectSQL = "select RespondentList from LW_SM_RespListStage where RespListID = :respListID FOR UPDATE NOWAIT";
                            using (OracleCommand cmd = new OracleCommand(selectSQL, conn))
                            {
                                OracleParameter parm = new OracleParameter("respListID", DbType.Int64);
                                parm.Value = respListID;
                                cmd.Parameters.Add(parm);
                                using (OracleTransaction trans = conn.BeginTransaction())
                                {
                                    cmd.Transaction = trans;
                                    _logger.Trace(_className, methodName, "begin updating blob");
                                    using (OracleDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            OracleLob blob = reader.GetOracleLob(0);
                                            using (FileStream fls = new FileStream(@inputFileName, FileMode.Open, FileAccess.Read))
                                            {
                                                byte[] buffer = new byte[blob.ChunkSize * 10];
                                                int actual = 0;
                                                while ((actual = fls.Read(buffer, 0, buffer.Length)) > 0)
                                                {
                                                    blob.Write(buffer, 0, actual);
                                                }
                                                blob.Close();
                                                fls.Close();
                                            }
                                        }
                                    }
                                    _logger.Trace(_className, methodName, "end updating blob");
                                    trans.Commit();
                                }
                            }
                            conn.Close();
                        }
                    }
                    break;

                case SupportedDataSourceType.MsSQL2005:
                    {
                        // TODO: implement as FILESTREAM and don't read entire file into memory
                        SqlConnection conn = Database.Connection as SqlConnection;
                        byte[] data = File.ReadAllBytes(inputFileName);
                        string insertSQL = "INSERT INTO LW_SM_RespListStage (RespListID,RespondentList,CreateDate,UpdateDate) VALUES(@respListID,@blobFieldValue,GETDATE(),GETDATE())";
                        using (SqlCommand cmd = new SqlCommand(insertSQL, conn))
                        {
                            cmd.Parameters.Add("@respListID", SqlDbType.BigInt).Value = respListID;
                            cmd.Parameters.Add("@blobFieldValue", SqlDbType.Image, data.Length).Value = data;
                            cmd.CommandTimeout = 0;
                            _logger.Trace(_className, methodName, "begin: " + insertSQL);
                            cmd.ExecuteNonQuery();
                            _logger.Trace(_className, methodName, "end: " + insertSQL);
                        }
                    }
                    break;

                default:
                    throw new Exception(string.Format("DBType '{0}' not supported", dbtype.ToString()));
            }
        }
    }
}
