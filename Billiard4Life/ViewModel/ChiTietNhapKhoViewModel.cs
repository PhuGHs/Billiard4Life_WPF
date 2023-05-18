using Billiard4Life.Models;
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

namespace Billiard4Life.ViewModel
{
    public class ChiTietNhapKhoViewModel : BaseViewModel
    {
        private ObservableCollection<NhapKho> _ListIn;
        public ObservableCollection<NhapKho> ListIn { get => _ListIn; set { _ListIn = value; OnPropertyChanged(); } }
        private NhapKho _Selected;
        public NhapKho Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
                OnPropertyChanged();
                if (Selected != null)
                {
                    ID = Selected.MaNhap;
                    Name = Selected.TenSP;
                    Count = Selected.SoLuong;
                    Group = Selected.Nhom;
                    Unit = Selected.DonVi;
                    DateIn = Selected.NgayNhap;
                    Value = Selected.DonGia;
                    Suplier = Selected.NguonNhap;
                    SuplierInfo = Selected.LienLac;

                    CountBeforeEdit = Count;
                }
                OnPropertyChanged();
            }
        }
        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;

        #region // Right Card
        private string CountBeforeEdit;
        private string _ID;
        public string ID { get => _ID; set { _ID = value; OnPropertyChanged(); } }
        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        private string _Count;
        public string Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }
        private string _Unit;
        public string Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }
        private string _Group;
        public string Group { get => _Group; set { _Group = value; OnPropertyChanged(); } }
        private string _Value;
        public string Value { get => _Value; set { _Value = value; OnPropertyChanged(); } }
        private string _DateIn;
        public string DateIn { get => _DateIn; set { _DateIn = value; OnPropertyChanged("DateIn"); } }
        private string _Suplier;
        public string Suplier { get => _Suplier; set { _Suplier = value; OnPropertyChanged(); } }
        private string _SuplierInfo;
        public string SuplierInfo { get => _SuplierInfo; set { _SuplierInfo = value; OnPropertyChanged(); } }
        #endregion

        private string _ItemName;
        public string ItemName
        {
            get => _ItemName;
            set
            {
                _ItemName = value;
                OnPropertyChanged();
            }
        }
        public ICommand CloseCM { get; set; }
        public ICommand EditCM { get; set; }
        public ChiTietNhapKhoViewModel(string itemName)
        {
            ListIn = new ObservableCollection<NhapKho>();
            GetListIn(itemName);
            ItemName = itemName;

            CloseCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;
                p.Close();
            });

            #region // edit command
            EditCM = new RelayCommand<object>((p) =>
            {
                foreach (NhapKho item in ListIn)
                {
                    if (ID == item.MaNhap && Name == item.TenSP && Count == item.SoLuong && DateIn == item.NgayNhap && Value == item.DonGia && Unit == item.DonVi && Suplier == item.NguonNhap && SuplierInfo == item.LienLac)
                        return false;
                }
                if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Count) || string.IsNullOrEmpty(DateIn.ToString()) || string.IsNullOrEmpty(Unit) || string.IsNullOrEmpty(Value))
                    return false;
                if (Count == "0") return false;
                if (!isMoney(Value)) return false;
                if (SuplierInfo != null && !isNumber(SuplierInfo)) return false;
                foreach (NhapKho item in ListIn)
                {
                    if (ID == item.MaNhap) return true;
                }
                return false;
            }, (p) =>
            {
                OpenConnect();


                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE CHITIETNHAP SET NhomSanPham = N'" + Group + "', DonVi = N'" + Unit + "', DonGia = " + Value + ", SoLuong = " + Count + ", NgayNhap = '" + DateIn + "', NguonNhap = N'" + Suplier + "', LienLac = '" + SuplierInfo + "' WHERE MaNhap = '" + ID + "'";
                cmd.Connection = sqlCon;

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MyMessageBox mess = new MyMessageBox("Sửa thành công!");
                    mess.ShowDialog();
                }
                else
                {
                    MyMessageBox mess = new MyMessageBox("Sửa không thành công!");
                    mess.ShowDialog();
                }
                int c = int.Parse(Count) - int.Parse(CountBeforeEdit);
                cmd.CommandText = "UPDATE KHO SET TonDu = TonDu + " + c.ToString() + " WHERE TenSanPham = N'" + Name + "'";
                cmd.ExecuteNonQuery();

                GetListIn(itemName);

                CloseConnect();
            });
            #endregion
        }
        public void GetListIn(string itemName)
        {
            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM CHITIETNHAP WHERE TenSanPham = N'" + itemName + "' ORDER BY NgayNhap DESC";
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();
            ListIn.Clear();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                string donvi = reader.GetString(2);
                string dongia = reader.GetSqlMoney(3).ToString();
                string nhom = reader.GetString(4);
                string soluong = reader.GetDouble(5).ToString();
                string date = reader.GetDateTime(6).ToShortDateString();
                string nguon = reader.GetString(7);
                string lienlac = reader.GetString(8);
                ListIn.Add(new NhapKho(ma, ten, donvi, nhom, dongia, soluong, date, nguon, lienlac));
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
