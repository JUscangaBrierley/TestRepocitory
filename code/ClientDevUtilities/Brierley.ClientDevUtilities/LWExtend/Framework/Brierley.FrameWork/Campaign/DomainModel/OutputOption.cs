using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[Serializable]
	public class OutputOption
	{
		public OutputType OutputType { get; set; }

		public string TableName { get; set; }

		/// <summary>
		/// Gets or sets the EmailName for the current OutputOption
		/// </summary>
		/// <remarks>
		/// This is used in two ways:
		/// 1. To assist the user in the mail file creation by showing the template field names. 
		///    User selects an email and a server request gathers the email's fields and populates the grid with the names of the fields.
		/// 2. A process may expect a campaign to create a mail file before a step sends an email using the file. In this case, we want
		///    to verify that the email file was created for the same email.
		/// </remarks>
		public string EmailName { get; set; }

		/// <summary>
		/// Gets or sets whether the email should be sent on commit/verify. This only applies to mail file output
		/// where the user as selected an email to tie the generated file to.
		/// </summary>
		public bool SendEmail { get; set; }

		public bool AppendData { get; set; }

		public string FileLocation { get; set; }

		public bool IncludeColumnNames { get; set; }

		public string TextQualifier { get; set; }

		public string ColumnDelimiter { get; set; }

		public string RowDelimiter { get; set; }

		public List<long> RefId { get; set; }

		public string StartDate { get; set; }

		public string ExpirationDate { get; set; }

		public bool CreateTable { get; set; }

		public bool CreateMTouch { get; set; }

		public MTouchType MTouchType { get; set; }

		public string MTouchFieldName { get; set; }

		public string DisplayOrder { get; set; }

		public bool UseCertificates { get; set; }
		
		public string SftpName { get; set; }
			
		public string SftpTriggerFile { get; set; }

		public string PgpName { get; set; }

		public string SocialMediaAppName { get; set; }

		public string Text { get; set; }

		public long TextBlockId { get; set; }

        public bool Transient { get; set; }

		public List<NextBestActionType> IncludedActionTypes { get; set; }

		public OutputOption()
		{
			CreateTable = true;
			OutputType = OutputType.FlatFile;
			TableName = string.Empty;
			FileLocation = string.Empty;
			TextQualifier = string.Empty;
			ColumnDelimiter = string.Empty;
			RowDelimiter = string.Empty;
			MTouchType = Common.MTouchType.Bonus;
		}
	}
}
