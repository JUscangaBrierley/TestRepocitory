using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class SkinItemDao : DaoBase<SkinItem>
	{
		public SkinItemDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public SkinItem Retrieve(long id)
		{
			return Database.FirstOrDefault<SkinItem>("select * from LW_SkinItem where id = @0", id);
		}

		public SkinItem Retrieve(long skinID, SkinItemTypeEnum skinItemType, string fileName)
		{
			return Database.FirstOrDefault<SkinItem>(
				"select * from LW_SkinItem where SkinId = @0 and SkinItemType = @1 and FileName = @2",
				skinID, skinItemType.ToString(), fileName);
		}

		public List<SkinItem> RetrieveAll(long skinID)
		{
			return Database.Fetch<SkinItem>(
				"select * from LW_SkinItem where SkinId = @0 order by SkinItemType, FileName", 
				skinID);
		}

		public List<SkinItem> RetrieveAll(long skinID, SkinItemTypeEnum skinItemType)
		{
			return Database.Fetch<SkinItem>(
				"select * from LW_SkinItem where SkinId = @0 and SkinItemType = @1 order by FileName", 
				skinID, 
				skinItemType.ToString());
		}

		public List<SkinItem> RetrieveAll(long skinID, DateTime changedSince)
		{
			return Database.Fetch<SkinItem>(
				"select * from LW_SkinItem where SkinId = @0 and UpdateDate >= @1 order by SkinItemType, FileName", 
				skinID, 
				changedSince);
		}

		public List<SkinItem> RetrieveAll(long skinID, SkinItemTypeEnum skinItemType, DateTime changedSince)
		{
			return Database.Fetch<SkinItem>(
				"select * from LW_SkinItem where SkinId = @0 and SkinItemType = @1 and UpdateDate >= @2 order by FileName", 
				skinID, 
				skinItemType.ToString(), 
				changedSince);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
