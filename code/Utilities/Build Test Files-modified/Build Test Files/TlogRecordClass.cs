using System;
using System.Collections;
using System.Collections.Generic;

namespace BuildTestFiles
{
    /// <summary>
    /// Summary description for TlogRecordClass.
    /// </summary>
    public class TlogRecordClass
    {
        public List<string> DetailRecords = new List<string>();
        public List<SKUItemDetail> ItemDetail = new List<SKUItemDetail>();
        public List<GiftCardClass> GiftCards = new List<GiftCardClass>();
        public List<TenderTypeDetail> TenderDetail = new List<TenderTypeDetail>();
        public List<PromotionItemDetail> PromotionItemDetail = new List<PromotionItemDetail>();
        public List<DiscountRecord> DiscountDetail = new List<DiscountRecord>();
        public List<string> Promotions = new List<string>();

        private string m_RawTransactionString = string.Empty;
        private string m_AltCardNumber = string.Empty;
        private string m_AECardNumber = string.Empty;
        private string m_LastPurchaseDate = string.Empty;
        private string m_PurchaseDate = string.Empty;
        private string m_PurchaseTime = string.Empty;
        private string m_ReasonCode = string.Empty;
        private string m_EmployeeNumber = string.Empty;
        private string m_HHKey = string.Empty;
        private string m_RegisterNumber = string.Empty;
        private string m_TransactionNumber = string.Empty;
        private decimal m_QualifiedPurchaseAmount = 0;
        private decimal m_PurchaseDiscount = 0;
        private string m_StoreCountry = string.Empty;
        private string m_ShipDate = string.Empty;
        private string m_AECCActivationDate = "";
        private string m_OriginalTransactionInformation = string.Empty;
        private string m_OriginalTransactionNumber = string.Empty;
        private string m_OriginalPurchaseDate = string.Empty;
        private string m_WebOrderNumber = string.Empty;
        private string m_WebOrderSuffix = string.Empty;
        private string m_State = string.Empty;
        private string m_Country = string.Empty;
        private string m_OrderNumber = "";
        private string m_MoneyCardSKU = "";
        private string m_OriginalWebOrderNumber = "";
        private string m_DateAwarded;
        private string m_DateRedeemed;
        private string m_DateReplaced;
        private string m_StatusRedeemed;
        private string m_StatusReplaced;
        private string m_OperatorID;
        private string m_CreditStartDate = string.Empty;
        private string m_CreditEndDate = string.Empty;
        private string m_DateMailed = string.Empty;
        private string m_String77 = string.Empty;
        private string m_OldLoyaltyNumber = string.Empty;
        private string m_NewLoyaltyNumber = string.Empty;
        // PI 26487, Akbar, Adding single use coupon fields starts 
        private string m_SingleUseCoupon = string.Empty;
        private string m_SUCNotValid = string.Empty;
        // PI 26487, Akbar, Adding single use coupon fields ends
        //AEO-844 adding Changes to include currency code
        private string m_currencycode = string.Empty;
        //AEO-844 adding Changes to include currency code

        private long m_TransactionID = 0;

