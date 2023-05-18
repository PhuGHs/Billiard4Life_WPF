using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class NhapKho
    {
        private string _MaNhap;
        public string MaNhap { get => _MaNhap; set => _MaNhap = value; }
        private string _TenSP;
        public string TenSP { get => _TenSP; set => _TenSP = value; }
        private string _DonVi;
        public string DonVi { get => _DonVi; set => _DonVi = value; }
        private string _Nhom;
        public string Nhom { get => _Nhom; set => _Nhom = value; }
        private string _DonGia;
        public string DonGia { get => _DonGia; set => _DonGia = value; }
        private string _SoLuong;
        public string SoLuong { get => _SoLuong; set => _SoLuong = value; }
        private string _NgayNhap;
        public string NgayNhap { get => _NgayNhap; set => _NgayNhap = value; }
        private string _NguonNhap;
        public string NguonNhap { get => _NguonNhap; set => _NguonNhap = value; }
        private string _LienLac;
        public string LienLac { get => _LienLac; set => _LienLac = value; }
        public NhapKho(string ma, string ten, string donvi, string nhom, string dongia, string soluong, string ngaynhap, string nguon, string lienlac)
        {
            MaNhap = ma;
            TenSP = ten;
            DonVi = donvi;
            DonGia = dongia;
            Nhom = nhom;
            SoLuong = soluong;
            NgayNhap = ngaynhap;
            NguonNhap = nguon;
            LienLac = lienlac;
        }
    }
}
