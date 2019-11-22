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
            balance.String = "00";

            points.Balance = balance;

            LoyaltyObject loyaltyObj = new LoyaltyObject();
            loyaltyObj.ClassId = classId;
            loyaltyObj.Id = objectId;
            loyaltyObj.Version = 1;
            loyaltyObj.State = "active";
            loyaltyObj.Barcode = barcode;
            loyaltyObj.Messages = messages;
            loyaltyObj.AccountName = "John Smith";
            loyaltyObj.AccountId = "12345";
            loyaltyObj.LoyaltyPoints = points;

            return loyaltyObj;
        }

        public static LoyaltyObject generateLoyaltyObject(String classId, String objectId, String name, String memberid, String pointsBalance,long version)
        {
            Barcode barcode = new Barcode();
            barcode.Type = "itf14";
            barcode.Value = memberid;
            barcode.Label = "Rewards ID";
            Google.Apis.Walletobjects.v1.Data.Uri msgImageUri = new Google.Apis.Walletobjects.v1.Data.Uri();
            msgImageUri.UriValue = "http://www.ae.com/Images/mobile/google_wallet/eag.png";
            Image msgImage = new Image();
            msgImage.SourceUri = msgImageUri;

            IList<WalletObjectMessage> messages = new List<WalletObjectMessage>();
            Google.Apis.Walletobjects.v1.Data.Uri actionUri = new Google.Apis.Walletobjects.v1.Data.Uri();
            actionUri.UriValue = "http://www.ae.com";
            if (memberid.Substring(0, 2) == "79")
            {
                WalletObjectMessage message1 = new WalletObjectMessage();
                message1.Header = "YOUR REWARD #";
                message1.Body = "Use this temporary number " + memberid + @" until we send you one that's permanent." + @" It won’t take long!";
                message1.Image = msgImage;
                
                TimeInterval tc = new TimeInterval();
                Google.Apis.Walletobjects.v1.Data.DateTime d1 = new Google.Apis.Walletobjects.v1.Data.DateTime();
                Google.Apis.Walletobjects.v1.Data.DateTime d2 = new Google.Apis.Walletobjects.v1.Data.DateTime();
                //d1.Date = System.Xml.XmlConvert.ToString(System.DateTime.Now, System.Xml.XmlDateTimeSerializationMode.Utc);
                string date1 = System.Xml.XmlConvert.ToString(System.DateTime.Now, System.Xml.XmlDateTimeSerializationMode.Utc);
                d1.Date = System.Xml.XmlConvert.ToDateTime(date1, System.Xml.XmlDateTimeSerializationMode.Utc);
                String date2 = System.Xml.XmlConvert.ToString(System.DateTime.Now.AddMonths(1), System.Xml.XmlDateTimeSerializationMode.Utc);
                d2.Date = System.Xml.XmlConvert.ToDateTime(date2, System.Xml.XmlDateTimeSerializationMode.Utc);
                tc.Start = d1;
                tc.End = d2;
                message1.DisplayInterval = tc;
                message1.ActionUri = actionUri;
                messages.Add(message1);
            }
            //else
            //{
            //    message.Header = "YOUR POINTS";
            //    message.Body = "Not seeing all the points you just earned? They’re on the way! We’ll update your point balance once a day.";
            //}
            WalletObjectMessage message2 = new WalletObjectMessage();
            message2.Header = "YOUR POINTS";
            message2.Body = @"Not seeing all the points you just earned? They’re on the way! "+@"We’ll update your point balance once a day.";
            message2.ActionUri = actionUri;
            message2.Image = msgImage;
            messages.Add(message2);

            ////IList<TextModuleData> texts = new List<TextModuleData>();
            ////TextModuleData text1 = new TextModuleData();
            ////text1.Header = "GET REWARDED";
            ////text1.Body = @"It's simple. Earn 1 point for every dollar spent, online or in-store at AEO, AEO Factory Stores or Aerie. Points are totaled every 3 months and your rewards are delivered right to you."+ @" It doesn't get any easier than that.";
            
            ////TextModuleData text2 = new TextModuleData();
            ////text2.Header = @"WAIT, THERE’S MORE!";
            ////text2.Body = @"Get a special birthday discount when you sign up for emails! Plus, we’ll send you info on exclusive AEREWARDS events and you can even get free Aerie bras! When you buy 5,"+@" you’ll get the 6th free.  Every time.";
            ////TextModuleData text3 = new TextModuleData();
            ////text3.Header = "GET HOOKED UP";
            ////text3.Body = @"Register your AEREWARDS info at AE.com to make managing your account even easier! Plus, you’ll speed up your checkout when you shop online.";
            ////texts.Add(text1);
            ////texts.Add(text2);
            ////texts.Add(text3);
            //TimeInterval tc2 = new TimeInterval();
            //Google.Apis.Walletobjects.v1.Data.DateTime d3 = new Google.Apis.Walletobjects.v1.Data.DateTime();
            //Google.Apis.Walletobjects.v1.Data.DateTime d4 = new Google.Apis.Walletobjects.v1.Data.DateTime();
            //d3.Date = System.Xml.XmlConvert.ToString(System.DateTime.Now, System.Xml.XmlDateTimeSerializationMode.Utc);
            //d4.Date = System.Xml.XmlConvert.ToString(System.DateTime.Now.AddMonths(1), System.Xml.XmlDateTimeSerializationMode.Utc);
            //tc2.Start = d3;
            //tc2.End = d4;
            //message2.DisplayInterval = tc2;
            //tc2.Start.Date = System.DateTime.Now.ToString();
            //tc2.End.Date = System.DateTime.Now.AddMonths(1).ToString();
            //message2.DisplayInterval = tc2;            

            LoyaltyPoints points = new LoyaltyPoints();
            points.Label = "Points";

            LoyaltyPointsBalance balance = new LoyaltyPointsBalance();
            balance.String = pointsBalance;

            points.Balance = balance;

            ////Google.Apis.Walletobjects.v1.Data.Uri linkmoduleUri = new Google.Apis.Walletobjects.v1.Data.Uri();
            ////linkmoduleUri.Description = "www.aerie.com";
            ////linkmoduleUri.UriValue = "http://www.ae.com/aerie/index.jsp";
            ////IList<Google.Apis.Walletobjects.v1.Data.Uri> lmuri = new List<Google.Apis.Walletobjects.v1.Data.Uri>();
            ////lmuri.Add(linkmoduleUri);
            ////LinksModuleData lm = new LinksModuleData();
            ////lm.Uris = lmuri;

            LoyaltyObject loyaltyObj = new LoyaltyObject();
            loyaltyObj.ClassId = classId;
            loyaltyObj.Id = objectId;
            loyaltyObj.Version = version;
            loyaltyObj.State = "active";
            loyaltyObj.Barcode = barcode;
            loyaltyObj.Messages = messages;
           // loyaltyObj.TextModulesData = texts;
            loyaltyObj.AccountName = name;
            loyaltyObj.AccountId = memberid;
            loyaltyObj.LoyaltyPoints = points;
            ////loyaltyObj.LinksModuleData = lm;
            
            return loyaltyObj;
        }

    }
}