using System;
using System.Collections.Generic;
using System.Text;
using DataMapping;
using MySql.Data.MySqlClient;
using System.Data;

namespace MySqlDataAccess
{
    public class SqlUtil
    {
        #region Functions
        public static MySqlCommand BuildIDbCommand(string connectionKey, string cmdString, IDataParameterCollection pars)
        {
            MySqlConnection cnn = ConnectionHandler.CreateConnectionByKey(connectionKey);
            MySqlCommand cmd = cnn.CreateCommand();
            cmd.CommandText = cmdString;
            cmd.Parameters.Add(pars);
            return cmd;
        }

        public static MySqlCommand BuildIDbCommand(string connectionKey, string cmdString)
        {
            MySqlConnection cnn = ConnectionHandler.CreateConnectionByKey(connectionKey);
            MySqlCommand cmd = cnn.CreateCommand();
            cmd.CommandText = cmdString;
            return cmd;
        }

        public static MySqlCommand BuildIDbCommand(MySqlConnection cnn, string cmdString)
        {
            MySqlCommand cmd = cnn.CreateCommand();
            cmd.CommandText = cmdString;
            return cmd;
        }

        private static MySqlCommand BuildIDbCommand(MySqlConnection cnn, string storedProcName, IDataParameter[] pars)
        {
            MySqlCommand cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = storedProcName;
            foreach (IDataParameter par in pars)
            {
                if (par != null)
                {
                    if ((par.Direction == ParameterDirection.InputOutput || par.Direction == ParameterDirection.Input) &&
                        (par.Value == null))
                    {
                        par.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(par);
                }
            }
            return cmd;
        }

        public static IDataParameter BuildIDataParameter(MySqlCommand cmd, string parName, object value)
        {
            IDataParameter par = cmd.CreateParameter();
            par.Value = value;
            par.ParameterName = parName;
            return par;
        }

        public static void FillParameters(MySqlCommand cmd, Dictionary<string, object> pars)
        {
            foreach (string key in pars.Keys)
            {
                cmd.Parameters.Add(BuildIDataParameter(cmd, key, pars[key]));
            }
        }

        public static IDataReader ExecuteProcedureReader(string connectionKey, string storedProcName, IDataParameter[] pars)
        {
            MySqlConnection cnn = ConnectionHandler.CreateConnectionByKey(connectionKey);
            IDataReader returnReader;
            MySqlCommand cmd = BuildIDbCommand(cnn, storedProcName, pars);
            returnReader = cmd.ExecuteReader();
            return returnReader;
        }

        public static IDataReader ExecuteProcedureReader(string connectionKey, string storedProcName, IDataParameter par)
        {
            return ExecuteProcedureReader(connectionKey, storedProcName, new IDataParameter[] { par });
        }

        public static IDataReader ExecuteReader(string connectionKey, string cmdString)
        {
            return ExecuteReader(ConnectionHandler.CreateConnectionByKey(connectionKey), cmdString);
        }

        public static IDataReader ExecuteReader(MySqlConnection cnn, string cmdString)
        {
            MySqlCommand cmd = BuildIDbCommand(cnn, cmdString);
            return cmd.ExecuteReader();
        }

        public static IDataReader ExecuteReader(MySqlConnection cnn, MySqlTransaction tran, string cmdString)
        {
            MySqlCommand cmd = BuildIDbCommand(cnn, cmdString);
            cmd.Transaction = tran;
            return cmd.ExecuteReader();
        }

        public static DataSet ExecuteProcedureDataSet(string connectionKey, string storedProcName, params IDataParameter[] pars)
        {
            using (MySqlConnection cnn = ConnectionHandler.CreateConnectionByKey(connectionKey))
            {
                DataSet ds = new DataSet();
                IDbDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = BuildIDbCommand(cnn, storedProcName, pars);
                da.Fill(ds);
                return ds;
            }
        }
        #endregion

        #region Query Functions
        public static C GetObjectList<T, C>(string connectionKey, string cmdString)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            return GetObjectList<T, C>(BuildIDbCommand(connectionKey, cmdString));
        }

        public static C GetObjectList<T, C>(MySqlCommand cmd)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            using (cmd.Connection)
            {
                using (cmd)
                {
                    IDataReader rdr = cmd.ExecuteReader();
                    return ObjectHelper.FillCollection<T, C>(rdr);
                }
            }
        }

