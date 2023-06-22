using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Billiard4Life.DataProvider;
using Billiard4Life.Models;
using Billiard4Life.View;
using System.Windows.Data;
using System.ComponentModel;
using Diacritics.Extensions;
using System.Data.SqlClient;
using System.Diagnostics;
using Button = System.Windows.Controls.Button;
using Kho = Billiard4Life.Models.Kho;

namespace Billiard4Life.ViewModel
{
    public class MenuAdminViewModel : BaseViewModel
    {
        public MenuAdminViewModel()
        {
            LoadMenu();
            Ingredients = MenuDP.Flag.GetIngredients();
            Ingredients_ForDishes = new ObservableCollection<ChiTietMon>();
            Deleted_Ingredients = new ObservableCollection<ChiTietMon>();
            _menuItemsView = new CollectionViewSource();
            _menuItemsView.Source = MenuItems;
            _menuItemsView.Filter += MenuItems_Filter;
            _ingredientsView = new CollectionViewSource();
            _ingredientsView.Source = Ingredients;
            _ingredientsView.Filter += Ingredients_Filter;
            addItem = new Models.MenuItem();
            MenuItem = new Models.MenuItem();
            AddItem.FoodImage = converting("pack://application:,,,/images/menu_default_image.jpg");
            #region Command executes
            AddOneMenuDish = new RelayCommand<Object>((p) => true, (p) =>
            {
                MenuAdmin_ThemMon window = new MenuAdmin_ThemMon(true);
                RefreshIngredients();
                window.DataContext = this;
                IsFirstTabVisible = true;
                IsAdding = true;
                AddItem.ID = MenuDP.Flag.AutoIDMenu();
                window.ShowDialog();
            });
            RemoveDish_Command = new RelayCommand<object>((p) =>
            {
                if (MenuItem == null) return false;
                if (MenuItem.FoodImage == null
                || MenuItem.FoodName == ""
                || MenuItem.ID == "") return false;
                return true;
            }, (p) =>
            {
                MyMessageBox msb = new MyMessageBox($"Bạn có muốn xoá món này?", true);
                msb.ShowDialog();
                if (msb.ACCEPT() == true)
                {
                    MenuDP.Flag.RemoveDish(MenuItem.ID);
                    MenuItems.Remove(MenuItem);
                    MyMessageBox msb2 = new MyMessageBox("Xoá thành công!");
                    msb2.Show();
                }
            });
            AddDish_Command = new RelayCommand<Button>((p) =>
            {
                if (AddItem.IsNullOrEmpty()) return false;
                return true;
            }, (p) =>
            {
                if (IsAnyIngredientSelected() == false)
                {
                    MyMessageBox msb = new MyMessageBox("Vui lòng chọn nguyên liệu cần thiết và định lượng!");
                    msb.ShowDialog();
                }
                else
                if (IsIngredientsValid() == false)
                {
                    MyMessageBox msb = new MyMessageBox("Định lượng phải lớn hơn 0!");
                    msb.ShowDialog();
                }
                else
                {
                    if (IsAdding)
                    {
                        MenuDP.Flag.AddDish(AddItem);
                        MenuItems.Add(new Models.MenuItem(AddItem.ID, AddItem.FoodName, AddItem.Price, AddItem.FoodImage));
                        foreach (Kho ctm in IngredientCollection)
                        {
                            if (ctm.DuocChon == true) MenuDP.Flag.SaveIngredients(new ChiTietMon(ctm.TenSanPham, AddItem.ID, ctm.DinhLuong));
                        }
                        RefreshIngredients();
                        AddItem.Clear();
                        AddItem.ID = MenuDP.Flag.AutoIDMenu();
                        MyMessageBox msb = new MyMessageBox("Thêm thành công!");
                        msb.Show();
                    }
                    else
                    {
                        MenuDP.Flag.EditDishInfo(AddItem);
                        MenuDP.Flag.RemoveIngredients(AddItem.ID);
                        foreach (Kho ctm in IngredientCollection)
                        {
                            if (ctm.DuocChon == true) MenuDP.Flag.SaveIngredients(new ChiTietMon(ctm.TenSanPham, AddItem.ID, ctm.DinhLuong));
                        }
                        MyMessageBox msb = new MyMessageBox("Sửa thành công!");
                        msb.Show();
                    }    
                }
            });
            AddImage_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
                op.Title = "Thêm ảnh món ăn";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    BitmapImage bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bmi.CacheOption = BitmapCacheOption.OnLoad;
                    bmi.UriSource = new Uri(op.FileName);
                    bmi.EndInit();
                    AddItem.FoodImage = bmi;
                }
            });

            #region Thêm nguyên liệu command execution
            EditIngredient_Command = new RelayCommand<object>((p) =>
            {
                if (MenuItem == null) return false;
                return true;
            }, 
            (p) =>
            {
                IsAdding = false;
                IsFirstTabVisible = false;
                AddItem = MenuItem;
                RefreshIngredients();
                Ingredients_ForDishes = MenuDP.Flag.GetIngredientsForDish(MenuItem.ID);
                foreach (ChiTietMon ctm in Ingredients_ForDishes)
                {
                    foreach (Kho item in IngredientCollection)
                    {
                        if (ctm.TenNL == item.TenSanPham)
                        {
                            item.DuocChon = true;
                            item.DinhLuong = ctm.SoLuong;
                        }
                    }
                }
                MenuAdmin_ThemMon IngreAddView = new(false);
                IngreAddView.DataContext = this;
                IngreAddView.ShowDialog();
            });

            #endregion
            #endregion

        }

        #region attributes
            private ObservableCollection<Models.MenuItem> _menuitems;
            private ObservableCollection<Kho> _ingredients;
            private ObservableCollection<ChiTietMon> _ingredients_ForDishes;
            private ObservableCollection<ChiTietMon> _deletedIngredients;
            private string _filterText;
            private string _ingreFilterText;
            private CollectionViewSource _menuItemsView;
            private CollectionViewSource _ingredientsView;
            private Models.MenuItem _menuitem;
            private Models.Kho _selected_Ingredient;
            private Models.MenuItem addItem;
            private bool IsAdding;
            private Visibility tabDish, tabIngredient;
            private bool _dishHasBeenAdded;
            private TabItem _selectedTab;
            private bool _isFirstTabVisible = true;
            public bool IsFirstTabVisible
            {
                get { return _isFirstTabVisible; }
                set
                {
                    _isFirstTabVisible = value;
                    OnPropertyChanged(nameof(IsFirstTabVisible));
                }
            }
        #endregion

        #region properties
        public ObservableCollection<Models.MenuItem> MenuItems { get { return _menuitems; } set { _menuitems = value; OnPropertyChanged(); } }
            public ObservableCollection<Kho> Ingredients { get { return _ingredients; } set { _ingredients = value; OnPropertyChanged(); } }
            public ObservableCollection<ChiTietMon> Ingredients_ForDishes { get { return _ingredients_ForDishes; } set { _ingredients_ForDishes = value; OnPropertyChanged(); } }
            public ObservableCollection<ChiTietMon> Deleted_Ingredients { get { return _deletedIngredients; } set { _deletedIngredients = value; OnPropertyChanged(); } }
            public Models.MenuItem MenuItem { get { return _menuitem; } set { _menuitem = value; OnPropertyChanged(); } }
            public Models.MenuItem AddItem { get { return addItem; } set { addItem = value; OnPropertyChanged(); } }
            public Kho Selected_Ingredient { get { return _selected_Ingredient; } set { _selected_Ingredient = value; OnPropertyChanged(); } }
            public bool DishHasBeenAdded { get { return _dishHasBeenAdded; } set { _dishHasBeenAdded = value; OnPropertyChanged(); } }
            public string FilterText { get { return _filterText; } set { _filterText = value; this._menuItemsView.View.Refresh(); OnPropertyChanged(); } }
            public string IngreFilterText { get { return _ingreFilterText; } set { _ingreFilterText = value; this._ingredientsView.View.Refresh(); OnPropertyChanged(); } }
            public Visibility TabDish { get { return tabDish; } set { tabDish = value; OnPropertyChanged(); } }
            public Visibility TabIngredient { get { return tabIngredient; } set { tabIngredient = value; OnPropertyChanged(); } }
            public TabItem SelectedTab { get { return _selectedTab; } set { _selectedTab = value; OnPropertyChanged(); } }
            public ICollectionView MenuItemCollection
            {
                get
                {
                    return this._menuItemsView.View;
                }
            }
            public ICollectionView IngredientCollection
            {
                get
                {
                    return this._ingredientsView.View;
                }
            }
        #endregion

        #region commands
            public ICommand AddDish_Command { get; set; }
            public ICommand RemoveDish_Command { get; set; }
            public ICommand AddImage_Command { get; set; }
            public ICommand EditIngredient_Command { get; set; }
            public ICommand AddOneMenuDish { get; set; }
        #endregion

        #region complementary functions
            public BitmapImage converting(string ur)
            {
                BitmapImage bmi = new BitmapImage();
                bmi.BeginInit();
                bmi.CacheOption = BitmapCacheOption.OnLoad;
                bmi.UriSource = new Uri(ur);
                bmi.EndInit();

                return bmi;
            }
            public bool IsListedInIngredientList(string TenNL)
            {
                if (Ingredients_ForDishes.Count == 0) return false;
                foreach (ChiTietMon ctm in Ingredients_ForDishes)
                {
                    if (ctm.TenNL.CompareTo(TenNL) == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            public bool IsListedInMenuList(string MaMon)
            {
                if (MenuItems.Count == 0) return false;
                foreach (Models.MenuItem mi in MenuItems)
                {
                    if (string.Compare(mi.ID, MaMon) == 0)
                        return true;
                }
                return false;
            }
            public bool CheckIfIngredientListInclude0InQuantity()
            {
                foreach (ChiTietMon ctm in Ingredients_ForDishes)
                {
                    if (ctm.SoLuong <= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            public void MenuItems_Filter(object sender, FilterEventArgs e)
            {
                if (string.IsNullOrEmpty(FilterText))
                {
                    e.Accepted = true;
                    return;
                }

                Models.MenuItem item = e.Item as Models.MenuItem;
                if (item.FoodName.RemoveDiacritics().ToLower().Contains(FilterText.RemoveDiacritics().ToLower()))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
            private void Ingredients_Filter(object sender, FilterEventArgs e)
            {
                if (string.IsNullOrEmpty(IngreFilterText))
                {
                    e.Accepted = true;
                    return;
                }

                Models.Kho item = e.Item as Models.Kho;
                if (item.TenSanPham.RemoveDiacritics().ToLower().Contains(IngreFilterText.RemoveDiacritics().ToLower()))
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
                _menuitems = await MenuDP.Flag.ConvertToCollection();
            }
        private bool IsAnyIngredientSelected()
        {
            foreach (Kho ingre in IngredientCollection)
            {
                if (ingre.DuocChon == true) return true;
            }
            return false;
        }
        private bool IsIngredientsValid()
        {
            foreach (Kho ingre in IngredientCollection)
            {
                if (ingre.DuocChon == true)
                {
                    if (ingre.DinhLuong <= 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void RefreshIngredients()
        {
            foreach (Kho item in IngredientCollection)
            {
                item.DuocChon = false;
                item.DinhLuong = 0;
            }
        }
        #endregion
    }
}
