using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("ContactHistoryID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_ContactHistory")]
    public class ContactHistory : LWCoreObjectBase
    {
        [PetaPoco.Column("ContactHistoryID", IsNullable = false)]
        public long ID { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        [UniqueIndex]
        public long CDWKey { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long CampaignKey { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string CampaignName { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long IPCode { get; set; }
        [PetaPoco.Column(Length = 12)]
        public string LoyaltyID { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long ContactStatusKey { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public ContactTypeEnum ContactType { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public DateTime ContactDate { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string MailingID { get; set; }
        [PetaPoco.Column(Length = 254)]
        public string EmailAddress { get; set; }
        [PetaPoco.Column(Length = 15)]
        public string MobileNbr { get; set; }
        [PetaPoco.Column(Length = 5)]
        public string CellCode { get; set; }
        [PetaPoco.Column(Length = 1)]
        public string Clicks { get; set; }
        [PetaPoco.Column(Length = 1)]
        public string Opens { get; set; }
        [PetaPoco.Column(Length = 1)]
        public string Conversion { get; set; }
        [PetaPoco.Column(Length = 2000)]
        public string EmailLink { get; set; }

		public ContactHistory()
		{
			ID = -1;
			CampaignKey = -1;
			IPCode = -1;
			ContactType = ContactTypeEnum.Email;
		}
	}
}
