using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    public class TemplateContentArea
    {
        /// <summary>
        /// Initializes a new instance of the TemplateContentArea class
        /// </summary>
        public TemplateContentArea()
        {
        }

		public String Name { get; set; }


		public ContentAreaType AreaType { get; set; }

		public long? StructuredElementId { get; set; }

		public string VisibilityFilter { get; set; }

		public string XsltVisibilityLeft { get; set; }
		public string XsltVisibilityOperator { get; set; }
		public string XsltVisibilityRight { get; set; }

		public TemplateContentArea Clone()
        {
            TemplateContentArea ca = new TemplateContentArea();
            ca.Name = this.Name;
            ca.AreaType = this.AreaType;
            return ca;
        }



		/*
        public virtual bool IsWithinContent(string content)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(content))
            {
                switch (_areaType)
                {
                    case ContentAreaType.ContentArea:
                        if (content.Contains(string.Format("!#{0}#!", _name))) result = true;
                        break;
                    case ContentAreaType.DynamicContentArea:
                        if (content.Contains(string.Format("%%{0}%%", _name))) result = true;
                        break;
                    case ContentAreaType.TextArea:
                        if (content.Contains(string.Format("!#{0}#!", _name))) result = true;
                        break;
                    case ContentAreaType.Field:
                        if (content.Contains(string.Format("##{0}##", _name))) result = true;
                        break;
                }
            }
            return result;
        }
		*/

    }
}
