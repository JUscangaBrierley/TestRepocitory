using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.IO;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Newtonsoft.Json;
using Brierley.FrameWork.bScript;
using System.Configuration;

namespace Brierley.FrameWork.WalletPay
{
    public class AppleWalletUtil
    {
        private class Location
        {
            public double longitude { get; set; }
            public double latitude { get; set; }
        }

        private const string _className = "AppleWalletUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);


        public static byte[] GenerateLoyaltyCard(Member member, AppleWalletLoyaltyCard card = null, MemberAppleWalletLoyaltyCard memberCard = null)
        {
            using (MobileDataService mobileDataService = LWDataServiceUtil.MobileServiceInstance())
            {
                byte[] result = null;
                bool isNewMemberCard = false;

                if (card == null)
                    card = GetCardForMember(member);

                if (card == null)
                    throw new Exception(string.Format("No Apple Wallet loyalty cards found that matched member with ipcode {0}", member.IpCode));

                if (memberCard == null)
                    memberCard = mobileDataService.GetMemberAppleWalletLoyaltyCardByMemberId(member.IpCode);

                if (memberCard == null)
                {
                    memberCard = new MemberAppleWalletLoyaltyCard() { MemberId = member.IpCode };
                    isNewMemberCard = true;
                }

                // generate json for the pass.json file
                string passJSON = LoyaltyCardToJSON(member, card, memberCard);

                // generate the pass
                result = GeneratePass(card, passJSON, memberCard);

                if (isNewMemberCard)
                    mobileDataService.CreateMemberAppleWalletLoyaltyCard(memberCard);
                else
                    mobileDataService.UpdateMemberAppleWalletLoyaltyCard(memberCard);

                return result;
            }
        }


        private static string GetNextSerialNumber()
        {
            using (var svc = LWDataServiceUtil.DataServiceInstance())
            {
                long ID = svc.GetNextID("PBSerialNumber");
                return ID.ToString();
            }
        }

        private static string GetAuthToken()
        {
            string result = Guid.NewGuid().ToString("N");
            return result;
        }

        private static byte[] GeneratePass(AppleWalletLoyaltyCard card, string passJSON, MemberAppleWalletLoyaltyCard memberCard)
        {
            byte[] result = null;

            string imageRoot = LWConfigurationUtil.GetConfigurationValue("LWContentRootPath");
            if (string.IsNullOrEmpty(imageRoot))
            {
                string msg = "LWContentRootPath must be defined in order to generate passbook passes.";
                _logger.Error(_className, "GeneratePass", msg);
                throw new LWException(msg);
            }
            imageRoot = imageRoot.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (!imageRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                imageRoot += Path.DirectorySeparatorChar;
            }

            // add organization
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            if (ctx != null && !string.IsNullOrEmpty(ctx.Organization))
            {
                imageRoot += ctx.Organization;
            }
            else
            {
                imageRoot += ConfigurationManager.AppSettings["LWOrganization"];
            }

            // generate the pass package
            string tmpPath = Path.GetTempPath();
            string passDir = string.Format("{0}PassbookPass_{1}.raw", tmpPath, Thread.CurrentThread.ManagedThreadId);
            string passFile = string.Format("{0}PassbookPass_{1}.pkpass", tmpPath, Thread.CurrentThread.ManagedThreadId);
            DirectoryInfo pd = Directory.CreateDirectory(passDir);
            Dictionary<string, string> manifest = new Dictionary<string, string>();
            try
            {
                // generate pass.json
                using (JsonTextWriter jtw = new JsonTextWriter(File.CreateText(passDir + Path.DirectorySeparatorChar + "pass.json")))
                {
                    jtw.WriteRaw(passJSON);
                    jtw.Close();
                }
                manifest.Add("pass.json", memberCard.LastHash = InsecureSHA1Hash(passJSON));

                // copy images, updating manifest
                if (!imageRoot.EndsWith("" + Path.DirectorySeparatorChar)) imageRoot += Path.DirectorySeparatorChar;

                HashAndCopyFile(card.IconImageX1, imageRoot, manifest, passDir, "icon.png");
                HashAndCopyFile(card.IconImageX2, imageRoot, manifest, passDir, "icon@2x.png");
                HashAndCopyFile(card.IconImageX3, imageRoot, manifest, passDir, "icon@3x.png");

                HashAndCopyFile(card.LogoImageX1, imageRoot, manifest, passDir, "logo.png");
                HashAndCopyFile(card.LogoImageX2, imageRoot, manifest, passDir, "logo@2x.png");
                HashAndCopyFile(card.LogoImageX3, imageRoot, manifest, passDir, "logo@3x.png");

                HashAndCopyFile(card.StripImageX1, imageRoot, manifest, passDir, "strip.png");
                HashAndCopyFile(card.StripImageX2, imageRoot, manifest, passDir, "strip@2x.png");
                HashAndCopyFile(card.StripImageX3, imageRoot, manifest, passDir, "strip@3x.png");

                // write manifest
                string manifestJSON = JsonConvert.SerializeObject(manifest, Formatting.Indented);
                File.WriteAllText(passDir + Path.DirectorySeparatorChar + "manifest.json", manifestJSON);

                // sign the manifest
                byte[] signature = SignManifest(manifestJSON);
                if (signature == null || signature.Length < 1) throw new LWException("Error signing the manifest");
                File.WriteAllBytes(passDir + Path.DirectorySeparatorChar + "signature", signature);

                // zip it up
                if (ZipUtils.ZipFolder(passDir, passFile))
                {
                    // read zip file into byte[] and return
                    result = File.ReadAllBytes(passFile);
                }
            }
            finally
            {
                if (pd != null) pd.Delete(true);
                if (File.Exists(passFile)) File.Delete(passFile);
            }
            return result;
        }

