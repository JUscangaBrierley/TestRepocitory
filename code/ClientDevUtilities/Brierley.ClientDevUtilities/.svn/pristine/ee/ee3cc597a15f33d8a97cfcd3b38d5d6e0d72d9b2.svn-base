using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for AsyncJob.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_LIBJobProcessedObjects")]
	public class AsyncJobProcessedObjects : LWCoreObjectBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string JobName { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public long JobNumber { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string ObjectName { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public long RowKey { get; set; }

		/// <summary>
		/// Initializes a new instance of the AsyncJob class
		/// </summary>
		public AsyncJobProcessedObjects()
		{
		}
	}
}