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
    public class ContactStatusDao : DaoBase<ContactStatus>
    {
        public ContactStatusDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public ContactStatus Retrieve(long ID)
        {
            if (ID < 0) throw new ArgumentException("ID " + ID + " is invalid");

            return GetEntity(ID);
        }

        public List<ContactStatus> RetrieveAll()
        {
            return Database.Fetch<ContactStatus>("select * from LW_ContactStatus order by Contact_Status_Key asc");
        }

        public void Delete(long ID)
        {
            if (ID < 0) throw new ArgumentException("ID " + ID + " is invalid");

            DeleteEntity(ID);
        }
    }
}
