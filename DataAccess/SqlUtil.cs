using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess;
using System.Text;
using DataMapping;

namespace DataAccess
{
    public class SqlUtil
    {
        #region Functions

        public static DateTime GetDbTime(IDbConnection conn)
        {
            IDataReader read = DataAccess.SqlUtil.ExecuteReader(conn, @"select getdate()");
            using (read)
            {
                DateTime dt = DateTime.Now;
                if (read.Read())
                {
                    dt = (DateTime)read[0];
                }
                return dt;
            }
        }

        public static int GetExecuteScalar(IDbConnection conn, string cmdText)
        {
            IDbCommand cmd = GetCommandByScript(conn, cmdText);
            Log.LogCommand(cmd);
            return (int)cmd.ExecuteScalar();
        }

        public static int GetExecuteScalarByParsCollection(IDbConnection conn, string tableName, ParameterCollection pc)
        {
            IDbCommand cmd = GetCommandByParsCollection(conn, tableName, pc);
            Log.LogCommand(cmd);
            return (int)cmd.ExecuteScalar();
        }

        public static int GetRecordsCountByParsCollection(IDbConnection conn, string tableName, ParameterCollection pc)
        {
            IDbCommand cmd = GetRecordsCountCommandByParsCollection(conn, tableName, pc);
            Log.LogCommand(cmd);
            return (int)cmd.ExecuteScalar();
        }

        public static void FillCommandByParsCollection(IDbCommand cmd, ParameterCollection pc)
        {
            foreach (Parameter p in pc)
            {
                if (p.TokType != TokenTypes.IsNotNull && p.TokType != TokenTypes.IsNull && p.Column != null && p.TokType != TokenTypes.In && p.TokType != TokenTypes.NotIn)
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(string.Format(SqlScriptHandler.ValueTmepString, p.ColumnPar), SqlScriptHandler.ProcessValue(p.TokType, p.Value)));
            }
        }

        public static IDataParameter SetDataParameter(IDbCommand cmd, string parName, object value)
        {
            IDataParameter par = cmd.CreateParameter();
            par.Value = value;
            par.ParameterName = parName;
            return par;
        }

        #region Command

        public static IDbCommand GetCommandByParsCollection(IDbConnection conn, string tableName, ParameterCollection pc)
        {
            IDbCommand cmd = GetCommandByScript(conn, string.Format(SqlScriptHandler.Search.SelectString, "", tableName) + SqlScriptHandler.GetCondition(pc, false));
            FillCommandByParsCollection(cmd, pc);
            return cmd;
        }

        public static IDbCommand GetRecordsCountCommandByParsCollection(IDbConnection conn, string tableName, ParameterCollection pc)
        {
            IDbCommand cmd = GetCommandByScript(conn, string.Format(SqlScriptHandler.Search.SelectRecordCountString, tableName) + SqlScriptHandler.GetCondition(pc, false));
            FillCommandByParsCollection(cmd, pc);
            return cmd;
        }

