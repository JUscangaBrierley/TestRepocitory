// ----------------------------------------------------------------------
// <copyright file="ViewAwardPoints.ascx.cs" company="B+P">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------


/*-------------------------------------------------------------------------
    change history 

  Who       when        What
 --------------------------------------------------------------------------
 MMV001    12May2015   add bussines rule to don't allow replacement cards
                       when MemberDetails.ExtendedPlayCode == 1 when page is loading 
 -------------------------------------------------------------------------*/

namespace Brierley.AEModules.ReplaceCard
{
    #region | Namespace |
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    

    //LW 4.1.14 change
    //using Brierley.DNNModules.PortalModuleSDK;
    //using Brierley.DNNModules.PortalModuleSDK.Controls;

    using Brierley.WebFrameWork.Controls;
    using Brierley.WebFrameWork.Controls.Grid;
    using Brierley.WebFrameWork.Portal.Configuration.Modules;
    using Brierley.WebFrameWork.Portal;
    using Brierley.WebFrameWork.Portal.Configuration;
    using Brierley.WebFrameWork.Ipc;

    //LW 4.1.14 change
    //using Brierley.DNNModules.ReplaceCard.Components;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.ClientDevUtilities.LWGateway;

    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyNavigator.Controls.Grid.AspGrid;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    //LW 4.1.14 change
    //using DotNetNuke.Entities.Modules;
    //using DotNetNuke.Entities.Modules.Actions;
    //using DotNetNuke.Services.Localization;

    #endregion

    #region | Class defination for view replace card |
    /// <summary>
    /// Class defination for view replace card
    /// </summary>
    public partial class ViewReplaceCard : ModuleControlBase, IIpcEventHandler
    {
        #region | Variable declartion |
        /// <summary>
        /// Logger object to implement the logging functionality
        /// </summary>
        private static LWLogger _logger = LWLoggerManager.GetLogger("ReplaceCard");
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// Config object to store the custom configuration values
        /// </summary>
        //LW 4.1.14 change
        //private ReplaceCardConfig _config = null;

        /// <summary>
        /// Dynamic grid object to implement the dynamic grid functionalities
        /// </summary>
        private AspDynamicGrid grid = null;

        
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


        #region | User control system generated events defination |

        /// <summary>
        /// Force not to load the skins from skins folder
        /// </summary>
        /// <returns>Returns true or false</returns>
        protected override bool ControlRequiresTelerikSkins()
        {
            return false;
        }

       
        
        /// <summary>
        /// Page load method for ViewReplaceCard form
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Loading config data
               
                if (!IsPostBack)
                {
                    this.SetBanners();

                    // Pre select the member brand
                    Member member = PortalState.GetFromCache("SelectedMember") as Member;
                    MemberDetails memberDetails = null;
                    long brandID = 1;
                    IList<IClientDataObject> lstMemberAttributes = member.GetChildAttributeSets("MemberDetails");
                    


                    if (lstMemberAttributes != null && lstMemberAttributes.Count > 0)
                    {
                        memberDetails = (MemberDetails)lstMemberAttributes[0];


                        if ( memberDetails.BaseBrandID.HasValue )
                                brandID = memberDetails.BaseBrandID.Value;

                        if ( brandID == 2 )
                            this.radioBaseBrand.SelectedValue = brandID.ToString();
                        else
                            this.radioBaseBrand.SelectedValue = "1";
                                               
                    }
                }

                this.grid = new AspDynamicGrid();
                this.grid.Provider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance("AmericanEagle.SDK.dll", "AmericanEagle.SDK.GridProvider.ReplaceCardProvider");

                phCardHistory.Controls.Add(this.grid);
                          
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
        }
        
