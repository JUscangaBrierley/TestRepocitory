using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class ConvertToMember : OperationProviderBase
	{
		private const string _className = "ConvertToMember";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public ConvertToMember() : base("ConvertToMember") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";

			try
			{
				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string response = string.Empty;
				DateTime effectiveDate = args.ContainsKey("EffectiveDate") ? (DateTime)args["EffectiveDate"] : DateTime.Now;

				Member member = LoadMember(args);
				if (member.MemberStatus == MemberStatusEnum.NonMember)
				{
					LoyaltyDataService.ConvertToMember(member, effectiveDate);
				}
				else
				{
					_logger.Debug(_className, methodName, string.Format("Member with ipcode {0} is already a member.", member.IpCode));
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
