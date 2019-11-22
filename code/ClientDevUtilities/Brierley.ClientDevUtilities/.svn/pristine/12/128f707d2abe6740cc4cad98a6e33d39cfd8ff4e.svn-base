using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class IdGeneratorDao : DaoBase<IDGenerator>
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private const string _className = "IDGeneratorDAO";

		public IdGeneratorDao(Database database, ServiceConfig config) : base(database, config)
		{
		}

		public void CreateIDGenerator(string objectName)
		{
			IDGenerator gen = new IDGenerator();
			gen.ObjectName = objectName;
			gen.IncrValue = 1;
			gen.PrevValue = 0;
			gen.SeedValue = 1;
			SaveEntity(gen);
		}

		/// <summary>
		/// This method uses IsolationLevel instead of a row level lock.
		/// </summary>
		/// <param name="objectName"></param>
		/// <param name="howMany"></param>
		/// <returns></returns>
		public Dictionary<String, IDGenStats> ReplenishIDs(string objectName, int howMany, int bucketSize, Dictionary<String, IDGenStats> idgenBuckets)
		{
            using (var db = Config.CreateDatabase())
            using (var txn = db.GetTransaction())
            {
                Database oldDb = Database;
                Database = db;
                try
                {
                    IDGenerator idgen = null;
                    if (db._dbType is PetaPoco.DatabaseTypes.OracleDatabaseType)
                    {
                        idgen = db.FirstOrDefault<IDGenerator>("select * from LW_IDGenerator where ObjectName = @0 for update", objectName);
                    }
                    else
                    {
                        idgen = db.FirstOrDefault<IDGenerator>("select * from LW_IDGenerator with (updlock) where ObjectName = @0", objectName);
                    }
                    if (idgen == null)
                    {
                        throw new LWDataServiceException(string.Format("ID Object '{0}' does not exist.", objectName));
                    }
                    // throw away any unused IDs and start a new bucket
                    idgenBuckets[objectName].CurrentId = idgen.PrevValue + idgen.IncrValue;
                    idgenBuckets[objectName].LastId = idgenBuckets[objectName].CurrentId + Math.Max(bucketSize, howMany) - 1;
                    idgen.PrevValue = idgenBuckets[objectName].LastId;
                    UpdateEntity(idgen);
                    txn.Complete();
                }
                finally
                {
                    Database = oldDb; // Make sure we always put the old database connection back, even if something goes horribly wrong
                }
            }
			return idgenBuckets;
		}

		public long GetNextSequence()
		{
			//todo: this never worked against SQL server because there are no sequences, but works fine against Oracle. Testing the original version 
			//		against SQL server executed the same seq_idgenerator.nextval query, which resulted in an exception. The only known caller to this
			//		method is LWQueryUtil.InsertRecord(), which checks that the database type is Oracle before calling, but we'll make sure here as
			//		well and throw a more meaningful error.
			//long nextSeq = Session.GetNamedQuery("GetSequence").UniqueResult<Int64>();
			//return nextSeq;
			if (!(Database._dbType is PetaPoco.DatabaseTypes.OracleDatabaseType))
			{
				throw new InvalidOperationException("Method GetNextSequence can only be called against Oracle databases.");
			}
			return Database.ExecuteScalar<long>("select SEQ_IDGENERATOR.NextVal from dual");
		}
	}
}
