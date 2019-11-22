using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace Brierley.FrameWork.Common.Exceptions
{
    /// <summary>
    /// Provides policy based handling of exceptions and a uniform method 
    /// for user notification within ASP.NET web applications. 
    /// </summary>
    public class ExceptionManager
    {
        #region Public Methods

        /// <summary>
        /// Handle an exception.
        /// </summary>
        /// <param name="exception">The exception that is to be handled.</param>
        /// <returns>True if the exception should be rethrown.</returns>
        public static bool HandleException(Exception exception)
        {
            return HandleException(exception, "Global Policy", null);
        }

        /// <summary>
        /// Handle an exception with hook for user notification.
        /// </summary>
        /// <param name="exception">The exception that is to be handled.</param>
        /// <param name="lblAlert">A <see cref="T:System.Web.UI.WebControls.Label"/> that will be 
        /// populated with JavaScript to handle user notification.</param>
        /// <returns>True if the exception should be rethrown.</returns>
        public static bool HandleException(Exception exception, Label lblAlert)
        {
            return HandleException(exception, "Global Policy", lblAlert);
        }

        /// <summary>
        /// Handle an exception using the given exception policy name with hook for user notification.
        /// </summary>
        /// <param name="exception">The exception that is to be handled.</param>
        /// <param name="policyName">The name of an exception policy that should be applied for this 
        /// exception.  This should be defined in the Web.Config or App.Config.</param>
        /// <param name="lblAlert">A <see cref="T:System.Web.UI.WebControls.Label"/> that will be 
        /// populated with JavaScript to handle user notification.</param>
        /// <returns>True if the exception should be rethrown.</returns>
        public static bool HandleException(Exception exception, string policyName, Label lblAlert)
        {
            // Apply the policy for this exception
            bool result = ApplyExceptionPolicy(exception, policyName);

            // Handle user notification
            if (result == true && lblAlert != null)
            {
                lblAlert.Text = "<script language=JavaScript>{radalert('" + exception.Message + "', 330, 210, 'Error');}</script>";
                result = false;
            }
            return result;
        }

        #endregion

        #region Private Methods

        private static bool ApplyExceptionPolicy(Exception exception, string policyName)
        {
            try
            {
                return ExceptionPolicy.HandleException(exception, policyName);
            }
            catch (ExceptionHandlingException)
            {
                // The policy is not defined in Web.Config or App.Config, so force rethrow.
                return true;
            }
        }

        #endregion
    }
}
