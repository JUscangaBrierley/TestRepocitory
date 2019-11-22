using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LWModules.Bonuses.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Util;

namespace Brierley.LWModules.Bonuses
{
	public partial class ViewBonuses : ModuleControlBase
	{
		private const string _className = "ViewBonuses";
		private const string _modulePath = "~/Controls/Modules/Bonuses/ViewBonuses.ascx";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private BonusesConfig _config = new BonusesConfig();
		private Member _member = null;
		private List<LinkButton> _categoryLinks = new List<LinkButton>();
		private LinkButton _lnkAll = null;

		protected string ControlBar
		{
			get
			{
				if (_config != null && _config.ShowVideoControls)
				{
					return "bottom";
				}
				return "none";
			}
		}

		protected long ViewingBonusId
		{
			get
			{
				if (ViewState["ViewingBonusId"] != null)
				{
					return (long)ViewState["ViewingBonusId"];
				}
				if (!string.IsNullOrEmpty(hdnBonusId.Value))
				{
					long id = -1;
					if (long.TryParse(hdnBonusId.Value, out id))
					{
						ViewingBonusId = id;
						hdnBonusId.Value = string.Empty;
						return id;
					}
				}
				return 0;
			}
			set
			{
				ViewState["ViewingBonusId"] = value;
			}
		}

		protected long ViewingSurveyId
		{
			get
			{
				if (ViewState["ViewingSurveyId"] != null)
				{
					return (long)ViewState["ViewingSurveyId"];
				}
				return 0;
			}
			set
			{
				ViewState["ViewingSurveyId"] = value;
			}
		}

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

		protected bool PauseVideoAtEnd
		{
			get
			{
				if (_config != null)
				{
					return _config.PauseVideoAtEnd;
				}
				return true;
			}
		}

		protected bool EnableVideoSharing
		{
			get
			{
				if (_config != null)
				{
					return _config.EnableVideoSharing;
				}
				return true;
			}
		}


