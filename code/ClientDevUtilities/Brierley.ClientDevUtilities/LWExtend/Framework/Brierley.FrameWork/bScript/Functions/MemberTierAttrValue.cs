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
	/// The MemberTierAttrValue function returns the property value of the tier at given by RowIndex
    /// </summary>
    /// <example>
	///     Usage : MemberTierAttrValue('PropertyName','RowIndex')
    /// </example>
    /// <remarks>
	/// PropertyName must be a property of the member tier.
	/// RowIndex must be the index of the tier.  An index of 0 will return property valu of the current tier.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the property value of the tier at given by RowIndex.",
		DisplayName = "MemberTierAttrValue", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Objects,
		EvalRequiresMember = true)]
	public class MemberTierAttrValue : UnaryOperation
    {
        //private static string className = "MemberTierAttrValue";

        private Expression PropertyName = null;
        private Expression RowIndex = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public MemberTierAttrValue()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal MemberTierAttrValue(Expression rhs)
			: base("MemberTierAttrValue", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 2)
            {
                PropertyName = ((ParameterList)rhs).Expressions[0];
                RowIndex = ((ParameterList)rhs).Expressions[1];
                return;
            }
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to MemberTierAttrValue.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
				return "MemberTierAttrValue('PropertyName','RowIndex')";
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
					IList<MemberTier> tiers = service.GetMemberTiers(member);
					if (tiers != null && tiers.Count > 0)
					{
						int theIndex = System.Int32.Parse(this.RowIndex.evaluate(contextObject).ToString());
						if (theIndex >= 0 && theIndex < tiers.Count)
						{
							MemberTier tier = tiers[theIndex];
							PropertyInfo pi = tier.GetType().GetProperty(PropertyName.evaluate(contextObject).ToString());
							if (pi != null)
							{
								propertyValue = pi.GetValue(tier, null);
							}
						}
					}
					return propertyValue;
				}
			}
			else
			{
				throw new LWBScriptException("MemberTierAttrValue must be invoked within the context of a member.");
			}			
        }        
    }
}
