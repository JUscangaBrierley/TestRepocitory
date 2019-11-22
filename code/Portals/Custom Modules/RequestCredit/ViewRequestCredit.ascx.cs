namespace Brierley.AEModules.RequestCredit
{
    #region |Using StatementslblApliedMsg
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Reflection;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml.Serialization;
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
    using Brierley.AEModules.RequestCredit.Components;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyNavigator.Controls;
    using Brierley.ClientDevUtilities.LWGateway;

    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyNavigator.Controls.Grid.AspGrid;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    //LW 4.1.14 change
    //using DotNetNuke.Entities.Modules;
    //using DotNetNuke.Entities.Modules.Actions;
    //using DotNetNuke.Services.Localization;
    #endregion |using|

    /// <summary>
    /// View request credit
    /// </summary>
    public partial class ViewRequestCredit : ModuleControlBase, IIpcEventHandler
    {
        #region Member variables
        /// <summary>
        /// Result grid instance
        /// </summary>
        protected AspDynamicGrid resultGrid = null; // AEO-74 Upgrade 4.5 changes here -----------SCJ

        /// <summary>
        /// Hold History grid 
        /// </summary>        
        protected AspDynamicGrid historyGrid = null; // AEO-74 Upgrade 4.5 changes here -----------SCJ

        /// <summary>
        /// Logger to trace or debug 
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("RequestCredit");
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// Hold module configuration information
        /// </summary>
        private Components.ModuleConfiguration moduleConfiguration = new ModuleConfiguration();

        /// <summary>
        /// Hold result grid 
        /// </summary>
        #endregion Member variables
        //LW 4.1.14 change
        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }
        public void HandleEvent(IpcEventInfo info)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Handling IPC event: " + info.EventName);
        }
        #region public member

        /// <summary>
        /// Gets or sets Search list
        /// </summary>
        public Dictionary<string, string> _searchList
        {
            get
            {
                if (ViewState["SearchList"] != null)
                {
                    return (Dictionary<string, string>)ViewState["SearchList"];
                }
                else
                {
                    return new Dictionary<string, string>();
                }
            }

            set
            {
                ViewState["SearchList"] = value;
            }
        }


        /// <summary>
        /// Method used for load the grid data
        /// </summary>
        /// <param name="searchParm">Search params</param>
        /// <returns>int: result grid record count</returns>       
        public int LoadResultGrid(Dictionary<string, string> searchParm)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            int resultCount = 0;
            SearchTxn searchTxn = new SearchTxn();
            this._searchList = searchParm;

            if (this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.Online).ToString()) || this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.Store).ToString()))
            {

                int receiptResult = searchTxn.SearchReceipts(searchParm);
                if (receiptResult != 0)
                {
                    return receiptResult;
                }
            }

            IList<IClientDataObject> searchResults = searchTxn.SearchTransaction(searchParm, this.ddlTransactionType.SelectedValue);
            if (null != searchResults && searchResults.Count > 0)
            {
                resultCount = searchResults.Count;
                if (searchResults.Count == 1)
                {
                    HistoryTxnDetail htd = searchResults[0] as HistoryTxnDetail;
                    if (htd.ProcessID == (int)ProcessId.RequestCreditProcesssed || htd.ProcessID == (int)ProcessId.ProcessedforLoyalty)
                    {
                        this.lblApliedMsg.Text = "Credit is already applied.";
                    }
                }

                this.lblResultGridTitle.Text = "Search result(s)";
                this.resultGrid.SetSearchParm("HistoryRecordList", searchResults);
                this.resultGrid.Visible = true;
                this.pnlSearchResult.Visible = true;
            }
            else
            {
                this.lblResultGridTitle.Text = string.Empty;
                this.pnlSearchResult.Visible = false;

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "ddlTransactionType.Text: " + ddlTransactionType.SelectedValue);
                // Dont change the name specific to AE requirement
                /*
                if (this.tblSearchCtrls.Rows.Count > 0)
                {
                    if (int.Parse(ddlTransactionType.SelectedValue) < 4)
                    {

                        //LNLinkButton lnbtnSearch = this.GetSearchButton(); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                        CustomLinkButtom lnbtnSearch = this.GetSearchButton();
                        if (null != lnbtnSearch)
                        {
                            lnbtnSearch.Text = "Submit Request";
                        }
                    }
                }
                */
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");

            return resultCount;
        }

        #endregion

        #region Page Loading and Initialization
        /// <summary>
        /// Get configuration loads result and history grid
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

                this.lblAddTransaction.Text = string.Empty;
                this.tblSearchCtrls.Visible = false;
                this.pnlSearchResult.Visible = false;
                this.lblApliedMsg.Text = string.Empty;
                this.lblResultGridTitle.Text = string.Empty;

                if (Page.IsPostBack)
                {
                    this.resultGrid = new AspDynamicGrid();
                    this.resultGrid.Provider = (IDynamicGridProvider)(new DefaultGridProvider());
                    this.resultGrid.GridActionClicked += new AspDynamicGrid.GridActionClickedHandler(this.Grid_GridActionClicked);
                    this.pnlSearchResult.Controls.Add(this.resultGrid);
                    this.LoadResultGrid(this._searchList);
                }

                // Load Get Points History grid
                this.historyGrid = new AspDynamicGrid();
                this.historyGrid.Provider = (IDynamicGridProvider)(new GetPointHistoryGridProvider());
                this.phGetPointHistory.Controls.Add(this.historyGrid);
                this.LoadGetPointsHistory();

                if (!IsPostBack)
                {
                    this.InitializeModuleConfiguration();
                }
                else
                {
                    this.LoadSearchControls(this.GetTransactionType(hdnInsertVal.Value));
                }
                Page.ClientScript.RegisterClientScriptBlock(typeof(System.Web.UI.Page), "Initialize", "<script>var transactiontype='" + ddlTransactionType.SelectedItem.Value + "';</script>");
                //}

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
        }

        #endregion
        #region Event Handlers

        /// <summary>
        /// Event for dynamic dropdown of transaction type
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event arguments</param>
        protected void DdlTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "TransactionType: " + ddlTransactionType.Text);

            this.lblApliedMsg.Text = string.Empty;
            this.lblResultGridTitle.Text = string.Empty;
            this.ClearSearch(pnlSearchFormTable);
            // this.GetSearchButton().Text = "Submit";
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        /// <summary>
        /// Event for dynamic radio button of transaction type
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event arguments</param>
        protected void RdbTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            this.lblApliedMsg.Text = string.Empty;
            this.lblResultGridTitle.Text = string.Empty;
            this.ClearSearch(pnlSearchFormTable);
            // this.GetSearchButton().Text = "Submit";
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        /// <summary>
        /// Method to verify if is necessary to limit the search considering the global value of LimitDaysCreditRequest days back if the member is pilot.
        /// </summary>
        /// <param name="transactionDate">Date to verify if is in the range</param>
        /// <param name="transactionType">Determine the transactionType (Online,Store)</param>
        /// <param name="finalStartTransactionDate">Start date, this may be the transactionDate, but it could be updated in case the transactiondate is older than the number of days specified in the global value LimitDaysCreditRequest</param
        protected bool IsLimitDaysForPilotMembers(DateTime transactionDate, TransactionType transactionType, out DateTime finalStartTransactionDate)
        {
            //For redisign project- If the member is in the Pilot program, Limit the search to global value LimitDaysCreditRequest days back            
            using (var dataService = _dataUtil.DataServiceInstance())
            {
                finalStartTransactionDate = transactionDate;
                int totalDaysToLookBack = Convert.ToInt32(dataService.GetClientConfigProp("LimitDaysCreditRequest"));

                // AEO-401 begin
                IList<IClientDataObject> tmp = WebUtilities.GetLoyaltyMemberFromCache().GetChildAttributeSets("MemberDetails", false);

                if (tmp != null && tmp.Count > 0 && tmp[0] != null)
                {
                    MemberDetails md = (MemberDetails)tmp[0];
                    if (Utilities.MemberIsInPilot(md.ExtendedPlayCode))
                    {
                        if ((DateTime.Today - transactionDate.Date).TotalDays > totalDaysToLookBack)
                        {
                            switch (transactionType)
                            {
                                case TransactionType.Online:
                                    finalStartTransactionDate = DateTime.Today.AddDays(totalDaysToLookBack * -1);//We substract the number of days specified in the global value LimitDaysCreditRequest
                                    return false;//It allow to continue but the start date and the end date update their values
                                case TransactionType.StoreLookup:
                                    return false;//It allow to continue
                                case TransactionType.OnlineLookup:
                                    return false;//It allow to continue
                            }
                            return true;
                        }
                    }
                    // AEO-401 end


                }
                return false;
            }
        }

        /// <summary>
        /// Event for searching the data
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event arguments</param>
        protected void CmdSearch_Click(object sender, EventArgs e)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            
            this.lblEmptyResultMsg.Visible = false;

            if (!this.ValidateMemberEmail())
            {
                this.lblApliedMsg.Text = "Please add a valid email address to the member's profile to submit a request for Points.";
                this.lblApliedMsg.ForeColor = Color.Red;
                return;
            }

            this.moduleConfiguration.SearchFields.Clear();
            this.lblApliedMsg.Text = string.Empty;            

            try
            {
                // if (this.requestCreditConfig != null && this.requestCreditConfig.AttributesResultfields.Count > 0)
                // {
                this._searchList = this.moduleConfiguration.SearchFields;
                this.GetSearchValues(pnlSearchFormTable);

                if (!this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()))
                {
                    if (_searchList.ContainsKey("txt_TxnDate") && !String.IsNullOrEmpty(_searchList["txt_TxnDate"]))
                    {
                        /*For redesign project-GetPoints CS Module Technical Design. 
                        Search transactions no older than days specified in the global value 
                        LimitDaysCreditRequest*/
                        using (var dataService = _dataUtil.DataServiceInstance())
                        {
                            DateTime dtTxnDate = DateTime.Today;
                            int daysLimitToSearch = Convert.ToInt32(dataService.GetClientConfigProp("LimitDaysCreditRequest"));
                            TransactionType transactionType = GetTransactionType(this.ddlTransactionType.SelectedValue);

                            if (_searchList.ContainsKey("txt_TxnDate"))
                            {
                                DateTime.TryParse(_searchList["txt_TxnDate"], out dtTxnDate);
                            }

                            if (IsLimitDaysForPilotMembers(dtTxnDate, transactionType, out dtTxnDate))
                            {
                                this.lblApliedMsg.Text = "Please enter a date within the last " + daysLimitToSearch.ToString() + " days.";
                                this.lblApliedMsg.ForeColor = Color.Red;
                                return;
                            }

                            _searchList["txt_TxnDate"] = dtTxnDate.ToString("MM/dd/yyyy");//We set the date of the out parameter passed                    
                        }
                    }
                    else
                    {
                        this.lblApliedMsg.Text = "Transaction Date not valid.";
                        this.lblApliedMsg.ForeColor = Color.Red;
                        return;
                    }
                }
                
                // RKG - 11/10/14 The Jira number is AEO-34; the PI is 29741.
                // With new Tlog processing in the database we are changing all request for points in api and CS Portal to automatically go to the queue because we 
                // no longer will do automatic lookups.
                if (this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.Online).ToString()) || this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.Store).ToString()))
                //if (this.GetSearchButton().Text == "Submit")
                {                           
                        LoyaltyTransaction.CreateMemberReceipt(WebUtilities.GetLoyaltyMemberFromCache(), this.moduleConfiguration.SearchFields, this.GetTransactionType(this.ddlTransactionType.SelectedItem.Value), WebUtilities.GetCurrentUserName(), (long)ReceiptStatus.Processing); //PI14342
                        // this.GetSearchButton().Text = "Submit";
                        this.lblAddTransaction.Text = "The transaction selected has been submitted for credit and a confirmation email has been sent to the address on the member profile. No Points have been issued at this time. If a matching transaction is received within the next 5 days, the member's account will be credited and an additional confirmation email will be sent to the member's email address on the profile.";
                        this.LoadGetPointsHistory();
                        this.ClearSearch(pnlSearchFormTable);
                        this.SendEmail();                    
                }
                else
                {
                    int intResult = this.LoadResultGrid(this.moduleConfiguration.SearchFields);
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "intResult: " + intResult);

                    this.resultGrid.Rebind();
                    if (intResult == 0)
                    {
                        this.lblEmptyResultMsg.Visible = true;
                        // Customizing error message for Online/Store lookup
                        if (this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) || this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.StoreLookup).ToString()))
                            this.lblEmptyResultMsg.Text = "A transaction matching the information submitted was not found. Please confirm the data entered.";
                        else
                            this.lblEmptyResultMsg.Text = "A transaction matching the information submitted was not found. Please confirm the data entered.<br/><br/>  If the data entered is correct, the transaction may not have been received in the loyalty system. Click ‘Submit Request’ to submit this transaction information for Get Points processing. If a matching transaction is received within 5 days, the member account will be credited.<br/><br/>   Note: All fields are required to submit a request for future processing.";
                    }
                    else if (intResult == (int)(ReceiptStatus.AlreadyRequested) | intResult == (int)(ReceiptStatus.AlreadyPosted))
                    {
                        //PI14540 - disallow Get Points request for txn that has already been requested
                        if (intResult == (int)(ReceiptStatus.AlreadyPosted))
                        {
                            this.lblEmptyResultMsg.Text = "*Request Point has already been POSTED.";
                        }
                        else
                        {
                            this.lblEmptyResultMsg.Text = "*Points have already been requested, and are being processed.";
                        }
                        this.lblEmptyResultMsg.Visible = true;
                    }
                    else
                    {
                        this.lblEmptyResultMsg.Visible = false;
                    }
                }
                //}     
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                
                //ShowAlert(ex); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                ShowWarning(ex.ToString());
                
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        /// <summary>
        /// Event for cancel the search
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event arguments</param>
        protected void CmdCancel_Click(object sender, EventArgs e)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                this.ClearSearch(pnlSearchFormTable);
                this.lblEmptyResultMsg.Visible = false;
                this.lblApliedMsg.Text = string.Empty;
                this.lblResultGridTitle.Text = string.Empty;
                // this.GetSearchButton().Text = "Submit";
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        /// <summary>
        /// Event for select button
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event arguments</param>
        protected void Grid_GridActionClicked(object sender, GridActionClickedArg e)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            string noteText = string.Empty;

            if (e.CommandName == AspDynamicGrid.SELECT_COMMAND)   
            {
                string strRowKey = Convert.ToString(e.Key);
                try
                {
                    using (var dataService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        string strDefaultAmount = string.Empty;
                        string strTxnHeaderId = string.Empty;

                        LWCriterion criterion = new LWCriterion("HistoryTxnDetail");
                        criterion.Add(LWCriterion.OperatorType.AND, "RowKey", strRowKey, LWCriterion.Predicate.Eq);
                        //IList<IClientDataObject> historyRecords = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "HistoryTxnDetail", criterion, new LWQueryBatchInfo(), false);
                        // AEO-74 Upgrade 4.5 here -----------SCJ
                        IList<IClientDataObject> historyRecords = dataService.GetAttributeSetObjects(null, "HistoryTxnDetail", criterion, null, false);

                        HistoryTxnDetail historyTxnDetail = historyRecords[0] as HistoryTxnDetail;
                        strDefaultAmount = Convert.ToString(historyTxnDetail.TxnQualPurchaseAmt);
                        strTxnHeaderId = historyTxnDetail.TxnHeaderID;

                        //PI21354 - Ensure transactoin selected from Store or online Lookup won't be further processed if the request is being processed or posted.
                        if (this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) || this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.StoreLookup).ToString()))
                        {
                            int intSearchResult = 0;
                            intSearchResult = LoyaltyTransaction.SearchReceiptDetails(historyTxnDetail.TxnNumber, historyTxnDetail.StoreNumber, historyTxnDetail.TxnRegisterNumber, historyTxnDetail.TxnDate, historyTxnDetail.TenderAmount.ToString(), historyTxnDetail.OrderNumber);
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "intSearchResult: " + intSearchResult);

                            if (intSearchResult == (int)(ReceiptStatus.AlreadyPosted))
                            {

                                //ShowAlert("AEMessage|*Request Point has already been POSTED."); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                                ShowWarning("AEMessage|*Request Point has already been POSTED.");

                                return;
                            }
                            else if (intSearchResult == (int)(ReceiptStatus.AlreadyRequested))
                            {

                                //ShowAlert("AEMessage|*Points have already been requested, and are being processed."); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                                ShowWarning("AEMessage|*Points have already been requested, and are being processed.");

                                return;
                            }
                            else if (historyTxnDetail.ProcessID.HasValue && (historyTxnDetail.ProcessID == (long)ProcessId.ProcessedforLoyalty || historyTxnDetail.ProcessID == (long)ProcessId.RequestCreditProcesssed))
                            {

                                //ShowAlert("AEMessage|*Credit is already applied."); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                                ShowWarning("AEMessage|*Credit is already applied.");

                                return;
                            }
                        }

                        if (historyTxnDetail.ProcessID.HasValue && (historyTxnDetail.ProcessID == (long)ProcessId.ProcessedforLoyalty || historyTxnDetail.ProcessID == (long)ProcessId.RequestCreditProcesssed))
                        {
                            this.lblApliedMsg.Text = "Credit is already applied.";
                        }
                        else
                        {
                            // RKG - 11/10/14 The Jira number is AEO-34; the PI is 29741.
                            // With new Tlog processing in the database we are changing all request for points in api and CS Portal to automatically go to the queue because we 
                            // no longer will do automatic lookups.
                            // On this part I just commented out the AddLoyaltyTransaction call and made the isSuccess flag true so it would just fall through and create the memberreceipt for StoreLookup and 
                            // OnlineLookup.  The normal Store and Online TxnTypes will not make it to here because they will automatically go to the queue.

                            // Update History Tables
                            Member member = WebUtilities.GetLoyaltyMemberFromCache();

                            //PI14342 - Post the Realtime GetPoint transaction in the GetPoint page
                            bool isSuccess = true;
                            //LoyaltyTransaction.AddLoyaltyTransaction(member, strTxnHeaderId, ProcessId.RequestCreditProcesssed, "Get Points", out isSuccess);

                            if (isSuccess == true)
                            {
                                this._searchList = this.moduleConfiguration.SearchFields;
                                this.GetSearchValues(pnlSearchFormTable);

                                if (this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.StoreLookup).ToString()))
                                {
                                    //PI21354 - Ensure transactoin selected from StoreLookup won't miss details in GetPointHistory
                                    Dictionary<string, string> transactionSearchParams = new Dictionary<string, string>();
                                    transactionSearchParams.Add("txt_TxnDate", historyTxnDetail.TxnDate.ToString());
                                    transactionSearchParams.Add("txt_TxnNumber", historyTxnDetail.TxnNumber);
                                    transactionSearchParams.Add("txt_TxnRegisterNumber", historyTxnDetail.TxnRegisterNumber);
                                    transactionSearchParams.Add("txt_TxnStoreID", historyTxnDetail.StoreNumber);
                                    //transactionSearchParams.Add("txt_TenderAmount", string.Format("{0:#####.00}", (double)(historyTxnDetail.TenderAmount))); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                                    transactionSearchParams.Add("txt_TenderAmount", string.Format("{0:#####.00}", (historyTxnDetail.TxnQualPurchaseAmt.HasValue ? historyTxnDetail.TxnQualPurchaseAmt.Value : 0)));
                                    LoyaltyTransaction.CreateMemberReceipt(WebUtilities.GetLoyaltyMemberFromCache(), transactionSearchParams, this.GetTransactionType(this.ddlTransactionType.SelectedItem.Value), WebUtilities.GetCurrentUserName(), (long)ReceiptStatus.Processing);
                                }
                                else if (this.ddlTransactionType.SelectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()))
                                {
                                    //PI21354 - Ensure transactoin selected from OnlineLookup won't miss details in GetPointHistory//PI21354 - 
                                    Dictionary<string, string> OrderSearchParams = new Dictionary<string, string>();
                                    OrderSearchParams.Add("txt_OrderNumber", historyTxnDetail.OrderNumber.ToString());
                                    //OrderSearchParams.Add("txt_TenderAmount", string.Format("{0:#####.00}", (double)(historyTxnDetail.TenderAmount))); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                                    OrderSearchParams.Add("txt_TenderAmount", string.Format("{0:#####.00}", (historyTxnDetail.TxnQualPurchaseAmt.HasValue ? historyTxnDetail.TxnQualPurchaseAmt.Value : 0)));
                                    LoyaltyTransaction.CreateMemberReceipt(member, OrderSearchParams, this.GetTransactionType(this.ddlTransactionType.SelectedItem.Value), WebUtilities.GetCurrentUserName(), (long)ReceiptStatus.Processing);
                                }
                                else
                                {
                                    LoyaltyTransaction.CreateMemberReceipt(WebUtilities.GetLoyaltyMemberFromCache(), this.moduleConfiguration.SearchFields, this.GetTransactionType(this.ddlTransactionType.SelectedItem.Value), WebUtilities.GetCurrentUserName(), (long)ReceiptStatus.Processing);
                                }
                                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "TransactionType: " + this.GetTransactionType(this.ddlTransactionType.SelectedItem.Value));


                                this.SendEmail();

                                // Adding CSNotes
                                noteText = "Transaction in the amount of " + strDefaultAmount + " was requested";
                                Utilities.CreateCSNote(noteText, member.IpCode, WebUtilities.GetCurrentUserId());
                                this.ClearSearch(this.pnlSearchFormTable);
                                this.LoadGetPointsHistory();
                                this.lblAddTransaction.Text = "The transaction selected has been submitted for credit and a confirmation email has been sent to the address on the member profile. No Points have been issued at this time. If a matching transaction is received within the next 5 days, the member's account will be credited and an additional confirmation email will be sent to the member's email address on the profile.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    
                    //ShowAlert(ex); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                    ShowNegative(ex.ToString());
                    
                }
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        #endregion

        #region Search Controls

        /// <summary>
        /// Method to load search form
        /// </summary>
        /// <param name="transactionType">Transaction type</param>
        private void LoadSearchControls(TransactionType transactionType)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            TableRow row = null;            
            switch (transactionType)
            {
                case TransactionType.Store:
                    this.CreateControls(GetStoreConfiguration(TransactionType.Store));
                    break;
                case TransactionType.Online:
                    this.CreateControls(GetOnlineConfiguration(TransactionType.Online));
                    break;
                // Added by Manoj
                case TransactionType.OnlineLookup:
                    this.CreateControls(GetOnlineConfiguration(TransactionType.OnlineLookup));
                    break;
                case TransactionType.StoreLookup:
                    this.CreateControls(GetStoreConfiguration(TransactionType.StoreLookup));
                    break;
                // Ends Here
                default:
                    break;
            }

            /* AEO-4225 [DEV - Francisco Sosa]
            row = new TableRow();
            pnlSearchFormTable.Rows.Add(row);

            row = new TableRow();
            TableCell tcell = new TableCell();
            tcell.ColumnSpan = 3;
            tcell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(tcell);

            Boolean AllowSubmit = WebUtilities.isMemberAllowedToSubmit(); //AEO-1644
            Boolean AllowOnlyView = WebUtilities.isRoleAllowedOnlyToView("ViewRequestCredit"); //AEO-1602

            //LNLinkButton btn = new LNLinkButton(); // AEO-74 Upgrade 4.5 changes here -----------SCJ
            CustomLinkButtom btn = new CustomLinkButtom(); 
            btn.ID = "cmdCancel";
            btn.Text = "Cancel";
            btn.CausesValidation = false;
          //  btn.ButtonType = LNLinkButton.ButtonTypes.Cancel;// AEO-74 Upgrade 4.5 changes here -----------SCJ
            btn.ButtonType =  CustomLinkButtom.ButtonTypes.Cancel;
            btn.Click += new EventHandler(this.CmdCancel_Click);
            //AEO-1644 Begin
            if (AllowSubmit)
            {
                btn.Visible = false;
            }
            //AEO-1644 End
            tcell.Controls.Add(btn);

            Label spacer = new Label();
            spacer.Text = " ";
            tcell.Controls.Add(spacer);

            //btn = new LNLinkButton();// AEO-74 Upgrade 4.5 changes here -----------SCJ
            btn = new CustomLinkButtom();

            btn.ID = "cmdSearch";
            btn.Text = "Submit";
            btn.ValidationGroup = "ValiDationSearchControls";
            //btn.ButtonType = LNLinkButton.ButtonTypes.Submit;// AEO-74 Upgrade 4.5 changes here -----------SCJ
            btn.ButtonType = CustomLinkButtom.ButtonTypes.Submit;
            btn.Click += new EventHandler(this.CmdSearch_Click);
            if (AllowSubmit || AllowOnlyView) //AEO-1602
            {
                btn.Visible = false;
            }
            tcell.Controls.Add(btn);

            row.Cells.Add(tcell);
            tblSearchCtrls.Rows.Add(row);
            */
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        /// <summary>
        /// Crate controls to search from configuration
        /// </summary>
        /// <param name="lstConfigurationItem">List oconfiguration items</param>
        private void CreateControls(List<ModuleConfiguration> lstConfigurationItem)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            TableRow row = null;
            TableCell cell = null;
            foreach (ModuleConfiguration attribute in lstConfigurationItem)
            {
                row = new TableRow();
                Label label = new Label();
                label.ID = "lbl" + attribute.Name;
                label.Text = attribute.DisplayText + ":&nbsp;&nbsp;";
                cell = new TableCell();
                cell.Controls.Add(label);
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.VerticalAlign = VerticalAlign.Top;
                row.Cells.Add(cell);
                TextBox textBox = new TextBox();
                textBox.ID = "txt_" + attribute.Name;
                textBox.Rows = 1;

                Label lblLabel = new Label();
                lblLabel.Text = "<br/>" + attribute.DescriptiveText;

                cell = new TableCell();
                cell.Controls.Add(textBox);
                cell.Controls.Add(lblLabel);
                row.Cells.Add(cell);
                if (!string.IsNullOrEmpty(attribute.Format))
                {
                    this.AddRegularExpressionValidator(cell, attribute, textBox);
                }

                if (attribute.IsRequired)
                {
                    this.AddRequiredValidator(cell, textBox, attribute);
                }

                row.Cells.Add(cell);
                pnlSearchFormTable.Rows.Add(row);
                row = new TableRow();
                cell = new TableCell();
                cell.ColumnSpan = 2;
                cell.Text = "&nbsp;";
                row.Cells.Add(cell);
                pnlSearchFormTable.Rows.Add(row);
                tblSearchCtrls.Visible = true;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        public List<ModuleConfiguration> GetStoreConfiguration(TransactionType txntype)
        {
            char quote = '"';

            List<ModuleConfiguration> lstConfigurationItems = new List<ModuleConfiguration>();

            lstConfigurationItems.Add(BuildAttribute("TxnStoreID", "<span style=" + quote + "color: red;" + quote + ">*</span>Store Number", @"^\d{5}$", "5 digits", "Store Number must be 5 digits.", true));
            if (txntype == TransactionType.Store)
            {
                lstConfigurationItems.Add(BuildAttribute("TxnRegisterNumber", "<span style=" + quote + "color: red;" + quote + ">*</span>Register Number", @"^\d{3}$", "3 digits.", "Register Number must be 3 digits.", true));
                lstConfigurationItems.Add(BuildAttribute("TenderAmount", "<span style=" + quote + "color: red;" + quote + ">*</span>Total", @"^\d+(\.\d{1,2})?$", "Format: 200.00", "Total must be in numeric format.", true));
                lstConfigurationItems.Add(BuildAttribute("TxnNumber", "<span style=" + quote + "color: red;" + quote + ">*</span>Transaction Number", @"^\d{3}[0-9]?[0-9]?[0-9]?$", "3 to 6 digits.", "Transaction Number must be 3 to 6 digits.", true));
            }
            else  //if storelookup
            {
                lstConfigurationItems.Add(BuildAttribute("TxnRegisterNumber", "Register Number", @"^\d{3}$", "3 digits.", "Register Number must be 3 digits.", false));
                lstConfigurationItems.Add(BuildAttribute("TenderAmount", "Total", @"^\d+(\.\d{1,2})?$", "Format: 200.00", "Total must be in numeric format.", false));
                lstConfigurationItems.Add(BuildAttribute("TxnNumber", "Transaction Number", @"^\d{3}[0-9]?[0-9]?[0-9]?$", "3 to 6 digits.", "Transaction Number must be 3 to 6 digits.", false));
            }

            lstConfigurationItems.Add(BuildAttribute("TxnDate", "<span style=" + quote + "color: red;" + quote + ">*</span>Transaction Date", @"^(([1-9])|(0[1-9])|(1[0-2]))\/(([0-9])|([0-2][0-9])|(3[0-1]))\/(([0-9][0-9])|([1-2][0,9][0-9][0-9]))$", "MM/DD/YYYY", "Please enter a valid transaction date.  Transaction Date must be within the current calendar year.", true));

            return lstConfigurationItems;
        }

        public List<ModuleConfiguration> GetOnlineConfiguration(TransactionType txntype)
        {
            char quote = '"';

            List<ModuleConfiguration> lstConfigurationItems = new List<ModuleConfiguration>();

            lstConfigurationItems.Add(BuildAttribute("OrderNumber", "<span style=" + quote + "color: red;" + quote + ">*</span>Order Number", @"\b\w{5,10}\b", "5-10 character.", "Order Number must be 5-10 characters.", true));

            if (txntype == TransactionType.Online)
            {
                lstConfigurationItems.Add(BuildAttribute("TenderAmount", "<span style=" + quote + "color: red;" + quote + ">*</span>Order Amount", @"^\d+(\.\d{1,2})?$", "Format: 200.00", "Order Amount must be in numeric format.", true));
                lstConfigurationItems.Add(BuildAttribute("TxnDate", "<span style=" + quote + "color: red;" + quote + ">*</span>Transaction Date", @"^(([1-9])|(0[1-9])|(1[0-2]))\/(([0-9])|([0-2][0-9])|(3[0-1]))\/(([0-9][0-9])|([1-2][0,9][0-9][0-9]))$", "MM/DD/YYYY", "Please enter a valid transaction date.  Transaction Date must be within the current calendar year.", true));
            }
            else //if OnlineLookup
            {
                lstConfigurationItems.Add(BuildAttribute("TenderAmount", "Order Amount", @"^\d+(\.\d{1,2})?$", "Format: 200.00", "Order Amount must be in numeric format.", false));

            }

            return lstConfigurationItems;
        }

        private ModuleConfiguration BuildAttribute(string attributeName, string displayText, string format, string descriptiveText, string message, bool isRequired)
        {
            ModuleConfiguration c = new ModuleConfiguration();
            c.Name = attributeName;
            c.DisplayText = displayText;
            c.DescriptiveText = descriptiveText;
            c.Format = format;
            c.Message = message;
            c.IsRequired = isRequired;
            return c;
        }

        /// <summary>
        /// Add compare validator to compare from and to values
        /// </summary>
        /// <param name="confAttribute">Configuration attribute</param>
        /// <param name="tabRow">Table row where to put validator</param>
        /// <param name="tabCell">Table cell where to put validator</param>
        private void AddCompareValidator(ConfigurationItem confAttribute, TableRow tabRow, TableCell tabCell)
        {
            // Compare start and end values through compare validators
            CompareValidator compareValidator = new CompareValidator();
            compareValidator.ControlToValidate = "txt_" + confAttribute.AttributeName + "_Start";
            compareValidator.ControlToCompare = "txt_" + confAttribute.AttributeName + "_End";
            compareValidator.Operator = ValidationCompareOperator.LessThanEqual;
            compareValidator.ValidationGroup = "ValiDationSearchControls";
            compareValidator.Type = (ValidationDataType)Enum.Parse(typeof(ValidationDataType), this.GetAttributeType(confAttribute.AttributeName));
            compareValidator.ErrorMessage = "Either value is not a valid date or 'From value' is greater than 'To value'.";
            compareValidator.ForeColor = Color.Red;
            tabCell.Controls.Add(compareValidator);
            tabCell.VerticalAlign = VerticalAlign.Top;
            tabRow.Cells.Add(tabCell);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Add rgular expression validator to specified text box
        /// </summary>
        /// <param name="cell">Table cell</param>
        /// <param name="attribute">Configuation item</param>
        /// <param name="textBox">text box to attach regular expression validator</param>
        private void AddRegularExpressionValidator(TableCell cell, ModuleConfiguration attribute, TextBox textBox)
        {
            RegularExpressionValidator validator = new RegularExpressionValidator();
            validator.ControlToValidate = textBox.ID;
            validator.Display = ValidatorDisplay.Dynamic;
            validator.ValidationExpression = attribute.Format;
            validator.ValidationGroup = "ValiDationSearchControls";
            validator.ErrorMessage = "&nbsp;&nbsp;&nbsp;" + attribute.Message;
            validator.ForeColor = Color.Red;
            cell.Controls.Add(validator);
        }

        /// <summary>
        /// Add required field validator to specified text box
        /// </summary>
        /// <param name="cell">Table cell where to put require field validator</param>
        /// <param name="textBox">Text box to validate</param>
        /// <param name="attribute">Configuration item</param>
        private void AddRequiredValidator(TableCell cell, TextBox textBox, ModuleConfiguration attribute)
        {
            RequiredFieldValidator validator = new RequiredFieldValidator();
            validator.ControlToValidate = textBox.ID;
            validator.Display = ValidatorDisplay.Dynamic;
            validator.ValidationGroup = "ValiDationSearchControls";
            validator.ErrorMessage = "&nbsp;&nbsp;&nbsp;" + attribute.DisplayText.Replace("*", string.Empty) + " is required.";
            validator.ForeColor = Color.Red;
            cell.Controls.Add(validator);
        }

        /// <summary>
        /// Validate member email
        /// </summary>
        /// <returns>Whether members email is valid or not</returns>
        private bool ValidateMemberEmail()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            bool blnReturn = false;
            try
            {
                Member member = WebUtilities.GetLoyaltyMemberFromCache();
                if (member == null)
                {
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member is null");
                    return false;
                }
                MemberDetails memberDetails = this.GetMemberDetails(member);
                if (!string.IsNullOrEmpty(memberDetails.EmailAddress))
                {
                    blnReturn = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert(ex);
                ShowNegative(ex.ToString());
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end - " + blnReturn.ToString());
            return blnReturn;
        }

        /// <summary>
        /// Returns _member details of a _member from cache
        /// </summary>
        /// <param name="member">Loyalty member</param>
        /// <returns>Loyalty member details</returns>
        private MemberDetails GetMemberDetails(Member member)
        {
            IList<IClientDataObject> lstMemberAttributes = member.GetChildAttributeSets("MemberDetails");
            if (null != lstMemberAttributes && lstMemberAttributes.Count > 0)
            {
                return lstMemberAttributes[0] as MemberDetails;
            }

            return null;
        }

        /// <summary>
        /// Method for clearing the search 
        /// </summary>
        /// <param name="ctlControl">Control to clear text</param>
        private void ClearSearch(Control ctlControl)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            foreach (Control ctrl in ctlControl.Controls)
            {
                if (ctrl is TableRow)
                {
                    foreach (Control rctrl in ctrl.Controls)
                    {
                        if (rctrl is TableCell)
                        {
                            foreach (Control tdctrl in rctrl.Controls)
                            {
                                if (tdctrl is TextBox)
                                {
                                    ((TextBox)tdctrl).Text = string.Empty;
                                }

                                if (tdctrl is Table)
                                {
                                    this.ClearSearch(tdctrl as Table);
                                }
                            }
                        }
                    }
                }
            }

            pnlSearchResult.Visible = false;
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        /// <summary>
        /// Loads GetPoints
        /// </summary>
        private void LoadGetPointsHistory()
        {
            List<MemberReceipts> memberReceipts = new List<MemberReceipts>();
            try
            {
                Member member = WebUtilities.GetLoyaltyMemberFromCache();
                if (member == null)
                {
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member is null");
                    return;
                }
                memberReceipts = LoyaltyTransaction.LoadMemberReceipts(WebUtilities.GetLoyaltyMemberFromCache());
                this.phGetPointHistory.Controls.Clear();
                if (memberReceipts.Count > 0)
                {
                    pnlGetPointHistory.Visible = true;
                    phGetPointHistory.Visible = true;
                    pnlGetPointHistoryEmpty.Visible = false;
                    lblGetPointHistoryEmptyMessage.Visible = false;
                    this.historyGrid = new AspDynamicGrid(); 
                    this.historyGrid.Provider = (IDynamicGridProvider)(new GetPointHistoryGridProvider());
                    this.historyGrid.SetSearchParm(string.Empty, memberReceipts);
                    this.phGetPointHistory.Controls.Add(this.historyGrid);
                }
                else
                {
                    pnlGetPointHistory.Visible = false;
                    phGetPointHistory.Visible = false;
                    pnlGetPointHistoryEmpty.Visible = true;
                    lblGetPointHistoryEmptyMessage.Visible = true;
                    lblGetPointHistoryEmptyMessage.Text = "There are no previous requests for this member!";
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert(ex);
                ShowNegative(ex.ToString());
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            }
        }

        /// <summary>
        /// Initialize module configuration
        /// </summary>
        private void InitializeModuleConfiguration()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            if (true)
            {
                pnlSearchForm.Visible = true;
                rdbTransactionType.Visible = false;
                ddlTransactionType.Visible = true;
                ddlTransactionType.Items.Add(new ListItem("Store", Convert.ToString((int)TransactionType.Store)));
                ddlTransactionType.Items.Add(new ListItem("Online", Convert.ToString((int)TransactionType.Online)));
                ddlTransactionType.Items.Add(new ListItem("Online Lookup", Convert.ToString((int)TransactionType.OnlineLookup)));
                ddlTransactionType.Items.Add(new ListItem("Store Lookup", Convert.ToString((int)TransactionType.StoreLookup)));
                hdnInsertVal.Value = ddlTransactionType.SelectedValue;
                this.LoadSearchControls(this.GetTransactionType(hdnInsertVal.Value));
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Converts string transaction type to corresponding enum
        /// </summary>
        /// <param name="strTransactionType">input trasaction type</param>
        /// <returns>transaction type enum</returns>
        private TransactionType GetTransactionType(string strTransactionType)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            TransactionType transactionType = TransactionType.Store;
            try
            {
                transactionType = (TransactionType)Enum.Parse(typeof(TransactionType), strTransactionType);
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");

            return transactionType;
        }

        /// <summary>
        /// Get search values from control
        /// </summary>
        /// <param name="ctlControl">Control to search value</param>
        private void GetSearchValues(Control ctlControl)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            foreach (Control ctrl in ctlControl.Controls)
            {
                if (ctrl is TableRow)
                {
                    foreach (Control rctrl in ctrl.Controls)
                    {
                        if (rctrl is TableCell)
                        {
                            foreach (Control tdctrl in rctrl.Controls)
                            {
                                if (tdctrl is TextBox)
                                {
                                    TextBox txtBox = tdctrl as TextBox;
                                    string attributeName = tdctrl.ID;
                                    if (!string.IsNullOrEmpty(txtBox.Text))
                                    {
                                        this.moduleConfiguration.SearchFields.Add(attributeName, txtBox.Text);
                                    }
                                }

                                if (tdctrl is Table)
                                {
                                    this.GetSearchValues(tdctrl);
                                }
                            }
                        }
                    }
                }
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
        }

        /// <summary>
        /// Send email to user
        /// </summary>
        private void SendEmail()
        {

            Member member = WebUtilities.GetLoyaltyMemberFromCache();
            IList<IClientDataObject> memberDetails = member.GetChildAttributeSets("MemberDetails");
            MemberDetails mbrDetails = (MemberDetails)memberDetails[0];

            Dictionary<string, string> additionalFields = new Dictionary<string, string>();
            additionalFields.Add("FirstName", member.FirstName);
            AEEmail.SendEmail(member, EmailType.RequestCreditReceived, additionalFields, mbrDetails.EmailAddress);
        }

        /// <summary>
        /// Returns type of attribute for compare validator
        /// </summary>
        /// <param name="strAttributeName">Name of the attribute</param>
        /// <returns>Get the string attribute type</returns>
        private string GetAttributeType(string strAttributeName)
        {
            Type t = typeof(HistoryTxnDetail);
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in properties)
            {
                if (pi.Name == strAttributeName)
                {
                    string tempType = pi.PropertyType.ToString();

                    // If Type is Nullable
                    if (tempType.Contains("Null"))
                    {
                        // SubString to get Type from full name
                        string strType = pi.PropertyType.FullName.Substring(0, pi.PropertyType.FullName.IndexOf(","));
                        tempType = strType.Substring(strType.LastIndexOf(".") + 1);
                        return this.GetRelatedType(tempType);
                    }
                    else
                    {
                        return this.GetRelatedType(tempType);
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Method to get related type of ValidationDataType enum
        /// </summary>
        /// <param name="strType">string type to get related tape that need to be used in compare validator</param>
        /// <returns>Returns related validator type</returns>
        private string GetRelatedType(string strType)
        {
            string retVal = "Integer";
            switch (strType)
            {
                case "Int16":
                    retVal = "Integer";
                    break;
                case "int":
                    retVal = "Integer";
                    break;
                case "Int64":
                    //retVal = "Double"; // AEO-74 Upgrade 4.5 changes here -----------SCJ
                    retVal = "Decimal";
                    break;
                case "DateTime":
                    retVal = "Date";
                    break;
                case "string":
                    retVal = "string";
                    break;
            }

            return retVal;
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        /// <summary>
        /// Returns search button (created dynamically using configuration)
        /// </summary>
        /// <returns>Command search buttong named cmdSearch</returns>        
        //private  LNLinkButton GetSearchButton()
        //{
        //    LNLinkButton lnbtn = null;
        //    foreach (TableCell cell in tblSearchCtrls.Rows[0].Cells)
        //    {
        //        lnbtn = (LNLinkButton)cell.FindControl("cmdSearch");
        //        break;
        //    }

        //    return lnbtn;
        //}

        private CustomLinkButtom GetSearchButton()
        {
            CustomLinkButtom lnbtn = null;
            foreach (TableCell cell in tblSearchCtrls.Rows[0].Cells)
            {
                lnbtn = (CustomLinkButtom)cell.FindControl("cmdSearch");
                break;
            }

            return lnbtn;
        }
        #endregion

        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    }
}
