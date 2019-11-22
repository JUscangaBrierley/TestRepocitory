using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class StoreWithDistance : StoreDef
	{
		[PetaPoco.Column("DistanceInMiles")]
		public double DistanceInMiles { get; set; }
	}


	public class StoreDao : DaoBase<StoreDef>
	{
		public StoreDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public StoreDef Retrieve(long storeId)
		{
			return GetEntity(storeId);
		}

		public List<StoreDef> Retrieve(string storeNumber)
		{
			return Database.Fetch<StoreDef>("select * from LW_StoreDef where StoreNumber = @0", storeNumber);
		}

		public StoreDef RetrieveByStoreNumberAndBrand(string storeNumber, string brandName)
		{
			return Database.FirstOrDefault<StoreDef>("select * from LW_StoreDef where StoreNumber = @0 and BrandName = @1", storeNumber, brandName);
		}

		public StoreDef RetrieveByBrandStore(string brandName, string brandStoreNumber)
		{
			return Database.FirstOrDefault<StoreDef>("select * from LW_StoreDef where BrandName = @0 and BrandStoreNumber = @1", brandName, brandStoreNumber);
		}

		public List<StoreDef> RetrieveAll(LWQueryBatchInfo batchInfo)
		{
			string sql = "select s.* from LW_StoreDef s";
			var args = new object[0];
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<StoreDef>(sql, args);
		}

		public List<StoreDef> RetrieveAll(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<StoreDef>();
            return RetrieveByArray<long>("select * from LW_StoreDef where StoreId in (@0)", ids);
		}

		public NearbyStoreCollection RetrieveNearby(double latitude, double longitude, double radiusInMiles, int maxRows)
		{
			string sql = @"select s.*, cast(
(Acos(Cos(s.Latitude * @3) * Cos(s.Longitude * @3) * Cos(@0 * @3) * Cos(@1 * @3) + 
Cos(s.Latitude * @3) * Sin(s.Longitude * @3) * Cos(@0 * @3) * Sin(@1 * @3) + 
Sin(s.Latitude * @3) * Sin(@0 * @3)
) * 3963.1) as decimal) as DistanceInMiles
from LW_StoreDef s where s.Status=1 and 
(Acos(Cos(s.Latitude * @3) * Cos(s.Longitude * @3) * Cos(@0 * @3) * Cos(@1 * @3) + 
Cos(s.Latitude * @3) * Sin(s.Longitude * @3) * Cos(@0 * @3) * Sin(@1 * @3) + 
Sin(s.Latitude * @3) * Sin(@0 * @3)
) * 3963.1) <= @2
order by (Acos(Cos(s.Latitude *@3) * Cos(s.Longitude * @3) * Cos(@0 * @3) * Cos(@1 * @3) + 
Cos(s.Latitude * @3) * Sin(s.Longitude * @3) * Cos(@0 * @3) * Sin(@1 * @3) + 
Sin(s.Latitude * @3) * Sin(@0 * @3)
) * 3963.1)";

			var args = new object[] { latitude, longitude, radiusInMiles, 0.017453293 };

			if (maxRows > 0)
			{
				ApplyBatchInfo(new LWQueryBatchInfo(0, maxRows), ref sql, ref args);
			}

			var stores = Database.Fetch<StoreWithDistance>(sql, args);

			var collection = new NearbyStoreCollection();
			foreach (var row in stores)
			{
				collection.Add(new NearbyStoreItem() { DistanceInMiles = row.DistanceInMiles, Store = (StoreDef)row });
			}
			return collection;
		}

		public List<StoreDef> RetrieveAllByCityAndStateOrProvince(string city, string stateOrProvince, int maxRows)
		{
			if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(stateOrProvince))
			{
				return RetrieveAll(new LWQueryBatchInfo(0, maxRows));
			}

			var args = new object[] { city, stateOrProvince };

			string sql = "select s.* from LW_StoreDef s where ";
			if (!string.IsNullOrWhiteSpace(city))
			{
				sql += "lower(city) = lower(@0)";
			}
			if (!string.IsNullOrWhiteSpace(stateOrProvince))
			{
				if (!string.IsNullOrWhiteSpace(city))
				{
					sql += " and ";
				}
				sql += "lower(StateOrProvince) = lower(@1)";
			}

			if (maxRows > 0)
			{
				ApplyBatchInfo(new LWQueryBatchInfo(0, maxRows), ref sql, ref args);
			}
			return Database.Fetch<StoreDef>(sql, args);
		}

		public List<StoreDef> RetrieveAllByZipOrPostalCode(string zipOrPostalCode, int maxRows)
		{
			if (string.IsNullOrEmpty(zipOrPostalCode))
			{
				return RetrieveAll(new LWQueryBatchInfo(0, maxRows));
			}
			var args = new object[] { zipOrPostalCode };
			string sql = "select s.* from LW_StoreDef s where lower(ZipOrPostalCode) = @0";
			if (maxRows > 0)
			{
				ApplyBatchInfo(new LWQueryBatchInfo(0, maxRows), ref sql, ref args);
			}
			return Database.Fetch<StoreDef>(sql, args);
		}

		public Dictionary<long, string> RetrieveByProperty(string propertyName, string whereClause)
		{
			string sql = string.Format("select StoreId as TheKey, {0} as TheValue from LW_StoreDef", propertyName);
			if (!string.IsNullOrEmpty(whereClause))
			{
				sql += " where " + whereClause;
			}

			return MakeDictionary(Database.Fetch<dynamic>(sql.ToUpper()));
		}

		public List<StoreDef> RetrieveByUserField(string userField)
		{
			return Database.Fetch<StoreDef>("select * from LW_StoreDef where StrUserField = @0", userField);
		}

		public List<StoreDef> RetrieveByUserField(long userField)
		{
			return Database.Fetch<StoreDef>("select * from LW_StoreDef where LongUserField = @0", userField);
		}

		public List<StoreDef> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<StoreDef>("select * from LW_StoreDef where UpdateDate >= @0", since);
		}

		public void Delete(long storeNumber)
		{
			DeleteEntity(storeNumber);
		}

		private Dictionary<long, string> MakeDictionary(List<dynamic> list)
		{
			Dictionary<long, string> ret = new Dictionary<long, string>();
			foreach (dynamic d in list)
			{
				ret.Add((long)d.THEKEY, d.THEVALUE.ToString());
			}
			return ret;
		}	
	}
}
