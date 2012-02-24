using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DataAccess
{
    public class Log
    {
        private static bool EnableLog = false;
        public static void InitLogging(string url)
        {
            EnableLog = true;
            DataMapping.Log.SetConfig(url);
        }

        public static string GetCommandMessage(System.Data.IDbCommand cmd)
        {
            if (cmd.CommandType == CommandType.StoredProcedure) return GenStroedProcedureMessage(cmd);
            StringBuilder sb = new StringBuilder();
            //sb.Append(cmd.CommandType.ToString());
            sb.Append("Use ").Append(cmd.Connection.Database).Append(" ");
            sb.Append("exec sp_executesql N'");
            sb.Append(cmd.CommandText.Replace("'", "''")).Append("'");
            if (cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                sb.Append(",N'");
                bool isFirst = true;
                foreach (System.Data.SqlClient.SqlParameter sp in cmd.Parameters)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                        sb.Append(",");
                    sb.Append(sp.ParameterName).Append(" ").Append(GetLogType(sp));
                }
                sb.Append("',");
                isFirst = true;
                foreach (System.Data.SqlClient.SqlParameter sp in cmd.Parameters)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    sb.Append(sp.ParameterName).Append("=").Append(GetLogValue(sp));
                }
            }
            return sb.ToString();
        }

        public static string GenStroedProcedureMessage(System.Data.IDbCommand cmd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Use ").Append(cmd.Connection.Database).Append(" ");
            sb.Append("EXEC [dbo].[").Append(cmd.CommandText).Append("] ");
            bool isFirst = true;
            foreach (System.Data.SqlClient.SqlParameter sp in cmd.Parameters)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append(sp.ParameterName).Append("=").Append(GetLogValue(sp));
            }
            return sb.ToString();
        }

        public static string GetLogValue(System.Data.SqlClient.SqlParameter sp)
        {
            bool appendN = false;
            bool appendSingleQuotes = false;
            switch (sp.SqlDbType)
            {
                case SqlDbType.BigInt:
                    break;
                case SqlDbType.Binary:
                    break;
                case SqlDbType.Bit: return (bool)sp.Value ? "1" : "0";
                case SqlDbType.Char: appendSingleQuotes = true;
                    break;
                case SqlDbType.DateTime: appendSingleQuotes = true;
                    break;
                case SqlDbType.Decimal:
                    break;
                case SqlDbType.Float:
                    break;
                case SqlDbType.Image:
                    break;
                case SqlDbType.Int:
                    break;
                case SqlDbType.Money:
                    break;
                case SqlDbType.NChar: appendSingleQuotes = true; appendN = true;
                    break;
                case SqlDbType.NText: appendSingleQuotes = true; appendN = true;
                    break;
                case SqlDbType.NVarChar: appendSingleQuotes = true; appendN = true;
                    break;
                case SqlDbType.Real:
                    break;
                case SqlDbType.SmallDateTime:
                    break;
                case SqlDbType.SmallInt:
                    break;
                case SqlDbType.SmallMoney:
                    break;
                case SqlDbType.Text: appendSingleQuotes = true; appendN = true;
                    break;
                case SqlDbType.Timestamp:
                    break;
                case SqlDbType.TinyInt:
                    break;
                case SqlDbType.Udt:
                    break;
                case SqlDbType.UniqueIdentifier:
                    break;
                case SqlDbType.VarBinary:
                    break;
                case SqlDbType.VarChar: appendSingleQuotes = true; appendN = true;
                    break;
                case SqlDbType.Variant:
                    break;
                case SqlDbType.Xml: appendSingleQuotes = true; appendN = true;
                    break;
                default:
                    break;
            }
            if (appendSingleQuotes && sp.SqlValue != null)
            {
                return string.Concat(appendN ? "N" : "", "'", sp.SqlValue.ToString().Replace("'", "''"), "'");
            }
            else
            {
                return sp.Value == null ? "NULL" : sp.Value.ToString();
            }
        }

        public static string GetLogType(System.Data.SqlClient.SqlParameter sp)
        {
            bool appendType = false;
            switch (sp.SqlDbType)
            {
                case SqlDbType.BigInt:
                    break;
                case SqlDbType.Binary:
                    break;
                case SqlDbType.Bit:
                    break;
                case SqlDbType.Char: appendType = true;
                    break;
                case SqlDbType.DateTime:
                    break;
                case SqlDbType.Decimal:
                    break;
                case SqlDbType.Float:
                    break;
                case SqlDbType.Image:
                    break;
                case SqlDbType.Int:
                    break;
                case SqlDbType.Money:
                    break;
                case SqlDbType.NChar: appendType = true;
                    break;
                case SqlDbType.NText: appendType = true;
                    break;
                case SqlDbType.NVarChar: appendType = true;
                    break;
                case SqlDbType.Real:
                    break;
                case SqlDbType.SmallDateTime:
                    break;
                case SqlDbType.SmallInt:
                    break;
                case SqlDbType.SmallMoney:
                    break;
                case SqlDbType.Text: appendType = true;
                    break;
                case SqlDbType.Timestamp:
                    break;
                case SqlDbType.TinyInt:
                    break;
                case SqlDbType.Udt:
                    break;
                case SqlDbType.UniqueIdentifier:
                    break;
                case SqlDbType.VarBinary:
                    break;
                case SqlDbType.VarChar: appendType = true;
                    break;
                case SqlDbType.Variant:
                    break;
                case SqlDbType.Xml: appendType = true;
                    break;
                default:
                    break;
            }
            if (appendType)
                return string.Concat(sp.SqlDbType, "(", sp.Size == 0 ? 1 : sp.Size, ")");
            else
                return sp.SqlDbType.ToString();
        }

        public static void LogCommand(System.Data.IDbCommand cmd)
        {
            string ctm = string.Empty;
            if (Config.CommandTimeout > 0)
            {
                if (EnableLog)
                {
                    ctm = string.Concat("Command timeout changed:", cmd.CommandTimeout, " > ", Config.CommandTimeout, "\r\n");
                }
                cmd.CommandTimeout = Config.CommandTimeout;
            }
            if (!EnableLog || cmd == null) return;
            string text = cmd.CommandText.ToLower();
            string msg = string.Concat(ctm, GetCommandMessage(cmd));
            if (cmd.CommandType == CommandType.StoredProcedure)
                StoredProcedure(msg);
            if (text.IndexOf("insert ") >= 0)
                Insert(msg);
            else if (text.IndexOf("select ") >= 0)
                Select(msg);
            else if (text.IndexOf("update ") >= 0)
                Update(msg);
            else if (text.IndexOf("delete ") >= 0)
                Delete(msg);
            else
                Select(msg);
        }

        public static void Info(string l)
        {
            if (EnableLog && Config.Log.LogInfo) DataMapping.Log.WriteInfo(Config.Log.Logger.Info, l);
        }

        public static void StoredProcedure(string l)
        {
            if (EnableLog && Config.Log.LogStoredProcedure) DataMapping.Log.WriteInfo(Config.Log.Logger.StoredProcedure, l);
        }

        public static void Warning(string l)
        {
            if (EnableLog && Config.Log.LogWarning) DataMapping.Log.WriteWarn(Config.Log.Logger.Warning, l);
        }

        public static void Debug(string l)
        {
            if (EnableLog && Config.Log.LogDebug) DataMapping.Log.WriteInfo(Config.Log.Logger.Debug, l);
        }

        public static void Insert(string l)
        {
            if (EnableLog && Config.Log.LogInsert) DataMapping.Log.WriteInfo(Config.Log.Logger.Insert, l);
        }

        public static void Select(string l)
        {
            if (EnableLog && Config.Log.LogSelect) DataMapping.Log.WriteInfo(Config.Log.Logger.Select, l);
        }

        public static void Update(string l)
        {
            if (EnableLog && Config.Log.LogUpdate) DataMapping.Log.WriteInfo(Config.Log.Logger.Update, l);
        }

        public static void Delete(string l)
        {
            if (EnableLog && Config.Log.LogDelete) DataMapping.Log.WriteInfo(Config.Log.Logger.Delete, l);
        }
    }
}
