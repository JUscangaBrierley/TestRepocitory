using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Extensions;

namespace Brierley.FrameWork.Email
{
	public class ContentArea
	{
		public enum ContentAreaTypes
		{
			Html = 1,
			Text = 2
		}

		public static class ClientAttributes
		{
			public const string Value = "title";
			public const string VisibilityFilter = "data-visibilityfilter";
			public const string BlockID = "data-blockid";
			public const string TextBlockName = "data-textblockname";
			public const string TextBlockText = "data-textblocktext";
			public const string SurveyName = "data-surveyname";
			public const string SurveyLanguage = "data-surveylanguage";
			public const string SurveyWidth = "data-surveywidth";
			public const string SurveyHeight = "data-surveyheight";
			public const string NextButtonType = "data-nextbuttontype";
			public const string NextButtonText = "data-nextbuttontext";
			public const string NextButtonCssStyle = "data-nextbuttoncssstyle";
			public const string NextButtonCssClass = "data-nextbuttoncssclass";
			public const string NextButtonOnClick = "data-nextbuttononclick";
			public const string PlaceholderMarkup = "[[[templatemarkup]]]";
			public const string ContentAttributeStart = "[[[";
			public const string ContentAttributeEnd = "]]]";
		}


		public static class AttributeNames
		{
			public const string StructuredNode = "structuredcontent";
			public const string ContentNode = "content";
			public const string MarkupNode = "markup";
			public const string AttributeFiltersNode = "attributefilters";
			public const string AttributeFilterNode = "filter";
			public const string FilterId = "id";
			public const string FilterName = "name";
			public const string ListField = "listfield";
			public const string VisibilityFilter = "visibilityfilter";
			public const string BatchSelectionType = "batchselectiontype";
			public const string BatchSelectionExpression = "batchselectionexpression";
			public const string RowSelectionType = "rowselectiontype";
			public const string RowSelectionExpression = "rowselectionexpression";
			public const string UseAttributeFiltering = "useattributefiltering";
			public const string XsltVisibilityLeft = "xsltvisibilityleft";
			public const string XsltVisibilityOperator = "xsltvisibilityoperator";
			public const string XsltVisibilityRight = "xsltvisibilityright";
			public const string MarkupStartTag = "<" + MarkupNode + ">";
			public const string MarkupEndTag = "</" + MarkupNode + ">";
		}

		private string _markup = string.Empty;
		private XElement _markupElement = null;

		public string VisibilityFilter { get; set; }
		public string XsltVisibilityLeft { get; set; }
		public string XsltVisibilityOperator { get; set; }
		public string XsltVisibilityRight { get; set; }
		public ContentAreaTypes AreaType { get; set; }
		public string AreaName { get; set; }
		public BatchSelectionTypes BatchSelectionType { get; set; }
		public string BatchSelectionExpression { get; set; }
		public RowSelectionTypes RowSelectionType { get; set; }
		public string RowSelectionExpression { get; set; }
		public bool UseAttributeFiltering { get; set; }


		/*
		 * This should be backward compatable. The old filter took id and name of the structured content attribute, and we're changing it 
		 * to use only the name of the element being filtered along with the name of the list field that's used (< 4.5 had field set in the
		 * element's configuration, but we're only using that as a default when no selection is made).
		 * old: public Dictionary<long, string> Filters { get; set; }
		 */
		public Dictionary<string, string> Filters { get; set; }


		public long? StructuredElementId { get; set; }

		public bool HasMarkup
		{
			get
			{
				return MarkupElement != null;
			}
		}

		public string Markup
		{
			get
			{
				return _markupElement.Value;
			}
			set
			{
				XElement e = new XElement(AttributeNames.MarkupNode);
				e.SetValue(value);
				_markupElement = e;
			}
		}

		public XElement MarkupElement
		{
			get
			{
				return _markupElement;
			}
			set
			{
				_markupElement = value;
			}
		}


		public ContentArea()
		{
			AreaType = ContentAreaTypes.Html;
			BatchSelectionType = BatchSelectionTypes.ActiveBatch;
			RowSelectionType = RowSelectionTypes.FirstRow;
			Filters = new Dictionary<string, string>();
		}

		public ContentArea(string area) : this()
		{
			XElement element = XElement.Parse(area);
		}

		public ContentArea(XElement area) : this()
		{
			Load(area);
		}

