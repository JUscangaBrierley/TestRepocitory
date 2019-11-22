using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;


namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// The attribute class encapsulates the row.attribute syntax in the expression engine. row.attribute is neither
    /// an operator or a function although it does inherit from expression. It represent a special case in the syntax
    /// parsing logic to handle the ability to address attributes in a row of data. In the future this could be re-worked
    /// into a function of the form Row("AttributeName") where the function would return the value of the named attribute.
    /// </summary>
    [Serializable]
    public class Attribute : Expression
    {
        private const string _className = "Attribute";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        internal string _name = string.Empty;

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="attribute"></param>
        internal Attribute(string attribute)
            : base()
        {
            _name = attribute;
        }

        /// <summary>
        /// The Name of the attribute being addressed.
        /// </summary>
        public string Name
        {
            get { return _name; }
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

            if (contextObject.InvokingRow != null)
            {
                msg = string.Format("Evaluating attribute of row {0}.", contextObject.InvokingRow.RowKey.ToString());
                _logger.Debug(_className, methodName, msg);

                IClientDataObject invokingRow = contextObject.InvokingRow;
				object value = null;
				try
				{
					value = invokingRow.GetAttributeValue(Name);					
				}
				catch (LWMetaDataException)
				{
                    if (invokingRow.HasTransientProperty(Name))
					{
						value = invokingRow.GetTransientProperty(Name);
					}
					else
					{
						msg = string.Format("Unable to find property {0}.", Name);
						throw new LWMetaDataException(msg);
					}
				}
                return value;
            }
            else
            {
                throw new CRMException("Attribute must be evaluated with an invoking row.");
            }
        }
    }
}
