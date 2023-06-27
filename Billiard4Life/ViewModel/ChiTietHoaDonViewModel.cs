using Billiard4Life.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Billiard4Life.ViewModel
{
    public class ChiTietHoaDonViewModel : BaseViewModel
    {
        private ObservableCollection<ChiTietHoaDon> _CTHD;
        public ObservableCollection<ChiTietHoaDon> CTHD { get => _CTHD; set { _CTHD = value; OnPropertyChanged(); } }
        private string _ItemName;
        public string ItemName { get { return _ItemName; } set { _ItemName = value; OnPropertyChanged(); } }
        public ICommand CloseCM { get; set; }
        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;
        public ChiTietHoaDonViewModel(string SoHD)
        {
            ItemName = SoHD;
            CTHD = new ObservableCollection<ChiTietHoaDon>();
            ListViewDisplay();

            CloseCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;
                p.Close();
            });
        }
        public void ListViewDisplay()
        {
            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT ct.*, m.TenMon, m.Gia FROM CTHD AS ct JOIN MENU AS m ON ct.MaMon = m.MaMon WHERE SoHD = " + ItemName;
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();

            CTHD.Clear();
            while (reader.Read())
            {
                string ten = reader.GetString(3);
                string soluong = reader.GetInt16(2).ToString();
                string gia = reader.GetSqlMoney(4).ToString();
                float tien = float.Parse(soluong) * float.Parse(gia);

                CTHD.Add(new ChiTietHoaDon(ten, soluong, gia, tien.ToString()));
            }

            CloseConnect();
        }
        private void OpenConnect()
        {
            sqlCon = new SqlConnection(strCon);
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
        }
        private void CloseConnect()
        {
            if (sqlCon.State == ConnectionState.Open)
            {
                sqlCon.Close();
            }
        }
    }
}
