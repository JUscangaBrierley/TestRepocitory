using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class AppleWalletDeviceDao : DaoBase<AppleWalletDevice>
    {
        public AppleWalletDeviceDao(Database database, ServiceConfig config)
			: base(database, config)
		{
        }

        public AppleWalletDevice Retrieve(long ID)
        {
            return GetEntity(ID);
        }

        public AppleWalletDevice Retrieve(string deviceID)
        {
            return Database.FirstOrDefault<AppleWalletDevice>("select * from LW_AWDevice where DeviceID = @0", deviceID);
        }

        public List<AppleWalletDevice> RetrieveAll()
        {
            return Database.Fetch<AppleWalletDevice>("select * from LW_AWDevice");
        }

        public List<AppleWalletDevice> RetrieveAll(string deviceID)
        {
            return Database.Fetch<AppleWalletDevice>("select * from LW_AWDevice where DeviceID = @0", deviceID);
        }

        public void Delete(long ID)
        {
            DeleteEntity(ID);
        }
    }
}
