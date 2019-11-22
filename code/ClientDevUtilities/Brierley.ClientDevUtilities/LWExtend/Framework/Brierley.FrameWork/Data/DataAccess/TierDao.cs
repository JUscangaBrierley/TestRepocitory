using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class TierDao : ContentDefDaoBase<TierDef>
	{
        public TierDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
            : base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.Tier)
        {
		}

		public TierDef Retrieve(long tierId)
		{
            TierDef ret = Database.FirstOrDefault<TierDef>("where TierId = @0", tierId);
            if (ret != null)
            {
                PopulateContent(ret);
            }
            return ret;
		}

		public TierDef RetrieveByName(string tierName)
		{
            TierDef ret = Database.FirstOrDefault<TierDef>("where TierName = @0", tierName);
            if (ret != null)
            {
                PopulateContent(ret);
            }
            return ret;
		}

		public List<TierDef> RetrieveAll()
		{
			var tiers = Database.Fetch<TierDef>(string.Empty);
            if(tiers != null)
            {
                PopulateContent(tiers, true);
            }
            return tiers;
		}

		public List<TierDef> RetrieveChangedObjects(DateTime since)
		{
            var tiers = Database.Fetch<TierDef>("where UpdateDate >= @0", since);
            if (tiers != null)
            {
                PopulateContent(tiers, true);
            }
            return tiers;
		}

		public void Delete(long tierID)
		{
			DeleteEntity(tierID);
		}
	}
}
