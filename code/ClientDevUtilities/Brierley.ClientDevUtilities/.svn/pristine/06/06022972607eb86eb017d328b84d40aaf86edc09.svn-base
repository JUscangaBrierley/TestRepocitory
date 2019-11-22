using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class RewardChoiceDao : DaoBase<RewardChoice>
	{
		public RewardChoiceDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		//retrieves the current reward choice for the member. When a history of choices has been established,
		//the current choice is always the last created/updated record
		public RewardChoice RetrieveCurrentChoice(long memberId)
		{
			return Database.FirstOrDefault<RewardChoice>("select * from LW_RewardChoice where memberId = @0 order by CreateDate desc", memberId);
		}

		//retrieves a member's full history of choices
		public PetaPoco.Page<RewardChoice> Retrieve(long memberId, long page, long resultsPerPage)
		{
			return Database.Page<RewardChoice>(page, resultsPerPage, "select c.* from LW_RewardChoice c order by CreateDate desc where c.memberId = @0", memberId);
		}
	}
}
