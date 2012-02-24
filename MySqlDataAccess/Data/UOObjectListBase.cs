using System;
using System.Collections.Generic;
using System.Text;
using MySqlDataAccess;
using System.Data;

namespace MySqlDataAccess.Data
{
    public class UOObjectListBase<T> : List<T>
        where T : class, new()
    {
        #region Attributes
        private DataObjectInfo _doinfo;
        public DataObjectInfo DOInfo
        {
            get { return _doinfo; }
            set { _doinfo = value; }
        }
        #endregion

        #region Constructors
        public UOObjectListBase()
        {

        }
        #endregion

    }
}
