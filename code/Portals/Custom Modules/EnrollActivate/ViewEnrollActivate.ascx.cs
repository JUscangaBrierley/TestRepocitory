namespace Brierley.AEModules.EnrollActivate
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using System.IO;
    using System.Reflection;

    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.WebFrameWork.Controls;
    using Brierley.FrameWork.Common;
    using Brierley.WebFrameWork.Controls.Grid;
    using Brierley.WebFrameWork.Portal.Configuration.Modules;
    using Brierley.WebFrameWork.Portal;
    using Brierley.WebFrameWork.Portal.Configuration;
    using Brierley.WebFrameWork.Ipc;
    using Brierley.FrameWork.Common.Logging;
    using AmericanEagle.SDK.Global;
    using Brierley.Clients.AmericanEagle.DataModel;
    using Brierley.ClientDevUtilities.LWGateway;

    #endregion

    /// <summary>
    /// View merge account
    /// </summary>
    public partial class ViewEnrollActivate : ModuleControlBase
    {
        #region Member Variables
        /// <summary>
        /// Logger for log trace, debug or error information
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("EnrollActivate");

        private static ILWDataServiceUtil _dataUtil = new ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        #endregion

        #region Protected Members

        /// <summary>
        /// Page load event to set control properties
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Page load event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            try
            {
                Member member = new Member();
                MemberDetails mbrDetails = new MemberDetails();

                if (IsPostBack)
                {
                    if (!string.IsNullOrEmpty(txtLoyaltyIDNumber.Text.Trim()) && LoyaltyCard.IsLoyaltyNumberValid(Convert.ToInt64(txtLoyaltyIDNumber.Text.Trim())))
                    {
                        using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            Member eMember = dataService.LoadMemberFromLoyaltyID(txtLoyaltyIDNumber.Text.Trim());
                            if (eMember == null)
                            {
                                radioBaseBrand.Enabled = false;
                            }
                            else
                            {
                                ShowWarning("AEMessage|The number entered is already registered. Please enter another AEREWARD$ number to continue.");
                                return;
                            }
                        }
                    }
                    else
                    {
                        radioBaseBrand.Enabled = true;
                        return;
                    }

                    if (null != selState.Items.FindByValue(Request["selState"]))
                    {
                        selState.SelectedValue = Request["selState"];
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "SelState:" + selState.SelectedValue);
                    }
                }
                else
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "!IsPostBack");
                    pnlEnroll.Visible = true;
                    pnlConfirm.Visible = false;
                    radioBaseBrand.Enabled = true;
                }
                this.SetControlProperties();
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Unexpected Exception", ex);
                ShowWarning(ex.Message);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Set control properties
        /// </summary>
        private void SetControlProperties()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                
                //AEO-1134 BEGIN
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Country default USA");
                this.selCountry.SelectedIndex = 0;
                this.BindStateByCountry();
                //AEO-1134 END

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
        }

        protected void selCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindStateByCountry(); //AEO-1134
        }

        #endregion

        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            Member pMember = new Member();
            MemberDetails mbrDetails = new MemberDetails();
            VirtualCard card = pMember.CreateNewVirtualCard();
            MemberBrand memberBrand = new MemberBrand();

            // added for redesign project
            mbrDetails.AutoIssueReward = 1;

            // AEO-141 Redesign Begin
            mbrDetails.PendingEmailVerification = 1;
            mbrDetails.PendingCellVerification = 1;
            mbrDetails.NextEmailReminderDate = DateTime.Now.AddDays(1) ;
            // AEO-141 Redesign end
            bool isPilotMember = Utilities.MemberIsInPilot(txtPostalCode.Text.Trim());
            
            card.DateIssued = DateTime.Now;
            card.IsPrimary = true;
            long newLoyaltyNumber = 0;
            bool isTempCardMember = false;

            try
            {
                using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                using (CSService inst = FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                {

                    if (string.IsNullOrEmpty(txtLoyaltyIDNumber.Text.Trim()))
                    {
                        int brandID = 1;
                        int.TryParse(radioBaseBrand.SelectedValue, out brandID);
                        brandID = brandID == 0 ? 1 : brandID;
                        memberBrand.BrandID = brandID;

                        if (isPilotMember)
                            newLoyaltyNumber = LoyaltyCard.GetNextLoyaltyNumber(LoyaltyCard.LoyaltyCardType.AE);
                        else
                            newLoyaltyNumber = LoyaltyCard.GetNextLoyaltyNumber(LoyaltyCard.LoyaltyCardType.Temporary);

                        card.LoyaltyIdNumber = newLoyaltyNumber.ToString();
                        isTempCardMember = true;
                    }
                    else
                    {
                        if (LoyaltyCard.IsLoyaltyNumberValid(Convert.ToInt64(txtLoyaltyIDNumber.Text.Trim())))
                        {
                            Member eMember = dataService.LoadMemberFromLoyaltyID(txtLoyaltyIDNumber.Text.Trim());
                            if (null == eMember)
                            {
                                card.LoyaltyIdNumber = txtLoyaltyIDNumber.Text.Trim();
                            }
                            else
                            {
                                ShowWarning("AEMessage|The number entered is already registered. Please enter another AEREWARD$ number to continue.");
                                return;
                            }
                        }
                        else
                        {
                            ShowWarning("AEMessage|The number entered is invalid.  Please update the AEREWARD$ number to continue.");
                            return;
                        }
                    }

                    // AEO-500 Begin
                    string strValue = txtFirstName.Text.Trim();
                    if (Utilities.IsNameValid(ref strValue))
                    {
                        pMember.FirstName = strValue;
                    }
                    else
                    {
                        ShowWarning("AEMessage|First name is invalid.");
                        return;
                    }

                    strValue = txtLastName.Text.Trim();
                    if (Utilities.IsNameValid(ref strValue))
                    {
                        pMember.LastName = strValue;
                    }
                    else
                    {
                        ShowWarning("AEMessage|Last name is invalid.");
                        return;
                    }
                    // AEO-500 End

                    //Check if under age.
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "birthDate: " + txtBirthday.Text.Trim());
                    DateTime birthDate = DateTime.MaxValue;
                    pMember.BirthDate = Convert.ToDateTime(txtBirthday.Text.Trim());
                    birthDate = Convert.ToDateTime(txtBirthday.Text.Trim());

                    mbrDetails.IsUnderAge = false;
                    tbUnderAge.Visible = false;

                    TimeSpan diff = DateTime.Now - birthDate;
                    int years = diff.Days / 366;
                    DateTime workingDate = birthDate.AddYears(years);
                    while (workingDate.AddYears(1) <= DateTime.Now)
                    {
                        workingDate = workingDate.AddYears(1);
                        years++;
                    }

                    if (years < 13)
                    {
                        tbUnderAge.Visible = true;
                        mbrDetails.IsUnderAge = true;
                    }

                    if (radioGender.SelectedValue != null)
                    {
                        mbrDetails.Gender = radioGender.SelectedValue;
                    }

                    if (radioLanguage.SelectedValue != null)
                    {
                        mbrDetails.LanguagePreference = radioLanguage.SelectedValue;
                    }

                    if (string.IsNullOrEmpty(selCountry.SelectedValue.Trim()) || selCountry.SelectedValue.Trim() == "--Select--")
                    {
                        ShowWarning("AEMessage|Please select Country.");
                        return;
                    }
                    else
                    {
                        mbrDetails.Country = selCountry.SelectedValue.Trim();
                    }


                    if (!string.IsNullOrEmpty(selState.Text.Trim()))
                    {
                        mbrDetails.StateOrProvince = selState.Text.Trim();
                    }
                    else
                    {
                        ShowWarning("AEMessage|Please select Country and State.");
                        return;
                    }

                    // Determine the brand id from the loyalty id number
                    if (!string.IsNullOrEmpty(txtLoyaltyIDNumber.Text.Trim()))
                    {
                        string loyaltyIdPrefix = txtLoyaltyIDNumber.Text.Trim().Substring(0, 2);
                        RefBrand refBrand = Utilities.GetRefBrandFromBrandPrefix(loyaltyIdPrefix);
                        memberBrand.BrandID = refBrand == null ? 1 : refBrand.BrandID;
                    }

                    mbrDetails.BaseBrandID = memberBrand.BrandID;
                    if (mbrDetails.BaseBrandID == null || mbrDetails.BaseBrandID == 0)
                    {
                        mbrDetails.BaseBrandID = (int)Brands.AE;
                    }

                    if (Utilities.IsAddressValid(txtAddress1.Text.Trim()))
                    {
                        mbrDetails.AddressLineOne = txtAddress1.Text.Trim();
                    }
                    else
                    {
                        ShowWarning("AEMessage|Address1 is invalid.");
                        return;
                    }


                    if (!string.IsNullOrEmpty(txtAddress2.Text.Trim()))
                    {
                        if (Utilities.IsAddressValid(txtAddress2.Text.Trim()))
                        {
                            mbrDetails.AddressLineTwo = txtAddress2.Text.Trim();
                        }
                        else
                        {
                            ShowWarning("AEMessage|Address2 is invalid.");
                            return;
                        }
                    }

                    if (Utilities.IsCityValid(txtCity.Text.Trim()))
                    {
                        mbrDetails.City = txtCity.Text.Trim();
                    }
                    else
                    {
                        ShowWarning("AEMessage|City is invalid.");
                        return;
                    }


                    if (Utilities.IsPostalCodeValid(txtPostalCode.Text.Trim()))
                    {
                        mbrDetails.ZipOrPostalCode = txtPostalCode.Text.Trim();

                    }
                    else
                    {
                        ShowWarning("AEMessage|Postal Code is invalid.");
                        return;
                    }
                    // Trim dashes and spaces from zip code and then copy to primary postal code
                    mbrDetails.ZipOrPostalCode = mbrDetails.ZipOrPostalCode.Replace("-", "").Replace(" ", "");
                    pMember.PrimaryPostalCode = mbrDetails.ZipOrPostalCode;

                    string strHomePhone = txtHomePhone.Text.Replace("-", string.Empty).Trim();

                    if (!string.IsNullOrEmpty(strHomePhone))
                    {
                        if (Utilities.IsPhoneValid(strHomePhone))
                        {
                            mbrDetails.HomePhone = strHomePhone;
                        }
                        else
                        {
                            ShowWarning("AEMessage|You have entered invalid home phone number.");
                            return;
                        }
                    }

                    string strMobilePhone = txtMobilePhone.Text.Replace("-", string.Empty).Trim();
                    if (!string.IsNullOrEmpty(strMobilePhone))
                    {
                        if (Utilities.IsPhoneValid(strMobilePhone))
                        {
                            mbrDetails.MobilePhone = strMobilePhone;
                        }
                        else
                        {
                            ShowWarning("AEMessage|You have entered invalid mobile phone number.");
                            return;
                        }
                    }

                    if (this.chkSMSOptIn.Checked)
                    {
                        if (strMobilePhone.Length != 0 && this.chkSMSOptIn.Checked && Utilities.IsPhoneValid(strMobilePhone))
                        {
                            mbrDetails.SmsOptInDate = DateTime.Now;
                            mbrDetails.SMSOptIn = true;
                        }
                        else
                        {
                            ShowWarning("AEMessage|You have selected to receive SMS messages through your cell phone. Please provide a valid mobile number on your profile.");
                            return;
                        }
                    }

                    if (Utilities.IsEmailValid(txtEmailAddress.Text.Trim()))
                    {
                        mbrDetails.EmailAddress = txtEmailAddress.Text.Trim();
                        mbrDetails.EmailAddressMailable = true;
                        mbrDetails.PassValidation = true;
                        pMember.PrimaryEmailAddress = txtEmailAddress.Text.Trim();
                    }
                    else
                    {
                        ShowWarning("AEMessage|You have entered invalid EmailAddress.");
                        return;
                    }

                    if ((pMember.PrimaryEmailAddress != null) && (pMember.PrimaryEmailAddress.Length > 0))
                    {
                        mbrDetails.EmailAddress = pMember.PrimaryEmailAddress;
                        pMember.PrimaryEmailAddress = null;
                    }

                    // added for redesign project
                    if (isPilotMember)
                    {
                        // mbrDetails.PassValidation = false;
                        mbrDetails.ExtendedPlayCode = 1;
                    }

                    mbrDetails.EmailOptIn = true;
                    mbrDetails.DirectMailOptInDate = DateTime.Today;
                    mbrDetails.EmailOptInDate = DateTime.Today;
                    mbrDetails.AddressMailable = true;
                    mbrDetails.MemberSource = 6;
                    mbrDetails.AITUpdate = true;
                    mbrDetails.HHKey = LoyaltyCard.GetNextHHKey().ToString();
                    mbrDetails.ChangedBy = "Enroll/Activate";
                    mbrDetails.NextEmailReminderDate = DateTime.Now.Date.AddDays(1); //AEO-156 Redesign 2015 Begin & End

                    pMember.AddChildAttributeSet(mbrDetails);
                    //pMember.AddChildAttributeSet(memberBrand);

                    pMember.MarkVirtualCardAsPrimary(card);

                    dataService.SaveMember(pMember);

                    // added for redesign project
                    if (!String.IsNullOrEmpty(mbrDetails.EmailAddress) && !isPilotMember)
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member getting email bonus");
                        Utilities.AddEmailBonus(card, mbrDetails);
                    }


                    //added for redesign project
                    if (!String.IsNullOrEmpty(mbrDetails.MobilePhone) && mbrDetails.SmsOptInDate != null && !isPilotMember)
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Give SMS OptIn Bonus");
                        Utilities.AddSMSBonus(card, mbrDetails);
                    }

                    //Add temp card member to MemberCardReplacement table, so it can be picked up by TempCardReplaceSend job
                    if (isTempCardMember)
                    {
                        AddMemberCardReplacements(pMember, isPilotMember);
                    }

                    //Add member to Proactive Merge table
                    Merge.AddMemberProactiveMerge(pMember);

                    //add callhistory
                    CSNote note = new CSNote();
                    note.Note = "Member is registered via Enroll/Activate page.";
                    note.MemberId = pMember.IpCode;
                    note.CreatedBy = WebUtilities.GetCurrentUserId();
                    inst.CreateNote(note);

                    Member dbMember = dataService.LoadMemberFromLoyaltyID(card.LoyaltyIdNumber);
                    this.lblMemberName.Text = dbMember.FirstName + " " + dbMember.LastName;

                    this.lblMemberLoyaltyNumber.Text = card.LoyaltyIdNumber;

                    this.lblMemberLoyaltyNumber2.Text = card.LoyaltyIdNumber;

                    pnlEnroll.Visible = false;
                    pnlConfirm.Visible = true;

                    // added for redesign project
                    if (isPilotMember)
                    {
                        pMember.AddTier("Blue", DateTime.Today, DateTime.Parse("12/31/2199"), "Base");
                    }

                    //Send email notification
                    SendEnrollActivateTriggerdEMail(dbMember, txtEmailAddress.Text.Trim());
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                ShowWarning(ex.Message);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public void SendEnrollActivateTriggerdEMail(Member member, string emailAddress)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            Dictionary<string, string> additionalFields = new Dictionary<string, string>();
            string strLoyaltyNumber = Utilities.GetLoyaltyIDNumber(member);

            additionalFields.Add("firstname", member.FirstName);
            additionalFields.Add("loyaltynumber", strLoyaltyNumber);

            AEEmail.SendEmail(member, EmailType.EnrollActivate, additionalFields, emailAddress);
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public void AddMemberCardReplacements(Member member, bool isPilot)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            MemberCardReplacements replaceCards = new MemberCardReplacements();
            MemberDetails memberDetail = new MemberDetails();
            try
            {
                // added for redesign project
                MemberCardReplacements CardReplacements = new MemberCardReplacements();
                bool isPilotMember = isPilot;
                if (null != member && replaceCards != null)
                    {
                        if (isPilotMember)
                        {
                            string strLoyaltyNumber = Utilities.GetLoyaltyIDNumber(member);
                            replaceCards.StatusCode = (long)CardReplaceStatus.SendToAE;
                            replaceCards.LoyaltyIDNumber = strLoyaltyNumber;
                            replaceCards.IsTemporary = true;
                            replaceCards.CHANGEDBY = "Enroll/Activate";
                            member.AddChildAttributeSet(replaceCards);                            
                        }
                        else
                        {
                            string strLoyaltyNumber = Utilities.GetLoyaltyIDNumber(member);
                            replaceCards.StatusCode = (long)CardReplaceStatus.ScheduleForReplacement;
                            replaceCards.LoyaltyIDNumber = strLoyaltyNumber;
                            replaceCards.IsTemporary = true;
                            replaceCards.CHANGEDBY = "Enroll/Activate";
                            member.AddChildAttributeSet(replaceCards);
                            replaceCards = new MemberCardReplacements();
                            replaceCards.StatusCode = (long)CardReplaceStatus.Original;
                            replaceCards.LoyaltyIDNumber = strLoyaltyNumber;
                            replaceCards.CHANGEDBY = "Enroll/Activate";
                            member.AddChildAttributeSet(replaceCards);
                        }
                    }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public void BindStateByCountry() //AEO-1134
        {
            StateValidation stateValid = new StateValidation();
            IList<RefStates> stateList = stateValid.LoadStates(this.selCountry.SelectedValue);
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Selected Country: " + this.selCountry.SelectedValue);
            this.selState.DataSource = stateList;
            this.selState.DataValueField = "StateCode";
            this.selState.DataTextField = "FullName";
            this.selState.DataBind();
        }

    }
}
