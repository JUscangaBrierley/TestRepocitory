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
	public class SmsQueueDao : DaoBase<SmsQueue>
	{
		public SmsQueueDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public SmsQueue Retrieve(long ID)
		{
			return GetEntity(ID);
		}

		public List<SmsQueue> Retrieve(CommunicationType messageType, SmsFailureType failureType)
		{
			return Database.Fetch<SmsQueue>("select * from LW_SmsQueue where MessageType = @0 and SmsFailureType = @1", messageType, failureType);
		}

		public List<SmsQueue> RetrieveBySmsID(long smsId)
		{
			return Database.Fetch<SmsQueue>("select * from LW_SmsQueue where SmsId = @0", smsId);
		}

		public List<SmsQueue> RetrieveBySmsIDAndFailureType(long smsId, SmsFailureType smsFailureType)
		{
			return Database.Fetch<SmsQueue>("select * from LW_SmsQueue where SmsId = @0 and SmsFailureType = @1", smsId, smsFailureType);
		}

		public List<SmsQueueSummaryItem> RetrieveSummary()
		{
			var summaryRows = Database.Fetch<SmsQueueSummaryItem>(@"select t.SmsID, t.MessageType, t.SmsFailureType, count(t.ID) as SendCount 
            from LW_SmsQueue t where t.SmsFailureType <> @0 group by t.SmsID, t.MessageType, t.SmsFailureType", SmsFailureType.SentSuccessfully);
			foreach (var id in summaryRows.Select(o => o.SmsID).Distinct())
			{
				var nameList = Database.Fetch<string>("select name from LW_SMS where ExternalId = @0", id);
				string names = string.Join(", ", nameList);
				foreach (var row in summaryRows.Where(o => o.SmsID == id))
				{
					row.Name = names;
				}
			}
			return summaryRows;
		}

		public void Delete(long ID)
		{
			DeleteEntity(ID);
		}
	}
}
