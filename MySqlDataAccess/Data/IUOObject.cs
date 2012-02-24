using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySqlDataAccess;
using MySql.Data.MySqlClient;

namespace MySqlDataAccess.Data
{
    interface IUOObject<T>
    {
        int Insert();
        int Insert(T t);
        int Insert(MySqlConnection cnn, MySqlTransaction tran, T t);
        int Update(ParsList pars, T t);
        int Update(MySqlConnection cnn, MySqlTransaction tran, ParsList pars, T t);
    }
}
