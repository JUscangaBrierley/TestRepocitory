using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
//using Brierley.WebFrameWork.ErrorHandling;
using Brierley.WebFrameWork.MessageHandling;

using Brierley.FrameWork.Common.Logging;
//using Brierley.FrameWork.Common.Exceptions;
// AEO-74 Upgrade 4.5 changes END here -----------SCJ
using AmericanEagle.SDK.Global;


namespace AmericanEagle.SDK.ErrorMappers
{
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //public class AEErrorMapper : IErrorMapper IMessageMapper
        public class AEErrorMapper :  IMessageMapper
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    {
        private const string _appName = "AmericanEagleCSPortal";
        private const string _className = "AEErrorMapper";
        private const string _stdMessage = "AEMessage";
        private const string _defaultMessage = "We're sorry, but an unexpected error has occurred";
        private static LWLogger _logger = null;

        static AEErrorMapper()
        {
            // Initialize logging
            _logger = LWLoggerManager.GetLogger(_appName);
        }

        #region IErrorMapper Members
        private string ParseErrorMessage(string errorMessage, out string pageName, out bool IsFriendlyMessage)
        {
            string returnMessage = _defaultMessage;
            
            IsFriendlyMessage = false;
            pageName = string.Empty;

            //If the message if already a friendly message then parse out the necessary information and then
            //return the friendly message.
            if (errorMessage.StartsWith(_stdMessage))
            {
                IsFriendlyMessage = true;
          
                returnMessage = errorMessage.Substring(_stdMessage.Length + 1);
                int position = returnMessage.IndexOf('|');
            
                if (position > 0)
                {
                    pageName = returnMessage.Substring(0, position);
                    returnMessage = returnMessage.Substring(position + 1);        
                }
            }
            else if (errorMessage.Contains("User Name is required"))
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("Provided password is incorrect."))
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("Login credentials not valid"))
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("User Not Authenticated"))
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("Password is required"))
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("Please enter your Password."))  //AEO-74 Upgrade 4.5 changes END here -----------SCJ
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("Cannot set password to expire"))
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("Password must"))
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("New password cannot"))
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("No more reward certificates"))
            {
                IsFriendlyMessage = true;
                returnMessage = "There are no more reward certificates for this reward";
            }
            else if (errorMessage.Contains("No more certificates")) //AEO-74 Upgrade 4.5 changes END here -----------SCJ
            {
                IsFriendlyMessage = true;
                returnMessage = "There are no more reward certificates for this reward";
            }
            else if (errorMessage.Contains("An agent already exists with this username"))
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("Account locked") || errorMessage.Contains("CSAgent is Locked") || errorMessage.Contains("Account Locked"))
            {
                IsFriendlyMessage = true;
                returnMessage = "Login credentials invalid. Your account is now locked. Please contact an administrator to reactivate your account.";
            }
            else if (errorMessage.Contains("Unable to find the CSAgent"))
            {
                IsFriendlyMessage = true;
                returnMessage = "Login credentials invalid. Unable to find the CSAgent.";
            }
            else if (errorMessage.Contains("CSAgent is InActive")) // AEO-28 capture CS Agent InActive error
            {
                IsFriendlyMessage = true;
                returnMessage = "Your account is not active. Please contact your administrator.";
            }
            else if (errorMessage.Contains("Your profile has been updated.")) //AEO-74 Upgrade 4.5 changes END here -----------SCJ
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }

            else if (errorMessage.Contains("CSR agent")) //AEO-74 Upgrade 4.5 changes END here -----------SCJ
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("CSR agent")) //AEO-74 Upgrade 4.5 changes END here -----------SCJ
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            else if (errorMessage.Contains("Password changed")) //AEO-74 Upgrade 4.5 changes END here -----------SCJ
            {
                IsFriendlyMessage = true;
                returnMessage = errorMessage;
            }
            return returnMessage;
        }
        /// <summary>
        /// Maps the error message when an exception is thrown.  An error handling
        /// context is provided that contains information such as the exception, the
        /// friendly error message to be shown to the user, and the type of alert to
        /// be used, and whether a stack trace will be shown to the user.  All of these
        /// items can be modified in this method so that the error display can be
        /// customized as desired by the application.
        /// </summary>
        /// <param name="context">error handling context</param>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public void MapError(IErrorHandlingContext context)
        public void MapMessage(IMessageHandlingContext context)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            string methodName = "MapError";
            string friendlyMessage = string.Empty;
            string pageName = _className;
            bool isFriendlyMessage = false;

            // This is how to get the current user name if needed
            string userName = "(anonymous user)";
            userName = WebUtilities.GetCurrentUserName();


            // Parse the error message coming in to determine if the message is already a friendly message and
            // we just need to just parse out the page name and then return that message
            string incomingMessage = string.Empty;
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //if (context.Exception == null)
            if (context.Message == null)
            {
                //  incomingMessage = context.FriendlyMessage; AEO-74 Upgrade 4.5 changes here -----------SCJ
                incomingMessage = context.Message;
                
            }
            else
            {

                //incomingMessage = context.Exception.Message;       AEO-74 Upgrade 4.5 changes here -----------SCJ
                incomingMessage = context.Message;
               
            }
            friendlyMessage = ParseErrorMessage(incomingMessage, out pageName, out isFriendlyMessage);
            //context.FriendlyMessage = friendlyMessage;
            context.Message = friendlyMessage;

            //Do not log any of the friendly messages.  Usually field validation messages.
            if (!isFriendlyMessage)
            {
                // Log the message
                if (_logger != null)
                {
                    string msg = string.Format("User = {0}", userName);
                    _logger.Error(pageName, methodName, msg, new Exception(context.Message));
                }
            }
        }

        #endregion
    }
    
}
