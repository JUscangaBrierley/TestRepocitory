using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The Round function returns the rounded value of a given number.
    /// </summary>
    /// <example>
    ///     Usage : Round(number)
    /// </example>
    [Serializable]
	[ExpressionContext(Description = "Returns the rounded value of a given number.", 
		DisplayName = "Round", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers, 
		ExpressionReturns = ExpressionApplications.Numbers)]
    public class Round : UnaryOperation
    {
        /// <summary>
        /// Normaly, 6.5 will be rounded to 6.  If this variable is tru then it will be rounded to 7.
        /// </summary>
        bool roundUp = false;
        decimal number = 0;
        /// <summary>
        /// Public Constructor
        /// </summary>
        public Round()
        {
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal Round(Expression rhs)
            : base("Round", rhs)
        {                        
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "Round(number,RoundUp)";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function.
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        /// <exception cref="CRMException">evaulate will throw an ellegal expression exception if
        /// its operand is non numeric.</exception>
        public override object evaluate(ContextObject contextObject)
        {
            Expression rhs = GetRight();
            ParameterList parmList = rhs as ParameterList;
            if (parmList != null)
            {
                if (parmList.Expressions.Length > 1)
                {
                    try
                    {
                        number = (decimal)((ParameterList)rhs).Expressions[0].evaluate(contextObject);
                    }
                    catch
                    {
                        throw new CRMException(string.Format("Illegal Expression:The operand of Round function must be numeric [{0}]", ((ParameterList)rhs).Expressions[0].ToString()));
                    }
                    try
                    {
                        roundUp = System.Convert.ToBoolean(((ParameterList)rhs).Expressions[1].ToString());
                    }
                    catch (Exception ex)
                    {
                        string errMsg = string.Format("Error while converting {0} to a boolean.", ((ParameterList)rhs).Expressions[1].ToString());
                        throw new LWException(errMsg, ex);
                    }
                }
                else
                {
                    string errMsg = string.Format("Invalid arguments.");
                    throw new LWException(errMsg);
                }
            }
            else
            {
                try
                {
                    number = (decimal)this.GetRight().evaluate(contextObject);
                }
                catch
                {
                    throw new LWException(string.Format("Illegal Expression:The operand of Round function must be numeric [{0}]", this.GetRight().ToString()));
                }
            }
			
            try
            {
                Decimal result = System.Math.Round(number);
                decimal diff = number - result;
                if (Math.Abs(diff) == (decimal).5)
                {
                    if (roundUp && diff == (decimal).5)
                    {
                        result += 1;
                    }
                    else if (!roundUp && diff == (decimal)-.5)
                    {
                        result -= 1;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("Error while getting result from {0}.", number.ToString());
                throw new LWException(errMsg, ex);
            }
        }        
    }
}