        private int m_PriorEmployee = 0;
        private int m_CurrentEmployee = 0;
        private int m_IPCode = 0;
        private int m_OriginatingIPCode = 0;
        private int m_Credits = 0;
        private int m_BonusCredits = 0;
        private int m_CreditCardBonus = 0;
        private int m_CreditType = 0;
        private int m_BonusType = 0;
        private int m_BonusType2 = 0;
        private int m_BonusType3 = 0;
        private int m_BonusType4 = 0;
        private int m_OneTimeBonus = 0;
        private int m_BonusOnItems = 0;
        private int m_TransactionBonus = 0;
        private int m_StoreCode = 0;
        private int m_TenderType = 0;
        private int m_TransType = 0;
        private int m_NumberOfItems = 0;
        private int m_TransactionTotal = 0;
        private int m_TransactionAmount = 0;
        private int m_Mode = 0;
        private int m_GiftCardMode = 0;
        private int m_AECCStatus = 0;
        private int m_GiftCardQuantity = 0;
        private int m_EmployeeFlag = 0;
        private int m_ProfileDataProcessed = 0;
        private int m_DetailRecordsCreated = 0;
        private int m_OriginalStoreCode = 0;
        private int m_OriginalTenderType = 0;
        private int m_NumberOfMoneyCards = 0;
        private int m_WishToReceiveEmail = 0;
        private int m_OrderID = 0;
        private int m_RewardLevelID = 0;
        private int m_StartingCredits = 0;
        private int m_ExpiredCredits = 0;
        private int m_RedeemedCredits = 0;
        private int m_RolloverCredits = 0;
        private int m_CreditsNeededForNextLevel = 0;
        private int m_HighTier;
        private int m_NumberOfItemsReturned = 0;
        private int m_NumberOfItemsPurchased = 0;
        private int m_BrandID = 0;
        private int m_ApplicationType = 0;
        private int m_CreditsForBonus = 0;

        private int m_AltStoreCode = 0;
        private string m_AltTransactionNumber = string.Empty;
        private string m_AltTransactionDate = string.Empty;
        private string m_AltOrderNumber = string.Empty;
        private string m_AltRegisterNumber = string.Empty;
        private decimal m_String181Amount = 0;
        private decimal m_String182Amount = 0;

        private decimal m_GiftCardAmount = 0;
        private decimal m_ActualAmount = 0;
        private decimal m_ReturnsTotal = 0;
        private decimal m_Tax = 0;
        private decimal m_TaxRate = 0;
        private decimal m_GiftCertificateAmount = 0;


        private bool m_Refund = false;
        private bool webOrder = false;
        private bool m_PromotionItem = false;
        private bool m_PreEnrollmentBonus = false;
        private bool m_GetCredits = false;
        private bool m_OrigGotCredits = false;



        public TlogRecordClass()
        {
        }

        public bool PreEnrollmentBonus
        {
            get { return m_PreEnrollmentBonus; }
            set { m_PreEnrollmentBonus = value; }
        }

        public int DetailRecordsCreated
        {
            get { return m_DetailRecordsCreated; }
            set { m_DetailRecordsCreated = value; }
        }
        public string OriginalWebOrderNumber
        {
            get { return m_OriginalWebOrderNumber; }
            set { m_OriginalWebOrderNumber = value; }
        }
        public int NumberOfMoneyCards
        {
            get { return m_NumberOfMoneyCards; }
            set { m_NumberOfMoneyCards = value; }
        }
        public int WishToReceiveEmail
        {
            get { return m_WishToReceiveEmail; }
            set { m_WishToReceiveEmail = value; }
        }

        public string MoneyCardSKU
        {
            get { return m_MoneyCardSKU; }
            set { m_MoneyCardSKU = value; }
        }
        public int AECCStatus
        {
            get { return m_AECCStatus; }
            set { m_AECCStatus = value; }
        }

        public string AECCActivationDate
        {
            get { return m_AECCActivationDate; }
            set { m_AECCActivationDate = value; }
        }

        public int NumberOfItems
        {
            get { return m_NumberOfItems; }
            set { m_NumberOfItems = value; }
        }

        public string RegisterNumber
        {
            get { return m_RegisterNumber; }
            set { m_RegisterNumber = value; }
        }

        public string OrderNumber
        {
            get { return m_OrderNumber; }
            set { m_OrderNumber = value; }
        }


        public int Credits
        {
            get { return m_Credits; }
            set { m_Credits = value; }
        }

        public int BonusCredits
        {
            get { return m_BonusCredits; }
            set { m_BonusCredits = value; }
        }
        public int CreditCardBonus
        {
            get { return m_CreditCardBonus; }
            set { m_CreditCardBonus = value; }
        }

