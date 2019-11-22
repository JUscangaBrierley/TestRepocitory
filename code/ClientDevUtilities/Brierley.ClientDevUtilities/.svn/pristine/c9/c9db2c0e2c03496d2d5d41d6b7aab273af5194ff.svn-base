using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{	
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_MemberOrder")]
    public class MemberOrder_AL : LWObjectAuditLogBase
	{					
		[PetaPoco.Column(IsNullable = false)]
		public Int64 ObjectId { get; set; }

        [PetaPoco.Column(Length = 100, IsNullable = false)]
		public string OrderNumber { get; set; }

        [PetaPoco.Column(Length = 100)]
		public string OrderCancellationNumber { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public Int64 MemberId { get; set; }

		[PetaPoco.Column(Length = 50)]
		public string FirstName { get; set; }

        [PetaPoco.Column(Length = 50)]
		public string LastName { get; set; }

        [PetaPoco.Column(Length = 254)]
		public string EmailAddress { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string Source { get; set; }

        [PetaPoco.Column(Length = 100)]
		public string Channel { get; set; }

        [PetaPoco.Column(Length = 50)]
		public string ChangedBy { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
		

		private Address _shippingAddress;

		public Address ShippingAddress
		{
			get
			{
				if (_shippingAddress == null)
				{
					_shippingAddress = new Address();
				}
				return _shippingAddress;
			}
			set
			{
				_shippingAddress = value;
			}
		}

        [PetaPoco.Column(Length = 100)]
		public string AddressLineOne
		{
			get { return ShippingAddress.AddressLineOne; }
			set { ShippingAddress.AddressLineOne = value; }
		}

        [PetaPoco.Column(Length = 100)]
		public string AddressLineTwo
		{
			get { return ShippingAddress.AddressLineTwo; }
			set { ShippingAddress.AddressLineOne = value; }
		}

        [PetaPoco.Column(Length = 100)]
		public string AddressLineThree
		{
			get { return ShippingAddress.AddressLineThree; }
			set { ShippingAddress.AddressLineThree = value; }
		}

        [PetaPoco.Column(Length = 100)]
		public string AddressLineFour
		{
			get { return ShippingAddress.AddressLineFour; }
			set { ShippingAddress.AddressLineFour = value; }
		}

        [PetaPoco.Column(Length = 50)]
		public string City
		{
			get { return ShippingAddress.City; }
			set { ShippingAddress.City = value; }
		}

        [PetaPoco.Column(Length = 50)]
		public string StateOrProvince
		{
			get { return ShippingAddress.StateOrProvince; }
			set { ShippingAddress.StateOrProvince = value; }
		}

        [PetaPoco.Column(Length = 25)]
		public string ZipOrPostalCode
		{
			get { return ShippingAddress.ZipOrPostalCode; }
			set { ShippingAddress.ZipOrPostalCode = value; }
		}

        [PetaPoco.Column(Length = 50)]
		public string County
		{
			get { return ShippingAddress.County; }
			set { ShippingAddress.County = value; }
		}

        [PetaPoco.Column(Length = 50)]
		public string Country
		{
			get { return ShippingAddress.Country; }
			set { ShippingAddress.Country = value; }
		}

		public double Longitude
		{
			get { return ShippingAddress.Longitude; }
			set { ShippingAddress.Longitude = value; }
		}

		public double Latitude
		{
			get { return ShippingAddress.Latitude; }
			set { ShippingAddress.Latitude = value; }
		}
	}
}