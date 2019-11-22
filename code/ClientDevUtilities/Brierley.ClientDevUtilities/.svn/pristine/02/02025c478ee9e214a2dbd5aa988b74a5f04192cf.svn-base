//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	public enum StoreStatus { Closed = 0, Open = 1 }
    public enum StoreType { Physical = 0, ECommerce = 1, Catalog = 2 }

	/// <summary>
	/// This class defines a Store.
	/// </summary>
	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("StoreId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_StoreDef")]
    [AuditLog(true)]
	public class StoreDef : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the StoreId for the current StoreDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long StoreId { get; set; }

		/// <summary>
		/// Gets or sets the StoreNumber for the current StoreDef
		/// </summary>
        [PetaPoco.Column(Length = 50)]
        [UniqueIndex(RequiresLowerFunction = false)]
		public string StoreNumber { get; set; }

		/// <summary>
		/// Gets or sets the StoreName for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column(Length = 100)]
		public string StoreName { get; set; }

		/// <summary>
		/// Gets or sets the BrandName for the current StoreDef
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string BrandName { get; set; }

		/// <summary>
		/// Gets or sets the BrandStoreNumber for the current StoreDef
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string BrandStoreNumber { get; set; }

		/// <summary>
		/// Gets or sets the PhoneNumber for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column(Length = 25)]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// Gets or sets the AddressLineOne for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column(Length = 100, IsNullable = false)]
		public string AddressLineOne { get; set; }

		/// <summary>
		/// Gets or sets the AddressLineTwo for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column(Length = 100)]
		public string AddressLineTwo { get; set; }

		/// <summary>
		/// Gets or sets the City for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column(Length = 75, IsNullable = false)]
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the StateOrProvince for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column(Length = 25, IsNullable = false)]
		public string StateOrProvince { get; set; }

		/// <summary>
		/// Gets or sets the ZipOrPostalCode for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column(Length = 15)]
		public string ZipOrPostalCode { get; set; }

		/// <summary>
		/// Gets or sets the County for the current StoreDef
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string County { get; set; }

		/// <summary>
		/// Gets or sets the Country for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public string Country { get; set; }

		/// <summary>
		/// Gets or sets the Status for the current StoreDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public StoreStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the Type for the current StoreDef
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public StoreType StoreType { get; set; }

		/// <summary>
		/// Gets or sets the OpenDate for the current StoreDef
		/// </summary>
        [PetaPoco.Column]
		public DateTime? OpenDate { get; set; }

		/// <summary>
		/// Gets or sets the CloseDate for the current StoreDef
		/// </summary>
        [PetaPoco.Column]
		public DateTime? CloseDate { get; set; }

		/// <summary>
		/// Gets or sets the CloseDate for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column]
		public double? Longitude { get; set; }

		/// <summary>
		/// Gets or sets the Latitude for the current StoreDef
		/// </summary>
		[JsonProperty]
        [PetaPoco.Column]
		public double? Latitude { get; set; }

		/// <summary>
		/// Gets or sets the Region for the current StoreDef
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string Region { get; set; }

		/// <summary>
		/// Gets or sets the District for the current StoreDef
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string District { get; set; }

		/// <summary>
		/// Gets or sets the Zone for the current StoreDef
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string Zone { get; set; }

        [PetaPoco.Column(Length = 100)]
		public string StrUserField { get; set; }

		[PetaPoco.Column]
		public long? LongUserField { get; set; }

		/// <summary>
		/// Initializes a new instance of the StoreDef class
		/// </summary>
		public StoreDef()
		{
		}
		
		public StoreDef Clone()
        {
            return Clone(new StoreDef());
        }

        public StoreDef Clone(StoreDef dest)
        {
            dest.StoreId = StoreId;
            dest.StoreNumber = StoreNumber;
            dest.StoreName = StoreName;
            dest.BrandName = BrandName;
            dest.BrandStoreNumber = BrandStoreNumber;
            dest.PhoneNumber = PhoneNumber;
            dest.AddressLineOne = AddressLineOne;
            dest.AddressLineTwo = AddressLineTwo;
            dest.City = City;
            dest.StateOrProvince = StateOrProvince;
            dest.ZipOrPostalCode = ZipOrPostalCode;
            dest.County = County;
            dest.Country = Country;
            dest.Status = Status;
            dest.StoreType = StoreType;
            dest.OpenDate = OpenDate;
            dest.CloseDate = CloseDate;
            dest.Longitude = Longitude;
            dest.Latitude = Latitude;
            dest.Region = Region;
            dest.District = District;
            dest.Zone = Zone;
            dest.StrUserField = StrUserField;
            dest.LongUserField = LongUserField;
            return (StoreDef)base.Clone(dest);
        }

		public override LWObjectAuditLogBase GetArchiveObject()
        {
            StoreDef_AL ar = new StoreDef_AL()
            {
                ObjectId = this.StoreId,
                StoreNumber = this.StoreNumber,
                StoreName = this.StoreName,
                BrandName = this.BrandName,
                BrandStoreNumber = this.BrandStoreNumber,
                PhoneNumber = this.PhoneNumber,
                AddressLineOne = this.AddressLineOne,
                AddressLineTwo = this.AddressLineTwo,
                City = this.City,
                StateOrProvince = this.StateOrProvince,
                ZipOrPostalCode = this.ZipOrPostalCode,
                County = this.County,
                Country = this.Country,
                Status = this.Status,
                StoreType = this.StoreType,
                OpenDate = this.OpenDate,
                CloseDate = this.CloseDate,
                Longitude = this.Longitude,
                Latitude = this.Latitude,
                Region = this.Region,
                District = this.District,
                Zone = this.Zone,
                StrUserField = this.StrUserField,
                LongUserField = this.LongUserField,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate
            };
            return ar;
        }
	}
}
