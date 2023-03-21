using Billiard4Life.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ListView = System.Windows.Forms.ListView;
using System.Security.Principal;
using Microsoft.Office.Interop.Excel;

namespace Billiard4Life.ViewModel
{

    public class BepViewModel : BaseViewModel
    {
        private ObservableCollection<KhachHang> _ListCustomer;
        public ObservableCollection<KhachHang> ListCustomer { get => _ListCustomer; set { _ListCustomer = value; OnPropertyChanged(); } }
        #region // List View Selected Item
        private KhachHang _Selected;
        public KhachHang Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region // Search bar
        private string _Search;
        public string Search
        {
            get => _Search;
            set
            {
                _Search = value;
                string strQuery;
                OnPropertyChanged();
                if (!String.IsNullOrEmpty(Search))
                {
                    strQuery = "SELECT * FROM KHACHHANG WHERE SDT LIKE N'%" + Search + "%'";
                }
                else
                    strQuery = "SELECT * FROM KHACHHANG";
                ListViewDisplay(strQuery);
            }
        }
        #endregion
        public ICommand AddCM { get; set; }
        public ICommand DeleteCM { get; set; }

        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;
        public BepViewModel()
        {
            ListCustomer = new ObservableCollection<KhachHang>();
            ListViewDisplay("SELECT * FROM KHACHHANG");

            AddCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Billiard4Life.View.ThemKhachHang adding = new View.ThemKhachHang();
                if (adding.ShowDialog() == true) { }
                ListViewDisplay("SELECT * FROM KHACHHANG");
                return;
            });

            #region //delete command
            DeleteCM = new RelayCommand<object>((p) =>
            {
                if (Selected == null) return false;
                return true;
            }, (p) =>
            {
                OpenConnect();

                MyMessageBox yesno = new MyMessageBox("Bạn có chắc chắn xóa?", true);
                yesno.ShowDialog();

                if (yesno.ACCEPT())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlCon;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE FROM KHACHHANG WHERE MaKH = '" + Selected.Ma + "'";

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MyMessageBox mess = new MyMessageBox("Xóa thành công!");
                        mess.ShowDialog();
                    }
                    else
                    {
                        MyMessageBox mess = new MyMessageBox("Xóa không thành công!");
                        mess.ShowDialog();
                    }
                }
                ListViewDisplay("SELECT * FROM KHACHHANG");

                CloseConnect();
            });
            #endregion
        }
        public void ListViewDisplay(string strQuery)
        {
            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strQuery;
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();

            ListCustomer.Clear();

            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                string sdt = reader.GetString(2);
                string email = reader.GetString(3);
                int diem = reader.GetInt16(4);

                ListCustomer.Add(new KhachHang(ma, ten, sdt, email, diem));
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
