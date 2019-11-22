using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using log4net.Appender;
using System;

namespace Brierley.FrameWork.Common.Logging
{
    public class LWDatabaseAppender : AdoNetAppender
    {
        private ServiceConfig _config = null;

        public LWDatabaseAppender()
        {
            _config = LWDataServiceUtil.GetServiceConfiguration();
        }

        protected override string ResolveConnectionString(out string connectionStringContext)
        {
            connectionStringContext = "FrameworkConnectionString";
            return _config.ConnectString;
        }

        protected override Type ResolveConnectionType()
        {
            if (_config.DatabaseType == SupportedDataSourceType.Oracle10g)
            {
                return typeof(Oracle.ManagedDataAccess.Client.OracleConnection);
            }
            if (_config.DatabaseType == SupportedDataSourceType.MsSQL2005)
            {
                return typeof(System.Data.SqlClient.SqlConnection);
            }
            throw new LWException(string.Format("Cannot resolve connection type. The provider type ({0}) is unsupported", _config.DatabaseType.ToString()));
        }
    }
}
