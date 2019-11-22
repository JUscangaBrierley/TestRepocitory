using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
	public class GetAllRewardOrderShippingAddresses : MemberRewardsBase
	{
		public GetAllRewardOrderShippingAddresses() : base("GetAllRewardOrderShippingAddresses") { }

		public override string Invoke(string source, string parms)
		{
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to retrieve member rewards.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				Member member = LoadMember(args);

				IList<MemberOrder> orders = LoyaltyDataService.GetMemberOrdersByMember(member.IpCode, null);

				APIStruct[] adresses = new APIStruct[orders.Count];
				int i = 0;
				foreach (MemberOrder order in orders)
				{
					APIArguments rparms = new APIArguments();
					rparms.Add("AddressLineOne", order.ShippingAddress.AddressLineOne);
					if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineTwo))
					{
						rparms.Add("AddressLineTwo", order.ShippingAddress.AddressLineTwo);
					}
					if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineThree))
					{
						rparms.Add("AddressLineThree", order.ShippingAddress.AddressLineThree);
					}
					if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineFour))
					{
						rparms.Add("AddressLineFour", order.ShippingAddress.AddressLineFour);
					}
					if (!string.IsNullOrEmpty(order.ShippingAddress.City))
					{
						rparms.Add("City", order.ShippingAddress.City);
					}
					if (!string.IsNullOrEmpty(order.ShippingAddress.StateOrProvince))
					{
						rparms.Add("StateOrProvince", order.ShippingAddress.StateOrProvince);
					}
					if (!string.IsNullOrEmpty(order.ShippingAddress.ZipOrPostalCode))
					{
						rparms.Add("ZipOrPostalCode", order.ShippingAddress.ZipOrPostalCode);
					}
					if (!string.IsNullOrEmpty(order.ShippingAddress.County))
					{
						rparms.Add("County", order.ShippingAddress.County);
					}
					if (!string.IsNullOrEmpty(order.ShippingAddress.Country))
					{
						rparms.Add("Country", order.ShippingAddress.Country);
					}
					APIStruct addr = new APIStruct() { Name = "ShippingAddress", IsRequired = false, Parms = rparms };
					adresses[i++] = addr;
				}

				APIArguments responseArgs = new APIArguments();
				responseArgs.Add("ShippingAddress", adresses);
				response = SerializationUtils.SerializeResult(Name, Config, responseArgs);

				return response;
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
	}
}
