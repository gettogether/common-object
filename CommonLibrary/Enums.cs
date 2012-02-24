using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public enum ParameterType
    {
        And,
        Or,
        Initial,
    }

    public enum TokenTypes
    {
        Equal,
        IsNull,
        IsNotNull,
        Like,
        LeftLike,
        RightLike,
        LessThanEqual,
        LessThan,
        GreaterThan,
        GreaterThanEqual,
        Unknown
    }

    public class Enums
    {
        public enum SystemMode
        {
            UNDEFINE = -1,
            DEV = 0,
            UAT = 1,
            LIVE = 2,
        }
        public static SystemMode GetSystemModeFromString(string m)
        {
            m = m.Trim().ToUpper();
            switch (m)
            {
                case "DEV":
                    return SystemMode.DEV;
                case "UAT":
                    return SystemMode.UAT;
                case "LIVE":
                    return SystemMode.LIVE;
                default:
                    return SystemMode.DEV;
            }
        }
        public static string GetSystemModeText(SystemMode m)
        {
            switch (m)
            {
                case SystemMode.DEV:
                    return Resources.Resource.DEV;
                case SystemMode.UAT:
                    return Resources.Resource.UAT;
                case SystemMode.LIVE:
                    return Resources.Resource.LIVE;
                default:
                    return string.Empty;
            }
        }
        public static string GetSystemModeText(int m)
        {
            return GetSystemModeText((SystemMode)m);
        }
    }
}
