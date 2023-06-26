using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class HoaDon
    {
        private string _SoHD;
        public string SoHD { get => _SoHD; set => _SoHD = value; }
        private string _SoGio;
        public string SoGio { get => _SoGio; set => _SoGio = value; }
        private string _TriGia;
        public string TriGia { get => _TriGia; set => _TriGia = value; }
        private string _MaNV;
        public string MaNV { get => _MaNV; set => _MaNV = value; }
        private string _TenKH;
        public string TenKH { get => _TenKH; set => _TenKH = value; }
        private string _MaKM;
        public string MaKM { get => _MaKM; set => _MaKM = value; }
        private string _SoBan;
        public string SoBan { get => _SoBan; set => _SoBan = value; }
        private string _NgayHD;
        public string NgayHD { get => _NgayHD; set => _NgayHD = value; }
        private string _HinhThucThanhToan;
        public string HinhThucThanhToan { get => _HinhThucThanhToan; set => _HinhThucThanhToan = value; }
        public HoaDon(string sohd, string sogio, string trigia, string manv, string ban, string ngay, string httt, string tenkh = "", string makm = "")
        {
            SoHD = sohd;
            SoGio = sogio;
            TriGia = trigia;
            MaNV = manv;
            TenKH = tenkh;
            MaKM = makm;
            SoBan = ban;
            NgayHD = ngay;
            HinhThucThanhToan = httt;
        }
    }
}
