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
        private string _NhomSP;
        public string NhomSP { get => _NhomSP; set { _NhomSP = value; OnPropertyChanged(); } }
        private bool _DuocChon;
        public bool DuocChon { get => _DuocChon; set { _DuocChon = value; OnPropertyChanged(); } }
        private float _DinhLuong;
        public float DinhLuong { get => _DinhLuong; set { _DinhLuong = value; OnPropertyChanged(); } }

        public Kho(string ten, float tondu, string donvi, string nhom)
        {
            TenSanPham = ten;
            TonDu = tondu;
            DonVi = donvi;
            NhomSP = nhom;
            DuocChon = false;
            DinhLuong = 0;
        }
        public Kho()
        {

        }
    }
}
