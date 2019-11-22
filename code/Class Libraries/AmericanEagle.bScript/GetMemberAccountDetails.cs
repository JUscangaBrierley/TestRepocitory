#region | Namespace |
using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Portal;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common;
using AmericanEagle.SDK.Global;
using Brierley.ClientDevUtilities.LWGateway;
#endregion

namespace AmericanEagle.bScript
{
    /// <summary>
    /// Returns member account details like AEREWARDS number, starting points, earned basic points, earned bonus points, total points, last purchase date and enrollment date
    /// <para>
    /// This method is case insensitive i.e. will return same for 'name'/'Name' or 'NAME'
    /// </para>
    /// </summary>    
    /// <example>
    /// 1. GetMemberAccountDetails('LoyaltyIDNumber')
    /// 2. GetMemberAccountDetails('name')//will rturn members full name like Manjur Alam
    /// </example>
    [ExpressionContext(Description = "This function returns Members AEREWARDS number, starting points, earned basic points, earned bonus points, total points, last purchase date, enrollment date, .",
       DisplayName = "GetMemberAccountDetails",
       ExcludeContext = ExpressionContexts.Email)]
    public class GetMemberAccountDetails : UnaryOperation
    {
        #region Member Variables

        /// <summary>
        /// Hold argument from outside  
        /// </summary>
        String _providedArg = String.Empty;
        /// <summary>
        /// Hold context member
        /// </summary>
        Member _member = null;
        /// <summary>
        /// Hold member details for context member
        /// </summary>
        MemberDetails _memberDetails = null;
        /// <summary>
        /// For service data APIs
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        private static LWLogger _logger = null;
        private const string _appName = "AmericanEagleCSPortal";
        private StateValidation stateValidate = null;

        #endregion member variables

        #region Public Methods
        /// <summary>
        /// Constructor for bScript class
        /// </summary>
        /// <param name="arg"></param>
        public GetMemberAccountDetails()
        {

        }
        public GetMemberAccountDetails(Expression arg)
            : base("GetMemberAccountDetails", arg)
        {
            // Initialize logging
            _logger = LWLoggerManager.GetLogger(_appName);

            ContextObject cObj = new ContextObject();            
            stateValidate = new StateValidation();

            try
            {
                _providedArg = arg.evaluate(cObj).ToString();
            }
            catch (Exception ex)
            {
                throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to GetMemberAccountDetails.", ex);
            }
        }
        /// <summary>
        /// Syntax to use GetMemberAccountDetails
        /// </summary>
        public override string Syntax
        {
            get
            {
                return "GetMemberAccountDetails('[LoyaltyIDNumber]|[StartingPoints]|[BasicPoints]|[BonusPoints]|[TotalPoints]|[LastPurchasedDate]|[EnrollmentDate]|[NAME]|[CurrentStatus]|[PointsToNextReward]|[CurrentQualifyinGB5G1BraPurchased]|[CurrentFreeB5G1BraEarned]|[PrimaryExpectedFullFillmentDateB5G1Bra]|[SecondaryExpectedFullFillmentDateB5G1Bra]|[TotalFreeB5G1BraMailedCurrentYear]|[TotalFreeB5G1BraMailedLifetime]|[LastFreeB5G1BraMailed]|[LastFreeB5G1BraRedeemed]|[TotalFreeB5G1BraRedeemed]|[CurrentQualifyinGB5G1JeanPurchased]|[CurrentFreeB5G1JeanEarned]|[PrimaryExpectedFullFillmentDateB5G1Jean]|[SecondaryExpectedFullFillmentDateB5G1Jean]|[TotalFreeB5G1JeanMailedCurrentYear]|[TotalFreeB5G1JeanMailedLifetime]|[LastFreeB5G1JeanMailed]|[LastFreeB5G1JeanRedeemed]|[TotalFreeB5G1JeanRedeemed]')";
            }
        }
        /// <summary>
        /// Evaluate the bScript expression
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            _logger.Trace("Custom bScript", "evaluate", "Begin");

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            String retValue = String.Empty;
            int braPromoFulFillmentThreshold = 0;

            try
            {
                _logger.Trace("Custom bScript", "evaluate", "GetMember");
                _member = contextObject.Owner as Member;
                if (_member == null)
                {
                    return retValue;
                }
                _logger.Trace("Custom bScript", "evaluate", "GetMemberDetails: " + _member.IpCode.ToString());
                _memberDetails = GetMemberDetails();

                //AEO-2108 Begin - CSPortal Date Drop Down
                String DateNR = PortalState.GetFromCache("NationalRolloutStartDate") as String;
                DateTime.TryParse(DateNR, out startDate);
                startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 00, 00, 00);
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                //AEO-2108 End

