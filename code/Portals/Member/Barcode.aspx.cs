using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Barcode;
using Brierley.WebFrameWork.Portal.Util;

public partial class Barcode : System.Web.UI.Page
{
	private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

		try
		{
			Response.ContentType = "image/jpg";
			var query = Request.QueryString["barcode"];
			if (!string.IsNullOrEmpty(query))
			{
				var link = new BarcodeLink(query);

				if (!string.IsNullOrEmpty(link.BarcodeAssembly) && !string.IsNullOrEmpty(link.BarcodeFactoryClass))
				{
					var factory = ClassLoaderUtil.CreateInstance(link.BarcodeAssembly, link.BarcodeFactoryClass) as IBarcodeGeneratorFactory;
					if (factory != null)
					{
						var nv = new NameValueCollection();
						if (!string.IsNullOrEmpty(link.BarcodeSymbology))
						{
							nv.Add("BarcodeSymbology", link.BarcodeSymbology);
						}
						IBarcodeGenerator bcGen = factory.CreateGenerator(nv);
						System.Drawing.Image image = bcGen.GenerateBarcode(link.Data);

						var ms = new MemoryStream();
						image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

						Response.AppendHeader("Content-Length", ms.Length.ToString());
						Response.BinaryWrite(ms.ToArray());
					}
				}
			}
		}
		catch (Exception ex)
		{
			_logger.Error("Barcode.aspx", "OnInit", string.Empty, ex);
		}
		finally
		{
			Response.End();
		}
	}

}