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
	public class TerminateMember : OperationProviderBase
	{
		public TerminateMember() : base("TerminateMember") { }

		public override string Invoke(string source, string parms)
		{
			try
			{
				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string response = string.Empty;
				DateTime effectiveDate = args.ContainsKey("EffectiveDate") ? (DateTime)args["EffectiveDate"] : DateTime.Now;
				string reason = (string)args["UpdateMemberStatusReason"];

				Member member = LoadMember(args);

				switch (member.MemberStatus)
				{
					case MemberStatusEnum.PreEnrolled:
					case MemberStatusEnum.Active:
					case MemberStatusEnum.Locked:
					case MemberStatusEnum.Disabled:
						LoyaltyDataService.CancelOrTerminateMember(member, effectiveDate, reason, true, new MemberCancelOptions());
						break;
					case MemberStatusEnum.Terminated:
						break;
					case MemberStatusEnum.Merged:
						throw new LWOperationInvocationException(string.Format("This member is in merged status.  It cannot be terminated.")) { ErrorCode = 3392 };
					case MemberStatusEnum.NonMember:
						throw new LWOperationInvocationException(string.Format("This entity is a non-member.  It cannot be terminated.")) { ErrorCode = 3393 };
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