        public int CreditsForBonus
        {
            get { return m_CreditsForBonus; }
            set { m_CreditsForBonus = value; }
        }
        public int CreditType
        {
            get { return m_CreditType; }
            set { m_CreditType = value; }
        }
        public int BonusType
        {
            get { return m_BonusType; }
            set { m_BonusType = value; }
        }
        public int BonusType2
        {
            get { return m_BonusType2; }
            set { m_BonusType2 = value; }
        }
        public int BonusType3
        {
            get { return m_BonusType3; }
            set { m_BonusType3 = value; }
        }
        public int BonusType4
        {
            get { return m_BonusType4; }
            set { m_BonusType4 = value; }
        }
        public int OneTimeBonus
        {
            get { return m_OneTimeBonus; }
            set { m_OneTimeBonus = value; }
        }
        public int BonusOnItems
        {
            get { return m_BonusOnItems; }
            set { m_BonusOnItems = value; }
        }
        public int TransactionBonus
        {
            get { return m_TransactionBonus; }
            set { m_TransactionBonus = value; }
        }
        public int IPCode
        {
            get { return m_IPCode; }
            set { m_IPCode = value; }
        }
        public string Country
        {
            get { return m_Country; }
            set { m_Country = value; }
        }
        public string State
        {
            get { return m_State; }
            set { m_State = value; }
        }
        public int OriginatingIPCode
        {
            get { return m_OriginatingIPCode; }
            set { m_OriginatingIPCode = value; }
        }

        public string EmployeeNumber
        {
            get { return m_EmployeeNumber; }
            set { m_EmployeeNumber = value; }
        }

        public int PriorEmployee
        {
            get { return m_PriorEmployee; }
            set { m_PriorEmployee = value; }
        }

        public int CurrentEmployee
        {
            get { return m_CurrentEmployee; }
            set { m_CurrentEmployee = value; }
        }

        public string ReasonCode
        {
            get { return m_ReasonCode; }
            set { m_ReasonCode = value; }
        }

        // PI 26487, Akbar, Adding properties single use coupon fields starts 
        public string SingleUseCoupon
        {
            get { return m_SingleUseCoupon; }
            set { m_SingleUseCoupon = value; }
        }

        public string SUCNotValid
        {
            get { return m_SUCNotValid; }
            set { m_SUCNotValid = value; }
        }
        // PI 26487, Akbar, Adding single use coupon fields ends

        public string HHKey
        {
            get { return m_HHKey; }
            set { m_HHKey = value; }
        }

        public int StoreCode
        {
            get { return m_StoreCode; }
            set { m_StoreCode = value; }
        }

        public int OriginalStoreCode
        {
            get { return m_OriginalStoreCode; }
            set { m_OriginalStoreCode = value; }
        }

        public string WebOrderNumber
        {
            get { return m_WebOrderNumber; }
            set { m_WebOrderNumber = value; }
        }

        public string WebOrderSuffix
        {
            get { return m_WebOrderSuffix; }
            set { m_WebOrderSuffix = value; }
        }

        public string this[int index]
        {
            get { return (string)DetailRecords[index]; }
        }

        public string AltCardNumber
        {
            get { return m_AltCardNumber; }
            set { m_AltCardNumber = value; }
        }

        public string AECardNumber
        {
            get { return m_AECardNumber; }
            set { m_AECardNumber = value; }
        }

        public string PurchaseDate
        {
            get { return m_PurchaseDate; }
            set { m_PurchaseDate = value; }
        }

        public string LastPurchaseDate
        {
            get { return m_LastPurchaseDate; }
            set { m_LastPurchaseDate = value; }
        }

        public string OriginalPurchaseDate
        {
            get { return m_OriginalPurchaseDate; }
            set { m_OriginalPurchaseDate = value; }
        }

        public string PurchaseTime
        {
            get { return m_PurchaseTime; }
            set { m_PurchaseTime = value; }
        }

