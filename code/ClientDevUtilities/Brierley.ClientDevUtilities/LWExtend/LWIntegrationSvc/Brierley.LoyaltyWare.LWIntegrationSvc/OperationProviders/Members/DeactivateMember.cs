using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class DeactivateMember : OperationProviderBase
	{
		public DeactivateMember() : base("DeactivateMember") { }

		public override string Invoke(string source, string parms)
		{
			try
			{
				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string response = string.Empty;
				DateTime effectiveDate = args.ContainsKey("EffectiveDate") ? (DateTime)args["EffectiveDate"] : DateTime.Now;
				string reason = args.ContainsKey("UpdateMemberStatusReason") ? (string)args["UpdateMemberStatusReason"] : string.Empty;
				bool deactivateCards = args.ContainsKey("DeactivateActiveCards") ? (bool)args["DeactivateActiveCards"] : false;
				bool expirePoints = args.ContainsKey("ExpirePoints") ? (bool)args["ExpirePoints"] : true;
				bool cancelTiers = args.ContainsKey("CancelTiers") ? (bool)args["CancelTiers"] : true;
				bool cancelRewards = args.ContainsKey("CancelRewards") ? (bool)args["CancelRewards"] : true;
				bool cancelPromotions = args.ContainsKey("CancelPromotions") ? (bool)args["CancelPromotions"] : true;
				bool cancelBonuses = args.ContainsKey("CancelBonuses") ? (bool)args["CancelBonuses"] : true;
				bool cancelCoupons = args.ContainsKey("CancelCoupons") ? (bool)args["CancelCoupons"] : true;

				MemberCancelOptions options = new MemberCancelOptions()
				{
					DeactivateCard = deactivateCards,
					ExpirePoints = expirePoints,
					CancelTiers = cancelTiers,
					CancelRewards = cancelRewards,
					CancelPromotions = cancelPromotions,
					CancelBonuses = cancelBonuses,
					CancelCoupons = cancelCoupons
				};

				Member member = LoadMember(args);
				switch (member.MemberStatus)
				{
					case MemberStatusEnum.PreEnrolled:
					case MemberStatusEnum.Active:
						LoyaltyDataService.CancelOrTerminateMember(member, effectiveDate, reason, false, options);
						break;
					case MemberStatusEnum.Locked:
					case MemberStatusEnum.Disabled:
						break;
					case MemberStatusEnum.Terminated:
						throw new LWOperationInvocationException(string.Format("This member is already in terminated status.  It cannot be deactivated.")) { ErrorCode = 3303 };
					case MemberStatusEnum.Merged:
						throw new LWOperationInvocationException(string.Format("This member is in merged status.  It cannot be deactivated.")) { ErrorCode = 3392 };
					case MemberStatusEnum.NonMember:
						throw new LWOperationInvocationException(string.Format("This entity is a non-member.  It cannot be deactivated.")) { ErrorCode = 3393 };
				}

				Dictionary<string, object> context = new Dictionary<string, object>();
				context.Add("member", member);
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
