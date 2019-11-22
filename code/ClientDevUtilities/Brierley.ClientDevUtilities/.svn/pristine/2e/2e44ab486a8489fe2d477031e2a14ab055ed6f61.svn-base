using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Common
{
    public class ContentManagementUtils
    {
        /// <summary>
        /// Checks that the source is a web address and prepends the alternateSource if the source isn't a complete address
        /// </summary>
        /// <param name="source">URL/file to be validated</param>
        /// <param name="alternateSource">Used as a prepended string if the source isn't a complete address</param>
        /// <returns></returns>
        public static string GetValidWebAddres(string source, string alternateSource)
        {
            // Source string starts with http and has '://' somewhere in the string (i.e. http:// or https://)
            return (source.StartsWith("http", StringComparison.OrdinalIgnoreCase) && source.Contains("://") ? string.Empty : alternateSource) + source;
        }
    }
}
