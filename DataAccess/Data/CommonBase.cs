using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DataAccess.Data
{
    public class CommonBase<T, C> : Interfaces.ICommonBase<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        #region Attributes
        [System.Xml.Serialization.XmlIgnore()]
        private ConnectionInformation _ConnInfo;
        public ConnectionInformation ConnInfo
        {
            get { return _ConnInfo; }
            set { _ConnInfo = value; }
        }

        #endregion

        #region Query Functions

        /// <summary>
        /// Get database time.
        /// </summary>
        /// <returns>DateTime</returns>
        public DateTime GetDbTime()
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                return SqlUtil.GetDbTime(conn);
            }
        }

        /// <summary>
        /// Get records count.
        /// </summary>
        /// <returns>int</returns>
        public int GetRecordsCount()
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName).Count;
                }
                else
                {
                    return SqlUtil.GetExecuteScalar(conn, string.Format(SqlScriptHandler.Search.SelectRecordCountString, ConnInfo.TableName));
                }
            }
        }

        /// <summary>
        /// Get records by parameter.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>int</returns>
        public int GetRecordsCount(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc).Count;
                }
                else
                {
                    return SqlUtil.GetRecordsCountByParsCollection(conn, ConnInfo.TableName, pc);
                }
            }
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <param name="fields">Return fileds.</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(ParameterCollection pc, int pageIndex, int pageSize, string sortBy, bool isAsc, params string[] fields)
        {
            return GetPagingList(SqlScriptHandler.GetConditionsFromParsList(pc), pageIndex, pageSize, sortBy, isAsc, fields);
        }

        /// <summary>
        /// Get paging records by sql conditions.
        /// </summary>
        /// <param name="parameters">Sql conditions</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <param name="fields">Return fileds.</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(string conditions, int pageIndex, int pageSize, string sortBy, bool isAsc, params string[] fields)
        {
            using (IDbConnection conn = ConnInfo.Connection)
            {
                return PagingResult<T, C>.GetPagingList(conn, ConnInfo.TableName, ConnInfo.PrimaryKeys, pageIndex, pageSize, sortBy == null ? null : new string[] { sortBy }, isAsc, conditions, fields);
            }
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(ParameterCollection pc, int pageIndex, int pageSize, string sortBy, bool isAsc)
        {
            return GetPagingList(pc, pageIndex, pageSize, sortBy, isAsc, null);
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(ParameterCollection pc, int pageIndex, int pageSize, object sortBy, bool isAsc)
        {
            return GetPagingList(pc, pageIndex, pageSize, sortBy.ToString(), isAsc, null);
        }

        public C GetList(int count)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName);
                    return ReturnTop(c, count);
                }
                else
                {
                    return GetList(SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, count));
                }
            }
        }

        public C GetList(object[] columns, int count)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return GetList(count);
            }
            else
            {
                return GetList(SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, columns, count));
            }
        }

        private C ReturnTop(C c, int count)
        {
            C ret = new C();
            int i = 0;
            foreach (T t in c)
            {
                if (i >= count) break;
                ret.Add(t);
                i++;
            }
            return ret;
        }

        public C GetList(int count, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                    return ReturnTop(c, count);
                }
                else
                {
                    return GetList(count, true, orderBy);
                }
            }
        }

        public C GetList(int count, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                    return ReturnTop(c, count);
                }
                else
                {
                    return GetList(count, true, orderBy);
                }
            }
        }

        public C GetList(int count, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    return ReturnTop(c, count);
                }
                else
                {
                    return GetList(SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                }
            }
        }

        public C GetList(int count, bool asc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    return ReturnTop(c, count);
                }
                else
                {
                    return GetList(SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                }
            }
        }

        public C GetAllList()
        {
            return GetList(ConnInfo.DefaultSelect);
        }

        public C GetAllList(object[] columns)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return GetAllList();
            }
            else
            {
                return GetList(SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
            }
        }

        public C GetAllList(string[] columns)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return GetAllList();
            }
            else
            {
                return GetList(SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
            }
        }

        public C GetAllList(bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                }
                else
                {
                    return GetList(SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, asc, orderBy));
                }
            }
        }

        public C GetAllList(bool asc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                }
                else
                {
                    return GetAllList(asc, SqlScriptHandler.ObjectsToStrings(orderBy));
                }
            }
        }

        public C GetAllList(string[] columns, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                }
                else
                {
                    return GetList(SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                }
            }
        }

        public C GetAllList(object[] columns, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                }
                else
                {
                    return GetList(SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                }
            }
        }

        public C GetList(string cmdText)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                return DataAccess.SqlUtil.GetObjectList<T, C>(conn, cmdText);
            }
        }

        public C GetList(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc);
                }
                else
                {
                    return GetList(pc, null);
                }
            }
        }

        public C GetList(ParameterCollection pc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                }
                else
                {
                    return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, orderBy);
                }
            }
        }

        public C GetList(ParameterCollection pc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                }
                else
                {
                    return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, SqlScriptHandler.ObjectsToStrings(orderBy));
                }
            }
        }

        public C GetList(ParameterCollection pc, int count, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                    return ReturnTop(c, count);
                }
                else
                {
                    return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, orderBy);
                }
            }
        }

        public C GetList(ParameterCollection pc, int count, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                    return ReturnTop(c, count);
                }
                else
                {
                    return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, SqlScriptHandler.ObjectsToStrings(orderBy));
                }
            }
        }

        public C GetList(ParameterCollection pc, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                }
                else
                {
                    return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, orderBy);
                }
            }
        }

        public C GetList(ParameterCollection pc, bool asc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                }
                else
                {
                    return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, SqlScriptHandler.ObjectsToStrings(orderBy));
                }
            }
        }

        public C GetList(ParameterCollection pc, int count, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                    return ReturnTop(c, count);
                }
                else
                {
                    return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, orderBy);
                }
            }
        }

        public C GetList(ParameterCollection pc, int count, bool asc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                    return ReturnTop(c, count);
                }
                else
                {
                    return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, SqlScriptHandler.ObjectsToStrings(orderBy));
                }
            }
        }

        public C GetList(IDbCommand cmd)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                //IDbConnection cnn = DataAccess.ConnectionHelper.CreateConnectionByKey(ConnInfo.ConnectionKey);
                cmd.Connection = conn;
                return DataAccess.SqlUtil.GetObjectList<T, C>(cmd);
            }
        }

        #region DataSet

        public DataSet GetDataSet(string cmdText)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                return SqlUtil.ExecuteDataSet(conn, cmdText);
            }
        }

        public DataSet GetDataSet()
        {
            return GetDataSet(this.ConnInfo.DefaultSelect);
        }

        public DataSet GetDataSet(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                return SqlUtil.ExecuteDataSetByParsCollection(conn, this.ConnInfo.TableName, pc);
            }
        }

        #endregion

        #region Command

        public IDbCommand GetIDbCommand(ParameterCollection pc, string[] columns, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                }
                else
                {
                    return DataAccess.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, columns, pc, 0, asc, orderBy);
                }
            }
        }

        public IDbCommand GetIDbCommand(ParameterCollection pc, int top, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.IsSqlSentence)
                {
                    return SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                }
                else
                {
                    return DataAccess.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, top, asc, orderBy);
                }
            }
        }

        #endregion

        #endregion

        #region Query Functions With Connection

        /// <summary>
        /// Get database time.
        /// </summary>
        /// <returns>DateTime</returns>
        public DateTime GetDbTime(System.Data.IDbConnection conn)
        {
            return SqlUtil.GetDbTime(conn);
        }

        /// <summary>
        /// Get records count.
        /// </summary>
        /// <returns>int</returns>
        public int GetRecordsCount(System.Data.IDbConnection conn)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName).Count;
            }
            else
            {
                return SqlUtil.GetExecuteScalar(conn, string.Format(SqlScriptHandler.Search.SelectRecordCountString, ConnInfo.TableName));
            }
        }

        /// <summary>
        /// Get records by parameter.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>int</returns>
        public int GetRecordsCount(System.Data.IDbConnection conn, ParameterCollection pc)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc).Count;
            }
            else
            {
                return SqlUtil.GetRecordsCountByParsCollection(conn, ConnInfo.TableName, pc);
            }
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <param name="fields">Return fileds.</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(IDbConnection conn, ParameterCollection pc, int pageIndex, int pageSize, string sortBy, bool isAsc, params string[] fields)
        {
            return GetPagingList(conn, SqlScriptHandler.GetConditionsFromParsList(pc), pageIndex, pageSize, sortBy, isAsc, fields);
        }

        /// <summary>
        /// Get paging records by sql conditions.
        /// </summary>
        /// <param name="parameters">Sql conditions</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <param name="fields">Return fileds.</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(IDbConnection conn, string conditions, int pageIndex, int pageSize, string sortBy, bool isAsc, params string[] fields)
        {
            return PagingResult<T, C>.GetPagingList(conn, ConnInfo.TableName, ConnInfo.PrimaryKeys, pageIndex, pageSize, sortBy == null ? null : new string[] { sortBy }, isAsc, conditions, fields);
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(IDbConnection conn, ParameterCollection pc, int pageIndex, int pageSize, string sortBy, bool isAsc)
        {
            return GetPagingList(conn, pc, pageIndex, pageSize, sortBy, isAsc, null);
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(IDbConnection conn, ParameterCollection pc, int pageIndex, int pageSize, object sortBy, bool isAsc)
        {
            return GetPagingList(conn, pc, pageIndex, pageSize, sortBy.ToString(), isAsc, null);
        }

        public C GetList(IDbConnection conn, int count)
        {
            if (ConnInfo.IsSqlSentence)
            {
                C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName);
                return ReturnTop(c, count);
            }
            else
            {
                return GetList(conn, SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, count));
            }
        }

        public C GetList(IDbConnection conn, object[] columns, int count)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return GetList(conn, count);
            }
            else
            {
                return GetList(conn, SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, columns, count));
            }
        }

        public C GetList(IDbConnection conn, int count, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                return ReturnTop(c, count);
            }
            else
            {
                return GetList(count, true, orderBy);
            }
        }

        public C GetList(IDbConnection conn, int count, params object[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                return ReturnTop(c, count);
            }
            else
            {
                return GetList(count, true, orderBy);
            }
        }

        public C GetList(IDbConnection conn, int count, bool asc, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                return ReturnTop(c, count);
            }
            else
            {
                return GetList(conn, SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
            }
        }

        public C GetList(IDbConnection conn, int count, bool asc, params object[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                return ReturnTop(c, count);
            }
            else
            {
                return GetList(conn, SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
            }
        }

        public C GetAllList(IDbConnection conn)
        {
            return GetList(conn, ConnInfo.DefaultSelect);
        }

        public C GetAllList(IDbConnection conn, object[] columns)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return GetAllList(conn);
            }
            else
            {
                return GetList(conn, SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
            }
        }

        public C GetAllList(IDbConnection conn, string[] columns)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return GetAllList(conn);
            }
            else
            {
                return GetList(conn, SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
            }
        }

        public C GetAllList(IDbConnection conn, bool asc, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
            }
            else
            {
                return GetList(conn, SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, asc, orderBy));
            }
        }

        public C GetAllList(IDbConnection conn, bool asc, params object[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
            }
            else
            {
                return GetAllList(conn, asc, SqlScriptHandler.ObjectsToStrings(orderBy));
            }
        }

        public C GetAllList(IDbConnection conn, string[] columns, bool asc, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
            }
            else
            {
                return GetList(conn, SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
            }
        }

        public C GetAllList(IDbConnection conn, object[] columns, bool asc, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
            }
            else
            {
                return GetList(conn, SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
            }
        }

        public C GetList(IDbConnection conn, string cmdText)
        {
            return DataAccess.SqlUtil.GetObjectList<T, C>(conn, cmdText);
        }

        public C GetList(IDbConnection conn, ParameterCollection pc)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc);
            }
            else
            {
                return GetList(conn, pc, null);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
            }
            else
            {
                return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, orderBy);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, params object[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
            }
            else
            {
                return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, SqlScriptHandler.ObjectsToStrings(orderBy));
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, int count, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                return ReturnTop(c, count);
            }
            else
            {
                return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, orderBy);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, int count, params object[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                return ReturnTop(c, count);
            }
            else
            {
                return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, SqlScriptHandler.ObjectsToStrings(orderBy));
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, bool asc, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
            }
            else
            {
                return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, orderBy);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, bool asc, params object[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
            }
            else
            {
                return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, SqlScriptHandler.ObjectsToStrings(orderBy));
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, int count, bool asc, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                return ReturnTop(c, count);
            }
            else
            {
                return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, orderBy);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, int count, bool asc, params object[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                C c = SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                return ReturnTop(c, count);
            }
            else
            {
                return DataAccess.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, SqlScriptHandler.ObjectsToStrings(orderBy));
            }
        }

        public C GetList(IDbConnection conn, IDbCommand cmd)
        {
            cmd.Connection = conn;
            return DataAccess.SqlUtil.GetObjectList<T, C>(cmd);
        }

        #region DataSet

        public DataSet GetDataSet(IDbConnection conn, string cmdText)
        {
            return SqlUtil.ExecuteDataSet(conn, cmdText);
        }

        public DataSet GetDataSet(IDbConnection conn)
        {
            return GetDataSet(conn, this.ConnInfo.DefaultSelect);
        }

        public DataSet GetDataSet(IDbConnection conn, ParameterCollection pc)
        {
            return SqlUtil.ExecuteDataSetByParsCollection(conn, this.ConnInfo.TableName, pc);
        }

        #endregion

        #region Command

        public IDbCommand GetIDbCommand(IDbConnection conn, ParameterCollection pc, string[] columns, bool asc, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
            }
            else
            {
                return DataAccess.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, columns, pc, 0, asc, orderBy);
            }
        }

        public IDbCommand GetIDbCommand(IDbConnection conn, ParameterCollection pc, int top, bool asc, params string[] orderBy)
        {
            if (ConnInfo.IsSqlSentence)
            {
                return SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
            }
            else
            {
                return DataAccess.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, top, asc, orderBy);
            }
        }

        #endregion

        #endregion

        #region Other Functions

        /// <summary>
        /// Get IDbconnection by this connection key.
        /// </summary>
        /// <returns>IDbConnection</returns>
        public IDbConnection GetConnection()
        {
            return ConnectionHelper.CreateConnectionByKey(this.ConnInfo.ConnectionKey);
        }

        public DataTable GetTableSchema()
        {
            string key = this.ToString();
            if (!Stater.DataTableSchema.ContainsKey(key))
            {
                string sql = string.Concat(this.ConnInfo.DefaultSelect, this.ConnInfo.DefaultSelect.ToLower().IndexOf(" where ") > 0 ? " and " : " where ", "1=2");
                Stater.DataTableSchema[key] = GetDataSet(sql).Tables[0];
            }
            return Stater.DataTableSchema[key];
        }

        #endregion
    }
}
