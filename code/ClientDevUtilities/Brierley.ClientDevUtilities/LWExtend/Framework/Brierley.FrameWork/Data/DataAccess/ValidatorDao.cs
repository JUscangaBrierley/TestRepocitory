using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ValidatorDao : DaoBase<Validator>
	{
		public ValidatorDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public Validator Retrieve(long id)
		{
			return GetEntity(id);
		}

		public Validator Retrieve(string name)
		{
			return Database.FirstOrDefault<Validator>("select * from LW_Validator where name = @0", name);
		}

		public List<Validator> Retrieve(long[] ids)
		{
			if (ids.Length == 0)
			{
				return new List<Validator>();
			}
            return RetrieveByArray<long>("select * from LW_Validator where ValidatorCode in (@0)", ids);
		}

		public List<Validator> RetrieveAll()
		{
			return Database.Fetch<Validator>("select * from LW_Validator");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public bool IsInUse(long validatorId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_ValidatorTriggers where ValidatorCode = @0", validatorId) > 0;
		}
	}
}
