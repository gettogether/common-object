using System;
using System.Collections.Generic;
using System.Text;
using DataAccess;
using System.Data;

namespace DataAccess.Data.Interfaces
{
    public interface IDOBase<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        int Insert(ParameterCollection pc);
        int Insert(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc);
        int Delete(ParameterCollection pc);
        int Delete(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc);
        int Update(ParameterCollection pcValues, ParameterCollection pcConditions);
        int Update(IDbConnection cnn, IDbTransaction tran, ParameterCollection pcValues, ParameterCollection pcConditions);
        bool UpdateColumn(object key, object keyValue, object columnName, object columnValue);
        bool UpdateColumn(object key, object keyValue, object columnName, object columnValue, ParameterCollection pcAdditionValues);
    }
}
