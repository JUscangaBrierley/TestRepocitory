using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.LWGateway.DataAccess
{
    public interface IContactHistoryDao : IDaoBase<ContactHistory>
    {
        void Delete(long ID);
        ContactHistory Retrieve(long ID);
        List<ContactHistory> RetrieveAll(long IPCode);
        List<ContactHistory> RetrieveAllInDateRange(long IPCode, DateTime fromDate, DateTime toDate);
        ContactHistory RetrieveByCDWKey(long cdwKey);
    }
}
