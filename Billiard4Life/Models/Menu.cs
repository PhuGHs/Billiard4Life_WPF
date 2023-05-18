using Billiard4Life.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Billiard4Life.Models
{
    public class MenuModel { }

    public class MenuItem : BaseViewModel
    {
        public MenuItem(string id = "", string foodName = "", decimal price = 0, BitmapImage foodImage = null)
        {
            this.id = id;
            this.foodName = foodName;
            this.price = price;
            this.foodImage = foodImage;
        }
        private string id;
        private string foodName;
        private decimal price;
        private BitmapImage foodImage;
        private int cookingTime;
        public string ID { get { return id; } set { id = value; OnPropertyChanged(); } }

        public string FoodName
        {
            get { return foodName; }
            set
            {
                foodName = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage FoodImage
        {
            get { return foodImage; }
            set
            {
                foodImage = value;
                OnPropertyChanged();
            }
        }

        public string PriceVNDCurrency
        {
            get { return String.Format("{0:0,0 VND}", Price); }
        }
        public string Str_Price
        {
            get
            {
                return Price.ToString();
            }
            set
            {
                if (!IsNumber(value))
                {
                    Price = 0;
                }
                else
                {
                    Price = Convert.ToDecimal(value);
                }
                OnPropertyChanged();
            }
        }
        public bool IsNullOrEmpty()
        {
            if (foodImage == null || id == "" || foodName == "" || price == 0)
            {
                return true;
            }
            return false;
        }

        public void Clear()
        {
            this.FoodName = "";
            this.Price = 0;
            this.ID = "";
        }

        private static bool IsNumber(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < 48 || s[i] > 57) return false;
            }
            return true;
        }
        private static bool IsMoney(string s)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if ((s[i] < 48 || s[i] > 57) && s[i] != '.')
                    return false;
                if (s[i] == '.') count++;
            }
            if (s[0] == '.') return false;
            if (s[s.Length - 1] == '.') return false;
            if (s[0] == '0') return false;
            if (count > 1) return false;
            return true;
        }

    }
    public class SelectedMenuItem : BaseViewModel
    {
        public SelectedMenuItem(string ID, string foodName, Decimal price, int quantity)
        {
            _id = ID;
            _foodName = foodName;
            _price = price;
            _quantity = quantity;
        }
        #region attributes
        private string _id;
        private string _foodName;
        private Decimal _price;
        private int _quantity;
        #endregion
        #region properties
        public string ID { get { return _id; } set { _id = value; } }

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
