using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Gets the value of the provided enum name.
    /// </summary>
    /// <example>
	///     Usage : GetEnumValue(enumType, name)
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Gets the value of the provided enum name.",
		DisplayName = "GetEnumValue",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Objects,
		ExpressionReturns = ExpressionApplications.Objects)]
    public class GetEnumValue : UnaryOperation
    {
		private Expression enumTypeExpression = null;
		private Expression enumNameExpression = null;		
        

        /// <summary>
        /// Syntax definition for this function.
        /// </summary>
        public new string Syntax
        {
			get { return "GetEnumValue(enumType, name)"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetEnumValue()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetEnumValue(Expression rhs)
			: base("GetEnumValue", rhs)
        {
            if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length == 2)
            {
				enumTypeExpression = ((ParameterList)rhs).Expressions[0];
				enumNameExpression = ((ParameterList)rhs).Expressions[1];                
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
				string enumName = enumNameExpression.evaluate(contextObject).ToString();
				object value = Enum.Parse(enumType, enumName);
				return value;
			}
			catch (Exception ex)
			{
				throw new LWBScriptException("Error evaluating GetEnumValue.", ex);
			}			            
        }        
    }
}
