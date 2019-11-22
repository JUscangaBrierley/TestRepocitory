using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The IsInSet function evaluates a list of items and returns true if value is found in the list.
    /// </summary>
    /// <example>
    ///     Usage : IsInSet('Value','item1;item2;item3...')
    /// </example>
    /// <remarks>
    /// Both parameter values are string values. The second parameter is a list of items seperated by the
    /// ";" character.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Evaluates a list of items and returns true if the value is found in the list.", 
		DisplayName = "IsInSet", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Strings, 
		ExpressionReturns = ExpressionApplications.Booleans,
		WizardDescription = "IsInSet",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function)]
	[ExpressionParameter(Order = 0, Name = "Value", Type = ExpressionApplications.All, Optional = false, WizardDescription = "Value")]
	[ExpressionParameter(Order = 1, Name = "Set", Type = ExpressionApplications.All, Optional = false, WizardDescription = "Set", AllowMultiple=true)]
	public class IsInSet : UnaryOperation
    {
        private Expression _theValue = null;
        private Expression _set = null;
        private string[] set = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public IsInSet()
        {
        }


        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        public IsInSet(Expression rhs)
            : base("IsInSet", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 2)
            {
                _theValue = ((ParameterList)rhs).Expressions[0];
                _set = ((ParameterList)rhs).Expressions[1];
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to Is In Set.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "IsInSet('Value','item1;item2;item3...')";
            }
        }
        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <param name="contextObject">The context provided at runtime</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
			object val = _theValue.evaluate(contextObject);
			if (val != null)
			{
				string searchValue = val.ToString();
				if (!string.IsNullOrEmpty(searchValue))
				{
					searchValue = searchValue.Trim();
                    set = _set.evaluate(contextObject).ToString().Split(';');
					foreach (string item in set)
					{
						if (searchValue.ToLower() == item.Trim().ToLower())
						{
							return true;
						}
					}
				}
			}
			return false;
        }        
    }
}
