using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.LWGateway.DataAccess;
using Brierley.ClientDevUtilities.LWGateway.PetaPocoGateway;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public class LoyaltyDataService : Brierley.FrameWork.Data.LoyaltyDataService, ILoyaltyDataService
    {
        private ILWEventDao _eventDao;
        private IMemberDao _memberDao;
        private IPointTransactionDao _pointTransactionDao;

        public LoyaltyDataService(Brierley.FrameWork.Data.ServiceConfig config)
            : base(config) { }

        public new IDatabase Database => new Database(base.Database);

        public new ILWEventDao LWEventDao
        {
            get
			{
                if (_eventDao == null)
				{
                    _eventDao = new LWEventDao(base.Database, Config);
				}
                return _eventDao;
			}
        }

        public new IMemberDao MemberDao
        {
            get
            {
                if (_memberDao == null)
                {
                    _memberDao = new MemberDao(base.Database, Config);
                }
                return _memberDao;
            }
        }

        public new IPointTransactionDao PointTransactionDao
        {
            get
            {
                if (_pointTransactionDao == null)
                {
                    _pointTransactionDao = new PointTransactionDao(base.Database, Config);
                }
                return _pointTransactionDao;
            }
        }
    }
    
}
