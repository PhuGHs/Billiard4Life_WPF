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
using Billiard4Life.DataProvider;

namespace Billiard4Life.ViewModel
{
    public class NhanVienViewModel : BaseViewModel
    {
        private ObservableCollection<NhanVien> _ListStaff;
        public ObservableCollection<NhanVien> ListStaff { get => _ListStaff; set { _ListStaff = value; OnPropertyChanged(); } }

        private NhanVien _NhanVienItem;
        public NhanVien NhanVienItem
        {
            get => _NhanVienItem;
            set
            {
                _NhanVienItem = value;
                OnPropertyChanged();
            }
        }
        #region //Selected Item
        private NhanVien _StaffSelected;
        public NhanVien StaffSelected
        {
            get => _StaffSelected;
            set
            {
                _StaffSelected = value;
                OnPropertyChanged();
                if (StaffSelected != null)
                {
                    NhanVienItem = StaffSelected;
                    if (StaffSelected.Fulltime) Fulltime = "Full-time";
                    else Fulltime = "Part-time";
                }
                OnPropertyChanged();
            }
        }
        #endregion

        private string _Fulltime;
        public string Fulltime
        {
            get => _Fulltime;
            set
            {
                _Fulltime = value; OnPropertyChanged();
                if (Fulltime == "Full-time") NhanVienItem.Fulltime = true; else NhanVienItem.Fulltime = false;
            }
        }

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
        public NhanVienViewModel()
        {
            ListStaff = new ObservableCollection<NhanVien>();
            NhanVienItem = new NhanVien();

            NhanVienItem.NgaySinh = DateTime.Now.ToShortDateString();
            NhanVienItem.NgayVaoLam = DateTime.Now.ToShortDateString();
            ListViewDisplay("SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV WHERE Xoa = 0");

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
                ListViewDisplay("SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV  WHERE Xoa = 0");
                return;
            });

            #region //edit command
            EditCM = new RelayCommand<object>((p) =>
            {
                if (String.IsNullOrEmpty(NhanVienItem.HoTen) || String.IsNullOrEmpty(NhanVienItem.ChucVu) 
                || String.IsNullOrEmpty(Fulltime) || String.IsNullOrEmpty(NhanVienItem.NgayVaoLam))
                    return false;
                if (!isNumber(NhanVienItem.SDT)) return false;
                if ((!String.IsNullOrEmpty(NhanVienItem.TaiKhoan) && String.IsNullOrEmpty(NhanVienItem.MatKhau)) 
                || (String.IsNullOrEmpty(NhanVienItem.TaiKhoan) && !String.IsNullOrEmpty(NhanVienItem.MatKhau))) return false;
                return true;

            }, (p) =>
            {
                NhanVienDP.Flag.UpdateInfoStaff(NhanVienItem);
                ListViewDisplay("SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV WHERE Xoa = 0");
            });
            #endregion

            #region //delete command
            DeleteCM = new RelayCommand<object>((p) =>
            {
                if (StaffSelected == null) return false;
                if (StaffSelected.ChucVu == "Quản lý") return false;
                return true;
            }, (p) =>
            {
                NhanVienDP.Flag.DeleteStaff(NhanVienItem.MaNV);
                ListViewDisplay("SELECT n.*, t.ID, t.MatKhau FROM NHANVIEN AS n LEFT JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV  WHERE Xoa = 0");
            });
            #endregion
        }
        #region Method
        public void ListViewDisplay(string query)
        {
            ListStaff.Clear();
            ListStaff = NhanVienDP.Flag.GetAllStaff(query);
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
