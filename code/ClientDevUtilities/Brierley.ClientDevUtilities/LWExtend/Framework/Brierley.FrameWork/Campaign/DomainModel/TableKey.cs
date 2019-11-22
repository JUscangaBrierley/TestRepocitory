using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	/// <summary>
	/// POCO for TableKey.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("KeyId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLTableKey")]
	public class TableKey
	{
		/// <summary>
		/// Gets or sets the ID for the current TableKey
		/// </summary>
		[PetaPoco.Column("KeyId")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the TableID for the current TableKey
		/// </summary>
		[PetaPoco.Column]
        [ForeignKey(typeof(CampaignTable), "Id")]
		public long TableId { get; set; }

		/// <summary>
		/// Gets or sets the AudienceID for the current TableKey
		/// </summary>
		[PetaPoco.Column]
        [ForeignKey(typeof(Audience), "Id")]
		public long AudienceId { get; set; }

		/// <summary>
		/// Gets or sets the FieldName for the current TableKey
		/// </summary>
		[PetaPoco.Column(Length = 150, IsNullable = false)]
		public string FieldName { get; set; }

		/// <summary>
		/// Gets or sets whether the field is advertised as a selectable audience level
		/// </summary>
		[PetaPoco.Column]
		public bool IsAudienceLevel { get; set; }

		/// <summary>
		/// Gets or sets the FieldType for the current TableKey
		/// </summary>
		[PetaPoco.Column(Length = 15, IsNullable = false)]
		public string FieldType { get; set; }

		/// <summary>
		/// Initializes a new instance of the TableKey class
		/// </summary>
		public TableKey()
		{
		}

		/// <summary>
		/// Initializes a new instance of the TableKey class
		/// </summary>
		/// <param name="tableID">Initial <see cref="TableKey.TableId" /> value</param>
		/// <param name="audienceID">Initial <see cref="TableKey.AudienceId" /> value</param>
		/// <param name="fieldName">Initial <see cref="TableKey.FieldName" /> value</param>
		/// <param name="fieldType">Initial <see cref="TableKey.FieldType" /> value</param>
		public TableKey(long tableID, long audienceID, string fieldName, string fieldType)
		{
			TableId = tableID;
			AudienceId = audienceID;
			FieldName = fieldName;
			FieldType = fieldType;
		}
	}
}