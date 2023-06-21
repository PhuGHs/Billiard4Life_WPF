using Billiard4Life.DataProvider;
using Billiard4Life.Models;
using Billiard4Life.View;
using Diacritics.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using KhuyenMai = Billiard4Life.Models.KhuyenMai;

namespace Billiard4Life.ViewModel
{
    public class KhuyenMaiViewModel : BaseViewModel
    {
        public KhuyenMaiViewModel()
        {
            KhuyenMais = KhuyenMaiDP.Flag.GetKhuyenMais();
            KhuyenMaiDP.Flag.AutoUpdateStatusKhuyenMai(KhuyenMais);
            AddKhuyenMaiItem = new KhuyenMai();
            SelectedItem = new KhuyenMai();
            LoadTrangThai();
            khuyenmai_view = new CollectionViewSource();
            khuyenmai_view.Source = KhuyenMais;
            khuyenmai_view.Filter += KhuyenMais_Filter;
            AddKhuyenMai_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                KhuyenMai_Them view = new KhuyenMai_Them();
                view.DataContext = this;
                AddKhuyenMaiItem.MAKM = KhuyenMaiDP.Flag.AutoIDKhuyenMai();
                AddKhuyenMaiItem.TrangThai = "Đang diễn ra";
                view.ShowDialog();
            });

