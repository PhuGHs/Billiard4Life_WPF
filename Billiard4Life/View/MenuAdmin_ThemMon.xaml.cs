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
    /// Interaction logic for MenuAdmin_ThemMon.xaml
    /// </summary>
    public partial class MenuAdmin_ThemMon : Window
    {
        private bool adding;
        public MenuAdmin_ThemMon(bool adding)
        {
            InitializeComponent();
            this.adding = adding;
            if (adding == false)
            {
                Title.Content = "   SỬA MÓN ĂN";
                Add_button.Content = "Sửa";
            }
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
