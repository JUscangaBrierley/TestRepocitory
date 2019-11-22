using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
    /// POCO for FulfillmentProviderProductMap. 
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_FulfillmentProductMap")]
	public class FulfillmentProviderProductMap
	{
		/// <summary>
        /// Initializes a new instance of the FulfillmentProviderProductMap class
		/// </summary>
        public FulfillmentProviderProductMap()
		{
		}

        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public long ProviderId { get; set; }

        [PetaPoco.Column(IsNullable = true)]
        public long? ProductId { get; set; }

        [PetaPoco.Column(IsNullable = true)]
        public long? ProductVariantId { get; set; }

        [PetaPoco.Column("FProviderPartNumber", IsNullable = false, Length = 100)]
        public string FulfillmentProviderPartNumber { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public DateTime CreateDate { get; set; }

        [PetaPoco.Column(IsNullable = true)]
        public DateTime? UpdateDate { get; set; }
	}
}