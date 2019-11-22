using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;

//TODO: There is another content evaluator in Brierley.FrameWork.Common that is used for TextBlocks.  It would be good to consolidate them.
namespace Brierley.FrameWork.Email
{
    public class ContentEvaluator : IDisposable
    {
        #region private variables
        private ContextObject _contextObject = null;
        private string _content = string.Empty;
        private ExpressionFactory _expressionFactory = new ExpressionFactory();
        #endregion

        #region constructors
		public ContentEvaluator(string content)
		{
			this._content = content;
			_contextObject = new ContextObject();
		}

        public ContentEvaluator(string content, ContextObject contextObject)
        {
            this._content = content;
			_contextObject = contextObject;
        }
        #endregion

        #region public methods
		public string Evaluate()
		{
			return Evaluate("##", 5);
		}

        public string Evaluate(string delimeter, int nPasses)
        {
            string result = StringUtils.FriendlyString(_content);
			if (!string.IsNullOrEmpty(result))
			{
				for (int i = 0; i < nPasses; i++)
				{
					if (result.IndexOf("##") != -1)
					{
						result = MatchAndReplace(delimeter, result);
					}
					else
					{
						break;
					}
				}
			}
            return result;
        }

        public void Dispose()
        {
        }
        #endregion

        #region private methods
        private class ExpressionToken
        {
            public int begin;
            public int end;
            public string expression;
            public string result;
        }

        private string EvaluateExpression(string expression)
        {
            Expression expr = _expressionFactory.Create(expression);
            string result = string.Empty;

            result = StringUtils.FriendlyString(expr.evaluate(_contextObject), string.Empty);
            return result;
        }

        private string MatchAndReplace(string delimeter, string lcontent)
        {
            List<ExpressionToken> tokens = new List<ExpressionToken>();

            // first phase - find and evaluate tokens.
            int beginIndex = -1;
            while (beginIndex < lcontent.Length - 4) //need at least 4 characters for the 2 ##'s (plus whatever expression falls in between)
            {
                beginIndex = lcontent.IndexOf(delimeter, beginIndex == -1 ? 0 : beginIndex + 2);
                if (beginIndex != -1)
                {
                    int endIndex = lcontent.IndexOf(delimeter, beginIndex + 1);
                    if (endIndex == -1)
                    {
                        throw new FormatException("Invalid content format.");
                    }
                    ExpressionToken token = new ExpressionToken();
                    tokens.Add(token);
                    token.begin = beginIndex;
                    token.end = endIndex;
                    string expr = lcontent.Substring(beginIndex + 2, endIndex - beginIndex - 2);
                    string evalautedStr = EvaluateExpression(expr);
                    token.expression = expr;
                    token.result = evalautedStr;
                    beginIndex = endIndex + 2;
                }
                else
                {
                    break;
                }
            }

            // phase 2 - construct return string.
            StringBuilder strBuilder = new StringBuilder();
            if (tokens.Count > 0)
            {
                int idx = 0;
                foreach (ExpressionToken token in tokens)
                {
                    string piece = lcontent.Substring(idx, token.begin - idx);
                    strBuilder.Append(piece);
                    strBuilder.Append(token.result);
                    idx = token.end + 2;
                }
                int left = lcontent.Length - idx;
                if (left > 0)
                {
                    strBuilder.Append(lcontent.Substring(idx, left));
                }
            }
            else
            {
                // no tokens found.
                strBuilder.Append(lcontent);
            }

            return strBuilder.ToString();
        }
        #endregion
    }
}
