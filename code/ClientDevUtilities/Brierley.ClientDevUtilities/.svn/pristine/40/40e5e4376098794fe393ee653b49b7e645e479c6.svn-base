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
    /// The PointsToRewardChoice function gets the points needed to get to the next chosen reward.
    /// </summary>
    /// <example>
    /// PointsToRewardChoice()
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Returns the number of points needed for the member to earn their next chosen reward.",
        DisplayName = "PointsToRewardChoice",
        ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
        ExpressionReturns = ExpressionApplications.Numbers,
        WizardDescription = "Points to chosen reward",
        AdvancedWizard = false,
        WizardCategory = WizardCategories.Points,
        EvalRequiresMember = true)]

    public class PointsToRewardChoice : UnaryOperation
    {
        public PointsToRewardChoice()
        {
        }

        internal PointsToRewardChoice(Expression rhs)
            : base("GetPointsToNextReward", rhs)
        {
        }

        public new string Syntax
        {
            get
            {
                return "PointsToRewardChoice()";
            }
        }

        public override object evaluate(ContextObject contextObject)
        {
            Member member = ResolveMember(contextObject.Owner);
            if (member == null)
            {
                throw new CRMException("PointsToRewardChoice must be evaluated in the context of a loyalty member.");
            }

            return member.GetPointsToRewardChoice();
        }
    }
}
