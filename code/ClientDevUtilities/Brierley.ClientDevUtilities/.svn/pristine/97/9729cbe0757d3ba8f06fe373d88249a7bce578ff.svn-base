using System;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("AudienceId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLAudience")]
	public class Audience : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the ID for the current Audience
		/// </summary>
		[PetaPoco.Column("AudienceId")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current Audience
		/// </summary>
		[PetaPoco.Column("AudienceName", Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = true)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description for the current Audience
		/// </summary>
		[PetaPoco.Column("AudienceDescription", Length = 500)]
		public string Description { get; set; }

		/// <summary>
		/// Initializes a new instance of the Audience class
		/// </summary>
		public Audience()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Audience class
		/// </summary>
		/// <param name="name">Initial <see cref="Audience.Name" /> value</param>
		/// <param name="description">Initial <see cref="Audience.Description" /> value</param>
		public Audience(string name, string description)
		{
			Name = name;
			Description = description;
		}

		public Audience Clone()
		{
			Audience cloned = new Audience();
			return Clone(cloned);
		}

		public Audience Clone(Audience cloned)
		{			
			cloned.Name = Name;
			cloned.Description = Description;
			return cloned;
		}
	}
}