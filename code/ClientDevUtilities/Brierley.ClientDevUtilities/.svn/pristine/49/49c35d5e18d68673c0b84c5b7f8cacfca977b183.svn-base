using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class TableFieldDao : DaoBase<TableField>
	{
		public TableFieldDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public TableField Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<TableField> RetrieveByTable(long tableId)
		{
			return Database.Fetch<TableField>("select * from LW_CLTableField where TableId = @0", tableId);
		}
		
		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
