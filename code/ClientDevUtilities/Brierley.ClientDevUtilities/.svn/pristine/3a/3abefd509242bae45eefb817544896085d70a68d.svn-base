using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.CampaignManagement.DataProvider;

namespace Brierley.FrameWork.Data
{
	public class CampaignConfig
	{
		/// <summary>
		/// gets or sets the default mode for executing campaigns (foreground or via job scheduler)
		/// </summary>
		public ExecutionTypes ExecutionType { get; set; }

		/// <summary>
		/// gets or sets whether scheduled immediate jobs will send an email to the user who initiated execution.
		/// </summary>
		/// <remarks>
		/// this value is global to each client and must be set through the DBConfig screen. It will only cause an email
		/// to be sent if campaing execution mode is set to scheduled.
		/// </remarks>
		public bool SendExecutionEmail { get; set; }

		public CampaignDataProvider BatchProvider { get; set; }

		public CampaignDataProvider RealTimeProvider { get; set; }
	}
}
