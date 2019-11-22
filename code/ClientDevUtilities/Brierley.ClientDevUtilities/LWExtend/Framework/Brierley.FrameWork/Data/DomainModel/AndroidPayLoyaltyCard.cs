using Brierley.FrameWork.Data.ModelAttributes;
using Google.Apis.Walletobjects.v1.Data;
using System.Xml.Serialization;

namespace Brierley.FrameWork.Data.DomainModel
{
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_APLoyaltyCard")]
    public class AndroidPayLoyaltyCard : ContentDefBase
    {
        public AndroidPayLoyaltyCard() : base(Common.ContentObjType.LoyaltyCard)
        {
        }

        [PetaPoco.Column]
        public string LoyaltyCardSelectionCriteria { get; set; }

        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Name { get; set; }

        // Logo
        [PetaPoco.Column(Length = 500)]
        public string ProgramLogoImage { get; set; }

        [PetaPoco.Column(Length = 100)]
        public string ProgramName { get; set; }

        [PetaPoco.Column(Length = 100)]
        public string IssuerName { get; set; }
        
        // Strip image
        [PetaPoco.Column(Length = 500)]
        public string MainImage { get; set; }
        
        // Foreground color
        [PetaPoco.Column(Length = 25)]
        public string ForegroundColor { get; set; }
        
        // Background color
        [PetaPoco.Column(Length = 25)]
        public string BackgroundColor { get; set; }
        // Lock screen image/icon? Not found

        // Barcode type, value, alternateText
        [PetaPoco.Column(PersistEnumAsString = false, IsNullable = false)]
        public AndroidPayBarcodeFormat BarcodeType { get; set; }
        [PetaPoco.Column]
        public string BarcodeValue { get; set; }
        [PetaPoco.Column]
        public string BarcodeAlternateText { get; set; }

        // Loyalty currency name and label
        [PetaPoco.Column(Length = 100)]
        public string LoyaltyCurrencyLabel { get; set; }

        [PetaPoco.Column]
        public string LoyaltyCurrencyValueExpression { get; set; }

        // Rewards tier name and label
        [PetaPoco.Column(Length = 100)]
        public string RewardTierLabel { get; set; }
        [PetaPoco.Column(Length = 100)]
        public string RewardTierName { get; set; }

        // Account name
        [PetaPoco.Column(Length = 100)]
        public string AccountNameLabel { get; set; }

        [PetaPoco.Column]
        public string AccountNameValue { get; set; }

        // Account Id
        [PetaPoco.Column(Length = 100)]
        public string AccountIdLabel { get; set; }
        [PetaPoco.Column]
        public string AccountIdValue { get; set; }

        // Legal copy
        [PetaPoco.Column]
        public string LegalCopy { get; set; }

        // Locations
        // More: messages, text, info, links, images

        [XmlIgnore]
        public LoyaltyClass LoyaltyClass { get; set; }

        public AndroidPayLoyaltyCard Clone()
        {
            return Clone(new AndroidPayLoyaltyCard());
        }

        public AndroidPayLoyaltyCard Clone(AndroidPayLoyaltyCard dest)
        {
            dest.AccountIdLabel = AccountIdLabel;
            dest.AccountIdValue = AccountIdValue;
            dest.AccountNameLabel = AccountNameLabel;
            dest.AccountNameValue = AccountNameValue;
            dest.BackgroundColor = BackgroundColor;
            dest.BarcodeAlternateText = BarcodeAlternateText;
            dest.BarcodeType = BarcodeType;
            dest.BarcodeValue = BarcodeValue;
            dest.ForegroundColor = ForegroundColor;
            dest.IssuerName = IssuerName;
            dest.LegalCopy = LegalCopy;
            dest.LoyaltyCardSelectionCriteria = LoyaltyCardSelectionCriteria;
            dest.LoyaltyCurrencyLabel = LoyaltyCurrencyLabel;
            dest.LoyaltyCurrencyValueExpression = LoyaltyCurrencyValueExpression;
            dest.MainImage = MainImage;
            dest.Name = Name;
            dest.ProgramLogoImage = ProgramLogoImage;
            dest.ProgramName = ProgramName;
            dest.RewardTierLabel = RewardTierLabel;
            dest.RewardTierName = RewardTierName;

            return (AndroidPayLoyaltyCard)base.Clone(dest);
        }
    }

    public enum AndroidPayBarcodeFormat
    {
        aztec,
        codabar,
        code128,
        code39,
        dataMatrix,
        ean13,
        ean8,
        itf14,
        pdf417,
        pdf417Compact,
        qrCode,
        textOnly,
        upcA,
        upcE
    }
}
