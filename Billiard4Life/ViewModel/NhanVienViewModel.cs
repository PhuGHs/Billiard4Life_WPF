using Billiard4Life.Models;
using RestaurantManagement.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System.Windows.Forms;

namespace Billiard4Life.ViewModel
{
    public class NhanVienViewModel : BaseViewModel
    {
        private ObservableCollection<NhanVien> _ListStaff;
        public ObservableCollection<NhanVien> ListStaff { get => _ListStaff; set { _ListStaff = value; OnPropertyChanged(); } }

        #region // List View Selected Item
        private NhanVien _Selected;
        public NhanVien Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
                OnPropertyChanged();
                if (Selected != null)
                {
                    ID = Selected.MaNV;
                    Name = Selected.HoTen;
                    Position = Selected.ChucVu;
                    if (Selected.Fulltime) Fulltime = "Full-time";
                    else Fulltime = "Part-time";
                    Address = Selected.DiaChi;
                    Phone = Selected.SDT;
                    DateBorn = Selected.NgaySinh;
                    DateStartWork = Selected.NgayVaoLam;
                    Account = Selected.TaiKhoan;
                    Password = Selected.MatKhau;
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region // right card
        private string _ID;
        public string ID { get => _ID; set { _ID = value; OnPropertyChanged(); } }
        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        private string _Position;
        public string Position { get => _Position; set { _Position = value; OnPropertyChanged(); } }
        private string _Fulltime;
        public string Fulltime { get => _Fulltime; set { _Fulltime = value; OnPropertyChanged(); } }
        private string _Address;
        public string Address { get => _Address; set { _Address = value; OnPropertyChanged(); } }
        private string _Phone;
        public string Phone { get => _Phone; set { _Phone = value; OnPropertyChanged(); } }
        private string _DateBorn;
        public string DateBorn { get => _DateBorn; set { _DateBorn = value; OnPropertyChanged(); } }
        private string _DateStartWork;
        public string DateStartWork { get => _DateStartWork; set { _DateStartWork = value; OnPropertyChanged(); } }
        private string _Account;
        public string Account { get => _Account; set { _Account = value; OnPropertyChanged(); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
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
                    strQuery = "SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV WHERE TenNV LIKE N'%" + Search + "%'";
                }
                else
                    strQuery = "SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV";
                ListViewDisplay(strQuery);
            }
        }
        #endregion

        public ICommand AddCM { get; set; }
        public ICommand EditCM { get; set; }
        public ICommand DeleteCM { get; set; }
        public ICommand CheckCM { get; set; }

        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;
        public NhanVienViewModel()
        {
            OpenConnect();

            DateBorn = DateTime.Now.ToShortDateString();
            DateStartWork = DateTime.Now.ToShortDateString();

            ListStaff = new ObservableCollection<NhanVien>();
            ListViewDisplay("SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV");

            CheckCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                RestaurantManagement.View.ChamCong chamCong = new RestaurantManagement.View.ChamCong();
                chamCong.Show();
                return;
            });


            AddCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Billiard4Life.View.ThemNhanVien adding = new View.ThemNhanVien();
                if (adding.ShowDialog() == true) { }
                ListViewDisplay("SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV");
                return;
            });

            #region //edit command
            EditCM = new RelayCommand<object>((p) =>
            {
                foreach (NhanVien item in ListStaff)
                {
                    if (ID == item.MaNV && Name == item.HoTen && Position == item.ChucVu && Address == item.DiaChi && Phone == item.SDT && Account == item.TaiKhoan && Password == item.MatKhau && DateBorn == item.NgaySinh && DateStartWork == item.NgayVaoLam)
                    {
                        if ((Fulltime == "Full-time" && item.Fulltime) || (Fulltime == "Part-time" && !item.Fulltime))
                            return false;
                    }
                }
                if (String.IsNullOrEmpty(ID) || String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Position) || String.IsNullOrEmpty(Fulltime) || String.IsNullOrEmpty(DateStartWork))
                    return false;
                if (!isNumber(Phone)) return false;
                if ((!String.IsNullOrEmpty(Account) && String.IsNullOrEmpty(Password)) || (String.IsNullOrEmpty(Account) && !String.IsNullOrEmpty(Password))) return false;
                return true;

            }, (p) =>
            {
                OpenConnect();

                int ft;
                if (Fulltime == "Full-time") ft = 1;
                else ft = 0;

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE NHANVIEN SET TenNV = N'" + Name + "', ChucVu = N'" + Position + "', DiaChi = N'" + Address + "', Fulltime = " + ft + ", SDT = '" + Phone + "', NgayVaoLam = '" + DateStartWork + "', NgaySinh = '" + DateBorn + "' WHERE MaNV = '" + ID + "'";
                cmd.Connection = sqlCon;

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MyMessageBox mess = new MyMessageBox("Sửa thành công!");
                    mess.ShowDialog();
                    Refresh();
                }
                else
                {
                    MyMessageBox mess = new MyMessageBox("Sửa không thành công!");
                    mess.ShowDialog();
                }
                ListViewDisplay("SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV");

                CloseConnect();
            });
            #endregion

            #region //delete command
            DeleteCM = new RelayCommand<object>((p) =>
            {
                if (Selected == null) return false;
                if (Selected.ChucVu == "Quản lý") return false;
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
                    if (!string.IsNullOrEmpty(Account))
                    {
                        cmd.CommandText = "DELETE FROM TAIKHOAN WHERE ID = '" + Account + "'";
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommandText = "DELETE FROM NHANVIEN WHERE MaNV = '" + ID + "'";
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MyMessageBox mess = new MyMessageBox("Xóa thành công!");
                        mess.ShowDialog();
                        Refresh();
                    }
                    else
                    {
                        MyMessageBox mess = new MyMessageBox("Xóa không thành công!");
                        mess.ShowDialog();
                    }
                }
                ListViewDisplay("SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV");

                CloseConnect();
            });
            #endregion

            CloseConnect();
        }
        public void ListViewDisplay(string strQuery)
        {
            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strQuery;
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();

            ListStaff.Clear();
            while (reader.Read())
            {
                string id = reader.GetString(0);
                string ten = reader.GetString(1);
                string chucvu = reader.GetString(2);
                bool ftime = reader.GetBoolean(3);
                string diachi = reader.GetString(4);
                string sdt = reader.GetString(5);
                string ngsinh = reader.GetDateTime(6).ToShortDateString();
                string ngvl = reader.GetDateTime(7).ToShortDateString();
                string tk = "";
                if (!reader.IsDBNull(8))
                    tk = reader.GetString(8);
                string mk = "";
                if (!reader.IsDBNull(9))
                    mk = reader.GetString(9);

                ListStaff.Add(new NhanVien(id, ten, chucvu, diachi, ftime, sdt, ngvl, ngsinh, tk, mk));
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
        private void Refresh()
        {
            ID = "";
            Name = "";
            Position = "";
            Fulltime = "";
            Address = "";
            Phone = "";
            DateBorn = "";
            DateStartWork = "";
            Account = "";
            Password = "";
        }
        private bool isNumber(string s)
        {
            if (s == null) return false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < 48 || s[i] > 57) return false;
            }
            return true;
        }
    }
}
