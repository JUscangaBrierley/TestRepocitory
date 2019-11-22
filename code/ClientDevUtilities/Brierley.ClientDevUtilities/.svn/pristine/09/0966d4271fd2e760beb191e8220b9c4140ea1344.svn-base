using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace Brierley.FrameWork.Pdf
{
    public class PdfPageHeader
    {
        #region Fields
        //private Document _document = null;
        #endregion

        #region Properties
        public PdfDocument.PdfAlignment HeaderFontAlignment { get; set; }
        public PdfDocument.PdfFontStyle HeaderFontStyle { get; set; }
        public PdfDocument.PdfFontFamily HeaderFontFamily { get; set; }
        public PdfDocument.PdfColor HeaderColor { get; set; }
        public int HeaderFontSize { get; set; }
        public IList<string> HeaderContent { get; set; }
        public Document Document { get; set; }
        #endregion

        #region Constructor        
        #endregion

        public void WriteHeader()
        {
            if (Document != null && Document.IsOpen())
            {
                Paragraph p = new Paragraph() { Alignment = PdfDocument.GetAlignment(HeaderFontAlignment) };
                Font font = PdfDocument.GetFont(HeaderFontFamily, HeaderFontStyle, HeaderFontSize);
                foreach (string line in HeaderContent)
                {
                    Chunk chunk = new Chunk(line, font);
                    p.Add(chunk);
                    p.Add(Chunk.NEWLINE);
                }
                Document.Add(p);
            }
        }
    }
}
