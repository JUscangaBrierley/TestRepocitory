using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.LWGateway.PetaPocoGateway;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public class ContentService : Brierley.FrameWork.Data.ContentService, IContentService
    {
        public ContentService(Brierley.FrameWork.Data.ServiceConfig config)
            : base(config) { }

        public new IDatabase Database => new Database(base.Database);
    }
}
