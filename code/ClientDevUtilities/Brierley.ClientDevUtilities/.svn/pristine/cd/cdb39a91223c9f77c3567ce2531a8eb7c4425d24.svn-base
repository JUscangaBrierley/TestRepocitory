using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The GetCurrentTierProperty function returns the property value of the member's current tier
	/// </summary>
	/// <example>
	///     Usage : GetCurrentTierProperty('PropertyName')
	/// </example>
	/// <remarks>
	///</remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns the property value of the member's current tier",
		DisplayName = "GetCurrentTierProperty",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Objects,
		WizardDescription = "Current Tier Property",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Tier,
		EvalRequiresMember = true
	)]

	[ExpressionParameter(Name = "Property", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which property?", Helpers = ParameterHelpers.MemberTierProperty)]
	public class GetCurrentTierProperty : UnaryOperation
	{
		/// <summary>
		/// Public Constructor
		/// </summary>
		public GetCurrentTierProperty()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetCurrentTierProperty(Expression rhs)
			: base("GetCurrentTierProperty", rhs)
		{
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "GetCurrentTierProperty('PropertyName')";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			string propertyName = GetRight().evaluate(contextObject).ToString();
			object propertyValue = null;
			Member member = ResolveMember(contextObject.Owner);
			if (member != null)
			{
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{

					MemberTier mtier = service.GetMemberTier(member, DateTime.Now);
					if (mtier != null)
					{
						PropertyInfo pi = mtier.GetType().GetProperty(propertyName);
						if (pi != null)
						{
							propertyValue = pi.GetValue(mtier, null);
						}
						else
						{
                            TierDef tier = service.GetTierDef(mtier.TierDefId);
                            //get from tier def
                            pi = typeof(TierDef).GetProperty(propertyName);
							if (pi != null)
							{
								propertyValue = pi.GetValue(tier, null);
							}
                            else // Check attributes
                            {
                                using (ContentService contentService = LWDataServiceUtil.ContentServiceInstance())
                                {
                                    var attribute = contentService.GetContentAttributeDef(propertyName);
                                    if (attribute != null)
                                    {
                                        propertyValue = (from a in tier.Attributes where a.ContentAttributeDefId == attribute.ID select a.Value).FirstOrDefault();
                                        if (propertyValue == null && !string.IsNullOrEmpty(attribute.DefaultValues))
                                            propertyValue = attribute.DefaultValues;
                                        if (propertyValue != null && !string.IsNullOrEmpty(propertyValue.ToString()) && attribute.DataType != Common.ContentAttributeDataType.String)
                                        {
                                            propertyValue = DateTime.Parse(propertyValue.ToString());
                                        }
                                    }
                                    else
                                        throw new LWBScriptException("Invalid property name given to GetTierProperty: " + propertyName);
                                }
                            }
						}
					}
				}
			}
			else
			{
				throw new LWBScriptException("GetCurrentTierProperty must be invoked within the context of a member.");
			}
			return propertyValue;
		}
	}
}
