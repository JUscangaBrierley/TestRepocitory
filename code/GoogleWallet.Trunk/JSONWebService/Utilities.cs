using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;

//using Brierley.FrameWork.Data;
//using Brierley.FrameWork.Data.DomainModel;
//using Brierley.FrameWork.Common.Logging;
//using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.LoyaltyWare.ClientLib;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Client;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Framework;
//using AmericanEagle.SDK.GridProvider;
using Brierley.FrameWork.Common;

namespace TestPOSTWebService  
{
    public static class Utilities
    {
       // private static LWLogger logger = LWLoggerManager.GetLogger("Utilities");
      // private static IList<MemberBraPromoSummary> lstMemberBraPromoSummary;
       // private static IList<MemberBraPromoCertSummary> lstMemberBraPromoCertSummary;
       // private static IList<MemberBraPromoCertHistory> lstMemberBraPromoCertHistory;
      //  private static IList<MemberBraPromoCertRedeem> lstMemberBraPromoCertRedeem;
        

        //public static IList<IClientDataObject> GetGlobalAttributeSet(ILWDataService dataService, string attributeSetName)
        //{
        //    string region = "AmericanEagleAttributeSets";
        //    IList<IClientDataObject> attributeSet = (IList<IClientDataObject>)dataService.CacheManager.Get(region, attributeSetName);
        //    if (attributeSet == null)
        //    {
        //        attributeSet = (IList<IClientDataObject>)dataService.GetAttributeSetObjects(null, attributeSetName, null, new LWQueryBatchInfo(), false);
        //        dataService.CacheManager.Update(region, attributeSetName, attributeSet);
        //    }
        //    return attributeSet;
        //}

        //LW 4.1.14 removed
        //LW 4.1.14 removed

        public static void GetQuarterDates(out DateTime startDate, out DateTime endDate)
        {
            int startMonth = 0;
            int startYear = DateTime.Now.Year;
            int startDay = 1;
            int endMonth = 0;
            int endYear = DateTime.Now.Year;
            int endDay = 31;

            string quarter = GetQuarter(DateTime.Now.Month);
            switch (quarter)
            {
                case "Q1":
                    startMonth = 1;
                    endMonth = 3;
                    endDay = 31;
                    break;

                case "Q2":
                    startMonth = 4;
                    endMonth = 6;
                    endDay = 30;
                    break;

                case "Q3":
                    startMonth = 7;
                    endMonth = 9;
                    endDay = 30;
                    break;

                case "Q4":
                    startMonth = 10;
                    endMonth = 12;
                    break;
            }

            startDate = new DateTime(startYear, startMonth, startDay);
            endDate = new DateTime(endYear, endMonth, endDay);
        }

        private static string GetQuarter(int month)
        {
            string quarter = string.Empty;

            if (month >= 1 && month <= 3)
            {
                quarter = "Q1";
            }
            if (month >= 4 && month <= 6)
            {
                quarter = "Q2";
            }
            if (month >= 7 && month <= 9)
            {
                quarter = "Q3";
            }
            if (month >= 10 && month <= 12)
            {
                quarter = "Q4";
            }

            return quarter;
        }
        /// <summary>
        /// Returns total points gained by a member on selected quarter 
        /// </summary>
        /// <returns></returns>
        


        /// <summary>
        /// Use to Validate Name
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsNameValid(string param)
        {
            Regex regEx = new Regex(@"^[a-z|A-Z|,|.|\-|'|\s]{1,50}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate City
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsCityValid(string param)
        {
            Regex regEx = new Regex(@"^[a-z|A-Z|,|.|'|\s]{1,50}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate Address
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsAddressValid(string param)
        {
            Regex regEx = new Regex(@"^[a-z|A-Z|0-9|,|.|%|#|\-|/|\s]{0,50}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Use to Validate BaseBrand
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsBaseBrandValid(string param)
        {
            Regex regEx = new Regex(@"^(0|00|20|50)$", RegexOptions.IgnoreCase);
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Use to Validate State
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>

        /// <summary>
        /// Use to Validate PostalCode
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsPostalCodeValid(string param)
        {
            Regex regEx = new Regex(@"(^\d{5}-\d{4}|\d{5}|\d{9})$|(^[ABCEGHJKLMNPRSTVXY]{1}\d{1}[A-Z]{1} *\d{1}[A-Z]{1}\d{1}$)");

            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate CountryCode
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsCountryCodeValid(string param)
        {
            Regex regEx = new Regex(@"^(USA|CAN)$", RegexOptions.IgnoreCase);
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate Email
        /// </summary>
        /// <param name="emailAddress">String to be validated</param>
        /// <returns>status of Vaidation</returns>
        public static bool IsEmailValid(string emailAddress)
        {
            if (emailAddress.Length > 255)
            {
                return false;
            }

            if (emailAddress.ToLower().StartsWith("www"))
            {
                return false;
            }

            Regex regEx = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            if (!regEx.Match(emailAddress).Success)
            {
                return false;
            }

            if (emailAddress.Substring(emailAddress.IndexOf('@') - 1, 1) == ".")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Matches Input FirstName and LastName with the mached Firstname and lastName
        /// from Brierly database.
        /// </summary>
        /// <param name="member">Instance of member found based on LoyaltyNumberID</param>
        /// <param name="firstName">FirstName provided as input</param>
        /// <param name="lastName">LastName provided as input</param>
        /// <returns>Validation Status</returns>
        /// <summary>
        /// Use to Validate Phone
        /// Allowed - 10 digits. No dashes or parenthesis
        /// Range - 0 to 10 character
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsPhoneValid(string param)
        {
            Regex regEx = new Regex(@"^[0-9]{0,10}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate Gender
        /// Numeric:
        /// Unknown = 0
        /// Male = 1
        /// Female = 2
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsGenderValid(string param)
        {
            Regex regEx = new Regex(@"^[0-2]{0,1}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use to Validate LanguagePreference
        /// Numeric:
        /// English = 0
        /// Spanish = 1
        /// French = 2
        /// </summary>
        /// <param name="param">String to be validated</param>
        /// <returns>status of Validation</returns>
        public static bool IsLanguagePreferenceValid(string param)
        {
            Regex regEx = new Regex(@"^[0-2]{0,1}$");
            if (!regEx.Match(param).Success)
            {
                return false;
            }

            return true;
        }



        #region Other Methods [Added By Manoj]

        /// <summary>
        /// Returns Loyalty ID numnber of a selected member
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Returns the name of the selected member
        /// </summary>
        /// <returns></returns>

  
        #endregion

        
    }
}
