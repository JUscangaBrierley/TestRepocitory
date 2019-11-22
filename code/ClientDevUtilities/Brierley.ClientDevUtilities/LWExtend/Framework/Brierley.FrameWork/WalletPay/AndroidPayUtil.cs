using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Google.Apis.Walletobjects.v1.Data;
using Google.Apis.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Walletobjects.v1;
using Google.Apis.Services;
using Google.Apis.Requests;
using Google;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using System.IO;

namespace Brierley.FrameWork.WalletPay
{
    public class AndroidPayUtil
    {
        private const string _className = "AndroidPayUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        public static bool IsAndroidPayConfigured
        {
            get
            {
                return !string.IsNullOrEmpty(LWConfigurationUtil.GetConfigurationValue("APServiceAccountEmailAddress")) &&
                       !string.IsNullOrEmpty(LWConfigurationUtil.GetConfigurationValue("APSigningCertificateName")) &&
                       !string.IsNullOrEmpty(LWConfigurationUtil.GetConfigurationValue("APIssuerID"));
            }
        }

        public static string GetLoyaltyClassId(string cardName)
        {
            string issuerId = LWConfigurationUtil.GetConfigurationValue("APIssuerID");
            string org = RemoveNonAlphaNums(LWConfigurationUtil.GetCurrentEnvironmentContext().Organization);
            string env = RemoveNonAlphaNums(LWConfigurationUtil.GetCurrentEnvironmentContext().Environment);

            // ID format: IssuerID.org_env_name
            return string.Format("{0}.{1}_{2}_{3}", issuerId, org, env, RemoveNonAlphaNums(cardName));
        }

        public static string GetLoyaltyObjectId(Member member)
        {
            string issuerId = LWConfigurationUtil.GetConfigurationValue("APIssuerID");
            string org = RemoveNonAlphaNums(LWConfigurationUtil.GetCurrentEnvironmentContext().Organization);
            string env = RemoveNonAlphaNums(LWConfigurationUtil.GetCurrentEnvironmentContext().Environment);

            // ID format: IssuerID.org_env_ipcode
            return string.Format("{0}.{1}_{2}_{3}", issuerId, org, env, member.IpCode);
        }

        public static void SendDrafts(AndroidPayLoyaltyCard card)
        {
            string loyaltyClassId = GetLoyaltyClassId(card.Name);

            if (card.LoyaltyClass == null) // Check if we've already sent this card to AP
            {
                card.LoyaltyClass = GetLoyaltyClass(card.Name);
            }

            // Generate LoyaltyClass objects
            LoyaltyClass newClass = GenerateLoyaltyClass(card, loyaltyClassId);
            // Send LoyaltyClass objects
            if (card.LoyaltyClass == null)
                GetService().Loyaltyclass.Insert(newClass).Execute(); // Insert
            else
                GetService().Loyaltyclass.Update(newClass, loyaltyClassId).Execute(); // Update

            card.LoyaltyClass = newClass;
        }

        public static LoyaltyClass GenerateLoyaltyClass(AndroidPayLoyaltyCard card, string loyaltyClassId)
        {
            LoyaltyClass loyaltyClass = new LoyaltyClass();

            loyaltyClass.AccountIdLabel = card.AccountIdLabel;
            loyaltyClass.AccountNameLabel = card.AccountNameLabel;
            loyaltyClass.AllowMultipleUsersPerObject = true;
            loyaltyClass.Id = loyaltyClassId;
            if (!string.IsNullOrEmpty(card.MainImage))
                loyaltyClass.ImageModulesData = new List<ImageModuleData>() {
                    new ImageModuleData() {
                        MainImage = new Image() {
                            SourceUri = new Google.Apis.Walletobjects.v1.Data.Uri() {
                                UriValue = GetValidUrl(card.MainImage)
                } } } };
            loyaltyClass.HexBackgroundColor = RGBToHexColor(card.BackgroundColor);
            //card.ForegroundColor -- BME - not sure what to do with this now, probably unused
            loyaltyClass.IssuerName = card.IssuerName;
            loyaltyClass.ProgramLogo = new Image() {
                SourceUri = new Google.Apis.Walletobjects.v1.Data.Uri() {
                    UriValue = GetValidUrl(card.ProgramLogoImage)
            } };
            loyaltyClass.ProgramName = card.ProgramName;
            loyaltyClass.ReviewStatus = card.LoyaltyClass != null && card.LoyaltyClass.ReviewStatus != "draft" ? "underReview" : "draft";
            loyaltyClass.RewardsTier = card.RewardTierName;
            loyaltyClass.RewardsTierLabel = card.RewardTierLabel;
            loyaltyClass.TextModulesData = new List<TextModuleData>() {
                new TextModuleData() {
                    Body = card.LegalCopy
            } };

            return loyaltyClass;  
        }

