using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.FrameWork.Rules;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.RewardAppeasement
{
	public partial class ViewRewardAppeasement : ModuleControlBase
	{
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private const string _className = "ViewRewardAppeasement";
		private ContextObject _co = null;
		private RewardAppeasementConfig _config = null;


		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";

			btnSave.Click += new EventHandler(btnSave_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
            pchSuccessMessage.Visible = false;
			try
			{
				_config = ConfigurationUtil.GetConfiguration<RewardAppeasementConfig>(ConfigurationKey);
				if (_config == null)
				{
					_logger.Error(_className, methodName, "Missing configuration.");
					this.Visible = false;
					return;
				}

				reqReward.ValidationGroup = btnSave.ValidationGroup = ValidationGroup;

				if (!IsPostBack)
				{
                    h2Title.Text = (GetLocalResourceObject(_config.ModuleTitle) ?? _config.ModuleTitle).ToString(); //_config.ModuleTitle;
					pchAdditionalNotes.Visible = _config.DisplayNotes;
					BindRewardList();
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
		}


		protected void btnSave_Click(object sender, EventArgs e)
		{
			const string methodName = "btnSave_Click";
			try
			{
				var member = PortalState.CurrentMember;
				if (member == null)
				{
                    //ShowMessage("No member is selected.");
                    string errMsg = string.Format(GetLocalResourceObject("NoCurrentMember.Text").ToString());
                    _logger.Error(_className, methodName, errMsg);
                    ShowNegative(errMsg);
                    return;
				}

                if (member.MemberStatus == MemberStatusEnum.PreEnrolled)
                {
                    if (!_config.AllowPreEnrolled)
                    {
                        string errMsg = string.Format(GetLocalResourceObject("PreEnrolledError.Text").ToString());
                        _logger.Error(_className, methodName, errMsg);
                        ShowNegative(errMsg);
                        return;
                    }
                }
                else if (member.MemberStatus != MemberStatusEnum.Active)
                {
                    string errMsg = string.Format(GetLocalResourceObject("MemberNotActive.Text").ToString());
                    _logger.Error(_className, methodName, errMsg);
                    ShowNegative(errMsg);
                    return;
                }

				RewardDef reward = ContentService.GetRewardDef(long.Parse(ddlReward.SelectedValue));
				if (reward == null)
				{
                    //throw new Exception("Unable to locate this reward.");
                    string errMsg = string.Format(GetLocalResourceObject("RewardNotFound.Text").ToString());
                    _logger.Error(_className, methodName, errMsg);
                    ShowNegative(errMsg);
                    return;
				}

				reward.Product = reward.Product ?? ContentService.GetProduct(reward.ProductId);
				string businessRuleName = GetBusinessRuleName(reward, reward.Product);
				if (string.IsNullOrEmpty(businessRuleName))
				{
					throw new Exception(string.Format(GetLocalResourceObject("RuleNotFound.Text").ToString(), reward.Name));
				}

				RuleTrigger rt = LoyaltyService.GetRuleByName(businessRuleName);
				if (rt == null)
				{
					throw new Exception(string.Format(GetLocalResourceObject("RuleLoadFailed.Text").ToString(), businessRuleName, reward.Name));
				}
                //var rule = (RewardCatalogIssueReward)rt.Rule;

                //if (rule.FulfillmentOption == RewardFulfillmentOption.ThirdParty)
                //{
                //    throw new Exception("Cannot issue appeasements that use a fulfillment provider.");
                //}
                RewardFulfillmentOption fulfillmentOption = MemberRewardsUtil.GetFulfillmentOption(rt);
                if (fulfillmentOption == RewardFulfillmentOption.ThirdParty)
                {
                    //throw new Exception("Cannot issue appeasements that use a fulfillment provider.");
                    string errMsg = string.Format(GetLocalResourceObject("NonAppeasementFulfillmentProvider.Text").ToString());
                    _logger.Error(_className, methodName, errMsg);
                    ShowNegative(errMsg);
                    return;
                }
				_co = new ContextObject();
				_co.Owner = member;

				var env = new Dictionary<string, object>();
				env.Add("RewardName", reward.Name);
                string changedBy = string.Empty;
                CSAgent agent = PortalState.GetLoggedInCSAgent();
                if (agent != null)
                {
                    env.Add("ChangedBy", agent.Username);                    
                }
				_co.Environment = env;

				LoyaltyService.Execute(rt, _co);

                if (_co.Results != null && _co.Results.Count > 0)
                {
                    foreach (ContextObject.RuleResult result in _co.Results)
                    {
                        if (result.SkipReason != null)
                        {
                            ShowNegative(string.Format(GetLocalResourceObject("RuleSkippedReason.Text").ToString(), rt.RuleName, result.SkipReason.Value.ToString()));
                            return;
                        }
                    }
                }

				if (!string.IsNullOrEmpty(_config.SuccessMessage))
				{
					if (_config.SuccessMessage.Contains("##"))
					{
						litSuccessMessage.Text = ParseExpressions(_config.SuccessMessage);
					}
					else
					{
						litSuccessMessage.Text = _config.SuccessMessage;
					}
					pchSuccessMessage.Visible = true;
				}

				//var agent = PortalState.GetLoggedInCSAgent();

				if (!string.IsNullOrEmpty(_config.GlobalNoteFormat))
				{
					var note = new CSNote();
					note.Note = string.Format(_config.GlobalNoteFormat, reward.Name);
					note.MemberId = member.IpCode;
					note.CreatedBy = agent.Id;
					CSService.CreateNote(note);
				}

				if (PortalState.Portal.PortalMode == PortalModes.CustomerService && _config.DisplayNotes && txtNotes.Text.Trim() != string.Empty)
				{
					CSNote note = new CSNote() { Note = txtNotes.Text.Trim() };
					note.MemberId = member.IpCode;
					note.CreatedBy = agent.Id;
					CSService.CreateNote(note);
				}
				txtNotes.Text = string.Empty;
				ddlReward.SelectedIndex = 0;

				IpcManager.PublishEvent("MemberUpdated", ConfigurationKey, member);
                IpcManager.PublishEvent("RefreshRewards", ConfigurationKey, member);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
            litSuccessMessage.Text = string.Empty;
            pchSuccessMessage.Visible = false;
            ddlReward.SelectedIndex = 0;
            txtNotes.Text = string.Empty;
		}


		private void BindRewardList()
		{
			const string methodName = "BindRewardList";
			try
			{
				ddlReward.Items.Clear();

				var searchParms = new List<Dictionary<string, object>>();
				string errMsg = string.Empty;
				for (int i = 0; i < _config.RewardFilters.Count; i++)
				{
					var filter = _config.RewardFilters[i];

					if (string.IsNullOrEmpty(filter.Property) || string.IsNullOrEmpty(filter.Value))
					{
						continue;
					}

					object searchValue = null;

					if (filter.Property == "HowManyPointsToEarn")
					{
						searchValue = long.Parse(filter.Value);
					}
                    //else if (filter.Property == "Active")
                    //{
                    //    searchValue = bool.Parse(filter.Value);
                    //}
					else
					{
						DateTime dt = DateTimeUtil.MinValue;
						if (DateTime.TryParse(filter.Value, out dt))
						{
							searchValue = dt;
						}
					}
					if (searchValue == null)
					{
						searchValue = filter.Value;
					}

					var dictionary = new Dictionary<string, object>();
					if (filter.IsAttribute)
					{
						dictionary.Add("IsAttribute", true);
					}
					if (!string.IsNullOrEmpty(filter.Operator))
					{
						dictionary.Add("Operator", filter.Operator);
					}
					dictionary.Add("Property", filter.Property);
					dictionary.Add("Predicate", filter.Predicate);
					dictionary.Add("Value", searchValue);
					searchParms.Add(dictionary);
				}

                /*
                 * Add the filter to suppress inactive rewards
                 * */
                var d = new Dictionary<string, object>();
                d.Add("Property", "Active");
                d.Add("Predicate", LWCriterion.Predicate.Eq);
                d.Add("Value", true);
                searchParms.Add(d);

				IList<RewardDef> rewards = null;
				if (searchParms.Count == 0)
				{
					rewards = ContentService.GetAllRewardDefs();
				}
				else
				{
					rewards = ContentService.GetRewardDefsByProperty(searchParms, "Name", true, new LWQueryBatchInfo() { BatchSize = int.MaxValue, StartIndex = 0 });
				}

				if (rewards != null && rewards.Count > 0)
				{
					ddlReward.Items.Add(new ListItem("-- Select --", string.Empty));
                    foreach (var reward in rewards.OrderBy(r => r.Name))
					{
						var li = new ListItem(reward.Name, reward.Id.ToString());
						ddlReward.Items.Add(li);
					}
				}
				else
				{
					pchNoResults.Visible = true;
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
		}

		private string ParseExpressions(string Text)
		{
			return System.Text.RegularExpressions.Regex.Replace(Text, @"\#\#(?<FieldName>.*?)\#\#", new MatchEvaluator(this.ExpressionEval));
		}
		
		private string ExpressionEval(Match m)
		{
			try
			{
				Expression e = new ExpressionFactory().Create(m.Groups["FieldName"].ToString());

				object value = e.evaluate(_co);
				if (value != null)
				{
					return value.ToString();
				}
				else
				{
					return string.Empty;
				}
			}
			catch
			{
				return m.ToString();
			}
		}
		
		private string GetBusinessRuleName(RewardDef reward, Product product)
		{
			//search for rule tied directly to reward
			foreach (var rule in _config.CustomRules)
			{
				if (rule.Rewards.Contains(reward.Id))
				{
					return rule.BusinessRuleName;
				}
			}

			//search for rule tied to product's category
			foreach (var rule in _config.CustomRules)
			{
				if (rule.Categories.Contains(product.CategoryId))
				{
					return rule.BusinessRuleName;
				}
			}

			//none found, use default business rule name
			return _config.BusinessRuleName;
		}

	}
}
