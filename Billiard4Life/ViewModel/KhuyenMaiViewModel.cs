using Billiard4Life.DataProvider;
using Billiard4Life.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.ViewModel
{
    public class KhuyenMaiViewModel : BaseViewModel
    {
        public KhuyenMaiViewModel()
        {
            KhuyenMais = KhuyenMaiDP.Flag.GetKhuyenMais();
        }

        #region attributes
        private ObservableCollection<KhuyenMai> khuyenMais = new ObservableCollection<KhuyenMai>();
        #endregion

        #region properties
        public ObservableCollection<KhuyenMai> KhuyenMais { get { return khuyenMais; } set { khuyenMais = value; OnPropertyChanged(); } }
        #endregion
    }
}
