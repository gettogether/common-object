using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Utility
{
    public class IP_AddressHelper
    {
        public static List<int> GetIP_Values(string ip)
        {
            List<string> ipValuesString = new List<string>();
            ipValuesString.AddRange(ip.Split('.'));
            List<int> ipValues = CommonLibrary.Utility.ListHelper.ConvertListType<string, int>(ipValuesString);
            return ipValues;
        }

        public static bool IsAllowdIPAddress(string ip, string ipSetting)
        {
            if (string.IsNullOrEmpty(ipSetting)) return true;
            List<int> ipValues = GetIP_Values(ip);
            if (ipValues.Count != 4) return false;
            ipSetting = ipSetting.Replace(" ", "").Trim();
            foreach (string address in ipSetting.Split(';'))
            {
                if (string.IsNullOrEmpty(address)) continue;
                string[] ipInfo = address.Trim().Split('.');
                if (ipInfo.Length != 4) continue;
                bool isAllowed = false;
                for (int i = 0; i < 4; i++)
                {
                    if (ipInfo[i] == "*")
                    {
                        isAllowed = true; continue;
                    }
                    else if (ipInfo[i].IndexOf("-") > 0)
                    {
                        string[] rvsSplit = ipInfo[i].Split('-');
                        if (rvsSplit.Length != 2)
                        {
                            isAllowed = false; break;
                        }
                        List<string> rvs = new List<string>();
                        rvs.AddRange(rvsSplit);
                        List<int> rangeValues = CommonLibrary.Utility.ListHelper.ConvertListType<string, int>(rvs);
                        if (rangeValues.Count != 2)
                        {
                            isAllowed = false; break;
                        }
                        if (rangeValues[0] <= ipValues[i] && ipValues[i] <= rangeValues[1])
                        {
                            isAllowed = true; continue;
                        }
                        else
                        {
                            isAllowed = false; break;
                        }
                    }
                    else if (CommonLibrary.Utility.NumberHelper.ToInt(ipInfo[i], 0) == ipValues[i])
                    {
                        isAllowed = true; continue;
                    }
                    else
                    {
                        isAllowed = false; break;
                    }
                }
                if (isAllowed) return true;
            }
            return false;
        }
    }
}
