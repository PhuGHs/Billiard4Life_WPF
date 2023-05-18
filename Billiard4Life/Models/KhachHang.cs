using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class KhachHang
    {
        private string _Ma;
        public string Ma { get { return _Ma; } set { _Ma = value; } }
        private string _TenKH;
        public string TenKH { get { return _TenKH; } set { _TenKH = value; } }
        private string _SDT;
        public string SDT { get { return _SDT;} set { _SDT = value; } }
        private string _Email;
        public string Email { get { return _Email; } set { _Email = value; } }
        private int _Diem;
        public int Diem { get { return _Diem; } set { _Diem = value; } }
        public KhachHang(string ma, string ten, string sdt, string email, int diem)
        {
            Ma = ma;
            TenKH = ten;
            SDT = sdt;
            Email = email;
            Diem = diem;
        }
    }
}
