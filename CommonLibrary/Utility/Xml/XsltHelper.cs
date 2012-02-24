using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Utility.Xml
{
    public class XsltHelper
    {
        public static string FormatDate(string s)
        {
            return Utility.DateHelper.FormatDateTimeToString(PraseString2DateTime(s), Utility.DateHelper.DateFormat.MMMddyyyyspcm);
        }

        public static string FormatDateTime(string s)
        {
            return Utility.DateHelper.FormatDateTimeToString(PraseString2DateTime(s), Utility.DateHelper.DateFormat.ddMMMyyyyspwt);
        }

        private static DateTime PraseString2DateTime(string o)
        {
            DateTime t = DateTime.MinValue;
            DateTime.TryParse(o, out t);
            return t;
        }

        public static string FormatMoney(string s)
        {
            decimal d = Utility.NumberHelper.ToDecimal(s, 0);
            if (d == 0) return s;
            return d.ToString("#,###");
        }
    }
}
