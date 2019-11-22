using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class ActivateMember : OperationProviderBase
	{
		public ActivateMember() : base("ActivateMember") { }

		public override string Invoke(string source, string parms)
		{
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided for activate member.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

				DateTime effectiveDate = args.ContainsKey("EffectiveDate") ? (DateTime)args["EffectiveDate"] : DateTime.Now;
				string reason = args.ContainsKey("UpdateMemberStatusReason") ? (string)args["UpdateMemberStatusReason"] : string.Empty;
				bool activateCards = args.ContainsKey("ActivateInactiveCards") ? (bool)args["ActivateInactiveCards"] : false;

				Member member = LoadMember(args);
				switch (member.MemberStatus)
				{
					case MemberStatusEnum.PreEnrolled:
					case MemberStatusEnum.Disabled:
					case MemberStatusEnum.Locked:
						member.NewStatus = MemberStatusEnum.Active;
						member.NewStatusEffectiveDate = effectiveDate;
						member.StatusChangeReason = reason;
						if (activateCards)
						{
							foreach (VirtualCard vc in member.LoyaltyCards)
							{
								if (vc.Status == VirtualCardStatusType.InActive)
								{
									vc.NewStatus = VirtualCardStatusType.Active;
									vc.NewStatusEffectiveDate = effectiveDate;
									vc.StatusChangeReason = reason;
								}
							}
						}
						LoyaltyDataService.SaveMember(member);
						break;
					case MemberStatusEnum.Active:
						break;
					case MemberStatusEnum.Merged:
						throw new LWOperationInvocationException(string.Format("This member is in merged status.  It cannot be activated anymore.")) { ErrorCode = 3392 };
					case MemberStatusEnum.NonMember:
						throw new LWOperationInvocationException(string.Format("This entity is a non-member.  It cannot be re-activated.")) { ErrorCode = 3393 };
					case MemberStatusEnum.Terminated:
						throw new LWOperationInvocationException(string.Format("This member is in terminated status.  It cannot be re-activated.")) { ErrorCode = 3303 };
				}

				// Do post processing
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
