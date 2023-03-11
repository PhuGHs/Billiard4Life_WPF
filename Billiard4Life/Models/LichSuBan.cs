using Billiard4Life.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LichSuBan.Models
{

    public class LichSuBanModel
    {

        private string _TenMon;
        public string TenMon { get => _TenMon; set => _TenMon = value; }
        private int _SoHD;
        public int SoHD { get => _SoHD; set => _SoHD = value; }
        private int _SoLuong;
        public int SoLuong { get => _SoLuong; set => _SoLuong = value; }
      
        private string _NgayHD;
        public string NgayHD { get => _NgayHD; set => _NgayHD = value; }
        private string _MaMon;
        public string MaMon { get => _MaMon; set => _MaMon = value; }
        private string _TriGia;
        public string TriGia { get => _TriGia; set => _TriGia = value; }
        private string _ngayHD;
        public string ngayHD { get => _ngayHD; set=>_ngayHD = value; }

        public LichSuBanModel(int ID, string maMON, string ProductName, int Quantity, string ImportPrice, string ngayhd)
        {
            TenMon = ProductName;
            SoHD = ID;
            SoLuong = Quantity;
            ngayHD = ngayhd;
            MaMon = maMON;
            TriGia = ImportPrice;


        }

       
    }

    
}


