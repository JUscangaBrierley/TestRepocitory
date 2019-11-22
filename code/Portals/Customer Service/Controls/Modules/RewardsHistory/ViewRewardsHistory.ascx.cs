using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.FrameWork.Email;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal;
using System.Web.UI;

namespace Brierley.LWModules.RewardsHistory
{
	public partial class ViewRewardsHistory : ModuleControlBase, IIpcEventHandler
	{
		#region Fields
		private const string _className = "ViewRewardsHistory";
		private const string _modulePath = "~/Controls/Modules/RewardsHistory/ViewRewardsHistory.ascx";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private RewardsHistoryConfig _config = null;
		protected AspDynamicGrid _grid = null;
		private IDynamicGridProvider _provider = null;
		private const int _maxDateRanges = 500;
		#endregion

		#region Page Lifecycle

		protected override void OnInit(EventArgs e)
		{
			string methodName = "OnInit";
			try
			{
				IpcManager.RegisterEventHandler("RefreshRewards", this, false);
				_config = ConfigurationUtil.GetConfiguration<RewardsHistoryConfig>(ConfigurationKey) ?? new RewardsHistoryConfig();
				_provider= (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
				if (_provider== null)
				{
					string errMsg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "CantCreateProvider.Text", "Unable to create the provider assembly {0} and class {1}"),
						_config.ProviderAssemblyName, _config.ProviderClassName);
					_logger.Error(_className, methodName, errMsg);
					throw new Exception(errMsg);
				}
				else
				{
                    _provider.ParentControl = "~/Controls/Modules/RewardsHistory/ViewRewardsHistory.ascx";

					lblNoResults.Text = StringUtils.FriendlyString(ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey), "No records to display.");
					h2Title.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.TitleResourceKey);

					_logger.Debug(_className, methodName, "Provider successfully created for CSRewardsHistory.");
					var member = PortalState.CurrentMember;

					if (member == null)
					{
						_logger.Error(_className, methodName, "No member selected.");
						this.Visible = false;
						return;
					}
					else
					{
						_provider.SetSearchParm("Member", member);
                        _provider.SetSearchParm("Config", _config);
						_provider.FilteringEnabled = _config.EnableGridFiltering;
					}

					_grid = new AspDynamicGrid() { AutoRebind = false, EnableViewState = false, Title = string.Empty, CreateTopPanel = false };
					((AspGridProviderBase)_provider).ValidationGroup = ValidationGroup;
					((AspGridProviderBase)_provider).ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
					_grid.Provider = _provider;
                    _grid.GridActionClicked += new AspDynamicGrid.GridActionClickedHandler(GridActionClickedHandler);
                    lnkCancelOk.Click += new EventHandler(lnkCancelOk_Click);
                    lnkCancelCancel.Click += new EventHandler(lnkCancelCancel_Click);

					if (!IsPostBack)
					{
						PopulateDateControls(_config.DateDisplayType, _config.MinimumDateRange, _config.DatesToDisplay);
					}

					phRewardsHistory.Controls.Add(_grid);

