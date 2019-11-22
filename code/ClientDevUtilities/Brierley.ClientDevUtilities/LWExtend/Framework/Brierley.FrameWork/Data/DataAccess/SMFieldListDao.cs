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
    public class SMFieldListDao : DaoBase<SMFieldList>
    {
        public SMFieldListDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public SMFieldList Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        public SMFieldList Retrieve(string fieldListName)
        {
            return Database.FirstOrDefault<SMFieldList>("select * from LW_SM_FieldList where FieldListName = @0", fieldListName);
        }

        public List<SMFieldList> RetrieveAll()
        {
            return Database.Fetch<SMFieldList>("select * from LW_SM_FieldList");
        }

        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }
    }
}