        private static string LoyaltyCardToJSON(Member member, AppleWalletLoyaltyCard card, MemberAppleWalletLoyaltyCard memberCard)
        {
            string language = member.PreferredLanguage;
            if (string.IsNullOrEmpty(language))
                language = LanguageChannelUtil.GetDefaultCulture();
            ContextObject contextObject = new ContextObject() { Owner = member, Mode = RuleExecutionMode.Real };

            Dictionary<string, object> pass = new Dictionary<string, object>();
            pass.Add("formatVersion", 1);
            pass.Add("passTypeIdentifier", LWConfigurationUtil.GetConfigurationValue("AWLoyaltyCardPassTypeID"));
            pass.Add("serialNumber", memberCard.SerialNumber = string.IsNullOrEmpty(memberCard.SerialNumber) ? GetNextSerialNumber() : memberCard.SerialNumber);
            pass.Add("teamIdentifier", LWConfigurationUtil.GetConfigurationValue("PassBookTeamID"));
            if (LWConfigurationUtil.GetConfigurationValue("PassBookWebServiceURL").Contains("https")) pass.Add("webServiceURL", LWConfigurationUtil.GetConfigurationValue("PassBookWebServiceURL")); // Must contain https
            pass.Add("authenticationToken", memberCard.AuthToken = string.IsNullOrEmpty(memberCard.AuthToken) ? GetAuthToken() : memberCard.AuthToken);
            pass.Add("organizationName", LWConfigurationUtil.GetConfigurationValue("PassBookSigningOrgName"));
            pass.Add("description", card.GetDescription(language));
            if (!string.IsNullOrEmpty(card.GetLogoText(language))) pass.Add("logoText", card.GetLogoText(language));
            pass.Add("foregroundColor", card.ForegroundColor);
            pass.Add("backgroundColor", card.BackgroundColor);
            Location[] locations = GetFavoriteStoreLatLongs(member);
            if (locations != null && locations.Length > 0) pass.Add("locations", locations);

            if (!string.IsNullOrEmpty(GetBscriptValue(card.BarcodeMessage, contextObject)) && !string.IsNullOrEmpty(card.BarcodeEncoding))
            {
                // iOS < 9.0
                pass.Add("barcode", new Dictionary<string, object>()
                {
                    { "message", GetBscriptValue(card.BarcodeMessage, contextObject) },
                    { "format", card.BarcodeFormat.ToString() },
                    { "messageEncoding", card.BarcodeEncoding },
                    { "altText", GetBscriptValue(card.BarcodeAlternateText, contextObject) }
                }
                );

                // iOS >= 9.0
                pass.Add("barcodes", new List<Dictionary<string, object>>() { new Dictionary<string, object>()
                {
                    { "message", GetBscriptValue(card.BarcodeMessage, contextObject) },
                    { "format", card.BarcodeFormat.ToString() },
                    { "messageEncoding", card.BarcodeEncoding },
                    { "altText", GetBscriptValue(card.BarcodeAlternateText, contextObject) }
                } }
                );
            }

            Dictionary<string, object> storeCardFields = new Dictionary<string, object>();
            List<Dictionary<string, object>> primaryFields = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> secondaryFields = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> auxilliaryFields = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> backFields = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> headerFields = new List<Dictionary<string, object>>();

            Dictionary<string, object> entry;

            card.ExtendedFields.Sort((x, y) => x.DisplayOrder.CompareTo(y.DisplayOrder));
            foreach (AppleWalletExtendedField extendedField in card.ExtendedFields)
            {
                entry = new Dictionary<string, object>();
                entry.Add("key", extendedField.Name);
                entry.Add("label", extendedField.GetLabel(language));
                entry.Add("value", new ContentEvaluator(extendedField.GetValue(language), contextObject).Evaluate("##", 10));

                switch (extendedField.FieldType)
                {
                    case AppleWalletExtendedFieldType.PrimaryField:
                        primaryFields.Add(entry);
                        break;

                    case AppleWalletExtendedFieldType.SecondaryField:
                        secondaryFields.Add(entry);
                        break;

                    case AppleWalletExtendedFieldType.AuxilliaryField:
                        auxilliaryFields.Add(entry);
                        break;

                    case AppleWalletExtendedFieldType.BackField:
                        backFields.Add(entry);
                        break;

                    case AppleWalletExtendedFieldType.HeaderField:
                        headerFields.Add(entry);
                        break;
                }
            }

            storeCardFields.Add("primaryFields", primaryFields.ToArray());
            if (secondaryFields.Count > 0)
                storeCardFields.Add("secondaryFields", secondaryFields.ToArray());
            if (auxilliaryFields.Count > 0)
                storeCardFields.Add("auxiliaryFields", auxilliaryFields.ToArray());
            if (backFields.Count > 0)
                storeCardFields.Add("backFields", backFields.ToArray());
            if (headerFields.Count > 0)
                storeCardFields.Add("headerFields", headerFields.ToArray());
            pass.Add("storeCard", storeCardFields);

            // serialize to JSON
            string result = JsonConvert.SerializeObject(pass, Formatting.Indented);
            return result;
        }

