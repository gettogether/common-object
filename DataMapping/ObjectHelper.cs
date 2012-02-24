using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using DataMapping;

namespace DataMapping
{
    public static class ObjectHelper
    {
        #region Private Functions

        private static List<PropertyMappingInfo> LoadPropertyMappingInfo(Type objType)
        {
            List<PropertyMappingInfo> mapInfoList = new List<PropertyMappingInfo>();
            foreach (PropertyInfo info in objType.GetProperties())
            {
                MappingAttribute mapAttr = (MappingAttribute)Attribute.GetCustomAttribute(info, typeof(MappingAttribute));
                if (mapAttr == null && (System.Type.GetTypeCode(info.PropertyType) != TypeCode.Object || info.PropertyType == new System.TimeSpan().GetType()))
                {
                    //mapAttr = new MappingAttribute(string.Concat(info.Name, ",un-insert,un-update"));
                    mapAttr = new MappingAttribute(info.Name);
                }
                if (mapAttr != null)
                {
                    PropertyMappingInfo mapInfo = new PropertyMappingInfo(mapAttr.DataFieldName, mapAttr.NullValue, info);
                    mapInfoList.Add(mapInfo);
                }
            }
            return mapInfoList;
        }

        private static int[] GetOrdinals(List<PropertyMappingInfo> propMapList, IDataReader dr)
        {
            int[] ordinals = new int[propMapList.Count];
            if (dr != null)
            {
                for (int i = 0; i <= propMapList.Count - 1; i++)
                {
                    ordinals[i] = -1;
                    try
                    {
                        ordinals[i] = dr.GetOrdinal(propMapList[i].DataFieldName);
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }
                }
            }

            return ordinals;
        }

