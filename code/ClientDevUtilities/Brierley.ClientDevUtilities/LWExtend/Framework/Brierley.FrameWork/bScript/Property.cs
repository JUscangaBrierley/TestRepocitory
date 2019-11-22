using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// Property place holder in the expression. Of the form Member.PropertyName
    /// </summary>
    [Serializable]
    public class Property : Expression
    {
        private const string _className = "Property";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private PropertyInfo propiinfo;
        internal string _name = string.Empty;

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="Name">The name of the <see cref="Brierley.Framework.Member"/> property</param>
        internal Property(string Name)
            : base()
        {
            _name = Name;
        }


        /// <summary>
        /// returns the name of the property.
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

            Member member = ResolveMember(contextObject.Owner);
            if (member == null)
            {
                throw new CRMException("Loyalty member cannot be null in this context.");
            }

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
            if (propiinfo != null)
            {
                msg = string.Format("Evaluating property {0} of member.", propiinfo.Name);
                _logger.Debug(_className, methodName, msg);

                object value = propiinfo.GetValue(member, null);

                if (value != null && value.GetType() == typeof(DateTime))
                {
                    return (DateTime)value;
                }                
                return value;
            }
			else // this is not a property.  This could be a transient property
			{                     
                if (member.HasTransientProperty(_name))
                {
                    return member.GetTransientProperty(_name);
                }
                else
                {
                    throw new LWBScriptException("Invalid Property name. Could not locate property " + Name + " on Member class");
                }
			}			                                    
        }

		public override string parseMetaData()
		{
			return "member." + Name;
		}
	}
}
