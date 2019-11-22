// ----------------------------------------------------------------------------------
// <copyright file="CardHolderEnrollUpdate.cs" company="Brierley and Partners">
//     Copyright statement. All right reserved
// </copyright>
// ----------------------------------------------------------------------------------

namespace AmericanEagle.SDK.OutputProviders
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Xml;
    using System.Linq;

    using AmericanEagle.SDK.Global;
    
    using Brierley.Clients.AmericanEagle.DataModel;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.DataAcquisition.Core;
    using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
    using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

    /// <summary>
    /// Class CardHolderEnrollUpdate
    /// </summary>
    public class CardHolderEnrollUpdate : IDAPOutputProvider
    {
        /// <summary>
        /// Description is used for point event
        /// </summary>
        private string description = string.Empty;

        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);

        /// <summary>
        /// Stores Variable LoyaltyNumber
        /// </summary>
        private string strLoyaltyNumber = string.Empty;

        private string birthdate = string.Empty;
        private string firstname = string.Empty;
        private string lastname = string.Empty;
        private string middlename = string.Empty;
        private string alternateid = string.Empty;
        private string emailaddress = string.Empty;
        private string addresslineone = string.Empty;
        private string addresslinetwo = string.Empty;
        private string city = string.Empty;
        private string stateorprovince = string.Empty;
        private string ziporpostalcode = string.Empty;
        private string country = string.Empty;
        private string addressmailable = string.Empty;
        private string enrolldate = string.Empty;
        private string membersource = string.Empty;
        private string cardtype = string.Empty;
        private string homestoreid = string.Empty;
        private string basebrand = string.Empty;
        private string closedate = string.Empty;
        private string opendate = string.Empty;
        private string accountkey = string.Empty;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.    
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// This method is called to initialize the message dispatcher
        /// </summary>
        /// <param name="globals">NameValueCollection globals</param>
        /// <param name="args">NameValueCollection args</param>
        /// <param name="jobId">long jobId</param>
        /// <param name="config">DAPDirectives config</param>
        /// <param name="parameters">NameValueCollection parameters</param>
        /// <param name="performUtil">DAPPerformanceCounterUtil performUtil</param>
        public void Initialize(NameValueCollection globals, NameValueCollection args, long jobId, DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
        }

        /// <summary>
        /// This method is called to process the messages in the batch
        /// </summary>
        /// <param name="messageBatch">String List</param>
        public void ProcessMessageBatch(IList<string> messageBatch)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            DateTime birthDate = DateTime.MinValue;
            DateTime enrollDate = DateTime.MinValue;
            DateTime openDate = DateTime.MinValue;
            DateTime closeDate = DateTime.MinValue;
            int homeStoreID = 0;
            int baseBrand = 0;
            bool IsNewEnrollment = false;
            bool isPilotMember = false;
            VirtualCard vc = null;
            MemberDetails mbrDetails = null;

            try
            {
                // Tracing for starting of method
                this.logger.Trace(this.className, methodName, "Starts");

                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    // Loding XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/Member");
                if (null == xmlNode)
                {
                    // Logging for null xml node
                    this.logger.Error(this.className, methodName, "xml node not found");
                }
                else
                {

                    strLoyaltyNumber = xmlNode.Attributes["AlternateID"].Value.Trim();
                    birthdate = xmlNode.Attributes["BirthDate"].Value.Trim();
                    firstname = xmlNode.Attributes["FirstName"].Value.Trim();
                    lastname = xmlNode.Attributes["LastName"].Value.Trim();
                    middlename = xmlNode.Attributes["MiddleName"].Value.Trim();
                    emailaddress = xmlNode.Attributes["EmailAddress"].Value.Trim();
                    addresslineone = xmlNode.Attributes["AddressLineOne"].Value.Trim();
                    addresslinetwo = xmlNode.Attributes["AddressLineTwo"].Value.Trim();
                    city = xmlNode.Attributes["City"].Value.Trim();
                    stateorprovince = xmlNode.Attributes["StateOrProvince"].Value.Trim();
                    ziporpostalcode = xmlNode.Attributes["ZipOrPostalCode"].Value.Trim();
                    country = xmlNode.Attributes["Country"].Value.Trim();
                    addressmailable = xmlNode.Attributes["AddressMailable"].Value.Trim();
                    enrolldate = xmlNode.Attributes["EnrollDate"].Value.Trim();
                    cardtype = xmlNode.Attributes["CardType"].Value.Trim();
                    homestoreid = xmlNode.Attributes["HomeStoreId"].Value.Trim();
                    basebrand = xmlNode.Attributes["BaseBrandId"].Value.Trim();
                    closedate = xmlNode.Attributes["CardClosedate"].Value.Trim();
                    opendate = xmlNode.Attributes["CardOpendate"].Value.Trim();
                    accountkey = xmlNode.Attributes["AccountKey"].Value.Trim();

                    DateTime.TryParse(birthdate, out birthDate);
                    DateTime.TryParse(enrolldate, out enrollDate);
                    DateTime.TryParse(closedate, out closeDate);
                    DateTime.TryParse(opendate, out openDate);
                    int.TryParse(homestoreid, out homeStoreID);
                    int.TryParse(basebrand, out baseBrand);


                    // Get member
                    Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(strLoyaltyNumber);

                    if (member == null)
                    {
                        // member not found so add it
                        this.logger.Trace(this.className, methodName, string.Format("Member Not Found for Loyalty Number - {0} so add it", strLoyaltyNumber));
                        member = new Member();
                        IsNewEnrollment = true;


                        member.BirthDate = birthDate;
                        member.FirstName = firstname;
                        member.LastName = lastname;
                        member.MiddleName = middlename;
                        member.PrimaryPostalCode = ziporpostalcode;
                        member.MemberCreateDate = enrollDate;

                        vc = member.CreateNewVirtualCard();
                        vc.LoyaltyIdNumber = strLoyaltyNumber;
                        vc.DateIssued = DateTime.Now;
                        vc.IsPrimary = true;

                        SynchronyAccountKey syncAccountKey = new SynchronyAccountKey();
                        syncAccountKey.AccountKey = accountkey;

                        vc.AddChildAttributeSet(syncAccountKey);

                        mbrDetails = new MemberDetails();
                        mbrDetails.EmailAddress = Utilities.IsEmailValid(emailaddress) ? emailaddress : string.Empty;  //AEO-377 // EHP // Email Validation, IF not valid blank it.
                        mbrDetails.AddressLineOne = addresslineone;
                        mbrDetails.AddressLineTwo = addresslinetwo;
                        mbrDetails.City = city;
                        mbrDetails.StateOrProvince = stateorprovince;
                        mbrDetails.ZipOrPostalCode = ziporpostalcode;
                        mbrDetails.Country = country;
                        mbrDetails.AddressMailable = addressmailable == "1";
                        mbrDetails.MemberSource = 14;
                        mbrDetails.PendingEmailVerification = 1;
                        mbrDetails.PendingCellVerification = 1;
                        mbrDetails.AITUpdate = true;
                        mbrDetails.Gender = "0";

                        if (cardtype == "D")
                        {
                            mbrDetails.CardType = (int)CardType.AEVisaMember;
                        }
                        else if (cardtype == "P")
                        {
                            mbrDetails.CardType = (int)CardType.AECCMember;
                        }

                        mbrDetails.HomeStoreID = homeStoreID;
                        mbrDetails.BaseBrandID = baseBrand;
                        if (closedate.Length > 0)
                        {
                            mbrDetails.CardCloseDate = closeDate;
                            mbrDetails.CardType = (int)CardType.NoCardType;
                        }
                        mbrDetails.CardOpenDate = openDate;

                        //AEO-873 BEGIN
                        // Set PassValidation and EmailAddressMailable Flags
                        if (!string.IsNullOrEmpty(emailaddress))
                        {
                            if (Utilities.IsEmailValid(emailaddress))
                            {
                                mbrDetails.PassValidation = true;
                                mbrDetails.EmailAddressMailable = true;
                            }
                            else
                            {
                                mbrDetails.PassValidation = false;
                                mbrDetails.EmailAddressMailable = false;
                            }
                        }
                        //AEO-873 END

                        mbrDetails.ChangedBy = "BP CardHolder";
                        // added for redesign project
                        isPilotMember = Utilities.MemberIsInPilot(ziporpostalcode);

                       
                        if (isPilotMember)
                        {
                            // memberNode.Element("MemberDetails").Add(new XAttribute("PassValidation", false));
                            // Set ExtendedPlayCode
                            mbrDetails.ExtendedPlayCode = 1;

                            /*  AEO-525 begin
                             ILWDataService service = LWDataServiceUtil.DataServiceInstance(true);


                             PointType braCreditPointType = service.GetPointType("Bra Points");
                             PointType jeanCreditPointType = service.GetPointType("Jean Points");


                             PointEvent braCreditPointEvent = service.GetPointEvent("Bra Credit");
                             PointEvent jeanCreditPointEvent = service.GetPointEvent("Jeans Credit"); ;

                             if (mbrDetails.CardType >= 1 && mbrDetails.CardType <= 3)
                             {
                                 LWDataServiceUtil.DataServiceInstance(true).Credit(vc, braCreditPointType, braCreditPointEvent,
                                            1, null, DateTime.Today, new DateTime(2199, 12, 31), string.Empty, string.Empty);

                                 LWDataServiceUtil.DataServiceInstance(true).Credit(vc, jeanCreditPointType, jeanCreditPointEvent,
                                            1, null, DateTime.Today, new DateTime(2199, 12, 31), string.Empty, string.Empty);
                             }
                             AEO-525 end */
                        }
                        

                        Boolean blnHasEmailBonusCredit = false;
                        if (null != mbrDetails.HasEmailBonusCredit)
                        {
                            blnHasEmailBonusCredit = mbrDetails.HasEmailBonusCredit.Value;
                        }
                        if (!String.IsNullOrEmpty(mbrDetails.EmailAddress) && !isPilotMember)
                        {
                            if (!blnHasEmailBonusCredit)
                            {
                                Utilities.AddEmailBonus(vc, mbrDetails);
                            }
                            mbrDetails.EmailOptIn = true;
                        }


                        member.AddChildAttributeSet(mbrDetails);


                    }
                    else
                    {
                        // member found so update it
                        this.logger.Trace(this.className, methodName, string.Format("Member Found for Loyalty Number - {0} so update it", strLoyaltyNumber));

                        // Getting MemberDetails for Member object
                        IList<IClientDataObject> clientObjects = member.GetChildAttributeSets("MemberDetails");

                        IList<MemberDetails> memberDetails = new List<MemberDetails>();
                        MemberDetails memberDetail = (MemberDetails)clientObjects[0];

                        /* AEO-618 Update ats_synchronyaccountkey */
                        vc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);

                        IList<IClientDataObject> synchronyObject = vc.GetChildAttributeSets("SynchronyAccountKey");

                        if (synchronyObject.Count > 0) {
                            SynchronyAccountKey syncAccountKey = (SynchronyAccountKey) synchronyObject[0];
                            syncAccountKey.AccountKey = accountkey;

                        } else {
                            SynchronyAccountKey syncAccountKey = new SynchronyAccountKey();
                            syncAccountKey.AccountKey = accountkey;
                            vc.AddChildAttributeSet(syncAccountKey);
                        }

                        /* AEO-618 Update ats_synchronyaccountkey */

                        if (memberDetails != null)
                        {
                            if (!string.IsNullOrEmpty(cardtype))
                            {
                                if (!string.IsNullOrEmpty(closedate))
                                {
                                    memberDetail.CardCloseDate = closeDate;
                                    memberDetail.ChangedBy = "BP CardHolder";
                                    //We have a close date, so check if there is an existing double card type.  If not then clear out the card type
                                    if (cardtype == "D")
                                    {
                                        if (memberDetail.CardType == (int)CardType.AECCAndAEVisaMember)
                                            memberDetail.CardType = (int)CardType.AECCMember;
                                        else
                                            memberDetail.CardType = (int)CardType.NoCardType;
                                    }
                                    if (cardtype == "P")
                                    {
                                        if (memberDetail.CardType == (int)CardType.AECCAndAEVisaMember)
                                            memberDetail.CardType = (int)CardType.AEVisaMember;
                                        else
                                            memberDetail.CardType = (int)CardType.NoCardType;
                                    }
                                }
                                else
                                {
                                    //no close date, so set the card type.
                                    IList<RefCardType> refCards = Utilities.GetCardType();
                                    if (refCards != null)
                                    {
                                        RefCardType objCard = refCards.Where(p => p.ClientCardType == cardtype).FirstOrDefault();
                                        if (objCard != null)
                                        {
                                            if (cardtype == "D")
                                                memberDetail.CardType = (int)CardType.AEVisaMember;
                                            else if (cardtype == "P")
                                                memberDetail.CardType = (int)CardType.AECCMember;
                                        }
                                    }
                                }
                            }

                            //Point Conversion
                            //If the following changes at anytime the following must be updated aswell. -> Utilities.SetMemberExtendedPlayCode
                            if (homestoreid.Length > 0)
                            {
                                IList<StoreDef> HomeStoreDefList = LWDataServiceUtil.DataServiceInstance(true).GetStoreDef(homestoreid);
                                StoreDef MemberHomeStore = HomeStoreDefList.Count > 0 ? HomeStoreDefList.FirstOrDefault() : null;

                                memberDetail.HomeStoreID = int.Parse(homestoreid);

                                if (MemberHomeStore != null)
                                {
                                    bool isHomeStorePilot = Utilities.MemberIsInPilot(MemberHomeStore.ZipOrPostalCode);
                                    switch (memberDetail.ExtendedPlayCode.HasValue ? memberDetail.ExtendedPlayCode.Value.ToString() : string.Empty)
                                    {
                                        case "1":
                                            if ( !isHomeStorePilot ) {
                                                //Set Program Change Date and Extende play code
                                                memberDetail.programchangedate = DateTime.Now;
                                                memberDetail.ExtendedPlayCode = 2;
                                            }
                                            /* AEO-503 Begin */
                                            else {
                                                MemberTier memberTier = LWDataServiceUtil.DataServiceInstance(true).GetMemberTier(member, DateTime.Today);
                                                if ( memberTier == null ) {
                                                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No tier, so add one");
                                                    member.AddTier("Blue", DateTime.Today, DateTime.Parse("12/31/2199"), "Base");
                                                }
                                            }
                                            /* AEO-503 end */
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
                                                memberDetail.programchangedate = DateTime.Now;
                                                memberDetail.ExtendedPlayCode = 3;

                                                 /* AEO-503 Begin */
                                                MemberTier memberTier = LWDataServiceUtil.DataServiceInstance(true).GetMemberTier(member, DateTime.Today);
                                                if ( memberTier == null ) {
                                                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No tier, so add one");
                                                    member.AddTier("Blue", DateTime.Today, DateTime.Parse("12/31/2199"), "Base");
                                                }
                                                /* AEO-503 End */
                                            }
                                            break;
                                    }
                                }

                            }
                        }
                    }
                    // Save member Information to Database
                    LWDataServiceUtil.DataServiceInstance(true).SaveMember(member);

                    if (IsNewEnrollment)
                    {
                        
                        Merge.AddMemberProactiveMerge(member); // AEO-414 

                        if (isPilotMember)
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Check for tier");

                            MemberTier memberTier = LWDataServiceUtil.DataServiceInstance(true).GetMemberTier(member, DateTime.Today);
                            if (memberTier == null)
                            {
                                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No tier, so add one");
                                member.AddTier("Blue", DateTime.Today, DateTime.Parse("12/31/2199"), "Base");
                            }
                            
                            ILWDataService service = LWDataServiceUtil.DataServiceInstance(true);


                            PointType braCreditPointType = service.GetPointType("Bra Points");
                            PointType jeanCreditPointType = service.GetPointType("Jean Points");


                            PointEvent braCreditPointEvent = service.GetPointEvent("Bra Credit");
                            PointEvent jeanCreditPointEvent = service.GetPointEvent("Jeans Credit"); ;

                            if (mbrDetails.CardType >= 1 && mbrDetails.CardType <= 3)
                            {

                                
                                LWDataServiceUtil.DataServiceInstance(true).Credit(vc, braCreditPointType, braCreditPointEvent,
                                           1, null, DateTime.Today, new DateTime(2199, 12, 31), string.Empty, string.Empty);

                                LWDataServiceUtil.DataServiceInstance(true).Credit(vc, jeanCreditPointType, jeanCreditPointEvent,
                                           1, null, DateTime.Today, new DateTime(2199, 12, 31), string.Empty, string.Empty);
                            }
                        }
                    }

                }


                // Logging for ending of method
                this.logger.Trace(this.className, methodName, "Ends");
            }
            catch (Exception ex)
            {
                // Logging for exception
                this.logger.Error(this.className, methodName, ex.Message);
            }
        }

        public int Shutdown()
        {
            return 0;
        }
    }
}