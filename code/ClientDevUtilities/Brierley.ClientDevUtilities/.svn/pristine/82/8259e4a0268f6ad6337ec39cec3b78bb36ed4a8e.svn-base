using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
using Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders;

namespace Brierley.ClientDevUtilities.OperationProvider
{
    public abstract class CDISOperationProviderBase : OperationProviderBase
    {
        private ILWDataServiceUtil _lwDataServiceUtil;
        private IDataService _dataService;
        private IContentService _contentService;
        private ILoyaltyDataService _loyaltyDataService;

        public CDISOperationProviderBase(string name, ILWDataServiceUtil lwDataServiceUtil)
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

        protected new ILoyaltyDataService LoyaltyDataService
        {
            get
            {
                if (_loyaltyDataService == null)
                {
                    _loyaltyDataService = _lwDataServiceUtil.LoyaltyDataServiceInstance();
                }
                return _loyaltyDataService;
            }
        }

        public virtual new Member LoadMember(APIArguments args, string memberIdentityName = "MemberIdentity", string searchTypeName = "MemberSearchType", string searchValueName = "SearchValue")
        {
            return base.LoadMember(args, memberIdentityName, searchTypeName, searchValueName);
        }

        public virtual APIArguments DeserializeRequest(string payload)
        {
            return SerializationUtils.DeserializeRequest(Name, Config, payload);
        }

        public virtual string SerializeResult(APIArguments resultParams)
        {
            return SerializationUtils.SerializeResult(Name, Config, resultParams);
        }

        public virtual string GetFunctionParameter(string parmName)
        {
            return base.GetFunctionParameter(parmName);
        }

        public virtual new NameValueCollection FunctionProviderParms
        {
            get
            {
                return base.FunctionProviderParms;
            }
        }

        public virtual new  LWIntegrationDirectives Config
        {
            get
            {
                return base.Config;
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

            if (_loyaltyDataService != null)
            {
                _loyaltyDataService.Dispose();
            }

            base.Cleanup();
        }
    }
}
