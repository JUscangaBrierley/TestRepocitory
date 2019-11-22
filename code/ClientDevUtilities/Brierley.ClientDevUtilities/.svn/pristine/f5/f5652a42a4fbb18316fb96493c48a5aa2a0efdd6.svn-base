using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class AndroidPayLoyaltyCardDao : ContentDefDaoBase<AndroidPayLoyaltyCard>
    {
        public AndroidPayLoyaltyCardDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.LoyaltyCard)
		{
        }

        public AndroidPayLoyaltyCard Retrieve(long id)
        {
            var ret = GetEntity(id);
            if (ret != null)
            {
                PopulateContent(ret);
            }
            return ret;
        }

        public AndroidPayLoyaltyCard Retrieve(string name)
        {
            var ret = Database.FirstOrDefault<AndroidPayLoyaltyCard>("select * from LW_APLoyaltyCard where lower(Name) = lower(@0)", name);
            if (ret != null)
            {
                PopulateContent(ret);
            }
            return ret;
        }

        public List<AndroidPayLoyaltyCard> RetrieveAll()
        {
            var ret = Database.Fetch<AndroidPayLoyaltyCard>("select * from LW_APLoyaltyCard");
            if (ret != null)
            {
                PopulateContent(ret, true);
            }
            return ret;
        }

        public List<AndroidPayLoyaltyCard> RetrieveChangedObjects(DateTime since)
        {
            var ret = Database.Fetch<AndroidPayLoyaltyCard>("select * from LW_APLoyaltyCard where UpdateDate >= @0", since);
            if(ret != null)
            {
                PopulateContent(ret, true);
            }
            return ret;
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
        }
    }
}
