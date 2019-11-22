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
    public class SmsPersonalizationDao : DaoBase<SmsPersonalization>
    {
        public SmsPersonalizationDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public void Create(long smsId, string name, string expression)
        {
            var personalization = new SmsPersonalization(smsId, name, expression);
            Create(personalization);
        }

        public override void Update(SmsPersonalization personalization)
        {
            Database.Execute("update LW_SmsPersonalization set Expression = @0 where SmsId = @1 and Name = @2", personalization.Expression, personalization.SmsId, personalization.Name);
        }

        public SmsPersonalization Retrieve(long smsId, string name)
        {
            return Database.FirstOrDefault<SmsPersonalization>("select * from LW_SmsPersonalization where SmsId = @0 and Name = @1", smsId, name);
        }

        public IEnumerable<SmsPersonalization> Retrieve(long smsId)
        {
            return Database.Fetch<SmsPersonalization>("select * from LW_SmsPersonalization where SmsId = @0", smsId);
        }

        public void Delete(long smsId, string name)
        {
			var personalization = Retrieve(smsId, name);
			if (personalization != null)
			{
				Database.Execute("delete from LW_SmsPersonalization where SmsId = @0 and Name = @1", smsId, name);
			}
        }

        public void Delete(long smsId)
        {
            Database.Execute("delete from LW_SmsPersonalization where SmsId = @0", smsId);
        }
    }
}
