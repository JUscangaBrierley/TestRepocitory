using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Rules
{
    /// <summary>
    /// 
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class EditorAttribute:System.Attribute
    {
        private Type _type = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public EditorAttribute(Type type)
            : base()
        {
            _type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TypeName
        {
            get
            {
                return _type.AssemblyQualifiedName;
            }
        }

    }
}
