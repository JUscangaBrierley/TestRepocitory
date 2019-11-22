using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	public enum SkinItemTypeEnum { html, css, js, font, image };

	/// <summary>
	/// An item associated with a Skin.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_SkinItem")]
	public class SkinItem : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the ID for the current SkinItem
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the ID of the Skin associated with the current SkinItem
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long SkinID { get; set; }

		/// <summary>
		/// Gets or sets the SkinItemType for the current SkinItem
		/// </summary>
        [PetaPoco.Column(Length = 10, IsNullable = false, PersistEnumAsString = true)]
		public SkinItemTypeEnum SkinItemType { get; set; }

		/// <summary>
		/// Gets or sets the FileName for the current SkinItem
		/// </summary>
        [PetaPoco.Column(Length = 1024, IsNullable = false)]
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets the Content for the current SkinItem.  If the SkinItemType is a 
		/// binary type (e.g., image or font), then it will be Base64 encoded.
		/// </summary>
        [PetaPoco.Column(IsNullable = true)]
		public string Content { get; set; }

		/// <summary>
		/// Initializes a new instance of the SkinItem class
		/// </summary>
		public SkinItem()
		{
			ID = -1;
			SkinID = -1;
			SkinItemType = SkinItemTypeEnum.html;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other">other Skin to copy</param>
		public SkinItem(SkinItem other)
		{
			ID = -1;
			SkinID = other.SkinID;
			SkinItemType = other.SkinItemType;
			FileName = other.FileName;
			Content = other.Content;
		}

		public virtual SkinItem Clone()
		{
			return Clone(new SkinItem());
		}

		public virtual SkinItem Clone(SkinItem other)
		{
			other.SkinID = SkinID;
			other.SkinItemType = SkinItemType;
			other.FileName = FileName;
			other.Content = Content;
			return (SkinItem)base.Clone(other);
		}
	}
}
