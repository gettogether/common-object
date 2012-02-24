using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.CSV
{
    public delegate object DataTranformDelegate(string data);

    [AttributeUsage(AttributeTargets.Property)]
    public class CSVAttribute : System.Attribute
    {
        public int Position;
        public string DataTransform = string.Empty;

        public CSVAttribute(int position, string dataTransform)
        {
            Position = position;
            DataTransform = dataTransform;
        }

        public CSVAttribute(int position)
        {
            Position = position;
        }

        public CSVAttribute()
        {
        }

    }
}
