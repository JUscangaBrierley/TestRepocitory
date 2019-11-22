using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;

namespace Brierley.ClientDevUtilities.LWGateway.DataAccess
{
    public interface IDaoBase<T>
    {
        string OwnerID { get; }
        ApplicationType ApplicationType { get; set; }
        string HostName { get; }
        UserIdentityType AuthIdentityType { get; }

        void Create(T t);
        void Create(IEnumerable<T> ts);
        void Update(IEnumerable<T> ts);
        void Update(T t);
    }
}
