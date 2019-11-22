using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.bScript;
using Brierley.FrameWork;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Determines if a Member has earned a specific reward.
    /// </summary>
    /// <example>
    ///     Usage : HasReward('RewardName'[,'StartDate','EndDate'])
    /// </example>
    /// <remarks>
    /// RewardName must be a valid reward name
    /// StartDate 
    /// </remarks>
    [Serializable]
    [ExpressionContext(Description = "Determine if a Member has earned a specific reward.",
        DisplayName = "HasReward",
        ExpressionType = ExpressionTypes.Function,
        ExpressionReturns = ExpressionApplications.Booleans,
        WizardDescription = "Has Reward?",
        AdvancedWizard = true,
        WizardCategory = WizardCategories.Profile,
        EvalRequiresMember = true)]


    [ExpressionParameter(Order = 0, Name = "Reward Def Name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which Reward?", Helpers = ParameterHelpers.Reward)]
    [ExpressionParameter(Order = 1, Name = "Start Date", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "From which date?")]
    [ExpressionParameter(Order = 2, Name = "End Date", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "To which date?")]
    public class HasReward : UnaryOperation
    {
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private Expression _rewardName = null;
        private Expression _startDate = null;
        private Expression _endDate = null;


        public HasReward()
        {
        }

        /// <summary>
        /// Internal constructor
        /// this will initialize any parameters sent
        /// </summary>
        /// <param name="rhs"></param>
        public HasReward(Expression rhs)
            : base("HasReward", rhs)
        {
            try
            {
                //Get the parameter list
                ParameterList parameters = rhs as ParameterList;

                //There are two valid sets of parameters, the function can be called with either 1 or three
                if (parameters != null)
                {
                    if (parameters.Expressions.Length == 3)
                    {
                        _rewardName = parameters.Expressions[0];
                        _startDate = parameters.Expressions[1];
                        _endDate = parameters.Expressions[2];
                    }
                    else
                    {
                        throw new CRMException("Invalid Function Call: Wrong number of arguments passed to HasReward.");
                    }
                }
                else
                {
                    // only one parameter
                    _rewardName = this.GetRight();
                }

                if (_rewardName == null)
                {
                    throw new CRMException("Invalid Function Call: HasReward requires one or three parameters");
                }
            }
            catch (Exception e)
            {
                _logger.Error("HasReward", "HasReward", string.Format("Failed executing bscript. Error Message: {0}", e.Message));
                throw e;
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "HasReward('RewardName'[,'StartDate','EndDate'])";
            }
        }

        /// <summary>
        /// This is the main method for the bscript, all the main logic is done here
        /// This bscript function will return either true or false depending if the meber has earned a specific reward or not
        /// There are two ways of calling this function, one only requires the reward name, the second will look a a specific date range.
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            MemberReward reward = null;
            try
            {
                //The context object needs to be member for this bscript.
                Member member = ResolveMember(contextObject.Owner);
                if (member != null)
                {

                    string rewardName = _rewardName.evaluate(contextObject).ToString();
                    DateTime? startDate = _startDate != null ? (DateTime?)(_startDate.evaluate(contextObject)) : null;
                    DateTime? endDate = _endDate != null ? (DateTime?)(_endDate.evaluate(contextObject)) : null;

                    if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
                        throw new CRMException("Start date must be before end date");

                    using (ContentService contentService = LWDataServiceUtil.ContentServiceInstance())
                    using (LoyaltyDataService loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
                    {
                        RewardDef rewardDefiniton = contentService.GetRewardDef(rewardName);

                        if (rewardDefiniton == null)
                        {
                            throw new CRMException("Reward Definition doesn't exist.");
                        }

                        if (startDate != null && endDate != null)
                        {
                            //use the date parameters to see if the member received the reward during the date range
                            reward = loyaltyService.GetMemberRewardsByDefId(member, rewardDefiniton.Id).FirstOrDefault(p => p.DateIssued >= startDate && p.DateIssued <= endDate);
                        }
                        else
                        {
                            //since no date range was sent look at all time
                            reward = loyaltyService.GetMemberRewardsByDefId(member, rewardDefiniton.Id).FirstOrDefault();
                        }
                        //if a reward was found return true
                        if (reward != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    throw new CRMException("HasReward must be evaluated in the context of a loyalty member.");
                }
            }
            catch (Exception e)
            {
                _logger.Error("HasReward", "evaluate", string.Format("Failed executing bscript. Error Message: {0}", e.Message));
                throw e;
            }

        }
    }
}
