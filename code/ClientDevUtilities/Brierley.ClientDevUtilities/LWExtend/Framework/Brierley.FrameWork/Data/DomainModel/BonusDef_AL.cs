//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// This class defines an advertising message.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_BonusDef")]
	public class BonusDef_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }
        [PetaPoco.Column(Length = 100, IsNullable = false)]
		public string Name { get; set; }
        [PetaPoco.Column(IsNullable = false)]
		public long CategoryId { get; set; }
        [PetaPoco.Column]
		public long? FolderId { get; set; }
        [PetaPoco.Column(Length = 255)]
		public string LogoImageHero { get; set; }
        [PetaPoco.Column(Length = 255)]
		public string LogoImageWeb { get; set; }
        [PetaPoco.Column(Length = 255)]
		public string LogoImageMobile { get; set; }
        [PetaPoco.Column]
		public decimal? Points { get; set; }
        [PetaPoco.Column(Length = 25)]
		public string SurveyText { get; set; }
        [PetaPoco.Column]
		public long? SurveyId { get; set; }
        [PetaPoco.Column]
		public string SurveyPointsExpression { get; set; }
        [PetaPoco.Column(Name = "ActionUrl", Length = 255)]
		public string MovieUrl { get; set; }
        [PetaPoco.Column(Length = 255)]
		public string ReferralUrl { get; set; }
        [PetaPoco.Column]
		public int? DisplayOrder { get; set; }
        [PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }
        [PetaPoco.Column]
		public DateTime? ExpiryDate { get; set; }
        [PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
        [PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
        [PetaPoco.Column]
		public long? Quota { get; set; }
        [PetaPoco.Column]
		public bool? ApplyQuotaToReferral { get; set; }
        [PetaPoco.Column]
		public long? Completed { get; set; }
	}
}
