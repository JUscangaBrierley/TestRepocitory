using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// A batch of structured content.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence", autoIncrement = false)]
	[PetaPoco.TableName("LW_Batch")]
	public class Batch : LWCoreObjectBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string Name { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime EndDate { get; set; }

		/// <summary>
		/// Initializes a new instance of the Batch class
		/// </summary>
		public Batch()
		{
			ID = -1;
			Name = string.Empty;
			StartDate = DateTimeUtil.MinValue;
			EndDate = DateTimeUtil.MaxValue;
		}

		public Batch Clone()
		{
			return Clone(new Batch());
		}

		public Batch Clone(Batch other)
		{
			other.ID = ID;
			other.Name = Name;
			other.StartDate = StartDate;
			other.EndDate = EndDate;
			return (Batch)base.Clone(other);
		}
	}
}
