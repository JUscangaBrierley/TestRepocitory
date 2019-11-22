using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for LIBMessageLog.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_LIBMessageLog")]
	public class LIBMessageLog
	{
		/// <summary>
		/// Initializes a new instance of the LIBMessageLog class
		/// </summary>
		public LIBMessageLog()
		{
			MsgTime = DateTime.Now;
		}

		/// <summary>
		/// Gets or sets the MessageId for the current LIBMessageLog
		/// </summary>
		[PetaPoco.Column("Id", IsNullable = false)]
		public long MessageId { get; set; }

		/// <summary>
		/// Gets or sets the EnvKey for the current LIBMessageLog
		/// </summary>
        [PetaPoco.Column(Length = 150, IsNullable = false)]
		public string EnvKey { get; set; }

		/// <summary>
		/// Gets or sets the LogSource for the current LIBMessageLog
		/// </summary>
        [PetaPoco.Column(Length = 150, IsNullable = false)]
		public string LogSource { get; set; }

		/// <summary>
		/// Gets or sets the FileName for the current LIBMessageLog
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets the JobNumber for the current LIBMessageLog
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long JobNumber { get; set; }

		/// <summary>
		/// Gets or sets the Message for the current LIBMessageLog
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the Reason for the current LIBMessageLog
		/// </summary>
        [PetaPoco.Column(Length = 500)]
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the Error for the current LIBMessageLog
		/// </summary>
        [PetaPoco.Column]
		public string Error { get; set; }

		/// <summary>
		/// Gets or sets the TryCount for the current LIBMessageLog
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long TryCount { get; set; }

		/// <summary>
		/// Gets or sets the MsgTime for the current LIBMessageLog
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DateTime MsgTime { get; set; }

		public Exception Exception { get; set; }
	}
}