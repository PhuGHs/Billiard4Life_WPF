using System;
using Billiard4Life.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Billiard4Life.ViewModel
{
    public class ThemNhanVienViewModel : BaseViewModel
    {
        private ObservableCollection<NhanVien> _ListStaff;
        public ObservableCollection<NhanVien> ListStaff { get => _ListStaff; set { _ListStaff = value; OnPropertyChanged(); } }

        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;

        #region // card
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

        public ICommand AddCM { get; set; }
        public ICommand CloseCM { get; set; }
        public ThemNhanVienViewModel()
        {
            ListStaff = new ObservableCollection<NhanVien>();
            GetListStaff();

            ID = GetHighestID();
            DateBorn = DateTime.Now.ToShortDateString();
            DateStartWork = DateTime.Now.ToShortDateString();

            CloseCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;
                p.Close();
            });
            #region //add command
            AddCM = new RelayCommand<object>((p) =>
            {
                foreach (NhanVien item in ListStaff)
                {
                    if (item.MaNV == ID) return false;
                }
                if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Position) || string.IsNullOrEmpty(Fulltime) || string.IsNullOrEmpty(DateStartWork))
                    return false;
                if (!isNumber(Phone)) return false;
                foreach (NhanVien nv in ListStaff)
                {
                    if (nv.TaiKhoan == Account && !string.IsNullOrEmpty(Account)) return false;
                }
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
                cmd.CommandText = "INSERT INTO NHANVIEN VALUES ('" + ID + "',N'" + Name + "',N'" + Position + "'," + ft + ",N'" + Address + "','" + Phone + "','" + DateBorn + "','" + DateStartWork + "')";
                cmd.Connection = sqlCon;

                int result = cmd.ExecuteNonQuery();

                if (!String.IsNullOrEmpty(Account))
                {
                    cmd.CommandText = "INSERT INTO TAIKHOAN(ID, MatKhau, Quyen, MaNV) VALUES('" + Account + "', '" + Password + "', 'nhan vien', '" + ID + "')";
                    cmd.ExecuteNonQuery();
                }
                if (result > 0)
                {
                    MyMessageBox mess = new MyMessageBox("Thêm thành công!");
                    mess.ShowDialog();
                }
                else
                {
                    MyMessageBox mess = new MyMessageBox("Thêm không thành công!");
                    mess.ShowDialog();
                }
                Refresh();
                ID = GetHighestID();

                CloseConnect();
            });
            #endregion
        }
        public void GetListStaff()
        {
            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV";
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
            ID = GetHighestID();
            Name = "";
            Position = "";
            Fulltime = "";
            Address = "";
            Phone = "";
            DateBorn = DateTime.Now.ToShortDateString();
            DateStartWork = DateTime.Now.ToShortDateString();
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
        private string GetHighestID()
        {
            string ID = "NV001", temp = "";

            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT TOP (1) MaNV FROM NHANVIEN ORDER BY MaNV DESC";
            cmd.Connection = sqlCon;

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                temp = reader.GetString(0);
            }
            reader.Close();
            
            if (!string.IsNullOrEmpty(temp))
            {
                int num = ExtractNumber(temp) + 1;
                temp = num.ToString();
                while (temp.Length < 3) temp = "0" + temp;
                ID = "NV" + temp;
            }

            CloseConnect();

            return ID;
        }
        private int ExtractNumber(string input)
        {
            string output = string.Empty;
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    output += c;
                }
            }
            return int.Parse(output);
        }
    }
}
