using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Promotions
{
	public class AddMemberPromotion : OperationProviderBase
	{
		private const string _className = "AddMemberPromotion";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public AddMemberPromotion() : base("AddMemberPromotion") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to add member promotion.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string promoCode = (string)args["PromotionCode"];
				string certNmbr = args.ContainsKey("CertificateNmbr") ? (string)args["CertificateNmbr"] : string.Empty;

				bool returnDefinition = false;
				if (args.ContainsKey("ReturnDefinition"))
				{
					returnDefinition = (bool)args["ReturnDefinition"];
				}
				string language = LanguageChannelUtil.GetDefaultCulture();
				string channel = LanguageChannelUtil.GetDefaultChannel();
				if (args.ContainsKey("Language"))
				{
					language = (string)args["Language"];
				}
				if (args.ContainsKey("Channel"))
				{
					channel = (string)args["Channel"];
				}
				bool returnAttributes = false;
				if (args.ContainsKey("ReturnAttributes"))
				{
					returnAttributes = (bool)args["ReturnAttributes"];
				}

				if (returnDefinition)
				{
					if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
					{
						throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
					}
					if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
					{
						throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
					}
				}

				Member member = LoadMember(args);

				Promotion promo = ContentService.GetPromotionByCode(promoCode);
				if (promo == null)
				{
					throw new LWOperationInvocationException("No content available that matches the specified criteria.") { ErrorCode = 3362 };
				}

				if (!promo.IsValid())
				{
					throw new LWOperationInvocationException(string.Format("Promotion {0} is not valid anymore.", promoCode)) { ErrorCode = 3343 };
				}

				if (!promo.Targeted)
				{
					throw new LWOperationInvocationException(string.Format("Promotion {0} is not targeted.", promoCode)) { ErrorCode = 3344 };
				}

				MemberPromotion p = new MemberPromotion() { MemberId = member.IpCode, Code = promoCode };
				if (!string.IsNullOrEmpty(certNmbr))
				{
					p.CertificateNmbr = certNmbr;
				}
				LoyaltyDataService.CreateMemberPromotion(p);
				_logger.Trace(
					_className, 
					methodName,
					string.Format("Member with Ipcode {1} added to this promotion {0}.", promoCode, member.IpCode));

				APIStruct memberPromotion = PromotionUtil.SerializeMemberPromotion(language, channel, p, returnDefinition, returnAttributes);
				APIArguments resultParams = new APIArguments();
				resultParams.Add("MemberPromotion", memberPromotion);
				response = SerializationUtils.SerializeResult(Name, Config, resultParams);
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

		protected override void Cleanup()
		{
		}
	}
}
