using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class StructuredContentAttributeDao : DaoBase<StructuredContentAttribute>
	{
		public StructuredContentAttributeDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		protected override void SaveEntity(object entity)
		{
			var attribute = (StructuredContentAttribute)entity;
			if (attribute.IsGlobal)
			{
				// global attributes can't be filters
				attribute.IsFilter = false;
				attribute.FilterOrder = -1;
			}
			base.SaveEntity(entity);
		}

		protected override void UpdateEntity(object entity)
		{
			var attribute = (StructuredContentAttribute)entity;
			if (attribute.IsGlobal)
			{
				// global attributes can't be filters
				attribute.IsFilter = false;
				attribute.FilterOrder = -1;
			}
			base.UpdateEntity(entity);
		}

		public StructuredContentAttribute Retrieve(long id)
		{
			return GetEntity(id);
		}

		public StructuredContentAttribute Retrieve(string name)
		{
			return Database.FirstOrDefault<StructuredContentAttribute>("select * from LW_StructuredContentAttribute where name = @0", name);
		}

		public StructuredContentAttribute RetrieveGlobal(long id)
		{
			return Database.FirstOrDefault<StructuredContentAttribute>("select * from LW_StructuredContentAttribute where Id = @0 and IsGlobal = 1 order by Id", id);
		}

		public StructuredContentAttribute RetrieveGlobal(string name)
		{
			return Database.FirstOrDefault<StructuredContentAttribute>("select * from LW_StructuredContentAttribute where Name = @0 and IsGlobal = 1 order by Id", name);
		}

		public StructuredContentAttribute RetrieveByNameAndElementId(string name, long elementId)
		{
			return Database.FirstOrDefault<StructuredContentAttribute>(
				"select * from LW_StructuredContentAttribute where Name = @0 and ElementId = @1 order by Id", 
				name, 
				elementId);
		}

		public List<StructuredContentAttribute> RetrieveByElementId(long elementId)
		{
			return Database.Fetch<StructuredContentAttribute>(
				"select * from LW_StructuredContentAttribute where ElementId = @1 and IsGlobal = 0 order by Id",
				elementId,
				elementId);
		}

		public List<StructuredContentAttribute> RetrieveByElementIds(long[] elementIds)
		{
            if (elementIds == null || elementIds.Length == 0)
                return new List<StructuredContentAttribute>();
            return RetrieveByArray<long>("select * from LW_StructuredContentAttribute where ElementId in (@0) and IsGlobal = 0 order by Id", elementIds);
		}

		public List<StructuredContentAttribute> RetrieveAll()
		{
			return Database.Fetch<StructuredContentAttribute>("select * from LW_StructuredContentAttribute order by Id");
		}

		public List<StructuredContentAttribute> RetrieveAll(DateTime changedSince)
		{
			return Database.Fetch<StructuredContentAttribute>("select * from LW_StructuredContentAttribute where UpdateDate >= @0 order by Id", changedSince);
		}

		public List<StructuredContentAttribute> RetrieveAllGlobals()
		{
			return Database.Fetch<StructuredContentAttribute>("select * from LW_StructuredContentAttribute where IsGlobal = 1 order by Id");
		}

		public List<StructuredContentAttribute> RetrieveAllGlobalFilters()
		{
			return new List<StructuredContentAttribute>();
		}

		public List<StructuredContentAttribute> RetrieveAllFiltersForElement(long elementId)
		{
			return Database.Fetch<StructuredContentAttribute>(
				"select * from LW_StructuredContentAttribute where ElementId = @0 and IsFilter = 1 and IsGlobal = 0 and FilterOrder > -1 order by FilterOrder", 
				elementId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
