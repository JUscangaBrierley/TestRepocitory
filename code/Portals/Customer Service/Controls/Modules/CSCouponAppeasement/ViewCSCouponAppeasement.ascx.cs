using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

namespace Brierley.LWModules.CSCouponAppeasement
{
    public partial class ViewCSCouponAppeasement : ModuleControlBase
	{
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private const string _className = "ViewCSCouponAppeasement";
        private const string _modulePath = "~/Controls/Modules/CSCouponAppeasement/ViewCSCouponAppeasement.ascx";
        private ContextObject _co = null;

        private CSCouponAppeasementConfig _config = null;

		protected override void OnInit(EventArgs e)
		{
			const string methodName = "OnInit";
			try
			{
				base.OnInit(e);							
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
            btnSave.Click += new EventHandler(btnSave_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            pchSuccessMessage.Visible = false;
            try
            {
                _config = ConfigurationUtil.GetConfiguration<CSCouponAppeasementConfig>(ConfigurationKey);
                if (_config == null)
                {
                    _logger.Error(_className, methodName, "Missing configuration.");
                    this.Visible = false;
                    return;
                }

                reqCoupon.ValidationGroup = btnSave.ValidationGroup = ValidationGroup;

                if (!IsPostBack)
                {
                    string title = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleResourceKey);
                    if (string.IsNullOrEmpty(title))
                    {
                        title = _config.ModuleTitleResourceKey;
                    }
                    h2Title.Text = title;
                    pchAdditionalNotes.Visible = _config.DisplayAdditionalNotes;
                    BindCouponList();
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
                    string errMsg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "NoMemberSelected.Text", "No member is selected."));
                    _logger.Error(_className, methodName, errMsg);
                    ShowNegative(errMsg);
                    return;
                }

                if (member.MemberStatus != MemberStatusEnum.Active)
                {
                    string errMsg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "MemberNotActive.Text", "Member is not active.  No rewards can be added."));
                    _logger.Error(_className, methodName, errMsg);
                    ShowNegative(errMsg);
                    return;
                }

                CouponDef coupon = ContentService.GetCouponDef(long.Parse(ddlCoupon.SelectedValue));
                if (coupon == null)
                {
                    //throw new Exception("Unable to locate this reward.");
                    string errMsg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "CannotFindCoupon.Text", "Unable to locate this coupon."));
                    _logger.Error(_className, methodName, errMsg);
                    ShowNegative(errMsg);
                    return;
                }

                string businessRuleName = GetBusinessRuleName(coupon);
                RuleTrigger rt = LoyaltyService.GetRuleByName(businessRuleName);

                _co = new ContextObject();
                _co.Owner = member;

                var env = new Dictionary<string, object>();
                env.Add("CouponName", coupon.Name);
                _co.Environment = env;

                LoyaltyService.Execute(rt, _co);
                string successMsg = ResourceUtils.GetLocalWebResource(_modulePath, _config.SuccessMessageResourceKey);
                if (string.IsNullOrEmpty(successMsg))
                {
                    successMsg = _config.SuccessMessageResourceKey;
                }
                if (!string.IsNullOrEmpty(successMsg))
                {
                    if (successMsg.Contains("##"))
                    {
                        litSuccessMessage.Text = ParseExpressions(successMsg);
                    }
                    else
                    {
                        litSuccessMessage.Text = successMsg;
                    }
                    pchSuccessMessage.Visible = true;
                }

                var agent = PortalState.GetLoggedInCSAgent();

                string globalMsgFormat = ResourceUtils.GetLocalWebResource(_modulePath, _config.GlobalNoteFormat);
                if (string.IsNullOrEmpty(globalMsgFormat))
                {
                    globalMsgFormat = _config.GlobalNoteFormat;
                }
                if (!string.IsNullOrEmpty(globalMsgFormat))
                {
                    var note = new CSNote();
                    note.Note = string.Format(globalMsgFormat, coupon.Name);
                    note.MemberId = member.IpCode;
                    note.CreatedBy = agent.Id;
                    CSService.CreateNote(note);
                }

                if (PortalState.Portal.PortalMode == PortalModes.CustomerService && _config.DisplayAdditionalNotes && txtNotes.Text.Trim() != string.Empty)
                {
                    CSNote note = new CSNote() { Note = txtNotes.Text.Trim() };
                    note.MemberId = member.IpCode;
                    note.CreatedBy = agent.Id;
                    CSService.CreateNote(note);
                }
                txtNotes.Text = string.Empty;
                ddlCoupon.SelectedIndex = 0;

                IpcManager.PublishEvent("MemberUpdated", ConfigurationKey, member);
                IpcManager.PublishEvent("RefreshCoupons", ConfigurationKey, member);
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
            ddlCoupon.SelectedIndex = 0;
            txtNotes.Text = string.Empty;
        }


        private void BindCouponList()
        {
            const string methodName = "BindCouponList";
            try
            {
                ddlCoupon.Items.Clear();

                IList<CouponDef> coupons = new List<CouponDef>();                
                string errMsg = string.Empty;
                string[] tokens = !string.IsNullOrEmpty(_config.CouponsFilter) ? _config.CouponsFilter.Split(';') : new string[] { };
                foreach (string name in tokens)
                {
                    CouponDef def = ContentService.GetCouponDef(name);
                    if (def != null && def.IsActive())
                    {
                        coupons.Add(def);
                    }
                }
                                                
                if (coupons != null && coupons.Count > 0)
                {
                    ddlCoupon.Items.Add(new ListItem(ResourceUtils.GetLocalWebResource(_modulePath, "DefaultSelect.Text", "-- Select --"), string.Empty));
                    foreach (var reward in coupons.OrderBy(r => r.Name))
                    {
                        var li = new ListItem(reward.Name, reward.Id.ToString());
                        ddlCoupon.Items.Add(li);
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

        private string GetBusinessRuleName(CouponDef coupon)
        {
            //search for rule tied directly to reward
            foreach (var rule in _config.CustomRules)
            {
                if (rule.Coupons.Contains(coupon.Name))
                {
                    return rule.BusinessRuleName;
                }
            }

            //search for rule tied to product's category
            foreach (var rule in _config.CustomRules)
            {
                if (rule.Categories.Contains(coupon.CategoryId))
                {
                    return rule.BusinessRuleName;
                }
            }

            //none found, use default business rule name
            return _config.DefaultBusinessRuleName;
        }

	}
}
