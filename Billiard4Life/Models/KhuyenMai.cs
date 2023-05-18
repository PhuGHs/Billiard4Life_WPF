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
        private string _NgayBatDau;
        private string _NgayKetThuc;
        private string _MoTa;
        private bool _isSelected;
        private string _trangThai;
        #endregion

        #region properties
        public string MAKM { get { return _MAKM; } set { _MAKM = value; OnPropertyChanged(); } }
        public string TenKM { get { return _TenKM; } set { _TenKM = value; OnPropertyChanged(); } }
        public int GiamGia { get { return _GiamGia; } set { _GiamGia = value; OnPropertyChanged(); } }
        public Decimal MucApDung { get { return _MucApDung; } set { _MucApDung = value; OnPropertyChanged(); } }
        public string NgayBatDau { get { return _NgayBatDau; } set { _NgayBatDau = value; OnPropertyChanged(); } }
        public string NGayKetThuc { get { return _NgayKetThuc; } set { _NgayKetThuc = value; OnPropertyChanged(); } }
        public string MoTa { get { return _MoTa; } set { _MoTa = value; OnPropertyChanged(); } }
        public string TrangThai { get { return _trangThai; } set { _trangThai = value; OnPropertyChanged(); } }
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged();} }
        #endregion

        public KhuyenMai(string makm, string tenkm, Decimal mucgiam, string trangthai, string ngaybd, string ngayketthuc, string mota, int giamgia)
        {
            _MAKM = makm;
            _TenKM = tenkm;
            _GiamGia = giamgia;
            _MucApDung = mucgiam;
            _NgayBatDau = ngaybd;
            _NgayKetThuc = ngayketthuc;
            _trangThai = trangthai;
            _MoTa = mota;
            _isSelected = false;
        }

        public KhuyenMai()
        {
            _GiamGia = 0;
            _MAKM = String.Empty;
            _TenKM = String.Empty;
            _trangThai = String.Empty;
            _MoTa = String.Empty;
            _NgayBatDau= String.Empty;
            _NgayKetThuc = String.Empty;
            _MucApDung = 0;
        }

        public bool isNullOrEmpty()
        {
            if(string.IsNullOrEmpty(_MAKM) 
                || string.IsNullOrEmpty(_TenKM) 
                || string.IsNullOrEmpty(_MoTa) 
                || string.IsNullOrEmpty(_trangThai) 
                || string.IsNullOrEmpty(_NgayBatDau) 
                || string.IsNullOrEmpty(_NgayKetThuc) 
                || _GiamGia == 0)
            {
                return true;
            }
            return false;
        }
        public void clear()
        {
            _GiamGia = 0;
            _MAKM = String.Empty;
            _TenKM = String.Empty;
            _trangThai = String.Empty;
            _MoTa = String.Empty;
            _NgayBatDau = String.Empty;
            _NgayKetThuc = String.Empty;
            _MucApDung = 0;
        }
    }
}
