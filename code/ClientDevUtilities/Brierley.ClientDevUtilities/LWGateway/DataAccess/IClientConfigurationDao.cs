using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.LWGateway.DataAccess
{
    public interface IClientConfigurationDao : IDaoBase<ClientConfiguration>
    {
        void Delete(string key);
        ClientConfiguration Retrieve(string key);
        List<ClientConfiguration> RetrieveAll();
        List<ClientConfiguration> RetrieveAllExternal();
        List<ClientConfiguration> RetrieveAllExternalByFolder(long folderId);
        List<string> RetrieveAllKeys();
        List<ClientConfiguration> RetrieveAllLike(string keyPattern);
        List<ClientConfiguration> RetrieveChangedObjects(DateTime since, bool onlyExternal);
    }
}
