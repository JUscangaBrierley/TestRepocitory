using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
//using Google.Apis.Json;
//using Google.Apis.Util;
//using Google.Apis.Http;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Walletobjects.v1.Data;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AmericanEagle.SDK.Global
{
    public class Utils
    {
        String issuer;
        IList<LoyaltyObject> objects;
        RSACryptoServiceProvider key;

        public Utils(String iss, IList<LoyaltyObject> obj, X509Certificate2 cert)
        {
            issuer = iss;
            objects = obj;
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;
            byte[] privateKeyBlob = rsa.ExportCspBlob(true);
            key = new RSACryptoServiceProvider();
            key.ImportCspBlob(privateKeyBlob);
        }

        private string CreateSerializedHeader()
        {
            var header = new GoogleJsonWebSignature.Header()
            {
                Algorithm = "RS256",
                Type = "JWT"
            };

            //return NewtonsoftJsonSerializer.Instance.Serialize(header);
            return "This is for to compile";
        }

        private string CreateSerializedPayload()
        {
            var iat = (int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var jwtContainer = new JsonWebToken.Payload()
            {
                Issuer = issuer,
                Audience = "google",
                Type = "savetowallet",
                IssuedAtTimeSeconds = iat,
                Objects = new JsonWebToken.Payload.Content()
                {
                    loyaltyObjects = objects
                },
                Origins = new[] { "http://localhost:56893" } //#ChangeHere
            };

            //return NewtonsoftJsonSerializer.Instance.Serialize(jwtContainer);
            return "This is for to compile";
        }

        private string CreateWSPayload(JsonWebToken.Payload.WebserviceResponse response)
        {
            var iat = (int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var jwtContainer = new JsonWebToken.Payload()
            {
                Issuer = issuer,
                Audience = "google",
                Type = "loyaltywebservice",
                IssuedAtTimeSeconds = iat,
                Objects = new JsonWebToken.Payload.Content()
                {
                    loyaltyObjects = objects,
                    webserviceResponse = response
                },
            };

            //return NewtonsoftJsonSerializer.Instance.Serialize(jwtContainer);
            return "This is for to compile";
        }

        public String GenerateJwt()
        {
            String header = UrlSafeBase64Encode(CreateSerializedHeader());
            String body = UrlSafeBase64Encode(CreateSerializedPayload());
            String content = header + "." + body;
            String signature = CreateSignature(content);
            return content + "." + signature;
        }

        public String GenerateWsJwt(JsonWebToken.Payload.WebserviceResponse response)
        {
            String header = UrlSafeBase64Encode(CreateSerializedHeader());
            String body = UrlSafeBase64Encode(CreateWSPayload(response));
            String content = header + "." + body;
            String signature = CreateSignature(content);
            return content + "." + signature;
        }
        private String CreateSignature(String content)
        {
            return UrlSafeBase64Encode(key.SignData(Encoding.UTF8.GetBytes(content), "SHA256"));
        }

        /// <summary>Encodes the provided UTF8 string into an URL safe base64 string.</summary>
        /// <param name="value">Value to encode</param>
        /// <returns>The URL safe base64 string</returns>
        private string UrlSafeBase64Encode(string value)
        {
            return UrlSafeBase64Encode(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>Encodes the byte array into an URL safe base64 string.</summary>
        /// <param name="bytes">Byte array to encode</param>
        /// <returns>The URL safe base64 string</returns>
        private string UrlSafeBase64Encode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).Replace("=", String.Empty).Replace('+', '-').Replace('/', '_');
        }
    }
}