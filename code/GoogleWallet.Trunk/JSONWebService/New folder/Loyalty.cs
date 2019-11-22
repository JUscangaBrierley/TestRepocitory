using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Walletobjects.v1;
using Google.Apis.Walletobjects.v1.Data;


namespace TestPOSTWebService 
{
    public class Loyalty
    {

        public static LoyaltyObject generateLoyaltyObject(String classId, String objectId)
        {
            Barcode barcode = new Barcode();
            barcode.Type = "itf14";
            barcode.Value = "1234567";
            barcode.Label = "Rewards ID";

            IList<WalletObjectMessage> messages = new List<WalletObjectMessage>();

            WalletObjectMessage message = new WalletObjectMessage();
            message.Header = "Winter Sale";
            message.Body = "Save 25-40% off in our Winter Sale shop instore or online now.";

            Google.Apis.Walletobjects.v1.Data.Uri actionUri = new Google.Apis.Walletobjects.v1.Data.Uri();
            actionUri.UriValue = "http://www.ae.com";
            message.ActionUri = actionUri;

            messages.Add(message);

            LoyaltyPoints points = new LoyaltyPoints();
            points.Label = "Points";

            LoyaltyPointsBalance balance = new LoyaltyPointsBalance();
            balance.String = "75";

            points.Balance = balance;

            LoyaltyObject loyaltyObj = new LoyaltyObject();
            loyaltyObj.ClassId = classId;
            loyaltyObj.Id = objectId;
            loyaltyObj.Version = "1";
            loyaltyObj.State = "active";
            loyaltyObj.Barcode = barcode;
            loyaltyObj.Messages = messages;
            loyaltyObj.AccountName = "John Smith";
            loyaltyObj.AccountId = "12345";
            loyaltyObj.LoyaltyPoints = points;

            return loyaltyObj;
        }

        public static LoyaltyObject generateLoyaltyObject(String classId, String objectId, String name, String memberid, String pointsBalance)
        {
            Barcode barcode = new Barcode();
            barcode.Type = "itf14";
            barcode.Value = memberid;
            barcode.Label = "Rewards ID";

            IList<WalletObjectMessage> messages = new List<WalletObjectMessage>();

            WalletObjectMessage message = new WalletObjectMessage();
            message.Header = "Winter Sale";
            message.Body = "Save 25-40% off in our Winter Sale shop instore or online now.";

            Google.Apis.Walletobjects.v1.Data.Uri actionUri = new Google.Apis.Walletobjects.v1.Data.Uri();
            actionUri.UriValue = "http://www.ae.com";
            message.ActionUri = actionUri;

            messages.Add(message);

            LoyaltyPoints points = new LoyaltyPoints();
            points.Label = "Points";

            LoyaltyPointsBalance balance = new LoyaltyPointsBalance();
            balance.String = pointsBalance;

            points.Balance = balance;

            LoyaltyObject loyaltyObj = new LoyaltyObject();
            loyaltyObj.ClassId = classId;
            loyaltyObj.Id = objectId;
            loyaltyObj.Version = "1";
            loyaltyObj.State = "active";
            loyaltyObj.Barcode = barcode;
            loyaltyObj.Messages = messages;
            loyaltyObj.AccountName = name;
            loyaltyObj.AccountId = memberid;
            loyaltyObj.LoyaltyPoints = points;

            return loyaltyObj;
        }

    }
}