using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class AtsLockDao : DaoBase<ATSLock>
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private const string _className = "ATSLockDao";

		public AtsLockDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public ATSLock ObtainLock(string objectName)
		{
			string methodName = "ObtainLock";
			ATSLock lockObj = Retrieve(objectName);
			if (lockObj == null)
			{
				lockObj = new ATSLock() { ObjectName = objectName, TimesLocked = 0 };
				SaveEntity(lockObj);
				lockObj = Retrieve(objectName);
			}
			_logger.Debug(_className, methodName, "Obtained lock for " + objectName);
			return lockObj;
		}

		public void UpdateLock(ATSLock lockObj)
		{
			string methodName = "UpdateLock";
			lockObj.TimesLocked++;
			UpdateEntity(lockObj);
			_logger.Debug(_className, methodName, "Updated lock for " + lockObj.ObjectName);
		}

		public ATSLock Retrieve(string objectName)
		{
			string sql = string.Format("{0}where ObjectName = @0{1}", WithUpdateClause(LockingMode.Upgrade), ForUpdateClause(LockingMode.Upgrade));
			return Database.FirstOrDefault<ATSLock>(sql, objectName);
		}
	}
}
