using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Sets a new value into an attribute.
    /// </summary>
    /// <example>
    ///     Usage : SetAttrValue('attrName',NewValue)
    /// </example>
    /// <remarks>
    /// The attrName parameter must be the name of an attribute on the row of data invoking the rule.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Sets a new value into an attribute.", 
		DisplayName = "SetAttrValue", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Numbers)]
	public class SetAttrValue : UnaryOperation
    {
        private const string _className = "SetAttrValue";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private Expression _attributeName = null;
        private Expression _newValue = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public SetAttrValue()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        public SetAttrValue(Expression rhs)
            : base("SetAttrValue", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 2)
            {
                _attributeName = ((ParameterList)rhs).Expressions[0];
                _newValue = ((ParameterList)rhs).Expressions[1];
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to Set attribute value.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "SetAttrValue('attrName',NewValue)";
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
            string msg = "";

            string attributeName = (string)_attributeName.evaluate(contextObject);
            string newValue = (string)_newValue.evaluate(contextObject);

            if (contextObject.InvokingRow != null)
            {
                AttributeSetMetaData metadata = contextObject.InvokingRow.GetMetaData();
                AttributeMetaData attmeta = metadata.GetAttribute(attributeName);
                object val = null;
                DataType dt = (DataType)Enum.Parse(typeof(DataType),attmeta.DataType);
                switch (dt)
                {
                    case DataType.Boolean:
                        val = Boolean.Parse(newValue.ToString());
                        break;
                    //case DataType.Clob:
                    case DataType.String:
                    //case DataType.Text:
                    //case DataType.XML:
                        val = newValue.ToString();
                        break;
                    case DataType.Date:
                        val = DateTime.Parse(newValue.ToString());
                        break;
                    case DataType.Decimal:
                    case DataType.Money:
                        val = decimal.Parse(newValue.ToString());
                        break;
                    case DataType.Integer:
                        val = int.Parse(newValue.ToString());
                        break;
                    case DataType.Number:
                        val = long.Parse(newValue.ToString());
                        break;                    
                }
                contextObject.InvokingRow.SetAttributeValue(attributeName, val, attmeta);                
            }
            else
            {
                msg = string.Format("SetAttrValue must be invoked in the context of an invoking row.");
                _logger.Error(_className, methodName, msg);
            }
            return 0;
        }
    }
}
