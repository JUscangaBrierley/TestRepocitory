using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;

namespace Brierley.FrameWork.LWIntegration.Util
{
	public class MemberRewardsUtil
	{
		#region Fields
		private const string _className = "MemberRewardsUtil";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
		private static Dictionary<string, IOrderFulfillmentProvider> _providerMap = new Dictionary<string, IOrderFulfillmentProvider>();
		private static object _providerLock = new object();
		#endregion

		#region General Private Helpers
		private static void GetRewardDetails(
			Dictionary<string, FulfillmentOrderStatus> statusMap,
			MemberOrder order,
			MemberReward reward
			)
		{
			using (var service = LWDataServiceUtil.ContentServiceInstance())
			{
				if (statusMap != null)
				{
					if (reward.ProductId > 0)
					{
						Product p = service.GetProduct(reward.ProductId);
						if (statusMap.ContainsKey(p.PartNumber))
						{
							if (string.IsNullOrEmpty(order.FirstName))
							{
								order.FirstName = statusMap[p.PartNumber].FirstName;
							}
							if (string.IsNullOrEmpty(order.LastName))
							{
								order.LastName = statusMap[p.PartNumber].LastName;
							}
							if (string.IsNullOrEmpty(order.EmailAddress))
							{
								order.EmailAddress = statusMap[p.PartNumber].EmailAddress;
							}
							reward.OrderStatus = statusMap[p.PartNumber].Status;
							reward.TrackingNumber = statusMap[p.PartNumber].TrackingNumber;
							reward.TrackingUrl = statusMap[p.PartNumber].TrackingUrl;
							if (reward.FulfillmentDate == null && statusMap[p.PartNumber].ShipDate != null)
							{
								reward.FulfillmentDate = statusMap[p.PartNumber].ShipDate;
							}
						}
					}
					else if (reward.ProductVariantId > 0)
					{
						ProductVariant p = service.GetProductVariant(reward.ProductVariantId);
						if (statusMap.ContainsKey(p.PartNumber))
						{
							if (string.IsNullOrEmpty(order.FirstName))
							{
								order.FirstName = statusMap[p.PartNumber].FirstName;
							}
							if (string.IsNullOrEmpty(order.LastName))
							{
								order.LastName = statusMap[p.PartNumber].LastName;
							}
							if (string.IsNullOrEmpty(order.EmailAddress))
							{
								order.EmailAddress = statusMap[p.PartNumber].EmailAddress;
							}
							reward.OrderStatus = statusMap[p.PartNumber].Status;
							reward.TrackingNumber = statusMap[p.PartNumber].TrackingNumber;
							reward.TrackingUrl = statusMap[p.PartNumber].TrackingUrl;
							if (reward.FulfillmentDate == null && statusMap[p.PartNumber].ShipDate != null)
							{
								reward.FulfillmentDate = statusMap[p.PartNumber].ShipDate;
							}
						}
					}
				}
			}
		}

		public static bool IsCreateProductsFlagOn()
		{
			string flagStr = LWConfigurationUtil.GetConfigurationValue("LW_CreateProductsInFulfillmentProvider");
			bool flag = false;
			if (!string.IsNullOrEmpty(flagStr))
			{
				flag = bool.Parse(flagStr);
			}
			return flag;
		}

		public static RewardFulfillmentOption GetFulfillmentOption(RuleTrigger rt)
		{
			PropertyInfo pInfo = rt.Rule.GetType().GetProperty("FulfillmentOption");
			if (pInfo == null)
			{
				string errMsg = string.Format("Configured rule {0} does not support the required property {1}.", rt.RuleName, "FulfillmentOption");
				throw new LWIntegrationException(errMsg) { ErrorCode = 3351 };
			}
			return (RewardFulfillmentOption)pInfo.GetValue(rt.Rule, null);
		}

		public static PointsConsumptionOnIssueReward GetPointConsumptionPolicy(RuleTrigger rt)
		{
			PropertyInfo pInfo = rt.Rule.GetType().GetProperty("PointsConsumption");
			if (pInfo == null)
			{
				string errMsg = string.Format("Configured rule {0} does not support the required property {1}.", rt.RuleName, "PointsConsumption");
				throw new LWIntegrationException(errMsg) { ErrorCode = 3351 };
			}
			return (PointsConsumptionOnIssueReward)pInfo.GetValue(rt.Rule, null);
		}

