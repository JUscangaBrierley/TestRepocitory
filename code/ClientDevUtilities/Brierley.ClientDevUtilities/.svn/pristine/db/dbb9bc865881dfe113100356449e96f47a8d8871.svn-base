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
    public class CSNoteDao : DaoBase<CSNote>
    {
        public CSNoteDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

        public CSNote Retrieve(long id)
        {
            return GetEntity(id);
        }

        public List<CSNote> RetrieveByMember(long ipcode, DateTime startDate, DateTime endDate)
        {
            return Database.Fetch<CSNote>("select * from LW_CSNote where MemberId = @0 and CreateDate between @1 and @2 order by CreateDate", ipcode, startDate, endDate);
        }

        public List<CSNote> RetrieveAll()
        {
            return Database.Fetch<CSNote>("select * from LW_CSNote");
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
        }
    }
}
