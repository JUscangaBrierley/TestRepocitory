//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.CodeGen.ClientDataModel
{
    public class ClassMapping
    {
        private string ns;
        private string name;
        private string assName;
        private AttributeSetMetaData attSet = null;
        private string publicKeyToken = null;
        private string version = "0.0.0.0";
        private string dtType;

        public ClassMapping(string ns,string name,string assName, string dtType,string publicKeyToken,AttributeSetMetaData attSet)
        {
            this.ns = ns;
            this.name = name;
            this.assName = assName;
            this.publicKeyToken = publicKeyToken;
            this.attSet = attSet;
            this.dtType = dtType;
        }

        public virtual string Namespace
        {
            get { return ns; }
        }

        public virtual string Name
        {
            get { return name; }
        }

        public virtual string ALName
        {
            get { return name + "_AL"; }
        }

        public virtual bool LWIdentifierRequired
        {
            get { return attSet.Type == AttributeSetType.Global; }
        }

        public virtual string DbType
        {
            get { return dtType; }
        }

        public virtual string TableName
        {
            get { return "ats_"+name; }
        }

        public virtual string ALTableName
        {
            get { return "ats_AL_" + name; }
        }

        public virtual string AssemblyName
        {
            get 
            {
                string lassName = string.Format("{0}, Version={1}, Culture=neutral, PublicKeyToken={2}", assName, version, publicKeyToken);
                //string assName = string.Format("{0}", ns);
                return lassName; 
            }
        }

        public virtual IList<AttributeMetaData> Attributes
        {
            get { return attSet.Attributes; }
        }

        

        public virtual bool IndexRequired
        {
            get
            {
                return attSet.Type != AttributeSetType.Global ? true : false;
            }
        }

        public virtual string IndexName
        {
            get
            {
                String indexName = "IDX_" + attSet.Name + "_PARENTROWKEY";
                if (indexName.Length > 30)
                    indexName = "IDX_ATS" + attSet.ID + "_PRK";
                return indexName;
            }
        }

        public virtual bool LinkIndexRequired
        {
            get
            {
                if (attSet.Type == AttributeSetType.Member || attSet.Type == AttributeSetType.VirtualCard)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual string LinkIndexName
        {
            get
            {
                string indexName = string.Empty;
                if (attSet.Type == AttributeSetType.VirtualCard)
                {
                    indexName = "IDX_" + attSet.Name + "_VCKEY";
                    if (indexName.Length > 30)
                        indexName = "IDX_ATS" + attSet.ID + "_VCKEY";
                }
                else
                {
                    indexName = "IDX_" + attSet.Name + "_IPCODE";
                    if (indexName.Length > 30)
                        indexName = "IDX_ATS" + attSet.ID + "_IPCODE";
                }
                return indexName;
            }
        }

		public virtual string GetEncryptionType(AttributeMetaData att)
		{
			string ret = "none";
			if (att.DataType == "String")
			{
				switch(att.EncryptionType)
				{
					case AttributeEncryptionType.None:
						break;
					case AttributeEncryptionType.Encoded:
						ret = "encoded";
						break;
					case AttributeEncryptionType.Symmetric:
						ret ="weak";
						break;
					case AttributeEncryptionType.Asymmetric:
						ret = "strong";
						break;
				}
			}
			return ret;
		}

        public virtual string GetLinkProperty()
        {
            if (attSet.Type == AttributeSetType.VirtualCard)
            {
                return "VcKey";
            }
            else
            {
                return "IpCode";
            }
        }

        public virtual string GetLinkField()
        {
            if (attSet.Type == AttributeSetType.VirtualCard)
            {
                return "vckey";
            }
            else
            {
                return "ipcode";
            }
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

        public virtual string GetActualType(AttributeMetaData att)
        {
            string dtstr = string.Empty;
            DataType dt = (DataType)Enum.Parse(typeof(DataType), att.DataType);
            switch (dt)
            {
                case DataType.Boolean:
                    dtstr = "Boolean";
                    break;
                case DataType.Date:
                    dtstr = "DateTime";
                    break;
                case DataType.Decimal:
                case DataType.Money:
                    dtstr = "decimal";
                    break;
                case DataType.Integer:
                    dtstr = "Int32";
                    break;
                case DataType.Number:
                    dtstr = "Int64";
                    break;
                case DataType.String:
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
            string dtstr = string.Empty;
            DataType dt = (DataType)Enum.Parse(typeof(DataType), att.DataType);
            if (dt == DataType.String && att.EncryptionType != AttributeEncryptionType.None)
            {
                long length = att.MaxLength * 7;
                return length.ToString();
            }
            else
            {
                return att.MaxLength.ToString();
            }
		}


		public bool IsSmsOptIn(AttributeMetaData att)
		{
			return att.Name.Equals("smsoptin", StringComparison.OrdinalIgnoreCase);
		}

		public bool IsMemberDetails()
		{
			return attSet.Name.Equals("MemberDetails", StringComparison.OrdinalIgnoreCase);
		}
	}
}
