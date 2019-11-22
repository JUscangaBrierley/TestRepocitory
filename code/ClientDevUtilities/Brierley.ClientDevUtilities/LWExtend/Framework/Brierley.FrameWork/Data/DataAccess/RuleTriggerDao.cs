using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class RuleTriggerDao : DaoBase<RuleTrigger>
	{
		public RuleTriggerDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public RuleTrigger Retrieve(long id)
		{
			return Database.FirstOrDefault<RuleTrigger>("where RuleTriggerId = @0", id);
		}

		public RuleTrigger Retrieve(string name)
		{
			return Database.FirstOrDefault<RuleTrigger>("where RuleName = @0", name);
		}

		public List<RuleTrigger> RetrieveByOwningObject(string name)
		{
			return Database.Fetch<RuleTrigger>("where OwningObject = @0 order by sequence", name);
		}

		public List<RuleTrigger> RetrieveByAttributeSet(long attSetCode)
		{
			return Database.Fetch<RuleTrigger>("where AttributeSetCode = @0 order by sequence", attSetCode);
		}

		public List<RuleTrigger> RetrieveByPromotion(string promoCode)
		{
			return Database.Fetch<RuleTrigger>("where PromotionCode = @0 order by sequence", promoCode);
		}

		public List<RuleTrigger> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<RuleTrigger>("where UpdateDate >= @0", since);
		}

		public List<RuleTrigger> RetrieveAll()
		{
			return Database.Fetch<RuleTrigger>("select * from LW_RuleTriggers");
		}

		public List<RuleTrigger> RetrieveAll(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<RuleTrigger>();
            return RetrieveByArray<long>("where RuleTriggerId in (@0)", ids);
        }

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
