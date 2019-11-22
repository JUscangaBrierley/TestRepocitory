using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Stores
{
	public class SaveTop10Stores : OperationProviderBase
	{
		#region Fields
		private const string _className = "SaveTop10Stores";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
		#endregion

		public SaveTop10Stores() : base("SaveTop10Stores") { }

		#region Private Helpers
		#endregion

		#region Invocation
		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			string methodName = "Invoke";

			Action invalidArgs = delegate
			{
				string errMsg = "Invalid parameters provided for SaveTop10Stores.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			};

			if (parms == null || parms.Length != 1 || parms[0] == null)
			{
				invalidArgs();
			}

			List<long> storeIds = parms[0] as List<long>;
			if (storeIds == null || storeIds.Count == 0)
			{
				invalidArgs();
			}

			Member member = token.CachedMember;
			LoyaltyService.DeleteMemberStoreByMember(member.IpCode);
			List<MGStoreDef> top10Stores = new List<MGStoreDef>();

			List<StoreDef> storeDefList = ContentService.GetAllStoreDefs(storeIds.ToArray());
			List<MemberStore> memberStores = new List<MemberStore>();
			foreach (StoreDef storeDef in storeDefList)
			{
				MemberStore mStore = new MemberStore()
				{
					MemberId = member.IpCode,
					StoreDefId = storeDef.StoreId,
				};
				for (int idxOrder = 0; idxOrder < storeIds.Count; idxOrder++)
				{
					if (storeIds[idxOrder] == storeDef.StoreId)
					{
						mStore.PreferenceOrder = idxOrder + 1;
						break;
					}
				}
				memberStores.Add(mStore);
			}
			LoyaltyService.SaveMemberStores(memberStores);
			foreach (MemberStore mStore in memberStores)
			{
				StoreDef storeDef = ContentService.GetStoreDef(mStore.StoreDefId);
				MGStoreDef mgStoreDef = MGStoreDef.Hydrate(storeDef);
				top10Stores.Add(mgStoreDef);
			}
			token.Top10Stores = top10Stores;

			return "";
		}
		#endregion
	}
}