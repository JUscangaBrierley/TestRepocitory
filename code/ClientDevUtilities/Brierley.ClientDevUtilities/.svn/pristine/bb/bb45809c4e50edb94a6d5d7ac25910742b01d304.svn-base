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
    public class CSFunctionDao : DaoBase<CSFunction>
    {
        public CSFunctionDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public CSFunction Retrieve(long funcID)
        {
            return GetEntity(funcID);
        }

        public List<CSFunction> RetrieveAll()
        {
            return Database.Fetch<CSFunction>("select * from LW_CSFunction");
        }

        public void Delete(long funcID)
        {
            DeleteEntity(funcID);
        }
    }
}
