using Billiard4Life.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class Kho : BaseViewModel
    {
        private string _TenSanPham;
        public string TenSanPham { get => _TenSanPham; set { _TenSanPham = value; OnPropertyChanged(); }}
        private float _TonDu;
        public float TonDu { get => _TonDu; set { _TonDu = value; OnPropertyChanged(); } }
        private string _DonVi;
        public string DonVi { get => _DonVi; set { _DonVi = value; OnPropertyChanged(); } }
        private string _DonGia;
        public string DonGia { get => _DonGia; set { _DonGia = value; OnPropertyChanged(); } }


        public Kho(string ten, float tondu, string donvi, string dongia)
        {
            TenSanPham = ten;
            TonDu = tondu;
            DonVi = donvi;
            DonGia = dongia;
        }
        public Kho()
        {

        }
    }
}
