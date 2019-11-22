using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for ContentAttributeDef.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_ContentAttributeDef")]
	public class ContentAttributeDef : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the ID for the current ContentAttributeDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current ContentAttributeDef
		/// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the ContentTypes for the current ContentAttributeDef
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string ContentTypes { get; set; }

		/// <summary>
		/// Gets or sets the DataType for the current ContentAttributeDef
		/// </summary>
        [PetaPoco.Column(Length = 25, IsNullable = false, PersistEnumAsString = true)]
		public ContentAttributeDataType DataType { get; set; }

		/// <summary>
		/// Gets or sets the DefaultValues for the current ContentAttributeDef
		/// </summary>
        [PetaPoco.Column(Length = 2000)]
		public string DefaultValues { get; set; }

		/// <summary>
		/// Gets or sets the VisibleInGrid for the current ContentAttributeDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool VisibleInGrid { get; set; }


		/// <summary>
		/// Initializes a new instance of the RewardAttribute class
		/// </summary>
		public ContentAttributeDef()
		{
		}

		public ContentAttributeDef Clone()
		{
			return Clone(new ContentAttributeDef());
		}

		public ContentAttributeDef Clone(ContentAttributeDef dest)
		{
			dest.Name = Name;
			dest.ContentTypes = ContentTypes;
			dest.DefaultValues = DefaultValues;
			dest.VisibleInGrid = VisibleInGrid;
            dest.DataType = DataType;
			base.Clone(dest);
			return dest;
		}
	}
}