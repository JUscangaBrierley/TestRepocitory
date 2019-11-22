using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberSocNetDao : DaoBase<MemberSocNet>
	{
		public MemberSocNetDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public override void Create(MemberSocNet entity)
		{
			if (entity.MemberId <= -1)
			{
				throw new ArgumentOutOfRangeException("Invalid member for mapping.");
			}
			if (entity.ProviderType == SocialNetworkProviderType.None)
			{
				throw new ArgumentOutOfRangeException("Can't map to 'none' provider.");
			}
			if (string.IsNullOrEmpty(entity.ProviderUID))
			{
				throw new ArgumentOutOfRangeException("Invalid provider UID for mapping.");
			}

			SaveEntity(entity);
		}

		public override void Update(MemberSocNet entity)
		{
			if (entity.MemberId <= -1)
			{
				throw new ArgumentOutOfRangeException("Invalid member for mapping.");
			}
			if (entity.ProviderType == SocialNetworkProviderType.None)
			{
				throw new ArgumentOutOfRangeException("Can't map to 'none' provider.");
			}
			if (string.IsNullOrEmpty(entity.ProviderUID))
			{
				throw new ArgumentOutOfRangeException("Invalid provider UID for mapping.");
			}

			UpdateEntity(entity);
		}

		public MemberSocNet Retrieve(long id)
		{
			return GetEntity(id);
		}

		public MemberSocNet RetrieveByProviderUId(SocialNetworkProviderType providerType, string providerUId)
		{
			return Database.FirstOrDefault<MemberSocNet>("select * from LW_MemberSocNet where ProviderType=@0 and ProviderUID=@1 order by CreateDate desc", providerType, providerUId); //LW-1658
		}

		public List<MemberSocNet> RetrieveByProviderUId(SocialNetworkProviderType providerType, string[] providerUIdList)
		{
            if (providerUIdList == null || providerUIdList.Length == 0)
                return new List<MemberSocNet>();
            return Database.Fetch<MemberSocNet>("select * from LW_MemberSocNet where ProviderType = @0 and ProviderUId in (@ids) order by CreateDate desc", providerType, new { ids = providerUIdList }); //LW-1658
		}

		public List<MemberSocNet> RetrieveAll()
		{
			return Database.Fetch<MemberSocNet>("select * from LW_MemberSocNet");
		}

		public List<MemberSocNet> RetrieveByMember(long memberId)
		{
            return Database.Fetch<MemberSocNet>("select * from LW_MemberSocNet where MemberId = @0 order by CreateDate desc", memberId); //LW-1658
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MemberSocNet where MemberId = @0", memberId);
		}

		public int DeleteByMembers(long[] memberIds)
		{
			int keysRemaining = memberIds.Length;
			int startIdx = 0;
			int nRows = 0;
			while (keysRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(memberIds, ref startIdx, ref keysRemaining);
				nRows += Database.Execute("delete from LW_MemberSocNet where MemberId in (@ids)", new { ids = ids });
			}
			return nRows;
		}
	}
}
