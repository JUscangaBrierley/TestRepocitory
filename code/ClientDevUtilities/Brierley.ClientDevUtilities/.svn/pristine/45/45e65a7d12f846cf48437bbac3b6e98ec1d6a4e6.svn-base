using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberNextBestActionDao : DaoBase<MemberNextBestAction>
	{
		public MemberNextBestActionDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public MemberNextBestAction Retrieve(long id)
		{
			return GetEntity(id);
		}

		public IEnumerable<MemberNextBestAction> Retrieve(long memberId, NextBestActionType actionType)
		{
			return Database.Fetch<MemberNextBestAction>("select * from LW_MemberNextBestAction where MemberId = @0 and ActionType = @1", memberId, actionType);
		}

		public IEnumerable<MemberNextBestAction> Retrieve(long memberId, NextBestActionType actionType, long actionId)
		{
			return Database.Fetch<MemberNextBestAction>("select * from LW_MemberNextBestAction where MemberId = @0 and ActionType = @1 and ActionId = 22", memberId, actionType, actionId);
		}
	}
}
