using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The RewardCertificateCount function returns the count of available certificates for a rule
    /// definition.
    /// </summary>
    /// <example>
    ///     Usage : RewardCertificateCount('Rule Name')
    /// </example>    
    [Serializable]
	[ExpressionContext(Description = "Returns the count of available certificates for a rule.", 
		DisplayName = "RewardCertificateCount", 
		ExcludeContext = ExpressionContexts.Email, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Numbers)]
	public class RewardCertificateCount : UnaryOperation
    {
        private static string _className = "RewardCertificateCount";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        /// <summary>
        /// Public Constructor
        /// </summary>
        public RewardCertificateCount()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal RewardCertificateCount(Expression rhs)
            : base("RewardCertificateCount", rhs)
        {			
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "RewardCertificateCount('Rule Name')";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {            
            string methodName = "evaluate";
            string err = string.Empty;
			using (var loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var contentService = LWDataServiceUtil.ContentServiceInstance())
			{
				string ruleName = (string)this.GetRight().evaluate(contextObject);
				RuleTrigger rule = loyaltyService.GetRuleByName(ruleName);
				if (rule == null)
				{
					err = string.Format("Unable to find rule {0}", ruleName);
					_logger.Error(_className, methodName, err);
					throw new CRMException(err);
				}
				Brierley.FrameWork.Rules.IssueReward ir = (Brierley.FrameWork.Rules.IssueReward)rule.Rule;
				if (ir == null)
				{
					err = string.Format("Unable to deserialize {0}", ruleName);
					_logger.Error(_className, methodName, err);
					throw new CRMException(err);
				}
				RewardDef rdef = contentService.GetRewardDef(ir.RewardType);
				return contentService.HowManyPromoCertificates(ContentObjType.Reward, rdef.CertificateTypeCode, null, null, true);
			}               
        }        
    }
}