        public decimal QualifiedPurchaseAmount
        {
            get { return m_QualifiedPurchaseAmount; }
            set { m_QualifiedPurchaseAmount = value; }
        }

        public decimal PurchaseDiscount
        {
            get { return m_PurchaseDiscount; }
            set { m_PurchaseDiscount = value; }
        }

        public int Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }
        public decimal ActualAmount
        {
            get { return m_ActualAmount; }
            set { m_ActualAmount = value; }
        }

        public decimal ReturnsTotal
        {
            get { return m_ReturnsTotal; }
            set { m_ReturnsTotal = value; }
        }

        public decimal GiftCardAmount
        {
            get { return m_GiftCardAmount; }
            set { m_GiftCardAmount = value; }
        }

        public int GiftCardQuantity
        {
            get { return m_GiftCardQuantity; }
            set { m_GiftCardQuantity = value; }
        }
        public int GiftCardMode
        {
            get { return m_GiftCardMode; }
            set { m_GiftCardMode = value; }
        }
        public decimal GiftCertificateAmount
        {
            get { return m_GiftCertificateAmount; }
            set { m_GiftCertificateAmount = value; }
        }


        public decimal Tax
        {
            get { return m_Tax; }
            set { m_Tax = value; }
        }

        public decimal TaxRate
        {
            get { return m_TaxRate; }
            set { m_TaxRate = value; }
        }

        public int TenderType
        {
            get { return m_TenderType; }
            set { m_TenderType = value; }
        }

        public int OriginalTenderType
        {
            get { return m_OriginalTenderType; }
            set { m_OriginalTenderType = value; }
        }

        public int TransType
        {
            get { return m_TransType; }
            set { m_TransType = value; }
        }

        public int NoDetailRecords
        {
            get { return DetailRecords.Count; }
        }

        public long TransactionID
        {
            get { return m_TransactionID; }
            set { m_TransactionID = value; }
        }

        public string TransactionNumber
        {
            get { return m_TransactionNumber; }
            set { m_TransactionNumber = value; }
        }

        public string OriginalTransactionNumber
        {
            get { return m_OriginalTransactionNumber; }
            set { m_OriginalTransactionNumber = value; }
        }

        public string OriginalTransactionInformation
        {
            get { return m_OriginalTransactionInformation; }
            set { m_OriginalTransactionInformation = value; }
        }
        public string RawTransactionString
        {
            get { return m_RawTransactionString; }
            set { m_RawTransactionString = value; }
        }

        public bool Refund
        {
            get { return m_Refund; }
            set { m_Refund = value; }
        }

        public bool GetCredits
        {
            get { return m_GetCredits; }
            set { m_GetCredits = value; }
        }

        public bool OrigGotCredits
        {
            get { return m_OrigGotCredits; }
            set { m_OrigGotCredits = value; }
        }
        public bool WebOrder
        {
            get { return webOrder; }
            set { webOrder = value; }
        }
        public bool PromotionItem
        {
            get { return m_PromotionItem; }
            set { m_PromotionItem = value; }
        }

        public string StoreCountry
        {
            get { return m_StoreCountry; }
            set { m_StoreCountry = value; }
        }

        public string ShipDate
        {
            get { return m_ShipDate; }
            set { m_ShipDate = value; }
        }

        public int EmployeeFlag
        {
            get { return m_EmployeeFlag; }
            set { m_EmployeeFlag = value; }
        }

        public int BrandID
        {
            get { return m_BrandID; }
            set { m_BrandID = value; }
        }

        public int ApplicationType
        {
            get { return m_ApplicationType; }
            set { m_ApplicationType = value; }
        }

        public int ProfileDataProcessed
        {
            get { return m_ProfileDataProcessed; }
            set { m_ProfileDataProcessed = value; }
        }

        public int OrderID
        {
            get { return m_OrderID; }
            set { m_OrderID = value; }
        }

