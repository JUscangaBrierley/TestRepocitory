using System;
using System.Diagnostics;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for LIBJob.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", autoIncrement = false)]
	[PetaPoco.TableName("LW_LIBRequestResponse")]
	public class SyncJob
	{
		private Stopwatch _watch;

		/// <summary>
		/// Gets or sets the MessageId for the current SyncJob
		/// </summary>
		[PetaPoco.Column("Id", IsNullable = false)]
		public long MessageId { get; set; }

		/// <summary>
		/// Gets or sets the Source for the current SyncJob
		/// </summary>
		[PetaPoco.Column(Length = 255)]
		public string Source { get; set; }

        [PetaPoco.Column(Length = 50)]
		public string SourceEnv { get; set; }

		[PetaPoco.Column]
		public long? ThreadId { get; set; }

        [PetaPoco.Column(Length = 50)]
		public string ExternalId { get; set; }

		/// <summary>
		/// Gets or sets the OperationName for the current SyncJob
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string OperationName { get; set; }

		/// <summary>
		/// Gets or sets the OperationParm for the current SyncJob
		/// </summary>
		[PetaPoco.Column]
		public string OperationParm { get; set; }

		/// <summary>
		/// Gets or sets the Status for the current SyncJob
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public long Status { get; set; }

		/// <summary>
		/// Gets or sets the StartTime for the current SyncJob
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public DateTime StartTime { get; internal set; }

		/// <summary>
		/// Gets or sets the EndTime for the current SyncJob
		/// </summary>
		[PetaPoco.Column]
		public DateTime? EndTime { get; internal set; }

		/// <summary>
		/// Gets or sets the Response for the current SyncJob
		/// </summary>
		[PetaPoco.Column]
		public string Response { get; set; }
		
		/// <summary>
		/// returns the elapsed time the job took to complete, in milliseconds
		/// </summary>
		/// <remarks>
		/// This must be used 
		/// </remarks>
		public long? ElapsedTime { get; private set; }

		/// <summary>
		/// Initializes a new instance of the LIBJob class
		/// </summary>
		public SyncJob()
		{
			Source = string.Empty;
			OperationName = string.Empty;
			OperationParm = string.Empty;
			StartTime = DateTime.Now;
			Response = string.Empty;
		}

		public void Start()
		{
			_watch = new Stopwatch();
			_watch.Start();
			StartTime = DateTime.Now;
		}

		public void End()
		{
			if (_watch == null)
			{
				throw new InvalidOperationException("SyncJob must first be started by calling Start() before calling End()");
			}
			_watch.Stop();
			EndTime = StartTime.AddTicks(_watch.ElapsedTicks);
			ElapsedTime = _watch.ElapsedMilliseconds;
		}
	}
}