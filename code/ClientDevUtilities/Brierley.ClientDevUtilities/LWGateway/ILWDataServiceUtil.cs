using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.Config;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public interface ILWDataServiceUtil
    {
        IContentService ContentServiceInstance();
        IContentService ContentServiceInstance(LWConfiguration config);
        IContentService ContentServiceInstance(string orgName, string envName);
        ILoyaltyDataService LoyaltyDataServiceInstance();
        ILoyaltyDataService LoyaltyDataServiceInstance(LWConfiguration config);
        ILoyaltyDataService LoyaltyDataServiceInstance(string orgName, string envName);
        IDataService DataServiceInstance();
        IDataService DataServiceInstance(LWConfiguration config);
        IDataService DataServiceInstance(string orgName, string envName);
        IEmailService EmailServiceInstance();
        IEmailService EmailServiceInstance(LWConfiguration config);
        IEmailService EmailServiceInstance(string orgName, string envName);
        ISurveyManager SurveyManagerInstance();
        ISurveyManager SurveyManagerInstance(LWConfiguration config);
        ISurveyManager SurveyManagerInstance(string orgName, string envName);
    }
}
