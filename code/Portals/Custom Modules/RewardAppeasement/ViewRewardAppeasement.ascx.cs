namespace Brierley.AEModules.RewardAppeasement
{
    #region | Namespace |
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.FrameWork;
    using System.Reflection;
    using Brierley.Clients.AmericanEagle.DataModel;
    using Brierley.FrameWork.Rules;
    using Brierley.FrameWork.bScript;

    using Brierley.WebFrameWork.Controls;
    using Brierley.WebFrameWork.Controls.Grid;
    using Brierley.WebFrameWork.Portal.Configuration.Modules;
    using Brierley.WebFrameWork.Portal;
    using Brierley.WebFrameWork.Portal.Configuration;
    using Brierley.WebFrameWork.Ipc;
    using Brierley.FrameWork.Data.Sql;
    using System.Linq;
    using Brierley.ClientDevUtilities.LWGateway;
    using AmericanEagle.SDK.Global;
    using System.Text.RegularExpressions;
    #endregion

    #region | Class defination for ViewRewardAppeasement |
    /// <summary>
    /// Class defination for ViewRewardAppeasement
    /// </summary>
    public partial class ViewRewardAppeasement : ModuleControlBase, IIpcEventHandler
    {
        #region | Private property declaration |
        private static LWLogger _logger = LWLoggerManager.GetLogger("RewardAppeasement");
        private static ILWDataServiceUtil _dataUtil = new ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        #endregion
				
        //LW 4.1.14 change
        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }
        public void HandleEvent(IpcEventInfo info)
        {
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Handling IPC event: " + info.EventName);
        }

        #region | Form user control |
        /// <summary>
        /// Page load event
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Check for Appeasement permissions"));
                CheckPermissions();

                if (!IsPostBack)
                {
                    // Set the banner caption and design
                    SetBanner();
                    //Bind Reward as configured in the configuration screen
                    BindReward();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
            }
        }

        private void CheckPermissions()
        {
            Boolean AllowSubmit = WebUtilities.isMemberAllowedToSubmit(); //AEO-1644
            Boolean AllowOnlyView = WebUtilities.isRoleAllowedOnlyToView("ViewRewardAppeasement"); //AEO-1602  ViewRequestCredit
            bool AllowAppeasementAssignmentToAgent = false;
            String rewardName = "";

            if ( !AllowOnlyView ) { // AEO-1881 begin & end
                if ( ddlReward.SelectedItem != null && ddlReward.SelectedItem.Text != "--Select--" ) {
                    rewardName = ddlReward.SelectedItem.Text;
                    AllowAppeasementAssignmentToAgent = WebUtilities.AllowAppeasementAssignmentToMemeber(rewardName);
                }
                else {
                    AllowAppeasementAssignmentToAgent = WebUtilities.AllowAppeasementAssignmentToMemeber();
                }


                if ( !AllowAppeasementAssignmentToAgent ) {
                    string userRole = WebUtilities.GetCurrentUserRole().ToLower();
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Don't display - Current Role: " + userRole));
                    if ( AllowOnlyView ) //AEO-1602
                    {
                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Only administrators can award appeasements"));
                        lblReward.Text = "Only administrators can award appeasements.";
                    }
                    else if (ddlReward.SelectedItem == null || (ddlReward.SelectedItem != null && ddlReward.SelectedItem.Text == "--Select--")) //AEO-2173
                    {
                        lblReward.Text = " Select a reward from the list to issue an appeasement that will be delivered to the member’s email address.";
                    }
                    else {
                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Appeasement assignment limit reached for your role"));
                        lblReward.Text = "Role - " + userRole + ": Limit exceeded for role, max value is " + AppeasementLimits.GetLimitFor(userRole, rewardName);
                    }

                    //lblRewardAmount.Visible = false;
                    ddlReward.Visible = true;   // AEO-1972 Allow drop down to be visible so you can change appeasement types.
                    lblAddNotes.Visible = false;
                    txtNotes.Visible = false;
                    btnSave.Visible = false;
                    btnCancel.Visible = false;
                }
                else {
                    //AEO-1644 Begin
                    if ( AllowSubmit ) {
                        btnSave.Visible = false;
                        btnCancel.Visible = false;
                    }
                    else {
                        btnSave.Visible = true;
                        btnCancel.Visible = true;
                    }
                    //AEO-1644 End

                    lblReward.Text = " Select a reward from the list to issue an appeasement that will be delivered to the member’s email address.";
                    ddlReward.Visible = true;
                    lblAddNotes.Visible = true;
                    txtNotes.Visible = true;
                }
            }
            else {
                // AEO-1881 begin
                ddlReward.Visible = true;   // AEO-1972 Allow drop down to be visible so you can change appeasement types.
                lblAddNotes.Visible = false;
                txtNotes.Visible = false;
                btnSave.Visible = false;
                btnCancel.Visible = false;
                lblReward.Text = " Select a reward from the list to issue an appeasement that will be delivered to the member’s email address.";
                // AEO-1881 end
            }                 
        }

        /// <summary>
        /// Save click
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnSave_Click(object sender, EventArgs e)
        {

            string memberRewardPartNumber = string.Empty;

            try
            {
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                EmailType emailType = EmailType.OnlineReward;
                string strRewardType = string.Empty;
                string strRewardIndicator = string.Empty;
                string strCouponCode = string.Empty;
                String strRuleName = ddlReward.SelectedItem.Text;
                Member member = PortalState.GetFromCache("SelectedMember") as Member;
                IList<IClientDataObject> memberDetails = member.GetChildAttributeSets("MemberDetails");
                MemberDetails mbrDetails = (MemberDetails)memberDetails[0];
                long memberRewardID = 0;

                if ((mbrDetails.EmailAddress != null &&  mbrDetails.PassValidation != null &&mbrDetails.PassValidation.Value == true)  ||
                    (mbrDetails.EmailAddress!= null &&  Utilities.IsEmailValid(mbrDetails.EmailAddress) )) // AEO-628 Begin & end
                {
                    using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                    using (IContentService contentService = _dataUtil.ContentServiceInstance())
                    using (CSService inst = FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                    {
                        ContextObject cobj = new ContextObject();
                        cobj.Owner = member;
                        cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];
                        RuleTrigger ruleTrigger = dataService.GetRuleByName(strRuleName);

                        if (ruleTrigger == null)
                        {
                            _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, strRuleName + " Rule Not Defined");
                            throw new Exception(strRuleName + " Rule Not Defined", new Exception(strRuleName + " Rule Not Defined"));
                        }

                        /*AEO-6156 BEGIN*/
                        //We verify that the rule has a expiration date Defined otherwise throw an error
                        //Note: it's only verifying that the expiration contains a value, it doesn't validate the content
                        string PatternRegexExpirationRule = @"(?<=>)(.*?)(?=<\/)";
                        bool isExpirationDateConfigured = false;
                        var auxCheckExpirationInRule = ruleTrigger.RuleInstance
                            .Split(new[] { "Property" }, StringSplitOptions.None)
                            .Where(item => item.Any(x
                                        => item.Contains("ExpiryDateExpression"))).FirstOrDefault();

                        Match IsExpirationDate = Regex.Match(auxCheckExpirationInRule, PatternRegexExpirationRule, RegexOptions.IgnoreCase);

                        if(IsExpirationDate.Success)
                        {
                            isExpirationDateConfigured = IsExpirationDate.Groups[0].Value.Length > 0;
                        }
                        
                        if(isExpirationDateConfigured == false)
                        {
                            _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, strRuleName + " Expiration Date Not Defined");
                            throw new Exception(strRuleName + " Expiration Date Not Defined", new Exception(strRuleName + " Expiration Date Not Defined"));
                        }
                        /*AEO-6156 END*/

                        string userName = string.Format("'{0}'", WebUtilities.GetCurrentUserName());

                        IssueReward issueRewardRule = (IssueReward)ruleTrigger.Rule;
                        issueRewardRule.ChangedByExpression = userName;

                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, " execute Rule");
                        //Execute the rule
                        dataService.Execute(ruleTrigger, cobj);
                        //AEO-2630 Begin
                        if (cobj != null)
                        {
                            if (cobj.Results.Count > 0)
                            {
                                memberRewardID = ((Brierley.FrameWork.Rules.IssueRewardRuleResult)cobj.Results[0]).RewardId;
                            }
                        }
                        //AEO-2630 End
                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, " add notes");
                        //**************************//
                        //Adding entry in CSNotes table
                        //**************************//
                        string additionalNotes = txtNotes.Text.Trim();

                        CSNote note = new CSNote();
                        note.Note = "Customer Service Reward issued - " + strRuleName;
                        if (additionalNotes.Length > 0)
                        {
                            note.Note += "<br><br>" + additionalNotes;
                        }
                        note.MemberId = member.IpCode;
                        note.CreatedBy = WebUtilities.GetCurrentUserId();

                        inst.CreateNote(note);
                        //**************************//

                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, " done");
                        lblSuccess.Text = string.Empty;
                        ddlReward.SelectedIndex = 0;
                        txtNotes.Text = string.Empty;

                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Send Email");
                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get Member Reward: " + memberRewardID.ToString());
                        //Send the Coupon Code in an email
                        MemberReward memberReward = dataService.GetMemberReward(memberRewardID);

                        if (memberReward != null)
                        {

                            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Got Member Reward: " + memberRewardID.ToString());

                            IList<RewardDef> r = contentService.GetRewardDefsByCertificateType(memberReward.OfferCode);
                       
                            Product product = contentService.GetProduct(memberReward.ProductId);

                            if (product != null)
                            {
                                memberRewardPartNumber = product.PartNumber;
                            }

                            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Check Rule: " + strRuleName);
                            if (strRuleName.Contains("Bra"))
                            {
                                strRewardType = string.Empty;
                                emailType = EmailType.FreeBraAppeasementEmail;
                            }

                            //AEO-489 changes begin here  ----------------------------------SCJ
                            else if (strRuleName.Contains("B5G1 Jean Appeasement"))
                            {

                                emailType = EmailType.B5G1JeanAppeasementEmail;
                            }
                            else if (strRuleName.Contains("$ Reward Appeasement"))
                            {
                                strRewardType = strRuleName.Split('-')[0].Trim().Replace("$", string.Empty);
                                emailType = EmailType.FiveDollarRwdAppeasementEmail;
                                strRewardIndicator = "$";
                            }
                            else if (strRuleName.Contains("Birthday"))
                            {
                                //Birthday email type
                                strRewardType = strRuleName.Split(' ')[0].Trim().Replace("%", string.Empty);
                                emailType = EmailType.BirthdayAppeasementEmail;
                            }
                            else if (strRuleName.EndsWith("% - Appeasement"))
                            {
                                strRewardType = strRuleName.Split('-')[0].Trim().Replace("%", string.Empty);
                                emailType = EmailType.LegacyRewardEmail;
                                strRewardIndicator = "%";
                            }
                            //AEO-2389 BEGIN
                            else if (strRuleName == "20% Choose Your Own Sale Day Appeasement")
                            {
                                strRewardType = strRuleName.Split('-')[0].Trim().Replace("%", string.Empty);
                                emailType = EmailType.ChooseYourOwnSaleDayAppeasement20Email;
                            }
                            //AEO-2389 END
                            //AEO-489 changes end here  ----------------------------------SCJ

                            strCouponCode = memberReward.CertificateNmbr;

                            AEEmail.SendRewardEmail(member, strRewardType, strCouponCode, emailType, mbrDetails.EmailAddress, strRewardIndicator);
                            Utilities.AddMemberRewardFulfillment(member, memberRewardID, "Appeasement", 0, strRewardType, memberRewardPartNumber);
                            memberReward.LWOrderNumber = "1";
                            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "memberReward.OrderNumber = " + memberReward.LWOrderNumber);

                            dataService.UpdateMemberReward(memberReward);
                        }
                    }
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                    CheckPermissions();
                    //Call the IPC manager to publish the RefreshRewards even so the RewardsHistory module will 
                    IpcManager.PublishEvent("RefreshRewards", base.ConfigurationKey, member);

                    //reload the rewards grid.
                    BindReward();
                    Response.Redirect(Request.RawUrl); //ML AEO-263 Prevents duplicate form submission

                }
                else
                {
                    ShowWarning("AEMessage|Please add a valid email address to the member's profile to submit a request for appeasements.");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                if (ex.Message.Contains("No more certificates"))
                {
                    ShowNegative("No more certificates of the type in system");
                }
                else
                    ShowNegative("An error has occured");
            }
            
        }
        /// <summary>
        /// Cancel click
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ddlReward.SelectedIndex = 0;
            txtNotes.Text = string.Empty;
        }

        #endregion

        #region | Custom method defination |

        /// <summary>
        /// SetBanner method
        /// </summary>
        private void SetBanner()
        {
            lblBanner.Text = "Reward Appeasements";
            pnlBanner.Visible = true;
        }

        /// <summary>
        /// Method that bind reward dropdown list
        /// </summary>
        private void BindReward()
        {
            string value = string.Empty;
            string lstReward = string.Empty;
            int IsAppeaseflag = 0;
            DateTime dtCurrentDate = DateTime.Now; //AEO-1689, remove % appeasements            
            decimal currentPointBalance = 0;

            try
            {
                ddlReward.Items.Clear();

                // This condition will execute if Use attribute flag is on
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetMember");

                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get Rewards");
                //Getting all active rewardsdefinitions
                IEnumerable<RewardDef> rewardDefs = new List<RewardDef>();
                using (IContentService contentService = _dataUtil.ContentServiceInstance())
                {
                    rewardDefs= contentService.GetAllRewardDefs().Where(x => x.Active == true && (x.CatalogEndDate > dtCurrentDate || x.CatalogEndDate == null)); //AEO-1689, remove % appeasements
                }                    

                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Current Balance: " + currentPointBalance.ToString());

                //The attribute is empty so just load rewards that don't have this attribute set
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Attribute is empty");
                // added for redesign project
                Member member = PortalState.GetFromCache("SelectedMember") as Member;
                if (member != null)
                {
                    MemberDetails mbrDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                    foreach (RewardDef reward in rewardDefs)
                    {
                        IsAppeaseflag = 0;

                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Check for Appeasements");
                        IsAppeaseflag = CheckExistenceOfAttribute("IsAppeasement", reward);
                        if (IsAppeaseflag == 1)
                        {
                            //AEO-2389 BEGIN
                            //AddItemToList(reward,mbrDetails);//Old code
                            if(mbrDetails!=null)
                            {
                                ddlReward.Items.Add(new ListItem(reward.Name, reward.Name));
                            }   
                            //AEO-2389 END
                        }
                    }

                }

                // Order list so appeasements aren't jumbled
                List<ListItem> listCopy = new List<ListItem>();
                foreach (ListItem item in ddlReward.Items)
                    listCopy.Add(item);
                ddlReward.Items.Clear();
                foreach (ListItem item in listCopy.OrderBy(item => item.Text))
                    ddlReward.Items.Add(item);

                //Add a select notification entry
                ddlReward.Items.Insert(0, new ListItem("--Select--", "0"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// Adding Item to List
        /// </summary>
        /// <param name="reward">Taking reward definition object</param>
        private void AddItemToList(RewardDef reward, MemberDetails mbrDetails)
        {
            try
            {
                string lstReward = string.Empty;
                if (reward.HowManyPointsToEarn > 0)
                {
                    lstReward = reward.Name;
                }
                else
                {
                    lstReward = reward.Name;
                }
                int IsNewProgram = 0;
                IsNewProgram = CheckExistenceOfAttribute("IsNewProgram", reward);

                if (mbrDetails != null)
                {
                    if (reward.Name.Contains("$ Reward Appeasement") || reward.Name.Contains("B5G1") ||
                       reward.Name.Contains("% - Appeasement") || reward.Name.Contains("% Birthday Appeasement"))
                    {
                        ddlReward.Items.Add(new ListItem(lstReward, lstReward));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check the exisitence of particular attribute
        /// </summary>
        /// <param name="attributeToCheck">Name of Attribute to Check</param>
        /// <param name="reward">Object of RewardDefinitions</param>
        /// <returns>If exisits return 1 else 0</returns>
        private int CheckExistenceOfAttribute(string attributeToCheck, RewardDef reward)
        {
            string value = string.Empty;
            int isExists = 0;
            try
            {
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                IList<ContentAttribute> rewardAttributes = reward.Attributes;
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "reward.Attributes: " + reward.Attributes.Count.ToString());
                using (IContentService contentService = _dataUtil.ContentServiceInstance())
                {
                    foreach (ContentAttribute attrib in rewardAttributes)
                    {
                        value = attrib.Value;
                        long attribValue = attrib.ContentAttributeDefId;
                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "attrib.RewardAttributeDefId: " + attrib.ContentAttributeDefId.ToString());


                        ContentAttributeDef rewardAttributeDef = contentService.GetContentAttributeDef(attrib.ContentAttributeDefId);
                        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                        if (rewardAttributeDef.Name == attributeToCheck)
                        {
                            if (value.ToUpper() == "TRUE")
                            {
                                isExists = 1;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isExists;
        }

        protected void ddlReward_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if(IsPostBack)
            //{
            //    CheckPermissions();
            //}
        }
    }
    #endregion

}
