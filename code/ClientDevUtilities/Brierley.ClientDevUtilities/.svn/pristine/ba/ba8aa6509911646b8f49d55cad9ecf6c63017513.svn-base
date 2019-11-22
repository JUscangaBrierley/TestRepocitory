using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLAttribute")]
	public class Attribute : LWCoreObjectBase
	{
		[PetaPoco.Column]
		public long Id { get; set; }

		[PetaPoco.Column]
		public AttributeTypes AttributeType { get; set; }

		[PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = true)]
		public string Name { get; set; }

		[PetaPoco.Column]
		public AttributeDataType DataType { get; set; }

		[PetaPoco.Column]
		public bool IsRequired { get; set; }

		[PetaPoco.Column]
		public bool VisibleInCampaign { get; set; }
		
		/// <summary>
		/// Gets or sets grid visibility for the attribute.
		/// </summary>
		/// <remarks>
		/// This currently only applies to Offer and Segment attributes shown in the offer and segment grid, not the campaign grid.
		/// </remarks>
		[PetaPoco.Column]
		public bool VisibleInGrid { get; set; }

		[PetaPoco.Column]
		public string AllowedValues { get; set; }

		public Attribute()
		{
			AttributeType = AttributeTypes.Campaign;
		}

		public string ToDatabaseName()
		{
			if (string.IsNullOrEmpty(Name))
			{
				throw new InvalidOperationException("Cannot convert null or empty Name to a database parameter name.");
			}
			string ret = Regex.Replace(Name, "[^0-9a-zA-Z]", "_");
			if (string.IsNullOrEmpty(ret) || ret.Replace("_", string.Empty) == string.Empty)
			{
				throw new InvalidOperationException(string.Format("Cannot convert Campaign Attribute from \"{0}\" to a database parameter name. Conversion results in an invalid name.", Name));
			}
			return ret;
		}
		
		public Attribute Clone()
		{
			Attribute cloned = new Attribute();
			cloned.Name = Name;
			cloned.DataType = DataType;
			cloned.IsRequired = IsRequired;
			return cloned;
		}
	}
}
