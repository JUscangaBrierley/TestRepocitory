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
    public class SmsDao : DaoBase<SmsDocument>
    {
        public SmsDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public SmsDocument Retrieve(long id)
        {
            return GetEntity(id);
        }

        public SmsDocument Retrieve(string name)
        {
            return Database.FirstOrDefault<SmsDocument>("select * from LW_Sms where Name = @0", name);
        }

        public List<SmsDocument> RetrieveByFolder(long folderId)
        {
            return Database.Fetch<SmsDocument>("select * from LW_Sms where FolderId = @0", folderId);
        }

        public List<SmsDocument> RetrieveAll()
        {
            return Database.Fetch<SmsDocument>("select * from LW_Sms order by Id desc");
        }

        public List<SmsDocument> RetrieveAll(DateTime changedSince)
        {
            return Database.Fetch<SmsDocument>("select * from LW_Sms where UpdateDate >= @0 order by Id desc", changedSince);
        }

        public void Delete(long id)
        {
            SmsDocument sms = Retrieve(id);
            if (sms != null)
            {
                DeleteEntity(id);
            }
        }

    }
}
