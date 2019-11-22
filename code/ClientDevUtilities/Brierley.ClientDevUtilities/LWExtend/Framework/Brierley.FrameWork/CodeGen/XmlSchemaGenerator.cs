//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.IO;

namespace Brierley.FrameWork.CodeGen
{
    public class XmlSchemaGenerator : CodeGenBase
    {
        IList<AttributeSetMetaData> attSets;

        public XmlSchemaGenerator(string organizationName, string path, bool overwrite)
            : base(organizationName,path,overwrite)
        {
            Initialize(organizationName + ".xsd", Encoding.Unicode);
        }
                        
        public void Generate()
        {
            try
            {
                string tns = string.Format("http://{0}.MemberProcessing.Schemas.{0}",OrganizationName);
                string header = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
                Writer.writeLine(header);
                Writer.writeLine("<xs:schema xmlns:b=\"http://schemas.microsoft.com/BizTalk/2003\"");                
                Writer.writeLine("xmlns=\""+tns+"\"");
                Writer.writeLine("targetNamespace=\""+tns+"\"");
                Writer.writeLine("xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">");
                int indent = 3;

                Writer.writeLine(indent, "<xs:simpleType name=\"MemberStatusEnums\">");
                Writer.writeLine(indent + 3, "<xs:restriction base=\"xs:string\">");
                Writer.writeLine(indent + 6, "<xs:enumeration value=\"Active\"/>");
                Writer.writeLine(indent + 6, "<xs:enumeration value=\"Disabled\"/>");
                Writer.writeLine(indent + 6, "<xs:enumeration value=\"Terminated\"/>");
                Writer.writeLine(indent + 6, "<xs:enumeration value=\"Locked\"/>");                
                Writer.writeLine(indent + 3, "</xs:restriction>");
                Writer.writeLine(indent, "</xs:simpleType>");

                Writer.writeLine(indent, "<xs:simpleType name=\"VirtualCardStatusEnums\">");
                Writer.writeLine(indent + 3, "<xs:restriction base=\"xs:string\">");
                Writer.writeLine(indent + 6, "<xs:enumeration value=\"Active\"/>");
                Writer.writeLine(indent + 6, "<xs:enumeration value=\"InActive\"/>");
                Writer.writeLine(indent + 6, "<xs:enumeration value=\"Hold\"/>");
                Writer.writeLine(indent + 6, "<xs:enumeration value=\"Cancelled\"/>");
                Writer.writeLine(indent + 6, "<xs:enumeration value=\"Replaced\"/>");
                Writer.writeLine(indent + 3, "</xs:restriction>");
                Writer.writeLine(indent, "</xs:simpleType>");

                Writer.writeLine(indent,"<xs:element name=\"AttributeSets\">");
                Writer.writeLine(indent+3, "<xs:complexType>");
				Writer.writeLine(indent + 6, "<xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">");
                Writer.writeLine(indent + 9, "<xs:element name=\"Member\" minOccurs=\"0\" maxOccurs=\"unbounded\">");
                Writer.writeLine(indent + 12, "<xs:complexType>");
				Writer.writeLine(indent + 15, "<xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">");
                // now add attribute sets.
				using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					attSets = svc.GetAllTopLevelAttributeSets();
				}
                GenerateVirtualCard(indent + 18);                
                foreach (AttributeSetMetaData attSet in attSets)
                {
                    if (attSet.Type == AttributeSetType.Member)
                    {
                        GenerateAttributeSet(indent + 18, attSet);
                    }                    
                }
                
                Writer.writeLine(indent + 15, "</xs:sequence>");
                // generate attributes for member
                Writer.writeLine(indent + 15, "<xs:attribute name=\"IpCode\" type=\"xs:long\"/>");
				Writer.writeLine(indent + 15, "<xs:attribute name=\"MemberCreateDate\" type=\"xs:dateTime\"/>");
				Writer.writeLine(indent + 15, "<xs:attribute name=\"MemberCloseDate\" type=\"xs:dateTime\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"MemberStatus\" type=\"MemberStatusEnums\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"BirthDate\" type=\"xs:dateTime\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"FirstName\" type=\"xs:string\" />");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"LastName\" type=\"xs:string\" />");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"MiddleName\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"NamePrefix\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"NameSuffix\" type=\"xs:string\"/>");
				Writer.writeLine(indent + 15, "<xs:attribute name=\"PrimaryEmailAddress\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"PrimaryPhoneNumber\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"PrimaryPostalCode\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"AlternateId\" type=\"xs:string\"/>");
                //Writer.writeLine(indent + 15, "<xs:attribute name=\"MobileDeviceType\" type=\"xs:long\"/>");
				Writer.writeLine(indent + 15, "<xs:attribute name=\"Username\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"Password\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"Salt\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"LastActivityDate\" type=\"xs:dateTime\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"IsEmployee\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 15, "<xs:attribute name=\"ChangedBy\" type=\"xs:string\"/>");
                Writer.writeLine(indent + 12, "</xs:complexType>");
                Writer.writeLine(indent + 9, "</xs:element>");
                GenerateGlobalSets(indent + 18);
                Writer.writeLine(indent + 6, "</xs:sequence>");
                Writer.writeLine(indent+3, "</xs:complexType>");
                Writer.writeLine(indent, "</xs:element>");
                Writer.writeLine("</xs:schema>");
                Success = true; 
            }
            finally
            {
                Dispose();
            }            
        }

