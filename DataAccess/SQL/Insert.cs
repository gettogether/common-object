using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess.SQL
{
    public class Insert
    {
        public static void BulkInsert<T, C>(C c)
            where T : DataAccess.Data.UOBase<T, C>, new()
            where C : System.Collections.Generic.ICollection<T>, new()
        {
            BulkInsert<T, C>(c, SqlBulkCopyOptions.Default, null);
        }

        public static void BulkInsert<T, C>(C c, SqlBulkCopyOptions copyOptions, SqlTransaction tran)
            where T : DataAccess.Data.UOBase<T, C>, new()
            where C : System.Collections.Generic.ICollection<T>, new()
        {
            if (c == null || c.Count == 0) return;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            DataTable tb = null;
            string tableName = string.Empty;
            System.Data.IDbConnection conn = null;
            bool isFirst = true;
            foreach (T t in c)
            {
                if (isFirst)
                {
                    isFirst = false;
                    tb = t.GetTableSchema().Clone();
                    tableName = t.ConnInfo.TableName;
                    conn = t.ConnInfo.Connection;
                    DataAccess.Log.Insert(string.Concat("Batch Insert,Table Name:", tableName, ",Parepare Data Source"));
                }
                DataRow dataRow = tb.NewRow();
                foreach (DataColumn col in tb.Columns)
                {
                    dataRow[col.ColumnName] = t[col.ColumnName];
                }
                tb.Rows.Add(dataRow);
            }
            DataAccess.Log.Insert(string.Concat("Batch Insert,Table Name:", tableName, ",Parepare Data Source,Elapsed Milliseconds:", sw.ElapsedMilliseconds));
            sw.Stop();
            DataAccess.Log.Insert(string.Concat("Batch Insert,Table Name:", tableName, ",Collection Count:", c.Count));
            using (conn)
            {
                BulkInsert((System.Data.SqlClient.SqlConnection)conn, tb, tableName, copyOptions, tran);
            }
        }

        public static void BulkInsert(System.Data.SqlClient.SqlConnection conn, DataTable tb, string tableName, System.Data.SqlClient.SqlBulkCopyOptions copyOptions, System.Data.SqlClient.SqlTransaction tran)
        {
            if (tb != null && tb.Rows.Count != 0)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                System.Data.SqlClient.SqlBulkCopy sbc = null;
                if (copyOptions != System.Data.SqlClient.SqlBulkCopyOptions.Default || tran != null)
                {
                    if (tran != null)
                    {
                        sbc = new System.Data.SqlClient.SqlBulkCopy(conn, copyOptions, tran);
                    }
                    else
                    {
                        sbc = new System.Data.SqlClient.SqlBulkCopy(conn.ConnectionString, copyOptions);
                    }
                }
                else
                {
                    sbc = new System.Data.SqlClient.SqlBulkCopy(conn);
                }
                if (Config.BulkCopyTimeout > 0)
                    sbc.BulkCopyTimeout = Config.BulkCopyTimeout;
                sbc.DestinationTableName = tableName;
                DataAccess.Log.Insert(string.Concat("Batch Insert,Table Name:", tableName, ",Table Count:", tb.Rows.Count));
                sbc.WriteToServer(tb);
                sbc.Close();
                DataAccess.Log.Insert(string.Concat("Batch Insert,Table Name:", tableName, ",Elapsed Milliseconds:", sw.ElapsedMilliseconds));
                sw.Stop();
            }
        }

        public static void BulkInsert(System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlDataReader dr, string tableName, System.Data.SqlClient.SqlBulkCopyOptions copyOptions, System.Data.SqlClient.SqlTransaction tran)
        {
            if (dr != null)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                System.Data.SqlClient.SqlBulkCopy sbc = null;
                if (copyOptions != System.Data.SqlClient.SqlBulkCopyOptions.Default || tran != null)
                {
                    if (tran != null)
                    {
                        sbc = new System.Data.SqlClient.SqlBulkCopy(conn, copyOptions, tran);
                    }
                    else
                    {
                        sbc = new System.Data.SqlClient.SqlBulkCopy(conn.ConnectionString, copyOptions);
                    }
                }
                else
                {
                    sbc = new System.Data.SqlClient.SqlBulkCopy(conn);
                }
                if (Config.BulkCopyTimeout > 0)
                    sbc.BulkCopyTimeout = Config.BulkCopyTimeout;
                sw.Start();
                sbc.DestinationTableName = tableName;
                DataAccess.Log.Insert(string.Concat("Batch Insert,Table Name:", tableName, ",Data Reader"));
                sbc.WriteToServer(dr);
                sbc.Close();
                DataAccess.Log.Insert(string.Concat("Batch Insert,Table Name:", tableName, ",Data Reader,Elapsed Milliseconds:", sw.ElapsedMilliseconds));
                sw.Stop();
            }
        }
    }
}
