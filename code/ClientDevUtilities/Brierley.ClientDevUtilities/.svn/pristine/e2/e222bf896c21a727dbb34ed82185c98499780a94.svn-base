using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Email;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public class TriggeredEmailFactory : ITriggeredEmailFactory
    {
        public static TriggeredEmailFactory Instance { get; private set; }

        static TriggeredEmailFactory()
        {
            Instance = new TriggeredEmailFactory();
        }

        public ITriggeredEmail Create(string name, ICommunicationLogger logger = null)
        {
            return Brierley.FrameWork.Email.TriggeredEmailFactory.Create(name, logger);
        }

        public ITriggeredEmail Create(long id, ICommunicationLogger logger = null)
        {
            return Brierley.FrameWork.Email.TriggeredEmailFactory.Create(id, logger);
        }
    }
}
