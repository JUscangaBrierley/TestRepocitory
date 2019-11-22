using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.Sql;
using Brierley.FrameWork.Common.Logging;

using Brierley.LWModules.MemberSearch;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Interfaces;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.LWModules.MemberSearch.Components
{
    public class MemberSearchUtil
    {
		#region Fields
		private const string _className = "MemberSearchUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private static IMemberSearchParmValidator _validator = null;
        private static bool validatorCreated = false;
		#endregion

        #region private methods
        private static void ValidateParameters(MemberSearchConfiguration config)
        {
            if (!validatorCreated)
            {
                validatorCreated = true;
                if (!string.IsNullOrEmpty(config.ParmValidatorClassName) && !string.IsNullOrEmpty(config.ParmValidatorAssemblyName))
                {
                    _validator = (IMemberSearchParmValidator)ClassLoaderUtil.CreateInstance(config.ParmValidatorAssemblyName, config.ParmValidatorClassName);
                }
            }
            if (_validator != null)
            {
                _validator.ValidateParameters(config.SearchFields);
            }
        }

        private static SupportedDataSourceType GetDataSourceType()
        {
            LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
            return config.DBConfig.DBType;
        }

        private static IDbDataParameter GetOracleProcParameter(IDbCommand cmd)
        {
            string enumType = "Oracle.DataAccess.Client.OracleDbType";
            string parmType = "Oracle.DataAccess.Client.OracleParameter";
            string oraAssembly = "Oracle.DataAccess";

            Type[] argTypes = new Type[3];
            argTypes[0] = typeof(string);
            argTypes[1] = Type.GetType(string.Format("{0}, {1}", enumType, oraAssembly));
            argTypes[2] = typeof(ParameterDirection);

            object[] args = new object[3];
            args[0] = "retval";
            args[1] = Enum.Parse(argTypes[1], "RefCursor");
            args[2] = ParameterDirection.Output;

            IDbDataParameter parm = (IDbDataParameter)Brierley.FrameWork.Common.ClassLoaderUtil.CreateInstance(oraAssembly, parmType, argTypes, args);
            return parm;
        }

        private static object GetParmvalue(DbType type, object value, string regex)
        {
            object ovalue = System.DBNull.Value;
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                string parmValue = value.ToString();
                if (!string.IsNullOrEmpty(regex))
                {
                    parmValue = System.Text.RegularExpressions.Regex.Replace(value.ToString(), regex, "");
                }
                switch (type)
                {
                    case DbType.Int32:
                        ovalue = System.Convert.ToInt64(parmValue);
                        break;
                    case DbType.Date:
                    case DbType.DateTime:
                        ovalue = DateTime.Parse(parmValue);
                        break;
                    case DbType.Decimal:
                    case DbType.Int64:
                        ovalue = System.Convert.ToDouble(parmValue);
                        break;
                    case DbType.String:
                        ovalue = parmValue;
                        break;
                    case DbType.Boolean:
                        if (value.GetType() == typeof(Boolean))
                        {
                            ovalue = (bool)value ? System.Convert.ToInt64("1") : System.Convert.ToInt64("0");
                        }
                        else
                        {
                            ovalue = int.Parse(parmValue);
                        }
                        break;
                }
            }
            return ovalue;
        }
        #endregion

        public static long[] SearchMembers(MemberSearchConfiguration searcgConfig)
        {
			string methodName = "SearchMembers";

            long[] ipCodeList = null;

			_logger.Trace(_className, methodName, string.Format("Executing {0} for searching members", searcgConfig.StoredProcedure));

            // first validate the parameters
			ValidateParameters(searcgConfig);

            List<LWDatabaseFieldType> procParms = new List<LWDatabaseFieldType>();
            foreach (MemberSearchConfiguration.SearchField field in searcgConfig.SearchFields)
            {
                LWDatabaseFieldType dbFields = new LWDatabaseFieldType()
                {
                    Name = field.FieldName,
                    Type = (DataType)Enum.Parse(typeof(DataType), field.FieldType),
                    Regex = field.Regex,
                    Value = field.FieldValue,
                };
                procParms.Add(dbFields);
            }

            IList<long> ipCodes = new List<long>();
            LWQueryUtil queryUtil = new LWQueryUtil();
            using (LWDataReader reader = queryUtil.ExecuteStoredProc(searcgConfig.StoredProcedure, procParms, true))
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
            if (ipCodes.Count > 0)
            {
                ipCodeList = new long[ipCodes.Count];
                ipCodes.CopyTo(ipCodeList, 0);
            }

            return ipCodeList;
        }
    }
}
