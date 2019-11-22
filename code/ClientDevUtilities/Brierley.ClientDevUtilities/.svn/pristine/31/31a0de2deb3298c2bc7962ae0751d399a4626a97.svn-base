using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Class used to describe links in emails.
	/// </summary>
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_EmailLink")]
	public class EmailLink : LWCoreObjectBase
	{
		/// <summary>
		/// The link's unique Id in the framework database.
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		/// <summary>
		/// The Id of the email which this link belongs to.
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public long EmailId { get; set; }

		/// <summary>
		/// Description of the link, used for email reporting.
		/// </summary>
		[PetaPoco.Column(Length = 500)]
		public string Descriptor { get; set; }

		/// <summary>
		/// The URL that the link points to.
		/// </summary>
		[PetaPoco.Column]
		public string Url { get; set; }

		/// <summary>
		/// The text of the link.
		/// </summary>
		[PetaPoco.Column]
		public string Text { get; set; }

		/// <summary>
		/// Is click tracking enabled for this link?
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public bool IsTrackingEnabled { get; set; }

		/// <summary>
		/// The order the link appears in the email.
		/// </summary>
		[PetaPoco.Column]
		public int? LinkOrder { get; set; }

		/// <summary>
		/// Is the link active, i.e., does it currently exist somewhere in the body of the email?
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public bool IsActive { get; set; }

		/// <summary>
		/// Does the link need validation?
		/// </summary>
		/// <remarks>
		/// A link may need validation if a perfect match is not found between 
		/// the XSLT body of the email and the list of known links in the database.
		/// </remarks>
		[PetaPoco.Column(IsNullable = false)]
		public bool NeedsValidation { get; set; }

		/// <summary>
		/// Flag to allow a consumer of the class determine if the link has 
		/// been inserted into the email body yet.
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public bool HasBeenInserted { get; set; }

		/// <summary>
		/// The link's position in the email XSLT body.
		/// </summary>
		/// <remarks>
		/// This property can be used along with <see cref="Length"/> to pinpoint the exact 
		/// location of the link, possibly to display as a reference to the user. 
		/// </remarks>
		[PetaPoco.Column("LinkIndex", IsNullable = false)]
		public int Index { get; set; }

		/// <summary>
		/// The length of the link in the email XSLT body.
		/// </summary>
		/// <remarks>
		/// This property can be used along with <see cref="Index"/> to pinpoint the exact 
		/// location of the link, possibly to display as a reference to the user. 
		/// </remarks>
		[PetaPoco.Column(IsNullable = false)]
		public int Length { get; set; }

		public EmailLink()
		{
			Id = -1;
			EmailId = -1;
			IsActive = true;
			Index = -1;
			Length = -1;
		}
	}
}

