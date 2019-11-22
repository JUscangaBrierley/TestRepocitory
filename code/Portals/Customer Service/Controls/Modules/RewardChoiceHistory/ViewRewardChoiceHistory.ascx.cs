using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brierley.FrameWork.Data;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;

namespace LoyaltyWareWebsite.Controls.Modules.RewardChoiceHistory
{
	public partial class ViewRewardChoiceHistory : ModuleControlBase
	{
		private class History
		{
			public DateTime Date { get; set; }
			public string Reward { get; set; }
			public string ChangedBy { get; set; }

			public History()
			{
			}
		}

		const int _resultsPerPage = 10;

		protected void Page_Load(object sender, EventArgs e)
		{
			grdHistory.PageIndexChanging += GrdHistory_PageIndexChanging;

			if(PortalState.CurrentMember == null)
			{
				this.Visible = false;
				return;
			}

			//hide changed by column if this is being shown in customer facing mode. Only CS agents can see who changed a choice.
			if(PortalState.Portal.PortalMode == PortalModes.CustomerFacing)
			{
				grdHistory.Columns[2].Visible = false;
			}

			if (!IsPostBack)
			{
				BindGrid();
			}
		}
		
		private void BindGrid()
		{
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				var choices = loyalty.GetRewardChoiceHistory(PortalState.CurrentMember.IpCode, grdHistory.PageIndex + 1, _resultsPerPage);

				int index = Convert.ToInt32(choices.CurrentPage - 1) * _resultsPerPage;
				var history = new History[Convert.ToInt32(choices.TotalItems)];
				foreach (var choice in choices.Items)
				{
					var h = new History() { Date = choice.CreateDate, ChangedBy = choice.ChangedBy, Reward = choice.RewardId.ToString() };
					var reward = content.GetRewardDef(choice.RewardId);
					if(reward != null)
					{
						h.Reward = PortalState.Portal.PortalMode == PortalModes.CustomerFacing ? reward.DisplayName : reward.Name;
					}
					history[index++] = h;
				}

				grdHistory.DataSource = history;
				grdHistory.DataBind();
			}
		}

		private void GrdHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			grdHistory.PageIndex = e.NewPageIndex;
			BindGrid();
		}
	}
}