using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CommonLibrary.CSV
{
    public class ObjectLoader
    {
        public static X LoadNew<X>(string[] fields, bool isSupressErrors)
        {
            X tempObj = (X)Activator.CreateInstance(typeof(X));
            Load(tempObj, fields, isSupressErrors);
            return tempObj;
        }

        public static void Load(object target, string[] fields, bool isSupressErrors)
        {
            Type targetType = target.GetType();
            PropertyInfo[] properties = targetType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    object[] attributes = property.GetCustomAttributes(typeof(CSVAttribute), false);
                    if (attributes.Length > 0)
                    {
                        CSVAttribute positionAttr = (CSVAttribute)attributes[0];
                        int position = positionAttr.Position;
                        try
                        {
                            object data = fields[position];
                            if (positionAttr.DataTransform != string.Empty)
                            {
                                MethodInfo method = targetType.GetMethod(positionAttr.DataTransform);

                                data = method.Invoke(target, new object[] { data });
                            }
                            property.SetValue(target, Convert.ChangeType(data, property.PropertyType), null);
                        }
                        catch
                        {
                            if (!isSupressErrors)
                                throw;
                        }

                    }
                }
            }
        }
    }
}
