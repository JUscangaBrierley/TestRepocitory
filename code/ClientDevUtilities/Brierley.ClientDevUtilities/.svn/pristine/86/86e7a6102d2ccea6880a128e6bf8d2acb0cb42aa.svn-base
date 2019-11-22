//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// This class defines a coupon.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CouponDef")]
    [AuditLog(true)]
	public class CouponDef : ContentDefBase
	{
		/// <summary>
		/// Gets or sets the Name for the current CouponDef
		/// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = false)]
		public string Name { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public bool IsGlobal { get; set; }

		/// <summary>
		/// Gets or sets the CategoryId for the current CouponDef
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Category), "ID")]
		public long CategoryId { get; set; }

		/// <summary>
		/// Gets or sets the CouponTypeCode for the current CouponDef
		/// </summary>
		[PetaPoco.Column(Length = 50)]
		public string CouponTypeCode { get; set; }

		/// <summary>
		/// Gets or sets the folder id for the current CouponDef
		/// </summary>
		[PetaPoco.Column]
		[ColumnIndex]
		public long? FolderId { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public long PassDefId { get; set; }

		/// <summary>
		/// Gets or sets the LogoFileName for the current CouponDef
		/// </summary>
		[PetaPoco.Column(Length = 255)]
		public string LogoFileName { get; set; }

		/// <summary>
		/// Gets or sets the ShortDescription for the current CouponDef
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
		/// Gets or sets the Description for the current CouponDef
		/// </summary>
		public string Description
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Description");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Description", value);
			}
		}

		/// <summary>
		/// Gets or sets the StartDate for the current CouponDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Gets or sets the ExpiryDate for the current CouponDef
		/// </summary>
        [PetaPoco.Column(IsNullable = true)]
		public DateTime? ExpiryDate { get; set; }

		/// <summary>
		/// Gets or sets the UsesAllowed for the current CouponDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long UsesAllowed { get; set; }

		[PetaPoco.Column]
		public int? UsesPerYear { get; set; }

		[PetaPoco.Column]
		public int? UsesPerMonth { get; set; }

		[PetaPoco.Column]
		public int? UsesPerWeek { get; set; }

		[PetaPoco.Column]
		public int? UsesPerDay { get; set; }

        [PetaPoco.Column(IsNullable = true)]
		public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the PushNotificationId associated with the current CouponDef
        /// </summary>
        [PetaPoco.Column]
        public long? PushNotificationId { get; set; }

		/// <summary>
		/// Initializes a new instance of the CouponDef class
		/// </summary>
		public CouponDef()
			: base(ContentObjType.Coupon)
		{
			StartDate = DateTime.Now;
			UsesAllowed = 1;
		}

		public string GetShortDescription(string language, string channel)
		{
			return GetContent(language, channel, "ShortDescription");
		}

		public string GetDescription(string language, string channel)
		{
			return GetContent(language, channel, "Description");
		}

		public void SetShortDescription(string language, string channel, string description)
		{
			SetContent(language, channel, "ShortDescription", description);
		}

		public void SetDescription(string language, string channel, string description)
		{
			SetContent(language, channel, "Description", description);
		}

		public bool IsActive()
		{
			if (this.StartDate > DateTime.Now)
			{
				// the coupon is not effective yet.
				return false;
			}

			if (!this.ExpiryDate.HasValue)
			{
				// there is no expiration date
				return true;
			}
			else if (this.ExpiryDate > DateTime.Now)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public CouponDef Clone()
		{
			return Clone(new CouponDef());
		}

		public CouponDef Clone(CouponDef dest)
		{
			dest.Name = Name;
            dest.IsGlobal = IsGlobal;
			dest.CategoryId = CategoryId;
			dest.FolderId = FolderId;
			dest.CouponTypeCode = CouponTypeCode;
			dest.LogoFileName = LogoFileName;
			dest.ShortDescription = ShortDescription;
			dest.Description = Description;
			dest.StartDate = StartDate;
			dest.ExpiryDate = ExpiryDate;
			dest.UsesAllowed = UsesAllowed;
			dest.DisplayOrder = DisplayOrder;

			dest.PassDefId = PassDefId;

			return (CouponDef)base.Clone(dest);
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			CouponDef_AL ar = new CouponDef_AL()
			{
				ObjectId = this.Id,
				PassDefId = this.PassDefId,
				Name = this.Name,
                IsGlobal = this.IsGlobal,
				CategoryId = this.CategoryId,
				FolderId = this.FolderId,
				CouponTypeCode = this.CouponTypeCode,
				LogoFileName = this.LogoFileName,
				StartDate = this.StartDate,
				ExpiryDate = this.ExpiryDate,
				UsesAllowed = this.UsesAllowed,
				UsesPerYear = this.UsesPerYear, 
				UsesPerMonth = this.UsesPerMonth, 
				UsesPerWeek = this.UsesPerWeek, 
				UsesPerDay = this.UsesPerDay, 
				DisplayOrder = this.DisplayOrder,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}
