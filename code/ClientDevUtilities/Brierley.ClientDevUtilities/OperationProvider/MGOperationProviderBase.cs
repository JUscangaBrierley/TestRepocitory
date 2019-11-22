using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.LoyaltyWare.LWMobileGateway.OperationProviders;

namespace Brierley.ClientDevUtilities.OperationProvider
{
    public abstract class MGOperationProviderBase : OperationProviderBase
    {
        private ILWDataServiceUtil _lwDataServiceUtil;
        private IDataService _dataService = null;
        private ILoyaltyDataService _loyaltyService = null;
        private IContentService _contentService = null;

        public MGOperationProviderBase(string name, ILWDataServiceUtil lwDataServiceUtil)
            : base(name)
        {
            _lwDataServiceUtil = lwDataServiceUtil;
        }

        protected new IDataService DataService
        {
            get
            {
                if (_dataService == null)
                {
                    _dataService = _lwDataServiceUtil.DataServiceInstance();
                }
                return _dataService;
            }
        }

        protected new ILoyaltyDataService LoyaltyService
        {
            get
            {
                if (_loyaltyService == null)
                {
                    _loyaltyService = _lwDataServiceUtil.LoyaltyDataServiceInstance();
                }
                return _loyaltyService;
            }
        }

        protected new IContentService ContentService
        {
            get
            {
                if (_contentService == null)
                {
                    _contentService = _lwDataServiceUtil.ContentServiceInstance();
                }
                return _contentService;
            }
        }

        protected override void Cleanup()
        {
            if (_dataService != null)
            {
                _dataService.Dispose();
            }

            if (_contentService != null)
            {
                _contentService.Dispose();
            }

            if (_loyaltyService != null)
            {
                _loyaltyService.Dispose();
            }

            base.Cleanup();
        }
    }
}
