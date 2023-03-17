using Billiard4Life.DataProvider;
using Billiard4Life.Models;
using Billiard4Life.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KhuyenMai = Billiard4Life.Models.KhuyenMai;

namespace Billiard4Life.ViewModel
{
    public class KhuyenMaiViewModel : BaseViewModel
    {
        public KhuyenMaiViewModel()
        {
            KhuyenMais = KhuyenMaiDP.Flag.GetKhuyenMais();
            AddKhuyenMai_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                KhuyenMai_Them view = new KhuyenMai_Them();
                view.DataContext = this;
                view.ShowDialog();
            });

            EditKhuyenMai_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                KhuyenMai_Sua view = new KhuyenMai_Sua();
                view.DataContext = this;
                view.ShowDialog();
            });
        }

        #region attributes
        private ObservableCollection<KhuyenMai> khuyenMais = new ObservableCollection<KhuyenMai>();
        #endregion

        #region properties
        public ObservableCollection<KhuyenMai> KhuyenMais { get { return khuyenMais; } set { khuyenMais = value; OnPropertyChanged(); } }
        #endregion

        #region commands
        public ICommand AddKhuyenMai_Command { get; set; }
        public ICommand EditKhuyenMai_Command { get; set; }
        #endregion
    }
}
