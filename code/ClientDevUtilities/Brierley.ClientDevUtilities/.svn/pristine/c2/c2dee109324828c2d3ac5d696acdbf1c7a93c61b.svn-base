using Brierley.FrameWork.Data.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.ClientDevUtilities.LWGateway.DataAccess
{
    public interface ILWEventDao : IDaoBase<LWEvent>
    {
        void Delete(long id);
        LWEvent Retrieve(long id);
        LWEvent Retrieve(string name);
        List<LWEvent> RetrieveAll(bool userDefinedOnly);
        List<LWEvent> RetrieveChangedObjects(DateTime since, bool userDefinedOnly);
    }
}
