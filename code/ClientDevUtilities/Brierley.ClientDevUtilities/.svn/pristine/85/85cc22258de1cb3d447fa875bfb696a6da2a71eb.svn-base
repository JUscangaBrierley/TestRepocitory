using System;
using System.Collections.Generic;
using System.Text;


using Brierley.FrameWork.Common.Exceptions;
namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The STRSubstring function takes two expressions that provide a start and index.
	/// </summary>
	/// <example>
	///     Usage : STRSubstring('start','end')
	/// </example>
	/// <remarks>The parameters supplied to STRConcat must both be string values.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the substring marked by the start and end positions.",
		DisplayName = "StrSubstring", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Strings, 
		ExpressionReturns = ExpressionApplications.Strings)]
    public class STRSubstring : UnaryOperation
    {
		private Expression stringExpression = null;
		private Expression startIndexExpression = null;
		private Expression endIndexExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public STRSubstring()
        {
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
		/// <param name="expr">an object of type <see cref="Brierley.Framework.bScript.Expression"/> contining the first first function parameter.</param>
        internal STRSubstring(Expression expr)
			: base("STRSubstring", expr)
        {
			if (expr is ParameterList && ((ParameterList)expr).Expressions.Length >= 2)
			{
				stringExpression = ((ParameterList)expr).Expressions[0];
				startIndexExpression = ((ParameterList)expr).Expressions[1];
				if (((ParameterList)expr).Expressions.Length > 2)
				{
					endIndexExpression = ((ParameterList)expr).Expressions[2];
				}
				return;
			}
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to STRSubstring.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
				return "STRSubstring(string,start,[end])";
            }
        }
        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
			try
			{
				string result = string.Empty;
				int start = -1;
				int end = -1;
				string str = stringExpression.evaluate(contextObject).ToString();
				if (!string.IsNullOrEmpty(str))
				{
					start = int.Parse(startIndexExpression.evaluate(contextObject).ToString());
					if (endIndexExpression != null)
					{
						end = int.Parse(endIndexExpression.evaluate(contextObject).ToString());
					}
					if (end != -1)
					{
						result = str.Substring(start, end);
					}
					else
					{
						result = str.Substring(start);
					}
				}
				return result;
			}
			catch (System.Exception ex)
			{
				throw new CRMException("The substring string function failed. " + ex.Message);
			}
        }        
    }
}
