using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Interfaces
{
	public class FulfillmentOrderStatus
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public RewardOrderStatus Status { get; set; }
		public DateTime? ShipDate { get; set; }
		public string TrackingNumber { get; set; }
		public string TrackingUrl { get; set; }
	}

    public class OrderItemInfo
    {
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
    }

	public interface IOrderFulfillmentProvider
	{
		/// <summary>
		/// Initialize the provider with any parameters
		/// </summary>
		/// <param name="parameters"></param>
		void Initialize(string lwOrg, string lwEnvironment, NameValueCollection parameters);

		/// <summary>
		/// This creates a product in the third part fulfillment system if real-time updates to the third party
		/// system are configured.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		long CreateProduct(Product p);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		long CreateProduct(ProductVariant v);

		/// <summary>
		/// This method is invoked to cerate an order.
		/// </summary>
		/// <param name="member">Member for whom the order is being created</param>
		/// <param name="shippingAddress">Shipping address</param>
		/// <param name="partNumbers">An array of part numbers that need to be shipped</param>
		/// <param name="quantities">A corresponding array of quantities for the part numbers</param>
		/// <returns></returns>
        string CreateOrder(Member member, string firstName, string lastName, string emailAddress, Address shippingAddress, IList<OrderItemInfo> orderItems);

		/// <summary>
		/// Returns the order status for the specified order number.
		/// </summary>
		/// <param name="orderNumber"></param>
		/// <returns></returns>
		FulfillmentOrderStatus GetOrderStatus(string orderNumber);

		/// <summary>
		/// Returns the order status items of the specified order number.
		/// </summary>
		/// <param name="orderNumber"></param>
		/// <returns></returns>
		Dictionary<string, FulfillmentOrderStatus> GetOrderItemStatus(string orderNumber);

		/// <summary>
		/// Updates the shipping address of the order
		/// </summary>
		/// <param name="orderNumber"></param>
		/// <param name="shippingAddress"></param>
		void UpdateOrder(Member member, string orderNumber, Address shippingAddress);

		/// <summary>
		/// Cancels an entire order.
		/// </summary>
		/// <param name="orderNumber"></param>
		/// <returns>Order cancellation number</returns>
		string CancelOrder(string orderNumber);

		/// <summary>
		/// Cancels only the specified part number from the order.
		/// </summary>
		/// <param name="orderNumber"></param>
		/// <param name="partNumber"></param>
		/// <returns></returns>
		string CancelOrder(string orderNumber, string partNumber);
	}
}
