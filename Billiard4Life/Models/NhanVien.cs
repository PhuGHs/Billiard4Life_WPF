using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Billiard4Life.ViewModel;

namespace Billiard4Life.Models
{
    public class NhanVien : BaseViewModel
    {
        private string _MaNV;
        public string MaNV { get => _MaNV; set { _MaNV = value; OnPropertyChanged(); } }
        private string _HoTen;
        public string HoTen { get => _HoTen; set { _HoTen = value; OnPropertyChanged(); } }
        private string _ChucVu;
        public string ChucVu { get => _ChucVu; set { _ChucVu = value; OnPropertyChanged(); }  }
        private string _DiaChi;
        public string DiaChi { get => _DiaChi; set {
                _DiaChi = value;
                OnPropertyChanged();
            } }
        private bool _Fulltime;
        public bool Fulltime { get => _Fulltime; set {
                _Fulltime = value;
                OnPropertyChanged();
            } }
        private string _SDT;
        public string SDT { get => _SDT; set {
                _SDT = value;
                OnPropertyChanged();
            } }
        private string _NgayVaoLam;
        public string NgayVaoLam { get => _NgayVaoLam; set {
                _NgayVaoLam = value;
                OnPropertyChanged();
            } }
        private string _NgaySinh;
        public string NgaySinh { get => _NgaySinh; set {
                _NgaySinh = value;
                OnPropertyChanged();
            } }
        private string _TaiKhoan;
        public string TaiKhoan { get => _TaiKhoan; set {
                _TaiKhoan = value;
                OnPropertyChanged();
            } }
        private string _MatKhau;
        public string MatKhau { get => _MatKhau; set {
                _MatKhau = value;
                OnPropertyChanged();
            } }
        private BitmapImage _anhdaidien;
        public BitmapImage AnhDaiDien { get => _anhdaidien; set { _anhdaidien = value; OnPropertyChanged(); }  }
        public NhanVien(string id, string ten, string chucvu, string diachi, bool fulltime, string sdt, string ngayvl, string ngsinh)
        {
            MaNV = id;
            HoTen = ten;
            ChucVu = chucvu;
            DiaChi = diachi;
            Fulltime = fulltime;
            SDT = sdt;
            NgayVaoLam = ngayvl;
            NgaySinh = ngsinh;
        }

        public NhanVien(string id, string ten, string chucvu, string diachi, bool fulltime, string sdt, string ngayvl, string ngsinh, string taikhoan, string matkhau)
        {
            MaNV = id;
            HoTen = ten;
            ChucVu = chucvu;
            DiaChi = diachi;
            Fulltime = fulltime;
            SDT = sdt;
            NgayVaoLam = ngayvl;
            NgaySinh = ngsinh;
            TaiKhoan = taikhoan;
            MatKhau = matkhau;
        }
        public NhanVien(string id, string ten)
        {
            MaNV = id;
            HoTen = ten;
            ChucVu = "";
            DiaChi = "";
            Fulltime = false;
            SDT = "";
            NgayVaoLam = "";
            NgaySinh = "";
            TaiKhoan = "";
            MatKhau = "";
        }
        public NhanVien(NhanVien nv)
        {
            MaNV = nv.MaNV;
            HoTen = nv.HoTen;
            ChucVu = nv.ChucVu;
            DiaChi = nv.DiaChi;
            Fulltime = nv.Fulltime;
            SDT = nv.SDT;
            NgayVaoLam = nv.NgayVaoLam;
            NgaySinh = nv.NgaySinh;
            TaiKhoan = "";
            MatKhau = "";
        }
        public NhanVien() 
        {
            MaNV = "";
            HoTen = "";
            ChucVu = "";
            DiaChi = "";
            Fulltime = false;
            SDT = "";
            NgayVaoLam = "";
            NgaySinh = "";
            TaiKhoan = "";
            MatKhau = "";
        }
        public void Reset()
        {
            MaNV = "";
            HoTen = "";
            ChucVu = "";
            DiaChi = "";
            Fulltime = false;
            SDT = "";
            NgayVaoLam = DateTime.Now.ToString();
            NgaySinh = DateTime.Now.ToString();
            TaiKhoan = "";
            MatKhau = "";
        }
    }
}
