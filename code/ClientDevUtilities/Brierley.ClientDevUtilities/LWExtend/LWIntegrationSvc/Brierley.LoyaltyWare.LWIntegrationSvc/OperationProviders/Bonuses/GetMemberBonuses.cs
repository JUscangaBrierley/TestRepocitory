using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Bonuses
{
	public class GetMemberBonuses : OperationProviderBase
	{
		private const string _className = "GetMemberBonuses";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetMemberBonuses() : base("GetMemberBonuses") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				string language = LanguageChannelUtil.GetDefaultCulture();
				string channel = LanguageChannelUtil.GetDefaultChannel();

				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to retrieve member bonuses.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				if (args.ContainsKey("Language"))
				{
					language = (string)args["Language"];
				}
				if (args.ContainsKey("Channel"))
				{
					channel = (string)args["Channel"];
				}

				int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
				int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;

				LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);
                
                List<MemberBonusStatus> statuses = new List<MemberBonusStatus>();
                if (args.ContainsKey("Status"))
                {
                    string[] statusArray = (string[])args["Status"];
                    if (statusArray != null && statusArray.Length > 0)
                    {
                        foreach (string s in statusArray)
                        {
                            MemberBonusStatus parsed = MemberBonusStatus.Completed;
                            if (!Enum.TryParse<MemberBonusStatus>(s, out parsed))
                            {
                                string msg = string.Format("Invalid MemberBonusStatus '{0}' provided for GetMemberBonuses.", s);
                                _logger.Error(_className, methodName, msg);
                                throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
                            }
                            statuses.Add(parsed);
                        }
                    }
                }

                bool activeOnly = args.ContainsKey("ActiveOnly") ? (bool)args["ActiveOnly"] : false;

                Member member = LoadMember(args);

				// validate language and channel
				if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
				{
					throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
				}
				if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
				{
					throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
				}

				IList<MemberBonus> bonuses = LoyaltyDataService.GetMemberBonusesByMember(member.IpCode, statuses, activeOnly, batchInfo);
				if (bonuses.Count > 0)
				{
					APIStruct[] memberBonuses = new APIStruct[bonuses.Count];
					int idx = 0;
					foreach (MemberBonus bonus in bonuses)
					{
						APIStruct rv = BonusUtil.SerializeMemberBonus(language, channel, bonus);
						memberBonuses[idx++] = rv;
					}
					APIArguments resultParams = new APIArguments();
					resultParams.Add("MemberBonus", memberBonuses);
					response = SerializationUtils.SerializeResult(Name, Config, resultParams);
					return response;
				}
				else
				{
					throw new LWOperationInvocationException("No member bonuses found.") { ErrorCode = 3362 };
				}
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
	}
}
