using LichSuBan.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Billiard4Life.View
{
    /// <summary>
    /// Interaction logic for LichSuBan.xaml
    /// </summary>
    public partial class LichSuBan : UserControl
    {
        public LichSuBan()
        {
            InitializeComponent();
           
        }
    
        
 
           
       
      

        private void Filterbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
          
            if (cbbmonth != null && timepicker != null)
            {
                switch (cbb.SelectedIndex)
                {
                    case 0:
                        {
                            cbbmonth.Visibility = System.Windows.Visibility.Collapsed;
                            timepicker.Visibility = System.Windows.Visibility.Collapsed;
                            break;
                        }
                    case 1:
                        {
                            cbbmonth.Visibility = System.Windows.Visibility.Collapsed;
                            timepicker.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                    case 2:
                        {
                            cbbmonth.Visibility = System.Windows.Visibility.Visible;
                            timepicker.Visibility = System.Windows.Visibility.Collapsed;
                            break;
                        }
                }
            }
        }
    }
}
