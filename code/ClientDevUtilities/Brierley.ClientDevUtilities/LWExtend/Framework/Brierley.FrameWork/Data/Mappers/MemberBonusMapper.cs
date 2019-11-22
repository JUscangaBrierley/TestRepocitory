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
	internal class MemberBonusMapper : IMapper
	{
		private PetaPoco.StandardMapper standardMapper = new PetaPoco.StandardMapper();

		public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
		{
			if (pi.PropertyType == typeof(MemberBonusStatus))
			{
				return src =>
				{
					if (src != null)
						return (MemberBonusStatus)Enum.Parse(typeof(MemberBonusStatus), src.ToString());
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
					return ret.ToString();
				};
			}
			return null;
		}
	}
}
