////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Data;

////using NHibernate;
////using NHibernate.Connection;

////using Brierley.FrameWork.Common;
////using Brierley.FrameWork.Common.Logging;

////namespace Brierley.FrameWork.Data.Sql
////{
////    public class LWConnectionProvider : DriverConnectionProvider
////    {
////        #region Fields
////        private const string _className = "LWConnectionProvider";
////        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

////        /// <summary>
////        /// Maximum number of retries
////        /// </summary>
////        private const int MAX_RETRIES = 10;
////        /// <summary>
////        /// Time to wait in between retries - default 3 sec.
////        /// </summary>
////        private const int WAIT_BETWEEN_RETRIES = 3000;
////        #endregion

////        #region Construction/Initialization
////        public LWConnectionProvider()
////        {            
////        }
////        #endregion

////        #region Overrides
////        public override IDbConnection GetConnection()
////        {
////            string methodName = "GetConnection";

////            IDbConnection conn = null;
////            int nTries = 0;
////            while (nTries < MAX_RETRIES)
////            {
////                try
////                {
////                    IDbConnection connection = Driver.CreateConnection();
////                    connection.ConnectionString = ConnectionString;
////                    connection.Open();
////                    conn = connection;
////                    return conn;
////                }
////                catch (Exception ex)
////                {
////                    nTries++;
////                    if (nTries >= MAX_RETRIES)
////                    {
////                        if (conn != null)
////                            conn.Dispose();
////                        _logger.Error(_className, methodName, string.Format("Unable to establish connection after {0} attempts.", nTries));
////                        throw;
////                    }
////                    else
////                    {
////                        _logger.Trace(_className, methodName, string.Format("Connection failed with error: {0}.  Retrying connection.", ex.Message));
////                        System.Threading.Thread.Sleep(WAIT_BETWEEN_RETRIES);
////                    }
////                }
////            }
////            // this should really be never reached.
////            throw new HibernateException(string.Format("Unable to establish connection after {0} attempts.", nTries));
////        }

////        public override void CloseConnection(IDbConnection conn)
////        {
////            base.CloseConnection(conn);
////        }
////        #endregion
////    }
////}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using System.Threading;

//using NHibernate.Connection;

//using Brierley.FrameWork.Common;
//using Brierley.FrameWork.Common.Logging;

//namespace Brierley.FrameWork.Data.Sql
//{
//	public class LWConnectionProvider : DriverConnectionProvider
//	{
//		public const String ConnectionDelayBetweenTries = "connection.delay_between_tries";
//		public const String ConnectionMaxTries = "connection.max_tries";

//		private static readonly LWLogger log = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        
//		public LWConnectionProvider()
//		{
//			this.MaxTries = 3;
//			this.DelayBetweenTries = TimeSpan.FromSeconds(5);
//		}

//		public Int32 MaxTries { get; set; }

//		public TimeSpan DelayBetweenTries { get; set; }

//		public override void Configure(IDictionary<String, String> settings)
//		{
//			String maxTries;
//			String delayBetweenTries;

//			if (settings.TryGetValue(ConnectionMaxTries, out maxTries) == true)
//			{
//				this.MaxTries = Int32.Parse(maxTries);
//			}

//			if (settings.TryGetValue(ConnectionDelayBetweenTries, out delayBetweenTries) == true)
//			{
//				this.DelayBetweenTries = TimeSpan.Parse(delayBetweenTries);
//			}

//			base.Configure(settings);
//		}

//		public override IDbConnection GetConnection()
//		{
//			IDbConnection con = null;

//			for (var i = 0; i < this.MaxTries; ++i)
//			{
//				try
//				{
//					log.Debug(String.Format("Attempting to get connection, {0} of {1}", (i + 1), this.MaxTries));
//					con = base.GetConnection();
//					log.Debug(String.Format("Got a connection after {0} tries", (i + 1)));

//					break;
//				}
//				catch (Exception ex)
//				{
//					if (i == this.MaxTries - 1)
//					{
//						log.Error(String.Format("Could not get connection after {0} tries", this.MaxTries), ex);
//						throw;
//					}
//					else
//					{
//						Thread.Sleep(this.DelayBetweenTries);
//					}
//				}
//			}

//			return (con);
//		}
//	}
//}

