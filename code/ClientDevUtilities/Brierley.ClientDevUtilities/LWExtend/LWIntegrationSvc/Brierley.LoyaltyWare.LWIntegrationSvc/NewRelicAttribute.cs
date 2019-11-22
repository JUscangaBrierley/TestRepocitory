using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.LoyaltyWare.LWIntegrationSvc
{
	[Serializable]
	public class NewRelicAttribute
	{
		public string Key { get; set; }
		public string Value { get; set; }

		public NewRelicAttribute()
		{
		}

		public NewRelicAttribute(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public override string ToString()
		{
			return string.Format("{0}={1}", Key, Value);
		}
	}
}
