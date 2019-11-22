namespace Brierley.AEModules.AwardPoints
{
    #region | Namespace |
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.WebFrameWork.Controls;
    using Brierley.WebFrameWork.Portal;
    using Brierley.WebFrameWork.Portal.Configuration;
    using Brierley.WebFrameWork.Ipc;
    using AmericanEagle.SDK.Global;
    using System.Text; // AEO Redesing 2015 Begin & end
    using Brierley.Clients.AmericanEagle.DataModel;
    using System.Reflection;
    using Brierley.ClientDevUtilities.LWGateway;
    #endregion

    #region Class defination for View Award Points
    /// <summary>
    /// Class defination for ViewAwardPoints
    /// </summary>
    public partial class ViewAwardPoints : ModuleControlBase, IIpcEventHandler

    {
        #region | Property defination |
        private static LWLogger _logger = LWLoggerManager.GetLogger("AwardPoints");
        private static ILWDataServiceUtil _dataUtil = new ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        #endregion

        #region | Usercontrol event defination |
        //LW 4.1.14 change
        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }
        public void HandleEvent(IpcEventInfo info)
        {
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Handling IPC event: " + info.EventName);
        }
        /// <summary>
        /// Page Load events
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void Page_Load(object sender, EventArgs e)
        {            
            try
            {
                if (!IsPostBack)
                {
                    pnlForm.Visible = false;

                    pnlPointType.Visible = false;
                    rqPointType.Enabled = false;

                    pnlTransDate.Visible = false;
                    rqAwardDate.Enabled = false;

                    pnlNotes.Visible = false;

                    lblBanner.Text = "Bonus Points";
                    pnlBanner.Visible = true;

                    //Bind point event to dropdown list
                    BindPointEvent();
                }

            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
            }
        }

        /// <summary>
        /// Award Bonus button click
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnAwardBonus_Click(object sender, EventArgs e)
        {

            // AEO-Redesing 2015 Begin
            try   {
                lblSuccess.Text = string.Empty;
                pnlMain.Visible = false;
                pnlForm.Visible = true;

                StringBuilder lineMsg = new StringBuilder("Current Qualifying Bras: ");
                Member member = PortalState.GetFromCache("SelectedMember") as Member;
                IList<IClientDataObject> details = member.GetChildAttributeSets("MemberDetails", true);
                MemberDetails mbrDet = (details != null && details.Count  > 0 ? details[0]: null) as MemberDetails;


                DateTime endDate = new DateTime(2199, 12, 31);

                String tmpMessage = Utilities.GetB5G1CurrentQualifyingPurchased(member, endDate,"BRA", false);
                lineMsg.Append(tmpMessage == null ? string.Empty : tmpMessage);

                this.lblBraMessage.Text = lineMsg.ToString();

                lineMsg.Clear();
                if ( mbrDet != null && Utilities.isInPilot(mbrDet.ExtendedPlayCode)) { // point conversion
                                     
                    lineMsg.Append("Current Qualifying Jeans: ");
                    tmpMessage = Utilities.GetJeansCurrentQualifyingPurchased(member, endDate);
                    lineMsg.Append(tmpMessage);

                }

                //AEO-1644 Begin
                Boolean AllowSubmit = WebUtilities.isMemberAllowedToSubmit();
                if (AllowSubmit)
                {
                    this.bntSave.Visible = false;
                    this.btnCancel.Visible = false;
                }//AEO-1644 End
                else
                {
                    //AEO-1602 Begin
                    Boolean AllowOnlyView = WebUtilities.isRoleAllowedOnlyToView("ViewAwardPoints");
                    if (AllowOnlyView)
                    {
                        this.bntSave.Visible = false;
                    } //AEO-1602 End
                    else
                    {
                        this.btnCancel.Visible = true;
                        this.bntSave.Visible = true;
                    }
                }                
                            
                this.lblJeanMessage.Text = lineMsg.ToString();
               
            }
            catch (Exception anError) {
                _logger.Error (MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, anError.Message, anError );
                ShowNegative(anError.ToString());
           }
                       
            // AEO-Redesing 2015 End

        }

        /// <summary>
        /// Cancel button click
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                pnlForm.Visible = false;
                pnlMain.Visible = true;
                lblSuccess.Text = string.Empty;
                ResetScreen();
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert(ex);
                 ShowNegative(ex.ToString());
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
        }

        
        /// <summary>
        /// Save button click
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            
            string sPointType = string.Empty, sPointEvent = string.Empty, sExpireDate = string.Empty, sTransDate = string.Empty;
            string strCSnotes = string.Empty;

            try
            {
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                List<string> aList = new List<string>(new string[] { "BRA CREDIT APPEASEMENT", "JEAN CREDIT APPEASEMENT" });
                bool braSelected = ddlPointEvent.SelectedItem != null && aList.IndexOf(ddlPointEvent.SelectedItem.Text.ToUpper()) == 0;
                bool jeanSelected = ddlPointEvent.SelectedItem != null && aList.IndexOf(ddlPointEvent.SelectedItem.Text.ToUpper()) == 1;
                bool braOrJeanCreditSelected = braSelected || jeanSelected;
                if (braSelected)
                {
                    sPointType = "Bra Points";
                    sPointEvent = "Bra Credit Appeasement";
                }
                else if (jeanSelected)
                {
                    sPointType = "Jean Points";
                    sPointEvent = "Jean Credit Appeasement";
                }
                else if (ddlPointEvent.SelectedItem.Text == "Customer Service")
                {
                    sPointType = "CS Points";
                    sPointEvent = "Customer Service Points Adjustment";
                }
                else
                {
                    sPointType = "CS Points";
                    sPointEvent = ddlPointEvent.SelectedItem.Text;
                }
                //AEO 193 changes end  here -------------------------------SCJ

                Member member = PortalState.GetFromCache("SelectedMember") as Member;
               
                //Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID("1");
                VirtualCard objVCard = member.GetLoyaltyCardByType(Brierley.FrameWork.Common.VirtualCardSearchType.PrimaryCard);


                // AEO-Redesign-2015 (AEO-173 ) Begin
                IList<IClientDataObject> details = member.GetChildAttributeSets("MemberDetails", true);
                MemberDetails mbrDet = ( details != null && details.Count > 0 ? details[0] : null ) as MemberDetails;

                //AEO-1605 BEGIN
                string assignedRole = WebUtilities.GetCurrentUserRole().ToLower();
                if ( assignedRole == "supervisor" || assignedRole == "csr" ||
                     assignedRole == "synchrony csr" || assignedRole == "synchrony admin" ) // AEO-1881
                {
                    long Points = 0;
                    long MaxPoints = 0;
                    long LimitPoints = 0;
                    string OptionSelected = string.Empty;
                    string limitErrorMsg = string.Empty;
                    DateTime startDate = DateTime.Now.Date;
                    DateTime endDate = startDate.AddDays(1);

                    string limitErrorMsg1 = string.Empty;
                    limitErrorMsg1 = "Range Dates: " + startDate.ToString() + "/" + endDate.ToString();
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, limitErrorMsg1);
                    
                    //We get the limit points for the roles and point event
                    MaxPoints = AppeasementLimits.GetLimitFor(assignedRole, ddlPointEvent.SelectedItem.Text);
                    if(braSelected)
                    {
                        OptionSelected = "braSelected";
                    }
                    else if(jeanSelected)
                    {
                        OptionSelected = "jeanSelected";
                    }
                    else
                    {
                        OptionSelected = "OtherPoints";
                    }
                    //We get the current issued points for the member.
                    Points = CheckBraJeanPoints(OptionSelected, startDate, endDate, WebUtilities.GetCurrentUserName());

                    limitErrorMsg1 = "Points: " + Points.ToString();
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, limitErrorMsg1);

                    if (Points > MaxPoints)
                    {
                        if (braSelected)
                        {
                            limitErrorMsg = "The member has reached the limit of " + MaxPoints.ToString() + " points for bra credits per day.";
                        }
                        else if (jeanSelected)
                        {
                            limitErrorMsg = "The member has reached the limit of " + MaxPoints.ToString() + " points for jean credits per day.";
                        }
                        else
                        {
                            ShowWarning("AEMessage|AwardPoints|The number of credits that you entered exceeds the maximum number of credits allowed for this adjustment per day");
                        }

                        ShowWarning("AEMessage|AwardPoints|" + limitErrorMsg);
                        return;
                    }
                    else
                    {
                        if (double.Parse(txtPoints.Text.Trim()) > MaxPoints)
                        {
                            LimitPoints = MaxPoints + 1;
                            ShowWarning("AEMessage|AwardPoints|Please enter a point amount less than " + LimitPoints.ToString());
                            return;
                        }
                        else
                        {
                            Points = Points + long.Parse(txtPoints.Text.Trim());
                            if (Points > MaxPoints)
                            {
                                ShowWarning("AEMessage|AwardPoints|The number of credits that you entered exceeds the maximum number of credits allowed for this adjustment per day");
                                return;
                            }
                        }
                    }
                }
                //AEO-1605 END

                sTransDate = System.DateTime.Now.ToString("MM/dd/yyyy");


                //Get the point event
                PointEvent pointEvent = null;

                //get the point type
                PointType pointType = null;

                if (Utilities.GetPointTypeAndEvent(sPointType, sPointEvent, out pointEvent, out pointType))
                {
                    //get the primary virtual card for the current member                  
                    if (objVCard == null)
                    {
                        throw new Exception("Virtual Card is empty");
                    }

                    using(IDataService dataService = _dataUtil.DataServiceInstance())
                    {
                        //get the configuration date based on the condition
                        ClientConfiguration objClientConfiguration = dataService.GetClientConfiguration("PointExpirationDate");
                        if (objClientConfiguration == null)
                        {
                            ClientConfiguration objDay = dataService.GetClientConfiguration("PointExpirationDays");
                            if (objDay != null)
                            {
                                sExpireDate = System.DateTime.Now.AddDays(Double.Parse(objDay.Value)).ToString("MM/dd/yyyy");
                            }
                            else
                            {
                                sExpireDate = "12/31/2100";
                            }
                        }
                        else
                        {
                            sExpireDate = objClientConfiguration.Value;
                        }
                    }   

                    string userName = WebUtilities.GetCurrentUserName();
                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                    //Utilities.AddBonusPoints(objVCard, pointType.Name, pointEvent.Name, double.Parse(txtPoints.Text.Trim()));
                    Utilities.AddBonusPoints(objVCard, pointType.Name, pointEvent.Name, decimal.Parse(txtPoints.Text.Trim()),
                        null, null, "AgentID=" + PortalState.GetLoggedInCSAgentId().ToString(), userName); //AEO-1841

                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ


                    // Insert note onto the CSnote table 
                    strCSnotes = "Bonus Points Applied: Number of Points " + txtPoints.Text.Trim() + " Reason " + ddlPointEvent.SelectedItem.Text.Trim();
                    CSNote note = new CSNote();
                    note.Note = strCSnotes;
                    note.MemberId = member.IpCode;
                    note.CreatedBy = WebUtilities.GetCurrentUserId();

                    using (CSService inst = FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                    {
                        inst.CreateNote(note);
                    }                                            
                    
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Publish Event to AccountActivity");
                    //Call the IPC manager to publish the RefreshRewards even so the RewardsHistory module will 
                    //refresh.
                    //LW 4.1.14 change
                    //IPCManager.PublishEvent("MemberUpdated", ModuleId, member);
                    IpcManager.PublishEvent("MemberUpdated", new ModuleConfigurationKey(PortalModules.Custom, "MemberUpdatedConfig"), member);

                    /* AEO-218 Begin */

                    String pointSelected = ddlPointEvent.SelectedItem.Text;

                    List<string> csPoints = new List<string>(new string[] {"Customer Service Points Adjustment", 
                                            "Period Adjustment","Points Correction",
                                            "Points Transfer", "Promo Adjustment"});

                    String eventDescription = pointSelected;

                    if ( csPoints.Contains(pointSelected) ) {
                        DateTime startDate = DateTime.MinValue;
                        DateTime endDate = DateTime.MinValue;

                        Utilities.GetProgramDates(member, out startDate, out endDate);//AEO-1055  

                       // startDate = endDate.AddYears(-3);

                        decimal points = Utilities.GetPointsBalance(member, startDate, endDate);
                        decimal pointsToNext = decimal.Zero;


                        if (Utilities.isInPilot(mbrDet.ExtendedPlayCode)) //AEO-722 IsPilot validation for Emails.
                        {

                            if (Decimal.TryParse(Utilities.GetPointsToNextReward(member, startDate, endDate), out pointsToNext)) {

                                long tmp = Decimal.ToInt64(points) % 2500;

                                //No longer have a different email whether you have enough points for a reward.  
                                pointsToNext = 2500 - tmp;
                                this.SendNew5RewardTriggerdEMail(member, points, decimal.Parse(txtPoints.Text.Trim()), pointsToNext);
                           
                            }

                        }
                                              

                    }



                    if ( braOrJeanCreditSelected )
                    {
                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Bra or Jean Selected");
                        eventDescription = ddlPointEvent.SelectedItem.Text.ToUpper().Contains("BRA") ? "Bra" : "Jean";
                       
                        DateTime startDate = DateTime.Now;
                        DateTime endDate = DateTime.Now;
                        Utilities.GetQuarterDates(out startDate, out endDate);

                      //  startDate = endDate.AddYears(-3);

                        string pointsJeans = Utilities.GetB5G1CurrentQualifyingPurchased(member, endDate,"JEAN", false);

                        decimal pointsToNextJeans = (5 - ( long.Parse (pointsJeans) % 5 ));

                        string pointsBra = Utilities.GetB5G1CurrentQualifyingPurchased(member, endDate,"BRA",false);

                        decimal pointsToNextBra = ( 5 - ( long.Parse(pointsBra) % 5 ) );


                        //Just like points, there is not a "No Reward" email for B5G1.  just one email 
                       if (braSelected)  {
                           eventDescription = "Bra";

                           _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Bra Selected: Points - " + pointsBra.ToString());
                           this.SendNewB5G1RewardTriggerdEMail(member, decimal.Parse(pointsBra), decimal.Parse(txtPoints.Text.Trim()), eventDescription, pointsToNextBra);

                       }
                       else {
                           eventDescription ="Jean";

                           _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Jean Selected: Points - " + pointsJeans.ToString());
                           this.SendNewB5G1RewardTriggerdEMail(member, decimal.Parse(pointsJeans), decimal.Parse(txtPoints.Text.Trim()), eventDescription, pointsToNextJeans);
                           
                       }

                        
                    }
                   

                    /* AEO-218 End */
                    btnAwardBonus_Click(member, null);
                    ResetScreen();
                    
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");                    
                    
                }
                else
                {
                    throw new Exception("Error in getting PointType or PointEvent");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert(ex);
                 ShowNegative(ex.ToString());
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
        }
        #endregion

        #region | Custome method defination |
        /// <summary>
        /// Method which reset the screen after saving data or clicking the cancel button.
        /// </summary>
        private void ResetScreen()
        {
            if (pnlPointType.Visible != false)
            {
                ddlPointType.SelectedIndex = 0;
            }
            if (pnlEvent.Visible != false)
            {
                ddlPointEvent.SelectedIndex = 0;
            }
            if (pnlTransDate.Visible != false)
            {
                rdpMonthYear.SelectedDate = null;
            }
            txtPoints.Text = string.Empty;
            if (pnlNotes.Visible != false)
            {
                txtNotes.Text = string.Empty;
            }

        }



        /// <summary>
        /// Bind Point event dropdown list
        /// </summary>
        private void BindPointEvent()
        {
            try
            {
                Member member = PortalState.GetFromCache("SelectedMember") as Member;
                IList<IClientDataObject> details = member.GetChildAttributeSets("MemberDetails", true);
                MemberDetails mbrDet = (details != null && details.Count > 0 ? details[0] : null) as MemberDetails;

                using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    IList<PointEvent> pointEvents = dataService.GetAllPointEvents();
                    PointEvent[] options = new PointEvent[10];

                    ddlPointEvent.Items.Clear();

                    foreach (PointEvent pointEvent in pointEvents)
                    {
                        //AEO Redesign 2015 Begin
                        if (pointEvent.Name == "Missing Receipt")
                        {
                            options[1] = pointEvent;
                        }
                        if (pointEvent.Name == "Customer Service Points Adjustment")
                        {
                            options[2] = pointEvent;
                        }

                        if (pointEvent.Name == "Period Adjustment")
                        {
                            options[3] = pointEvent;
                        }


                        if (pointEvent.Name == "Points Correction")
                        {
                            options[4] = pointEvent;
                        }

                        if (pointEvent.Name == "Points Transfer")
                        {
                            options[5] = pointEvent;
                        }

                        if (pointEvent.Name == "Promo Adjustment")
                        {
                            options[6] = pointEvent;
                        }


                        if (pointEvent.Name.Equals("Bra Credit Appeasement"))
                        {
                            options[8] = pointEvent;

                        }

                        if (mbrDet != null && Utilities.isInPilot(mbrDet.ExtendedPlayCode))
                        { // point conversion

                            if (pointEvent.Name.Equals("Jean Credit Appeasement"))
                            {
                                options[7] = pointEvent;

                            }
                        }


                    }
                    foreach (PointEvent tmp in options)
                    {


                        if (tmp != null)
                        {


                            if (tmp.Name.ToUpper().Equals("JEANS CREDIT"))
                            {
                                ddlPointEvent.Items.Add(new ListItem("Jean Credit Appeasement", tmp.ID.ToString()));
                            }
                            else if (tmp.Name.ToUpper().Equals("BRA CREDIT"))
                            {
                                ddlPointEvent.Items.Add(new ListItem("Bra Credit Appeasement", tmp.ID.ToString()));
                            }
                            else
                            {
                                ddlPointEvent.Items.Add(new ListItem(tmp.Name, tmp.ID.ToString()));
                            }


                        }
                    }
                    ddlPointEvent.Items.Insert(0, new ListItem("Select a Bonus Type", "0"));
                }
                //AEO Redesign 2015 End
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        // AEO-218 Begin
        private void SendNo5RewardTriggerdEMail ( Member toMember, decimal pointToNExtReward , decimal pointsBalance, decimal numPointsIssued, string pointtype )
        {


            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            decimal pointsToNextreward = pointToNExtReward;

            Dictionary<string, string> additionalFields = new Dictionary<string, string>();

            additionalFields.Add("FirstName", toMember.FirstName);
            additionalFields.Add("NewPointsBalance", pointsBalance.ToString());
            additionalFields.Add("NumPointsIssued", numPointsIssued.ToString());
            additionalFields.Add("PointType", pointtype);
            additionalFields.Add("RewardPointsBalance", pointToNExtReward.ToString());

            foreach (string value in additionalFields.Keys)
            {
                _logger.Trace("ViewAwardPoints", "SendNo5RewardTriggeredEmail", value + ": " + additionalFields[value]);
            }
                        

            IList<IClientDataObject> details = toMember.GetChildAttributeSets("MemberDetails", true);
            MemberDetails mbrDet = ( details != null && details.Count > 0 ? details[0] : null ) as MemberDetails;

            string emailAddress = mbrDet != null && mbrDet.EmailAddress != null ? mbrDet.EmailAddress : string.Empty;

            if ( emailAddress != string.Empty )
            {
                AEEmail.SendEmail(toMember, EmailType.PointsNoReward, additionalFields, emailAddress);
            }

            else
            {
                ShowNegative("It was not possible to send E-mail to he member " + toMember.FirstName + "," + toMember.LastName);
            }


            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        private void SendNew5RewardTriggerdEMail ( Member toMember, decimal pointsBalance, decimal numPointsIssued, decimal pointsToNextReward )
        {

           
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            Dictionary<string, string> additionalFields = new Dictionary<string, string>();

            //pointsBalance = pointsBalance + numPointsIssued;

            additionalFields.Add("FirstName", toMember.FirstName);
            additionalFields.Add("NewPointsBalance", (pointsBalance.ToString()));
            additionalFields.Add("NumPointsIssued", numPointsIssued.ToString());
            additionalFields.Add("PointsToNextReward", pointsToNextReward.ToString());
            additionalFields.Add("RewardAmount", "10");

             IList<IClientDataObject> details =toMember.GetChildAttributeSets("MemberDetails", true);
            MemberDetails mbrDet = ( details != null && details.Count > 0 ? details[0] : null ) as MemberDetails;

            string emailAddress = mbrDet != null && mbrDet.EmailAddress != null ? mbrDet.EmailAddress : string.Empty;

            if ( emailAddress != string.Empty )
            {
                AEEmail.SendEmail(toMember, EmailType.PointsReward, additionalFields, emailAddress);
            }

            else {
                ShowNegative("It was not possible to send E-mail to he member " + toMember.FirstName + "," + toMember.LastName );
            }
          

            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        // AEO-218 End


        // AEO-219 Begin
        private void SendNoB5G1RewardTriggerdEMail ( Member toMember, decimal pointToNExtReward, decimal pointsBalance, decimal numPointsIssued, string pointtype )
        {


            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            decimal pointsToNextreward = pointToNExtReward;

            Dictionary<string, string> additionalFields = new Dictionary<string, string>();

            additionalFields.Add("FirstName", toMember.FirstName);
            additionalFields.Add("TotalCreditsEarned",  pointsBalance.ToString());
            additionalFields.Add("NumCreditsIssued", numPointsIssued.ToString());
            additionalFields.Add("rewardtype", pointtype);
            additionalFields.Add("RewardCreditBalance", pointToNExtReward.ToString());


            IList<IClientDataObject> details = toMember.GetChildAttributeSets("MemberDetails", true);
            MemberDetails mbrDet = ( details != null && details.Count > 0 ? details[0] : null ) as MemberDetails;

            string emailAddress = mbrDet != null && mbrDet.EmailAddress != null ? mbrDet.EmailAddress : string.Empty;

            if ( emailAddress != string.Empty )
            {
                AEEmail.SendEmail(toMember, EmailType.B5G1NoReward, additionalFields, emailAddress);
            }

            else
            {
                ShowNegative("It was not possible to send E-mail to he member " + toMember.FirstName + "," + toMember.LastName);
            }


            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        
        private void SendNewB5G1RewardTriggerdEMail ( Member toMember, decimal pointsBalance, decimal numPointsIssued, string pointtype, decimal pointsToNextReward )
        {


            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            Dictionary<string, string> additionalFields = new Dictionary<string, string>();

            //pointsBalance = pointsBalance + numPointsIssued;

            additionalFields.Add("FirstName", toMember.FirstName);
            additionalFields.Add("RewardCreditBalance", (pointsBalance).ToString());
            additionalFields.Add("TotalCreditsEarned", numPointsIssued.ToString());
            additionalFields.Add("rewardtype", pointtype);
            additionalFields.Add("RewardAmount", pointsToNextReward.ToString());

            IList<IClientDataObject> details = toMember.GetChildAttributeSets("MemberDetails", true);
            MemberDetails mbrDet = ( details != null && details.Count > 0 ? details[0] : null ) as MemberDetails;

            string emailAddress = mbrDet != null && mbrDet.EmailAddress != null ? mbrDet.EmailAddress : string.Empty;

            if ( emailAddress != string.Empty )
            {
                AEEmail.SendEmail(toMember, EmailType.B5G1Reward, additionalFields, emailAddress);
            }

            else
            {
                ShowNegative("It was not possible to send E-mail to he member " + toMember.FirstName + "," + toMember.LastName);
            }


            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        // AEO-219 End

        #region | AEO Redesign 2015  |

        
        protected void ddlPointEvent_SelectedIndexChanged ( object sender, 
                                                             EventArgs e )   {

         List<string> aList = new List<string>(new string[] { "BRA CREDIT", "JEAN CREDIT" }); // AEO-173 Begin & End
         bool braSelected = ddlPointEvent.SelectedItem != null && aList.IndexOf(ddlPointEvent.SelectedItem.Text.ToUpper()) == 0;
         bool jeanSelected = ddlPointEvent.SelectedItem != null && aList.IndexOf(ddlPointEvent.SelectedItem.Text.ToUpper()) == 1;
         bool braOrJeanCreditSelected = braSelected || jeanSelected;

         if ( braOrJeanCreditSelected )
         {
             this.lblPoints.Text = "Number of Credits";
             this.rqPoints.Text = "“Please enter a valid number of credits";

         }
         else {
             this.lblPoints.Text = "Number of Points";
             this.rqPoints.Text = "“Please enter a valid number of points";
         }
          
            return;
        }

        #endregion

        //AEO-1605 BEGIN
        /// <summary>
        /// Returns the bra or jean points or points earned by a range of dates
        /// </summary>
        /// <returns></returns>        
        private static long CheckBraJeanPoints(string OptionSelected, DateTime fromDate1, DateTime toDate2, string Agent = null)
        {
            long Result = 0;

            try
            {
                //ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();
                using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    Member member = PortalState.GetFromCache("SelectedMember") as Member;
                    VirtualCard objVCard = member.GetLoyaltyCardByType(Brierley.FrameWork.Common.VirtualCardSearchType.PrimaryCard);
                    IList<PointTransaction> creditsTxn = dataService.GetPointTransactions(objVCard, fromDate1, toDate2, null, null, false);
                    IList<PointEvent> events = dataService.GetPointEvents(new string[] { "Bra Credit", "Jeans Credit", "Customer Service Points Adjustment", "Period Adjustment", "Points Correction", "Points Transfer", "Promo Adjustment", "Missing Receipt", "Jean Credit Appeasement", "Bra Credit Appeasement" });
                    IList<PointEvent> BraOrJeanCredit = dataService.GetPointEvents(new string[] { "Bra Credit", "Jeans Credit", "Jean Credit Appeasement", "Bra Credit Appeasement" });

                    decimal txnCounterBra = 0;
                    decimal txnCounterJean = 0;
                    decimal txnCounterPoint = 0;
                    HashSet<long> BraID = new HashSet<long>();

                    HashSet<long> JeanID = new HashSet<long>();

                    //Get the Id for BRA and JEAN credits
                    foreach (PointEvent loCredit in BraOrJeanCredit)
                    {
                        if (loCredit.Name.Contains("Bra Credit"))
                        {
                            BraID.Add(loCredit.ID);
                        }
                        else
                        {
                            JeanID.Add(loCredit.ID);
                        }
                    }

                    foreach (PointTransaction loTxn in creditsTxn)
                    {
                        foreach (PointEvent loEvent in events)
                        {
                            if (loEvent.ID == loTxn.PointEventId) //List of all adj
                            {
                                //Sum the points
                                if (BraID.Contains(loEvent.ID))
                                {
                                    if (string.IsNullOrEmpty(Agent))
                                    {
                                        txnCounterBra += loTxn.Points;
                                    }
                                    else
                                    {
                                        if (loTxn.ChangedBy == Agent)
                                            txnCounterBra += loTxn.Points;
                                    }
                                }
                                else if (JeanID.Contains(loEvent.ID))
                                {
                                    if (string.IsNullOrEmpty(Agent))
                                    {
                                        txnCounterJean += loTxn.Points;
                                    }
                                    else
                                    {
                                        if (loTxn.ChangedBy == Agent)
                                            txnCounterJean += loTxn.Points;
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(Agent))
                                    {
                                        txnCounterPoint += loTxn.Points;
                                    }
                                    else
                                    {
                                        if (loTxn.ChangedBy == Agent)
                                            txnCounterPoint += loTxn.Points;
                                    }

                                }
                            }
                        }
                    }

                    switch (OptionSelected)
                    {
                        case "braSelected":
                            Result = (long)txnCounterBra;
                            break;
                        case "jeanSelected":
                            Result = (long)txnCounterJean;
                            break;
                        default:
                            Result = (long)txnCounterPoint;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        //AEO-1605 END
    }
    #endregion
}
