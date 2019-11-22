//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_StoreDef")]
	public class StoreDef_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string StoreNumber { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string StoreName { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string BrandName { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string BrandStoreNumber { get; set; }
		[PetaPoco.Column(Length = 25)]
		public string PhoneNumber { get; set; }
        [PetaPoco.Column(Length = 100, IsNullable = false)]
		public string AddressLineOne { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string AddressLineTwo { get; set; }
        [PetaPoco.Column(Length = 75, IsNullable = false)]
		public string City { get; set; }
        [PetaPoco.Column(Length = 25, IsNullable = false)]
		public string StateOrProvince { get; set; }
        [PetaPoco.Column(Length = 15)]
		public string ZipOrPostalCode { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string County { get; set; }
        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public string Country { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public StoreStatus Status { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public StoreType StoreType { get; set; }
		[PetaPoco.Column]
		public DateTime? OpenDate { get; set; }
		[PetaPoco.Column]
		public DateTime? CloseDate { get; set; }
		[PetaPoco.Column]
		public double? Longitude { get; set; }
		[PetaPoco.Column]
		public double? Latitude { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string Region { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string District { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string Zone { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string StrUserField { get; set; }
		[PetaPoco.Column]
		public long? LongUserField { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
	}
}
