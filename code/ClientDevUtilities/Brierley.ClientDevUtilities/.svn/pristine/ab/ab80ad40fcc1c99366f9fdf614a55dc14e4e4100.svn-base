using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.Data
{
    public static class ContentAttributeExtensions
    {
        public static ContentAttribute GetByDefId(this IList<ContentAttribute> list, long contentAttributeDefId)
        {
            return list.FirstOrDefault(x => x.ContentAttributeDefId == contentAttributeDefId);
        }

        public static string GetValueByDefId(this IList<ContentAttribute> list, long contentAttributeDefId)
        {
            var attribute = GetByDefId(list, contentAttributeDefId);

            if (attribute == null)
            {
                return null;
            }

            return attribute.Value;
        }

        public static ContentAttribute GetByDefName(this IList<ContentAttribute> list, ILWDataServiceUtil lwDataServiceUtil, string contentAttributeDefName)
        {
            using (var content = lwDataServiceUtil.ContentServiceInstance())
            {
                return GetByDefId(list, content.GetContentAttributeDef(contentAttributeDefName).ID);
            }
        }

        public static string GetValueByDefName(this IList<ContentAttribute> list, ILWDataServiceUtil lwDataServiceUtil, string contentAttributeDefName)
        {
            var attribute = GetByDefName(list, lwDataServiceUtil, contentAttributeDefName);

            if (attribute == null)
            {
                return null;
            }

            return attribute.Value;
        }

        public static GetValueSetByDefNamesResult GetValueSetByDefNames(this IList<ContentAttribute> list, ILWDataServiceUtil lwDataServiceUtil, IEnumerable<string> defNames)
        {
            var attributeDefs = new List<ContentAttributeDef>();

            using (var content = lwDataServiceUtil.ContentServiceInstance())
            {
                foreach (string name in defNames)
                {
                    attributeDefs.Add(content.GetContentAttributeDef(name));
                }
            }

            var values = attributeDefs.GroupJoin(list, x => x.ID, x => x.ContentAttributeDefId, (def, attributes) => new { DefName = def.Name, Attribute = attributes.FirstOrDefault() })
                .ToDictionary(x => x.DefName, x => x.Attribute != null ? x.Attribute.Value : null);

            var result = new GetValueSetByDefNamesResult();
            
            result.AnyValuePopulated = values.Any(x => !string.IsNullOrEmpty(x.Value));
            result.AllValuesPopulated = result.AnyValuePopulated && !values.Any(x => string.IsNullOrEmpty(x.Value));
            result.Values = values;

            return result;
        }

        public class GetValueSetByDefNamesResult
        {
            public bool AnyValuePopulated { get; set; }
            public bool AllValuesPopulated { get; set; }
            public IDictionary<string, string> Values { get; set; }
        }
    }
}