		protected override void OnInit(EventArgs e)
		{
			_config = ConfigurationUtil.GetConfiguration<BonusesConfig>(ConfigurationKey);
			if (_config == null)
			{
				return;
			}
			if (!IsPostBack && !IsAjaxPostback())
			{
				if (!string.IsNullOrEmpty(Request.QueryString["bonus"]))
				{
					Response.ContentType = "image/gif";
					Response.AppendHeader("Content-Length", WebBug.ImageBytes.Length.ToString());
					Response.BinaryWrite(WebBug.ImageBytes);

					//lookup the bonus and update its status
					var request = Request.QueryString["bonus"];
					if (!string.IsNullOrEmpty(request))
					{
						var bonusId = WebBug.ParseBonusLink(request);
                        if (bonusId > 0)
                        {
                            //get the logged in member, or lookup by cooke if unauthenticated
                            if (PortalState.CurrentMember != null)
                            {
                                _member = PortalState.CurrentMember;
                            }
                            else
                            {
                                long ipCode = WebBug.GetIdentifierCookie();
                                if (ipCode > 0)
                                {
                                    _member = LoyaltyService.LoadMemberFromIPCode(ipCode);
                                }
                            }

                            if (_member == null)
                            {
                                _logger.Trace(_className, "OnInit", string.Format("Unable to determine the member for referral bonus id {0}", bonusId.ToString()));
                            }
                            else
                            {
                                BonusDef bonus = ContentService.GetBonusDef(bonusId);

                                if (bonus != null)
                                {
                                    //we have a bonus and a member. The member may have multiple bonuses of the same type, so we'll need to look at all of them
                                    //and find the first one that's completed but not referral completed.
                                    var memberBonuses = LoyaltyService.GetMemberBonusesByMember(_member.IpCode, null);
                                    if (memberBonuses != null)
                                    {
                                        var memberBonus = memberBonuses.Where(o => o.BonusDefId == bonusId && o.ExpiryDate >= DateTime.Now && o.Status == MemberBonusStatus.Completed && !o.ReferralCompleted).FirstOrDefault();
                                        if (memberBonus != null)
                                        {
                                            RfeUtil.ProcessBonusAction(memberBonus.ID, null, LanguageChannelUtil.GetDefaultCulture(), PortalState.UserChannel, _config.BonusCompletedEventName, _config.AwardPointsRuleId, true);
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.Trace(_className, "OnInit", string.Format("Unable to find bonus with id {0}", bonusId.ToString()));
                                }
                            }
                        }
					}

					Response.End();
				}

				litTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleResourceKey);
				mvMain.SetActiveView(viewBonusList);
			}
			else if (IsAjaxPostback())
			{
				string result = string.Empty;
				string request = Page.Request.Form["bonus"];
				var bonus = GetBonusDetail(long.Parse(request));

				if (bonus.Status == MemberBonusStatus.Completed && bonus.ReferralCompleted)
				{
					//complete
					result = "complete";
				}
				else
				{
					//incomplete
					result = "incomplete";
				}

				Page.Response.ContentType = "application/text; charset=utf-8";
				Page.Response.Clear();
				Page.Response.Write(result);
				Response.End();
				return;
			}
			base.OnInit(e);
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsAjaxPostback())
			{
				return;
			}

			if (_config == null)
			{
				return;
			}

			try
			{
				_member = PortalState.CurrentMember;
				if (_member == null)
				{
					this.Visible = false;
					_logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No member selected");
					return;
				}

				SurveyRunner.SurveyCompleted += new EventHandler(surveyRunner_SurveyCompleted);
				SurveyRunner.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
				SurveyRunner.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
				SurveyRunner.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
				SurveyRunner.ExceptionThrown += SurveyRunner_ExceptionThrown;

				// member clicked 'Finished' at VideoOffer screen
				lnkBonusActionComplete.Click += new EventHandler(lnkBonusActionComplete_Click);

				// member clicked 'Complete' at HtmlOffer page
				lnkButtonNextStep.Click += new EventHandler(lnkBonusActionComplete_Click);

				lstBonuses.ItemCommand += new EventHandler<ListViewCommandEventArgs>(lstBonuses_ItemCommand);
				lstBonuses.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lstBonuses_ItemDataBound);
				//lnkToOffers.Click += new EventHandler(lnkToOffers_Click);
				lnkPreferences.Click += new EventHandler(lnkPreferences_Click);

				lstProfileSurveys.ItemCommand += new EventHandler<ListViewCommandEventArgs>(lstProfileSurveys_ItemCommand);

				lnkBackToOffersFinished.Click += delegate
				{
					mvMain.SetActiveView(viewBonusList);
					BindBonusList();
				};
				lnkBackToOffersProfile.Click += delegate
				{
					mvMain.SetActiveView(viewBonusList);
					BindBonusList();
				};
				lnkBackToOffersHeader.Click += delegate
				{
					mvMain.SetActiveView(viewBonusList);
					BindBonusList();
				};


				BuildCategoryListView();
                if (!IsPostBack)
                {
                    //if we're being directed to a specific bonus, show it instead of showing the list. If the bonus
                    //specified does not exist, we'll fall back on the list instead.
                    long id = 0;
                    if (!string.IsNullOrEmpty(Request.QueryString["memberbonusid"]))
                    {
                        //bonusid is the id of the member bonus
                        long bonusId = 0;
                        if (long.TryParse(Request.QueryString["memberbonusid"], out bonusId))
                        {
                            var memberBonus = LoyaltyService.GetMemberOffer(bonusId);
                            if (memberBonus != null)
                            {
                                id = memberBonus.ID;
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(Request.QueryString["definitionid"]) || !string.IsNullOrEmpty(Request.QueryString["bonusname"]))
                    {
                        //either of these leads to looking up the bonus definition, then retrieving a member bonus by that definition id
                        long defId = 0;
                        if (!string.IsNullOrEmpty(Request.QueryString["bonusname"]))
                        {
                            string name = Request.QueryString["bonusname"];
                            var bonus = ContentService.GetBonusDef(name);
                            if (bonus != null)
                            {
                                defId = bonus.Id;
                            }
                        }
                        else
                        {
                            long.TryParse(Request.QueryString["definitionid"], out defId);
                        }

                        if (defId > 0)
                        {
                            var memberBonuses = LoyaltyService.GetMemberBonusesByMember(PortalState.CurrentMember.IpCode, null, true, defId, null);
                            if (memberBonuses != null && memberBonuses.Count > 0)
                            {
                                id = memberBonuses.First().ID;
                            }
                        }
                    }

                    if (id > 0)
                    {
                        //show the bonus specified in the query string
                        ViewingBonusId = id;
                        ShowBonus();
                    }
                    else
                    {
                        //show the list of available bonuses
                        BindBonusList();

                        var surveys = SurveyManager.RetrieveProfileSurveys();
                        if (surveys.Count == 0)
                        {
                            //no profile surveys = nothing for the preferences link to do, so hide it
                            lnkPreferences.Visible = false;
                        }
                    }
                }
			}
			catch (Exception ex)
			{
				_logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Empty, ex);
				throw;
			}
		}

