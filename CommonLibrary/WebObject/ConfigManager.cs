using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;

namespace CommonLibrary.WebObject
{
    public class ConfigManager
    {
        #region Property
        public const string SPLIT_FLAG = "[-]";
        #endregion

        #region Function
        public static string GetConfigPath(Enums.SystemMode mode)
        {
            string folder = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Config\\");
            return string.Concat(folder, mode.ToString(), ".xml");
        }

        public static bool ReadConfig<T>(ref T t, Enums.SystemMode mode) where T : class, new()
        {
            string path = GetConfigPath(mode);
            if (System.IO.File.Exists(path))
            {
                try
                {
                    t = Utility.SerializationHelper.FromXml<T>(System.IO.File.ReadAllText(path, Encoding.UTF8));
                    return true;
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText(string.Concat(path, ".error.txt"), ex.ToString());
                    return false;
                }
            }
            return false;
        }

        public static bool WriteConfig<T>(T t, Enums.SystemMode mode) where T : class, new()
        {
            string path = GetConfigPath(mode);
            try
            {
                System.IO.File.WriteAllText(path, Utility.SerializationHelper.SerializeToXml(t));
                return true;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(string.Concat(path, ".error.txt"), ex.ToString());
                return false;
            }
        }

        public static void InitConfig<T>(ref T t, Enums.SystemMode mode) where T : class, new()
        {
            if (!ReadConfig<T>(ref t, mode))
            {
                Utility.ConfigHelper.SetConfig<T>(t);
                if (mode != Enums.SystemMode.UNDEFINE)
                {
                    WriteConfig<T>(t, mode);
                }
            }
        }

        public static int GetMode(string url, string encryptKey, out string errMsg)
        {
            string rs = Utility.RequestHelper.GetRequest(string.Concat(url, "?type=0"), 0);
            int mode = Utility.NumberHelper.ToInt(rs, -1);
            errMsg = mode == -1 ? rs : string.Empty;
            return mode;
        }

        public static ConfigInfoList GetAllConfigs(string url, string encryptKey, out string errMsg)
        {
            string result = Utility.RequestHelper.GetRequest(string.Concat(url, "?type=3"), 0);
            if (!string.IsNullOrEmpty(result))
                result = Decrypt(result, encryptKey);
            ConfigInfoList cl = new ConfigInfoList();
            ConfigInfo ci;
            string[] config;
            errMsg = string.Empty;
            foreach (string c in result.Split(new string[] { Definition.SPLIT_ROW_FLAG }, StringSplitOptions.RemoveEmptyEntries))
            {
                config = c.Split(new string[] { SPLIT_FLAG }, StringSplitOptions.None);
                if (config.Length == 3)
                {
                    ci = new ConfigInfo(config[0], config[1], config[2]);
                    cl.Add(ci);
                }
                else
                {
                    errMsg = result;
                }
            }
            return cl;
        }

        public static bool SetConfig(string url, string key, string value, string encryptKey, out string errMsg, out ConfigInfo ci)
        {
            url = string.Concat(url, "?type=2&key=", HttpUtility.UrlEncode(Encrypt(key, encryptKey)), "&value=", HttpUtility.UrlEncode(Encrypt(value, encryptKey)));
            string rs = Utility.RequestHelper.GetRequest(url, 0);
            bool isSuccess = false;
            ci = null;
            errMsg = string.Empty;
            if (!string.IsNullOrEmpty(rs))
            {
                string prefix = string.Concat(Definition.OK_FLAG, Definition.SPLIT_EXTRA_FALAG);
                if (rs.StartsWith(prefix))
                {
                    string r = rs.Substring(prefix.Length);
                    if (!string.IsNullOrEmpty(r))
                    {
                        string or = Decrypt(r, encryptKey);
                        string[] config = or.Split(new string[] { SPLIT_FLAG }, StringSplitOptions.None);
                        if (config.Length == 3)
                        {
                            ci = new ConfigInfo(config[0], config[1], config[2]);
                            isSuccess = true;
                        }
                    }
                }
                else
                {
                    errMsg = rs;
                }
            }
            else
            {
                errMsg = "The specify key get empty!";
            }
            return isSuccess;
        }

        public static ConfigInfo GetConfig(string url, string key, string encryptKey, out string errMsg)
        {
            url = string.Concat(url, "?type=1&key=", HttpUtility.UrlEncode(Encrypt(key, encryptKey)));
            string result = Utility.RequestHelper.GetRequest(url, 0);
            ConfigInfo ci = null;
            errMsg = string.Empty;
            if (!string.IsNullOrEmpty(result))
            {
                result = Decrypt(result, encryptKey);
                string[] config = result.Split(new string[] { SPLIT_FLAG }, StringSplitOptions.None);
                if (config.Length == 3)
                {
                    ci = new ConfigInfo(config[0], config[1], config[2]);
                }
                else
                {
                    errMsg = result;
                }
            }
            return ci;
        }

        private static string Decrypt(string text, string encrKey)
        {
            return Utility.SecretHelper.DesDecrypt(text, encrKey);
        }

        private static string Encrypt(string text, string encrKey)
        {
            return Utility.SecretHelper.DesEncrypt(text, encrKey);
        }
        #endregion

        #region Defination
        public class ConfigInfo
        {
            public enum Columns
            {
                Key,
                Type,
                Value,
            }
            public ConfigInfo()
            {

            }
            public ConfigInfo(string key, string type, string value)
            {
                this._Key = key;
                this._Type = type;
                this._Value = value;
            }
            private string _Key;

            public string Key
            {
                get { return _Key; }
                set { _Key = value; }
            }
            private string _Type;

            public string Type
            {
                get { return _Type; }
                set { _Type = value; }
            }
            private string _Value;

            public string Value
            {
                get { return _Value; }
                set { _Value = value; }
            }
        }
        public class ConfigInfoList : CommonLibrary.ObjectBase.ListBase<ConfigInfo>
        {
            public ConfigInfoList()
            {

            }
        }
        #endregion
    }
}
