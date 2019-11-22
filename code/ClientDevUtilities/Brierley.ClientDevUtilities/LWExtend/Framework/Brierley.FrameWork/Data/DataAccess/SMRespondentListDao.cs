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
    public class SMRespondentListDao : DaoBase<SMRespondentList>
    {
        public SMRespondentListDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

        public SMRespondentList Retrieve(long ID)
        {
                return GetEntity(ID);
        }

        public SMRespondentList RetrieveByBatchID(string batchID)
        {
            return Database.FirstOrDefault<SMRespondentList>("select * from LW_SM_RespondentList where lower(BatchId) = @0", batchID);
        }

        public List<SMRespondentList> RetrieveAll()
        {
            return Database.Fetch<SMRespondentList>("select * from LW_SM_RespondentList");
        }

        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }

        public bool IsRespondentListStaged(string batchID)
        {
            string sql = "SELECT BatchID FROM LW_SM_RespListStage WHERE BatchID = @0";
            List<string> batchIDs = Database.Fetch<string>(sql, batchID);
            return batchIDs != null && batchIDs.Count > 0;
        }

        public int DeleteStagedRespondentList(string batchID)
        {
            string sql = "DELETE FROM LW_SM_RespListStage WHERE BatchID = @0";
            return Database.Execute(sql, batchID);
        }
    }
}
