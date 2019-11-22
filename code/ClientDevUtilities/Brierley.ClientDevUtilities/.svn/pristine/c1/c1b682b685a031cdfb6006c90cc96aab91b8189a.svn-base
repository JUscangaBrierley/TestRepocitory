using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ProductImageDao : DaoBase<ProductImage>
	{
		public ProductImageDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public ProductImage Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<ProductImage> RetrieveByProduct(long productId)
		{
			return Database.Fetch<ProductImage>("select * from LW_ProductImage where ProductId = @0 order by ImageOrder", productId);
		}

		public ProductImage RetrieveByProduct(long productId, string image)
		{
			return Database.FirstOrDefault<ProductImage>("select * from LW_ProductImage where ProductId = @0 and ProductImage = @1 order by ImageOrder", productId, image);
		}

		public List<ProductImage> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<ProductImage>("select * from LW_ProductImage where UpdateDate >= @0", since);
		}

		public List<ProductImage> RetrieveAll()
		{
			return Database.Fetch<ProductImage>("select * from LW_ProductImage");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
