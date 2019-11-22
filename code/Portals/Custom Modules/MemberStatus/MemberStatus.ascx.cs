namespace Brierley.AEModules.MemberStatus
{
    #region | Namespace |
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using AmericanEagle.SDK.Global;
    using Brierley.Clients.AmericanEagle.DataModel;
    using System.Reflection;
    using Brierley.WebFrameWork.Controls;
    using Brierley.FrameWork.Common;
    using Brierley.WebFrameWork.Controls.Grid;
    using Brierley.WebFrameWork.Portal.Configuration.Modules;
    using Brierley.WebFrameWork.Portal;
    using Brierley.WebFrameWork.Portal.Configuration;
    using Brierley.WebFrameWork.Ipc;
    using Brierley.ClientDevUtilities.LWGateway;
    #endregion

    #region | Class defination for MemberStatus |
    /// <summary>
    /// Class defination for TerminateMember
    /// </summary>
    public partial class MemberStatus : ModuleControlBase
    {
        #region | Private property declaration |
        /// <summary>
        /// Object for logging
        /// </summary>
        private static LWLogger _logger = LWLoggerManager.GetLogger("TerminateMember");
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// ILWDataService instance
        /// </summary>

        #endregion

        #region | Form user control |
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Bind the Current Status label and dropdown for the new status
                    this.BindStatus();
                    //Bind Termination Reasons as configured in the configuration screen
                    this.BindTerminateReason();
                    //Permission per role
                    this.CheckPermissions(); //AEO-1602
                }
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
            }
        }

        private void BindStatus()
        {
            lblCurrenStatus.Text = "--";
            lblCurrenStatus.Visible = true;
            ddlNewStatus.Items.Clear();

            Dictionary<int, string> MemberStatusValues = new Dictionary<int, string>()
            {
                { 1, "Active"},
                { 4, "Frozen"},
                { 3, "Terminated"}
            };
            //      Active = 1,
            //      Disabled = 2,
            //      Terminated = 3,
            //      Locked = 4,  --Frozen

            Member member = PortalState.GetFromCache("SelectedMember") as Member;
            if (member != null)
            {
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get Current Status and New");
                switch (member.MemberStatus)
                {
                    case MemberStatusEnum.Active:
                        lblCurrenStatus.Text = "Active";
                        MemberStatusValues.Remove(1);
                        ddlNewStatus.Visible = true;
                        break;
                    case MemberStatusEnum.Locked:
                        lblCurrenStatus.Text = "Frozen";
                        MemberStatusValues.Remove(4);
                        ddlNewStatus.Visible = true;
                        break;
                    case MemberStatusEnum.Terminated:
                        lblCurrenStatus.Text = "Terminated";
                        this.lblNewStatus.Visible = false;
                        ddlNewStatus.Visible = false;
                        ddlNewStatus.Enabled = false;
                        //btnSave.Enabled = false;
                        MemberStatusValues.Clear();
                        break;
                    //Used with any other member status
                    default:
                        ddlNewStatus.Visible = false;
                        ddlNewStatus.Enabled = false;
                        btnSave.Enabled = false;
                        lblCurrenStatus.Text = "Other";
                        break;
                }

                string assignedRole = WebUtilities.GetCurrentUserRole().ToLower();



                if (assignedRole == "csr" || assignedRole == "supervisor" /*|| assignedRole == "synchrony" AEO-1881 begin & end*/)
                {
                    MemberStatusValues.Remove(4);
                    MemberStatusValues.Remove(1); //AEO-1627
                }

                // AEO-1881 MMV begin

                // get the CSAgent object of the current signed user
                if (assignedRole.Contains("synchrony"))
                {

                    MemberStatusValues.Remove(1); // to keep aeo-1627 working for synchrony roles
                    using (var csService = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                    {
                        CSAgent agent = csService.GetCSAgentById(WebUtilities.GetCurrentUserId());

                        if (agent == null)
                        {
                            MemberStatusValues.Clear();
                        }
                        else
                        {
                            CSRole role = csService.GetRole(agent.RoleId, true);
                            if (role == null)
                            {
                                MemberStatusValues.Clear();
                            }
                            else
                            {
                                IList<CSFunction> privileges = role.Functions;
                                if (privileges == null || privileges.Count == 0)
                                {
                                    MemberStatusValues.Clear();
                                }
                                else
                                {
                                    Boolean terminateAllowed = false;
                                    Boolean FreezeAllowed = false;
                                    foreach (CSFunction p in privileges)
                                    {
                                        if (p.Name.ToUpper() == "TERMINATE")
                                        {
                                            terminateAllowed = true;
                                        }
                                        if (p.Name.ToUpper() == "FREEZEACCOUNT")
                                        {
                                            FreezeAllowed = true;
                                        }
                                        if (terminateAllowed && FreezeAllowed)
                                        {
                                            break; //if both are turned on then stop checking the list 
                                        }
                                    }

                                    if (!terminateAllowed)
                                    {
                                        MemberStatusValues.Remove(3);
                                    }
                                    else
                                    {
                                        if (!MemberStatusValues.ContainsKey(3))
                                        {
                                            MemberStatusValues.Add(3, "Terminated");
                                        }
                                    }

                                    if (!FreezeAllowed)
                                    {
                                        MemberStatusValues.Remove(4);
                                    }
                                    else
                                    {
                                        if (!MemberStatusValues.ContainsKey(4))
                                        {
                                            MemberStatusValues.Add(3, "Frozen");
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                // AEO-1881 MMV end

                foreach (var item in MemberStatusValues)
                {
                    ddlNewStatus.Items.Add(new ListItem(item.Value, item.Key.ToString()));
                }
            }

        }

        /// <summary>
        /// Save click
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin Save");

                Member member = PortalState.GetFromCache("SelectedMember") as Member;
                IList<IClientDataObject> memberDetails = member.GetChildAttributeSets("MemberDetails");
                MemberDetails mbrDetails = (MemberDetails)memberDetails[0];
                MemberStatusEnum newStatus = (MemberStatusEnum)int.Parse(ddlNewStatus.SelectedItem.Value);


                String strAgentNote = txtNotes.Text;

                if (mbrDetails != null)
                {
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member IpCode - " + member.IpCode.ToString());

                    // 1, "Active"
                    // 3, "Terminated"
                    // 4, "Frozen"
                    //Create System text and attach note to csportal.
                    string strSystem = "Member Status Changed from " + lblCurrenStatus.Text;
                    strSystem += " to " + ddlNewStatus.SelectedItem.Text;

                    MemberStatusHistory statusHistory = new MemberStatusHistory();
                    statusHistory.ChangeDate = DateTime.Now;
                    statusHistory.ChangedBy = PortalState.GetLoggedInCSAgent().Username;
                    statusHistory.FromStatus = (int)member.MemberStatus;
                    statusHistory.ReasonCode = (int)TerminationReason.Other_SeeNotes;


                    switch (ddlNewStatus.SelectedItem.Value)
                    {
                        case "1":

                            //AEO-2054 
                            if (this.lblCurrenStatus.Text == "Frozen")
                            {
                                if (mbrDetails.IsUnderAge.HasValue && (mbrDetails.IsUnderAge.Value))
                                {
                                    ShowWarning("AEMessage| Only members aged 15 and older can be changed from Frozen to Active");
                                    return;
                                }
                                ActivateMember(member, mbrDetails);
                            }
                            break;
                        case "3":
                            TerminateMember(member, mbrDetails);
                            strSystem += "- " + ddlTerminate.SelectedItem.Text;
                            break;
                        case "4":
                            FreezeMember(member, mbrDetails);
                            break;
                    }

                    statusHistory.ToStatus = (int)member.MemberStatus;
                    member.AddChildAttributeSet(statusHistory);

                    mbrDetails.AITUpdate = true;
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Adding Note");

                    CreateStatusNote(member, strAgentNote, strSystem);

                    // Save member Information to Database
                    using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        lwService.SaveMember(member);
                    }
                    Merge.UpdateMemberProactiveMergeMemberStatus(member, newStatus);

                    //**************************//
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End Save");
                    lblSuccess.Text = string.Empty;
                    ddlTerminate.SelectedIndex = 0;
                    txtNotes.Text = string.Empty;

                    //Go to Previous page
                    Response.Redirect(Request.RawUrl);

                }
                else
                {
                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                    ShowWarning("AEMessage|Please select a member.");
                    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowNegative("An error has occured");
            }
        }

        private void FreezeMember(Member member, MemberDetails mbrDetails)
        {
            member.NewStatus = MemberStatusEnum.Locked;
            member.NewStatusEffectiveDate = DateTime.Today;
        }

        private void ActivateMember(Member member, MemberDetails mbrDetails)
        {
            member.NewStatus = MemberStatusEnum.Active;
            member.NewStatusEffectiveDate = DateTime.Today;
        }

        private void TerminateMember(Member member, MemberDetails mbrDetails)
        {
            using (var _dataService = _dataUtil.LoyaltyDataServiceInstance())
            {
                //Only if Terminating the account
                String strTerminate = ddlTerminate.SelectedItem.Text;
                LWCriterion criteriaTerm = new LWCriterion("RefTerminationReason");
                criteriaTerm.Add(LWCriterion.OperatorType.AND, "TerminationReason", strTerminate, LWCriterion.Predicate.Eq);

                IList<IClientDataObject> tmp2 = _dataService.GetAttributeSetObjects(null, "RefTerminationReason", criteriaTerm, new LWQueryBatchInfo() { BatchSize = 1, StartIndex = 0 }, true, false);
                if (tmp2 != null && tmp2.Count > 0)
                {
                    RefTerminationReason Term = (RefTerminationReason)tmp2[0];

                    //Terminate Status
                    _dataService.CancelOrTerminateMember(member, DateTime.Today, String.Empty, true, new MemberCancelOptions());
                    //member.PrimaryEmailAddress = string.Empty; // AEO-1037 AH
                    member.MemberCloseDate = DateTime.Now;

                    // Unlink from ae.com
                    if ((mbrDetails.MemberSource == (int)MemberSource.OnlineAEEnrolled) || (mbrDetails.MemberSource == (int)MemberSource.OnlineAERegistered))
                    {
                        mbrDetails.MemberSource = (int)MemberSource.CSPortalUnlinked;
                    }
                    //Terminate reason
                    mbrDetails.TerminationReasonID = Term.TerminationReasonID;
                }
            }
        }

        private static void CreateStatusNote(Member member, String strAgentNote, String strSystem)
        {
            string _note = strSystem + " - " + strAgentNote;
            using (var ilwcsservice = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
            {
                CSNote note = new CSNote();
                note.Note = _note;
                note.MemberId = member.IpCode;
                note.CreateDate = DateTime.Now;
                note.CreatedBy = WebUtilities.GetCurrentUserId();
                ilwcsservice.CreateNote(note);
            }
        }


        /// <summary>
        /// Cancel click
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ddlTerminate.SelectedIndex = 0;
            txtNotes.Text = string.Empty;
            Response.Redirect(Request.RawUrl);
        }
        #endregion

        #region | Custom method defination |
        /// <summary>
        /// Method that bind Terminate Reason dropdown list
        /// </summary>
        private void BindTerminateReason()
        {
            string lstTerminate = string.Empty;
            int index = 0;

            ddlTerminate.Items.Clear();

            Member member = PortalState.GetFromCache("SelectedMember") as Member;
            if (member != null)
            {
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get Terminate Reason");

                //Getting all Terminate reasons
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    IList<IClientDataObject> tmp = lwService.GetAttributeSetObjects(null, "RefTerminationReason", null, null, true, false);
                    if (tmp != null && tmp.Count > 0)
                    {
                        foreach (IClientDataObject tmpobj in tmp)
                        {

                            RefTerminationReason Term = (RefTerminationReason)tmp[index];

                            lstTerminate = Term.TerminationReason;

                            ddlTerminate.Items.Add(new ListItem(lstTerminate, lstTerminate));

                            index++;
                        }
                    }
                }
            }

            //Add a select notification entry
            ddlTerminate.Items.Insert(0, new ListItem("--Select--", "0"));
        }

        /// <summary>
        /// Method that Remove the ability to submit changes
        /// </summary>
        private void CheckPermissions()
        {
            //AEO-1628 Begin
            Boolean AllowOnlyView = WebUtilities.isRoleAllowedOnlyToView("MemberStatus");
            if (AllowOnlyView)
            {
                this.btnSave.Visible = false;
                this.lblNewStatus.Visible = false;
                this.ddlNewStatus.Visible = false;
                this.lblTerminate.Visible = false;
                this.ddlTerminate.Visible = false;
            }
            //AEO-1628 End

            //AEO-1644 Begin
            Boolean AllowSubmit = WebUtilities.isMemberAllowedToSubmit();
            if (AllowSubmit)
            {
                btnSave.Visible = false;
                btnCancel.Visible = false;
            }
            //AEO-1644 End
        }
        #endregion
    }
    #endregion
}