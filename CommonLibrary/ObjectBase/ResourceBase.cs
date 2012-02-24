using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.ObjectBase
{
    public class ResourceBase<T> where T : class, new()
    {
        public ResourceBase()
        {

        }

        public static string GetResource(string name)
        {
            return Utility.MutiLanguage.GetResource(typeof(T), name);
        }

        public static string GetResource(string name, Utility.MutiLanguage.Languages lang)
        {
            return Utility.MutiLanguage.GetResource(typeof(T), name, Utility.MutiLanguage.EnumToString(lang));
        }

        public static string GetResource(string name, string language)
        {
            return Utility.MutiLanguage.GetResource(typeof(T), name, language);
        } 

        public string GetResourceByName(string name)
        {
            return GetResource(name);
        }
    }
}
