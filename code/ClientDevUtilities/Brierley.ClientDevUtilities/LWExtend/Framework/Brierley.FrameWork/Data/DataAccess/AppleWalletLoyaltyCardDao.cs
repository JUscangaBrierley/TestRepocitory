using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class AppleWalletLoyaltyCardDao : ContentDefDaoBase<AppleWalletLoyaltyCard>
    {
        private AppleWalletExtendedFieldDao _extendedFieldsDao;

        public override void Create(AppleWalletLoyaltyCard t)
        {
            base.Create(t);
            if (t.ExtendedFields != null)
            {
                foreach (var entity in t.ExtendedFields)
                {
                    entity.AppleWalletLoyaltyCardId = t.Id;
                    _extendedFieldsDao.Create(entity);
                }
            }
        }

        public override void Update(AppleWalletLoyaltyCard t, bool deep = true)
        {
            base.Update(t, deep);
            if (t.ExtendedFields != null)
            {
                foreach (var entity in t.ExtendedFields)
                {
                    entity.AppleWalletLoyaltyCardId = t.Id;
                    if (entity.Id == 0)
                        _extendedFieldsDao.Create(entity);
                    else
                        _extendedFieldsDao.Update(entity);
                }
            }
        }

        public AppleWalletLoyaltyCardDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.LoyaltyCard)
		{
            _extendedFieldsDao = new AppleWalletExtendedFieldDao(database, config, langChanContentDao, contentAttributeDao);
        }

        public AppleWalletLoyaltyCard Retrieve(long id)
        {
            var ret = GetEntity(id);
            if (ret != null)
            {
                PopulateContent(ret);
                ret.ExtendedFields = _extendedFieldsDao.RetrieveAllByParentId(ret.Id);
            }
            return ret;
        }

        public AppleWalletLoyaltyCard Retrieve(string name)
        {
            var ret = Database.FirstOrDefault<AppleWalletLoyaltyCard>("select * from LW_AWLoyaltyCard where lower(Name) = lower(@0)", name);
            if (ret != null)
            {
                PopulateContent(ret);
                ret.ExtendedFields = _extendedFieldsDao.RetrieveAllByParentId(ret.Id);
            }
            return ret;
        }

        public List<AppleWalletLoyaltyCard> RetrieveAll()
        {
            var ret = Database.Fetch<AppleWalletLoyaltyCard>("select * from LW_AWLoyaltyCard");
            if(ret != null)
            {
                PopulateContent(ret, true);
                foreach (var r in ret)
                    r.ExtendedFields = _extendedFieldsDao.RetrieveAllByParentId(r.Id);
            }
            return ret;
        }

        public List<AppleWalletLoyaltyCard> RetrieveChangedObjects(DateTime since)
        {
            var ret = Database.Fetch<AppleWalletLoyaltyCard>("select * from LW_AWLoyaltyCard where UpdateDate >= @0", since);
            if (ret != null)
            {
                PopulateContent(ret, true);
                foreach (var r in ret)
                    r.ExtendedFields = _extendedFieldsDao.RetrieveAllByParentId(r.Id);
            }
            return ret;
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
            _extendedFieldsDao.DeleteByParentId(id);
        }
    }
}
