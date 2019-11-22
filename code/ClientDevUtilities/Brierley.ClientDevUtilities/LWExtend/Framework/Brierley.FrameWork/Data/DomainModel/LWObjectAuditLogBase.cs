using System;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
    public class LWObjectAuditLogBase
    {
        /// <summary>
        /// Unique identifier (PK) of the auditing record.
        /// </summary>
        [PetaPoco.Column("Ar_ID", IsNullable = false)]
		public Int64 Id { get; set; }
        
        /// <summary>
        /// It identifies the application where this data change took place:
        ///   CS:   CS Portal, 
        ///   CF:   Customer Facing web site, 
        ///   LN:   Loyalty Navigator, 
        ///   AP:   API,
        ///   DA:   DAP, 
        ///   RU:   Rule Processor, 
        ///   OT:   Others
        /// </summary>
        [PetaPoco.Column(Length = 4, PersistEnumAsString = true, IsNullable = false)]
		public ApplicationType Ar_AppType { get; set; }
        
        /// <summary>
        /// It identifies the name of the machine where the application is hosted.
        /// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = true)]
		public string Ar_HostName { get; set; }
                
        /// <summary>
        /// It identifies the unique ID of the user who initiated the data change:
        ///   For CS Portal user:  the CS agent’s user name is logged;
        ///   For Customer Facing web site user:  Loyalty member’s ipcode is logged;
        ///   For Loyalty Navigator user:  Loyalty Navigator user’s login name is logged;
        ///   For API:   ‘API’ is logged;
        ///   For DAP:  ‘DAP’ is logged;
        ///   For Rule Processor:  ‘RuleProcessor’ is logged;
        ///   Others:   ‘(Unknown)’ is logged
        /// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public string Ar_OwnerID { get; set; }
        
        /// <summary>
        /// It identifies the nature of the data change:
        ///   U:  a record has been updated
        ///   I:  a record has been inserted
        ///   D:  a record has been deleted
        /// </summary>
        [PetaPoco.Column(Length = 2, PersistEnumAsString = true, IsNullable = false)]
		public DataChangeType Ar_ChangeType { get; set; }
        
        /// <summary>
        /// It identifies the date and time the data change occurred.
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DateTime Ar_CreatedOn { get; set; }
    }
}