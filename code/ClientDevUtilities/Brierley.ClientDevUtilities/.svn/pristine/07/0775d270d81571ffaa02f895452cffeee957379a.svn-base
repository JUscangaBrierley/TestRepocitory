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
	public class AssociateMemberSocialHandles : OperationProviderBase
	{
		private const string _className = "AssociateMemberSocialHandles";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public AssociateMemberSocialHandles() : base("AssociateMemberSocialHandles") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided for AssociateMemberSocialHandles.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

				Member member = LoadMember(args);

				if (!args.ContainsKey("MemberSocialHandle"))
				{
					throw new LWOperationInvocationException("No social handles provided for AssociateMemberSocialHandles") { ErrorCode = 3396 };
				}
				APIStruct[] attList = (APIStruct[])args["MemberSocialHandle"];

				IList<MemberSocNet> socNets = new List<MemberSocNet>();
				foreach (APIStruct att in attList)
				{
					MemberSocNet socNet = new MemberSocNet() { MemberId = member.IpCode };
					try
					{
						socNet.ProviderType = (SocialNetworkProviderType)Enum.Parse(typeof(SocialNetworkProviderType), (string)att.Parms["ProviderType"]);
					}
					catch
					{
						string msg = string.Format(_className, methodName, string.Format("Invalid value {0} provided for ProviderType.", (string)att.Parms["ProviderType"]));
						_logger.Error(_className, methodName, msg);
						throw new LWOperationInvocationException(msg) { ErrorCode = 3397 };
					}
					socNet.ProviderUID = (string)att.Parms["ProviderUID"];

					MemberSocNet existing = LoyaltyDataService.GetSocNetForMember(socNet.ProviderType, member.IpCode);
					if (existing == null)
					{
						LoyaltyDataService.CreateMemberSocNet(socNet);
					}
					else if (existing.ProviderUID == socNet.ProviderUID)
					{
						LoyaltyDataService.UpdateMemberSocNet(socNet);
					}
					socNets.Add(socNet);
				}

				Dictionary<string, object> context = new Dictionary<string, object>();
				context.Add("member", member);
				context.Add("SocialHandles", socNets);
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