		void SurveyRunner_ExceptionThrown(object sender, WebFrameWork.MessageHandling.ExceptionEventArgs e)
		{
			if (e != null)
			{
				string message = StringUtils.FriendlyString(e.Message, e.Exception != null ? e.Exception.Message : string.Empty);

				if (message == "You've already answered this question.")
				{
					message = string.Format("You've already answered this question.  <a href=\"{0}\">Please click here to continue.</a>", Request.UrlReferrer);
					Control btnNext = SurveyRunner.GetNextButton();
					if (btnNext != null)
					{
						btnNext.Visible = false;
					}
				}

				ShowNegative(message);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			lnkBackToOffersHeader.Visible = !viewBonusList.Visible && !viewNoBonuses.Visible;
			base.OnPreRender(e);
		}

		void lstBonuses_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			try
			{
				if (e.Item.ItemType == ListViewItemType.DataItem)
				{
					var bonus = e.Item.DataItem as BonusDetail;
					if (bonus != null)
					{
						if (bonus.Id < 1)
						{
							//this is a filler bonus. Filler bonuses cannot be removed by the member
							var removeBonus = e.Item.FindControl("lnkRemove") as LinkButton;
							removeBonus.Visible = false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "lstBonuses_ItemDataBound", null, ex);
			}
		}

		/// <summary>
		/// Show Finished HTML here
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void surveyRunner_SurveyCompleted(object sender, EventArgs e)
		{
			const string methodName = "surveyRunner_SurveyCompleted";

			if (ViewingSurveyId <= 0 || SurveyRunner.StateModelStatus == StateModelStatus.NoSurvey)
				return;

            if (SurveyRunner.TerminationType == StateModelTerminationType.Success || SurveyRunner.TerminationType == StateModelTerminationType.Skip)
            {
                var survey = SurveyManager.RetrieveSurvey(ViewingSurveyId);
                if (survey != null)
                {
                    if (survey.SurveyType == SurveyType.Profile)
                    {
                        mvMain.SetActiveView(viewProfileSurveys);
                        BindProfileSurveyList();
                    }
                    else
                    {
                        var detail = this.GetBonusDetail();
                        RfeUtil.ProcessBonusAction(ViewingBonusId, MemberBonusStatus.Completed, LanguageChannelUtil.GetDefaultCulture(), PortalState.UserChannel, _config.BonusCompletedEventName, _config.AwardPointsRuleId, false);
                        pchFinishedHtml.Controls.Clear();
                        pchFinishedHtml.Controls.Add(new LiteralControl(detail.FinishedHtml));
                        SetReferralLink(detail);
                        mvMain.SetActiveView(viewFinishedHtml);
                    }
                }
            }
            else
            {
                string exception = string.Format("surveyRunner_SurveyCompleted called with unexpected TerminationType {0}, StateModelStatus {1}", SurveyRunner.TerminationType.ToString(), SurveyRunner.StateModelStatus.ToString());
                _logger.Error(_className, "surveyRunner_SurveyCompleted", exception);
                throw new Exception(exception);
            }
		}

		/// <summary>
		/// Fires when user clicks a bonus in the list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        void lstBonuses_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            long bonusId = long.Parse(e.CommandArgument.ToString());
            ViewingBonusId = bonusId;

            if (bonusId < 0)
            {
                //hack: we have a negative bonus id, which means it's a filler bonus (not tied to the member). Get the definition
                //instead of the member bonus.
                var bonusDef = ContentService.GetBonusDef(Math.Abs(bonusId));

                hdnVideoUrl.Value = bonusDef.MovieUrl;
                lnkButtonNextStep.Visible = true; //string.IsNullOrEmpty(bonusDef.ActionUrl);
                pchHtmlOffer.Controls.Add(new LiteralControl(bonusDef.HtmlContent));
                mvMain.SetActiveView(viewHtmlOffer);
            }
            else
            {
                if (e.CommandName == "RemoveBonus")
                {
                    RemoveBonus(bonusId);
                    return;
                }
                ShowBonus();
            }
        }


		void lstProfileSurveys_ItemCommand(object sender, ListViewCommandEventArgs e)
		{
			long surveyId = long.Parse(e.CommandArgument.ToString());
			DisplaySurvey(surveyId, string.Empty, null);
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
					BindBonusList();
				}
			}
		}


