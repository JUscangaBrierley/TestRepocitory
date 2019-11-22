using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberSocNet")]
	public class MemberSocNet : LWCoreObjectBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        [ColumnIndex]
		public long MemberId { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public SocialNetworkProviderType ProviderType { get; set; }

        [PetaPoco.Column(Length = 896, IsNullable = false)]
		public string ProviderUID { get; set; }

        [PetaPoco.Column]
		public string Properties { get; set; }

		public MemberSocNet()
		{
			Id = -1;
			MemberId = -1;
			ProviderType = SocialNetworkProviderType.None;
			ProviderUID = string.Empty;
		}
	}
}