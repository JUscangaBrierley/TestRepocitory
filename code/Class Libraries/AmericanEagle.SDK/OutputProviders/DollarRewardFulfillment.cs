using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Configuration;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

using AmericanEagle.SDK.Global;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Common.Exceptions;
using System.Xml.Linq;
// Added for DOllar Reward Program
namespace AmericanEagle.SDK.OutputProviders
{
    class DollarReward
    {
        public String LoyaltyId { get; set; }
        public String Offer { get; set; }
        public String AuthenticationCode { get; set; }
        // Commented out for PI#31663-Update to Dollar Reward fulfillment file
        //public String AuthenticationCode2 { get; set; }
        //public String AuthenticationCode3 { get; set; }
        //public String AuthenticationCode4 { get; set; }
        //public String AuthenticationCode5 { get; set; }
        //public String AuthenticationCode6 { get; set; }
        //public String AuthenticationCode7 { get; set; }
        public Member LoyaltyMember
        {
            get
            {
                return LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(LoyaltyId);
            }
        }
    }
    enum DollarRewardType
    {
        DollarTen = 10,
        DollarTwenty = 20,
        DollarThirty = 30,
        DollarFourty = 40,
        DollarFifty = 50,
        // PI#31663-Update to Dollar Reward fulfillment file, Start changes
        DollarSixty = 60,
        DollarSeventy = 70,
        DollarEighty = 80,
        DollarNinty = 90,
        DollarHundred = 100
        // PI#31663-Update to Dollar Reward fulfillment file, End changes
    }

