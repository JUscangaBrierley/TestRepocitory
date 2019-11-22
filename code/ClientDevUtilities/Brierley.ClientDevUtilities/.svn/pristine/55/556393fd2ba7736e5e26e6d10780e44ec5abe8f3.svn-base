using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class MemberAppleWalletLoyaltyCardDao : DaoBase<MemberAppleWalletLoyaltyCard>
    {
        public MemberAppleWalletLoyaltyCardDao(Database database, ServiceConfig config)
			: base(database, config)
		{
        }

        public MemberAppleWalletLoyaltyCard Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        public MemberAppleWalletLoyaltyCard RetrieveByMemberId(long memberId)
        {
            return Database.FirstOrDefault<MemberAppleWalletLoyaltyCard>("select * from LW_MemberAWLoyaltyCard where MemberId = @0 order by CreateDate desc", memberId);
        }

        public MemberAppleWalletLoyaltyCard Retrieve(string serialNumber, string authToken)
        {
            return Database.FirstOrDefault<MemberAppleWalletLoyaltyCard>("select * from LW_MemberAWLoyaltyCard where SerialNumber = @0 and AuthToken = @1", serialNumber, authToken);
        }

        public List<MemberAppleWalletLoyaltyCard> RetrieveAll()
        {
            return Database.Fetch<MemberAppleWalletLoyaltyCard>("select * from LW_MemberAWLoyaltyCard");
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
        }
    }
}
