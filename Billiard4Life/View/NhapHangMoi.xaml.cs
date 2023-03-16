using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Billiard4Life.View
{
    /// <summary>
    /// Interaction logic for NhapHangMoi.xaml
    /// </summary>
    public partial class NhapHangMoi : Window
    {
        public NhapHangMoi()
        {
            InitializeComponent();

            DataContext = new ViewModel.NhapHangMoiViewModel();
        }
        public NhapHangMoi(Billiard4Life.Models.Kho kho)
        {
            InitializeComponent();

            DataContext = new ViewModel.NhapHangMoiViewModel(kho);
        }
    }
}
