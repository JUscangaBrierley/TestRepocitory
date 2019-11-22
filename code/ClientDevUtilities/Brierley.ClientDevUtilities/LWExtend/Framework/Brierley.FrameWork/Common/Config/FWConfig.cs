using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.Common.Config
{
    public class FWConfig
    {
		public const string DEFAULT_P3P_POLICY = @"CP=""IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT""";
        private string _orgName;
        private string _envName;
        private Dictionary<string, string> _fwConfigProperties = new Dictionary<string, string>();

        public const string LCAPDefaultCurrency = "LoyaltyCurrencyAsPayment DefaultCurrency";
        public const string LCAPMinPoints = "LoyaltyCurrencyAsPayment MinimumPoints";
        public const string LCAPMaxPoints = "LoyaltyCurrencyAsPayment MaximumPoints";
        public const string LCAPMinMoney = "LoyaltyCurrencyAsPayment MinimumMonetaryValue";
        public const string LCAPMaxMoney = "LoyaltyCurrencyAsPayment MaximumMonetaryValue";
        public const string LCAPMinPercent = "LoyaltyCurrencyAsPayment MinimumMonetaryPercent";
        public const string LCAPMaxPercent = "LoyaltyCurrencyAsPayment MaximumMonetaryPercent";
        public const string LCAPMaxExchangeRateAge = "LoyaltyCurrencyAsPayment MaximumExchangeRateAge";
        public const string LCAPAssemblyName = "LoyaltyCurrencyAsPayment AssemblyName";
        public const string LCAPProviderClassName = "LoyaltyCurrencyAsPayment ClassName";

        public FWConfig()
        {
            LoadDefaultProperties();
        }

        public FWConfig(string fwConfigXml)
        {
            LoadDefaultProperties();
            LoadFWConfig(fwConfigXml);
        }

		public FWConfig(string fwConfigXml, string orgName, string envName)
		{
			_orgName = orgName;
			_envName = envName;
			LoadDefaultProperties();
			LoadFWConfig(fwConfigXml);
		}

        public string Serialize()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            tmp.AppendLine("<LWConfiguration xmlns=\"http://brierley.schemas.com/LWConfiguration\"");
            tmp.AppendLine("   xsi:schemaLocation=\"http://brierley.schemas.com/LWConfiguration file:///C:/Temp/XMLTest/LWConfiguration.xsd\"");
            tmp.AppendLine("   xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
            tmp.AppendLine("   Organization=\"" + _orgName + "\" Environment=\"" + _envName + "\">");
            foreach (string name in _fwConfigProperties.Keys)
            {
                string value = _fwConfigProperties[name];
				value = value.Replace("\"", "&quot;");
                tmp.AppendLine("   <ConfigurationEntry Name=\"" + name + "\" Value=\"" + value + "\"/>");
            }
            tmp.AppendLine("</LWConfiguration>");

            return tmp.ToString();
        }

        public Dictionary<string, string> GetFWConfigProperties()
        {
            return _fwConfigProperties;
        }

        public string GetFWConfigProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            string result = null;
            if (_fwConfigProperties.ContainsKey(propertyName))
            {
                result = _fwConfigProperties[propertyName];
            }
            return result;
        }

        public void SetFWConfigProperty(string propertyName, string propertyValue)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            if (propertyValue == null)  // "" is OK
                throw new ArgumentNullException("propertyValue");

            propertyName = propertyName.Trim();
            propertyValue = propertyValue.Trim();
            if (_fwConfigProperties.ContainsKey(propertyName))
            {
                _fwConfigProperties[propertyName] = propertyValue;
            }
            else
            {
                _fwConfigProperties.Add(propertyName, propertyValue);
            }
        }

        public void DeleteFWConfigProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            propertyName = propertyName.Trim();
            if (_fwConfigProperties.ContainsKey(propertyName))
            {
                _fwConfigProperties.Remove(propertyName);
            }
            else
            {
                throw new LWException("Property '" + propertyName + "' not found");
            }
        }

        public string OrgName
        {
            get { return _orgName; }
            set { _orgName = value; }
        }

        public string EnvName
        {
            get { return _envName; }
            set { _envName = value; }
        }

        private void LoadDefaultProperties()
        {
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            if (ctx != null)
            {
                if (string.IsNullOrEmpty(_orgName) && !string.IsNullOrEmpty(ctx.Organization)) _orgName = ctx.Organization;
                if (string.IsNullOrEmpty(_envName) && !string.IsNullOrEmpty(ctx.Environment)) _envName = ctx.Environment;
            }

			_fwConfigProperties.Add("AllowDuplicateProductPartNumbers", "true");

            _fwConfigProperties.Add(LWConfiguration.CONFIGNAME_KEYSTORE_PASS, "");
            _fwConfigProperties.Add(LWConfiguration.CONFIGNAME_KEYSIZE, "");
			_fwConfigProperties.Add("LWSurveyURL", "set to survey website url");

            //_fwConfigProperties.Add("LW_JavaLibs", "");            
            _fwConfigProperties.Add("LW_DAPProcessNonMembers", "false");
            _fwConfigProperties.Add("LW_NonMemberPointGracePeriod", "0");
            //_fwConfigProperties.Add("LW_ConvertEmailToUpper", "false"); // LW-1320

            // Reward catalog related
            _fwConfigProperties.Add("LW_FulfillmentProvider", "MMS");
            _fwConfigProperties.Add("LW_CreateProductsInFulfillmentProvider", "false");

            _fwConfigProperties.Add("MessagingEnabled", "true");

            _fwConfigProperties.Add("LW_PointAccrualDebitPayoff", "false");
            _fwConfigProperties.Add("LW_DebitPayoffPointTypeRestriction", "false"); // LW-1301
            _fwConfigProperties.Add("LW_DebitPayoffPointEventRestriction", "false"); // LW-1301

            _fwConfigProperties.Add("PassBookTeamID", "get this from apple.");
            _fwConfigProperties.Add("PassBookWebServiceURL", "get this from apple.");
            _fwConfigProperties.Add("PassBookSigningOrgName", "get this from apple.");

			// in practice this will not be a virtual directory
			//_fwConfigProperties.Add("LoyaltyCardPassGeneratorURL", string.Format("http://{0}/LWMobileGateway/MobileGateway.svc/Passbook/GetPassFromMTouch?type=card", System.Net.Dns.GetHostName()));
            _fwConfigProperties.Add("LoyaltyCardPassGeneratorURL", string.Format("http://{0}/MobileGateway.svc/Passbook/GetPassFromMTouch?type=card", System.Net.Dns.GetHostName()));

            _fwConfigProperties.Add("SurveyDefaultAwardPointRule", "Default business rule for survey points.");

			_fwConfigProperties.Add("LogTriggeredEmails", "false");
            _fwConfigProperties.Add("LogTriggeredSmsMessages", "false");

			_fwConfigProperties.Add("EnableCloudStorage", "false");
			_fwConfigProperties.Add("CloudProvider", "AmazonAws");
            _fwConfigProperties.Add("CloudBucketName", _orgName);
            _fwConfigProperties.Add("CloudEndpointName", "USEast1");
			_fwConfigProperties.Add("AwsStorageAccessKey", "get this from Amazon");
			_fwConfigProperties.Add("AwsStorageSecretKey", "get this from Amazon");

			//LW-3715 Remove Mutual Minds references

            _fwConfigProperties.Add("SmtpServer", "cypwebmail.brierleyweb.com");

            _fwConfigProperties.Add("PromoTestOnlineSetSize", "100");

			_fwConfigProperties.Add("P3PEnabled", "true");
			_fwConfigProperties.Add("P3PPolicy", DEFAULT_P3P_POLICY);

			_fwConfigProperties.Add(Brierley.FrameWork.Dmc.Constants.DmcUrl, string.Empty);
			_fwConfigProperties.Add(Brierley.FrameWork.Dmc.Constants.DmcUsername, string.Empty);
			_fwConfigProperties.Add(Brierley.FrameWork.Dmc.Constants.DmcPassword, string.Empty);
			_fwConfigProperties.Add(Brierley.FrameWork.Dmc.Constants.DmcUseAlternateEmail, "false");
            _fwConfigProperties.Add(Brierley.FrameWork.Dmc.Constants.DmcUseAlternateMobile, "false");

            _fwConfigProperties.Add("DmcSmsEnabled", "true");
            _fwConfigProperties.Add("DmcSmsDoubleOptIn", "true");

            _fwConfigProperties.Add("EncryptionBaseURL", string.Empty);

            _fwConfigProperties.Add(Brierley.FrameWork.Push.Constants.GCMSenderID, string.Empty);
            _fwConfigProperties.Add(Brierley.FrameWork.Push.Constants.GCMAuthToken, string.Empty);
            _fwConfigProperties.Add(Brierley.FrameWork.Push.Constants.APNSPushCertName, string.Empty);

            _fwConfigProperties.Add("APIssuerID", "get this from Google");
            _fwConfigProperties.Add("APServiceAccountEmailAddress", "Google API service account email address ending in '@developer.gserviceaccount.com'");
            _fwConfigProperties.Add("APSigningCertificateName", "");

            _fwConfigProperties.Add("AWLoyaltyCardPassTypeID", "");

            _fwConfigProperties.Add("LWEmailProvider", "");
            _fwConfigProperties.Add("AwsRegion", "");
            _fwConfigProperties.Add("AwsEmailAccessKey", "");
            _fwConfigProperties.Add("AwsEmailSecretKey", "");

			//email feedback/bounce rules
			_fwConfigProperties.Add(Email.Constants.PermanentRule, EmailBounceRuleType.Strict.ToString());
			_fwConfigProperties.Add(Email.Constants.ComplaintRule, EmailBounceRuleType.Strict.ToString());

			_fwConfigProperties.Add(Email.Constants.UndeterminedRule, EmailBounceRuleType.Sliding.ToString());
			_fwConfigProperties.Add(Email.Constants.UndeterminedLimit, "1");
			_fwConfigProperties.Add(Email.Constants.UndeterminedInterval, "30");

			_fwConfigProperties.Add(Email.Constants.TransientRule, EmailBounceRuleType.Sliding.ToString());
			_fwConfigProperties.Add(Email.Constants.TransientLimit, "1");
			_fwConfigProperties.Add(Email.Constants.TransientInterval, "30");

            // Loyalty Currency as Payment
            _fwConfigProperties.Add(LCAPDefaultCurrency, "");
            _fwConfigProperties.Add(LCAPMinPoints, "");
            _fwConfigProperties.Add(LCAPMaxPoints, "");
            _fwConfigProperties.Add(LCAPMinMoney, "");
            _fwConfigProperties.Add(LCAPMaxMoney, "");
            _fwConfigProperties.Add(LCAPMinPercent, "0.1");
            _fwConfigProperties.Add(LCAPMaxPercent, "1.0");


            _fwConfigProperties.Add(LCAPMaxExchangeRateAge, "");
            _fwConfigProperties.Add(LCAPAssemblyName, "");
            _fwConfigProperties.Add(LCAPProviderClassName, "");
        }

		private void LoadFWConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode rootNode = doc.DocumentElement;
            if (rootNode.LocalName == "LWConfiguration" && rootNode.HasChildNodes)
            {
                _orgName = StringUtils.FriendlyString(rootNode.Attributes["Organization"].Value, _orgName);
                _envName = StringUtils.FriendlyString(rootNode.Attributes["Environment"].Value, _envName);
                foreach (XmlNode entryNode in rootNode)
                {
                    if (entryNode.NodeType == XmlNodeType.Element && entryNode.LocalName == "ConfigurationEntry")
                    {
                        string propName = entryNode.Attributes["Name"].Value;
                        string propValue = entryNode.Attributes["Value"].Value;
						propValue = propValue.Replace("&quot;", "\"");
                        if (_fwConfigProperties.ContainsKey(propName))
                        {
                            _fwConfigProperties[propName] = propValue;
                        }
                        else
                        {
                            _fwConfigProperties.Add(propName, propValue);
                        }
                    }
                }
            }
        }
    }
}
