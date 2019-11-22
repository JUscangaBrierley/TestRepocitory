using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Brierley.WebFrameWork.Controls;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Ipc;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;
using Brierley.ClientDevUtilities.LWGateway;

namespace Brierley.AEModules.PointsCalculator
{
    public partial class ViewPointCalculator :  ModuleControlBase, IIpcEventHandler
    {
        #region Member Variables
        /// <summary>
        /// Logger for log trace, debug or error information
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("PointCalculator");

        private static ILWDataServiceUtil _dataUtil = new ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630

        /// <summary>
        ///  Results list of executed header rules 
        /// </summary>
        private List<Brierley.FrameWork.ContextObject.RuleResult> results = new List<Brierley.FrameWork.ContextObject.RuleResult>();

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                IpcManager.RegisterEventHandler("EnablePointCalfromawardPoints", this, false);
                IpcManager.RegisterEventHandler("EnablePointCalfromRequestCredit", this, false);
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowWarning("An unexpected error has occurred.Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
            }

        }
        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }
        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                decimal _totalPoints = 0;

                if ((AmountSpentTxtBox.Text.Trim() != null) && AmountSpentTxtBox.Text.Trim() != System.String.Empty)
                {
                    string UserRole = WebUtilities.GetCurrentUserRole().ToLower();
                    if (UserRole == "supervisor" || UserRole == "csr" || UserRole == "super admin") //AEO-1605
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                        Member member = PortalState.GetFromCache("SelectedMember") as Member;
                        // Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID("80105813800244");
                        VirtualCard vc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                        TxnHeader txnHeader = new TxnHeader();
                        txnHeader.StoreNumber = 123456;
                        txnHeader.TxnDate = System.DateTime.Now;
                        try
                        {
                            txnHeader.TxnQualPurchaseAmt = Decimal.Parse(AmountSpentTxtBox.Text.Trim().Length > 0 ? AmountSpentTxtBox.Text.Trim() : "0"); // National rollout fix
                            if (AmountSpentTxtBox.Text.Trim().Contains('.'))
                            {
                                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Invalid purchase amount entered: " + AmountSpentTxtBox.Text.Trim());
                                rePoints.ErrorMessage = "Please provide whole numbers only.";
                                rePoints.IsValid = false;
                                return;
                            }

                        }
                        catch (FormatException)
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Invalid purchase amount entered: " + AmountSpentTxtBox.Text.Trim());
                            rePoints.ErrorMessage = "Invalid purchase amount. Purchase amount must be numeric.";
                            rePoints.IsValid = false;
                            return; //throw new FormatException("Invalid point amount: " + AmountSpentTxtBox.Text.Trim());
                        }
                        txnHeader.TxnTypeId = 1;
                        txnHeader.BrandId = "1";  //Set the brandid as an AE purchase
                        txnHeader.TxnHeaderId = "123456789";
                        txnHeader.TxnRegisterNumber = "001";
                        txnHeader.TxnNumber = "148323";
                        txnHeader.VcKey = vc.VcKey;
                        txnHeader.Parent = vc;
                        vc.AddChildAttributeSet(txnHeader);

                        using(ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            foreach (RuleTrigger rule in txnHeader.GetMetaData().RuleTriggers)
                            {
                                if ( rule.InvocationType == Enum.GetName(typeof(RuleInvocationType), RuleInvocationType.AfterInsert) 
                                    && rule.Rule.ToString() == "AmericanEagle.CustomRules.AEAwardPoints" && rule.IsActive == true)
                                {
                                    dataService.Execute(rule, vc, txnHeader, results, RuleExecutionMode.Simulation);
                                }
                            }

                        }                        

                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Saved.  Results:");


                        foreach (Brierley.FrameWork.ContextObject.RuleResult result in results)
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Result Name: " + result.Name);

                            if ((result.GetType() == typeof(Brierley.FrameWork.Rules.AwardPointsRuleResult)) && (result.Name.Contains("Basic")))
                            {
                                Brierley.FrameWork.Rules.AwardPointsRuleResult rule = (Brierley.FrameWork.Rules.AwardPointsRuleResult)result;
                                //_totalBasePoints += rule.PointsAwarded;
                                _totalPoints += rule.PointsAwarded;
                            }
                        }

                        //decimal _totalPoints = _totalBasePoints;
                        //HttpCookie totalpoints = new HttpCookie("Pointcalculatortotalpoints");
                        //totalpoints.Value = _totalPoints;
                        //AEO-1605 BEGIN
                        if (UserRole == "supervisor" || UserRole == "csr")
                        {
                            if (_totalPoints > 5000)   //Setting a max value as 5000
                            {
                                //  TotalPointslbl.Text = "5000";
                                TotalPointslbl.Text = _totalPoints.ToString();
                                PortalState.PutInCache("PointCalculator-TotalPoints", "5000");
                                Totalpointvalidatorlabel.Text = "Limit exceeded for role, max value is " + "5000";
                            }
                            else
                            {
                                TotalPointslbl.Text = _totalPoints.ToString();
                                PortalState.PutInCache("PointCalculator-TotalPoints", _totalPoints.ToString());
                                Totalpointvalidatorlabel.Text = "";
                            }
                        }
                        else //Rol: super admin
                        {
                            TotalPointslbl.Text = _totalPoints.ToString();
                            PortalState.PutInCache("PointCalculator-TotalPoints", _totalPoints.ToString());
                            Totalpointvalidatorlabel.Text = "";
                        }
                        //AEO-1605 END
                        IpcManager.PublishEvent("PointCalculator_Clicked", base.ConfigurationKey, member);
                    }
                    else
                    {
                        Totalpointvalidatorlabel.Text = "Only administrators or supervisors can calculate points.";
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowWarning("An unexpected error has occurred.Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
            }
        }

        public void HandleEvent(IpcEventInfo info)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "HandleEventBegin");
            if ((info.EventName == "EnablePointCalfromawardPoints"))
            {
                panelBanner.Visible = true;
                pnlFormPointCalculator.Visible = true;
                pnlFormPointCalculator2.Visible = true;
                Totalpointvalidatorlabel.Text = "";
                Totalpointvalidatorlabel.Visible = true;
            }
            if ((info.EventName == "EnablePointCalfromRequestCredit"))
            {
                panelBanner.Visible = true;
                pnlFormPointCalculator.Visible = true;
                pnlFormPointCalculator2.Visible = true;
               
            }

            
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "HandleEventEnd");
        }


        
 
    }
}