        public static LoyaltyClass GetLoyaltyClass(string cardName)
        {
            try
            {
                return GetService().Loyaltyclass.Get(GetLoyaltyClassId(cardName)).Execute();
            }
            catch(GoogleApiException ex)
            {
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound) // If it isn't found, just return null
                    return null;
                throw; // Otherwise throw the exception
            }
        }

        public static List<LoyaltyClass> GetAllLoyaltyClasses()
        {
            string sIssuerId = GetCredentials().IssuerId;
            return GetService().Loyaltyclass.List(long.Parse(sIssuerId)).Execute().Resources.ToList();
        }

        public static LoyaltyObject GetLoyaltyObject(Member member)
        {
            try
            {
                return GetService().Loyaltyobject.Get(GetLoyaltyObjectId(member)).Execute();
            }
            catch (GoogleApiException ex)
            {
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound) // If it isn't found, just return null
                    return null;
                throw; // Otherwise throw the exception
            }
        }

        public static LoyaltyObject GenerateLoyaltyObject(Member member, AndroidPayLoyaltyCard card, LoyaltyObject loyaltyObject)
        {
            // Required: classId, id, state, version
            // Optional: accountId, accountName, barcode locations, loyaltyPoints
            if (loyaltyObject == null)
                loyaltyObject = new LoyaltyObject();

            loyaltyObject.ClassId = GetLoyaltyClassId(card.Name);
            loyaltyObject.ClassReference = GetLoyaltyClass(card.Name);
            loyaltyObject.Id = GetLoyaltyObjectId(member);
            loyaltyObject.State = "active";

            ContextObject contextObject = new ContextObject()
            {
                Owner = member,
                Mode = RuleExecutionMode.Real
            };

            loyaltyObject.AccountId = GetBscriptValue(card.AccountIdValue, contextObject);
            loyaltyObject.AccountName = GetBscriptValue(card.AccountNameValue, contextObject);
            if (!string.IsNullOrEmpty(card.BarcodeValue))
            {
                loyaltyObject.Barcode = new Barcode()
                {
                    AlternateText = GetBscriptValue(card.BarcodeAlternateText, contextObject),
                    Type = card.BarcodeType.ToString(),
                    Value = GetBscriptValue(card.BarcodeValue, contextObject)
                };
            }
            if(!string.IsNullOrEmpty(card.LoyaltyCurrencyValueExpression))
            {
                LoyaltyPointsBalance balance = new LoyaltyPointsBalance();

                object currency = ExpressionUtil.Create(card.LoyaltyCurrencyValueExpression).evaluate(contextObject);

                if (currency is long || currency is int)
                {
                    balance.Int = (int)currency;
                }
                else if (currency is double || currency is decimal || currency is float)
                {
                    decimal currencyBalance = (decimal)currency;
                    if (currencyBalance % 1 == 0)
                        balance.Int = (int)currencyBalance;
                    else
                        balance.Double = (double)currencyBalance;
                }
                else
                    throw new LWException(string.Format("Unsupported loyalty currency value type: {0}", currency.GetType().ToString()));

                loyaltyObject.LoyaltyPoints = new LoyaltyPoints()
                {
                    Label = card.LoyaltyCurrencyLabel,
                    Balance = balance
                };
            }
            loyaltyObject.Locations = GetFavoriteStoreLatLongs(member);

            return loyaltyObject;
        }
        
        public static void SendForApproval(string name)
        {
            LoyaltyClass c = GetLoyaltyClass(name);
            c.ReviewStatus = "underReview";
            GetService().Loyaltyclass.Update(c, c.Id).Execute();
        }

        public static bool LoyaltyClassHasChanges(AndroidPayLoyaltyCard card)
        {
            return card.AccountIdLabel != card.LoyaltyClass.AccountIdLabel ||
                   card.AccountNameLabel != card.LoyaltyClass.AccountNameLabel ||
                   card.MainImage != card.LoyaltyClass.ImageModulesData?.FirstOrDefault()?.MainImage?.SourceUri?.UriValue ||
                   //RGBToHexColor(card.BackgroundColor) != card.LoyaltyClass.HexBackgroundColor || Disabling for now since HexBackgroundColor appears to be ignored
                   card.IssuerName != card.LoyaltyClass.IssuerName ||
                   card.ProgramLogoImage != card.LoyaltyClass.ProgramLogo?.SourceUri?.UriValue ||
                   card.ProgramName != card.LoyaltyClass.ProgramName ||
                   card.RewardTierName != card.LoyaltyClass.RewardsTier ||
                   card.RewardTierLabel != card.LoyaltyClass.RewardsTierLabel ||
                   card.LegalCopy != card.LoyaltyClass.TextModulesData?.FirstOrDefault()?.Body;
        }

        public static string GetJWT(Member member, AndroidPayLoyaltyCard card = null)
        {
            WobUtils utils = GetUtils();

            // Find correct card
            if (card == null)
                card = GetCardForMember(member);

            if (card == null)
                throw new LWException(string.Format("No Android Pay loyalty cards found that matched member with ipcode {0}", member.IpCode));

            // Determine if member already has a card
            LoyaltyObject loyaltyObject = GetLoyaltyObject(member);

            utils.addObject(GenerateLoyaltyObject(member, card, loyaltyObject));
            return utils.GenerateJwt();
        }

        public static bool UpdateLoyaltyObject(Member member)
        {
            LoyaltyObject loyaltyObject = GetLoyaltyObject(member);

            if (loyaltyObject == null)
                return false;

            AndroidPayLoyaltyCard card = GetCardForMember(member);

            if (card == null)
                throw new LWException(string.Format("No Android Pay loyalty cards found that matched member with ipcode {0}", member.IpCode));

            loyaltyObject = GenerateLoyaltyObject(member, card, loyaltyObject);

            GetService().Loyaltyobject.Update(loyaltyObject, loyaltyObject.Id).Execute();

            return true;
        }

        private static WalletobjectsService GetService()
        {
            WobCredentials credentials = GetCredentials();

            X509Certificate2 certificate;
            using (DataService service = LWDataServiceUtil.DataServiceInstance())
            {
                X509Cert x509cert = service.GetX509Cert(credentials.ServiceAccountPrivateKey);

                if (x509cert == null)
                    throw new LWException("Invalid service account private key selected. Please ensure the APSigningCertificateName configuration value matches the name of a certificate");

                certificate = x509cert.X509Certificate2;
            }

            ServiceAccountCredential credential = new ServiceAccountCredential(
            new ServiceAccountCredential.Initializer(credentials.serviceAccountId)
            {
                Scopes = new[] { "https://www.googleapis.com/auth/wallet_object.issuer" }
            }.FromCertificate(certificate));

            // create the service
            return new WalletobjectsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Wallet Objects API Sample",
            });
        }

        private static WobCredentials GetCredentials()
        {
            return new WobCredentials(
                LWConfigurationUtil.GetConfigurationValue("APServiceAccountEmailAddress"),
                LWConfigurationUtil.GetConfigurationValue("APSigningCertificateName"),
                "LoyaltyNavigator",
                LWConfigurationUtil.GetConfigurationValue("APIssuerID"));
        }

        private static WobUtils GetUtils()
        {
            WobCredentials credentials = GetCredentials();
            X509Certificate2 certificate;
            using (DataService service = LWDataServiceUtil.DataServiceInstance())
                certificate = service.GetX509Cert(credentials.ServiceAccountPrivateKey).X509Certificate2;
            return new WobUtils(credentials.ServiceAccountId, certificate);
        }

        private static AndroidPayLoyaltyCard GetCardForMember(Member member)
        {
            List<AndroidPayLoyaltyCard> cards;
            using (MobileDataService service = LWDataServiceUtil.MobileServiceInstance())
                cards = service.GetAllAndroidPayLoyaltyCard();

            foreach (AndroidPayLoyaltyCard apCard in cards)
            {
                if ((bool)ExpressionUtil.Create(apCard.LoyaltyCardSelectionCriteria).evaluate(new ContextObject() { Owner = member }))
                {
                    return apCard;
                }
            }
            return null;
        }

        private static string RemoveNonAlphaNums(string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(input, string.Empty);
        }

        private static string RGBToHexColor(string rgbColor)
        {
            System.Drawing.Color result = System.Drawing.Color.Black;
            // rgb({0},{1},{2})
            if (!string.IsNullOrWhiteSpace(rgbColor)
                && rgbColor.StartsWith("rgb(", StringComparison.CurrentCultureIgnoreCase)
                && rgbColor.EndsWith(")"))
            {
                rgbColor = rgbColor.Substring("rgb(".Length);
                rgbColor = rgbColor.Substring(0, rgbColor.Length - 1);
                string[] colors = rgbColor.Split(',');
                if (colors != null && colors.Length == 3)
                {
                    int red = int.Parse(colors[0]);
                    int green = int.Parse(colors[1]);
                    int blue = int.Parse(colors[2]);
                    result = System.Drawing.Color.FromArgb(red, green, blue);
                }
            }
            return "#" + result.R.ToString("X2") + result.G.ToString("X2") + result.B.ToString("X2");
        }

        private static string GetBscriptValue(string value, ContextObject contextObject)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
                return ExpressionUtil.Create(value).evaluate(contextObject).ToString();
        }

        private static List<LatLongPoint> GetFavoriteStoreLatLongs(Member member)
        {
            using (ContentService contentService = LWDataServiceUtil.ContentServiceInstance())
            using (LoyaltyDataService loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                List<MemberStore> memberStores = loyaltyService.GetMemberStoresByMember(member.IpCode);

                if (memberStores == null || memberStores.Count == 0)
                    return null;

                long[] storeIds = (from s in memberStores select s.StoreDefId).ToArray();
                List<StoreDef> storeDefs = contentService.GetAllStoreDefs(storeIds);

                List<LatLongPoint> points = new List<LatLongPoint>();

                foreach (StoreDef store in storeDefs)
                {
                    // Make sure this is a valid location before adding it
                    if (store.Latitude.HasValue && Math.Abs(store.Latitude.Value) <= 90 &&
                        store.Longitude.HasValue && Math.Abs(store.Longitude.Value) <= 180)
                        points.Add(new LatLongPoint()
                        {
                            Latitude = store.Latitude,
                            Longitude = store.Longitude
                        });

                    if (points.Count >= 10) // To maintain parity with Apple Wallet, limit favorited stores to 10
                        break;
                }

                return points.Count > 0 ? points : null;
            }
        }

        private static string GetValidUrl(string imageUrl)
        {
            System.Uri result;
            if (System.Uri.TryCreate(imageUrl, UriKind.Absolute, out result))
                return imageUrl;

            string lwContentRootURL = LWConfigurationUtil.GetConfigurationValue("LWContentRootURL");
            if (string.IsNullOrEmpty(lwContentRootURL))
            {
                string msg = "LWContentRootURL must be defined in order to generate Android Pay passes.";
                _logger.Error(_className, "GetValidUrl", msg);
                throw new LWException(msg);
            }
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            if (ctx == null)
            {
                string msg = "Current environment context not defined.";
                _logger.Error(_className, "GetValidUrl", msg);
                throw new LWException(msg);
            }
            if (!lwContentRootURL.EndsWith("/"))
                lwContentRootURL += "/";
            return string.Format("{0}{1}/{2}", lwContentRootURL, ctx.Organization, imageUrl);
        }
    }
}
