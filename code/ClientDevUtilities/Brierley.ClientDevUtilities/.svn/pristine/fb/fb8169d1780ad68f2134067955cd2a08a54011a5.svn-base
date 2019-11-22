using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement;
using PetaPoco;

namespace Brierley.FrameWork.Data.Mappers
{
	internal class CampaignMapper : IMapper
	{
		private PetaPoco.StandardMapper standardMapper = new PetaPoco.StandardMapper();

		public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
		{
			if (pi.PropertyType == typeof(ExecutionTypes?))
            {
                return src =>
                {
                    return (ExecutionTypes)Enum.Parse(typeof(ExecutionTypes), src.ToString());
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
			return standardMapper.GetToDbConverter(SourceProperty);
		}
	}
}