        protected void GenerateVirtualCard(int indent)
        {
            Writer.writeLine(indent, "<xs:element name=\"VirtualCard\" minOccurs=\"0\" maxOccurs=\"unbounded\">");
            Writer.writeLine(indent + 3, "<xs:complexType>");
			Writer.writeLine(indent + 6, "<xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">");
            foreach (AttributeSetMetaData attSet in attSets)
            {
                if (attSet.Type == AttributeSetType.VirtualCard)
                {
                    GenerateAttributeSet(indent + 9, attSet);
                }
            }                        
            Writer.writeLine(indent + 6, "</xs:sequence>");
			Writer.writeLine(indent + 6, "<xs:attribute name=\"IpCode\" type=\"xs:long\"/>");
			Writer.writeLine(indent + 6, "<xs:attribute name=\"VcKey\" type=\"xs:long\"/>");
            Writer.writeLine(indent + 6, "<xs:attribute name=\"LoyaltyIdNumber\" type=\"xs:string\" use=\"required\"/>");
            Writer.writeLine(indent + 6, "<xs:attribute name=\"DateIssued\" type=\"xs:string\"/>");
            Writer.writeLine(indent + 6, "<xs:attribute name=\"DateRegistered\" type=\"xs:string\"/>");
            Writer.writeLine(indent + 6, "<xs:attribute name=\"Status\" type=\"VirtualCardStatusEnums\" use=\"required\"/>");
            Writer.writeLine(indent + 6, "<xs:attribute name=\"IsPrimary\" type=\"xs:boolean\"/>");
			Writer.writeLine(indent + 6, "<xs:attribute name=\"CardType\" type=\"xs:string\"/>");
            Writer.writeLine(indent + 3, "</xs:complexType>");
            Writer.writeLine(indent, "</xs:element>");
        }

        protected void GenerateGlobalSets(int indent)
        {
            Writer.writeLine(indent, "<xs:element name=\"Global\" minOccurs=\"0\" maxOccurs=\"unbounded\">");
            Writer.writeLine(indent + 3, "<xs:complexType>");
			Writer.writeLine(indent + 6, "<xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">");
            foreach (AttributeSetMetaData attSet in attSets)
            {
                if (attSet.Type == AttributeSetType.Global)
                {
                    GenerateAttributeSet(indent + 9, attSet);
                }
            }            
            Writer.writeLine(indent + 6, "</xs:sequence>");
            Writer.writeLine(indent + 3, "</xs:complexType>");
            Writer.writeLine(indent, "</xs:element>");
        }

        protected static void AddAnnotation(int indent, FileWriter Writer, AttributeSetMetaData atd)
        {
            string itemFormat = "<xs:appinfo>{0}={1}</xs:appinfo>";
            Writer.writeLine(indent, "<xs:annotation>");
            HashSet<string> propertiesToSet = new HashSet<string>() { "Alias", "Description", "DisplayText" };
            foreach (var property in atd.GetType().GetProperties())
            {
                if (propertiesToSet.Contains(property.Name))
                    Writer.writeLine(indent + 3, string.Format(itemFormat, property.Name, property.GetValue(atd)));
            }
            Writer.writeLine(indent, "</xs:annotation>");
        }

        protected static void AddAnnotation(int indent, FileWriter Writer, AttributeMetaData atd)
        {
            string itemFormat = "<xs:appinfo>{0}={1}</xs:appinfo>";
            Writer.writeLine(indent, "<xs:annotation>");
            HashSet<string> propertiesToSkip = new HashSet<string>() { "CreateDate", "UpdateDate", "ID", "AttributeSetCode", "Status" };
            foreach (var property in atd.GetType().GetProperties())
            {
                if (propertiesToSkip.Contains(property.Name))
                    continue;

                Writer.writeLine(indent + 3, string.Format(itemFormat, property.Name, property.GetValue(atd)));
            }
            Writer.writeLine(indent, "</xs:annotation>");
        }

        protected void GenerateAttribute(int indent, AttributeMetaData at)
        {
            DataType dt = (DataType)Enum.Parse(typeof(DataType), at.DataType);
            string xmlType = "xs:string";
            if ( dt == DataType.Number )
            {
                xmlType = "xs:long";
            }
            else if (dt == DataType.Integer)
            {
                xmlType = "xs:int";
            }
            else if (dt == DataType.String)
            {
                xmlType = "xs:string";
            }
            else if (dt == DataType.Date)
            {
                xmlType = "xs:dateTime";
            }
            else if (dt == DataType.Decimal)
            {
                xmlType = "xs:decimal";
            }            
            else if (dt == DataType.Money)
            {
                xmlType = "xs:decimal";
            }
            else if (dt == DataType.Boolean)
            {
                xmlType = "xs:boolean";
            }
            if (at.IsRequired)
            {
                Writer.writeLine(indent, "<xs:attribute name=\"" + at.Name + "\" type=\"" + xmlType + "\" use=\"required\">");
            }
            else
            {
                Writer.writeLine(indent, "<xs:attribute name=\"" + at.Name + "\" type=\"" + xmlType + "\">");
            }
            AddAnnotation(indent + 3, Writer, at);
            Writer.writeLine(indent, "</xs:attribute>");
        }

        protected void GenerateAttributeSet(int indent, AttributeSetMetaData ad)
        {
            Writer.writeLine(indent, "<xs:element minOccurs=\"0\" maxOccurs=\"unbounded\" name=\"" + ad.Name + "\">");
            AddAnnotation(indent + 3, Writer, ad);
            Writer.writeLine(indent + 3, "<xs:complexType>");
			Writer.writeLine(indent + 6, "<xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">");
            foreach (AttributeSetMetaData asd in ad.ChildAttributeSets)
            {
                GenerateAttributeSet(indent + 9, asd);
            }            
            Writer.writeLine(indent + 6, "</xs:sequence>");
            foreach (AttributeMetaData atd in ad.Attributes)
            {
                GenerateAttribute(indent + 6, atd);
            }
            Writer.writeLine(indent + 3, "</xs:complexType>");
            Writer.writeLine(indent, "</xs:element>");
        }
    }
}
