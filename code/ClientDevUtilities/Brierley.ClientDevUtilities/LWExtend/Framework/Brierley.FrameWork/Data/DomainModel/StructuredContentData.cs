using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// A structured content datum.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "SEQ_STRUCUREDDATA")]
	[PetaPoco.TableName("LW_StructuredContentData")]
	public class StructuredContentData : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the ID for the current StructuredContentData
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the BatchId for the current StructuredContentData
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Batch), "ID")]
		public long BatchID { get; set; }

		/// <summary>
		/// Gets or sets the SequenceID for the current StructuredContentData
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long SequenceID { get; set; }

		/// <summary>
		/// Gets or sets the AttributeId for the current StructuredContentData
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(StructuredContentAttribute), "ID")]
        [ColumnIndex]
		public long AttributeID { get; set; }

		/// <summary>
		/// Gets or sets the Data for the current StructuredContentData
		/// </summary>
        [PetaPoco.Column]
        //[ColumnIndex]  // BME: Commenting out for now since this index doesn't make much sense
        public string Data { get; set; }

		/// <summary>
		/// Initializes a new instance of the StructuredContentData class
		/// </summary>
		public StructuredContentData()
		{
			ID = -1;
			BatchID = -1;
			SequenceID = -1;
			AttributeID = -1;
			Data = string.Empty;
		}

		public StructuredContentData Clone()
		{
			return Clone(new StructuredContentData());
		}

		public StructuredContentData Clone(StructuredContentData other)
		{
			other.BatchID = BatchID;
			other.SequenceID = SequenceID;
			other.AttributeID = AttributeID;
			other.Data = Data;
			return (StructuredContentData)base.Clone(other);
		}
	}
}