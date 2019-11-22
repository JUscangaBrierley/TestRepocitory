//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AuditLogConfig")]
	public class AuditLogConfig : LWCoreObjectBase
	{
		[PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		[PetaPoco.Column(Length=255, IsNullable=false)]
        [UniqueIndex(RequiresLowerFunction = false)]
		public string TypeName { get; set; } // either domain object name or attribute set name

		[PetaPoco.Column(IsNullable = false)]
		public bool IsCoreObject { get; set; } // if false then it is an attribute set

        [PetaPoco.Column(IsNullable = false)]
		public bool LoggingEnabled { get; set; }

        public static Dictionary<string, AuditLogConfig> GetDefaultLogging()
        {
            Dictionary<string, AuditLogConfig> configs = new Dictionary<string, AuditLogConfig>();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attributes = t.GetCustomAttributes<AuditLogAttribute>(false);
                if (attributes != null && ((AuditLogAttribute[])attributes).Length > 0)
                {
                    var auditLogAttribute = ((AuditLogAttribute[])attributes)[0];
                    string typeName = string.IsNullOrEmpty(auditLogAttribute.TypeName) ? t.Name : auditLogAttribute.TypeName;
                    configs.Add(typeName, new AuditLogConfig() { TypeName = typeName, IsCoreObject = true, LoggingEnabled = auditLogAttribute.LoggingEnabled });
                }
            }
            return configs;
        }
	}
}
