using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.Sql;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class GetMembersExt : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetMembersExt";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetMembersExt() : base("GetMembersExt") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {
            string methodName = "Invoke";

            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("parameters specified for member search.") { ErrorCode = 3300 };
                }

                List<Member> members = null;

                string procName = GetFunctionParameter("StoreProcName");
                if (string.IsNullOrEmpty(procName))
                {
                    string msg = string.Format("No stored proc name specified for member search.");
                    _logger.Error(_className, methodName, msg);
                    throw new LWOperationInvocationException(msg) { ErrorCode = 3386 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

                string[] allowedTypes = { "string", "boolean", "date", "short", "long", "decimal" };
                
                #region Get Stored Proc Fields
                List<string> reqParms = new List<string>();
                foreach (string name in FunctionProviderParms)
                {
                    if (name != "MaxNumberOfResults" && name != "StoreProcName")
                    {
                        string tolower = GetFunctionParameter(name).ToLower();
                        bool isAllowed = (from x in allowedTypes where x == tolower select 1).Count() > 0;
                        if (!isAllowed)
                        {
                            string msg = string.Format("Datatype {0} not allowed for member search.", name);
                            _logger.Error(_className, methodName, msg);
                            throw new LWOperationInvocationException(msg) { ErrorCode = 3388 };
                        }
                        reqParms.Add(name);
                    }
                }
                #endregion
                
                List<LWDatabaseFieldType> procParms = new List<LWDatabaseFieldType>();
                APIStruct[] parmList = null;
                if (args.ContainsKey("MemberSearchParms"))
                {
                    parmList = (APIStruct[])args["MemberSearchParms"];
                    foreach (APIStruct parm in parmList)
                    {
                        string parmName = (string)parm.Parms["ParmName"];
                        if ( string.IsNullOrEmpty(GetFunctionParameter(parmName) ))
                        {
                            string msg = string.Format("Invalid parameter {0} specified for member search.", parmName);
                            _logger.Error(_className, methodName, msg);
                            throw new LWOperationInvocationException(msg) { ErrorCode = 3387 };
                        }                        
                    }
                }

                foreach (string sParm in reqParms)
                {
                    LWDatabaseFieldType dbField = new LWDatabaseFieldType();
                    dbField.Name = sParm;
                    string typeStr = GetFunctionParameter(sParm);
                    dbField.Type = (DataType)Enum.Parse(typeof(DataType), typeStr);
                    // is there a value for it in the incoming parms
					if (parmList != null)
					{
						foreach (APIStruct parm in parmList)
						{
							string parmName = (string)parm.Parms["ParmName"];
							if (parmName.ToLower() == sParm.ToLower())
							{
								// yes - there is
								dbField.Value = (string)parm.Parms["ParmValue"];
								break;
							}
						}
					}
                    procParms.Add(dbField);
                }
                
                List<long> ipCodes = new List<long>();
                LWQueryUtil queryUtil = new LWQueryUtil();
                using (LWDataReader reader = queryUtil.ExecuteStoredProc(procName, procParms, true))
                {
                    if (reader != null)
                    {
                        while (reader.Next())
                        {
                            long ic = System.Convert.ToInt64(reader.GetData("ipcode"));
                            ipCodes.Add(ic);
                            _logger.Debug(_className, methodName, string.Format("Adding IPCODE {0} to search result", ic));
                        }
                    }
                    else
                    {
                        _logger.Trace(_className, methodName, "No result returned by stored procedure for member search.");
                    }
                }
                long[] ipCodeList = null;
                if (ipCodes.Count > 0)
                {
                    ipCodeList = new long[ipCodes.Count];
                    ipCodes.CopyTo(ipCodeList, 0);
                }
                else
                {
                    string msg = string.Format("No members found.");
                    _logger.Error(_className, methodName, msg);
                    throw new LWOperationInvocationException(msg) { ErrorCode = 3323 };
                }

                // check for vague criteria
                string maxResultsStr = GetFunctionParameter("MaxNumberOfResults");
                if (!string.IsNullOrEmpty(maxResultsStr))
                {
                    int maxResults = int.Parse(maxResultsStr);
                    if (ipCodeList.Length > maxResults)
                    {
                        string msg = string.Format("Member search criteria returned {0} members and is too vague.", ipCodeList.Length);
                        _logger.Error(_className, methodName, msg);
                        throw new LWOperationInvocationException(msg) { ErrorCode = 3389 };
                    }
                }

                _logger.Debug(_className, methodName, string.Format("Member search returned {0} members", ipCodeList.Length));

                int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
                int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;

                //LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

                //int ipcsize = batchInfo != null ? batchInfo.BatchSize : ipCodeList.Length;
                //int idx = batchInfo != null ? batchInfo.StartIndex : 0;

                //if (ipcsize > ipCodeList.Length)
                //{
                //    ipcsize = ipCodeList.Length;
                //}
                //if (idx > ipCodeList.Length)
                //{
                //    string msg = string.Format("Starting index is past the end.");
                //    _logger.Error(_className, methodName, msg);
                //    throw new LWOperationInvocationException(msg) { ErrorCode = 3390 };
                //}

                //if (ipcsize + idx > ipCodeList.Length)
                //{
                //    string msg = string.Format("Starting index is past the end.");
                //    _logger.Error(_className, methodName, msg);
                //    throw new LWOperationInvocationException(msg) { ErrorCode = 3390 };
                //}

                //long[] ipcodes = new long[ipcsize];
                //Array.Copy(ipCodeList, idx, ipcodes, 0, ipcsize);

                long[] ipcodes = LWQueryBatchInfo.GetIds(ipCodeList, startIndex, batchSize, Config.EnforceValidBatch);

                members = LoyaltyDataService.GetAllMembers(ipcodes, true);

                if (members == null || members.Count == 0)
                {
                    throw new LWOperationInvocationException(string.Format("No members found.")) { ErrorCode = 3323 };
                }
                else
                {
                    APIArguments responseArgs = new APIArguments();
                    responseArgs.Add("Count", (int)ipCodeList.Length);
                    responseArgs.Add("member", members);                    
                    response = SerializationUtils.SerializeResult(Name, Config, responseArgs);
                }
                return response;
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
