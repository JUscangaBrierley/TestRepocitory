using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_AWLoyaltyCard")]
    public class AppleWalletLoyaltyCard : ContentDefBase
    {
        private const string _channel = "Mobile";

        public AppleWalletLoyaltyCard() : base(ContentObjType.LoyaltyCard)
        {
            BarcodeEncoding = "iso-8859-1";
            ExtendedFields = new List<AppleWalletExtendedField>();
        }

        [PetaPoco.Column]
        public string LoyaltyCardSelectionCriteria { get; set; }

        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Name { get; set; }

        //Icon image
        [PetaPoco.Column(Length = 500)]
        public string IconImageX1 { get; set; }
        [PetaPoco.Column(Length = 500)]
        public string IconImageX2 { get; set; }
        [PetaPoco.Column(Length = 500)]
        public string IconImageX3 { get; set; }

        //Logo image
        [PetaPoco.Column(Length = 500)]
        public string LogoImageX1 { get; set; }
        [PetaPoco.Column(Length = 500)]
        public string LogoImageX2 { get; set; }
        [PetaPoco.Column(Length = 500)]
        public string LogoImageX3 { get; set; }

        //Strip image
        [PetaPoco.Column(Length = 500)]
        public string StripImageX1 { get; set; }
        [PetaPoco.Column(Length = 500)]
        public string StripImageX2 { get; set; }
        [PetaPoco.Column(Length = 500)]
        public string StripImageX3 { get; set; }

        //Barcode - format, encoding, message, alt text
        [PetaPoco.Column(PersistEnumAsString = false, IsNullable = false)]
        public AppleWalletBarcodeFormat BarcodeFormat { get; set; }
        [PetaPoco.Column(Length = 100)]
        public string BarcodeEncoding { get; set; }
        [PetaPoco.Column]
        public string BarcodeMessage { get; set; }
        [PetaPoco.Column]
        public string BarcodeAlternateText { get; set; }

        //Foreground color
        [PetaPoco.Column(Length = 25)]
        public string ForegroundColor { get; set; }

        //Background color
        [PetaPoco.Column(Length = 25)]
        public string BackgroundColor { get; set; }

        //Logo text
        public string GetLogoText(string language)
        {
            return GetContent(language, _channel, "LogoText");
        }
        public void SetLogoText(string language, string content)
        {
            SetContent(language, _channel, "LogoText", content);
        }
        
        //Description
        public string GetDescription(string language)
        {
            return GetContent(language, _channel, "Description");
        }
        public void SetDescription(string language, string content)
        {
            SetContent(language, _channel, "Description", content);
        }

        //  -- The next group needs a value and (optionally) a label. The label is translated text while the value is bscript
        //Header fields(up to 3)
        //Primary field (1)
        //Back fields (unlimited, for things like T's&C's)
        public List<AppleWalletExtendedField> ExtendedFields { get; set; }

        /*
        Additional payload items (not visible to the member):
            Format version -- always 1
            Team ID -- Framework config
            Web service URL -- Framework config
            Organization name -- Framework config
            Locations -- Member stores
        */

        public AppleWalletLoyaltyCard Clone()
        {
            return Clone(new AppleWalletLoyaltyCard());
        }

        public AppleWalletLoyaltyCard Clone(AppleWalletLoyaltyCard dest)
        {
            dest.BackgroundColor = BackgroundColor;
            dest.BarcodeAlternateText = BarcodeAlternateText;
            dest.BarcodeEncoding = BarcodeEncoding;
            dest.BarcodeFormat = BarcodeFormat;
            dest.BarcodeMessage = BarcodeMessage;
            dest.ForegroundColor = ForegroundColor;
            dest.IconImageX1 = IconImageX1;
            dest.IconImageX2 = IconImageX2;
            dest.IconImageX3 = IconImageX3;
            dest.LogoImageX1 = LogoImageX1;
            dest.LogoImageX2 = LogoImageX2;
            dest.LogoImageX3 = LogoImageX3;
            dest.LoyaltyCardSelectionCriteria = LoyaltyCardSelectionCriteria;
            dest.Name = Name;
            dest.StripImageX1 = StripImageX1;
            dest.StripImageX2 = StripImageX2;
            dest.StripImageX3 = StripImageX3;

            return (AppleWalletLoyaltyCard)base.Clone(dest);
        }
    }

    public enum AppleWalletBarcodeFormat
    {
        PKBarcodeFormatQR,
        PKBarcodeFormatPDF417,
        PKBarcodeFormatAztec,
        PKBarcodeFormatCode128
    }
}
