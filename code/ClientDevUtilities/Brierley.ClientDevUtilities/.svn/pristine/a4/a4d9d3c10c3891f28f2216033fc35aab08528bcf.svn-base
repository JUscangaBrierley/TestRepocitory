using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class AttributeMetaDataDao: DaoBase<AttributeMetaData>
	{
		public AttributeMetaDataDao(Database database, ServiceConfig config) : base(database, config)
		{
		}

		public AttributeMetaData Retrieve(long attCode)
		{
			return Database.FirstOrDefault<AttributeMetaData>("select * from LW_Attribute where AttributeCode = @0", attCode);
		}

		public List<AttributeMetaData> RetrieveByAttributeSetCode(long attSetCode)
		{
			return Database.Fetch<AttributeMetaData>("select * from LW_Attribute where AttributeSetCode = @0 order by AttributeCode", attSetCode);
		}

		public void DeleteByAttributeSetCode(long attSetCode)
		{
			Database.Execute("delete from LW_Attribute where AttributeSetCode = @0", attSetCode);
		}

		public void Delete(long attCode)
		{
			var entity = Retrieve(attCode);
			if (entity != null)
			{
				DeleteEntity(entity);
			}
		}
	}
}
