using Billiard4Life.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Models
{
    public class ThongKeModel { }
    public class Months : BaseViewModel
    {
        public Months(string month)
        {
            _month = month;
        }
        private string _month;
        public string Month
        {
            get { return _month; }
            set
            {
                _month = value;
                OnPropertyChanged();
            }
        }
    }
    public class Years : BaseViewModel
    {
        public Years(string year)
        {
            _year = year;
        }
        private string _year;
        public string Year
        {
            get { return _year; }
            set { _year = value; OnPropertyChanged(); }
        }
    }

}
