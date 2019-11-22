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
	internal class RuleExecutionLogMapper : IMapper
	{
		private PetaPoco.StandardMapper standardMapper = new PetaPoco.StandardMapper();

		public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
		{
			if (sourceType == typeof(string) && pi.PropertyType == typeof(RuleExecutionStatus))
			{
				return src =>
				{
					return (RuleExecutionStatus)Enum.Parse(typeof(RuleExecutionStatus), src.ToString());
				};
			}
			else if (sourceType == typeof(string) && pi.PropertyType == typeof(RuleExecutionMode))
			{
				return src =>
				{
					return (RuleExecutionMode)Enum.Parse(typeof(RuleExecutionMode), src.ToString());
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
				if (dst is RuleExecutionMode || dst is RuleExecutionStatus)
				{
					return dst.ToString();
				}
				return dst;
			};
		}
	}
}





