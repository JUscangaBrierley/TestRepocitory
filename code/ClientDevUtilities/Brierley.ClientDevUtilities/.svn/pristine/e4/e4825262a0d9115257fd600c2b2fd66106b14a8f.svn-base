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
	/// The GetTierProperty function returns the property value of a specified tier
	/// </summary>
	/// <example>
	///     Usage : GetTierProperty('TierName','PropertyName')
	/// </example>
	/// <remarks>
	///</remarks>
	[Serializable]
    [ExpressionContext(Description = "Returns the property value of a specified tier",
        DisplayName = "GetTierProperty",
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.All,
        ExpressionReturns = ExpressionApplications.Objects,
        WizardDescription = "Tier Property",
        AdvancedWizard = false,
        WizardCategory = WizardCategories.Tier,
        EvalRequiresMember = false
    )]

    [ExpressionParameter(Name = "TierName", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which tier?", Helpers = ParameterHelpers.Tier)]
    [ExpressionParameter(Name = "Property", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which property?", Helpers = ParameterHelpers.TierProperty)]
    public class GetTierProperty : UnaryOperation
    {
		public GetTierProperty()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetTierProperty(Expression rhs)
			: base("GetTierProperty", rhs)
		{
            if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length == 2)
                return;
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetTierProperty");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetTierProperty('TierName','PropertyName')";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            ParameterList plist = GetRight() as ParameterList;
            string tierName = plist.Expressions[0].evaluate(contextObject).ToString();
            string propertyName = plist.Expressions[1].evaluate(contextObject).ToString();
            object propertyValue = null;
            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                TierDef tier = service.GetTierDef(tierName);
                if (tier == null)
                    throw new LWBScriptException("Invalid tier name given to GetTierProperty: " + tierName);

                //get from tier def
                PropertyInfo pi = typeof(TierDef).GetProperty(propertyName);
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
            return propertyValue;
        }
    }
}
