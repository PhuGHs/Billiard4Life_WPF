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
    /// Interaction logic for KhuyenMai_Them.xaml
    /// </summary>
    public partial class KhuyenMai_Them : Window
    {
        public KhuyenMai_Them()
        {
            InitializeComponent();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)e.AddedItems[0];
            DateTime currentDate = DateTime.Now;

            if (selectedDate < currentDate.Date)
            {
                datepickerstart.SelectedDate = currentDate.Date;
            }
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime currentDate = DateTime.Now;
            datepickerstart.SelectedDate = currentDate.Date;
        }

        private void DatePickerend_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime currentDate = DateTime.Now;
            datepickerend.SelectedDate = currentDate.Date;
        }

        private void DatePickerend_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)e.AddedItems[0];
            DateTime currentDate = DateTime.Now;

            if (selectedDate < currentDate.Date)
            {
                datepickerend.SelectedDate = currentDate.Date;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
