using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class PassDef
	{
		public string ToJSON()
		{
			string result = @"{
  ""formatVersion"" : 1,
  ""passTypeIdentifier"" : ""pass.com.brierley.arttest.card"",
  ""serialNumber"" : ""1234"",
  ""teamIdentifier"" : ""F2U34HTK7X"",
  ""webServiceURL"" : ""https://10.4.4.56:4567/passes/"",
  ""authenticationToken"" : ""ThisIsATestAuthToken"",
  ""locations"" : [
    {
      ""longitude"" : -96.820797,
      ""latitude"" : 33.071603
    }
  ],
  ""barcode"" : {
    ""message"" : ""123456789"",
    ""format"" : ""PKBarcodeFormatPDF417"",
    ""messageEncoding"" : ""iso-8859-1""
  },
  ""organizationName"" : ""Brierley Partners"",
  ""description"" : ""Store card"",
  ""logoText"" : ""Brierley Partners"",
  ""foregroundColor"" : ""rgb(255, 255, 255)"",
  ""backgroundColor"" : ""rgb(118, 74, 50)"",
  ""storeCard"" : {
    ""primaryFields"" : [
      {
        ""key"" : ""balance"",
        ""label"" : ""remaining balance"",
        ""value"" : 25,
        ""currencyCode"" : ""USD""
      }
    ],
    ""auxiliaryFields"" : [
      {
        ""key"" : ""level"",
        ""label"" : ""LEVEL"",
        ""value"" : ""Gold""
      },
      {
        ""key"" : ""usual"",
        ""label"" : ""THE USUAL"",
        ""value"" : ""Trusted Advisor""
      }
    ],
    ""backFields"" : [
      {
        ""key"" : ""terms"",
        ""label"" : ""TERMS AND CONDITIONS"",
        ""value"" : ""Brierley Partners offers this card as a test.""
      }
    ]
  }
}";
			return result;
		}
	}
}
