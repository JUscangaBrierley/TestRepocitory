//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data.Common;

//using NHibernate;
//using NHibernate.Exceptions;

//using Brierley.FrameWork.Common;
//using Brierley.FrameWork.Common.Logging;
//using Brierley.FrameWork.Common.Exceptions;

//namespace Brierley.FrameWork.Data.Sql
//{
//	public class LWSqlExceptionConverter : ISQLExceptionConverter
//	{
//		#region Fields
//		private const string _className = "LWSqlExceptionConverter";
//		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);        
//		#endregion

//		#region Private Helpers
//		//protected bool IsInTransaction(ISession session)
//		//{
//		//    return (session.Transaction != null ? session.Transaction.IsActive : false);
//		//}

//		private Type GetOracleExceptionType(Exception inner)
//		{
//			Type oracleError = null;
//			try
//			{
//				System.Reflection.Assembly oraAssembly = ClassLoaderUtil.LoadAssemblyFromName("Oracle.ManagedDataAccess", false);
//				if (oraAssembly != null)
//				{
//					string typeName = string.Format("Oracle.ManagedDataAccess.Client.OracleException, {0}", oraAssembly.FullName);
//					oracleError = Type.GetType(typeName);
//					if (inner.GetType() != oracleError)
//					{
//						return null;
//					}
//				}
//			}
//			catch
//			{
//			}
//			return oracleError;
//		}

//		protected bool IsConnectionError(Exception ex)
//		{
//			bool connectError = false;
//			//Exception inner = ex.InnerException;
//			if (ex != null)
//			{
//				// first check to see if we are using Oracle.
//				Type oracleError = GetOracleExceptionType(ex);
//				if (oracleError != null)
//				{
//					// we are using oracle.
//					if (!string.IsNullOrEmpty(ex.Message) &&
//						ex.Message.Contains("ORA-03135") ||		// contact lost to server
//						ex.Message.Contains("ORA-03114") ||		// not connected to ORACLE
//						ex.Message.Contains("ORA-12571") ||		// TNS: packet writer failure
//						ex.Message.Contains("ORA-00028") ||		// session has been killed
//						ex.Message.Contains("ORA-01012")			// not logged on
//						)
//					{
//						connectError = true;
//					}

//				}
//				else
//				{
//					// Check to see if we are using SQLServer.
//				}
//			}
//			return connectError;
//		}

//		protected LWDbConnectException MakeConnectionError(Exception ex)
//		{
//			string methodName = "MakeConnectionError";

//			_logger.Critical(_className, methodName, "Critical connection error encountered.", ex);

//			LWDbConnectException e = null;
//			if (ex.InnerException != null)
//			{
//				e = new LWDbConnectException(ex.InnerException.Message);
//			}
//			else
//			{
//				e = new LWDbConnectException(ex.Message);
//			}
//			return e;
//		}
        
//		#endregion

//		public Exception Convert(AdoExceptionContextInfo adoExceptionContextInfo)
//		{
//			DbException ex = ADOExceptionHelper.ExtractDbException(adoExceptionContextInfo.SqlException);
//			if (ex == null || !IsConnectionError(ex))
//			{
//				return new GenericADOException(adoExceptionContextInfo.Message, adoExceptionContextInfo.SqlException, adoExceptionContextInfo.Sql);
//			}            
//			else
//			{                
//				return MakeConnectionError(ex);
//			}
//		}
//	}
//}
