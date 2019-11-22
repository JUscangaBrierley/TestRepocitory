using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class SocialCampaignDataDao : DaoBase<SocialCampaignData>
	{
		public SocialCampaignDataDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}
	}
}
