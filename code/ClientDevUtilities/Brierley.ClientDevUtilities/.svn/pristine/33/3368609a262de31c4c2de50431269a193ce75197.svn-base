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
    public class FulfillmentProviderProductMapDao : DaoBase<FulfillmentProviderProductMap>
    {
        public FulfillmentProviderProductMapDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

        public void Create(FulfillmentProviderProductMap prodMap)
        {
            prodMap.CreateDate = DateTime.Now;
            SaveEntity(prodMap);
        }

        public void Update(FulfillmentProviderProductMap prodMap)
        {
            prodMap.UpdateDate = DateTime.Now;
            UpdateEntity(prodMap);
        }

        public FulfillmentProviderProductMap Retrieve(long id)
        {
            return GetEntity(id);
        }

        public FulfillmentProviderProductMap RetrieveByProduct(long providerId, long productId)
        {
            return Database.FirstOrDefault<FulfillmentProviderProductMap>("select * from LW_FulfillmentProductMap where ProviderId = @0 and ProductId = @1", providerId, productId);
        }

        public FulfillmentProviderProductMap RetrieveByProductVariant(long providerId, long variantId)
        {
            return Database.FirstOrDefault<FulfillmentProviderProductMap>("select * from LW_FulfillmentProductMap where ProviderId = @0 and ProductVariantId = @1", providerId, variantId);
        }

        public FulfillmentProviderProductMap RetrieveByProviderPartNumber(long providerId, string partNumber)
        {
            return Database.FirstOrDefault<FulfillmentProviderProductMap>("select * from LW_FulfillmentProductMap where ProviderId = @0 and FProviderPartNumber = @1", providerId, partNumber);
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
        }
    }
}
