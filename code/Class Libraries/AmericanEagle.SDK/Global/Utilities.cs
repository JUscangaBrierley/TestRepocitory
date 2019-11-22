using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.GridProvider;
using Brierley.FrameWork.Common;
using System.Net.Mail;
using System.Xml.Linq;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Global
{
    public static class Utilities
    {
        private static LWLogger logger = LWLoggerManager.GetLogger("Utilities");
        private static IList<MemberBraPromoSummary> lstMemberBraPromoSummary;
        private static IList<MemberBraPromoCertSummary> lstMemberBraPromoCertSummary;
        private static IList<MemberBraPromoCertHistory> lstMemberBraPromoCertHistory;
        private static IList<MemberBraPromoCertRedeem> lstMemberBraPromoCertRedeem;
        private static IList<MemberReward> mbrRewards;
        private static RewardDef reward_1;
        private static RewardDef reward_15;
        private static RewardDef reward_bra;  //AEO-Redesign-2015 Start & end (JRA-240)
        private static RewardDef reward_Jean;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        // AEO-1334 begin
        //This version of GetPointsOnHold should not be called by any Bra or Jean type related method since it will return PointsOnHold for all other Point Types
        public static decimal GetPointsOnHold(Member member, DateTime startDate, DateTime endDate)
        {

            decimal RetValue = 0;

            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())

                if (member != null)
                {


                    long[] loyaltyids = member.GetLoyaltyCardIds();


                    if (loyaltyids == null || loyaltyids.Length == 0)
                    {
                        return decimal.Zero;
                    }

                    IList<PointEvent> events = lwService.GetAllPointEvents();

                    IList<VirtualCard> cards = new List<VirtualCard>();
                    for (int i = 0; i < loyaltyids.Length; i++)
                    {
                        cards.Add(member.GetLoyaltyCard(loyaltyids[i]));
                    }

                    IList<PointType> allpointTypes = lwService.GetAllPointTypes();
                    IList<PointType> pointtypes = new List<PointType>();

                    foreach (PointType pt in allpointTypes)
                    {
                        if (!((pt.Name.ToUpper().Contains("BRA")) || (pt.Name.ToUpper().Contains("JEAN"))))
                        {
                            pointtypes.Add(pt);
                        }
                    }
                    RetValue = lwService.GetPointsOnHold(cards, pointtypes, events, startDate, endDate);
                }
            return RetValue;
        }
        // AEO/1334 end

        // Point Conversion - Begin MMV

        public static Boolean isInPilot(long? psExtendedPlayCode)
        {


            Boolean lbRetVal = true;

            /* AEO-1197 Begin
            if ( psExtendedPlayCode == null ) {
                return lbRetVal;
            }

            ClientConfiguration objClientConfiguration = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("PilotStatusCodes");

            if ( objClientConfiguration != null && objClientConfiguration.Value != null ) { 
                List<string> laListValues = new List<string>( objClientConfiguration.Value.Split( new char[] {','},
                                                                StringSplitOptions.RemoveEmptyEntries));                

                lbRetVal= laListValues.IndexOf( psExtendedPlayCode.ToString()) >=0;
            }
              AEO-1197 end */

            return lbRetVal;
        }

        // Point Conversion - End MMV



        // PI 30364 - Dollar reward program - Start
        public static DateTime DollarRewardsProgramStartDate
        {
            get
            {
                using (var dService = _dataUtil.DataServiceInstance())
                {
                    DateTime returnDate = DateTime.Now;
                    ClientConfiguration objClientConfiguration = dService.GetClientConfiguration("DollarRewardStartDate");
                    if (objClientConfiguration != null)
                    {
                        System.DateTime.TryParse(objClientConfiguration.Value, out returnDate);
                    }
                    return returnDate;
                }
            }
        }
        public static DateTime DollarRewardsProgramEndDate
        {
            get
            {
                using (var dService = _dataUtil.DataServiceInstance())
                {
                    DateTime returnDate = DateTime.Now;
                    ClientConfiguration objClientConfiguration = dService.GetClientConfiguration("DollarRewardEndDate");
                    if (objClientConfiguration != null)
                    {
                        System.DateTime.TryParse(objClientConfiguration.Value, out returnDate);
                    }
                    return returnDate;
                }
            }
        }
        // PI 30364 - Dollar reward program - End

        //public static IList<IClientDataObject> GetGlobalAttributeSet(ILWDataService dataService, string attributeSetName)
        //{
        //    string region = "AmericanEagleAttributeSets";
        //    IList<IClientDataObject> attributeSet = (IList<IClientDataObject>)dataService.CacheManager.Get(region, attributeSetName);
        //    if (attributeSet == null)
        //    {
        //        attributeSet = (IList<IClientDataObject>)dataService.GetAttributeSetObjects(null, attributeSetName, null, new LWQueryBatchInfo(), false);
        //        dataService.CacheManager.Update(region, attributeSetName, attributeSet);
        //    }
        //    return attributeSet;
        //}

        //LW 4.1.14 rem
        public static AsyncJob StartJob(IDataService dataService, string FileName, string JobName)
        {
            AsyncJob job = new AsyncJob();

            job.FileName = FileName;
            job.JobName = JobName;
            job.JobNumber = dataService.GetNextID("LIBJob");
            job.JobDirection = Brierley.FrameWork.Common.LIBJobDirectionEnum.OutBound;
            job.StartTime = DateTime.Now;
            dataService.CreateAsyncJob(job);

            return job;

        }
        //LW 4.1.14 removed
        public static void StopJob(IDataService dataService, long jobID, long numberOfRecordsProcessed)
        {
            AsyncJob job = dataService.GetAsyncJobById(jobID);
            job.JobStatus = Brierley.FrameWork.Common.LIBJobStatusEnum.Finished;
            job.MessagesReceived = numberOfRecordsProcessed;
            job.EndTime = DateTime.Now;
            dataService.UpdateAsyncJob(job);
        }
        /// <summary>
        /// Return a boolean that shows expired points should be included or not
        /// An extra parameter is added for PI 30364 - Dollar reward program
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IncludeExpired(DateTime startDate, Member member)
        {
            DateTime currentStartDate = DateTime.MinValue;
            DateTime currentEndDate = DateTime.MinValue;
            bool returnValue = false;
            // PI 30364 - Dollar reward program - Start
            Utilities.GetProgramDates(member, out currentStartDate, out currentEndDate);
            // PI 30364 - Dollar reward program - End
            if (currentStartDate == startDate)
            {
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
            return returnValue;
        }
        public static void GetQuarterDates(out DateTime startDate, out DateTime endDate)
        {
            int startMonth = 0;
            int startYear = DateTime.Now.Year;
            int startDay = 1;
            int endMonth = 0;
            int endYear = DateTime.Now.Year;
            int endDay = 31;

            string quarter = GetQuarter(DateTime.Now.Month);
            switch (quarter)
            {
                case "Q1":
                    startMonth = 1;
                    endMonth = 3;
                    endDay = 31;
                    break;

                case "Q2":
                    startMonth = 4;
                    endMonth = 6;
                    endDay = 30;
                    break;

                case "Q3":
                    startMonth = 7;
                    endMonth = 9;
                    endDay = 30;
                    break;

                case "Q4":
                    startMonth = 10;
                    endMonth = 12;
                    break;
            }

            startDate = new DateTime(startYear, startMonth, startDay);
            endDate = new DateTime(endYear, endMonth, endDay);
        }

        public static Product GetProduct(long productID)
        {
            using (var lContentService = _dataUtil.ContentServiceInstance())
            {
                Product product = null;

                product = lContentService.GetProduct(productID);

                return product;
            }
        }
        private static string GetQuarter(int month)
        {
            string quarter = string.Empty;

            if (month >= 1 && month <= 3)
            {
                quarter = "Q1";
            }
            if (month >= 4 && month <= 6)
            {
                quarter = "Q2";
            }
            if (month >= 7 && month <= 9)
            {
                quarter = "Q3";
            }
            if (month >= 10 && month <= 12)
            {
                quarter = "Q4";
            }

            return quarter;
        }
        /// <summary>
        /// Returns total points gained by a member on selected quarter 
        /// </summary>
        /// <returns></returns>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public static double GetCurrentBalance(Member member)
        public static decimal GetCurrentBalance(Member member)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            //Set default as 0
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //double points = 0;
            Decimal points = 0;
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            decimal pointsOnHold = 0;

            try
            {
                if (null != member)
                {
                    GetProgramDates(member, out startDate, out endDate); // PI 30364 - Dollar reward program
                    points = member.GetPoints(startDate, endDate);
                    pointsOnHold = GetPointsOnHold(member, startDate, endDate);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetCurrentBalance - " + ex.Message);

            }
            return points - pointsOnHold;
        }
        public static void AddEmailBonus(VirtualCard virtualCard, MemberDetails mbrDetails)
        {
            AddEmailBonus(virtualCard, mbrDetails, "EmailBonusPoints");
        }
        public static void AddEmailBonus(VirtualCard virtualCard, MemberDetails mbrDetails, string emailBonusConfig)
        {
            string _pointType = "Bonus Points";
            string _pointEvent = "E-mail Address Bonus";

            String strPoints = String.Empty;
            using (var ldService = _dataUtil.DataServiceInstance())
            {
                strPoints = ldService.GetClientConfigProp(emailBonusConfig);
            }
            //Double dblPoints = 0;  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal dblPoints = 0;  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            //Double.TryParse(strPoints, out dblPoints);  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal.TryParse(strPoints, out dblPoints);  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            if (AddBonusPoints(virtualCard, _pointType, _pointEvent, dblPoints))
            {
                mbrDetails.HasEmailBonusCredit = true;
            }
        }

        public static void AddSMSBonus(VirtualCard virtualCard, MemberDetails mbrDetails)
        {
            AddSMSBonus(virtualCard, mbrDetails, "SMSBonusPoints");
        }

        public static void AddSMSBonus(VirtualCard virtualCard, MemberDetails mbrDetails, string smsBonusConfig)
        {
            string _pointType = "Bonus Points";
            string _pointEvent = "Text Message Sign-up Bonus";

            String strPoints = String.Empty;
            using (var ldService = _dataUtil.DataServiceInstance())
                strPoints = ldService.GetClientConfigProp(smsBonusConfig);
            //Double dblPoints = 0;   // AEO-74 Upgrade 4.5 changes here -----------SCJ
            //Double.TryParse(strPoints, out dblPoints);  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal dblPoints = 0;   // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal.TryParse(strPoints, out dblPoints);  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            if (AddBonusPoints(virtualCard, _pointType, _pointEvent, dblPoints))
            {
                mbrDetails.SMSOptIn = true;
                mbrDetails.SmsOptInDate = DateTime.Now;
            }
        }

        /// <summary>
        /// PI#30016 , Rizwan, Added method to award Google Wallet Bonus
        /// </summary>
        /// <param name="virtualCard"></param>
        /// <param name="mbrDetails"></param>
        public static void AddGWBonus(VirtualCard virtualCard, MemberDetails mbrDetails)
        {
            string _pointType = "Bonus Points";
            string _pointEvent = "Google Wallet Sign-Up Bonus";

            String strPoints = String.Empty;
            using (var ldService = _dataUtil.DataServiceInstance())
                strPoints = ldService.GetClientConfigProp("GWBonusPoints");
            //Double dblPoints = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            //Double.TryParse(strPoints, out dblPoints); // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal dblPoints = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal.TryParse(strPoints, out dblPoints); // AEO-74 Upgrade 4.5 changes here -----------SCJ
            if (AddBonusPoints(virtualCard, _pointType, _pointEvent, dblPoints))
            {
                mbrDetails.HasGWBonusCredit = 1;
            }
        }

        public static void AddRegistrationBonus(VirtualCard virtualCard)
        {
            string _pointType = "Bonus Points";
            string _pointEvent = "Registration Bonus";

            String strPoints = String.Empty;
            using (var ldService = _dataUtil.DataServiceInstance())
                strPoints = ldService.GetClientConfigProp("RegistrationBonusPoints");
            //Double dblPoints = 0;  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            //Double.TryParse(strPoints, out dblPoints); // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal dblPoints = 0;  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal.TryParse(strPoints, out dblPoints); // AEO-74 Upgrade 4.5 changes here -----------SCJ
            AddBonusPoints(virtualCard, _pointType, _pointEvent, dblPoints);
        }

        // AEO-74 Upgrade 4.5 changes here -----------SCJ
        //public static bool AddBonusPoints(VirtualCard virtualCard, string pointTypeName, string pointEventName, double points)

        public static bool AddBonusPoints(VirtualCard virtualCard, string pointTypeName, string pointEventName, decimal points, DateTime? optionalEndDate = null, DateTime? optionalTransactionDate = null, string optionalNote = null, string OptionalChangeBy = null)
        {
            PointType pointType = null;
            PointEvent pointEvent = null;
            bool returnValue = false;
            DateTime currentStartDate = DateTime.MinValue;
            DateTime currentEndDate = optionalEndDate ?? DateTime.MinValue;
            DateTime transactionDate = optionalTransactionDate ?? DateTime.Today;
            String strNote = optionalNote ?? string.Empty;
            String strChangedBy = OptionalChangeBy ?? string.Empty; //AEO-1841 Adding CS Agent Name to Point Appeasement
                                                                    // PI 30364 - Dollar reward program changes - Start
                                                                    //GetQuarterDates(out currentStartDate, out currentEndDate);

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            string loyaltyNumber = virtualCard.LoyaltyIdNumber;
            Member member;
            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                member = lwService.LoadMemberFromLoyaltyID(loyaltyNumber);
            }
            if (currentEndDate == DateTime.MinValue)
            {
                GetProgramDates(member, out currentStartDate, out currentEndDate);
            }
            // PI 30364 - Dollar reward program - End

            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get MemberDetails");
                // AEO-Redesign 2015 Begin
                IList<IClientDataObject> details = member.GetChildAttributeSets("MemberDetails", true);

                MemberDetails md = (details == null || details.Count == 0 ? null : details[0]) as MemberDetails;
                if (md != null)
                {

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "currentEndDate: " + currentEndDate.ToShortDateString());
                    IList<string> oldPoints = new List<string>(new string[] { "Bonus Points", "CS Points" });
                    IList<string> newPoints = new List<string>(new string[] { "AEO Connected Bonus Points", "AEO Customer Service Points" });

                    if (Utilities.isInPilot(md.ExtendedPlayCode))
                    {  // Point conversion
                        if (oldPoints.Contains(pointTypeName))
                        {
                            int index = oldPoints.IndexOf(pointTypeName);
                            pointTypeName = newPoints[index];
                        }
                    }

                    // AEO Redesign 2015 end
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Add Bonus Points for Event: {0}, StartDate: {1}, EndDate: {2}", pointEventName, transactionDate, currentEndDate.AddDays(1)));
                    if (GetPointTypeAndEvent(pointTypeName, pointEventName, out pointEvent, out pointType))
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Give Bonus Points: " + pointEvent.Name);
                        // AEO-74 Upgrade 4.5 changes here -----------SCJ
                        // LWDataServiceUtil.DataServiceInstance(true).Credit( virtualCard, pointType, pointEvent, points, DateTime.Today, currentEndDate.AddDays(1), string.Empty, string.Empty);
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            lwService.Credit(virtualCard, pointType, pointEvent, points, null, transactionDate, currentEndDate.AddDays(1), new PointTransactionOwnerType(), -1, -1, strNote, string.Empty, strChangedBy);
                        }
                        returnValue = true;
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = false;
                }

            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                returnValue = false;
            }
            return returnValue;

        }
        public static void AddMemberRewardFulfillment(Member member, long memberRewardID, string rewardType, long parentRewardId, string rewardDescription, string rewardPartNumber)
        {
            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                VirtualCard vCard = GetVirtualCard(member);
                MemberRewardFulfillment rewardFulfillment = new MemberRewardFulfillment();
                rewardFulfillment.LoyaltyNumber = vCard.LoyaltyIdNumber;
                rewardFulfillment.MemberRewardID = memberRewardID;
                rewardFulfillment.RewardType = rewardType;
                rewardFulfillment.RewardDescription = rewardDescription;
                rewardFulfillment.RewardPartNumber = rewardPartNumber;
                rewardFulfillment.ParentRewardId = parentRewardId;
                member.AddChildAttributeSet(rewardFulfillment);
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    lwService.SaveMember(member);
                }
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
        public static Boolean HasMemberEarnedSMSBonus(Member pMember)
        {
            PointType pointType = null;
            PointEvent pointEvent = null;

            //Double dblBonusPoints = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            Decimal dblBonusPoints = 0; // AEO-74 Upgrade 4.5 changes  here -----------SCJ

            DateTime endDate = DateTime.Parse("12/31/2100");

            int index = 0;
            long[] vcKeys = new long[pMember.LoyaltyCards.Count];
            long[] pointTypeIDs = new long[1];
            long[] pointEventIDs = new long[1];

            string _pointType = "Bonus Points";
            string _pointEvent = "Text Message Sign-up Bonus";

            if (Utilities.GetPointTypeAndEvent(_pointType, _pointEvent, out pointEvent, out pointType))
            {
                foreach (VirtualCard card in pMember.LoyaltyCards)
                {
                    vcKeys[index] = card.VcKey;
                    ++index;
                }

                pointTypeIDs[0] = pointType.ID;
                pointEventIDs[0] = pointEvent.ID;
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //dblBonusPoints = pMember.GetPoints(pointType, pointEvent, DateTime.MinValue, endDate) + LWDataServiceUtil.DataServiceInstance(true).GetExpiredPointBalance(vcKeys, pointTypeIDs, pointEventIDs, null, DateTime.MinValue, endDate, null, null, null, null, null, null, null);
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    dblBonusPoints = pMember.GetPoints(pointTypeIDs, pointEventIDs, DateTime.MinValue, endDate) + lwService.GetExpiredPointBalance(vcKeys, pointTypeIDs, pointEventIDs, null, DateTime.MinValue, endDate, null, null, null, null, null, null, null);
                }
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
            if (dblBonusPoints > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Create a Customer Service Note
        /// </summary>
        /// <param name="noteText"></param>
        /// <param name="memberID"></param>
        /// <param name="agentUserID"></param>
        public static void CreateCSNote(string noteText, long memberID, long createdBy)
        {
            CSNote note = new CSNote();
            note.Note = noteText;
            note.MemberId = memberID;
            note.CreateDate = DateTime.Now;
            note.CreatedBy = createdBy;
            using (var inst = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
            {
                inst.CreateNote(note);
            }
        }

        public static Boolean HasMemberEarnedRegistrationBonus(Member pMember)
        {
            PointType pointType = null;
            PointEvent pointEvent = null;
            //Double dblBonusPoints = 0; // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            Decimal dblBonusPoints = 0;  // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            DateTime endDate = DateTime.Parse("12/31/2100");

            string _pointType = "Bonus Points";
            string _pointEvent = "Registration Bonus";

            if (Utilities.GetPointTypeAndEvent(_pointType, _pointEvent, out pointEvent, out pointType))
            {
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                long[] pointTypeL = new long[1];
                long[] pointEventL = new long[1];
                pointTypeL[0] = pointType.ID;
                pointEventL[0] = pointEvent.ID;

                //dblBonusPoints = pMember.GetPoints(pointType, pointEvent, DateTime.MinValue, endDate);
                dblBonusPoints = pMember.GetPoints(pointTypeL, pointEventL, DateTime.MinValue, endDate);
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
            if (dblBonusPoints > 0)
            {
                return true;
            }
            return false;
        }
        public static int GetBraThreshold()
        {
            int returnValue = 0;

            try
            {
                using (var dServices = _dataUtil.DataServiceInstance())
                {
                    ClientConfiguration objClientConfiguration = dServices.GetClientConfiguration("BraPromoFulFillmentThreshold");
                    if (null != objClientConfiguration)
                    {
                        Int32 i32BraPromoFulFillmentThreshold = 0;
                        Int32.TryParse(objClientConfiguration.Value, out i32BraPromoFulFillmentThreshold);
                        if (i32BraPromoFulFillmentThreshold > 0)
                        {
                            returnValue = i32BraPromoFulFillmentThreshold;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Trace("Custom bScript", "GetBraThreshold", "Error: " + ex.Message);
            }
            return returnValue;
        }
        public static bool GetPointTypeAndEvent(string pointTypeName, string pointEventName, out PointEvent pointEvent, out PointType pointType)
        {
            pointEvent = null;
            pointType = null;
            bool returnValue = true;

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: PointTypeName - " + pointTypeName + ", PointEventName - " + pointEventName);
            //string env = LWDataServiceUtil.DataServiceInstance(true).EnvName; Upgrade 5.0 changes

            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                pointType = lwService.GetPointType(pointTypeName);
                if (pointType == null)
                {
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Point Type is not defined: " + pointTypeName);
                }
                pointEvent = lwService.GetPointEvent(pointEventName);

                if (pointEvent == null)
                {
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Point Event is not defined: " + pointEventName);
                }

                if ((pointEvent == null) || (pointType == null))
                {
                    returnValue = false;
                }
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
            return returnValue;
        }
        /// <summary>
        /// Returns card type
        /// </summary>
        /// <param name="dataService">ILWDataService dataService</param>
        /// <returns>return refCardType</returns>
        public static IList<RefCardType> GetCardType()
        {
            IList<RefCardType> refCardType = null;
            //IList<IClientDataObject> objCardType = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "RefCardType", null, new LWQueryBatchInfo(), false);  // AEO-74 Upgrade 4.5 here -----------SCJ
            IList<IClientDataObject> objCardType;
            using (var dService = _dataUtil.LoyaltyDataServiceInstance())
            {
                objCardType = dService.GetAttributeSetObjects(null, "RefCardType", null, null, false);
            }

            refCardType = new List<RefCardType>();
            foreach (IClientDataObject obj in objCardType)
            {
                RefCardType cardType = (RefCardType)obj;
                refCardType.Add(cardType);
            }
            return refCardType;
        }
        /// <summary>
        /// Use to Validate Name
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsNameValid(ref string param)
        {
            if (param == null)
            {
                param = string.Empty;
            }

            Regex regEx = new Regex(@"^[a-zA-Z,.\-'’‘\s\p{L}]{1,50}$"); /* AEO-500 */
            if (!regEx.Match(param).Success)
            {
                return false;
            }
            else
            {
                StringBuilder paramTransformed = new StringBuilder(param);
                paramTransformed.Replace("’", "'");
                paramTransformed.Replace("‘", "'");
                param = paramTransformed.ToString();
            }

            return true;
        }

        /// <summary>
        /// Use to Validate City
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsCityValid(string param)
        {
            Regex regEx = new Regex(@"^[a-zA-Z,.'\s\p{L}]{1,50}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate Address
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsAddressValid(string param)
        {
            Regex regEx = new Regex(@"^[a-zA-Z0-9,.%#\-/\s_!*+:;@'\p{L}]{0,50}$"); // AEO-33 Add unicode character support
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Use to Validate BaseBrand
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsBaseBrandValid(string param)
        {
            Regex regEx = new Regex(@"^(0|00|20|50)$", RegexOptions.IgnoreCase);
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Use to Validate State
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsStateValid(string param, string countryCode)
        {
            Regex regEx = new Regex(@"^[A-Z]{1,2}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }
            else
            {
                // Initialize Instance of dataservice
                StateValidation stateValid = new StateValidation();
                stateValid.LoadStateList();
                if (!stateValid.StateIsValid(param, countryCode))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Use to Validate PostalCode
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsPostalCodeValid(string param)
        {
            Regex regEx = new Regex(@"(^\d{5}-\d{4}|\d{5}|\d{9})$|(^[ABCEGHJKLMNPRSTVXY]{1}\d{1}[A-Z]{1} *\d{1}[A-Z]{1}\d{1}$)");

            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate CountryCode
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsCountryCodeValid(string param)
        {
            Regex regEx = new Regex(@"^(USA|CAN)$", RegexOptions.IgnoreCase);
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate Email
        /// </summary>
        /// <param name="emailAddress">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsEmailValid(string emailAddress)
        {
            try
            {
                //Validate Lenght
                if (emailAddress.Length > 255)
                    return false;

                //Validate start
                if (emailAddress.ToLower().StartsWith("www"))
                    return false;

                //Validate pipes. //AEO-377 // EHP //
                if (emailAddress.Contains("|"))
                    return false;
                // AEO-33 Add support for unicode language characters AH
                Regex regEx = new Regex(@"^([a-zA-Z0-9_\-\.\p{L}]+)@((\[[0-9]{1,3}" +
                                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-\p{L}]+\" +
                                        @".)+))([a-zA-Z\p{L}]{2,4}|[0-9]{1,3})(\]?)$");
                //Regex regEx = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                if (!regEx.Match(emailAddress).Success)
                    return false;

                if (emailAddress.Substring(emailAddress.IndexOf('@') - 1, 1) == ".")
                    return false;

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Matches Input FirstName and LastName with the mached Firstname and lastName
        /// from Brierly database.
        /// </summary>
        /// <param name="member">Instance of member found based on LoyaltyNumberID</param>
        /// <param name="firstName">FirstName provided as input</param>
        /// <param name="lastName">LastName provided as input</param>
        /// <returns>Validation Status</returns>
        public static bool IsNameMatches(Member member, string firstName, string lastName)
        {
            // Validate FirstName
            int considerableLength = firstName.Length < member.FirstName.Length ? firstName.Length : member.FirstName.Length;

            if (considerableLength >= 3)
            {
                if (firstName.Trim().ToLower().Substring(0, 3) != member.FirstName.ToLower().Substring(0, 3))
                {
                    return false;
                }
            }
            else
            {
                if (considerableLength == 2)
                {
                    if (firstName.Trim().ToLower().Substring(0, 2) != member.FirstName.ToLower().Substring(0, 2))
                    {
                        return false;
                    }
                }
                else if (considerableLength == 1)
                {
                    if (firstName.Trim().ToLower().Substring(0, 1) != member.FirstName.ToLower().Substring(0, 1))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            // Validate LastName
            string lastNameForCompare = lastName.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty);
            if (lastNameForCompare.ToLower() != member.LastName.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty).ToLower())
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Use to Validate Phone
        /// Allowed - 10 digits. No dashes or parenthesis
        /// Range - 0 to 10 character
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsPhoneValid(string param)
        {
            Regex regEx = new Regex(@"^[0-9]{0,10}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate Gender
        /// Numeric:
        /// Unknown = 0
        /// Male = 1
        /// Female = 2
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsGenderValid(string param)
        {
            Regex regEx = new Regex(@"^[0-2]{0,1}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate LanguagePreference
        /// Numeric:
        /// English = 0
        /// Spanish = 1
        /// French = 2
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsLanguagePreferenceValid(string param)
        {
            Regex regEx = new Regex(@"^[0-2]{0,1}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }
        #region Other Methods [Added By Manoj]

        /// <summary>
        /// Returns Loyalty ID numnber of a selected member
        /// </summary>
        /// <returns></returns>
        public static String GetLoyaltyIDNumber(Member member)
        {
            foreach (VirtualCard vc in member.LoyaltyCards)
            {
                if (vc.IsPrimary)
                {
                    return vc.LoyaltyIdNumber;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Returns the name of the selected member
        /// </summary>
        /// <returns></returns>
        public static String GetMemberName(Member member)
        {
            String strRetValue = String.Empty;
            try
            {
                if (null != member)
                {
                    strRetValue = member.FirstName + " " + member.LastName;
                }
            }
            catch (Exception) { }
            return strRetValue;
        }

        //LW 4.1.14 removed - no reference
        /// <summary>
        /// Returns true or false for memeber termination status
        /// </summary>
        /// <returns></returns>
        //public static string IsMemberTerminated(Member member)
        //{
        //    String strReturnVal = "false";
        //    try
        //    {
        //        if (member.MemberStatus == MemberStatusEnum.Terminated)
        //        {
        //            strReturnVal = "true";
        //        }
        //    }
        //    catch (Exception) { }
        //    return strReturnVal;
        //}

        /// <summary>
        /// Returns true if member dosn't have a linked account else true
        /// </summary>
        /// <returns></returns>
        public static string IsMemberDoesNotHaveLinkedAccount(MemberDetails memberDetails)
        {
            String strReturnVal = "true";
            try
            {
                if (null != memberDetails && null != memberDetails.MemberSource)
                {
                    if (memberDetails.MemberSource.Value == (Int32)MemberSource.OnlineAEEnrolled || memberDetails.MemberSource.Value == (Int32)MemberSource.OnlineAERegistered)
                    {
                        strReturnVal = "false";
                    }
                }
            }
            catch (Exception) { }
            return strReturnVal;
        }

        /// <summary>
        /// Returns true if mail is undeliverable else false
        /// </summary>
        /// <returns></returns>
        public static string isEmailUndeliverable(MemberDetails memberDetails)
        {
            String strReturnVal = "false";
            try
            {
                if (null != memberDetails && memberDetails.EmailAddressMailable.HasValue)
                {
                    strReturnVal = memberDetails.EmailAddressMailable.Value.ToString();
                }
            }
            catch (Exception) { }
            return strReturnVal;
        }

        /// <summary>
        /// Returns email validation result true/false
        /// </summary>
        /// <returns></returns>
        public static string isEmailPassValidation(MemberDetails memberDetails)
        {
            String strReturnVal = "false";
            try
            {
                if (null != memberDetails && memberDetails.PassValidation.HasValue)
                {
                    strReturnVal = memberDetails.PassValidation.Value.ToString();
                }
            }
            catch (Exception) { }
            return strReturnVal;
        }

        /// <summary>
        /// Returns member's SMS Opt-in status
        /// </summary>
        /// <returns></returns>
        public static string IsMemberSMSOptIn(MemberDetails memberDetails)
        {
            String strReturnVal = "false";
            try
            {
                if (null != memberDetails && memberDetails.SMSOptIn.HasValue)
                {
                    if (memberDetails.SMSOptIn.Value)
                    {
                        strReturnVal = memberDetails.SMSOptIn.Value.ToString();
                    }
                }
            }
            catch (Exception) { }
            return strReturnVal;
        }

        /// <summary>
        /// Returns true if member's address if mailable else false
        /// </summary>
        /// <returns></returns>
        public static string IsMemberAddressMailable(MemberDetails memberDetails)
        {
            String strReturnVal = "false";
            try
            {
                if (null != memberDetails && memberDetails.AddressMailable.HasValue)
                {
                    strReturnVal = memberDetails.AddressMailable.Value.ToString();
                }
            }
            catch (Exception) { }
            return strReturnVal;
        }

        /// <summary>
        /// Returns _member brand 
        /// </summary>
        /// <returns></returns>
        public static string GetMemberBrand(MemberDetails memberDetails)
        {

            String StrRetValue = String.Empty;
            try
            {
                LWCriterion lwCriteria = new LWCriterion("RefBrands");
                lwCriteria.Add(LWCriterion.OperatorType.AND, "BRANDID", memberDetails.BaseBrandID, LWCriterion.Predicate.Eq);
                //List<IClientDataObject> lwBrands = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "RefBrands", lwCriteria, new LWQueryBatchInfo(), false).ToList();  // AEO-74 Upgrade 4.5 here -----------SCJ
                using (var lwServices = _dataUtil.LoyaltyDataServiceInstance())
                {
                    List<IClientDataObject> lwBrands = lwServices.GetAttributeSetObjects(null, "RefBrands", lwCriteria, null, false).ToList();
                    if (null != lwBrands)
                    {
                        RefBrand refBrand = (RefBrand)lwBrands[0];
                        StrRetValue = refBrand.BrandName;
                    }
                }
            }
            catch (Exception) { }
            return StrRetValue;
        }

        #endregion
        #region Account Details [Added By Manoj]

        /// <summary>
        /// Returns bonus points of a selected member on selected quarter 
        /// </summary>
        /// <returns>Bonus points</returns>
        public static string GetBonusPoints(Member member, DateTime startDate, DateTime endDate)
        {
            // AEO-2099 int index = 0;
            //Default return value would be 0
            String strRetValue = "0";
            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                    IList<PointType> pointTypes = lwService.GetAllPointTypes();
                    // AEO-2099 long[] pointTypeIDs = new long[pointTypes.Count];

                    // AEO-2099 Begin
                    // AEO-Redesign-2015 Begin
                    /*IList<IClientDataObject> loDetails = member.GetChildAttributeSets("MemberDetails");
                    MemberDetails details = ( loDetails == null || loDetails.Count == 0 ? null : loDetails[0] ) as MemberDetails;
                    bool isPilot = details != null ? Utilities.isInPilot(details.ExtendedPlayCode): false; // point conversion*/
                    // AEO-Redesign-2015 End
                    bool isPilot = true;
                    // AEO-2099 End

                    /* AEO-2099 Begin
                    foreach (PointType pt in pointTypes)
                    {

                        // AEO-Redesign-2015 Begin
                        bool isExcluded = isPilot ?
                            ((pt.Name.ToUpper() == "BASIC POINTS") || (pt.Name.ToUpper() == "ADJUSTMENT POINTS")) || 
                            ((pt.Name.ToUpper() == "AEO CONNECTED POINTS") || (pt.Name.ToUpper() == "AEO VISA CARD POINTS") || 
                            (pt.Name.ToUpper() == "BRA POINTS") || (pt.Name.ToUpper() == "JEAN POINTS")) || 
                            (   ( pt.Name.ToUpper() == "AE CONNECTED BASE POINTS" || pt.Name.ToUpper() == "AEO CONNECTED POINTS" )) //|| (pt.Name.ToUpper() == "BONUS POINTS") ---AEO-593
                            : ((pt.Name.ToUpper() == "BASIC POINTS") || (pt.Name.ToUpper() == "ADJUSTMENT POINTS") || (pt.Name.ToUpper() == "BRA POINTS") || (pt.Name.ToUpper() == "JEAN POINTS"));

                        if ( isExcluded)
                            // AEO-Redesign-2015 End
                        {
                            //Exclude these PointTypes
                        }
                        else
                        {
                            pointTypeIDs[index] = pt.ID;
                            ++index;
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Include: " + pt.Name);
                        }
                    }
                    AEO-2099 End */

                    // AEO-2099 Begin
                    var pointTypeFilter = pointTypes.Where(pointType =>
                    {
                        bool isExcluded = isPilot ?
                            ((pointType.Name.ToUpper() == "BASIC POINTS") || (pointType.Name.ToUpper() == "ADJUSTMENT POINTS")) ||
                            ((pointType.Name.ToUpper() == "AEO CONNECTED POINTS") || (pointType.Name.ToUpper() == "AEO VISA CARD POINTS") ||
                            (pointType.Name.ToUpper() == "BRA POINTS") || (pointType.Name.ToUpper() == "JEAN POINTS")) ||
                            ((pointType.Name.ToUpper() == "AE CONNECTED BASE POINTS" || pointType.Name.ToUpper() == "AEO CONNECTED POINTS") || (pointType.Name.ToUpper() == "NETSPEND")) //|| (pt.Name.ToUpper() == "BONUS POINTS") ---AEO-593
                            : ((pointType.Name.ToUpper() == "BASIC POINTS") || (pointType.Name.ToUpper() == "ADJUSTMENT POINTS") || (pointType.Name.ToUpper() == "BRA POINTS") || (pointType.Name.ToUpper() == "JEAN POINTS"));

                        if (isExcluded)
                            //Exclude these PointTypes
                            return false;
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Include: " + pointType.Name);
                        return true;
                    });

                    long[] pointTypeIDs = pointTypeFilter.Any() ? pointTypeFilter.Select(pointType => pointType.ID).ToArray() : new long[0];
                    // AEO-2099 End

                    //strRetValue = Convert.ToString(member.GetPoints(pointTypeIDs, startDate, endDate));
                    // PI 21845, aali, show the bonus points balance as zero, if it gets negative
                    strRetValue = GetPointsBalance(member, startDate, endDate, pointTypeIDs, true).ToString();
                }
            }
            catch (Exception) { }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
            return strRetValue;
        }

        /* AEO-2690 BEGIN*/


        /// <summary>
        /// Returns Engagement points of a selected member on selected quarter 
        /// </summary>
        /// <returns>basic points</returns>
        public static string GetEngagementPoints(Member member, DateTime startDate, DateTime endDate)
        {
            //Default return value would be 0
            String strRetValue = "0";

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetEngagementPoints : Begin - StartDate: " + startDate.ToShortDateString());
            try
            {
                // AEO-2099 int index = 0;
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    IList<PointType> pointTypes = lwService.GetAllPointTypes();
                     bool isPilot = true;
                 
                    // AEO-2099 Begin
                    var pointTypeFilter = pointTypes.Where(pointType =>
                    {
                        bool isIncluded = isPilot ?
                       ((pointType.Name.ToUpper() == "AEO CONNECTED ENGAGEMENT POINTS"))
                       : ((pointType.Name.ToUpper() == "AEO CONNECTED ENGAGEMENT POINTS"));

                        return isIncluded;
                    });

                    long[] pointTypeIDs = pointTypeFilter.Any() ? pointTypeFilter.Select(pointType => pointType.ID).ToArray() : new long[0];
                    // AEO-2099 End

                    //strRetValue = Convert.ToString(member.GetPoints(pointTypeIDs, startDate, endDate));
                    // PI 21845, aali, show the basic points balance as zero, if it gets negative
                    strRetValue = GetPointsBalance(member, startDate, endDate, pointTypeIDs, true).ToString();

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetEngagementPoints : Points: " + strRetValue);

                }
            }
            catch (Exception) { }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetEngagementPoints : End");
            return strRetValue;
        }

        /* AEO-2690 END*/

        /// <summary>
        /// Returns basic points of a selected member on selected quarter 
        /// </summary>
        /// <returns>basic points</returns>
        public static string GetBasicPoints(Member member, DateTime startDate, DateTime endDate)
        {
            //Default return value would be 0
            String strRetValue = "0";

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetBasicPoints : Begin - StartDate: " + startDate.ToShortDateString());
            try
            {
                // AEO-2099 int index = 0;
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    IList<PointType> pointTypes = lwService.GetAllPointTypes();
                    // AEO-2099 long[] pointTypeIDs = new long[pointTypes.Count];

                    // AEO-2099 begin
                    // AEO-Redesign-2015 Begin
                    /*IList<IClientDataObject> loDetails = member.GetChildAttributeSets("MemberDetails");
                    MemberDetails details = ( loDetails == null || loDetails.Count == 0 ? null : loDetails[0] ) as MemberDetails;
                    bool isPilot = details != null ? Utilities.isInPilot(details.ExtendedPlayCode) : false; // Point Conversion*/
                    // AEO-Redesign-2015 End
                    bool isPilot = true;
                    //AEO-2099 end

                    /* AEO-2099 Begin
                    foreach (PointType pt in pointTypes)
                    {
                        // AEO-Redesign-2015 Begin
                        bool isIncluded = isPilot ?
                            ( ( pt.Name.ToUpper() == "BASIC POINTS" ) || ( pt.Name.ToUpper() == "ADJUSTMENT POINTS" ) ) || 
                            ( ( pt.Name.ToUpper() == "AEO CONNECTED POINTS" ) || ( pt.Name.ToUpper() == "AEO VISA CARD POINTS" ) ) ||
                            ( pt.Name.ToUpper() == "AE CONNECTED BASE POINTS" || pt.Name.ToUpper() == "AEO CONNECTED POINTS" )

                            : ((pt.Name.ToUpper() == "BASIC POINTS") || (pt.Name.ToUpper() == "ADJUSTMENT POINTS")) ;

                        if (isIncluded)
                        // AEO-Redesign-2015 End
                        {
                            pointTypeIDs[index] = pt.ID;
                            ++index;
                        }
                    }
                    AEO-2099 End */

                    // AEO-2099 Begin
                    var pointTypeFilter = pointTypes.Where(pointType =>
                    {
                        bool isIncluded = isPilot ?
                           ((pointType.Name.ToUpper() == "BASIC POINTS") || (pointType.Name.ToUpper() == "ADJUSTMENT POINTS")) ||
                           ((pointType.Name.ToUpper() == "AEO CONNECTED POINTS") || (pointType.Name.ToUpper() == "AEO VISA CARD POINTS")) ||
                           (pointType.Name.ToUpper() == "AE CONNECTED BASE POINTS" || pointType.Name.ToUpper() == "AEO CONNECTED POINTS")
                           : ((pointType.Name.ToUpper() == "BASIC POINTS") || (pointType.Name.ToUpper() == "ADJUSTMENT POINTS"));

                        return isIncluded;
                    });

                    long[] pointTypeIDs = pointTypeFilter.Any() ? pointTypeFilter.Select(pointType => pointType.ID).ToArray() : new long[0];
                    // AEO-2099 End

                    //strRetValue = Convert.ToString(member.GetPoints(pointTypeIDs, startDate, endDate));
                    // PI 21845, aali, show the basic points balance as zero, if it gets negative
                    strRetValue = GetPointsBalance(member, startDate, endDate, pointTypeIDs, true).ToString();

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetBasicPoints : Points: " + strRetValue);

                }
            }
            catch (Exception) { }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetBasicPoints : End");
            return strRetValue;
        }

        /// <summary>
        /// Returns starting points of a member on selected quarter 
        /// </summary>
        /// <returns>starting points</returns>
        public static string GetStartingPoints(Member member, DateTime startDate, DateTime endDate)
        {
            //Default return value would be 0
            String strRetValue = "0";
            long[] pointTypeIDs = new long[1];
            try
            {
                PointType pointType = GetPointType("StartingPoints");
                pointTypeIDs[0] = pointType.ID;
                //strRetValue = Convert.ToString(member.GetPoints(pointType, startDate, endDate));
                strRetValue = GetPointsBalance(member, startDate, endDate, pointTypeIDs).ToString();


            }
            catch (Exception) { }
            return strRetValue;
        }
        /// <summary>
        /// Returns the current point balance based on the dates only for all point types.  
        /// This method had to be written to call the GetExpiredPointBalance when we are 
        /// getting points for a previous quarter.
        /// </summary>
        /// <returns></returns>
        //public static double GetPointsBalance(Member member, DateTime startDate, DateTime endDate)  // AEO-74 Upgrade 4.5 changes here -----------SCJ
        public static decimal GetPointsBalance(Member member, DateTime startDate, DateTime endDate)
        {
            int index = 0;
            //double RetValue = 0; // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            decimal RetValue = 0; // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            if (null != member)
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    IList<PointType> pointTypes = lwService.GetAllPointTypes();
                    long[] pointTypeIDs = new long[pointTypes.Count];

                    foreach (PointType pt in pointTypes)
                    {
                        if (((pt.Name.ToUpper().Contains("BRA")) || (pt.Name.ToUpper().Contains("JEAN")) || (pt.Name.ToUpper().Contains("NETSPEND"))))
                        {
                            //Exlude the bra points from the point balance.
                        }
                        else
                        {
                            pointTypeIDs[index] = pt.ID;
                            ++index;
                        }
                    }
                    //strRetValue = Convert.ToString(member.GetPoints(startDate, endDate));
                    RetValue = GetPointsBalance(member, startDate, endDate, pointTypeIDs);
                }
            }
            return RetValue;
        }
        /// <summary>
        /// Returns the current point balance based on the dates and point types.  
        /// This method had to be written to call the GetExpiredPointBalance when we are 
        /// getting points for a previous quarter.
        /// </summary>
        /// <returns></returns>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public static decimal GetPointsBalance(Member member, DateTime startDate, DateTime endDate, long[] pointTypeIDs, bool isBasicOrBonusPoints = false) // PI 21845, aali, new parameter to accomodate negative basic or bonus points
        {
            //set default 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            //double retValue = 0;
            Decimal retValue = 0;
            long[] vcKeys = new long[member.LoyaltyCards.Count];
            int index = 0;
            try
            {
                foreach (VirtualCard card in member.LoyaltyCards)
                {
                    vcKeys[index] = card.VcKey;
                    ++index;
                }
                DateTime currentStartDate = DateTime.MinValue;
                DateTime currentEndDate = DateTime.MinValue;
                decimal pointsOnHold = 0;
                // PI 30364 - Dollar reward program - Start
                // AEO-57 Changes start
                MemberDetails mbrDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;


                /* AEO-532 begin
                 
                bool isDollarRewardMember = false;
                isDollarRewardMember = (member != null && mbrDetails != null && mbrDetails.ExtendedPlayCode.HasValue && (  Utilities.isInPilot(mbrDetails.ExtendedPlayCode))); //pilot conversion
                if (isDollarRewardMember == true && startDate >= DollarRewardsProgramStartDate && startDate <= DollarRewardsProgramEndDate)
                {
                    if (DateTime.Now >= DollarRewardsProgramStartDate && DateTime.Now <= DollarRewardsProgramEndDate.AddDays(1).AddSeconds(-1))
                    {
                        retValue = member.GetPoints(pointTypeIDs, startDate, endDate);
                    }
                    else
                    {
                        retValue = LWDataServiceUtil.DataServiceInstance(true).GetExpiredPointBalance(vcKeys, pointTypeIDs, null, null, startDate, endDate, null, null, null, null, null, null, null);
                    }
                }
                 AEO-532 end */

                // AEO-532 begin
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    if (!Utilities.isInPilot(mbrDetails.ExtendedPlayCode))
                    {
                        //Unreachable code for National Rollout
                        GetQuarterDates(out currentStartDate, out currentEndDate);
                        if (currentStartDate.ToShortDateString() == startDate.ToShortDateString())
                        {
                            retValue = member.GetPoints(pointTypeIDs, startDate, endDate);
                        }
                        else
                        {
                            retValue = lwService.GetExpiredPointBalance(vcKeys, pointTypeIDs, null, null, startDate, endDate, null, null, null, null, null, null, null);
                        }
                    }
                    else
                    {
                        retValue = member.GetPoints(pointTypeIDs, startDate, endDate);
                        List<PointType> types = new List<PointType>();
                        foreach (var type in pointTypeIDs)
                        {
                            PointType pType = lwService.GetPointType(type);
                            if (pType != null)
                                types.Add(lwService.GetPointType(type));
                        }
                        pointsOnHold = lwService.GetPointsOnHold(member.LoyaltyCards, types, lwService.GetAllPointEvents(), startDate, endDate);
                    }
                }
                // AEO-532 end

                //GetProgramDates(member, out currentStartDate, out currentEndDate, true);
                //if ((startDate >= Utilities.DollarRewardsProgramStartDate && mbrDetails.ExtendedPlayCode.HasValue && mbrDetails.ExtendedPlayCode == 1 && startDate.Year == DateTime.Now.Year) || currentStartDate.ToShortDateString() == startDate.ToShortDateString())
                //{
                //    retValue = member.GetPoints(pointTypeIDs, startDate, endDate);
                //}
                //else
                //{
                //    retValue = LWDataServiceUtil.DataServiceInstance(true).GetExpiredPointBalance(vcKeys, pointTypeIDs, null, null, startDate, endDate, null, null, null, null, null, null, null);
                //}
                // AEO-57 Changes end
                // PI 30364 - Dollar reward program - End
                // PI 21845, aali, show the basic or bonus points balance as zero, if it gets negative, Starting
                retValue -= pointsOnHold;

                if (isBasicOrBonusPoints == false)
                {
                    if (retValue < 0)
                    {
                        retValue = 0;
                    }
                }


                // PI 21845, aali, show the basic or bonus points balance as zero, if it gets negative, Ending
            }
            catch (Exception) { }
            return retValue;
        }


        /// <summary>
        /// 
        /// Returns point to next rewards
        /// Find Reward with Points just above Total Points balance
        /// and subtract the total points balance from the reward points.
        /// Display 0 if Total Points Balance > highest Reward value.
        /// </summary>
        /// <returns></returns>
        public static string GetPointsToNextReward(Member member, DateTime startDate, DateTime endDate)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            //set default 
            String strRetValue = "0";
            // PI 30364 - Dollar reward program changes - Start
            decimal[] pointsExtraAccess = new decimal[] { 10000, 7500, 5000, 2500 };
            decimal[] pointsFullAccess = new decimal[] { 15000, 12500, 10000, 7500, 5000, 2500 };


            int gDollarRewPoints = 0;
            TierDef tier = null; // aeo-1581
            MemberTier mt = null; //EO-1581
            List<Decimal> pointsneeded = new List<Decimal>();

            try
            {

                //AEO-1581 BEGIN
                mt = member.GetTier(DateTime.Now);

                if (mt == null)
                {
                    throw new Exception("No tier defined for member =" + member.IpCode);
                }

                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    tier = lwService.GetTierDef(mt.TierDefId);
                }


                if (tier.Name.ToUpper() == "EXTRA ACCESS")
                {
                    pointsneeded.AddRange(pointsExtraAccess);


                    /*
                    RewardDef rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected Extra Access $60 Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected Extra Access $45 Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected Extra Access $30 Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected Extra Access $15 Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }*/
                }
                else if (tier.Name.ToUpper() == "FULL ACCESS")
                {
                    pointsneeded.AddRange(pointsFullAccess);
                    /*
                    RewardDef rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $100  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $95  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $90  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $85  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $80  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $75  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $70  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }
                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $65  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $60  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $55  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $50  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $45  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $40  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }
                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $35  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $30  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $25  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }
                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $20  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $15  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $10  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }

                    rewTemp = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Connected $5  Reward");
                    if ( rewTemp != null ) {
                        pointsneeded.Add(rewTemp.HowManyPointsToEarn);
                    }*/
                }
                // AEO-1581 END

                MemberDetails mbrDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                using (var dwService = _dataUtil.DataServiceInstance())
                {
                    ClientConfiguration objClientConfiguration = dwService.GetClientConfiguration("DollarRewardsPoints");
                    gDollarRewPoints = Convert.ToInt32(objClientConfiguration.Value);
                }
                //Double lngPointsBalance = GetPointsBalance(member, startDate, endDate); // AEO-74 Upgrade 4.5 changes here -----------SCJ


                // Decimal tmppending = Utilities.GetPointsOnHold(member, startDate, endDate); // AEO-2083 Points to Next Reward and Points File Updates
                String tmpstr = Utilities.GetTotalPoints(member, startDate, endDate);
                Decimal tmptotal = decimal.Zero;
                Decimal lngPointsBalance = decimal.Zero;

                // Decimal lngPointsBalance = GetPointsBalance(member, startDate, endDate); // AEO-74 Upgrade 4.5 changes here -----------SCJ


                if (decimal.TryParse(tmpstr, out tmptotal))
                {
                    lngPointsBalance = tmptotal;
                    //lngPointsBalance = tmptotal - tmppending; // AEO-2083 Points to Next Reward and Points File Updates
                }
                //else {
                //    lngPointsBalance = ( -1 * tmppending ); // AEO-2083 Points to Next Reward and Points File Updates
                //}

                /*
                if (lngPointsBalance < 0)
                {
                    lngPointsBalance = 0;
                }
                */
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Reward Points, Points: {0}, {1}", gDollarRewPoints, lngPointsBalance));

                //AEO Redesign 2015 Begin
                if (Utilities.isInPilot(mbrDetails.ExtendedPlayCode)) // point conversion
                {

                    // if the current balance is higher or equal than the maximum then we substract the maximum
                    while (lngPointsBalance >= pointsneeded[0])
                    {
                        lngPointsBalance -= pointsneeded[0];
                    }

                    Decimal nextlimit = 0;

                    for (int i = 0; i < pointsneeded.Count; i++)
                    {
                        if (lngPointsBalance < pointsneeded[i])
                        {
                            nextlimit = pointsneeded[i];
                        }
                        else
                        {
                            break;
                        }
                    }



                    strRetValue = Convert.ToString(Decimal.ToInt32(nextlimit - lngPointsBalance));

                    /*
                    if ( lngPointsBalance >= 1000 ) //Adding changes for AEO-1074              SCJ
                    {
                            int temp = (int)( ( Math.Floor(lngPointsBalance / 1000) * 1000 ) + 1000 - lngPointsBalance );

                            strRetValue = Convert.ToString(temp);
                    }
                    else {
                            //AEO-206
                            int temp = (int)( 1000 - lngPointsBalance );

                            strRetValue = Convert.ToString(temp);
                    }}*/
                    //AEO Redesign 2015 End

                }
                // PI 30364 - Dollar reward program changes - End
                else
                {
                    using (var ldService = _dataUtil.ContentServiceInstance())
                    {
                        IList<RewardDef> lstRewardDef = ldService.GetAllRewardDefs();

                        foreach (RewardDef reward in lstRewardDef.OrderBy(RewardDef => RewardDef.HowManyPointsToEarn))
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward Name: " + reward.Name);
                            //if (!reward.Name.Contains("Bra") && !reward.Name.Contains("Dollar Reward") && !reward.Name.Contains("Jean")) // PI 30364 - Dollar reward program changes
                            //{
                            //    if (reward.Active)
                            //    {
                            //        if (reward.HowManyPointsToEarn > lngPointsBalance)
                            //        {
                            //            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward Name Used: " + reward.Name);
                            //            strRetValue = Convert.ToString(reward.HowManyPointsToEarn - lngPointsBalance);
                            //            break;
                            //        }
                            //    }
                            //}
                            //AEO-1041 changes here ---------------------------SCJ

                            if ((reward.Name == "40% - Reward") || (reward.Name == "30% - Reward") || (reward.Name == "20% - Reward") || (reward.Name == "15% - Reward"))
                            {
                                if (lngPointsBalance > 499)
                                {
                                    strRetValue = "0";
                                }
                                else
                                {
                                    if (reward.HowManyPointsToEarn > lngPointsBalance)
                                    {
                                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward Name Used: " + reward.Name);
                                        strRetValue = Convert.ToString(reward.HowManyPointsToEarn - lngPointsBalance);
                                        break;
                                    }
                                }
                            }
                            //AEO-1041 changes end here ---------------------------SCJ
                        }
                    }
                }

            }
            catch (Exception) { }
            return strRetValue;
        }

        /// <summary>
        /// Returns Current Reward Level
        /// </summary>
        /// <returns>Current Reward Level</returns>
        public static string GetCurrentRewardLevel(Member member, DateTime startDate, DateTime endDate)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            //set default 
            String strRetValue = string.Empty;
            //double lowestPoints = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            decimal lowestPoints = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            // PI 30364 - Dollar reward program changes - Start
            int gDollarRewPoints = 0;
            int rewardLevel = 0;

            try
            {
                MemberDetails mbrDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                using (var dwService = _dataUtil.DataServiceInstance())
                {
                    ClientConfiguration objClientConfiguration = dwService.GetClientConfiguration("DollarRewardsPoints");
                    gDollarRewPoints = Convert.ToInt32(objClientConfiguration.Value);
                }
                // AEO-57 - APIs changes : using the passed date range instead of GetProgramDates or GetQuarterDates -Start
                endDate = endDate.AddDays(1);
                endDate = endDate.AddSeconds(-1);
                //Double lngPointsBalance = GetPointsBalance(member, startDate, endDate); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                Decimal lngPointsBalance = GetPointsBalance(member, startDate, endDate); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                // AEO-57 - APIs changes : using the passed date range instead of GetProgramDates or GetQuarterDates -End

                // AEO-Redesign Begin
                if (Utilities.isInPilot(mbrDetails.ExtendedPlayCode)) // point conversin
                {

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Points, current reward, start date, end date: {0}, {1}, {2}, {3}", lngPointsBalance, lngPointsBalance / 1000, startDate, endDate));

                    rewardLevel = (int)(lngPointsBalance / 1000);
                    strRetValue = rewardLevel.ToString();

                    /*
                     
                    if (lngPointsBalance > 1500)
                    {
                        strRetValue = String.Format("{0:C}", 100);
                    }
                    else if (lngPointsBalance >= 150)
                    {
                        rewardLevel = (int)(lngPointsBalance / gDollarRewPoints);
                        strRetValue = String.Format("{0:C}", rewardLevel * 10);
                    }
                     
                    */

                    // AEO-Redesign End
                }
                else
                {
                    using (var cService = _dataUtil.ContentServiceInstance())
                    {
                        Category category = cService.GetCategory(0, "Reward");

                        if (category != null)
                        {

                            IList<RewardDef> lstRewardDef = cService.GetRewardDefsByCategory(category.ID);

                            foreach (RewardDef reward in lstRewardDef.OrderByDescending(RewardDef => RewardDef.HowManyPointsToEarn))
                            {
                                // if (!reward.Name.Contains("Dollar Reward")) PI 31368: Incorrect reward displaying on ae.com website for quarterly reward members
                                if (reward.Name.Contains("% - Reward"))
                                {
                                    if (reward.Active)
                                    {
                                        if (lngPointsBalance >= reward.HowManyPointsToEarn)
                                        {
                                            strRetValue = reward.Name.Replace(" - Reward", null);
                                            break;
                                        }
                                        else
                                        {
                                            if (lowestPoints == 0)
                                            {
                                                lowestPoints = reward.HowManyPointsToEarn;
                                            }
                                            else if (reward.HowManyPointsToEarn < lowestPoints)
                                            {
                                                lowestPoints = reward.HowManyPointsToEarn;
                                            }
                                        }
                                    }
                                }
                            }
                            if (lngPointsBalance >= lowestPoints)
                            {
                                if (strRetValue.Length == 0)
                                {
                                    foreach (RewardDef rwd in lstRewardDef.OrderByDescending(RewardDef => RewardDef.HowManyPointsToEarn))
                                    {
                                        // if (!rwd.Name.Contains("Dollar Reward")) PI 31368: Incorrect reward displaying on ae.com website for quarterly reward members
                                        if (!rwd.Name.Contains("% - Reward"))
                                        {
                                            if (rwd.Active)
                                            {
                                                if (lowestPoints == rwd.HowManyPointsToEarn)
                                                {
                                                    strRetValue = rwd.Name.Replace(" - Reward", null);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // PI 30364 - Dollar reward program changes - End

            }
            catch (Exception) { }
            return strRetValue;
        }


        /// <summary>
        /// Returns enrollment date of a member
        /// </summary>
        /// <returns></returns>
        public static string GetEnrollmentDate(Member member)
        {
            String strRetVal = String.Empty;
            try
            {
                if (null != member)
                {
                    strRetVal = member.MemberCreateDate.ToString("M/d/yyyy");
                }
            }
            catch (Exception) { }
            return strRetVal;
        }

        /// <summary>
        /// Returns last purchased date 
        /// </summary>
        /// <returns></returns>
        public static string GetLastPurchasedDate(Member member)
        {
            String strRetVal = String.Empty;
            try
            {
                if (null != member)
                {
                    strRetVal = member.LastActivityDate.Value.ToString("M/d/yyyy");
                }
            }
            catch (Exception) { }
            return strRetVal;
        }

        /// <summary>
        /// Returns total points gained by a member on selected quarter 
        /// </summary>
        /// <returns></returns>
        public static string GetTotalPoints(Member member, DateTime startDate, DateTime endDate)
        {
            //Set default as 0
            String strRetValue = "0";
            try
            {
                if (null != member)
                {


                    /* AEO-532 Begin */

                    IList<IClientDataObject> loDetails = member.GetChildAttributeSets("MemberDetails");
                    MemberDetails details = (loDetails == null || loDetails.Count == 0 ? null : loDetails[0]) as MemberDetails;
                    /* AEO-1197 BEgin
                   bool isPilot = details != null && details.ExtendedPlayCode.HasValue ? Utilities.isInPilot(details.ExtendedPlayCode) : false; // Point Conversion

                   
                   if ( isPilot ) {
                       startDate = new DateTime(2000, 1, 1);
                       endDate = DateTime.Now.Date.AddDays(1);
                   }
                     AEO-1197 end*/
                    strRetValue = GetPointsBalance(member, startDate, endDate).ToString();

                    /* AEO-532 End */

                }
            }
            catch (Exception) { }
            return strRetValue;
        }
        /// <summary>
        /// Get Pimary Virtual card for a member
        /// </summary>
        /// <param name="member">Member object</param>
        /// <returns>Virtual card for a member </returns>
        public static VirtualCard GetVirtualCard(Member member)
        {
            VirtualCard virtualCard = member.GetLoyaltyCardByType(Brierley.FrameWork.Common.VirtualCardSearchType.PrimaryCard);
            if (virtualCard == null)
            {
                virtualCard = member.GetLoyaltyCardByType(Brierley.FrameWork.Common.VirtualCardSearchType.MostRecentIssued);
            }

            return virtualCard;
        }
        public static String GetMemberState(MemberDetails memberDetails)
        {
            String strRetValue = String.Empty;
            try
            {
                strRetValue = memberDetails.StateOrProvince;
            }
            catch (Exception) { }
            return strRetValue;
        }

        #endregion Account Details

        #region Promotional Methods

        /// <summary>
        /// Initialize all bra attribute sets
        /// </summary>
        private static void LoadBraAttributeSets(Member member)
        {
            bool isDeleted = false;
            bool isFulfilled = false;

            lstMemberBraPromoSummary = new List<MemberBraPromoSummary>();
            IList<IClientDataObject> objList = member.GetChildAttributeSets("MemberBraPromoSummary");
            foreach (IClientDataObject obj in objList)
            {
                MemberBraPromoSummary promoSummary = (MemberBraPromoSummary)obj;
                lstMemberBraPromoSummary.Add(promoSummary);
            }

            lstMemberBraPromoCertSummary = new List<MemberBraPromoCertSummary>();
            objList = member.GetChildAttributeSets("MemberBraPromoCertSummary");
            foreach (IClientDataObject obj in objList)
            {
                MemberBraPromoCertSummary promoCertSummary = (MemberBraPromoCertSummary)obj;
                lstMemberBraPromoCertSummary.Add(promoCertSummary);
            }

            lstMemberBraPromoCertHistory = new List<MemberBraPromoCertHistory>();
            objList = member.GetChildAttributeSets("MemberBraPromoCertHistory");
            foreach (IClientDataObject obj in objList)
            {
                //Take care of removing the history that is redeemed or deleted here so we don't have to worry about it other places
                MemberBraPromoCertHistory certHistory = (MemberBraPromoCertHistory)obj;
                if (certHistory.IsFulfilled != null)
                {
                    isFulfilled = (bool)certHistory.IsFulfilled;
                }
                if (certHistory.IsDeleted != null)
                {
                    isDeleted = (bool)certHistory.IsDeleted;
                }
                if ((!isDeleted) && (!isFulfilled))
                {
                    lstMemberBraPromoCertHistory.Add(certHistory);
                }
            }

            lstMemberBraPromoCertRedeem = new List<MemberBraPromoCertRedeem>();
            objList = member.GetChildAttributeSets("MemberBraPromoCertRedeem");
            foreach (IClientDataObject obj in objList)
            {
                MemberBraPromoCertRedeem promoCertRedeem = (MemberBraPromoCertRedeem)obj;
                lstMemberBraPromoCertRedeem.Add(promoCertRedeem);
            }

        }
        /// <summary>
        /// Returns current qualifying bra purchased
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentQualifyingBrasPurchased(Member member)
        {
            String StrRetVal = "0";
            long currentBalance = 0;

            /*
             * member promo history table
             * member promo summary
             * 
             * take currentbalance + # of hist records where isdeleted = 0 and isfulfilled = 0
           */
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                if (null != member)
                {
                    CheckForAttributeSets(member);

                    if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
                    {
                        MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];
                        currentBalance = (long)memberSummary.CurrentBalance;
                    }
                    if ((null != lstMemberBraPromoCertHistory) && (lstMemberBraPromoCertHistory.Count > 0))
                    {
                        //We take care of removing the history that is redeemed or deleted in the LoadBraAttributeSets so we don't have to worry about it here
                        foreach (MemberBraPromoCertHistory certHistory in lstMemberBraPromoCertHistory)
                        {
                            currentBalance += (long)certHistory.Threshold;
                        }
                    }

                    StrRetVal = Convert.ToString(currentBalance);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return StrRetVal;
        }

        private static void CheckForAttributeSets(Member member)
        {
            if ((lstMemberBraPromoSummary == null) || (lstMemberBraPromoSummary.Count == 0))
            {
                LoadBraAttributeSets(member);
            }
            else
            {
                if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
                {
                    MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];
                    if (member.IpCode != memberSummary.IpCode)
                    {
                        LoadBraAttributeSets(member);
                    }
                }
            }
        }

        /// <summary>
        /// Returns total free bra mailed in the current year
        /// </summary>
        /// <param name="pblnYear">true: Current year/false: lifetime</param>
        /// <returns></returns>
        public static string GetTotalFreeBrasMailed(Member member, Boolean pblnYear)
        {
            String strRetVal = "0";
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {

                //Evaluaate the last date of previous year
                String strLastDateOfPrevousYear = "12/31/" + (DateTime.Now.Year - 1).ToString();
                DateTime dtLastDateOfPrevousYear = DateTime.Now;
                DateTime.TryParse(strLastDateOfPrevousYear, out dtLastDateOfPrevousYear);
                long lngNumCertsAwarded = 0;
                if (null != member)
                {
                    CheckForAttributeSets(member);

                    if ((null != lstMemberBraPromoCertSummary) && (lstMemberBraPromoCertSummary.Count > 0))
                    {
                        foreach (MemberBraPromoCertSummary certSummary in lstMemberBraPromoCertSummary)
                        {
                            //true for last year and false for lifetime
                            if (pblnYear)
                            {
                                if (certSummary.FulfillmentPeriod > dtLastDateOfPrevousYear)
                                {
                                    lngNumCertsAwarded += (long)certSummary.NumCertsAwarded;
                                }
                            }
                            else
                            {
                                lngNumCertsAwarded += (long)certSummary.NumCertsAwarded;
                            }
                        }
                    }
                    strRetVal = lngNumCertsAwarded.ToString();
                }

            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetVal;
        }

        /// <summary>
        /// Get last free bra mailed to _member
        /// </summary>
        /// <returns></returns>
        public static string GetLastFreeBraMailed(Member member)
        {
            String strRetVal = String.Empty;
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                if (null != member)
                {
                    CheckForAttributeSets(member);

                    if ((null != lstMemberBraPromoCertSummary) && (lstMemberBraPromoCertSummary.Count > 0))
                    {
                        //evaluate last process date
                        DateTime? dtLastFreeBraMailed = lstMemberBraPromoCertSummary.Max(MemberBraPromoCertSummary => MemberBraPromoCertSummary.ProcessDate);
                        strRetVal = Convert.ToString(dtLastFreeBraMailed.Value.ToString("M/d/yyyy"));
                    }
                }
                //IList<IClientDataObject> lstmbrBraPromoCertSummary = member.GetChildAttributeSets("MemberBraPromoCertSummary");
                ////MemberBraPromoCertSummary certSummary = (MemberBraPromoCertSummary)lstmbrBraPromoCertSummary[0];

                ////evaluate last process date
                //DateTime? dtLastFreeBraMailed = lstmbrBraPromoCertSummary.Max(MemberBraPromoCertSummary => MemberBraPromoCertSummary.ProcessDate);
                //strRetVal = Convert.ToString(dtLastFreeBraMailed.Value.ToString("M/d/yyyy"));
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetVal;
        }

        /// <summary>
        /// Get last free bra in home date
        /// </summary>
        /// <returns></returns>
        public static string GetLastFreeBraInHomeDate(Member member)
        {
            String strRetVal = String.Empty;
            try
            {
                CheckForAttributeSets(member);

                if ((null != lstMemberBraPromoCertSummary) && (lstMemberBraPromoCertSummary.Count > 0))
                {
                    //evaluate last process date
                    DateTime? dtLastFreeBraInHomeDate = lstMemberBraPromoCertSummary.Max(MemberBraPromoCertSummary => MemberBraPromoCertSummary.InHomeDate);
                    strRetVal = Convert.ToString(dtLastFreeBraInHomeDate.Value.ToString("M/d/yyyy"));
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetVal;
        }

        /// <summary>
        /// Get last free bra redeemed date
        /// </summary>
        /// <returns></returns>
        public static string GetLastFreeBraRedeemed(Member member)
        {
            String strRetVal = String.Empty;
            try
            {
                CheckForAttributeSets(member);

                if ((null != lstMemberBraPromoCertRedeem) && (lstMemberBraPromoCertRedeem.Count > 0))
                {
                    //evaluate last redeemed date
                    DateTime? dtLastFreeBraRedeemed = lstMemberBraPromoCertRedeem.Max(MemberBraPromoCertSummary => MemberBraPromoCertSummary.RedemptionDate);
                    strRetVal = Convert.ToString(dtLastFreeBraRedeemed.Value.ToString("M/d/yyyy"));
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetVal;
        }

        /// <summary>
        /// Get total free bras redeemed
        /// </summary>
        /// <returns></returns>
        public static string GetTotalFreeBrasRedeemed(Member member)
        {
            String strRetVal = "0";
            try
            {
                CheckForAttributeSets(member);

                if ((null != lstMemberBraPromoCertRedeem) && (lstMemberBraPromoCertRedeem.Count > 0))
                {
                    //sum up all the redemption points
                    long? lngTotalFreeBrasRedeemed = lstMemberBraPromoCertRedeem.Sum(MemberBraPromoCertSummary => MemberBraPromoCertSummary.RedemptionAmount);
                    strRetVal = Convert.ToString(lngTotalFreeBrasRedeemed.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetVal;
        }

        /// Returns primary or secondary fullfillment dates
        /// </summary>
        /// <param name="p">true:Primary/false:secondary</param>
        /// <returns></returns>
        public static string GetExpectedFullFillmentDate(Member member, bool pblnFullfillment)
        {
            String StrRetVal = String.Empty;
            try
            {


                if (null != member)
                {
                    CheckForAttributeSets(member);

                    if ((null != lstMemberBraPromoCertHistory) && (lstMemberBraPromoCertHistory.Count > 0))
                    {
                        DateTime dtPrimaryFullfillmentDate = DateTime.Now;
                        DateTime dtSecondaryFullfillmentDate = DateTime.Now;
                        int primaryCount = 0;
                        int secondaryCount = 0;

                        dtPrimaryFullfillmentDate = (DateTime)lstMemberBraPromoCertHistory.Max(MemberBraPromoCertHistory => MemberBraPromoCertHistory.FulfillmentDate);
                        primaryCount = (int)lstMemberBraPromoCertHistory.Count(certHist => certHist.FulfillmentDate == dtPrimaryFullfillmentDate);

                        //We take care of removing the history that is redeemed or deleted in the LoadBraAttributeSets so we don't have to worry about it here
                        foreach (MemberBraPromoCertHistory mbrBraPromoCertSummary in lstMemberBraPromoCertHistory)
                        {
                            if (mbrBraPromoCertSummary.FulfillmentDate.HasValue)
                            {
                                if (mbrBraPromoCertSummary.FulfillmentDate != dtPrimaryFullfillmentDate)
                                {
                                    ++secondaryCount;
                                    if (mbrBraPromoCertSummary.FulfillmentDate >= dtSecondaryFullfillmentDate)
                                    {
                                        dtSecondaryFullfillmentDate = (DateTime)mbrBraPromoCertSummary.FulfillmentDate;
                                    }
                                }
                            }
                        }
                        //true for primary and false for secondary
                        if (pblnFullfillment)
                        {
                            StrRetVal = string.Format("{0} for {1}", primaryCount, dtPrimaryFullfillmentDate.ToShortDateString());
                        }
                        else
                        {
                            if (secondaryCount > 0)
                            {
                                StrRetVal = string.Format("{0} for {1}", secondaryCount, dtSecondaryFullfillmentDate.ToShortDateString());
                            }
                        }
                    }
                }

                //Create criteria to get MemberBraPromoCertHistory
                //of a member and fullfillment date should be future date
                //LWCriterion crit = new LWCriterion("MemberBraPromoCertHistory");
                //crit.Add(LWCriterion.OperatorType.AND, "IPCODE", _member.IpCode, LWCriterion.Predicate.Eq);
                //crit.Add(LWCriterion.OperatorType.AND, "FulfillmentDate", DateTime.Now, LWCriterion.Predicate.Ge);
                //IList<IClientDataObject> memSummary = _dataService.GetAttributeSetObjects(null, "MemberBraPromoCertHistory", crit, new LWQueryBatchInfo(), false);

                //if (null != _member)
                //{
                //    IList<MemberBraPromoCertHistory> memSummary = _member.GetChildAttributeSets("MemberBraPromoCertHistory") as List<MemberBraPromoCertHistory>;

                //    if (null != memSummary)
                //    {
                //        DateTime? dtPrimaryFullfillmentDate = null;
                //        DateTime? dtSecondaryFullfillmentDate = null;
                //        //evaluating primary and secondary fullfillment dates
                //        foreach (MemberBraPromoCertHistory mbrBraPromoCertSummary in memSummary)
                //        {
                //            if (mbrBraPromoCertSummary.FulfillmentDate.HasValue)
                //            {
                //                if (mbrBraPromoCertSummary.FulfillmentDate > dtPrimaryFullfillmentDate)
                //                {
                //                    dtPrimaryFullfillmentDate = mbrBraPromoCertSummary.FulfillmentDate;
                //                }
                //                else
                //                {
                //                    dtSecondaryFullfillmentDate = dtPrimaryFullfillmentDate;
                //                }
                //            }
                //        }
                //        //true for primary and false for secondary
                //        if (pblnFullfillment)
                //        {
                //            StrRetVal = Convert.ToString(dtPrimaryFullfillmentDate.Value.ToString("M/d/yyyy"));
                //        }
                //        else
                //        {
                //            StrRetVal = Convert.ToString(dtSecondaryFullfillmentDate.Value.ToString("M/d/yyyy"));
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return StrRetVal;
        }

        /// <summary>
        /// Returns free bra earned
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFreeBrasEarned(Member member)
        {
            String StrRetVal = "0";
            long currentBalance = 0;

            /*
             * count of ats_memberbrapromocerthistory where isfulfilled = 0 and isdeleted = 0
             * 
            */
            try
            {
                using (var dwService = _dataUtil.DataServiceInstance())
                {
                    ClientConfiguration objClientConfiguration = dwService.GetClientConfiguration("BraPromoFulFillmentThreshold");
                    if (null != member)
                    {
                        CheckForAttributeSets(member);

                        if ((null != lstMemberBraPromoCertHistory) && (lstMemberBraPromoCertHistory.Count > 0))
                        {
                            //We take care of removing the history that is redeemed or deleted in the LoadBraAttributeSets so we don't have to worry about it here
                            foreach (MemberBraPromoCertHistory certHistory in lstMemberBraPromoCertHistory)
                            {
                                ++currentBalance;
                            }
                        }
                        StrRetVal = currentBalance.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return StrRetVal;
        }

        /// <summary>
        /// Get life time count for Bras
        /// </summary>
        /// <returns></returns>
        public static string GetBraLifetimeBalance(Member member)
        {
            String strRetVal = String.Empty;
            try
            {
                CheckForAttributeSets(member);

                if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
                {
                    MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];

                    //if (memberSummary.BraLifeTimeBalance != null)
                    //{
                    //    strRetVal = Convert.ToString(memberSummary.BraLifeTimeBalance);
                    //}
                    //else
                    //{
                    //    // default the value if it is null
                    //    strRetVal = "0";
                    //}
                }
            }
            catch (Exception ex)
            {
                // default the value incase of an error
                strRetVal = "0";
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetVal;
        }

        /// <summary>
        /// Get the rolling balance for Bras 
        /// </summary>
        /// <returns></returns>
        public static string GetBraRollingBalance(Member member)
        {
            String strRetVal = String.Empty;
            try
            {
                CheckForAttributeSets(member);

                if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
                {
                    MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];

                    //if (memberSummary.BraRollingBalance != null)
                    //{
                    //    strRetVal = Convert.ToString(memberSummary.BraRollingBalance);
                    //}
                    //else
                    //{
                    //    // default the value if it is null
                    //    strRetVal = "0";
                    //}
                }
            }
            catch (Exception ex)
            {
                // default the value incase of an error
                strRetVal = "0";
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetVal;
        }

        /// <summary>
        /// Get the first purchase date for Bras
        /// </summary>
        /// <returns></returns>
        //public static string GetBraFirstPurchaseDate(Member member)
        //{
        //    String strRetVal = String.Empty;
        //    try
        //    {
        //        CheckForAttributeSets(member);

        //        if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
        //        {
        //            MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];

        //            if (memberSummary.BraFirstPurchaseDate != null)
        //            {
        //                strRetVal = Convert.ToString(memberSummary.BraFirstPurchaseDate.Value.ToString("M/d/yyyy"));
        //            }
        //            else
        //            {
        //                // default the value if it is null
        //                strRetVal = "1/1/1900";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // default the incase of an error
        //        strRetVal = "1/1/1900";
        //        logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
        //    }
        //    return strRetVal;
        //}

        /// <summary>
        /// Get life time count for Jeans
        /// </summary>
        /// <returns></returns>
        public static string GetJeansLifetimeBalance(Member member)
        {
            String strRetVal = String.Empty;
            try
            {
                CheckForAttributeSets(member);

                if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
                {
                    MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];

                    //if (memberSummary.JeansLifeTimeBalance != null)
                    //{
                    //    strRetVal = Convert.ToString(memberSummary.JeansLifeTimeBalance);
                    //}
                    //else
                    //{
                    //    // default the value if it is null
                    //    strRetVal = "0";
                    //}
                }
            }
            catch (Exception ex)
            {
                // default the value incase of an error
                strRetVal = "0";
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetVal;
        }

        /// <summary>
        /// Get the rolling balance for Jeans 
        /// </summary>
        /// <returns></returns>
        public static string GetJeansRollingBalance(Member member)
        {
            String strRetVal = String.Empty;
            try
            {
                CheckForAttributeSets(member);

                if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
                {
                    MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];

                    //if (memberSummary.JeansRollingBalance != null)
                    //{
                    //    strRetVal = Convert.ToString(memberSummary.JeansRollingBalance);
                    //}
                    //else
                    //{
                    //    // default the value if it is null
                    //    strRetVal = "0";
                    //}
                }
            }
            catch (Exception ex)
            {
                // default the value incase of an error
                strRetVal = "0";
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetVal;
        }

        /// <summary>
        /// Get the first purchase date for Bras
        /// </summary>
        /// <returns></returns>
        public static string GetJeansFirstPurchaseDate(Member member)
        {
            String strRetVal = String.Empty;
            try
            {
                CheckForAttributeSets(member);

                if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
                {
                    MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];

                    //if (memberSummary.JeansFirstPurchaseDate != null)
                    //{
                    //    strRetVal = Convert.ToString(memberSummary.JeansFirstPurchaseDate.Value.ToString("M/d/yyyy"));
                    //}
                    //else
                    //{
                    //    // default the value if it is null
                    //    strRetVal = "1/1/1900";
                    //}
                }
            }
            catch (Exception ex)
            {
                // default the incase of an error
                strRetVal = "1/1/1900";
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetVal;
        }

        #endregion Promotional methods

        #region New Promotional Methods

        /// <summary>
        /// Initialize all bra attribute sets
        /// </summary>
        public static void LoadB5G1Rewards(Member member, string promoType)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: Member: " + member.IpCode.ToString());

            try
            {
                using (var cService = _dataUtil.ContentServiceInstance())
                {
                    switch (promoType.ToUpper())
                    {
                        case "BRA":
                            reward_1 = cService.GetRewardDef("Bra Reward-1");
                            reward_15 = cService.GetRewardDef("Bra Reward-15");
                            reward_bra = cService.GetRewardDef("B5G1 Bra Reward"); //AEO-Redesign-2015 Start & End (JRA-240)

                            break;
                        case "JEAN":
                            reward_Jean = cService.GetRewardDef("B5G1 Jean Reward");
                            break;
                    }
                }

                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    IList<MemberReward> memberRewards = lwService.GetMemberRewards(member, null);
                    mbrRewards = new List<MemberReward>();


                    // AEO-Redesign-2015 BEGIN (JRA-240)
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Rewards: " + memberRewards.Count());
                    if (promoType == "BRA")
                    {


                        foreach (MemberReward mr in memberRewards)
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "mr " + mr.Id.ToString());
                            if ((mr.RewardDefId == reward_1.Id) || (mr.RewardDefId == reward_15.Id) || (mr.RewardDefId == reward_bra.Id))
                            {
                                mbrRewards.Add(mr);
                                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward Added: " + mr.Id.ToString());
                            }
                        }
                    }

                    if (promoType == "JEAN")
                    {
                        foreach (MemberReward mr in memberRewards)
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "mr: " + mr.Id.ToString());
                            if ((mr.RewardDefId == reward_Jean.Id))
                            {
                                mbrRewards.Add(mr);
                                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward Added: " + mr.Id.ToString());
                            }
                        }

                    }

                    // AEO-Redesign-2015 END (JRA-240)

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End - Number of rewards: " + mbrRewards.Count.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }


        }
        /// <summary>
        /// Returns current qualifying bra purchased
        /// </summary>
        /// <returns></returns>
        public static string GetB5G1CurrentQualifyingPurchased(Member member, DateTime endDate, string promoType, bool holdPoints, DateTime? OptionalStartDate = null, DateTime? OptionalEndDate = null)
        {
            //Default return value would be 0
            String strRetValue = "0";
            DateTime startDate = DateTime.Parse("1/1/" + DateTime.Today.Year.ToString());

            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {

                long[] loyaltyids = member.GetLoyaltyCardIds();

                if (loyaltyids == null || loyaltyids.Length == 0)
                {
                    return strRetValue;
                }

                IList<PointEvent> events = lwService.GetAllPointEvents();

                IList<VirtualCard> cards = new List<VirtualCard>();
                for (int i = 0; i < loyaltyids.Length; i++)
                {
                    cards.Add(member.GetLoyaltyCard(loyaltyids[i]));
                }



                // AEO-604 end

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin - StartDate: " + startDate.ToShortDateString());
                try
                {
                    int index = 0;
                    IList<PointType> pointTypes = lwService.GetAllPointTypes();
                    long[] pointTypeIDs = new long[pointTypes.Count];

                    IList<IClientDataObject> atrList = member.GetChildAttributeSets("MemberDetails", true);
                    MemberDetails md = (MemberDetails)atrList[0];

                    if (Utilities.isInPilot(md.ExtendedPlayCode))
                    {
                        startDate = DateTime.Compare(startDate, new DateTime(1990, 1, 1)) > 0 ? new DateTime(1990, 1, 1) : startDate;
                        endDate = DateTime.Compare(endDate.Date, DateTime.Now.Date) > 0 ? endDate : DateTime.Now.Date;
                    }



                    // AEO-431 Begin

                    if (promoType.Trim().ToUpper().Equals("JEAN"))
                    {

                        if (atrList != null && atrList.Count > 0)
                        {

                            if (md != null && Utilities.isInPilot(md.ExtendedPlayCode))
                            {
                                // this a pilot and JEAN so we have to change the startdate parameter value 
                                using (var dService = _dataUtil.DataServiceInstance())
                                {
                                    ClientConfiguration lsJeanStartDate = dService.GetClientConfiguration("JeansProgramBeginDate");
                                    if (lsJeanStartDate != null && lsJeanStartDate.Value != null)
                                    {
                                        DateTime tmpdate = new DateTime();
                                        if (DateTime.TryParse(lsJeanStartDate.Value, out tmpdate))
                                        {
                                            // just to be sure that the start date will not be greater than enddate
                                            startDate = DateTime.Compare(tmpdate, endDate) >= 0 ? startDate : tmpdate;
                                        }
                                    }
                                }
                            }
                        }

                    }

                    // AEO-431 end

                    //RKG - 2/5/2013
                    //Removed Bra Adjustment Points as per PI 22636

                    IList<PointType> pointTypesObj = new List<PointType>();

                    foreach (PointType pt in pointTypes)
                    {
                        if ((pt.Name.ToUpper() == promoType + " POINTS") || (pt.Name.ToUpper() == "B5G1 EMPLOYEE POINTS"))
                        {
                            pointTypeIDs[index] = pt.ID;
                            ++index;
                            pointTypesObj.Add(pt);
                        }
                    }

                    decimal balance = decimal.Zero;
                    //AEO-1898 BEGIN (B5G1 Bra/Jean data monthly called by member account page)
                    if (OptionalStartDate != null)
                    {
                        startDate = OptionalStartDate ?? startDate;
                    }
                    if (OptionalEndDate != null)
                    {
                        endDate = OptionalEndDate ?? endDate;
                    }
                    //AEO-1898 END

                    if (holdPoints)
                    {
                        balance = lwService.GetPointsOnHold(cards, pointTypesObj, events, startDate, endDate);

                    }
                    else
                    {
                        //It should consider the points on hold when getting the balance.
                        balance = member.GetPoints(pointTypeIDs, startDate, endDate) - lwService.GetPointsOnHold(cards, pointTypesObj, events, startDate, endDate);
                    }


                    if (balance <= 0)
                    {
                        return strRetValue;
                    }
                    // strRetValue = Convert.ToString(member.GetPoints(pointTypeIDs, startDate, endDate)); AEO-431
                    strRetValue = Convert.ToString(balance);

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Points: " + strRetValue);
                }
                catch (Exception) { }
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetValue;
        }


        // AEO Redesign 2015 Begin

        /// <summary>
        /// Returns current qualifying jean purchased
        /// </summary>
        /// <returns></returns>
        public static string GetJeansCurrentQualifyingPurchased(Member member, DateTime endDate)
        {
            //Default return value would be 0
            String strRetValue = "0";
            DateTime startDate = DateTime.Parse("1/1/" + DateTime.Today.Year.ToString());

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin - StartDate: " + startDate.ToShortDateString());
            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    int index = 0;
                    IList<PointType> pointTypes = lwService.GetAllPointTypes();
                    long[] pointTypeIDs = new long[pointTypes.Count];


                    foreach (PointType pt in pointTypes)
                    {
                        if ((pt.Name.ToUpper() == "JEAN POINTS"))
                        {
                            pointTypeIDs[index] = pt.ID;
                            ++index;
                        }
                    }

                    decimal balance = member.GetPoints(pointTypeIDs, startDate, endDate);
                    if (balance <= 0)
                    {
                        return strRetValue;
                    }
                    strRetValue = Convert.ToString(member.GetPoints(pointTypeIDs, startDate, endDate));

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Points: " + strRetValue);
                }
            }
            catch (Exception) { }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetValue;

        }
        // AEO Redesign 2015 End






        private static void CheckForRewards(Member member, string promoType)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: ");
            if ((mbrRewards == null) || (mbrRewards.Count == 0))
            {
                LoadB5G1Rewards(member, promoType);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Returns total free bra mailed in the current year
        /// </summary>
        /// <param name="pblnYear">true: Current year/false: lifetime</param>
        /// <returns></returns>
        public static string GetB5G1TotalFreeMailed(Member member, Boolean pblnYear, string promoType)
        {
            String strRetVal = "0";
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {

                //Evaluaate the last date of previous year
                String strLastDateOfPrevousYear = "12/31/" + (DateTime.Now.Year - 1).ToString();
                DateTime dtLastDateOfPrevousYear = DateTime.Now;
                DateTime.TryParse(strLastDateOfPrevousYear, out dtLastDateOfPrevousYear);
                long lngNumCertsAwarded = 0;
                if (null != member)
                {

                    CheckForRewards(member, promoType);
                    using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        mbrRewards = lwService.GetMemberRewards(member, null);
                    }

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "pblnYear: " + pblnYear.ToString());
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "dtLastDateOfPrevousYear: " + dtLastDateOfPrevousYear.ToShortDateString());
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Number of MemberRewards: " + mbrRewards.Count.ToString());
                    foreach (MemberReward mbrReward in mbrRewards)
                    {
                        if (promoType == "BRA")
                        {
                            if ((mbrReward.RewardDefId == reward_1.Id) || (mbrReward.RewardDefId == reward_15.Id) || (mbrReward.RewardDefId == reward_bra.Id)) //EDO REdesign 2015, (JRA-240) 
                            {
                                if (pblnYear)
                                {
                                    if (mbrReward.DateIssued > dtLastDateOfPrevousYear)
                                    {
                                        ++lngNumCertsAwarded;
                                    }
                                }
                                else
                                {
                                    ++lngNumCertsAwarded;
                                }
                            }
                        }
                        else if (promoType == "JEAN")
                        {
                            if ((mbrReward.RewardDefId == reward_Jean.Id))
                            {
                                if (pblnYear)
                                {
                                    if (mbrReward.DateIssued > dtLastDateOfPrevousYear)
                                    {
                                        ++lngNumCertsAwarded;
                                    }
                                }
                                else
                                {
                                    ++lngNumCertsAwarded;
                                }
                            }
                        }
                    }

                    strRetVal = lngNumCertsAwarded.ToString();
                }

            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetVal;
        }

        /// <summary>
        /// Get last free bra mailed to _member
        /// </summary>
        /// <returns></returns>
        public static string GetB5G1LastFreeMailed(Member member, string promoType)
        {
            String strRetVal = String.Empty;
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                if (null != member)
                {
                    CheckForRewards(member, promoType);

                    if ((null != mbrRewards) && (mbrRewards.Count > 0))
                    {
                        //evaluate last process date
                        DateTime? dtLastFreeBraMailed = mbrRewards.Max(MemberReward => MemberReward.DateIssued);
                        strRetVal = Convert.ToString(dtLastFreeBraMailed.Value.ToString("M/d/yyyy"));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetVal;
        }

        /// <summary>
        /// Get last free bra in home date
        /// </summary>
        /// <returns></returns>
        public static string GetB5G1LastFreeInHomeDate(Member member, string promoType)
        {
            String strRetVal = String.Empty;
            try
            {
                CheckForRewards(member, promoType);

                if ((null != mbrRewards) && (mbrRewards.Count > 0))
                {
                    //evaluate last process date
                    // PI 31291 - CS Portal - Change In-home Date, Start Changes
                    //DateTime dtLastFreeBraMailed = mbrRewards.Max(MemberReward => MemberReward.DateIssued).AddDays(14);
                    DateTime issueDate = mbrRewards.Max(MemberReward => MemberReward.DateIssued);
                    //AEO-77 changes - start

                    //if (issueDate.Day == 8)
                    //{
                    //    if (issueDate.Month == 2)
                    //    {
                    //        strRetVal = issueDate.AddDays(20).ToShortDateString();
                    //    }
                    //    else
                    //    {
                    //        strRetVal = issueDate.AddDays(22).ToShortDateString();
                    //    }
                    //}
                    //else if (issueDate.Day == 21)
                    //{
                    //    strRetVal = issueDate.AddMonths(1).AddDays(-6).ToShortDateString();
                    //}

                    var startOfMonth = new DateTime(issueDate.Year, issueDate.Month, 1);
                    var daysInMonth = DateTime.DaysInMonth(issueDate.Year, issueDate.Month);
                    var lastDay = new DateTime(issueDate.Year, issueDate.Month, daysInMonth);

                    if (issueDate.Day > 15)
                    {
                        strRetVal = lastDay.AddDays(15).ToShortDateString();
                    }

                    if (lastDay.Day == 31)
                    {
                        strRetVal = lastDay.AddDays(-1).ToShortDateString();
                    }
                    // AEO-77 changes - end

                    //strRetVal = dtLastFreeBraMailed.ToShortDateString();
                    // PI 31291 - CS Portal - Change In-home Date, End Changes
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetVal;
        }

        /// <summary>
        /// Get last free bra redeemed date
        /// </summary>
        /// <returns></returns>
        public static string GetB5G1LastFreeRedeemed(Member member, string promoType)
        {
            String strRetVal = String.Empty;
            string exclusionList = string.Empty;
            int startIndex = 1;
            int batchSize = 1000;
            DateTime startDate = DateTime.Parse("1/1/2009");
            DateTime endDate = DateTime.Now;

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    // AEO-673 Begin
                    String pointKey = string.Empty;

                    if (promoType.ToUpper() == "JEAN")
                    {
                        pointKey = "Jean Redemptions";
                    }
                    else if (promoType.ToUpper() == "BRA")
                    {
                        pointKey = "Bra Redemptions";
                    }
                    PointType pointType = lwService.GetPointType(pointKey);
                    // AEO-673 End

                    ///commented for disable AEO-673... PointType pointType = LWDataServiceUtil.DataServiceInstance(true).GetPointType(promoType + " Redemptions");

                    // AEO-74 Upgrade 4.5 changes here -----------SCJ
                    //IList<PointTransaction> txns = LWDataServiceUtil.DataServiceInstance(true).GetPointTransactions(member, startDate, endDate, exclusionList, startIndex, batchSize, true);
                    IList<PointTransaction> txns = lwService.GetPointTransactions(member, startDate, endDate, null, exclusionList, startIndex, batchSize, true);
                    IList<PointTransaction> pointTxns = new List<PointTransaction>();

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "txns count: " + txns.Count.ToString());
                    foreach (PointTransaction txn in txns)
                    {
                        if (pointType != null && txn.PointTypeId == pointType.ID)    // AEO-673 Begin & End
                                                                                     ///commented for disable AEO-673... if (txn.PointTypeId == pointType.ID)
                        {
                            pointTxns.Add(txn);
                        }
                    }
                    if ((null != pointTxns) && (pointTxns.Count > 0))
                    {
                        //evaluate last redeemed date
                        DateTime dtLastFreeBraRedeemed = pointTxns.Max(PointTransaction => PointTransaction.TransactionDate);
                        strRetVal = dtLastFreeBraRedeemed.ToShortDateString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetVal;
        }

        /// <summary>
        /// Get total free bras redeemed
        /// </summary>
        /// <returns></returns>
        public static string GetB5G1TotalFreeRedeemed(Member member, string promoType)
        {
            //Default return value would be 0
            String strRetValue = "0";
            DateTime startDate = DateTime.Today.AddYears(-3);
            DateTime endDate = DateTime.Now;


            //At the time of this writing Redemptions are both expired and not expired so we have to get the day before the current quarter as the end date for the first query to get
            //all the previous points that are expired.  Then set it to the end of the quarter to get the rest.

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetBraTotalFreeRedeemed : Begin - StartDate: " + startDate.ToShortDateString());
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "EndDate: " + endDate.ToShortDateString());
            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    int index = 0;
                    IList<PointType> pointTypes = lwService.GetAllPointTypes();
                    long[] pointTypeIDs = new long[pointTypes.Count];

                    foreach (PointType pt in pointTypes)
                    {
                        if (pt.Name.ToUpper() == promoType + " REDEMPTIONS")
                        {
                            pointTypeIDs[index] = pt.ID;
                            ++index;
                        }
                    }
                    //strRetValue = Convert.ToString(member.GetPoints(pointTypeIDs, startDate, endDate));
                    //double oldPoints = Utilities.GetPointsBalance(member, startDate, endDate, pointTypeIDs); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                    decimal oldPoints = Utilities.GetPointsBalance(member, startDate, endDate, pointTypeIDs); // AEO-74 Upgrade 4.5 changes here -----------SCJ

                    startDate = new DateTime(DateTime.Today.Year, 1, 1);
                    //double newPoints = Utilities.GetPointsBalance(member, startDate, endDate, pointTypeIDs);
                    //double newPoints = member.GetPoints(pointTypeIDs, startDate, endDate); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                    decimal newPoints = member.GetPoints(pointTypeIDs, startDate, endDate); // AEO-74 Upgrade 4.5 changes here -----------SCJ

                    strRetValue = (oldPoints + newPoints).ToString();

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetBraTotalFreeRedeemed : Points: " + strRetValue);
                }
            }
            catch (Exception) { }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "GetBraTotalFreeRedeemed : End");
            return strRetValue;
        }

        /// Returns primary or secondary fullfillment dates
        /// </summary>
        /// <param name="p">true:Primary/false:secondary</param>
        /// <returns></returns>
        public static string GetB5G1ExpectedFullFillmentDate(Member member, bool isPrimary, DateTime endDate, int braPromoFulFillmentThreshold, bool isPilot, string promoType)
        {
            //Default return value would be 0
            String strRetValue = string.Empty;
            //double primaryPointBalance = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            //double secondaryPointBalance = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            decimal primaryPointBalance = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            decimal secondaryPointBalance = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            DateTime processDate = DateTime.Today;
            DateTime startDate = DateTime.Parse("1/1/" + DateTime.Today.Year.ToString());
            DateTime primaryFulfillmentDate = DateTime.MinValue;
            DateTime primaryStartDate = DateTime.MinValue;
            DateTime primaryEndDate = DateTime.MinValue;
            DateTime secondaryFulfillmentDate = DateTime.MinValue;
            DateTime secondaryStartDate = DateTime.MinValue;
            DateTime secondaryEndDate = DateTime.MinValue;
            int currentDay = 0;
            int primaryCount = 0;
            int secondaryCount = 0;
            int remainder = 0;
            //double newcurrentBalance = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            decimal newcurrentBalance = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ

            try
            {


                if (null != member)
                {

                    IList<IClientDataObject> obj = member.GetChildAttributeSets("MemberDetails");
                    MemberDetails mbrDetails = (MemberDetails)obj[0];

                    Boolean _blnAddressUnmailable = false;
                    if (mbrDetails.AddressMailable.HasValue)
                    {
                        _blnAddressUnmailable = mbrDetails.AddressMailable.Value;
                    }

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin - StartDate: " + startDate.ToShortDateString());
                    if (!_blnAddressUnmailable)
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Address Unmailable: ");
                        return strRetValue;
                    }

                    if (null != ConfigurationManager.AppSettings["ProcessDate"])
                    {
                        string strProcessDate = ConfigurationManager.AppSettings["ProcessDate"];
                        DateTime.TryParse(strProcessDate, out processDate);
                    }
                    currentDay = processDate.Day;

                    GetB5G1NextFulfillmentDates(out primaryStartDate, out primaryEndDate, out secondaryStartDate, out secondaryEndDate, out primaryFulfillmentDate, out secondaryFulfillmentDate, isPilot);

                    GetB5G1ExpectedPointBalances(member, ref primaryPointBalance, ref secondaryPointBalance, primaryStartDate, primaryEndDate, secondaryStartDate, secondaryEndDate, promoType);

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "primaryPointBalance: " + primaryPointBalance.ToString());
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "secondaryPointBalance: " + secondaryPointBalance.ToString());
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "CurrentDay: " + currentDay.ToString());
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "isPrimary: " + isPrimary.ToString());

                    //add primary and secondary if primary is less then 5
                    if (primaryPointBalance < braPromoFulFillmentThreshold)
                    {
                        secondaryPointBalance += primaryPointBalance;
                    }

                    if (secondaryPointBalance > 0)
                    {
                        //newcurrentBalance = secondaryPointBalance / braPromoFulFillmentThreshold;

                        //if (newcurrentBalance >= 1)
                        //{
                        //    secondaryCount = Convert.ToInt32(newcurrentBalance);
                        //}
                        newcurrentBalance = secondaryPointBalance / braPromoFulFillmentThreshold;
                        remainder = Convert.ToInt32(newcurrentBalance) % braPromoFulFillmentThreshold;

                        if (remainder == 0)
                        {
                            if (newcurrentBalance >= 1)
                            {
                                secondaryCount = Convert.ToInt32(newcurrentBalance);
                            }
                        }
                        else
                        {
                            secondaryCount = Convert.ToInt32(secondaryPointBalance) / braPromoFulFillmentThreshold;
                        }

                    }

                    if (primaryPointBalance > 0)
                    {
                        newcurrentBalance = primaryPointBalance / braPromoFulFillmentThreshold;
                        remainder = Convert.ToInt32(newcurrentBalance) % braPromoFulFillmentThreshold;

                        if (remainder == 0)
                        {
                            if (newcurrentBalance >= 1)
                            {
                                primaryCount = Convert.ToInt32(newcurrentBalance);
                            }
                        }
                        else
                        {
                            primaryCount = Convert.ToInt32(primaryPointBalance) / braPromoFulFillmentThreshold;
                        }
                    }

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "primaryCount: " + primaryCount.ToString());
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "dtPrimaryFullfillmentDate: " + primaryFulfillmentDate.ToShortDateString());
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "secondaryCount: " + secondaryCount.ToString());
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "dtSecondaryFullfillmentDate: " + secondaryFulfillmentDate.ToShortDateString());

                    //true for primary and false for secondary
                    if (isPrimary)
                    {
                        strRetValue = string.Format("{0} for {1}", primaryCount, primaryFulfillmentDate.ToShortDateString());
                    }
                    else
                    {
                        if (secondaryCount > 0)
                        {
                            strRetValue = string.Format("{0} for {1}", secondaryCount, secondaryFulfillmentDate.ToShortDateString());
                        }

                    }
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Points: " + strRetValue);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return strRetValue;
        }
        // AEO-74 Upgrade 4.5 changes here -----------SCJ
        //private static void GetBraExpectedPointBalances(Member member, ref double primaryPointBalance, ref double secondaryPointBalance, DateTime primaryStartDate, DateTime primaryEndDate, DateTime secondaryStartDate, DateTime secondaryEndDate)
        private static void GetB5G1ExpectedPointBalances(Member member, ref decimal primaryPointBalance, ref decimal secondaryPointBalance, DateTime primaryStartDate, DateTime primaryEndDate, DateTime secondaryStartDate, DateTime secondaryEndDate, string promoType)
        {
            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                IList<PointType> pointTypes = lwService.GetAllPointTypes();
                IList<PointEvent> pointEvents = lwService.GetAllPointEvents();
                IList<VirtualCard> virtualCards = member.LoyaltyCards;

                long[] pointTypeIDs = new long[pointTypes.Count];
                long[] pointEventIds = new long[pointEvents.Count];
                long[] vcKeys = new long[virtualCards.Count];
                int index = 0;

                foreach (VirtualCard vc in virtualCards)
                {
                    vcKeys[index] = vc.VcKey;
                    ++index;
                }

                index = 0;
                foreach (PointType pt in pointTypes)
                {
                    if (pt.Name.ToUpper() == promoType + " POINTS")
                    {
                        pointTypeIDs[index] = pt.ID;
                        ++index;
                    }
                }

                index = 0;
                foreach (PointEvent pt in pointEvents)
                {
                    //AEO- Redesign 2015. Validations
                    if (promoType == "BRA")
                    {
                        if ((pt.Name.ToUpper() == "B5G1 BRA RETURN") || (pt.Name.ToUpper() == "B5G1 BRA PURCHASE"))
                        {
                            pointEventIds[index] = pt.ID;
                            ++index;
                        }
                    }
                    else if (promoType == "JEAN")
                    {
                        if ((pt.Name.ToUpper() == "B5G1 JEAN RETURN") || (pt.Name.ToUpper() == "B5G1 JEAN PURCHASE"))
                        {
                            pointEventIds[index] = pt.ID;
                            ++index;
                        }
                    }
                }


                // AEO-552 Begin
                if (promoType.Trim().ToUpper() == "JEAN")
                {
                    ClientConfiguration lsJeanStartDate;
                    using (var dService = _dataUtil.DataServiceInstance())
                    {
                        lsJeanStartDate = dService.GetClientConfiguration("JeansProgramBeginDate");
                    }
                    if (lsJeanStartDate != null && lsJeanStartDate.Value != null)
                    {
                        DateTime tmpdate = new DateTime();
                        if (DateTime.TryParse(lsJeanStartDate.Value, out tmpdate))
                        {


                            DateTime currentDate = DateTime.Now.Date;

                            int nextDay = 0;
                            int previousMonth = 0;

                            //Calculate the previous month and year
                            if (currentDate.Month == 1)
                            {
                                previousMonth = 1;
                            }
                            else
                            {
                                previousMonth = currentDate.Month - 1;
                            }


                            DateTime nextDate = new DateTime(currentDate.Year, currentDate.Month, 1);

                            nextDay = nextDate.AddMonths(1).AddDays(-1).Day;

                            if (DateTime.Compare(currentDate, tmpdate) >= 0)
                            {

                                if (currentDate.Month == 10 && currentDate.Year == 2015)
                                {
                                    primaryStartDate = new DateTime(2015, 10, 1);
                                    primaryEndDate = new DateTime(2015, currentDate.Month, 15);

                                    secondaryStartDate = new DateTime(2015, 10, 16);
                                    secondaryEndDate = new DateTime(2015, 10, nextDay);
                                }
                                else
                                {

                                    if (currentDate.Day > 15)
                                    {
                                        //Next Primary Fulfillment date is the 1st of the next month and secondary is the 15th of the next month

                                        primaryStartDate = new DateTime(currentDate.Year, currentDate.Year == 2015 ? 10 : 1, 1);
                                        primaryEndDate = new DateTime(currentDate.Year, currentDate.Month, 15);

                                        secondaryStartDate = new DateTime(currentDate.Year, currentDate.Month, 16);
                                        secondaryEndDate = new DateTime(currentDate.Year, currentDate.Month, nextDay);
                                    }
                                    else
                                    {
                                        //Next Primary Fulfillment date is the 15th of the current month and secondary is the 1st of the next month

                                        primaryStartDate = new DateTime(currentDate.Year, currentDate.Year == 2015 ? 10 : 1, 1);
                                        primaryEndDate = new DateTime(currentDate.Year, previousMonth, 15);

                                        secondaryStartDate = new DateTime(currentDate.Year, previousMonth, 16);
                                        secondaryEndDate = new DateTime(currentDate.Year, currentDate.Month, nextDay);
                                    }

                                }


                            }
                            else
                            {

                                primaryPointBalance = 0;
                                secondaryPointBalance = 0;
                                return;
                            }
                        }
                    }
                }
                // AEO-552 End


                //Get the consumed point balance first because we need to get all consumed points before and after the hold date
                Brierley.FrameWork.Common.PointBankTransactionType[] txnTypes = new Brierley.FrameWork.Common.PointBankTransactionType[1];
                txnTypes[0] = Brierley.FrameWork.Common.PointBankTransactionType.Consumed;
                // AEO-74 Upgrade 4.5 changes here -----------SCJ
                //double consumedPointBalance = LWDataServiceUtil.DataServiceInstance(true).GetPointBalance(vcKeys, pointTypeIDs, pointEventIds, txnTypes, primaryStartDate, secondaryEndDate, null, null, string.Empty, null, null, null, null);
                decimal consumedPointBalance = lwService.GetPointBalance(vcKeys, pointTypeIDs, pointEventIds, txnTypes, primaryStartDate, secondaryEndDate, null, null, string.Empty, null, null, null, null);

                txnTypes = new Brierley.FrameWork.Common.PointBankTransactionType[2];
                txnTypes[0] = Brierley.FrameWork.Common.PointBankTransactionType.Credit;
                txnTypes[1] = Brierley.FrameWork.Common.PointBankTransactionType.Debit;

                primaryPointBalance = consumedPointBalance + lwService.GetPointBalance(vcKeys, pointTypeIDs, pointEventIds, txnTypes, primaryStartDate, primaryEndDate, null, null, string.Empty, null, null, null, null);
                secondaryPointBalance = lwService.GetPointBalance(vcKeys, pointTypeIDs, pointEventIds, txnTypes, secondaryStartDate, secondaryEndDate, null, null, string.Empty, null, null, null, null);


                if (primaryPointBalance < 0)
                {
                    primaryPointBalance = 0;
                }
                if (secondaryPointBalance < 0)
                {
                    secondaryPointBalance = 0;
                }
            }
        }

        private static void GetB5G1NextFulfillmentDates(out DateTime primaryStartDate, out DateTime primaryEndDate, out DateTime secondaryStartDate, out DateTime secondaryEndDate, out DateTime primaryFulfillmentDate, out DateTime secondaryFulfillmentDate, bool isPilot)
        {
            DateTime currentDate = DateTime.Today;
            int nextYear = 0;
            int nextMonth = 0;
            int nextDay = 0;
            int previousMonth = 0;
            int previousDay = 0;

            primaryStartDate = DateTime.MinValue;
            primaryEndDate = DateTime.MinValue;
            secondaryStartDate = DateTime.MaxValue;
            secondaryEndDate = DateTime.MinValue;
            primaryFulfillmentDate = DateTime.MinValue;
            secondaryFulfillmentDate = DateTime.MinValue;

            //Calculate the next month and year
            if (currentDate.Month == 12)
            {
                nextMonth = 1;
                nextYear = currentDate.Year + 1;
            }
            else
            {
                nextMonth = currentDate.Month + 1;
                nextYear = currentDate.Year;
            }

            //Calculate the previous month and year
            if (currentDate.Month == 1)
            {
                previousMonth = 1;
            }
            else
            {
                previousMonth = currentDate.Month - 1;
            }

            DateTime nextDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            nextDay = nextDate.AddMonths(1).AddDays(-1).Day;

            DateTime previousDate = new DateTime(currentDate.Year, previousMonth, 1);
            previousDay = previousDate.AddMonths(-1).AddDays(-1).Day;

            if (currentDate.Day > 15)
            {
                //Next Primary Fulfillment date is the 1st of the next month and secondary is the 15th of the next month
                primaryFulfillmentDate = new DateTime(nextYear, nextMonth, 1);
                primaryStartDate = new DateTime(currentDate.Year, 1, 1);
                primaryEndDate = new DateTime(currentDate.Year, currentDate.Month, 15);
                secondaryFulfillmentDate = new DateTime(nextYear, nextMonth, 15);
                secondaryStartDate = new DateTime(currentDate.Year, currentDate.Month, 16);
                secondaryEndDate = new DateTime(currentDate.Year, currentDate.Month, nextDay);
            }
            else
            {
                //Next Primary Fulfillment date is the 15th of the current month and secondary is the 1st of the next month
                primaryFulfillmentDate = new DateTime(currentDate.Year, currentDate.Month, 15);
                primaryStartDate = new DateTime(currentDate.Year, 1, 1);
                primaryEndDate = new DateTime(currentDate.Year, previousMonth, 15);
                secondaryFulfillmentDate = new DateTime(currentDate.Year, nextMonth, 1);
                secondaryStartDate = new DateTime(currentDate.Year, previousMonth, 16);
                secondaryEndDate = new DateTime(currentDate.Year, currentDate.Month, nextDay); // AEO-552
            }

            //Redesign AEO 2015
            if (isPilot)
            {
                //It returns the next tuesday
                primaryFulfillmentDate = GetNextWeekday(DateTime.Today, DayOfWeek.Tuesday);
                secondaryFulfillmentDate = GetNextWeekday(DateTime.Today, DayOfWeek.Tuesday);
            }

        }
        /// <summary>
        /// Returns free B5G1 earned
        /// </summary>
        /// <returns></returns>
        public static string GetB5G1CurrentFreeEarned(Member member, DateTime endDate, int braPromoFulFillmentThreshold, string promoType, DateTime? OptionalStartDate = null, DateTime? OptionalEndDate = null)
        {
            //Default return value would be 0
            String strRetValue = "0";
            //double currentBalance = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            decimal currentBalance = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            DateTime startDate = DateTime.Parse("1/1/" + DateTime.Today.Year.ToString());

            IList<IClientDataObject> obj = member.GetChildAttributeSets("MemberDetails");
            MemberDetails mbrDetails = (MemberDetails)obj[0];



            if (Utilities.isInPilot(mbrDetails.ExtendedPlayCode))
            {
                startDate = DateTime.Compare(startDate, new DateTime(1990, 1, 1)) > 0 ? new DateTime(1990, 1, 1) : startDate;
                endDate = DateTime.Compare(endDate.Date, DateTime.Now.Date) > 0 ? endDate : DateTime.Now.Date;
            }

            Boolean _blnAddressUnmailable = false;
            if (mbrDetails.AddressMailable.HasValue)
            {
                _blnAddressUnmailable = mbrDetails.AddressMailable.Value;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin - StartDate: " + startDate.ToShortDateString());
            if (!_blnAddressUnmailable)
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Address Unmailable: ");
                return strRetValue;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin - StartDate: " + startDate.ToShortDateString());
            try
            {
                int index = 0;
                IList<PointType> pointTypes;

                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    pointTypes = lwService.GetAllPointTypes();
                }
                long[] pointTypeIDs = new long[pointTypes.Count];

                foreach (PointType pt in pointTypes)
                {
                    if (pt.Name.ToUpper() == promoType + " POINTS")
                    {
                        pointTypeIDs[index] = pt.ID;
                        ++index;
                    }
                }

                // AEO-451 Begin
                if (promoType.Trim().ToUpper() == "JEAN")
                {
                    ClientConfiguration lsJeanStartDate;
                    using (var dService = _dataUtil.DataServiceInstance())
                    {
                        lsJeanStartDate = dService.GetClientConfiguration("JeansProgramBeginDate");
                    }
                    if (lsJeanStartDate != null && lsJeanStartDate.Value != null)
                    {
                        DateTime tmpdate = new DateTime();
                        if (DateTime.TryParse(lsJeanStartDate.Value, out tmpdate))
                        {
                            // just to be sure that the start date will not be greater than enddate
                            startDate = DateTime.Compare(tmpdate, endDate) >= 0 ? startDate : tmpdate;
                        }
                    }
                }
                // AEO-451 End
                //AEO-1898 BEGIN (B5G1 Bra/Jean data monthly called by member account page)
                if (OptionalStartDate != null)
                {
                    startDate = OptionalStartDate ?? startDate;
                }
                if (OptionalEndDate != null)
                {
                    endDate = OptionalEndDate ?? endDate;
                }
                //AEO-1898 END
                currentBalance = Decimal.Parse(Utilities.GetB5G1CurrentQualifyingPurchased(member, endDate, promoType, false, startDate, endDate));
                //currentBalance = member.GetPoints(pointTypeIDs, startDate, endDate);
                if (currentBalance <= 0)
                {
                    return strRetValue;
                }
                //currentBalance = GetPointsBalance(member, startDate, endDate, pointTypeIDs);

                //double newcurrentBalance = currentBalance / braPromoFulFillmentThreshold; // AEO-74 Upgrade 4.5 changes here -----------SCJ
                decimal newcurrentBalance = currentBalance / braPromoFulFillmentThreshold; // AEO-74 Upgrade 4.5 changes here -----------SCJ
                int remainder = Convert.ToInt32(currentBalance) % braPromoFulFillmentThreshold;

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "CurrentBalance: " + currentBalance.ToString());
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "braPromoFulFillmentThreshold: " + braPromoFulFillmentThreshold.ToString());
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "newcurrentBalance: " + newcurrentBalance.ToString());
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "remainder: " + remainder.ToString());

                if (remainder == 0)
                {
                    if (newcurrentBalance >= 1)
                    {
                        strRetValue = Convert.ToInt32(newcurrentBalance).ToString();
                    }
                }
                else
                {
                    int newBalance = Convert.ToInt32(currentBalance) / braPromoFulFillmentThreshold;
                    strRetValue = newBalance.ToString();
                }
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Points: " + strRetValue);
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetValue;
        }

        /// <summary>
        /// Get life time count for Bras
        /// </summary>
        /// <returns></returns>
        /*
        public static string GetBraLifetimeBalancex(Member member, DateTime endDate)
        {
            //Default return value would be 0
            String strRetValue = "0";
            DateTime startDate = DateTime.Parse("1/1/" + DateTime.Today.Year.ToString());

            startDate = DateTime.Today.AddYears(-3);

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin - StartDate: " + startDate.ToShortDateString());
            try
            {
                int index = 0;
                IList<PointType> pointTypes = LWDataServiceUtil.DataServiceInstance(true).GetAllPointTypes();
                long[] pointTypeIDs = new long[pointTypes.Count];

                foreach (PointType pt in pointTypes)
                {
                    if (pt.Name.ToUpper() == "BRA POINTS")
                    {
                        pointTypeIDs[index] = pt.ID;
                        ++index;
                    }
                }
                //strRetValue = Convert.ToString(member.GetPoints(pointTypeIDs, startDate, endDate));
                strRetValue = GetPointsBalance(member, startDate, endDate, pointTypeIDs).ToString();

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Points: " + strRetValue);
            }
            catch (Exception) { }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetValue;
        }
        */
        /// <summary>
        /// Get the rolling balance for B5G1s 
        /// </summary>
        /// <returns></returns>
        public static string GetBraRollingBalancex(Member member, DateTime startDate, DateTime endDate)
        {
            //Default return value would be 0
            String strRetValue = "0";

            startDate = DateTime.Today.AddMonths(-12);

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin - StartDate: " + startDate.ToShortDateString());
            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    int index = 0;
                    IList<PointType> pointTypes = lwService.GetAllPointTypes();
                    long[] pointTypeIDs = new long[pointTypes.Count];

                    foreach (PointType pt in pointTypes)
                    {
                        if (pt.Name.ToUpper() == "BRA POINTS")
                        {
                            pointTypeIDs[index] = pt.ID;
                            ++index;
                        }
                    }
                    //strRetValue = Convert.ToString(member.GetPoints(pointTypeIDs, startDate, endDate));
                    strRetValue = GetPointsBalance(member, startDate, endDate, pointTypeIDs).ToString();

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Points: " + strRetValue);
                }
            }
            catch (Exception) { }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return strRetValue;
        }

        /// <summary>
        /// Get life time count for Jeans
        /// </summary>
        /// <returns></returns>
        //public static string GetJeansLifetimeBalancex(Member member)
        //{
        //    String strRetVal = String.Empty;
        //    try
        //    {
        //        CheckForAttributeSets(member);

        //        if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
        //        {
        //            MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];

        //            if (memberSummary.JeansLifeTimeBalance != null)
        //            {
        //                strRetVal = Convert.ToString(memberSummary.JeansLifeTimeBalance);
        //            }
        //            else
        //            {
        //                // default the value if it is null
        //                strRetVal = "0";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // default the value incase of an error
        //        strRetVal = "0";
        //        logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
        //    }
        //    return strRetVal;
        //}

        /// <summary>
        /// Get the rolling balance for Jeans 
        /// </summary>
        /// <returns></returns>
        //public static string GetJeansRollingBalancex(Member member)
        //{
        //    String strRetVal = String.Empty;
        //    try
        //    {
        //        CheckForAttributeSets(member);

        //        if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
        //        {
        //            MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];

        //            if (memberSummary.JeansRollingBalance != null)
        //            {
        //                strRetVal = Convert.ToString(memberSummary.JeansRollingBalance);
        //            }
        //            else
        //            {
        //                // default the value if it is null
        //                strRetVal = "0";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // default the value incase of an error
        //        strRetVal = "0";
        //        logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
        //    }
        //    return strRetVal;
        //}

        /// <summary>
        /// Get the first purchase date for Bras
        /// </summary>
        /// <returns></returns>
        //public static string GetJeansFirstPurchaseDatex(Member member)
        //{
        //    String strRetVal = String.Empty;
        //    try
        //    {
        //        CheckForAttributeSets(member);

        //        if ((null != lstMemberBraPromoSummary) && (lstMemberBraPromoSummary.Count > 0))
        //        {
        //            MemberBraPromoSummary memberSummary = lstMemberBraPromoSummary[0];

        //            if (memberSummary.JeansFirstPurchaseDate != null)
        //            {
        //                strRetVal = Convert.ToString(memberSummary.JeansFirstPurchaseDate.Value.ToString("M/d/yyyy"));
        //            }
        //            else
        //            {
        //                // default the value if it is null
        //                strRetVal = "1/1/1900";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // default the incase of an error
        //        strRetVal = "1/1/1900";
        //        logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
        //    }
        //    return strRetVal;
        //}

        #endregion New Promotional methods


        #region Methods To Validate ReqestPointsStore [Added by Shovit]

        /// <summary>
        /// Use to Validate StoreNumber
        /// Numeric. 5 digits
        /// </summary>
        /// <param name="param">string to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsStoreNumberValid(string param)
        {
            Regex regEx = new Regex(@"(^\d{5}$)");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate RegisterNumber
        /// Numeric. 3 digits 
        /// </summary>
        /// <param name="param">string to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsRegisterNumberValid(string param)
        {
            Regex regEx = new Regex(@"(^\d{3}$)");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate TotalPayment
        /// Numeric value + optional 2 decimal places
        /// Format is 200 or 200.00 
        /// Dollar signs not allowed
        /// No value greater than 9999.99 allowed.
        /// </summary>
        /// <param name="param">string to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsTotalPaymentValid(string param)
        {
            param = param.Replace(",", "");
            Regex regEx = new Regex(@"^\d*(\.\d{1,2})?$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            if (double.Parse(param) > 9999.99)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate TransactionNumber
        /// Numeric. Min 3 and max of 6 digits required.
        /// </summary>
        /// <param name="param">string to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsTransactionNumberValid(string param)
        {
            Regex regEx = new Regex(@"^\d{3,6}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Validate OrderNumber and OrderAmount [Added By Manoj]

        /// <summary>
        /// Used to check if the passed order number is valid [Order number must be alphanumeric and within the range
        /// of min and max limit.
        /// </summary>
        /// <param name="orderNumber">string orderNumber</param>
        /// <param name="minLimit">int minLimit</param>
        /// <param name="maxLimit">int maxLimit</param>
        /// <returns>bool value</returns>
        public static bool IsOrderNumberValid(string orderNumber, int minLimit, int maxLimit)
        {
            if (orderNumber != null)
            {
                if (orderNumber.Length >= minLimit && orderNumber.Length <= maxLimit)
                {
                    Regex regEx = new Regex("^[a-zA-Z0-9]*$");
                    return regEx.IsMatch(orderNumber);
                }
            }

            return false;
        }

        /// <summary>
        /// Used to Check if order amount is valid. [ Order amount must be a valid amount string, with optional
        /// two decimal places. It must lie between a 0.0 and maxLimit
        /// </summary>
        /// <param name="orderAmountString">string orderAmountString</param>
        /// <param name="maxLimit">double maxLimit</param>
        /// <returns>bool value</returns>
        public static bool IsOrderAmountValid(string orderAmountString, double maxLimit)
        {
            double orderAmount = 0.0;

            if (double.TryParse(orderAmountString, out orderAmount))
            {
                if (orderAmount >= 0.0 && orderAmount <= maxLimit)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Returns point type object
        /// </summary>
        /// <param name="pstrPointType">Point type</param>
        /// <returns>PointType</returns>
        private static PointType GetPointType(String pstrPointType)
        {
            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                return lwService.GetPointType(pstrPointType) as PointType;
            }
        }
        /// <summary>
        /// Method to get RefBrand
        /// </summary>
        /// <param name="prefix">Prefix of brand</param>
        /// <returns>return RefBrand object</returns>
        public static RefBrand GetRefBrandFromBrandPrefix(string prefix)
        {
            RefBrand brand = new RefBrand();
            LWCriterion criteria = new LWCriterion("RefBrand");
            criteria.Add(LWCriterion.OperatorType.AND, "LoyaltyNumberPrefix", prefix, LWCriterion.Predicate.Eq);
            //IList<IClientDataObject> lstRefBrand = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "RefBrand", criteria, new LWQueryBatchInfo(), false);  // AEO-74 Upgrade 4.5 here -----------SCJ
            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                IList<IClientDataObject> lstRefBrand = lwService.GetAttributeSetObjects(null, "RefBrand", criteria, null, false);
                if (lstRefBrand.Count > 0)
                {
                    brand = (RefBrand)lstRefBrand[0];
                    if (brand != null)
                    {
                        return brand;
                    }
                }
            }
            return brand;
        }

        /// <summary>
        /// Method to get base brand Id
        /// </summary>
        /// <param name="prefix">Prefix of brand</param>
        /// <returns>return int</returns>
        public static int GetBaseBrandIdFromBrandPrefix(string loyaltyIdPrefix)
        {
            //PI21346 - Basebrand and brand flags not set for some members
            int baseBrandId = 1;
            MemberBrand memberBrand = new MemberBrand();
            RefBrand refBrand = GetRefBrandFromBrandPrefix(loyaltyIdPrefix);
            memberBrand.BrandID = refBrand == null ? 1 : refBrand.BrandID;
            if (memberBrand.BrandID == 2 || memberBrand.BrandID == 7 || memberBrand.BrandID == 8 || memberBrand.BrandID == 10)
            {
                baseBrandId = 2;
            }
            return baseBrandId;
        }

        /// <summary>
        /// Method to get bonus points for a transaction header having Gift Card items
        /// </summary>
        /// <param name="txnHeader">TxnHeader</param>
        /// <returns>IList<PointTransaction> </returns>
        public static IList<PointTransaction> GetGiftCardBonusPoints(TxnHeader txnHeader)
        {
            long[] vckeys = { txnHeader.VcKey };
            List<long> detailsRowKeys = new List<long>();

            foreach (IClientDataObject txnDetail in txnHeader.GetChildAttributeSets("TxnDetailItem", true))
            {
                TxnDetailItem detail = (TxnDetailItem)txnDetail;
                if (detail != null && detail.DtlClassCode == "9911")
                {
                    detailsRowKeys.Add(detail.RowKey);
                }
            }
            // Gift card points for the current txn header from its details
            if (detailsRowKeys.Count > 0)
            {
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //return LWDataServiceUtil.DataServiceInstance(true).GetPointTransactionsByRowkeys(vckeys, null, PointTransactionOwnerType.AttributeSet, 104, detailsRowKeys.ToArray(), true);
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    return lwService.GetPointTransactionsByOwner(vckeys, null, PointTransactionOwnerType.AttributeSet, 104, detailsRowKeys.ToArray(), true);
                }
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// A new Method for Dollar Reward Program that gets start date and end date of current month for subscribed members.
        /// </summary>
        /// <param name="memberDetails"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public static void GetProgramDates(Member member, out DateTime startDate, out DateTime endDate, Boolean currentMonth = false)
        {
            //AEO-237
            //RKG - 8/31/2015
            //Code added for redesign to default dates was bypassing the GetQuarterDates for legacy members.  changed to call GetQuarterDates if not in pilot
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            startDate = DateTime.MinValue;
            endDate = DateTime.MaxValue;
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            if (member != null)
            {
                MemberDetails mbrDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                if (mbrDetails != null && Utilities.isInPilot(mbrDetails.ExtendedPlayCode)) //point conversion
                {
                    startDate = new DateTime(2015, 1, 1);
                    endDate = new DateTime(2199, 12, 31);
                }
                else
                {
                    GetQuarterDates(out startDate, out endDate);
                }
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        /// <summary>
        /// Method returns annual valid dates for Dollar Reward Members
        /// Added for DOllar Reward Program
        /// </summary>
        /// <param name="memberDetails"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public static void GetProgramStartEndDates(Member member, ref DateTime startDate, ref DateTime endDate)
        {
            MemberDetails mbrDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;

            // point conversion
            if ((Utilities.isInPilot(mbrDetails.ExtendedPlayCode)) && startDate >= Utilities.DollarRewardsProgramStartDate && startDate <= DollarRewardsProgramEndDate) // AEO-57 Changes
            {
                int year = startDate.Year;
                if (year == 2014)
                {
                    startDate = new DateTime(year, 07, 01);
                    endDate = new DateTime(year, 12, 31);
                }
                else
                {
                    startDate = new DateTime(year, 1, 1);
                    endDate = new DateTime(year, 12, 31);
                }

            }

        }

        //AEO-Redesign-2015 Begin
        //AEO-752 Added bool hasBarcode to funtion
        public static string getBraJeanPrefix(bool txnHasBra, bool txnHasJean,
                                         bool isPilot, string aDesc, bool isRedemption) // AEO-799 begin & end
        {

            StringBuilder lsResult = new StringBuilder();


            if (txnHasBra)
            {
                if ((string.IsNullOrEmpty(aDesc)) || !(aDesc.Contains("B"))) /*AEO-253 changes here -----------------SCJ*/
                {
                    lsResult.Append("B");
                }

            }
            if (txnHasJean)
            {
                if ((string.IsNullOrEmpty(aDesc)) || !(aDesc.Contains("J"))) /*AEO-253 changes here -----------------SCJ*/
                {

                    if (isPilot)
                    {
                        lsResult.Append("J");
                    }
                }
            }

            /*AEO-799 BEgin
             if (isRedemption)
             {
                 //AEO-752 Begin
                 if (hasBarcode)
                 {
                     if ((string.IsNullOrEmpty(aDesc)) || !(aDesc.Contains("R"))) 
                     {
                         lsResult.Append("R");
                     }
                 }
                 else
                 {
                     if ((string.IsNullOrEmpty(aDesc)) || !(aDesc.Contains("F")))
                     {
                         lsResult.Append("F");
                     }
                 }
                 //AEO-752 End
             }
              AEO-799 End */


            if (lsResult.Length > 0)
            {
                lsResult.Insert(0, '-');
            }

            lsResult.Append(aDesc == null ? String.Empty : aDesc);

            return (lsResult.ToString());
        }

        //AEO-Redesign-2015 End

        #region AEO-448_FC
        /// <summary>
        /// AEO-448 Change Redemption Icons for Bras and Jeans in CS Portal---FC
        /// This is an overloaded function to add the isRedemption parameter 
        /// so we can add the JR and BR labels for redemptions points 
        /// without affecting all other point types
        /// 1/25/2016
        /// </summary>
        //public static string getBraJeanPrefix(bool txnHasBra, bool txnHasJean,
        //                                 bool isPilot, string aDesc, bool isRedemption)
        //{

        //    StringBuilder lsResult = new StringBuilder();


        //    if (txnHasBra)
        //    {
        //        if ((string.IsNullOrEmpty(aDesc)) || !(aDesc.Contains("B"))) /*AEO-253 changes here -----------------SCJ*/
        //        {
        //            if (isRedemption)
        //                lsResult.Append("BR");
        //            else
        //                lsResult.Append("B");
        //        }

        //    }
        //    if (txnHasJean && isRedemption)
        //    {                                        
        //        if ((string.IsNullOrEmpty(aDesc)) || !(aDesc.Contains("J"))) /*AEO-253 changes here -----------------SCJ*/
        //        {

        //            if (isPilot)
        //            {
        //                if (isRedemption)
        //                    lsResult.Append("JR");
        //                else
        //                    lsResult.Append("J");
        //            }
        //        }
        //    }


        //    if (lsResult.Length > 0)
        //    {
        //        lsResult.Insert(0, '-');
        //    }

        //    lsResult.Append(aDesc == null ? String.Empty : aDesc);

        //    return (lsResult.ToString());
        //}
        #endregion



        // AEO-401 Begin
        public static Boolean MemberIsInPilot(long? extendedplaycode)
        {

            return extendedplaycode != null && (extendedplaycode == 1 || extendedplaycode == 3);
        }
        // AEO-401 end

        /// <summary>
        /// Method returns true if the Member is in the list of Members selected for the redesing project
        /// </summary>
        /// <param name="memberPostalCode"></param>
        /// <returns>boolean</returns>
        public static bool MemberIsInPilot(string memberPostalCode)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");


            string region = "AttributeSetRefPilotZipCodes";
            string refPilotZipCode = "RefPilotZipCodes";
            IList<RefPilotZipCodes> objRefPilotZipCodes;
            LWCriterion critPilotZip;

            memberPostalCode = memberPostalCode.Substring(0, 5);
            using (var cService = _dataUtil.ContentServiceInstance())
            {
                objRefPilotZipCodes = (IList<RefPilotZipCodes>)cService.CacheManager.Get(region, refPilotZipCode);


                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("ZipCode: {0}", memberPostalCode));
                if (objRefPilotZipCodes == null || objRefPilotZipCodes.Count == 0)
                {
                    critPilotZip = new LWCriterion(refPilotZipCode);
                    critPilotZip.Add(LWCriterion.OperatorType.AND, "PilotPostalCode", memberPostalCode, LWCriterion.Predicate.Eq);

                    IList<IClientDataObject> refPilotZipCodes;
                    using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        refPilotZipCodes = lwService.GetAttributeSetObjects(null, refPilotZipCode, critPilotZip, null, false);
                    }
                    objRefPilotZipCodes = new List<RefPilotZipCodes>();

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("ZipCode Count: {0}", refPilotZipCodes.Count));
                    foreach (RefPilotZipCodes pilotZipCode in refPilotZipCodes)
                    {
                        objRefPilotZipCodes.Add(pilotZipCode);
                    }
                    cService.CacheManager.Add(region, refPilotZipCode, objRefPilotZipCodes);
                    var objRefPilotZipCodes1 = (IList<RefPilotZipCodes>)cService.CacheManager.Get(region, refPilotZipCode);
                }
            }
            //var pilotZipCodes = objRefPilotZipCodes.FirstOrDefault(zip=>zip.PilotPostalCode==memberPostalCode);
            critPilotZip = new LWCriterion(refPilotZipCode);
            critPilotZip.Add(LWCriterion.OperatorType.AND, "PilotPostalCode", memberPostalCode, LWCriterion.Predicate.Eq);

            IList<IClientDataObject> pilotZipCodes;
            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                pilotZipCodes = lwService.GetAttributeSetObjects(null, refPilotZipCode, critPilotZip, null, false);
            }
            objRefPilotZipCodes = new List<RefPilotZipCodes>();

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("ZipCode Count: {0}", pilotZipCodes.Count));

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            if (pilotZipCodes.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            if (start.DayOfWeek == day)
            {
                start = start.AddDays(1);
            }
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }


        /// <summary>
        /// POINT CONVERSION.
        /// Set ExtendedPlayCode and ProgramChangeDate for Members based on their current ExtendedPlayCode and HomeStore. 
        /// Possible set Values are "2" (Pilot to Legacy) or "3" (Legacy to Pilot)
        /// Member with ExntendedPlayCode "1" and homeStore is pilont won't be affected
        /// </summary>
        /// <param name="strHomeStoreId">Member's Current Home Store</param>
        /// <param name="strExtendedPlayCode">Members's current ExtendedPlaycode</param>
        /// <param name="memberNode"></param>
        public static void SetMemberExtendedPlayCode(string HomeStoreNmbr, string ExtendedPlayCode, ref XElement memberNode)
        {

            //EHP - 2015 Point Conersion
            if (!string.IsNullOrEmpty(HomeStoreNmbr))
            {
                using (var cService = _dataUtil.ContentServiceInstance())
                {
                    IList<StoreDef> HomeStoreDefList = cService.GetStoreDef(HomeStoreNmbr);
                    StoreDef MemberHomeStore = HomeStoreDefList.Count > 0 ? HomeStoreDefList.FirstOrDefault() : null;

                    if (MemberHomeStore != null)
                    {
                        bool isHomeStorePilot = Utilities.MemberIsInPilot(MemberHomeStore.ZipOrPostalCode);
                        switch (ExtendedPlayCode)
                        {
                            case "1":
                                if (!isHomeStorePilot)
                                {
                                    //Set Program Change Date
                                    if (memberNode.Element("MemberDetails").Attribute("ProgramChangeDate") != null)
                                        memberNode.Element("MemberDetails").SetAttributeValue("ProgramChangeDate", DateTime.Now);
                                    else
                                        memberNode.Element("MemberDetails").Add(new XAttribute("ProgramChangeDate", DateTime.Now));

                                    //Set Extended play code
                                    if (memberNode.Element("MemberDetails").Attribute("ExtendedPlayCode") != null)
                                        memberNode.Element("MemberDetails").SetAttributeValue("ExtendedPlayCode", "2");
                                    else
                                        memberNode.Element("MemberDetails").Add(new XAttribute("ExtendedPlayCode", "2"));

                                }
                                //else do nothing
                                break;
                            case "2":
                                // do nothing
                                break;
                            case "3":
                                // do nothing
                                break;
                            default:
                                if (isHomeStorePilot)
                                {
                                    //Set Program Change Date
                                    if (memberNode.Element("MemberDetails").Attribute("ProgramChangeDate") != null)
                                        memberNode.Element("MemberDetails").SetAttributeValue("ProgramChangeDate", DateTime.Now);
                                    else
                                        memberNode.Element("MemberDetails").Add(new XAttribute("ProgramChangeDate", DateTime.Now));

                                    //Set Extended play code
                                    if (memberNode.Element("MemberDetails").Attribute("ExtendedPlayCode") != null)
                                        memberNode.Element("MemberDetails").SetAttributeValue("ExtendedPlayCode", "3");
                                    else
                                        memberNode.Element("MemberDetails").Add(new XAttribute("ExtendedPlayCode", "3"));

                                }
                                break;
                        }
                    }
                }
            }
            else
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member HomeStoreId doesn't exists");

            //EHP - 2015 Point Conersion


        }

        //AEO-885 BEGIN
        /// <summary>
        /// TerminationReason      
        /// </summary>
        /// <param name="strHomeStoreId">reasonID</param>               
        public static string GetTerminationReason(long? TerminationReasonID)
        {
            string strTerminationReason = string.Empty;
            LWCriterion criteriaTerm = new LWCriterion("RefTerminationReason");
            criteriaTerm.Add(LWCriterion.OperatorType.AND, "TerminationReasonID", TerminationReasonID, LWCriterion.Predicate.Eq);

            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                IList<IClientDataObject> tmp = lwService.GetAttributeSetObjects(null, "RefTerminationReason", criteriaTerm, new LWQueryBatchInfo() { BatchSize = 1, StartIndex = 0 }, true, false);
                if (tmp != null && tmp.Count > 0)
                {
                    RefTerminationReason Term = (RefTerminationReason)tmp[0];
                    strTerminationReason = Term.TerminationReason;
                }
            }
            return strTerminationReason;
        }
        //AEO-885 END

        //AEO-1793 Begin - Date drop down Member Account Summary
        /// <summary>
        /// Get Range of Dates from drop down date for account summary and account activity pages
        /// </summary>        
        public static void GetRangeDates(string DateRange, ref DateTime startDate, ref DateTime endDate)
        {
            startDate = DateTime.MinValue;
            endDate = DateTime.MaxValue;
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Range of Dates From " + DateRange);

            String strStartDate = DateRange.Substring(0, DateRange.IndexOf("to"));
            DateTime.TryParse(strStartDate, out startDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 00, 00, 00);

            String strEndDate = DateRange.Substring(DateRange.IndexOf("to") + 3);
            DateTime.TryParse(strEndDate, out endDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Range of Dates From " + startDate.ToString() + " To " + endDate.ToString());
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        //AEO-1793 END
    }
}
