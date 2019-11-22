using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;


namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// The attribute class encapsulates the row.Parent.attribute syntax in the expression engine. row.Parent.attribute is neither
    /// an operator or a function although it does inherit from expression. It represent a special case in the syntax
    /// parsing logic to handle the ability to address attributes of  the parent in a row of data. In the future this could be re-worked
    /// into a function of the form Parent() where the function would return the value of the named attribute.
    /// </summary>
    [Serializable]
    public class ParentAttribute : Expression
    {
        private const string _className = "Attribute";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        internal string _name = string.Empty;

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="attribute"></param>
        internal ParentAttribute(string attribute)
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
            object value = null;
            if (contextObject.InvokingRow != null)
            {
                if (contextObject.InvokingRow.Parent == null)
                {
                    throw new LWBScriptException("The invoking row must have a parent");
                }
                else
                {
                    object parent = contextObject.InvokingRow.Parent;
                    if (parent.GetType() == typeof(Member))
                    {
                        value = GetMemberProperty((Member)parent, Name);
                    }
                    else if (parent.GetType() == typeof(VirtualCard))
                    {
                    }
                    else
                    {
                        IClientDataObject parentRow = (IClientDataObject) parent;                        
                        try
                        {
                            value = parentRow.GetAttributeValue(Name);
                        }
                        catch (LWMetaDataException)
                        {
                            if (parentRow.HasTransientProperty(Name))
                            {
                                value = parentRow.GetTransientProperty(Name);
                            }
                            else
                            {
                                string msg = string.Format("Unable to find property {0} in parent {1}.", Name, parentRow.GetMetaData().Name);
                                throw new LWMetaDataException(msg);
                            }
                        }
                    }
                }                
            }
            else
            {
                throw new LWBScriptException("Attribute must be evaluated with an invoking row.");
            }
            return value;
        }

        public object GetMemberProperty(Member member,string propertyName)
        {
            PropertyInfo propiinfo = null;
            Type t = typeof(Member);
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in properties)
            {
                if (pi.Name.ToLower() == _name.ToLower())
                {
                    propiinfo = pi;
                    break;
                }
            }
            if (propiinfo == null)
            {
                throw new LWBScriptException("Invalid Property name. Could not locate property " + Name + " on Member class");
            }

            object value = propiinfo.GetValue(member, null);
            if (value != null && value.GetType() == typeof(DateTime))
            {
                return (DateTime)value;
            }
            return value;
        }        
    }
}
