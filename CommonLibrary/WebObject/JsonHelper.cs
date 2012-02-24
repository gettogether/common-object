using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace CommonLibrary.WebObject
{
    public class JsonHelper
    {
        private static Dictionary<string, System.Reflection.PropertyInfo[]> CachePropertyInfos = new Dictionary<string, System.Reflection.PropertyInfo[]>();

        public static string GetJsonFromList(IList list, string jsonName, string[] columnParameters)
        {
            try
            {
                if (list == null)
                {
                    return "List is null.";
                }
                if (jsonName.Length < 1)
                {
                    return "Json name is empty.";
                }
                string key = list[0].ToString();
                System.Reflection.PropertyInfo[] p = null;
                if (CachePropertyInfos.ContainsKey(key)) p = CachePropertyInfos[key] as System.Reflection.PropertyInfo[];
                if (p == null)
                {
                    p = list[0].GetType().GetProperties();
                    CachePropertyInfos[key] = p;
                }
                StringBuilder sbJson = new StringBuilder();
                sbJson.Append("{\"").Append(jsonName).Append("\":[");
                for (int i = 0; i < list.Count; i++)
                {
                    sbJson.Append("{");
                    IList TempList = new ArrayList();
                    int j = 0;
                    foreach (System.Reflection.PropertyInfo pi in p)
                    {
                        if (pi.Name.ToUpper().Equals("CONNINFO") || pi.Name.ToUpper().Equals("ITEM"))
                        {
                            continue;
                        }
                        object oo = pi.GetValue(list[i], null);
                        string columnName = pi.Name;
                        if (columnParameters == null)
                        {
                            sbJson.Append("\"").Append(columnName).Append("\":\"").Append(oo).Append("\"");
                            if (j != p.Length - 1)
                            {
                                sbJson.Append(",");
                            }
                            j++;
                        }
                        else
                        {
                            foreach (string s in columnParameters)
                            {
                                if (s.ToLower().Equals(columnName.ToLower()))
                                {
                                    sbJson.Append("\"").Append(columnName).Append("\":\"").Append(oo).Append("\"");
                                    if (j != columnParameters.Length - 1)
                                    {
                                        sbJson.Append(",");
                                    }
                                    j++;
                                }
                            }
                        }
                    }
                    if (i != list.Count - 1)
                        sbJson.Append("},");
                    else
                        sbJson.Append("}");

                }
                sbJson.Append("]}");
                return sbJson.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string GetJsonFromDataTable(DataTable table, string jsonName, string[] columnParameters)
        {
            try
            {
                if (table == null)
                {
                    return "Table is null.";
                }
                if (jsonName.Length < 1)
                {
                    return "Jason name is empty.";
                }
                if (columnParameters != null && table.Columns.Count < columnParameters.Length)
                {
                    return "Table's columns is bigger than parameter's columns!";
                }
                StringBuilder sbJson = new StringBuilder();
                sbJson.Append("{\"").Append(jsonName).Append("\":[");
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    sbJson.Append("{");
                    if (columnParameters != null)
                    {
                        for (int i = 0; i < columnParameters.Length; i++)
                        {
                            sbJson.Append("\"").Append(columnParameters[i]).Append("\":\"").Append(table.Rows[j][columnParameters[i]]).Append("\"");
                            if (i != columnParameters.Length - 1)
                            {
                                sbJson.Append(",");
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            sbJson.Append("\"").Append(table.Columns[i]).Append("\":\"").Append(table.Rows[j][i]).Append("\"");
                            if (i != table.Columns.Count - 1)
                            {
                                sbJson.Append(",");
                            }
                        }
                    }
                    if (j != table.Rows.Count - 1)
                        sbJson.Append("},");
                    else
                        sbJson.Append("}");
                }
                sbJson.Append("]}");
                return sbJson.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
