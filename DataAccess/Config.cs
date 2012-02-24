using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public static class Config
    {
        public static class Log
        {
            public static bool LogDebug = true;
            public static bool LogInfo = true;
            public static bool LogWarning = true;
            public static bool LogInsert = true;
            public static bool LogSelect = true;
            public static bool LogUpdate = true;
            public static bool LogDelete = true;
            public static bool LogStoredProcedure = true;

            public class Logger
            {
                public static string Info = "InfoLog";
                public static string Debug = "DebugLog";
                public static string Warning = "WarningLog";
                public static string Insert = "InsertLog";
                public static string Select = "SelectLog";
                public static string Update = "UpdateLog";
                public static string Delete = "DeleteLog";
                public static string StoredProcedure = "StoredProcedureLog";
            }
        }

        public static int CommandTimeout = 0;

        public static int BulkCopyTimeout = 0;
    }
}
