//-----------------------------------------------------------------------
//(C) 2013 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Class used to associate an email to a LoyaltyWare content.
	/// </summary>
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_EmailAssociation")]
    public class EmailAssociation : LWCoreObjectBase
    {
        /// <summary>
		/// The item's unique ID in the framework database.
		/// </summary>
        [PetaPoco.Column("ID", IsNullable = false)]
        public long Id { get; set; }
		
        /// <summary>
        /// The unique id of the email queue row
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long EmailQueueId { get; set; }

        /// <summary>
        /// This describes the owenr type of the loyaltyware content artifact.
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public PointTransactionOwnerType OwnerType { get; set; }

        /// <summary>
        /// This is the definition id of the loyalty ware artifact.
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long OwnerId { get; set; }

        /// <summary>
        /// This is the unique id of the loyalty wallet row
        /// </summary>
        [PetaPoco.Column]
        public long? RowKey { get; set; }


        public EmailAssociation()
		{
		}
    }
}
