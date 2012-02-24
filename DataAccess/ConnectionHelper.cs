using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace DataAccess
{
    public class ConnectionHelper
    {
        public ConnectionHelper()
        {

        }
        public static IDbConnection CreateConnection(string connectionString)
        {
            IDbConnection cnn = new System.Data.SqlClient.SqlConnection(connectionString);
            cnn.Open();
            return cnn;
        }

        public static IDbConnection CreateConnectionByKey(string key)
        {
            return CreateConnection(ConfigurationManager.ConnectionStrings[key].ConnectionString);
        }

        public static void DisposeConnection(IDbConnection conn)
        {
            conn.Close();
            conn.Dispose();
        }
    }
}
