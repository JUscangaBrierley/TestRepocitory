using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class GetMemberTiers : OperationProviderBase
    {
        private const string _className = "GetMemberTiers";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        public GetMemberTiers() : base("GetMemberTiers") { }

        public override string Invoke(string source, string parms)
        {
            string methodName = "Invoke";
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided to retrieve member tiers.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

                DateTime startDate = args.ContainsKey("StartDate") ? (DateTime)args["StartDate"] : DateTimeUtil.MinValue;
                DateTime endDate = args.ContainsKey("EndDate") ? (DateTime)args["EndDate"] : DateTimeUtil.MaxValue;
                int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
                int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;

                Member member = LoadMember(args);

                List<MemberTier> memberTiers = member.GetTiers() as List<MemberTier>;
                if (memberTiers.Count == 0)
                    throw new LWOperationInvocationException("No member tiers found.") { ErrorCode = 3362 };

                List<APIStruct> memberTiersStructs = new List<APIStruct>();
                foreach (MemberTier memberTier in memberTiers.Where(m => m.FromDate <= endDate && m.ToDate >= startDate)
                                                             .OrderByDescending(m => m.FromDate).Skip(startIndex ?? 0)
                                                             .Take(batchSize ?? memberTiers.Count))
                {
                    memberTiersStructs.Add(SerializeMemberTier(memberTier));
                }
                APIArguments resultParams = new APIArguments();
                resultParams.Add("MemberTier", memberTiersStructs.ToArray());
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);
                return response;
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

        private APIStruct SerializeMemberTier(MemberTier memberTier)
        {
            APIArguments args = new APIArguments();
            args.Add("FromDate", memberTier.FromDate);
            args.Add("ToDate", memberTier.ToDate);
            args.Add("Description", memberTier.Description);

            TierDef tierDef = LoyaltyDataService.GetTierDef(memberTier.TierDefId);
            APIArguments tparms = new APIArguments();
            tparms.Add("Name", tierDef.Name);
            if (!string.IsNullOrEmpty(tierDef.DisplayText))
            {
                tparms.Add("DisplayText", tierDef.DisplayText);
            }
            if (!string.IsNullOrEmpty(tierDef.Description))
            {
                tparms.Add("Description", tierDef.Description);
            }
            tparms.Add("EntryPoints", tierDef.EntryPoints);
            tparms.Add("ExitPoints", tierDef.ExitPoints);
            if (!string.IsNullOrEmpty(tierDef.PointTypeNames))
            {
                tparms.Add("PointTypeNames", tierDef.PointTypeNames);
            }
            if (!string.IsNullOrEmpty(tierDef.PointEventNames))
            {
                tparms.Add("PointEventNames", tierDef.PointEventNames);
            }
            if (tierDef.Attributes.Count > 0)
            {
                APIStruct[] atts = new APIStruct[tierDef.Attributes.Count];
                int index = 0;
                foreach (ContentAttribute ra in tierDef.Attributes)
                {
                    ContentAttributeDef def = ContentService.GetContentAttributeDef(ra.ContentAttributeDefId);
                    APIArguments attparms = new APIArguments();
                    attparms.Add("AttributeName", def.Name);
                    attparms.Add("AttributeValue", ra.Value);
                    APIStruct v = new APIStruct() { Name = "ContentAttributes", IsRequired = false, Parms = attparms };
                    atts[index++] = v;
                }
                tparms.Add("ContentAttributes", atts);
            }
            APIStruct pevent = new APIStruct() { Name = "TierDef", IsRequired = false, Parms = tparms };

            args.Add("TierDef", pevent);

            return new APIStruct() { Name = "MemberTier", IsRequired = true, Parms = args };
        }
    }
}
