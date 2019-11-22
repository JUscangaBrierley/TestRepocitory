using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class StructuredContentElementDao : DaoBase<StructuredContentElement>
	{
		public StructuredContentElementDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public StructuredContentElement Retrieve(long id)
		{
			return GetEntity(id);
		}

		public StructuredContentElement Retrieve(string name)
		{
			return Database.FirstOrDefault<StructuredContentElement>("select * from LW_StructuredContentElement where Name = @0", name);
		}

		// Implements GetElementsForBatch of StructuredContentData
		public List<StructuredContentElement> RetrieveBatch(long batchId)
		{
			return Database.Fetch<StructuredContentElement>(
				@"select distinct a.elementid from LW_StructuredContentAttribute a, LW_StructuredContentData d 
				where d.BatchId = @0 and d.AttributeId = a.id and a.ElementId <> -1 
				order by a.ElementId", 
				batchId);
		}

		public List<StructuredContentElement> RetrieveAll()
		{
			return Database.Fetch<StructuredContentElement>("select * from LW_StructuredContentElement order by Id desc");
		}

		public List<StructuredContentElement> RetrieveAll(DateTime changedSince)
		{
			return Database.Fetch<StructuredContentElement>("select * from LW_StructuredContentElement where UpdateDate >= @0", changedSince);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
