using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class AppleWalletExtendedFieldDao : ContentDefDaoBase<AppleWalletExtendedField>
    {
        public AppleWalletExtendedFieldDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.LoyaltyCard)
		{
        }

        public AppleWalletExtendedField Retrieve(long id)
        {
            var ret = GetEntity(id);
            if (ret != null)
            {
                PopulateContent(ret);
            }
            return ret;
        }

        public AppleWalletExtendedField Retrieve(string name)
        {
            var ret = Database.FirstOrDefault<AppleWalletExtendedField>("select * from LW_AWExtendedField where lower(Name) = lower(@0)", name);
            if (ret != null)
            {
                PopulateContent(ret);
            }
            return ret;
        }

        public List<AppleWalletExtendedField> RetrieveAllByParentId(long id)
        {
            var ret = Database.Fetch<AppleWalletExtendedField>("select * from LW_AWExtendedField where AppleWalletLoyaltyCardId = @0", id);
            if(ret != null)
            {
                PopulateContent(ret, true);
            }
            return ret;
        }

        public List<AppleWalletExtendedField> RetrieveAll()
        {
            var ret = Database.Fetch<AppleWalletExtendedField>("select * from LW_AWExtendedField");
            if (ret != null)
            {
                PopulateContent(ret, true);
            }
            return ret;
        }

        public List<AppleWalletExtendedField> RetrieveAllChanged(DateTime since)
        {
            var ret = Database.Fetch<AppleWalletExtendedField>("select * from LW_AWExtendedField where UpdateDate >= @0", since);
            if (ret != null)
            {
                PopulateContent(ret, true);
            }
            return ret;
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
        }

        public void DeleteByParentId(long id)
        {
            var ret = RetrieveAllByParentId(id);
            if (ret != null)
            {
                foreach (var r in ret)
                {
                    Delete(r.Id);
                }
            }
        }
    }
}
