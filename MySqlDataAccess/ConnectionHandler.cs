using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MySqlDataAccess
{
    public class ConnectionHandler
    {
        public static MySqlConnection CreateConnection(string connectionString)
        {
            MySqlConnection cnn = new MySqlConnection(connectionString);
            cnn.Open();
            return cnn;
        }

        public static MySqlConnection CreateConnectionByKey(string key)
        {
            return CreateConnection(ConfigurationManager.ConnectionStrings[key].ConnectionString);
        }
    }
}
