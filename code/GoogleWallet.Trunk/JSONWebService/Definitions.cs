using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSONWebService
{
        public enum MemberStatus
        {
            Active = 1,
            Disabled = 2,
            Terminated = 3,
            Locked = 4,
        }

        public enum EmailType
        {
            UpdateProfile = 1,
            OnlineReward = 2,
            OnlineAppeasementReward = 3,
            RequestCreditExpiredInStore = 4,
            RequestCreditExpiredWeb = 5,
            RequestCreditReceived = 6,
            RequestCreditReceiptFound = 7,
            FreeBraAppeasementEmail = 8,
            EnrollActivate = 9,
        }
    
        public enum Brands
        {
            AE = 1,
            Aerie = 2,
            SeventySevenKids = 3,
            SideBySide = 4,
            AEDotCom = 5,
            SeventySevenKidsOnline = 6,
        }
        public enum CardType
        {
            NoCardType = 0,
            AECCMember = 1,
            AEVisaMember = 2,
            AECCAndAEVisaMember = 3,
        }
        public enum Employee
        {
            NonEmployee = 0,
            CurrentEmployee = 1,
            PreviousEmployee = 2,
        }
        public enum ExtendedPlay
        {
            NonExtendedPlayMember = 0,
            CurrentExtendedPlayMember = 1,
            ExtendedPlayControlMember = 2,
            PreviousExtendedPlayMember = 3,
        }
        public enum Gender
        {
            Unknown = 0,
            Male = 1,
            Female = 2,
        }
        public enum ItemType
        {
            Purchase = 1,
            Return = 2,
        }
        public enum LanguagePref
        {
            English = 0,
            Spanish = 1,
            French = 2,
        }
        public enum MemberSource
        {
            InStoreEnrollment = 1,
            RegistrationEnrollment = 2,
            PilotPreEnrollment = 3,
            MailInEnrollment = 4,
            OnlineEnrollment = 5,
            OnlineEnrollmentfromCustomerService = 6,
            RegistrationfromCustomerService = 7,
            InStoreRegistration = 8,
            MailInRegistration = 9,
            InStoreRegistrationfromCustomerService = 10,
            PilotPreEnrollmentRegister = 11,
            LaunchPreEnrollment = 12,
            LaunchPreEnrollmentRegister = 13,
            AECardHolderFile = 14,
            AEEDMFile = 15,
            AEDeclineFile = 16,
            OnlineUniversalRegistration = 17,
            OnlineUniversalRegistrationActivated = 18,
            OnlineAERegistered = 19,
            OnlineAEEnrolled = 20,
            CSPortalUnlinked = 21,
            GWLinked = 22, //PI27662 Google Wallet Project (SCJ) changes here
        }
        public enum ProcessId
        {
            Ready = 0,
            ProcessedforLoyalty = 1,
            UnRegisteredCard = 2,
            NoCard = 3,
            NoProduct = 4,
            NoLineItem = 5,
            Employee = 6,
            RequestCreditProcesssed = 7,
            Error = 8,
            SeventySevenKidsChild = 100,
            SeventySevenKidsMaster = 101,
            SeventySevenKidsMerged = 102,
            GiftCertItem = 103,
            GiftCardItem = 104,

        }
        public enum ReceiptTypes
        {
            InStore = 1,
            Web = 2,
        }
        public enum CardReplaceStatus
        {
            Original = 1,
            ScheduleForReplacement = 2,
            Replaced = 3,
        }
        public enum ReceiptStatus
        {
            Processing = 1,
            Posted = 2,
            NoMatch = 3,
            AlreadyRequested = 100,
            AlreadyPosted = 200,
        }
        /// <summary>
        /// Transaction types
        /// </summary>
        public enum TransactionType
        {
            Store = 0,
            Online = 1,
            Mobile = 2,
            Social = 3,
            OnlineLookup = 4,
            StoreLookup = 5,
        }
        /// <summary>
        /// Reward Status
        /// </summary>
        public enum RewardStatus
        {
            Awarded = 0,
            Sent = 1,
            Merged = 2,
            Redeemed = 3,
            Mailed = 4
        }
        public enum PromoType
        {
            Bra = 1,
            Jeans = 2
        }
        /// <summary>
        /// Response Codes
        /// </summary>
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
            RequestPointsTransactionAlreadyPosted = 620
        }
        public static class Definitions
        {
            public static int GetBaseBrandId(string baseBrand)
            {
                int baseBrandID = 0;

                switch (baseBrand)
                {
                    case "0":
                        baseBrandID = 1;
                        break;
                    case "00":
                        baseBrandID = 1;
                        break;
                    case "20":
                        baseBrandID = 2;
                        break;
                    case "50":
                        baseBrandID = 1;
                        break;
                    default:
                        baseBrandID = 1;
                        break;
                }
                return baseBrandID;

            }
            public static string GetCardReplaceStatus(long cardReplaceStatus)
            {
                string returnValue = string.Empty;

                switch (cardReplaceStatus)
                {
                    case (long)CardReplaceStatus.Original:
                        returnValue = "Original";
                        break;
                    case (long)CardReplaceStatus.Replaced:
                        returnValue = "Replaced";
                        break;
                    case (long)CardReplaceStatus.ScheduleForReplacement:
                        returnValue = "Schedule For Replacement";
                        break;
                    default:
                        break;
                }
                return returnValue;
            }
            public static string GetReceiptStatus(long receiptStatus)
            {
                string returnValue = string.Empty;

                switch (receiptStatus)
                {
                    case (long)ReceiptStatus.Processing:
                        returnValue = "Processing";
                        break;
                    case (long)ReceiptStatus.Posted:
                        returnValue = "Posted";
                        break;
                    case (long)ReceiptStatus.NoMatch:
                        returnValue = "No Match";
                        break;
                    default:
                        break;
                }
                return returnValue;
            }
            public static string GetResponseMessage(ResponseCode responseCode)
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
