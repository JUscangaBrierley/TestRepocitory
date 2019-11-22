using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Brierley.FrameWork.Pdf
{   
    class PdfPageHandler : PdfPageEventHelper
    {
        #region Fields        
        PdfContentByte cb;
        PdfTemplate template;
        BaseFont _bf;        
        #endregion

        #region Properties
        public PdfPageHeader Header { get; set; }
        public bool HasDocumentTitle { get; set; }
        #endregion

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {            
            _bf = BaseFont.CreateFont();
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);            
            base.OnOpenDocument(writer, document);
        }

        public override void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title)
        {
            base.OnChapter(writer, document, paragraphPosition, title);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            if (Header != null)
            {
                if (!HasDocumentTitle || writer.PageNumber > 1)
                {
                    Header.WriteHeader();
                }
            }
            base.OnStartPage(writer, document);
        }
                      
        public override void OnEndPage(PdfWriter writer, Document document)
        {            
            int pageN = writer.PageNumber;
            if (!HasDocumentTitle || pageN > 1)
            {
                if (HasDocumentTitle)
                {
                    pageN -= 1;
                }
                string text = "Page " + pageN + " of ";
                float len = _bf.GetWidthPoint(text, 8);
                cb.BeginText();
                cb.SetFontAndSize(_bf, 8);
                cb.SetTextMatrix(280, 25);
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(template, 280 + len, 25);
                cb.BeginText();
                cb.SetFontAndSize(_bf, 8);
                cb.SetTextMatrix(280, 820);
                cb.EndText();
            }            

        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            int pageN = writer.PageNumber - 1;
            if (HasDocumentTitle)
            {
                pageN -= 1;
            }
            template.BeginText();
            template.SetFontAndSize(_bf, 8);
            //template.ShowText((writer.PageNumber - 1).ToString());
            template.ShowText((pageN).ToString());
            template.EndText();
        }


    }
}
