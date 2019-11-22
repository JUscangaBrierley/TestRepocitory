using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// An attribute definition for structured content.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_StructuredContentAttribute")]
	public class StructuredContentAttribute : LWCoreObjectBase
	{
		public const string BATCH_NAME = "BatchName";
		public const string START_DATE = "Start";
		public const string END_DATE = "End";

		/// <summary>
		/// Gets or sets the ID for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(Length = 30, IsNullable = false)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the DataType for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(Length = 30, IsNullable = false)]
		public string DataType { get; set; }

		/// <summary>
		/// Gets or sets the DefaultValue for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(Length = 1024)]
		public string DefaultValue { get; set; }

		/// <summary>
		/// Gets or sets the IsMandatory for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool IsMandatory { get; set; }

		/// <summary>
		/// Gets or sets the IsGlobal for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool IsGlobal { get; set; }

		/// <summary>
		/// Gets or sets the ElementID for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long ElementID { get; set; }

		/// <summary>
		/// Gets or sets the IsFilter for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public bool IsFilter { get; set; }

		/// <summary>
		/// Gets or sets the FilterOrder for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long FilterOrder { get; set; }

		/// <summary>
		/// Gets or sets the ListField for the current StructuredContentAttribute
		/// </summary>
        [PetaPoco.Column(Length = 1024)]
		public string ListField { get; set; }

		/// <summary>
		/// Initializes a new instance of the StructuredContentAttribute class
		/// </summary>
		public StructuredContentAttribute()
		{
			ID = -1;
			Name = string.Empty;
			DataType = string.Empty;
			DefaultValue = string.Empty;
			ElementID = -1;
			FilterOrder = -1;
			ListField = string.Empty;
		}

		public StructuredContentAttribute Clone()
		{
			return Clone(new StructuredContentAttribute());
		}

		public StructuredContentAttribute Clone(StructuredContentAttribute other)
		{
            other.Name = Name;
			other.DataType = DataType;
			other.DefaultValue = DefaultValue;
			other.IsMandatory = IsMandatory;
			other.IsGlobal = IsGlobal;
			other.ElementID = ElementID;
			other.IsFilter = IsFilter;
			other.FilterOrder = FilterOrder;
			other.ListField = ListField;
			return (StructuredContentAttribute)base.Clone(other);
		}
	}
}