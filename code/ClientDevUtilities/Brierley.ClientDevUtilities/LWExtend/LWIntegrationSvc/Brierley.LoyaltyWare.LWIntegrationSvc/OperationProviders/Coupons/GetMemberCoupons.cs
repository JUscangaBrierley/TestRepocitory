using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
	public class GetMemberCoupons : OperationProviderBase
	{
		private const string _className = "GetMemberCoupons";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetMemberCoupons() : base("GetMemberCoupons") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				string language = LanguageChannelUtil.GetDefaultCulture();
				string channel = LanguageChannelUtil.GetDefaultChannel();

				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to retrieve member coupons.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				if (args.ContainsKey("Language"))
				{
					language = (string)args["Language"];
				}
				if (args.ContainsKey("Channel"))
				{
					channel = (string)args["Channel"];
				}

				string typeCode = args.ContainsKey("TypeCode") ? (string)args["TypeCode"] : string.Empty;

				bool returnAttributes = false;
				if (args.ContainsKey("ReturnAttributes"))
				{
					returnAttributes = (bool)args["ReturnAttributes"];
				}

				int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
				int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;

				LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

                bool activeOnly = args.ContainsKey("ActiveOnly") ? (bool)args["ActiveOnly"] : false;

                Member member = LoadMember(args);

				// validate language and channel
				if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
				{
					throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
				}
				if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
				{
					throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
				}

				IList<MemberCoupon> coupons = null;
				if (string.IsNullOrEmpty(typeCode))
				{
					coupons = LoyaltyDataService.GetMemberCouponsByMember(member.IpCode, batchInfo, false, activeOnly);
				}
				else
				{
					coupons = LoyaltyDataService.GetMemberCouponsByMemberByTypeCode(member.IpCode, typeCode, batchInfo);
				}

				if (coupons.Count > 0)
				{
					APIStruct[] memberCoupons = new APIStruct[coupons.Count];
					int idx = 0;
					foreach (MemberCoupon coupon in coupons)
					{
						memberCoupons[idx++] = CouponHelper.SerializeMemberCoupon(member, language, channel, coupon, returnAttributes);
					}
					APIArguments resultParams = new APIArguments();
					resultParams.Add("MemberCoupon", memberCoupons);
					response = SerializationUtils.SerializeResult(Name, Config, resultParams);
					return response;
				}
				else
				{
					throw new LWOperationInvocationException("No member coupons found.") { ErrorCode = 3362 };
				}
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
