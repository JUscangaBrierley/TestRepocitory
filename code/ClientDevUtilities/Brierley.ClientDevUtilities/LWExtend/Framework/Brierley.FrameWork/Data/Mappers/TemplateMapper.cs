using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace Brierley.FrameWork.Data.Mappers
{
	internal class TemplateMapper : IMapper
	{
		private PetaPoco.StandardMapper standardMapper = new PetaPoco.StandardMapper();

		public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
		{
            if (sourceType == typeof(string) && pi.PropertyType == typeof(bool))
            {
                return src =>
                {
                    if (src != null)
                        return src.ToString().Equals("T", StringComparison.OrdinalIgnoreCase);
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
			return dst =>
			{
				if (dst is string && SourceProperty.GetType() == typeof(bool))
				{
					return (bool)dst ? "T" : "F";
				}
				return dst;
			};
		}
	}
}
