using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.LWGateway.DataAccess;
using Brierley.ClientDevUtilities.LWGateway.PetaPocoGateway;
using Brierley.FrameWork;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public class DataService : Brierley.FrameWork.Data.DataService, IDataService
    {
        private IClientConfigurationDao _clientConfigDao;
        private IContactHistoryDao _contactHistoryDao;

        public DataService(Brierley.FrameWork.Data.ServiceConfig config)
            : base(config) { }

        public new IDatabase Database => new Database(base.Database);

        public new IClientConfigurationDao ClientConfigurationDao
        {
            get
            {
                if (_clientConfigDao == null)
                {
                    _clientConfigDao = new ClientConfigurationDao(base.Database, Config);
                }
                return _clientConfigDao;
            }

        }

        //If CSService becomes available, currently its one costructor is internal, then this Dao can be moved under it
        public IContactHistoryDao ContactHistoryDao
        {
            get
            {
                if(_contactHistoryDao == null)
                {
                    _contactHistoryDao = new ContactHistoryDao(base.Database, Config);
                }
                return _contactHistoryDao;
            }
        }
    }
}
