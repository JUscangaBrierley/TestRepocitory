using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	/// <summary>
	/// POCO for CampaignTable.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("TableId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLTable")]
	public class CampaignTable : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the ID for the current CampaignTable
		/// </summary>
		[PetaPoco.Column("TableId")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current CampaignTable
		/// </summary>
		[PetaPoco.Column("TableName", Length = 150, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = true)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Alias name for the current CampaignTable
		/// </summary>
		[PetaPoco.Column(Length = 150)]
		public string Alias { get; set; }

		/// <summary>
		/// Gets or sets the TableType for the current CampaignTable
		/// </summary>
		[PetaPoco.Column("TableTypeId")]
		public TableType TableType { get; set; }

		/// <summary>
		/// Gets or sets whether the configured table resides in the default schema.
		/// </summary>
		/// <remarks>
		/// This property only applies to clients that are configured to use separate campaign and data schemas (campaign schema = the 
		/// schema/database where data is written, including output tables. data schema = the schema/database where the bulk of the data 
		/// resides - i.e., the data warehouse). If a data schema is used, then tables could be mapped from either schema. This property
		/// helps to identify which schema the mapped table resides in.
		/// </remarks>
		[PetaPoco.Column]
		public bool IsCampaignSchema { get; set; }

		[PetaPoco.Column]
		public bool IsFrameworkSchema { get; set; }

		public bool ResidesInAlternateSchema
		{
			get
			{
				return TableType == TableType.Framework && !IsCampaignSchema;
			}
		}

		/// <summary>
		/// Gets or sets the transient list of fields belonging to this table. Used when mapping joins for execution.
		/// </summary>
		public IEnumerable<TableField> Fields { get; set; }

		/// <summary>
		/// Initializes a new instance of the CampaignTable class
		/// </summary>
		public CampaignTable()
		{
		}

		/// <summary>
		/// Initializes a new instance of the CampaignTable class
		/// </summary>
		/// <param name="name">Initial <see cref="CampaignTable.Name" /> value</param>
		/// <param name="tableType">Initial <see cref="CampaignTable.TableType" /> value</param>
		public CampaignTable(string name, TableType tableType)
		{
			Name = name;
			TableType = tableType;
		}

		public CampaignTable Clone()
		{
			return Clone(new CampaignTable());
		}

		public CampaignTable Clone(CampaignTable dest)
		{
			dest.Alias = Alias;
			dest.CreateDate = DateTime.Now;
			dest.IsCampaignSchema = IsCampaignSchema;
			dest.IsFrameworkSchema = IsFrameworkSchema;
			dest.Name = Name;
			dest.TableType = TableType;
			return dest;
		}
	}
}