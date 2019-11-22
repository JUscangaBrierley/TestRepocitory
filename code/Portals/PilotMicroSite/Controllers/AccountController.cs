using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Security;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

using Brierley.LoyaltyWare.ClientLib;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Client;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Framework;

using PilotMicroSite.Models;

namespace PilotMicroSite.Controllers
{
    public class AccountController : Controller
    {
        public Utilities utils = new Utilities();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum ResponseCode
        {
            Normal = 0,
            LoyaltyAccountAlreadyLinked = 10,
            InvalidLoyaltyNumber = 20,
            LoyaltyNumberRequired = 30,
            LoyaltyAccountNotFound = 40,
            LoyaltyAccountTerminated = 50,
            LoyaltyAccountArchived = 60,
            FirstNameRequired = 70,
            InvalidFirstName = 80,
            InvalidLastName = 90,
            LastNameRequired = 100,
            AddressLine1Required = 110,
            InvalidAddressLine1 = 120,
            InvalidAddressLine2 = 130,
            CityRequired = 140,
            InvalidCity = 150,
            StateRequired = 160,
            InvalidState = 170,
            PostalCodeRequired = 180,
            InvalidPostalCode = 190,
            CountryCodeRequired = 200,
            InvalidCountryCode = 210,
            BirthDateRequired = 220,
            InvalidBirthDate = 230,
            EmailAddressRequired = 240,
            InvalidEmailAddress = 250,
            InvalidHomePhone = 260,
            InvalidMobilePhone = 270,
            MobilePhoneNumberRequiredForSMSOptIn = 280,
            InvalidSMSOptIn = 290,
            InvalidGender = 300,
            InvalidLanguagePreference = 310,
            NoMatchOnMemberProfileData = 320,
            NoTransactionDataAvailable = 330,
            StartDateRequired = 340,
            InvalidStartDate = 350,
            PageNumberRequired = 360,
            InvalidPageNumber = 370,
            StoreNumberRequired = 380,
            InvalidStoreNumber = 390,
            RegisterNumberRequired = 400,
            InvalidRegisterNumber = 410,
            TotalPaymentRequired = 420,
            InvalidTotalPayment = 430,
            TransactionNumberRequired = 440,
            InvalidTransactionNumber = 450,
            TransactionDateRequired = 460,
            InvalidTransactionDate = 470,
            OrderNumberRequired = 480,
            InvalidOrderNumber = 490,
            OrderAmountRequired = 500,
            InvalidOrderAmount = 510,
            TechnicalDifficulties = 520,
            PurchaseNotValidForCurrentEarningPeriod = 530,
            LoyaltyNumberMatchWithAddressAvailable = 540,
            LoyaltyNumberMatchWithNoAddressAvailable = 550,
            RequestPointsTransactionAlreadyReceived = 560,
            BaseBrandRequired = 570,
            InvalidBaseBrand = 580,
            EndDateRequired = 590,
            InvalidEndDate = 600,
            RequestPointsTransactionAlreadyRequested = 610,
            RequestPointsTransactionAlreadyPosted = 620,
            EngagementTypeRequired = 630
        }
        /// <summary>
        /// This is login action method.  This was a test login page that we created prior to SSO getting integrated to be 
        /// able to test the functionality of the pages
        /// </summary>
        /// <param name="loyaltynumber"></param>
        /// <returns></returns>
        public ActionResult Login(string loyaltynumber)
        {

            var earnSummary = new EarningSummary();
            string loyaltyNumber = string.Empty;
            string memberName = string.Empty;
            string viewName = string.Empty;
            double jeansToNextReward = 0;
            double brasToNextReward = 0;
            string accountStatus = string.Empty;

            try
            {

                FormsAuthentication.SignOut();
                if (loyaltynumber == "Clear")
                {
                    DeleteSession("State");
                }
                else
                {
                    utils.SaveToSession("State", "state", "XYZ");
                }
                DeleteSession("Logout_URL");
                string validState = GetCookie("State", "state");
                string logoutURL = GetCookie("Logout_URL", "logoutURL");
                logger.Debug(string.Format("Stage: {0}, LogoutURL: {1}", validState, logoutURL));

                if (loyaltynumber != null && loyaltynumber.Length > 0)
                {
                    loyaltyNumber = loyaltynumber;
                }

                if (loyaltyNumber.Length > 0)
                {
                    AEGetAccountSummaryOut acctSummary = utils.GetMember(loyaltyNumber);
                    if (acctSummary == null)
                    {
                        if (utils.errorCode == 40)
                        {
                            ViewBag.ErrorMessage = "You need to create an account on the ae.com site";
                        }
                        if (utils.errorCode == 3323)
                        {
                            ViewBag.ErrorMessage = "Member not found for LoyaltyNumber: " + loyaltyNumber;
                        }
                    }
                    else
                    {
                        memberName = utils.member.FirstName;
                        earnSummary.Tier = acctSummary.MemberTier;
                        IList<Brierley.LoyaltyWare.ClientLib.DomainModel.LWAttributeSetContainer> memberDetails = utils.member.GetAttributeSets("MemberDetails");
                        MemberDetails memberDetail = (MemberDetails)memberDetails[0];

                        switch (utils.member.MemberStatus)
                        {
                            case Member.MemberStatusEnum.Active:
                                if ((memberDetail.PendingCellVerification == 1) && (memberDetail.PendingEmailVerification == 1))
                                {
                                    accountStatus = "Pending";
                                }
                                else
                                {
                                    accountStatus = "Active";
                                }
                                break;
                            case Member.MemberStatusEnum.Locked:
                                accountStatus = "Frozen";
                                break;
                            case Member.MemberStatusEnum.Terminated:
                                accountStatus = "Terminated";
                                break;
                            default:
                                accountStatus = "Unknown";
                                break;
                        }
                        earnSummary.AccountStatus = accountStatus;
                        earnSummary.LoyaltyNumber = loyaltyNumber;
                        earnSummary.BasePoints = acctSummary.BasicPoints;
                        earnSummary.BonusPoints = acctSummary.BonusPoints;
                        earnSummary.BrasPurchased = acctSummary.CurrentBrasPurchased;
                        earnSummary.JeansPurchase = acctSummary.CurrentJeansPurchased;
                        earnSummary.PointsToNextReward = acctSummary.PointsNeeded;
                        if (acctSummary.RewardLevel == "1")
                        {
                            earnSummary.Reward = "You have a $5 reward waiting!";
                        }
                        else
                        {
                            earnSummary.Reward = string.Format("You have {0} $5 rewards waiting! ", acctSummary.RewardLevel);
                        }
                        earnSummary.TotalPoints = acctSummary.TotalPoints;

                        double jeansPurchased = 0;
                        if (acctSummary.CurrentJeansPurchased != null)
                        {
                            jeansPurchased = double.Parse(acctSummary.CurrentJeansPurchased);
                        }
                        if (jeansPurchased > 5)
                        {
                            jeansToNextReward = (Math.Floor(jeansPurchased / 5) * 5) + 5 - jeansPurchased;
                        }
                        else
                        {
                            jeansToNextReward = 5 - jeansPurchased;
                        }
                        earnSummary.JeansToNextReward = jeansToNextReward.ToString();

                        double brPurchased = double.Parse(acctSummary.CurrentBrasPurchased);
                        if (brPurchased > 5)
                        {
                            brasToNextReward = (Math.Floor(brPurchased / 5) * 5) + 5 - brPurchased;
                        }
                        else
                        {
                            brasToNextReward = 5 - brPurchased;
                        }
                        earnSummary.BrasToNextReward = brasToNextReward.ToString();
                        ViewBag.MemberName = utils.member.FirstName + "'s";

                    }

                    CreateNewCookie(loyaltyNumber);
                    viewName = "EarningSummary";

                }
            }

            catch (LWClientException ex)
            {
                logger.Error(string.Format("LoyaltyNumber: {0} - Error: {1}", loyaltyNumber, ex.Message));
            }

            catch (Exception ex1)
            {
                logger.Error(string.Format("LoyaltyNumber: {0} - Error: {1}", loyaltyNumber, ex1.Message));
            }

            if (viewName.Length > 0)
            {
                return View(viewName, earnSummary);
            }
            else
            {
                return View();
            }

        }
        /// <summary>
        /// This is the method used to persist the Loyalty Number that we receive back from the client in the PostToAE method.  
        /// All authenticated pages call this method and if loyalty number is empty then they call the Authenticate method to 
        /// redirect back to AE for authenticate.
        /// </summary>
        /// <param name="loyaltyNumber"></param>
        private void CreateNewCookie(string loyaltyNumber)
        {
            HttpCookie myCookie = new HttpCookie("LoyaltyMember");

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, "LoyaltyMember", DateTime.Now, DateTime.Now.AddMinutes(30), true, loyaltyNumber, FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

        }
        /// <summary>
        /// This is the Action method for the EarningSummary view.
        /// There is some code in here to display an error message based on what is returned from the SSO method PostToAE and also
        /// the account status.
        /// </summary>
        /// <param name="SubmitButton"></param>
        /// <param name="loyaltynumber"></param>
        /// <returns></returns>
        public ActionResult EarningSummary(string SubmitButton, string loyaltynumber)
        {
            var earnSummary = new EarningSummary();
            string loyaltyNumber = string.Empty;
            string memberName = string.Empty;
            string viewName = string.Empty;
            string accountStatus = string.Empty;

            try
            {
                logger.Debug("Begin");

                loyaltyNumber = CheckForCookie();

                if (loyaltyNumber.Length == 0)
                {
                    logger.Debug("Authenticate");
                    Authenticate();
                }


                if (loyaltyNumber.Length > 0)
                {
                    logger.Debug("LoyaltyNumber: " + loyaltyNumber);
                    if (loyaltyNumber == "GenericError")
                    {
                        earnSummary = DefaultEarningSummary();
                        CreateNewCookie(string.Empty);
                    }
                    else
                    {
                        logger.Debug("Starting with LoyaltyNumber: " + loyaltyNumber);
                        earnSummary = GetEarningSummary(loyaltyNumber);
                        if (earnSummary == null)
                        {
                            earnSummary = DefaultEarningSummary();
                            CreateNewCookie(string.Empty);
                        }
                        else
                        {
                            CreateNewCookie(loyaltyNumber);
                        }
                    }
                }

            }
            catch (LWClientException ex)
            {
                logger.Error(ex.Message);
                earnSummary = DefaultEarningSummary();
                CreateNewCookie(string.Empty);
            }
            if (viewName.Length > 0)
            {
                return View(viewName);
            }
            else
            {
                return View(earnSummary);
            }
            
        }
        /// <summary>
        /// This is a method display an error message and default the data to dashes.
        /// </summary>
        /// <returns></returns>
        private EarningSummary DefaultEarningSummary()
        {

            EarningSummary earnSummary = new Models.EarningSummary();

                earnSummary.Tier = "--";
                earnSummary.AccountStatus = "--";
                earnSummary.LoyaltyNumber = "--";
                earnSummary.BasePoints = "--";
                earnSummary.BonusPoints = "--";
                earnSummary.BrasPurchased = "--";
                earnSummary.JeansPurchase = "--";
                earnSummary.PointsToNextReward = "--";
                earnSummary.Reward = "--";
                earnSummary.TotalPoints = "--";
                earnSummary.JeansToNextReward = "--";
                earnSummary.BrasToNextReward = "--";
                ViewBag.TopMessage = "HOLD UP, THERE'S A PROBLEM";
                ViewBag.MessageBody = "Sorry, something has happened and we could not load your information. Please sign in again or if you continue to experience this problem contact us at 1-800-340-0532.";
            return earnSummary;
        }
        /// <summary>
        /// This method gets the member data and populates the EarningSummary model object
        /// </summary>
        /// <param name="loyaltyNumber"></param>
        /// <returns></returns>
        private EarningSummary GetEarningSummary(string loyaltyNumber)
        {
            double jeansToNextReward = 0;
            double brasToNextReward = 0;
            string memberName = string.Empty;
            string viewName = string.Empty;
            string accountStatus = string.Empty;
            char quote = '"';

            EarningSummary earnSummary = new Models.EarningSummary();

            AEGetAccountSummaryOut acctSummary = utils.GetMember(loyaltyNumber);
            if (acctSummary == null)
            {
                if (utils.errorCode > 0)
                {

                    if (utils.errorCode == 40)
                    {
                        ViewBag.ErrorMessage = "You need to create an account on the ae.com site";
                    }
                    else if (utils.errorCode == 3323)
                    {
                        ViewBag.ErrorMessage = "Member not found for LoyaltyNumber: " + loyaltyNumber;
                    }
                }
            }
            else
            {
                memberName = utils.member.FirstName;
                earnSummary.Tier = acctSummary.MemberTier;
                IList<Brierley.LoyaltyWare.ClientLib.DomainModel.LWAttributeSetContainer> memberDetails = utils.member.GetAttributeSets("MemberDetails");
                MemberDetails memberDetail = (MemberDetails)memberDetails[0];

                switch (utils.member.MemberStatus)
                {
                    case Member.MemberStatusEnum.Active:
                        if ((memberDetail.PendingCellVerification == 1) && (memberDetail.PendingEmailVerification == 1))
                        {
                            accountStatus = "Pending";
                            ViewBag.TopMessage = "ACCOUNT PENDING";
                            ViewBag.MessageBody = "To activate your account and start earning Rewards, find the " + quote + "Activate your Account" + quote + " email we sent to " + memberDetail.EmailAddress + " or contact us at 1-800-340-0532.";
                        }
                        else
                        {
                            accountStatus = "Active";
                        }
                        break;
                    case Member.MemberStatusEnum.Locked:
                        accountStatus = "Frozen";
                        break;
                    case Member.MemberStatusEnum.Terminated:
                        accountStatus = "Terminated";
                        break;
                    default:
                        accountStatus = "Unknown";
                        break;
                }
                earnSummary.AccountStatus = accountStatus;
                earnSummary.LoyaltyNumber = loyaltyNumber;
                earnSummary.BasePoints = acctSummary.BasicPoints;
                earnSummary.BonusPoints = acctSummary.BonusPoints;
                earnSummary.BrasPurchased = acctSummary.CurrentBrasPurchased;
                earnSummary.JeansPurchase = acctSummary.CurrentJeansPurchased;
                earnSummary.PointsToNextReward = acctSummary.PointsNeeded;
                if (acctSummary.RewardLevel == "1")
                {
                    earnSummary.Reward = "You have a $5 reward waiting!";
                }
                else
                {
                    earnSummary.Reward = string.Format("You have {0} $5 rewards waiting! ", acctSummary.RewardLevel);
                }
                earnSummary.TotalPoints = acctSummary.TotalPoints;

                double jeansPurchased = 0;
                if (acctSummary.CurrentJeansPurchased != null)
                {
                    jeansPurchased = double.Parse(acctSummary.CurrentJeansPurchased);
                }
                if (jeansPurchased > 5)
                {
                    jeansToNextReward = (Math.Floor(jeansPurchased / 5) * 5) + 5 - jeansPurchased;
                }
                else
                {
                    jeansToNextReward = 5 - jeansPurchased;
                }
                earnSummary.JeansToNextReward = jeansToNextReward.ToString();

                double brPurchased = double.Parse(acctSummary.CurrentBrasPurchased);
                if (brPurchased > 5)
                {
                    brasToNextReward = (Math.Floor(brPurchased / 5) * 5) + 5 - brPurchased;
                }
                else
                {
                    brasToNextReward = 5 - brPurchased;
                }
                earnSummary.BrasToNextReward = brasToNextReward.ToString();
                ViewBag.MemberName = utils.member.FirstName + "'s";
            }
            return earnSummary;
        }
        /// <summary>
        /// This is the Action method for the EarningActivity view.
        /// </summary>
        /// <returns></returns>
        public ActionResult EarningActivity()
        {
            List<EarningActivity> earningActivities = new List<Models.EarningActivity>();

            string externalID = new Guid().ToString();
            double elapsedTime = 0;
            string loyaltyNumber = string.Empty;
            string viewName = "EarningActivity";

            try
            {
                LWIntegrationSvcClientManager svcMgr = utils.GetServiceManager();

                loyaltyNumber = CheckForCookie();

                if (loyaltyNumber.Length == 0)
                {
                    logger.Debug("Authenticate");
                    Authenticate();
                }

                if (utils.member != null)
                {
                    ViewBag.MemberName = utils.member.FirstName + "'s";
                }

                if (loyaltyNumber.Length > 0)
                {
                    try
                    {
                        AEGetAccountActivityOut acctActivity = svcMgr.AEGetAccountActivity(loyaltyNumber, DateTime.Parse("7/1/2015"), DateTime.Parse("12/31/2199"), 1, externalID, out elapsedTime);
                        foreach (TransactionHeadersStruct item in acctActivity.TransactionHeaders)
                        {
                            var earnActivity = new EarningActivity();
                            earnActivity.TxnDate = item.PurchaseDate;
                            earnActivity.Store = item.StoreName;
                            earnActivity.StoreNumber = item.StoreNumber;
                            earnActivity.Activity = item.Description;
                            earnActivity.Order = item.TransactionNumber;
                            earnActivity.Points = item.Points;
                            earningActivities.Add(earnActivity);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.StartsWith("No transaction data available"))
                        {
                            viewName = "NoActivity";
                        }
                    }
                }
                ViewData["Activities"] = earningActivities;

            }
            catch (LWClientException ex)
            {
                logger.Error(string.Format("LoyaltyNumber: {0} - Error: {1}", loyaltyNumber, ex.Message));
                viewName = "Error";
            }

            return View(viewName);
        }
        /// <summary>
        /// this is the action method for the AvailableRewards view
        /// </summary>
        /// <returns></returns>
        public ActionResult AvailableRewards()
        {
            List<AvailableRewards> availableRewards = new List<Models.AvailableRewards>();

            string externalID = new Guid().ToString();
            double elapsedTime = 0;
            string viewName = "AvailableRewards";
            string loyaltyNumber = string.Empty;

            try
            {
                LWIntegrationSvcClientManager svcMgr = utils.GetServiceManager();

                loyaltyNumber = CheckForCookie();

                if (loyaltyNumber.Length == 0)
                {
                    logger.Debug("Authenticate");
                    Authenticate();
                }

                if (utils.member != null)
                {
                    ViewBag.MemberName = utils.member.FirstName + "'s";
                }

                if (loyaltyNumber.Length > 0)
                {
                    try
                    {
                        MemberRewardSummaryStruct[] rewardSumm = svcMgr.GetMemberRewardsSummary(loyaltyNumber, null, null, null, null, null, null, null, null, externalID, out elapsedTime);

                        for (var i = 0; i < rewardSumm.Length; i++)
                        {
                            var memberReward = new AvailableRewards();

                            memberReward.CouponCode = rewardSumm[i].CertificateNumber;
                            memberReward.Description = rewardSumm[i].RewardName;
                            DateTime expirationDate = (DateTime)rewardSumm[i].ExpirationDate;
                            memberReward.ExpirationDate = expirationDate.ToShortDateString();

                            availableRewards.Add(memberReward);
                        }
                    }
                    catch (LWClientException ex)
                    {
                        int code = ex.ErrorCode;
                        if (ex.Message.StartsWith("No member rewards found"))
                        {
                            viewName = "NoRewards";
                        }
                    }
                }
                ViewData["Rewards"] = availableRewards;
            }
            catch (LWClientException ex)
            {
                int code = ex.ErrorCode;
                logger.Error(string.Format("LoyaltyNumber: {0} - Error: {1}", loyaltyNumber, ex.Message));
                viewName = "Error";
            }

            return View(viewName);
        }
        /// <summary>
        /// This is the read-only action method of the Profile page.  The only way I could detect the clicking of the link was the length 
        /// paramenter.  when that is 4 then I redirect to the ProfileEdit page which uses the Post version of this method.
        /// </summary>
        /// <param name="Length"></param>
        /// <param name="profileModel"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Profile(string Length, Profile profileModel, string Address)
        {
            var profile = new Profile();
            string loyaltyNumber = string.Empty;
            string gender = string.Empty;
            string viewName = string.Empty;

            try
            {

                if (Length == "4") //Edit
                {
                    viewName = "ProfileEdit";
                }
                loyaltyNumber = CheckForCookie();

                if (loyaltyNumber.Length == 0)
                {
                    logger.Debug("Authenticate");
                    Authenticate();
                }

                if (utils.member != null)
                {
                    ViewBag.MemberName = utils.member.FirstName + "'s";
                }

                profile = PopulateMember(loyaltyNumber);

                if (Length == "6") //Cancel
                {
                    return View(profile);
                }

            }
            catch (LWClientException ex)
            {
                logger.Error(string.Format("LoyaltyNumber: {0} - Error: {1}", loyaltyNumber, ex.Message));
                viewName = "Error";
            }

            if (viewName.Length > 0)
            {
                return View(viewName);
            }
            else
            {
                return View(profile);
            }

        }
        /// <summary>
        /// This is the Post version of the ProfileEdit page.  Once the member gets into the edit version of the page this method is called
        /// to update the member.
        /// </summary>
        /// <param name="updateProfile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Profile(Profile updateProfile)
        {

            string loyaltyNumber = string.Empty;
            string gender = string.Empty;
            string viewName = string.Empty;
            Profile profile = null;

            try
            {

                loyaltyNumber = CheckForCookie();

                if (loyaltyNumber.Length == 0)
                {
                    logger.Debug("Authenticate");
                    Authenticate();
                }

                ViewBag.MemberName = utils.member.FirstName + "'s";

                string errorMessage = UpdateMember(updateProfile, loyaltyNumber);
                if (errorMessage.Length > 0)
                {
                    ViewBag.ErrorMessage = errorMessage;
                }
                profile = PopulateMember(loyaltyNumber);
            }
            catch (LWClientException ex)
            {
                logger.Error(string.Format("LoyaltyNumber: {0} - Error: {1}", loyaltyNumber, ex.Message));
                viewName = "Error";
            }

            return View(profile);

        }
        /// <summary>
        /// This is the action method of the logout page which is used to logout the member here and then call AE to log the member out
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        public ActionResult logout(string sign)
        {
            string loyaltyNumber = string.Empty;

            try
            {
                logger.Debug("Begin: ");

                Logout(loyaltyNumber);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return View("Error");
            }
            return View();
        }
        public ActionResult Error()
        {
            logger.Debug("Begin: ");

            return View();
        }
        /// <summary>
        /// This is the page that AE posts back to after a member is authenticated on their side.  It will call the PostToAE method
        /// which will call their restful api to return the LoyaltyNumber in a Json object.  If any errors are returned from this call
        /// we will display the EarningSummary page with the generic error message and the fields loaded with dashes.  If we get a good
        /// call then we get the loyalty number, save it to a cookie and then all the pages will work.
        /// </summary>
        /// <param name="code">
        /// This parmeter is a unique code that is generated on the AE side that I have to pass back in their api
        /// call.
        /// </param>
        /// <param name="state">
        /// This is a state variable that I generate on my side if someone hits the microsite directly.  I create a guid and then save
        /// that in a cookie.  Then when I call their login to authenticate, I pass the state.  Then they have to pass it back to me and 
        /// if it matches the cookie then we are good, if not then we've been hacked and I throw up an error message
        /// </param>
        /// <param name="error">
        /// This is an error parameter that they may pass back from authentication that I have to log and show an error.
        /// </param>
        /// <param name="termurl">
        /// This is a url that tells me what environment they are returning to me and it tells me how to build the url for the api
        /// call and the logout call.  I take the termurl and look up in appSettings for the correct urls.  The termURL is the api call
        /// so I just have to look up the logout url.
        /// </param>
        /// <returns></returns>
        public ActionResult cb(string code, string state, string error, string termurl)
        {
            string validState = string.Empty;
            var earnSummary = new EarningSummary();
            string viewName = string.Empty;
            string loyaltyNumber = string.Empty;

            logger.Debug("Begin: " + code);

            if (error != null)
            {
                logger.Debug("Error from AE Authentication: " + error);
                CreateNewCookie("GenericError");
                return RedirectToAction("EarningSummary");
            }

            try
            {
                logger.Debug("Check for LoyaltyMember Session/Cookie");
                loyaltyNumber = CheckForCookie();

                if (loyaltyNumber.Length > 0)
                {
                    logger.Debug("LoyaltyMember Session/Cookie found load summery");
                    earnSummary = GetEarningSummary(loyaltyNumber);

                    if (earnSummary == null)
                    {
                        CreateNewCookie("GenericError");
                        return RedirectToAction("EarningSummary");
                    }
                    else
                    {
                        return RedirectToAction("EarningSummary");

                    }
                }
                else
                {
                    logger.Debug("LoyaltyMember Session/Cookie not found so, PostToAE to get it.");
                    string errorMessage = PostToAE(code, termurl, state, out loyaltyNumber);
                    if (errorMessage.Length > 0)
                    {
                        if (errorMessage == "GenericError")
                        {
                            CreateNewCookie(errorMessage);
                            return RedirectToAction("EarningSummary");
                        }
                        else if (errorMessage == "StateBlank")
                        {
                            return View();
                        }
                    }
                    else
                    {
                        CreateNewCookie(loyaltyNumber);
                        earnSummary = GetEarningSummary(loyaltyNumber);

                        if (earnSummary == null)
                        {
                            logger.Debug("Error from PostToAE, so redirect to Error");
                            CreateNewCookie("GenericError");
                            return RedirectToAction("EarningSummary");
                        }
                        else
                        {
                            return RedirectToAction("EarningSummary");
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                logger.Error("Error from PostToAE, so redirect to Error: " + ex.Message);
                CreateNewCookie("GenericError");
                return RedirectToAction("EarningSummary");
            }
        }
        /// <summary>
        /// This is the method that calls AE's resful api to validate that the code I received from the cb post is valid.  If valid
        /// it returns the loyalty number.  In this method, we check the state variable.  If it is blank then that means the member
        /// initiated on the ae.com side.  If not then they started here and if the two states don't match we throw an error.
        /// We also use the termURL to get the logout URL for Sign Out
        /// </summary>
        /// <param name="code">
        /// This parmeter is a unique code that is generated on the AE side that I have to pass back in their api
        /// call.
        /// </param>
        /// <param name="state">
        /// This is a state variable that I generate on my side if someone hits the microsite directly.  I create a guid and then save
        /// that in a cookie.  Then when I call their login to authenticate, I pass the state.  Then they have to pass it back to me and 
        /// if it matches the cookie then we are good, if not then we've been hacked and I throw up an error message
        /// </param>
        /// <param name="termurl">
        /// This is a url that tells me what environment they are returning to me and it tells me how to build the url for the api
        /// call and the logout call.  I take the termurl and look up in appSettings for the correct urls.  The termURL is the api call
        /// so I just have to look up the logout url.
        /// </param>
        /// <param name="loyaltyNumber">
        /// The Loyalty Number is retrieved from the api call and then returned back to the cb action method to be persisted for other pages to use
        /// </param>
        /// <returns></returns>
        private string PostToAE(string code, string termURL, string state, out string loyaltyNumber)
        {
            string data = string.Empty;
            loyaltyNumber = string.Empty;
            string url = string.Empty;
            string clientId = string.Empty;
            string validState = string.Empty;
            string logoutURL = string.Empty;
            string errorMessage = string.Empty;

            logger.Debug("Begin: Code = " + code);

            try
            {
                validState = GetCookie("State", "state");

                if (validState.Length == 0)
                {
                    logger.Debug(string.Format("State is blank, so Continue"));
                }
                else
                {
                    if (validState != state)
                    {
                        logger.Debug(string.Format("State is invalid.  valid state: {0} - State Sent back: {1}", validState, state));
                        return "GenericError";
                    }
                }

                url = "https://" + termURL + "/integrations/Brierley/v1/profile";
                if (null == ConfigurationManager.AppSettings[termURL])
                {
                    throw new Exception(termURL + " not defined in web.config");
                }
                else
                {
                    logoutURL = ConfigurationManager.AppSettings[termURL];
                }

                utils.SaveToSession("Logout_URL", "logoutURL", logoutURL);

                data = "code=" + HttpUtility.UrlEncode(code) + "&" + "grant_type=authorization_code";

                ViewBag.LogoutURL = logoutURL;

                logger.Debug("logoutURL: " + logoutURL);
                logger.Debug("data: " + data);
                logger.Debug("url: " + url);

                logger.Debug("Create Request");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                // set post headers
                request.Method = "POST";
                request.KeepAlive = true;
                request.ContentLength = data.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers["Authorization"] = "Basic MjgzMTM3Mzk3ODQ4ODU2NTI1OTpzU2M1QmFNTndLMUNsdGxH";

                logger.Debug("write the data to the requeststream");
                // write the data to the request stream         
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write(data);
                writer.Close();

                logger.Debug("Post it");
                // iirc this actually triggers the post
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                logger.Debug("Get the response");

                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                logger.Debug("Parse the json");
                string json = streamReader.ReadToEnd();

                loyaltyNumber = JsonConvert.DeserializeObject<ReturnLoyaltyNumber>(json).LoyaltyNumber;

                streamReader.Close();
                logger.Debug("LoyaltyNumber: " + loyaltyNumber);

                return string.Empty;

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                ViewBag.Error = ex.Message;
               return "GenericError";
            }
        }
        /// <summary>
        /// This method is used to call AE to authenticate the member if someone comes to the site and tries to 
        /// hit a page and is not authenticated on this side
        /// </summary>
        public void Authenticate()
        {
            string clientId = string.Empty;
            string url = string.Empty;
            Guid state = Guid.NewGuid();

            url = utils.BuildAuthenticateURL();

            logger.Debug("Authenticate: Redirect to " + url);

            Response.Redirect(url);

        }

        /// <summary>
        /// This is used to delete the session.  This is mostly used for testing purposes.
        /// </summary>
        /// <param name="sessionName"></param>
        private void DeleteSession(string sessionName)
        {
            HttpCookie currentUserCookie = Request.Cookies[sessionName];
            if (currentUserCookie != null)
            {
                Response.Cookies.Remove(sessionName);
                currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                currentUserCookie.Value = null;
                Response.SetCookie(currentUserCookie);
            }
        }
        /// <summary>
        /// This method looks for the loyalty number in the Forms Authentation cookie.  this is used by all secured pages.  If the loyalty number
        /// doesn't exist in the cookie then the user is redirected back to AE for authentication.
        /// </summary>
        /// <returns></returns>
        private string CheckForCookie()
        {
            string loyaltyNumber = string.Empty;

            // Retrieves the cookie that contains your custom FormsAuthenticationTicket.
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null)
            {
                loyaltyNumber = string.Empty;
            }
            else
            {
                if (!string.IsNullOrEmpty(authCookie.Value))
                {
                    // Decrypts the FormsAuthenticationTicket that is held in the cookie's .Value property.
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    loyaltyNumber = authTicket.UserData;
                    AEGetAccountSummaryOut acctSummary = utils.GetMember(loyaltyNumber);
                }
            }

            return loyaltyNumber;
        }
        /// <summary>
        /// This method calls the GetMember method in the utilities class that calls the GetMembers api to get the profile information and populates the profile model
        /// </summary>
        /// <param name="loyaltyNumber"></param>
        /// <returns></returns>
        public Profile PopulateMember(string loyaltyNumber)
        {
            string gender = string.Empty;
            long mobilePhone = 0;
            var profile = new Profile();

            try
            {
                AEGetAccountSummaryOut acctSummary = utils.GetMember(loyaltyNumber);


                if (utils.member != null)
                {
                    IList<Brierley.LoyaltyWare.ClientLib.DomainModel.LWAttributeSetContainer> memberDetails = utils.member.GetAttributeSets("MemberDetails");
                    MemberDetails memberDetail = (MemberDetails)memberDetails[0];

                    profile.Address1 = memberDetail.AddressLineOne;
                    profile.Address2 = memberDetail.AddressLineTwo;
                    DateTime birthDate = (DateTime)utils.member.BirthDate;
                    profile.Birthday = birthDate.ToShortDateString();
                    profile.CityStateZip = string.Format("{0}, {1} {2}", memberDetail.City, memberDetail.StateOrProvince, memberDetail.ZipOrPostalCode);
                    profile.Country = memberDetail.Country;
                    profile.SelectedCountry = memberDetail.Country;
                    LoadCountries(memberDetail.Country, ref profile);
                    profile.SelectedState = memberDetail.StateOrProvince;
                    LoadStates(memberDetail.StateOrProvince, ref profile);

                    profile.EmailAddress = memberDetail.EmailAddress;


                    switch (memberDetail.Gender)
                    {
                        case "1":
                            gender = "Male";
                            ViewBag.MaleChecked = "checked";
                            break;
                        case "2":
                            gender = "Female";
                            ViewBag.FemaleChecked = "checked";
                            break;
                        default:
                            gender = string.Empty;
                            break;
                    }

                    ViewBag.Gender = gender;
                    if (memberDetail.MobilePhone != null && memberDetail.MobilePhone.Length > 0)
                    {
                        mobilePhone = long.Parse(memberDetail.MobilePhone);
                        profile.MobilePhone = String.Format("{0:(###) ###-####}", mobilePhone);
                    }

                    profile.FirstName = utils.member.FirstName;
                    profile.LastName = utils.member.LastName;
                    ViewBag.FirstName = utils.member.FirstName;
                    ViewBag.LastName = utils.member.LastName;
                    DateTime birthday = (DateTime)utils.member.BirthDate;
                    ViewBag.Birthday = birthday.ToShortDateString();
                    ViewBag.Address = memberDetail.AddressLineOne;
                    ViewBag.Address2 = memberDetail.AddressLineTwo;
                    ViewBag.SelectedState = profile.SelectedState;
                    ViewBag.States = profile.States;
                    ViewBag.SelectedCountry = profile.SelectedCountry;
                    ViewBag.Countries = profile.Countries;
                    ViewBag.City = memberDetail.City;
                    ViewBag.Email = memberDetail.EmailAddress;
                    ViewBag.MobilePhone = memberDetail.MobilePhone;
                    ViewBag.MemberName = utils.member.FirstName + "'s";

                    
                }

            }
            catch (LWClientException ex)
            {
                logger.Error(string.Format("LoyaltyNumber: {0} - Error: {1}", loyaltyNumber, ex.Message));
                return null;
            }
            return profile;

        }
        /// <summary>
        /// This method is excuted when the member clicks the sign out button and redirects to the AE logout page.  
        /// The LogoutURL cookie is written in the PostToAE method after AE redirects back to the cb page passing in the
        /// termURL which tells us which environment we are in.   The logout url is built based on that termURL
        /// </summary>
        /// <param name="loyaltyNumber"></param>
        private void Logout(string loyaltyNumber)
        {
            string url = string.Empty;

            try
            {
                logger.Debug("Get the logoutURL cookie");
                string logoutURL = GetCookie("Logout_URL", "logoutURL");

                logger.Debug("Logout: Logout_URL: " + logoutURL);

                FormsAuthentication.SignOut();
                if (logoutURL.Length == 0)
                {
                    logger.Error("Logout URL is blank, so redirect to ae.com logout");
                    logoutURL = "https://www.ae.com/web/logout.jsp";
                }
                logger.Debug("Logout: Redirect to " + logoutURL);

                Response.Redirect(logoutURL);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw ex;
            }


        }
        /// <summary>
        /// This is a method used to save the logout URL and state parameters in session or cookie to be used on later pages
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValueName"></param>
        /// <returns></returns>
        private string GetCookie(string cookieName, string cookieValueName)
        {
            string cookieValue = string.Empty;

            logger.Debug("Get " + cookieName + " cookie");

            //Read the cookie from Request.
            HttpCookie myCookie = Request.Cookies[cookieName];

            if (myCookie == null)
            {
                logger.Debug(cookieName + " cookie is null get from session");
                //No cookie found or cookie expired.
                //Redirect back to login
                if (Session[cookieValueName] != null)
                {
                    cookieValue = Session[cookieValueName].ToString();
                }
                logger.Debug(cookieValueName + ": " + cookieValue);
            }
            else
            {
                logger.Debug(cookieValueName + " cookie is found");
                //ok - cookie is found.
                //Gracefully check if the cookie has the key-value as expected.
                if (!string.IsNullOrEmpty(myCookie.Values[cookieValueName]))
                {

                    cookieValue = myCookie.Values[cookieValueName].ToString();

                }
            }
            return cookieValue;
        }
        /// <summary>
        /// This is a method used by the Profile page to call the UpdateMember api to update the member profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="loyaltyNumber"></param>
        /// <returns></returns>
        private string UpdateMember(Profile profile, string loyaltyNumber)
        {
            string errorMessage = string.Empty;

            try
            {
                string externalID = new Guid().ToString();
                double elapsedTime = 0;

                LWIntegrationSvcClientManager svcMgr = utils.GetServiceManager();

                AEGetAccountSummaryOut summery = utils.GetMember(loyaltyNumber);

                if (utils.member != null)
                {
                    //Member member = members[0];
                    IList<Brierley.LoyaltyWare.ClientLib.DomainModel.LWAttributeSetContainer> memberDetails = utils.member.GetAttributeSets("MemberDetails");
                    MemberDetails mbrDetails = (MemberDetails)memberDetails[0];
                    if (mbrDetails.SMSOptIn == null)
                    {
                        mbrDetails.SMSOptIn = false;
                    }
                    mbrDetails.AddressLineOne = profile.Address1;
                    mbrDetails.AddressLineTwo = profile.Address2;
                    mbrDetails.EmailAddress = profile.EmailAddress;
                    mbrDetails.MobilePhone = profile.MobilePhone;
                    mbrDetails.City = profile.City;
                    mbrDetails.StateOrProvince = profile.SelectedState;
                    mbrDetails.Country = profile.SelectedCountry;
                    mbrDetails.Gender = profile.Gender;

                    Member newMember = svcMgr.UpdateMember(utils.member, externalID, out elapsedTime);
                }
                return errorMessage;

            }
            catch (LWClientException ex)
            {
                int errorCode = ex.ErrorCode;
                logger.Error(string.Format("LoyaltyNumber: {0} - Error: {1}", loyaltyNumber, ex.Message));
                errorMessage = GetResponseMessage((ResponseCode)ex.ErrorCode);
                return errorMessage;
            }

        }
        /// <summary>
        /// This is the method that loads the country dropdown
        /// </summary>
        /// <param name="country"></param>
        /// <param name="profile"></param>
        private void LoadCountries(string country, ref Profile profile)
        {
            profile.Countries = new Dictionary<string, string>();

            profile.Countries.Add("USA", "United States");
            profile.Countries.Add("CAN", "Canada");
        }
        /// <summary>
        /// this is the method that loads the provinces in the state dropdown
        /// </summary>
        /// <param name="state"></param>
        /// <param name="profile"></param>
        private void LoadProvinces(string state, ref Profile profile)
        {
            profile.States = new Dictionary<string, string>();

            profile.States.Add("AB", "Alberta");
            profile.States.Add("BC", "British Columbia");
            profile.States.Add("MB", "Manitoba");
            profile.States.Add("NB", "New Brunswick");
            profile.States.Add("NL", "Newfoundland/Labrador");
            profile.States.Add("NT", "Northwest Territories");
            profile.States.Add("NS", "Nova Scotia");
            profile.States.Add("NU", "Nunavut");
            profile.States.Add("ON", "Ontario");
            profile.States.Add("PE", "Prince Edward Island");
            profile.States.Add("QC", "Quebec");
            profile.States.Add("SK", "Saskatchewan");
            profile.States.Add("YT", "Yukon");
        }
        /// <summary>
        /// this is the method that loads the states in the state dropdown
        /// </summary>
        /// <param name="state"></param>
        /// <param name="profile"></param>
        private void LoadStates(string state, ref Profile profile)
        {
            profile.States = new Dictionary<string, string>();

            profile.States.Add("AL", "Alabama");
            profile.States.Add("AK", "Alaska");
            profile.States.Add("AS", "American Samoa");
            profile.States.Add("AZ", "Arizona");
            profile.States.Add("AR", "Arkansas");
            profile.States.Add("AA", "Armed Forces (the) Americas");
            profile.States.Add("AE", "Armed Forces Europe");
            profile.States.Add("AP", "Armed Forces Pacific");
            profile.States.Add("CA", "California");
            profile.States.Add("CO", "Colorado");
            profile.States.Add("CT", "Connecticut");
            profile.States.Add("DE", "Delaware");
            profile.States.Add("DC", "District of Columbia");
            profile.States.Add("FM", "Federated States of Micronesia");
            profile.States.Add("FL", "Florida");
            profile.States.Add("GA", "Georgia");
            profile.States.Add("GU", "Guam");
            profile.States.Add("HI", "Hawaii");
            profile.States.Add("ID", "Idaho");
            profile.States.Add("IL", "Illinois");
            profile.States.Add("IN", "Indiana");
            profile.States.Add("IA", "Iowa");
            profile.States.Add("KS", "Kansas");
            profile.States.Add("KY", "Kentucky");
            profile.States.Add("LA", "Louisiana");
            profile.States.Add("ME", "Maine");
            profile.States.Add("MH", "Marshall Islands");
            profile.States.Add("MD", "Maryland");
            profile.States.Add("MA", "Massachusetts");
            profile.States.Add("MI", "Michigan");
            profile.States.Add("MN", "Minnesota");
            profile.States.Add("MS", "Mississippi");
            profile.States.Add("MO", "Missouri");
            profile.States.Add("MT", "Montana");
            profile.States.Add("NE", "Nebraska");
            profile.States.Add("NV", "Nevada");
            profile.States.Add("NH", "New Hampshire");
            profile.States.Add("NJ", "New Jersey");
            profile.States.Add("NM", "New Mexico");
            profile.States.Add("NY", "New York");
            profile.States.Add("NC", "North Carolina");
            profile.States.Add("ND", "North Dakota");
            profile.States.Add("MP", "Northern Mariana Islands");
            profile.States.Add("OH", "Ohio");
            profile.States.Add("OK", "Oklahoma");
            profile.States.Add("OR", "Oregon");
            profile.States.Add("PW", "Palau");
            profile.States.Add("PA", "Pennsylvania");
            profile.States.Add("PR", "Puerto Rico");
            profile.States.Add("RI", "Rhode Island");
            profile.States.Add("SC", "South Carolina");
            profile.States.Add("SD", "South Dakota");
            profile.States.Add("TN", "Tennessee");
            profile.States.Add("TX", "Texas");
            profile.States.Add("UT", "Utah");
            profile.States.Add("VT", "Vermont");
            profile.States.Add("VI", "Virgin Islands");
            profile.States.Add("VA", "Virginia");
            profile.States.Add("WA", "Washington");
            profile.States.Add("WV", "West Virginia");
            profile.States.Add("WI", "Wisconsin");
            profile.States.Add("WY", "Wyoming");
        }
        /// <summary>
        /// this is the method that will convert the responsecode from the api to a string message.
        /// </summary>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        public string GetResponseMessage(ResponseCode responseCode)
        {
            String returnMessage = string.Empty;

            switch (responseCode)
            {
                case ResponseCode.Normal:
                    returnMessage = "Normal Response";//Response Code : 0
                    break;
                case ResponseCode.LoyaltyAccountAlreadyLinked:
                    returnMessage = "Loyalty account already linked to ae.com account";//Response Code : 10
                    break;
                case ResponseCode.InvalidLoyaltyNumber:
                    returnMessage = "Invalid loyalty number";//Response Code : 20
                    break;
                case ResponseCode.LoyaltyNumberRequired:
                    returnMessage = "Loyalty number required";//Response Code : 30
                    break;
                case ResponseCode.LoyaltyAccountNotFound:
                    returnMessage = "Loyalty Account Not Found";//Response Code : 40
                    break;
                case ResponseCode.LoyaltyAccountTerminated:
                    returnMessage = "Loyalty account terminated";//Response Code : 50
                    break;
                case ResponseCode.LoyaltyAccountArchived:
                    returnMessage = "Loyalty account archived";//Response Code : 60
                    break;
                case ResponseCode.FirstNameRequired:
                    returnMessage = "First name required";//Response Code : 70
                    break;
                case ResponseCode.InvalidFirstName:
                    returnMessage = "Invalid first name";//Response Code : 80
                    break;
                case ResponseCode.InvalidLastName:
                    returnMessage = "Invalid last name";//Response Code : 90
                    break;
                case ResponseCode.LastNameRequired:
                    returnMessage = "Last name required";//Response Code : 100
                    break;
                case ResponseCode.AddressLine1Required:
                    returnMessage = "Address line 1 required";//Response Code : 110
                    break;
                case ResponseCode.InvalidAddressLine1:
                    returnMessage = "Invalid address line 1";//Response Code : 120
                    break;
                case ResponseCode.InvalidAddressLine2:
                    returnMessage = "Invalid address line 2";//Response Code : 130
                    break;
                case ResponseCode.CityRequired:
                    returnMessage = "City required";//Response Code : 140
                    break;
                case ResponseCode.InvalidCity:
                    returnMessage = "Invalid city";//Response Code : 150
                    break;
                case ResponseCode.StateRequired:
                    returnMessage = "State required";//Response Code : 160
                    break;
                case ResponseCode.InvalidState:
                    returnMessage = "Invalid state";//Response Code : 170
                    break;
                case ResponseCode.PostalCodeRequired:
                    returnMessage = "Postal code required";//Response Code : 180
                    break;
                case ResponseCode.InvalidPostalCode:
                    returnMessage = "Invalid postal code";//Response Code : 190
                    break;
                case ResponseCode.CountryCodeRequired:
                    returnMessage = "Country code required";//Response Code : 200
                    break;
                case ResponseCode.InvalidCountryCode:
                    returnMessage = "Invalid country code";//Response Code : 210
                    break;
                case ResponseCode.BirthDateRequired:
                    returnMessage = "Birth date required";//Response Code : 220
                    break;
                case ResponseCode.InvalidBirthDate:
                    returnMessage = "Invalid birth date";//Response Code : 230
                    break;
                case ResponseCode.EmailAddressRequired:
                    returnMessage = "Email address required";//Response Code : 240
                    break;
                case ResponseCode.InvalidEmailAddress:
                    returnMessage = "Invalid email address";//Response Code : 250
                    break;
                case ResponseCode.InvalidHomePhone:
                    returnMessage = "Invalid home phone";//Response Code : 260
                    break;
                case ResponseCode.InvalidMobilePhone:
                    returnMessage = "Invalid mobile phone";//Response Code : 270
                    break;
                case ResponseCode.MobilePhoneNumberRequiredForSMSOptIn:
                    returnMessage = "Mobile phone number required for sms opt in";//Response Code : 280
                    break;
                case ResponseCode.InvalidSMSOptIn:
                    returnMessage = "Invalid sms opt in";//Response Code : 290
                    break;
                case ResponseCode.InvalidGender:
                    returnMessage = "Invalid gender";//Response Code : 300
                    break;
                case ResponseCode.InvalidLanguagePreference:
                    returnMessage = "Invalid language preference";//Response Code : 310
                    break;
                case ResponseCode.NoMatchOnMemberProfileData:
                    returnMessage = "No match on member profile data"; // Respons Code: 320
                    break;
                case ResponseCode.NoTransactionDataAvailable:
                    returnMessage = "No transaction data available"; // Respons Code: 330
                    break;
                case ResponseCode.StartDateRequired:
                    returnMessage = "Start date required"; // Respons Code: 340
                    break;
                case ResponseCode.InvalidStartDate:
                    returnMessage = "Invalid start date"; // Respons Code: 350
                    break;
                case ResponseCode.PageNumberRequired:
                    returnMessage = "Page number required"; // Respons Code: 360
                    break;
                case ResponseCode.InvalidPageNumber:
                    returnMessage = "Invalid page number"; // Respons Code: 370
                    break;
                case ResponseCode.StoreNumberRequired:
                    returnMessage = "Store number required"; // Respons Code: 380
                    break;
                case ResponseCode.InvalidStoreNumber:
                    returnMessage = "Invalid store number"; // Respons Code: 390
                    break;
                case ResponseCode.RegisterNumberRequired:
                    returnMessage = "Register number required"; // Respons Code: 400
                    break;
                case ResponseCode.InvalidRegisterNumber:
                    returnMessage = "Invalid register number"; // Respons Code: 410
                    break;
                case ResponseCode.TotalPaymentRequired:
                    returnMessage = "Total payment required"; // Respons Code: 420
                    break;
                case ResponseCode.InvalidTotalPayment:
                    returnMessage = "Invalid total payment"; // Respons Code: 430
                    break;
                case ResponseCode.TransactionNumberRequired:
                    returnMessage = "Transaction number required"; // Respons Code: 440
                    break;
                case ResponseCode.InvalidTransactionNumber:
                    returnMessage = "Invalid transaction number"; // Respons Code: 450
                    break;
                case ResponseCode.TransactionDateRequired:
                    returnMessage = "Transaction date required"; // Respons Code: 460
                    break;
                case ResponseCode.InvalidTransactionDate:
                    returnMessage = "Invalid transaction date"; // Respons Code: 470
                    break;
                case ResponseCode.OrderNumberRequired:
                    returnMessage = "Order number is required";// Respons Code: 480
                    break;
                case ResponseCode.InvalidOrderNumber:
                    returnMessage = "Invalid order number";// Respons Code: 490
                    break;
                case ResponseCode.OrderAmountRequired:
                    returnMessage = "Order amount is required";// Respons Code: 500
                    break;
                case ResponseCode.InvalidOrderAmount:
                    returnMessage = "Invalid order amount";// Respons Code: 510
                    break;
                case ResponseCode.TechnicalDifficulties:
                    returnMessage = "Technical Difficulties";// Respons Code: 520
                    break;
                case ResponseCode.PurchaseNotValidForCurrentEarningPeriod:
                    returnMessage = "Purchase not valid for current earning period"; // Respons Code: 530
                    break;
                case ResponseCode.LoyaltyNumberMatchWithAddressAvailable:
                    returnMessage = "Loyalty number match with address available";// Response Code: 540
                    break;
                case ResponseCode.LoyaltyNumberMatchWithNoAddressAvailable:
                    returnMessage = "Loyalty number match with no address available";// Response Code: 550
                    break;
                case ResponseCode.RequestPointsTransactionAlreadyReceived:
                    returnMessage = "Request points transaction already received"; // Respons Code: 560
                    break;
                case ResponseCode.BaseBrandRequired:
                    returnMessage = "Base brand required";// Response Code: 570
                    break;
                case ResponseCode.InvalidBaseBrand:
                    returnMessage = "Invalid base brand";// Response Code: 580
                    break;
                case ResponseCode.EndDateRequired:
                    returnMessage = "End date required";// Response Code: 590
                    break;
                case ResponseCode.InvalidEndDate:
                    returnMessage = "Invalid end date";// Response Code: 600
                    break;
                case ResponseCode.RequestPointsTransactionAlreadyRequested:
                    returnMessage = "Points already requested";// Response Code: 610
                    break;
                case ResponseCode.RequestPointsTransactionAlreadyPosted:
                    returnMessage = "Request points already been posted";// Response Code: 620
                    break;
                default:
                    returnMessage = "Message Not Defined";
                    break;
            }
            return returnMessage;
        }

    }
}
