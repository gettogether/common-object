using System;
using System.Collections.Generic;
using System.Text;
using DataAccess;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess.Data
{
    public class DOBase<T, C> : CommonBase<T, C>, Interfaces.IDOBase<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        #region Constructors
        public DOBase(string tableName, string connKey)
        {
            ConnInfo = new ConnectionInformation(tableName, connKey);
        }

        public DOBase()
        {

        }
        #endregion

        #region Delete Functions
        public int Delete(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc)
        {
            return DataAccess.SqlUtil.ExecuteDelete(cnn, tran, ConnInfo.TableName, pc);
        }

        public int Delete(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                return DataAccess.SqlUtil.ExecuteDelete(conn, ConnInfo.TableName, pc);
            }
        }

        public int Delete(IDbConnection conn, ParameterCollection pc)
        {
            return DataAccess.SqlUtil.ExecuteDelete(conn, ConnInfo.TableName, pc);
        }

        #endregion

        #region Update Functions

        public int Update(IDbConnection conn, IDbTransaction tran, ParameterCollection pcValues, ParameterCollection pcConditions)
        {
            return DataAccess.SqlUtil.Update(conn, tran, ConnInfo.TableName, pcValues, pcConditions);
        }

        public int Update(ParameterCollection pcValues, ParameterCollection pcConditions)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                return DataAccess.SqlUtil.Update(conn, ConnInfo.TableName, pcValues, pcConditions);
            }
        }

        public bool UpdateColumn(object key, object keyValue, object columnName, object columnValue)
        {
            return UpdateColumn(key, keyValue, columnName, columnValue, null);
        }

        public bool UpdateColumn(object key, object keyValue, object columnName, object columnValue, ParameterCollection pcAdditionValues)
        {
            DataAccess.ParameterCollection pcCondition = new ParameterCollection();
            pcCondition.Add(new Parameter(ParameterType.Initial, TokenTypes.Equal, key, keyValue));
            DataAccess.ParameterCollection pcValues = new ParameterCollection();
            pcValues.Add(new Parameter(ParameterType.Initial, TokenTypes.Unknown, columnName, columnValue));
            if (pcAdditionValues != null)
            {
                foreach (Parameter p in pcAdditionValues)
                {
                    pcValues.Add(new Parameter(ParameterType.Initial, TokenTypes.Unknown, p.Column, p.Value));
                }
            }
            return Update(pcValues, pcCondition) > 0;
        }

        public int Update(IDbConnection conn, ParameterCollection pcValues, ParameterCollection pcConditions)
        {
            return DataAccess.SqlUtil.Update(conn, ConnInfo.TableName, pcValues, pcConditions);
        }

        public bool UpdateColumn(IDbConnection conn, object key, object keyValue, object columnName, object columnValue)
        {
            return UpdateColumn(conn, key, keyValue, columnName, columnValue, null);
        }

        public bool UpdateColumn(IDbConnection conn, object key, object keyValue, object columnName, object columnValue, ParameterCollection pcAdditionValues)
        {
            DataAccess.ParameterCollection pcCondition = new ParameterCollection();
            pcCondition.Add(new Parameter(ParameterType.Initial, TokenTypes.Equal, key, keyValue));
            DataAccess.ParameterCollection pcValues = new ParameterCollection();
            pcValues.Add(new Parameter(ParameterType.Initial, TokenTypes.Unknown, columnName, columnValue));
            if (pcAdditionValues != null)
            {
                foreach (Parameter p in pcAdditionValues)
                {
                    pcValues.Add(new Parameter(ParameterType.Initial, TokenTypes.Unknown, p.Column, p.Value));
                }
            }
            return Update(conn, pcValues, pcCondition) > 0;
        }


        #endregion

        #region Insert Functions
        public int Insert(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                return Insert(pc);
            }
        }

        public int Insert(System.Data.IDbConnection conn, ParameterCollection pc)
        {
            return DataAccess.SqlUtil.ExecuteInsert(conn, ConnInfo.TableName, pc);
        }

        public int Insert(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc)
        {
            return DataAccess.SqlUtil.ExecuteInsert(cnn, tran, ConnInfo.TableName, pc);
        }
        #endregion

    }
}
