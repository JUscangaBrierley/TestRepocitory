using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The StoreByIdAttrValue function returns the property value of the store identified by its id
    /// </summary>
    /// <example>
    ///     Usage : StoreByIdAttrValue('id','PropertyName')
    /// </example>
    /// <remarks>
    /// The id must be the identifier of the store.  The property must be a valid property of the store.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the property value of the store identified by its id.",
        DisplayName = "StoreByIdAttrValue", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Objects, 
		
		WizardDescription = "Property value of a store", 
		AdvancedWizard = true
		)]

	[ExpressionParameter(Order = 1, Name = "Store Id", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which Store ID?")]
	[ExpressionParameter(Order = 2, Name = "Property Name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which Property?", Helpers = ParameterHelpers.StoreProperty)]

	public class StoreByIdAttrValue : UnaryOperation
    {
        Expression idExpression = null;        
        Expression propNameExpression = null;        

        /// <summary>
        /// Public Constructor
        /// </summary>
        public StoreByIdAttrValue()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal StoreByIdAttrValue(Expression rhs)
            : base("StoreByIdAttrValue", rhs)
        {
            ParameterList plist = rhs as ParameterList;
            if (plist != null && plist.Expressions.Length > 1)
            {
                if (plist.Expressions.Length > 1)
                {
                    idExpression = plist.Expressions[0];
                    propNameExpression = plist.Expressions[1];
                }                
            }
            else
            {
                throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to StoreByIdAttrValue.");
            }			
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "StoreByIdAttrValue('store id','PropertyName')";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				object propertyValue = null;
				long id = long.Parse(idExpression.evaluate(contextObject).ToString());
				string propName = (string)propNameExpression.evaluate(contextObject);
				StoreDef s = content.GetStoreDef(id);
				if (s != null)
				{
					PropertyInfo pi = s.GetType().GetProperty(propName);
					if (pi != null)
					{
						propertyValue = pi.GetValue(s, null);
					}
					else
					{
						throw new LWBScriptException(string.Format("Property {0} does not exist on store", propName));
					}
				}
				else
				{
					throw new LWBScriptException("No store could be retrieved by id " + id);
				}
				return propertyValue;
			}
		}
    }
}
