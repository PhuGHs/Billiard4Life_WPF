using Project;
using Billiard4Life.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Billiard4Life;
using System.Data.SqlTypes;
using Billiard4Life.DataProvider;

namespace RestaurantManagement.ViewModel
{
    public class LoginWindowVM : BaseViewModel
    {
        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;
        public bool IsLoggedIn { get; set; }
        private string _UserName;
        private static string _MaNV;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public static string MaNV { get => _MaNV; set { _MaNV = value; } }
        public string Role { get; set; }
        private bool Online;
        public ICommand CloseLoginCM { get; set; }
        public ICommand LoginCM { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public LoginWindowVM()
        {
            IsLoggedIn = false;
            CloseLoginCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;
                p.Close();
            });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            LoginCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Login(p);
            });
            
        }

        public void Login(Window p)
        {
            OpenConnect();

            if (p == null) return;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM TAIKHOAN WHERE ID = '" + UserName + "' AND MatKhau = '" + Password + "'";
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                IsLoggedIn = true;
                Role = reader.GetString(2);
                MaNV = reader.GetString(3);
                Online = reader.GetBoolean(7);
            }
            reader.Close();

            CloseConnect();

            if (MaNV == null)
            {
                MyMessageBox x = new MyMessageBox("Không tồn tại tài khoản");
                x.Show();
                return;
            }
            if (NhanVienDP.Flag.IsAnyStaffOnline() && Online == false && NhanVienDP.Flag.IsStaff(MaNV))
            {
                MyMessageBox msb = new MyMessageBox("Đã có nhân viên trực ca.\nVui lòng đăng nhập sau!");
                msb.ShowDialog();
            }
            else
            if (IsLoggedIn)
            {
                p.Close();
                return;
            }
            else
            {
                MyMessageBox msb = new MyMessageBox("Sai tên đăng nhập hoặc mật khẩu!");
                msb.ShowDialog();
            }
        }

        void OpenConnect()
        {
            sqlCon = new SqlConnection(strCon);
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
        }

        void CloseConnect()
        {
            if (sqlCon.State == ConnectionState.Open)
            {
                sqlCon.Close();
            }
        }
    }
}
