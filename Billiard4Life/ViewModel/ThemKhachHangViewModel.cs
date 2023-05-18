using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Billiard4Life.ViewModel
{
    public class ThemKhachHangViewModel : BaseViewModel
    {
        private string _ID;
        public string ID { get { return _ID; } set { _ID = value; OnPropertyChanged(); } }
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; OnPropertyChanged(); } }
        private string _Phone;
        public string Phone { get { return _Phone; } set { _Phone = value; OnPropertyChanged(); } }
        private string _Email;
        public string Email { get { return _Email; } set { _Email = value; OnPropertyChanged(); } }
        public ICommand AddCM { get; set; }
        public ICommand CloseCM { get; set; }
        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;

        public ThemKhachHangViewModel()
        {
            ID = GetHighestID();

            CloseCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;
                p.Close();
            });
            AddCM = new RelayCommand<object>((p) => 
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Phone)) return false;
                return true; 
            }, (p) =>
            {
                OpenConnect();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO KHACHHANG VALUES('" + ID + "', N'" + Name + "', '" + Phone + "', '" + Email + "', 0)";
                cmd.Connection = sqlCon;
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MyMessageBox msb = new MyMessageBox("Thêm thành công!");
                    msb.ShowDialog();
                }
                else
                {
                    MyMessageBox msb = new MyMessageBox("Thêm không thành công!");
                    msb.ShowDialog();
                }    
                
                ID = GetHighestID();
                Name = "";
                Phone = "";
                Email = "";
                CloseConnect();
            });
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
        private string GetHighestID()
        {
            string ID = "KH0001", temp = "";

            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT TOP (1) MaKH FROM KHACHHANG ORDER BY MaKH DESC";
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
                while (temp.Length < 4) temp = "0" + temp;
                ID = "KH" + temp;
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