    public class DollarRewardFulfillment : IDAPOutputProvider
    {
        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private DollarReward DollarRewardMember = new DollarReward();
        public void Initialize(System.Collections.Specialized.NameValueCollection globals, System.Collections.Specialized.NameValueCollection args, long jobId, DAPDirectives config, System.Collections.Specialized.NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public void ProcessMessageBatch(IList<string> messageBatch)
        {
            String fileName = String.Empty;
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //Double pointsToConsume = 0;
            //Double estimatedConsumptionPoints = 0; // Added for PI#31663-Update to Dollar Reward fulfillment file
            Decimal pointsToConsume = 0;
            Decimal estimatedConsumptionPoints = 0; // Added for PI#31663-Update to Dollar Reward fulfillment file
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            
            try
            {
                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    // Loding XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/Global");
                ///doc.DocumentElement.HasAttribute
                foreach (XmlAttribute att in xmlNode.Attributes)
                    {
                        switch (att.Name)
                            {
                                case "LOY_ID":
                                    this.DollarRewardMember.LoyaltyId = xmlNode.Attributes["LOY_ID"].Value.Trim();
                                    break;
                                case "OFFER":
                                    this.DollarRewardMember.Offer = xmlNode.Attributes["OFFER"].Value.Trim();
                                    break;
                                case "AUTH_CD":
                                    this.DollarRewardMember.AuthenticationCode = xmlNode.Attributes["AUTH_CD"].Value.Trim();
                                    break;
                             // Commented out for PI#31663-Update to Dollar Reward fulfillment file,    
                            //case "AUTH2":
                                //    this.DollarRewardMember.AuthenticationCode2 = xmlNode.Attributes["AUTH2"].Value.Trim();
                                //    break;
                                //case "AUTH3":
                                //    this.DollarRewardMember.AuthenticationCode3 = xmlNode.Attributes["AUTH3"].Value.Trim();
                                //    break;
                                //case "AUTH4":
                                //    this.DollarRewardMember.AuthenticationCode4 = xmlNode.Attributes["AUTH4"].Value.Trim();
                                //    break;
                                //case "AUTH5":
                                //    this.DollarRewardMember.AuthenticationCode5 = xmlNode.Attributes["AUTH5"].Value.Trim();
                                //    break;
                                //case "AUTH6":
                                //    this.DollarRewardMember.AuthenticationCode6 = xmlNode.Attributes["AUTH6"].Value.Trim();
                                //    break;
                                //case "AUTH7":
                                //    this.DollarRewardMember.AuthenticationCode7 = xmlNode.Attributes["AUTH7"].Value.Trim();
                                //    break;
                                case "ExceptionRecordsFileName":
                                    fileName = xmlNode.Attributes["ExceptionRecordsFileName"].Value.Trim();
                                    break;
                                default:
                                    break;

                            }
                       
                }

                Member member = this.DollarRewardMember.LoyaltyMember;
                if (null == member)
                {
                    // Log error when member not found
                    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for Loyalty Number - " + this.DollarRewardMember.LoyaltyId);
                }
                else
                {
                    //COmmented out for PI#31663-Update to Dollar Reward fulfillment file
                    //ClientConfiguration objClientConfiguration = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("DollarRewardsPoints");
                    //int gDollarPoints = Convert.ToInt32(objClientConfiguration.Value);
                    if (!String.IsNullOrEmpty(this.DollarRewardMember.AuthenticationCode))
                    {
                        // PI#31663-Update to Dollar Reward fulfillment file, Start changes
                        if(!String.IsNullOrEmpty(this.DollarRewardMember.Offer))
                        {
                            DollarRewardType  dollarRewType = GetDollarRewardType(this.DollarRewardMember.Offer, out estimatedConsumptionPoints);
                            if (CreateRedemptionsAndRewards(dollarRewType, this.DollarRewardMember.AuthenticationCode, fileName, this.DollarRewardMember.Offer))
                            {
                                pointsToConsume = estimatedConsumptionPoints;
                            }
                        }

                    }
                    //if (!String.IsNullOrEmpty(this.DollarRewardMember.AuthenticationCode2))
                    //{
                    //    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Calling for Dollar Twenty");
                    //    if (CreateRedemptionsAndRewards(DollarRewardType.DollarTwenty, this.DollarRewardMember.AuthenticationCode2, fileName, this.DollarRewardMember.Offer))
                    //    {
                    //        pointsToConsume = pointsToConsume + (gDollarPoints * 2);
                    //    }
                    //}
                    //if (!String.IsNullOrEmpty(this.DollarRewardMember.AuthenticationCode3))
                    //{
                    //    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Calling for Dollar Thirty");
                    //    if (CreateRedemptionsAndRewards(DollarRewardType.DollarThirty, this.DollarRewardMember.AuthenticationCode3, fileName, this.DollarRewardMember.Offer))
                    //    {
                    //        pointsToConsume = pointsToConsume + (gDollarPoints * 3);
                    //    }
                    //}
                    //if (!String.IsNullOrEmpty(this.DollarRewardMember.AuthenticationCode4))
                    //{
                    //    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Calling for Dollar Fourty");
                    //    if (CreateRedemptionsAndRewards(DollarRewardType.DollarFourty, this.DollarRewardMember.AuthenticationCode4, fileName, this.DollarRewardMember.Offer))
                    //    {
                    //        pointsToConsume = pointsToConsume + (gDollarPoints * 4);
                    //    }
                    //}
                    //if (!String.IsNullOrEmpty(this.DollarRewardMember.AuthenticationCode5))
                    //{
                    //    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Calling for Dollar Fifty");
                    //    if (CreateRedemptionsAndRewards(DollarRewardType.DollarFifty, this.DollarRewardMember.AuthenticationCode5, fileName, this.DollarRewardMember.Offer))
                    //    {
                    //        pointsToConsume = pointsToConsume + (gDollarPoints * 5);
                    //    }
                    //}
                    //if (!String.IsNullOrEmpty(this.DollarRewardMember.AuthenticationCode6))
                    //{
                    //    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Calling for Dollar Second Fifty");
                    //    if (CreateRedemptionsAndRewards(DollarRewardType.DollarFifty, this.DollarRewardMember.AuthenticationCode6, fileName, this.DollarRewardMember.Offer))
                    //    {
                    //        pointsToConsume = pointsToConsume + (gDollarPoints * 5);
                    //    }
                    //}
                    // PI#31663-Update to Dollar Reward fulfillment file, End Changes
                    if (pointsToConsume > 0)
                    {
                        CreateConsumedPoints(member, pointsToConsume);
                    }
            
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
            }

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public void Dispose()
        {
        }

        private void CreateRefRedemptionCode(String reasonCode, DollarRewardType dollarRewardType)
        {
            int year = DateTime.Now.Year;
            DateTime startDate = new DateTime(year, DateTime.Now.AddMonths(-1).Month, 1);
            DateTime endDate = new DateTime(year, DateTime.Now.AddMonths(1).Month, 1);
            try
            {
                RefRedemptionsCodes redemptionCode = new RefRedemptionsCodes();
                redemptionCode.ReasonCode = reasonCode;
                redemptionCode.EffectiveStartDate = startDate;
                redemptionCode.EffectiveEndDate = endDate;
                redemptionCode.Description = string.Format("{0} Reward - ${1}.00", DateTime.Now.ToString("MMMM").ToUpper(), (int)dollarRewardType);
                LWDataServiceUtil.DataServiceInstance(true).SaveAttributeSetObject(redemptionCode);
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
            }
        }
        //private void CreateReward(Member member, String reasonCode, RewardDef rdef, double rewardAmount) // AEO-74 Upgrade 4.5 changes here -----------SCJ
            private void CreateReward(Member member, String reasonCode, RewardDef rdef, decimal rewardAmount) 
        {
            MemberReward reward = new MemberReward();
            reward.RewardDefId = rdef.Id;
            reward.OfferCode = reasonCode;
            reward.MemberId = member.IpCode;
            reward.ProductId = rdef.ProductId;
            if ((DateTime.Now.Month == 1) && (DateTime.Now.Year == 2015))
            {
                reward.DateIssued = new DateTime(2014, 12, 31);
            }
            else
            {
                reward.DateIssued = DateTime.Now;
            }
            //reward.Expiration = expiryDate;
            reward.AvailableBalance = (long)rewardAmount;
            reward.CertificateNmbr = rdef.CertificateTypeCode;
            LWDataServiceUtil.DataServiceInstance(true).CreateMemberReward(reward);
            
        }
        //private void CreateConsumedPoints(Member member, double pointsToConsume)  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            private void CreateConsumedPoints(Member member, decimal pointsToConsume)
        {

            string _pointType = String.Empty;
            string _pointEvent = "Dollar Reward";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //Double existingBonusPoints = 0;
           // Double bonusToConsume = 0;
           // Double basicToConsume = 0;
            Decimal existingBonusPoints = 0;
            Decimal bonusToConsume = 0;
            Decimal basicToConsume = 0;
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            PointType pointType = null;
            PointEvent pointEvent = null;
           
            DateTime   currentStartDate = DateTime.MinValue;
            DateTime   currentEndDate = DateTime.MinValue;
           
            try
            {
                VirtualCard virtualCard = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                /*
               Changes to AEO 56 begin here      ----------SCJ
               */
             //   Utilities.GetProgramDates(member, out currentStartDate, out currentEndDate);           
               DollarRwdEOYDates(member, out currentStartDate, out currentEndDate);
                /*
               Changes to AEO 56 end here      ----------SCJ
               */
               //existingBonusPoints = Double.Parse(Utilities.GetBonusPoints(member, currentStartDate, currentEndDate));  // AEO-74 Upgrade 4.5 changes here -----------SCJ
                existingBonusPoints = Decimal.Parse(Utilities.GetBonusPoints(member, currentStartDate, currentEndDate));
                if (pointsToConsume > existingBonusPoints)
                {
                    bonusToConsume = existingBonusPoints * (-1);
                    basicToConsume = (pointsToConsume - existingBonusPoints) * (-1);
                }
                else
                {
                    bonusToConsume = pointsToConsume * (-1);
                }
                _pointType = "Bonus Points";
                if (Utilities.GetPointTypeAndEvent(_pointType, _pointEvent, out pointEvent, out pointType))
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Give Points: " + pointEvent.Name);
              /*
             Changes to AEO 56 begin here      ----------SCJ
             */
                    if ((DateTime.Now.Month == 1) && (DateTime.Now.Year == 2015))
                    {
                        //LWDataServiceUtil.DataServiceInstance(true).Credit(virtualCard, pointType, pointEvent, bonusToConsume, currentEndDate.AddDays(-1), currentEndDate.AddDays(1), string.Empty, string.Empty);
                        LWDataServiceUtil.DataServiceInstance(true).Credit(virtualCard, pointType, pointEvent, bonusToConsume,null, currentEndDate.AddDays(-1), currentEndDate.AddDays(1), string.Empty, string.Empty);
                    }
            /*
           Changes to AEO 56 end here      ----------SCJ
           */
                    else
                    {
                        // AEO-74 Upgrade 4.5 changes here -----------SCJ
                        //LWDataServiceUtil.DataServiceInstance(true).Credit( virtualCard, pointType, pointEvent, bonusToConsume,DateTime.Today, currentEndDate.AddDays(1), string.Empty, string.Empty);
                        LWDataServiceUtil.DataServiceInstance(true).Credit(virtualCard, pointType, pointEvent, bonusToConsume,null, DateTime.Today, currentEndDate.AddDays(1), string.Empty, string.Empty);
                    }
                }
                else
                {
                    throw new LWException(string.Format("Could not consume {0} bonus points for member id {1}", pointsToConsume.ToString(), member.MyKey.ToString()));
                }
                if (basicToConsume < 0)
                {
                    _pointType = "Basic Points";
                    if (Utilities.GetPointTypeAndEvent(_pointType, _pointEvent, out pointEvent, out pointType))
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Give Points: " + pointEvent.Name);
        /*
       Changes to AEO 56 begin here      ----------SCJ
       */
                        if ((DateTime.Now.Month == 1) && (DateTime.Now.Year == 2015))
                        {
                            // AEO-74 Upgrade 4.5 changes here -----------SCJ
                           // LWDataServiceUtil.DataServiceInstance(true).Credit(virtualCard, pointType, pointEvent, basicToConsume, currentEndDate.AddDays(-1), currentEndDate.AddDays(1), string.Empty, string.Empty);
                            LWDataServiceUtil.DataServiceInstance(true).Credit(virtualCard, pointType, pointEvent, basicToConsume,null, currentEndDate.AddDays(-1), currentEndDate.AddDays(1), string.Empty, string.Empty);
                        }
           /*
           Changes to AEO 56 end here      ----------SCJ
           */
                        else
                        {
                            // AEO-74 Upgrade 4.5 changes here -----------SCJ
                            //LWDataServiceUtil.DataServiceInstance(true).Credit(virtualCard, pointType, pointEvent, basicToConsume, DateTime.Today, currentEndDate.AddDays(1), string.Empty, string.Empty);
                            LWDataServiceUtil.DataServiceInstance(true).Credit(virtualCard, pointType, pointEvent, basicToConsume,null, DateTime.Today, currentEndDate.AddDays(1), string.Empty, string.Empty);
                        }
                    }
                    else
                    {
                        throw new LWException(string.Format("Could not consume {0} basic points for member id {1}", pointsToConsume.ToString(), member.MyKey.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
            }

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
            //public double GetTotalPoints(Member member)  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            public decimal GetTotalPoints(Member member)
        {
            int index = 0;
            DateTime from = DateTime.Now;
            DateTime to = DateTime.Now;
            //double points = 0;     // AEO-74 Upgrade 4.5 changes here -----------SCJ
            decimal points = 0;     // AEO-74 Upgrade 4.5 changes here -----------SCJ
            IList<PointType> pointTypes = LWDataServiceUtil.DataServiceInstance(true).GetAllPointTypes();
            long[] pointTypeIDs = new long[pointTypes.Count];
            if (member != null)
            {
                foreach (PointType pt in pointTypes)
                {
                    if (((pt.Name.ToUpper().Contains("BRA")) || (pt.Name.ToUpper().Contains("JEAN"))))
                    {
                        //Exlude the bra points from the point balance.
                    }
                    else
                    {
                        pointTypeIDs[index] = pt.ID;
                        ++index;
                    }
                }
            }
            /*
              Changes to AEO 56 begin here      ----------SCJ
              */
           // Utilities.GetProgramDates(member, out from, out to);
           
            DollarRwdEOYDates(member, out from, out to);
            to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddSeconds(-1); // PI 31427 - Need correction to Dollar Reward fulfillment process
           // points = member.GetPoints(pointTypeIDs, from, to);
            points = Utilities.GetPointsBalance(member, from, to);
            /*
          Changes to AEO 56 end here      ----------SCJ
          */
         
            return points;
        }
            // AEO-74 Upgrade 4.5 changes here -----------SCJ
        //private void GetRewardAmountAndRDef(DollarRewardType dollarRewardType, out RewardDef rdef, out double rewardAmount)
              private void GetRewardAmountAndRDef(DollarRewardType dollarRewardType, out RewardDef rdef, out decimal rewardAmount)
        {
            String rewardType = String.Empty;
            int gDollarPoints = 0;
           
            ClientConfiguration objClientConfiguration = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("DollarRewardsPoints");
            gDollarPoints = Convert.ToInt32(objClientConfiguration.Value);
            switch (dollarRewardType)
            {
                case DollarRewardType.DollarTen:
                    rewardType = "$10-Dollar Reward";
                    break;
                case DollarRewardType.DollarTwenty:
                    rewardType = "$20-Dollar Reward";
                    break;
                case DollarRewardType.DollarThirty:
                    rewardType = "$30-Dollar Reward";
                    break;
                case DollarRewardType.DollarFourty:
                    rewardType = "$40-Dollar Reward";
                    break;
                case DollarRewardType.DollarFifty:
                    rewardType = "$50-Dollar Reward";
                    break;
                // PI#31663-Update to Dollar Reward fulfillment file, Start Changes
                case DollarRewardType.DollarSixty:
                    rewardType = "$60-Dollar Reward";
                    break;
                case DollarRewardType.DollarSeventy:
                    rewardType = "$70-Dollar Reward";
                    break;
                case DollarRewardType.DollarEighty:
                    rewardType = "$80-Dollar Reward";
                    break;
                case DollarRewardType.DollarNinty:
                    rewardType = "$90-Dollar Reward";
                    break;
                case DollarRewardType.DollarHundred:
                    rewardType = "$100-Dollar Reward";
                    break;
                // PI#31663-Update to Dollar Reward fulfillment file, End Changes
            }
            rdef = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef(rewardType);
            string rewardAmountString = rdef.Name.Substring(1, 2);
            //rewardAmount = double.Parse(rewardAmountString); // AEO-74 Upgrade 4.5 changes here -----------SCJ
            rewardAmount = decimal.Parse(rewardAmountString); // AEO-74 Upgrade 4.5 changes here -----------SCJ
            
        }
        private Boolean CreateRedemptionsAndRewards(DollarRewardType dollarRewardType, String reasonCode, String fileName, String offer)
        {
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //double totalPoints = 0; 
            //double pointsNeededForReward = 0;
            //double rewardAmount = 0;
            decimal totalPoints = 0;
            decimal pointsNeededForReward = 0;
            decimal rewardAmount = 0;
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            int gDollarPoints = 0;
            Boolean rewardCreated = false;
            RewardDef rdef;
            String lastRecord = String.Empty;
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Start");
            StreamWriter exceptionFile = new StreamWriter(fileName, true);
            exceptionFile.Close();
            ClientConfiguration objClientConfiguration = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("DollarRewardsPoints");
            gDollarPoints = Convert.ToInt32(objClientConfiguration.Value);
            totalPoints = GetTotalPoints(this.DollarRewardMember.LoyaltyMember);
            GetRewardAmountAndRDef(dollarRewardType, out rdef, out rewardAmount);
            //pointsNeededForReward = ((double.Parse(offer))/10) * gDollarPoints; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            pointsNeededForReward = ((decimal.Parse(offer)) / 10) * gDollarPoints;
            if (totalPoints >= pointsNeededForReward)
            {
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Before Calling For Redemptions");
                CreateRefRedemptionCode(reasonCode, dollarRewardType);
                CreateReward(this.DollarRewardMember.LoyaltyMember, reasonCode, rdef, rewardAmount);
                rewardCreated = true;
            }
            else
            {
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Writing Rejected Record " + this.DollarRewardMember.LoyaltyId);
                
                StreamReader reader = new StreamReader(fileName);
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    lastRecord = line;
                }
                reader.Close();
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Last Record is " + lastRecord + "and Loyalty ID is " + this.DollarRewardMember.LoyaltyId);
                if (lastRecord != this.DollarRewardMember.LoyaltyId)
                {
                    exceptionFile = new StreamWriter(fileName, true);
                    exceptionFile.WriteLine(this.DollarRewardMember.LoyaltyId);
                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "After Writing");
                    exceptionFile.Close();
                }
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "After Closing");
                rewardCreated = false;
            }
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return rewardCreated;
        }
        //PI#31663-Update to Dollar Reward fulfillment file, Start Changes
        //private DollarRewardType GetDollarRewardType(String offer, out Double estConsumePoints)  // AEO-74 Upgrade 4.5 changes here -----------SCJ
            private DollarRewardType GetDollarRewardType(String offer, out Decimal estConsumePoints)
        {
            int dollarReward = Convert.ToInt32(offer);
            estConsumePoints = dollarReward * 15;
            switch (dollarReward)
            {
                case 100:
                    return DollarRewardType.DollarHundred;
                case 20:
                    return DollarRewardType.DollarTwenty;
                case 30:
                    return DollarRewardType.DollarThirty;
                case 40:
                    return DollarRewardType.DollarFourty;
                case 50:
                    return DollarRewardType.DollarFifty;
                case 60:
                    return DollarRewardType.DollarSixty;
                case 70:
                    return DollarRewardType.DollarSeventy;
                case 80:
                    return DollarRewardType.DollarEighty;
                case 90:
                    return DollarRewardType.DollarNinty;
                default:
                    return DollarRewardType.DollarTen;
                    
            }
        }
        // PI#31663-Update to Dollar Reward fulfillment file, End changes

        /*
               Changes to AEO 56 begin here      ----------SCJ
               */
        private void DollarRwdEOYDates(Member member, out DateTime stDate, out DateTime edDate)
        {
            if ((DateTime.Now.Month == 1) && (DateTime.Now.Year == 2015))
            {
                MemberDetails mbrDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                if (Utilities.isInPilot(mbrDetails.ExtendedPlayCode)) //AEO-point conversion
                {
                    stDate = new DateTime(2014, 07, 01);
                    edDate = new DateTime(2014, 12, 31);
                }
                else
                {
                    stDate = new DateTime(2014, 10, 01);
                    edDate = new DateTime(2014, 12, 31);
                }
            }
            else
            {
                Utilities.GetProgramDates(member, out stDate, out edDate); 
            }

        }
        /*
       Changes to AEO 56 end here      ----------SCJ
       */
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown()
        {
            return 0;
            //     throw new System.NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    }
}
