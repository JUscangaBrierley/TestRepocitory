//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Brierley.FrameWork.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateTimeUtil
    {        
        #region Properties

        public static DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        /*
         * SQL Server uses this as the min date value.
         * */
        public static DateTime MinValue
        {
            get
            {
                return new DateTime(1753, 1, 1);                
            }
        }

        public static DateTime MaxValue
        {
            get
            {
				return new DateTime(9998, 12, 31);
            }
        }
        #endregion

        #region General
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetDateOnly(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime GetBeginningOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        public static DateTime GetEndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }

        public static DateTime StripSecondsFromDates(DateTime t)
        {
            return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0, 0);
        }

        public static void GetQuarterDates(DateTime date, ref DateTime startDate, ref DateTime endDate)
        {
            int quarterNumber = (date.Month - 1) / 3 + 1;
            startDate = GetBeginningOfDay(new DateTime(date.Year, (quarterNumber - 1) * 3 + 1, 1));
            endDate = GetEndOfDay(startDate.AddMonths(3).AddDays(-1));
        }

		public static string GetLocalTimezone()
		{
			string result = null;
			switch (TimeZoneInfo.Local.BaseUtcOffset.Hours)
			{
				case -5:
					result = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? "EDT" : "EST");
					break;

				case -6:
					result = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? "CDT" : "CST");
					break;

				case -7:
					result = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? "MDT" : "MST");
					break;

				case -8:
					result = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? "PDT" : "PST");
					break;

				default:
					result = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now)
						? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName);
					break;
			}
			return result;
		}
        #endregion

        #region Comparison Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool Equal(DateTime date1, DateTime date2)
        {
            DateTime d1 = GetDateOnly(date1);
            DateTime d2 = GetDateOnly(date2);
            int result = DateTime.Compare(d1, d2);
            return (result == 0 ? true : false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool GreaterEqual(DateTime date1, DateTime date2)
        {
            DateTime d1 = GetDateOnly(date1);
            DateTime d2 = GetDateOnly(date2);
            int result = DateTime.Compare(d1, d2);
            return (result >= 0 ? true : false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool GreaterThan(DateTime date1, DateTime date2)
        {
            DateTime d1 = GetDateOnly(date1);
            DateTime d2 = GetDateOnly(date2);
            int result = DateTime.Compare(d1, d2);
            return (result > 0 ? true : false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool LessEqual(DateTime date1, DateTime date2)
        {
            DateTime d1 = GetDateOnly(date1);
            DateTime d2 = GetDateOnly(date2);
            int result = DateTime.Compare(d1, d2);
            return (result <= 0 ? true : false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool LessThan(DateTime date1, DateTime date2)
        {
            DateTime d1 = GetDateOnly(date1);
            DateTime d2 = GetDateOnly(date2);
            int result = DateTime.Compare(d1, d2);
            return (result < 0 ? true : false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="isInclusive"></param>
        /// <returns></returns>
        public static bool IsDateInBetween(DateTime date, DateTime from, DateTime to,bool isInclusive)
        {
            if (isInclusive)
            {
                if (LessEqual(from, date) && LessEqual(date, to))
                    return true;
                else
                    return false;
            }
            else
            {
                if (LessThan(from, date) && LessThan(date, to))
                    return true;
                else
                    return false;
            }
        }
        #endregion

        #region Conversion Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        public static DateTime ConvertStringToDate(string format, string dateStr)
        {
            DateTime dt;
            try
            {
                if (!string.IsNullOrEmpty(format))
                {
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                    dt = DateTime.ParseExact(dateStr, format, culture);
                }
                else
                {
                    dt = DateTime.Parse(dateStr);
                }
            }
            catch (FormatException ex)
            {
                throw new Brierley.FrameWork.Common.Exceptions.LWValidationException(string.Format("Invalid date '{0}' provided.",dateStr), ex);
            }
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDateToString(string format, DateTime dt)
        {
            string dateStr = "";
            if (!string.IsNullOrEmpty(format))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                dateStr = dt.ToString(format, culture);
            }
            else
            {
                dateStr = dt.ToString();
            }
            return dateStr;
        }

		/// <summary>
		/// Convert a DateTime into an ISO 8601 format string.  ISO 8601 is the standard format used in XML for xs:dateTime.
		/// </summary>
		/// <param name="dateTime">a DateTime object</param>
		/// <param name="ignoreTimezone">whether to omit the time zone offset from the result</param>
		/// <returns>ISO8601 format string</returns>
		public static string ConvertDateToISO8601String(DateTime dateTime, bool ignoreTimezone)
		{
			string dateStr = null;
			if (ignoreTimezone)
			{
				dateStr = dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff");
			}
			else
			{
				dateStr = dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
			}
			return dateStr;
		}

		/// <summary>
		/// Converts an ISO 8601 format string into a DateTime.  ISO 8601 is the standard format used in XML for xs:dateTime.
		/// </summary>
		/// <param name="dateStr">ISO 8601 format data string, e.g. "2001-09-11T08:46:00.000-04:00"</param>
		/// <param name="ignoreTimezone">whether to omit the time zone when converting to a DateTime</param>
		/// <returns>DateTime corresponding to the dateStr</returns>
		public static DateTime ConvertISO8601StringToDate(string dateStr, bool ignoreTimezone)
		{
			if (ignoreTimezone)
			{
				dateStr = StripISO8601TimeZoneOffset(dateStr);
			}
			DateTime dt = DateTime.Parse(dateStr);
			return dt;
		}

		/// <summary>
		/// Strips off the time zone offset component (if any) from an ISO8601 format date string
		/// </summary>
		/// <param name="dateStr">ISO 8601 format data string, e.g. "2001-09-11T08:46:00.000-04:00"</param>
		/// <returns>ISO 8601 format data string without time zone offset, e.g. "2001-09-11T08:46:00.000"</returns>
		public static string StripISO8601TimeZoneOffset(string dateStr)
		{
			if (dateStr.Length > 6 && (Regex.IsMatch(dateStr, "-[0-9][0-9]:[0-9][0-9]$") || Regex.IsMatch(dateStr, "\\+[0-9][0-9]:[0-9][0-9]$")))
			{
				dateStr = dateStr.Substring(0, dateStr.Length - 6);
			}
			else if (dateStr.Length > 1 && Regex.IsMatch(dateStr, "Z$"))
			{
				dateStr = dateStr.Substring(0, dateStr.Length - 1);
			}
			return dateStr;
		}

		public static string ConvertDateToMicrosoftWcfString(DateTime date)
		{
			JavaScriptSerializer s = new JavaScriptSerializer();
			return s.Serialize(date);
		}

		public static DateTime ConvertMicrosoftWcfStringToDate(string date)
		{
			string val = System.Text.RegularExpressions.Regex.Replace(date, @"[Date()/\\""]", string.Empty);
			JavaScriptSerializer s = new JavaScriptSerializer();
			return s.Deserialize<DateTime>(string.Format(@"""\/Date({0})\/""", val)).ToLocalTime();
		}


		#endregion
    }
}
