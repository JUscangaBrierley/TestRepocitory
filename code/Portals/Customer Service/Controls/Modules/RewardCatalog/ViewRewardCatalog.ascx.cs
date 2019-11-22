using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.FrameWork.Rules;
using Brierley.LWModules.RewardCatalog.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Controls.FixedView;
using Brierley.WebFrameWork.Portal.Validators;

namespace Brierley.LWModules.RewardCatalog
{
	public partial class ViewRewardCatalog : ModuleControlBase, IIpcEventHandler
	{
		#region fields

		private const string _className = "ViewRewardCatalog";
		private const string _modulePath = "~/Controls/Modules/RewardCatalog/ViewRewardCatalog.ascx";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private RewardCatalogConfig _config = new RewardCatalogConfig();
		private List<LinkButton> _categoryLinks = new List<LinkButton>();
		private Member _member = null;
		private IList<LanguageDef> _languages;
		private IList<ChannelDef> _channels;
		private RewardCatalogFilter _rewardFilter;
		private Dictionary<string, decimal> _availablePointsByType = new Dictionary<string, decimal>();
		private FixedLayoutManager _layoutManager = null;
        private RewardRuleUtil _rewardUtil = new RewardRuleUtil();
		#endregion

		#region properties

		private Int64 SelectedCategoryId
		{
			get
			{
				if (ViewState["SelectedCategoryId"] != null)
				{ return (Int64)ViewState["SelectedCategoryId"]; }
				else
				{
					return 0;
				}
			}
			set
			{
				ViewState["SelectedCategoryId"] = value;
			}
		}

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

		#endregion


		#region page life cycle

