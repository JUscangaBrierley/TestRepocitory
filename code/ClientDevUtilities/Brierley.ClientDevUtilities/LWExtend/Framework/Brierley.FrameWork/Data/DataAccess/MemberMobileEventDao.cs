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
    public class MemberMobileEventDao : DaoBase<MemberMobileEvent>
    {
        public MemberMobileEventDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public long HowManyEvents(long memberId, MemberMobileEventActionType action, DateTime? startDate, DateTime? endDate)
        {
            string queryStr = "select count(*) from LW_MemberMobileEvent where MemberId = @0";
            DateTime d1 = DateTime.Now;
            DateTime d2 = DateTime.Now;
            if (startDate != null)
            {
                queryStr = string.Format("{0} and CreateDate >= @1", queryStr);
                d1 = DateTimeUtil.GetBeginningOfDay(startDate.Value);
            }
            if (endDate != null)
            {
                queryStr = string.Format("{0} and CreateDate <= @2", queryStr);
                d2 = DateTimeUtil.GetEndOfDay(endDate.Value);
            }
            return Database.ExecuteScalar<long>(queryStr, memberId, d1, d2);
        }

        public List<MemberMobileEvent> Retrieve(long memberId, MemberMobileEventActionType action, DateTime? startDate, DateTime? endDate)
        {
            string queryStr = "select * from LW_MemberMobileEvent where MemberId = @0";
            DateTime d1 = DateTime.Now;
            DateTime d2 = DateTime.Now;
            if (startDate != null)
            {
                queryStr = string.Format("{0} and CreateDate >= @1", queryStr);
                d1 = DateTimeUtil.GetBeginningOfDay(startDate.Value);
            }
            if (endDate != null)
            {
                queryStr = string.Format("{0} and CreateDate <= @2", queryStr);
                d2 = DateTimeUtil.GetEndOfDay(endDate.Value);
            }
            return Database.Fetch<MemberMobileEvent>(queryStr, memberId, d1, d2);
        }
    }
}