            EditKhuyenMai_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                KhuyenMai_Sua view = new KhuyenMai_Sua();
                view.DataContext = this;
                view.ShowDialog();
            });

            SelectAllKhuyenMai_Command = new RelayCommand<CheckBox>((p) => true, (p) =>
            {
                if(p.IsChecked == true)
                {

                }
            });

            DeleteKhuyenMai_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                if (IsAnyKhuyenMaiSelected() == false)
                {
                    MyMessageBox msb = new MyMessageBox("Hãy tích vào khuyến mãi mà bạn muốn xóa!");
                    msb.ShowDialog();
                }
                else
                {
                    string mess = String.Empty;
                    try
                    {
                        foreach (KhuyenMai item in KhuyenMais.ToList())
                        {
                            if (item.IsSelected == true)
                            {
                                KhuyenMais.Remove(item);
                                KhuyenMaiDP.Flag.DeleteKhuyenmai(item.MAKM);
                            }
                        }
                        mess = "Xoá thành công";
                    }
                    catch (Exception ex)
                    {
                        mess = ex.Message;
                    }
                    finally
                    {
                        MyMessageBox msb = new MyMessageBox(mess);
                        msb.Show();
                    }
                }    
            });

            AddKhuyenMaiItem_Command = new RelayCommand<Grid>((p) =>
            {
                if(AddKhuyenMaiItem.isNullOrEmpty())
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                string mess = "";
                try
                {
                    if (KhuyenMaiIsListed())
                    {
                        mess = "Thông tin khuyến mãi này đã bị trùng (Mã khuyến mãi)";
                        return;
                    }
                    if(!NgayBDBeHonNgayKT(AddKhuyenMaiItem.NgayBatDau, AddKhuyenMaiItem.NGayKetThuc))
                    {
                        mess = "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu";
                        return;
                    }
                    if(!percentageValidation(AddKhuyenMaiItem.GiamGia))
                    {
                        mess = "Giá trị giảm phải lớn hơn 0 và bé hơn hoặc bằng 100";
                        return;
                    }
                    KhuyenMaiDP.Flag.AddKhuyenMai(AddKhuyenMaiItem);
                    KhuyenMais.Add(new KhuyenMai(AddKhuyenMaiItem.MAKM, AddKhuyenMaiItem.TenKM, AddKhuyenMaiItem.MucApDung, AddKhuyenMaiItem.TrangThai, AddKhuyenMaiItem.NgayBatDau, AddKhuyenMaiItem.NGayKetThuc, AddKhuyenMaiItem.MoTa, AddKhuyenMaiItem.GiamGia));
                    AddKhuyenMaiItem.clear();
                    mess = "Thêm thành công";
                } catch (Exception ex)
                {
                    mess = ex.Message;
                } finally
                {
                    MyMessageBox msb = new MyMessageBox(mess);
                    msb.Show();
                }
            });

            EditKhuyenMaiItem_Command = new RelayCommand<object>((p) =>
            {
                if (SelectedItem.isNullOrEmpty())
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                string mess = String.Empty;
                try
                {
                    KhuyenMaiDP.Flag.UpdateKhuyenMai(SelectedItem);
                    if (!NgayBDBeHonNgayKT(SelectedItem.NgayBatDau, SelectedItem.NGayKetThuc))
                    {
                        mess = "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu";
                        return;
                    }
                    if (!percentageValidation(SelectedItem.GiamGia))
                    {
                        mess = "Giá trị giảm phải lớn hơn 0 và bé hơn hoặc bằng 100";
                        return;
                    }
                    mess = "Sửa thành công";
                    OnPropertyChanged();
                } catch(Exception ex)
                {
                    mess = ex.Message;
                } finally
                {
                    MyMessageBox msb = new MyMessageBox(mess);
                    msb.Show();
                }
            });
        }

        #region attributes
        private ObservableCollection<KhuyenMai> khuyenMais = new ObservableCollection<KhuyenMai>();
        private ObservableCollection<string> trangthais = new ObservableCollection<string>();
        private KhuyenMai selectedKhuyenMaiItem;
        private KhuyenMai addKhuyenMaiItem;
        private KhuyenMai selectedItem;
        private bool _isAllSelected;
        private CollectionViewSource khuyenmai_view;
        private string _searchText;
        #endregion

        #region properties
        public ObservableCollection<KhuyenMai> KhuyenMais { get { return khuyenMais; } set { khuyenMais = value; OnPropertyChanged(); } }
        public KhuyenMai SelectedKhuyenMai { get { return selectedKhuyenMaiItem; } set { selectedKhuyenMaiItem = value; OnPropertyChanged(); } }
        public KhuyenMai AddKhuyenMaiItem { get { return addKhuyenMaiItem; } set { addKhuyenMaiItem = value; OnPropertyChanged(); } }
        public KhuyenMai SelectedItem { get { return selectedItem; } set { selectedItem = value; OnPropertyChanged(); } }
        public ObservableCollection<string> TrangThais { get { return trangthais; } set { trangthais = value; OnPropertyChanged(); } }
        public ICollectionView KhuyenMaiView { get { return khuyenmai_view.View; } }
        public string SearchText { get { return _searchText; } set { _searchText = value; this.khuyenmai_view.View.Refresh(); OnPropertyChanged(); } }
        public bool IsAllSelected { 
            get { 
                return _isAllSelected; 
            } 
            set { 
                _isAllSelected = value; 
                foreach(var item in KhuyenMais)
                {
                    item.IsSelected = value;
                }
                OnPropertyChanged(); 
            } 
        }
        #endregion

        #region commands
        public ICommand AddKhuyenMai_Command { get; set; }
        public ICommand EditKhuyenMai_Command { get; set; }
        public ICommand SelectAllKhuyenMai_Command { get; set; }
        public ICommand AddKhuyenMaiItem_Command { get; set; }
        public ICommand EditKhuyenMaiItem_Command { get; set; }
        public ICommand DeleteKhuyenMai_Command { get; set; }
        #endregion

        #region methods
        private bool KhuyenMaiIsListed()
        {
            foreach(var item in KhuyenMais)
            {
                if(item.MAKM == AddKhuyenMaiItem.MAKM)
                {
                    return true;
                }
            }
            return false;
        }
        private void LoadTrangThai()
        {
            TrangThais.Add("Hết hạn");
            TrangThais.Add("Đang diễn ra");
            TrangThais.Add("Chưa đến kỳ hạn");
        }
        private bool NgayBDBeHonNgayKT(string ngaybd, string ngaykt)
        {
            DateTime startdate = Convert.ToDateTime(ngaybd);
            DateTime enddate = Convert.ToDateTime(ngaykt);

            if(DateTime.Compare(startdate, enddate) < 0 || DateTime.Compare(startdate, enddate) == 0)
            {
                return true;
            }
            return false;
        }
        private bool percentageValidation(int x)
        {
            return x > 0 && x < 100;
        }
        public void KhuyenMais_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                e.Accepted = true;
                return;
            }

            KhuyenMai ?item = e.Item as KhuyenMai;
            if (item.TenKM.RemoveDiacritics().ToLower().Contains(SearchText.RemoveDiacritics().ToLower()))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }
        private bool IsAnyKhuyenMaiSelected()
        {
            foreach (KhuyenMai item in KhuyenMais.ToList())
            {
                if (item.IsSelected == true)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
