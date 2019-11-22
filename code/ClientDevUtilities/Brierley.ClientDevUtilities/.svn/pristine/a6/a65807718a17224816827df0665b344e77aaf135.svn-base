using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// A template in the content management system.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_Template")]
    [AuditLog(true)]
	public class Template : LWCoreObjectBase
	{
		private const string _xsltPrefix = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt""
    exclude-result-prefixes=""msxsl"">
    <xsl:output method=""xml"" indent=""no"" />
    <xsl:template match=""@* | node()"">
        <xsl:copy>
            <xsl:apply-templates select=""@* | node()"" />
        </xsl:copy>
    </xsl:template>
    <xsl:template match=""/"">
";

		private const string _xsltSuffix = @"
    </xsl:template>
</xsl:stylesheet>
";


		/// <summary>
		/// Gets or sets the ID for the current Template
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current Template
		/// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description for the current Template
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the TemplateType for the current Template
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public TemplateType TemplateType { get; set; }

		/// <summary>
		/// Gets or sets the Version for the current Template
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public int Version { get; set; }

		/// <summary>
		/// Gets or sets the IsLocked for the current Template
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool IsLocked { get; set; }

		/// <summary>
		/// Gets or sets the Fields for the current Template
		/// </summary>
        [PetaPoco.Column]
		public string Fields { get; set; }

		/// <summary>
		/// Gets or sets the HtmlContent for the current Template
		/// </summary>
        [PetaPoco.Column]
		public string HtmlContent { get; set; }

		/// <summary>
		/// Gets or sets the TextContent for the current Template
		/// </summary>
        [PetaPoco.Column]
		public string TextContent { get; set; }

		/// <summary>
		/// Gets or sets the folder id for the current Template
		/// </summary>
        [PetaPoco.Column]
		public long? FolderId { get; set; }


		/// <summary>
		/// Initializes a new instance of the Template class
		/// </summary>
		public Template()
		{
			ID = -1;
			Version = 1;
			TemplateType = TemplateType.Webpage;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other">other template to copy</param>
		public Template(Template other)
		{
			ID = -1;
			Name = other.Name;
			Description = other.Description;
			Version = other.Version;
			Fields = other.Fields;
			TemplateType = other.TemplateType;
			HtmlContent = other.HtmlContent;
			TextContent = other.TextContent;
			IsLocked = false;
		}

		public Template Clone()
		{
			return Clone(new Template());
		}

		public Template Clone(Template other)
		{
			other.Name = Name;
			other.Description = Description;
			other.Version = Version;
			other.Fields = Fields;
			other.TemplateType = TemplateType;
			other.HtmlContent = HtmlContent;
			other.TextContent = TextContent;
			other.IsLocked = false;
			other.FolderId = FolderId;
			return (Template)base.Clone(other);
		}

		public List<TemplateContentArea> GetHtmlContentAreas()
		{
			List<TemplateContentArea> result = new List<TemplateContentArea>();
			if (!string.IsNullOrEmpty(HtmlContent))
			{
				MatchCollection matches = Regex.Matches(HtmlContent, @"<contentarea.*?>.*?</contentarea>");
				foreach (Match match in matches)
				{
					XElement element = XElement.Parse(match.ToString());
					TemplateContentArea tca = new TemplateContentArea();
					tca.AreaType = ContentAreaType.ContentArea;
					tca.Name = element.AttributeValue("name");
					long elementId = -1;
					long.TryParse(element.AttributeValue("contenttype"), out elementId);
					if (elementId > 0)
					{
						tca.StructuredElementId = elementId;
					}
					tca.VisibilityFilter = element.AttributeValue("visibilityfilter");
					tca.XsltVisibilityLeft = element.AttributeValue("xsltvisibilityleft");
					tca.XsltVisibilityOperator = element.AttributeValue("xsltvisibilityoperator");
					tca.XsltVisibilityRight = element.AttributeValue("xsltvisibilityright");
					result.Add(tca);
				}

				matches = Regex.Matches(HtmlContent, @"<dynamicarea.*?>.*?</dynamicarea>");
				foreach (Match match in matches)
				{
					XElement element = XElement.Parse(match.ToString());
					TemplateContentArea tca = new TemplateContentArea();
					tca.AreaType = ContentAreaType.DynamicContentArea;
					tca.Name = element.AttributeValue("name");
					tca.StructuredElementId = -1;
					tca.VisibilityFilter = string.Empty;
					result.Add(tca);
				}
			}
			return result;
		}

		public List<TemplateContentArea> GetHtmlDynamicAreas()
		{
			Regex rDynamicAreas = new Regex(@"\%\%(?<DynamicAreaName>\w+)\%\%");
			MatchCollection mcDynamicAreas = rDynamicAreas.Matches(StringUtils.FriendlyString(HtmlContent));
			List<TemplateContentArea> result = EnumerateContentAreas(mcDynamicAreas, ContentAreaType.DynamicContentArea, "DynamicAreaName");
			return result;
		}

		public List<TemplateContentArea> GetTextContentAreas()
		{
			List<TemplateContentArea> result = new List<TemplateContentArea>();
			if (!string.IsNullOrEmpty(TextContent))
			{
				MatchCollection matches = Regex.Matches(TextContent, @"<contentarea.*?>.*?</contentarea>");
				foreach (Match match in matches)
				{
					XElement element = XElement.Parse(match.ToString());
					TemplateContentArea tca = new TemplateContentArea();
					tca.AreaType = ContentAreaType.ContentArea;
					tca.Name = element.AttributeValue("name");
					long elementId = -1;
					long.TryParse(element.AttributeValue("contenttype"), out elementId);
					if (elementId > 0)
					{
						tca.StructuredElementId = elementId;
					}
					tca.VisibilityFilter = element.AttributeValue("visibilityfilter");
					result.Add(tca);
				}
			}
			return result;
		}

		public string GetHtmlAsXslt()
		{
			string result = _xsltPrefix + ContentAreasToXslt(StringUtils.FriendlyString(HtmlContent)) + _xsltSuffix;
			return result;
		}

		public string GetTextAsXslt()
		{
			string result = _xsltPrefix + ContentAreasToXslt(StringUtils.FriendlyString(TextContent)) + _xsltSuffix;
			return result;
		}


		public override LWObjectAuditLogBase GetArchiveObject()
		{
			Template_AL ar = new Template_AL()
			{
				ObjectId = this.ID,
				Name = this.Name,
				Description = this.Description,
				TemplateType = this.TemplateType,
				Version = this.Version,
				IsLocked = this.IsLocked,
				Fields = this.Fields,
				HtmlContent = this.HtmlContent,
				TextContent = this.TextContent,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}

		protected string ConvertContentAreaEval(Match m)
		{
			string ret = string.Empty;
			XDocument doc = XDocument.Parse(m.ToString());
			switch (doc.Root.Name.LocalName)
			{
				case "contentarea":
				case "dynamicarea":
					ret = string.Format("<xsl:copy-of select=\"ContentAreas/{0}\" />", XAttributeValue(doc.Root.Attribute("name")));
					break;
				case "templatefield":
					ret = string.Format("##{0}##", XAttributeValue(doc.Root.Attribute("name")));
					break;
			}
			return ret;
		}

		private string ContentAreasToXslt(string content)
		{
			MatchEvaluator evaluator = new MatchEvaluator(this.ConvertContentAreaEval);
			content = Regex.Replace(content, @"<contentarea.*?>.*?</contentarea>", evaluator);
			content = Regex.Replace(content, @"<dynamicarea.*?>.*?</dynamicarea>", evaluator);
			content = Regex.Replace(content, @"<templatefield.*?>.*?</templatefield>", evaluator);
			return content;
		}

		private string XAttributeValue(XAttribute attribute)
		{
			if (attribute == null)
			{
				return string.Empty;
			}
			else
			{
				return attribute.Value;
			}
		}

		private List<TemplateContentArea> EnumerateContentAreas(MatchCollection matchCollection, ContentAreaType contentAreaType, string regExToken)
		{
			List<TemplateContentArea> result = new List<TemplateContentArea>();
			foreach (Match match in matchCollection)
			{
				TemplateContentArea templateContentArea = new TemplateContentArea();
				templateContentArea.Name = StringUtils.FriendlyString(match.Groups[regExToken]);
				//templateContentArea.TextPosition = match.Index;
				templateContentArea.AreaType = contentAreaType;
				result.Add(templateContentArea);
			}
			return result;
		}

		private string Html2Xslt()
		{
			string content = StringUtils.FriendlyString(HtmlContent);

			// Content areas
			Regex regex = new Regex(@"!\#(?<AreaName>\w+)\#!");
			MatchCollection matchCollection = regex.Matches(content);
			content = ContentArea2Xslt(content, matchCollection);

			// Dynamic areas
			regex = new Regex(@"\%\%(?<AreaName>\w+)\%\%");
			matchCollection = regex.Matches(content);
			content = ContentArea2Xslt(content, matchCollection);

			return content;
		}

		private string Text2Xslt()
		{
			string content = StringUtils.FriendlyString(TextContent);

			// Text areas
			Regex regex = new Regex(@"!\#(?<AreaName>\w+)\#!");
			MatchCollection matchCollection = regex.Matches(content);
			content = ContentArea2Xslt(content, matchCollection);

			return content;
		}

		private string ContentArea2Xslt(string content, MatchCollection matchCollection)
		{
			foreach (Match match in matchCollection)
			{
				string areaName = StringUtils.FriendlyString(match.Groups["AreaName"]);
				string fullAreaName = string.Format(@"!\#{0}\#!", areaName);
				string xsltTag = string.Format("<xsl:copy-of select=\"ContentAreas/{0}\" />", areaName);
				content.Replace(fullAreaName, xsltTag);
			}
			return content;
		}
	}
}