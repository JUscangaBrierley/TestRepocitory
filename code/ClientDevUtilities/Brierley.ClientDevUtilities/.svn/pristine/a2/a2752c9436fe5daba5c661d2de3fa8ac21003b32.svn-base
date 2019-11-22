using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using PetaPoco;

namespace Brierley.FrameWork.Data.Mappers
{
	internal class MemberRewardMapper : IMapper
	{
		private PetaPoco.StandardMapper standardMapper = new PetaPoco.StandardMapper();

		public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
		{
            if (pi.PropertyType == typeof(RewardOrderStatus?))
            {
                return src =>
                {
                    if (src != null)
                        return (RewardOrderStatus)Enum.Parse(typeof(RewardOrderStatus), src.ToString());
                    return null;
                };
            }

			if (pi.PropertyType == typeof(RewardFulfillmentOption?))
			{
				return src =>
				{
					if (src != null)
						return (RewardFulfillmentOption)Enum.Parse(typeof(RewardFulfillmentOption), src.ToString());
					return null;
				};
			}

            return null;
		}

		public TableInfo GetTableInfo(Type pocoType)
		{
			return standardMapper.GetTableInfo(pocoType);
		}

		public ColumnInfo GetColumnInfo(PropertyInfo pocoProperty)
		{
			return standardMapper.GetColumnInfo(pocoProperty);
		}

		public Func<object, object> GetToDbConverter(PropertyInfo SourceProperty)
		{
			if (/*SourceProperty.Name == "FulfillmentOption" ||*/ SourceProperty.Name == "OrderStatus")
			{
				return ret =>
				{
					if (ret != null)
					{
						return ret.ToString();
					}
					return null;
				};
			}
			return null;
		}
	}
}
