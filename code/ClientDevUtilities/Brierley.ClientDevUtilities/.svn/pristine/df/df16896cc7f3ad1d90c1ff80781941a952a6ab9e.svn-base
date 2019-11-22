using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Bonuses
{
	public class GetMemberBonuses : OperationProviderBase
	{
		private const string _className = "GetMemberBonuses";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public GetMemberBonuses()
			: base("GetMemberBonuses")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			const string methodName = "Invoke";

			if (parms == null || parms.Length != 5)
			{
				string errMsg = "Invalid parameters provided for GetMemberBonuses.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			bool activeOnly = (bool)parms[0];

			string language = (string)parms[1];
			if (string.IsNullOrEmpty(language))
			{
				language = LanguageChannelUtil.GetDefaultCulture();
			}
			string channel = (string)parms[2];
			if (string.IsNullOrEmpty(channel))
			{
				channel = LanguageChannelUtil.GetDefaultChannel();
			}

			int startIndex = (int)parms[3];
			int batchSize = (int)parms[4];

			LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

			Member member = token.CachedMember;

			IList<MemberBonus> bonuses = LoyaltyService.GetMemberBonusesByMember(member.IpCode, null, activeOnly, batchInfo);

			var ret = new List<MGMemberBonus>();
			foreach (MemberBonus bonus in bonuses)
			{
				ret.Add(MGMemberBonus.Hydrate(member, bonus, language, channel));
			}
			return ret;
		}
	}
}