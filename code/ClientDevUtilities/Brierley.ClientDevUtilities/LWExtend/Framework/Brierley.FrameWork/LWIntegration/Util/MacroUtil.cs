using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.LWIntegration.Util
{
    public class MacroUtil
    {
        public static string SubstituteMacros(NameValueCollection globals, NameValueCollection args, string str)
        {
            string originalString = str;
            string replacedToken = string.Empty;
            string token = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                int tokenStart = str.IndexOf("{$");
                if (tokenStart != -1)
                {
                    // there is a macro that needs to be substituted
                    int tokenEnd = str.IndexOf("}");
                    if (tokenEnd == -1)
                    {
                        // syntax error
                        throw new LWIntegrationException("Invalid syntax in string: " + str) { ErrorCode = 4300 };
                    }
                    else
                    {
                        token = str.Substring(tokenStart, tokenEnd - tokenStart + 1);
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            string actualToken = token.Substring(2, tokenEnd - tokenStart - 2);
                            string macro = actualToken.ToLower();
                            if (macro.StartsWith("date") )
                            {
                                string dateFormat = globals["DateConversionFormat"];
                                if (macro.Contains("(format"))
                                {
                                    int fmtIndexStart = macro.IndexOf("=");
                                    int fmtIndexEnd = macro.IndexOf(")");
                                    dateFormat = actualToken.Substring(fmtIndexStart + 1, fmtIndexEnd - fmtIndexStart - 1);
                                }
                                replacedToken = DateTimeUtil.ConvertDateToString(dateFormat, DateTime.Now);                                                               
                            }
                            else if (macro.StartsWith("sequencenumber"))
                            {
                                int fmtIndexStart = macro.IndexOf("=");                                
                                string objId = actualToken.Substring(fmtIndexStart + 1);
                                using (DataService dataService = LWDataServiceUtil.DataServiceInstance())
                                    replacedToken = dataService.GetNextID(objId).ToString();
                            }
                            else if (macro == "jobnumber")
                            {
                                replacedToken = globals["JobNumber"];
                            }
                            else if (macro == "jobname")
                            {
                                replacedToken = globals["JobName"];
                            }
                            else if (macro == "filename")
                            {
                                replacedToken = globals["FileName"];
                            }
                            else if (macro.StartsWith("arg=") && args != null)
                            {
                                string argName = macro.Substring(4);
                                string argValue = args[argName];                                
                                replacedToken = argValue;
                            }
                        }
                        else
                        {
                            throw new LWIntegrationException("Empty token in string: " + str) { ErrorCode = 4301 };
                        }
                    }
                    if (!string.IsNullOrEmpty(replacedToken))
                    {
                        //string token = originalString.Substring(tokenStart, tokenEnd - tokenStart + 1);                        
                        originalString = StringUtils.Replace(originalString, replacedToken, tokenStart, tokenEnd);
                    }
                    else
                    {
                        // a value of the replaced token was not found.
                        string errMsg = string.Format("No value found for the token \"{0}\". The original string is \"{1}\"", token, originalString);
                        throw new LWIntegrationException("Error evaluating macro: " + errMsg) { ErrorCode = 4302 };
                    }
                    return SubstituteMacros(globals, args, originalString);
                }
                // no more macros to substitute
            }
            return str;
        }
    }
}
