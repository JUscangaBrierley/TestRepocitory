using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The MemberRewardAttrValue function returns the property value of the reward at given by RowIndex
    /// </summary>
    /// <example>
    ///     Usage : MemberRewardAttrValue('PropertyName','RowIndex')
    /// </example>
    /// <remarks>
    /// Attribute Set Name must be the name of a valid global attribute set.
    /// attrName must be the name of a valid attribute in the named attribute set.
    /// WhereClause must be a valid where clause of an SQL statement that would result in one row.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the property value of the reward at given by RowIndex.", 
		DisplayName = "MemberRewardAttrValue", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Objects,
		EvalRequiresMember = true)]
	public class MemberRewardAttrValue : UnaryOperation
    {
        //private static string className = "MemberRewardAttrValue";

        private Expression PropertyName = null;
        private Expression RowIndex = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public MemberRewardAttrValue()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal MemberRewardAttrValue(Expression rhs)
            : base("MemberRewardAttrValue", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 2)
            {
                PropertyName = ((ParameterList)rhs).Expressions[0];
                RowIndex = ((ParameterList)rhs).Expressions[1];
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to MemberRewardAttrValue.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "MemberRewardAttrValue('PropertyName','RowIndex')";
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
			if (member != null)
			{
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					var rewards = service.GetMemberRewards(member, null);
					if (rewards != null && rewards.Count > 0)
					{
						int theIndex = System.Int32.Parse(this.RowIndex.evaluate(contextObject).ToString());
						if (theIndex >= 0 && theIndex < rewards.Count)
						{
							MemberReward reward = rewards[theIndex];
							PropertyInfo pi = reward.GetType().GetProperty(PropertyName.evaluate(contextObject).ToString());
							if (pi != null)
							{
								propertyValue = pi.GetValue(reward, null);
							}
						}
					}
				}
			}
			else
			{
				throw new LWBScriptException("MemberRewardsAttrValue must be incoked within the context of a member.");
			}
            return propertyValue;
        }        
    }
}
