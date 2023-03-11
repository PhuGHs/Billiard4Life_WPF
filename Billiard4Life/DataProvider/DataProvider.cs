using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Billiard4Life.DataProvider
{
    public class DataProvider : DBConnection
    {
        public DataTable LoadInitialData(string cmdText)
        {
            DataTable dt = new DataTable();
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand(cmdText, SqlCon);  
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            finally
            {
                DBClose();
            }
            return dt;
        }
    }
}
