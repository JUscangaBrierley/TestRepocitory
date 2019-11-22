using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for AttributeSet.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_ArchiveObject")]
    public class ArchiveObject : LWCoreObjectBase
	{        	
		/// <summary>
        /// Gets or sets the ID for the current LWArchiveObject
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

        /// <summary>
        /// Gets or sets the ItemId for the current LWArchiveObject
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ItemId { get; set; }

        /// <summary>
        /// Gets or sets the GroupId for the current LWArchiveObject
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long GroupId { get; set; }

		/// <summary>
        /// Gets or sets the SetId for the current LWArchiveObject
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long SetId { get; set; }

        /// <summary>
        /// Gets or sets the RunNumber for the current LWArchiveObject
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long RunNumber { get; set; }

		/// <summary>
        /// Gets or sets the ObjectType for the current LWArchiveObject
		/// </summary>
        [PetaPoco.Column(Length = 25, PersistEnumAsString = true, IsNullable = false)]
		public SupportedObjectType ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the Classtype for the current LWArchiveObject
        /// </summary>
        [PetaPoco.Column(Length = 255)]
		public string Classtype { get; set; }

        /// <summary>
        /// Gets or sets the ObjectId for the current LWArchiveObject
        /// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the ObjectState for the current LWArchiveObject
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public string ObjectState { get; set; }        
	}
}