					reqFromDate.ValidationGroup = reqToDate.ValidationGroup = btnSearch.ValidationGroup = ValidationGroup;
                    customToDateValidator.ValidationGroup = btnSearch.ValidationGroup = ValidationGroup;
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected Exception", ex);
				throw;
			}
		}
                        
		protected void Page_Load(object sender, EventArgs e)
		{
			if (_grid != null)
			{
				_grid.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
				_grid.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
				_grid.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
			}
			RebindGrid();
		}

		#endregion

        #region Reward Cancellation

        protected long SelectedRewardId
        {
            get
            {
                if (ViewState["SelectedRewardId"] != null)
                {
                    return (long)ViewState["SelectedRewardId"];
                }
                return 0;
            }
            set
            {
                ViewState["SelectedRewardId"] = value;
            }
        }

        void lnkCancelCancel_Click(object sender, EventArgs e)
        {
            SelectedRewardId = 0;
            lblOrderNumber.Text = string.Empty;
            pnlCancelOrder.Visible = false;
        }

        void lnkCancelOk_Click(object sender, EventArgs e)
        {
            MemberReward reward = LoyaltyService.GetMemberReward(SelectedRewardId);
            string notes = txtNotes.Text;
            MemberRewardsUtil.CancelOrReturnMemberReward(reward, true, notes);
            SelectedRewardId = 0;
            lblOrderNumber.Text = string.Empty;
            pnlCancelOrder.Visible = false;
            _grid.Rebind();
        }

        void GridActionClickedHandler(object sender, GridActionClickedArg e)
        {
            string methodName = "GridActionClickedHandler";
            if (e.Key != null && e.CommandName == "Cancel Reward")
            {
                long rewardId = long.Parse(e.Key.ToString());
                SelectedRewardId = rewardId;
                MemberReward reward = LoyaltyService.GetMemberReward(rewardId);
                RewardDef def = ContentService.GetRewardDef(reward.RewardDefId);
                lblCancelRewardTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, "lblCancelling.Text", "Cancelling") + " " + def.Name;
                if (!string.IsNullOrEmpty(reward.LWOrderNumber))
                {
                    lblOrderNumber.Text = reward.LWOrderNumber;
                }
                lblRewardName.Text = def.Name;
                pnlCancelOrder.Visible = true;
            }
            else if (e.Key != null && e.CommandName == "Resend Email")
            {
                long rewardId = long.Parse(e.Key.ToString());
                SelectedRewardId = rewardId;
                MemberReward reward = LoyaltyService.GetMemberReward(rewardId);
                RewardDef def = ContentService.GetRewardDef(reward.RewardDefId);
                IList<EmailAssociation> list = EmailService.GetEmailAssociations(PointTransactionOwnerType.Reward, reward.RewardDefId, reward.Id);
                if (list.Count > 0)
                {
                    EmailAssociation emailAss = list[0];
                    EmailQueue emailQueue = EmailService.GetEmailQueue(emailAss.EmailQueueId);
                    if (emailQueue == null)
                    {
                        string msg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "EmailNotFound.Text","Unable to retrieve email queue entry with Id {0}."), emailAss.EmailQueueId);
                        _logger.Error(_className, methodName, string.Format("Unable to retrieve email queue entry with Id {0}.", emailAss.EmailQueueId));
                        ShowNegative(msg);
                        return;
                    }
                    using (DmcTriggeredEmailRetry email = new DmcTriggeredEmailRetry(emailQueue.EmailID))
                    {
                        try
                        {
                            if (email.Resend(emailAss.EmailQueueId))
                            {
                                string msg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "EmailResendSuccess.Text","Successfully resent email."));
                                _logger.Trace(_className, methodName, "Successfully resent email.");
                                ShowPositive(msg);
                            }
                            else
                            {
                                string msg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "EmailResendFailed.Text", "Unable to resend email."));
                                _logger.Error(_className, methodName, "Unable to resend email.");
                                ShowNegative(string.Format(msg));
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            string msg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "EmailResendError.Text", "Error resending email."));
                            _logger.Error(_className, methodName, "Error resending email.", ex);
                            ShowNegative(string.Format(msg, ex.Message));
                            return;
                        }
                    }
                    // Insert the CSNote if necessary
                    if (_config.InsertCSNoteOnResend)
                    {
                        _logger.Debug(_className, methodName, "Inserting CS Note for email resend.");
                        CSAgent agent = PortalState.GetLoggedInCSAgent();
                        CSNote note = new CSNote()
                        {
                            MemberId = PortalState.CurrentMember.IpCode,
                            Note = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "EmailResending.Text", "Email being resent for reward {0}"), def.Name),
                            CreatedBy = agent.Id
                        };
                        CSService.CreateNote(note);

						// Notify that member was updated so CSNote grid will rebind
						IpcManager.PublishEvent("MemberUpdated", ConfigurationKey, PortalState.CurrentMember);
                    }
                }
            }
        }
        #endregion

        private void RebindGrid()
		{
			switch (_config.DateDisplayType)
			{
				case DateDisplayTypes.None:
					break;
				case DateDisplayTypes.TextBoxRange:
					_grid.SetSearchParm("FromDate", dpFromDate.SelectedDate);
					_grid.SetSearchParm("ToDate", dpToDate.SelectedDate);
					break;
				case DateDisplayTypes.DropDownByMonth:
				case DateDisplayTypes.DropDownByQuarter:
				case DateDisplayTypes.DropDownByWeek:
				case DateDisplayTypes.DropDownByYear:
					DateTime start = DateTime.Today;
					DateTime end = DateTime.Today;
					if (!string.IsNullOrEmpty(ddlDateRange.SelectedValue))
					{
						var split = ddlDateRange.SelectedValue.Split('|');
						if (split.Length == 2 && DateTime.TryParse(split[0], out start) && DateTime.TryParse(split[1], out end))
						{
							_grid.SetSearchParm("FromDate", start);
							_grid.SetSearchParm("ToDate", end);
						}
					}
					break;
			}

			_grid.Rebind();
			lblNoResults.Visible = _provider != null && _provider.GetNumberOfRows() < 1;
		}



		#region Date Helpers
		// These methods have been copied from ViewAccountActivity.  They should probably be factored
		// into common controls.
		private List<KeyValuePair<DateTime, DateTime>> GetWeeks(DateTime? minDate, int? datesToDisplay)
		{
			var ret = new List<KeyValuePair<DateTime, DateTime>>();

			DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
			DateTime start = DateTime.Today;
			while (start.DayOfWeek != firstDayOfWeek)
			{
				start = start.AddDays(-1);
			}

			while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
			{
				ret.Add(new KeyValuePair<DateTime, DateTime>(start, start.AddDays(6)));
				start = start.AddDays(-7);
			}
			return ret;
		}

		private List<KeyValuePair<DateTime, DateTime>> GetQuarters(DateTime? minDate, int? datesToDisplay)
		{
			var ret = new List<KeyValuePair<DateTime, DateTime>>();
			int quarterStartMonth = 0;
			switch (DateTime.Today.Month)
			{
				case 1:
				case 2:
				case 3:
					quarterStartMonth = 1;
					break;
				case 4:
				case 5:
				case 6:
					quarterStartMonth = 4;
					break;
				case 7:
				case 8:
				case 9:
					quarterStartMonth = 7;
					break;
				case 10:
				case 11:
				case 12:
					quarterStartMonth = 10;
					break;
			}
			DateTime start = new DateTime(DateTime.Today.Year, quarterStartMonth, 1);

			while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
			{
				DateTime end = start.AddMonths(3).AddDays(-1);
				ret.Add(new KeyValuePair<DateTime, DateTime>(start, end));
				start = start.AddMonths(-3);
			}
			return ret;

		}

		private List<KeyValuePair<DateTime, DateTime>> GetMonths(DateTime? minDate, int? datesToDisplay)
		{
			var ret = new List<KeyValuePair<DateTime, DateTime>>();

			DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

			while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
			{
				DateTime end = start.AddMonths(1).AddDays(-1);
				ret.Add(new KeyValuePair<DateTime, DateTime>(start, end));
				start = start.AddMonths(-1);
			}
			return ret;

		}

		private List<KeyValuePair<DateTime, DateTime>> GetYears(DateTime? minDate, int? datesToDisplay)
		{
			var ret = new List<KeyValuePair<DateTime, DateTime>>();

			DateTime start = new DateTime(DateTime.Today.Year, 1, 1);

			while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
			{
				DateTime end = new DateTime(start.Year, 12, 31);
				ret.Add(new KeyValuePair<DateTime, DateTime>(start, end));
				start = start.AddYears(-1);
			}
			return ret;
		}

		private void PopulateDateControls(DateDisplayTypes displayType, DateTime? minimumDateRange, int? datesToDisplay)
		{
			if (_config.DateDisplayType == DateDisplayTypes.None)
			{
				pchDateTextBox.Visible = false;
				pchDateRange.Visible = false;
			}
			else if (_config.DateDisplayType == DateDisplayTypes.TextBoxRange)
			{
				pchDateTextBox.Visible = true;
				pchDateRange.Visible = false;
				var startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
				var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
				dpFromDate.SelectedDate = startdate;
				dpToDate.SelectedDate = endDate;
				_grid.SetSearchParm("FromDate", dpFromDate.SelectedDate);
				_grid.SetSearchParm("ToDate", dpToDate.SelectedDate.Value);
			}
			else
			{
				pchDateTextBox.Visible = false;
				pchDateRange.Visible = true;

				List<KeyValuePair<DateTime, DateTime>> dates = null;
				switch (_config.DateDisplayType)
				{
					case DateDisplayTypes.DropDownByWeek:
						dates = GetWeeks(minimumDateRange, datesToDisplay);
						break;
					case DateDisplayTypes.DropDownByMonth:
						dates = GetMonths(minimumDateRange, datesToDisplay);
						break;
					case DateDisplayTypes.DropDownByQuarter:
						dates = GetQuarters(minimumDateRange, datesToDisplay);
						break;
					case DateDisplayTypes.DropDownByYear:
						dates = GetYears(minimumDateRange, datesToDisplay);
						break;
				}


				foreach (var range in dates)
				{
					ListItem li = new ListItem();
					li.Value = range.Key.ToShortDateString() + "|" + range.Value.ToShortDateString();
					switch (displayType)
					{
						case DateDisplayTypes.DropDownByWeek:
						case DateDisplayTypes.DropDownByQuarter:
							li.Text = range.Key.ToShortDateString() 
								+ " " + ResourceUtils.GetLocalWebResource(_modulePath, "lblTo.Text") 
								+ " " + range.Value.ToShortDateString();
							break;
						case DateDisplayTypes.DropDownByYear:
							li.Text = range.Key.Year.ToString();
							break;
						case DateDisplayTypes.DropDownByMonth:
							li.Text = range.Key.ToString("MMMM yyyy");
							break;
					}

					ddlDateRange.Items.Add(li);
				}

				var startdate = dates[0].Key;
				var endDate = dates[0].Value;
				_grid.SetSearchParm("FromDate", startdate);
				_grid.SetSearchParm("ToDate", endDate);

			}

		}
		#endregion


		#region IPC Handler

		public ModuleConfigurationKey GetConfigurationKey()
		{
			return ConfigurationKey;
		}

		public void HandleEvent(IpcEventInfo info)
		{
			string methodName = "HandleEvent";

			_logger.Debug(_className, methodName, "Handling event " + info.EventName);
			if (info.EventName == "RefreshRewards")
			{
				_logger.Debug(_className, methodName, "Refreshing rewards.");
				if (_grid != null)
				{
					_grid.Rebind();
                    lblNoResults.Visible = _provider != null && _provider.GetNumberOfRows() < 1;
				}
			}
		}
		#endregion

	}
}
