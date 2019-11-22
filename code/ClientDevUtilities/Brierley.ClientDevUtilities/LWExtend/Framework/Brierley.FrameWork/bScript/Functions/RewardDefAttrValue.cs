using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;


namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The RewardDefAttrValue function returns the value for the named property of the reward
    /// definition.
    /// </summary>
    /// <example>
    ///     Usage : RewardDefAttrValue('Reward Name','Property Name')
    /// </example>    
    [Serializable]
	[ExpressionContext(Description = "Returns the value for the named property of the reward.", 
		DisplayName = "RewardDefAttrValue", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Objects,
		EvalRequiresMember = true)]
	public class RewardDefAttrValue : UnaryOperation
    {
        //private static string className = "RewardDefAttrValue";

        private Expression RewardName = null;
        private Expression PropertyName = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public RewardDefAttrValue()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal RewardDefAttrValue(Expression rhs)
            : base("RewardDefAttrValue", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 2)
            {
                RewardName = ((ParameterList)rhs).Expressions[0];
                PropertyName = ((ParameterList)rhs).Expressions[1]; 
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to RewardDefAttrValue.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "RewardDefAttrValue('Reward Name','Property Name')";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {            
            object propertyValue = null;
			Member member = ResolveMember(contextObject.Owner);

            string rewardName = (string)RewardName.evaluate(contextObject);
            string propertyName = (string)PropertyName.evaluate(contextObject);

			using (var service = LWDataServiceUtil.ContentServiceInstance())
			{
				RewardDef rdef = service.GetRewardDef(rewardName);
				PropertyInfo pi = rdef.GetType().GetProperty(propertyName);
				if (pi != null)
				{
					propertyValue = pi.GetValue(rdef, null);
				}
				return propertyValue;
			}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string parseMetaData()
        {
            string meta = RewardName.ToString() + "." + PropertyName.ToString();
            return meta;
        }
    }
}
