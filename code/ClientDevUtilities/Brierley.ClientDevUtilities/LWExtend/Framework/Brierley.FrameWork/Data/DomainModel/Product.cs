using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for Product. This class is autogenerated
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_Product")]
    [AuditLog(true)]
    public class Product : ContentDefBase
	{
		/// <summary>
		/// Initializes a new instance of the Product class
		/// </summary>
		public Product() : base(ContentObjType.Product)
		{
			CreateDate = Brierley.FrameWork.Common.DateTimeUtil.MinValue;
		}


        /// <summary>
        /// Gets or sets the CategoryId for the current Product
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Category), "ID")]
        [ColumnIndex]
        public long CategoryId { get; set; }

        public Category ProductCategory { get; set; }

        /// <summary>
        /// Gets or sets the IsVisibleInLn for the current Product
        /// </summary>
        [PetaPoco.Column]
        public bool? IsVisibleInLn { get; set; }

        /// <summary>
        /// Gets or sets the Name for the current Product
        /// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the BrandName for the current Product
        /// </summary>
        [PetaPoco.Column(Length = 50)]
        public string BrandName { get; set; }

        /// <summary>
        /// Gets or sets the PartNumber for the current Product
        /// </summary>
        [PetaPoco.Column(Length = 100)]
        [ColumnIndex]
        public string PartNumber { get; set; }

        /// <summary>
        /// Gets or sets the ShortDescription for the current Product
        /// </summary>
        public string ShortDescription
        {
            get
            {
                return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "ShortDescription");
            }
            set
            {
                SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "ShortDescription", value);
            }
        }

        /// <summary>
        /// Gets or sets the LongDescription for the current Product
        /// </summary>
        public string LongDescription
        {
            get
            {
                return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "LongDescription");
            }
            set
            {
                SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "LongDescription", value);
            }
        }


        /// <summary>
        /// Gets or sets the BasePrice for the current Product
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Gets or sets the PointType for the current Product
        /// </summary>
        [PetaPoco.Column]
        [ForeignKey(typeof(PointType), "ID")]
        public long? PointType { get; set; }

        /// <summary>
        /// Gets or sets the Quantity for the current Product
        /// </summary>
        [PetaPoco.Column]
        public long? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the Threshold for the current Product
        /// </summary>
        [PetaPoco.Column]
        public long? QuantityThreshold { get; set; }

        [PetaPoco.Column(Length = 50)]
        public string ClassCode { get; set; }

        [PetaPoco.Column(Length = 100)]
        public string ClassDescription { get; set; }

        [PetaPoco.Column(Length = 50)]
        public string StyleCode { get; set; }

        [PetaPoco.Column(Length = 100)]
        public string StyleDescription { get; set; }

        [PetaPoco.Column(Length = 50)]
        public string DeptCode { get; set; }

        [PetaPoco.Column(Length = 100)]
        public string DeptDescription { get; set; }

        [PetaPoco.Column(Length = 50)]
        public string DivisionCode { get; set; }

        [PetaPoco.Column(Length = 100)]
        public string DivisionDescription { get; set; }

        [PetaPoco.Column(Length = 50)]
        public string CompanyCode { get; set; }

        [PetaPoco.Column(Length = 100)]
        public string CompanyDescription { get; set; }

        /// <summary>
        /// Gets or sets the CreatedByUser for the current Product
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long CreatedByUser { get; set; }

        [PetaPoco.Column(Length = 100)]
        public string StrUserField { get; set; }

        [PetaPoco.Column]
        public long? LongUserField { get; set; }
        
		public virtual List<ProductVariant> GetVariants()
		{
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				return content.GetAllProductVariantsByProduct(Id);
			}
		}

        public virtual Product Clone()
        {
            return Clone(new Product());
        }

        public virtual Product Clone(Product dest)
        {
            dest.CategoryId = CategoryId;
			dest.ProductCategory = ProductCategory;
			dest.IsVisibleInLn = IsVisibleInLn;
            dest.Name = Name;
			dest.BrandName = BrandName;
            dest.PartNumber = PartNumber;
			dest.ShortDescription = ShortDescription;
			dest.LongDescription = LongDescription;
            dest.BasePrice = BasePrice;
            dest.PointType = PointType;
            dest.Quantity = Quantity;
            dest.QuantityThreshold = QuantityThreshold;
			dest.ClassCode = ClassCode;
			dest.ClassDescription = ClassDescription;
			dest.StyleCode = StyleCode;
			dest.StyleDescription = StyleDescription;
			dest.DeptCode = DeptCode;
			dest.DeptDescription = DeptDescription;
			dest.DivisionCode = DivisionCode;
			dest.DivisionDescription = DivisionDescription;
			dest.CompanyCode = CompanyCode;
			dest.CompanyDescription = CompanyDescription;
			dest.CreatedByUser = CreatedByUser;
			dest.StrUserField = StrUserField;
			dest.LongUserField = LongUserField;
            dest.CreateDate = CreateDate;
            return (Product)base.Clone(dest);            
        }

        public virtual string GetShortDescription(string language, string channel)
		{
            return GetContent(language, channel, "ShortDescription");
		}

        public virtual string GetLongDescription(string language, string channel)
		{
            return GetContent(language, channel, "LongDescription");
		}

        public virtual void SetShortDescription(string language, string channel, string shortDescription)
		{
            SetContent(language, channel, "ShortDescription", shortDescription);
		}

        public virtual void SetLongDescription(string language, string channel, string longDescription)
		{
            SetContent(language, channel, "LongDescription", longDescription);
		}

        public override LWObjectAuditLogBase GetArchiveObject()
        {
            Product_AL ar = new Product_AL()
            {
                ObjectId = this.Id,
                CategoryId = this.CategoryId,
                IsVisibleInLn = this.IsVisibleInLn,
                Name = this.Name,
                BrandName = this.BrandName,
                PartNumber = this.PartNumber,
                BasePrice = this.BasePrice,
                PointType = this.PointType,
                Quantity = this.Quantity,
                QuantityThreshold = this.QuantityThreshold,
                ClassCode = this.ClassCode,
                ClassDescription = this.ClassDescription,
                StyleCode = this.StyleCode,
                StyleDescription = this.StyleDescription,
                DeptCode = this.DeptCode,
                DeptDescription = this.DeptDescription,
                DivisionCode = this.DivisionCode,
                DivisionDescription = this.DivisionDescription,
                CompanyCode = this.CompanyCode,
                CompanyDescription = this.CompanyDescription,
                CreatedByUser = this.CreatedByUser,
                StrUserField = this.StrUserField,
                LongUserField = this.LongUserField,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate
            };
            return ar;
        }
	}
}