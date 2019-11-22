using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Security;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

using Brierley.LoyaltyWare.ClientLib;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Client;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Framework;

namespace PilotMicroSite.Controllers
{
    public class Utilities
    {
        public Member member;
        public int errorCode = 0;
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// This method is used to call AE to authenticate the member if someone comes to the site and tries to 
        /// hit a page and is not authenticated on this side
        /// </summary>
        public string BuildAuthenticateURL()
        {
            string clientId = string.Empty;
            string url = "https://www.ae.com/web/flat/account_login.jsp?navdetail=top:utility:p3";
            Guid state = Guid.NewGuid();

            if (null == ConfigurationManager.AppSettings["ClientID"])
            {
                throw new Exception("ClientID not defined in web.config");
            }
            else
            {
                clientId = ConfigurationManager.AppSettings["ClientID"];
            }

            if (null == ConfigurationManager.AppSettings["Auth_URL"])
            {
                url = "https://www.ae.com/web/flat/account_login.jsp?navdetail=top:utility:p3";
                logger.Error("Auth_URL not defined in web.config");
            }
            else
            {
                url = ConfigurationManager.AppSettings["Auth_URL"];
                SaveToSession("State", "state", state.ToString());
                url = string.Concat(url, "?client_id=", clientId, "&state=", state.ToString());
            }

            return url;

        }

        public AEGetAccountSummaryOut GetMember(string loyaltyNumber)
        {
            try
            {
                string externalID = new Guid().ToString();
                double elapsedTime = 0;
                string[] searchType = new string[1];
                string[] searchValue = new string[1];

                LWIntegrationSvcClientManager svcMgr = GetServiceManager();

                errorCode = 0;
                member = null;
                searchType[0] = "CardID";
                searchValue[0] = loyaltyNumber;

                logger.Debug("Using GetMembers for LoyaltyNumber: " + loyaltyNumber);

                Member[] members = svcMgr.GetMembers(searchType, searchValue, 1, 100, externalID, out elapsedTime);

                //member = svcMgr.AEGetMember(loyaltyNumber, externalID, out elapsedTime);
                logger.Debug("Successful call from GetMembers");


                if (members.Length > 0)
                {
                    member = members[0];
                }


                AEGetAccountSummaryOut acctSummary = svcMgr.AEGetAccountSummary(loyaltyNumber, externalID, out elapsedTime);

                return acctSummary;


            }
            catch (LWClientException ex)
            {
                errorCode = ex.ErrorCode;
                logger.Error(string.Format("LoyaltyNumber: {0} - Error: {1}", loyaltyNumber, ex.Message));
                member = null;
                return null;
            }


        }
        public LWIntegrationSvcClientManager GetServiceManager()
        {
            string url = string.Empty;
            LWIntegrationSvcClientManager svcMgr = null;

            try
            {
                if (null == ConfigurationManager.AppSettings["API_URL"])
                {
                    throw new Exception("No path defined in web.config");
                }
                else
                {
                    url = ConfigurationManager.AppSettings["API_URL"];
                }


                svcMgr = new LWIntegrationSvcClientManager(url, "Microsite", true, string.Empty);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Error in getting Service Manager: {0} ", ex.Message);
                logger.Error(errorMessage);
                throw new Exception(errorMessage);
            }
            return svcMgr;

        }
        /// <summary>
        /// This method is used to store the state and logoutURL. The state is used to send to the client to prevent cross site scripting and the logout url is used for the Sign Out link
        /// </summary>
        /// <param name="sessionName">
        /// Session name and Cookie name
        /// </param>
        /// <param name="sessionValueName">
        /// Cookie value name
        /// </param>
        /// <param name="sessionValue">
        /// Value of the cookie
        /// </param>
        public void SaveToSession(string sessionName, string sessionValueName, string sessionValue)
        {
            HttpContext.Current.Session[sessionValueName] = sessionValue;

            //create a cookie
            HttpCookie myCookie = new HttpCookie(sessionName);

            //Add key-values in the cookie
            myCookie.Values.Add(sessionValueName, sessionValue);

            //set cookie expiry date-time. Made it to last for next 10 minutes.
            myCookie.Expires = DateTime.Now.AddMinutes(10);

            //Most important, write the cookie to client.
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

    }
}