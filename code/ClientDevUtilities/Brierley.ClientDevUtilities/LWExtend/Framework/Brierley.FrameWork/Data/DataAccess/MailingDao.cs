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
    public class MailingDao : DaoBase<Mailing>
    {
        public MailingDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public List<Mailing> Retrieve(long emailID)
        {
            return Database.Fetch<Mailing>("select * from LW_Mailing where EmailID = @0", emailID);
        }

        public Mailing RetrieveByMailingId(long mailingID)
        {
            return GetEntity(mailingID);
        }

        public void Delete(long mailingID)
        {
            DeleteEntity(mailingID);
        }
    }
}
