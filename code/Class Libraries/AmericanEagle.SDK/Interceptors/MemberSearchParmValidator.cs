using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
//LW 4.1.14 change
//using Brierley.DNNModules.MemberSearch;
using Brierley.LWModules.MemberSearch;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using System.Collections.Generic;
using System.Reflection;
using Brierley.FrameWork.Common.Logging;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.WebFrameWork.Controls;

namespace AmericanEagle.SDK.Interceptors
{
    /// <summary>
    /// Validate parameters for member search
    /// </summary>
    public class MemberSearchParmValidator : ModuleControlBase, IMemberSearchParmValidator
    {
        /// <summary>        
        /// Validate parameters for member search
        /// Truth table for validation
        /// LN  FN  PC  Exception
        /// 1   1   1   False   
        /// 1   1   0   False
        /// 1   0   0   False
        /// 0   0   0   True
        /// 0   1   0   True
        /// 0   0   1   True
        /// 0   1   1   False
        /// Legends LN:LoyaltyIDNumber, FN:First Name, PC:Postal Code
        /// Parameters should be LoyaltyIDNumber, FirstName, PrimaryPostalCode
        /// </summary>
        /// <param name="parms"></param>
        /// 
        private const string ClassName = "MemberSearchParmValidator";
        private LWLogger logger = LWLoggerManager.GetLogger(ClassName);
        public virtual void ValidateParameters(NameValueCollection parms)
        {
            String strLoyaltyIDNumber = String.Empty;
            String strLastName = String.Empty;
            String strPrimaryPostalCode = String.Empty;

            strLoyaltyIDNumber = parms["LoyaltyIDNumber"];
            strLastName = parms["LastName"];
            strPrimaryPostalCode = parms["PrimaryPostalCode"];

            if (String.IsNullOrEmpty(strLoyaltyIDNumber) && String.IsNullOrEmpty(strLastName) && String.IsNullOrEmpty(strPrimaryPostalCode))
            {
                throw new Exception("AEMessage|CustomerSearch|Please enter search criteria.");
            }
            if (String.IsNullOrEmpty(strLoyaltyIDNumber))
            {
                if ((String.IsNullOrEmpty(strLastName)) || (String.IsNullOrEmpty(strPrimaryPostalCode)))
                {
                    throw new Exception("AEMessage|CustomerSearch|Please enter Zip/Postal Code along with Last Name!.");
                }
            }
        }
        // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
        public virtual void ValidateParameters(List<MemberSearchConfiguration.SearchField> parms)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            String strLoyaltyIDNumber = String.Empty;
            String strLastName = String.Empty;
            String strPrimaryPostalCode = String.Empty;
            String strEmail = string.Empty;
            String strMobile = string.Empty;

            foreach (MemberSearchConfiguration.SearchField items in parms)
            {

                if (items.FieldName == "LoyaltyIDNumber")
                {
                    if (items.FieldValue != null)
                    {
                        strLoyaltyIDNumber = items.FieldValue.ToString();

                    }
                    else
                        strLoyaltyIDNumber = String.Empty;

                }
                if (items.FieldName == "LoyaltyIDNumber")
                {
                    if (items.FieldValue != null)
                    {
                        strLoyaltyIDNumber = items.FieldValue.ToString();

                    }
                    else
                        strLoyaltyIDNumber = String.Empty;

                }

                if (items.FieldName == "PrimaryPostalCode")
                {
                    if (items.FieldValue != null)
                    {
                        strPrimaryPostalCode = items.FieldValue.ToString();


                    }
                    else
                    {
                        strPrimaryPostalCode = String.Empty;
                    }
                }
                if (items.FieldName == "LastName")
                {
                    if (items.FieldValue != null)
                    {
                        strLastName = items.FieldValue.ToString();

                    }
                    else
                    {
                        strLastName = String.Empty;
                    }
                }

                /* AEO-1333 begin */
                if ( items.FieldName == "EmailAddress" ) {
                    if ( items.FieldValue != null ) {
                        strEmail = items.FieldValue.ToString();

                    }
                    else
                        strEmail = String.Empty;

                }

                if ( items.FieldName == "MobilePhone" ) {
                    if ( items.FieldValue != null ) {
                        strMobile = items.FieldValue.ToString();

                    }
                    else
                        strMobile = String.Empty;

                }
                /* AEO-1333 end */

            }

            /* AEO-1333 begin */

            if ( String.IsNullOrEmpty(strLoyaltyIDNumber) && String.IsNullOrEmpty(strLastName) &&
                String.IsNullOrEmpty(strPrimaryPostalCode) && String.IsNullOrEmpty(strEmail) &&
                String.IsNullOrEmpty(strMobile) )  /* AEO-1333 begin & end */ {

                ShowWarning("AEMessage|CustomerSearch|Please enter search criteria.");
                return;
            }

            else {

                if ( String.IsNullOrEmpty(strLoyaltyIDNumber) ) {
                    if ( ( String.IsNullOrEmpty(strLastName) ) || ( String.IsNullOrEmpty(strPrimaryPostalCode) ) ) {
                        if ( string.IsNullOrEmpty(strEmail) && string.IsNullOrEmpty(strMobile) ) {
                            ShowWarning("AEMessage|CustomerSearch|Please enter a valid search criteria.");
                            return;

                        }
                    }

                }
            
            }
            /* AEO-1333 end */
            
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
        }
    }
}
