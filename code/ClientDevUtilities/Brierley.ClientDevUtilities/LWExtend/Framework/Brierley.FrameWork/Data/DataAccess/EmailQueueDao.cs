using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class EmailQueueDao : DaoBase<EmailQueue>
    {
        public EmailQueueDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public EmailQueue Retrieve(long ID)
        {
            return GetEntity(ID);
        }

		public List<EmailQueue> Retrieve(CommunicationType messageType, EmailFailureType failureType)
		{
			return Database.Fetch<EmailQueue>("select * from LW_EmailQueue where MessageType = @0 and FailureType = @1", messageType, failureType);
		}

        public List<EmailQueue> RetrieveByEmailID(long emailID)
        {
            return Database.Fetch<EmailQueue>("select * from LW_EmailQueue where EmailID = @0", emailID);
        }

        public List<EmailQueue> RetrieveByEmailIDAndFailureType(long emailID, EmailFailureType emailFailureType)
        {
            return Database.Fetch<EmailQueue>("select * from LW_EmailQueue where EmailID = @0 and EmailFailureType = @1", emailID, emailFailureType);
        }

        public List<EmailQueueSummaryItem> RetrieveSummary()
        {            
            List<EmailQueueSummaryItem> summaryRows = Database.Fetch<EmailQueueSummaryItem>(@"select e.Name, t.EmailID, t.MessageType, t.EmailFailureType, count(t.ID) as SendCount 
            from LW_EmailQueue t left join LW_Email e on e.ExternalId = t.EmailId where t.EmailFailureType <> @0 group by t.EmailID, e.Name, t.MessageType, t.EmailFailureType", EmailFailureType.SentSuccessfully);

            if (summaryRows != null && summaryRows.Count > 0)
                for (int rowIndex = 0; rowIndex < summaryRows.Count; rowIndex++)
                    summaryRows[rowIndex].ID = rowIndex;
            
            return summaryRows;
        }

        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }
    }
}
