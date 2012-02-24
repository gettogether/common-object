using System;
using System.Collections.Generic;
using System.Text;
using MySqlDataAccess;
using System.Data;
using MySql.Data.MySqlClient;

namespace MySqlDataAccess.Data
{
    interface IDOObject<T, C>
    {
        C GetList(int count);
        C GetList(object[] columns, int count);
        C GetList(int count, params string[] orderBy);
        C GetList(int count, params object[] orderBy);
        C GetList(int count, bool asc, params string[] orderBy);
        C GetList(int count, bool asc, params object[] orderBy);
        C GetAllList(object[] columns, bool asc, params string[] orderBy);
        C GetAllList(string[] columns, bool asc, params string[] orderBy);
        C GetAllList();
        C GetAllList(object[] columns);
        C GetAllList(string[] columns);
        C GetList(string cmdString);
        C GetList(ParsList pars);
        C GetList(ParsList pars, int count);
        C GetList(ParsList pars, params string[] orderBy);
        C GetList(ParsList pars, int count, params string[] orderBy);
        C GetList(ParsList pars, params object[] orderBy);
        C GetList(ParsList pars, int count, params object[] orderBy);
        C GetList(ParsList pars, bool asc, params string[] orderBy);
        C GetList(ParsList pars, int count, bool asc, params string[] orderBy);
        C GetList(ParsList pars, bool asc, params object[] orderBy);
        C GetList(ParsList pars, int count, bool asc, params object[] orderBy);
        C GetList(MySqlCommand cmd);
        int Insert(ParsList pars);
        int Insert(MySqlConnection cnn, MySqlTransaction tran, ParsList pars);
        int Delete(ParsList pars);
        int Delete(MySqlConnection cnn, MySqlTransaction tran, ParsList pars);
        int Update(ParsList updatePars, ParsList condisionPars);
        int Update(MySqlConnection cnn, MySqlTransaction tran, ParsList updatePars, ParsList condisionPars);
    }
}
