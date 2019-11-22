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
	internal class X509CertMapper : IMapper
	{
		private PetaPoco.StandardMapper standardMapper = new PetaPoco.StandardMapper();

		public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
		{
            if (sourceType == typeof(string) && pi.PropertyType == typeof(X509CertType))
            {
                return src =>
                {
                    return (X509CertType)Enum.Parse(typeof(X509CertType), src.ToString());
                };
            }
            //nullable booleans do not convert nicely from Oracle
            if (sourceType == typeof(short) && pi.PropertyType == typeof(bool?))
            {
                return src =>
                {
                    if (src != null)
                        return Convert.ToBoolean((short)src);
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
				if (dst is X509CertType)
				{
					return dst.ToString();
				}
				return dst;
			};
		}
	}

}







