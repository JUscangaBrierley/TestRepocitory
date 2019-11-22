using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Brierley.FrameWork.Common.Exceptions;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace Brierley.FrameWork.Pdf
{
	public class PdfDocument : IDisposable
	{
		public class PdfTableHeaderCell
		{
			public string Name { get; set; }
			public float Span { get; set; }
			public string Format { get; set; }
			public bool IsImage { get; set; }
		}

		public enum PdfSize { Letter, Legal, LetterLandscape, LegalLandscape }
		public enum PdfFontFamily { Courier, Helvetica, Symbol, TimesRoman }
		public enum PdfFontStyle { Bold, BoldItalic, Italic, Normal, StrikeThru, Underline }
		public enum PdfColor { Black, Blue, Cyan, DarkGrey, Gray, Green, LightGray, Magenta, Orange, Pink, Red, White, Yello }
		public enum PdfAlignment { Left, Center, Right }

		private string _filename = string.Empty;
		private Document _document = null;
		private PdfWriter _writer = null;
		private Font _currentFont = null;
		private PdfPageHandler _pageHandler = null;
		private PdfPageHeader _pageHeader = null;
		private bool _disposed = false;

		public static int GetAlignment(PdfAlignment a)
		{
			switch (a)
			{
				case PdfAlignment.Right:
					return Element.ALIGN_RIGHT;
				case PdfAlignment.Center:
					return Element.ALIGN_CENTER;
				case PdfAlignment.Left:
				default:
					return Element.ALIGN_LEFT;
			}
		}

		public PdfSize Size { get; set; }
		public PdfFontFamily FontFamily { get; set; }
		public PdfFontStyle FontStyle { get; set; }
		public int FontSize { get; set; }
		// line properties
		public float LineWidth { get; set; }
		// title properties
		public PdfFontFamily TableTitleFontFamily { get; set; }
		public PdfColor TableTitleColor { get; set; }
		public int TableTitleFontSize { get; set; }
		// header properties
		public PdfAlignment HeaderFontAlignment { get; set; }
		public PdfFontStyle HeaderFontStyle { get; set; }
		public PdfFontFamily HeaderFontFamily { get; set; }
		public PdfColor HeaderColor { get; set; }
		public int HeaderFontSize { get; set; }


		public static BaseColor GetColor(PdfColor pdfColor)
		{
			switch (pdfColor)
			{
				case PdfColor.Black:
					return BaseColor.BLACK;
				case PdfColor.Blue:
					return BaseColor.BLUE;
				case PdfColor.Cyan:
					return BaseColor.CYAN;
				case PdfColor.DarkGrey:
					return BaseColor.DARK_GRAY;
				case PdfColor.Gray:
					return BaseColor.GRAY;
				case PdfColor.Green:
					return BaseColor.GREEN;
				case PdfColor.LightGray:
					return BaseColor.LIGHT_GRAY;
				case PdfColor.Magenta:
					return BaseColor.MAGENTA;
				case PdfColor.Orange:
					return BaseColor.ORANGE;
				case PdfColor.Pink:
					return BaseColor.PINK;
				case PdfColor.White:
					return BaseColor.WHITE;
				case PdfColor.Yello:
					return BaseColor.YELLOW;
				default:
					return BaseColor.WHITE;
			}
		}

		public static Font GetFont(PdfFontFamily family, PdfFontStyle fstyle, int size)
		{
			Font.FontFamily fontFamily = Font.FontFamily.TIMES_ROMAN;
			int style = Font.NORMAL;
			switch (family)
			{
				case PdfFontFamily.Courier:
					fontFamily = Font.FontFamily.COURIER;
					break;
				case PdfFontFamily.Helvetica:
					fontFamily = Font.FontFamily.HELVETICA;
					break;
				case PdfFontFamily.Symbol:
					fontFamily = Font.FontFamily.SYMBOL;
					break;
				case PdfFontFamily.TimesRoman:
					fontFamily = Font.FontFamily.TIMES_ROMAN;
					break;
			}
			switch (fstyle)
			{
				case PdfFontStyle.Bold:
					style = Font.BOLD;
					break;
				case PdfFontStyle.BoldItalic:
					style = Font.BOLDITALIC;
					break;
				case PdfFontStyle.Italic:
					style = Font.ITALIC;
					break;
				case PdfFontStyle.Normal:
					style = Font.NORMAL;
					break;
				case PdfFontStyle.StrikeThru:
					style = Font.STRIKETHRU;
					break;
				case PdfFontStyle.Underline:
					style = Font.UNDERLINE;
					break;
			}
			return new Font(fontFamily, size, style);
		}

		public Rectangle GetPageSize()
		{
			switch (Size)
			{
				case PdfSize.Letter:
					return PageSize.LETTER;
				case PdfSize.LetterLandscape:
					return PageSize.LETTER_LANDSCAPE;
				case PdfSize.Legal:
					return PageSize.LEGAL;
				case PdfSize.LegalLandscape:
					return PageSize.LEGAL_LANDSCAPE;
				default:
					return PageSize.LETTER;
			}
		}

		public Font GetFont()
		{
			_currentFont = GetFont(FontFamily, FontStyle, FontSize);
			return _currentFont;
		}



		public PdfDocument(string filename)
		{
			_filename = filename;

			// Set defaults
			Size = PdfSize.Letter;
			FontFamily = PdfFontFamily.TimesRoman;
			FontStyle = PdfFontStyle.Normal;
			FontSize = 12;

			LineWidth = 1;

			TableTitleFontFamily = PdfFontFamily.Helvetica;
			TableTitleColor = PdfColor.Black;
			TableTitleFontSize = 12;

			HeaderFontAlignment = PdfAlignment.Center;
			HeaderFontFamily = PdfFontFamily.Helvetica;
			HeaderColor = PdfColor.LightGray;
			HeaderFontSize = 12;
			HeaderFontStyle = PdfFontStyle.Normal;

			//Initialize(FileMode.Create, documentTitle);
		}

		public PdfDocument(string filename, PdfPageHeader pageHeader)
		{
			_filename = filename;

			// Set defaults
			Size = PdfSize.Letter;
			FontFamily = PdfFontFamily.TimesRoman;
			FontStyle = PdfFontStyle.Normal;
			FontSize = 12;

			LineWidth = 1;

			TableTitleFontFamily = PdfFontFamily.Helvetica;
			TableTitleColor = PdfColor.Black;
			TableTitleFontSize = 12;

			HeaderFontAlignment = PdfAlignment.Center;
			HeaderFontFamily = PdfFontFamily.Helvetica;
			HeaderColor = PdfColor.LightGray;
			HeaderFontSize = 12;
			HeaderFontStyle = PdfFontStyle.Normal;

			_pageHeader = pageHeader;
			//Initialize(FileMode.Create, documentTitle);
		}

		public void Initialize(FileMode createMode, IList<string> documentTitle)
		{
			bool hasDocTitle = documentTitle != null && documentTitle.Count > 0 ? true : false;
			_document = new Document(GetPageSize());
			_writer = PdfWriter.GetInstance(_document, new FileStream(_filename, createMode));
			_pageHandler = new PdfPageHandler() { HasDocumentTitle = hasDocTitle };
			_writer.PageEvent = _pageHandler;
			if (_pageHeader != null)
			{
				_pageHeader.HeaderColor = HeaderColor;
				_pageHeader.HeaderFontAlignment = HeaderFontAlignment;
				_pageHeader.HeaderFontFamily = HeaderFontFamily;
				_pageHeader.HeaderFontSize = HeaderFontSize;
				_pageHeader.HeaderFontStyle = HeaderFontStyle;
				_pageHeader.Document = _document;
				_pageHandler.Header = _pageHeader;
			}
			_document.Open();

			if (hasDocTitle)
			{
				AddDocumentTitlePage(documentTitle);
			}
		}

		public void Initialize()
		{
			Initialize(FileMode.Create, null);
		}

		public void Initialize(IList<string> documentTitle)
		{
			Initialize(FileMode.Create, documentTitle);
		}

		public void SetPageHeader(PdfPageHeader pageHeader)
		{
			_pageHeader = pageHeader;
			_pageHeader.HeaderColor = HeaderColor;
			_pageHeader.HeaderFontAlignment = HeaderFontAlignment;
			_pageHeader.HeaderFontFamily = HeaderFontFamily;
			_pageHeader.HeaderFontSize = HeaderFontSize;
			_pageHeader.HeaderFontStyle = HeaderFontStyle;
			_pageHeader.Document = _document;
			_pageHandler.Header = _pageHeader;
		}

		public void WriteTable(string title, IList<PdfTableHeaderCell> headerCells, IList<IList<object>> tableData)
		{
			Validate();
			var relativeWidhts = from cell in headerCells select cell.Span;
			PdfPTable table = new PdfPTable(relativeWidhts.ToArray<float>());

			// Add the table title
			if (!string.IsNullOrEmpty(title))
			{
				PdfPCell cell = new PdfPCell(new Phrase(title, GetFont(TableTitleFontFamily, PdfFontStyle.Bold, TableTitleFontSize)));
				cell.Colspan = headerCells.Count;
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				cell.BackgroundColor = GetColor(TableTitleColor);
				cell.Rowspan = 2;
				table.AddCell(cell);
			}

			// add the header
			table.DefaultCell.BackgroundColor = GetColor(HeaderColor);
			table.DefaultCell.Rowspan = 2;
			foreach (PdfTableHeaderCell cell in headerCells)
			{
				table.AddCell(new Phrase(cell.Name, GetFont(HeaderFontFamily, PdfFontStyle.Normal, HeaderFontSize)));
			}

			table.DefaultCell.BackgroundColor = BaseColor.WHITE;
			table.DefaultCell.Rowspan = 1;

			foreach (var row in tableData)
			{
				for (int i = 0; i < headerCells.Count; i++)
				{
					PdfPCell cell = new PdfPCell();
					string formattedValue = string.Empty;
					if (headerCells[i].IsImage)
					{
						Image pic = null;
						if (row[i] is Image)
						{
							pic = row[i] as Image;
						}
						else if (row[i] is byte[])
						{
							pic = Image.GetInstance(row[i] as byte[]);
						}
						cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
						cell.AddElement(pic);
					}
					else if (!string.IsNullOrEmpty(headerCells[i].Format))
					{
						formattedValue = string.Format(headerCells[i].Format, row[i]);
						cell.Phrase = new Phrase(formattedValue, GetFont());
					}
					else
					{
						formattedValue = (row[i] ?? string.Empty).ToString();
						cell.Phrase = new Phrase(formattedValue, GetFont());
					}
					table.AddCell(cell);
				}
			}

			_document.Add(table);
		}

		public void NewPage()
		{
			Validate();
			_document.NewPage();
		}

		public void NewLine()
		{
			Validate();
			_document.Add(Chunk.NEWLINE);
		}

		public void NewLine(int numLines)
		{
			Validate();
			for (int i = 0; i < numLines; i++)
			{
				_document.Add(Chunk.NEWLINE);
			}
		}

		public void LineSeparator()
		{
			Validate();
			iTextSharp.text.pdf.draw.LineSeparator line = new LineSeparator(LineWidth, 100, null, Element.ALIGN_CENTER, -2);
			_document.Add(line);
		}

		public void Write(string text)
		{
			Validate();
			Chunk chunk = new Chunk(text);
			chunk.Font = GetFont();
			_document.Add(new Phrase(chunk));
		}

		public void WriteLine(string text)
		{
			Validate();
			Chunk chunk = new Chunk(text);
			chunk.Font = GetFont();
			_document.Add(new Phrase(chunk));
			_document.Add(Chunk.NEWLINE);
		}

		public void WriteLine(PdfAlignment alignment, string text)
		{
			Validate();
			Chunk chunk = new Chunk(text);
			chunk.Font = GetFont();
			Paragraph p = new Paragraph() { Alignment = GetAlignment(alignment) };
			p.Add(chunk);
			_document.Add(p);
			_document.Add(Chunk.NEWLINE);
		}

		public void WriteHtml(string htmlSnippet)
		{
			Validate();
			List<IElement> objects = HTMLWorker.ParseToList(new StringReader(htmlSnippet), null);
			foreach (IElement obj in objects)
			{
				_document.Add(obj);
			}
		}

		public iTextSharp.text.Image GetImage(byte[] data)
		{
			Validate();
			return iTextSharp.text.Image.GetInstance(data);
		}

		public void WriteImage(string imageUri)
		{
			Validate();
			iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(imageUri);
			_document.Add(pic);
		}

		public void WriteImage(byte[] data)
		{
			Validate();
			iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(data);
			_document.Add(pic);
		}

		public void EmptyLine()
		{
			Validate();
			_document.Add(Chunk.NEWLINE);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_document != null && _document.IsOpen())
					{
						_document.Close();
					}
					_document = null;
				}
				_disposed = true;
			}
		}

		protected void AddDocumentTitlePage(IList<string> documentTitle)
		{
			Rectangle titlePage = new Rectangle(GetPageSize());
			titlePage.BackgroundColor = new BaseColor(104, 123, 24);
			_document.Add(titlePage);
			// top left corner (llx,lly)
			float llx = 0;
			float lly = GetPageSize().Height - 200;
			float urx = GetPageSize().Width;
			float ury = GetPageSize().Height / 2;
			Rectangle textBox = new Rectangle(llx, lly, urx, ury);
			textBox.BackgroundColor = BaseColor.WHITE;
			PdfContentByte pb = _writer.DirectContent;
			pb.BeginText();
			BaseFont bf = GetFont().GetCalculatedBaseFont(false);
			pb.SetFontAndSize(bf, 20);
			float tx = 100;
			float ty = lly - 50; // 50 points below the top of the rectangle.
			foreach (string text in documentTitle)
			{
				tx = GetPageSize().Width / 2;
				pb.ShowTextAligned(Element.ALIGN_CENTER, text, tx, ty, 0);
				ty -= 20;
			}
			pb.EndText();
			_document.Add(textBox);
			_document.NewPage();
		}
		
		private void Validate()
		{
			if (_document == null || !_document.IsOpen())
			{
				throw new LWPdfDocumentException("The PdfDocument is not open.");
			}
		}
	}
}
