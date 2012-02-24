using System;

namespace DataMapping
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MappingAttribute : System.Attribute
    {
        public MappingAttribute(string dataFieldName, object nullValue)
            : base()
        {
            _dataFieldName = dataFieldName;
            _nullValue = nullValue;
        }

        public MappingAttribute(object nullValue) : this(string.Empty, nullValue) { }
        #region Attributes
        private string _dataFieldName;
        public string DataFieldName
        {
            get { return _dataFieldName; }
        }
        private object _nullValue;
        public object NullValue
        {
            get { return _nullValue; }
        }

        #endregion
    }
}
