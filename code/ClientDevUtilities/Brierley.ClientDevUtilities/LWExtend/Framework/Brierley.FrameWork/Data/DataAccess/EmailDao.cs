using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class EmailDao : DaoBase<EmailDocument>
    {
        public EmailDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public EmailDocument Retrieve(long id)
        {
            return GetEntity(id);
        }

        public EmailDocument Retrieve(string name)
        {
            return Database.FirstOrDefault<EmailDocument>("select * from LW_Email where Name = @0", name);
        }

        public List<EmailDocument> RetrieveByFolder(long folderId)
        {
            return Database.Fetch<EmailDocument>("select * from LW_Email where FolderId = @0", folderId);
        }

		public List<EmailDocument> RetrieveByTemplate(long templateId)
		{
			return Database.Fetch<EmailDocument>("select email.* from LW_Email email inner join LW_Document doc on doc.Id = email.DocumentId  where doc.TemplateId = @0", templateId);
		}

        public List<EmailDocument> RetrieveAll()
        {
            return Database.Fetch<EmailDocument>("select * from LW_Email order by Id desc");
        }

        public List<EmailDocument> RetrieveAll(DateTime changedSince)
        {
            return Database.Fetch<EmailDocument>("select * from LW_Email where UpdateDate >= @0 order by Id desc", changedSince);
        }

        public void Delete(long id)
        {
            EmailDocument email = Retrieve(id);
            if (email != null)
            {
                DeleteEntity(id);
            }
        }

    }
}
