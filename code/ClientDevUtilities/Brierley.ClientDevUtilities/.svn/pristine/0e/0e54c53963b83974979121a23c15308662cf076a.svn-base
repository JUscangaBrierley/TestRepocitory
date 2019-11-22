using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class LockdownMember : OperationProviderBase
	{
		public LockdownMember() : base("LockdownMember") { }

		public override string Invoke(string source, string parms)
		{
			try
			{
				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string response = string.Empty;
				DateTime effectiveDate = args.ContainsKey("EffectiveDate") ? (DateTime)args["EffectiveDate"] : DateTime.Now;
				string reason = args.ContainsKey("UpdateMemberStatusReason") ? (string)args["UpdateMemberStatusReason"] : string.Empty;

				Member member = LoadMember(args);
				switch (member.MemberStatus)
				{
					case MemberStatusEnum.PreEnrolled:
					case MemberStatusEnum.Active:
						member.NewStatus = MemberStatusEnum.Locked;
						member.NewStatusEffectiveDate = effectiveDate;
						member.StatusChangeReason = reason;
						LoyaltyDataService.SaveMember(member);
						break;
					case MemberStatusEnum.Locked:
						break;
					case MemberStatusEnum.Disabled:
						throw new LWOperationInvocationException(string.Format("This member is already deactivated.  It cannot be locked down.")) { ErrorCode = 3303 };
					case MemberStatusEnum.Merged:
						throw new LWOperationInvocationException(string.Format("This member is in merged status.  It cannot be locked down.")) { ErrorCode = 3392 };
					case MemberStatusEnum.NonMember:
						throw new LWOperationInvocationException(string.Format("This entity is a non-member.  It cannot be locked down.")) { ErrorCode = 3393 };
					case MemberStatusEnum.Terminated:
						throw new LWOperationInvocationException(string.Format("This member is already in terminated status.  It cannot be locked down.")) { ErrorCode = 3303 };
				}

				Dictionary<string, object> context = new Dictionary<string, object>();
				context.Add("member", member);
				PostProcessSuccessfullInvocation(context);

				return response;
			}
			catch (LWOperationInvocationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message);
			}
		}
	}
}
