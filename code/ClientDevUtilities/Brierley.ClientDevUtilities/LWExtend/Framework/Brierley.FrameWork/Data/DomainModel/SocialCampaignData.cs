using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SocialCampaignData")]
    public class SocialCampaignData : LWCoreObjectBase
    {
        #region properties
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long MemberId { get; set; }
        [PetaPoco.Column(Length = 50)]
        public string ContentId { get; set; }
        [PetaPoco.Column(Length = 896, IsNullable = false)]
        public string ActorId { get; set; }
        [PetaPoco.Column(Length = 150, IsNullable = false)]
		public string ActorName { get; set; }
        [PetaPoco.Column(Length = 500)]
        public string ActorUserName { get; set; }
        [PetaPoco.Column(Length = 500)]
        public string Keywords { get; set; }        
		[PetaPoco.Column(IsNullable = false)]
        public DateTime PostedOn { get; set; }
        [PetaPoco.Column(Length = 25, IsNullable = false, PersistEnumAsString = true)]
        public SocialNetworkProviderType Publisher { get; set; }
        [PetaPoco.Column(Length = 25, IsNullable = false, PersistEnumAsString = true)]
        public SocialSentiment Sentiment { get; set; }
        [PetaPoco.Column]
        public float? KloutScore { get; set; }
        #endregion        
	}
}
