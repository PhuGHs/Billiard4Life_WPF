using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billiard4Life.ViewModel;

namespace Billiard4Life.Models
{
    public class ChiTietMon : BaseViewModel
    {
        private string _maMon;
        private string _tenNL;
        private float _soluong;

        public string MaMon { get { return _maMon; } set { _maMon = value; OnPropertyChanged(); } }
        public string TenNL { get { return _tenNL; } set { _tenNL = value; OnPropertyChanged(); } }
        public float SoLuong { get { return _soluong; } set { _soluong = value; OnPropertyChanged(); } }
        public ChiTietMon(string TenNL, string MaMon, float SoLuong = 0)
        {
            _maMon = MaMon;
            _tenNL = TenNL;
            _soluong = SoLuong;
        }
        
        public ChiTietMon(string TenNL, float SoLuong)
        {
            _tenNL = TenNL;
            _soluong= SoLuong;
        }
    }
}
