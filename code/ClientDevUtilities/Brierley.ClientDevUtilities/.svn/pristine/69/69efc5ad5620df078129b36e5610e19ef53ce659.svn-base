using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// A document in the content management system.
    /// </summary>
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_Document")]
    [AuditLog(true)]
    public class Document : LWCoreObjectBase
    {
        private const string _contentAreaMarkup = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ContentAreas version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"">
</ContentAreas>";

		private List<string> _postProcessors = null;
		private string _htmlContentAreas = null;
		private string _textContentAreas = null;


        /// <summary>
        /// Gets or sets the ID for the current Document
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

        /// <summary>
        /// Gets or sets the TemplateID for the current Document
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Template), "ID")]
        public long TemplateID { get; set; } //marked as nullable in the database

        /// <summary>
        /// Gets or sets the Name for the current Document
        /// </summary>
        [PetaPoco.Column(Length = 50)]
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the DocumentType for the current Document
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DocumentType DocumentType { get; set; }

        /// <summary>
        /// Gets or sets the Version for the current Document
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public int Version { get; set; }

        /// <summary>
        /// Gets or sets the IsLocked for the current Document
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the Properties for the current Document
        /// </summary>
        [PetaPoco.Column]
		public string Properties { get; set; }

        /// <summary>
        /// Gets or sets the HtmlContentAreas for the current Document
        /// </summary>
        [PetaPoco.Column]
		public string HtmlContentAreas 
		{
			get
			{
				return _htmlContentAreas;
			}
			set
			{
				_htmlContentAreas = value;
			}
		}

        /// <summary>
        /// Gets or sets the TextContentAreas for the current Document
        /// </summary>
        [PetaPoco.Column]
		public string TextContentAreas
		{
			get
			{
				return _textContentAreas;
			}
			set
			{
				_textContentAreas = value;
			}
		}


		public List<string> PostProcessors
		{
			get
			{
				return _postProcessors;
			}
			set
			{
				_postProcessors = value;
			}
		}


		/// <summary>
		/// Gets or sets the PostProcessingConfig for the current email
		/// </summary>
		[PetaPoco.Column]
		protected virtual string PostProcessingConfig
		{
			get
			{
				if (_postProcessors != null && _postProcessors.Count > 0)
				{
					lock (_postProcessors)
					{
						return Newtonsoft.Json.JsonConvert.SerializeObject(_postProcessors);
					}
				}
				return null;
			}
			set
			{
				_postProcessors = null;
				//deserialize
				if (!string.IsNullOrEmpty(value))
				{
					_postProcessors = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(value);
				}
				if (_postProcessors == null)
				{
					_postProcessors = new List<string>();
				}
			}
		}


        /// <summary>
        /// Initializes a new instance of the Document class
        /// </summary>
        public Document()
        {
			HtmlContentAreas = TextContentAreas = _contentAreaMarkup;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">other document to copy</param>
        public Document(Document other)
        {
            ID = -1;
            TemplateID = other.TemplateID;
            Name = other.Name;
            Version = other.Version;
            DocumentType = other.DocumentType;
            Properties = other.Properties;
            HtmlContentAreas = other.HtmlContentAreas;
            TextContentAreas = other.TextContentAreas;
            IsLocked = false;
        }

		public Document Clone()
		{
			return Clone(new Document());
		}

		public Document Clone(Document other)
		{
            other.TemplateID = TemplateID;
			other.Name = Name;
			other.Version = Version;
			other.DocumentType = DocumentType;
			other.Properties = Properties;
			other.HtmlContentAreas = HtmlContentAreas;
			other.TextContentAreas = TextContentAreas;
			other.IsLocked = false;
			return (Document)base.Clone(other);
		}

        public XElement GetHtmlAreaContent(string areaName)
        {
            return GetContentAreaContent(areaName, HtmlContentAreas);
        }

		public void SetHtmlAreaContent(XElement content)
		{
			if (string.IsNullOrEmpty(HtmlContentAreas))
			{
				HtmlContentAreas = _contentAreaMarkup;
			}
			SetContentAreaContent(content, ref _htmlContentAreas);
		}
		
        public XElement GetTextAreaContent(string areaName)
        {
            return GetContentAreaContent(areaName, TextContentAreas);
        }

		public void SetTextAreaContent(XElement content)
        {
			if (string.IsNullOrEmpty(TextContentAreas))
			{
				TextContentAreas = _contentAreaMarkup;
			}
            SetContentAreaContent(content, ref _textContentAreas);
        }

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			Document_AL ar = new Document_AL()
			{
				ObjectId = this.ID,
				TemplateID = this.TemplateID,
				Name = this.Name,
				DocumentType = this.DocumentType,
				Version = this.Version,
				IsLocked = this.IsLocked,
				Properties = this.Properties,
				HtmlContentAreas = this.HtmlContentAreas,
				TextContentAreas = this.TextContentAreas,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}

        private XElement GetContentAreaContent(string areaName, string contentAreas)
        {
			XElement result = null;
			if (!string.IsNullOrEmpty(contentAreas))
			{
				XDocument doc = XDocument.Parse(contentAreas);
				result = doc.Root.Element(areaName);
			}
            return result;
        }


		private void SetContentAreaContent(XElement content, ref string contentAreas)
		{
			XDocument doc = XDocument.Parse(contentAreas);
			if (doc.Descendants(content.Name) != null)
			{
				doc.Descendants(content.Name).Remove();
			}
			doc.Root.Add(content);
			contentAreas = doc.ToString();
		}
    }
}