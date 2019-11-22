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
	internal class SocialCampaignMapper : IMapper
	{
		private PetaPoco.StandardMapper standardMapper = new PetaPoco.StandardMapper();

		public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				if (pi.PropertyType == typeof(SocialNetworkProviderType))
				{
					return src =>
					{
						return (SocialNetworkProviderType)Enum.Parse(typeof(SocialNetworkProviderType), src.ToString());
					};
				}
				if (pi.PropertyType == typeof(SocialSentiment))
				{
					return src =>
					{
						return (SocialSentiment)Enum.Parse(typeof(SocialSentiment), src.ToString());
					};
				}
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
			if (SourceProperty.Name == "Publisher" || SourceProperty.Name == "Sentiment")
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
