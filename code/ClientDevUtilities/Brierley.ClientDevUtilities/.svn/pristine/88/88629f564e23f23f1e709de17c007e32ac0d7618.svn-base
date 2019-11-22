//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_Version")]
	public class LWSchemaVersion
	{
		public const string FRAMEWORK_OBJECTS = "FrameworkObjects";
		public const string CAMPAIGN_OBJECTS = "CampaignObjects";
		public const string SURVEY_OBJECTS = "SurveyObjects";

		/// <summary>
		/// Gets or sets the Id for the Version
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the TargetType for the Version
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string TargetType { get; set; }

		/// <summary>
		/// Gets or sets the VersionNumber for the Version
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string VersionNumber { get; set; }

		/// <summary>
		/// Gets or sets the AppliedBy for the Version
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string AppliedBy { get; set; }

		/// <summary>
		/// Gets or sets the DateApplied for the Version
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DateTime DateApplied { get; set; }

		/// <summary>
		/// Initializes a new instance of the Version class
		/// </summary>
		public LWSchemaVersion()
		{
		}
	}
}
