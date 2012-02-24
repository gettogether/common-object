using System;
using System.Collections.Generic;
using System.Text;
using DataMapping;

namespace CommonLibrary.Utility
{
    public class PredicateHelper<V>
    {
        #region PredicateHelper
        public PredicateHelper()
        {

        }

        public PredicateHelper(string column, V value, TokenTypes token)
        {
            this._column = column;
            this.Value = value;
            this.Token = token;
        }

        public PredicateHelper(object column, V value, TokenTypes token)
        {
            this._column = column.ToString();
            this.Value = value;
            this.Token = token;
        }
        public PredicateHelper(object column, V value)
        {
            this._column = column.ToString();
            this.Value = value;
        }
        #endregion

        #region Attribute
        private TokenTypes _token = TokenTypes.Unknown;

        public TokenTypes Token
        {
            get { return _token; }
            set { _token = value; }
        }

        private string _column;
        public string Column
        {
            get { return _column; }
            set { _column = value; }
        }
        private V _value;
        public V Value
        {
            get { return _value; }
            set { _value = value; }
        }
        #endregion

        #region Public Functions
        public bool Find<T>(T t)
        {
            List<PropertyMappingInfo> mapInfo = ObjectHelper.GetProperties(t.GetType());
            int index = GetColumnIndex(mapInfo, this.Column);
            if (index >= 0)
            {
                try
                {
                    Type type = mapInfo[index].PropertyInfo.PropertyType;
                    object ov = mapInfo[index].PropertyInfo.GetValue(t, null);
                    if (ov == null) return false;
                    if (type == typeof(String))
                    {
                        string strValue = (string)ov;
                        string strPar = Value.ToString();
                        switch (Token)
                        {
                            case TokenTypes.Equal:
                                if (strValue.Trim() == strPar.Trim()) return true;
                                break;
                            case TokenTypes.Like:
                                if (strValue.ToUpper().IndexOf(strPar.ToUpper()) >= 0) return true;
                                break;
                            case TokenTypes.LeftLike:
                                if (strValue.ToUpper().StartsWith(strPar.ToUpper())) return true;
                                break;
                            case TokenTypes.RightLike:
                                if (strValue.ToUpper().EndsWith(strPar.ToUpper())) return true;
                                break;
                            default: if (strValue.Trim() == strPar.Trim()) return true;
                                return false;
                        }

                    }
                    else if (type == (typeof(DateTime)))
                    {
                        DateTime dtValue = (DateTime)ov;
                        DateTime dtPar = Convert.ToDateTime(Value);
                        switch (Token)
                        {
                            case TokenTypes.GreaterThan:
                                if (dtValue > dtPar) return true;
                                break;
                            case TokenTypes.LessThan:
                                if (dtValue < dtPar) return true;
                                break;
                            case TokenTypes.GreaterThanEqual:
                                if (dtValue >= dtPar) return true;
                                break;
                            case TokenTypes.LessThanEqual:
                                if (dtValue <= dtPar) return true;
                                break;
                            default: if (dtValue == dtPar) return true;
                                return false;
                        }
                        return false;
                    }
                    else if (type == typeof(Int32))
                    {
                        int intValue = (int)ov;
                        int intPar = Convert.ToInt32(Value);
                        switch (Token)
                        {
                            case TokenTypes.GreaterThan:
                                if (intValue > intPar) return true;
                                break;
                            case TokenTypes.LessThan:
                                if (intValue < intPar) return true;
                                break;
                            case TokenTypes.GreaterThanEqual:
                                if (intValue >= intPar) return true;
                                break;
                            case TokenTypes.LessThanEqual:
                                if (intValue <= intPar) return true;
                                break;
                            default: if (intValue == intPar) return true;
                                return false;
                        }
                    }
                    else if (type == typeof(Int16))
                    {
                        Int16 intValue = (Int16)ov;
                        Int16 intPar = Convert.ToInt16(Value);
                        switch (Token)
                        {
                            case TokenTypes.GreaterThan:
                                if (intValue > intPar) return true;
                                break;
                            case TokenTypes.LessThan:
                                if (intValue < intPar) return true;
                                break;
                            case TokenTypes.GreaterThanEqual:
                                if (intValue >= intPar) return true;
                                break;
                            case TokenTypes.LessThanEqual:
                                if (intValue <= intPar) return true;
                                break;
                            default: if (intValue == intPar) return true;
                                return false;
                        }
                    }
                    else if (type == typeof(Int64))
                    {
                        Int64 intValue = (Int64)ov;
                        Int64 intPar = Convert.ToInt64(Value);
                        switch (Token)
                        {
                            case TokenTypes.GreaterThan:
                                if (intValue > intPar) return true;
                                break;
                            case TokenTypes.LessThan:
                                if (intValue < intPar) return true;
                                break;
                            case TokenTypes.GreaterThanEqual:
                                if (intValue >= intPar) return true;
                                break;
                            case TokenTypes.LessThanEqual:
                                if (intValue <= intPar) return true;
                                break;
                            default: if (intValue == intPar) return true;
                                return false;
                        }
                    }
                    else if (type == typeof(Decimal))
                    {
                        decimal intValue = (decimal)ov;
                        decimal intPar = Convert.ToInt64(Value);
                        switch (Token)
                        {
                            case TokenTypes.GreaterThan:
                                if (intValue > intPar) return true;
                                break;
                            case TokenTypes.LessThan:
                                if (intValue < intPar) return true;
                                break;
                            case TokenTypes.GreaterThanEqual:
                                if (intValue >= intPar) return true;
                                break;
                            case TokenTypes.LessThanEqual:
                                if (intValue <= intPar) return true;
                                break;
                            default: if (intValue == intPar) return true;
                                return false;
                        }
                    }
                    else if (type == typeof(Double))
                    {
                        double intValue = (double)ov;
                        double intPar = Convert.ToDouble(Value);
                        switch (Token)
                        {
                            case TokenTypes.GreaterThan:
                                if (intValue > intPar) return true;
                                break;
                            case TokenTypes.LessThan:
                                if (intValue < intPar) return true;
                                break;
                            case TokenTypes.GreaterThanEqual:
                                if (intValue >= intPar) return true;
                                break;
                            case TokenTypes.LessThanEqual:
                                if (intValue <= intPar) return true;
                                break;
                            default: if (intValue == intPar) return true;
                                return false;
                        }
                    }
                    else if (type == typeof(Boolean))
                    {
                        bool bValue = (bool)ov;
                        bool bPar = Convert.ToBoolean(Value);
                        return (bValue == bPar);
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        public int GetColumnIndex(List<PropertyMappingInfo> map_info, string column)
        {
            for (int i = 0; i < map_info.Count; i++)
            {
                if (map_info[i].DataFieldName.Trim().ToUpper() == column.Trim().ToUpper())
                    return i;
            }
            return -1;
        }
        #endregion
    }
}
