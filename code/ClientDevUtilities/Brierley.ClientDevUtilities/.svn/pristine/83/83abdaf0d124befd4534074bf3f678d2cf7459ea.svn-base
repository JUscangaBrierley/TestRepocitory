using System;
using System.Globalization;
using System.Threading;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Get the current UI language.
    /// </summary>
    /// <example>
    ///     Usage : GetCurrentUILanguage()
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Get the current UI language.",
		DisplayName = "GetCurrentUILanguage",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings,
		ExpressionReturns = ExpressionApplications.Strings,
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Content,
		WizardDescription = "Get Current UI Language")]
    public class GetCurrentUILanguage : UnaryOperation
    {
        /// <summary>
        /// Syntax definition for this function.
        /// </summary>
        public new string Syntax
        {
            get { return "GetCurrentUILanguage()"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetCurrentUILanguage()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetCurrentUILanguage(Expression rhs)
			: base("GetCurrentUILanguage", rhs)
        {
        }

        /// <summary>
        /// Performs the operation defined by this function. 
        /// </summary>
        /// <param name="contextObject">A container object used to pass context at runtime.</param>
        /// <returns>An object representing the result of the evaluation</returns>
        /// <exception cref="Brierley.Framework.Common.Exceptions.CRMException">thrown for illegal arguments</exception>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
				string iso639_1 = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
				string englishName = CultureInfo.GetCultureInfo(iso639_1).EnglishName;

				return englishName;
            }
            catch (Exception)
            {
				throw new CRMException("Error evaluating GetCurrentUILanguage function");
            }
        }        
    }
}
