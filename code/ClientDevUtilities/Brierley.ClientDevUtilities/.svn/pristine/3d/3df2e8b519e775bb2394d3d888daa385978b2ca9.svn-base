using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// This class defines an advertising offer in member's account.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberStore")]
	public class MemberStore : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the Id for the current MemberStore
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the StoreDefId for the current MemberStore
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(StoreDef), "StoreId")]
		public long StoreDefId { get; set; }

		/// <summary>
		/// Gets or sets the MemberId for the current MemberStore
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Member), "IpCode")]
        [ColumnIndex]
		public long MemberId { get; set; }

		/// <summary>
		/// Gets or sets the PreferenceOrder for the current MemberStore
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long PreferenceOrder { get; set; }

				/// <summary>
		/// Initializes a new instance of the MemberStore class
		/// </summary>
        public MemberStore()
		{
            CreateDate = DateTime.Now;
		}

	}
}