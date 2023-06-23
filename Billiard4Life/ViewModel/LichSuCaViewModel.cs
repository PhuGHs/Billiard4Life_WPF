using Billiard4Life.DataProvider;
using Billiard4Life.Models;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.ViewModel
{
    public class LichSuCaViewModel : BaseViewModel
    {
        private string _TimeStart;
        public string TimeStart
        {
            get => _TimeStart;
            set
            {
                _TimeStart = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<HoaDon> _ListBill;
        public ObservableCollection<HoaDon> ListBill { get => _ListBill; set { _ListBill = value; OnPropertyChanged(); } }
        private HoaDon _BillSelected;
        public HoaDon BillSelected
        {
            get => _BillSelected;
            set { _BillSelected = value; OnPropertyChanged(); }
        }
        private ObservableCollection<string> _PayMethods;
        public ObservableCollection<string> PayMethods
        {
            get => _PayMethods;
            set
            {
                _PayMethods = value;
                OnPropertyChanged();
            }
        }
        private string _PayMethodSelected;
        public string PayMethodSelected
        {
            get => _PayMethodSelected;
            set
            {
                _PayMethodSelected = value;
                ListViewDisplay(PayMethodSelected);
                OnPropertyChanged();
            }
        }
        public LichSuCaViewModel()
        {
            //initialize
            PayMethods = new ObservableCollection<string>();
            ListBill = new ObservableCollection<HoaDon>();
            TimeStart = NhanVienDP.Flag.TimeStartWork();
            GetPayMethods();
            ListViewDisplay("Tất cả");
        }
        #region Method
        public void GetPayMethods()
        {
            PayMethods.Add("Tiền mặt");
            PayMethods.Add("Thẻ ngân hàng");
            PayMethods.Add("Chuyển khoản ngân hàng");
            PayMethods.Add("Chuyển MOMO");
            PayMethods.Add("Tất cả");
        }
        public void ListViewDisplay(string paymethod)
        {
            ListBill = HoaDonDP.Flag.GetBillsShift(paymethod);
        }
        #endregion
    }
}