        public static C GetObjectList<T, C>(string connectionKey, string tableName, ParsList pars, int count)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            return GetObjectList<T, C>(connectionKey, tableName, pars, count, null);
        }

        public static C GetObjectList<T, C>(string connectionKey, string tableName, ParsList pars, int count, bool asc, params string[] orderBy)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            MySqlCommand cmd = SqlUtil.BuildIDbCommand(connectionKey, SqlScriptHandler.Search.GetSelectString(tableName, pars, asc, count, orderBy));
            using (cmd.Connection)
            {
                using (cmd)
                {
                    foreach (Pars p in pars)
                    {

                        cmd.Parameters.Add(new MySqlParameter(string.Format(SqlScriptHandler.ValueTmepString, p.ColumnPar), SqlScriptHandler.ProcessValue(p.TokType, p.Value)));
                    }
                    return SqlUtil.GetObjectList<T, C>(cmd);
                }
            }
        }

        public static C GetObjectList<T, C>(string connectionKey, string tableName, string[] columns, ParsList pars, int count, bool asc, params string[] orderBy)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            MySqlCommand cmd = SqlUtil.BuildIDbCommand(connectionKey, SqlScriptHandler.Search.GetSelectString(tableName, pars, asc, count, orderBy));
            using (cmd.Connection)
            {
                using (cmd)
                {
                    foreach (Pars p in pars)
                    {
                        cmd.Parameters.Add(new MySqlParameter(string.Format(SqlScriptHandler.ValueTmepString, p.ColumnPar), SqlScriptHandler.ProcessValue(p.TokType, p.Value)));
                    }
                    return SqlUtil.GetObjectList<T, C>(cmd);
                }
            }
        }

        public static C GetObjectList<T, C>(string connectionKey, string tableName, ParsList pars, int count, params string[] orderBy)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            return GetObjectList<T, C>(connectionKey, tableName, pars, count, true, orderBy);
        }

        #endregion

        #region Insert
        public static int GetIdentityId(MySqlConnection cnn)
        {
            IDataReader idr = ExecuteReader(cnn, SqlScriptHandler.Search.SelectIdentityId);
            int ret = 0;
            if (idr.Read())
            {
                if (idr[0] is DBNull)
                    ret = 1;
                else
                    ret = Convert.ToInt32(idr[0]);
            }
            idr.Close();
            return ret;
        }

        public static int GetIdentityId(MySqlConnection cnn, MySqlTransaction tran)
        {
            IDataReader idr = ExecuteReader(cnn, tran, SqlScriptHandler.Search.SelectIdentityId);
            int ret = 0;
            if (idr.Read())
                ret = Convert.ToInt32(idr[0]);
            idr.Close();
            return ret;
        }

        public static int ExecuteInsert<T>(string connectionKey, string tableName, T t) where T : class, new()
        {
            MySqlCommand cmd = GetInsertIDbCommand(ConnectionHandler.CreateConnectionByKey(connectionKey), null, tableName, t);
            using (cmd.Connection)
            {
                if (cmd.ExecuteNonQuery() > 0)
                {
                    return GetIdentityId(cmd.Connection);
                }
            }
            return 0;
        }

        public static int ExecuteInsert<T>(MySqlConnection cnn, MySqlTransaction tran, string tableName, T t) where T : class, new()
        {
            if (GetInsertIDbCommand(cnn, tran, tableName, t).ExecuteNonQuery() > 0)
            {
                return GetIdentityId(cnn, tran);
            }
            return 0;
        }

        public static MySqlCommand GetInsertIDbCommand<T>(MySqlConnection cnn, MySqlTransaction tran, string tableName, T t) where T : class, new()
        {
            StringBuilder sb_c = new StringBuilder();
            StringBuilder sb_v = new StringBuilder();
            List<PropertyMappingInfo> mapInfo = ObjectHelper.GetProperties(typeof(T));
            Dictionary<string, object> pars = new Dictionary<string, object>();
            for (int i = 0; i < mapInfo.Count; i++)
            {
                if (mapInfo[i].DefaultValue.ToString().ToUpper().IndexOf(SqlScriptHandler.UnInsert) > 0) continue;
                object value = mapInfo[i].PropertyInfo.GetValue(t, null);
                if (value == null) continue;
                if (sb_c.Length <= 0)
                    sb_c.Append(string.Format(SqlScriptHandler.ColumnTempString, mapInfo[i].DataFieldName));
                else
                    sb_c.Append(",").Append(string.Format(SqlScriptHandler.ColumnTempString, mapInfo[i].DataFieldName));
                if (sb_v.Length <= 0)
                    sb_v.Append(string.Format(SqlScriptHandler.ValueTmepString, mapInfo[i].DataFieldName));
                else
                    sb_v.Append(",").Append(string.Format(SqlScriptHandler.ValueTmepString, mapInfo[i].DataFieldName));
                pars.Add(string.Format(SqlScriptHandler.ValueTmepString, mapInfo[i].DataFieldName), value);
            }
            MySqlCommand cmd = BuildIDbCommand(cnn, string.Format(SqlScriptHandler.Insert.InsertString, tableName, sb_c.ToString(), sb_v.ToString()));
            if (tran != null)
                cmd.Transaction = tran;
            FillParameters(cmd, pars);
            return cmd;
        }

        public static int ExecuteInsert(string connectionKey, string tableName, ParsList insertPars)
        {
            MySqlCommand cmd = GetInsertIDbCommandByUOPars(ConnectionHandler.CreateConnectionByKey(connectionKey), null, tableName, insertPars);
            using (cmd.Connection)
            {
                using (cmd)
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static int ExecuteInsert(MySqlConnection cnn, MySqlTransaction tran, string tableName, ParsList insertPars)
        {
            return GetInsertIDbCommandByUOPars(cnn, tran, tableName, insertPars).ExecuteNonQuery();
        }

        public static MySqlCommand GetInsertIDbCommandByUOPars(MySqlConnection cnn, MySqlTransaction tran, string tableName, ParsList insertPars)
        {
            StringBuilder sb_c = new StringBuilder();
            StringBuilder sb_v = new StringBuilder();
            Dictionary<string, object> pars = new Dictionary<string, object>();
            foreach (Pars p in insertPars)
            {
                if (sb_c.Length <= 0)
                    sb_c.Append(string.Format(SqlScriptHandler.ColumnTempString, p.Column));
                else
                    sb_c.Append(",").Append(string.Format(SqlScriptHandler.ColumnTempString, p.Column));
                if (sb_v.Length <= 0)
                    sb_v.Append(string.Format(SqlScriptHandler.ValueTmepString, p.Column));
                else
                    sb_v.Append(",").Append(string.Format(SqlScriptHandler.ValueTmepString, p.Column));
                pars.Add(string.Format(SqlScriptHandler.ValueTmepString, p.Column), p.Value);
            }
            MySqlCommand cmd = BuildIDbCommand(cnn, string.Format(SqlScriptHandler.Insert.InsertString, tableName, sb_c.ToString(), sb_v.ToString()));
            FillParameters(cmd, pars);
            if (tran != null)
                cmd.Transaction = tran;
            return cmd;
        }
        #endregion

        #region Delete
        public static int ExecuteDelete(string connectionKey, string tableName, ParsList pars)
        {
            MySqlCommand cmd = GetDeleteIDbCommand(ConnectionHandler.CreateConnectionByKey(connectionKey), null, tableName, pars);
            using (cmd.Connection)
            {
                using (cmd)
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static int ExecuteDelete(MySqlConnection cnn, MySqlTransaction tran, string tableName, ParsList pars)
        {
            return GetDeleteIDbCommand(cnn, tran, tableName, pars).ExecuteNonQuery();
        }

        public static MySqlCommand GetDeleteIDbCommand(MySqlConnection cnn, MySqlTransaction tran, string tableName, ParsList pars)
        {
            MySqlCommand cmd = BuildIDbCommand(cnn, SqlScriptHandler.Delete.GetDeleteString(tableName, pars));
            foreach (Pars p in pars)
                cmd.Parameters.Add(new MySqlParameter(string.Format(SqlScriptHandler.ValueTmepString, p.ColumnPar), SqlScriptHandler.ProcessValue(p.TokType, p.Value)));
            if (tran != null)
                cmd.Transaction = tran;
            return cmd;
        }
        #endregion

        #region Update
        public static MySqlCommand GetUpdateIDbCommand<T>(MySqlConnection cnn, MySqlTransaction tran, string tableName, T t, ParsList pars) where T : class, new()
        {
            StringBuilder sb_c = new StringBuilder();
            List<PropertyMappingInfo> mapInfo = ObjectHelper.GetProperties(typeof(T));
            Dictionary<string, object> pars_v = new Dictionary<string, object>();
            for (int i = 0; i < mapInfo.Count; i++)
            {
                if (mapInfo[i].DefaultValue.ToString().ToUpper().IndexOf(SqlScriptHandler.UnUpdate) > 0) continue;
                object value = mapInfo[i].PropertyInfo.GetValue(t, null);
                if (value != null)
                {
                    if (sb_c.Length <= 0)
                        sb_c.Append(string.Format(SqlScriptHandler.UpdateColumnTempString, mapInfo[i].DataFieldName));
                    else
                        sb_c.Append("," + string.Format(SqlScriptHandler.UpdateColumnTempString, mapInfo[i].DataFieldName));
                    pars_v.Add(string.Format(SqlScriptHandler.ValueTmepString, mapInfo[i].DataFieldName), value);
                }
            }
            MySqlCommand cmd = BuildIDbCommand(cnn, SqlScriptHandler.GetParsedExpression(string.Format(SqlScriptHandler.Update.UpdateString, "{0}", sb_c.ToString()), tableName, pars));
            foreach (Pars p in pars)
                cmd.Parameters.Add(new MySqlParameter(string.Format(SqlScriptHandler.ValueTmepString, p.ColumnPar), SqlScriptHandler.ProcessValue(p.TokType, p.Value)));
            FillParameters(cmd, pars_v);
            if (tran != null)
                cmd.Transaction = tran;
            return cmd;
        }

        public static int ExecuteUpdate<T>(string connectionKey, string tableName, T t, ParsList pars) where T : class, new()
        {
            MySqlCommand cmd = GetUpdateIDbCommand(ConnectionHandler.CreateConnectionByKey(connectionKey), null, tableName, t, pars);
            using (cmd.Connection)
            {
                using (cmd)
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static int ExecuteUpdate<T>(MySqlConnection cnn, MySqlTransaction tran, string tableName, T t, ParsList pars) where T : class, new()
        {
            return GetUpdateIDbCommand(cnn, tran, tableName, t, pars).ExecuteNonQuery();
        }

        public static int Update(string connectionKey, string tableName, ParsList updatePars, ParsList condisionPars)
        {
            MySqlCommand cmd = GetUpdateIDbCommandByUOPars(ConnectionHandler.CreateConnectionByKey(connectionKey), null, tableName, updatePars, condisionPars);
            using (cmd.Connection)
            {
                using (cmd)
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static int Update(MySqlConnection cnn, MySqlTransaction tran, string tableName, ParsList updatePars, ParsList condisionPars)
        {
            return GetUpdateIDbCommandByUOPars(cnn, tran, tableName, updatePars, condisionPars).ExecuteNonQuery();
        }

        public static MySqlCommand GetUpdateIDbCommandByUOPars(MySqlConnection cnn, MySqlTransaction tran, string tableName, ParsList updatePars, ParsList condisionPars)
        {
            StringBuilder sb_c = new StringBuilder();
            Dictionary<string, object> pars_v = new Dictionary<string, object>();
            foreach (Pars up in updatePars)
            {
                if (sb_c.Length <= 0)
                    sb_c.Append(string.Format(SqlScriptHandler.UpdateColumnTempString, up.Column));
                else
                    sb_c.Append("," + string.Format(SqlScriptHandler.UpdateColumnTempString, up.Column));
                pars_v.Add(string.Format(SqlScriptHandler.ValueTmepString, up.Column), up.Value);
            }
            MySqlCommand cmd = BuildIDbCommand(cnn, SqlScriptHandler.GetParsedExpression(string.Format(SqlScriptHandler.Update.UpdateString, "{0}", sb_c.ToString()), tableName, condisionPars));
            FillParameters(cmd, pars_v);
            foreach (Pars cp in condisionPars)
                cmd.Parameters.Add(new MySqlParameter(string.Format(SqlScriptHandler.ValueTmepString, cp.Column), SqlScriptHandler.ProcessValue(cp.TokType, cp.Value)));
            if (tran != null)
                cmd.Transaction = tran;
            return cmd;
        }
        #endregion
    }
}