using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonLibrary.Utility
{
    public class NumberHelper
    {
        #region Enums

        public enum CalcOperator
        {
            Add = 0,
            Subtract = 1,
            Multiply = 2,
            Divide = 3,
        }

        public enum RateOperator
        {
            Multiply = 0,
            Divide = 1,
        }

        public enum RoundingMethod
        {
            Disabled = 0,
            Dollars = 1,
            Ten_Dollars = 2,
            Ten_Cent = 3,
        }

        public enum RoundingTypes
        {
            Ceiling,
            Floor,
            Round
        }

        #endregion

        public static int ToInt(string s, int default_value)
        {
            try
            {
                return Int32.Parse(s);
            }
            catch
            {
                return default_value;
            }
        }

        public static decimal ToDecimal(string s, decimal default_value)
        {
            try
            {
                return decimal.Parse(s);
            }
            catch
            {
                return default_value;
            }
        }

        public static bool IsNumeric(string sValue)
        {
            Regex r = new Regex(@"^(-)?\d+(\.)?\d*$");
            if (r.IsMatch(sValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsInteger(string sValue)
        {
            return new Regex(@"\d+").IsMatch(sValue);
        }

        public static string ToMoneyString(decimal d)
        {
            string ret = string.Format("${0:###,###,###.##}", d);
            if (ret == "$")
                return "$0.00";
            else
                return ret;
        }
        public static string ToMoneyStringZero2Line(decimal d)
        {
            if (d <= 0) return "-";
            return ToMoneyString(d);
        }
        public static string ToMoney(decimal d)
        {
            string ret = string.Format("{0:###,###,###.##}", d);
            if (string.IsNullOrEmpty(ret))
                ret = "0";
            return ret;
        }

        public static double Rounding(double dMarkups, RoundingTypes roundingType, int roundingDigit)
        {
            if (dMarkups <= 0)
                return dMarkups;
            dMarkups = dMarkups / Math.Pow(10, roundingDigit);
            switch (roundingType)
            {
                case RoundingTypes.Ceiling: dMarkups = Math.Ceiling(dMarkups); break;
                case RoundingTypes.Floor: dMarkups = Math.Floor(dMarkups); break;
                case RoundingTypes.Round: dMarkups = Math.Round(dMarkups); break;
            }
            return dMarkups * Math.Pow(10, roundingDigit);
        }
        public static decimal Rounding(decimal d, RoundingTypes roundingType, int roundingDigit)
        {
            return (decimal)Rounding((double)d, roundingType, roundingDigit);
        }

        public static decimal Rounding(decimal d, RoundingMethod roundingMethod)
        {
            switch (roundingMethod)
            {
                case RoundingMethod.Disabled:
                    return d;
                case RoundingMethod.Dollars:
                    return Rounding(d, RoundingTypes.Ceiling, 0);
                case RoundingMethod.Ten_Dollars:
                    return Rounding(d, RoundingTypes.Ceiling, 1);
                case RoundingMethod.Ten_Cent:
                    return Rounding(d, RoundingTypes.Ceiling, -1);
                default:
                    return d;
            }
        }

        public static decimal Calc(decimal dOne, decimal dTwo, CalcOperator calcOperator)
        {
            decimal d = (decimal)0;
            switch (calcOperator)
            {
                case CalcOperator.Add:
                    d = dOne + dTwo;
                    break;
                case CalcOperator.Subtract:
                    d = dOne - dTwo;
                    break;
                case CalcOperator.Multiply:
                    d = dOne * dTwo;
                    break;
                case CalcOperator.Divide:
                    d = dOne / dTwo;
                    break;
            }
            return d;
        }

        public static decimal CalcRate(decimal dBase, decimal rate, RateOperator rateOperator)
        {
            decimal d = (decimal)0;
            switch (rateOperator)
            {
                case RateOperator.Multiply:
                    d = dBase * rate;
                    break;
                case RateOperator.Divide:
                    if (rate == 1)
                        d = dBase;
                    else
                        d = dBase / (1 - rate) - dBase;
                    break;
            }
            return d;
        }
    }
}
