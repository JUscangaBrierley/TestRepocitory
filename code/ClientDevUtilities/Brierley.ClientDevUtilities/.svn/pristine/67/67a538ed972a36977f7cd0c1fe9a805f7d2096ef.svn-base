using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for MemberOrder.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberOrder")]
    [AuditLog(false)]
	public class MemberOrder : LWCoreObjectBase
	{
		private Address _shippingAddress;

        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

        [PetaPoco.Column(Length = 100, IsNullable = false)]
		public string OrderNumber { get; set; }

        [PetaPoco.Column(Length = 100)]
		public string OrderCancellationNumber { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        [ColumnIndex]
		public long MemberId { get; set; }

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


		// this is not ideal... the hibernate mapping had a relationship between MemberOrder and Address, but they
		//live flattened in a single row in LW_MemberOrder. The only logical way to do this withPetaPoco is to
		//flatten the POCO. We'll keep the address object, but include all of its properties here:
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
            set { ShippingAddress.AddressLineTwo = value; }
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

		public VirtualCard LoyaltyCard { get; set; }
		public IList<RewardOrderItem> OrderItems { get; set; }


		/// <summary>
		/// Initializes a new instance of the MemberOrder class
		/// </summary>
		public MemberOrder()
		{
			OrderItems = new List<RewardOrderItem>();
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			MemberOrder_AL ar = new MemberOrder_AL()
			{
				ObjectId = this.Id,
				OrderNumber = this.OrderNumber,
				OrderCancellationNumber = this.OrderCancellationNumber,
				MemberId = this.MemberId,
				FirstName = this.FirstName,
				LastName = this.LastName,
				EmailAddress = this.EmailAddress,
				ShippingAddress = this.ShippingAddress,
				Source = this.Source,
				Channel = this.Channel,
				ChangedBy = this.ChangedBy,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}