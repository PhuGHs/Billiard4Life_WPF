using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billiard4Life.Command;
using System.Windows.Input;
using Diacritics.Extensions;
using Billiard4Life.Models;
using Billiard4Life.View;
using Billiard4Life.DataProvider;
using Billiard4Life.ViewModel;
using Billiard4Life.State.Navigator;
using System.Windows.Data;
using System.ComponentModel;
using TinhTrangBan.Models;
using System.Windows;
using RestaurantManagement.ViewModel;

namespace Billiard4Life.ViewModel
{
    public class MenuViewModel : BaseViewModel
    {
        public MenuViewModel()
        {
            //LoadMenu
            LoadMenu();
            Tables = MenuDP.Flag.GetTables();
            Kho = MenuDP.Flag.GetIngredients();
            _selectedIngredientsName = new ObservableCollection<string>();
            _menuItemsView = new CollectionViewSource();
            _menuItemsView.Source = MenuItems;
            _menuItemsView.Filter += MenuItems_Filter;
            _menuItemsView.SortDescriptions.Add(new SortDescription("FoodName", ListSortDirection.Ascending));
            //Command actions
            OrderFeature_Command = new RelayCommand<MenuItem>((p) => true, (p) => OrderAnItem(p.ID));
            RemoveItemFeature_Command = new RelayCommand<SelectedMenuItem>((p) => true, (p) => RemoveAnItem(p));
            ClearAllSelectedDishes = new RelayCommand<object>((p) =>
            {
                if (SelectedItems.Count == 0) return false;
                return true;
            }, (p) => {
                MyMessageBox msb = new MyMessageBox("Bạn có muốn xoá tất cả những món đã chọn?", true);
                msb.ShowDialog();
                if (msb.ACCEPT() == false)
                {
                    return;
                }
                SelectedItems.Clear();
                DecSubtotal = 0;
                StrSubtotal = "0 VND";
            });
            SortingFeature_Command = new RelayCommand<object>((p) => true, (p) => {
                SortMenuItems();
            });
            Inform_Chef_Of_OrderedDishes = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                string mess = "";
                string tennl = String.Empty;
                try
                {
                    if (SelectedTable != null)
                    {
                        if (SelectedItems.Count > 0)
                        {
                            HasEnoughIngredients();
                            if (SelectedTable.Status == 0)
                            {
                                MyMessageBox typeOfCustomerAnnouncement = new MyMessageBox("Bạn muốn order thêm món ăn?", true);
                                typeOfCustomerAnnouncement.ShowDialog();
                                if (typeOfCustomerAnnouncement.ACCEPT() == false)
                                {
                                    mess = "Bàn hiện đang được sử dụng! Hãy chọn bàn khác";
                                    return;
                                }
                            }
                            if (_sumIngredients.Count == 0)
                            {
                                mess = "Hãy thêm thông tin nguyên liệu cho món!";
                                return;
                            }
                            if (_selectedIngredientsName.Count > 0)
                            {
                                tennl += $"{_selectedIngredientsName[0]}";
                                if (_selectedIngredientsName.Count > 1)
                                {
                                    for (int i = 1; i < _selectedIngredientsName.Count; i++)
                                    {
                                        tennl += $" , {_selectedIngredientsName[i]}";
                                    }
                                }
                                mess = $"Không đủ nguyên liệu ({tennl}). Hãy nhập thêm!";
                                return;
                            }
                            MaNV = getMaNV();
                            MenuDP.Flag.PayABill(Convert.ToInt16(SelectedTable.NumOfTable), DecSubtotal, MaNV);
                            string SoHD = MenuDP.Flag.GetCurrentBillIDForThisTable(Convert.ToInt16(SelectedTable.NumOfTable));
                            foreach (SelectedMenuItem orderdish in SelectedItems)
                            {
                                MenuDP.Flag.Fill_CTHD(SoHD, orderdish.ID, orderdish.Quantity);
                                MenuDP.Flag.UpdateKho(orderdish.ID, orderdish.Quantity);
                            }
                            mess = "Đặt bàn thành công!";
                            SelectedItems.Clear();
                            DecSubtotal = 0;
                            StrSubtotal = "0 VND";
                            _selectedIngredientsName.Clear();
                            _sumIngredients.Clear();
                        }
                        else
                        {
                            MaNV = getMaNV();
                            MenuDP.Flag.PayABill(Convert.ToInt16(SelectedTable.NumOfTable), 0, MaNV);
                            mess = "Đã bắt đầu tính thời gian!";
                        }
                        Tables = MenuDP.Flag.GetTables();
                    }
                    else if (SelectedTable == null)
                    {
                        mess = "Bạn chưa chọn bàn";
                    }
                }
                catch (Exception ex)
                {
                    MyMessageBox msb = new MyMessageBox(ex.Message);
                    msb.Show();
                }
                finally
                {
                    MyMessageBox ms = new MyMessageBox(mess);
                    ms.Show();
                }
            });
            _selectedItems = new ObservableCollection<SelectedMenuItem>();
            _comboBox_2Items = new ObservableCollection<string>();
            LoadCombobox_2Items();
        }

        #region attributes
        private ObservableCollection<MenuItem> _menuItems;
        private ObservableCollection<SelectedMenuItem> _selectedItems;
        private ObservableCollection<Table> _tables;
        private ObservableCollection<ChiTietMon> _ingredients;
        private ObservableCollection<ChiTietMon> _sumIngredients;
        private ObservableCollection<Models.Kho> _kho;
        private ObservableCollection<string> _selectedIngredientsName;
        private Table _selectedTable;
        private ObservableCollection<string> _comboBox_2Items;
        private CollectionViewSource _menuItemsView;
        private string myComboboxSelection = "A -> Z";
        private decimal dec_subtotal = 0;
        private string str_subtotal = "0 VND";
        private string _searchText;
        private string MaNV;
        #endregion

        #region properties
        public ObservableCollection<MenuItem> MenuItems { get { return _menuItems; } set { _menuItems = value; OnPropertyChanged(); } }
        public ObservableCollection<SelectedMenuItem> SelectedItems { get { return _selectedItems; } set { _selectedItems = value; OnPropertyChanged(); } }
        public ObservableCollection<Table> Tables { get { return _tables; } set { _tables = value; OnPropertyChanged(); } }
        public ObservableCollection<ChiTietMon> Ingredients { get { return _ingredients; } set { _ingredients = value; OnPropertyChanged(); } }
        public ObservableCollection<Models.Kho> Kho { get { return _kho; } set { _kho = value; OnPropertyChanged(); } }
        public ObservableCollection<string> ComboBox_2Items { get { return _comboBox_2Items; } set { _comboBox_2Items = value; } }
        public string MyComboboxSelection { get { return myComboboxSelection; } set { myComboboxSelection = value; OnPropertyChanged(); } }
        public Table SelectedTable { get { return _selectedTable; } set { _selectedTable = value; OnPropertyChanged(); } }
        public ICollectionView MenuItemCollection
        {
            get
            {
                return this._menuItemsView.View;
            }
        }
        public string Day
        {
            get
            {
                return DateTime.Today.DayOfWeek.ToString() + ", " + DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
        public Decimal DecSubtotal
        {
            set
            {
                dec_subtotal = value;
                OnPropertyChanged();
            }
            get { return dec_subtotal; }
        }

        public string StrSubtotal
        {
            set
            {
                str_subtotal = value;
                OnPropertyChanged();
            }
            get { return str_subtotal; }
        }

        public string SearchText { get { return _searchText; } set { _searchText = value; this._menuItemsView.View.Refresh(); OnPropertyChanged(); } }
        #endregion

        #region commands
        public ICommand OrderFeature_Command { get; set; }
        public ICommand RemoveItemFeature_Command { get; set; }
        public ICommand SortingFeature_Command { get; set; }
        public ICommand ClearAllSelectedDishes { get; set; }
        public ICommand Inform_Chef_Of_OrderedDishes { get; set; }
        public ICommand SwitchCustomerTable { get; set; }
        #endregion

        #region methods
        private void LoadCombobox_2Items()
        {
            _comboBox_2Items.Add("Giá cao -> thấp");
            _comboBox_2Items.Add("Giá thấp -> cao");
            _comboBox_2Items.Add("A -> Z");
            _comboBox_2Items.Add("Z -> A");

            ComboBox_2Items = _comboBox_2Items;
        }

        private void OrderAnItem(string ID)
        {
            foreach (MenuItem item in _menuItems)
            {
                if (item.ID == ID)
                {
                    DecSubtotal += item.Price;
                    StrSubtotal = String.Format("{0:0,0 VND}", DecSubtotal);
                    SelectedMenuItem x = checkIfAnItemIsInOrderItems(ID);
                    if (x != null)
                    {
                        x.Quantity++;
                        return;
                    }
                    SelectedMenuItem s_item = new SelectedMenuItem(item.ID, item.FoodName, item.Price, 1);
                    SelectedItems.Add(s_item);
                    break;
                }
            }
        }
        private void RemoveAnItem(SelectedMenuItem x)
        {
            DecSubtotal -= x.Price;
            StrSubtotal = String.Format("{0:0,0 VND}", DecSubtotal);
            if (x.Quantity > 1)
            {
                x.Quantity--;
            }
            else
            {
                SelectedItems.Remove(x);
            }
        }

        public void SortMenuItems()
        {
            _menuItemsView.SortDescriptions.Clear();

            if (MyComboboxSelection == "Giá cao -> thấp")
            {
                _menuItemsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Descending));
            }
            else if (MyComboboxSelection == "Giá thấp -> cao")
            {
                _menuItemsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Ascending));
            }
            else if (MyComboboxSelection == "A -> Z")
            {
                _menuItemsView.SortDescriptions.Add(new SortDescription("FoodName", ListSortDirection.Ascending));
            }
            else if (MyComboboxSelection == "Z -> A")
            {
                _menuItemsView.SortDescriptions.Add(new SortDescription("FoodName", ListSortDirection.Descending));
            }
        }
        #endregion

        #region complementary methods
        private SelectedMenuItem checkIfAnItemIsInOrderItems(string ID)
        {
            foreach (SelectedMenuItem item in _selectedItems)
            {
                if (item.ID == ID)
                {
                    return item;
                }
            }
            return null;
        }
        public void MenuItems_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                e.Accepted = true;
                return;
            }

            Models.MenuItem item = e.Item as Models.MenuItem;
            if (item.FoodName.RemoveDiacritics().ToLower().Contains(SearchText.RemoveDiacritics().ToLower()))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private async Task LoadMenu()
        {
            _menuItems = await MenuDP.Flag.ConvertToCollection();
        }

        private string getMaNV()
        {
            return LoginWindowVM.MaNV;
        }
        private void HasEnoughIngredients()
        {
            Models.Kho x = null;
            _sumIngredients = MenuDP.Flag.getSumIngredients(SelectedItems);
            foreach (SelectedMenuItem item in SelectedItems)
            {
                foreach (ChiTietMon ctm in _sumIngredients)
                {
                    x = getKhoItem(ctm.TenNL);
                    if (x.TonDu - ctm.SoLuong * item.Quantity < 0)
                    {
                        if (!_selectedIngredientsName.Contains(ctm.TenNL))
                        {
                            _selectedIngredientsName.Add(ctm.TenNL);
                        }
                    }
                }
            }
        }

        private Models.Kho getKhoItem(string tensp)
        {
            Models.Kho item = null;
            try
            {
                foreach (Models.Kho x in Kho)
                {
                    if (String.Compare(x.TenSanPham, tensp) == 0)
                    {
                        item = x; break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return item;
        }
        #endregion
    }
}
