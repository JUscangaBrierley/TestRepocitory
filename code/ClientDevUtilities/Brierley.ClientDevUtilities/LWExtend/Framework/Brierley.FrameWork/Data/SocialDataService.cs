using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Data
{
	public class SocialDataService : ServiceBase
	{
		
		private SocialCampaignDataDao _dataDao = null;

        //LW-3715 Remove Mutual Minds references


        public SocialCampaignDataDao SocialCampaignDataDao
		{
			get
			{
				if (_dataDao == null)
				{
					_dataDao = new SocialCampaignDataDao(Database, Config);
				}
				return _dataDao;
			}
		}

		internal SocialDataService(ServiceConfig config)
			: base(config)
		{
		}

        public List<SocialMediaApp> GetSocialMediaApps()
        {
            using (var data = LWDataServiceUtil.DataServiceInstance(Config.Organization, Config.Environment))
            {
                string json = data.GetClientConfigProp("SocialMediaApps");
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return JsonConvert.DeserializeObject<List<SocialMediaApp>>(json);
                }
                return new List<SocialMediaApp>();
            }
        }

		//LW-3715 Remove Mutual Minds references

		public void CreateSocialCampaignData(SocialCampaignData data)
		{
			SocialCampaignDataDao.Create(data);
		}

		public void CreateSocialCampaignData(IEnumerable<SocialCampaignData> dataList)
		{
			SocialCampaignDataDao.Create(dataList);
		}
	}
}