        public int HighTier
        {
            get { return m_HighTier; }
            set { m_HighTier = value; }
        }
        public int RewardLevelID
        {
            get { return m_RewardLevelID; }
            set { m_RewardLevelID = value; }
        }
        public int StartingCredits
        {
            get { return m_StartingCredits; }
            set { m_StartingCredits = value; }
        }
        public int ExpiredCredits
        {
            get { return m_ExpiredCredits; }
            set { m_ExpiredCredits = value; }
        }
        public int RedeemedCredits
        {
            get { return m_RedeemedCredits; }
            set { m_RedeemedCredits = value; }
        }
        public int RolloverCredits
        {
            get { return m_RolloverCredits; }
            set { m_RolloverCredits = value; }
        }
        public int CreditsNeededForNextLevel
        {
            get { return m_CreditsNeededForNextLevel; }
            set { m_CreditsNeededForNextLevel = value; }
        }

        public string CreditStartDate
        {
            get { return m_CreditStartDate; }
            set { m_CreditStartDate = value; }
        }
        public string CreditEndDate
        {
            get { return m_CreditEndDate; }
            set { m_CreditEndDate = value; }
        }
        public string DateMailed
        {
            get { return m_DateMailed; }
            set { m_DateMailed = value; }
        }
        public string DateAwarded
        {
            get { return m_DateAwarded; }
            set { m_DateAwarded = value; }
        }
        public string DateRedeemed
        {
            get { return m_DateRedeemed; }
            set { m_DateRedeemed = value; }
        }
        public string DateReplaced
        {
            get { return m_DateReplaced; }
            set { m_DateReplaced = value; }
        }
        public string StatusRedeemed
        {
            get { return m_StatusRedeemed; }
            set { m_StatusRedeemed = value; }
        }
        public string StatusReplaced
        {
            get { return m_StatusReplaced; }
            set { m_StatusReplaced = value; }
        }
        public string OperatorID
        {
            get { return m_OperatorID; }
            set { m_OperatorID = value; }
        }
        public int NumberOfItemsReturned
        {
            get { return m_NumberOfItemsReturned; }
            set { m_NumberOfItemsReturned = value; }
        }
        public int NumberOfItemsPurchased
        {
            get { return m_NumberOfItemsPurchased; }
            set { m_NumberOfItemsPurchased = value; }
        }
        public string String77
        {
            get { return m_String77; }
            set { m_String77 = value; }
        }
        public int AltStoreCode
        {
            get { return m_AltStoreCode; }
            set { m_AltStoreCode = value; }
        }
        public string AltTransactionNumber
        {
            get { return m_AltTransactionNumber; }
            set { m_AltTransactionNumber = value; }
        }
        public string AltTransactionDate
        {
            get { return m_AltTransactionDate; }
            set { m_AltTransactionDate = value; }
        }
        public string AltOrderNumber
        {
            get { return m_AltOrderNumber; }
            set { m_AltOrderNumber = value; }
        }
        public string AltRegisterNumber
        {
            get { return m_AltRegisterNumber; }
            set { m_AltRegisterNumber = value; }
        }
        public decimal String181Amount
        {
            get { return m_String181Amount; }
            set { m_String181Amount = value; }
        }
        public decimal String182Amount
        {
            get { return m_String182Amount; }
            set { m_String182Amount = value; }
        }
        public int TransactionTotal
        {
            get { return m_TransactionTotal; }
            set { m_TransactionTotal = value; }
        }
        public int TransactionAmount
        {
            get { return m_TransactionAmount; }
            set { m_TransactionAmount = value; }
        }

        public string OldLoyaltyNumber
        {
            get { return m_OldLoyaltyNumber; }
            set { m_OldLoyaltyNumber = value; }
        }

        public string NewLoyaltyNumber
        {
            get { return m_NewLoyaltyNumber; }
            set { m_NewLoyaltyNumber = value; }
        }

        public void AddDetailRecord(string rawDetail)
        {
            DetailRecords.Add(rawDetail);
        }
    }
}
