using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.WebObject.Entities
{
    #region Defination

    public class CacheInfo
    {
        public enum Columns
        {
            Key,
            Type,
        }
        public CacheInfo()
        {

        }
        public CacheInfo(string k, string t)
        {
            this._Type = t;
            this._Key = k;
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

    }

    public class CacheList : ObjectBase.ListBase<CacheInfo>
    {
        public CacheList()
        {

        }
    }

    public class Caches
    {
        private CacheList _CacheList;
        public CacheList CacheList
        {
            get { return _CacheList; }
            set { _CacheList = value; }
        }

        private int _Total;
        public int Total
        {
            get { return _Total; }
            set { _Total = value; }
        }
    }

    #endregion
}
