using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ValidatorTriggerDao : DaoBase<ValidatorTrigger>
	{
		public ValidatorTriggerDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public ValidatorTrigger Retrieve(long id)
		{
			return GetEntity(id);
		}

		public ValidatorTrigger Retrieve(long validatorId, long attCode)
		{
			return Database.FirstOrDefault<ValidatorTrigger>("select * from LW_ValidatorTriggers where ValidatorId = @0 and AttributeCode = @1", validatorId, attCode);
		}

		public List<ValidatorTrigger> RetrieveByAttribute(long attCode)
		{
			return Database.Fetch<ValidatorTrigger>("select * from LW_ValidatorTriggers where AttributeCode = @0", attCode);
		}

		public List<ValidatorTrigger> RetrieveByAttribute(long[] ids)
		{
			int idsRemaining = ids.Length;
			int startIdx = 0;
			var ret = new List<ValidatorTrigger>();
			while (idsRemaining > 0)
			{
				long[] vldIds = LimitInClauseList<long>(ids, ref startIdx, ref idsRemaining);
				var triggers = Database.Fetch<ValidatorTrigger>("select * from LW_ValidatorTriggers where AttributeCode in (@ids)", new { ids = vldIds });
				if (triggers != null && triggers.Count > 0)
				{
					ret.AddRange(triggers);
				}
			}
			return ret;
		}

		public List<ValidatorTrigger> RetrieveAll()
		{
			return Database.Fetch<ValidatorTrigger>("select * from LW_ValidatorTriggers");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
