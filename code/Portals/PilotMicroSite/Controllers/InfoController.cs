using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;

using Brierley.LoyaltyWare.ClientLib;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Client;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Framework;

namespace PilotMicroSite.Controllers
{
    public class InfoController : Controller
    {
        //
        public Utilities utils = new Utilities();

        public ActionResult ProgramDetails()
        {

            string loyaltyNumber = CheckForCookie();
            if (utils.member != null)
            {
                ViewBag.MemberName = utils.member.FirstName + "'s";
            }

            return View();
        }

        public ActionResult Program()
        {
            string url = string.Empty;
            url = utils.BuildAuthenticateURL();
            ViewBag.SignInURL = url;

            return View();
        }

        public ActionResult ProgramTerms()
        {

            return View();
        }

        public ActionResult ProgramFAQ()
        {

            return View();
        }

        public ActionResult FAQ()
        {
            string loyaltyNumber = CheckForCookie();
            if (utils.member != null)
            {
                ViewBag.MemberName = utils.member.FirstName + "'s";
            }
            return View();
        }

        public ActionResult Terms()
        {
            string loyaltyNumber = CheckForCookie();
            if (utils.member != null)
            {
                ViewBag.MemberName = utils.member.FirstName + "'s";
            }
            return View();
        }
        private string CheckForCookie()
        {
            string loyaltyNumber = string.Empty;

            // Retrieves the cookie that contains your custom FormsAuthenticationTicket.
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null)
            {
                loyaltyNumber = string.Empty;
            }
            else
            {
                // Decrypts the FormsAuthenticationTicket that is held in the cookie's .Value property.
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                loyaltyNumber = authTicket.UserData;
                AEGetAccountSummaryOut acctSummary = utils.GetMember(loyaltyNumber);
            }

            return loyaltyNumber;
        }

    }
}
