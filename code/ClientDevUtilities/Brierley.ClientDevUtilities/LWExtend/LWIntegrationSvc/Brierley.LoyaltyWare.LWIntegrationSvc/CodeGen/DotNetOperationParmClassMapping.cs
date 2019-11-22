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
    public class DotNetOperationParmClassMapping
    {
        private string ns;
        private string name;
        private IList<LWIntegrationDirectives.OperationParmType> parms = null;

        public DotNetOperationParmClassMapping(string ns, string name, IList<LWIntegrationDirectives.OperationParmType> parms)
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

        //public virtual string IsParameterRequired(LWIntegrationDirectives.OperationParmType parm)
        //{
        //    return parm.IsRequired.ToString().ToLower();
        //}

        public virtual string GetMetaDataAttribute(LWIntegrationDirectives.OperationParmType parm)
        {
            StringBuilder sb = new StringBuilder("LWMeta(");
            sb.Append(parm.IsRequired.ToString().ToLower());
            if (parm.Type == LWIntegrationDirectives.ParmDataTypeEnums.String )
            {
                sb.Append(",");
                sb.Append(parm.StringLength.ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }

        public virtual string GetParameterType(LWIntegrationDirectives.OperationParmType parm)
        {
            string dtstr = string.Empty;            
            switch (parm.Type)
            {
                case LWIntegrationDirectives.ParmDataTypeEnums.String:
                    dtstr = !parm.IsArray ? "string" : "string[]";
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Integer:
                    dtstr = !parm.IsArray ? "int" : "int[]";                    
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Long:
                //case LWIntegrationDirectives.ParmDataTypeEnums.Key:
                    dtstr = !parm.IsArray ? "long" : "long[]";                    
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Decimal:
                    dtstr = !parm.IsArray ? "decimal" : "decimal[]";                    
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Date:
                    dtstr = !parm.IsArray ? "DateTime" : "DateTime[]";                    
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Boolean:
                    dtstr = !parm.IsArray ? "bool" : "bool[]";
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Member:
                    dtstr = !parm.IsArray ? "Member" : "Member[]";
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet:
                    dtstr = !parm.IsArray ? "LWAttributeSetContainer" : "LWAttributeSetContainer[]";
                    break;
                case LWIntegrationDirectives.ParmDataTypeEnums.Struct:
                    dtstr = !parm.IsArray ? parm.Name + "Struct" : parm.Name + "Struct" + "[]";                    
                    break;
            }
            if (parm.Type != LWIntegrationDirectives.ParmDataTypeEnums.String && 
                parm.Type != LWIntegrationDirectives.ParmDataTypeEnums.Struct && 
                parm.Type != LWIntegrationDirectives.ParmDataTypeEnums.AttributeSet &&
                parm.Type != LWIntegrationDirectives.ParmDataTypeEnums.Member &&
                !parm.IsRequired && !parm.IsArray)
            {
                dtstr += "?";
            }
            return dtstr;
        }
        
    }
}
