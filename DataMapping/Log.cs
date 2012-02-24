using System;
using System.Collections.Generic;
using System.Text;

namespace DataMapping
{
    public class Log
    {
        private static log4net.ILog mainLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool setted = false;
        public enum LogTypes
        {
            Error,
            Warning,
            Info,
            Debug,
        }

        public static void SetConfig(string url)
        {
            if (!setted)
            {
                setted = true;
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(url));
            }
        }

        public static void Error(string l)
        {
            WriteError("", l);
        }

        public static void Info(string l)
        {
            if (Config.Log.LogInfo) WriteInfo(Config.Log.Logger.Info, l);
        }

        public static void Warning(string l)
        {
            if (Config.Log.LogWarning) WriteWarn(Config.Log.Logger.Warning, l);
        }

        public static void Debug(string l)
        {
            if (Config.Log.LogDebug) WriteInfo(Config.Log.Logger.Debug, l);
        }

        public static void Write(string logger, LogTypes logType, string message)
        {
            log4net.ILog log = mainLog;
            if (!string.IsNullOrEmpty(logger))
            {
                log = log4net.LogManager.GetLogger(logger);
            }
            switch (logType)
            {
                case LogTypes.Error:
                    log.Error(message);
                    break;
                case LogTypes.Warning:
                    log.Warn(message);
                    break;
                case LogTypes.Info:
                    log.Info(message);
                    break;
                case LogTypes.Debug:
                    log.Debug(message);
                    break;
                default:
                    log.Error(message);
                    break;
            }

        }

        public static void Write(LogTypes logType, string message)
        {
            Write(null, logType, message);
        }

        public static void WriteWarn(string logger, string message)
        {
            Write(logger, LogTypes.Warning, message);
        }

        public static void WriteError(string logger, string message)
        {
            Write(logger, LogTypes.Error, message);
        }

        public static void WriteInfo(string logger, string message)
        {
            Write(logger, LogTypes.Info, message);
        }

        public static void WriteDebug(string logger, string message)
        {
            Write(logger, LogTypes.Debug, message);
        }

        public static void WriteWarn(object logger, string message)
        {
            Write(logger.ToString(), LogTypes.Warning, message);
        }

        public static void WriteError(object logger, string message)
        {
            Write(logger.ToString(), LogTypes.Error, message);
        }

        public static void WriteInfo(object logger, string message)
        {
            Write(logger.ToString(), LogTypes.Info, message);
        }

        public static void WriteDebug(object logger, string message)
        {
            Write(logger.ToString(), LogTypes.Debug, message);
        }
    }
}
