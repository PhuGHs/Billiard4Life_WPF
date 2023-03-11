using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class ChamCong
    {
        private string _MaNV;
        public string MaNV { get => _MaNV; set => _MaNV = value; }
        private string _NgayCC;
        public string NgayCC { get => _NgayCC; set => _NgayCC = value; }
        private string _SoGioCong;
        public string SoGioCong { get => _SoGioCong; set => _SoGioCong = value; }
        private string _GhiChu;
        public string GhiChu { get => _GhiChu; set => _GhiChu = value; }
        public ChamCong(string ma, string ngay, string gio, string note)
        {
            MaNV = ma;
            NgayCC = ngay;
            SoGioCong = gio;
            GhiChu = note;
        }
        public ChamCong(string ma) 
        {
            MaNV = ma;
        }
        public void Set(string ngay, string gio, string note)
        {
            NgayCC = ngay;
            SoGioCong = gio;
            GhiChu = note;
        }
    }
}