        private static string GetExpiryDateHeaderValue(DateTime expiryDate)
        {
            string result = string.Empty;
            DateTime now = DateTime.Now;
            if (DateTimeUtil.GreaterThan(now, expiryDate))
            {
                result = "expired";
            }
            else
            {
                long tickDiff = expiryDate.Ticks - now.Ticks;
                TimeSpan ts = TimeSpan.FromTicks(tickDiff);
                int numDays = ts.Days;
                int numHours = ts.Hours;
                if (numDays > 30)
                {
                    result = ">1 mo";
                }
                else if (numDays == 30)
                {
                    result = "1 mo";
                }
                else if (numDays >= 7)
                {
                    int numWeeks = (int)Math.Floor(numDays / 7d);
                    result = string.Format("{0} wk{1}", numWeeks, (numWeeks > 1 ? "s" : string.Empty));
                }
                else if (numDays >= 1)
                {
                    result = string.Format("{0} day{1}", numDays, (numDays > 1 ? "s" : string.Empty));
                }
                else if (numHours >= 1)
                {
                    result = string.Format("{0} hr{1}", numHours, (numHours > 1 ? "s" : string.Empty));
                }
                else
                {
                    result = "soon";
                }
            }
            return result;
        }

        private static byte[] SignManifest(string manifest)
        {
            string methodName = "SignManifest";

            byte[] result = null;

            X509Cert appleWWDRCACert;
            X509Cert signingCert;

            using (DataService dataService = LWDataServiceUtil.DataServiceInstance())
            {
                List<X509Cert> tmp = dataService.GetAllX509CertByCertType(X509CertType.AppleWWDRCACert);
                if (tmp == null || tmp.Count == 0)
                {
                    string msg = "Can't find the Apple WWDRCA Certificate that is required to generate passbook passes.";
                    _logger.Error(_className, methodName, msg);
                    throw new Exception(msg);
                    
                }
                appleWWDRCACert = tmp[0];

                List<X509Cert> certs = dataService.GetAllX509CertByCertType(X509CertType.PassbookSigningCert);
                if (certs == null || certs.Count == 0)
                {
                    string msg = string.Format("First please install a certificate for certificate type {0}", X509CertType.PassbookSigningCert);
                    _logger.Error(_className, methodName, msg);
                    throw new LWException(msg);
                }
                signingCert = certs[0];
            }

            byte[] signingCertBytes = CryptoUtil.Decode(signingCert.Value);
            string signingCertPassword = null;
            if (!string.IsNullOrEmpty(signingCert.CertPassword))
            {
                signingCertPassword = CryptoUtil.DecodeUTF8(signingCert.CertPassword);
            }
            X509Certificate2 signingCert2 = null;
            try
            {
                signingCert2 = new X509Certificate2(signingCertBytes, signingCertPassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error loading signing certificate.", ex);
                throw;
            }

            byte[] appleWWDRCACertBytes = CryptoUtil.Decode(appleWWDRCACert.Value);
            X509Certificate2 appleWWDRCACert2 = new X509Certificate2(appleWWDRCACertBytes);

            // sign it
            ContentInfo contentInfo = new ContentInfo(Encoding.UTF8.GetBytes(manifest));
            SignedCms signedCms = new SignedCms(contentInfo, true);
            var signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, signingCert2);
            signer.Certificates.Add(appleWWDRCACert2);
            signer.Certificates.Add(signingCert2);
            signer.IncludeOption = X509IncludeOption.None;
            signer.SignedAttributes.Add(new Pkcs9SigningTime());
            signedCms.ComputeSignature(signer, false);
            result = signedCms.Encode();

            return result;
        }

