using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Utility
{
    public static class DateHelper
    {
        public enum DateFormat
        {
            /// <summary>
            /// dd/MM/yyyy
            /// </summary>
            ddMMyyyys,
            /// <summary>
            /// dd/MM/yyyy HH:mm
            /// </summary>
            ddMMyyyysHHmm,
            /// <summary>
            /// ddMMyyyy
            /// </summary>
            ddMMyyyyn,
            /// <summary>
            /// dd MM yyyy
            /// </summary>
            ddMMyyyysp,
            /// <summary>
            /// dd-MM-yyyy
            /// </summary>
            ddMMyyyyd,
            /// <summary>
            /// dd/MMM/yyyy
            /// </summary>
            ddMMMyyyys,
            /// <summary>
            /// ddMMMyyyy
            /// </summary>
            ddMMMyyyyn,
            /// <summary>
            /// dd MMM yyyy
            /// </summary>
            ddMMMyyyysp,
            /// <summary>
            /// dd-MMM-yyyy
            /// </summary>
            ddMMMyyyyd,
            /// <summary>
            /// MMM dd, yyyy
            /// </summary>
            MMMddyyyyspcm,
            /// <summary>
            /// dd MMM yyyy HH:mm:ss
            /// </summary>
            ddMMMyyyyspwt,
            /// <summary>
            /// dd MMM yyyy HH:mm
            /// </summary>
            ddMMMyyyyHHmm,
            /// <summary>
            /// dd MMM HH:mm:ss
            /// </summary>
            ddMMMspwt,
            /// <summary>
            /// yyyyMMMdd
            /// </summary>
            yyyyMMMdd,
            /// <summary>
            /// yyyyMMdd
            /// </summary>
            yyyyMMdd,
            /// <summary>
            /// yyMMddHHmm
            /// </summary>
            yyMMddHHmm,
            /// <summary>
            /// yyMMddHHmm
            /// </summary>
            yyMMddHHmms,
            /// <summary>
            /// MMM dd
            /// </summary>
            MMMdd,
            /// <summary>
            /// dd MMM,yyyy (ddd)
            /// </summary>
            ddMMMyyyyddd,
            //MMMddyyyyq //single quote eg:Oct 31' 08 quote
            /// <summary>
            ///  //Muti-Language dd MMM
            /// </summary>
            ddMMMml,
            /// <summary>
            /// Muti-Language dd MMM yyyy
            /// </summary>
            ddMMMyyyyml,
            /// <summary>
            /// yyyy-MM-dd eg:1990-10-05
            /// </summary>
            yyyyMMddd,
            /// <summary>
            /// yyyy-M-dd eg:1990-8-15
            /// </summary>
            yyyyMddd,
            /// <summary>
            /// ddMMMyyyy eg:11APR2010
            /// </summary>
            ddMMMyyyy,
            /// <summary>
            /// ddMMMyyyy eg:APR2010
            /// </summary>
            MMMyyyy,
            /// <summary>
            /// dd MMM,yyyy
            /// </summary>
            dd_space_MMM_comma_yyyy,
        }
        /// <summary>
        /// define special time of the day,Morning:00:00:00,Noon:00:00:00,Night:23:59:59
        /// </summary>
        public enum DaySegment
        {
            Morning = 0,
            Noon = 1,
            Night = 2,

        }
        public static string GetDateString(DateFormat xDateFormat)
        {
            string myDateString = string.Empty;
            switch (xDateFormat)
            {
                case DateFormat.ddMMyyyys:
                    myDateString = Resources.Date.ddMMyyyys;
                    break;
                case DateFormat.ddMMyyyyn:
                    myDateString = Resources.Date.ddMMyyyyn;
                    break;
                case DateFormat.ddMMyyyysp:
                    myDateString = Resources.Date.ddMMyyyysp;
                    break;
                case DateFormat.ddMMyyyyd:
                    myDateString = Resources.Date.ddMMyyyyd;
                    break;
                case DateFormat.ddMMMyyyys:
                    myDateString = Resources.Date.ddMMMyyyys;
                    break;
                case DateFormat.ddMMMyyyyn:
                    myDateString = Resources.Date.ddMMMyyyyn;
                    break;
                case DateFormat.ddMMMyyyysp:
                    myDateString = Resources.Date.ddMMMyyyysp;
                    break;
                case DateFormat.ddMMMyyyyd:
                    myDateString = Resources.Date.ddMMMyyyyd;
                    break;
                case DateFormat.MMMddyyyyspcm:
                    myDateString = Resources.Date.MMMddyyyyspcm;
                    break;
                case DateFormat.ddMMMyyyyspwt:
                    myDateString = Resources.Date.ddMMMyyyyspwt;
                    break;
                case DateFormat.ddMMMyyyyHHmm:
                    myDateString = Resources.Date.ddMMMyyyyHHmm;
                    break;
                case DateFormat.yyyyMMMdd:
                    myDateString = Resources.Date.yyyyMMMdd;
                    break;
                case DateFormat.yyyyMMdd:
                    myDateString = Resources.Date.yyyyMMdd;
                    break;
                case DateFormat.yyyyMMddd:
                    myDateString = Resources.Date.yyyyMMddd;
                    break;
                case DateFormat.yyMMddHHmm:
                    myDateString = Resources.Date.yyMMddHHmm;
                    break;
                case DateFormat.MMMdd:
                    myDateString = Resources.Date.MMMdd;
                    break;
                case DateFormat.ddMMMyyyyddd:
                    myDateString = Resources.Date.ddMMMyyyyddd;
                    break;
                case DateFormat.ddMMMyyyyml:
                    myDateString = Resources.Date.ddMMMyyyyml.ToString();
                    break;
                case DateFormat.ddMMMml:
                    myDateString = Resources.Date.ddMMMml.ToString();
                    break;
                case DateFormat.ddMMMspwt:
                    myDateString = Resources.Date.ddMMMspwt;
                    break;
                case DateFormat.yyMMddHHmms:
                    myDateString = Resources.Date.ddMMMspwt;
                    break;
                //case DateFormat.MMMddyyyyq:
                //    myDateString = "MMM dd'yyyy";
                //    break;
                case DateFormat.ddMMMyyyy:
                    myDateString = Resources.Date.ddMMMyyyy;
                    break;
                case DateFormat.MMMyyyy:
                    myDateString = Resources.Date.MMMyyyy;
                    break;
                case DateFormat.dd_space_MMM_comma_yyyy:
                    myDateString = Resources.Date.dd_space_MMM_comma_yyyy;
                    break;
                default:
                    myDateString = Resources.Date.GetResource(xDateFormat.ToString());
                    break;
            }
            return myDateString;
        }

        #region Public Functions
        public static DateTime ConvertDate(string xDateString, DateFormat xDateFormat)
        {
            return ConvertDate(xDateString, xDateFormat, false);
        }
        public static DateTime ConvertDate(string xDateString, DateFormat xDateFormat, bool isMutiLang)
        {
            if (string.IsNullOrEmpty(xDateString)) return DateTime.MinValue;
            switch (xDateFormat)
            {
                case DateFormat.ddMMyyyys:
                    string[] dateInfo = xDateString.Split('/');
                    string dd = string.Empty;
                    string MM = string.Empty;
                    string yyyy = string.Empty;
                    if (dateInfo.Length == 3)
                    {
                        dd = dateInfo[0].Trim();
                        if (dd.Length != 2)
                        {
                            if (dd.Length > 2)
                            {
                                dd = dd.Substring(0, 2);
                            }
                            else if (dd.Length == 1 && dd != "0")
                            {
                                dd = "0" + dd;
                            }
                            else
                            {
                                dd = "01";
                            }
                        }
                        else if (dd == "00")
                        {
                            dd = "01";
                        }
                        MM = dateInfo[1].Trim();
                        if (MM.Length != 2)
                        {
                            if (MM.Length > 2)
                            {
                                MM = MM.Substring(0, 2);
                            }
                            else if (MM.Length == 1)
                            {
                                MM = "0" + MM;
                            }
                            else
                            {
                                MM = "01";
                            }
                        }
                        else if (MM == "00")
                        {
                            MM = "01";
                        }
                        yyyy = dateInfo[2].Trim();
                        if (yyyy.Length == 2)
                        {
                            yyyy += "20";
                        }
                        else if (yyyy.Length != 4)
                        {
                            if (yyyy.Length > 4)
                            {
                                yyyy = yyyy.Substring(0, 4);
                            }
                            else
                            {
                                yyyy = DateTime.Now.ToString("yyyy");
                            }
                        }
                        else if (yyyy == "0000")
                        {
                            yyyy = DateTime.Now.ToString("yyyy");
                        }
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                    xDateString = string.Format("{0}/{1}/{2}", dd, MM, yyyy);
                    break;
                case DateFormat.ddMMyyyyn:
                    break;
                case DateFormat.ddMMyyyysp:
                    break;
                case DateFormat.ddMMyyyyd:
                    break;
                case DateFormat.ddMMMyyyys:
                    break;
                case DateFormat.ddMMMyyyyn:
                    break;
                case DateFormat.ddMMMyyyysp:
                    break;
                case DateFormat.ddMMMyyyyd:
                    break;
                case DateFormat.MMMddyyyyspcm:
                    break;
                case DateFormat.ddMMMyyyyspwt:
                    break;
                case DateFormat.ddMMMyyyyHHmm:
                    break;
                case DateFormat.yyyyMMMdd:
                    break;
                case DateFormat.yyyyMMdd:
                    break;
                case DateFormat.yyMMddHHmm:
                    break;
                case DateFormat.MMMdd:
                    break;
                case DateFormat.ddMMMyyyyddd:
                    break;
                case DateFormat.ddMMMyyyyml:
                    break;
                default:
                    break;
            }
            MutiLanguage.Languages lang = MutiLanguage.GetCultureType();
            lang = isMutiLang ? lang : MutiLanguage.Languages.en_us;
            string langstr = isMutiLang ? MutiLanguage.EnumToString(lang) : MutiLanguage.EnumToString(MutiLanguage.Languages.en_us);
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(Resources.Date));
            System.Resources.ResourceSet rs = rm.GetResourceSet(new System.Globalization.CultureInfo(langstr), true, true);
            return DateTime.ParseExact(xDateString, rs.GetString(xDateFormat.ToString()), System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }
        /// <summary>
        /// Get DateTime,the time only can be
        /// </summary>
        /// <param name="xDateString"></param>
        /// <param name="xDateFormat"></param>
        /// <param name="xdaySegment"></param>
        /// <returns></returns>
        public static DateTime ConvertDate(string xDateString, DateFormat xDateFormat, DaySegment xdaySegment)
        {
            return GetAbsDate(ConvertDate(xDateString, xDateFormat), xdaySegment);
        }
        public static string FormatDateTimeToString(DateTime xDateTime, DateFormat xDateFormat, bool isMutiLang)
        {
            if (xDateTime == null || Convert.ToDateTime(xDateTime) == DateTime.MinValue) return "";
            MutiLanguage.Languages lang = MutiLanguage.GetCultureType();
            lang = isMutiLang ? lang : MutiLanguage.Languages.en_us;
            string langstr = isMutiLang ? MutiLanguage.EnumToString(lang) : MutiLanguage.EnumToString(MutiLanguage.Languages.en_us);
            //System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(Resources.Date));
            //System.Resources.ResourceSet rs = rm.GetResourceSet(new System.Globalization.CultureInfo(langstr), true, true);
            //return xDateTime.ToString(rs.GetString(xDateFormat.ToString()), System.Globalization.DateTimeFormatInfo.InvariantInfo);
            return xDateTime.ToString(Resources.Date.ResourceManager.GetString(xDateFormat.ToString(), new System.Globalization.CultureInfo(langstr)), System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }
        public static string FormatDateTimeToString(DateTime xDateTime, DateFormat xDateFormat, MutiLanguage.Languages lang)
        {
            if (xDateTime == null || Convert.ToDateTime(xDateTime) == DateTime.MinValue) return "";
            string langstr = MutiLanguage.EnumToString(lang);
            //System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(Resources.Date));
            //System.Resources.ResourceSet rs = rm.GetResourceSet(new System.Globalization.CultureInfo(langstr), false, false);
            //return xDateTime.ToString(rs.GetString(xDateFormat.ToString()), System.Globalization.DateTimeFormatInfo.InvariantInfo);
            return xDateTime.ToString(Resources.Date.ResourceManager.GetString(xDateFormat.ToString(), new System.Globalization.CultureInfo(langstr)), System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }
        public static string FormatDateTimeToString(DateTime xDateTime, DateFormat xDateFormat)
        {
            return FormatDateTimeToString(xDateTime, xDateFormat, false);
        }
        public static string FormatDateToString(object xDateTime, DateFormat xDateFormat)
        {
            return FormatDateTimeToString(Convert.ToDateTime(xDateTime), xDateFormat, false);
        }
        public static string FormatDateToString(object xDateTime, DateFormat xDateFormat, bool isMutiLang)
        {
            return FormatDateTimeToString(Convert.ToDateTime(xDateTime), xDateFormat, isMutiLang);
        }
        public static string FormatDateToString(object xDateTime, DateFormat xDateFormat, MutiLanguage.Languages lang)
        {
            return FormatDateTimeToString(Convert.ToDateTime(xDateTime), xDateFormat, lang);
        }
        public static bool IsNullOrMinValue(DateTime xDateTime)
        {
            return (xDateTime == null || xDateTime == DateTime.MinValue);
        }
        public static bool IsNullOrMaxValue(DateTime xDateTime)
        {
            return (xDateTime == null || xDateTime == DateTime.MaxValue);
        }
        public static bool IsBusinessDay(DateTime dtIn)
        {
            switch (dtIn.DayOfWeek)
            {
                case DayOfWeek.Sunday: return false;
                case DayOfWeek.Saturday: return false;
            }
            return true;
        }
        public static DateTime NextBusinessDay(DateTime dtIn, int offset)
        {
            DateTime dtRet = dtIn.AddDays(offset);
            int increment = (offset > 0) ? 1 : -1;
            offset = Math.Abs(offset);

            if (!IsBusinessDay(dtRet))
            {
                while (true)
                {
                    dtRet = dtRet.AddDays(increment);
                    if (IsBusinessDay(dtRet)) { offset--; break; }
                }
            }

            dtRet = dtRet.AddDays(increment * (offset / 5));
            return dtRet;
        }
        public static int GetDateTimeWeekDay(DateTime date)
        {
            int y = date.Year;
            int m = date.Month;
            int d = date.Day;
            return GetDateTimeWeekDay(y, m, d);
        }
        public static int GetDateTimeWeekDay(int y, int m, int d)
        {
            if (m == 1)
            {
                m = 13;
                y = y - 1;
            }
            if (m == 2)
            {
                m = 14;
                y = y - 1;
            }
            int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
            week = week + 1;
            return week;
        }

        public static string GetMonthString(byte month)
        {
            string monthString = string.Empty;
            switch (month)
            {
                case 1:
                    monthString = Resources.Date.Jan;
                    break;
                case 2:
                    monthString = Resources.Date.Feb;
                    break;
                case 3:
                    monthString = Resources.Date.Mar;
                    break;
                case 4:
                    monthString = Resources.Date.Apr;
                    break;
                case 5:
                    monthString = Resources.Date.May;
                    break;
                case 6:
                    monthString = Resources.Date.Jun;
                    break;
                case 7:
                    monthString = Resources.Date.Jul;
                    break;
                case 8:
                    monthString = Resources.Date.Aug;
                    break;
                case 9:
                    monthString = Resources.Date.Sep;
                    break;
                case 10:
                    monthString = Resources.Date.Oct;
                    break;
                case 11:
                    monthString = Resources.Date.Nov;
                    break;
                case 12:
                    monthString = Resources.Date.Dec;
                    break;
            }
            return monthString;
        }

        public static string GetWeekdayString(DateTime weekday)
        {
            int i = GetDateTimeWeekDay(weekday);
            string weekString = "";
            switch (i)
            {
                case 1:
                    weekString = Resources.Date.Monday;
                    break;
                case 2:
                    weekString = Resources.Date.Tuesday;
                    break;
                case 3:
                    weekString = Resources.Date.Webnesday;
                    break;
                case 4:
                    weekString = Resources.Date.Thursday;
                    break;
                case 5:
                    weekString = Resources.Date.Friday;
                    break;
                case 6:
                    weekString = Resources.Date.Saturday;
                    break;
                case 7:
                    weekString = Resources.Date.Sunday;
                    break;
            }
            return weekString;
        }

        public static string GetWeekdayStringShortFormat(DateTime weekday)
        {
            int i = GetDateTimeWeekDay(weekday);
            string weekString = "";
            switch (i)
            {
                case 1:
                    weekString = Resources.Date.Mon;
                    break;
                case 2:
                    weekString = Resources.Date.Tue;
                    break;
                case 3:
                    weekString = Resources.Date.Web;
                    break;
                case 4:
                    weekString = Resources.Date.Thu;
                    break;
                case 5:
                    weekString = Resources.Date.Fri;
                    break;
                case 6:
                    weekString = Resources.Date.Sat;
                    break;
                case 7:
                    weekString = Resources.Date.Sun;
                    break;
            }
            return weekString;
        }

        public static string GetWeekdayString(byte weekday)
        {
            string[] weekdayStrings = new string[] { Resources.Date.Monday,
            Resources.Date.Tuesday, Resources.Date.Webnesday,
           Resources.Date.Thursday, Resources.Date.Friday,
            Resources.Date.Saturday, Resources.Date.Sunday };
            return GetWeekdayStr(weekday, weekdayStrings);
        }
        public static string GetWeekdayStringShortFormat(byte weekday)
        {
            string[] weekdayStrings = new string[] { Resources.Date.Mon,
            Resources.Date.Tue, Resources.Date.Web,
           Resources.Date.Thu, Resources.Date.Fri,
            Resources.Date.Sat, Resources.Date.Sun };
            return GetWeekdayStr(weekday, weekdayStrings);
        }
        private static string GetWeekdayStr(byte weekday, string[] weekdayStrings)
        {
            string ret = "";
            int start = -1, i;
            for (i = 0; i < 7; i++)
            {
                if ((weekday & ((byte)Math.Pow(2, i))) > 0)
                {
                    if (start < 0) start = i;
                }
                else
                {
                    if (start >= 0)
                    {
                        if (ret.Length > 0) ret += ", ";
                        ret += weekdayStrings[start];
                        if (start != i - 1 && i > 0) ret += "-" + weekdayStrings[i - 1];
                        start = -1;
                    }
                }
            }
            if (start >= 0)
            {
                if (ret.Length > 0) ret += ", ";
                ret += weekdayStrings[start];
                if (start != i - 1 && i > 0) ret += "-" + weekdayStrings[i - 1];
            }
            return ret;
        }
        public static byte EncodeWeekdayString(string sWeekday)
        {
            string sLookup = "1234567";
            byte ret = 0;
            foreach (char c in sWeekday.ToCharArray())
            {
                int idx = sLookup.IndexOf(c);
                if (idx < 0 || idx >= 7) return 0;
                ret |= (byte)Math.Pow(2, idx);
            }
            return ret;
        }

        public static bool isLeepYear(int year)
        {
            return (year % 4 == 0) && ((year % 100 != 0) || (year % 400 == 0));
        }

        public static int GetMonthDay(DateTime dateTime)
        {
            switch (dateTime.Month)
            {
                case 1: return 31;
                case 2: if (DateTime.IsLeapYear(dateTime.Year)) return 29; else return 28;
                case 3: return 31;
                case 4: return 30;
                case 5: return 31;
                case 6: return 30;
                case 7: return 31;
                case 8: return 31;
                case 9: return 30;
                case 10: return 31;
                case 11: return 30;
                case 12: return 31;
                default: return 0;
            }
        }
        /// <summary>
        /// get the ealist time or latest time of the given date
        /// </summary>
        /// <param name="dateTime">Date time</param>
        /// <param name="xDaySegment"> DaySegment.Morning:time with 00:00:00,DaySegment.Night: time with 23:59:59,DaySegment.Noon:time with 12:00:00</param>
        /// <returns></returns>
        public static DateTime GetAbsDate(DateTime dateTime, DaySegment xDaySegment)
        {
            DateTime dt = new DateTime();
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue)
                return dateTime;
            switch (xDaySegment)
            {
                case DaySegment.Morning:
                    dt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
                    break;
                case DaySegment.Noon:
                    dt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 12, 0, 0);
                    break;
                case DaySegment.Night:
                    dt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
                    break;

            }
            return dt;
        }

        public static string GetTimeString(DateTime xDateTime)
        {
            return xDateTime.ToString("HH:mm");
        }
        public static string GetTimeString(TimeSpan xDateTime)
        {
            return string.Concat(xDateTime.Hours.ToString("00"), xDateTime.Minutes.ToString("00"));
        }
        public static TimeSpan ConvertTime(string time)
        {
            int h = 0, m = 0, s = 0;
            if (IsTime(time))
            {
                string[] ts = time.Split(':');
                if (ts != null)
                {
                    if (ts.Length > 0) int.TryParse(ts[0], out h);
                    if (ts.Length > 1) int.TryParse(ts[1], out m);
                    if (ts.Length > 2) int.TryParse(ts[2], out s);
                }
            }
            TimeSpan t = new TimeSpan(h, m, s);
            return t;
        }
        public static bool IsTime(string time)
        {
            try
            {
                int h, m, s;
                string[] ts = time.Split(':');
                if (ts != null && ts.Length > 1)
                {
                    h = int.Parse(ts[0]);
                    if (h < 0 || h > 23) return false;
                    m = int.Parse(ts[1]);
                    if (m < 0 || m > 59)
                        return false;
                    if (ts.Length > 2)
                    {
                        s = int.Parse(ts[2]);
                        if (s < 0 || s > 59)
                            return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static DateTime ConvertDate(string xDateString, DateFormat xDateFormat, string xTimeString)
        {
            DateTime xDate = ConvertDate(xDateString, xDateFormat);
            TimeSpan xTime = ConvertTime(xTimeString);
            return new DateTime(xDate.Year, xDate.Month, xDate.Day, xTime.Hours, xTime.Minutes, xTime.Seconds);
        }

        public static DateTime GetFirstOrLastDayOfMonth(DateTime datetime, bool isNextMonth, bool isFirstDay)
        {
            return GetFirstOrLastDayOfMonth(datetime, isNextMonth ? 1 : -1, isFirstDay);
        }
        public static DateTime GetFirstOrLastDayOfMonth(DateTime datetime, int monthCount, bool isFirstDay)
        {
            return isFirstDay ? datetime.AddDays(1 - datetime.Day).AddMonths(monthCount) : datetime.AddDays(1 - datetime.Day).AddMonths(monthCount + 1).AddDays(-1);
        }

        public static int GetAge(DateTime now, DateTime birthDay)
        {
            int age = 1;
            now = now == DateTime.MinValue ? DateTime.Now : now;
            age = now.Year - birthDay.Year;
            if (age <= 1)
            {
                age = 1;
            }
            else
            {
                if (birthDay.Month > now.Month)
                {
                    age--;
                }
                else if (birthDay.Month == now.Month && birthDay.Day >= now.Day)
                {
                    age--;
                }
            }
            return age;
        }
        #endregion
    }
}
