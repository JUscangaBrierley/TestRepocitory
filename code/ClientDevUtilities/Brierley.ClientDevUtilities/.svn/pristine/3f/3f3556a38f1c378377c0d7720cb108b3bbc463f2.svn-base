using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class EmailLinkDao : DaoBase<EmailLink>
	{
		public EmailLinkDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public EmailLink Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<EmailLink> RetrieveByEmailId(long emailID, bool excludeInactiveLinks)
		{
			if (excludeInactiveLinks)
			{
				return Database.Fetch<EmailLink>("select * from LW_EmailLink where emailId = @0 and IsActive = 1 order by LinkOrder", emailID);
			}
			else
			{
				return Database.Fetch<EmailLink>("select * from LW_EmailLink where emailId = @0 order by LinkOrder", emailID);
			}
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
