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
	internal class MemberCouponMapper : IMapper
	{
		private PetaPoco.StandardMapper standardMapper = new PetaPoco.StandardMapper();

		public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
		{
			if (pi.PropertyType == typeof(CouponStatus?))
            {
                return src =>
                {
                    if (src != null)
                        return (CouponStatus)Enum.Parse(typeof(CouponStatus), src.ToString());
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
			if (SourceProperty.Name == "Status")
			{
				return ret =>
				{
                    return ret == null ? null : ret.ToString();
				};
			}
			return null;
		}
	}
}
