using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DataAccess;

namespace DataAccess.Data.Interfaces
{
    public interface IUOBase<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        int Insert();
        int Insert(IDbConnection cnn, IDbTransaction tran);

        int Update(ParameterCollection pc);
        int Update(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc);
        int UpdateAllColumns(ParameterCollection pc);
        int UpdateAllColumns(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc);

        object this[string name, params string[] formatDateTime] { get;  set; }
    }
}