        private static string InsecureSHA1Hash(byte[] unhashedData)
        {
            if (unhashedData == null || unhashedData.Length < 1)
                throw new ArgumentException("PassbookUtil.SHA1(unhashedData): unhashedData is null or empty");

            // SHA1 is insecure so we only use this because apple requires it
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hashedData = sha.ComputeHash(unhashedData);

            string result = BitConverter.ToString(hashedData).ToLower().Replace("-", "");
            return result;
        }

        private static string InsecureSHA1Hash(string unhashedValue)
        {
            if (string.IsNullOrEmpty(unhashedValue))
                throw new ArgumentException("PassbookUtil.SHA1(unhashedValue): unhashedValue is null or empty");

            byte[] unhashedData = Encoding.UTF8.GetBytes(unhashedValue);

            return InsecureSHA1Hash(unhashedData);
        }

        private static AppleWalletLoyaltyCard GetCardForMember(Member member)
        {
            List<AppleWalletLoyaltyCard> cards;

            using (MobileDataService service = LWDataServiceUtil.MobileServiceInstance())
                cards = service.GetAllAppleWalletLoyaltyCard();

            foreach(AppleWalletLoyaltyCard awCard in cards)
            {
                if ((bool)ExpressionUtil.Create(awCard.LoyaltyCardSelectionCriteria).evaluate(new ContextObject() { Owner = member }))
                {
                    return awCard;
                }
            }
            return null;
        }

        private static string GetBscriptValue(string value, ContextObject contextObject)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
                return ExpressionUtil.Create(value).evaluate(contextObject).ToString();
        }

        private static void HashAndCopyFile(string file, string imageRoot, Dictionary<string, string> manifest, string passDir, string destFileName)
        {
            if (string.IsNullOrEmpty(file))
                return;

            // First check if it is a local file
            string fullPath = imageRoot + StringUtils.JSDecode(file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                byte[] contents = File.ReadAllBytes(fullPath);
                string hash = InsecureSHA1Hash(contents);
                manifest.Add(destFileName, hash);
                File.WriteAllBytes(passDir + Path.DirectorySeparatorChar + destFileName, contents);
            }
            else // Next check if it is a URL
            {
                using (var client = new System.Net.WebClient())
                {
                    byte[] contents = client.DownloadData(file);
                    string hash = InsecureSHA1Hash(contents);
                    manifest.Add(destFileName, hash);
                    File.WriteAllBytes(passDir + Path.DirectorySeparatorChar + destFileName, contents);
                }
            }
        }

        private static Location[] GetFavoriteStoreLatLongs(Member member)
        {
            using (ContentService contentService = LWDataServiceUtil.ContentServiceInstance())
            using (LoyaltyDataService loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                List<MemberStore> memberStores = loyaltyService.GetMemberStoresByMember(member.IpCode);

                if (memberStores == null || memberStores.Count == 0)
                    return null;

                long[] storeIds = (from s in memberStores select s.StoreDefId).ToArray();
                List<StoreDef> storeDefs = contentService.GetAllStoreDefs(storeIds);

                List<Location> points = new List<Location>();

                foreach (StoreDef store in storeDefs)
                {
                    // Make sure this is a valid location before adding it
                    if (store.Latitude.HasValue && Math.Abs(store.Latitude.Value) <= 90 &&
                        store.Longitude.HasValue && Math.Abs(store.Longitude.Value) <= 180)
                        points.Add(new Location()
                        {
                            latitude = store.Latitude.Value,
                            longitude = store.Longitude.Value
                        });

                    if (points.Count >= 10) // To maintain parity with Apple Wallet, limit favorited stores to 10
                        break;
                }

                return points.Count > 0 ? points.ToArray() : null;
            }
        }
    }
}
