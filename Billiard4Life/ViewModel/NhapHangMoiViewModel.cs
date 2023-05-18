using Billiard4Life.Models;
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
    public class NhapHangMoiViewModel : BaseViewModel
    {
        #region // Right Card
        private string IDBeforeEdit;
        private string _ID;
        public string ID { get => _ID; set { _ID = value; OnPropertyChanged(); } }
        private string NameBeforeEdit;
        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        private string _Count;
        public string Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }
        private string _Unit;
        public string Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }
        private string _Group;
        public string Group { get => _Group; set { _Group = value; OnPropertyChanged(); } }
        private string _Info;
        public string Info { get => _Info; set { _Info = value; OnPropertyChanged(); } }
        private string _Value;
        public string Value { get => _Value; set { _Value = value; OnPropertyChanged(); } }
        private string _DateIn;
        public string DateIn { get => _DateIn; set { _DateIn = value; OnPropertyChanged("DateIn"); } }
        private string _Suplier;
        public string Suplier { get => _Suplier; set { _Suplier = value; OnPropertyChanged(); } }
        private string _SuplierInfo;
        public string SuplierInfo { get => _SuplierInfo; set { _SuplierInfo = value; OnPropertyChanged(); } }
        #endregion

        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;

        private string _NameEnable;
        private string _InfoEnable;
        private string _GroupEnable;
        public string NameEnable
        {
            get => _NameEnable;
            set { _NameEnable = value; OnPropertyChanged(); }
        }
        public string InfoEnable
        {
            get => _InfoEnable;
            set { _InfoEnable = value; OnPropertyChanged(); }
        }
        public string GroupEnable
        {
            get => _GroupEnable; set
            {
                _GroupEnable = value;
                OnPropertyChanged();
            }
        }
        public ICommand CloseCM { get; set; }
        public ICommand AddCM { get; set; }
        public NhapHangMoiViewModel()
        {
            OpenConnect();

            NameEnable = "True";
            InfoEnable = "True";
            GroupEnable = "True";
            DateIn = DateTime.Now.ToShortDateString();

            CloseCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;
                p.Close();
            });
            ID = GetHighestID();
            AddCM = new RelayCommand<object>((p) => 
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Count) || string.IsNullOrEmpty(DateIn.ToString()) || string.IsNullOrEmpty(Unit) || string.IsNullOrEmpty(Value))
                    return false;
                if (Count == "0") return false;
                if (!isMoney(Value)) return false;
                if (SuplierInfo != null && !isNumber(SuplierInfo)) return false;
                return true; 
            }, (p) =>
            {
                OpenConnect();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM KHO WHERE TenSanPham = N'" + Name + "'";
                cmd.Connection = sqlCon;
                SqlDataReader reader = cmd.ExecuteReader();
                bool added = false;

                while (reader.Read())
                {
                    added = true;
                }
                reader.Close();
                
                if (!added)
                {
                    cmd.CommandText = "INSERT INTO KHO VALUES(N'" + Name + "', 0," + Info + ", N'" + Unit + "', N'" + Group + "', 0)";
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }

                
                cmd.CommandText = "INSERT INTO CHITIETNHAP VALUES ('" + ID + "',N'" + Name + "',N'" + Unit + "'," + Value + ", N'" + Group + "'," + Count + ",'" + DateIn + "',N'" + Suplier + "','" + SuplierInfo + "')";
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    cmd.CommandText = "UPDATE KHO SET TonDu = TonDu + " + Count + " WHERE TenSanPham = N'" + Name + "'";
                    cmd.ExecuteNonQuery();

                    MyMessageBox mess = new MyMessageBox("Nhập thành công!");
                    mess.ShowDialog();
                    ID = GetHighestID();
                }
                else
                {
                    MyMessageBox mess = new MyMessageBox("Nhập không thành công!");
                    mess.ShowDialog();
                }

                CloseConnect();
            });

            CloseConnect();
        }
        public NhapHangMoiViewModel(Kho kho)
        {
            ID = GetHighestID();
            NameEnable = "False";
            InfoEnable = "False";
            GroupEnable = "False";
            DateIn = DateTime.Now.ToShortDateString();
            Name = kho.TenSanPham;
            Group = kho.NhomSP;
            Unit = kho.DonVi;

            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT MucBaoNhap FROM KHO WHERE TenSanPham = N'" + Name + "'" ;
            cmd.Connection = sqlCon;

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Info = reader.GetDouble(0).ToString();
            }
            reader.Close();
            

            CloseCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;
                p.Close();
            });
            AddCM = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Count) || string.IsNullOrEmpty(DateIn.ToString()) || string.IsNullOrEmpty(Unit) || string.IsNullOrEmpty(Value))
                    return false;
                if (Count == "0") return false;
                if (!isMoney(Value)) return false;
                if (SuplierInfo != null && !isNumber(SuplierInfo)) return false;
                return true;
            }, (p) =>
            {
                OpenConnect();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO CHITIETNHAP VALUES ('" + ID + "',N'" + Name + "',N'" + Unit + "'," + Value + ", N'" + Group + "'," + Count + ",'" + DateIn + "',N'" + Suplier + "','" + SuplierInfo + "')";
                cmd.Connection = sqlCon;
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    cmd.CommandText = "UPDATE KHO SET TonDu = TonDu + " + Count + " WHERE TenSanPham = N'" + Name + "'";
                    cmd.ExecuteNonQuery();

                    MyMessageBox mess = new MyMessageBox("Nhập thành công!");
                    mess.ShowDialog();
                    ID = GetHighestID();
                }
                else
                {
                    MyMessageBox mess = new MyMessageBox("Nhập không thành công!");
                    mess.ShowDialog();
                }

                CloseConnect();
            });

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
        private string GetHighestID()
        {
            OpenConnect();

            string ID = "10001";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT TOP (1) MaNhap FROM CHITIETNHAP ORDER BY MaNhap DESC";
            cmd.Connection = sqlCon;

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int num = int.Parse(reader.GetString(0)) + 1;
                ID = num.ToString();
            }
            reader.Close();

            CloseConnect();
            
            return ID;
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
        private bool isMoney(string s)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if ((s[i] < 48 || s[i] > 57) && s[i] != '.')
                    return false;
                if (s[i] == '.') count++;
            }
            if (s[0] == '.') return false;
            if (s[s.Length - 1] == '.') return false;
            if (s[0] == '0') return false;
            if (count > 1) return false;
            return true;
        }
    }
}
