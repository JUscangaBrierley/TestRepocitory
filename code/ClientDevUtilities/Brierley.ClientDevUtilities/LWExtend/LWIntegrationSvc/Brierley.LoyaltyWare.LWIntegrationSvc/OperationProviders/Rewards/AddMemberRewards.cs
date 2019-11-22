using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
	public class AddMemberRewards : MemberRewardsBase
	{
		public AddMemberRewards() : base("AddMemberRewards") { }

		public override void Initialize(string opName, LWIntegrationDirectives config, NameValueCollection functionProviderParms)
		{
			base.Initialize(opName, config, functionProviderParms);
		}

		public override string Invoke(string source, string parms)
		{
			/*
			 * A lot of this logic needs to be synchronized with IssueReward rule.
			 * */
			try
			{
				//string errMsg = string.Empty;
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No card id provided for add loyalty event.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string cardId = args.ContainsKey("CardID") ? (string)args["CardID"] : string.Empty;
				string firstName = args.ContainsKey("FirstName") ? (string)args["FirstName"] : string.Empty;
				string lastName = args.ContainsKey("LastName") ? (string)args["LastName"] : string.Empty;
				string emailAddress = args.ContainsKey("EmailAddress") ? (string)args["EmailAddress"] : string.Empty;
				// shipping address
				Address shippingAddress = new Address();
				shippingAddress.AddressLineOne = args.ContainsKey("AddressLineOne") ? (string)args["AddressLineOne"] : string.Empty;
				shippingAddress.AddressLineTwo = args.ContainsKey("AddressLineTwo") ? (string)args["AddressLineTwo"] : string.Empty;
				shippingAddress.AddressLineThree = args.ContainsKey("AddressLineThree") ? (string)args["AddressLineThree"] : string.Empty;
				shippingAddress.AddressLineFour = args.ContainsKey("AddressLineFour") ? (string)args["AddressLineFour"] : string.Empty;
				shippingAddress.City = args.ContainsKey("City") ? (string)args["City"] : string.Empty;
				shippingAddress.StateOrProvince = args.ContainsKey("StateOrProvince") ? (string)args["StateOrProvince"] : string.Empty;
				shippingAddress.ZipOrPostalCode = args.ContainsKey("ZipOrPostalCode") ? (string)args["ZipOrPostalCode"] : string.Empty;
				shippingAddress.County = args.ContainsKey("County") ? (string)args["County"] : string.Empty;
				shippingAddress.Country = args.ContainsKey("Country") ? (string)args["Country"] : string.Empty;

				string channel = args.ContainsKey("Channel") ? (string)args["Channel"] : string.Empty;
				string changedBy = args.ContainsKey("ChangedBy") ? (string)args["ChangedBy"] : string.Empty;

				string language = args.ContainsKey("Language") ? (string)args["Language"] : LanguageChannelUtil.GetDefaultCulture();

				Member member = LoadMember(args);

				if (member.MemberStatus == MemberStatusEnum.NonMember)
				{
					throw new LWOperationInvocationException(string.Format("Cannot give rewards to non-member with ipcode {0}.", member.IpCode)) { ErrorCode = 9969 };
				}

				if (member.MemberStatus != MemberStatusEnum.Active && member.MemberStatus != MemberStatusEnum.PreEnrolled)
				{
					throw new LWOperationInvocationException(string.Format("Member is not active.  No rewards can be added.")) { ErrorCode = 3314 };
				}

				IList<RewardOrderItem> rewards = new List<RewardOrderItem>();
				if (!args.ContainsKey("RewardOrderInfo"))
				{
					throw new LWOperationInvocationException("No reward information provided for AddMemberRewards") { ErrorCode = 3360 };
				}

				APIStruct[] attList = (APIStruct[])args["RewardOrderInfo"];

				foreach (APIStruct att in attList)
				{
					RewardOrderItem info = new RewardOrderItem() { PartNumber = string.Empty };
					if (att.Parms.ContainsKey("RewardName"))
					{
						info.RewardName = (string)att.Parms["RewardName"];
					}
					if (att.Parms.ContainsKey("TypeCode"))
					{
						info.TypeCode = (string)att.Parms["TypeCode"];
					}
					if (string.IsNullOrEmpty(info.RewardName) && string.IsNullOrEmpty(info.TypeCode))
					{
						throw new LWOperationInvocationException("No reward name or type code provided.") { ErrorCode = 3340 };
					}

					if (att.Parms.ContainsKey("ExpirationDate"))
					{
						info.ExpirationDate = (DateTime)att.Parms["ExpirationDate"];
					}
					if (att.Parms.ContainsKey("CertificateNumber"))
					{
						info.CertificateNumber = (string)att.Parms["CertificateNumber"];
					}
					if (att.Parms.ContainsKey("VariantPartNumber"))
					{
						info.PartNumber = (string)att.Parms["VariantPartNumber"];
					}
					rewards.Add(info);
				}

				IAddMemberRewardInterceptor interceptor = null;
				LWIntegrationDirectives.APIOperationDirective opDirective = Config.GetOperationDirective(Name) as LWIntegrationDirectives.APIOperationDirective;
				if (opDirective != null && opDirective.Interceptor != null)
				{
					interceptor = InterceptorUtil.GetInterceptor(opDirective.Interceptor) as IAddMemberRewardInterceptor;
				}

				MemberOrder order = MemberRewardsUtil.CreateRewardOrder(member, firstName, lastName, cardId, shippingAddress, channel, changedBy, emailAddress, rewards, FunctionProviderParms, interceptor, language);

				APIArguments resultParams = new APIArguments();
				if (order.OrderItems.Count == 1)
				{
					resultParams.Add("MemberRewardID", order.OrderItems[0].MemberRewardId);
				}
				if (!string.IsNullOrEmpty(order.OrderNumber))
				{
					resultParams.Add("OrderNumber", order.OrderNumber);
				}
				resultParams.Add("CurrencyBalance", member.GetPoints(null, null, null, null).ToString());
				response = SerializationUtils.SerializeResult(Name, Config, resultParams);

				Dictionary<string, object> context = new Dictionary<string, object>();
				context.Add("member", member);
				context.Add("cardId", cardId);
				context.Add("order", order);
				PostProcessSuccessfullInvocation(context);

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
