using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	public class PromoTestSetConfig
	{
		// Online or Batch
        //[JsonProperty(PropertyName = "testType")]
        //public string TestType { get; set; }

        [JsonProperty(PropertyName = "promoMappingFile")]
        public string MappingFile { get; set; }

		[JsonProperty(PropertyName = "templates")]
		public PromoTestSetConfigTemplate[] Templates { get; set; }
	}

	[Serializable]
	public class PromoTestSetConfigTemplate
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "numMembers")]
		public long NumMembers { get; set; }
	}
}
