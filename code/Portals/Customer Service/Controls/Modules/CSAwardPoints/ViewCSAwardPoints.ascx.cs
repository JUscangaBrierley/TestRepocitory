using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal;

using Brierley.FrameWork.bScript;
using Brierley.FrameWork;
using Brierley.WebFrameWork.Ipc;

namespace Brierley.LWModules.CSAwardPoints
{
	public partial class ViewCSAwardPoints : ModuleControlBase
	{
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private const string _className = "ViewCSAwardPoints";

		private CSAwardPointsConfig _config = null;

		protected override void OnInit(EventArgs e)
		{
			const string methodName = "OnInit";
			try
			{
				base.OnInit(e);

				_config = ConfigurationUtil.GetConfiguration<CSAwardPointsConfig>(ConfigurationKey);
				if (_config == null)
				{
					this.Visible = false;
					return;
				}

				bool valid = true;
				if (
					_config.PointTypesAndEvents == null ||
					_config.PointTypesAndEvents.Count == 0
					)
				{
					valid = false;
				}

				if (!valid)
				{
					this.Visible = false;
					return;
				}

				rqTxnDate.ValidationGroup = reqPoints.ValidationGroup = vldPoints.ValidationGroup = btnSave.ValidationGroup = ValidationGroup;

				var distinctTypes = (from x in _config.PointTypesAndEvents select x.PointTypeId).Distinct();
				var distinctEvents = new List<long>();
				foreach (var pointType in _config.PointTypesAndEvents)
				{
					foreach (var pointEvent in pointType.PointEvents)
					{
						if (!distinctEvents.Contains(pointEvent))
						{
							distinctEvents.Add(pointEvent);
						}
					}
				}

				if (distinctTypes != null && distinctTypes.Count() > 1)
				{
					pchPointType.Visible = true;
				}
				else
				{
					pchPointType.Visible = false;
					if (distinctTypes != null && distinctTypes.Count() == 1)
					{
						hdnSelectedPointType.Value = distinctTypes.First().ToString();
					}
				}


				if (distinctEvents != null && distinctEvents.Count() > 1)
				{
					pchPointEvent.Visible = true;
				}
				else
				{
					pchPointEvent.Visible = false;
					if (distinctEvents != null && distinctEvents.Count() == 1)
					{
						hdnSelectedPointEvent.Value = distinctEvents.First().ToString();
					}
				}

                litTitle.Text = (GetLocalResourceObject(_config.ModuleTitle) ?? _config.ModuleTitle).ToString();

				if (!_config.AllowDateOverride)
				{
					pchPointAwardDate.Visible = false;
				}
				else
				{
					rdpPointTxnDate.MinDate = DateTimeUtil.MinValue;
					rdpPointTxnDate.MaxDate = _config.AllowFutureDate ? DateTimeUtil.MaxValue : DateTime.Today;
				}

				if (_config.DefaultToCurrentDate)
				{
					rdpPointTxnDate.SelectedDate = DateTime.Today;
				}

				if (!_config.DisplayNotes)
				{
					pchAdditionalNotes.Visible = false;
				}

				if (_config.AllowFractionalPoints)
				{
					vldPoints.Type = ValidationDataType.Double;
					vldPoints.MinimumValue = _config.AllowNegativePoints ? int.MinValue.ToString() : "0.01";
				}
				else
				{
					vldPoints.Type = ValidationDataType.Integer;
					vldPoints.MinimumValue = _config.AllowNegativePoints ? int.MinValue.ToString() : "1";
				}

				//hack: website EditCSAwardPoints.ascx was allowing zero to be set for max points. This causes an ugly exception
				//when loading the module if negative points are not allowed because vldPoints gets a min value of either 1 or 0.01
				//and a max of 0, which causes it to throw an exception. We'll set the max points to 1 in that case to avoid the 
				//exception. A user will hopefully notice the max only allows one point and change the configuration, rather than 
				//complain about the exception being thrown.
				if (_config.MaxPointsPerMember < 1 && !_config.AllowNegativePoints)
				{
					vldPoints.MaximumValue = "1";
				}
				else
				{
					vldPoints.MaximumValue = _config.MaxPointsPerMember.ToString();
				}
				if (PortalState.Portal != null && PortalState.Portal.PortalMode == PortalModes.CustomerService)
				{
					var agent = PortalState.GetLoggedInCSAgent();
					var role = CSService.GetRole(agent.RoleId, true);

                    // Roll back the code that checks agent's access to AllowPointAward function
                    // Please look at comments in PI 19822 and PI 25558
                    //if (role != null)
                    //{
                    //    if (!role.HasFunction(CSFunction.LW_CSFUNCTION_ALLOWPOINTAWARD))
                    //    {
                    //        this.txtPoints.Enabled = false;
                    //        this.btnSave.Enabled = false;
                    //        this.btnCancel.Enabled = false;
                    //    }
                    //    else if (role.PointAwardLimit.HasValue &&
                    //             role.PointAwardLimit.Value < _config.MaxPointsPerMember)
                    //    {
                    //        if ((double)role.PointAwardLimit.Value < double.Parse(vldPoints.MinimumValue))
                    //        {
                    //            this.Visible = false;
                    //            return;
                    //        }
                    //        vldPoints.MaximumValue = role.PointAwardLimit.Value.ToString();
                    //    }
                    //}
                    if (role != null &&
                        role.PointAwardLimit.HasValue &&
                        role.PointAwardLimit.Value < _config.MaxPointsPerMember)
                    {
                        if ((double)role.PointAwardLimit.Value < double.Parse(vldPoints.MinimumValue))
                        {
                            this.Visible = false;
                            return;
                        }
                        vldPoints.MaximumValue = role.PointAwardLimit.Value.ToString();
                    }
				}
				if (!string.IsNullOrEmpty(vldPoints.ErrorMessage) && vldPoints.ErrorMessage.Contains("{0}"))
				{
					vldPoints.ErrorMessage = string.Format(vldPoints.ErrorMessage, vldPoints.MinimumValue, vldPoints.MaximumValue);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			const string methodName = "Page_Load";
			try
			{
				string validationGroup = this.ClientID + "_CSAwardPoints_" + ConfigurationKey.ConfigName;
				rqTxnDate.ValidationGroup = validationGroup;
				reqPoints.ValidationGroup = validationGroup;
				vldPoints.ValidationGroup = validationGroup;
				btnSave.ValidationGroup = validationGroup;

				btnSave.Click += new EventHandler(btnSave_Click);
				btnCancel.Click += new EventHandler(btnCancel_Click);
				pchSuccessMessage.Visible = false;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			const string methodName = "OnPreRender";
			try
			{
                if (_config != null && _config.PointTypesAndEvents != null && _config.PointTypesAndEvents.Count > 0)
                {
                    var pointTypes = LoyaltyService.GetAllPointTypes();
                    var pointEvents = LoyaltyService.GetAllPointEvents();

                    var sb = new StringBuilder();
                    sb.AppendLine("<script type=\"text/javascript\">");
                    sb.AppendLine(this.ClientID + "_pointTypes = [");

                    bool firstType = true;
                    foreach (var rule in _config.PointTypesAndEvents)
                    {
                        var pointType = pointTypes.Where(o => o.ID == rule.PointTypeId).FirstOrDefault();
                        if (pointType != null)
                        {
                            if (!firstType)
                            {
                                sb.AppendLine(", ");
                            }
                            firstType = false;

                            //new PointType(1, 'Some Point Type', [ new PointEvent(1, 'some event') ])
                            sb.Append("new " + this.ClientID + "_PointType(");
                            sb.Append(rule.PointTypeId);
                            sb.Append(", '");
                            sb.Append(pointType.Name.Replace("'", "\\'"));
                            sb.Append("', [");

                            bool firstEvent = true;
                            foreach (var eventId in rule.PointEvents)
                            {
                                var pointEvent = pointEvents.Where(o => o.ID == eventId).FirstOrDefault();
                                if (pointEvent != null)
                                {
                                    if (!firstEvent)
                                    {
                                        sb.Append(", ");
                                    }
                                    firstEvent = false;

                                    sb.Append("new " + this.ClientID + "_PointEvent(");
                                    sb.Append(eventId.ToString());
                                    sb.Append(", '");
                                    sb.Append(pointEvent.Name.Replace("'", "\\'"));
                                    sb.Append("')");
                                }
                            }


                            sb.Append("])");

                        }
                    }

                    sb.AppendLine("];");
                    sb.AppendLine("</script>");

                    Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID + "_PointEvents", sb.ToString());
                }


				base.OnPreRender(e);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			const string methodName = "btnCancel_Click";
			try
			{
				litSuccessMessage.Text = string.Empty;
				pchSuccessMessage.Visible = false;
				ResetForm();
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

        /// <summary>
        /// The behavior and messaging should also be consistent with AwardLoyaltyCurrency operation of the service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
            const string methodName = "btnSave_Click";
            try
            {
                Member member = PortalState.CurrentMember;
                PointType pointType = null;
                PointEvent pointEvent = null;
                var points = decimal.Parse(txtPoints.Text);

                #region Check for the correct points
                if (points < 0 && !_config.AllowNegativePoints)
                {
                    string errMsg = string.Format(GetLocalResourceObject("NegPointsNotAllowed.Text").ToString());
                    _logger.Error(_className, methodName, errMsg);
                    AddInvalidField(errMsg, txtPoints);
                    return;
                }

                if (!_config.AllowFractionalPoints)
                {
                    decimal p = System.Math.Round(points);
                    if (p != points)
                    {
                        string errMsg = string.Format(GetLocalResourceObject("FractionPointsNotAllowed.Text").ToString());
                        _logger.Error(_className, methodName, errMsg);
                        AddInvalidField(errMsg, txtPoints);
                        return;
                    }
                }

                if (_config.RoundUpPoints)
                {
                    decimal p = System.Math.Ceiling(points);
                    if (p != points)
                    {
                        string errMsg = string.Format(GetLocalResourceObject("FractionPointsRounded.Text").ToString(), points, p);
                        _logger.Debug(_className, methodName, errMsg);
                        points = p;
                    }
                }
                #endregion

                if (member.MemberStatus == MemberStatusEnum.PreEnrolled)
                {
                    if (!_config.AllowPreEnrolled)
                    {
                        string errMsg = string.Format(GetLocalResourceObject("PreEnrolledAppeasementNotAllowed.Text").ToString());
                        _logger.Error(_className, methodName, errMsg);
                        ShowNegative(errMsg);
                        return;
                    }
                }
                else if (member.MemberStatus != MemberStatusEnum.Active)
                {
                    string errMsg = string.Format(GetLocalResourceObject("MemberStatusNotAllowedPoints.Text").ToString(), member.MemberStatus.ToString());
                    _logger.Error(_className, methodName, errMsg);
                    ShowNegative(errMsg);
                    return;
                }

                // determine the point types and events to be used for limit checking                
                var pointtypes = from pr in _config.PointTypesAndEvents select pr.PointTypeId;
                Dictionary<long, long> eventidmap = new Dictionary<long, long>();
                foreach (CSAwardPointsConfig.PointRule pr in _config.PointTypesAndEvents)
                {
                    var elist = from eid in pr.PointEvents select eid;
                    foreach (long eid in elist)
                    {
                        if (!eventidmap.ContainsKey(eid))
                        {
                            eventidmap.Add(eid, eid);
                        }
                    }
                }
                long[] pointevents = eventidmap.Keys.ToArray<long>();


                var card = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                if (card == null)
                {
                    string errMsg = string.Format(GetLocalResourceObject("NoPrimaryCard.Text").ToString());
                    _logger.Error(_className, methodName, errMsg);
                    ShowNegative(errMsg);
                    return;
                }

                if (card.Status != VirtualCardStatusType.Active)
                {
                    string errMsg = string.Format(GetLocalResourceObject("VCStatusNotAllowedPoints.Text").ToString(), card.LoyaltyIdNumber, card.Status.ToString());
                    _logger.Error(_className, methodName, errMsg);
                    //throw new LWException(errMsg) { ErrorCode = 3307 };
                    ShowNegative(errMsg);
                    return;
                }

                long hdnPointType = StringUtils.FriendlyInt64(hdnSelectedPointType.Value);
                if (hdnPointType == -1)
                {
                    string errMsg = GetLocalResourceObject("NoPrimaryCard.Text").ToString();
                    _logger.Error(_className, methodName, errMsg);
                    throw new Exception(errMsg);
                }
                pointType = LoyaltyService.GetPointType(hdnPointType);

                long hdnPointEvent = StringUtils.FriendlyInt64(hdnSelectedPointEvent.Value);
                if (hdnPointEvent == -1)
                {
                    string errMsg = GetLocalResourceObject("PointEventsNotConfigured.Text").ToString();
                    _logger.Error(_className, methodName, errMsg);
                    throw new Exception(errMsg);
                }
                pointEvent = LoyaltyService.GetPointEvent(hdnPointEvent);

                DateTime expirationDate = DateTimeUtil.MaxValue;
                Expression expression = null;
                if (!string.IsNullOrEmpty(_config.PointExpirationExpression))
                {
                    expression = new ExpressionFactory().Create(_config.PointExpirationExpression);
                    ContextObject context = new ContextObject() { Owner = member };
                    if (card != null)
                    {
                        context.Owner = card;
                    }
                    expirationDate = Convert.ToDateTime(expression.evaluate(new ContextObject()));
                }

                DateTime txnDate = DateTime.Now;
                if (_config.AllowDateOverride)
                {
                    txnDate = rdpPointTxnDate.SelectedDate.GetValueOrDefault(DateTime.Now);
                }

                var agent = PortalState.GetLoggedInCSAgent();

                #region Limit Checking
                CSRole role = CSService.GetRole(agent.RoleId, false);
                var vcKeys = from vc in member.LoyaltyCards select vc.VcKey;
                DateTime startDate = DateTimeUtil.GetBeginningOfDay(DateTime.Now);    // default is per day
                DateTime endDate = DateTimeUtil.GetEndOfDay(DateTime.Now);

                if (!string.IsNullOrEmpty(_config.MaxPointsStartDateExpression))
                {
                    expression = new ExpressionFactory().Create(_config.MaxPointsStartDateExpression);
                    startDate = Convert.ToDateTime(expression.evaluate(new ContextObject()));
                }
                if (!string.IsNullOrEmpty(_config.MaxPointsEndDateExpression))
                {
                    expression = new ExpressionFactory().Create(_config.MaxPointsEndDateExpression);
                    endDate = Convert.ToDateTime(expression.evaluate(new ContextObject()));
                }

                // check to see that the requested points are not greater than the limit.
                PointBankTransactionType[] txnTypes = new PointBankTransactionType[] { PointBankTransactionType.Credit, PointBankTransactionType.Debit };
                decimal pointsByAgent = LoyaltyService.GetPointBalance(vcKeys.ToArray<long>(), pointtypes.ToArray<long>(),
                    pointevents, txnTypes, null, null, startDate, endDate, agent.Username, null, null, null, null);
                if (role.PointAwardLimit != null && points > role.PointAwardLimit)
                {
                    string msg = string.Format(GetLocalResourceObject("PointsOverAgentsLimit.Text").ToString());
                    _logger.Error(_className, methodName, "Requested points are more than the agent's limit.");
                    AddInvalidField(msg, txtPoints);
                    return;
                }
                // check to make sure that the requested points will not tip over the agents per member 
                // limit for the configured period
                if (role.PointAwardLimit != null && (pointsByAgent + points) > role.PointAwardLimit)
                {
                    string msg = string.Format(GetLocalResourceObject("AgentsPointLimitReached.Text").ToString(), role.PointAwardLimit);
                    _logger.Error(_className, methodName, string.Format("The limit of {0} points for this role has already been reached.",role.PointAwardLimit));
                    AddInvalidField(msg, txtPoints);
                    return;
                }
                // now check the limit of points per member for the configured period
                decimal memberLimit = _config.MaxPointsPerMember;

                // check to make sure that requested points will not tip over the member's limit.
                decimal pointsByMember = LoyaltyService.GetPointBalance(vcKeys.ToArray<long>(), pointtypes.ToArray<long>(),
                    pointevents, txnTypes, null, null, startDate, endDate, null, null, null, null, null);
                if ((pointsByMember + points) > memberLimit)
                {
                    string msg = string.Format(GetLocalResourceObject("MemberPointLimitReached.Text").ToString(), memberLimit);
                    _logger.Error(_className, methodName, string.Format("The limit of {0} points for this member has already been reached.", memberLimit));
                    AddInvalidField(msg, txtPoints);
                    return;
                }
                #endregion

                #region Point Award and Rule Processing
                string location = PortalState.Portal.PortalMode == PortalModes.CustomerService ? "Customer Service" : "Customer Facing";
                if (points >= 0)
                {
                    _logger.Trace(_className, methodName,
                        string.Format(GetLocalResourceObject("AgentAwardingPointsMessage.Text").ToString(), points, member.IpCode, agent.Username));
                    LoyaltyService.Credit(card, pointType, pointEvent, points, string.Empty, txnDate, expirationDate, location, agent.Username);
                }
                else
                {
                    decimal dpoints = Math.Abs(points); // debit will make the points negative.
                    _logger.Trace(_className, methodName,
                        string.Format(GetLocalResourceObject("AgentDebitingPointsMessage.Text").ToString(), dpoints, member.IpCode, agent.Username));
                    LoyaltyService.Debit(card, pointType, pointEvent, dpoints, txnDate, expirationDate, location, agent.Username);
                }

                if (_config.UserEventId != 0)
                {
                    LWEvent lwevent = LoyaltyService.GetLWEvent(_config.UserEventId);
                    _logger.Trace(_className, methodName, "Executing business rules in user defined event " + lwevent.Name);
                    ContextObject context = new ContextObject() { Owner = card };
                    LoyaltyService.ExecuteEventRules(context, lwevent.Name, RuleInvocationType.Manual);
                }

                if (!string.IsNullOrEmpty(_config.GlobalNoteFormat))
                {
                    var note = new CSNote();
                    note.Note = string.Format(_config.GlobalNoteFormat, points, pointType.Name, pointEvent.Name);
                    note.MemberId = member.IpCode;
                    note.CreatedBy = agent.Id;
                    CSService.CreateNote(note);
                }

                if (_config.DisplayNotes && !string.IsNullOrEmpty(txtNotes.Text.Trim()))
                {
                    var note = new CSNote();
                    note.Note = txtNotes.Text.Trim();
                    note.MemberId = member.IpCode;
                    note.CreatedBy = agent.Id;
                    CSService.CreateNote(note);
                }
                #endregion

                litSuccessMessage.Text = (GetLocalResourceObject(_config.SuccessMessage) ?? _config.SuccessMessage).ToString();
                pchSuccessMessage.Visible = true;
                IpcManager.PublishEvent("MemberUpdated", ConfigurationKey, PortalState.CurrentMember);
                ResetForm();
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
                throw;
            }
		}


		private void ResetForm()
		{
			if (_config.DefaultToCurrentDate)
			{
                rdpPointTxnDate.SelectedDate = DateTime.Today;
			}
			else
			{
                rdpPointTxnDate.SelectedDate = null;
			}
			txtPoints.Text = string.Empty;
			txtNotes.Text = string.Empty;
		}

	}
}
