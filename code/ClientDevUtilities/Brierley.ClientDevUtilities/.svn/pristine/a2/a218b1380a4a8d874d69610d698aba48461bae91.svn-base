using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	[Serializable]
	[ExpressionContext(Description = "The URL at which the survey is taken by end users, as defined in web.config.",
		DisplayName = "SurveyURL",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Strings)]
    public class SurveyURL : UnaryOperation
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SurveyURL()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public SurveyURL(Expression rhs)
            : base("SurveyURL", rhs)
        {
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "SurveyURL(['MTouch'])";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">An instance of ContextObject</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            // mtouch argument is optional
            string mtouch = string.Empty;
            try
            {
                mtouch = GetRight().evaluate(contextObject).ToString();
            }
            catch
            {
                // ignore missing or invalid expression
                mtouch = string.Empty;
            }
            string encryptedMTouch = string.Empty;
            try
            {
                if (mtouch.Contains("##"))
                    encryptedMTouch = mtouch;  // email field
                else
                    encryptedMTouch = CryptoUtil.EncryptMTouch(mtouch);

                if (encryptedMTouch == null) encryptedMTouch = string.Empty;
            }
            catch
            {
                // treat invalid mtouch like missing one
                encryptedMTouch = string.Empty;
            }

            // Get configured survey URL
            string surveyURL = ConfigurationManager.AppSettings["LWSurveyURL"];
            if (string.IsNullOrEmpty(surveyURL))
                throw new CRMException("LWSurveyURL property not defined.");

            // Add mtouch query param, invalid url throws exception
            Uri url = new Uri(surveyURL);
            if (string.IsNullOrEmpty(url.Query))
                surveyURL += "?MTouch=";
            else if (!url.Query.ToLower().Contains("mtouch="))
                surveyURL += "MTouch=";
            surveyURL += encryptedMTouch;

            return surveyURL;
        }
    }
}
