using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Class used for email resend grid.
	/// </summary>
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
    public class EmailQueueSummaryItem
    {
		/// <summary>
		/// The row number in the result
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The ID of the email which this send belongs to.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long EmailID { get; set; }

		/// <summary>
		/// The name of the email which this send belongs to.
		/// </summary>
		[PetaPoco.Column]
		public string Name { get; set; }

		/// <summary>
		/// The type of message that was sent
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public CommunicationType MessageType { get; set; }

		/// <summary>
		/// Reason for the last send failure
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public EmailFailureType FailureReason { get; set; }

		/// <summary>
		/// The count of queued sends.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long SendCount { get; set; }

        public EmailQueueSummaryItem()
		{
			ID = -1;
			EmailID = -1;
			FailureReason = EmailFailureType.Unknown;
		}
    }
}
