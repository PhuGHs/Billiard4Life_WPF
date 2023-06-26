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
using Billiard4Life.DataProvider;

namespace Billiard4Life.ViewModel
{
    public class ThemNhanVienViewModel : BaseViewModel
    {
        private ObservableCollection<NhanVien> _ListStaff;
        public ObservableCollection<NhanVien> ListStaff { get => _ListStaff; set { _ListStaff = value; OnPropertyChanged(); } }
        private NhanVien _AddItem;
        public NhanVien AddItem
        {
            get { return _AddItem; }
            set { _AddItem = value; OnPropertyChanged();}
        }
        private string _Fulltime;
        public string Fulltime 
        {
            get => _Fulltime; 
            set 
            { 
                _Fulltime = value; OnPropertyChanged();
                if (Fulltime == "Full-time") AddItem.Fulltime = true; else AddItem.Fulltime = false;
            } 
        }

        public ICommand AddCM { get; set; }
        public ICommand CloseCM { get; set; }
        public ThemNhanVienViewModel()
        {
            ListStaff = new ObservableCollection<NhanVien>();
            AddItem = new NhanVien();

            ListStaff = NhanVienDP.Flag.GetAllStaff("SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV WHERE Xoa = 0");
            AddItem.MaNV = NhanVienDP.Flag.AutoIDStaff();
            AddItem.NgaySinh = DateTime.Now.ToShortDateString();
            AddItem.NgayVaoLam = DateTime.Now.ToShortDateString();

            CloseCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;
                p.Close();
            });
            AddCM = new RelayCommand<object>((p) =>
            {
                return IfCanAddStaff();   
            }, (p) =>
            {
                NhanVienDP.Flag.AddStaff(AddItem);
                AddItem.Reset();
                AddItem.MaNV = NhanVienDP.Flag.AutoIDStaff();
                Fulltime = "Full-time";
            });
        }
        #region Method
        public bool IfCanAddStaff()
        {
            if (string.IsNullOrEmpty(AddItem.MaNV) || string.IsNullOrEmpty(AddItem.HoTen) || string.IsNullOrEmpty(AddItem.ChucVu)
                || string.IsNullOrEmpty(Fulltime) || string.IsNullOrEmpty(AddItem.NgayVaoLam))
                return false;
            if (!isNumber(AddItem.SDT)) return false;
            foreach (NhanVien nv in ListStaff)
            {
                if (nv.TaiKhoan == AddItem.TaiKhoan && !string.IsNullOrEmpty(AddItem.TaiKhoan)) return false;
            }
            if ((!String.IsNullOrEmpty(AddItem.TaiKhoan) && String.IsNullOrEmpty(AddItem.MatKhau))
            || (String.IsNullOrEmpty(AddItem.TaiKhoan) && !String.IsNullOrEmpty(AddItem.MatKhau))) return false;

            return true;
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
        #endregion
    }
}
