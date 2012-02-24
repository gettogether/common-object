using System;
using MySqlDataAccess;

namespace MySqlDataAccess.Data
{
    public class DataObjectInfo
    {
        #region Constructors
        public DataObjectInfo()
        {

        }

        public DataObjectInfo(string tableName, string connectionKey)
        {
            this.TableName = tableName;
            this.ConnectionKey = connectionKey;
        }
        #endregion
        #region Attribute
        private string _tablename;
        private string _defaultselect;
        private string _connectionkey;
        public string TableName
        {
            get
            {
                return this._tablename;
            }
            set
            {
                this._tablename = value;
                this._defaultselect = SqlScriptHandler.Search.GetSelectString(_tablename);
            }
        }

        public string DefaultSelect
        {
            get { return _defaultselect; }
            set { _defaultselect = value; }
        }

        public string ConnectionKey
        {
            get { return _connectionkey; }
            set { _connectionkey = value; }
        }
        #endregion
    }
}
