using System;
using System.Collections.Generic;
using System.Text;

namespace DataMapping
{
    public static class Config
    {
        public static class Log
        {
            public static bool LogDebug = true;
            public static bool LogInfo = true;
            public static bool LogWarning = true;

            public class Logger
            {
                public static string Info = "DM_InfoLog";
                public static string Debug = "DM_DebugLog";
                public static string Warning = "DM_WarningLog";
            }
        }
    }
}
