using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;

namespace CommonLibrary.Extension
{
    public class SoapLogs : SoapExtension
    {
        Stream _OldStream;
        Stream _NewStream;
        string LogsFolder = CommonLibrary.Utility.ConfigHelper.GetAppSetting("SoapLogs", "D:\\Logs\\Soap");
        const string LogSplit = "-------------------------------------------------------------------------------------------";

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return "";
            //return ((SoapTraceExtensionAttribute)attribute).Priority;
        }

        public override object GetInitializer(Type serviceType)
        {
            return serviceType.GetType();
        }

        public override void Initialize(object initializer)
        {

        }

        public static string GetValueForSplit(string v, char separator)
        {
            if (string.IsNullOrEmpty(v)) return v;
            string[] vi = v.Split('/');
            return vi[vi.Length - 1];
        }

        public override void ProcessMessage(SoapMessage msg)
        {
            try
            {
                string logFile = Path.Combine(LogsFolder, DateTime.Now.ToString("yyyy-MM-dd"));
                if (!string.IsNullOrEmpty(msg.Url))
                {
                    logFile = Path.Combine(logFile, GetValueForSplit(msg.Url, '/').Split('.')[0]);
                }
                if (!System.IO.Directory.Exists(logFile)) System.IO.Directory.CreateDirectory(logFile);
                string action = "";
                try
                {
                    action = msg.Action;
                    if (!string.IsNullOrEmpty(action))
                    {
                        logFile = Path.Combine(logFile, GetValueForSplit(action, '/'));
                    }
                }
                catch //(Exception ex)
                {
                    //string errorFile = Path.Combine(logFile, "Error.txt");
                    //if (!System.IO.File.Exists(errorFile)) System.IO.File.Create(errorFile);
                    //using (FileStream fs = new FileStream(errorFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    //{
                    //    StreamWriter w = new StreamWriter(fs);
                    //    w.WriteLine(LogSplit);
                    //    w.WriteLine(DateTime.Now);
                    //    w.WriteLine();
                    //    w.WriteLine(ex.ToString());
                    //    w.Flush();
                    //}
                    action = "No-Action";
                    logFile = Path.Combine(logFile, action);
                }
                logFile += ".txt";
                using (FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    StreamWriter w = new StreamWriter(fs);
                    w.WriteLine(LogSplit);
                    w.WriteLine(string.Concat("Message in: ", msg.Stage.ToString()));
                    w.WriteLine(string.Concat("Action: ", action));
                    w.WriteLine(string.Concat("URL: ", msg.Url));
                    w.WriteLine(DateTime.Now);
                    w.WriteLine();
                    w.WriteLine("Content Start:");
                    w.Flush();
                }
                using (FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {

                    if (msg.Stage == SoapMessageStage.AfterSerialize)
                    {
                        Stream memStream;
                        if (_NewStream.CanSeek)
                        {
                            memStream = _NewStream;
                        }
                        else
                        {
                            memStream = new MemoryStream(((MemoryStream)_NewStream).ToArray());
                        }
                        memStream.Position = 0;
                        CopyStream(memStream, fs);
                        memStream.Position = 0;
                        CopyStream(memStream, _OldStream);
                    }
                    else if (msg.Stage == SoapMessageStage.BeforeDeserialize)
                    {
                        CopyStream(_OldStream, _NewStream);
                        _NewStream.Position = 0;
                        CopyStream(_NewStream, fs);
                        _NewStream.Position = 0;
                    }
                }
                using (FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    StreamWriter w = new StreamWriter(fs);
                    w.WriteLine("Content End");
                    w.WriteLine();
                    w.Flush();
                }
            }
            catch
            {
            }
        }

        public override Stream ChainStream(Stream stream)
        {
            _OldStream = stream;
            _NewStream = new MemoryStream();
            return _NewStream;
        }

        void CopyStream(Stream from, Stream to)
        {
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SoapLogsAttribute : SoapExtensionAttribute
    {
        private int _Priority;
        public override int Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }

        public override Type ExtensionType
        {
            get { return typeof(SoapLogs); }
        }
    }
}
