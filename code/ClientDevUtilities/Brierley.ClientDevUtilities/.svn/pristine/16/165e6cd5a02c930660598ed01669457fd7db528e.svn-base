using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Contact_Status_Key", autoIncrement = false)]
    [PetaPoco.TableName("LW_ContactStatus")]
    public class ContactStatus
    {
        [PetaPoco.Column("Contact_Status_Key", IsNullable = false)]
        public long ID { get; set; }
        [PetaPoco.Column("CS_Descr", Length = 500)]
        public string CSDescr { get; set; }
        [PetaPoco.Column("CS_Source", Length = 500)]
        public string CSSource { get; set; }
        [PetaPoco.Column("CS_Code", Length = 500)]
        public string CSCode { get; set; }

		public ContactStatus()
		{
			ID = -1;
		}
	}
}
