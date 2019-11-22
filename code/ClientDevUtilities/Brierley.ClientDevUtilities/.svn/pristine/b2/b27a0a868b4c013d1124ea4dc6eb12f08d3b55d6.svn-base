using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.bScript.Functions;
using Brierley.FrameWork.bScript.LogicalOperators;
using Brierley.FrameWork.bScript.MathOperators;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// The ExpressionFactory class is used to obtain a Binary Expression tree capable of being evaluated by
    /// supplying a string representation of the expression.
    /// <remarks>
    /// Using bScript expressions is a relatively simple task. The expressions used by bScript are standard text
    /// expressions written in the common form (In Fix Notation) that we are all familiar with. For example the string
    /// '1 + 1' is a valid expression. Expressions are used throughout the framework to externalize required calculated,
    /// derived, or conditional information allowing us to design more generic types to accomplish common goals and
    /// significantly reduce the coding requirements to implement and then maitain a system. Consider the use of rules.
    /// In many cases there are conditions upon with the rule is based that determine whether or not the rule should do
    /// its work. A date range for example where the client might want to grant double points during a particular week.
    /// If the code to determine whether the current date falls within the range has to compiled directly into the
    /// code then we significantly reduce the re-use of that class. We also have a number of examples where calulated values
    /// for points accrued or redeemed can be externalized out of the class itself in the form of an expression. The bScript
    /// expression engine supports the following operations, functions, and conditionals;
    /// <b>Basic math operators</b>
    /// Sum (+)
    /// Difference (-)
    /// Product (*)
    /// Quotient (/)
    /// <b>Conditional/Logical Operators</b>
    /// Is Equal (==)
    /// Logical AND 
    /// Logical OR (|)
    /// Logical NOT (!)
    /// </remarks>
    /// </summary>
    [Serializable]
    public class ExpressionFactory:System.Collections.ArrayList
    {
        private const string _className = "ExpressionFactory";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		public const string _bScriptByExpression = "bScriptByExpression";

        private int expressionCount = 0;
        private string regExParentheticalGrouping = @"\((?>[^()]+|\( (?<number>)|\) (?<-number>))*(?(number)(?!))\)";        		
		
		//We now allow backslashes to be used to escape characters. We support the use of \r, \n, \t and \'. 
		//The backslash itself will need some escaping of its own, so double backslash will escape it
		//and convert to a single backslash. The old Regex:
		//		private string regExStringConstantMatch = @"'[^'\r\n|\\']*'";
		//would check for any text between two single quotes. The new regex will pass over an 
		//escaped single quote. The regex allows for a string to end in "\\'", because the backslash is escaped.
		//It will not allow a string to end in "\'"; the backslash is not escaped and therefore escapes the quote, 
		//meaning the string is not terminated.
		//this version did not allow an escaped quote at the end of a quoted string (e.g., 'hello\\'')
		//private string regExStringConstantMatch = @"'([^\\']|\\.)*(\\\\'|[^\\]')";
		//private string regExStringConstantMatch = @"'([^\\']|\\.)*(\\\\'|[^\\']'|\\'')";
		//above string wasn't matching empty string literals. Added catch for '':
		string regExStringConstantMatch = @"'([^\\']|\\.)*(\\\\'|[^\\']'|\\'')|''";

		public Expression Create(long expressionId)
		{
			using (var svc = LWDataServiceUtil.DataServiceInstance())
			{
				var bScript = svc.GetBscriptExpression(expressionId);
				if (bScript == null)
				{
					throw new Exception("Could not locate bscript expression with id " + expressionId.ToString());
				}
				return Create(bScript.Expression);
			}
		}

        /// <summary>
        /// Creates a binary expression tree based on the string expression supplied
        /// </summary>
        /// <param name="expr">The string representation of the expression</param>
        /// <returns>An binary tree expression object</returns>
		public Expression Create(string expr)
        {
            string methodName = "Create";
            string msg = string.Format("Creating expression from {0}.", expr);
            _logger.Debug(_className, methodName, msg);
			Expression ret = null;

			using (var mgr = LWDataServiceUtil.DataServiceInstance())
			{
				bool hasCache = mgr.CacheManager.RegionExists(_bScriptByExpression);

				string key = expr;
				if (hasCache)
				{
					ret = (Expression)mgr.CacheManager.Get(_bScriptByExpression, key);
					if (ret != null)
					{
						return ret;
					}
				}

				//load from library
				if (ExpressionUtil.IsLibraryExpression(expr))
				{
					string library = ExpressionUtil.GetLibraryName(expr);
					Bscript bscript = mgr.GetBscriptExpression(library);
					ret = Create(bscript.Expression);
					if (hasCache)
					{
						mgr.CacheManager.Update(_bScriptByExpression, key, ret);
					}
					return ret;
				}

				//load as expression builder expression
				if (expr.Trim().StartsWith("{"))
				{
					//this may be a json object that needs to be converted to an expression
					try
					{
						var r = Newtonsoft.Json.JsonConvert.DeserializeObject<WizardExpression>(expr);
						expr = r.GetRawExpression();
					}
					catch (Exception)
					{
						//invalid expression. Let the exception be thrown later
					}
				}

				System.Text.RegularExpressions.MatchCollection matches;

				// Start out by making sure we have a least a well formed expression with balanced parenthetical grouping.
				if (!EnsurematchingParenth(expr))
				{
					throw new ApplicationException("Invalid Expression: The expression contains unbalanced parenthases");
				}

				// Parse out string constants of the form 'xxx' in the entire expression and replace all occurances
				// with a pointer to a string constant instance. We do not want to convert to lower at this point because
				// casing may be important in the string constant.
				matches = new Regex(regExStringConstantMatch).Matches(expr);
				foreach (Match m in matches)
				{
					string value = m.ToString().Replace("'", "");
					DateTime dateValue = DateTimeUtil.MinValue;
					if (DateTime.TryParseExact(value, Enum.GetNames(typeof(SupportedDateFormats)), System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat, System.Globalization.DateTimeStyles.None, out dateValue))
					{
						// First lets see if the string is a date
						DateConstant dateConst = new DateConstant(dateValue);
						dateConst.Id = "~" + expressionCount.ToString() + "~";
						expr = expr.Replace(m.ToString(), dateConst.Id);
						expressionCount++;
						this.Add(dateConst);
					}
					else
					{
						// If not a date then just treat the value as a string constant
						StringConstant cexpr = new StringConstant(m.ToString());
						cexpr.Id = "~" + expressionCount.ToString() + "~";
						expr = expr.Replace(m.ToString(), cexpr.Id);
						expressionCount++;
						this.Add(cexpr);
					}
				}


				// To eliminate casing issues and simplify things we convert the expression to lower case.
				// String constants that may require specific casing have already been parsed out to 
				// expression pointers.
				expr = expr.ToLower();


				// now we must parse the expression for parenthetical groups starting with the inner most group first.
				Regex regEx = new Regex(regExParentheticalGrouping);
				matches = regEx.Matches(expr);
				while (matches.Count > 0)
				{
					foreach (Match m in matches)
					{
						Expression theXpr = ParseExpression(m.ToString());
						if (theXpr != null)
						{
							Add(theXpr);
							expr = expr.Replace(m.ToString(), theXpr.Id);
							expressionCount++;
						}
						else
						{
							expr = expr.Replace(m.ToString(), "");
						}
					}
					matches = regEx.Matches(expr);
				}

				// Parse the final expression
				ret = ParseExpression(expr);
				if (hasCache)
				{
					mgr.CacheManager.Update(_bScriptByExpression, key, ret);
				}
				return ret;
			}
        }


        /// <summary>
        /// Parses an expression string, and returns an expression object.
        /// The approach taken to parse an expression is recursive and done in stages. This method will call itself a number of times to 
        /// deal with ever smaller expressions, until the expression itself has been reduced to just a series of expression pointers. We must
        /// take into account operator precedence when parsing. The only thing we know about expr at this point is that it contains no 
        /// parenthetical groups, thanks to our regular expression. We cannot however, assume that it only has two operands.
        /// It could be b*x+y+z > 50. At this point we do not have parentheses to guide the order. So operator precedence takes over.
        /// We will allways reduce the expression to the form x operator y
        /// </summary>
        /// <param name="expr">The string to parse</param>
        /// <returns>An instance of Expression</returns>
        private Expression ParseExpression(string expr)
        {
            string methodName = "ParseExpression";
            string msg = string.Format("Parsing expression {0}.", expr);
            _logger.Debug(_className, methodName, msg);

            // Lets do some general cleanup on the expression. Since our handy parenthetical group regular expression
            // ensures that we are parsing the inner most groups first and then working our way out, 
            // we can safely remove any parenthesis in the expression as well as spaces.
            expr = expr.Replace("(", "");
            expr = expr.Replace(")", "");
            expr = expr.Replace(" ", "");
            expr = expr.Replace("--", "+"); // Double negative is the same thing as +
            expr = expr.Replace("+-", "-"); // Adding a negative number is the same thing as subtracting.
            expr = expr.Trim();

            Expression Xpr = null;

            if (string.IsNullOrEmpty(expr))
            {
                return null;
            }

            // Is the expression a function parameter list.
            if (expr.Contains(Symbols.PARAMETERSEPERATOR)) 
            {
                string[] parts = expr.Split(Symbols.PARAMETERSEPERATOR.ToCharArray()); // It's preferable to use the string split here becuase we may have more than two parameters
                Expression[] expressions = new Expression[parts.Length];
                for (int x = 0; x <= parts.GetUpperBound(0); x++)
                {
                    expressions[x] = ParseExpression(parts[x]);
                }
                Xpr = new ParameterList(expressions);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }

            // Before we waste a lot of time jumping around with our hair on fire, parsing the string
            // lets see if the expression is simply calling for a number, if so well just return that 
            // number as a Constant.
            decimal aNumber = 0;
            if (System.Decimal.TryParse(expr,out aNumber))
            {
                //aNumber = System.Double.Parse(expr);
                Xpr = new Constant(aNumber);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }

            // Does the expression contain logical operators AND, OR, NOT
            if (expr.Contains(Symbols.AND))
            {
                string[] parts = Split(expr, Symbols.AND);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new LogicalAND(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.OR)) // If the lhs of the expression returns true the rhs never gets evaluated.
            {
                string[] parts = Split(expr, Symbols.OR);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new LogicalOR(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.NOT)) 
            {
                expr = expr.Replace("!", "");
                Expression rhs = ParseExpression(expr);
                Xpr = new LogicalNOT(rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }


            // Does the expression contain comparison operators >, <, >=, <=, ==, <>
            if (expr.Contains(Symbols.GREATEREQUAL))
            {
                string[] parts = Split(expr, Symbols.GREATEREQUAL);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new GreaterEqual(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.LESSEQUAL))
            {
                string[] parts = Split(expr, Symbols.LESSEQUAL);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new LessEqual(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.EQUALS)) 
            {
                string[] parts = Split(expr, Symbols.EQUALS);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new Equals(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.NOTEQUAL))
            {
                string[] parts = Split(expr, Symbols.NOTEQUAL);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new NotEqual(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.GREATER))
            {
                string[] parts = Split(expr, Symbols.GREATER);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new GreaterThan(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.LESS))
            {
                string[] parts = Split(expr, Symbols.LESS);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new LessThan(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.PRODUCT))
            {
                string[] parts = Split(expr, Symbols.PRODUCT);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new Brierley.FrameWork.bScript.MathOperators.Product(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.QUOTIENT))
            {
                string[] parts = Split(expr, Symbols.QUOTIENT);
                Expression lhs = ParseExpression(parts[0]);
                Expression rhs = ParseExpression(parts[1]);
                Xpr = new Quotient(lhs, rhs);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.Contains(Symbols.SUM))
            {
                string[] parts = Split(expr, Symbols.SUM);
                if (string.IsNullOrEmpty(parts[0])) // it was just a signed number
                {
                    Xpr = new Constant(System.Decimal.Parse(expr));
                    Xpr.Id = "~" + expressionCount.ToString() + "~";
                    return Xpr;
                }
                else
                {
                    Expression lhs = ParseExpression(parts[0]);
                    Expression rhs = ParseExpression(parts[1]);
                    Xpr = new Brierley.FrameWork.bScript.MathOperators.Sum(lhs, rhs);
                    Xpr.Id = "~" + expressionCount.ToString() + "~";
                    return Xpr;
                }
            }
            if (expr.Contains(Symbols.DIFFERENCE))
            {
                string[] parts = Split(expr, Symbols.DIFFERENCE);
                if (string.IsNullOrEmpty(parts[0])) // it was just a signed number
                {
                    Xpr = new Constant(System.Decimal.Parse(expr));
                    Xpr.Id = "~" + expressionCount.ToString() + "~";
                    return Xpr;
                }
                else
                {
                    Expression lhs = ParseExpression(parts[0]);
                    Expression rhs = ParseExpression(parts[1]);
                    Xpr = new Difference(lhs, rhs);
                    Xpr.Id = "~" + expressionCount.ToString() + "~";
                    return Xpr;
                }
            }

            // It could be that this function is being asked to parse just an expression id of the form ~x~
            // If this is the case then we just need to return it.
            if (expr.StartsWith("~") && expr.EndsWith("~"))
            {
                return this[expr];
            }

            // recursively parse out functions and their parameter lists beginning with the inner most functions first.
            // we replace the function call with a function pointer in the expression itself.
            string[] fNames = GetFunctionNames();
            //foreach (string functionName in Enum.GetNames(typeof(FunctionNames)))
            foreach (string functionName in fNames)
            {
                if (expr.StartsWith(functionName.ToLower()))
                {
                    if (expr.Contains("~"))
                    {
                        string[] parts = Split(expr, "~");
                        Xpr = FunctionFactory.GetFunction(parts[0], this["~" + parts[1]]); // have to put the tilde back in after the split
                        if (Xpr == null)
                        {
                            string errMsg = "The expression seems to be invalid.  Can you validate it.";
                            _logger.Error(_className, methodName, errMsg);
                            throw new LWException(errMsg);
                        }
                        else
                        {
                            Xpr.Id = "~" + expressionCount.ToString() + "~";
                            return Xpr;
                        }
                    }
                    else
                    {
                        Xpr = FunctionFactory.GetFunction(expr, null);
                        Xpr.Id = "~" + expressionCount.ToString() + "~";
                        return Xpr;
                    }
                }
            }
            if (expr.StartsWith("Member.".ToLower()))
            {
                string ProperyName = expr.Remove(0, expr.IndexOf('.') + 1);
                Xpr = new Property(ProperyName);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if ("currentmember" == expr.ToLower())
            {                
                Xpr = new CurrentMember();
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.StartsWith("parent.".ToLower()))
            {
                string AttrName = expr.Remove(0, expr.IndexOf('.') + 1);
                Xpr = new ParentAttribute(AttrName);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }
            if (expr.StartsWith("row."))
            {
                string AttrName = expr.Remove(0, expr.IndexOf('.') + 1);
                Xpr = new Attribute(AttrName);
                Xpr.Id = "~" + expressionCount.ToString() + "~";
                return Xpr;
            }

            // if we get this far something has gone horribly wrong!
            throw new CRMException("Expression Parsing Error: Unrecognized Syntax in the expression. " + expr);
        }

        /// <summary>
        /// This method combines the builtin function names with the custom defined functions.
        /// </summary>
        /// <returns></returns>
        private static string[] GetFunctionNames()
        {
            List<string> names = new List<string>(Enum.GetNames(typeof(FunctionNames)));
			using (var svc = LWDataServiceUtil.DataServiceInstance())
			{
                List<RemoteAssembly> assemblies = svc.GetAllRemoteAssemblies();
                Type baseType = typeof(Brierley.FrameWork.bScript.Expression);
                foreach (RemoteAssembly remoteAssembly in assemblies)
                {
                    foreach(var references in remoteAssembly.FindComponents(CustomComponentTypeEnum.BScript))
                    {
                        names.Add(references.Name);
                    }
                }
                return names.ToArray();
			}
        }

        /// <summary>
        /// The Split method is used here to split an expression at the first occurrance of a specified character. It will result in exactly
        /// two array elements, as opposed to the normal string.split method, that splits a string at all occurrances of the character. The
        /// normal string.split methodology will break expression parsing. Consider the expression 1+1+1+1. We recursively deal with each
        /// operator in turn by splitting the string into pairs. Thus the expression above becomes 1+(1+(1+1)). The only place this implementation
        /// of split is NOT used by the parsing engine is when dealing with a Functions parameter list. In that case, the normal behavior of split
        /// is preferable.
        /// </summary>
        /// <param name="theString">The string to split</param>
        /// <param name="theCharacter">The character to split at</param>
        /// <returns>A 2 element string array</returns>
        private static string[] Split(string theString,string theCharacter)
        {
            int ChrLocation = theString.IndexOf(theCharacter);
            string[] theResults = new string[2];
            theResults[0] = theString.Substring(0, ChrLocation);
            theResults[1] = theString.Substring(ChrLocation + theCharacter.Length);
            return theResults;
        }

        /// <summary>
        /// Does a very simple check, based on count to make sure that the expressions pararenthetical groups are balanced. Basically the same
        /// number of opening and closing parenthases are present. This function does NOT check to make sure the grouping makes any sense logically
        /// or mathematically
        /// </summary>
        /// <param name="expr">The expression to check</param>
        /// <returns>A boolean value, true if the expression is balanced false otherwise</returns>
        private static bool EnsurematchingParenth(string expr)
        {
            if (expr.Contains("("))
            {
                int OpenIndex = expr.IndexOf("(", 0);
                int OpenCount = 1;
                while (OpenIndex != -1)
                {
                    OpenIndex = expr.IndexOf("(", OpenIndex + 1);
                    if (OpenIndex > 0)
                    {
                        OpenCount++;
                    }
                }

                int CloseIndex = expr.IndexOf(")", 0);
                if (CloseIndex == -1)
                {
                    return false;
                }
                else
                {
                    int CloseCount = 1;
                    while (CloseIndex != -1)
                    {
                        CloseIndex = expr.IndexOf(")", CloseIndex + 1);
                        if (CloseIndex > 0)
                        {
                            CloseCount++;
                        }
                    }
                    if (OpenCount != CloseCount)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// A string indexer on the class, used to return a parsed expression based on expression id.
        /// </summary>
        /// <param name="ExpressionId">The ID of the desired expression</param>
        /// <returns>An instance of Expression</returns>
        public Expression this[string ExpressionId]
        {
            get
            {
                for (int x = 0; x <= this.Count - 1; x++)
                {
                    if (this[x] != null && ((Expression)this[x]).Id == ExpressionId)
                    {
                        return (Expression)this[x];
                    }
                }
                return null;
            }
        }
    }
}
