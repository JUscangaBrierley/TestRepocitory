//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.LoyaltyWare.LWIntegrationSvc;

using Brierley.FrameWork.Common;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen
{
    public class JavaOperationParmClassMapping
    {
        private string ns;
        private string name;
        private IList<LWIntegrationDirectives.OperationParmType> parms = null;

        public JavaOperationParmClassMapping(string ns, string name, IList<LWIntegrationDirectives.OperationParmType> parms)
        {
            this.ns = ns;
            this.name = name;
            this.parms = parms;            
        }

        public virtual string Namespace
        {
            get { return ns; }
        }

        public virtual string Name
        {
            get { return name; }
        }

        public virtual IList<LWIntegrationDirectives.OperationParmType> Parameters
        {
            get { return parms; }
        }

        public virtual string IsParameterRequired(LWIntegrationDirectives.OperationParmType parm)
        {
            return parm.IsRequired.ToString().ToLower();
        }

        public virtual string GetMetaDataAttribute(LWIntegrationDirectives.OperationParmType parm)
        {
            StringBuilder sb = new StringBuilder("");
            if (parm.IsRequired)
            {
                sb.Append("@LWIsRequired ");
            }
            if (parm.Type == LWIntegrationDirectives.ParmDataTypeEnums.String && parm.StringLength > 0)
            {
                sb.Append("@LWStringLength(");
                sb.Append(parm.StringLength.ToString());
                sb.Append(") ");
            }
            return sb.ToString();
        }

        public virtual string GetParameterType(LWIntegrationDirectives.OperationParmType parm)
        {
            string dtstr = string.Empty;            
            switch (parm.Type)
            {
                case LWIntegrationDirectives.ParmDataTypeEnums.String:
                    dtstr = !parm.IsArray ? "String" : "String[]";
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                    dtstr = !parm.IsArray ? "Integer" : "Integer[]";                    
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                    dtstr = !parm.IsArray ? "Long" : "Long[]";                    
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                    dtstr = !parm.IsArray ? "java.math.BigDecimal" : "java.math.BigDecimal[]";                     
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                    dtstr = !parm.IsArray ? "Date" : "Date[]";                    
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Boolean:
                    dtstr = !parm.IsArray ? "Boolean" : "Boolean[]";
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet:
                    dtstr = !parm.IsArray ? "LWAttributeSetContainer" : "LWAttributeSetContainer[]";
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Member:
                    dtstr = !parm.IsArray ? "Member" : "Member[]";
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Struct:
                    dtstr = !parm.IsArray ? parm.Name + "Struct" : parm.Name + "Struct" + "[]";
                    break;
            }            
            return dtstr;
        }
        
    }
}