        private static T CreateObject<T>(IDataReader dr, List<PropertyMappingInfo> propInfoList, int[] ordinals) where T : class, new()
        {
            T obj = new T();
            for (int i = 0; i <= propInfoList.Count - 1; i++)
            {

                Type type = propInfoList[i].PropertyInfo.PropertyType;
                object value = propInfoList[i].DefaultValue;
                try
                {
                    if (ordinals[i] != -1)
                    {
                        value = dr.GetValue(ordinals[i]);
                        try
                        {
                            if (DBNull.Value != value)
                                propInfoList[i].PropertyInfo.SetValue(obj, value, null);
                        }
                        catch
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(value.ToString()))
                                {
                                    if (type.BaseType.Equals(typeof(System.Enum)))
                                    {
                                        propInfoList[i].PropertyInfo.SetValue(obj, System.Enum.ToObject(type, value), null);
                                    }
                                    else
                                    {
                                        propInfoList[i].PropertyInfo.SetValue(obj, Convert.ChangeType(value, type), null);
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    else
                    {
                        if (type == typeof(Int32) || type == typeof(int) || type == typeof(Int16) || type == typeof(Int64) || type == typeof(Decimal) || type == typeof(Double))
                            value = 0;
                        else if (type == typeof(Boolean))
                            value = false;
                        else if (type == typeof(DateTime))
                            value = DateTime.MinValue;
                        else
                            value = string.Empty;
                    }
                }
                catch { }

            }
            return obj;
        }

        private static object GetValue(Type type, object value, object default_value, string datetime_format)
        {
            object v = default_value;
            if (type == typeof(String))
                v = value;
            else if (type == typeof(Int32) || type == typeof(int) || type == typeof(Int16) || type == typeof(Int64))
            {
                if (value == null) value = "0";
                int iv = 0;
                int.TryParse(value.ToString(), out iv);
                v = iv;
            }
            else if (type == typeof(Decimal))
            {
                if (value == null) value = "0";
                decimal dv = 0M;
                decimal.TryParse(value.ToString(), out dv);
                v = dv;
            }
            else if (type == typeof(Double))
            {
                if (value == null) value = "0";
                double dbv = 0.0D;
                double.TryParse(value.ToString(), out dbv);
                v = dbv;
            }
            else if (type == typeof(Boolean))
            {
                bool bv = false;
                if (!Boolean.TryParse(value.ToString(), out bv))
                {
                    bv = value.ToString() == "1";
                    if (!bv)
                    {
                        bv = value.ToString() == "Y";
                    }
                }
                v = bv;
            }
            else if (type == typeof(DateTime))
            {
                if (type == value.GetType())
                    v = value;
                else if (!string.IsNullOrEmpty(value.ToString().Trim()))
                {
                    v = DateTime.ParseExact(value.ToString(), datetime_format, System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }
                else
                {
                    v = DateTime.MinValue;
                }
            }
            else if (type == new System.TimeSpan().GetType())
            {
                System.TimeSpan ts = new TimeSpan();
                System.TimeSpan.TryParse(value.ToString(), out ts);
                v = ts;
            }
            else if (type.IsEnum)
            {
                int vInt;
                int.TryParse(value.ToString(), out vInt);
                v = vInt;
            }
            return v;
        }

        private static T DataRowToObject<T>(DataRow dataRow) where T : class, new()
        {
            Object objReturn = null;
            Object pValue = null;
            PropertyInfo propertyInfo = null;
            try
            {
                if (dataRow != null)
                {
                    objReturn = Activator.CreateInstance(typeof(T));
                    foreach (DataColumn dc in dataRow.Table.Columns)
                    {
                        propertyInfo = typeof(T).GetProperty(dc.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        pValue = dataRow[dc];
                        if (pValue != DBNull.Value)
                        {
                            try
                            {
                                propertyInfo.SetValue(objReturn, Convert.ChangeType(pValue, dc.DataType), null);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return objReturn as T;
        }

        #endregion

        #region Public Functions

        public static T FillObject<T>(IDataReader dr) where T : class, new()
        {
            Type objType = typeof(T);
            T obj = null;
            try
            {
                List<PropertyMappingInfo> mapInfo = GetProperties(objType);
                int[] ordinals = GetOrdinals(mapInfo, dr);
                if (dr.Read())
                {
                    obj = CreateObject<T>(dr, mapInfo, ordinals);
                }
            }
            finally
            {
                if (dr.IsClosed == false)
                {
                    dr.Close();
                }
            }
            return obj;
        }

        public static C FillCollection<T, C>(IDataReader dr)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            Type objType = typeof(T);
            C coll = new C();//Activator.CreateInstance(C);
            try
            {
                List<PropertyMappingInfo> mapInfo = GetProperties(objType);
                int[] ordinals = GetOrdinals(mapInfo, dr);
                while (dr.Read())
                {
                    T obj = CreateObject<T>(dr, mapInfo, ordinals);
                    coll.Add(obj);
                }
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            //finally
            //{
            //    if (dr.IsClosed == false && dr.NextResult() == false)
            //    {
            //        dr.Close();
            //    }
            //}
            return coll;
        }

        public static List<PropertyMappingInfo> GetProperties(Type objType)
        {
            List<PropertyMappingInfo> info = MappingInfoCache.GetCache(objType.ToString());
            if (info == null)
            {
                info = LoadPropertyMappingInfo(objType);
                MappingInfoCache.SetCache(objType.ToString(), info);
            }
            return info;
        }

        public static void SetValue<T>(T t, object column, object value) where T : class, new()
        {
            SetValue<T>(t, column.ToString(), value, "dd/MM/yyyy");
        }

        public static void SetValue<T>(T t, string column, object value) where T : class, new()
        {
            SetValue<T>(t, column, value, "dd/MM/yyyy");
        }

        public static void SetValue<T>(T t, object column, object value, string datetime_format) where T : class, new()
        {
            SetValue<T>(t, column.ToString(), value, datetime_format);
        }

        public static void SetValue<T>(T t, string column, object value, string datetime_format) where T : class, new()
        {
            List<DataMapping.PropertyMappingInfo> pmis = DataMapping.ObjectHelper.GetProperties(typeof(T));
            for (int i = 0; i < pmis.Count; i++)
            {
                Type type = pmis[i].PropertyInfo.PropertyType;
                object default_value = pmis[i].PropertyInfo.GetValue(t, null);
                if (pmis[i].DataFieldName.Trim().ToUpper().Equals(column.Trim().ToUpper()))
                {
                    pmis[i].PropertyInfo.SetValue(t, GetValue(type, value, default_value, datetime_format), null);
                    break;
                }
            }
        }

        public static object GetValue<T>(T t, string column) where T : class, new()
        {
            List<DataMapping.PropertyMappingInfo> pmis = DataMapping.ObjectHelper.GetProperties(typeof(T));
            for (int i = 0; i < pmis.Count; i++)
            {
                Type type = pmis[i].PropertyInfo.PropertyType;
                object default_value = pmis[i].PropertyInfo.GetValue(t, null);
                if (pmis[i].DataFieldName.Trim().ToUpper().Equals(column.Trim().ToUpper()))
                {
                    return default_value;
                }
            }
            return null;
        }

        public static object GetFieldValue<T>(T t, string field, BindingFlags bf) where T : class, new()
        {
            return typeof(T).InvokeMember(field, bf == BindingFlags.Default ? (BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static) : bf, null, t, null);
        }

        public static List<string> GetObjectFields(Type t)
        {
            List<string> fields = new List<string>();
            foreach (DataMapping.PropertyMappingInfo p in DataMapping.ObjectHelper.GetProperties(t))
            {
                fields.Add(p.DataFieldName);
            }
            return fields;
        }

        public static void CopyAttributes<F, T>(F from, T to, params string[] ignoreAttrs)
            where F : class, new()
            where T : class, new()
        {
            if (from == null || to == null)
                return;
            Type fromType = from.GetType();
            Type toType = to.GetType();
            PropertyMappingInfo piFr;
            List<PropertyMappingInfo> piFrList = ObjectHelper.GetProperties(fromType);
            if (piFrList.Count > 0)
            {
                foreach (DataMapping.PropertyMappingInfo piTo in DataMapping.ObjectHelper.GetProperties(toType))
                {
                    if (ignoreAttrs != null && ignoreAttrs.Length > 0)
                    {
                        bool ignore = false;
                        foreach (string attr in ignoreAttrs)
                            if (attr == piTo.PropertyInfo.Name)
                            {
                                ignore = true;
                                break;
                            }
                        if (ignore) continue;
                    }
                    piFr = GetPropertyByName(piFrList, piTo.PropertyInfo.Name);
                    if (piFr != null)
                        piTo.PropertyInfo.SetValue(to, piFr.PropertyInfo.GetValue(from, null), null);
                }
            }
        }

        private static PropertyMappingInfo GetPropertyByName(List<PropertyMappingInfo> properties, string propertyName)
        {
            if (properties != null && properties.Count > 0)
            {
                foreach (PropertyMappingInfo pi in properties)
                {
                    if (pi.PropertyInfo.Name == propertyName)
                        return pi;
                }
            }
            return null;
        }

        #endregion
    }
}
