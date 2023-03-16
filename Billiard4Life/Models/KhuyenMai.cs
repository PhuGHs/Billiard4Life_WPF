using Billiard4Life.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.Models
{
    public class KhuyenMai : BaseViewModel
    {
        #region attributes
        private string _MAKM;
        private string _TenKM;
        private int _GiamGia;
        private Decimal _MucApDung;
        private DateTime _NgayBatDau;
        private DateTime _NgayKetThuc;
        private string _MoTa;
        #endregion

        #region properties
        public string MAKM { get { return _MAKM; } set { _MAKM = value; OnPropertyChanged(); } }
        public string TenKM { get { return _TenKM; } set { _TenKM = value; OnPropertyChanged(); } }
        public int GiamGia { get { return _GiamGia; } set { _GiamGia = value; OnPropertyChanged(); } }
        public Decimal MucApDung { get { return _MucApDung; } set { _MucApDung = value; OnPropertyChanged(); } }
        public DateTime NgayBatDau { get { return _NgayBatDau; } set { _NgayBatDau = value; OnPropertyChanged(); } }
        public DateTime NGayKetThuc { get { return _NgayKetThuc; } set { _NgayKetThuc = value; OnPropertyChanged(); } }
        public string MoTa { get { return _MoTa; } set { _MoTa = value; OnPropertyChanged(); } }
        #endregion

        public KhuyenMai(string makm, string tenkm, int giamgia, Decimal mucgiam, DateTime ngaybd, DateTime ngayketthuc, string mota)
        {
            _MAKM = makm;
            _TenKM = tenkm;
            _GiamGia = giamgia;
            _MucApDung = mucgiam;
            _NgayBatDau = ngaybd;
            _NgayKetThuc = ngayketthuc;
            _MoTa = mota;
        }
    }
}
