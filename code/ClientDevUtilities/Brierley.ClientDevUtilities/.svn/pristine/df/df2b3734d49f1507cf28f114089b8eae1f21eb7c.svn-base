using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MTouchDao : DaoBase<MTouch>
	{
		public MTouchDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public MTouch Retrieve(long id)
		{
			return GetEntity(id);
		}

		public MTouch Retrieve(string mtouch)
		{
			return Database.FirstOrDefault<MTouch>("select * from LW_MTouch where MTouchValue = @0", mtouch);
		}

		public MTouch RetrieveNextMTouchId(MTouchType type)
		{
			return Database.FirstOrDefault<MTouch>("select * from LW_MTouch where MTouchType = @0 and Available = 1", type);
		}

		public List<MTouch> RetrieveByObjectType(MTouchType type, bool onlyAvailable)
		{
			string sql = "select * from LW_MTouch where MTouchType = @0";
			if (onlyAvailable)
			{
				sql += "and Available = 1";
			}
			return Database.Fetch<MTouch>(sql, type);
		}

		public int GetUsageCount(MTouch mTouch)
		{
			if (mTouch.MTouchType == MTouchType.Email || mTouch.MTouchType == MTouchType.Survey)
			{
				return 0;
			}

			string sql = "select count(*) from {0} where MTouchId = @0";

			switch (mTouch.MTouchType)
			{
				case MTouchType.Bonus:
					sql = string.Format(sql, "LW_MemberBonus");
					break;
				case MTouchType.Coupon:
					sql = string.Format(sql, "LW_MemberCoupon");
					break;
				case MTouchType.Promotion:
					sql = string.Format(sql, "LW_MemberPromotion");
					break;
			}

			return Database.ExecuteScalar<int>(sql, mTouch.ID);
		}

		public int GetMTouchCount(MTouchType type, string secondaryId)
		{
			if (type == MTouchType.Email || type == MTouchType.Survey)
			{
				return 0;
			}

			string sql = "select count(*) from LW_MTouch where MTouchType = @0";

			if (!string.IsNullOrEmpty(secondaryId))
			{
				sql += " and SecondaryId = @1";
			}

			return Database.ExecuteScalar<int>(sql, type, secondaryId);
		}

		public List<long> RetrieveIDs(MTouchType mtouchType, bool onlyAvailable, string secondaryId)
		{
			string sql = "select Id from MTouch where MTouchType = @0";
			if (onlyAvailable)
			{
				sql += " and Available = 1";
			}
			if (!string.IsNullOrWhiteSpace(secondaryId))
			{
				sql += " and SecondaryId = @1";
			}

			return Database.Fetch<long>(sql, mtouchType, secondaryId);
		}

		public void Delete(long mtouchId)
		{
			DeleteEntity(mtouchId);
		}

		public int DeleteByIDs(IList<long> mtouchIds)
		{
			if (mtouchIds.Count < 1)
			{
				throw new ArgumentNullException("mtouchIds");
			}

			return Database.Execute("delete from LW_Mtouch where Id in (@ids)", new { ids = mtouchIds.ToArray() });
		}
	}
}
