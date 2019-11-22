using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JSONWebService
{
    public class WebserviceResponse
    {
        public string result { get; set; }
    }

    public class Barcode
    {
        public string alternateText { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Label
    {
        public string @string { get; set; }
    }

    public class Value
    {
        public string @string { get; set; }
    }

    public class Col0
    {
        public Label label { get; set; }
        public Value value { get; set; }
    }

    public class Row0
    {
        public Col0 col0 { get; set; }
    }

    public class Label2
    {
        public string @string { get; set; }
    }

    public class Value2
    {
        public int @int { get; set; }
    }

    public class Col1
    {
        public Label2 label { get; set; }
        public Value2 value { get; set; }
    }

    public class InfoModule
    {
        public Row0 row0 { get; set; }
        public Col1 col1 { get; set; }
    }

    public class GExpanded
    {
        public InfoModule infoModule { get; set; }
    }

    public class IssuerData
    {
        public GExpanded g_expanded { get; set; }
    }

    public class Balance
    {
        public string @string { get; set; }
    }

    public class LoyaltyPoints
    {
        public Balance balance { get; set; }
        public string label { get; set; }
        public string pointsType { get; set; }
    }

    public class ActionUri
    {
        public string uri { get; set; }
    }

    public class SourceUri
    {
        public string uri { get; set; }
    }

    public class Image
    {
        public SourceUri sourceUri { get; set; }
    }

    public class Message
    {
        public ActionUri actionUri { get; set; }
        public string body { get; set; }
        public string header { get; set; }
        public Image image { get; set; }
    }

    public class LoyaltyObject
    {
        public string accountId { get; set; }
        public string accountName { get; set; }
        public Barcode barcode { get; set; }
        public string classId { get; set; }
        public int classVersion { get; set; }
        public string id { get; set; }
        public IssuerData issuerData { get; set; }
        public LoyaltyPoints loyaltyPoints { get; set; }
        public List<Message> messages { get; set; }
        public string state { get; set; }
        public string type { get; set; }
        public int version { get; set; }
    }

    public class Payload
    {
        public WebserviceResponse webserviceResponse { get; set; }
        public List<LoyaltyObject> loyaltyObjects { get; set; }
    }

    public class RestapiRootObject
    {
        public string apiVersion { get; set; }
        public string iss { get; set; }
        public string typ { get; set; }
        public string aud { get; set; }
        public int iat { get; set; }
        public Payload payload { get; set; }
    }
}