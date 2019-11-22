using System;
using System.Web;
using System.Web.Configuration;
using System.Security.Cryptography.X509Certificates;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Services;
using Google.Apis.Walletobjects.v1;
using Google.Apis.Walletobjects.v1.Data ;
using WalletObjectsSample.Utils;
using System.Collections.Generic;
//using WalletObjectsSample.Verticals;


namespace WalletObjectsSample.Handlers
{
	public class WobInsertHandler 
	{
	public WobInsertHandler()
    {
      //  string stop = "stop";
      WobCredentials credentials = new WobCredentials(
        "649758045458@developer.gserviceaccount.com",
        "GWprivatekey.p12",
        "American Eagle",
        "2966215529466925520");

      X509Certificate2 certificate = null;
      // OAuth
      try
      {
          certificate = new X509Certificate2(
           @"C:\Users\sjohn\Downloads\TestPOSTWebService\TestPOSTWebService\TestPOSTWebService\GWprivatekey.p12", //#ChangeHere
            "notasecret",
            X509KeyStorageFlags.Exportable);
      }
      catch (Exception ex)
      {
          string errmsg = ex.Message;
      }
      ServiceAccountCredential credential = new ServiceAccountCredential(
          new ServiceAccountCredential.Initializer(credentials.serviceAccountId)
          {
              Scopes = new[] { "https://www.googleapis.com/auth/wallet_object.issuer" }
          }.FromCertificate(certificate));



        /*
      var tokenProvider = new AssertionFlowClient(GoogleAuthenticationServer.Description, certificate)
      {
        ServiceAccountId = credentials.serviceAccountId,
        Scope = "https://www.googleapis.com/auth/wallet_object.issuer"
      };

      var authenticator = new OAuth2Authenticator<AssertionFlowClient>(tokenProvider, AssertionFlowClient.GetState);
*/

      // WalletobjectsService
      WalletobjectsService woService = new WalletobjectsService(
        new BaseClientService.Initializer()      
        {
          HttpClientInitializer = credential,
          ApplicationName = "WOBS Sample App"
        });

      // LoyaltyclassResource
      LoyaltyclassResource lcResource = new LoyaltyclassResource(woService);

      // get the class type
        LoyaltyClass loyaltyClass = Loyalty.generateLoyaltyClass(credentials.IssuerId, "LoyaltyClassAE");
        LoyaltyClass lcResponse = woService.Loyaltyclass.Insert(loyaltyClass).Execute();
    
      //else if (type.Equals("offer"))
      //{
      //  OfferClass offerClass = Offer.generateOfferClass(credentials.IssuerId, "OC");
      //  OfferClass ocResponse = woService.Offerclass.Insert(offerClass).Execute();
      //}*/
		}
	}

    public class Loyalty
    {
        public static LoyaltyClass generateLoyaltyClass(string issuerId, string classId)
        {
            // Define general messages
            /*
            IList<WalletObjectMessage> messages = new List<WalletObjectMessage>();
            WalletObjectMessage message = new WalletObjectMessage();
            message.Header = "Welcome";
            message.Body = "Welcome to Banconrista Rewards!";

            Google.Apis.Walletobjects.v1.Data.Uri imageUri = new Google.Apis.Walletobjects.v1.Data.Uri();
            imageUri.UriValue = "https://ssl.gstatic.com/codesite/ph/images/search-48.gif";
            Image messageImage = new Image();
            messageImage.SourceUri = imageUri;
            message.Image = messageImage;

            Google.Apis.Walletobjects.v1.Data.Uri actionUri = new Google.Apis.Walletobjects.v1.Data.Uri();
            actionUri.UriValue = "http://baconrista.com";
            message.ActionUri = actionUri;

            messages.Add(message);*/

            // Define rendering templates per view
            IList<RenderSpec> renderSpec = new List<RenderSpec>();

            RenderSpec listRenderSpec = new RenderSpec();
            listRenderSpec.ViewName = "g_list";
            listRenderSpec.TemplateFamily = "1.loyaltyCard1_list";

            RenderSpec expandedRenderSpec = new RenderSpec();
            expandedRenderSpec.ViewName = "g_expanded";
            expandedRenderSpec.TemplateFamily = "1.loyaltyCard1_expanded";

            renderSpec.Add(listRenderSpec);
            renderSpec.Add(expandedRenderSpec);

            // Define Geofence locations
            IList<LatLongPoint> locations = new List<LatLongPoint>();

            LatLongPoint llp1 = new LatLongPoint();
            llp1.Latitude = 37.422601;
            llp1.Longitude = -122.085286;

            LatLongPoint llp2 = new LatLongPoint();
            llp2.Latitude = 37.429379;
            llp2.Longitude = -122.122730;

            locations.Add(llp1);
            locations.Add(llp2);

            // Create class
            LoyaltyClass wobClass = new LoyaltyClass();
            wobClass.Id = issuerId + "." + classId;
            wobClass.Version = 1;//#ChangeHere
            wobClass.IssuerName = "American Eagle";
            wobClass.ProgramName = "AERewards";

            Google.Apis.Walletobjects.v1.Data.Uri homepageUri = new Google.Apis.Walletobjects.v1.Data.Uri();
            homepageUri.UriValue = "https://www.ae.com";
            homepageUri.Description = "Website";
            wobClass.HomepageUri = homepageUri;

            Google.Apis.Walletobjects.v1.Data.Uri logoImageUri = new Google.Apis.Walletobjects.v1.Data.Uri();
            logoImageUri.UriValue = "http://images4.wikia.nocookie.net/__cb20120115220656/logopedia/images/8/8d/American_Eagle_Outfitters_logo.gif";//#ChangeHere
            Image logoImage = new Image();
            logoImage.SourceUri = logoImageUri;
            wobClass.ProgramLogo = logoImage;

            wobClass.AccountNameLabel = "Member Name";
            wobClass.AccountIdLabel = "Member Id";
            wobClass.RenderSpecs = renderSpec;
            //wobClass.Messages = messages;
            wobClass.ReviewStatus = "underReview";
            wobClass.AllowMultipleUsersPerObject = true;
            wobClass.Locations = locations;
            // Skip issuer data for now
            //wobClass.IssuerData = issuerData; 

            return wobClass;
        }


  
    }

}

