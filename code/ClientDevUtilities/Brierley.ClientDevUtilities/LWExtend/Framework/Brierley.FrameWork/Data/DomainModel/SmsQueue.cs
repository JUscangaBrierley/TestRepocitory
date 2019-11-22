using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Class used for queue failed email sends.
	/// </summary>
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SmsQueue")]
    public class SmsQueue : LWCoreObjectBase
    {
		/// <summary>
		/// The queued item's unique ID in the framework database.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The ID of the email which this send belongs to.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long SmsID { get; set; }

		/// <summary>
		/// The type of message that was sent
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public CommunicationType MessageType { get; set; }

		/// <summary>
		/// The fields for the send.
		/// </summary>
        [PetaPoco.Column]
        public string Records { get; set; }

		/// <summary>
		/// Reason for the last send failure
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public SmsFailureType SmsFailureType { get; set; }

		/// <summary>
		/// # times email has been sends have been attempted
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long SendAttempts { get; set; }

		/// <summary>
		/// Date of last send attempt
		/// </summary>
        [PetaPoco.Column]
        public DateTime? LastSendAttempt { get; set; }

		public SmsQueue()
		{
			ID = -1;
			SmsID = -1;
            SmsFailureType = SmsFailureType.Unknown;
		}
    }
}
