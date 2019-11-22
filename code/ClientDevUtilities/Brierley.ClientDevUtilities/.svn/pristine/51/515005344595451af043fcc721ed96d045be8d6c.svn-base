using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// A Skin for a LWPortal.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_Skin")]
	public class Skin : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the ID for the current Skin
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current Skin
		/// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description for the current Skin
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = true)]
		public string Description { get; set; }

		/// <summary>
		/// Initializes a new instance of the Skin class
		/// </summary>
		public Skin()
		{
			ID = -1;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other">other Skin to copy</param>
		public Skin(Skin other)
		{
			ID = -1;
			Name = other.Name;
			Description = other.Description;
		}

		public virtual Skin Clone()
		{
			return Clone(new Skin());
		}

		public virtual Skin Clone(Skin other)
		{
			other.Name = Name;
			other.Description = Description;
			return (Skin)base.Clone(other);
		}
	}
}
