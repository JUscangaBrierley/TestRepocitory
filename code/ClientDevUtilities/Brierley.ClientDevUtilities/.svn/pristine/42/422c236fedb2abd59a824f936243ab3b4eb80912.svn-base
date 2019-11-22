using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class EmailPersonalizationDao : DaoBase<EmailPersonalization>
    {
        public EmailPersonalizationDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public void Create(long emailId, string name, string expression)
        {
            var personalization = new EmailPersonalization(emailId, name, expression);
            Create(personalization);
        }

        public override void Update(EmailPersonalization personalization)
        {
            Database.Execute("update LW_EmailPersonalization set Expression = @0 where EmailId = @1 and Name = @2", personalization.Expression, personalization.EmailId, personalization.Name);
        }

        public EmailPersonalization Retrieve(long emailId, string name)
        {
            return Database.FirstOrDefault<EmailPersonalization>("select * from LW_EmailPersonalization where EmailId = @0 and Name = @1", emailId, name);
        }

        public IEnumerable<EmailPersonalization> Retrieve(long emailId)
        {
            return Database.Fetch<EmailPersonalization>("select * from LW_EmailPersonalization where EmailId = @0", emailId);
        }

        public void Delete(long emailId, string name)
        {
            EmailPersonalization personalization = Retrieve(emailId, name);
            if (personalization != null)
            {
				Database.Execute("delete from LW_EmailPersonalization where EmailId = @0 and Name = @1", emailId, name);
            }
        }

        public void Delete(long emailId)
        {
			Database.Execute("delete from LW_EmailPersonalization where EmailId = @0", emailId);
        }
    }
}
