using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class ChiTietHoaDon
    {
        private string _TenSP;
        public string TenSP { get => _TenSP; set => _TenSP = value; }
        private string _SoLuong;
        public string SoLuong { get => _SoLuong; set => _SoLuong = value; }
        private string _DonGia;
        public string DonGia { get => _DonGia; set => _DonGia = value; }
        private string _ThanhTien;
        public string ThanhTien { get => _ThanhTien; set => _ThanhTien = value; }
        public ChiTietHoaDon(string ten, string soluonng, string dongia, string tien)
        {
            TenSP = ten;
            SoLuong = soluonng;
            DonGia = dongia;
            ThanhTien = tien;
        }
    }
}
