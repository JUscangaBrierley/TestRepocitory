using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[Serializable]
	[DataContract]
	public class ColumnCondition
	{
		private string _nonAliasedConditionExpression;

		/// <summary>
		/// Gets or sets the RowOrder for the current ColumnCondition
		/// </summary>
		[DataMember]
		public int RowOrder { get; set; }

		/// <summary>
		/// Gets or sets the operator for the current ColumnCondition
		/// </summary>
		[DataMember]
		public string Operator { get; set; }

		/// <summary>
		/// Gets or sets the field function for the current ColumnCondition
		/// </summary>
		[DataMember]
		public StringFunctions FieldFunction { get; set; }

		/// <summary>
		/// Gets or sets the date part for the current ColumnCondition
		/// </summary>
		[DataMember]
		public DateParts? DatePart { get; set; }

		/// <summary>
		/// Gets or sets the assignment table id for the current ColumnCondition
		/// <remarks>
		/// Used by assignment steps to assign a field values to output rows 
		/// e.g., assign each member's tier name to a column that will be carried
		/// throughout the rest of the campaign.
		/// This property is deprecated in 4.4, held in place for backward compatability 
		/// with older campaigns, and will be removed in 4.5. LoyaltyWare 4.4 adds the 
		/// formula builder control, which will be used in place of the assignment box, 
		/// so it will now be possible to assign using a formula potentially consisting of
		/// multiple fields from multiple tables. There's no way to track it all with a 
		/// single table id.
		/// </remarks>
		/// </summary>
		[DataMember]
		public long? AssignmentTableId { get; set; }

		/// <summary>
		/// Gets or sets the assignment field name for the current ColumnCondition
		/// <remarks>
		/// Used by assignment steps to assign a field values to output rows 
		/// e.g., assign each member's tier name to a column that will be carried
		/// throughout the rest of the campaign.
		/// This property is also deprecated in 4.4 (see AssignmentTableId)
		/// </remarks>
		/// </summary>
		[DataMember]
		public string AssignmentFieldName { get; set; }

		/// <summary>
		/// Gets or sets the ConditionExpression for the current ColumnCondition
		/// </summary>
		[DataMember]
		public string ConditionExpression { get; set; }


		/// <summary>
		/// Gets or sets the transient non-aliased column expression
		/// </summary>
		[XmlIgnore]
		public string NonAliasedConditionExpression
		{
			get { return _nonAliasedConditionExpression ?? ConditionExpression; }
			set { _nonAliasedConditionExpression = value; }
		}


		/// <summary>
		/// sets or sets the name of the table the for the current ColumnCondition
		/// </summary>
		[DataMember]
		public string AssignmentTableName { get; set; }

		/// <summary>
		/// Initializes a new instance of the ColumnCondition class
		/// </summary>
		public ColumnCondition()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ColumnCondition class
		/// </summary>
		/// <param name="rowOrder">Initial <see cref="ColumnCondition.RowOrder" /> value</param>
		/// <param name="conditionExpression">Initial <see cref="ColumnCondition.ConditionExpression" /> value</param>
		public ColumnCondition(int rowOrder, string conditionExpression)
		{
			RowOrder = rowOrder;
			ConditionExpression = conditionExpression;
		}

	}


}