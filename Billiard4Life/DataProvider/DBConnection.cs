using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.DataProvider
{
    public class DBConnection
    {
        private string _connectionString;
        public SqlConnection SqlCon { get; set; } = null;

        public DBConnection()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Billiard4Life"].ToString();
            if(SqlCon == null)
            {
                SqlCon = new SqlConnection(_connectionString);
            }
        }

        public void DBOpen()
        {
            if (SqlCon.State == ConnectionState.Closed)
            {
                SqlCon.Open();
            }
        }
        public void DBClose()
        {
            if (SqlCon.State == ConnectionState.Open)
            {
                SqlCon.Close();
            }
        }
    }
}