		private void Load(XElement area)
		{
			if (area == null) return;

			AreaName = area.Name.LocalName;

			VisibilityFilter = area.AttributeValue(AttributeNames.VisibilityFilter);

			XsltVisibilityLeft = area.AttributeValue(AttributeNames.XsltVisibilityLeft);
			XsltVisibilityOperator = area.AttributeValue(AttributeNames.XsltVisibilityOperator);
			XsltVisibilityRight = area.AttributeValue(AttributeNames.XsltVisibilityRight);

			try
			{
				if (area.Attribute(AttributeNames.BatchSelectionType) != null)
				{
					BatchSelectionType = (BatchSelectionTypes)Enum.Parse(typeof(BatchSelectionTypes), area.AttributeValue(ContentArea.AttributeNames.BatchSelectionType));
				}
			}
			catch { }

			BatchSelectionExpression = area.AttributeValue(AttributeNames.BatchSelectionExpression);

			try
			{
				if (area.Attribute(AttributeNames.RowSelectionType) != null)
				{
					RowSelectionType = (RowSelectionTypes)Enum.Parse(typeof(RowSelectionTypes), area.AttributeValue(AttributeNames.RowSelectionType));
				}
			}
			catch { }

			RowSelectionExpression = area.AttributeValue(AttributeNames.RowSelectionExpression);

			try
			{
				if (area.AttributeValue(AttributeNames.UseAttributeFiltering) != null)
				{
					UseAttributeFiltering = bool.Parse(area.AttributeValue(AttributeNames.UseAttributeFiltering));
				}
			}
			catch { }

			if (area.Element(AttributeNames.MarkupNode) != null)
			{
				string markup = string.Empty;
				if (area.Element(AttributeNames.MarkupNode).InnerXml().Contains("<"))
				{
					this.Markup = area.Element(AttributeNames.MarkupNode).InnerXml();
				}
				else
				{
					this.Markup = area.Element(AttributeNames.MarkupNode).Value;
					this.MarkupElement = area.Element(AttributeNames.MarkupNode);
				}

			}

			if (area.Element(AttributeNames.AttributeFiltersNode) != null)
			{
				foreach (XElement e in area.Element("attributefilters").Elements())
				{
					//long attributeId = -1;
					//long.TryParse(e.AttributeValue(AttributeNames.FilterId), out attributeId);
					//if (attributeId > -1)
					//{
						Filters.Add(e.AttributeValue(AttributeNames.FilterName), e.AttributeValue(AttributeNames.ListField));
					//}
				}
			}
		}


		public override string ToString()
		{
			return this.ToXElement().InnerXml();
		}

		public XElement ToXElement()
		{
			XElement e = new XElement(AreaName,
				new XAttribute(AttributeNames.VisibilityFilter, VisibilityFilter),
				new XAttribute(AttributeNames.BatchSelectionType, BatchSelectionType.ToString()),
				new XAttribute(AttributeNames.BatchSelectionExpression, BatchSelectionExpression.ToString()),
				new XAttribute(AttributeNames.RowSelectionType, RowSelectionType.ToString()),
				new XAttribute(AttributeNames.RowSelectionExpression, RowSelectionExpression.ToString()),
				new XAttribute(AttributeNames.UseAttributeFiltering, UseAttributeFiltering.ToString()),
				new XElement(AttributeNames.AttributeFiltersNode),
				MarkupElement
				);

			if(!string.IsNullOrEmpty(XsltVisibilityLeft) && !string.IsNullOrEmpty(XsltVisibilityOperator) && !string.IsNullOrEmpty(XsltVisibilityRight))
			{
				e.Add(
					new XAttribute(AttributeNames.XsltVisibilityLeft, XsltVisibilityLeft),
					new XAttribute(AttributeNames.XsltVisibilityOperator, XsltVisibilityOperator),
					new XAttribute(AttributeNames.XsltVisibilityRight, XsltVisibilityRight)
					);
			}

			foreach (string key in Filters.Keys)
			{
				e.Element(AttributeNames.AttributeFiltersNode).Add
					(
						new XElement(AttributeNames.AttributeFilterNode,
							new XAttribute(AttributeNames.FilterName, key),
							new XAttribute(AttributeNames.ListField, Filters[key])
						)
					);
			}

			return e;

		}


	}
}
