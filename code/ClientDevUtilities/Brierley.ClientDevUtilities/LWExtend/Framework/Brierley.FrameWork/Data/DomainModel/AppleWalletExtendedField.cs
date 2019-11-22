using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_AWExtendedField")]
    [UniqueIndex(ColumnName = "AppleWalletLoyaltyCardId,FieldType,DisplayOrder", RequiresLowerFunction = false)]
    public class AppleWalletExtendedField : ContentDefBase
    {
        private const string _channel = "Mobile";

        public AppleWalletExtendedField() : base(ContentObjType.LoyaltyCard)
        {

        }

        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(AppleWalletLoyaltyCard), "Id")]
        public long AppleWalletLoyaltyCardId { get; set; }

        [PetaPoco.Column(Length = 255)]
        [ColumnIndex(RequiresLowerFunction = true)]
        public string Name { get; set; }

        [PetaPoco.Column(PersistEnumAsString = false, IsNullable = false)]
        public AppleWalletExtendedFieldType FieldType { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public int DisplayOrder { get; set; }

        public string GetLabel(string language)
        {
            return GetContent(language, _channel, "Label");
        }

        public void SetLabel(string language, string value)
        {
            SetContent(language, _channel, "Label", value);
        }

        public string GetValue(string language)
        {
            return GetContent(language, _channel, "Value");
        }

        public void SetValue(string language, string value)
        {
            SetContent(language, _channel, "Value", value);
        }

        public AppleWalletExtendedField Clone()
        {
            return Clone(new AppleWalletExtendedField());
        }

        public AppleWalletExtendedField Clone(AppleWalletExtendedField dest)
        {
            dest.DisplayOrder = DisplayOrder;
            dest.FieldType = FieldType;
            dest.Name = Name;

            return (AppleWalletExtendedField)base.Clone(dest);
        }
    }

    public enum AppleWalletExtendedFieldType
    {
        PrimaryField,
        SecondaryField,
        AuxilliaryField,
        HeaderField,
        BackField
    }
}
