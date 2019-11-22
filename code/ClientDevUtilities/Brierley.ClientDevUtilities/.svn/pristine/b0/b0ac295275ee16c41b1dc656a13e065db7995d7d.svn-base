using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class TestListDao : DaoBase<TestList>
	{
		public TestListDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public TestList Retrieve(long id)
		{
			return GetEntity(id);
		}

		public TestList Retrieve(string name)
		{
			return Database.FirstOrDefault<TestList>("select * from LW_TestList where name = @0", name);
		}

		public List<TestList> RetrieveAll()
		{
			return Database.Fetch<TestList>("select * from LW_TestList");
		}

		public List<TestList> RetrieveAll(DateTime changedSince)
		{
			return Database.Fetch<TestList>("select * from LW_TestList where UpdateDate >= @0", changedSince);
		}

		public List<TestList> RetrieveAllByTemplate(long templateId)
		{
			return Database.Fetch<TestList>("select * from LW_TestList where TemplateId = @0", templateId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
