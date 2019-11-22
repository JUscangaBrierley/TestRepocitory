using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class FieldValueDao : DaoBase<FieldValue>
	{
		public FieldValueDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public FieldValue Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<FieldValue> RetrieveAllByFieldId(long fieldId)
		{
			return Database.Fetch<FieldValue>("select * from LW_CLTableFieldValue where FieldId = @0", fieldId);
		}
		
		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public void DeleteByFieldId(long fieldId)
		{
			Database.Execute("delete from LW_CLTableFieldValue where FieldId = @0", fieldId);
		}
	}
}