		public static RuleTrigger GetRuleForReward(string rewardName, string categoryName, NameValueCollection config)
		{
			// first see if there is a one-to-one mapping for the reward.
			string ruleName = config[rewardName];
			if (string.IsNullOrEmpty(ruleName))
			{
				// now see if there is a rule mapped for the category
				ruleName = config[categoryName];
				if (string.IsNullOrEmpty(ruleName))
				{
					ruleName = config["AllRewards"];
				}
			}
			if (string.IsNullOrEmpty(ruleName))
			{
				throw new LWIntegrationException("No rule mapping found for reward " + rewardName) { ErrorCode = 3350 };
			}
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				RuleTrigger rule = service.GetRuleByName(ruleName);
				if (rule == null)
				{
					string errMsg = string.Format("Configured rule {0} for reward {1} does not exist.", ruleName, rewardName);
					throw new LWIntegrationException(errMsg) { ErrorCode = 3351 };
				}
				return rule;
			}
		}

		/// <summary>
		/// This operation gets a map of all fulfillemnt providers for this order, indexed by the
		/// FP order number
		/// </summary>
		/// <param name="lwOrderNumber"></param>
		/// <returns></returns>
		private static Dictionary<string, IOrderFulfillmentProvider> GetFulfillmentProviderMap(string lwOrderNumber)
		{
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				IList<MemberReward> rewardsList = service.GetMemberRewardsByOrderNumber(lwOrderNumber);

				Dictionary<string, IOrderFulfillmentProvider> fpMap = new Dictionary<string, IOrderFulfillmentProvider>();
				if (rewardsList != null && rewardsList.Count > 0)
				{
					foreach (MemberReward reward in rewardsList)
					{
						if (reward.FulfillmentOption == RewardFulfillmentOption.ThirdParty && reward.FulfillmentProviderId != null)
						{
							FulfillmentProvider fp = GetFulfillmentProviderInfo(reward.FulfillmentProviderId);
							IOrderFulfillmentProvider provider = GetFulfillmentProvider(fp, null);
							if (!string.IsNullOrEmpty(reward.FPOrderNumber) && !fpMap.ContainsKey(reward.FPOrderNumber))
							{
								fpMap.Add(reward.FPOrderNumber, provider);
							}
						}
					}
				}
				return fpMap;
			}
		}

		/// <summary>
		/// This method gets all status maps from all rewards for a given order.
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		private static Dictionary<string, Dictionary<string, FulfillmentOrderStatus>> GetOrderStatusMap(MemberOrder order)
		{
			Dictionary<string, Dictionary<string, FulfillmentOrderStatus>> fpStatusMap = null;
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				IList<MemberReward> rewardsList = service.GetMemberRewardsByOrderNumber(order.OrderNumber);
				if (rewardsList != null && rewardsList.Count > 0)
				{
					fpStatusMap = new Dictionary<string, Dictionary<string, FulfillmentOrderStatus>>();
					foreach (MemberReward reward in rewardsList)
					{
						if (
							reward.FulfillmentProviderId != null &&
							!string.IsNullOrEmpty(reward.FPOrderNumber) &&
							fpStatusMap.ContainsKey(reward.FPOrderNumber))
						{
							IOrderFulfillmentProvider fprovider = GetFulfillmentProvider(reward.FulfillmentProviderId, null);
							Dictionary<string, FulfillmentOrderStatus> statusMap = fprovider.GetOrderItemStatus(reward.FPOrderNumber);
							fpStatusMap.Add(reward.FPOrderNumber, statusMap);
						}
					}
				}
				return fpStatusMap;
			}
		}

		/// <summary>
		/// This method is only usefule when there are multiple rewards in the order.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="rewards"></param>
		/// <param name="totalPointsNeeded"></param>
		private static void EnsureSufficientPoints(Member member, IList<RewardOrderItem> rewards, decimal totalPointsNeeded)
		{
			Dictionary<string, long> ptMap = new Dictionary<string, long>();
			Dictionary<string, long> peMap = new Dictionary<string, long>();

			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var contentService = LWDataServiceUtil.ContentServiceInstance())
			{

				long[] ptIds = null;
				long[] peIds = null;

                bool skipPointTypes = false;
                bool skipPointEvents = false;

				foreach (RewardOrderItem info in rewards)
				{
                    if (skipPointTypes && skipPointEvents) // End early if we've mapped all point types and events
                        break;

					RewardDef rdef = contentService.GetRewardDef(info.RewardName);
                    if (!skipPointTypes)
                    {
                        string[] types = rdef.GetPointTypes();
                        if (types != null)
                        {
                            foreach (string ptName in types)
                            {
                                if (!ptMap.ContainsKey(ptName))
                                {
                                    PointType pt = service.GetPointType(ptName);
                                    ptMap.Add(ptName, pt.ID);
                                }
                            }
                        }
                        else
                        {
                            // all
                            IList<PointType> ptList = service.GetAllPointTypes();
                            foreach (PointType pt in ptList)
                            {
                                if (!ptMap.ContainsKey(pt.Name))
                                {
                                    ptMap.Add(pt.Name, pt.ID);
                                }
                            }
                            skipPointTypes = true;
                        }
                    }

                    if (!skipPointEvents)
                    {
                        string[] events = rdef.GetPointEvents();
                        if (events != null)
                        {
                            foreach (string peName in events)
                            {
                                if (!peMap.ContainsKey(peName))
                                {
                                    PointEvent pe = service.GetPointEvent(peName);
                                    peMap.Add(peName, pe.ID);
                                }
                            }
                        }
                        else
                        {
                            // all
                            IList<PointEvent> peList = service.GetAllPointEvents();
                            foreach (PointEvent pe in peList)
                            {
                                if (!peMap.ContainsKey(pe.Name))
                                {
                                    peMap.Add(pe.Name, pe.ID);
                                }
                            }
                            skipPointEvents = true;
                        }
                    }
				}

                ptIds = ptMap.Values.ToArray();
                peIds = peMap.Values.ToArray();

				decimal pointBalance = member.GetPoints(ptIds, peIds, null, null);
				if (pointBalance < totalPointsNeeded)
				{
					string errMsg = string.Format("Total points needed to fulfill this order are {0}.  The member has only {1} points.",
						totalPointsNeeded, pointBalance);
					throw new LWIntegrationException(errMsg) { ErrorCode = 3354 };
				}
			}
		}

		#endregion

		#region Fulfillment Provider Helpers
		//private static string GetKey(string org, string env)
		//{
		//    return org + "::" + env;
		//}

		private static IOrderFulfillmentProvider CreateFulfillmentProvider(string type, string assembly, NameValueCollection parms)
		{
			IOrderFulfillmentProvider provider = null;
			try
			{
				provider = (IOrderFulfillmentProvider)ClassLoaderUtil.CreateInstance(assembly, type);
				if (provider == null)
				{
					throw new LWIntegrationException("Unable to create instance of fulfillment provider.") { ErrorCode = 3352 };
				}
				LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
				provider.Initialize(ctx.Organization, ctx.Environment, null);
			}
			catch (Exception ex)
			{
				throw new LWIntegrationException("Unable to create instance of fulfillment provider.", ex) { ErrorCode = 3352 };
			}
			return provider;
		}

		private static FulfillmentProvider GetFulfillmentProviderInfo()
		{
			string fproviderName = LWConfigurationUtil.GetConfigurationValue("LW_FulfillmentProvider");
			FulfillmentProvider fp = null;
			if (!string.IsNullOrEmpty(fproviderName))
			{
				using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					fp = loyalty.GetFulfillmentProvider(fproviderName);
				}
			}
			else
			{
				throw new LWIntegrationException("No fulfillment provider specified for third party fulfillment.") { ErrorCode = 3363 };
			}
			return fp;
		}

		// new set of methods
		private static string GetKey(long id, string org, string env)
		{
			return id.ToString() + "::" + org + "::" + env;
		}

		private static FulfillmentProvider GetFulfillmentProviderInfo(long? id)
		{
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				FulfillmentProvider fp = null;
				if (id != null)
				{
					fp = loyalty.GetFulfillmentProvider(id.Value);
				}
				else
				{
					string fproviderName = LWConfigurationUtil.GetConfigurationValue("LW_FulfillmentProvider");
					fp = loyalty.GetFulfillmentProvider(fproviderName);
				}
				if (fp == null)
				{
					throw new LWIntegrationException("No fulfillment provider specified for third party fulfillment.") { ErrorCode = 3363 };
				}
				return fp;
			}
		}

		public static IOrderFulfillmentProvider GetFulfillmentProvider(long? providerId, NameValueCollection parms)
		{
			FulfillmentProvider fp = GetFulfillmentProviderInfo(providerId);
			IOrderFulfillmentProvider provider = null;
			LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
			string key = GetKey(fp.Id, ctx.Organization, ctx.Environment);
			lock (_providerLock)
			{
				if (_providerMap.ContainsKey(key))
				{
					provider = _providerMap[key];
				}
				else
				{
					provider = CreateFulfillmentProvider(fp.ProviderTypeName, fp.ProviderAssemblyName, parms);
					_providerMap.Add(key, provider);
				}
			}
			return provider;
		}

		public static IOrderFulfillmentProvider GetFulfillmentProvider(FulfillmentProvider providerDef, NameValueCollection parms)
		{
			IOrderFulfillmentProvider provider = null;
			LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
			string key = GetKey(providerDef.Id, ctx.Organization, ctx.Environment);
			lock (_providerLock)
			{
				if (_providerMap.ContainsKey(key))
				{
					provider = _providerMap[key];
				}
				else
				{
					provider = CreateFulfillmentProvider(providerDef.ProviderTypeName, providerDef.ProviderAssemblyName, parms);
					_providerMap.Add(key, provider);
				}
			}
			return provider;
		}
		#endregion

		#region Public Methods

		public static MemberOrder CreateRewardOrder
			(
			Member member,
			string firstName, string lastName,
			string cardId,
			Address shippingAddress,
			string channel,
			string changedBy,
			string emailAddress,
			IList<RewardOrderItem> rewards,
			NameValueCollection rewardsConfig,
			IAddMemberRewardInterceptor interceptor = null,
			string language = null
			)
		{
			string methodName = "CreateRewardOrder";

			MemberOrder order = new MemberOrder()
			{
				ShippingAddress = shippingAddress,
				Channel = channel,
				ChangedBy = changedBy
			};

			string errMsg = string.Empty;

			using (var service = LWDataServiceUtil.DataServiceInstance())
			using (var loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var contentService = LWDataServiceUtil.ContentServiceInstance())
			{
				decimal totalPointsNeeded = 0;

				bool thirdParty = false;

				string lworder = order.OrderNumber = "LW-" + service.GetNextID("LWOrderNumber").ToString();

				/*
				 * This loop gets the following information for each reward.
				 * 1. Product related information.
				 * 2. Fulfillment provider if needed.
				 * 3. Points information.
				 * */
				foreach (RewardOrderItem info in rewards)
				{
					RewardDef rdef = null;
					string rid = string.Empty;
					if (!string.IsNullOrEmpty(info.RewardName))
					{
						rid = info.RewardName;
						rdef = contentService.GetRewardDef(info.RewardName);
					}
					if (rdef == null && !string.IsNullOrEmpty(info.TypeCode))
					{
						rid = info.TypeCode;
						IList<RewardDef> rdefs = contentService.GetRewardDefsByCertificateType(info.TypeCode);
						if (rdefs.Count > 0)
						{
							rdef = rdefs[0];
							info.RewardName = rdef.Name;
						}
					}
					if (rdef == null)
					{
						throw new LWIntegrationException(string.Format("Unable to find reward {0}.", rid)) { ErrorCode = 3342 };
					}
					if (!rdef.Active)
					{
						throw new LWIntegrationException(string.Format("Cannot issue reward {0} because it is not active.", rdef.Name)) { ErrorCode = 3232 };
					}
					rdef.Product = contentService.GetProduct(rdef.ProductId);
					rdef.Product.ProductCategory = contentService.GetCategory(rdef.Product.CategoryId);
					info.RewardDefinition = rdef;
					if (string.IsNullOrEmpty(info.PartNumber))
					{
						info.PartNumber = rdef.Product.PartNumber;
					}
					RuleTrigger rule = GetRuleForReward(info.RewardName, rdef.Product.ProductCategory.Name, rewardsConfig);
					info.Rule = rule;
					info.FulfillmentOption = MemberRewardsUtil.GetFulfillmentOption(rule);
					if (info.FulfillmentOption == RewardFulfillmentOption.ThirdParty)
					{
						thirdParty = true;
						FulfillmentProvider fp = GetFulfillmentProviderInfo(rdef.FulfillmentProviderId);
						if (fp != null)
						{
							info.ProviderId = fp.Id;
							info.Provider = GetFulfillmentProvider(fp, null);
						}
					}
					info.PointsConsumptionPolicy = MemberRewardsUtil.GetPointConsumptionPolicy(rule);
					if (info.PointsConsumptionPolicy != PointsConsumptionOnIssueReward.Hold)
					{
						totalPointsNeeded += rdef.HowManyPointsToEarn;
					}
				}


				/*
				 * If name and email is provided then use it, otherwise use from the member.
				 * */
				order.FirstName = !string.IsNullOrEmpty(firstName) ? firstName : member.FirstName;
				order.LastName = !string.IsNullOrEmpty(lastName) ? lastName : member.LastName;
				order.EmailAddress = !string.IsNullOrEmpty(emailAddress) ? emailAddress : member.PrimaryEmailAddress;

				order.OrderItems = rewards;

				// Reward can be issued only to an active member.
				if (member.MemberStatus != MemberStatusEnum.Active && member.MemberStatus != MemberStatusEnum.PreEnrolled)
				{
					throw new LWIntegrationException(string.Format("Member is not active.  No rewards can be added.")) { ErrorCode = 3314 };
				}
				order.MemberId = member.IpCode;
				VirtualCard vc = null;
				if (!string.IsNullOrEmpty(cardId))
				{
					vc = member.GetLoyaltyCard(cardId);
					if (vc == null)
					{
						throw new LWIntegrationException(string.Format("Member has no card with id = {0}.", cardId)) { ErrorCode = 3306 };
					}
				}

				if (vc != null)
				{
					if (vc.Status != VirtualCardStatusType.Active)
					{
						throw new LWIntegrationException(string.Format("VirtualCard is not active.  No rewards can be awarded.")) { ErrorCode = 3307 };
					}
					order.LoyaltyCard = vc;
				}
				else
				{
					bool hasValidCard = false;
					// verufy that there is atleast one valid card
					foreach (VirtualCard v in member.LoyaltyCards)
					{
						if (v.Status == VirtualCardStatusType.Active)
						{
							hasValidCard = true;
							vc = v;
							break;
						}
					}
					if (!hasValidCard)
					{
						throw new LWIntegrationException(string.Format("Member has no valid cards.  No rewards can be awarded.")) { ErrorCode = 3361 };
					}
				}

				//order.LoyaltyCard = vc;

				// check to see if there are enough points
				EnsureSufficientPoints(member, rewards, totalPointsNeeded);

				if (thirdParty)
				{
					Dictionary<long, IList<OrderItemInfo>> orderItemMap = new Dictionary<long, IList<OrderItemInfo>>();
					foreach (RewardOrderItem item in order.OrderItems)
					{
						if (item.FulfillmentOption != RewardFulfillmentOption.ThirdParty)
						{
							continue;
						}
						string description = item.RewardDefinition.GetShortDescription(language, channel);
						if (string.IsNullOrEmpty(description))
						{
							// There is no description on the reward. Try to get it from the product.
							Product p = item.RewardDefinition.Product;
							if (p == null)
							{
								p = contentService.GetProduct(item.RewardDefinition.ProductId);
								description = p.GetShortDescription(language, channel);
							}
							else
							{
								description = p.GetShortDescription(language, channel);
							}
						}
						if (string.IsNullOrEmpty(description))
						{
							description = string.Format("No description available for the reward {0}.", item.RewardName);
						}

						IList<OrderItemInfo> orderItems = null;
						long providerId = item.ProviderId.GetValueOrDefault(0);
						if (orderItemMap.ContainsKey(providerId))
						{
							orderItems = orderItemMap[providerId];
						}
						else
						{
							orderItems = new List<OrderItemInfo>();
							orderItemMap.Add(providerId, orderItems);
						}

						OrderItemInfo oItem = new OrderItemInfo()
						{
							PartName = item.RewardName,
							PartNumber = item.PartNumber,
							Quantity = 1,
							Description = description
						};
						orderItems.Add(oItem);

					}
					Dictionary<string, IOrderFulfillmentProvider> fpMap = new Dictionary<string, IOrderFulfillmentProvider>();
					try
					{
						if (interceptor != null)
						{
							_logger.Debug(_className, methodName, "Invoking interceptor to validate order " + order.OrderNumber);
							interceptor.ValidateOrderInformation(order);
						}
					}
					catch (NotImplementedException)
					{
						// not implemented.
					}
					catch (LWValidationException)
					{
						throw;
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Error validating reward order " + order.OrderNumber + ".", ex);
						throw;
					}
					try
					{
						foreach (KeyValuePair<long, IList<OrderItemInfo>> pair in orderItemMap)
						{
							IOrderFulfillmentProvider fprovider = GetFulfillmentProvider(pair.Key, null);
							string fpOrderNumber = fprovider.CreateOrder(member, order.FirstName, order.LastName, order.EmailAddress, shippingAddress, pair.Value);
							if (!string.IsNullOrEmpty(fpOrderNumber))
							{
								fpMap.Add(fpOrderNumber, fprovider);
								foreach (RewardOrderItem item in order.OrderItems)
								{
									if (item.ProviderId == pair.Key)
									{
										item.FPOrderNumber = fpOrderNumber;
									}
								}
							}
						}
						order.OrderNumber = lworder;
						loyaltyService.CreateMemberOrder(member, order);
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Error creating order.  Cancelling the reward with fulfillment provider.", ex);
						foreach (KeyValuePair<string, IOrderFulfillmentProvider> pair in fpMap)
						{
							try
							{
								pair.Value.CancelOrder(pair.Key);
							}
							catch (Exception ex1)
							{
								_logger.Error(_className, methodName, string.Format("Unable to cancel order {0}.", pair.Key), ex1);
							}
						}
						throw;
					}
				}
				else
				{
					order.OrderNumber = lworder;
					loyaltyService.CreateMemberOrder(member, order);
				}

				return order;
			}
		}

		/// <summary>
		/// This method retrieves the details for the given list of member rewards
		/// </summary>
		/// <param name="rewardsList"></param>
		/// <param name="rewardMap"></param>
		/// <param name="orderMap"></param>
		/// <param name="batchInfo"></param>
		/// <returns></returns>
		public static int GetMemberRewardsStatus(
			IList<MemberReward> rewardsList,
			out Dictionary<long, MemberReward> rewardMap,
			out Dictionary<string, MemberOrder> orderMap)
		{
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{

				orderMap = new Dictionary<string, MemberOrder>();
				rewardMap = new Dictionary<long, MemberReward>();
				IList<MemberOrder> orders = null;

				if (rewardsList != null && rewardsList.Count > 0)
				{
					rewardMap = new Dictionary<long, MemberReward>();
					var orderNumbers = (from reward in rewardsList
										select reward.LWOrderNumber
					).Distinct().OrderBy(o => o).ToArray();

					orders = service.GetMemberOrders(orderNumbers);
					foreach (MemberOrder order in orders)
					{
						orderMap.Add(order.OrderNumber, order);
					}

					Dictionary<string, IOrderFulfillmentProvider> fpMap = new Dictionary<string, IOrderFulfillmentProvider>();
					foreach (MemberReward reward in rewardsList)
					{
						RewardOrderItem item = null;
						MemberOrder order = null;
						if (!string.IsNullOrEmpty(reward.LWOrderNumber))
						{
							order = orderMap[reward.LWOrderNumber];
						}
						else
						{
							order = new MemberOrder();
							orders.Add(order);
						}
						item = new RewardOrderItem();
						item.MemberRewardId = reward.Id;
						if (reward.FulfillmentOption == RewardFulfillmentOption.ThirdParty && reward.FulfillmentProviderId != null)
						{
							FulfillmentProvider fp = GetFulfillmentProviderInfo(reward.FulfillmentProviderId);
							IOrderFulfillmentProvider provider = GetFulfillmentProvider(fp, null);
							item.ProviderId = fp.Id;
							item.Provider = provider;
							if (!string.IsNullOrEmpty(reward.FPOrderNumber) && !fpMap.ContainsKey(reward.FPOrderNumber))
							{
								fpMap.Add(reward.FPOrderNumber, provider);
							}
						}
						order.OrderItems.Add(item);
						rewardMap.Add(reward.Id, reward);
					}

					Dictionary<string, Dictionary<string, FulfillmentOrderStatus>> fpStatusMap = new Dictionary<string, Dictionary<string, FulfillmentOrderStatus>>();
					foreach (KeyValuePair<string, IOrderFulfillmentProvider> pair in fpMap)
					{
						Dictionary<string, FulfillmentOrderStatus> statusMap = pair.Value.GetOrderItemStatus(pair.Key);
						fpStatusMap.Add(pair.Key, statusMap);
					}

					foreach (MemberReward reward in rewardsList)
					{
						MemberOrder order = null;
						if (!string.IsNullOrEmpty(reward.LWOrderNumber) && orderMap.ContainsKey(reward.LWOrderNumber))
						{
							order = orderMap[reward.LWOrderNumber];
						}
						Dictionary<string, FulfillmentOrderStatus> statusMap = null;
						if (reward.OrderStatus == null)
						{
							if (!string.IsNullOrEmpty(reward.FPOrderNumber) && fpStatusMap.ContainsKey(reward.FPOrderNumber))
							{
								statusMap = fpStatusMap[reward.FPOrderNumber];
							}
							GetRewardDetails(statusMap, order, reward);
						}
					}
				}
				return rewardsList != null ? rewardsList.Count : 0;
			}
		}

		/// <summary>
		/// Get the member rewards by order number
		/// </summary>
		/// <param name="orderNumber"></param>
		/// <param name="order"></param>
		/// <param name="rewardMap"></param>
		/// <returns></returns>
		public static bool GetMemberRewards(string orderNumber, out MemberOrder order, out Dictionary<long, MemberReward> rewardMap)
		{
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{

				order = service.GetMemberOrder(orderNumber);
				rewardMap = new Dictionary<long, MemberReward>();
				if (order != null)
				{
					IList<MemberReward> rewardsList = service.GetMemberRewardsByOrderNumber(orderNumber);
					if (rewardsList != null && rewardsList.Count > 0)
					{
						Dictionary<string, Dictionary<string, FulfillmentOrderStatus>> fpStatusMap = new Dictionary<string, Dictionary<string, FulfillmentOrderStatus>>();
						foreach (MemberReward reward in rewardsList)
						{
							if (
								reward.FulfillmentProviderId != null &&
								!string.IsNullOrEmpty(reward.FPOrderNumber) &&
								fpStatusMap.ContainsKey(reward.FPOrderNumber))
							{
								IOrderFulfillmentProvider fprovider = GetFulfillmentProvider(reward.FulfillmentProviderId, null);
								Dictionary<string, FulfillmentOrderStatus> statusMap = fprovider.GetOrderItemStatus(reward.FPOrderNumber);
								fpStatusMap.Add(reward.FPOrderNumber, statusMap);
							}
						}
						foreach (MemberReward reward in rewardsList)
						{
							Dictionary<string, FulfillmentOrderStatus> statusMap = null;
							if (!string.IsNullOrEmpty(reward.FPOrderNumber) && fpStatusMap.ContainsKey(reward.FPOrderNumber))
							{
								statusMap = fpStatusMap[reward.FPOrderNumber];
							}
							GetRewardDetails(statusMap, order, reward);
							RewardOrderItem item = new RewardOrderItem();
							item.MemberRewardId = reward.Id;
							order.OrderItems.Add(item);
							rewardMap.Add(reward.Id, reward);
						}
					}
				}
				return order != null ? true : false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="order"></param>
		/// <param name="cancellation"></param>
		/// <param name="notes"></param>
		public static void CancelOrReturnMemberOrder(MemberOrder order, bool cancellation, string notes)
		{
			string methodName = "CancelOrReturnMemberReward";

			using (var svc = LWDataServiceUtil.DataServiceInstance())
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				string lwCancellationNmbr = string.Empty;
				if (cancellation)
				{
					lwCancellationNmbr = "LW-C-" + svc.GetNextID("LWOrderNumber").ToString();
					Dictionary<string, IOrderFulfillmentProvider> fpMap = GetFulfillmentProviderMap(order.OrderNumber);
					if (fpMap != null && fpMap.Count > 0)
					{
						Dictionary<string, string> fpCancelMap = new Dictionary<string, string>();
						foreach (KeyValuePair<string, IOrderFulfillmentProvider> pair in fpMap)
						{
							if (!fpCancelMap.ContainsKey(pair.Key))
							{
								string fpCancellationNmbr = pair.Value.CancelOrder(pair.Key);
								fpCancelMap.Add(pair.Key, fpCancellationNmbr);
								_logger.Trace(_className, methodName,
								string.Format("Order {0} is being cancelled.  LW Cancellation number is {1}.  FP Cancellation number is {2}.",
								order.OrderNumber, lwCancellationNmbr, fpCancellationNmbr));
							}
						}
					}
				}
				else
				{
					// This is a return.
					Dictionary<string, IOrderFulfillmentProvider> fpMap = GetFulfillmentProviderMap(order.OrderNumber);
					if (fpMap != null && fpMap.Count > 0)
					{
						Dictionary<string, Dictionary<string, FulfillmentOrderStatus>> fpStatusMap = GetOrderStatusMap(order);
						IList<MemberReward> rewardsList = loyalty.GetMemberRewardsByOrderNumber(order.OrderNumber);
						foreach (MemberReward reward in rewardsList)
						{
							Dictionary<string, FulfillmentOrderStatus> statusMap = null;
							if (!string.IsNullOrEmpty(reward.FPOrderNumber))
							{
								statusMap = fpStatusMap.ContainsKey(reward.FPOrderNumber)
									? fpStatusMap[reward.FPOrderNumber] : null;
							}
							if (reward.OrderStatus == null)
							{
								GetRewardDetails(statusMap, order, reward);
							}
							if (reward.OrderStatus != null &&
								(reward.OrderStatus == RewardOrderStatus.Cancelled || reward.OrderStatus == RewardOrderStatus.Returned)
								)
							{
								RewardDef r = reward.RewardDef != null ? reward.RewardDef : content.GetRewardDef(reward.RewardDefId);
								string errMsg = string.Format("Reward {0} of order {1} is already in {2} state.  it cannot be returned.", r.Name, order.OrderNumber, ((RewardOrderStatus)reward.OrderStatus).ToString());
								_logger.Error(_className, methodName, errMsg);
								throw new LWIntegrationException(errMsg) { ErrorCode = 3368 };
							}
						}
					}
				}
				loyalty.CancelOrReturnMemberOrder(order.OrderNumber, lwCancellationNmbr, notes);
			}
		}

		/// <summary>
		/// This method cancels a single reward.
		/// </summary>
		/// <param name="reward"></param>
		/// <param name="cancellation"></param>
		/// <param name="notes"></param>
		public static void CancelOrReturnMemberReward(MemberReward reward, bool cancellation, string notes)
		{
			string methodName = "CancelOrReturnMemberReward";

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			using (var svc = LWDataServiceUtil.DataServiceInstance())
			{
				string lwCancellationNmbr = string.Empty;
				string fpCancellationNmbr = string.Empty;
				if (!string.IsNullOrEmpty(reward.FPOrderNumber))
				{
					if (cancellation)
					{
						lwCancellationNmbr = "LW-C-" + svc.GetNextID("LWOrderNumber").ToString();
						//MemberOrder order = service.GetMemberOrder(reward.OrderNumber);
						if (reward.FulfillmentProviderId != null)
						{
							IOrderFulfillmentProvider fprovider = GetFulfillmentProvider((long)reward.FulfillmentProviderId, null);
							if (reward.ProductId > 0)
							{
								Product p = content.GetProduct(reward.ProductId);
								fpCancellationNmbr = fprovider.CancelOrder(reward.FPOrderNumber, p.PartNumber);
								_logger.Trace(_className, methodName,
									string.Format("Order {0} - {1} is being cancelled.  Cancellation number is {2}",
									reward.FPOrderNumber, p.PartNumber, fpCancellationNmbr));
							}
							else if (reward.ProductVariantId > 0)
							{
								ProductVariant p = content.GetProductVariant(reward.ProductVariantId);
								fpCancellationNmbr = fprovider.CancelOrder(reward.FPOrderNumber, p.PartNumber);
								_logger.Trace(_className, methodName,
									string.Format("Order {0} - {1} is being cancelled.  Cancellation number is {2}",
									reward.FPOrderNumber, p.PartNumber, fpCancellationNmbr));
							}
						}
						//else
						//{
						//    cancellationNmbr = System.Guid.NewGuid().ToString();
						//}
					}
					else
					{
						// This is a return.
						MemberOrder order = loyalty.GetMemberOrder(reward.LWOrderNumber);
						if (reward.FulfillmentProviderId != null)
						{
							IOrderFulfillmentProvider fprovider = GetFulfillmentProvider((long)reward.FulfillmentProviderId, null);
							Dictionary<string, FulfillmentOrderStatus> statusMap = fprovider.GetOrderItemStatus(reward.FPOrderNumber);
							if (reward.OrderStatus == null)
							{
								GetRewardDetails(statusMap, order, reward);
							}
							if (reward.OrderStatus != null &&
								(reward.OrderStatus == RewardOrderStatus.Cancelled || reward.OrderStatus == RewardOrderStatus.Returned)
								)
							{
								RewardDef r = reward.RewardDef != null ? reward.RewardDef : content.GetRewardDef(reward.RewardDefId);
								string errMsg = string.Format("Reward {0} of order {1} is already in {2} state.  it cannot be returned.", r.Name, order.OrderNumber, ((RewardOrderStatus)reward.OrderStatus).ToString());
								_logger.Error(_className, methodName, errMsg);
								throw new LWIntegrationException(errMsg) { ErrorCode = 3368 };
							}
						}
						else
						{
							// this should not have been called.
							string errMsg = string.Format("Order {0} is being returned but there is no fulfillment for this order.", order.OrderNumber);
							_logger.Error(_className, methodName, errMsg);
							throw new LWIntegrationException(errMsg) { ErrorCode = 3367 };
						}
					}
				}

				RewardOrderStatus newStatus = RewardOrderStatus.Cancelled;
				if (cancellation)
				{
					//reward.OrderStatus = RewardOrderStatus.Cancelled;
					reward.LWCancellationNumber = lwCancellationNmbr;
				}
				else
				{
					newStatus = RewardOrderStatus.Returned;
					//reward.OrderStatus = RewardOrderStatus.Returned;
				}

				loyalty.CancelOrReturnMemberReward(reward, newStatus, notes);
			}
		}

		#endregion
	}
}
