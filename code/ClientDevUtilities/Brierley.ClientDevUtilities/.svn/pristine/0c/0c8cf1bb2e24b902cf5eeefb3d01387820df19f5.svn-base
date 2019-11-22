//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Brierley.FrameWork.Common;
//using Brierley.FrameWork.Data.Cache;

//namespace Brierley.FrameWork.Data
//{
//	//todo: DataServiceConfig being its own class doesn't make sense (anymore). We maintain a single map of service configs, so they should all be of the same type. 
//	//Move the options from data service config into the base class and seal it.
//	public class DataServiceConfig : ServiceConfig
//	{
//		public DataServiceConfig(string connectString, SupportedDataSourceType databaseType, DbProviderFactory factory, IDataCacheProvider cacheManager)
//			: base(connectString, databaseType, factory, cacheManager)
//		{
//		}

//		public bool DebitPayOffMethodOn { get; set; }
//		public bool DebitPayOffPointTypeRestrictionOn { get; set; }
//		public bool DebitPayOffPointEventRestrictionOn { get; set; }

//		internal object IdGenMutex = new Object();
//		internal int BucketSize { get; set; }
//		internal Dictionary<string, IDGenStats> IdGenBuckets = new Dictionary<string, IDGenStats>();
//	}
//}
