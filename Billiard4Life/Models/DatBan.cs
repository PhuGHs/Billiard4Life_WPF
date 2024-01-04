using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class DatBan
    {
        public int ID { get; set; }
        public string? TenKhachHang { get; set; }
        public string? SDT { get; set; }
        public string? NgayGio { get; set; }
        public string? LoaiBan { get; set; }
        public string? XacNhan { get; set; }
        public DatBan(int id, string ten, string sdt, string ngay, string loai, bool xacnhan)
        {
            ID = id;
            TenKhachHang = ten;
            SDT = sdt;
            NgayGio = ngay;
            LoaiBan = loai;
            XacNhan = xacnhan ? "Đã xác nhận" : "Chưa xác nhận";
        }
    }
}
