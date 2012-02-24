using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DataMapping
{
    public class PropertyMappingInfo
    {
        #region Constructors
        internal PropertyMappingInfo() : this(string.Empty, null, null) { }
        internal PropertyMappingInfo(string dataFieldName, object defaultValue, PropertyInfo info)
        {
            _dataFieldName = dataFieldName;
            _defaultValue = defaultValue;
            _propInfo = info;
        }
        #endregion

        #region Public Properties
        private string _dataFieldName;
        public string DataFieldName
        {
            get
            {
                if (string.IsNullOrEmpty(_dataFieldName))
                {
                    _dataFieldName = _propInfo.Name;
                }
                return _dataFieldName;
            }
        }
        private object _defaultValue;
        public object DefaultValue
        {
            get { return _defaultValue; }
        }
        private PropertyInfo _propInfo;
        public PropertyInfo PropertyInfo
        {
            get { return _propInfo; }
        }

        #endregion
    }
}
