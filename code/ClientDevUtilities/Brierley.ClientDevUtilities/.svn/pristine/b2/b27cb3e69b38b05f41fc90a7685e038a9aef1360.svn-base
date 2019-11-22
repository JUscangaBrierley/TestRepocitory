using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Web;

namespace Brierley.FrameWork.Common
{
    public static class StringUtils
    {
        private static Regex WhiteSpace = new Regex(@"\s+");

        public static int CountConsecutiveCharacters(string str, char c, ref int start, ref int end)
        {
            int nChars = 0;
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Length == 1)
                {
                    nChars = str[0] == c ? 1 : 0;
                }
                else
                {
                    bool foundFirst = false;
                    for (int i = 0; i < str.Length; i++)
                    {
                        char cs = str[i];
                        if (!foundFirst && cs == c)
                        {
                            foundFirst = true;
                            start = i;
                        }
                        if (foundFirst)
                        {
                            if (cs == c)
                            {
                                nChars++;
                            }
                            else
                            {
                                end = i - 1;
                                break;
                            }
                        }
                    }
                    if (foundFirst && end == 0)
                    {
                        end = str.Length - 1;
                    }
                }
            }
            return nChars;
        }

        public static string Reverse(string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static string Replace(string str, string newValue, int start, int end)
        {
            string left = str.Substring(0, start);
            string right = str.Substring(end + 1);
            return left + newValue + right;
        }

		public static string CapitalizeWord(string str)
		{
			string newStr = str;
			if (!string.IsNullOrEmpty(str))
			{
				newStr = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);				
			}
			return newStr;
		}

        public static string FriendlyString(object stringObject)
        {
            return FriendlyString(stringObject, "");
        }

        public static string FriendlyString(object stringObject, string defaultValue)
        {
            string result = defaultValue;
            if (stringObject != null)
            {
                string tmp = stringObject.ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        public static string FriendlyString(object stringObject, string defaultValue, int length)
        {
            string result = defaultValue;
            if (stringObject != null)
            {
                string tmp = stringObject.ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    result = tmp;
                }
                if (result.Length > length)
                {
                    result = result.Substring(0, length);
                }
            }
            return result;
        }

        public static Int32 FriendlyInt32(object stringObject)
        {
            return FriendlyInt32(stringObject, -1);
        }

        public static Int32 FriendlyInt32(object stringObject, Int32 defaultValue)
        {
            Int32 result = defaultValue;
            if (stringObject != null)
            {
                string tmp = stringObject.ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    try
                    {
                        result = Int32.Parse(tmp);
                    }
                    catch
                    {
                        result = defaultValue;
                    }
                }
            }
            return result;
        }

        public static Int64 FriendlyInt64(object stringObject)
        {
            return FriendlyInt64(stringObject, -1);
        }

        public static Int64 FriendlyInt64(object stringObject, Int64 defaultValue)
        {
            Int64 result = defaultValue;
            if (stringObject != null)
            {
                string tmp = stringObject.ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    try
                    {
                        result = Int64.Parse(tmp);
                    }
                    catch
                    {
                        result = defaultValue;
                    }
                }
            }
            return result;
        }

		public static double FriendlyDouble(object stringObject)
		{
			return FriendlyDouble(stringObject, (double)0.0);
		}

		public static double FriendlyDouble(object stringObject, double defaultValue)
		{
			double result = defaultValue;
			if (stringObject != null)
			{
				string tmp = stringObject.ToString();
				if (!string.IsNullOrEmpty(tmp))
				{
					try
					{
						result = double.Parse(tmp);
					}
					catch
					{
						result = defaultValue;
					}
				}
			}
			return result;
		}

        public static decimal FriendlyDecimal(object stringObject)
        {
            return FriendlyDecimal(stringObject, (decimal)0.0);
        }

        public static decimal FriendlyDecimal(object stringObject, decimal defaultValue)
        {
            decimal result = defaultValue;
            if (stringObject != null)
            {
                string tmp = stringObject.ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    try
                    {
                        result = decimal.Parse(tmp);
                    }
                    catch
                    {
                        result = defaultValue;
                    }
                }
            }
            return result;
        }

        public static bool FriendlyBool(object stringObject)
        {
            return FriendlyBool(stringObject, false);
        }

        public static bool FriendlyBool(object stringObject, bool defaultValue)
        {
            bool result = defaultValue;
            if (stringObject != null)
            {
                string tmp = stringObject.ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    try
                    {
                        result = bool.Parse(tmp);
                    }
                    catch
                    {
                        result = defaultValue;
                    }
                }
            }
            return result;
        }

        public static DateTime FriendlyDateTime(object stringObject)
        {
            return FriendlyDateTime(stringObject, DateTime.Now);
        }

        public static DateTime FriendlyDateTime(object stringObject, DateTime defaultValue)
        {
            DateTime result = defaultValue;
            if (stringObject != null)
            {
                string tmp = stringObject.ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    try
                    {
                        result = DateTime.Parse(tmp);
                    }
                    catch
                    {
						try
						{
							// the RadDate picker can return this format
							result = DateTimeUtil.ConvertStringToDate("yyyy-MM-dd-HH-mm-ss", tmp);
							if (result == null)
							{
								result = defaultValue;
							}
						}
						catch
						{
							result = defaultValue;
						}
                    }
                }
            }
            return result;
        }

		public static string FriendlyXElement(object xelementObject)
		{
			return FriendlyXElement(xelementObject, string.Empty);
		}

		public static string FriendlyXElement(object xelementObject, string defaultValue)
		{
			string result = defaultValue;
			if (xelementObject != null && xelementObject is XElement)
			{
				XElement tmp = (XElement)xelementObject;
				if (!string.IsNullOrEmpty(tmp.ToString()))
				{
                    result = tmp.ToString();
				}
			}
			return result;
		}

		public static string FriendlyXAttribute(object xattributeObject)
		{
			return FriendlyXAttribute(xattributeObject, string.Empty);
		}

		public static string FriendlyXAttribute(object xattributeObject, string defaultValue)
		{
			string result = defaultValue;
			if (xattributeObject != null && xattributeObject is XAttribute)
			{
				XAttribute tmp = (XAttribute)xattributeObject;
				if (!string.IsNullOrEmpty(tmp.Value))
				{
					result = tmp.Value;
				}
			}
			return result;
		}

        public static string HtmlFriendly(string content)
        {
            if ( !string.IsNullOrEmpty(content) )
            {
				return content
					.Replace(Environment.NewLine, "<br />")
					.Replace("\n", "<br />")
					.Replace("\r", "<br />")
					.Replace("'", "&quot;")
					//.Replace("\"", "\\\"") <-- // this replaces quotes (") with backslash-quote (\")
					.Replace("\\", "\\\\");
            }
            else
            {
                return string.Empty;
            }
        }

		public static string JavascriptFriendly(string content)
		{
			if (!string.IsNullOrEmpty(content))
			{
				return content
					.Replace(Environment.NewLine, "\\n")
					.Replace("<br />", "\\n")
					.Replace("'", "\\'");
			}
			else
			{
				return string.Empty;
			}
		}

		public static string JavascriptEscapeQuotes(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return string.Empty;
			}
			return content.Replace("'", "\\'");
		}

        public static string DBFriendly(string statement)
        {
            // replace ' with \"
            return !string.IsNullOrEmpty(statement) ? statement.Replace("'","\\\"") : string.Empty;
        }

        public static string NoSpaces(string str)
        {
			return WhiteSpace.Replace(FriendlyString(str), @"");
        }

        public static string DBQuote(string arg)
        {
            return "'" + DBEscape(arg) + "'";
        }

        public static string DBEscape(string arg)
        {
            if (String.IsNullOrEmpty(arg)) return arg;
            return arg.Replace("'", "''");
        }

		public static string DeHTML(string html)
		{
			string noCss = Regex.Replace(FriendlyString(html), @"<style type=""text/css"">(.|\n)*?</style>", string.Empty);
			string noHtmlTags = Regex.Replace(noCss, @"<(.|\n)*?>", string.Empty);
			string noHtmlEscapes = Regex.Replace(noHtmlTags, @"&(.|\n)*?;", string.Empty);
			string result = noHtmlEscapes.Trim();
			return result;
		}

		public static string DeBScript(string html)
		{
			return Regex.Replace(FriendlyString(html), @"##(.|\n)*?##", string.Empty);
		}

		public static Uri AbsoluteUri(Uri requestUri, string relativeUri)
		{
			Uri result = null;
			if (!Uri.TryCreate(relativeUri, UriKind.Absolute, out result))
			{
				Uri.TryCreate(requestUri, relativeUri, out result);
			}
			return result;
		}

		public static string JSEncode(string str)
		{
			string result = HttpUtility.UrlEncode(FriendlyString(str)).Replace("+", "%20");
			return result;
		}

		public static string JSDecode(string str)
		{
			string result = HttpUtility.UrlDecode(FriendlyString(str));
			return result;
		}

        public static string XmlFriendlyString(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            StringBuilder buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (IsLegalXmlChar(c))
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        public static bool IsLegalXmlChar(int character)
        {
            return
            (
                   character == 0x9 /* == '\t' == 9   */          ||
                   character == 0xA /* == '\n' == 10  */          ||
                   character == 0xD /* == '\r' == 13  */          ||
                   (character >= 0x20 && character <= 0xD7FF) ||
                   (character >= 0xE000 && character <= 0xFFFD) ||
                   (character >= 0x10000 && character <= 0x10FFFF)
            );
        }

        public static int IndexOfOccurrence(string s, string match, int occurrence)
        {
            int i = 1;
            int index = 0;

            while (i <= occurrence && (index = s.IndexOf(match, index + 1)) != -1)
            {
                if (i == occurrence)
                    return index;

                i++;
            }

            return -1;
        }
    }
}