        public static IDbCommand GetCommandByScript(IDbConnection conn, string cmdText, IDataParameterCollection parameters)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = cmdText;
            cmd.Parameters.Add(parameters);
            return cmd;
        }

        public static IDbCommand GetCommandByScript(IDbConnection conn, string cmdText)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = cmdText;
            return cmd;
        }


        private static IDbCommand GetCommandByStoreProc(IDbConnection conn, string storedProcName, IDataParameter[] parameters)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = storedProcName;
            foreach (IDataParameter par in parameters)
            {
                if (par != null)
                {
                    //if (par.Value == null)
                    //{
                    //    par.Value = DBNull.Value;
                    //}
                    cmd.Parameters.Add(par);
                }
            }
            return cmd;
        }

        public static void FillParameters(IDbCommand cmd, Dictionary<string, object> parameters)
        {
            foreach (string key in parameters.Keys)
            {
                if (parameters[key] == null)
                {
                    cmd.Parameters.Add(SetDataParameter(cmd, key, DBNull.Value));
                }
                else if (parameters[key].GetType() == typeof(DateTime) && ((DateTime)parameters[key]) == DateTimeValues.Null)
                {
                    cmd.Parameters.Add(SetDataParameter(cmd, key, DBNull.Value));
                }
                else if (parameters[key].GetType() == typeof(DateTime) && ((DateTime)parameters[key]) == DateTimeValues.DbTime)
                {
                    continue;
                }
                else
                {
                    cmd.Parameters.Add(SetDataParameter(cmd, key, parameters[key]));
                }
            }
        }

        #endregion

        #region DataReader

        public static IDataReader ExecuteProcedureReader(IDbConnection conn, string storedProcName, IDataParameter[] parameters)
        {
            IDataReader returnReader;
            IDbCommand cmd = GetCommandByStoreProc(conn, storedProcName, parameters);
            Log.LogCommand(cmd);
            returnReader = cmd.ExecuteReader();
            return returnReader;
        }

        public static IDataReader ExecuteProcedureReader(IDbConnection conn, string storedProcName, SqlParameter parameters)
        {
            return ExecuteProcedureReader(conn, storedProcName, new SqlParameter[] { parameters });
        }

        public static IDataReader ExecuteReader(IDbConnection conn, string cmdText)
        {
            IDbCommand cmd = GetCommandByScript(conn, cmdText);
            Log.LogCommand(cmd);
            return cmd.ExecuteReader();
        }

        public static IDataReader ExecuteReader(IDbConnection conn, IDbTransaction tran, string cmdText)
        {
            IDbCommand cmd = GetCommandByScript(conn, cmdText);
            cmd.Transaction = tran;
            Log.LogCommand(cmd);
            return cmd.ExecuteReader();
        }

        #endregion

        #region DataSet

        public static DataSet ExecuteProcedureDataSet(IDbConnection conn, string storedProcName, params IDataParameter[] parameters)
        {
            return ExecuteDataSet(GetCommandByStoreProc(conn, storedProcName, parameters));
        }

        public static DataSet ExecuteDataSet(IDbConnection conn, string cmdText)
        {
            return ExecuteDataSet((SqlCommand)GetCommandByScript(conn, cmdText));
        }

        public static DataSet ExecuteDataSet(IDbCommand cmd)
        {
            DataSet dsReturn = new DataSet();
            IDbDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            Log.LogCommand(cmd);
            da.Fill(dsReturn);
            return dsReturn;
        }

        public static DataSet ExecuteDataSetByParsCollection(IDbConnection conn, string tableName, ParameterCollection pc)
        {
            return ExecuteDataSet(GetCommandByParsCollection(conn, tableName, pc));
        }

        #endregion

        #endregion

        #region Query Functions

        public static C GetObjectList<T, C>(IDbConnection conn, string baseSql, ParameterCollection pc)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            return GetObjectList<T, C>(conn, baseSql, pc, true, null);
        }

        public static C GetObjectList<T, C>(IDbConnection conn, string baseSql, ParameterCollection pc, bool isAsc, params object[] orderBy)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            return GetObjectList<T, C>(GetIDbCommand(conn, baseSql, pc, isAsc, orderBy));
        }

        public static IDbCommand GetIDbCommand(IDbConnection conn, string baseSql, ParameterCollection pc, bool isAsc, params object[] orderBy)
        {
            string orderby = SqlScriptHandler.ArrayToString(orderBy, ",", true);
            if (!string.IsNullOrEmpty(orderby))
            {
                orderby = " order by " + orderby;
                orderby += (isAsc ? " asc" : " desc");
            }
            string conditions = SqlScriptHandler.GetConditionsFromParsList(pc);
            if (!string.IsNullOrEmpty(conditions))
            {
                if (baseSql.ToLower().IndexOf(" where ") < 0)
                {
                    baseSql += " where ";
                }
                else
                {
                    baseSql += " and ";
                }
                baseSql += conditions;
            }
            baseSql += orderby;
            return GetCommandByScript(conn, baseSql);
        }

        public static C GetObjectList<T, C>(IDbConnection conn, string cmdText)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            return GetObjectList<T, C>(GetCommandByScript(conn, cmdText));
        }

        public static C GetObjectList<T, C>(IDbCommand cmd)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            Log.LogCommand(cmd);
            IDataReader rdr = cmd.ExecuteReader();
            using (rdr)
            {
                return ObjectHelper.FillCollection<T, C>(rdr);
            }
        }

        public static C GetObjectList<T, C>(IDbConnection conn, string tableName, ParameterCollection pc, int count)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            return GetObjectList<T, C>(conn, tableName, pc, count, null);
        }

        public static C GetObjectList<T, C>(IDbConnection conn, string tableName, ParameterCollection pc, int count, bool asc, params string[] orderBy)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            IDbCommand cmd = GetIDbCommand(conn, tableName, pc, count, asc, orderBy);
            return DataAccess.SqlUtil.GetObjectList<T, C>(cmd);
        }

        public static IDbCommand GetIDbCommand(IDbConnection conn, string tableName, ParameterCollection pc, int count, bool asc, params string[] orderBy)
        {
            IDbCommand cmd = DataAccess.SqlUtil.GetCommandByScript(conn, SqlScriptHandler.Search.GetSelectString(tableName, pc, asc, count, orderBy));
            FillCommandByParsCollection(cmd, pc);
            return cmd;
        }

        public static C GetObjectList<T, C>(IDbConnection conn, string tableName, string[] columns, ParameterCollection pc, int count, bool asc, params string[] orderBy)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            IDbCommand cmd = GetIDbCommand(conn, tableName, columns, pc, count, asc, orderBy);
            return DataAccess.SqlUtil.GetObjectList<T, C>(cmd);
        }

        public static IDbCommand GetIDbCommand(IDbConnection conn, string tableName, string[] columns, ParameterCollection pc, int count, bool asc, params string[] orderBy)
        {
            IDbCommand cmd = DataAccess.SqlUtil.GetCommandByScript(conn, SqlScriptHandler.Search.GetSelectString(tableName, pc, asc, count, orderBy));
            FillCommandByParsCollection(cmd, pc);
            return cmd;
        }

        public static C GetObjectList<T, C>(IDbConnection conn, string tableName, ParameterCollection pc, int count, params string[] orderBy)
            where T : class, new()
            where C : ICollection<T>, new()
        {
            return GetObjectList<T, C>(conn, tableName, pc, count, true, orderBy);
        }

        #endregion

        #region Insert

        public static int GetIdentityId(IDbConnection conn)
        {
            IDataReader idr = ExecuteReader(conn, SqlScriptHandler.Search.SelectIdentityId);
            using (idr)
            {
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
        }

        public static int GetIdentityId(IDbConnection conn, IDbTransaction tran)
        {
            IDataReader idr = ExecuteReader(conn, tran, SqlScriptHandler.Search.SelectIdentityId);
            using (idr)
            {
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
        }

        public static int ExecuteInsert<T>(IDbConnection conn, string tableName, T t) where T : class, new()
        {
            IDbCommand cmd = GetInsertIDbCommand(conn, null, tableName, t);
            Log.LogCommand(cmd);
            if (cmd.ExecuteNonQuery() > 0)
            {
                return GetIdentityId(cmd.Connection);
            }
            return 0;
        }

        public static int ExecuteInsert<T>(IDbConnection conn, IDbTransaction tran, string tableName, T t) where T : class, new()
        {
            if (GetInsertIDbCommand(conn, tran, tableName, t).ExecuteNonQuery() > 0)
            {
                return GetIdentityId(conn, tran);
            }
            return 0;
        }

        public static IDbCommand GetInsertIDbCommand<T>(IDbConnection conn, IDbTransaction tran, string tableName, T t) where T : class, new()
        {
            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            List<PropertyMappingInfo> mappingInfos = ObjectHelper.GetProperties(typeof(T));
            Dictionary<string, object> pars = new Dictionary<string, object>();
            for (int i = 0; i < mappingInfos.Count; i++)
            {
                if (mappingInfos[i].DefaultValue.ToString().ToUpper().IndexOf(SqlScriptHandler.UnInsert) > 0) continue;
                object value = mappingInfos[i].PropertyInfo.GetValue(t, null);
                if (value == null) continue;
                bool isUseDbTime = false;
                if (mappingInfos[i].PropertyInfo.PropertyType == typeof(DateTime) && (DateTime)value == DateTimeValues.DbTime)
                {
                    isUseDbTime = true;
                }
                if (value.GetType() == typeof(DateTime) && (DateTime)value == DateTimeValues.Null)
                {
                    continue;
                }
                if (sbColumns.Length <= 0)
                    sbColumns.Append(string.Format(SqlScriptHandler.ColumnTempString, mappingInfos[i].DataFieldName));
                else
                    sbColumns.Append(",").Append(string.Format(SqlScriptHandler.ColumnTempString, mappingInfos[i].DataFieldName));

                if (isUseDbTime)
                {
                    if (sbValues.Length <= 0)
                        sbValues.Append(SqlScriptHandler.GetDBTime);
                    else
                        sbValues.Append(",").Append(SqlScriptHandler.GetDBTime);
                }
                else
                {
                    if (sbValues.Length <= 0)
                        sbValues.Append(string.Format(SqlScriptHandler.ValueTmepString, mappingInfos[i].DataFieldName));
                    else
                        sbValues.Append(",").Append(string.Format(SqlScriptHandler.ValueTmepString, mappingInfos[i].DataFieldName));
                }
                pars.Add(string.Format(SqlScriptHandler.ValueTmepString, mappingInfos[i].DataFieldName), value);
            }
            IDbCommand cmd = GetCommandByScript(conn, string.Format(SqlScriptHandler.Insert.InsertString, tableName, sbColumns.ToString(), sbValues.ToString()));
            if (tran != null)
                cmd.Transaction = tran;
            FillParameters(cmd, pars);
            return cmd;
        }

        public static int ExecuteInsert(IDbConnection conn, string tableName, ParameterCollection insertParameters)
        {
            IDbCommand cmd = GetInsertIDbCommandByUOPars(conn, null, tableName, insertParameters);
            Log.LogCommand(cmd);
            return cmd.ExecuteNonQuery();
        }

        public static int ExecuteInsert(IDbConnection conn, IDbTransaction tran, string tableName, ParameterCollection insertParameters)
        {
            return GetInsertIDbCommandByUOPars(conn, tran, tableName, insertParameters).ExecuteNonQuery();
        }

        public static IDbCommand GetInsertIDbCommandByUOPars(IDbConnection conn, IDbTransaction tran, string tableName, ParameterCollection insertParameters)
        {
            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            Dictionary<string, object> pars = new Dictionary<string, object>();
            foreach (Parameter p in insertParameters)
            {
                if (sbColumns.Length <= 0)
                    sbColumns.Append(string.Format(SqlScriptHandler.ColumnTempString, p.Column));
                else
                    sbColumns.Append(",").Append(string.Format(SqlScriptHandler.ColumnTempString, p.Column));
                if (sbValues.Length <= 0)
                    sbValues.Append(string.Format(SqlScriptHandler.ValueTmepString, p.Column));
                else
                    sbValues.Append(",").Append(string.Format(SqlScriptHandler.ValueTmepString, p.Column));
                pars.Add(string.Format(SqlScriptHandler.ValueTmepString, p.Column), p.Value);
            }
            IDbCommand cmd = GetCommandByScript(conn, string.Format(SqlScriptHandler.Insert.InsertString, tableName, sbColumns.ToString(), sbValues.ToString()));
            FillParameters(cmd, pars);
            if (tran != null)
                cmd.Transaction = tran;
            return cmd;
        }

        #endregion

        #region Delete

        public static int ExecuteDelete(IDbConnection conn, string tableName, ParameterCollection pc)
        {
            IDbCommand cmd = GetDeleteIDbCommand(conn, null, tableName, pc);
            Log.LogCommand(cmd);
            return cmd.ExecuteNonQuery();
        }

        public static int ExecuteDelete(IDbConnection conn, IDbTransaction tran, string tableName, ParameterCollection pc)
        {
            return GetDeleteIDbCommand(conn, tran, tableName, pc).ExecuteNonQuery();
        }

        public static IDbCommand GetDeleteIDbCommand(IDbConnection conn, IDbTransaction tran, string tableName, ParameterCollection pc)
        {
            IDbCommand cmd = GetCommandByScript(conn, SqlScriptHandler.Delete.GetDeleteString(tableName, pc));
            foreach (Parameter p in pc)
            {
                if (p.TokType != TokenTypes.In && p.TokType != TokenTypes.NotIn)
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(string.Format(SqlScriptHandler.ValueTmepString, p.ColumnPar), SqlScriptHandler.ProcessValue(p.TokType, p.Value)));
            }
            if (tran != null)
                cmd.Transaction = tran;
            return cmd;
        }

        #endregion

        #region Update

        public static IDbCommand GetUpdateIDbCommand<T>(IDbConnection conn, IDbTransaction tran, string tableName, T t, ParameterCollection pc, bool isUpdateAll) where T : class, new()
        {
            StringBuilder sbColumns = new StringBuilder();
            List<PropertyMappingInfo> mappingInfos = ObjectHelper.GetProperties(typeof(T));
            Dictionary<string, object> dValues = new Dictionary<string, object>();
            for (int i = 0; i < mappingInfos.Count; i++)
            {
                if (mappingInfos[i].DefaultValue.ToString().ToUpper().IndexOf(SqlScriptHandler.UnUpdate) > 0) continue;
                object value = mappingInfos[i].PropertyInfo.GetValue(t, null);
                bool isUseDbTime = false;
                if (mappingInfos[i].PropertyInfo.PropertyType == typeof(DateTime) && (DateTime)value == DateTimeValues.DbTime)
                {
                    isUseDbTime = true;
                }
                if (value != null || isUpdateAll)
                {
                    if (isUseDbTime)
                    {
                        if (sbColumns.Length <= 0)
                            sbColumns.Append(string.Format(SqlScriptHandler.ColumnWithValueNotParameter, mappingInfos[i].DataFieldName, SqlScriptHandler.GetDBTime));
                        else
                            sbColumns.Append(",").Append(string.Format(SqlScriptHandler.ColumnWithValueNotParameter, mappingInfos[i].DataFieldName, SqlScriptHandler.GetDBTime));
                    }
                    else
                    {
                        if (sbColumns.Length <= 0)
                            sbColumns.Append(string.Format(SqlScriptHandler.ColumnWithValueTempString, mappingInfos[i].DataFieldName));
                        else
                            sbColumns.Append(",").Append(string.Format(SqlScriptHandler.ColumnWithValueTempString, mappingInfos[i].DataFieldName));
                        dValues.Add(string.Format(SqlScriptHandler.ValueTmepString, mappingInfos[i].DataFieldName), value);
                    }
                }
            }
            IDbCommand cmd = GetCommandByScript(conn, SqlScriptHandler.GetParsedExpression(string.Format(SqlScriptHandler.Update.UpdateString, "{0}", sbColumns.ToString()), tableName, pc));
            foreach (Parameter p in pc)
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(string.Format(SqlScriptHandler.ValueTmepString, p.ColumnPar), SqlScriptHandler.ProcessValue(p.TokType, p.Value)));
            FillParameters(cmd, dValues);
            if (tran != null)
                cmd.Transaction = tran;
            return cmd;
        }

        public static int ExecuteUpdate<T>(IDbConnection conn, string tableName, T t, ParameterCollection pc, bool isUpdateAll) where T : class, new()
        {
            IDbCommand cmd = GetUpdateIDbCommand(conn, null, tableName, t, pc, isUpdateAll);
            Log.LogCommand(cmd);
            return cmd.ExecuteNonQuery();
        }

        public static int ExecuteUpdate<T>(IDbConnection conn, IDbTransaction tran, string tableName, T t, ParameterCollection pc, bool isUpdateAll) where T : class, new()
        {
            return GetUpdateIDbCommand(conn, tran, tableName, t, pc, isUpdateAll).ExecuteNonQuery();
        }

        public static int Update(IDbConnection conn, string tableName, ParameterCollection pcValues, ParameterCollection condisionPars)
        {
            IDbCommand cmd = GetUpdateIDbCommandByUOPars(conn, null, tableName, pcValues, condisionPars);
            Log.LogCommand(cmd);
            return cmd.ExecuteNonQuery();
        }

        public static int Update(IDbConnection conn, IDbTransaction tran, string tableName, ParameterCollection pcValues, ParameterCollection condisionPars)
        {
            return GetUpdateIDbCommandByUOPars(conn, tran, tableName, pcValues, condisionPars).ExecuteNonQuery();
        }

        public static IDbCommand GetUpdateIDbCommandByUOPars(IDbConnection conn, IDbTransaction tran, string tableName, ParameterCollection pcValues, ParameterCollection pcConditions)
        {
            StringBuilder sbColumns = new StringBuilder();
            Dictionary<string, object> dValues = new Dictionary<string, object>();
            foreach (Parameter up in pcValues)
            {
                bool isUseDbTime = false;
                if (up.Value.GetType() == typeof(DateTime) && (DateTime)up.Value == DateTimeValues.DbTime)
                {
                    isUseDbTime = true;
                }
                if (isUseDbTime)
                {
                    if (sbColumns.Length <= 0)
                        sbColumns.Append(string.Format(SqlScriptHandler.ColumnWithValueNotParameter, up.Column, SqlScriptHandler.GetDBTime));
                    else
                        sbColumns.Append("," + string.Format(SqlScriptHandler.ColumnWithValueNotParameter, up.Column, SqlScriptHandler.GetDBTime));
                }
                else
                {
                    if (sbColumns.Length <= 0)
                        sbColumns.Append(string.Format(SqlScriptHandler.ColumnWithValueTempString, up.Column));
                    else
                        sbColumns.Append("," + string.Format(SqlScriptHandler.ColumnWithValueTempString, up.Column));
                    dValues.Add(string.Format(SqlScriptHandler.ValueTmepString, up.Column), up.Value);
                }
            }
            IDbCommand cmd = GetCommandByScript(conn, SqlScriptHandler.GetParsedExpression(string.Format(SqlScriptHandler.Update.UpdateString, "{0}", sbColumns.ToString()), tableName, pcConditions));
            FillParameters(cmd, dValues);
            foreach (Parameter cp in pcConditions)
            {
                if (cp.TokType != TokenTypes.In && cp.TokType != TokenTypes.NotIn)
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(string.Format(SqlScriptHandler.ValueTmepString, cp.Column), SqlScriptHandler.ProcessValue(cp.TokType, cp.Value)));
            }
            if (tran != null)
                cmd.Transaction = tran;
            return cmd;
        }

        #endregion
    }
}