        /// <summary>
        /// Submit click for ViewReplaceCard form
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // PI 12339 Changes Start
            String role = WebUtilities.GetCurrentUserRole().ToLower();
            long repCardCount = 0;
            DateTime startDate, endDate;
            // PI 12339 Changes End
            try
            {


                // PI AEO-2015_Redesign_Pilot Begin

                Member member = PortalState.GetFromCache("SelectedMember") as Member;
                MemberDetails memberDetails = null;
                IList<IClientDataObject> lstMemberAttributes = member.GetChildAttributeSets("MemberDetails");


                if ( lstMemberAttributes != null && lstMemberAttributes.Count > 0 )   {
                    memberDetails = (MemberDetails)lstMemberAttributes[0];

                    if ( Utilities.isInPilot(memberDetails.ExtendedPlayCode ))  {  // point conversion
                        this.ShowWarning("AEMessage|Pilot members can not have their card replaced.");
                        return;
                    }                    
                }

                // PI AEO-2015_Redesign_Pilot End
                               
                        

                if (string.IsNullOrEmpty(this.radioBaseBrand.SelectedValue))
                {
                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                    //ShowAlert("Please select a brand, and then submit your request.");
                    ShowWarning("AEMessage|Please select a brand, and then submit your request.");
                    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                    return;
                }

                //  PI AEO-2015_Redesign_Pilot Begin
                // variable declaration moved to line 176 to be used before
                //

                //Member member = PortalState.GetFromCache("SelectedMember") as Member;
                //PI AEO-2015_Redesign_Pilot End

                IList<IClientDataObject> replaceCards = member.GetChildAttributeSets("MemberCardReplacements");
                VirtualCard vCard = member.GetLoyaltyCardByType(Brierley.FrameWork.Common.VirtualCardSearchType.PrimaryCard);
                MemberCardReplacements mcr = null; // PI 12339 Changes
                // CSR issues card replacement
                MemberCardReplacements cardReplacement = new MemberCardReplacements();
                // PI 12339 Changes Start
                Utilities.GetProgramDates(member, out startDate, out endDate); // PI 30364 - Dollar reward program 
                for (int i = 0; i < replaceCards.Count; i++)
                {
                    mcr = (MemberCardReplacements) replaceCards[i];
                    if (WebUtilities.GetCurrentUserName() == mcr.CHANGEDBY && startDate <= mcr.CreateDate && endDate.AddDays(1).AddSeconds(-1) >= mcr.CreateDate)
                    {
                        repCardCount++;
                    }
                }

                    // If first time any card is goint to be replaced
                    if (replaceCards.Count == 0)
                    {
                        cardReplacement = new MemberCardReplacements();
                        cardReplacement.StatusCode = (long)CardReplaceStatus.Original;
                        cardReplacement.LoyaltyIDNumber = vCard.LoyaltyIdNumber;
                        cardReplacement.CHANGEDBY = WebUtilities.GetCurrentUserName();
                        member.AddChildAttributeSet(cardReplacement);
                        cardReplacement.StatusCode = (long)CardReplaceStatus.ScheduleForReplacement;
                        cardReplacement.LoyaltyIDNumber = vCard.LoyaltyIdNumber;
                        cardReplacement.CHANGEDBY = WebUtilities.GetCurrentUserName();
                        member.AddChildAttributeSet(cardReplacement);
                        lblSucc.Text = "This card has been scheduled for replacement. Please allow 3-4 weeks for new card delivery.";
                    }
                    else if (role == "csr" && repCardCount >= 1)
                    {
                        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                        //ShowAlert("AEMessage|Replacement Card limit reached for your role.");
                        ShowWarning("AEMessage|Replacement Card limit reached for your role.");
                        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                    }
                    else if (role == "supervisor" && repCardCount >= 2)
                    {
                        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                        // ShowAlert("AEMessage|Replacement Card limit reached for your role.");
                        ShowWarning("AEMessage|Replacement Card limit reached for your role.");
                        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                    }
                    else if (role == "super admin" || role == "admin" || role == "csr" || role == "supervisor" )
                    {
                        cardReplacement.StatusCode = (long)CardReplaceStatus.ScheduleForReplacement;
                        cardReplacement.LoyaltyIDNumber = vCard.LoyaltyIdNumber;
                        cardReplacement.CHANGEDBY = WebUtilities.GetCurrentUserName();
                        member.AddChildAttributeSet(cardReplacement);
                        this.lblSucc.Text = "This card has already been scheduled for replacement. Please allow 3-4 weeks for new card delivery. If the time period has already passed. Please contact the system administrator.";
                    }
                // PI 12339 Changes End
                // Setting member base brand id based on selection

                //MMV001 Begin
                //MemberDetails memberDetails = null;
                //IList<IClientDataObject> lstMemberAttributes = member.GetChildAttributeSets("MemberDetails");
                //MMV001 End

                if (lstMemberAttributes != null && lstMemberAttributes.Count > 0)
                {
                    memberDetails = (MemberDetails)lstMemberAttributes[0];
                    memberDetails.BaseBrandID = Convert.ToInt64(this.radioBaseBrand.SelectedValue);
                    memberDetails.ChangedBy = "PI 17923 Set Base Brand";
                }
                using (var dataService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    dataService.SaveMember(member);
                }
                // Add CSNotes
                CSNote note = new CSNote();
                note.Note = "Card scheduled for replacement";
                note.MemberId = member.IpCode;
                note.CreatedBy = WebUtilities.GetCurrentUserId();
                using (var inst = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                {
                    inst.CreateNote(note);
                }
                this.grid.Rebind();
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
        }
        #endregion

        #region | User defined method defination
        /// <summary>
        /// Set the banner if set in configuration screen
        /// </summary>
        private void SetBanners()
        {
            this.pnlRepCard.Visible = true;
            this.lblRepCard.Text = "Replace Card";
            this.pnlRepCard.CssClass = "CSBanner";
            this.pnlCardHistory.Visible = true;
            this.lblCardHistory.Text = "Card History";
            this.pnlCardHistory.CssClass = "CSBanner";
        }
        #endregion
    }
    #endregion
}
