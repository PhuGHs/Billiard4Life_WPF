using Billiard4Life.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TinhTrangBan.Models
{
    public class TinhTrangBan { }
    public class Table : BaseViewModel
    {
        private string numoftable;
        private int status;
        private string coloroftable;
        private string kindOfTable;
        private Decimal price;
        private int id;
        private int id_bill;
        public string NumOfTable
        {
            get { return numoftable; }
            set
            {
                numoftable = value;
                OnPropertyChanged();
            }
        }

        public Decimal Price
        {
            get { return price; }
            set { price = value; OnPropertyChanged(); }
        }

        public string KindOfTable
        {
            get { return kindOfTable; }
            set
            {
                kindOfTable = value;
                OnPropertyChanged();
            }
        }

        public int Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public string Coloroftable
        {
            get { return coloroftable; }
            set
            {
                coloroftable = value;
                OnPropertyChanged();
            }
        }

        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }
        public int Bill_ID
        {
            get { return id_bill; }
            set
            {
                id_bill = value;
                OnPropertyChanged();
            }
        }
    }
    public class SelectedMenuItems : BaseViewModel
    {
        #region variables
        private int _id;
        private string _foodName;
        private Decimal _price;
        private int _quantity;
        #endregion
        public SelectedMenuItems(string foodName, Decimal price, int quantity)
        {
            _foodName = foodName;
            _price = price;
            _quantity = quantity;
        }
        #region properties
        public int ID { get { return _id; } set { _id = value; } }

        public string FoodName
        {
            get { return _foodName; }
            set
            {
                if (_foodName != value)
                {
                    _foodName = value;
                    OnPropertyChanged("food name");
                }
            }
        }

        public Decimal Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged("price");
                }
            }
        }

        public string PriceVNDCurrency
        {
            get { return String.Format("{0:0,0 VND}", Price); }
        }
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged("quantity");
                }
            }
        }
        #endregion
    }
    
}
