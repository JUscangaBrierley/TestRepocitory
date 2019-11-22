using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Promotions
{
	public class EnrollMemberPromotion : OperationProviderBase
	{
		private const string _className = "EnrollMemberPromotion";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public EnrollMemberPromotion()
			: base("EnrollMemberPromotion")
		{
		}

		public override string Invoke(string source, string parameters)
		{
			const string methodName = "Invoke";
			try
			{
				if (string.IsNullOrEmpty(parameters))
				{
					throw new LWOperationInvocationException("No parameters provided to enroll member promotion.", 3300);
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parameters);

				Member member = LoadMember(args);

				if (member.MemberStatus != MemberStatusEnum.Active)
				{
					throw new LWOperationInvocationException(string.Format("Member is not active and therefore cannot enroll in promotions."), 3314);
				}

				string promoCode = args.ContainsKey("PromotionCode") ? (string)args["PromotionCode"] : string.Empty;
				if (string.IsNullOrEmpty(promoCode))
				{
					throw new LWOperationInvocationException(string.Format("No promotion code provided."), 3340);
				}

				bool returnDefinition = args.ContainsKey("ReturnDefinition") ? (bool)args["ReturnDefinition"] : false;
				string language = args.ContainsKey("Language") ? (string)args["Language"] : LanguageChannelUtil.GetDefaultCulture();
				string channel = args.ContainsKey("Channel") ? (string)args["Channel"] : LanguageChannelUtil.GetDefaultChannel();
				bool returnAttributes = args.ContainsKey("ReturnAttributes") ? (bool)args["ReturnAttributes"] : false;
				bool useCertificate = args.ContainsKey("UseCertificate") ? (bool)args["UseCertificate"] : false;

				if (returnDefinition)
				{
					if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
					{
						throw new LWOperationInvocationException("Specified language is not defined.", 6002);
					}
					if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
					{
						throw new LWOperationInvocationException("Specified channel is not defined.", 6003);
					}
				}
				
				Promotion promo = ContentService.GetPromotionByCode(promoCode);
				if (promo == null)
				{
					throw new LWOperationInvocationException("No promotion exists that matches the specified code.", 3362);
				}

				if (promo.EnrollmentSupportType == PromotionEnrollmentSupportType.None)
				{
					throw new LWOperationInvocationException(string.Format("Promotion {0} does not support enrollment.", promoCode), 3381);
				}
				
				if (!promo.IsValid())
				{
					throw new LWOperationInvocationException(string.Format("Promotion {0} is no longer valid.", promoCode), 3343);
				}

				MemberPromotion ret = null;
				try
				{
					ret = LoyaltyDataService.EnrollMemberPromotion(promo.Id, member, useCertificate);
				}
				catch (LWException ex)
				{
					//this is sort of lame. The framework method will throw an exception if the member is attempting to enroll in a targeted promotion 
					//that the member has not been targeted for. Cool, but CDIS is supposed to return an error with an arbitrary code, so instead of 
					//duplicating the effort, we'll catch the exception, check its message and throw our own. The client should have already checked 
					//the promotion definition against the member's promotion list, so the odds of this happening should be slim, anyway.
					if (ex.Message.Contains("the promotion is targeted and the member is not in the promotion"))
					{
						throw new LWOperationInvocationException(string.Format("Promotion {0} is targeted and no member promotion was found.", promoCode), ex) { ErrorCode = 3362 };
					}
					throw;
				}

				APIStruct apiStruct = PromotionUtil.SerializeMemberPromotion(language, channel, ret, returnDefinition, returnAttributes);
				APIArguments resultParams = new APIArguments();
				resultParams.Add("MemberPromotion", apiStruct);
				return SerializationUtils.SerializeResult(Name, Config, resultParams);
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw new LWOperationInvocationException(ex.Message, 1);
			}
		}
	}
}
