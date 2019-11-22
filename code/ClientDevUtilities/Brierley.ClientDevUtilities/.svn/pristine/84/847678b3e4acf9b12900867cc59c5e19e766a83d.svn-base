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
	internal class PromotionCertificateMapper : IMapper
	{
		private PetaPoco.StandardMapper standardMapper = new PetaPoco.StandardMapper();

		public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
		{
			if (sourceType == typeof(string) && pi.PropertyType == typeof(ContentObjType))
			{
				return src =>
				{
					return (ContentObjType)Enum.Parse(typeof(ContentObjType), src.ToString());
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
				if (dst is ContentObjType)
				{
					return dst.ToString();
				}
				return dst;
			};
		}
	}
}





