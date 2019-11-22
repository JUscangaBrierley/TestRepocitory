using Brierley.FrameWork.Data.ModelAttributes;
using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
    /// POCO for FulfillmentProvider.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_FulfillmentProvider")]
    public class FulfillmentProvider : LWCoreObjectBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

        [PetaPoco.Column(Length = 255, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string ProviderTypeName { get; set; }

        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string ProviderAssemblyName { get; set; }

		/// <summary>
		/// Initializes a new instance of the FulfillmentProvider class
		/// </summary>
		public FulfillmentProvider()
		{
		}
	}
}