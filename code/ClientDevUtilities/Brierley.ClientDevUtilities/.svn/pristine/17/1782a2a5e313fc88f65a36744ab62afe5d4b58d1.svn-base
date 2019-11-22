using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
	public class GetMemberRewards : MemberRewardsBase
	{
		private const string _className = "GetMemberRewards";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetMemberRewards() : base("GetMemberRewards") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to retrieve member rewards.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				long? categoryId = args.ContainsKey("CategoryId") ? (long?)args["CategoryId"] : null;
				DateTime startDate = args.ContainsKey("StartDate") ? (DateTime)args["StartDate"] : DateTimeUtil.MinValue;
				DateTime endDate = args.ContainsKey("EndDate") ? (DateTime)args["EndDate"] : DateTimeUtil.MaxValue;
                bool unredeemedOnly = args.ContainsKey("UnRedeemedOnly") ? (bool)args["UnRedeemedOnly"] : false;
                bool unexpiredOnly = args.ContainsKey("UnexpiredOnly") ? (bool)args["UnexpiredOnly"] : false;
				int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
				int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;

				if (endDate < startDate)
				{
					_logger.Error(_className, methodName, "End date cannot be earlier than the start date");
					throw new LWOperationInvocationException("End date cannot be earlier than the start date") { ErrorCode = 3204 };
				}

				Member member = LoadMember(args);

				Dictionary<long, MemberReward> rewardMap = null;
				Dictionary<string, MemberOrder> orderMap = null;
				IList<MemberOrder> orders = new List<MemberOrder>();

				List<long> rewardIds = LoyaltyDataService.GetMemberRewardIds(
							member, categoryId, startDate, endDate, null, unredeemedOnly, unexpiredOnly);
				if (rewardIds.Count > 0)
				{
					long[] ids = LWQueryBatchInfo.GetIds(rewardIds.ToArray<long>(), startIndex, batchSize, Config.EnforceValidBatch);
					List<MemberReward> rewardsList = LoyaltyDataService.GetMemberRewardByIds(ids);
					/*int nRewards = */
					MemberRewardsUtil.GetMemberRewardsStatus(rewardsList, out rewardMap, out orderMap);
					/*
					 * Since there may be rewards that do not have an order number associated, we need to handle that situation.
					 * */
					foreach (MemberReward reward in rewardMap.Values)
					{
						// the reward does not have an order number
						RewardOrderItem item = null;
						MemberOrder order = null;
						if (string.IsNullOrEmpty(reward.LWOrderNumber))
						{
							order = new MemberOrder();
							orders.Add(order);
							item = new RewardOrderItem();
							item.MemberRewardId = reward.Id;
							order.OrderItems.Add(item);
						}
						else
						{
							// the order has an order number.
							var o1 = from o in orders
									 where o.OrderNumber == reward.LWOrderNumber
									 select o;
							if (o1.Count() == 0)
							{
								order = orderMap[reward.LWOrderNumber];
								orders.Add(order);
							}
						}
					}

					APIStruct[] memberOrders = new APIStruct[orders.Count];
					int orderIdx = 0;
					foreach (MemberOrder order in orders)
					{
						APIArguments orderParams = new APIArguments();
						orderParams.Add("OrderNumber", order.OrderNumber);
						if (!string.IsNullOrEmpty(order.Source))
						{
							orderParams.Add("Source", order.Source);
						}
						if (!string.IsNullOrEmpty(order.Channel))
						{
							orderParams.Add("Channel", order.Channel);
						}
						if (!string.IsNullOrEmpty(order.ChangedBy))
						{
							orderParams.Add("ChangedBy", order.ChangedBy);
						}
						if (!string.IsNullOrEmpty(order.FirstName))
						{
							orderParams.Add("FirstName", order.FirstName);
						}
						if (!string.IsNullOrEmpty(order.LastName))
						{
							orderParams.Add("LastName", order.LastName);
						}
						if (!string.IsNullOrEmpty(order.EmailAddress))
						{
							orderParams.Add("EmailAddress", order.EmailAddress);
						}
						if (order.ShippingAddress != null)
						{
							if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineOne))
							{
								orderParams.Add("AddressLineOne", order.ShippingAddress.AddressLineOne);
							}
							if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineTwo))
							{
								orderParams.Add("AddressLineTwo", order.ShippingAddress.AddressLineTwo);
							}
							if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineThree))
							{
								orderParams.Add("AddressLineThree", order.ShippingAddress.AddressLineThree);
							}
							if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineFour))
							{
								orderParams.Add("AddressLineFour", order.ShippingAddress.AddressLineFour);
							}
							if (!string.IsNullOrEmpty(order.ShippingAddress.City))
							{
								orderParams.Add("City", order.ShippingAddress.City);
							}
							if (!string.IsNullOrEmpty(order.ShippingAddress.StateOrProvince))
							{
								orderParams.Add("StateOrProvince", order.ShippingAddress.StateOrProvince);
							}
							if (!string.IsNullOrEmpty(order.ShippingAddress.ZipOrPostalCode))
							{
								orderParams.Add("ZipOrPostalCode", order.ShippingAddress.ZipOrPostalCode);
							}
							if (!string.IsNullOrEmpty(order.ShippingAddress.County))
							{
								orderParams.Add("County", order.ShippingAddress.County);
							}
							if (!string.IsNullOrEmpty(order.ShippingAddress.Country))
							{
								orderParams.Add("Country", order.ShippingAddress.Country);
							}
						}

						APIStruct[] memberRewards = new APIStruct[order.OrderItems.Count];
						int rewardIdx = 0;
						foreach (RewardOrderItem item in order.OrderItems)
						{
							MemberReward reward = rewardMap[item.MemberRewardId];
							APIArguments rewardParams = new APIArguments();
							rewardParams.Add("MemberRewardID", reward.Id);
							rewardParams.Add("RewardDefID", reward.RewardDefId);
							if (!string.IsNullOrEmpty(reward.FPOrderNumber))
							{
								rewardParams.Add("FPOrderNumber", reward.FPOrderNumber);
							}
							if (!string.IsNullOrEmpty(reward.CertificateNmbr))
							{
								rewardParams.Add("CertificateNumber", reward.CertificateNmbr);
							}
							rewardParams.Add("AvailableBalance", reward.AvailableBalance);
							rewardParams.Add("DateIssued", reward.DateIssued);
							if (reward.Expiration != null)
							{
								rewardParams.Add("ExpirationDate", reward.Expiration);
							}
							if (reward.FulfillmentDate != null)
							{
								rewardParams.Add("FulfillmentDate", reward.FulfillmentDate);
							}
							if (reward.RedemptionDate != null)
							{
								rewardParams.Add("RedemptionDate", reward.RedemptionDate);
							}
							if (reward.OrderStatus != null)
							{
								rewardParams.Add("OrderStatus", Enum.GetName(typeof(RewardOrderStatus), reward.OrderStatus));
							}
							if (!string.IsNullOrEmpty(reward.TrackingUrl))
							{
								rewardParams.Add("TrackingUrl", reward.TrackingUrl);
							}
							if (!string.IsNullOrEmpty(reward.TrackingNumber))
							{
								rewardParams.Add("TrackingNumber", reward.TrackingNumber);
							}
							APIStruct rv = new APIStruct() { Name = "MemberRewardInfo", IsRequired = false, Parms = rewardParams };
							memberRewards[rewardIdx++] = rv;
						}
						orderParams.Add("MemberRewardInfo", memberRewards);
						APIStruct v = new APIStruct() { Name = "MemberRewardOrder", IsRequired = false, Parms = orderParams };
						memberOrders[orderIdx++] = v;
					}
					APIArguments resultParams = new APIArguments();
					resultParams.Add("MemberRewardOrder", memberOrders);
					response = SerializationUtils.SerializeResult(Name, Config, resultParams);
				}
				else
				{
					throw new LWOperationInvocationException("No member rewards found.") { ErrorCode = 3362 };
				}

				return response;
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
	}
}
