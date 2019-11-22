using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The GetPoints function the members points value for the given date range and point type.
    /// </summary>
    /// <example>
    ///     Usage : GetPoints(['PointType'],Start Date, End Date)
    /// </example>
    /// <remarks>
    ///     PointType is optional.
    ///     Start and End dates must be valid date formated strings.
    /// </remarks>
    [Serializable]
    [ExpressionContext(Description = "Returns the count of rewards a member has based on the criteria passed to this bscript.",
        DisplayName = "GetMemberRewardCount",
        ExpressionType = ExpressionTypes.Function,
        ExpressionReturns = ExpressionApplications.Numbers,

        WizardDescription = "Reward Count",
        AdvancedWizard = true,
        WizardCategory = WizardCategories.Profile,
        EvalRequiresMember = true)]

    [ExpressionParameter(Order = 0, Name = "Reward(s)", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which reward(s) are you looking for ';' delimited")]
    [ExpressionParameter(Order = 1, Name = "Not These Reward(s)", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which reward(s) would you like excluded from this count?")]
    [ExpressionParameter(Order = 2, Name = "Redeemed", Type = ExpressionApplications.Booleans, Optional = true, WizardDescription = "Include Redeemed rewards?")]
    [ExpressionParameter(Order = 3, Name = "Expired", Type = ExpressionApplications.Booleans, Optional = true, WizardDescription = "Include Expired rewards?")]
    [ExpressionParameter(Order = 4, Name = "IssuedFrom", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "Date the rewards were issued from")]
    [ExpressionParameter(Order = 5, Name = "IssuedTo", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "Date the rewards were issued to")]
    [ExpressionParameter(Order = 6, Name = "RedeemedFrom", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "Date the rewards were redeemed from")]
    [ExpressionParameter(Order = 7, Name = "RedeemedTo", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "Date the rewards were redeemed to")]
    public class GetMemberRewardCount : UnaryOperation
    {
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private const string _className = "GetMemberRewardCount";
        private Expression _includeRewardsExp;
        private Expression _excludeRewardsExp;
        private Expression _redeemedExp;
        private Expression _expiredExp;
        private Expression _issuedFromExp;
        private Expression _issuedToExp;
        private Expression _redeemedFromExp;
        private Expression _redeemedToExp;

        public GetMemberRewardCount()
        {

        }

        public new string Syntax
        {
            get { return "GetMemberRewardCount('includeReward1;includeReward2', 'excludeReward3;excludeReward4', [Redeemed], [Expired], Issued From Date, Issued To Date, Redeemed From Date, Redeemed To Date)"; }
        }

        public GetMemberRewardCount(Expression rhs)
            :base("GetMemberRewardCount", rhs)
        {
            if(rhs == null)
            {
                return;
            }
            else if(rhs is ParameterList)
            {
                ParameterList args = rhs as ParameterList;
                switch((args.Expressions.Length))
                {
                    case 1:
                        _includeRewardsExp = args.Expressions[0];
                        break;
                    case 2:
                        _includeRewardsExp = args.Expressions[0];
                        _excludeRewardsExp = args.Expressions[1];
                        break;
                    case 3:
                        _includeRewardsExp = args.Expressions[0];
                        _excludeRewardsExp = args.Expressions[1];
                        _redeemedExp = args.Expressions[2];
                        break;
                    case 4:
                        _includeRewardsExp = args.Expressions[0];
                        _excludeRewardsExp = args.Expressions[1];
                        _redeemedExp = args.Expressions[2];
                        _expiredExp = args.Expressions[3];
                        break;
                    case 6:
                        _includeRewardsExp = args.Expressions[0];
                        _excludeRewardsExp = args.Expressions[1];
                        _redeemedExp = args.Expressions[2];
                        _expiredExp = args.Expressions[3];
                        _issuedFromExp = args.Expressions[4];
                        _issuedToExp = args.Expressions[5];
                        break;
                    case 8:
                        _includeRewardsExp = args.Expressions[0];
                        _excludeRewardsExp = args.Expressions[1];
                        _redeemedExp = args.Expressions[2];
                        _expiredExp = args.Expressions[3];
                        _issuedFromExp = args.Expressions[4];
                        _issuedToExp = args.Expressions[5];
                        _redeemedFromExp = args.Expressions[6];
                        _redeemedToExp = args.Expressions[7];
                        break;
                    default:
                        throw new LWBScriptException("Unsupported number of paramters passed to GetMemberRewardCount acceptable number of parameters is between 0 and 4");
                }
            }
            else
            {
                _includeRewardsExp = rhs;
            }
        }

        public override object evaluate(ContextObject contextObject)
        {
            string methodName = "evaluate";
            //grab the member
            Member member = ResolveMember(contextObject.Owner);
            if (member == null)
            {
                throw new CRMException("GetMemberRewardCount must be evaluated in the context of a loyalty member.");
            }

            //parse and evaluate the parameters
            string includes = _includeRewardsExp != null ? (string)_includeRewardsExp.evaluate(contextObject) : null;
            string excludes = _excludeRewardsExp != null ? (string)_excludeRewardsExp.evaluate(contextObject) : null;
            bool? redeemed = null;
            if(_redeemedExp != null)
            {
                object evalObj = _redeemedExp.evaluate(contextObject);
                if (!String.IsNullOrEmpty(evalObj.ToString()))
                {
                    redeemed = (bool?)Convert.ToBoolean(evalObj);
                }
            }
            bool? expired = null;
            if(_expiredExp != null)
            {
                object evalObj = _expiredExp.evaluate(contextObject);
                if (!String.IsNullOrEmpty(evalObj.ToString()))
                {
                    expired = (bool?)Convert.ToBoolean(evalObj);
                }
            }
            DateTime? issuedFrom = null;
            if (_issuedFromExp != null)
            {
                issuedFrom = (DateTime?)_issuedFromExp.evaluate(contextObject);
            }
            DateTime? issuedTo = null;
            if (_issuedToExp != null)
            {
                issuedTo = (DateTime?)_issuedToExp.evaluate(contextObject);
            }
            DateTime? redeemedFrom = null;
            if (_redeemedFromExp != null)
            {
                redeemedFrom = (DateTime?)_redeemedFromExp.evaluate(contextObject);
            }
            DateTime? redeemedTo = null;
            if (_redeemedToExp != null)
            {
                redeemedTo = (DateTime?)_redeemedToExp.evaluate(contextObject);
            }

            //load the IDs of the reward deffs included/excluded
            using (LoyaltyDataService ldService = LWDataServiceUtil.LoyaltyDataServiceInstance())
            using (ContentService cService = LWDataServiceUtil.ContentServiceInstance())
            {
                IList<long> includeRewardDefs = new List<long>();
                IList<long> excludeRewardDefs = new List<long>();
                if (!String.IsNullOrEmpty(includes))
                {
                    string[] rewardNames = includes.Split(';');
                    foreach (string rewardName in rewardNames)
                    {
                        RewardDef reward = cService.GetRewardDef(rewardName);
                        if(reward == null)
                        {
                            _logger.Warning(_className, methodName, String.Format("RewardName '{0}' not found ignoring this one for inclusion", rewardName));
                            continue;
                        }
                        includeRewardDefs.Add(reward.Id);
                    }
                }
                if (!String.IsNullOrEmpty(excludes))
                {

                    string[] rewardNames = excludes.Split(';');
                    foreach (string rewardName in rewardNames)
                    {
                        RewardDef reward = cService.GetRewardDef(rewardName);
                        if (reward == null)
                        {
                            _logger.Warning(_className, methodName, String.Format("RewardName '{0}' not found ignoring this one for exclusion", rewardName));
                            continue;
                        }
                        excludeRewardDefs.Add(reward.Id);
                    }
                }
                
                long[] includeArray = includeRewardDefs.Count > 0 ? includeRewardDefs.ToArray() : null;
                long[] excludeArray = excludeRewardDefs.Count > 0 ? excludeRewardDefs.ToArray() : null;

                //run the lookup
                List<long> foundRewards = ldService.GetMemberRewardIds(member, includeArray, excludeArray, issuedFrom, issuedTo, redeemedFrom, redeemedTo, redeemed, expired);
                if(foundRewards == null)
                {
                    return 0;
                }
                return foundRewards.Count;
            }
        }
    }
}
