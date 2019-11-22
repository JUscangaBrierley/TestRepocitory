//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen
{
    public class DotNetClassMapping
    {
        private string ns;
        private string name;
        private AttributeSetMetaData attSet = null;        

        public DotNetClassMapping(string ns,string name,AttributeSetMetaData attSet)
        {
            this.ns = ns;
            this.name = name;
            this.attSet = attSet;            
        }

        public virtual string Namespace
        {
            get { return ns; }
        }

        public virtual string Name
        {
            get { return name; }
        }
               
        public virtual IList<AttributeMetaData> Attributes
        {
            get { return attSet.Attributes; }
        }
        
        public virtual string GetAttributeReturnType(AttributeMetaData att)
        {
            string dtstr = string.Empty;
            DataType dt = (DataType)Enum.Parse(typeof(DataType), att.DataType);
            switch (dt)
            {
                //case DataType.Blob:
                //    dtstr = "byte[]";
                //    break;
                case DataType.Boolean:
                    dtstr = "bool";
                    if (!att.IsRequired)
                    {
                        dtstr += "?";
                    }
                    break;
                case DataType.Date:
                    dtstr = "DateTime";
                    if (!att.IsRequired)
                    {
                        dtstr += "?";
                    }
                    break;
                case DataType.Decimal:                
                case DataType.Money:
                    dtstr = "decimal";
                    if (!att.IsRequired)
                    {
                        dtstr += "?";
                    }
                    break;
                case DataType.Integer:
                    dtstr = "int";
                    if (!att.IsRequired)
                    {
                        dtstr += "?";
                    }
                    break;
                case DataType.Number:
                    dtstr = "long";
                    if (!att.IsRequired)
                    {
                        dtstr += "?";
                    }
                    break;
                //case DataType.Clob:
                case DataType.String:
                //case DataType.Text:
                //case DataType.XML:
                    dtstr = "string";
                    break;
            }            
            return dtstr;
        }
        
        public virtual string GetAttributeFieldName(AttributeMetaData att)
        {
            return att.Name.ToLower();
        }

		public virtual string GetAttributeLength(AttributeMetaData att)
		{
			return att.MaxLength.ToString();
		}

        //public virtual string IsRequired(AttributeMetaData att)
        //{
        //    return att.IsRequired.ToString().ToLower();
        //}
        public virtual string GetMetaDataAttribute(AttributeMetaData att)
        {
            StringBuilder sb = new StringBuilder("LWMeta(");
            sb.Append(att.IsRequired.ToString().ToLower());
            if (att.DataType == "String")
            {
                sb.Append(",");
                sb.Append(att.MaxLength.ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
