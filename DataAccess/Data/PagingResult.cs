using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess.Data
{
    public class PagingResult<T, C> : Interfaces.IPagingResult<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        private C _Result;

        public C Result
        {
            get { return _Result; }
            set { _Result = value; }
        }
        private int _Total;

        public int Total
        {
            get { return _Total; }
            set { _Total = value; }
        }
        public PagingResult()
        {

        }

        public static Data.PagingResult<T, C> GetPagingList(IDbConnection conn, string tableName, string[] primaryKeys, int pageIndex, int pageSize, string[] fieldsOrder, bool isAsc, string where, params string[] fieldsShow)
        {
            PagingResult<T, C> ret = new PagingResult<T, C>();
            using (IDataReader idr =
                SqlUtil.ExecuteProcedureReader(conn, "sp_Paging",
                    new SqlParameter[]{new SqlParameter("@TableName", tableName),
                    new SqlParameter("@PrimaryKeys", SqlScriptHandler.ArrayToString(primaryKeys,",",true)),
                    new SqlParameter("@PageIndex", pageIndex),
                    new SqlParameter("@PageSize", pageSize),
                    new SqlParameter("@FieldsShow", (fieldsShow==null)?"*":SqlScriptHandler.ArrayToString(fieldsShow,",",true)),
                    new SqlParameter("@FieldsOrder", fieldsOrder==null?string.Empty:string.Concat(SqlScriptHandler.ArrayToString(fieldsOrder,",",true),(!isAsc?" desc":""))),
                    new SqlParameter("@Where", where)}))
            {
                ret.Result = DataMapping.ObjectHelper.FillCollection<T, C>(idr);
                if (idr.NextResult())
                {
                    if (idr.Read())
                    {
                        ret.Total = (int)idr[0];
                    }
                }
            }
            if (ret.Total == 0)
            {
                ret.Total = ret.Result.Count;
            }
            return ret;
        }
    }
}
