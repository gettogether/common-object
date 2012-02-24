using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DataAccess.Data.Interfaces
{
    public interface ICommonBase<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        #region Query Functions

        DateTime GetDbTime();

        int GetRecordsCount();

        C GetAllList(object[] columns, bool asc, params string[] orderBy);
        C GetAllList(string[] columns, bool asc, params string[] orderBy);
        C GetAllList();
        C GetAllList(object[] columns);
        C GetAllList(string[] columns);

        C GetList(int count);
        C GetList(object[] columns, int count);
        C GetList(int count, params string[] orderBy);
        C GetList(int count, params object[] orderBy);
        C GetList(int count, bool asc, params string[] orderBy);
        C GetList(int count, bool asc, params object[] orderBy);
        C GetList(string cmdText);

        C GetList(ParameterCollection pc);
        C GetList(ParameterCollection pc, params string[] orderBy);
        C GetList(ParameterCollection pc, int count, params string[] orderBy);
        C GetList(ParameterCollection pc, params object[] orderBy);
        C GetList(ParameterCollection pc, int count, params object[] orderBy);
        C GetList(ParameterCollection pc, bool asc, params string[] orderBy);
        C GetList(ParameterCollection pc, int count, bool asc, params string[] orderBy);
        C GetList(ParameterCollection pc, bool asc, params object[] orderBy);
        C GetList(ParameterCollection pc, int count, bool asc, params object[] orderBy);

        C GetList(IDbCommand cmd);

        IDbCommand GetIDbCommand(ParameterCollection pc, string[] columns, bool asc, params string[] orderBy);
        IDbCommand GetIDbCommand(ParameterCollection pc, int top, bool asc, params string[] orderBy);

        DataSet GetDataSet(string sql);
        DataSet GetDataSet(ParameterCollection pc);

        #endregion

        #region Other Functions

        IDbConnection GetConnection();
        DataTable GetTableSchema();

        #endregion
    }
}