                _logger.Trace("Custom bScript", "evaluate", "Start: " + startDate.ToShortDateString() + ", End: " + endDate.ToShortDateString());
                _logger.Trace("Custom bScript", "evaluate", "Determine Call: " + _providedArg.ToUpper());
                IAEPointsUtilies pointsUtility = new AEPointsUtilies(_dataUtil);
                //PI30364  Changes begin here ----------------------------------------------------------------SCJ                 
                if (_providedArg.ToUpper().Contains("POINTSFOREVENT"))
                {
                    int strindex = _providedArg.IndexOf("|") + 1;
                    retValue = GETPOINTSFOREVENT(_member, _providedArg.Substring(strindex));  // takes the PointEvent entered after '|'
                }
                else
                    switch (_providedArg.ToUpper())
                    {

                        case "PENDINGPOINTS":                          
                            retValue = Utilities.GetPointsOnHold(_member, startDate, endDate).ToString();
                            break;
  
                        case "POINTELIGIBLEFORREWARD":
                            retValue = pointsUtility.GetDollarPointsEllegibleForRewards(_member, startDate, endDate).ToString();
                            //Lines commented by [AEO-4326] start
                            //decimal tmppending = decimal.Zero;
                            //decimal tmptotal = decimal.Zero;
                            //tmppending = Utilities.GetPointsOnHold(_member, startDate, endDate);                                                         
                            //String tmpstr = Utilities.GetTotalPoints(_member, startDate, endDate);

                            //if ( decimal.TryParse(tmpstr, out tmptotal) ) {
                            //    retValue=  ((long) ( tmptotal - tmppending )).ToString();
                            //}
                            //else {
                            // retValue =  ((long) ( -1* tmppending )).ToString();
                            //}
                            //Lines commented by [AEO-4326] end
                            break;

                        case "PENDINGBRAPOINTS":
                            retValue = Utilities.GetB5G1CurrentQualifyingPurchased(_member, endDate, "BRA", true, startDate, endDate);
                            break;

                        case "PENDINGJEANPOINTS":
                            retValue = Utilities.GetB5G1CurrentQualifyingPurchased(_member, endDate, "JEAN", true, startDate, endDate);
                            break;

                        case "LOYALTYIDNUMBER":
                            retValue = GetLoyaltyIDNumber();
                            break;

                        case "STARTINGPOINTS":
                           if ( IsPilotMember().ToLower() == "true" ) {
                                startDate = DateTime.Compare(startDate, new DateTime(1990, 1, 1)) > 0 ? new DateTime(1990, 1, 1) : startDate;
                                endDate = DateTime.Compare(endDate.Date, DateTime.Now.Date) > 0 ? endDate : DateTime.Now.Date;
                            }
                            retValue = Utilities.GetStartingPoints(_member, startDate, endDate);
                            break;
                        case "BASICPOINTS":
                            retValue = pointsUtility.GetTotalBasicDollarPoints(_member, startDate, endDate).ToString(); //AEO-5244
                            break;

                        case "BONUSPOINTS":
                            retValue = pointsUtility.GetTotalBonusDollarPoints(_member, startDate, endDate).ToString();//AEO-5244
                            break;

                        /*AEO-2690 BEGIN*/
                        case "ENGAGEMENTPOINTS":
                            retValue = Utilities.GetEngagementPoints(_member, startDate, endDate);
                            break;
                        /*AEO-2690 END*/

                        case "TOTALPOINTS":                            
                            retValue = pointsUtility.GetTotalDollarPoints(_member, startDate, endDate).ToString();
                            //retValue = Utilities.GetTotalPoints(_member, startDate, endDate); //AEO-4326
                            break;

                        case "LASTPURCHASEDDATE":
                            retValue = GetLastPurchasedDate();
                            break;

                        case "ENROLLMENTDATE":
                            retValue = GetEnrollmentDate();
                            break;

                        case "NAME":
                            retValue = GetMemberName();
                            break;

                        case "POINTSTONEXTREWARD":
                            retValue = Utilities.GetPointsToNextReward(_member, startDate, endDate);
                            break;

                        case "CURRENTQUALIFYINGB5G1BRAPURCHASED":
                            retValue = Utilities.GetB5G1CurrentQualifyingPurchased(_member, endDate, "BRA", false, startDate, endDate);
                            break;

                        case "CURRENTFREEB5G1BRAEARNED":
                            braPromoFulFillmentThreshold = Utilities.GetBraThreshold();
                            retValue = Utilities.GetB5G1CurrentFreeEarned(_member, endDate, braPromoFulFillmentThreshold, "BRA", startDate, endDate);
                            break;

                        case "PRIMARYEXPECTEDFULLFILLMENTDATEB5G1BRA":
                            braPromoFulFillmentThreshold = Utilities.GetBraThreshold();
                            retValue = Utilities.GetB5G1ExpectedFullFillmentDate(_member, true, endDate, braPromoFulFillmentThreshold, Boolean.Parse(IsPilotMember()), "BRA");
                            break;

                        case "SECONDARYEXPECTEDFULLFILLMENTDATEB5G1BRA":
                            braPromoFulFillmentThreshold = Utilities.GetBraThreshold();
                            retValue = Utilities.GetB5G1ExpectedFullFillmentDate(_member, false, endDate, braPromoFulFillmentThreshold, Boolean.Parse(IsPilotMember()), "BRA");
                            break;

                        case "TOTALFREEB5G1BRAMAILEDCURRENTYEAR":
                            Utilities.LoadB5G1Rewards(_member, "BRA");
                            retValue = Utilities.GetB5G1TotalFreeMailed(_member, true,"BRA");
                            break;

                        case "TOTALFREEB5G1BRAMAILEDLIFETIME":
                            Utilities.LoadB5G1Rewards(_member, "BRA");
                            retValue = Utilities.GetB5G1TotalFreeMailed(_member, false,"BRA");
                            break;

                        case "TOTALFREEB5G1BRAREDEEMED":
                            retValue = Utilities.GetB5G1TotalFreeRedeemed(_member,"BRA");
                            break;

                        case "LASTFREEB5G1BRAMAILED":
                            retValue = Utilities.GetB5G1LastFreeMailed(_member,"BRA");
                            break;

                        case "LASTFREEB5G1BRAINHOMEDATE":
                            retValue = Utilities.GetB5G1LastFreeInHomeDate(_member,"BRA");
                            break;

                        case "LASTFREEB5G1BRAREDEEMED":
                            retValue = Utilities.GetB5G1LastFreeRedeemed(_member,"BRA");
                            break;

                        case "ACCOUNTTERMINATEDMESSAGE":
                            retValue = GetAccountTerminatedMessage();
                            break;

                        case "ISDOLLARREWARDSMEMBER":
                            retValue = IsDollarRewardsMember();
                            break;

                        case "GETUSERROLE":
                            retValue = GetUserRole();
                            break;

                        case "EMPLOYEESTATUSMESSAGE":
                            retValue = GetEmployeeStatusMessage();
                            break;

                        case "LINKEDACCOUNTMESSAGE":
                            retValue = GetLinkedAccountMessage();
                            break;

                        case "UNMAILABLEADDRESSMESSAGE":
                            retValue = GetUnmailableAddressMessage();
                            break;

                        case "MEMBERBRAND":
                            retValue = GetMemberBrand();
                            break;

                        case "MEMBERSTATE":
                            retValue = GetMemberState();
                            break;

                        case "STATESCRIPT":
                            retValue = stateValidate.BuildLoadStatesJavaScript();
                            break;

                        case "ISADDRESSMAILABLE":
                            retValue = IsMemberAddressMailable();
                            break;

                        case "ISSMSOPTIN":
                            retValue = IsMemberSMSOptIn();
                            break;

                        case "ISEMAILPASSVALIDATION":
                            retValue = isEmailPassValidation();
                            break;

                        case "ISEMAILUNDELIVERABLE":
                            retValue = isEmailUndeliverable();
                            break;

                        case "ISMEMBERDOESNOTHAVELINKEDACCOUNT":
                            retValue = IsMemberDoesNotHaveLinkedAccount();
                            break;

                        case "ISMEMBERTERMINATED":
                            retValue = IsMemberTerminated();
                            break;

                        case "GETCURRENTUSER":
                            retValue = WebUtilities.GetCurrentUserName();
                            break;

                        case "UNDER13MESSAGE":
                            retValue = GetUnder13Message();
                            break;

                        case "CURRENTQUALIFYINGB5G1JEANPURCHASED":
                            retValue = Utilities.GetB5G1CurrentQualifyingPurchased(_member, endDate, "JEAN", false, startDate, endDate);
                            break;

                        case "CURRENTFREEB5G1JEANEARNED":
                            braPromoFulFillmentThreshold = Utilities.GetBraThreshold();
                            retValue = Utilities.GetB5G1CurrentFreeEarned(_member, endDate, braPromoFulFillmentThreshold,"JEAN",startDate, endDate);
                            break;

                        case "PRIMARYEXPECTEDFULLFILLMENTDATEB5G1JEAN":
                            braPromoFulFillmentThreshold = Utilities.GetBraThreshold();
                            retValue = Utilities.GetB5G1ExpectedFullFillmentDate(_member, true, endDate, braPromoFulFillmentThreshold,Boolean.Parse(IsPilotMember()),"JEAN");
                            break;

                        case "SECONDARYEXPECTEDFULLFILLMENTDATEB5G1JEAN":
                            braPromoFulFillmentThreshold = Utilities.GetBraThreshold();
                            retValue = Utilities.GetB5G1ExpectedFullFillmentDate(_member, false, endDate, braPromoFulFillmentThreshold,Boolean.Parse(IsPilotMember()),"JEAN");
                            break;

                        case "TOTALFREEB5G1JEANMAILEDCURRENTYEAR":
                            Utilities.LoadB5G1Rewards(_member, "JEAN");
                            retValue = Utilities.GetB5G1TotalFreeMailed(_member, true,"JEAN");
                            break;

                        case "TOTALFREEB5G1JEANMAILEDLIFETIME":
                            Utilities.LoadB5G1Rewards(_member, "JEAN");
                            retValue = Utilities.GetB5G1TotalFreeMailed(_member, false,"JEAN");
                            break;

                        case "TOTALFREEB5G1JEANREDEEMED":
                            retValue = Utilities.GetB5G1TotalFreeRedeemed(_member,"JEAN");
                            break;

                        case "LASTFREEB5G1JEANMAILED":
                            retValue = Utilities.GetB5G1LastFreeMailed(_member,"JEAN");
                            break;

                        case "LASTFREEB5G1JEANINHOMEDATE":
                            retValue = Utilities.GetB5G1LastFreeInHomeDate(_member,"JEAN");
                            break;

                        case "LASTFREEB5G1JEANREDEEMED":
                            retValue = Utilities.GetB5G1LastFreeRedeemed(_member,"JEAN");
                            break;

                        case "GETVALIDATIONMESSAGE":
                            retValue = GetValidationMessage();
                            break;

                        case "GETMEMBERSTATUS":
                            retValue = GetMemberStatus();
                            break;

                        case "GETCREDITCARDSTATUS":
                            retValue = GetCreditCardStatus();
                            break;

                        case "GETNETSPEND":
                            retValue = GetNetSpend();
                            break;

                        case "ISPILOTMEMBER":
                            retValue = IsPilotMember();
                            break;

                        case "GETTERMINATIONREASON":
                            retValue = Utilities.GetTerminationReason(_memberDetails.TerminationReasonID);
                            break;
							
                        //AEO-2054 BEGIN
                        case "ISUNDER15":
                            retValue = IsUnder15();
                            break;
                        case "GETUNDER15MESSAGE":
                            retValue = GetUnder15Message();
                            break;
                        //AEO-2054 END


                        default:
                            retValue = _providedArg;
                            break;
                    }
                _logger.Trace("Custom bScript", "evaluate", "End");

            }
            catch (Exception ex)
            {
                _logger.Error("Custom bScript", "evaluate", ex.Message);
            }
            return retValue;
        }

        private string IsMemberTerminated()
        {
            String strReturnVal = "false";
            try
            {
                if (_member.MemberStatus == MemberStatusEnum.Terminated)
                {
                    strReturnVal = "true";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "IsMemberTerminated", ex.Message);
            }
            return strReturnVal;
        }

        private string IsMemberDoesNotHaveLinkedAccount()
        {
            _logger.Trace("Custom bScript", "IsMemberDoesNotHaveLinkedAccount", "Begin");
            String strReturnVal = "true";
            try
            {
                if (null != _memberDetails && null != _memberDetails.MemberSource)
                {
                    if (_memberDetails.MemberSource.Value == (Int32)MemberSource.OnlineAEEnrolled || _memberDetails.MemberSource.Value == (Int32)MemberSource.OnlineAERegistered)
                    {
                        strReturnVal = "false";
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Custom bScript", "IsMemberDoesNotHaveLinkedAccount", e.Message);
            }
            _logger.Trace("Custom bScript", "IsMemberDoesNotHaveLinkedAccount", "End");
            return strReturnVal;

        }

        /// <summary>
        /// To check if the member is subscribed to the Dollar Reward program - For calling from within the LN module for Opt-Out.
        /// </summary>
        /// <returns></returns>
        private string IsDollarRewardsMember()
        {
            _logger.Trace("Custom bScript", "IsDollarRewardsMember", "Begin");
            String strReturnVal = "false";
            try
            {
                if (null != _memberDetails  && _memberDetails.ExtendedPlayCode.Value == 1) // AEO-point conversion
                {
                    strReturnVal = "true";
                }
            }
            catch (Exception e)
            {
                _logger.Error("Custom bScript", "IsDollarRewardsMember", e.Message);
            }
            _logger.Trace("Custom bScript", "IsDollarRewardsMember", "End");
            return strReturnVal;
        }

        /// <summary>
        /// Get the role of currently logged in user - For calling from within the LN module for Opt-Out.
        /// </summary>
        /// <returns></returns>
        private string GetUserRole()
        {
            _logger.Trace("Custom bScript", "GetUserRole", "Begin");
            String strReturnVal = "";
            try
            {
                strReturnVal = WebUtilities.GetCurrentUserRole();
            }
            catch (Exception e)
            {
                _logger.Error("Custom bScript", "GetUserRole", e.Message);
            }
            _logger.Trace("Custom bScript", "GetUserRole", "End");
            return strReturnVal;

        }

        private string isEmailUndeliverable()
        {
            String strReturnVal = "false";
            try
            {
                if (null != _memberDetails && _memberDetails.EmailAddressMailable.HasValue)
                {
                    strReturnVal = _memberDetails.EmailAddressMailable.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "isEmailUndeliverable", ex.Message);
            }
            return strReturnVal;
        }

        private string isEmailPassValidation()
        {
            String strReturnVal = "false";
            try
            {
                if (null != _memberDetails && _memberDetails.PassValidation != null)
                {
                    strReturnVal = _memberDetails.PassValidation.Value.ToString().ToLower();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "isEmailPassValidation", ex.Message);
            }
            return strReturnVal;
        }

        private string IsMemberSMSOptIn()
        {
            String strReturnVal = "false";
            try
            {
                if (null != _memberDetails && _memberDetails.SMSOptIn.HasValue)
                {
                    if (_memberDetails.SMSOptIn.Value)
                    {
                        strReturnVal = _memberDetails.SMSOptIn.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "IsMemberSMSOptIn", ex.Message);
            }
            return strReturnVal;
        }

        private string IsMemberAddressMailable()
        {
            String strReturnVal = "false";
            try
            {
                if (null != _memberDetails && _memberDetails.AddressMailable.HasValue)
                {
                    strReturnVal = _memberDetails.AddressMailable.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "IsMemberAddressMailable", ex.Message);
            }
            return strReturnVal;
        }
        /// <summary>
        /// Returns member brand 
        /// </summary>
        /// <returns></returns>
        private string GetMemberBrand()
        {
            String StrRetValue = String.Empty;
            try
            {
                _memberDetails = GetMemberDetails();
                ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();
                LWCriterion lwCriteria = new LWCriterion("RefBrands");
                lwCriteria.Add(LWCriterion.OperatorType.AND, "BRANDID", _memberDetails.BaseBrandID, LWCriterion.Predicate.Eq);

                List<IClientDataObject> lwBrands = _LoyaltyData.GetAttributeSetObjects(null, "RefBrands", lwCriteria, null, false).ToList();
                if (null != lwBrands)
                {
                    RefBrand refBrand = (RefBrand)lwBrands[0];
                    StrRetValue = refBrand.BrandName;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetMemberBrand", ex.Message);
            }
            return StrRetValue;
        }
        #endregion public methods

        #region Private Members
        /// <summary>

        #region Account Details

        /// <summary>
        /// Returns enrollment date of a member
        /// </summary>
        /// <returns></returns>
        private string GetEnrollmentDate()
        {
            String strRetVal = String.Empty;
            try
            {
                if (null != _member)
                {
                    strRetVal = _member.MemberCreateDate.ToString("M/d/yyyy");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetEnrollmentDate", ex.Message);
            }
            return strRetVal;
        }
        /// <summary>
        /// Returns last purchased date 
        /// </summary>
        /// <returns></returns>
        private string GetLastPurchasedDate()
        {
            String strRetVal = String.Empty;
            try
            {
                if (null != _member)
                {
                    strRetVal = _member.LastActivityDate.Value.ToString("M/d/yyyy");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetLastPurchasedDate", ex.Message);
            }
            return strRetVal;
        }

        private String GetMemberState()
        {
            String strRetValue = String.Empty;
            try
            {
                strRetValue = _memberDetails.StateOrProvince;
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetMemberState", ex.Message);
            }
            return strRetValue;
        }
        #endregion Account Details

        #region Global Dashboard

        /// <summary>
        /// Returns employee current status message whether employee is current or previous
        /// </summary>
        /// <returns></returns>
        private string GetEmployeeStatusMessage()
        {
            String StrRetValue = String.Empty;
            try
            {
                ClientConfiguration objClientConfiguration = null;
                IDataService _DataService = _dataUtil.DataServiceInstance();

                /* AEO-1481 begin */
                if (!_memberDetails.EmployeeCode.HasValue)
                {
                    objClientConfiguration = _DataService.GetClientConfiguration("NonEmployeeMessage");
                    StrRetValue = objClientConfiguration.Value;
                }
                else
                {

                    if (_memberDetails.EmployeeCode.Value == (Int32)Employee.CurrentEmployee)
                    {
                        objClientConfiguration = _DataService.GetClientConfiguration("CurrentEmployeeMessage");
                        StrRetValue = objClientConfiguration.Value;
                    }
                    else if (_memberDetails.EmployeeCode.Value == (Int32)Employee.PreviousEmployee)
                    {
                        objClientConfiguration = _DataService.GetClientConfiguration("PriorEmployeeMessage");
                        StrRetValue = objClientConfiguration.Value;
                    }

                    else
                    {
                        objClientConfiguration = _DataService.GetClientConfiguration("NonEmployeeMessage");
                        StrRetValue = objClientConfiguration.Value;
                    }
                }
                /* AEO-1481 end */
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bScript", "GetEmployeeStatusMessage", ex.Message);
            }
            return StrRetValue;
        }

        /// <summary>
        /// Returns member terminated message
        /// </summary>
        /// <returns></returns>
        private string GetAccountTerminatedMessage()
        {
            String StrRetValue = String.Empty;
            try
            {
                IDataService _DataService = _dataUtil.DataServiceInstance();

                if (_member.MemberStatus == MemberStatusEnum.Terminated)
                {
                    ClientConfiguration objClientConfiguration = _DataService.GetClientConfiguration("AccountTerminatedMessage");
                    StrRetValue = objClientConfiguration.Value;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bScript", "GetAccountTerminatedMessage", ex.Message);

            }
            return StrRetValue;
        }

        /// <summary>
        /// Returns linked account message
        /// </summary>
        /// <returns></returns>
        private string GetLinkedAccountMessage()
        {
            String StrRetValue = String.Empty;
            try
            {
                IDataService _DataService = _dataUtil.DataServiceInstance();
                if (_memberDetails.MemberSource.Value == (Int32)MemberSource.OnlineAEEnrolled || _memberDetails.MemberSource.Value == (Int32)MemberSource.OnlineAERegistered)
                {
                    ClientConfiguration objClientConfiguration = _DataService.GetClientConfiguration("LinkedAccountMessage");
                    StrRetValue = objClientConfiguration.Value;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bScript", "GetLinkedAccountMessage", ex.Message);
            }
            return StrRetValue;
        }

        /// <summary>
        /// Returns unmailable address message
        /// </summary>
        /// <returns></returns>
        private string GetUnmailableAddressMessage()
        {
            String StrRetValue = String.Empty;
            try
            {
                IDataService _DataService = _dataUtil.DataServiceInstance();
                if (!_memberDetails.AddressMailable.Value)
                {
                    ClientConfiguration objClientConfiguration = _DataService.GetClientConfiguration("UnmailableAddressMessage");
                    StrRetValue = objClientConfiguration.Value;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bScript", "GetUnmailableAddressMessage", ex.Message);
            }
            return StrRetValue;
        }

        /// <summary>
        /// Returns under 13 message
        /// </summary>
        /// <returns></returns>
        private string GetUnder13Message()
        {
            String StrRetValue = String.Empty;
            try
            {
                _logger.Trace("Custom bScript", "GetUnder13Message", "BEGIN");
                if (_memberDetails == null)
                {
                    _logger.Error("Custom bScript", "GetUnder13Message", "MemberDetails is empty");
                }
                else if (_memberDetails.IsUnderAge.HasValue && _memberDetails.IsUnderAge.Value)
                {
                    IDataService _DataService = _dataUtil.DataServiceInstance();
                    ClientConfiguration objClientConfiguration = _DataService.GetClientConfiguration("Under13Message");
                    StrRetValue = objClientConfiguration.Value;
                }
                _logger.Trace("Custom bScript", "GetUnder13Message", "End");
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bScript", "GetUnder13Message", ex.Message, ex);

            }

            return StrRetValue;
        }

        #endregion Global Dashboard

        #region Common Methods

        /// <summary>
        /// Returns the name of the selected member
        /// </summary>
        /// <returns></returns>
        private String GetMemberName()
        {
            String strRetValue = string.Empty;

            try
            {
                if (null != _member)
                {
                    strRetValue = _member.FirstName + " " + _member.LastName;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetMemberName", ex.Message);
            }
            return strRetValue;
        }

        /// <summary>
        /// Returns Loyalty ID numnber of a selected member
        /// </summary>
        /// <returns></returns>
        private String GetLoyaltyIDNumber()
        {
            String strRetValue = string.Empty;

            try
            {
                if (_member != null)
                {
                    foreach (VirtualCard vc in _member.LoyaltyCards)
                    {
                        if (vc.IsPrimary)
                        {
                            strRetValue = vc.LoyaltyIdNumber;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetLoyaltyIDNumber", ex.Message);
            }

            return strRetValue;
        }

        /// <summary>
        /// Grab selected member account date(start and end date of selected quarter)
        /// </summary>
        /// <param name="enmQuarterDate">Enum QuarterDate</param>
        /// <returns>Start date/End date</returns>
        private DateTime GetQuarterStartEndDate(QuarterDate enmQuarterDate)
        {
            DateTime retDate = DateTime.MinValue;
            _logger.Trace("Custom bScript", "GetQuarterStartEndDate", "Begin");

            try
            {
                String DateRange = PortalState.GetFromCache("MemberAccountDate") as String;
                _logger.Trace("Custom bScript", "GetQuarterStartEndDate", "DateRange: " + DateRange);
                if (DateRange.StartsWith("Init"))
                {
                    DateRange = DateRange.Substring(5);
                }
                switch (enmQuarterDate)
                {
                    case QuarterDate.StartDate:
                        String strStartDate = DateRange.Substring(0, DateRange.IndexOf("to"));
                        DateTime dtStartDate = DateTime.Now;
                        DateTime.TryParse(strStartDate, out dtStartDate);
                        retDate = dtStartDate;
                        break;
                    case QuarterDate.EndDate:
                        String strEndDate = DateRange.Substring(DateRange.IndexOf("to") + 3);
                        DateTime dtEndDate = DateTime.Now;
                        DateTime.TryParse(strEndDate, out dtEndDate);
                        retDate = dtEndDate;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetQuarterStartEndDate", ex.Message);
            }

            _logger.Trace("Custom bScript", "GetQuarterStartEndDate", "END");
            return retDate;
        }
        /// <summary>
        /// Grab selected member account date(start and end date of selected month or quarter)
        /// Added for PI 30364 - Quick Wins - Phase I loyalty enhancements - Dollar Rewards - Job Code
        /// </summary>
        /// <param name="enmProgramDate"></param>
        /// <returns></returns>
        private DateTime GetProgramStartEndDate(ProgramDate enmProgramDate)
        {
            DateTime retDate = DateTime.MinValue;
            _logger.Trace("Custom bScript", "GetProgramStartEndDate", "Begin");

            try
            {
                String DateRange = PortalState.GetFromCache("MemberAccountDate") as String;
                _logger.Trace("Custom bScript", "GetProgramStartEndDate", "DateRange: " + DateRange);
                if (DateRange.StartsWith("Init"))
                {
                    DateRange = DateRange.Substring(5);
                }
                switch (enmProgramDate)
                {
                    case ProgramDate.StartDate:
                        String strStartDate = DateRange.Substring(0, DateRange.IndexOf("to"));
                        DateTime dtStartDate = DateTime.Now;
                        DateTime.TryParse(strStartDate, out dtStartDate);
                        retDate = dtStartDate;
                        break;
                    case ProgramDate.EndDate:
                        String strEndDate = DateRange.Substring(DateRange.IndexOf("to") + 3);
                        DateTime dtEndDate = DateTime.Now;
                        DateTime.TryParse(strEndDate, out dtEndDate);
                        retDate = dtEndDate;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetProgramStartEndDate", ex.Message);
            }

            _logger.Trace("Custom bScript", "GetProgramStartEndDate", "END");
            return retDate;
        }
        /// <summary>
        /// Returns member details of a member from cache
        /// </summary>
        /// <returns>MemberDetails</returns>
        private MemberDetails GetMemberDetails()
        {
            try
            {
                _logger.Trace("Custom bScript", "GetMemberDetails", "Begin");
                IList<IClientDataObject> lstMemberAttributes = _member.GetChildAttributeSets("MemberDetails");
                if (null != lstMemberAttributes && lstMemberAttributes.Count > 0)
                {
                    MemberDetails memberDetails = (MemberDetails)lstMemberAttributes[0];
                    _logger.Trace("Custom bScript", "GetMemberDetails", "Return MemberDetails.");
                    return lstMemberAttributes[0] as MemberDetails;
                }
                _logger.Trace("Custom bScript", "GetMemberDetails", "End");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        //PI30364 -Dollarreward Changes begin here                         ------------ SCJ 
        /// <summary>
        //this method takes in a pointevent and returns the points associated to it
        /// </summary>
        /// <param name="pPointEvent">pPointEvent</param>
        /// <returns>points</returns>
        private static string GETPOINTSFOREVENT(Member pMember, string pPointEvent)
        {

            PointType pointType = null;
            PointEvent pointEvent = null;
            //Double dblBonusPoints = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal dblBonusPoints = 0;
            DateTime endDate = DateTime.Parse("07/31/2114");
            DateTime startdate = DateTime.Parse("07/01/2014");
            int index = 0;
            long[] vcKeys = new long[pMember.LoyaltyCards.Count];
            long[] pointTypeIDs = new long[1];
            long[] pointEventIDs = new long[1];

            string _pointType = "Bonus Points";
            //string _pointEvent = "Dollar Reward Tier Bonus";
            string _pointEvent = pPointEvent;

            try
            {
                if (Utilities.GetPointTypeAndEvent(_pointType, _pointEvent, out pointEvent, out pointType))
                {
                    foreach (VirtualCard card in pMember.LoyaltyCards)
                    {
                        vcKeys[index] = card.VcKey;
                        ++index;
                    }

                    pointTypeIDs[0] = pointType.ID;
                    pointEventIDs[0] = pointEvent.ID;
                    //dblBonusPoints = pMember.GetPoints( pointType, pointEvent, startdate, endDate);   // AEO-74 Upgrade 4.5 changes here -----------SCJ
                    dblBonusPoints = pMember.GetPoints(pointTypeIDs, pointEventIDs, startdate, endDate);
                }
            }
            catch (Exception ex)
            {
                _logger.Trace("Custom bScript", "GETPOINTSFOREVENT", ex.Message);

            }
            return Convert.ToString(dblBonusPoints);


        }
		
		 private string GetUnder15Message()
        {
            String StrRetValue = String.Empty;
            try
            {
                IDataService _DataService = _dataUtil.DataServiceInstance();
                _logger.Trace("Custom bScript", "GetUnder15Message", "BEGIN");
                ClientConfiguration objClientConfiguration = _DataService.GetClientConfiguration("LegendUnder15");
                StrRetValue = objClientConfiguration.Value;
                _logger.Trace("Custom bScript", "GetUnder15Message", "End");
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bScript", "GetUnder15Message", ex.Message);

            }
            return StrRetValue;
        }
        private string IsUnder15()
        {
            try
            {
                _logger.Trace("Custom bScript", "IsUnder15", "BEGIN");
                _logger.Trace("MemberDetails", "Isunder15", "MemberdetailsHasValue" + _memberDetails.IsUnderAge.HasValue);
                _logger.Trace("MemberDetails", "Isunder15", "MemberdetailsIsUnderAge.Value" + _memberDetails.IsUnderAge.Value);


                if (_memberDetails == null)
                {
                    _logger.Error("Custom bScript", "GetUnder15Message", "MemberDetails is empty");
                }
                else if (_memberDetails.IsUnderAge.HasValue && _memberDetails.IsUnderAge.Value)
                {
                    return "true";
                }
        
                _logger.Trace("Custom bScript", "IsUnder15", "END");

            }
            catch(Exception ex)
            {
                _logger.Error("Custom bScript", "IsUnder15", "Error" + ex.Message);
            }

            return "false";
        }
		
        //PI30364 -Dollarreward Changes end here - SCJ 

        // added for redesign project
        /// <summary>
        /// Returns the validation of the cell and email
        /// </summary>
        /// <returns></returns>
        private String GetValidationMessage()
        {
            String strRetValue = string.Empty;
            String strCellValue = string.Empty;
            String strEmailValue = string.Empty;

            try
            {
                if (null != _memberDetails)
                {
                    // AEO-550 Begin                  
                     strCellValue = _memberDetails.PendingCellVerification == null ||  _memberDetails.PendingCellVerification  == 1? "No" : "Yes";
                     strEmailValue = _memberDetails.PendingEmailVerification == null || _memberDetails.PendingEmailVerification == 1 ? "No" : "Yes";
                    

                    strRetValue = (  "Email: " + strEmailValue  ) +  ( ", Cell: " + strCellValue  );
                    // AEO-55 End
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetValidationMessage", ex.Message);
            }
            return strRetValue;
        }

        /// <summary>
        /// Returns the status of the member
        /// </summary>
        /// <returns></returns>
        private String GetMemberStatus()
        {
            String strRetValue = string.Empty;

            try
            {
               
                if (null != _member)
                {
                    // AEO-1576 begin
                    //Pilot Member
                    if (IsPilotMember().ToLower() == "true") //AEO-1058
                    {//Is memberstatus==3/(Terminated)?                       
                        if (_member.MemberStatus == MemberStatusEnum.Terminated)
                        {
                            strRetValue = this.GetAccountTerminatedMessage();
                        } //Is memberstatus == 4-repurposed as Frozen?
                        else if (_member.MemberStatus == MemberStatusEnum.Locked)
                        {
                            strRetValue = "Frozen";
                        }
                        else
                        {
                            if (null != _memberDetails)
                            {
                                strRetValue = Enum.GetName(typeof(MemberStatusEnum), _member.MemberStatus);
                                //For Active Members-Check the MemberStatusCode for member inactivation?
                                if (_member.MemberStatus == MemberStatusEnum.Active && _memberDetails.MemberStatusCode == 1)
                                    strRetValue = "Inactive";
                                if (_member.MemberStatus == MemberStatusEnum.Active && _memberDetails.MemberStatusCode != 1)
                                    strRetValue = "Active";
                            }
                        }
                        // AEO-1576 end  
                    }
                    else //AEO-1058//This part of code would not be reached under current business logic
                    {
                        //Legacy Member
                        if (_member.MemberStatus == MemberStatusEnum.Terminated) //AEO-1111
                        {
                            strRetValue = "Account Terminated";
                        }
                        else
                        {
                            strRetValue = Enum.GetName(typeof(MemberStatusEnum), _member.MemberStatus);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetMemberStatus", ex.Message);
            }
            return strRetValue;
        }

        /// <summary>
        /// Returns the credit card status of the member
        /// </summary>
        /// <returns></returns>
        private String GetCreditCardStatus()
        {
            String strRetValue = string.Empty;

            try
            {
                if (null != _memberDetails)
                {
                    if (_memberDetails.CardType.Value == Convert.ToInt64(CardType.NoCardType))
                        strRetValue = "";
                    else if (_memberDetails.CardType.Value == Convert.ToInt64(CardType.AECCMember))
                        strRetValue = "AECC Member";
                    else if (_memberDetails.CardType.Value == Convert.ToInt64(CardType.AEVisaMember))
                        strRetValue = "AEVisa Member";
                    else if (_memberDetails.CardType.Value == Convert.ToInt64(CardType.AECCAndAEVisaMember))
                        strRetValue = "AECC and AEVisa Member";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetCreditCardStatus", ex.Message);
            }
            return strRetValue;
        }

        /// <summary>
        /// Returns the name of the selected member
        /// </summary>
        /// <returns></returns>
        private String GetNetSpend()
        {
            String strRetValue = string.Empty;

            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    int year = DateTime.Now.Year;
                    DateTime startDate = new DateTime(year, 1, 1);
                    DateTime endDate = new DateTime(year, 12, 31).Add(new TimeSpan(23, 59, 59));
                    PointType netSpendPointType = lwService.GetPointType("NetSpend");
                    decimal retValue = _member.GetPoints(netSpendPointType, startDate, endDate);
                    strRetValue = retValue.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bscript", "GetNetSpend", ex.Message);
            }
            return strRetValue;
        }

        /// <summary>
        /// Returns the status of pilot member
        /// </summary>
        /// <returns></returns>
        /// 

        // AEO-401 Begin
        private string IsPilotMember ( ) {
            string strRetval = "true";

            return strRetval;
        }

        #endregion Common Methods

        #region Enum
        /// <summary>
        /// Enumeration to indicate start and end date of quarter 
        /// </summary>
        private enum QuarterDate
        {
            StartDate,
            EndDate
        }
        /// <summary>
        /// Enumeration to indicate start and end date of month or quarter
        /// Added for PI 30364 - Quick Wins - Phase I loyalty enhancements - Dollar Rewards 
        /// </summary>
        private enum ProgramDate
        {
            StartDate,
            EndDate
        }
        #endregion Enum
        #endregion

    }
}
