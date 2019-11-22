using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class AttributeSetMetaDataDao : DaoBase<AttributeSetMetaData>
	{
		public AttributeMetaDataDao AttributeMetaDataDao { get; set; }

		public AttributeSetMetaDataDao(Database database, ServiceConfig config) : base(database, config)
		{
		}

		//todo: we're shadowing our base methods because business logic was placed in the DAO class. Move the null checks up at least to data service.
		public new void Create(AttributeSetMetaData attSet)
		{
			if (string.IsNullOrEmpty(attSet.DisplayText))
			{
				attSet.DisplayText = attSet.Name;
			}
			SaveEntity(attSet);
		}

		public new void Update(AttributeSetMetaData attSet)
		{
			if (string.IsNullOrEmpty(attSet.DisplayText))
			{
				attSet.DisplayText = attSet.Name;
			}
			UpdateEntity(attSet);
		}

		private AttributeSetMetaData Populate(AttributeSetMetaData attSet)
		{
			List<AttributeMetaData> atts = AttributeMetaDataDao.RetrieveByAttributeSetCode(attSet.ID) ?? new List<AttributeMetaData>();
			attSet.Attributes = atts;
			List<AttributeSetMetaData> children = RetrieveChildAttSets(attSet.ID);
			if (children != null)
			{
				attSet.ChildAttributeSets = children;
			}
			return attSet;
		}

		public AttributeSetMetaData Retrieve(long attSetCode)
		{
			var attSet = Database.FirstOrDefault<AttributeSetMetaData>("select * from LW_Attributeset where AttributeSetCode = @0", attSetCode);
			if (attSet != null)
			{
				return Populate(attSet);
			}
			return null;
		}

		public AttributeSetMetaData Retrieve(string attSetName)
		{
			if (!string.IsNullOrEmpty(attSetName))
			{
				attSetName = attSetName.ToLower();
			}
			var set = Database.FirstOrDefault<AttributeSetMetaData>("select * from LW_Attributeset where lower(AttributeSetName) = @0", attSetName);
			if (set != null)
			{
				return Populate(set);
			}
			return set;
		}
		
		public bool Exists(string attSetName)
		{
			if (!string.IsNullOrEmpty(attSetName))
			{
				attSetName = attSetName.ToLower();
			}
			return Database.Exists<AttributeSetMetaData>("lower(AttributeSetName) = @0", attSetName);
		}

		public List<AttributeSetMetaData> RetrieveChildAttSets(long parentId)
		{
			var set = Database.Fetch<AttributeSetMetaData>("select * from LW_AttributeSet where ParentAttributeSetCode = @0", parentId);
			if (set != null && set.Count > 0)
			{
				foreach (AttributeSetMetaData attSet in set)
				{
					Populate(attSet);
				}
			}
			return set;
		}

		/// <summary>
		/// This method will retrieve all top level attributes (Member, Virtualcard and Global attributes).
		/// </summary>
		/// <returns></returns>
		public List<AttributeSetMetaData> RetrieveTopLevel()
		{
			long pid = -1;
			return RetrieveChildAttSets(pid);
		}

		/// <summary>
		/// Depending on the provided type, this method will either retrieve only Member or Virtualcard or
		/// Global attributes.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public List<AttributeSetMetaData> RetrieveTopLevel(AttributeSetType type)
		{
			var set = Database.Fetch<AttributeSetMetaData>("select * from LW_AttributeSet where ParentAttributeSetCode = -1 and TypeCode = @0", type);
			if (set != null && set.Count > 0)
			{
				foreach (AttributeSetMetaData attSet in set)
				{
					Populate(attSet);
				}
			}
			return set;
		}

		public List<AttributeSetMetaData> RetrieveChangedObjects(DateTime since)
		{
			var set = Database.Fetch<AttributeSetMetaData>("select * from LW_AttributeSet where UpdateDate >= @0", since);
			if (set != null && set.Count > 0)
			{
				foreach (AttributeSetMetaData attSet in set)
				{
					Populate(attSet);
				}
			}
			return set;
		}

		public List<AttributeSetMetaData> RetrieveByIds(long[] ids)
		{
			if (ids.Length == 0)
			{
				return new List<AttributeSetMetaData>();
			}
            var sets = RetrieveByArray<long>("select * from LW_AttributeSet where AttributeSetCode in (@0)", ids);
            if (sets != null && sets.Count > 0)
			{
				foreach (AttributeSetMetaData set in sets)
				{
					Populate(set);
				}
			}
			return sets;
		}

		public List<AttributeSetMetaData> RetrieveAll()
		{
			var sets = Database.Fetch<AttributeSetMetaData>("select * from LW_AttributeSet");
			if (sets != null && sets.Count > 0)
			{
				foreach (AttributeSetMetaData set in sets)
				{
					Populate(set);
				}
			}
			return sets;
		}


		public List<string> RetrieveAllNames()
		{
			return Database.Fetch<string>("select AttributeSetName from LW_AttributeSet");
		}

		public void Delete(long attSetCode)
		{
			var entity = Retrieve(attSetCode);
			if (entity != null)
			{
				DeleteEntity(entity);
			}
		}
	}
}