		void RemoveBonus(long bonusID)
		{
			const string methodName = "RemoveBonus";
            try
            {
                MemberBonus bonus = LoyaltyService.GetMemberOffer(bonusID);
                bonus.ExpiryDate = DateTimeUtil.MinValue;
                //todo: expiring the bonus will remove it from the list. Did we want to add a status of "Removed" to MemberBonusStatus instead?
                LoyaltyService.UpdateMemberOffer(bonus);
                mvMain.SetActiveView(viewBonusList);
                this.BindBonusList();
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, ex.Message, ex);
                throw;
            }
		}

		/// <summary>
		/// This is called when HtmlOffer is viewed or VideoOffer is watched.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lnkBonusActionComplete_Click(object sender, EventArgs e)
		{
			long id = 0;
			if (long.TryParse(hdnBonusId.Value, out id))
			{
				ViewingBonusId = id;
			}
			hdnBonusId.Value = string.Empty;

			if (ViewingBonusId < 0)
			{
				//a negative id means the user is viewing a filler bonus
				mvMain.SetActiveView(viewBonusList);
				return;
			}

			var detail = GetBonusDetail();

			RfeUtil.ProcessBonusAction(ViewingBonusId, MemberBonusStatus.Viewed, LanguageChannelUtil.GetDefaultCulture(), PortalState.UserChannel, _config.BonusCompletedEventName, _config.AwardPointsRuleId, false);

			if (detail.SurveyId > 0 && detail.Status != MemberBonusStatus.Completed)
			{
				DisplaySurvey(detail.SurveyId.GetValueOrDefault(), detail.SurveyPointsExpression, detail);
			}
			else
			{
				ShowBonusComplete(detail.FinishedHtml);
				SetReferralLink(detail);
			}
		}


		protected void lnkPreferences_Click(object sender, EventArgs e)
		{
			mvMain.SetActiveView(viewProfileSurveys);
			BindProfileSurveyList();
		}


		private void SetReferralLink(BonusDetail detail)
		{
			if (!string.IsNullOrEmpty(detail.ReferralUrl))
			{
				//string thisUrl = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, Request.RawUrl);
				//if (thisUrl.Contains("?"))
				//{
				//	thisUrl = thisUrl.Substring(0, thisUrl.IndexOf("?"));
				//}
				//string url = detail.ReferralUrl;
				//if (url.Contains("?"))
				//{
				//	url += "&bonus=";
				//}
				//else
				//{
				//	url += "?bonus=";
				//}
				//url += Server.UrlEncode(thisUrl + "?bonus=" + WebBug.CreateBonusLink(detail.Id));

				//lnkReferralUrl.NavigateUrl = url;

				//lnkReferralUrl.Text = string.IsNullOrEmpty(detail.ReferralLabel) ? ResourceUtils.GetLocalWebResource(_modulePath, "lnkReferralUrl.Text", "Learn More") : detail.ReferralLabel;

				//// Referral link has moved to detail page. We no longer have any need to redirect back to the bonus list once the
				//// referral has been completed, which means we also have no need to poll the server to see if it's been completed.
				////lnkReferralUrl.Attributes.Add("onclick", "return ReferralIntercept(" + detail.Id + ")");
				//lnkReferralUrl.Visible = true;


				lnkReferralUrl.NavigateUrl = detail.ReferralUrl;

				lnkReferralUrl.Text = string.IsNullOrEmpty(detail.ReferralLabel) ? ResourceUtils.GetLocalWebResource(_modulePath, "lnkReferralUrl.Text", "Learn More") : detail.ReferralLabel;

				// Referral link has moved to detail page. We no longer have any need to redirect back to the bonus list once the
				// referral has been completed, which means we also have no need to poll the server to see if it's been completed.
				//lnkReferralUrl.Attributes.Add("onclick", "return ReferralIntercept(" + detail.Id + ")");
				lnkReferralUrl.Visible = true;

				if (_member != null)
				{
					WebBug.SetIdentifierCookie(_member.IpCode);
				}
			}
			else
			{
				lnkReferralUrl.Visible = false;
			}
		}


        private void BindBonusList()
        {
            const string methodName = "BindBonusList";
            var bonuses = LoyaltyService.GetMemberBonusesByMember(_member.IpCode, null, true, null);

            List<BonusDetail> bonusList = new List<BonusDetail>();
            string contentUrl = LWConfigurationUtil.GetConfigurationValue("LWContentRootURL");
            if (string.IsNullOrEmpty(contentUrl))
            {
                _logger.Error(_className, "BindBonusList", "LWContentRootURL is not defined.");
                throw new LWException("LWContentRootURL is not defined.") { ErrorCode = 1 };
            }
            if (!contentUrl.EndsWith("/"))
            {
                contentUrl += "/";
            }
            contentUrl += LoyaltyService.Organization + "/";

            Func<string, string, string> makeImageUrl = delegate(string content, string url)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    if (url.Contains("://"))
                    {
                        return url;
                    }
                    else
                    {
                        return (contentUrl ?? string.Empty) + url.Replace("\\", "/");
                    }
                }
                return string.Empty;
            };

            if (bonuses != null && bonuses.Count > 0)
            {
                foreach (var bonus in bonuses)
                {
                    if (bonus.ExpiryDate <= DateTime.Now)
                    {
                        _logger.Debug(_className, methodName, "Found expired bonus: " + bonus.ID.ToString());
                        continue;
                    }
                    if (bonus.Status == MemberBonusStatus.Completed)
                    {
                        continue;
                    }

                    BonusDef bonusDef = ContentService.GetBonusDef(bonus.BonusDefId);
                    if (bonusDef == null)
                    {
                        continue;
                    }
                    if (SelectedCategoryId > 0 && bonusDef.CategoryId != SelectedCategoryId)
                    {
                        continue;
                    }
                    if (bonus.ExpiryDate == null && bonusDef.ExpiryDate <= DateTime.Now) // LW-1064
                    {
                        _logger.Debug(_className, methodName, "Found expired bonus: " + bonus.ID.ToString());
                        continue;
                    }
                    if (bonusDef.StartDate > DateTime.Now) // LW-1064
                    {
                        _logger.Debug(_className, methodName, "Found future bonus: " + bonus.ID.ToString());
                        continue;
                    }
                    //double points = bonusDef.Points.GetValueOrDefault();
                    decimal points = 0;
                    if (
                        (bonusDef.Points > 0 &&
                         bonus.Status != MemberBonusStatus.Viewed &&
                         bonus.Status != MemberBonusStatus.Completed &&
                         !string.IsNullOrEmpty(bonusDef.MovieUrl)) ||
                        (bonusDef.SurveyId.GetValueOrDefault() > 0 &&
                         bonus.Status != MemberBonusStatus.Completed)
                        )
                    {
                        if (!string.IsNullOrEmpty(bonusDef.SurveyPointsExpression))
                        {
                            // evaluate the bScript Expression
                            ExpressionFactory factory = new ExpressionFactory();
                            Expression exp = factory.Create(bonusDef.SurveyPointsExpression);
                            object pointsObj = exp.evaluate(new FrameWork.ContextObject());
                            if (pointsObj != null)
                            {
                                points = decimal.Parse(pointsObj.ToString());
                            }
                        }
                    }
                    points += bonusDef.Points.GetValueOrDefault();
                    bonusList.Add(
                        new BonusDetail()
                        {
                            Id = bonus.ID,
                            Headline = bonusDef.Headline,
                            Description = bonusDef.Description,
                            LogoImageHero = makeImageUrl(contentUrl, bonusDef.LogoImageHero),
                            LogoImageWeb = makeImageUrl(contentUrl, (string.IsNullOrEmpty(bonusDef.LogoImageWeb) ? bonusDef.LogoImageHero : bonusDef.LogoImageWeb)),
                            LogoImageMobile = makeImageUrl(contentUrl, (string.IsNullOrEmpty(bonusDef.LogoImageMobile) ? bonusDef.LogoImageHero : bonusDef.LogoImageMobile)),
                            Points = points,
                            DisplayOrder = bonus.DisplayOrder ?? bonusDef.DisplayOrder,
                            IsVideo = string.IsNullOrEmpty(bonusDef.MovieUrl),
                            MovieUrl = bonusDef.MovieUrl,
                            ReferralLabel = bonusDef.ReferralLabel,
                            SurveyText = bonusDef.SurveyText,
                            Status = bonus.Status,
                            SurveyId = bonusDef.SurveyId,
                            GoButtonLabel = bonusDef.GoButtonLabel
                        });
                }
            }

            if (_config.FillerBonuses.Count > 0 && bonusList.Count <= _config.FillerThreshold.GetValueOrDefault())
            {
                foreach (var id in _config.FillerBonuses)
                {
                    var bonusDef = ContentService.GetBonusDef(id);
                    if (bonusDef == null)
                    {
                        continue;
                    }
                    bonusList.Add(
                        new BonusDetail()
                        {
                            //hack - we need to be able to tell what's an actual bonus and what's a filler. Filler bonuses will have
                            //a negative id assigned. When clicking an item, we get the MemberBonus, but a filler bonus is not tied
                            //to the member, so we use the bonus def id. Retrieving a member bonus using the def id would either result
                            //in a null object returned or a clash in ids (a member bonus exists with the same id as the bonus, which would
                            //most likely be the wrong one). In lstBonuses_ItemCommand, we'll check to see if the id is negative, and if so, 
                            //we'll retrieve the bonus def. For positive, we'll retrieve the member bonus as we normally would.
                            Id = -id,
                            Headline = bonusDef.Headline,
                            Description = bonusDef.Description,
                            LogoImageHero = makeImageUrl(contentUrl, bonusDef.LogoImageHero),
                            LogoImageWeb = makeImageUrl(contentUrl, string.IsNullOrEmpty(bonusDef.LogoImageWeb) ? bonusDef.LogoImageHero : bonusDef.LogoImageWeb),
                            LogoImageMobile = makeImageUrl(contentUrl, string.IsNullOrEmpty(bonusDef.LogoImageMobile) ? bonusDef.LogoImageHero : bonusDef.LogoImageMobile),
                            Points = 0,
                            DisplayOrder = bonusDef.DisplayOrder /*int.MaxValue to keep at the end?*/,
                            IsVideo = string.IsNullOrEmpty(bonusDef.MovieUrl),
                            MovieUrl = bonusDef.MovieUrl,
                            ReferralLabel = bonusDef.ReferralLabel,
                            SurveyText = bonusDef.SurveyText,
                            Status = MemberBonusStatus.Issued,
                            SurveyId = bonusDef.SurveyId,
                            GoButtonLabel = bonusDef.GoButtonLabel
                        });
                }
            }

            _lnkAll.Text = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "AllBonusCategories.Text", "All"), bonusList.Count);

            if (bonusList.Count > 0)
            {
                lstBonuses.DataSource = bonusList.OrderBy(o => o.DisplayOrder);
                lstBonuses.DataBind();
                lstBonuses.Visible = true;
            }
            else
            {
                lstBonuses.Items.Clear();
                lstBonuses.DataBind();

                if (!string.IsNullOrEmpty(_config.NoBonusesRouteUrl))
                {
                    Response.Redirect(_config.NoBonusesRouteUrl, false);
                }
                else if (_config.NoBonusesTextBlock.GetValueOrDefault() > 0)
                {
                    TextBlock tb = ContentService.GetTextBlock(_config.NoBonusesTextBlock.Value);
                    if (tb != null)
                    {
                        ContentEvaluator contentEvaluator = new ContentEvaluator(tb.GetContent(), new ContextObject() { Owner = PortalState.CurrentMember });
                        pchNoBonuses.Controls.Add(new LiteralControl("<div id=\"NoBonuses\">"));
                        pchNoBonuses.Controls.Add(new LiteralControl(contentEvaluator.Evaluate("##", 5)));
                        pchNoBonuses.Controls.Add(new LiteralControl("</div>"));
                        mvMain.SetActiveView(viewNoBonuses);
                    }
                }
            }
        }


        private void BindProfileSurveyList()
        {
            var surveys = SurveyManager.RetrieveProfileSurveys();
            lstProfileSurveys.DataSource = surveys;
            lstProfileSurveys.DataBind();
        }


        private void DisplaySurvey(long surveyId, string pointsExpression, BonusDetail bonus)
        {
            const string methodName = "DisplaySurvey";
            mvMain.SetActiveView(viewSurvey);

            SMSurvey survey = SurveyManager.RetrieveSurvey(surveyId);

            if (survey != null)
            {
                decimal points = 0;
                // calculate survey points
                if (!string.IsNullOrEmpty(pointsExpression))
                {
                    // evaluate the bScript Expression
                    ExpressionFactory factory = new ExpressionFactory();
                    Expression exp = factory.Create(pointsExpression);
                    object pointsObj = exp.evaluate(new FrameWork.ContextObject());
                    if (pointsObj != null)
                    {
                        points = decimal.Parse(pointsObj.ToString());
                    }
                }

                ViewingSurveyId = surveyId;
                SMLanguage english = SurveyManager.RetrieveLanguage("English");
                SurveyRunner.SurveyID = survey.ID;
                SurveyRunner.LanguageID = (english != null ? english.ID : 1);
                SurveyRunner.IPCode = _member.IpCode;
                SurveyRunner.SurveyCompletedAccrualPoints = points;
                if (bonus != null)
                {
                    SurveyRunner.OwnerType = PointTransactionOwnerType.Bonus;
                    SurveyRunner.OwnerId = bonus.BonusDefId;
                    SurveyRunner.RowKey = bonus.Id;
                }
                SurveyRunner.Visible = true;
                SurveyRunner.UseAppCache = true;

                //call to IsSurveyCompleted will create the respondent if it doesn't already exist
                //This is needed, otherwise the survey runner will find no respondent and show the
                //"no surveys are available" message
                SurveyManager.IsSurveyCompleted(survey, english, _member);

                SurveyRunner.Rebind();

                if (SurveyRunner.SurveyID == -1)
                {
                    //the survey had an issue loading. If we end up in this state, the bonus will not go away and the member 
                    //will always see the message "No surveys are available!" when clicking the bonus. Instead of showing the 
                    //message, we'll call surveyRunner_SurveyCompleted, which will mark the bonus as completed and the member
                    //will see the finished html instead of the message.
                    _logger.Error(_className, methodName, string.Format("Unable to load survey {0} for member {1}. Skipping the survey.", surveyId, _member.IpCode));
                    surveyRunner_SurveyCompleted(this, new EventArgs());
                }
            }
            else
            {
                SurveyRunner.Visible = false;
                string error = "Survey ID '" + surveyId.ToString() + "' is invalid";
                _logger.Error(_className, methodName, error);
                throw new Exception(error);
            }
        }

		private void ShowBonus()
		{
			//check the status of the bonus. We may need to do one of three or more things: show html offer, show survey, or show the finished html
			var detail = GetBonusDetail();

			switch (detail.Status)
			{
				case MemberBonusStatus.Issued:
					//show video and/or html content
					hdnVideoUrl.Value = detail.MovieUrl;
					//lnkButtonNextStep.Enabled = string.IsNullOrEmpty(detail.ActionUrl);
					if (!string.IsNullOrEmpty(detail.SurveyText))
					{
						lnkButtonNextStep.Text = detail.SurveyText;
					}
					pchHtmlOffer.Controls.Add(new LiteralControl(detail.HtmlContent));
					//SetReferralLink(detail);
					mvMain.SetActiveView(viewHtmlOffer);
					break;
				case MemberBonusStatus.Viewed:
					//show survey
					if (detail.SurveyId.GetValueOrDefault(0) > 0)
					{
						DisplaySurvey(detail.SurveyId.GetValueOrDefault(), detail.SurveyPointsExpression, detail);
					}
					else
					{
						ShowBonusComplete(detail.FinishedHtml);
						SetReferralLink(detail);
					}
					break;
				case MemberBonusStatus.Completed:
					ShowBonusComplete(detail.FinishedHtml);
					SetReferralLink(detail);
					break;
			}
		}

		private void ShowBonusComplete(string htmlContent)
		{
			mvMain.SetActiveView(viewFinishedHtml);
			pchFinishedHtml.Controls.Add(new LiteralControl(htmlContent));
		}


		private BonusDetail GetBonusDetail(long? id = null)
		{
			long bonusId = id.GetValueOrDefault(ViewingBonusId);
			BonusDetail ret = null;
            try
            {
                MemberBonus bonus = LoyaltyService.GetMemberOffer(bonusId);
                if (bonus != null)
                {
                    BonusDef def = ContentService.GetBonusDef(bonus.BonusDefId);
                    if (def != null)
                    {
                        ret = new BonusDetail();
                        ret.Id = ViewingBonusId;
                        ret.BonusDefId = def.Id;
                        ret.HtmlContent = def.HtmlContent;
                        ret.FinishedHtml = def.FinishedHtml;
                        ret.QuotaMetHtml = def.QuotaMetHtml;
                        ret.SurveyText = def.SurveyText;
                        ret.ReferralLabel = def.ReferralLabel;
                        ret.MovieUrl = def.MovieUrl;
                        ret.SurveyId = def.SurveyId.HasValue ? def.SurveyId.Value : 0;
                        ret.Status = bonus.Status;
                        ret.ReferralUrl = def.ReferralUrl;
                        ret.IsVideo = !string.IsNullOrEmpty(def.MovieUrl);
                        ret.Points = def.Points;
                        ret.SurveyPointsExpression = def.SurveyPointsExpression;
                        ret.Quota = def.Quota;
                        ret.ApplyQuotaToReferral = def.ApplyQuotaToReferral.GetValueOrDefault();
                        ret.ReferralCompleted = bonus.ReferralCompleted;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, "GetBonusDetail", ex.Message, ex);
                throw;
            }
		}

		private void BuildCategoryListView()
		{
			var ul = new HtmlGenericControl("ul");
			var liAll = new HtmlGenericControl("li");
			_lnkAll = new LinkButton() { CommandArgument = "0", CssClass = "DashboardLink" };
			_lnkAll.Click += new EventHandler(lnkCategory_Click);
			if (SelectedCategoryId < 1)
			{
				_lnkAll.CssClass += " Current";
			}
			liAll.Controls.Add(_lnkAll);
			ul.Controls.Add(liAll);
			ul.Controls.Add(new LiteralControl("<li class=\"separator\"></li>"));

			_categoryLinks.Add(_lnkAll);

            if (_config.ShowBonusCategories)
            {
                IList<Category> categories = ContentService.GetTopLevelCategoriesByType(CategoryType.Bonus, true);
                if (categories != null)
                {
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

                        ul.Controls.Add(new LiteralControl("<li class=\"separator\"></li>"));
                    }
                }
            }
			pchCategories.Controls.Add(ul);
		}


		private bool IsAjaxPostback()
		{
			return Page.Request.Form["bonus"] != null && Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}
	}
}
