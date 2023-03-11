using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class NhanVienCC
    {
        private string _MaNV;
        public string MaNV { get => _MaNV; set => _MaNV = value; }
        private string _HoTen;
        public string HoTen { get => _HoTen; set => _HoTen = value; }
        private string _ChucVu;
        public string ChucVu { get => _ChucVu; set => _ChucVu = value; }
        private string _Fulltime;
        public string Fulltime { get => _Fulltime; set => _Fulltime = value; }
        private float _TongSogio;
        public float TongSoGio { get => _TongSogio; set => _TongSogio = value; }
        public NhanVienCC(string ma, string ten, string chucvu, string ft, float tongso = 0)
        {
            MaNV = ma;
            HoTen = ten;
            ChucVu = chucvu;
            Fulltime = ft;
            TongSoGio = tongso;
        }
    }
}
