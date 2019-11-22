using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement
{
    public class CampaignRuleUtil
    {
        public CampaignRuleUtil()
        {
        }

        public Dictionary<string, string> GetRealtimeCampaigns()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                var campaigns = manager.GetCampaigns(false, CampaignType.RealTime);
                foreach (var campaign in campaigns)
                {
                    list.Add(campaign.Name, campaign.Id.ToString());
                }
            }
            return list;
        }

        public List<CampaignResult> ExecuteRealTimeCampaign(long campaignId, ContextObject co)
        {
            var sm = new StateManager(campaignId, false);
            List<CampaignResult> result = sm.ExecuteRealTimeCampaign(co);
            if (co.Environment.ContainsKey("Result"))
            {
                // append the result.
                List<CampaignResult> prevResult = co.Environment["Result"] as List<CampaignResult>;
                if (prevResult != null)
                {
                    prevResult.InsertRange(prevResult.Count, result);
                }
                else
                {
                    // the previous result was of different type.
                    // don;t know what to do.
                }
            }
            else
            {
                co.Environment.Add(EnvironmentKeys.Result, result);
            }
            if (co.Results == null)
            {
                co.Results = new List<ContextObject.RuleResult>();
            }
            foreach (var r in result)
            {
                co.Results.Add(r);
            }
            return result;
        }

        public List<long> ExtractResultContent(string type, object result)
        {
            List<long> memberRefIds = new List<long>();
            List<CampaignResult> campaignResult = (List<CampaignResult>)result;
            OutputType oType = (OutputType)Enum.Parse(typeof(OutputType), type);
            var results = (from x in campaignResult where x.OutputType == oType select x);
            if (results.Count() > 0)
            {
                foreach (var cr in results)
                {
                    memberRefIds.Add(cr.MemberReferenceId);
                }
            }
            return memberRefIds;
        }

        public static List<CampaignResult> GetCampaignRuleResult(IList<ContextObject.RuleResult> results)
        {
            var ret = new List<CampaignResult>();
            if (results != null && results.Count > 0)
            {
                foreach (ContextObject.RuleResult result in results.Where(o => o is CampaignResult))
                {
                       ret.Add((CampaignResult)result);
                }
            }
            return ret;
        }
    }
}