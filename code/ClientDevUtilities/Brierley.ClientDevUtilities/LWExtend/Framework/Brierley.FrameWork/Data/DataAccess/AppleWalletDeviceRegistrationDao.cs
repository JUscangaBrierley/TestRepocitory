using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class AppleWalletDeviceRegistrationDao : DaoBase<AppleWalletDeviceRegistration>
    {
        public AppleWalletDeviceRegistrationDao(Database database, ServiceConfig config)
			: base(database, config)
		{
        }

        public AppleWalletDeviceRegistration Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        public AppleWalletDeviceRegistration Retrieve(long deviceId, long memberWalletCardID)
        {
            return Database.FirstOrDefault<AppleWalletDeviceRegistration>("select * from LW_AWDeviceRegistration where AppleWalletDeviceID = @0 and MemberAppleWalletLoyaltyCardID = @1"
                , deviceId, memberWalletCardID);
        }

        public List<AppleWalletDeviceRegistration> RetrieveAll()
        {
            return Database.Fetch<AppleWalletDeviceRegistration>("select * from LW_AWDeviceRegistration");
        }

        public List<AppleWalletDeviceRegistration> RetrieveAllForDevice(long deviceId)
        {
            return Database.Fetch<AppleWalletDeviceRegistration>("select * from LW_AWDeviceRegistration where AppleWalletDeviceID = @0", deviceId);
        }

        public List<AppleWalletDeviceRegistration> RetrieveAllForMemberPass(long memberWalletCardID)
        {
            return Database.Fetch<AppleWalletDeviceRegistration>("select * from LW_AWDeviceRegistration where MemberAppleWalletLoyaltyCardID = @0", memberWalletCardID);
        }

        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }
    }
}
