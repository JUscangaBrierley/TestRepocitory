using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Data.ModelAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AuditLogAttribute : Attribute
    {
        public AuditLogAttribute(bool loggingEnabled)
        {
            LoggingEnabled = loggingEnabled;
        }

        public AuditLogAttribute(string typeName, bool loggingEnabled)
        {
            TypeName = typeName;
            LoggingEnabled = loggingEnabled;
        }

        /// <summary>
        /// Used to override the type name for the marked class
        /// </summary>
        public string TypeName { get; set; }
        public bool LoggingEnabled { get; set; }
    }
}
