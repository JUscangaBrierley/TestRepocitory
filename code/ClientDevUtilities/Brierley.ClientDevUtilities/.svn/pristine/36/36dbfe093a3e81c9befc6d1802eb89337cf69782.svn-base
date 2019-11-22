using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Brierley.ClientDevUtilities.Net
{
    public class QueryStringBuilder
    {
        private StringBuilder builder = new StringBuilder();

        public void AddParameter(string name, string value)
        {
            if (value != null)
            {
                string parameter = (builder.Length == 0 ? "?" : "&") +
                                   HttpUtility.UrlEncode(name) + "=" +
                                   HttpUtility.UrlEncode(value);

                builder.Append(parameter);
            }
        }

        public void AddParameter<T>(string name, Nullable<T> value) where T : struct
        {
            if (value != null)
            {
                AddParameter(name, value.ToString());
            }
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
