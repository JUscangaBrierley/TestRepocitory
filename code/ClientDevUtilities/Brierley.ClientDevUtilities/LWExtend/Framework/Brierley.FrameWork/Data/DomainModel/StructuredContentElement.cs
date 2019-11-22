using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Structured Content Element
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_StructuredContentElement")]
	public class StructuredContentElement : LWCoreObjectBase
	{
		private string _name = "-1";

		/// <summary>
		/// Gets or sets the ID for the current StructuredContentElement
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current StructuredContentElement
		/// </summary>
        [PetaPoco.Column(Length = 30, IsNullable = false)]
		public string Name
		{
			get { return _name; }
			set { _name = StringUtils.FriendlyString(value, ID.ToString()); }
		}

		/// <summary>
		/// Initializes a new instance of the StructuredContentElement class
		/// </summary>
		public StructuredContentElement()
		{
			ID = -1;
		}

		public StructuredContentElement Clone()
		{
			return Clone(new StructuredContentElement());
		}

		public StructuredContentElement Clone(StructuredContentElement other)
		{
			other.Name = Name;
			return (StructuredContentElement)base.Clone(other);
		}

		public List<StructuredContentAttribute> Attributes { get; set; }
	}
}