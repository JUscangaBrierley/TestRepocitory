using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildTestFiles
{
    public static class Global
    {
        public static string MakeFixedLength(string xstrValue, int xintLength, string FillCharacter)
        {
            string pstrValue = string.Empty;
            int pintDelta = 0;

            if (xstrValue.Length > xintLength)
            {
                xstrValue = xstrValue.Substring(1, xintLength);
            }

            try
            {
                if (xstrValue == null)
                {
                    pstrValue = string.Empty;
                    xstrValue = string.Empty;
                }
                else
                {
                    if (FillCharacter == " ")
                    {
                        pstrValue = xstrValue;
                    }
                    else
                    {
                        pstrValue = "";
                    }
                }
                if (xstrValue.Length < xintLength)
                {
                    pintDelta = xintLength - xstrValue.Length;

                    for (int pintCount = 0; pintCount < pintDelta; pintCount++)
                    {
                        pstrValue += FillCharacter;
                    }
                }
                if (FillCharacter == "0")
                {
                    pstrValue += xstrValue;
                }

                return pstrValue;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
