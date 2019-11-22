using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Gets the string name of the provided enum value.
    /// </summary>
    /// <example>
	///     Usage : GetEnumName(enumType, value)
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Gets the string name of the provided enum value.",
		DisplayName = "GetEnumName",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings,
		ExpressionReturns = ExpressionApplications.Strings)]
    public class GetEnumName : UnaryOperation
    {
		private Expression enumTypeExpression = null;
		private Expression enumValueExpression = null;		
        

        /// <summary>
        /// Syntax definition for this function.
        /// </summary>
        public new string Syntax
        {
			get { return "GetEnumName(enumType, value)"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetEnumName()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetEnumName(Expression rhs)
			: base("GetEnumName", rhs)
        {
            if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length == 2)
            {
				enumTypeExpression = ((ParameterList)rhs).Expressions[0];
				enumValueExpression = ((ParameterList)rhs).Expressions[1];                
                return;
            }
			throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to GetEnumValue.");
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
				Type enumType = Type.GetType(enumTypeExpression.evaluate(contextObject).ToString());
				string enumValue = enumValueExpression.evaluate(contextObject).ToString();
				Type ut = Enum.GetUnderlyingType(enumType);
				object v = Convert.ChangeType(Enum.Parse(enumType, enumValue), ut);
				return Enum.GetName(enumType, v);
			}
			catch (Exception ex)
			{
				throw new LWBScriptException("Error evaluating GetEnumName.", ex);
			}			            
        }        
    }
}
