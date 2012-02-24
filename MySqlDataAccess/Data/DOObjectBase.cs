using System;
using System.Collections.Generic;
using System.Text;
using MySqlDataAccess;
using System.Data;
using MySql.Data.MySqlClient;

namespace MySqlDataAccess.Data
{
    public class DOObjectBase<T, C> : IDOObject<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        #region Constructors
        public DOObjectBase(string tableName, string connectionKey)
        {
            DOInfo = new DataObjectInfo(tableName, connectionKey);
        }

        public DOObjectBase()
        {

        }
        #endregion

        #region Query Functions
        public C GetList(int count)
        {
            return GetList(SqlScriptHandler.Search.GetSelectTopString(DOInfo.TableName, count, ""));
        }

        public C GetList(object[] columns, int count)
        {
            return GetList(SqlScriptHandler.Search.GetSelectTopString(DOInfo.TableName, columns, count,""));
        }

        public C GetList(int count, params string[] orderBy)
        {
            return GetList(count, true, orderBy);
        }

        public C GetList(int count, params object[] orderBy)
        {
            return GetList(count, true, orderBy);
        }

        public C GetList(int count, bool asc, params string[] orderBy)
        {
            return GetList(SqlScriptHandler.Search.GetSelectTopOrderString(DOInfo.TableName, count, asc, orderBy));
        }

        public C GetList(int count, bool asc, params object[] orderBy)
        {
            return GetList(SqlScriptHandler.Search.GetSelectTopOrderString(DOInfo.TableName, count, asc, orderBy));
        }

        public C GetAllList()
        {
            return GetList(DOInfo.DefaultSelect);
        }

        public C GetAllList(object[] columns)
        {
            return GetList(SqlScriptHandler.Search.GetSelectString(DOInfo.TableName, columns));
        }

        public C GetAllList(string[] columns)
        {
            return GetList(SqlScriptHandler.Search.GetSelectString(DOInfo.TableName, columns));
        }

        public C GetAllList(bool asc, params string[] orderBy)
        {
            return GetList(SqlScriptHandler.Search.GetSelectOrderString(DOInfo.TableName, asc, orderBy));
        }

        public C GetAllList(bool asc, params object[] orderBy)
        {
            return GetAllList(asc, SqlScriptHandler.ObjectsToStrings(orderBy));
        }

        public C GetAllList(string[] columns, bool asc, params string[] orderBy)
        {
            return GetList(SqlScriptHandler.Search.GetSelectOrderString(DOInfo.TableName, columns, asc, orderBy));
        }

        public C GetAllList(object[] columns, bool asc, params string[] orderBy)
        {
            return GetList(SqlScriptHandler.Search.GetSelectOrderString(DOInfo.TableName, columns, asc, orderBy));
        }

        public C GetList(string cmdString)
        {
            return SqlUtil.GetObjectList<T, C>(DOInfo.ConnectionKey, cmdString);
        }

        public C GetList(ParsList pars)
        {
            return GetList(pars, null);
        }

        public C GetList(ParsList pars, int count)
        {
            return GetList(pars, null);
        }

        public C GetList(ParsList pars, params string[] orderBy)
        {
            return SqlUtil.GetObjectList<T, C>(DOInfo.ConnectionKey, DOInfo.TableName, pars, 0, orderBy);
        }

        public C GetList(ParsList pars, int count, params string[] orderBy)
        {
            return SqlUtil.GetObjectList<T, C>(DOInfo.ConnectionKey, DOInfo.TableName, pars, count, orderBy);
        }

        public C GetList(ParsList pars, params object[] orderBy)
        {
            return SqlUtil.GetObjectList<T, C>(DOInfo.ConnectionKey, DOInfo.TableName, pars, 0, SqlScriptHandler.ObjectsToStrings(orderBy));
        }

        public C GetList(ParsList pars, int count, params object[] orderBy)
        {
            return SqlUtil.GetObjectList<T, C>(DOInfo.ConnectionKey, DOInfo.TableName, pars, count, SqlScriptHandler.ObjectsToStrings(orderBy));
        }

        public C GetList(ParsList pars, bool asc, params string[] orderBy)
        {
            return SqlUtil.GetObjectList<T, C>(DOInfo.ConnectionKey, DOInfo.TableName, pars, 0, asc, orderBy);
        }

        public C GetList(ParsList pars, int count, bool asc, params string[] orderBy)
        {
            return SqlUtil.GetObjectList<T, C>(DOInfo.ConnectionKey, DOInfo.TableName, pars, count, asc, orderBy);
        }

        public C GetList(ParsList pars, bool asc, params object[] orderBy)
        {
            return SqlUtil.GetObjectList<T, C>(DOInfo.ConnectionKey, DOInfo.TableName, pars, 0, asc, SqlScriptHandler.ObjectsToStrings(orderBy));
        }

        public C GetList(ParsList pars, int count, bool asc, params object[] orderBy)
        {
            return SqlUtil.GetObjectList<T, C>(DOInfo.ConnectionKey, DOInfo.TableName, pars, count, asc, SqlScriptHandler.ObjectsToStrings(orderBy));
        }

        public C GetList(MySqlCommand cmd)
        {
            MySqlConnection cnn = ConnectionHandler.CreateConnectionByKey(DOInfo.ConnectionKey);
            cmd.Connection = cnn;
            return SqlUtil.GetObjectList<T, C>(cmd);
        }
        #endregion

        #region Delete Functions
        public int Delete(MySqlConnection cnn, MySqlTransaction tran, ParsList pars)
        {
            return SqlUtil.ExecuteDelete(cnn, tran, DOInfo.TableName, pars);
        }

        public int Delete(ParsList pars)
        {
            return SqlUtil.ExecuteDelete(DOInfo.ConnectionKey, DOInfo.TableName, pars);
        }
        #endregion

        #region Update Functions
        public int Update(ParsList updatePars, ParsList condisionPars)
        {
            return SqlUtil.Update(DOInfo.ConnectionKey, DOInfo.TableName, updatePars, condisionPars);
        }

        public int Update(MySqlConnection cnn, MySqlTransaction tran, ParsList updatePars, ParsList condisionPars)
        {
            return SqlUtil.Update(cnn, tran, DOInfo.TableName, updatePars, condisionPars);
        }
        #endregion

        #region Insert Functions
        public int Insert(ParsList pars)
        {
            return SqlUtil.ExecuteInsert(DOInfo.ConnectionKey, DOInfo.TableName, pars);
        }

        public int Insert(MySqlConnection cnn, MySqlTransaction tran, ParsList pars)
        {
            return SqlUtil.ExecuteInsert(cnn, tran, DOInfo.TableName, pars);
        }
        #endregion

        #region Attributes
        private DataObjectInfo _doinfo;
        public DataObjectInfo DOInfo
        {
            get { return _doinfo; }
            set { _doinfo = value; }
        }
        #endregion
    }
}
