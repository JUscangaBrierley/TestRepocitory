using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace LoyaltyWareWebsite.Controls.Modules.RewardChoice
{
	public partial class ViewRewardChoice : ModuleControlBase
	{
		private const string _className = "ViewRewardChoice";
		private const string _modulePath = "~/Controls/Modules/Messages/ViewMessages.ascx";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private Member _member = null;

		private long _pointBalance = 0;

		protected long PointBalance
		{
			get
			{
				return _pointBalance;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			litSuccess.Visible = false;
			litModuleUnavailable.Visible = false;
			btnSave.Click += BtnSave_Click;
			LoadRewardChoices();
		}
		
		private bool LoadRewardChoices()
		{
			const string methodName = "LoadRewardChoices";
			litModuleUnavailable.Visible = false;

			//for now, there are no specific error messages that give a reason for the module being unavailable, just a generic one
			//var warnings = new List<string>();
			//Action showWarnings = () =>
			//{
			//	if (warnings.Count == 0)
			//	{
			//		return;
			//	}
			//	string message = string.Join("<br/><br/>", warnings);
			//	ErrorPanel.ShowWarning(message);
			//};

			Action<string> moduleUnavailable = (string message) =>
			{
				_logger.Warning(_className, methodName, message);
				litModuleUnavailable.Visible = true;

				litModuleUnavailable.Text += message;
			};

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				_member = PortalState.CurrentMember;
				if (_member == null)
				{
					moduleUnavailable("PortalState.CurrentMember is null.");
					return false;
				}

				//member must have a tier
				var tier = _member.GetTier(DateTime.Now);
				if (tier == null)
				{
					moduleUnavailable("Member has no tier.");
					return false;
				}

				//must have rewards
				var rewards = content.GetRewardDefsByTier(tier.TierDefId).Where(o => o.Active && o.CatalogStartDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.Now && o.CatalogEndDate.GetValueOrDefault(DateTime.MaxValue) > DateTime.Now);
				if (rewards == null || rewards.Count() == 0)
				{
                    moduleUnavailable(string.Format("No rewards exist for tier {0}", tier.TierDef != null ? tier.TierDef.Name : tier.TierDefId.ToString()));
					return false;
				}

				var choice = loyalty.GetCurrentRewardChoice(_member.IpCode);

				if (choice != null)
				{
					//make sure the member can still receive this reward (tier hasn't changed)
					//if this choice is invalid, we'll use the default choice instead
					if (rewards.Count(o => o.Id == choice.RewardId) == 0)
					{
						choice = null;
					}
				}

				if (choice == null)
				{
					//Use default reward choice of member's tier. Better to show the default choice as the member's selection? This shows
					//the member which reward they will recieve, which may prevent any surprises when they earn it; maybe better than showing 
					//the member that they have no selection and then later give them a frosty because it's the default and they made no choice.
					//"If you choose not to decide, you still have made a choice"
					var t = loyalty.GetTierDef(tier.TierDefId);
					if (t.DefaultRewardId.HasValue)
					{
						choice = new Brierley.FrameWork.Data.DomainModel.RewardChoice(_member.IpCode, t.DefaultRewardId.Value);
					}
				}

				//build selection
				pchRewardList.Controls.Clear();
				string contentUrl = LWConfigurationUtil.GetConfigurationValue("LWContentRootURL");
				if (!contentUrl.EndsWith("/"))
				{
					contentUrl += "/";
				}
				contentUrl += LWConfigurationUtil.GetCurrentEnvironmentContext().Organization.Replace(" ", string.Empty) + "/";

				foreach (var r in rewards.OrderBy(o => o.Name))
				{
					var div = new HtmlGenericControl("div");
					div.Attributes.Add("class", "reward");
					div.Attributes.Add("data-display-name", r.DisplayName);
					div.Attributes.Add("data-image", (r.MediumImageFile.Contains("://") ? string.Empty : contentUrl) + (r.LargeImageFile ?? r.SmallImageFile ?? r.MediumImageFile));
					div.Attributes.Add("data-legal-text", r.LegalText);
					div.Attributes.Add("data-long-description", r.LongDescription);
					div.Attributes.Add("data-short-description", r.ShortDescription);
					div.Attributes.Add("data-points", r.HowManyPointsToEarn.ToString());

					var logo = new HtmlGenericControl("div");
					logo.Attributes.Add("class", "logo");
					var img = new HtmlImage() { Src = (r.MediumImageFile.Contains("://") ? string.Empty : contentUrl) + r.MediumImageFile };
					logo.Controls.Add(img);
					div.Controls.Add(logo);

					var name = new HtmlGenericControl("div");
					name.Attributes.Add("class", "name");

					var p = new HtmlGenericControl("p");
					p.Attributes.Add("class", "display-name");
					p.InnerHtml = r.DisplayName;
					name.Controls.Add(p);
					
					div.Controls.Add(name);

					var radio = new HtmlGenericControl("input");
					radio.Attributes.Add("type", "radio");
					radio.Attributes.Add("id", "rdo_" + r.Id.ToString());
					radio.Attributes.Add("value", r.Id.ToString());
					radio.Attributes.Add("name", "rdoRewardChoice");
					if (choice != null && choice.RewardId == r.Id)
					{
						radio.Attributes.Add("checked", "checked");
					}
					div.Controls.Add(radio);

					pchRewardList.Controls.Add(div);
				}

				_pointBalance = Convert.ToInt64(_member.GetPoints(null, null));
			}
			return true;
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			var choice = Request.Form["rdoRewardChoice"];
			if (!string.IsNullOrEmpty(choice))
			{
				long rewardId = 0;
				if (long.TryParse(choice, out rewardId))
				{
					using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
					{
                        string changedBy = null;
                        if(PortalState.Portal.PortalMode == PortalModes.CustomerService && PortalState.IsCSAgentLoggedIn())
                        {
                            changedBy = PortalState.GetLoggedInCSAgent().Username;
                        }
						loyalty.SetMemberRewardChoice(_member, rewardId, changedBy);
					}
				}
			}

			var config = ConfigurationUtil.GetConfiguration<RewardChoiceConfig>(base.ConfigurationKey);
			if (config != null && !string.IsNullOrEmpty(config.RedirectUrl))
			{
				Response.Redirect(config.RedirectUrl, false);
			}
			else
			{
				litSuccess.Visible = true;
				LoadRewardChoices();
			}
		}
	}
}