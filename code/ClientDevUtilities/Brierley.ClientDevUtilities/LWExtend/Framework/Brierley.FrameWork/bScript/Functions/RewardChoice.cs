using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// This function will return the name of the member's reward choice. If the member has not made a choice or their current choice is invalid, this function 
    /// will return the name of the tier's default reward choice. The member must be in a tier and the tier must have rewards associated with it. In the event 
    /// that the required conditions are not met, this method will return null.
    /// </summary>
    /// <example>
    /// RewardChoice()
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Returns the name of the member's current reward choice.",
        DisplayName = "RewardChoice",
        ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
        ExpressionReturns = ExpressionApplications.Numbers,
        WizardDescription = "Member's current reward choice",
        AdvancedWizard = false,
        WizardCategory = WizardCategories.Profile,
        EvalRequiresMember = true)]

    public class RewardChoice : UnaryOperation
    {
        public RewardChoice()
        {
        }

        internal RewardChoice(Expression rhs)
            : base("RewardChoice", rhs)
        {
        }

        public new string Syntax
        {
            get
            {
                return "RewardChoice()";
            }
        }

        public override object evaluate(ContextObject contextObject)
        {
            Member member = ResolveMember(contextObject.Owner);
            if (member == null)
            {
                throw new CRMException("RewardChoice must be evaluated in the context of a loyalty member.");
            }
            
            string ret = null;

            using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                RewardDef reward = loyalty.GetCurrentRewardChoiceOrDefault(member);
                if (reward != null)
                {
                    ret = reward.Name;
                }
            }
            return ret;
        }
    }
}
