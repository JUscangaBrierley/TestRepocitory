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
    public class ContactHistoryDao : DaoBase<ContactHistory>
    {
        public ContactHistoryDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public ContactHistory Retrieve(long ID)
        {
            if (ID < 0) throw new ArgumentException("ID " + ID + " is invalid");

            return GetEntity(ID);
        }

        public ContactHistory RetrieveByCDWKey(long cdwKey)
        {
            return Database.FirstOrDefault<ContactHistory>("select * from LW_ContactHistory where CDWKey = @0", cdwKey);
        }

        public List<ContactHistory> RetrieveAll(long IPCode)
        {
            return Database.Fetch<ContactHistory>("select * from LW_ContactHistory where IPCode = @0", IPCode);
        }

        public List<ContactHistory> RetrieveAllInDateRange(long IPCode, DateTime fromDate, DateTime toDate)
        {
            return Database.Fetch<ContactHistory>("select * from LW_ContactHistory where IPCode = @0 and ContactDate between @1 and @2", IPCode, fromDate, toDate);
        }

        public void Delete(long ID)
        {
            if (ID < 0) throw new ArgumentException("ID " + ID + " is invalid");

            DeleteEntity(ID);
        }
    }
}