		protected override void OnInit(EventArgs e)
		{
			_config = Brierley.WebFrameWork.Portal.Configuration.ConfigurationUtil.GetConfiguration<RewardCatalogConfig>(ConfigurationKey);
			if (_config == null)
			{
				return;
			}

			_member = PortalState.CurrentMember;
			if (_member == null)
			{
				return;
			}

			_layoutManager = new FixedLayoutManager(LayoutTypes.ResponsiveMultiColumn, pchShippingFields);

			lnkOrderShipping.ValidationGroup = ValidationGroup;

			litTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleResourceKey);
			lblNoResults.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey);
			pnlRewards.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.AvailableRewardsResourceKey);
			lblOrderBy.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.OrderByResourceKey);
			lblLanguage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.LanguageResourceKey);
			lblChannel.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.ChannelResourceKey);
			if (_config.PageSize > 0)
			{
				Pager.PageSize = _config.PageSize;
			}

			if (_config.ShowRewardFilter)
			{
				_rewardFilter = new RewardCatalogFilter(this);
				phRewardFilter.Controls.Add(_rewardFilter);
			}

			BuildOrderPage();

			if (!IsPostBack)
			{
				// if config.ShowLanguage/Channel Dropdowns
				if (_config.ShowLanguageDropdown)
				{
					_languages = ContentService.GetLanguageDefs();
					if (drpLanguage.Items.Count < 2 && _languages != null && _languages.Count > 0)
					{
						foreach (LanguageDef language in _languages)
						{
							if (drpLanguage.Items.FindByText(language.Language) == null)
							{
								drpLanguage.Items.Add(new ListItem(language.Language, language.Culture));
							}
						}
					}
				}
				else
				{
					lblLanguage.Visible = false;
					drpLanguage.Visible = false;
				}
				if (_config.ShowChannelDropdown)
				{
					_channels = ContentService.GetChannelDefs();
					if (drpChannel.Items.Count < 2 && _channels != null && _channels.Count > 0)
					{
						foreach (ChannelDef channel in _channels)
						{
							if (drpChannel.Items.FindByText(channel.Name) == null)
							{
								drpChannel.Items.Add(new ListItem(channel.Name, channel.Name));
							}
						}
					}
				}
				else
				{
					lblChannel.Visible = false;
					drpChannel.Visible = false;
				}
			}

			base.OnInit(e);
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";

			if (_config == null)
			{
				return;
			}
			if (_member == null)
			{
				this.Visible = false;
				_logger.Trace(_className, methodName, "No member selected");
				return;
			}

			IpcManager.RegisterEventHandler("MemberSelected", this, false);
			lstRewards.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lstRewards_ItemDataBound);
			lstRewards.ItemCommand += new EventHandler<ListViewCommandEventArgs>(lstRewards_ItemCommand);
			ddlOrder.SelectedIndexChanged += delegate { LoadRewardList(); };
			drpLanguage.SelectedIndexChanged += delegate { LoadRewardList(); };
			drpChannel.SelectedIndexChanged += delegate { LoadRewardList(); };
			lstRewards.PagePropertiesChanged += delegate { LoadRewardList(); };

			lnkDone.Click += new EventHandler(lnkDone_Click);
			lnkOrder.Click += new EventHandler(lnkOrder_Click);

			lnkCancelShipping.Click += new EventHandler(lnkDone_Click);
			lnkOrderShipping.Click += new EventHandler(lnkOrderShipping_Click);

			lnkCancelReview.Click += new EventHandler(lnkDone_Click);
			lnkPlaceOrder.Click += new EventHandler(lnkPlaceOrder_Click);

            this.Page.Form.Attributes.Add("onsubmit", "return CheckDuplicateSubmit();");

			try
			{
				BuildCategoryListView();
				if (!IsPostBack)
				{
					if (_member != null)
					{
						LoadRewardList();
						mvMain.SetActiveView(RewardListView);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}


		protected override void OnPreRender(EventArgs e)
		{
			//todo: move this and ViewMemberProfile.ascx.cs to a common file - they share everything exception for
			//the exclusion of _datePickers code at the end

			Func<string, string> javascriptEscape = delegate(string s)
			{
				if (string.IsNullOrEmpty(s))
				{
					return string.Empty;
				}
				return s.Replace("'", "\\'");
			};

			//create client side scripts for parent/child list controls
			if (_config != null && _config.AddressFieldsToShow != null)
			{
				var relations =
					from p in _config.AddressFieldsToShow
					join c in _config.AddressFieldsToShow on p.DataKey equals c.ParentId
					where
						p.Control != null &&
						c.Control != null &&
						p.Control is ListControl &&
						c.Control is ListControl
					select new { ParentAttribute = p, ChildAttribute = c };

				if (relations.Count() > 0)
				{
					bool first = true;
					var sb = new StringBuilder("<script type=\"text/javascript\">\r\n");

					sb.Append("_parentLists = [");
					var distinctParents = (from r in relations select new { id = r.ParentAttribute.Control.ClientID }).Distinct();
					foreach (var parent in distinctParents)
					{
						if (!first)
						{
							sb.Append(", ");
						}
						sb.AppendFormat("'{0}'", parent.id);
						first = false;
					}
					sb.AppendLine("];");

					sb.AppendLine("_relations = [");
					first = true;
					foreach (var relation in relations)
					{
						if (!first)
						{
							sb.Append(", ");
						}
						sb.AppendLine();
						sb.AppendFormat("new Relation('{0}', '{1}', [", relation.ParentAttribute.Control.ClientID, relation.ChildAttribute.Control.ClientID);

						bool firstItem = true;
						List<ItemListSource> items = relation.ChildAttribute.ListSource ?? new List<ItemListSource>();
						if (!string.IsNullOrEmpty(relation.ChildAttribute.GlobalSetName) && !string.IsNullOrEmpty(relation.ChildAttribute.GlobalColumnName))
						{
							//add global set values to list
							var crit = new LWCriterion(relation.ChildAttribute.GlobalSetName);
							crit.AddOrderBy(relation.ChildAttribute.GlobalColumnName, LWCriterion.OrderType.Ascending);
							IList<IClientDataObject> set = LoyaltyService.GetAttributeSetObjects(null, relation.ChildAttribute.GlobalSetName, crit, null, false);
							if (set != null)
							{
								foreach (var row in set)
								{
									items.Add(new ItemListSource(row.RowKey.ToString(), row.GetAttributeValue(relation.ChildAttribute.GlobalColumnName).ToString(), row.ParentRowKey.ToString()));
								}
							}
						}
						foreach (var li in relation.ChildAttribute.ListSource)
						{
							if (!firstItem)
							{
								sb.Append(", ");
							}
							sb.AppendFormat("new Item('{0}', '{1}', '{2}')", javascriptEscape(li.ParentKey), javascriptEscape(li.Key), javascriptEscape(li.Value));
							firstItem = false;
						}
						sb.Append("])");

						first = false;
					}
					sb.AppendLine("];");
					sb.AppendLine("</script>");
					Page.ClientScript.RegisterStartupScript(this.GetType(), "ParentChildLists", sb.ToString());
				}

				/*
				if (_datePickers.Count > 0)
				{
					var sb = new StringBuilder("<script type=\"text/javascript\">\r\n$(document).ready(function() { ");

					sb.Append("var datePickers = [");
					for (int i = 0; i < _datePickers.Count; i++)
					{
						if (i > 0)
						{
							sb.Append(", ");
						}
						sb.AppendFormat("'#{0}'", _datePickers[i].ClientID);
					}
					sb.AppendLine("];");

					sb.AppendLine(@"
					for(var i=0, len=datePickers.length; i < len; i++) {
						$(datePickers[i]).datepicker();
					}");
					sb.AppendLine("});");
					sb.AppendLine("</script>");
					Page.ClientScript.RegisterStartupScript(this.GetType(), "DatePickers", sb.ToString());
				}
				*/
			}
			base.OnPreRender(e);
		}

		#endregion


		#region event handlers

		void lstRewards_ItemCommand(object sender, ListViewCommandEventArgs e)
		{
			const string methodName = "lstRewards_ItemCommand";
			try
			{			

				long rewardId = long.Parse(e.CommandArgument.ToString());
				SelectedRewardId = rewardId;
				var reward = ContentService.GetRewardDef(rewardId);
				var product = reward.Product ?? ContentService.GetProduct(reward.ProductId);
				if (reward.Product == null)
				{
					reward.Product = product;
				}

				string culture = LanguageChannelUtil.GetDefaultCulture();
				if (drpLanguage.SelectedValue != "Default") culture = drpLanguage.SelectedValue;
				string channel = LanguageChannelUtil.GetDefaultChannel();
				if (drpChannel.SelectedValue != "Default") channel = drpChannel.SelectedValue;

				litRewardNameDetail.Text = litRewardNameShipping.Text = litRewardNameReview.Text = reward.GetDisplayName(culture, channel);
				litRewardDescriptionDetail.Text = litRewardDescriptionShipping.Text = litRewardDescriptionReview.Text = reward.GetLongDescription(culture, channel);
				litProductDescriptionDetail.Text = product.GetLongDescription(culture, channel);
                RewardImageDetail.ImageUrl = RewardImageShipping.ImageUrl = RewardImageReview.ImageUrl = ContentManagementUtils.GetValidWebAddres(reward.LargeImageFile, PortalState.GetImageUrl(string.Empty));

                string[] types = reward.GetPointTypes();
                if (types != null && types.Length == 1)
                {
                    litRewardCostDetail.Text = litRewardCostShipping.Text = litRewardCostReview.Text = string.Format("{0:0.#####} {1}", reward.HowManyPointsToEarn, types[0]);
                }
                else
                {
                    litRewardCostDetail.Text = litRewardCostShipping.Text = litRewardCostReview.Text = string.Format("{0:0.#####} Points", reward.HowManyPointsToEarn);
                }

                decimal points = _rewardUtil.GetPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), _member, null);

				litCurrentBalanceReview.Text = points.ToString("0.#####");
				litRemainingBalanceReview.Text = (points - reward.HowManyPointsToEarn).ToString("0.#####");

				#region Validate The Reward Selection
				IAddMemberRewardInterceptor interceptor = null;
				if (!string.IsNullOrEmpty(_config.RewardInterceptorAssemblyName) && !string.IsNullOrEmpty(_config.RewardInterceptorTypeName))
				{
					interceptor = ClassLoaderUtil.CreateInstance(_config.RewardInterceptorAssemblyName, _config.RewardInterceptorTypeName) as IAddMemberRewardInterceptor;
					if (interceptor != null)
					{
						try
						{
                            interceptor.ValidateRewardSelection(_member, reward);
						}
						catch (LWValidationException ex)
						{
							_logger.Error(_className, methodName, "Error validating reward selection.", ex);
							ShowNegative(ex.Message);
							return;
						}
						catch (NotImplementedException)
						{
							// not implemented
						}
						catch (Exception ex)
						{
							_logger.Error(_className, methodName, "Error validating reward selection.", ex);
							throw;
						}
					}
				}
				#endregion

				mvMain.SetActiveView(RewardDetailView);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}


		void lstRewards_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			if (e.Item.ItemType == ListViewItemType.DataItem)
			{
				var reward = e.Item.DataItem as RewardDef;
				var lnk = e.Item.FindControl("lnkReward") as LinkButton;
				var img = e.Item.FindControl("imgThumbnail") as Image;
				var litCost = e.Item.FindControl("litRewardCost") as Literal;
				var litDesc = e.Item.FindControl("litRewardDescription") as Literal;

				if (!_availablePointsByType.ContainsKey(reward.PointType))
				{
                    _availablePointsByType.Add(reward.PointType, _rewardUtil.GetPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), _member, null));
				}
				decimal points = _availablePointsByType[reward.PointType];
				if (points < reward.HowManyPointsToEarn)
				{
					lnk.Attributes.Add("class", "Unavailable");
					lnk.Enabled = false;
				}

				if (lnk != null)
				{
					lnk.CommandArgument = reward.Id.ToString();
				}
				if (img != null)
				{
                    if (!string.IsNullOrWhiteSpace(reward.SmallImageFile))
                    {
                        img.ImageUrl = ContentManagementUtils.GetValidWebAddres(reward.SmallImageFile, PortalState.GetImageUrl(string.Empty));
                    }
				}
				if (litCost != null)
				{
                    string[] types = reward.GetPointTypes();
                    if (types != null && types.Length == 1)
                    {
                        litCost.Text = string.Format("{0:0.#####} {1}", reward.HowManyPointsToEarn, types[0]);
                    }
                    else
                    {
                        litCost.Text = string.Format("{0:0.#####} Points", reward.HowManyPointsToEarn);
                    }
				}
				if (litDesc != null && reward.ProductId > 0)
				{
					var product = reward.Product ?? ContentService.GetProduct(reward.ProductId);
					if (reward.Product == null)
					{
						reward.Product = product;
					}

					string culture = LanguageChannelUtil.GetDefaultCulture();
					if (drpLanguage.SelectedValue != "Default") culture = drpLanguage.SelectedValue;
					string channel = LanguageChannelUtil.GetDefaultChannel();
					if (drpChannel.SelectedValue != "Default") channel = drpChannel.SelectedValue;
					litDesc.Text = product.GetShortDescription(culture, channel);
				}
			}
		}


		void lnkCategory_Click(object sender, EventArgs e)
		{
			var lnk = sender as LinkButton;
			if (lnk != null)
			{
				long catId = 0;
				if (long.TryParse(lnk.CommandArgument, out catId))
				{
					foreach (var link in _categoryLinks)
					{
						link.CssClass = "DashboardLink";
					}
					lnk.CssClass = "DashboardLink Current";

					SelectedCategoryId = catId;
					Pager.SetPageProperties(0, Pager.MaximumRows, false);
					LoadRewardList();
				}
			}
		}


		/// <summary>
		/// Standard cancel button event, tied to all views
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void lnkDone_Click(object sender, EventArgs e)
		{
			mvMain.SetActiveView(RewardListView);
		}


		/// <summary>
		/// Event for order button in the initial detail view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void lnkOrder_Click(object sender, EventArgs e)
		{
			const string methodName = "lnkOrder_Click";
			long rewardId = SelectedRewardId;
			try
			{
				if (_member != null)
				{
					RewardDef rdef = ContentService.GetRewardDef(rewardId);
					if (rdef == null)
					{
						throw new RewardCatalogException(ResourceUtils.GetLocalWebResource(_modulePath, "RewardNotFound.Text" ,"Unable to locate this reward."));
					}

                    decimal points = _rewardUtil.GetPoints(rdef, rdef.GetPointTypes(), rdef.GetPointEvents(), _member, null);
					if (points < rdef.HowManyPointsToEarn)
					{
						ShowNegative(ResourceUtils.GetLocalWebResource(_modulePath, "NotEnoughPoints.Text", "You do not have enough available points to get this reward."));
					}

					var product = rdef.Product ?? ContentService.GetProduct(rdef.ProductId);
					if (rdef.Product == null)
					{
						rdef.Product = product;
					}

					string businessRuleName = GetBusinessRuleName(rdef, product);
					if (string.IsNullOrEmpty(businessRuleName))
					{
						throw new RewardCatalogException(ResourceUtils.GetLocalWebResource(_modulePath, "NoRuleConfigured.Text","No business rule has been configured to facilitate checkout."));
					}

					RuleTrigger rt = LoyaltyService.GetRuleByName(businessRuleName);
					if (rt == null || rt.Rule == null)
					{
						throw new RewardCatalogException(string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "RuleNotFound.Text", "Business rule {0} could not be found."), businessRuleName));
					}

					if (rt.Rule is RewardCatalogIssueReward == false)
					{
						throw new RewardCatalogException(ResourceUtils.GetLocalWebResource(_modulePath, "InvalieRuleType.Text","Invalid rule type selected. Rule must be of type RewardCatalogIssueReward."));
					}

					var rule = (RewardCatalogIssueReward)rt.Rule;
                    if (_config.FulfillmentOptionRequiresShippingForm(rule.FulfillmentOption.ToString()))
                    {
                        //show shipping information screen
						mvMain.SetActiveView(ShippingInformationView);
						return;
                    }                    
					else
					{
						//no shipping information needed. go directly to order review screen
						pchReviewShipping.Visible = false;
						mvMain.SetActiveView(OrderReviewView);
					}
				}
				else
				{
					throw new RewardCatalogException(ResourceUtils.GetLocalWebResource(_modulePath, "MemberLoginMessage.Text","Please log on as a member to get this reward."));
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}


		/// <summary>
		/// Event for order button in the shipping information view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void lnkOrderShipping_Click(object sender, EventArgs e)
		{
			BuildConfirmationPage();
			pchReviewShipping.Visible = true;
			mvMain.SetActiveView(OrderReviewView);
		}


		/// <summary>
		/// Event for final order button in the order review view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void lnkPlaceOrder_Click(object sender, EventArgs e)
		{
			const string methodName = "lnkPlaceOrder_Click";
			try
			{
				string orderNumber = string.Empty;

				if (_member == null)
				{
					throw new RewardCatalogException(ResourceUtils.GetLocalWebResource(_modulePath, "MemberLoginMessage.Text", "Please log on as a member to get this reward."));
				}

				RewardDef reward = ContentService.GetRewardDef(SelectedRewardId);
				if (reward == null)
				{
					throw new RewardCatalogException(ResourceUtils.GetLocalWebResource(_modulePath, "RewardNotFound.Text", "Unable to locate this reward."));
				}

                decimal points = _rewardUtil.GetPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), _member, null);
				if (points < reward.HowManyPointsToEarn)
				{
					throw new LWValidationException(ResourceUtils.GetLocalWebResource(_modulePath, "NotEnoughPoints.Text", "You do not have enough available points to get this reward."));
				}

				var product = reward.Product ?? ContentService.GetProduct(reward.ProductId);
				if (reward.Product == null)
				{
					reward.Product = product;
				}

				RuleTrigger rt = LoyaltyService.GetRuleByName(GetBusinessRuleName(reward, product));
				var rule = (RewardCatalogIssueReward)rt.Rule;

				Address address = null;
                if (_config.FulfillmentOptionRequiresShippingForm(rule.FulfillmentOption.ToString()))
                {
                    address = new Address();
					foreach (var item in _config.AddressFieldsToShow)
					{
						switch (item.AttributeName)
						{
							case "Address Line 1":
								address.AddressLineOne = GetControlValue(item, false);
								break;
							case "Address Line 2":
								address.AddressLineTwo = GetControlValue(item, false);
								break;
							case "Address Line 3":
								address.AddressLineThree = GetControlValue(item, false);
								break;
							case "Address Line 4":
								address.AddressLineFour = GetControlValue(item, false);
								break;
							case "Country":
								address.Country = GetControlValue(item, false);
								break;
							case "City":
								address.City = GetControlValue(item, false);
								break;
							case "State":
								address.StateOrProvince = GetControlValue(item, false);
								break;
							case "Zip Code":
								address.ZipOrPostalCode = GetControlValue(item, false);
								break;
						}
					}
                }
                
				var orderItems = new List<RewardOrderItem>();
				orderItems.Add(new RewardOrderItem()
				{
					PartNumber = product != null ? product.PartNumber : string.Empty,
					PointsConsumptionPolicy = rule.PointsConsumption,
					RewardDefinition = reward,
					RewardName = reward.Name,
					Rule = rt
				});


				var ruleConfig = new NameValueCollection();
				ruleConfig.Add(reward.Name, GetBusinessRuleName(reward, product));

				IAddMemberRewardInterceptor interceptor = null;
				if (!string.IsNullOrEmpty(_config.RewardInterceptorAssemblyName) && !string.IsNullOrEmpty(_config.RewardInterceptorTypeName))
				{
					interceptor = ClassLoaderUtil.CreateInstance(_config.RewardInterceptorAssemblyName, _config.RewardInterceptorTypeName) as IAddMemberRewardInterceptor;
				}

				//todo: IOrderFulfillmentProvider does not accept prefix, middle name or suffix, but they are taken from
				//the member object and put into the order record. "Mr. John Q Doe IV" can send a reward to "Mrs. Jane J Doe" and
				//the order record will go to "Mr. Jane Q Doe IV". Add these fields to CreateOrder().
				string prefix = string.Empty;
				string firstName = string.Empty;
				string middleName = string.Empty;
				string lastName = string.Empty;
				string suffix = string.Empty;
				string email = string.Empty;
				foreach (var item in _config.AddressFieldsToShow)
				{
					switch (item.AttributeName)
					{
						case "Name Prefix":
							prefix = GetControlValue(item);
							break;
						case "First Name":
							firstName = GetControlValue(item);
							break;
						case "Middle Name":
							middleName = GetControlValue(item);
							break;
						case "Last Name":
							lastName = GetControlValue(item);
							break;
						case "Name Suffix":
							suffix = GetControlValue(item);
							break;
						case "Email Address":
							email = GetControlValue(item);
							break;
					}
				}

                string changedBy = string.Empty;
                CSAgent agent = PortalState.GetLoggedInCSAgent();
                if (agent != null)
                {
                    changedBy = agent.Username;
                }
                else if (PortalState.IsMemberLoggedIn())
                {
                    Member member = PortalState.GetLoggedInMember();
                    changedBy = member.Username;                    
                }

				var order = MemberRewardsUtil.CreateRewardOrder(_member, firstName, lastName, null, address, _config.Channel, changedBy, email, orderItems, ruleConfig, interceptor, LanguageChannelUtil.GetDefaultCulture());
				orderNumber = order.OrderNumber;

				// No support for printed reward in this version
				//if (rule.FulfillmentOption == RewardFulfillmentOption.Printed)
				//{
				//	PortalState.PutInCache("SelectedReward", reward);
				//	NavigateToUserControl("RewardCatalog", "printCouponView");
				//	RegisterResponseScript("onclick", "window.open('" + Request.ApplicationPath + "/DesktopModules/RewardCatalog/Coupon.aspx','PrintMe','height=800px,width=800px,scrollbars=1');");
				//}

				string orderCompleteMessage = ResourceUtils.GetLocalWebResource(_modulePath, _config.OrderCompleteMessageResourceKey);
				if (!string.IsNullOrEmpty(orderCompleteMessage))
				{
					if (orderCompleteMessage.Contains("##"))
					{
						var co = new ContextObject();
						co.Owner = _member;
						var env = new Dictionary<string, object>();
						env.Add("RewardName", reward.Name);
						env.Add("OrderNumber", orderNumber);
						env.Add("PointsDeducted", reward.HowManyPointsToEarn);
						co.Environment = env;
						litOrderComplete.Text = ExpressionUtil.ParseExpressions(orderCompleteMessage, co);
					}
					else
					{
						litOrderComplete.Text = orderCompleteMessage;
					}
				}

				mvMain.SetActiveView(OrderCompleteView);
				IpcManager.PublishEvent("MemberUpdated", ConfigurationKey, _member);
			}
			catch (LWValidationException ex)
			{
				//rebuild the confirmation page if there is a validation issue, so the user doesn't see an incomplete page
				BuildConfirmationPage();
				ShowNegative(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		#endregion


		#region IpcEventInfo implementation

		public void HandleEvent(IpcEventInfo info)
		{
			const string methodName = "HandleEvent";
			try
			{
				if (info.EventName == "MemberSelected")
				{
					_member = PortalState.CurrentMember;
					if (_member != null)
					{
						LoadRewardList();
						mvMain.SetActiveView(RewardListView);
						this.Visible = true;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
			}
		}

		public ModuleConfigurationKey GetConfigurationKey()
		{
			return ConfigurationKey;
		}

		#endregion


		internal void LoadRewardList()
		{
			var co = new ContextObject() { Owner = _member };

			var filters = new List<Dictionary<string, object>>();
			if (_config.RewardFilters != null && _config.RewardFilters.Count > 0)
			{
				foreach (var filter in _config.RewardFilters)
				{
					Dictionary<string, object> f = new Dictionary<string, object>();
					if (!string.IsNullOrEmpty(filter.Operator))
					{
						f.Add("Operator", filter.Operator);
					}
					f.Add("Property", filter.Property);
					f.Add("Predicate", filter.Predicate);
					f.Add("Value", Brierley.FrameWork.bScript.ExpressionUtil.ParseExpressions(filter.Value, co));
					if (filter.IsAttribute)
					{
						f.Add("IsAttribute", true);
					}
					filters.Add(f);
				}
			}

			if (_rewardFilter != null && _rewardFilter.SearchParams != null && _rewardFilter.SearchParams.Count > 0)
			{
				foreach (var filter in _rewardFilter.SearchParams)
				{
					filters.Add(filter);
				}
			}

			IList<RewardDef> rewards = null;
			if (filters.Count > 0)
			{
				rewards = ContentService.GetRewardDefsByProperty(filters, null, true, null);
			}
			else if (SelectedCategoryId < 1)
			{
				//all rewards
				rewards = ContentService.GetAllRewardDefs();
			}
			else
			{
				rewards = ContentService.GetRewardDefsByCategory(SelectedCategoryId);
			}

			if (rewards != null && rewards.Count > 0)
			{
				foreach (var reward in rewards.ToList())
				{
					if (reward.CatalogStartDate.GetValueOrDefault(DateTimeUtil.MinValue) > DateTime.Now)
					{
						rewards.Remove(reward);
						continue;
					}
					if (reward.CatalogEndDate.GetValueOrDefault(DateTimeUtil.MaxValue) < DateTime.Now)
					{
						rewards.Remove(reward);
						continue;
					}

					if (_config.HideWhenNotEnoughPoints)
					{
                        decimal points = _rewardUtil.GetPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), _member, null);
						if (points < reward.HowManyPointsToEarn)
						{
							rewards.Remove(reward);
							continue;
						}
					}
					if (_config.HideWhenOutOfStock)
					{
						var product = reward.Product ?? ContentService.GetProduct(reward.ProductId);
						if (reward.Product == null)
						{
							reward.Product = product;
						}
						if (product != null && product.Quantity.GetValueOrDefault(1) < 1)
						{
							rewards.Remove(reward);
						}
					}
				}
			}

			if (rewards == null || rewards.Count == 0)
			{
				pchRewardList.Visible = false;
				lblNoResults.Visible = true;
			}
			else
			{
				switch (ddlOrder.SelectedValue)
				{
					case "NameAsc":
					default:
						rewards = rewards.OrderBy(o => o.Name).ToList();
						break;
					case "NameDesc":
						rewards = rewards.OrderByDescending(o => o.Name).ToList();
						break;
					case "PriceAsc":
						rewards = rewards.OrderBy(o => o.HowManyPointsToEarn).ToList();
						break;
					case "PriceDesc":
						rewards = rewards.OrderByDescending(o => o.HowManyPointsToEarn).ToList();
						break;
				}
				lstRewards.DataSource = rewards;
				lstRewards.DataBind();

				pchRewardList.Visible = true;
				lblNoResults.Visible = false;
			}
		}


		#region private methods

		private void BuildCategoryListView()
		{
			var ul = new HtmlGenericControl("ul");

			var liAll = new HtmlGenericControl("li");
			var lnkAll = new LinkButton() { Text = "All", CommandArgument = "0", CssClass = "DashboardLink" };
			lnkAll.Click += new EventHandler(lnkCategory_Click);
			if (SelectedCategoryId < 1)
			{
				lnkAll.CssClass += " Current";
			}
			HtmlGenericControl h2 = new HtmlGenericControl("h2");
			h2.Controls.Add(lnkAll);
			liAll.Controls.Add(h2);
			ul.Controls.Add(liAll);
			pchCategories.Controls.Add(ul);
			_categoryLinks.Add(lnkAll);

			IList<Category> categories = ContentService.GetTopLevelCategoriesByType(CategoryType.Product, true);
			if (categories != null)
			{
				foreach (var cat in categories)
				{
					ul = new HtmlGenericControl("ul");
					var li = new HtmlGenericControl("li");
					var lnk = new LinkButton() { Text = cat.Name, CommandArgument = cat.ID.ToString(), CssClass = "DashboardLink", ToolTip = cat.Description };
					if (SelectedCategoryId == cat.ID)
					{
						lnk.CssClass += " Current";
					}
					lnk.Click += new EventHandler(lnkCategory_Click);
					h2 = new HtmlGenericControl("h2");
					h2.Controls.Add(lnk);
					_categoryLinks.Add(lnk);
					li.Controls.Add(h2);
					ul.Controls.Add(li);
					pchCategories.Controls.Add(ul);
					BuildChildCategories(li, cat);
				}
			}
		}


		private void BuildChildCategories(HtmlGenericControl control, Category category)
		{
			IList<Category> categories = ContentService.GetChildCategories(category.ID, true);

			if (categories != null)
			{
				var ul = new HtmlGenericControl("ul");
				foreach (var cat in categories)
				{
					var li = new HtmlGenericControl("li");
					var lnk = new LinkButton() { Text = cat.Name, CommandArgument = cat.ID.ToString(), CssClass = "DashboardLink", ToolTip = cat.Description };
					if (SelectedCategoryId == cat.ID)
					{
						lnk.CssClass += " Current";
					}
					lnk.Click += new EventHandler(lnkCategory_Click);
					li.Controls.Add(lnk);
					_categoryLinks.Add(lnk);
					ul.Controls.Add(li);
					BuildChildCategories(li, cat);
				}
				control.Controls.Add(ul);
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

		#endregion


		#region Shipping information and confirmation page views
		

		private void BuildOrderPage()
		{
			foreach (ConfigurationItem attribute in _config.AddressFieldsToShow)
			{
				switch (attribute.AttributeType)
				{
					case ItemTypes.Attribute:
						string value = string.Empty;
						if (_member != null)
						{
							value = GetMemberAttributeValue(attribute);
						}
						AddAttributeItem(attribute, value);
						break;
					case ItemTypes.HtmlBlock:
						AddHtmlBlock(attribute);
						break;
					case ItemTypes.SwitchLayout:
						_layoutManager.SwitchLayoutMode(attribute.LayoutType);
						break;
				}
			}
		}


		private void BuildConfirmationPage()
		{
			_layoutManager = new FixedLayoutManager(LayoutTypes.ResponsiveMultiColumn, pchReviewShippingFields);

			foreach (ConfigurationItem attribute in _config.AddressFieldsToShow)
			{
				switch (attribute.AttributeType)
				{
					case ItemTypes.Attribute:
						string value = GetControlValue(attribute);
						attribute.DisplayType = DisplayTypes.Label;
						AddAttributeItem(attribute, value);
						break;
					case ItemTypes.HtmlBlock:
						AddHtmlBlock(attribute);
						break;
					case ItemTypes.SwitchLayout:
						_layoutManager.SwitchLayoutMode(attribute.LayoutType);
						break;
				}
			}
		}


		private void AddAttributeItem(ConfigurationItem item, string value)
		{
			if (!string.IsNullOrEmpty(item.DefaultValue) && string.IsNullOrEmpty(value))
			{
				value = ExpressionUtil.ParseExpressions(item.DefaultValue, new ContextObject() { Owner = _member });
			}
			
			WebControl control;
			switch (item.DisplayType)
			{
				case DisplayTypes.TextBox:
				case DisplayTypes.DatePicker:
					control = new TextBox();
					((TextBox)control).Text = value;
					if (item.MaxLength > -1)
					{
						((TextBox)control).MaxLength = item.MaxLength;
					}
					break;
				case DisplayTypes.CheckBox:
					control = new CheckBox();
					value = value.ToLower() == "true" || item.DefaultValue == "1" ? "true" : "false";
					((CheckBox)control).Text = GetLabelText(item);
					((CheckBox)control).Checked = value == "1" || value.ToLower() == "true";
					break;
				case DisplayTypes.DropDownList:
					control = new DropDownList();
					break;
				case DisplayTypes.RadioList:
					control = new RadioButtonList();
					((RadioButtonList)control).RepeatLayout = RepeatLayout.Flow;
					((RadioButtonList)control).RepeatDirection = RepeatDirection.Horizontal;
					((RadioButtonList)control).SelectedValue = value;
					break;
				case DisplayTypes.Label:
					control = new Label();
					((Label)control).Text = value;
					break;
				default:
					throw new Exception(ResourceUtils.GetLocalWebResource(_modulePath, "DisplayTypeUndetermined.Text","Failed to determine display type for attribute") + " " + item.AttributeName);
			}

			if (item.DisplayType == DisplayTypes.DropDownList || item.DisplayType == DisplayTypes.RadioList)
			{
				if (item.ListSource != null)
				{
					foreach (ItemListSource source in item.ListSource)
					{
						((ListControl)control).Items.Add(new ListItem(source.Value, source.Key));
					}
				}

				if (item.ListType == ListSourceType.GlobalAttributeSets)
				{
					if (!string.IsNullOrEmpty(item.GlobalSetName) && !string.IsNullOrEmpty(item.GlobalColumnName))
					{
						var crit = new LWCriterion(item.GlobalSetName);
						crit.AddOrderBy(item.GlobalColumnName, LWCriterion.OrderType.Ascending);
						IList<IClientDataObject> set = LoyaltyService.GetAttributeSetObjects(null, item.GlobalSetName, crit, null, false);
						if (set != null)
						{
							foreach (var row in set)
							{
								var li = new ListItem(row.GetAttributeValue(item.GlobalColumnName).ToString(), row.RowKey.ToString());
								((ListControl)control).Items.Add(li);
							}
						}
					}
				}
				else if (item.ListType == ListSourceType.FrameworkObject)
				{
					if (!string.IsNullOrEmpty(item.LWFrameworkObjectName) && !string.IsNullOrEmpty(item.LWFrameworkObjectProperty))
					{
						Dictionary<long, string> coll = LWObjectBinderUtil.GetObjectValues(item.LWFrameworkObjectName,
							item.LWFrameworkObjectProperty, item.LWFrameworkObjectWhereClause);
						foreach (long key in coll.Keys)
						{
                            var li = new ListItem(coll[key], key.ToString());
							((ListControl)control).Items.Add(li);
						}
					}
				}

				if (item.DisplayType == DisplayTypes.DropDownList)
				{
					if (!IsPostBack && !string.IsNullOrEmpty(item.DefaultValue) && string.IsNullOrEmpty(value))
					{
						DropDownList list = (DropDownList)control;
						foreach (ListItem li in list.Items)
						{
							if (li.Text == item.DefaultValue)
							{
								li.Selected = true;
								break;
							}
						}
					}
					else
					{
						((DropDownList)control).SelectedValue = value;
					}
				}
				else
				{
					((RadioButtonList)control).SelectedValue = value;
				}
			}

			control.ID = item.DataKey;
			control.CssClass = item.ControlCSSClass;

			var controlLabel = new HtmlGenericControl("label");
			controlLabel.InnerText = item.DisplayType == DisplayTypes.CheckBox ? "" : GetLabelText(item);
			controlLabel.Attributes.Add("class", item.LabelCssClass);

			List<BaseValidator> validators = new List<BaseValidator>();

			if (item.DisplayType != DisplayTypes.Label)
			{
				foreach (AttributeValidator validator in item.validators)
				{
					BaseValidator vldBase = null;

					switch (validator.ValidatorType)
					{
						case ValidatorTypes.Compare:
							CompareValidator vldCompare = new CompareValidator();
							vldCompare.ControlToCompare = validator.CompareToID;
							vldCompare.Type = validator.CompareType.GetValueOrDefault(ValidationDataType.String);
							vldBase = vldCompare;
							break;
						case ValidatorTypes.Range:
							RangeValidator vldRange = new RangeValidator();
							vldRange.MinimumValue = validator.MinValue;
							vldRange.MaximumValue = validator.MaxValue;
							vldRange.Type = validator.CompareType.GetValueOrDefault(ValidationDataType.String);
							vldBase = vldRange;
							break;
						case ValidatorTypes.RegularExpression:
							RegularExpressionValidator vldRegex = new RegularExpressionValidator();
							vldRegex.ValidationExpression = validator.RegularExpression;
							vldBase = vldRegex;
							break;
						case ValidatorTypes.RequiredField:
							RequiredFieldValidator vldReq = new RequiredFieldValidator();
							vldBase = vldReq;
							break;
						case ValidatorTypes.Custom:
							CustomValidator vldCustom = new CustomValidator();
							vldCustom.ClientValidationFunction = validator.ClientValidationFunction;
							vldCustom.ValidateEmptyText = true;
							vldBase = vldCustom;
							break;
					}

					vldBase.Display = ValidatorDisplay.Dynamic;
					vldBase.ControlToValidate = control.ID;
					vldBase.ValidationGroup = ValidationGroup;
					vldBase.CssClass = validator.CssClass;

					if (!string.IsNullOrEmpty(validator.ResourceKey))
					{
						vldBase.ErrorMessage = GetLocalResourceObject(validator.ResourceKey).ToString();
					}
					else
					{
						vldBase.ErrorMessage = validator.ErrorMessage;
					}

					validators.Add(vldBase);
				}
			}
			_layoutManager.AddItem(control, controlLabel, validators);
			item.Control = control;

			//no date fields exist for this module. The user may still select date picker, but it's probably
			//overkill to put the code in place to use date pickers if they won't be used
			//if (Attribute.DisplayType == DisplayTypes.DatePicker)
			//{
			//    _datePickers.Add(control);
			//}
		}


		private string GetMemberAttributeValue(ConfigurationItem item)
		{
			string val = null;
			try
			{
				switch (item.AttributeName)
				{
					case "Name Prefix":
						val = _member.NamePrefix;
						break;
					case "First Name":
						val = _member.FirstName;
						break;
					case "Middle Name":
						val = _member.MiddleName;
						break;
					case "Last Name":
						val = _member.LastName;
						break;
					case "Name Suffix":
						val = _member.NameSuffix;
						break;
					case "Email Address":
						val = _member.PrimaryEmailAddress;
						break;
					default:
						AttributeSetMetaData meta = LoyaltyService.GetAttributeSetMetaData("MemberDetails");
						IList<IClientDataObject> details = null;
						IClientDataObject detail = null;
						if (meta != null)
						{
							details = _member.GetChildAttributeSets("MemberDetails");
							if (details != null && details.Count > 0)
							{
								detail = details[0];

								switch (item.AttributeName)
								{
									case "Address Line 1":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("AddressLineOne"));
										}
										break;
									case "Address Line 2":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("AddressLineTwo"));
										}
										break;
									case "Address Line 3":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("AddressLineThree"));
										}
										break;
									case "Address Line 4":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("AddressLineFour"));
										}
										break;
									case "Country":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("Country"));
										}
										break;
									case "City":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("City"));
										}
										break;
									case "State":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("StateOrProvince"));
										}
										break;
									case "Zip Code":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("ZipOrPostalCode"));
										}
										break;
								}
							}
						}
						break;
				}

			}
			catch (LWMetaDataException ex)
			{
				//This could be thrown because the standard attribute set is not correctly defined, which doesn't completely 
				//kill the functionality of the module, so we'll log the exception and continue on without displaying it to the user.
				_logger.Error(_className, "GetMemberAttributeValue", "Unexpected exception: " + ex.Message, ex);
			}
			catch (Exception)
			{
				throw;
			}
			return val;
		}


		private string GetControlValue(ConfigurationItem item, bool getTextFromListItem = true)
		{
			string val = string.Empty;
			if (item.AttributeType == ItemTypes.Attribute)
			{
				switch (item.DisplayType)
				{
					case DisplayTypes.Label:
						var lbl = item.Control as Label;
						if (lbl != null)
						{
							val = lbl.Text;
						}
						break;
					case DisplayTypes.CheckBox:
						var chk = item.Control as CheckBox;
						if (chk != null)
						{
							val = chk.Checked.ToString();
						}
						break;
					case DisplayTypes.DatePicker:
					case DisplayTypes.TextBox:
						var txt = item.Control as TextBox;
						if (txt != null)
						{
							val = txt.Text;
						}
						break;
					case DisplayTypes.DropDownList:
					case DisplayTypes.RadioList:
						var lst = item.Control as ListControl;
						if (lst != null && lst.SelectedItem != null)
						{
							if (getTextFromListItem)
							{
								val = lst.SelectedItem.Text;
							}
							else
							{
								val = lst.SelectedItem.Value;
							}
						}
						break;
				}
			}
			return val;
		}


		private void AddHtmlBlock(ConfigurationItem item)
		{
			string displayText = string.Empty;

			if (!string.IsNullOrEmpty(item.ResourceKey))
			{
				displayText = ResourceUtils.GetLocalWebResource(_modulePath, item.ResourceKey);
			}
			else
			{
				displayText = item.DisplayText;
			}

			if (!string.IsNullOrEmpty(displayText))
			{
				displayText = ExpressionUtil.ParseExpressions(item.DisplayText, new ContextObject() { Owner = _member });
			}
			_layoutManager.AddHtmlBlock(item.DisplayText);
		}


		private string GetLabelText(ConfigurationItem item)
		{
			return ResourceUtils.GetLocalWebResource(_modulePath, item.ResourceKey, item.DisplayText);
		}


		#endregion
		
	}
}
