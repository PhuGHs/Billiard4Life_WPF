using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class Bep
    {
        
        private string _TenMon;
        public string TenMon { get => _TenMon; set => _TenMon = value; }
        private int _SoBan;
        public int SoBan { get => _SoBan; set => _SoBan = value; }
        private int _SoLuong;
        public int SoLuong { get => _SoLuong; set => _SoLuong = value; }
        private string? _TinhTrang;
        public string? TinhTrang { get => _TinhTrang; set => _TinhTrang = value; }
        private string _NgayCB;
        public string NgayCB { get => _NgayCB; set => _NgayCB = value; }
        private string _MaMon;
        public string MaMon { get => _MaMon; set => _MaMon = value; }
        private long _MaDM;
        public long MaDM { get => _MaDM; set => _MaDM = value; }

        public Bep(  long maDM,string maMon, int soBan, int soLuong, string ngayCB, string tinhTrang, string tenMon)
        {

          
            TenMon = tenMon;
            SoBan = soBan;
            SoLuong = soLuong;
            MaMon = maMon;
            TinhTrang = tinhTrang;
            NgayCB = ngayCB;
            MaDM = maDM;
        }
    }
}
