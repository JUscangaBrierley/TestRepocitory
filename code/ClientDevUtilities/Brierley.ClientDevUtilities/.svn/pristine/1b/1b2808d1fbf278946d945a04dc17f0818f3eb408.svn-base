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
    /// The ProductByPartNumberAttrValue function returns the property value of the product identified by its part number
    /// </summary>
    /// <example>
    ///     Usage : ProductByPartNumberAttrValue('part number','PropertyName', ['language'], ['channel'])
    /// </example>
    /// <remarks>
    /// The property must be a valid property of the product.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the property value of the product identified by its part number.",
        DisplayName = "ProductByPartNumberAttrValue", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Objects, 
		
		WizardDescription = "Property value of a product (by Part #)",
		AdvancedWizard = true
		)]

	[ExpressionParameter(Order = 1, Name = "Part Number", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which Part Number?")]
	[ExpressionParameter(Order = 2, Name = "Property Name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which Property?", Helpers = ParameterHelpers.ProductProperty)]
	[ExpressionParameter(Order = 3, Name = "Language", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which Language?", Helpers = ParameterHelpers.Language)]
	[ExpressionParameter(Order = 4, Name = "Channel", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which Channel?", Helpers = ParameterHelpers.Channel)]

	public class ProductByPartNumberAttrValue : UnaryOperation
    {
        Expression partNumberExpression = null;        
        Expression propNameExpression = null;
        Expression langExpression = null;
        Expression chanExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public ProductByPartNumberAttrValue()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal ProductByPartNumberAttrValue(Expression rhs)
            : base("ProductByPartNumberAttrValue", rhs)
        {
			ParameterList plist = rhs as ParameterList;
            if (plist != null && plist.Expressions.Length > 1)
            {
                if (plist.Expressions.Length > 1)
                {
                    partNumberExpression = plist.Expressions[0];
                    propNameExpression = plist.Expressions[1];
                }
                if (plist.Expressions.Length > 2)
                {
                    langExpression = plist.Expressions[2];
                }
                if (plist.Expressions.Length > 3)
                {
                    chanExpression = plist.Expressions[3];
                }
            }
            else
            {
                throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to ProductByPartNumberAttrValue.");
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "ProductByPartNumberAttrValue('part number','PropertyName', ['language'], ['channel'])";
            }
        }

        private string GetDescription(ContextObject contextObject, string propertyName, Product p)
        {
            string opName = "Get" + propertyName;
            string language = string.Empty;
            string channel = string.Empty;
            if (langExpression != null)
            {
                language = (string)langExpression.evaluate(contextObject);                
            }
            if ( string.IsNullOrEmpty(language) )
            {
                language = LanguageChannelUtil.GetDefaultCulture();
            }
            if (chanExpression != null)
            {
                channel = (string)chanExpression.evaluate(contextObject);
            }            
            if ( string.IsNullOrEmpty(channel) )
            {
                channel = LanguageChannelUtil.GetDefaultChannel();
            }
            string[] args = new string[]{language, channel};
            return ClassLoaderUtil.InvokeMethod(p, opName, args) as string;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			using (var service = LWDataServiceUtil.ContentServiceInstance())
			{
				object propertyValue = null;
				string partNmbr = (string)partNumberExpression.evaluate(contextObject);
				string propName = (string)propNameExpression.evaluate(contextObject);
				Product p = service.GetProductByPartNumber(partNmbr);
				if (p != null)
				{
					if (propName == "Description" || propName == "LongDescription")
					{
						propertyValue = GetDescription(contextObject, propName, p);
					}
					else
					{
						PropertyInfo pi = p.GetType().GetProperty(propName);
						if (pi != null)
						{
							propertyValue = pi.GetValue(p, null);
						}
						else
						{
							throw new LWBScriptException(string.Format("Property {0} does not exist on product", propName));
						}
					}
				}
				else
				{
					throw new LWBScriptException("No product could be retrieved by part number " + partNmbr);
				}
				return propertyValue;
			}
		}
    }
}
